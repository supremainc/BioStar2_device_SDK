using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Data.SQLite;


/////////////////////////////////////////////////////////////
//
// 2021.08.02
// Examples for ServerMatching are no longer provided.
// Please refer only at code level.
//
/////////////////////////////////////////////////////////////

/*
namespace Suprema
{
    abstract class BaseTask
    {
        public DataBaseHandler dbHandler;
        public IntPtr sdkContext;
        public UInt32 deviceID;
        public UInt16 seq;
        public BS2SimpleDeviceInfo deviceInfo;

        protected BaseTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, DataBaseHandler dbHandler, BS2SimpleDeviceInfo deviceInfo)
        {
            this.sdkContext = sdkContext;
            this.deviceID = deviceID;
            this.seq = seq;
            this.dbHandler = dbHandler;
            this.deviceInfo = deviceInfo;
        }

        public abstract void execute();
    }

    abstract class VerifyUserTask : BaseTask
    {
        protected abstract BS2ErrorCode find(ref BS2UserBlob userBlob);

        protected VerifyUserTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, DataBaseHandler dbHandler, BS2SimpleDeviceInfo deviceInfo) : base(sdkContext, deviceID, seq, dbHandler, deviceInfo)
        {

        }

        public override void execute()
        {
            BS2UserBlob userBlob = Util.AllocateStructure<BS2UserBlob>();
            BS2ErrorCode handleResult = find(ref userBlob);
            Console.WriteLine("[Server] responded with a status of {0} : device[{1}] seq[{2}].", handleResult, deviceID, seq);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_VerifyUser(sdkContext, deviceID, seq, (int)handleResult, ref userBlob);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            if (handleResult == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (userBlob.cardObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.cardObjs);
                }

                if (userBlob.fingerObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.fingerObjs);
                }

                if (userBlob.faceObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.faceObjs);
                }
            }
        }
    }

    abstract class IdentifyUserTask : BaseTask
    {
        public BS2FingerprintTemplateFormatEnum templateFormatEnum;
        public BS2FingerprintSecurityEnum securityEnum;
        public byte[] templateData = new byte[BS2Environment.BS2_FINGER_TEMPLATE_SIZE];
        public UInt32 templateSize;

        protected abstract BS2ErrorCode find(ref BS2UserBlob userBlob);

        protected IdentifyUserTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, DataBaseHandler dbHandler, BS2SimpleDeviceInfo deviceInfo, BS2FingerprintTemplateFormatEnum templateFormatEnum, BS2FingerprintSecurityEnum securityEnum, byte[] templateData, UInt32 templateSize)
            : base(sdkContext, deviceID, seq, dbHandler, deviceInfo)
        {
            this.templateFormatEnum = templateFormatEnum;
            this.securityEnum = securityEnum;
            Array.Copy(templateData, this.templateData, templateSize);
            this.templateSize = templateSize;
        }

        public override void execute()
        {
            BS2UserBlob userBlob = Util.AllocateStructure<BS2UserBlob>();
            BS2ErrorCode handleResult = find(ref userBlob);
            Console.WriteLine("[Server] responded with a status of {0} : device[{1}] seq[{2}].", handleResult, deviceID, seq);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_IdentifyUser(sdkContext, deviceID, seq, (int)handleResult, ref userBlob);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            if (handleResult == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (userBlob.cardObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.cardObjs);
                }

                if (userBlob.fingerObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.fingerObjs);
                }

                if (userBlob.faceObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.faceObjs);
                }
            }
        }
    }

    class IdVerifyUserTask : VerifyUserTask
    {
        public string userID;

        public IdVerifyUserTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, DataBaseHandler dbHandler, BS2SimpleDeviceInfo deviceInfo, string userID)
            : base(sdkContext, deviceID, seq, dbHandler, deviceInfo)
        {
            this.userID = userID;
        }

        protected override BS2ErrorCode find(ref BS2UserBlob userBlob)
        {
            if (dbHandler.GetUserBlob(ref deviceInfo, userID, ref userBlob))
            {
                return BS2ErrorCode.BS_SDK_SUCCESS;
            }

            return BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_USER;
        }
    }

    class CardVerifyUserTask : VerifyUserTask
    {
        public BS2CSNCard csnCard;

        public CardVerifyUserTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, DataBaseHandler dbHandler, BS2SimpleDeviceInfo deviceInfo, BS2CSNCard csnCard)
            : base(sdkContext, deviceID, seq, dbHandler, deviceInfo)
        {
            this.csnCard = csnCard;
        }

        protected override BS2ErrorCode find(ref BS2UserBlob userBlob)
        {
            if (dbHandler.GetUserBlob(ref deviceInfo, ref csnCard, ref userBlob))
            {
                return BS2ErrorCode.BS_SDK_SUCCESS;
            }

            return BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_USER;
        }
    }

    class UFMIdentifyUserTask : IdentifyUserTask
    {
        const int pageCount = 1024;
        static private UFMatcher matcher = new UFMatcher();

        public UFMIdentifyUserTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, DataBaseHandler dbHandler, BS2SimpleDeviceInfo deviceInfo, BS2FingerprintTemplateFormatEnum templateFormatEnum, BS2FingerprintSecurityEnum securityEnum, byte[] templateData, UInt32 templateSize) :
            base(sdkContext, deviceID, seq, dbHandler, deviceInfo, templateFormatEnum, securityEnum, templateData, templateSize)
        {
            switch (securityEnum)
            {
                case BS2FingerprintSecurityEnum.NORMAL:
                    matcher.SecurityLevel = 3;
                    break;
                case BS2FingerprintSecurityEnum.HIGH:
                    matcher.SecurityLevel = 5;
                    break;
                case BS2FingerprintSecurityEnum.HIGHEST:
                    matcher.SecurityLevel = 7;
                    break;
                default:
                    matcher.SecurityLevel = 4;
                    break;
            }
            
            matcher.UseSIF = false;
            matcher.nTemplateType = 2001 + (int)templateFormatEnum;
        }

        protected override BS2ErrorCode find(ref BS2UserBlob userBlob)
        {
            UFM_STATUS status = matcher.IdentifyInit(templateData, (int)templateSize);
            if (status != UFM_STATUS.OK)
            {
                Console.WriteLine("Can't initialize matcher. error code :{0}", status);
            }
            else
            {                
                int offset = 0;
                bool IdentifySucceed;
                byte[] curTemplate = new byte[BS2Environment.BS2_FINGER_TEMPLATE_SIZE];
                List<KeyValuePair<string, BS2Fingerprint>> fingerprintList = new List<KeyValuePair<string, BS2Fingerprint>>();

                while (true)
                {
                    if (!dbHandler.GetFingerprintList(templateFormatEnum, pageCount, offset, ref fingerprintList))
                    {
                        break;
                    }

                    foreach (KeyValuePair<string, BS2Fingerprint> item in fingerprintList)
                    {
                        for (int templateIndex = 0; templateIndex < BS2Environment.BS2_TEMPLATE_PER_FINGER; ++templateIndex)
                        {
                            Array.Copy(item.Value.data, templateIndex * BS2Environment.BS2_FINGER_TEMPLATE_SIZE, curTemplate, 0, BS2Environment.BS2_FINGER_TEMPLATE_SIZE);

                            status = matcher.IdentifyNext(curTemplate, BS2Environment.BS2_FINGER_TEMPLATE_SIZE, out IdentifySucceed);
                            if (status == UFM_STATUS.OK && IdentifySucceed)
                            {
                                if (dbHandler.GetUserBlob(ref deviceInfo, item.Key, ref userBlob))
                                {
                                    if ((BS2FingerprintFlagEnum)item.Value.flag == BS2FingerprintFlagEnum.DURESS)
                                    {
                                        return BS2ErrorCode.BS_SDK_DURESS_SUCCESS;
                                    }

                                    return BS2ErrorCode.BS_SDK_SUCCESS;
                                }
                            }
                           
                        }
                        
                    }

                    if (fingerprintList.Count < pageCount)
                    {
                        break;
                    }
                    else
                    {
                        offset += pageCount;
                    }
                }
            }

            return BS2ErrorCode.BS_SDK_ERROR_CANNOT_FIND_USER;
        }
    }

    class ServerMatchingTask : IDisposable
    {
        private bool running;
        private Thread thread;
        private readonly object locker = new object();
        private EventWaitHandle eventWaitHandle = new AutoResetEvent(false);
        private Queue<BaseTask> taskQueue = new Queue<BaseTask>();

        public ServerMatchingTask()
        {
            thread = new Thread(run);            
        }

        public void enqueue(BaseTask task)
        {
            lock (locker)
            {
                taskQueue.Enqueue(task);
            }

            eventWaitHandle.Set();
        }

        public void Dispose()
        {
            stop();
        }

        public void start()
        {
            if (!running)
            {
                running = true;
                thread.Start();
            }
        }

        public void stop()
        {
            if (running)
            {
                running = false;
                lock (locker)
                {
                    taskQueue.Clear();
                }
                eventWaitHandle.Set();
                thread.Join();
                eventWaitHandle.Close();
            }
        }

        public void run()
        {
            while (running)
            {
                BaseTask task = null;

                lock (locker)
                {
                    if (taskQueue.Count > 0)
                    {
                        task = taskQueue.Dequeue();
                    }
                }

                if (task != null)
                {
                    task.execute();
                }
                else
                {
                    eventWaitHandle.WaitOne();
                }
            }
        }
    }

    class DataBaseHandler : IDisposable
    {
        SQLiteConnection connection;

        public DataBaseHandler()
        {
            connection = openDataBase();
        }

        public void Dispose()
        {
            connection.Close();
        }

        SQLiteConnection openDataBase()
        {
            string dbPath = "Data Source=user.db;foreign keys=true;";

            SQLiteConnection connection = new SQLiteConnection(dbPath);
            connection.Open();

            SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS User(id CHAR(32) NOT NULL, formatVersion INTEGER, flag INTEGER, version INTEGER, authGroupID INTEGER, faceChecksum INTEGER, PRIMARY KEY(id))", connection);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2UserSetting(userID CHAR(32), startTime INTEGER, endTime INTEGER, fingerAuthMode INTEGER, cardAuthMode INTEGER, idAuthMode INTEGER, securityLevel INTEGER, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2UserName(userID CHAR(32), userName CHAR(192), FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2PinCode(userID CHAR(32), hash BLOB, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2CSNCard(userID CHAR(32), type INTEGER, size INTEGER, data BLOB UNIQUE, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2Fingerprint(userID CHAR(32), fingerIndex INTEGER, flag INTEGER, data BLOB UNIQUE, templateFormat INTEGER NOT NULL, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2Face(userID CHAR(32), faceIndex INTEGER, numOfTemplate INTEGER, flag INTEGER, imageLen INTEGER, imageData BLOB UNIQUE, templateData BLOB UNIQUE, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2UserPhoto(userID CHAR(32), size INTEGER, data BLOB, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS UserAccessGroup(userID CHAR(32), accessGroupId INTEGER, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE VIEW IF NOT EXISTS BS2User AS SELECT User.id AS userID, User.formatVersion, User.flag, User.version, User.authGroupID, User.faceChecksum, (SELECT COUNT(BS2CSNCard.userID) FROM BS2CSNCard WHERE BS2CSNCARD.userID = User.id) AS numCards, (SELECT COUNT(BS2Fingerprint.userID) FROM BS2Fingerprint WHERE BS2Fingerprint.userID = User.id) as numFingers, (SELECT COUNT(BS2Face.userID) FROM BS2Face WHERE BS2Face.userID = User.id) AS numFaces FROM User";
            cmd.ExecuteNonQuery();

            return connection;
        }

        public bool RemoveUser(string userID)
        {
            byte[] targetUid = new byte[BS2Environment.BS2_USER_ID_SIZE];
            byte[] uid = Encoding.UTF8.GetBytes(userID);

            Array.Clear(targetUid, 0, BS2Environment.BS2_USER_ID_SIZE);
            Array.Copy(uid, 0, targetUid, 0, uid.Length);
            SQLiteCommand cmd = new SQLiteCommand("DELETE FROM User WHERE id = @idParam", connection);

            cmd.Parameters.AddWithValue("@idParam", targetUid);
            if (cmd.ExecuteNonQuery() < 1)
            {
                return false;
            }

            return true;
        }

        public bool GetUserList(ref BS2SimpleDeviceInfo deviceInfo, ref List<BS2User> userList)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT userID, formatVersion, flag, version, authGroupID, faceChecksum, numCards, numFingers, numFaces FROM BS2User", connection);
            SQLiteDataReader rdr = cmd.ExecuteReader();

            userList.Clear();

            while (rdr.Read())
            {
                BS2User user = Util.AllocateStructure<BS2User>();
                Array.Clear(user.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                string userID = (string)rdr[0];
                byte[] uid = Encoding.UTF8.GetBytes(userID);
                Array.Copy(uid, 0, user.userID, 0, uid.Length);
                user.formatVersion = Convert.ToByte(rdr[1]);
                user.flag = Convert.ToByte(rdr[2]);
                user.version = Convert.ToUInt16(rdr[3]);
                user.authGroupID = Convert.ToUInt32(rdr[4]);
                user.faceChecksum = Convert.ToUInt32(rdr[5]);

                if (Convert.ToBoolean(deviceInfo.cardSupported))
                {
                    user.numCards = Convert.ToByte(rdr[6]);
                }
                else
                {
                    user.numCards = 0;
                }

                if (Convert.ToBoolean(deviceInfo.fingerSupported))
                {
                    user.numFingers = Convert.ToByte(rdr[7]);
                }
                else
                {
                    user.numFingers = 0;
                }

                if (Convert.ToBoolean(deviceInfo.faceSupported))
                {
                    user.numFaces = Convert.ToByte(rdr[7]);
                }
                else
                {
                    user.numFaces = 0;
                }

                userList.Add(user);
            }

            rdr.Close();
            return true;
        }

        public bool GetUserBlob(ref BS2SimpleDeviceInfo deviceInfo, ref BS2CSNCard csnCard, ref BS2UserBlob userBlob)
        {
            string userID = "";
            SQLiteCommand cmd = new SQLiteCommand("SELECT userID FROM BS2CSNCard WHERE type = @typeParam AND size = @sizeParam AND data = @dataParam", connection);
            cmd.Parameters.AddWithValue("@typeParam", csnCard.type);
            cmd.Parameters.AddWithValue("@sizeParam", csnCard.size);
            cmd.Parameters.AddWithValue("@dataParam", csnCard.data);

            SQLiteDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                userID = (string)rdr[0];
            }
            rdr.Close();

            return GetUserBlob(ref deviceInfo, userID, ref userBlob);
        }

        public bool GetUserBlob(ref BS2SimpleDeviceInfo deviceInfo, string userID, ref BS2UserBlob userBlob)
        {
            if (userID.Length > 0)
            {
                BS2User targetUser = Util.AllocateStructure<BS2User>();

                Array.Clear(targetUser.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                byte[] uid = Encoding.UTF8.GetBytes(userID);
                Array.Copy(uid, 0, targetUser.userID, 0, uid.Length);

                SQLiteCommand cmd = new SQLiteCommand("SELECT userID, formatVersion, flag, version, authGroupID, faceChecksum, numCards, numFingers, numFaces FROM BS2User WHERE userID = @userIDParam", connection);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);
                SQLiteDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    targetUser.formatVersion = Convert.ToByte(rdr[1]);
                    targetUser.flag = Convert.ToByte(rdr[2]);
                    targetUser.version = Convert.ToUInt16(rdr[3]);
                    targetUser.authGroupID = Convert.ToUInt32(rdr[4]);
                    targetUser.faceChecksum = Convert.ToUInt32(rdr[5]);

                    if (Convert.ToBoolean(deviceInfo.cardSupported))
                    {
                        targetUser.numCards = Convert.ToByte(rdr[6]);
                    }
                    else
                    {
                        targetUser.numCards = 0;
                    }

                    if (Convert.ToBoolean(deviceInfo.fingerSupported))
                    {
                        targetUser.numFingers = Convert.ToByte(rdr[7]);
                    }
                    else
                    {
                        targetUser.numFingers = 0;
                    }

                    if (Convert.ToBoolean(deviceInfo.faceSupported))
                    {
                        targetUser.numFaces = Convert.ToByte(rdr[7]);
                    }
                    else
                    {
                        targetUser.numFaces = 0;
                    }

                    return GetUserBlob(ref deviceInfo, ref targetUser, ref userBlob);
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool GetUserBlob(ref BS2SimpleDeviceInfo deviceInfo, ref BS2User targetUser, ref BS2UserBlob userBlob)
        {
            userBlob.user = targetUser;

            SQLiteCommand cmd = new SQLiteCommand("SELECT startTime, endTime, fingerAuthMode, cardAuthMode, idAuthMode, securityLevel FROM BS2UserSetting WHERE userID = @userIDParam", connection);
            cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                userBlob.setting.startTime = Convert.ToUInt32(rdr[0]);
                userBlob.setting.endTime = Convert.ToUInt32(rdr[1]);
                userBlob.setting.fingerAuthMode = Convert.ToByte(rdr[2]);
                userBlob.setting.cardAuthMode = Convert.ToByte(rdr[3]);
                userBlob.setting.idAuthMode = Convert.ToByte(rdr[4]);
                userBlob.setting.securityLevel = Convert.ToByte(rdr[5]);
            }
            rdr.Close();

            Array.Clear(userBlob.name, 0, BS2Environment.BS2_USER_NAME_LEN);
            if (Convert.ToBoolean(deviceInfo.userNameSupported))
            {
                cmd.CommandText = "SELECT userName from BS2UserName WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    byte[] userName = Encoding.UTF8.GetBytes((string)rdr[0]);
                    Array.Copy(userName, 0, userBlob.name, 0, userName.Length);
                }
                rdr.Close();
            }

            userBlob.photo.size = 0;
            Array.Clear(userBlob.photo.data, 0, BS2Environment.BS2_USER_PHOTO_SIZE);
            if (Convert.ToBoolean(deviceInfo.userPhotoSupported))
            {
                cmd.CommandText = "SELECT size, data from BS2UserPhoto WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    UInt32 photoSize = Convert.ToUInt32(rdr[0]);
                    byte[] photoData = (byte[])rdr[1];

                    userBlob.photo.size = photoSize;
                    if (photoSize > 0)
                    {
                        Array.Copy(photoData, 0, userBlob.photo.data, 0, photoSize);
                    }
                }
                rdr.Close();
            }

            Array.Clear(userBlob.pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
            if (Convert.ToBoolean(deviceInfo.pinSupported))
            {
                cmd.CommandText = "SELECT hash from BS2PinCode WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    byte[] pinCode = (byte[])rdr[0];
                    Array.Copy(pinCode, 0, userBlob.pin, 0, pinCode.Length);
                }
                rdr.Close();
            }

            if (targetUser.numCards > 0)
            {
                cmd.CommandText = "SELECT type, size, data from BS2CSNCard WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);

                rdr = cmd.ExecuteReader();
                userBlob.cardObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2CSNCard)) * targetUser.numCards);
                IntPtr curCardObjs = userBlob.cardObjs;

                while (rdr.Read())
                {
                    byte cardType = Convert.ToByte(rdr[0]);
                    byte cardSize = Convert.ToByte(rdr[1]);
                    byte[] cardData = (byte[])rdr[2];

                    Marshal.WriteByte(curCardObjs, cardType);
                    curCardObjs += 1;
                    Marshal.WriteByte(curCardObjs, cardSize);
                    curCardObjs += 1;
                    Marshal.Copy(cardData, 0, curCardObjs, BS2Environment.BS2_CARD_DATA_SIZE);
                    curCardObjs += BS2Environment.BS2_CARD_DATA_SIZE;
                }
                rdr.Close();
            }
            else
            {
                userBlob.cardObjs = IntPtr.Zero;
            }

            if (targetUser.numFingers > 0)
            {
                cmd.CommandText = "SELECT fingerIndex, flag, data FROM BS2Fingerprint WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);

                rdr = cmd.ExecuteReader();
                userBlob.fingerObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2Fingerprint)) * targetUser.numFingers);
                IntPtr curFingerObjs = userBlob.fingerObjs;

                while (rdr.Read())
                {
                    byte fingerIndex = Convert.ToByte(rdr[0]);
                    byte fingerFlag = Convert.ToByte(rdr[1]);
                    byte[] templateData = (byte[])rdr[2];

                    Marshal.WriteByte(curFingerObjs, fingerIndex);
                    curFingerObjs += 1;
                    Marshal.WriteByte(curFingerObjs, fingerFlag);
                    curFingerObjs += 3;
                    Marshal.Copy(templateData, 0, curFingerObjs, BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                    curFingerObjs += BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE;
                }
                rdr.Close();
            }
            else
            {
                userBlob.fingerObjs = IntPtr.Zero;
            }

            if (targetUser.numFaces > 0)
            {
                cmd.CommandText = "SELECT faceIndex, templateIndex, data FROM BS2Face WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);

                rdr = cmd.ExecuteReader();
                userBlob.faceObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2Face)) * targetUser.numFaces);
                IntPtr curFaceObjs = userBlob.faceObjs;

                while (rdr.Read())
                {
                    byte faceIndex = Convert.ToByte(rdr[0]);
                    byte templateIndex = Convert.ToByte(rdr[1]);
                    byte[] templateData = (byte[])rdr[2];

                    Marshal.WriteByte(curFaceObjs, faceIndex);
                    curFaceObjs += 1;
                    Marshal.WriteByte(curFaceObjs, templateIndex);
                    curFaceObjs += 3;
                    Marshal.Copy(templateData, 0, curFaceObjs, BS2Environment.BS2_TEMPLATE_PER_FACE * BS2Environment.BS2_FACE_TEMPLATE_LENGTH);
                    curFaceObjs += BS2Environment.BS2_TEMPLATE_PER_FACE * BS2Environment.BS2_FACE_TEMPLATE_LENGTH;
                }
                rdr.Close();
            }
            else
            {
                userBlob.faceObjs = IntPtr.Zero;
            }

            Array.Clear(userBlob.accessGroupId, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER);
            int access_group_count = 0;
            cmd.CommandText = "SELECT accessGroupId from UserAccessGroup WHERE userID = @userIDParam";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);

            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                userBlob.accessGroupId[access_group_count++] = Convert.ToUInt32(rdr[0]);
                if (access_group_count >= BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER)
                {
                    break;
                }
            }
            rdr.Close();

            return true;
        }

        public bool GetFingerprintList(BS2FingerprintTemplateFormatEnum templateFormat, int limit, int offset, ref List<KeyValuePair<string, BS2Fingerprint>> fingerprintList)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT userID, fingerIndex, flag, data FROM BS2Fingerprint WHERE templateFormat = @templateFormatParam LIMIT @limitParam OFFSET @offsetParam", connection);
            cmd.Parameters.AddWithValue("@templateFormatParam", templateFormat);
            cmd.Parameters.AddWithValue("@limitParam", limit);
            cmd.Parameters.AddWithValue("@offsetParam", offset);

            fingerprintList.Clear();

            BS2Fingerprint fingerprint = Util.AllocateStructure<BS2Fingerprint>();
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                string userID = (string)rdr[0];
                fingerprint.index = Convert.ToByte(rdr[1]);
                fingerprint.flag = Convert.ToByte(rdr[2]);
                fingerprint.data = (byte[])rdr[3];

                fingerprintList.Add(new KeyValuePair<string, BS2Fingerprint>(userID, fingerprint));
            }
            rdr.Close();

            if (fingerprintList.Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool AddUserBlob(ref BS2SimpleDeviceInfo deviceInfo, ref BS2UserBlob userBlob, BS2FingerprintTemplateFormatEnum templateFormat)
        {
            SQLiteTransaction transaction = connection.BeginTransaction();

            SQLiteCommand cmd = new SQLiteCommand("INSERT INTO User (Id, formatVersion, flag, version, authGroupID, faceChecksum) VALUES (@userIDParam, @formatVersionParam, @flagParam, @versionParam, @authGroupIDParam, @faceChecksumParam)", connection);
            cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
            cmd.Parameters.AddWithValue("@formatVersionParam", userBlob.user.formatVersion);
            cmd.Parameters.AddWithValue("@flagParam", userBlob.user.flag);
            cmd.Parameters.AddWithValue("@versionParam", userBlob.user.version);
            cmd.Parameters.AddWithValue("@authGroupIDParam", userBlob.user.authGroupID);
            cmd.Parameters.AddWithValue("@faceChecksumParam", userBlob.user.faceChecksum);

            if (cmd.ExecuteNonQuery() < 1)
            {
                transaction.Rollback();
                return false;
            }

            cmd.CommandText = "INSERT INTO BS2UserSetting (userID, startTime, endTime, fingerAuthMode, cardAuthMode, idAuthMode, securityLevel) VALUES (@userIDParam, @startTimeParam, @endTimeParam, @fingerAuthModeParam, @cardAuthModeParam, @idAuthModeParam, @securityLevelParam)";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
            cmd.Parameters.AddWithValue("@startTimeParam", userBlob.setting.startTime);
            cmd.Parameters.AddWithValue("@endTimeParam", userBlob.setting.endTime);
            cmd.Parameters.AddWithValue("@fingerAuthModeParam", userBlob.setting.fingerAuthMode);
            cmd.Parameters.AddWithValue("@cardAuthModeParam", userBlob.setting.cardAuthMode);
            cmd.Parameters.AddWithValue("@idAuthModeParam", userBlob.setting.idAuthMode);
            cmd.Parameters.AddWithValue("@securityLevelParam", userBlob.setting.securityLevel);

            if (cmd.ExecuteNonQuery() < 1)
            {
                transaction.Rollback();
                return false;
            }

            if (Convert.ToBoolean(deviceInfo.userNameSupported))
            {
                cmd.CommandText = "INSERT INTO BS2UserName (userID, userName) VALUES (@userIDParam, @userNameParam)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                cmd.Parameters.AddWithValue("@userNameParam", userBlob.name);

                if (cmd.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            if (Convert.ToBoolean(deviceInfo.pinSupported))
            {
                cmd.CommandText = "INSERT INTO BS2PinCode (userID, hash) VALUES (@userIDParam, @hashParam)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                cmd.Parameters.AddWithValue("@hashParam", userBlob.pin);

                if (cmd.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            if (Convert.ToBoolean(deviceInfo.cardSupported) && userBlob.user.numCards > 0)
            {
                int structSize = Marshal.SizeOf(typeof(BS2CSNCard));
                Type type = typeof(BS2CSNCard);
                IntPtr curObjs = userBlob.cardObjs;
                cmd.CommandText = "INSERT INTO BS2CSNCard (userID, type, size, data) VALUES (@userIDParam, @typeParam, @sizeParam, @dataParam)";

                for (byte idx = 0; idx < userBlob.user.numCards; ++idx)
                {
                    BS2CSNCard csnCard = (BS2CSNCard)Marshal.PtrToStructure(curObjs, type);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                    cmd.Parameters.AddWithValue("@typeParam", csnCard.type);
                    cmd.Parameters.AddWithValue("@sizeParam", csnCard.size);
                    cmd.Parameters.AddWithValue("@dataParam", csnCard.data);

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    curObjs += structSize;
                }
            }

            if (Convert.ToBoolean(deviceInfo.fingerSupported) && userBlob.user.numFingers > 0)
            {
                int structSize = Marshal.SizeOf(typeof(BS2Fingerprint));
                Type type = typeof(BS2Fingerprint);
                IntPtr curObjs = userBlob.fingerObjs;
                cmd.CommandText = "INSERT INTO BS2Fingerprint (userID, fingerIndex, flag, data, templateFormat) VALUES (@userIDParam, @fingerIndexParam, @flagParam, @dataParam, @templateFormatParam)";

                for (byte idx = 0; idx < userBlob.user.numFingers; ++idx)
                {
                    BS2Fingerprint finger = (BS2Fingerprint)Marshal.PtrToStructure(curObjs, type);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                    cmd.Parameters.AddWithValue("@fingerIndexParam", finger.index);
                    cmd.Parameters.AddWithValue("@flagParam", finger.flag);
                    cmd.Parameters.AddWithValue("@dataParam", finger.data);
                    cmd.Parameters.AddWithValue("@templateFormatParam", templateFormat);

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    curObjs += structSize;
                }
            }

            if (Convert.ToBoolean(deviceInfo.faceSupported) && userBlob.user.numFaces > 0)
            {
                int structSize = Marshal.SizeOf(typeof(BS2Face));
                Type type = typeof(BS2Face);
                IntPtr curObjs = userBlob.faceObjs;
                cmd.CommandText = "INSERT INTO BS2Face (userID, faceIndex, numOfTemplate, flag, imageLen, imageData, templateData) VALUES (@userIDParam, @faceIndexParam, @numOfTemplateParam, @flagParam, @imageLenParam, @imageDataParam, @templatedataParam)";

                for (byte idx = 0; idx < userBlob.user.numFaces; ++idx)
                {
                    BS2Face face = (BS2Face)Marshal.PtrToStructure(curObjs, type);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                    cmd.Parameters.AddWithValue("@faceIndexParam", face.faceIndex);
                    cmd.Parameters.AddWithValue("@numOfTemplateParam", face.numOfTemplate);
                    cmd.Parameters.AddWithValue("@flagParam", face.flag);
                    cmd.Parameters.AddWithValue("@imageLenParam", face.imageLen);
                    cmd.Parameters.AddWithValue("@imageDataParam", face.imageData);
                    cmd.Parameters.AddWithValue("@templatedataParam", face.templateData);

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    curObjs += structSize;
                }
            }

            cmd.CommandText = "INSERT INTO UserAccessGroup (userID, accessGroupId) VALUES (@userIDParam, @accessGroupIdParam)";
            for (int idx = 0; idx < BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER; ++idx)
            {
                if (userBlob.accessGroupId[idx] != 0)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                    cmd.Parameters.AddWithValue("@accessGroupIdParam", userBlob.accessGroupId[idx]);

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                else
                {
                    break;
                }
            }

            transaction.Commit();
            return true;
        }
    }

    public class ServerMatchingControl : FunctionModule
    {
        private API.OnReadyToScan cbCardOnReadyToScan = null;
        private API.OnReadyToScan cbFingerOnReadyToScan = null;
        private API.OnVerifyUser cbOnVerifyUser = null;
        private API.OnIdentifyUser cbOnIdentifyUser = null;

        private DataBaseHandler dbHandler = new DataBaseHandler();
        private ServerMatchingTask matchingTask;
        private IntPtr sdkContext;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Show users in the system", showUserList));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Enroll a user in the system", enrolUser));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Removing a user from the system", removeUser));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Start server matching", serverMatching));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Show device list", ShowConnectionDeviceList));

            return functionList;
        }

        public void ShowConnectionDeviceList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            uint numDevice = 0;
            IntPtr deviceObjList = IntPtr.Zero;
            API.BS2_GetDevices(sdkContext, out deviceObjList, out numDevice);

            Console.WriteLine("Number of connected devices: " + numDevice);
            if (numDevice > 0)
            {
                BS2SimpleDeviceInfo deviceInfo;

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                for (UInt32 idx = 0; idx < numDevice; ++idx)
                {
                    deviceID = Convert.ToUInt32(Marshal.ReadInt32(deviceObjList, (int)idx * sizeof(UInt32)));
                    Console.WriteLine("[{0:000}] ==> ID[{1, 10}]",
                            idx,
                            deviceID);
                }
                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("Number of connected devices: " + numDevice);
            }

            API.BS2_ReleaseObject(deviceObjList);
        }

        public void showUserList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<BS2User> userList = new List<BS2User>();
            if (dbHandler.GetUserList(ref deviceInfo, ref userList))
            {
                if (userList.Count > 0)
                {
                    foreach (BS2User user in userList)
                    {
                        print(sdkContext, user);
                    }
                }
                else
                {
                    Console.WriteLine("There is no user.");
                }
            }
            else
            {
                Console.WriteLine("An error occurred while attempting to retrieve user list.");
            }
        }

        public void enrolUser(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            BS2FingerprintTemplateFormatEnum templateFormat = BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA;
            SortedSet<BS2CardAuthModeEnum> privateCardAuthMode = new SortedSet<BS2CardAuthModeEnum>();
            SortedSet<BS2FingerAuthModeEnum> privateFingerAuthMode = new SortedSet<BS2FingerAuthModeEnum>();
            SortedSet<BS2IDAuthModeEnum> privateIDAuthMode = new SortedSet<BS2IDAuthModeEnum>();

            bool cardSupported = Convert.ToBoolean(deviceInfo.cardSupported);
            bool fingerSupported = Convert.ToBoolean(deviceInfo.fingerSupported);
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            bool qrSupported = Convert.ToBoolean(deviceInfoEx.supported | (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_QR);

            bool fingerScanSupported = Convert.ToBoolean(deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN);
            bool faceScanSupported = Convert.ToBoolean(deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN);
            bool faceExScanSupported = Convert.ToBoolean(deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN);

            privateIDAuthMode.Add(BS2IDAuthModeEnum.PROHIBITED);

            if (cardSupported)
            {
                privateCardAuthMode.Add(BS2CardAuthModeEnum.PROHIBITED);
                privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_ONLY);

                if (pinSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_PIN);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_PIN);

                    if (fingerScanSupported)
                    {
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN);
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN);

                        privateFingerAuthMode.Add(BS2FingerAuthModeEnum.BIOMETRIC_PIN);

                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN);
                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_PIN);
                    }
                }

                if (fingerScanSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC);

                    privateFingerAuthMode.Add(BS2FingerAuthModeEnum.BIOMETRIC_ONLY);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC);
                }
            }
            else if (fingerScanSupported)
            {
                if (pinSupported)
                {
                    privateFingerAuthMode.Add(BS2FingerAuthModeEnum.BIOMETRIC_PIN);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN);
                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_PIN);
                }

                privateFingerAuthMode.Add(BS2FingerAuthModeEnum.BIOMETRIC_ONLY);

                privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC);
            }
            else if (pinSupported)
            {
                privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_PIN);
            }

            BS2UserBlob userBlob = Util.AllocateStructure<BS2UserBlob>();
            userBlob.user.version = 0;
            userBlob.user.formatVersion = 0;
            userBlob.user.faceChecksum = 0;
            userBlob.user.authGroupID = 0;
            userBlob.user.numCards = 0;
            userBlob.user.numFingers = 0;
            userBlob.user.numFaces = 0;
            userBlob.user.flag = 0;

            userBlob.cardObjs = IntPtr.Zero;
            userBlob.fingerObjs = IntPtr.Zero;
            userBlob.faceObjs = IntPtr.Zero;

            Console.WriteLine("Enter the ID for the User which you want to enroll");
            Console.Write(">>>> ");
            string userID = Console.ReadLine();
            if (userID.Length == 0)
            {
                Console.WriteLine("The user id can not be empty.");
                return;
            }
            else if (userID.Length > BS2Environment.BS2_USER_ID_SIZE)
            {
                Console.WriteLine("The user id should less than {0} words.", BS2Environment.BS2_USER_ID_SIZE);
                return;
            }
            else
            {
                //TODO Alphabet user id is not implemented yet.
                UInt32 uid;
                if (!UInt32.TryParse(userID, out uid))
                {
                    Console.WriteLine("The user id should be a numeric.");
                    return;
                }

                byte[] userIDArray = Encoding.UTF8.GetBytes(userID);
                Array.Clear(userBlob.user.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(userIDArray, userBlob.user.userID, userIDArray.Length);
            }

            Console.WriteLine("When is this user valid from? [default(Today), yyyy-MM-dd HH:mm:ss]");
            Console.Write(">>>> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out userBlob.setting.startTime))
            {
                return;
            }

            Console.WriteLine("When is this user valid to? [default(Today), yyyy-MM-dd HH:mm:ss]");
            Console.Write(">>>> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out userBlob.setting.endTime))
            {
                return;
            }

            if (cardSupported)
            {
                Console.WriteLine("Do you want to set the private card auth mode for this user? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    Console.Write("Enter the private card auth mode. [");
                    foreach (BS2CardAuthModeEnum cardAuthModeEnum in privateCardAuthMode)
                    {
                        if (cardAuthModeEnum == BS2CardAuthModeEnum.CARD_ONLY)
                        {
                            Console.Write("{0}: {1}(default) ", (byte)cardAuthModeEnum, cardAuthModeEnum);
                        }
                        else
                        {
                            Console.Write("{0}: {1} ", (byte)cardAuthModeEnum, cardAuthModeEnum);
                        }
                    }
                    Console.WriteLine("]");
                    Console.Write(">>>> ");
                    userBlob.setting.cardAuthMode = Util.GetInput((byte)BS2CardAuthModeEnum.CARD_ONLY);
                }
                else
                {
                    userBlob.setting.cardAuthMode = (byte)BS2CardAuthModeEnum.NONE;
                }
            }

            if (fingerScanSupported)
            {
                Console.WriteLine("Enter the security level for this user: [{0}: {1}, {2}: {3}, {4}: {5}(default), {6}: {7}, {8}: {9}]",
                                (byte)BS2UserSecurityLevelEnum.LOWER,
                                BS2UserSecurityLevelEnum.LOWER,
                                (byte)BS2UserSecurityLevelEnum.LOW,
                                BS2UserSecurityLevelEnum.LOW,
                                (byte)BS2UserSecurityLevelEnum.NORMAL,
                                BS2UserSecurityLevelEnum.NORMAL,
                                (byte)BS2UserSecurityLevelEnum.HIGH,
                                BS2UserSecurityLevelEnum.HIGH,
                                (byte)BS2UserSecurityLevelEnum.HIGHER,
                                BS2UserSecurityLevelEnum.HIGHER);
                Console.Write(">>>> ");
                userBlob.setting.securityLevel = Util.GetInput((byte)BS2UserSecurityLevelEnum.NORMAL);

                Console.WriteLine("Do you want to set the private biometric auth mode for this user? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    Console.Write("Enter the private biometric auth mode. [");
                    foreach (BS2FingerAuthModeEnum fingerAuthModeEnum in privateFingerAuthMode)
                    {
                        if (fingerAuthModeEnum == BS2FingerAuthModeEnum.BIOMETRIC_ONLY)
                        {
                            Console.Write("{0}: {1}(default) ", (byte)fingerAuthModeEnum, fingerAuthModeEnum);
                        }
                        else
                        {
                            Console.Write("{0}: {1} ", (byte)fingerAuthModeEnum, fingerAuthModeEnum);
                        }
                    }
                    Console.WriteLine("]");
                    Console.Write(">>>> ");
                    userBlob.setting.fingerAuthMode = Util.GetInput((byte)BS2FingerAuthModeEnum.BIOMETRIC_ONLY);
                }
                else
                {
                    userBlob.setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.NONE;
                }
            }

            Console.WriteLine("Do you want to set the private id auth mode for this user? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.Write("Enter the private id auth mode. [");
                foreach (BS2IDAuthModeEnum idAuthModeEnum in privateIDAuthMode)
                {
                    if (idAuthModeEnum == BS2IDAuthModeEnum.PROHIBITED)
                    {
                        Console.Write("{0}: {1}(default) ", (byte)idAuthModeEnum, idAuthModeEnum);
                    }
                    else
                    {
                        Console.Write("{0}: {1} ", (byte)idAuthModeEnum, idAuthModeEnum);
                    }
                }
                Console.WriteLine("]");
                Console.Write(">>>> ");
                userBlob.setting.idAuthMode = Util.GetInput((byte)BS2IDAuthModeEnum.PROHIBITED);
            }
            else
            {
                userBlob.setting.idAuthMode = (byte)BS2IDAuthModeEnum.PROHIBITED;
            }

            if (Convert.ToBoolean(deviceInfo.userNameSupported))
            {
                Array.Clear(userBlob.name, 0, BS2Environment.BS2_USER_NAME_LEN);
                Console.WriteLine("Do you want to set user name? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("Enter the name for this user");
                    Console.Write(">>>> ");
                    string userName = Console.ReadLine();
                    if (userName.Length == 0)
                    {
                        Console.WriteLine("[Warning] user name will be displayed as empty.");
                    }
                    else if (userName.Length > BS2Environment.BS2_USER_NAME_LEN)
                    {
                        Console.WriteLine("The user name should less than {0} words.", BS2Environment.BS2_USER_NAME_LEN);
                        return;
                    }
                    else
                    {
                        byte[] userNameArray = Encoding.UTF8.GetBytes(userName);
                        Array.Copy(userNameArray, userBlob.name, userNameArray.Length);
                    }
                }
            }

            if (pinSupported)
            {
                Array.Clear(userBlob.pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                Console.WriteLine("Do you want to set pin code? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("Enter pin code for this user");
                    Console.Write(">>>> ");
                    string pin = Console.ReadLine();
                    if (pin.Length == 0)
                    {
                        Console.WriteLine("Pin code can not be empty.");
                        return;
                    }
                    else if (pin.Length > BS2Environment.BS2_PIN_HASH_SIZE)
                    {
                        Console.WriteLine("Pin code should less than {0} words.", BS2Environment.BS2_PIN_HASH_SIZE);
                        return;
                    }
                    else
                    {
                        IntPtr ptrChar = Marshal.StringToHGlobalAnsi(pin);
                        IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                        //result = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, pin, pinCode);
                        result = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, ptrChar, pinCode);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Can't generate pin code.");
                            return;
                        }

                        Marshal.Copy(pinCode, userBlob.pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                        Marshal.FreeHGlobal(ptrChar);
                        Marshal.FreeHGlobal(pinCode);
                    }
                }
            }

            if (cardSupported)
            {
                Console.WriteLine("How many cards do you want to register? [1(default) - {0}]", BS2Environment.BS2_MAX_NUM_OF_CARD_PER_USER);
                Console.Write(">>>> ");
                userBlob.user.numCards = Util.GetInput((byte)1);

                if (userBlob.user.numCards > 0)
                {
                    int structSize = Marshal.SizeOf(typeof(BS2CSNCard));
                    BS2Card card = Util.AllocateStructure<BS2Card>();
                    userBlob.cardObjs = Marshal.AllocHGlobal(structSize * userBlob.user.numCards);
                    IntPtr curCardObjs = userBlob.cardObjs;
                    cbCardOnReadyToScan = new API.OnReadyToScan(ReadyToScanForCard);

                    for (byte idx = 0; idx < userBlob.user.numCards; )
                    {
                        Console.WriteLine("Trying to scan card.");
                        result = (BS2ErrorCode)API.BS2_ScanCard(sdkContext, deviceID, out card, cbCardOnReadyToScan);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                            return;
                        }
                        else if (Convert.ToBoolean(card.isSmartCard))
                        {
                            Console.WriteLine("CSN card is only available. Try again");
                        }
                        else
                        {
                            Marshal.Copy(card.cardUnion, 0, curCardObjs, structSize);
                            curCardObjs += structSize;
                            ++idx;
                        }
                    }

                    cbCardOnReadyToScan = null;
                }
            }

            // +2.8 XS2 QR support
            if (qrSupported)
            {
                Console.WriteLine("Would you like to register the QR code string to be used for authentication? [y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("Enter the ASCII QR code.");
                    Console.WriteLine("  [ASCII code consisting of values between 32 and 126].");
                    Console.Write(">>>> ");
                    string qrCode = Console.ReadLine();

                    IntPtr qrCodePtr = Marshal.StringToHGlobalAnsi(qrCode);
                    BS2CSNCard qrCard = Util.AllocateStructure<BS2CSNCard>();
                    result = (BS2ErrorCode)API.BS2_WriteQRCode(qrCodePtr, ref qrCard);
                    if (BS2ErrorCode.BS_SDK_SUCCESS != result)
                    {
                        Console.WriteLine("Got error({0}).", result);
                    }
                    else
                    {
                        int numOfRealloc = userBlob.user.numCards + 1;
                        int structSize = Marshal.SizeOf(typeof(BS2CSNCard));
                        byte[] tempCard = new byte[structSize * userBlob.user.numCards];

                        if (0 < userBlob.user.numCards && IntPtr.Zero != userBlob.cardObjs)
                        {
                            Marshal.Copy(userBlob.cardObjs, tempCard, 0, structSize * userBlob.user.numCards);
                            Marshal.FreeHGlobal(userBlob.cardObjs);
                        }

                        userBlob.cardObjs = Marshal.AllocHGlobal(structSize * numOfRealloc);
                        if (0 < userBlob.user.numCards)
                        {
                            Marshal.Copy(tempCard, 0, userBlob.cardObjs, structSize * userBlob.user.numCards);
                        }

                        IntPtr curCardObjs = userBlob.cardObjs + structSize * userBlob.user.numCards;

                        byte[] qrArray = Util.StructToBytes<BS2CSNCard>(ref qrCard);
                        Marshal.Copy(qrArray, 0, curCardObjs, structSize);
                        userBlob.user.numCards++;

                        Marshal.FreeHGlobal(qrCodePtr);
                    }
                }
            }
            // +V2.8 XS2 QR support

            if (fingerScanSupported)
            {
                Console.WriteLine("How many fingerprints do you want to register? [1(default) - {0}]", BS2Environment.BS2_MAX_NUM_OF_FINGER_PER_USER);
                Console.Write(">>>> ");
                userBlob.user.numFingers = Util.GetInput((byte)1);

                if (userBlob.user.numFingers > 0)
                {
                    BS2FingerprintConfig fingerprintConfig;
                    Console.WriteLine("Trying to get fingerprint config");
                    result = (BS2ErrorCode)API.BS2_GetFingerprintConfig(sdkContext, deviceID, out fingerprintConfig);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Got error({0}).", result);
                        return;
                    }
                    else
                    {
                        templateFormat = (BS2FingerprintTemplateFormatEnum)fingerprintConfig.templateFormat;
                    }

                    int structSize = Marshal.SizeOf(typeof(BS2Fingerprint));
                    BS2Fingerprint fingerprint = Util.AllocateStructure<BS2Fingerprint>();
                    userBlob.fingerObjs = Marshal.AllocHGlobal(structSize * userBlob.user.numFingers);
                    IntPtr curFingerObjs = userBlob.fingerObjs;
                    cbFingerOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFinger);

                    UInt32 outquality;
                    for (int idx = 0; idx < userBlob.user.numFingers; ++idx)
                    {
                        Console.WriteLine("Trying to get fingerprint[{0}]", idx);
                        for (UInt32 templateIndex = 0; templateIndex < BS2Environment.BS2_TEMPLATE_PER_FINGER; )
                        {
                            Console.WriteLine("Trying to scan finger.");
                            result = (BS2ErrorCode)API.BS2_ScanFingerprintEx(sdkContext, deviceID, ref fingerprint, templateIndex, (UInt32)BS2FingerprintQualityEnum.QUALITY_STANDARD, (byte)BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA, out outquality, cbFingerOnReadyToScan);
                            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                            {
                                if (result == BS2ErrorCode.BS_SDK_ERROR_EXTRACTION_LOW_QUALITY ||
                                    result == BS2ErrorCode.BS_SDK_ERROR_CAPTURE_LOW_QUALITY)
                                {
                                    Console.WriteLine("Bad fingerprint quality. Try again");
                                }
                                else
                                {
                                    Console.WriteLine("Got error({0}).", result);
                                    return;
                                }
                            }
                            else
                            {
                                ++templateIndex;
                            }
                        }

                        Console.WriteLine("Verify the fingerprints.");
                        result = (BS2ErrorCode)API.BS2_VerifyFingerprint(sdkContext, deviceID, ref fingerprint);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            if (result == BS2ErrorCode.BS_SDK_ERROR_NOT_SAME_FINGERPRINT)
                            {
                                Console.WriteLine("The fingerprint does not match. Try again");
                                --idx;
                                continue;
                            }
                            else
                            {
                                Console.WriteLine("Got error({0}).", result);
                                return;
                            }
                        }

                        fingerprint.index = (byte)idx;
                        Console.WriteLine("Is it duress finger? [0 : Normal(default), 1 : Duress]");
                        Console.Write(">>>> ");
                        fingerprint.flag = Util.GetInput((byte)BS2FingerprintFlagEnum.NORMAL);

                        Marshal.StructureToPtr(fingerprint, curFingerObjs, false);
                        curFingerObjs += structSize;
                    }

                    cbFingerOnReadyToScan = null;
                }
            }

#if false //TODO TBD 
            if (Convert.ToBoolean(deviceInfo.faceSupported))
            {
            }
#endif
            Array.Clear(userBlob.accessGroupId, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER);

            Console.WriteLine("Which access groups does this user belongs to? [ex. ID_1 ID_2 ...]");
            Console.Write(">>>> ");
            int accessGroupIdIndex = 0;
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

            foreach (string accessGroupID in accessGroupIDs)
            {
                if (accessGroupID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(accessGroupID, out item))
                    {
                        userBlob.accessGroupId[accessGroupIdIndex++] = item;
                    }
                }
            }

            Console.WriteLine("Trying to enroll user.");
            if (!dbHandler.AddUserBlob(ref deviceInfo, ref userBlob, templateFormat))
            {
                Console.WriteLine("Can not enroll user in the system.");
            }

            //Release memory
            if (userBlob.cardObjs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(userBlob.cardObjs);
            }

            if (userBlob.fingerObjs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(userBlob.fingerObjs);
            }

            if (userBlob.faceObjs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(userBlob.faceObjs);
            }
        }

        public void removeUser(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID for the User which you want to enroll");
            Console.Write(">>>> ");
            string userID = Console.ReadLine();
            if (userID.Length == 0)
            {
                Console.WriteLine("The user id can not be empty.");
                return;
            }
            else if (userID.Length > BS2Environment.BS2_USER_ID_SIZE)
            {
                Console.WriteLine("The user id should less than {0} words.", BS2Environment.BS2_USER_ID_SIZE);
                return;
            }
            else
            {
                Console.WriteLine("Trying to remove a user.");
                if (!dbHandler.RemoveUser(userID))
                {
                    Console.WriteLine("Can not remove user from the system.");
                }
            }
        }

        public void serverMatching(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2AuthConfig authConfig;
            Console.WriteLine("Getting auth config.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, deviceID, out authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            if (authConfig.useServerMatching == 0)
            {
                authConfig.useServerMatching = 1;
                Console.WriteLine("Trying to activate server matching.");
                result = (BS2ErrorCode)API.BS2_SetAuthConfig(sdkContext, deviceID, ref authConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }
            }

            this.sdkContext = sdkContext;

            Console.WriteLine("Attaching a server matching handler.");
            cbOnVerifyUser = new API.OnVerifyUser(VerifyUser);
            cbOnIdentifyUser = new API.OnIdentifyUser(IdentifyUser);
            result = (BS2ErrorCode)API.BS2_SetServerMatchingHandler(sdkContext, cbOnVerifyUser, cbOnIdentifyUser);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            matchingTask = new ServerMatchingTask();
            matchingTask.start();

            Console.WriteLine("Press ESC to stop server matching.");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Thread.Sleep(100);
            }

            matchingTask.stop();

            Console.WriteLine("Detaching a server matching handler.");
            result = (BS2ErrorCode)API.BS2_SetServerMatchingHandler(sdkContext, null, null);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Trying to deactivate server matching.");
            authConfig.useServerMatching = 0;
            result = (BS2ErrorCode)API.BS2_SetAuthConfig(sdkContext, deviceID, ref authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            cbOnVerifyUser = null;
            cbOnIdentifyUser = null;
        }

        void ReadyToScanForCard(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your card on the device.");
        }

        void ReadyToScanForFinger(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your finger on the device.");
        }

        public void VerifyUser(UInt32 deviceId, UInt16 seq, byte isCard, byte cardType, IntPtr data, UInt32 dataLen)
        {
            if (Convert.ToBoolean(isCard))
            {
                BS2CSNCard csnCard = Util.AllocateStructure<BS2CSNCard>();
                // FISF-1124  server matching fail when using multi wiegand format;
                //csnCard.type = (byte)BS2CardTypeEnum.CSN;
                const byte WIEGAND_CARD = (byte)BS2CardTypeEnum.WIEGAND;
                csnCard.type = (WIEGAND_CARD & cardType) == WIEGAND_CARD ? WIEGAND_CARD : cardType;
                // FISF-1124  server matching fail when using multi wiegand format;
                csnCard.size = (byte)dataLen;
                Array.Clear(csnCard.data, 0, BS2Environment.BS2_CARD_DATA_SIZE);
                Marshal.Copy(data, csnCard.data, 0, (int)dataLen);

                CardVerifyUserTask cardVerifyUserTask = new CardVerifyUserTask(sdkContext, deviceId, seq, dbHandler, deviceInfo, csnCard);
                matchingTask.enqueue(cardVerifyUserTask);
            }
            else
            {
                byte[] uid = new byte[dataLen];
                Marshal.Copy(data, uid, 0, (int)dataLen);
                string userID = Encoding.UTF8.GetString(uid).TrimEnd('\0');
                IdVerifyUserTask idVerifyUserTask = new IdVerifyUserTask(sdkContext, deviceId, seq, dbHandler, deviceInfo, userID);
                matchingTask.enqueue(idVerifyUserTask);
            }
        }

        public void IdentifyUser(UInt32 deviceId, UInt16 seq, byte format, IntPtr templateData, UInt32 templateSize)
        {
            byte[] fingerprintTemplateData = new byte[templateSize];
            Marshal.Copy(templateData, fingerprintTemplateData, 0, (int)templateSize);

            UFMIdentifyUserTask ufmIdentifyUserTask = new UFMIdentifyUserTask(sdkContext, 
                                                                            deviceId, 
                                                                            seq, 
                                                                            dbHandler, 
                                                                            deviceInfo, 
                                                                            (BS2FingerprintTemplateFormatEnum)format, 
                                                                            BS2FingerprintSecurityEnum.HIGH,
                                                                            fingerprintTemplateData,
                                                                            templateSize);
            matchingTask.enqueue(ufmIdentifyUserTask);
        }

        void print(IntPtr sdkContext, BS2User user)
        {
            Console.WriteLine(">>>> User id[{0}] numCards[{1}] numFingers[{2}] numFaces[{3}]", 
                                Encoding.UTF8.GetString(user.userID).TrimEnd('\0'), 
                                user.numCards,
                                user.numFingers,
                                user.numFaces);
        }
    }
}
*/