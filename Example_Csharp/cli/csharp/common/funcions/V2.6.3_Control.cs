using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Data.SQLite;
using System.Net;


/////////////////////////////////////////////////////////////
//
// 2021.08.02
// Examples for ServerMatching are no longer provided.
// Please refer only at code level.
//
/////////////////////////////////////////////////////////////

namespace Suprema
{
    using BS2_USER_MASK = UInt32;
    
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

    /*
    abstract class VerifyUserTask : BaseTask
    {
        protected abstract BS2ErrorCode find(ref BS2UserSmallBlob userBlob);

        protected VerifyUserTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, DataBaseHandler dbHandler, BS2SimpleDeviceInfo deviceInfo) : base(sdkContext, deviceID, seq, dbHandler, deviceInfo)
        {

        }

        public override void execute()
        {
            BS2UserSmallBlob userBlob = Util.AllocateStructure<BS2UserSmallBlob>();
            BS2ErrorCode handleResult = find(ref userBlob);
            Console.WriteLine("[Server] responded with a status of {0} : device[{1}] seq[{2}].", handleResult, deviceID, seq);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_VerifyUserSmall(sdkContext, deviceID, seq, (int)handleResult, ref userBlob);
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

                if (userBlob.user_photo_obj != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.user_photo_obj);
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

        protected abstract BS2ErrorCode find(ref BS2UserSmallBlob userBlob);

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
            BS2UserSmallBlob userBlob = Util.AllocateStructure<BS2UserSmallBlob>();
            BS2ErrorCode handleResult = find(ref userBlob);
            Console.WriteLine("[Server] responded with a status of {0} : device[{1}] seq[{2}].", handleResult, deviceID, seq);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_IdentifyUserSmall(sdkContext, deviceID, seq, (int)handleResult, ref userBlob);
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

                if (userBlob.user_photo_obj != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.user_photo_obj);
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

        protected override BS2ErrorCode find(ref BS2UserSmallBlob userBlob)
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

        protected override BS2ErrorCode find(ref BS2UserSmallBlob userBlob)
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

        protected override BS2ErrorCode find(ref BS2UserSmallBlob userBlob)
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
    */
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
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS UserAccessGroup(userID CHAR(32), accessGroupId INTEGER, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2UserPhoto(userID CHAR(32), size INTEGER, data BLOB, FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2UserPhrase(userID CHAR(32), userPhrase CHAR(128), FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS BS2Job(userID CHAR(32), code INTEGER, label CHAR(48), FOREIGN KEY(userID) REFERENCES User(id) ON DELETE CASCADE)";
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
                    user.numFaces = Convert.ToByte(rdr[8]);
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

        public bool GetUserBlob(ref BS2SimpleDeviceInfo deviceInfo, ref BS2CSNCard csnCard, ref BS2UserSmallBlob userBlob)
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

        public bool GetUserBlob(ref BS2SimpleDeviceInfo deviceInfo, string userID, ref BS2UserSmallBlob userBlob)
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

        public bool GetUserBlob(ref BS2SimpleDeviceInfo deviceInfo, ref BS2User targetUser, ref BS2UserSmallBlob userBlob)
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

            userBlob.user_photo_obj = IntPtr.Zero;
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

                    userBlob.user_photo_obj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2UserPhoto)));
                    IntPtr curObj = userBlob.user_photo_obj;

                    Marshal.WriteInt32(curObj, (Int32)photoSize);
                    curObj += 4;
                    Marshal.Copy(photoData, 0, curObj, Math.Min((int)photoSize, BS2Environment.BS2_USER_PHOTO_SIZE));
                    curObj += BS2Environment.BS2_USER_PHOTO_SIZE;
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
                cmd.CommandText = "SELECT faceIndex, numOfTemplate, flag, imageLen, imageData, templateData FROM BS2Face WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);

                rdr = cmd.ExecuteReader();
                userBlob.faceObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2Face)) * targetUser.numFaces);
                IntPtr curFaceObjs = userBlob.faceObjs;

                while (rdr.Read())
                {
                    byte faceIndex = Convert.ToByte(rdr[0]);
                    byte numOfTemplate = Convert.ToByte(rdr[1]);
                    byte flag = Convert.ToByte(rdr[2]);
                    UInt16 imageLen = Convert.ToUInt16(rdr[3]);

                    byte[] imageData = (byte[])rdr[4];
                    byte[] templateData = (byte[])rdr[5];

                    Marshal.WriteByte(curFaceObjs, faceIndex);
                    curFaceObjs += 1;
                    Marshal.WriteByte(curFaceObjs, numOfTemplate);
                    curFaceObjs += 1;
                    Marshal.WriteByte(curFaceObjs, flag);
                    curFaceObjs += 2; //flag(1) + reserved(1)
                    Marshal.WriteInt16(curFaceObjs, (Int16)imageLen);
                    curFaceObjs += 4; //imageLen(2) + reserved(2)
                    Marshal.Copy(imageData, 0, curFaceObjs, BS2Environment.BS2_FACE_IMAGE_SIZE);
                    curFaceObjs += BS2Environment.BS2_FACE_IMAGE_SIZE;
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

        public bool GetUserBlobEx(ref BS2SimpleDeviceInfo deviceInfo, ref BS2CSNCard csnCard, ref BS2UserSmallBlobEx userBlob)
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

            return GetUserBlobEx(ref deviceInfo, userID, ref userBlob);
        }

        public bool GetUserBlobEx(ref BS2SimpleDeviceInfo deviceInfo, string userID, ref BS2UserSmallBlobEx userBlob)
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

                    return GetUserBlobEx(ref deviceInfo, ref targetUser, ref userBlob);
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool GetUserBlobEx(ref BS2SimpleDeviceInfo deviceInfo, ref BS2User targetUser, ref BS2UserSmallBlobEx userBlob)
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

            userBlob.user_photo_obj = IntPtr.Zero;
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

                    userBlob.user_photo_obj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2UserPhoto)));
                    IntPtr curObj = userBlob.user_photo_obj;

                    Marshal.WriteInt32(curObj, (Int32)photoSize);
                    curObj += 4;
                    Marshal.Copy(photoData, 0, curObj, Math.Min((int)photoSize, BS2Environment.BS2_USER_PHOTO_SIZE));
                    curObj += BS2Environment.BS2_USER_PHOTO_SIZE;
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
                cmd.CommandText = "SELECT faceIndex, numOfTemplate, flag, imageLen, imageData, templateData FROM BS2Face WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);

                rdr = cmd.ExecuteReader();
                userBlob.faceObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2Face)) * targetUser.numFaces);
                IntPtr curFaceObjs = userBlob.faceObjs;

                while (rdr.Read())
                {
                    byte faceIndex = Convert.ToByte(rdr[0]);
                    byte numOfTemplate = Convert.ToByte(rdr[1]);
                    byte flag = Convert.ToByte(rdr[2]);
                    UInt16 imageLen = Convert.ToUInt16(rdr[3]);

                    byte[] imageData = (byte[])rdr[4];
                    byte[] templateData = (byte[])rdr[5];

                    Marshal.WriteByte(curFaceObjs, faceIndex);
                    curFaceObjs += 1;
                    Marshal.WriteByte(curFaceObjs, numOfTemplate);
                    curFaceObjs += 1;
                    Marshal.WriteByte(curFaceObjs, flag);
                    curFaceObjs += 2; //flag(1) + reserved(1)
                    Marshal.WriteInt16(curFaceObjs, (Int16)imageLen);
                    curFaceObjs += 4; //imageLen(2) + reserved(2)
                    Marshal.Copy(imageData, 0, curFaceObjs, BS2Environment.BS2_FACE_IMAGE_SIZE);
                    curFaceObjs += BS2Environment.BS2_FACE_IMAGE_SIZE;
                    Marshal.Copy(templateData, 0, curFaceObjs, BS2Environment.BS2_TEMPLATE_PER_FACE * BS2Environment.BS2_FACE_TEMPLATE_LENGTH);
                    curFaceObjs += BS2Environment.BS2_TEMPLATE_PER_FACE * BS2Environment.BS2_FACE_TEMPLATE_LENGTH;
                }
                rdr.Close();
            }
            else
            {
                userBlob.faceObjs = IntPtr.Zero;
            }

            userBlob.job.numJobs = 0;

            if (Convert.ToBoolean(deviceInfo.jobCodeSupported))
            {
                cmd.CommandText = "SELECT code, label from BS2Job WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);
                rdr = cmd.ExecuteReader();

                byte numJobs = 0;
                while (rdr.Read())
                {
                    userBlob.job.jobs[numJobs].code = Convert.ToUInt32(rdr[0]);
                    Array.Clear(userBlob.job.jobs[numJobs].label, 0, BS2Environment.BS2_MAX_JOBLABEL_LEN);

                    byte[] label = Encoding.UTF8.GetBytes((string)rdr[1]);
                    Array.Copy(label, 0, userBlob.job.jobs[numJobs].label, 0, label.Length);

                    numJobs++;
                }
                rdr.Close();
                userBlob.job.numJobs = numJobs;
            }

            Array.Clear(userBlob.phrase, 0, BS2Environment.BS2_USER_PHRASE_SIZE);
            if (Convert.ToBoolean(deviceInfo.phraseCodeSupported))
            {
                cmd.CommandText = "SELECT userPhrase from BS2UserPhrase WHERE userID = @userIDParam";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", targetUser.userID);
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    byte[] phrase = Encoding.UTF8.GetBytes((string)rdr[0]);
                    Array.Copy(phrase, 0, userBlob.phrase, 0, phrase.Length);
                }
                rdr.Close();
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

        public bool AddUserBlob(ref BS2UserSmallBlob userBlob, BS2FingerprintTemplateFormatEnum templateFormat)
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

            string userName = System.Text.Encoding.UTF8.GetString(userBlob.name).TrimEnd(new char[] { '\0' });
            if (userName.Length > 0)
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

            if (userBlob.user_photo_obj != IntPtr.Zero)
            {
                BS2UserPhoto photo = (BS2UserPhoto)Marshal.PtrToStructure(userBlob.user_photo_obj, typeof(BS2UserPhoto));

                cmd.CommandText = "INSERT INTO BS2UserPhoto (userID, size, data) VALUES (@userIDParam, @sizeParam, @dataParam)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                cmd.Parameters.AddWithValue("@sizeParam", photo.size);
                cmd.Parameters.AddWithValue("@dataParam", photo.data);

                if (cmd.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            bool isEmptyPinCode = true;
            for (int idx = 0; idx < BS2Environment.BS2_PIN_HASH_SIZE; ++idx)
            {
                if (userBlob.pin[idx] != 0)
                {
                    isEmptyPinCode = false;
                    break;
                }
            }

            if (!isEmptyPinCode)
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

            if (userBlob.user.numCards > 0)
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

            if (userBlob.user.numFingers > 0)
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

            if (userBlob.user.numFaces > 0)
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

        public bool AddUserBlobEx(ref BS2UserSmallBlobEx userBlob, BS2FingerprintTemplateFormatEnum templateFormat)
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

            string userName = System.Text.Encoding.UTF8.GetString(userBlob.name).TrimEnd(new char[] { '\0' });
            if (userName.Length > 0)
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

            if (userBlob.user_photo_obj != IntPtr.Zero)
            {
                BS2UserPhoto photo = (BS2UserPhoto)Marshal.PtrToStructure(userBlob.user_photo_obj, typeof(BS2UserPhoto));
                cmd.CommandText = "INSERT INTO BS2UserPhoto (userID, size, data) VALUES (@userIDParam, @sizeParam, @dataParam)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                cmd.Parameters.AddWithValue("@sizeParam", photo.size);
                cmd.Parameters.AddWithValue("@dataParam", photo.data);

                if (cmd.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            bool isEmptyPinCode = true;
            for (int idx = 0; idx < BS2Environment.BS2_PIN_HASH_SIZE; ++idx)
            {
                if (userBlob.pin[idx] != 0)
                {
                    isEmptyPinCode = false;
                    break;
                }
            }

            if (!isEmptyPinCode)
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

            if (userBlob.user.numCards > 0)
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

            if (userBlob.user.numFingers > 0)
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

            if (userBlob.user.numFaces > 0)
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

            if (userBlob.job.numJobs > 0)
            {
                cmd.CommandText = "INSERT INTO BS2Job (userID, code, label) VALUES (@userIDParam, @codeParam, @labelParam)";
                for (int idx = 0; idx < userBlob.job.numJobs; ++idx)
                {
                    if (userBlob.job.jobs[idx].code != 0)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                        cmd.Parameters.AddWithValue("@codeParam", userBlob.job.jobs[idx].code);
                        cmd.Parameters.AddWithValue("@labelParam", userBlob.job.jobs[idx].label);

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
            }

            string userPhrase = System.Text.Encoding.UTF8.GetString(userBlob.phrase).TrimEnd(new char[] { '\0' });
            if (userPhrase.Length > 0)
            {
                cmd.CommandText = "INSERT INTO BS2UserPhrase (userID, userPhrase) VALUES (@userIDParam, @userPhraseParam)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                cmd.Parameters.AddWithValue("@userPhraseParam", userBlob.phrase);

                if (cmd.ExecuteNonQuery() < 1)
                {
                    transaction.Rollback();
                    return false;
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

    public class V2_6_3_Control : FunctionModule
    {
        private const int USER_PAGE_SIZE = 1024;

        private API.OnReadyToScan cbCardOnReadyToScan = null;
        private API.OnReadyToScan cbFingerOnReadyToScan = null;
        private API.OnReadyToScan cbFaceOnReadyToScan = null;

        private DataBaseHandler dbHandler = new DataBaseHandler();

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            //IPv6 
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get IPConfig", getIPConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set IPConfig", setIPConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get IPV6Config", getIPV6Config));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set IPV6Config", setIPV6Config));

            //User Small Blob
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("List a user from database", listUserFromDatabase));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a user into database", insertUserIntoDatabase));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Delete a user from database", deleteUserFromDatabase));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("List a user with Datas from device", listUserFromDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("List a user with Infos from device", listUserFromDeviceVerInfos));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("List a user with DatasEx from device", listUserFromDeviceVerDatasEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("List a user with InfosEx from device", listUserFromDeviceVerInfosEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a user into device", insertUserIntoDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Delete a user from device", deleteUserFromDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a userEx into database", insertUserIntoDatabaseEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a userEx into device", insertUserIntoDeviceEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get supported User Mask", getUserMask));
            //functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Start server matching", serverMatching));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list with Infos (USB)", listUserInfos));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list with Datas (USB)", listUserDatas));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list with InfosEx (USB)", listUserInfosEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list with DatasEx (USB)", listUserDatasEx));


            //Admin 1000
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Auth Operator Level Ex", getAuthOperatorLevelEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get All Auth Operator Level Ex", getAllAuthOperatorLevelEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Auth Operator Level Ex", setAuthOperatorLevelEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Del Auth Operator Level Ex", delAuthOperatorLevelEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Del All Auth Operator Level Ex", delAllAuthOperatorLevelEx));

            //SSL
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("disable ssl", disbleSSL));

            //Upgrade
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Upgrade firmware", upgradeFirmware));

            return functionList;
        }

        void print(BS2IpConfig config)
        {
            Console.WriteLine(">>>> IP configuration ");
            Console.WriteLine("     |--connectionMode : {0}", config.connectionMode);
            Console.WriteLine("     |--useDHCP : {0}", config.useDHCP);
            Console.WriteLine("     |--useDNS : {0}", config.useDNS);
            Console.WriteLine("     |--ipAddress : {0}", Encoding.UTF8.GetString(config.ipAddress), BitConverter.ToString(config.ipAddress));
            Console.WriteLine("     |--gateway : {0}", Encoding.UTF8.GetString(config.gateway), BitConverter.ToString(config.gateway));
            Console.WriteLine("     |--subnetMask : {0}", Encoding.UTF8.GetString(config.subnetMask), BitConverter.ToString(config.subnetMask));
            Console.WriteLine("     |--serverAddr : {0}", Encoding.UTF8.GetString(config.serverAddr), BitConverter.ToString(config.serverAddr));
            Console.WriteLine("     |--port : {0}", config.port);
            Console.WriteLine("     |--serverPort : {0}", config.serverPort);
            Console.WriteLine("     |--mtuSize : {0}", config.mtuSize);
            Console.WriteLine("     |--baseband : {0}", config.baseband);
            Console.WriteLine("     |--sslServerPort : {0}", config.sslServerPort);
            Console.WriteLine("<<<< ");
        }

        void getIPConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2IpConfig config;
            Console.WriteLine("Trying to get IPConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetIPConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }
        }

        public void setIPConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2IpConfig config = Util.AllocateStructure<BS2IpConfig>();
            Console.WriteLine("Trying to get Current IPConfig");
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            result = (BS2ErrorCode)API.BS2_GetIPConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }

            do
            {
                Console.WriteLine("useDhcp ? [{0}]", config.useDHCP != 0 ? "Y/n" : "y/N");
                Console.Write(">>>> ");
                bool bInput = config.useDHCP != 0 ? Util.IsYes() : !Util.IsNo();
                config.useDHCP = (byte)(bInput ? 1 : 0);

                Console.WriteLine("useDns ? [{0}]", config.useDNS != 0 ? "Y/n" : "y/N");
                Console.Write(">>>> ");
                bInput = config.useDNS != 0 ? Util.IsYes() : !Util.IsNo();
                config.useDNS = (byte)(bInput ? 1 : 0);

                string strInput;
                byte[] bytesInput = null;
                if (config.useDHCP == 0)
                {
                    Console.WriteLine("ipAddress ? [(Blank:{0})]", Encoding.UTF8.GetString(config.ipAddress));
                    Console.Write(">>>> ");
                    strInput = Console.ReadLine();
                    if (strInput.Length == 0)
                    {
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / n(clear)");
                        Console.Write("   >>>> ");
                        if (!Util.IsYes())
                        {
                            Array.Clear(config.ipAddress, 0, config.ipAddress.Length);
                        }
                    }
                    else
                    {
                        Array.Clear(config.ipAddress, 0, config.ipAddress.Length);
                        bytesInput = Encoding.UTF8.GetBytes(strInput);
                        Array.Copy(bytesInput, 0, config.ipAddress, 0, Math.Min(bytesInput.Length, config.ipAddress.Length));
                    }
                    if (Encoding.UTF8.GetString(config.ipAddress).Length > 0)
                    {
                        IPAddress dummy;
                        if (IPAddress.TryParse(Encoding.UTF8.GetString(config.ipAddress).TrimEnd('\0'), out dummy) == false)
                        {
                            Console.WriteLine("Wrong ipAddress: {0})", Encoding.UTF8.GetString(config.ipAddress));
                            return;
                        }
                    }

                    Console.WriteLine("gateway ? [(Blank:{0})]", Encoding.UTF8.GetString(config.gateway));
                    Console.Write(">>>> ");
                    strInput = Console.ReadLine();
                    bytesInput = null;
                    if (strInput.Length == 0)
                    {
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / n(clear)]");
                        Console.Write("   >>>> ");
                        if (!Util.IsYes())
                        {
                            Array.Clear(config.gateway, 0, config.gateway.Length);
                        }
                    }
                    else
                    {
                        Array.Clear(config.gateway, 0, config.gateway.Length);
                        bytesInput = Encoding.UTF8.GetBytes(strInput);
                        Array.Copy(bytesInput, 0, config.gateway, 0, Math.Min(bytesInput.Length, config.gateway.Length));
                    }
                    if (Encoding.UTF8.GetString(config.gateway).Length > 0)
                    {
                        IPAddress dummy;
                        if (IPAddress.TryParse(Encoding.UTF8.GetString(config.gateway).TrimEnd('\0'), out dummy) == false)
                        {
                            Console.WriteLine("Wrong gateway: {0})", Encoding.UTF8.GetString(config.gateway));
                            return;
                        }
                    }

                    Console.WriteLine("subnetMask ? [(Blank:{0})]", Encoding.UTF8.GetString(config.subnetMask));
                    Console.Write(">>>> ");
                    strInput = Console.ReadLine();
                    bytesInput = null;
                    if (strInput.Length == 0)
                    {
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / n(clear)]");
                        Console.Write("   >>>> ");
                        if (!Util.IsYes())
                        {
                            Array.Clear(config.subnetMask, 0, config.subnetMask.Length);
                        }
                    }
                    else
                    {
                        Array.Clear(config.subnetMask, 0, config.subnetMask.Length);
                        bytesInput = Encoding.UTF8.GetBytes(strInput);
                        Array.Copy(bytesInput, 0, config.subnetMask, 0, Math.Min(bytesInput.Length, config.subnetMask.Length));
                    }
                    if (Encoding.UTF8.GetString(config.subnetMask).Length > 0)
                    {
                        IPAddress dummy;
                        if (IPAddress.TryParse(Encoding.UTF8.GetString(config.subnetMask).TrimEnd('\0'), out dummy) == false)
                        {
                            Console.WriteLine("Wrong subnetMask: {0})", Encoding.UTF8.GetString(config.subnetMask));
                            return;
                        }
                    }
                }


                Console.WriteLine("port ? [1~65535 (Blank:{0})]", BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT);
                Console.Write(">>>> ");
                int nInput = Util.GetInput(BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT);
                config.port = (UInt16)nInput;

                Console.WriteLine("Do you want to use server to device connection mode? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                    config.connectionMode = (byte)BS2ConnectionModeEnum.SERVER_TO_DEVICE;
                else
                    config.connectionMode = (byte)BS2ConnectionModeEnum.DEVICE_TO_SERVER;

                if (config.connectionMode == (byte)BS2ConnectionModeEnum.DEVICE_TO_SERVER)
                {
                    Console.WriteLine("serverAddr ? [(Blank:{0})]", Encoding.UTF8.GetString(config.serverAddr));
                    Console.Write(">>>> ");
                    strInput = Console.ReadLine();
                    bytesInput = null;
                    if (strInput.Length == 0)
                    {
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / n(clear)]");
                        Console.Write("   >>>> ");
                        if (!Util.IsYes())
                        {
                            Array.Clear(config.serverAddr, 0, config.serverAddr.Length);
                        }
                    }
                    else
                    {
                        Array.Clear(config.serverAddr, 0, config.serverAddr.Length);
                        bytesInput = Encoding.UTF8.GetBytes(strInput);
                        Array.Copy(bytesInput, 0, config.serverAddr, 0, Math.Min(bytesInput.Length, config.serverAddr.Length));
                    }
                    if (Encoding.UTF8.GetString(config.serverAddr).TrimEnd('\0').Length > 0)
                    {
                        IPAddress dummy;
                        if (IPAddress.TryParse(Encoding.UTF8.GetString(config.serverAddr).TrimEnd('\0'), out dummy) == false)
                        {
                            Console.WriteLine("Wrong serverAddr: {0})", Encoding.UTF8.GetString(config.serverAddr));
                            return;
                        }
                    }

                    Console.WriteLine("serverPort ? [1~65535 (Blank:{0})]", BS2Environment.BS2_TCP_SERVER_PORT_DEFAULT);
                    Console.Write(">>>> ");
                    nInput = Util.GetInput(BS2Environment.BS2_TCP_SERVER_PORT_DEFAULT);
                    config.serverPort = (UInt16)nInput;

                    Console.WriteLine("sslServerPort ? [1~65535 (Blank:{0})]", BS2Environment.BS2_TCP_SSL_SERVER_PORT_DEFAULT);
                    Console.Write(">>>> ");
                    nInput = Util.GetInput(BS2Environment.BS2_TCP_SSL_SERVER_PORT_DEFAULT);
                    config.sslServerPort = (UInt16)nInput;
                }
            } while (false);

            Console.WriteLine("Trying to set IPConfig");
            result = (BS2ErrorCode)API.BS2_SetIPConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            BS2IpConfig changedConfig = Util.AllocateStructure<BS2IpConfig>();
            Console.WriteLine("Trying to get Changed IPConfig");
            result = (BS2ErrorCode)API.BS2_GetIPConfig(sdkContext, deviceID, out changedConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(changedConfig);
            }
        }

        void print(BS2IPV6Config config)
        {
            Console.WriteLine(">>>> IPV6 configuration ");
            Console.WriteLine("     |--useIPV6 : {0}", config.useIPV6);
            Console.WriteLine("     |--reserved1 : {0}", config.reserved1);// useIPV4);
            Console.WriteLine("     |--useDhcpV6 : {0}", config.useDhcpV6);
            Console.WriteLine("     |--useDnsV6 : {0}", config.useDnsV6);
            Console.WriteLine("     |--staticIpAddressV6 : {0}", Encoding.UTF8.GetString(config.staticIpAddressV6), BitConverter.ToString(config.staticIpAddressV6));
            Console.WriteLine("     |--staticGatewayV6 : {0}", Encoding.UTF8.GetString(config.staticGatewayV6), BitConverter.ToString(config.staticGatewayV6));
            Console.WriteLine("     |--dnsAddrV6 : {0}", Encoding.UTF8.GetString(config.dnsAddrV6), BitConverter.ToString(config.dnsAddrV6));
            Console.WriteLine("     |--serverIpAddressV6 : {0}", Encoding.UTF8.GetString(config.serverIpAddressV6), BitConverter.ToString(config.serverIpAddressV6));
            Console.WriteLine("     |--serverPortV6 : {0}", config.serverPortV6);
            Console.WriteLine("     |--sslServerPortV6 : {0}", config.sslServerPortV6);
            Console.WriteLine("     |--portV6 : {0}", config.portV6);
            Console.WriteLine("     |--numOfAllocatedAddressV6 : {0}", config.numOfAllocatedAddressV6);
            Console.WriteLine("     |--numOfAllocatedGatewayV6 : {0}", config.numOfAllocatedGatewayV6);
            byte[] tempIPV6 = new byte[BS2Environment.BS2_IPV6_ADDR_SIZE];
            for (int idx = 0; idx < config.numOfAllocatedAddressV6; ++idx)
            {
                Array.Copy(config.allocatedIpAddressV6, idx * BS2Environment.BS2_IPV6_ADDR_SIZE, tempIPV6, 0, BS2Environment.BS2_IPV6_ADDR_SIZE);
                Console.WriteLine("     |--allocatedIpAddressV6[{0}] : {1}", idx, Encoding.UTF8.GetString(tempIPV6), BitConverter.ToString(tempIPV6));
            }
            for (int idx = 0; idx < config.numOfAllocatedGatewayV6; ++idx)
            {
                Array.Copy(config.allocatedGatewayV6, idx * BS2Environment.BS2_IPV6_ADDR_SIZE, tempIPV6, 0, BS2Environment.BS2_IPV6_ADDR_SIZE);
                Console.WriteLine("     |--allocatedGatewayV6[{0}] : {1}", idx, Encoding.UTF8.GetString(tempIPV6), BitConverter.ToString(tempIPV6));
            }
            Console.WriteLine("<<<< ");
        }

        void getIPV6Config(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2IPV6Config config;
            Console.WriteLine("Trying to get IPV6Config");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetIPV6Config(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }
        }

        public void setIPV6Config(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2IPV6Config config = Util.AllocateStructure<BS2IPV6Config>();
            Console.WriteLine("Trying to get Current IPV6Config");
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            result = (BS2ErrorCode)API.BS2_GetIPV6Config(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }

            do
            {
                Console.WriteLine("useDhcpV6 ? [{0}]", config.useDhcpV6 != 0 ? "Y/n" : "y/N");
                Console.Write(">>>> ");
                bool bInput = config.useDhcpV6 != 0 ? Util.IsYes() : !Util.IsNo();
                config.useDhcpV6 = (byte)(bInput ? 1 : 0);

                Console.WriteLine("useDnsV6 ? [{0}]", config.useDnsV6 != 0 ? "Y/n" : "y/N");
                Console.Write(">>>> ");
                bInput = config.useDnsV6 != 0 ? Util.IsYes() : !Util.IsNo();
                config.useDnsV6 = (byte)(bInput ? 1 : 0);

                string strInput;
                byte[] bytesInput = null;
                if (config.useDhcpV6 == 0)
                {
                    Console.WriteLine("staticIpAddressV6 ? [(Blank:{0})]", Encoding.UTF8.GetString(config.staticIpAddressV6));
                    Console.Write(">>>> ");
                    strInput = Console.ReadLine();
                    if (strInput.Length == 0)
                    {
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / N(clear), (Blank:Y)]");
                        Console.Write("   >>>> ");
                        if (!Util.IsYes())
                        {
                            Array.Clear(config.staticIpAddressV6, 0, config.staticIpAddressV6.Length);
                        }
                    }
                    else
                    {
                        Array.Clear(config.staticIpAddressV6, 0, config.staticIpAddressV6.Length);
                        bytesInput = Encoding.UTF8.GetBytes(strInput);
                        Array.Copy(bytesInput, 0, config.staticIpAddressV6, 0, Math.Min(bytesInput.Length, config.staticIpAddressV6.Length));
                    }
                    if (Encoding.UTF8.GetString(config.staticIpAddressV6).Length > 0)
                    {
                        IPAddress dummy;
                        if (IPAddress.TryParse(Encoding.UTF8.GetString(config.staticIpAddressV6).TrimEnd('\0'), out dummy) == false)
                        {
                            Console.WriteLine("Wrong staticIpAddressV6: {0})", Encoding.UTF8.GetString(config.staticIpAddressV6));
                            return;
                        }
                    }


                    Console.WriteLine("staticGatewayV6 ? [(Blank:{0})]", Encoding.UTF8.GetString(config.staticGatewayV6));
                    Console.Write(">>>> ");
                    strInput = Console.ReadLine();
                    bytesInput = null;
                    if (strInput.Length == 0)
                    {
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / n(clear)]");
                        Console.Write("   >>>> ");
                        if (!Util.IsYes())
                        {
                            Array.Clear(config.staticGatewayV6, 0, config.staticGatewayV6.Length);
                        }
                    }
                    else
                    {
                        Array.Clear(config.staticGatewayV6, 0, config.staticGatewayV6.Length);
                        bytesInput = Encoding.UTF8.GetBytes(strInput);
                        Array.Copy(bytesInput, 0, config.staticGatewayV6, 0, Math.Min(bytesInput.Length, config.staticGatewayV6.Length));
                    }
                    if (Encoding.UTF8.GetString(config.staticGatewayV6).Length > 0)
                    {
                        IPAddress dummy;
                        if (IPAddress.TryParse(Encoding.UTF8.GetString(config.staticGatewayV6).TrimEnd('\0'), out dummy) == false)
                        {
                            Console.WriteLine("Wrong staticGatewayV6: {0})", Encoding.UTF8.GetString(config.staticGatewayV6));
                            return;
                        }
                    }
                }

                if (config.useDnsV6 == 1)
                {
                    Console.WriteLine("dnsAddrV6 ? [(Blank:{0})]", Encoding.UTF8.GetString(config.dnsAddrV6));
                    Console.Write(">>>> ");
                    strInput = Console.ReadLine();
                    bytesInput = null;
                    if (strInput.Length == 0)
                    {
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / n(clear)]");
                        Console.Write("   >>>> ");
                        if (!Util.IsYes())
                        {
                            Array.Clear(config.dnsAddrV6, 0, config.dnsAddrV6.Length);
                        }
                    }
                    else
                    {
                        Array.Clear(config.dnsAddrV6, 0, config.dnsAddrV6.Length);
                        bytesInput = Encoding.UTF8.GetBytes(strInput);
                        Array.Copy(bytesInput, 0, config.dnsAddrV6, 0, Math.Min(bytesInput.Length, config.dnsAddrV6.Length));
                    }
                    if (Encoding.UTF8.GetString(config.dnsAddrV6).Length > 0)
                    {
                        IPAddress dummy;
                        if (IPAddress.TryParse(Encoding.UTF8.GetString(config.dnsAddrV6).TrimEnd('\0'), out dummy) == false)
                        {
                            Console.WriteLine("Wrong dnsAddrV6: {0})", Encoding.UTF8.GetString(config.dnsAddrV6));
                            return;
                        }
                    }
                }

                Console.WriteLine("serverIpAddressV6 ? [(Blank:{0})]", Encoding.UTF8.GetString(config.serverIpAddressV6));
                Console.Write(">>>> ");
                strInput = Console.ReadLine();
                bytesInput = null;
                if (strInput.Length == 0)
                {
                    Console.WriteLine("   Do you want to keep the value? [Y(keep) / n(clear)]");
                    Console.Write("   >>>> ");
                    if (!Util.IsYes())
                    {
                        Array.Clear(config.serverIpAddressV6, 0, config.serverIpAddressV6.Length);
                    }
                }
                else
                {
                    Array.Clear(config.serverIpAddressV6, 0, config.serverIpAddressV6.Length);
                    bytesInput = Encoding.UTF8.GetBytes(strInput);
                    Array.Copy(bytesInput, 0, config.serverIpAddressV6, 0, Math.Min(bytesInput.Length, config.serverIpAddressV6.Length));
                }
                if (Encoding.UTF8.GetString(config.serverIpAddressV6).TrimEnd('\0').Length > 0)
                {
                    IPAddress dummy;
                    if (IPAddress.TryParse(Encoding.UTF8.GetString(config.serverIpAddressV6), out dummy) == false)
                    {
                        Console.WriteLine("Wrong serverIpAddressV6: {0})", Encoding.UTF8.GetString(config.serverIpAddressV6));
                        return;
                    }
                }

                Console.WriteLine("serverPortV6 ? [1~65535 (Blank:{0})]", BS2Environment.BS2_TCP_SERVER_PORT_DEFAULT_V6);
                Console.Write(">>>> ");
                int nInput = Util.GetInput(BS2Environment.BS2_TCP_SERVER_PORT_DEFAULT_V6);
                config.serverPortV6 = (UInt16)nInput;

                Console.WriteLine("sslServerPortV6 ? [1~65535 (Blank:{0})]", BS2Environment.BS2_TCP_SSL_SERVER_PORT_DEFAULT_V6);
                Console.Write(">>>> ");
                nInput = Util.GetInput(BS2Environment.BS2_TCP_SSL_SERVER_PORT_DEFAULT_V6);
                config.sslServerPortV6 = (UInt16)nInput;

                Console.WriteLine("portV6 ? [1~65535 (Blank:{0})]", BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT_V6);
                Console.Write(">>>> ");
                nInput = Util.GetInput(BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT_V6);
                config.portV6 = (UInt16)nInput;

                config.numOfAllocatedAddressV6 = 0;
                config.numOfAllocatedGatewayV6 = 0;

            } while (false);

            Console.WriteLine("Trying to set IPV6Config");
            result = (BS2ErrorCode)API.BS2_SetIPV6Config(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            BS2IPV6Config changedconfig = Util.AllocateStructure<BS2IPV6Config>();
            Console.WriteLine("Trying to get Changed IPV6Config");
            result = (BS2ErrorCode)API.BS2_GetIPV6Config(sdkContext, deviceID, out changedconfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(changedconfig);
            }
        }

        public void getUserMask(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2_USER_MASK userMask = 0;

            Console.WriteLine("Trying to get supported user mask");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSupportedUserMask(sdkContext, deviceID, out userMask);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Supported User Mask: 0x{0:X}", userMask);
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void listUserFromDatabase(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<BS2User> userList = new List<BS2User>();
            if (dbHandler.GetUserList(ref deviceInfo, ref userList))
            {
                if (userList.Count > 0)
                {
                    foreach (BS2User user in userList)
                    {
                        print(user);
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

        public void insertUserIntoDatabase(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            BS2FingerprintTemplateFormatEnum templateFormat = BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA;
            SortedSet<BS2CardAuthModeEnum> privateCardAuthMode = new SortedSet<BS2CardAuthModeEnum>();
            SortedSet<BS2FingerAuthModeEnum> privateFingerAuthMode = new SortedSet<BS2FingerAuthModeEnum>();
            SortedSet<BS2IDAuthModeEnum> privateIDAuthMode = new SortedSet<BS2IDAuthModeEnum>();
            SortedSet<BS2FaceAuthModeEnum> privateFaceAuthMode = new SortedSet<BS2FaceAuthModeEnum>();

            bool cardSupported = Convert.ToBoolean(deviceInfo.cardSupported);
            bool fingerSupported = Convert.ToBoolean(deviceInfo.fingerSupported);
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            bool faceSupported = Convert.ToBoolean(deviceInfo.faceSupported);

            privateIDAuthMode.Add(BS2IDAuthModeEnum.PROHIBITED);

            if (cardSupported)
            {
                privateCardAuthMode.Add(BS2CardAuthModeEnum.PROHIBITED);
                privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_ONLY);

                if (pinSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_PIN);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_PIN);

                    if (fingerSupported)
                    {
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN);
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN);

                        privateFingerAuthMode.Add(BS2FingerAuthModeEnum.BIOMETRIC_PIN);

                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN);
                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_PIN);
                    }

                    if (faceSupported)
                    {
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN);
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN);

                        privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_PIN);

                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN);
                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_PIN);
                    }
                }

                if (fingerSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC);

                    privateFingerAuthMode.Add(BS2FingerAuthModeEnum.BIOMETRIC_ONLY);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC);
                }

                if (faceSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC);

                    privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_ONLY);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC);
                }
            }
            else if (fingerSupported)
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
            else if (faceSupported)
            {
                if (pinSupported)
                {
                    privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_PIN);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN);
                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_PIN);
                }

                privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_ONLY);
                privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC);
            }

            BS2UserSmallBlob userBlob = Util.AllocateStructure<BS2UserSmallBlob>();
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
            userBlob.user_photo_obj = IntPtr.Zero;

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

            if (fingerSupported)
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

            if (faceSupported)
            {
                Console.WriteLine("Enter the security level for this user: [{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}]",
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
                    foreach (BS2FaceAuthModeEnum faceAuthModeEnum in privateFaceAuthMode)
                    {
                        if (faceAuthModeEnum == BS2FaceAuthModeEnum.BIOMETRIC_ONLY)
                        {
                            Console.Write("{0}: {1}(default) ", (byte)faceAuthModeEnum, faceAuthModeEnum);
                        }
                        else
                        {
                            Console.Write("{0}: {1} ", (byte)faceAuthModeEnum, faceAuthModeEnum);
                        }
                    }
                    Console.WriteLine("]");
                    Console.Write(">>>> ");
                    userBlob.setting.fingerAuthMode = Util.GetInput((byte)BS2FaceAuthModeEnum.BIOMETRIC_ONLY);
                }
                else
                {
                    userBlob.setting.fingerAuthMode = (byte)BS2FaceAuthModeEnum.NONE;
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
                userBlob.setting.idAuthMode = (byte)BS2IDAuthModeEnum.NONE;
            }

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

            Console.WriteLine("Do you want to set profile image? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Enter the jpg file path for this user.");
                Console.Write(">>>> ");
                string imagePath = Console.ReadLine();

                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Invalid file path");
                    return;
                }

                Image profileImage = Image.FromFile(imagePath);
                if (!profileImage.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                {
                    Console.WriteLine("Invalid image file format");
                    return;
                }

                IntPtr imageData = IntPtr.Zero;
                UInt32 imageDataLen = 0;

                if (Util.LoadBinary(imagePath, out imageData, out imageDataLen))
                {
                    if (imageDataLen == 0)
                    {
                        Console.WriteLine("Empty image file");
                        return;
                    }
                    else if (imageDataLen > BS2Environment.BS2_USER_PHOTO_SIZE)
                    {
                        Console.WriteLine("The profile image should less than {0} bytes.", BS2Environment.BS2_USER_PHOTO_SIZE);
                        return;
                    }

                    userBlob.user_photo_obj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2UserPhoto)));
                    IntPtr curObj = userBlob.user_photo_obj;

                    Marshal.WriteInt32(curObj, (Int32)imageDataLen);
                    curObj += 4;
                    IntPtr curDest = curObj;
                    IntPtr curSrc = imageData;
                    for (int idx = 0; idx < Math.Min((int)imageDataLen, BS2Environment.BS2_USER_PHOTO_SIZE); ++idx)
                    {
                        Marshal.WriteByte(curDest, Marshal.ReadByte(curSrc));
                        curDest += 1;
                        curSrc += 1;
                    }
                    curObj += BS2Environment.BS2_USER_PHOTO_SIZE;
                    Marshal.FreeHGlobal(imageData);
                }
            }

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

            if (fingerSupported)
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
                            result = (BS2ErrorCode)API.BS2_ScanFingerprintEx(sdkContext, deviceID, ref fingerprint, templateIndex, (UInt32)BS2FingerprintQualityEnum.QUALITY_STANDARD, (byte)templateFormat, out outquality, cbFingerOnReadyToScan);
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

            if (faceSupported)
            {
                Console.WriteLine("How many faces do you want to register? [1(default) - {0}]", BS2Environment.BS2_MAX_NUM_OF_FACE_PER_USER);
                Console.Write(">>>> ");
                userBlob.user.numFaces = Util.GetInput((byte)1);

                if (userBlob.user.numFaces > 0)
                {
                    byte enrollThreshold;
                    BS2FaceConfig faceConfig;
                    Console.WriteLine("Trying to get face config");
                    result = (BS2ErrorCode)API.BS2_GetFaceConfig(sdkContext, deviceID, out faceConfig);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Got error({0}).", result);
                        return;
                    }
                    else
                    {
                        enrollThreshold = faceConfig.enrollThreshold;
                    }

                    int structSize = Marshal.SizeOf(typeof(BS2Face));
                    BS2Face[] face = Util.AllocateStructureArray<BS2Face>(1);

                    userBlob.faceObjs = Marshal.AllocHGlobal(structSize * userBlob.user.numFaces);
                    IntPtr curFaceObjs = userBlob.faceObjs;
                    cbFaceOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFace);

                    for (int idx = 0; idx < userBlob.user.numFaces; ++idx)
                    {
                        Console.WriteLine("Trying to scan face[{0}]", idx);
                        result = (BS2ErrorCode)API.BS2_ScanFace(sdkContext, deviceID, face, enrollThreshold, cbFaceOnReadyToScan);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                            return;
                        }

                        face[0].faceIndex = (byte)idx;
                        Marshal.StructureToPtr(face[0], curFaceObjs, false);
                        curFaceObjs += structSize;

                        Thread.Sleep(100);
                    }

                    cbFaceOnReadyToScan = null;
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
            if (!dbHandler.AddUserBlob(ref userBlob, templateFormat))
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

            if (userBlob.user_photo_obj != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(userBlob.user_photo_obj);
            }
        }

        public void deleteUserFromDatabase(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID for the User which you want to remove");
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

        public void listUserFromDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = null; // we don't need to user id filtering

            Console.WriteLine("Trying to get user list.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserList(sdkContext, deviceID, out outUidObjs, out numUserIds, cbIsAcceptableUserID);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (numUserIds > 0)
                {
                    IntPtr curUidObjs = outUidObjs;
                    BS2UserSmallBlob[] userBlobs = new BS2UserSmallBlob[USER_PAGE_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        result = (BS2ErrorCode)API.BS2_GetUserSmallDatas(sdkContext, deviceID, curUidObjs, available, userBlobs, (UInt32)BS2UserMaskEnum.ALL);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(userBlobs[loop].user);
                                // don't need to release cardObj, FingerObj, FaceObj because we get only BS2User
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                                if (userBlobs[loop].user_photo_obj != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].user_photo_obj);
                            }

                            idx += available;
                            curUidObjs += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            break;
                        }
                    }

                    API.BS2_ReleaseObject(outUidObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void listUserFromDeviceVerInfos(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = null; // we don't need to user id filtering

            Console.WriteLine("Trying to get user list.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserList(sdkContext, deviceID, out outUidObjs, out numUserIds, cbIsAcceptableUserID);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (numUserIds > 0)
                {
                    IntPtr curUidObjs = outUidObjs;
                    BS2UserSmallBlob[] userBlobs = new BS2UserSmallBlob[USER_PAGE_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        result = (BS2ErrorCode)API.BS2_GetUserSmallInfos(sdkContext, deviceID, curUidObjs, available, userBlobs);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(userBlobs[loop].user);
                                // don't need to release cardObj, FingerObj, FaceObj because we get only BS2User
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                                if (userBlobs[loop].user_photo_obj != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].user_photo_obj);
                            }

                            idx += available;
                            curUidObjs += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            break;
                        }
                    }

                    API.BS2_ReleaseObject(outUidObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void listUserFromDeviceVerDatasEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = null; // we don't need to user id filtering

            Console.WriteLine("Trying to get user list.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserList(sdkContext, deviceID, out outUidObjs, out numUserIds, cbIsAcceptableUserID);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (numUserIds > 0)
                {
                    IntPtr curUidObjs = outUidObjs;
                    BS2UserSmallBlobEx[] userBlobs = new BS2UserSmallBlobEx[USER_PAGE_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        result = (BS2ErrorCode)API.BS2_GetUserSmallDatasEx(sdkContext, deviceID, curUidObjs, available, userBlobs, (UInt32)BS2UserMaskEnum.ALL);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(userBlobs[loop].user);
                                // don't need to release cardObj, FingerObj, FaceObj because we get only BS2User
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                                if (userBlobs[loop].user_photo_obj != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].user_photo_obj);
                            }

                            idx += available;
                            curUidObjs += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            break;
                        }
                    }

                    API.BS2_ReleaseObject(outUidObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void listUserFromDeviceVerInfosEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = null; // we don't need to user id filtering

            Console.WriteLine("Trying to get user list.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserList(sdkContext, deviceID, out outUidObjs, out numUserIds, cbIsAcceptableUserID);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (numUserIds > 0)
                {
                    IntPtr curUidObjs = outUidObjs;
                    BS2UserSmallBlobEx[] userBlobs = new BS2UserSmallBlobEx[USER_PAGE_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        result = (BS2ErrorCode)API.BS2_GetUserSmallInfosEx(sdkContext, deviceID, curUidObjs, available, userBlobs);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(userBlobs[loop].user);
                                // don't need to release cardObj, FingerObj, FaceObj because we get only BS2User
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                                if (userBlobs[loop].user_photo_obj != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].user_photo_obj);
                            }

                            idx += available;
                            curUidObjs += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            break;
                        }
                    }

                    API.BS2_ReleaseObject(outUidObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }


        public void insertUserIntoDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<BS2User> userList = new List<BS2User>();
            if (dbHandler.GetUserList(ref deviceInfo, ref userList))
            {
                if (userList.Count > 0)
                {
                    Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                    for (int idx = 0; idx < userList.Count; ++idx)
                    {
                        Console.Write("[{0:000}] ==> ", idx);
                        print(userList[idx]);
                    }
                    Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                    Console.WriteLine("Please, choose the index of the user which you want to enroll.");
                    Console.Write(">>>> ");

                    Int32 selection = Util.GetInput();
                    if (selection >= 0)
                    {
                        if (selection < userList.Count)
                        {
                            BS2User user = userList[selection];
                            BS2UserSmallBlob[] userBlob = Util.AllocateStructureArray<BS2UserSmallBlob>(1);
                            if (dbHandler.GetUserBlob(ref deviceInfo, ref user, ref userBlob[0]))
                            {
                                Console.WriteLine("Trying to enroll user.");
                                //BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrolUser(sdkContext, deviceID, userBlob, 1, 1);
                                BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrollUserSmall(sdkContext, deviceID, userBlob, 1, 1);
                                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                                {
                                    Console.WriteLine("Got error({0}).", result);
                                }

                                if (userBlob[0].cardObjs != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(userBlob[0].cardObjs);
                                }

                                if (userBlob[0].fingerObjs != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(userBlob[0].fingerObjs);
                                }

                                if (userBlob[0].faceObjs != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(userBlob[0].faceObjs);
                                }

                                if (userBlob[0].user_photo_obj != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(userBlob[0].user_photo_obj);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid selection[{0}]", selection);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid user index");
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

        public void deleteUserFromDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all users? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all user from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllUser(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID for the User which you want to remove");
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
                    byte[] uidArray = new byte[BS2Environment.BS2_USER_ID_SIZE];
                    byte[] rawUid = Encoding.UTF8.GetBytes(userID);
                    IntPtr uids = Marshal.AllocHGlobal(BS2Environment.BS2_USER_ID_SIZE);

                    Array.Clear(uidArray, 0, BS2Environment.BS2_USER_ID_SIZE);
                    Array.Copy(rawUid, 0, uidArray, 0, rawUid.Length);
                    Marshal.Copy(uidArray, 0, uids, BS2Environment.BS2_USER_ID_SIZE);

                    Console.WriteLine("Trying to remove a user.");
                    result = (BS2ErrorCode)API.BS2_RemoveUser(sdkContext, deviceID, uids, 1);

                    Marshal.FreeHGlobal(uids);
                }
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void ReadyToScanForCard(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your card on the device.");
        }

        void ReadyToScanForFinger(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your finger on the device.");
        }

        void ReadyToScanForFace(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your face on the device.");
        }

        void print(BS2User user)
        {
            Console.WriteLine(">>>> User id[{0}] numCards[{1}] numFingers[{2}] numFaces[{3}]",
                                Encoding.UTF8.GetString(user.userID).TrimEnd('\0'),
                                user.numCards,
                                user.numFingers,
                                user.numFaces);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Ex
        public void insertUserIntoDatabaseEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            BS2FingerprintTemplateFormatEnum templateFormat = BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA;
            SortedSet<BS2CardAuthModeEnum> privateCardAuthMode = new SortedSet<BS2CardAuthModeEnum>();
            SortedSet<BS2FingerAuthModeEnum> privateFingerAuthMode = new SortedSet<BS2FingerAuthModeEnum>();
            SortedSet<BS2IDAuthModeEnum> privateIDAuthMode = new SortedSet<BS2IDAuthModeEnum>();
            SortedSet<BS2FaceAuthModeEnum> privateFaceAuthMode = new SortedSet<BS2FaceAuthModeEnum>();

            bool cardSupported = Convert.ToBoolean(deviceInfo.cardSupported);
            bool fingerSupported = Convert.ToBoolean(deviceInfo.fingerSupported);
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            bool faceSupported = Convert.ToBoolean(deviceInfo.faceSupported);
            bool jobSupported = Convert.ToBoolean(deviceInfo.jobCodeSupported);
            bool phraseSupported = Convert.ToBoolean(deviceInfo.phraseCodeSupported);

            privateIDAuthMode.Add(BS2IDAuthModeEnum.PROHIBITED);

            if (cardSupported)
            {
                privateCardAuthMode.Add(BS2CardAuthModeEnum.PROHIBITED);
                privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_ONLY);

                if (pinSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_PIN);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_PIN);

                    if (fingerSupported)
                    {
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN);
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN);

                        privateFingerAuthMode.Add(BS2FingerAuthModeEnum.BIOMETRIC_PIN);

                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN);
                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_PIN);
                    }

                    if (faceSupported)
                    {
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN);
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN);

                        privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_PIN);

                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN);
                        privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_PIN);
                    }
                }

                if (fingerSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC);

                    privateFingerAuthMode.Add(BS2FingerAuthModeEnum.BIOMETRIC_ONLY);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC);
                }

                if (faceSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC);

                    privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_ONLY);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC);
                }
            }
            else if (fingerSupported)
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
            else if (faceSupported)
            {
                if (pinSupported)
                {
                    privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_PIN);

                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN);
                    privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC_PIN);
                }

                privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_ONLY);
                privateIDAuthMode.Add(BS2IDAuthModeEnum.ID_BIOMETRIC);
            }

            BS2UserSmallBlobEx userBlob = Util.AllocateStructure<BS2UserSmallBlobEx>();
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
            userBlob.user_photo_obj = IntPtr.Zero;

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

            if (fingerSupported)
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

            if (faceSupported)
            {
                Console.WriteLine("Enter the security level for this user: [{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}]",
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
                    foreach (BS2FaceAuthModeEnum faceAuthModeEnum in privateFaceAuthMode)
                    {
                        if (faceAuthModeEnum == BS2FaceAuthModeEnum.BIOMETRIC_ONLY)
                        {
                            Console.Write("{0}: {1}(default) ", (byte)faceAuthModeEnum, faceAuthModeEnum);
                        }
                        else
                        {
                            Console.Write("{0}: {1} ", (byte)faceAuthModeEnum, faceAuthModeEnum);
                        }
                    }
                    Console.WriteLine("]");
                    Console.Write(">>>> ");
                    userBlob.setting.fingerAuthMode = Util.GetInput((byte)BS2FaceAuthModeEnum.BIOMETRIC_ONLY);
                }
                else
                {
                    userBlob.setting.fingerAuthMode = (byte)BS2FaceAuthModeEnum.NONE;
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
                userBlob.setting.idAuthMode = (byte)BS2IDAuthModeEnum.NONE;
            }

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

            Console.WriteLine("Do you want to set profile image? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Enter the jpg file path for this user.");
                Console.Write(">>>> ");
                string imagePath = Console.ReadLine();

                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Invalid file path");
                    return;
                }

                Image profileImage = Image.FromFile(imagePath);
                if (!profileImage.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                {
                    Console.WriteLine("Invalid image file format");
                    return;
                }

                IntPtr imageData = IntPtr.Zero;
                UInt32 imageDataLen = 0;

                if (Util.LoadBinary(imagePath, out imageData, out imageDataLen))
                {
                    if (imageDataLen == 0)
                    {
                        Console.WriteLine("Empty image file");
                        return;
                    }
                    else if (imageDataLen > BS2Environment.BS2_USER_PHOTO_SIZE)
                    {
                        Console.WriteLine("The profile image should less than {0} bytes.", BS2Environment.BS2_USER_PHOTO_SIZE);
                        return;
                    }


                    userBlob.user_photo_obj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2UserPhoto)));
                    IntPtr curObj = userBlob.user_photo_obj;

                    Marshal.WriteInt32(curObj, (Int32)imageDataLen);
                    curObj += 4;
                    IntPtr curDest = curObj;
                    IntPtr curSrc = imageData;
                    for (int idx = 0; idx < Math.Min((int)imageDataLen, BS2Environment.BS2_USER_PHOTO_SIZE); ++idx)
                    {
                        Marshal.WriteByte(curDest, Marshal.ReadByte(curSrc));
                        curDest += 1;
                        curSrc += 1;
                    }
                    curObj += BS2Environment.BS2_USER_PHOTO_SIZE;
                    Marshal.FreeHGlobal(imageData);
                }
            }

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

            if (fingerSupported)
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
                            result = (BS2ErrorCode)API.BS2_ScanFingerprintEx(sdkContext, deviceID, ref fingerprint, templateIndex, (UInt32)BS2FingerprintQualityEnum.QUALITY_STANDARD, (byte)templateFormat, out outquality, cbFingerOnReadyToScan);
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

            if (faceSupported)
            {
                Console.WriteLine("How many faces do you want to register? [1(default) - {0}]", BS2Environment.BS2_MAX_NUM_OF_FACE_PER_USER);
                Console.Write(">>>> ");
                userBlob.user.numFaces = Util.GetInput((byte)1);

                if (userBlob.user.numFaces > 0)
                {
                    byte enrollThreshold;
                    BS2FaceConfig faceConfig;
                    Console.WriteLine("Trying to get face config");
                    result = (BS2ErrorCode)API.BS2_GetFaceConfig(sdkContext, deviceID, out faceConfig);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Got error({0}).", result);
                        return;
                    }
                    else
                    {
                        enrollThreshold = faceConfig.enrollThreshold;
                    }

                    int structSize = Marshal.SizeOf(typeof(BS2Face));
                    BS2Face[] face = Util.AllocateStructureArray<BS2Face>(1);

                    userBlob.faceObjs = Marshal.AllocHGlobal(structSize * userBlob.user.numFaces);
                    IntPtr curFaceObjs = userBlob.faceObjs;
                    cbFaceOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFace);

                    for (int idx = 0; idx < userBlob.user.numFaces; ++idx)
                    {
                        Console.WriteLine("Trying to scan face[{0}]", idx);
                        result = (BS2ErrorCode)API.BS2_ScanFace(sdkContext, deviceID, face, enrollThreshold, cbFaceOnReadyToScan);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                            return;
                        }

                        face[0].faceIndex = (byte)idx;
                        Marshal.StructureToPtr(face[0], curFaceObjs, false);
                        curFaceObjs += structSize;

                        Thread.Sleep(100);
                    }

                    cbFaceOnReadyToScan = null;
                }
            }

            if (phraseSupported)
            {
                Array.Clear(userBlob.phrase, 0, BS2Environment.BS2_USER_PHRASE_SIZE);
                Console.WriteLine("Do you want to set user phrase? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("Enter the phrase for this user");
                    Console.Write(">>>> ");
                    string userPhrase = Console.ReadLine();
                    if (userPhrase.Length == 0)
                    {
                        Console.WriteLine("[Warning] user phrase will be displayed as empty.");
                    }
                    else if (userPhrase.Length > BS2Environment.BS2_USER_PHRASE_SIZE)
                    {
                        Console.WriteLine("The user phrase should less than {0} words.", BS2Environment.BS2_USER_PHRASE_SIZE);
                        return;
                    }
                    else
                    {
                        byte[] userPhraseArray = Encoding.UTF8.GetBytes(userPhrase);
                        Array.Copy(userPhraseArray, userBlob.phrase, userPhraseArray.Length);
                    }
                }
            }

            if (jobSupported)
            {
                Console.WriteLine("How many jobs do you want to set? [1(default)-16]");
                Console.Write(">>>> ");
                char[] delimiterChars2 = { ' ', ',', '.', ':', '\t' };
                byte amount = Util.GetInput(1);
                userBlob.job.numJobs = amount;

                for (int idx = 0; idx < amount; ++idx)
                {
                    Console.WriteLine("Enter a value for job[{0}]", idx);
                    Console.WriteLine("  Enter the code for the job which you want to set [1(default) .... N]");
                    Console.Write("  >>>> ");
                    userBlob.job.jobs[idx].code = (UInt32)Util.GetInput();
                    Console.WriteLine("  Enter the label for the job which you want to set");
                    Console.Write("  >>>> ");
                    string label = Console.ReadLine();
                    if (label.Length == 0)
                    {
                        Console.WriteLine("  [Warning] label will be displayed as empty.");
                    }
                    else if (label.Length > BS2Environment.BS2_MAX_JOBLABEL_LEN)
                    {
                        Console.WriteLine("  label of job should less than {0} words.", BS2Environment.BS2_MAX_JOBLABEL_LEN);
                        return;
                    }
                    else
                    {
                        byte[] labelArray = Encoding.UTF8.GetBytes(label);
                        Array.Clear(userBlob.job.jobs[idx].label, 0, BS2Environment.BS2_MAX_JOBLABEL_LEN);
                        Array.Copy(labelArray, userBlob.job.jobs[idx].label, labelArray.Length);
                    }
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
            if (!dbHandler.AddUserBlobEx(ref userBlob, templateFormat))
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

            if (userBlob.user_photo_obj != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(userBlob.user_photo_obj);
            }
        }

        public void insertUserIntoDeviceEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<BS2User> userList = new List<BS2User>();
            if (dbHandler.GetUserList(ref deviceInfo, ref userList))
            {
                if (userList.Count > 0)
                {
                    Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                    for (int idx = 0; idx < userList.Count; ++idx)
                    {
                        Console.Write("[{0:000}] ==> ", idx);
                        print(userList[idx]);
                    }
                    Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                    Console.WriteLine("Please, choose the index of the user which you want to enroll.");
                    Console.Write(">>>> ");

                    Int32 selection = Util.GetInput();
                    if (selection >= 0)
                    {
                        if (selection < userList.Count)
                        {
                            BS2User user = userList[selection];
                            BS2UserSmallBlobEx[] userBlob = Util.AllocateStructureArray<BS2UserSmallBlobEx>(1);
                            if (dbHandler.GetUserBlobEx(ref deviceInfo, ref user, ref userBlob[0]))
                            {
                                Console.WriteLine("Trying to enroll user.");
                                //BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrolUserEx(sdkContext, deviceID, userBlob, 1, 1);
                                BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrollUserSmallEx(sdkContext, deviceID, userBlob, 1, 1);
                                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                                {
                                    Console.WriteLine("Got error({0}).", result);
                                }

                                if (userBlob[0].cardObjs != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(userBlob[0].cardObjs);
                                }

                                if (userBlob[0].fingerObjs != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(userBlob[0].fingerObjs);
                                }

                                if (userBlob[0].faceObjs != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(userBlob[0].faceObjs);
                                }

                                if (userBlob[0].user_photo_obj != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(userBlob[0].user_photo_obj);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid selection[{0}]", selection);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid user index");
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


        //private IntPtr sdkContext;

        /*
        private ServerMatchingTask matchingTask;

        public void VerifyUser(UInt32 deviceId, UInt16 seq, byte isCard, byte cardType, IntPtr data, UInt32 dataLen)
        {
            if (Convert.ToBoolean(isCard))
            {
                BS2CSNCard csnCard = Util.AllocateStructure<BS2CSNCard>();
                csnCard.type = (byte)BS2CardTypeEnum.CSN;
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
            API.OnVerifyUser cbOnVerifyUser = new API.OnVerifyUser(VerifyUser);
            API.OnIdentifyUser cbOnIdentifyUser = new API.OnIdentifyUser(IdentifyUser);
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
            matchingTask = null;
        }
        */

        private int cbAcceptableUserID(string uid)
        {
            return 1;
        }

        public void listUserInfos(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = new API.IsAcceptableUserID(cbAcceptableUserID); // we don't need to user id filtering

            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine("Trying to get user list.");
            IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserListFromDir(sdkContext, ptrDir, out outUidObjs, out numUserIds, cbIsAcceptableUserID);
            Marshal.FreeHGlobal(ptrDir);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (numUserIds > 0)
                {
                    IntPtr curUidObjs = outUidObjs;
                    BS2UserSmallBlob[] userBlobs = new BS2UserSmallBlob[USER_PAGE_SIZE];
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        IntPtr ptrDir2 = Marshal.StringToHGlobalAnsi(strDir);
                        result = (BS2ErrorCode)API.BS2_GetUserSmallInfosFromDir(sdkContext, ptrDir2, curUidObjs, available, userBlobs);
                        Marshal.FreeHGlobal(ptrDir2);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(userBlobs[loop].user);
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                                if (userBlobs[loop].user_photo_obj != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].user_photo_obj);
                            }

                            idx += available;
                            curUidObjs += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            break;
                        }
                    }

                    API.BS2_ReleaseObject(outUidObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void listUserDatas(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = new API.IsAcceptableUserID(cbAcceptableUserID); // we don't need to user id filtering

            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine("Trying to get user list.");
            IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserListFromDir(sdkContext, ptrDir, out outUidObjs, out numUserIds, cbIsAcceptableUserID);
            Marshal.FreeHGlobal(ptrDir);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (numUserIds > 0)
                {
                    IntPtr curUidObjs = outUidObjs;
                    BS2UserSmallBlob[] userBlobs = new BS2UserSmallBlob[USER_PAGE_SIZE];
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        IntPtr ptrDir2 = Marshal.StringToHGlobalAnsi(strDir);
                        result = (BS2ErrorCode)API.BS2_GetUserSmallDatasFromDir(sdkContext, ptrDir2, curUidObjs, available, userBlobs, (UInt32)BS2UserMaskEnum.DATA | (UInt32)BS2UserMaskEnum.NAME);
                        Marshal.FreeHGlobal(ptrDir2);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(userBlobs[loop].user);
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                                if (userBlobs[loop].user_photo_obj != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].user_photo_obj);
                            }

                            idx += available;
                            curUidObjs += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            break;
                        }
                    }

                    API.BS2_ReleaseObject(outUidObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void listUserInfosEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = new API.IsAcceptableUserID(cbAcceptableUserID); // we don't need to user id filtering

            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine("Trying to get user list.");
            IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserListFromDir(sdkContext, ptrDir, out outUidObjs, out numUserIds, cbIsAcceptableUserID);
            Marshal.FreeHGlobal(ptrDir);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (numUserIds > 0)
                {
                    IntPtr curUidObjs = outUidObjs;
                    BS2UserSmallBlobEx[] userBlobs = new BS2UserSmallBlobEx[USER_PAGE_SIZE];
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        IntPtr ptrDir2 = Marshal.StringToHGlobalAnsi(strDir);
                        result = (BS2ErrorCode)API.BS2_GetUserSmallInfosExFromDir(sdkContext, ptrDir2, curUidObjs, available, userBlobs);
                        Marshal.FreeHGlobal(ptrDir2);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(userBlobs[loop].user);
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                                if (userBlobs[loop].user_photo_obj != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].user_photo_obj);
                            }

                            idx += available;
                            curUidObjs += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            break;
                        }
                    }

                    API.BS2_ReleaseObject(outUidObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void listUserDatasEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = new API.IsAcceptableUserID(cbAcceptableUserID); // we don't need to user id filtering

            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine("Trying to get user list.");
            IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserListFromDir(sdkContext, ptrDir, out outUidObjs, out numUserIds, cbIsAcceptableUserID);
            Marshal.FreeHGlobal(ptrDir);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (numUserIds > 0)
                {
                    IntPtr curUidObjs = outUidObjs;
                    BS2UserSmallBlobEx[] userBlobs = new BS2UserSmallBlobEx[USER_PAGE_SIZE];
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        IntPtr ptrDir2 = Marshal.StringToHGlobalAnsi(strDir);
                        result = (BS2ErrorCode)API.BS2_GetUserSmallDatasExFromDir(sdkContext, ptrDir2, curUidObjs, available, userBlobs, (UInt32)BS2UserMaskEnum.DATA | (UInt32)BS2UserMaskEnum.NAME);
                        Marshal.FreeHGlobal(ptrDir2);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(userBlobs[loop].user);
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                                if (userBlobs[loop].user_photo_obj != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].user_photo_obj);
                            }

                            idx += available;
                            curUidObjs += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            break;
                        }
                    }

                    API.BS2_ReleaseObject(outUidObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }


        public void setAuthOperatorLevelEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID for the User which you want to set");
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
                List<string> userIDs = new List<string>();
                userIDs.Add(userID);

                BS2AuthOperatorLevel item = Util.AllocateStructure<BS2AuthOperatorLevel>();
                int structSize = Marshal.SizeOf(typeof(BS2AuthOperatorLevel));
                IntPtr operatorlevelObj = Marshal.AllocHGlobal(structSize * userIDs.Count);
                IntPtr curOperatorlevelObj = operatorlevelObj;
                foreach (string strUserID in userIDs)
                {
                    byte[] userIDArray = Encoding.UTF8.GetBytes(strUserID);
                    Array.Clear(item.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                    Array.Copy(userIDArray, item.userID, userIDArray.Length);
                    item.level = (byte)BS2UserOperatorEnum.ADMIN;

                    Marshal.StructureToPtr(item, curOperatorlevelObj, false);
                    curOperatorlevelObj = (IntPtr)((long)curOperatorlevelObj + structSize);
                }

                Console.WriteLine("Trying to set auth operator level ex to device.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAuthOperatorLevelEx(sdkContext, deviceID, operatorlevelObj, (UInt32)userIDs.Count);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(operatorlevelObj);
            }
        }

        void print(ref BS2AuthOperatorLevel operatorLevel)
        {
            Console.WriteLine(">>>> Auth Operator Level userID[{0}] level[{1}]",
                                        Encoding.UTF8.GetString(operatorLevel.userID).TrimEnd('\0'),
                                        (BS2UserOperatorEnum)operatorLevel.level);
        }

        public void getAuthOperatorLevelEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID for the User which you want to get");
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
                List<string> userIDs = new List<string>();
                userIDs.Add(userID);

                int structSize = BS2Environment.BS2_USER_ID_SIZE;
                byte[] userIDBuf = new byte[structSize];
                IntPtr userIDObj = Marshal.AllocHGlobal(structSize * userIDs.Count);
                IntPtr curUserIDObj = userIDObj;
                foreach (string strUserID in userIDs)
                {
                    Array.Clear(userIDBuf, 0, userIDBuf.Length);
                    byte[] userIDArray = Encoding.UTF8.GetBytes(strUserID);
                    Array.Copy(userIDArray, userIDBuf, Math.Min(userIDArray.Length, userIDBuf.Length));
                    Marshal.Copy(userIDBuf, 0, curUserIDObj, userIDBuf.Length);
                    curUserIDObj = (IntPtr)((long)curUserIDObj + structSize);
                }

                IntPtr operatorlevelObj = IntPtr.Zero;
                UInt32 numOperatorlevel = 0;

                Console.WriteLine("Trying to get auth operator level ex from device.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthOperatorLevelEx(sdkContext, deviceID, userIDObj, (UInt32)userIDs.Count, out operatorlevelObj, out numOperatorlevel);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else if (numOperatorlevel > 0)
                {
                    IntPtr curOperatorLevelObj = operatorlevelObj;
                    structSize = Marshal.SizeOf(typeof(BS2AuthOperatorLevel));

                    for (int idx = 0; idx < numOperatorlevel; ++idx)
                    {
                        BS2AuthOperatorLevel item = (BS2AuthOperatorLevel)Marshal.PtrToStructure(curOperatorLevelObj, typeof(BS2AuthOperatorLevel));
                        print(ref item);
                        curOperatorLevelObj = (IntPtr)((long)curOperatorLevelObj + structSize);
                    }
                }
                else
                {
                    Console.WriteLine(">>> There is no auth operator level ex in the device.");
                }

                if (operatorlevelObj != IntPtr.Zero)
                    API.BS2_ReleaseObject(operatorlevelObj);

                Marshal.FreeHGlobal(userIDObj);
            }
        }

        public void getAllAuthOperatorLevelEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr operatorlevelObj = IntPtr.Zero;
            UInt32 numOperatorlevel = 0;

            Console.WriteLine("Trying to get all auth operator level ex from device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAllAuthOperatorLevelEx(sdkContext, deviceID, out operatorlevelObj, out numOperatorlevel);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numOperatorlevel > 0)
            {
                IntPtr curOperatorLevelObj = operatorlevelObj;
                int structSize = Marshal.SizeOf(typeof(BS2AuthOperatorLevel));

                for (int idx = 0; idx < numOperatorlevel; ++idx)
                {
                    BS2AuthOperatorLevel item = (BS2AuthOperatorLevel)Marshal.PtrToStructure(curOperatorLevelObj, typeof(BS2AuthOperatorLevel));
                    print(ref item);
                    curOperatorLevelObj = (IntPtr)((long)curOperatorLevelObj + structSize);
                }
            }
            else
            {
                Console.WriteLine(">>> There is no auth operator level ex in the device.");
            }

            if (operatorlevelObj != IntPtr.Zero)
                API.BS2_ReleaseObject(operatorlevelObj);
        }

        public void delAuthOperatorLevelEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID for the User which you want to remove");
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
                List<string> userIDs = new List<string>();
                userIDs.Add(userID);

                int structSize = BS2Environment.BS2_USER_ID_SIZE;
                byte[] userIDBuf = new byte[structSize];
                IntPtr userIDObj = Marshal.AllocHGlobal(structSize * userIDs.Count);
                IntPtr curUserIDObj = userIDObj;
                foreach (string strUserID in userIDs)
                {
                    Array.Clear(userIDBuf, 0, userIDBuf.Length);
                    byte[] userIDArray = Encoding.UTF8.GetBytes(strUserID);
                    Array.Copy(userIDArray, userIDBuf, Math.Min(userIDArray.Length, userIDBuf.Length));
                    Marshal.Copy(userIDBuf, 0, curUserIDObj, userIDBuf.Length);
                    curUserIDObj = (IntPtr)((long)curUserIDObj + structSize);
                }

                Console.WriteLine("Trying to remove auth operator level ex from device.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_RemoveAuthOperatorLevelEx(sdkContext, deviceID, userIDObj, (UInt32)userIDs.Count);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(userIDObj);
            }
        }

        public void delAllAuthOperatorLevelEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to remove all auth operator level ex from device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_RemoveAllAuthOperatorLevelEx(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void disbleSSL(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to disable ssl");

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_DisableSSL(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        private const UInt32 MAX_PERCENT = 40;
        private API.OnProgressChanged cbOnProgressChanged = null;

        void FirmwareProgressChanged(UInt32 deviceId, UInt32 progressPercentage)
        {
            UInt32 curProgress = progressPercentage * MAX_PERCENT / 100;

            Console.Write("\r>>>> [");
            for (UInt32 idx = 0; idx < curProgress; ++idx)
            {
                Console.Write("#");
            }

            for (UInt32 idx = curProgress; idx < MAX_PERCENT; ++idx)
            {
                Console.Write(" ");
            }

            Console.Write("] {0, 3}%", progressPercentage);

            if (progressPercentage < 99)
            {
                Console.Write(" Firmware downloading");
            }
            else if (progressPercentage == 99)
            {
                Console.Write(" Firmware flashing    ");
            }
            else
            {
                Console.WriteLine(" Done                                       ");
                Console.WriteLine(">>>> Your device[{0}] has been upgraded.", deviceId);
                Console.WriteLine(">>>> The device[{0}] will restart.", deviceId);
            }
        }

        public void upgradeFirmware(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the path of firmware file which you want to upgrade.");
            Console.Write(">>>> ");
            string firmwarePath = Console.ReadLine();

            if (!File.Exists(firmwarePath))
            {
                Console.WriteLine("Invalid firmware path");
                return;
            }

            IntPtr firmwareData = IntPtr.Zero;
            UInt32 firmwareDataLen = 0;

            if (Util.LoadBinary(firmwarePath, out firmwareData, out firmwareDataLen))
            {
                Console.WriteLine("Trying to upgrade firmware.");
                cbOnProgressChanged = new API.OnProgressChanged(FirmwareProgressChanged);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_UpgradeFirmware(sdkContext, deviceID, firmwareData, firmwareDataLen, 0, cbOnProgressChanged);
                Marshal.FreeHGlobal(firmwareData);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                cbOnProgressChanged = null;
            }
        }
    }
}
