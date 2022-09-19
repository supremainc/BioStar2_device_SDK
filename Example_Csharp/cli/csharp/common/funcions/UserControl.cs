using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Data.SQLite;

namespace Suprema
{
    using BS2_USER_MASK = UInt32;

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

        public bool GetUserBlobEx(ref BS2SimpleDeviceInfo deviceInfo, ref BS2CSNCard csnCard, ref BS2UserBlobEx userBlob)
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

        public bool GetUserBlobEx(ref BS2SimpleDeviceInfo deviceInfo, string userID, ref BS2UserBlobEx userBlob)
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

        public bool GetUserBlobEx(ref BS2SimpleDeviceInfo deviceInfo, ref BS2User targetUser, ref BS2UserBlobEx userBlob)
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

        public bool AddUserBlob(ref BS2UserBlob userBlob, BS2FingerprintTemplateFormatEnum templateFormat)
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

            if (userBlob.photo.size > 0)
            {
                cmd.CommandText = "INSERT INTO BS2UserPhoto (userID, size, data) VALUES (@userIDParam, @sizeParam, @dataParam)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                cmd.Parameters.AddWithValue("@sizeParam", userBlob.photo.size);
                cmd.Parameters.AddWithValue("@dataParam", userBlob.photo.data);

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

        public bool AddUserBlobEx(ref BS2UserBlobEx userBlob, BS2FingerprintTemplateFormatEnum templateFormat)
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

            if (userBlob.photo.size > 0)
            {
                cmd.CommandText = "INSERT INTO BS2UserPhoto (userID, size, data) VALUES (@userIDParam, @sizeParam, @dataParam)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userIDParam", userBlob.user.userID);
                cmd.Parameters.AddWithValue("@sizeParam", userBlob.photo.size);
                cmd.Parameters.AddWithValue("@dataParam", userBlob.photo.data);

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

    public class UserControl : FunctionModule
    {
        private const int USER_PAGE_SIZE = 1024;

        private API.OnReadyToScan cbCardOnReadyToScan = null;
        private API.OnReadyToScan cbFingerOnReadyToScan = null;
        private API.OnReadyToScan cbFaceOnReadyToScan = null;
        private API.OnUserPhrase cbOnUserPhrase = null;

        private DataBaseHandler dbHandler = new DataBaseHandler();
        private IntPtr sdkContext;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("List a user from database", listUserFromDatabase));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a user into database", insertUserIntoDatabase));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Delete a user from database", deleteUserFromDatabase));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("List a user from device", listUserFromDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a user into device", insertUserIntoDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Delete a user from device", deleteUserFromDevice));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get a userEx from device", getUserExFromDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a userEx into database", insertUserIntoDatabaseEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a userEx into device", insertUserIntoDeviceEx));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Insert a FaceEx user into device (directly)", insertFaceExUserDirectly));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get a FaceEx user from device (directly)", getFaceExUserDirectly));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get normalize FaceEx image from FSF2", getNormalizedImageFaceEx));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Partial update of user", partialUpdateFaceEx));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get supported User Mask", getUserMask));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user statistic", getUserStatistic));

            //[Admin 1000]
			functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Auth Operator Level Ex", getAuthOperatorLevelEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get All Auth Operator Level Ex", getAllAuthOperatorLevelEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Auth Operator Level Ex", setAuthOperatorLevelEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Del Auth Operator Level Ex", delAuthOperatorLevelEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Del All Auth Operator Level Ex", delAllAuthOperatorLevelEx));
			//<=

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Activate user phrase", activateUserPhrase));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Deactivate user phrase", deactivateUserPhrase));

            return functionList;
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

                    if (faceScanSupported)
                    {
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN);
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN);

                        privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_PIN);

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

                if (faceScanSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC);

                    privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_ONLY);

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
            else if (faceScanSupported)
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

            if (faceScanSupported)
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

            userBlob.photo.size = 0;
            Array.Clear(userBlob.photo.data, 0, BS2Environment.BS2_USER_PHOTO_SIZE);
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

                if(Util.LoadBinary(imagePath, out imageData, out imageDataLen))
                {
                    if(imageDataLen == 0)
                    {
                        Console.WriteLine("Empty image file");
                        return;
                    }
                    else if (imageDataLen > BS2Environment.BS2_USER_PHOTO_SIZE)
                    {
                        Console.WriteLine("The profile image should less than {0} bytes.", BS2Environment.BS2_USER_PHOTO_SIZE);
                        return;
                    }

                    userBlob.photo.size = imageDataLen;
                    Marshal.Copy(imageData, userBlob.photo.data, 0, (int)imageDataLen);
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
#if TEST_CODE
                    BS2CSNCard csnCard = Util.AllocateStructure<BS2CSNCard>();
                    UInt64 cardID = 0;
                    int structSize = Marshal.SizeOf(typeof(BS2CSNCard));
                    userBlob.cardObjs = Marshal.AllocHGlobal(structSize * userBlob.user.numCards);
                    IntPtr curCardObjs = userBlob.cardObjs;

                    csnCard.type = 1;
                    csnCard.size = 32;
                    
                    string bCard = "3138335243";
                    cardID = Convert.ToUInt64(bCard);
                    byte[] csnData = BitConverter.GetBytes(cardID);
                    Array.Reverse(csnData);
                    Array.Copy(csnData, 0, csnCard.data, BS2Environment.BS2_CARD_DATA_SIZE - csnData.Length, csnData.Length);

                    IntPtr csnObj = Marshal.AllocHGlobal(structSize);
                    Marshal.StructureToPtr(csnCard, csnObj, true);
                    byte[] cardData = new byte[structSize];
                    Marshal.Copy(csnObj, cardData, 0, structSize);
                    Marshal.FreeHGlobal(csnObj);

                    Marshal.Copy(cardData, 0, curCardObjs, structSize);
#endif
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
                                Int32 score = 0;
                                IntPtr templatePtr = Marshal.AllocHGlobal(fingerprint.data.Length);
                                Marshal.Copy(fingerprint.data, (int)(templateIndex * BS2Environment.BS2_FINGER_TEMPLATE_SIZE), templatePtr, (int)BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                                result = (BS2ErrorCode)API.BS2_GetFingerTemplateQuality(templatePtr, (uint)BS2Environment.BS2_FINGER_TEMPLATE_SIZE, out score);
                                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                                {
                                    Console.WriteLine("Got error({0})", result);
                                }
                                else
                                {
                                    string decision;
                                    if (80 < score)
                                        decision = "Best";
                                    else if (60 < score)
                                        decision = "Good";
                                    else if (40 < score)
                                        decision = "Normal";
                                    else if (20 < score)
                                        decision = "Bad";
                                    else // (0 ~ 20)
                                        decision = "Worst";

                                    Console.WriteLine("Template {0} quality ({1}) - {2}", templateIndex, score, decision);
                                }

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

            if (faceScanSupported)
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
                    BS2UserBlob[] userBlobs = new BS2UserBlob[USER_PAGE_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; )
                    {
                        UInt32 available = numUserIds - idx;
                        if(available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        result = (BS2ErrorCode)API.BS2_GetUserDatas(sdkContext, deviceID, curUidObjs, available, userBlobs, (UInt32)BS2UserMaskEnum.ALL);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                //print(sdkContext, userBlobs[loop].user);
                                print(userBlobs[loop]);
                                // don't need to release cardObj, FingerObj, FaceObj because we get only BS2User
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
                            }

                            idx += available;
                            curUidObjs += (int)available*BS2Environment.BS2_USER_ID_SIZE;
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
                    for (int idx = 0 ; idx < userList.Count ; ++idx)
                    {
                        Console.Write("[{0:000}] ==> ", idx);
                        print(sdkContext, userList[idx]);
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
                            BS2UserBlob[] userBlob = Util.AllocateStructureArray<BS2UserBlob>(1);
                            if (dbHandler.GetUserBlob(ref deviceInfo, ref user, ref userBlob[0]))
                            {
                                Console.WriteLine("Trying to enroll user.");
                                BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrolUser(sdkContext, deviceID, userBlob, 1, 1);
                                //BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrollUser(sdkContext, deviceID, userBlob, 1, 1);
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

        void print(IntPtr sdkContext, BS2User user)
        {
            Console.WriteLine(">>>> User id[{0}] numCards[{1}] numFingers[{2}] numFaces[{3}]", 
                                Encoding.UTF8.GetString(user.userID).TrimEnd('\0'), 
                                user.numCards,
                                user.numFingers,
                                user.numFaces);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Ex
        public void getUserExFromDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            byte[] tempUID = new byte[BS2Environment.BS2_USER_ID_SIZE];
            Array.Clear(tempUID, 0, BS2Environment.BS2_USER_ID_SIZE);
            IntPtr ptrUID = Marshal.AllocHGlobal(tempUID.Length);

            Console.WriteLine("Enter the user ID to get from device.");
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

            //TODO Alphabet user id is not implemented yet.
            UInt32 uid;
            if (!UInt32.TryParse(userID, out uid))
            {
                Console.WriteLine("The user id should be a numeric.");
                return;
            }

            byte[] userIDArray = Encoding.UTF8.GetBytes(userID);
            Array.Copy(userIDArray, tempUID, userIDArray.Length);
            Marshal.Copy(tempUID, 0, ptrUID, tempUID.Length);

            BS2UserBlobEx[] userBlobs = new BS2UserBlobEx[1];
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserDatasEx(sdkContext, deviceID, ptrUID, 1, userBlobs, (UInt32)BS2UserMaskEnum.ALL);

            Marshal.FreeHGlobal(ptrUID);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(sdkContext, userBlobs[0].user);

            // Remove a user from database
            Console.WriteLine("Trying to remove a user.");
            if (!dbHandler.RemoveUser(userID))
            {
                Console.WriteLine("Can not remove user from the database.");
                //return;
            }
            else
            {
                Console.WriteLine("Removed a user successfully from the database. {0}", userID);
            }

            Console.WriteLine("Trying to enroll user.");
            if (!dbHandler.AddUserBlobEx(ref userBlobs[0], BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA))
            {
                Console.WriteLine("Can not enroll user in the system.");
            }

            //Release
            if (userBlobs[0].cardObjs != IntPtr.Zero)
                API.BS2_ReleaseObject(userBlobs[0].cardObjs);
            if (userBlobs[0].fingerObjs != IntPtr.Zero)
                API.BS2_ReleaseObject(userBlobs[0].fingerObjs);
            if (userBlobs[0].faceObjs != IntPtr.Zero)
                API.BS2_ReleaseObject(userBlobs[0].faceObjs);
        }

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

                    if (faceScanSupported)
                    {
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN);
                        privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN);

                        privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_PIN);

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

                if (faceScanSupported)
                {
                    privateCardAuthMode.Add(BS2CardAuthModeEnum.CARD_BIOMETRIC);

                    privateFaceAuthMode.Add(BS2FaceAuthModeEnum.BIOMETRIC_ONLY);

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
            else if (faceScanSupported)
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

            BS2UserBlobEx userBlob = Util.AllocateStructure<BS2UserBlobEx>();
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

            if (faceScanSupported)
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

            userBlob.photo.size = 0;
            Array.Clear(userBlob.photo.data, 0, BS2Environment.BS2_USER_PHOTO_SIZE);
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

                    userBlob.photo.size = imageDataLen;
                    Marshal.Copy(imageData, userBlob.photo.data, 0, (int)imageDataLen);
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

            if (faceScanSupported)
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
                        print(sdkContext, userList[idx]);
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
                            BS2UserBlobEx[] userBlob = Util.AllocateStructureArray<BS2UserBlobEx>(1);
                            if (dbHandler.GetUserBlobEx(ref deviceInfo, ref user, ref userBlob[0]))
                            {
                                Console.WriteLine("Trying to enroll user.");
                                BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrolUserEx(sdkContext, deviceID, userBlob, 1, 1);
                                //BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrollUserEx(sdkContext, deviceID, userBlob, 1, 1);
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

        //[Admin 1000]	
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

                BS2AuthOperatorLevel item = Util.AllocateStructure< BS2AuthOperatorLevel>();
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
		//<=    

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Ex
        public void getFaceExUserDirectly(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            bool nameSupported = Convert.ToBoolean(deviceInfo.userNameSupported);
            bool photoSupported = Convert.ToBoolean(deviceInfo.userPhotoSupported);
            bool cardSupported = Convert.ToBoolean(deviceInfo.cardSupported);
	        bool fingerScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN;
	        bool faceExScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN;
            BS2_USER_MASK userMask = (UInt32)BS2UserMaskEnum.DATA | (UInt32)BS2UserMaskEnum.SETTING;

            IntPtr uid = IntPtr.Zero;
	        UInt32 numUser = 1;

	        Console.WriteLine("Please enter a user ID:");
	        Console.Write(">> ");
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
                UInt32 numID;
                if (!UInt32.TryParse(userID, out numID))
                {
                    Console.WriteLine("The user id should be a numeric.");
                    return;
                }

                uid = Marshal.AllocHGlobal(BS2Environment.BS2_USER_ID_SIZE);
                byte[] userIDArray = Util.StringToByte(BS2Environment.BS2_USER_ID_SIZE, userID);
                Marshal.Copy(userIDArray, 0, uid, userIDArray.Length);
            }

	        Console.WriteLine("Get a [1: User header, 2: Card, 3: Finger, 4: FaceEx]");
	        Console.Write(">> ");
	        ushort maskType = Util.GetInput(4);

	        switch (maskType)
	        {
	        case 1:
		        userMask |= (UInt32)BS2UserMaskEnum.ACCESS_GROUP;
		        if (nameSupported)
			        userMask |= (UInt32)BS2UserMaskEnum.NAME;
		        if (pinSupported)
			        userMask |= (UInt32)BS2UserMaskEnum.PIN;
		        if (photoSupported)
			        userMask |= (UInt32)BS2UserMaskEnum.PHOTO;
		        break;
	        case 2:
		        if (cardSupported)
			        userMask |= (UInt32)BS2UserMaskEnum.CARD;
		        break;
	        case 3:
		        if (fingerScanSupported)
			        userMask |= (UInt32)BS2UserMaskEnum.FINGER;
		        break;
	        case 4:
		        if (faceExScanSupported)
			        userMask |= ((UInt32)BS2UserMaskEnum.SETTING_EX | (UInt32)BS2UserMaskEnum.FACE_EX);
		        break;
	        default:
		        break;
	        }

            Console.WriteLine("Trying to get user list.");
            BS2UserFaceExBlob[] userBlobs = new BS2UserFaceExBlob[numUser];
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserDatasFaceEx(sdkContext, deviceID, uid, numUser, userBlobs, userMask);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                for (UInt32 index = 0; index < numUser; index++)
                {
                    print(userBlobs[index]);

                    if (userBlobs[index].cardObjs != IntPtr.Zero)
                        API.BS2_ReleaseObject(userBlobs[index].cardObjs);
                    if (userBlobs[index].fingerObjs != IntPtr.Zero)
                        API.BS2_ReleaseObject(userBlobs[index].fingerObjs);
                    if (userBlobs[index].faceObjs != IntPtr.Zero)
                        API.BS2_ReleaseObject(userBlobs[index].faceObjs);
                    if (userBlobs[index].faceExObjs != IntPtr.Zero)
                        API.BS2_ReleaseObject(userBlobs[index].faceExObjs);
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
            }

            if (uid != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(uid);
            }
        }

        public void getNormalizedImageFaceEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            bool faceExScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN;
            if (!faceExScanSupported)
            {
                Console.WriteLine("Not supported device");
                return;
            }

            Console.WriteLine("Enter the path and name of unwarped face image file.");
            Console.Write(">> ");
            string unwarpedPath = Console.ReadLine();
            if (unwarpedPath.Length == 0)
            {
                Console.WriteLine("The path and name can not be empty.");
                return;
            }

            byte[] readData = File.ReadAllBytes(unwarpedPath);
            if (readData.Length == 0)
            {
                Console.WriteLine("Invalid file. Check please. {0}", unwarpedPath);
                return;
            }

            IntPtr unwarpedImagePtr = Marshal.AllocHGlobal(readData.Length);
            Marshal.Copy(readData, 0, unwarpedImagePtr, readData.Length);
            UInt32 unwarpedImageLen = (UInt32)readData.Length;
            IntPtr warpedImagePtr = Marshal.AllocHGlobal(BS2Environment.BS2_MAX_WARPED_IMAGE_LENGTH);
            UInt32 warpedImageLen = 0;

            Console.WriteLine("Trying to get warped face image with unwarped.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetNormalizedImageFaceEx(sdkContext, deviceID, unwarpedImagePtr, unwarpedImageLen, warpedImagePtr, out warpedImageLen);
            if (BS2ErrorCode.BS_SDK_SUCCESS == result)
            {
                byte[] warpedBuffer = new byte[warpedImageLen];
                Array.Clear(warpedBuffer, 0, (int)warpedImageLen);
                Marshal.Copy(warpedImagePtr, warpedBuffer, 0, (int)warpedImageLen);
            
                Console.WriteLine("Enter the path and name of warped image file");
                Console.Write(">> ");
                string warpedPath = Console.ReadLine();
                if (warpedPath.Length == 0)
                {
                    Console.WriteLine("The path and name can not be empty.");
                    return;
                }

                File.WriteAllBytes(warpedPath, warpedBuffer);
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(unwarpedImagePtr);
            Marshal.FreeHGlobal(warpedImagePtr);
        }

        public void insertFaceExUserDirectly(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            bool nameSupported = Convert.ToBoolean(deviceInfo.userNameSupported);
            bool cardSupported = Convert.ToBoolean(deviceInfo.cardSupported);
	        bool fingerScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN;
	        bool faceScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN;
	        bool faceExScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN;

            BS2UserFaceExBlob[] userBlob = Util.AllocateStructureArray<BS2UserFaceExBlob>(1);
            userBlob[0].cardObjs = IntPtr.Zero;
            userBlob[0].fingerObjs = IntPtr.Zero;
            userBlob[0].faceObjs = IntPtr.Zero;
            userBlob[0].user_photo_obj = IntPtr.Zero;
            userBlob[0].faceExObjs = IntPtr.Zero;

            BS2ErrorCode sdkResult = BS2ErrorCode.BS_SDK_SUCCESS;

	        Console.WriteLine("Please enter a user ID:");
	        Console.Write(">> ");
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
                Array.Clear(userBlob[0].user.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(userIDArray, userBlob[0].user.userID, userIDArray.Length);
            }

            Array.Clear(userBlob[0].name, 0, BS2Environment.BS2_USER_NAME_LEN);
	        if (nameSupported)
	        {
		        Console.WriteLine("Enter your name:");
	            Console.Write(">> ");
		        string name = Console.ReadLine();
                if (name.Length > BS2Environment.BS2_USER_NAME_LEN)
                {
                    Console.WriteLine("The user name should less than {0} words.", BS2Environment.BS2_USER_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] nameArray = Encoding.UTF8.GetBytes(name);
                    Array.Copy(nameArray, userBlob[0].name, nameArray.Length);
                }
	        }

            Console.WriteLine("When is this user valid from? [default(Today), yyyy-MM-dd HH:mm:ss]");
            Console.Write(">> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out userBlob[0].setting.startTime))
            {
                return;
            }

            Console.WriteLine("When is this user valid to? [default(Today), yyyy-MM-dd HH:mm:ss]");
            Console.Write(">> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out userBlob[0].setting.endTime))
            {
                return;
            }

            Array.Clear(userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
	        if (pinSupported)
	        {
                Console.WriteLine("Do you want to encrypt the PIN code with a custom key and apply it? [y/n]");
                Console.Write(">> ");
                if (Util.IsNo())
                {
                    // Default
                    Console.WriteLine("Enter the PIN code:");
                    Console.Write(">> ");
                    string pinString = Console.ReadLine();
                    if (BS2Environment.BS2_PIN_HASH_SIZE < pinString.Length)
                    {
                        Console.WriteLine("PIN code is too long");
                        return;
                    }

                    IntPtr ptrChar = Marshal.StringToHGlobalAnsi(pinString);
                    IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    sdkResult = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, ptrChar, pinCode);
                    if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                    {
                        Console.WriteLine("BS2_MakePinCode call failed: {0}", sdkResult);
                        return;
                    }

                    Marshal.Copy(pinCode, userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    Marshal.FreeHGlobal(ptrChar);
                    Marshal.FreeHGlobal(pinCode);
                }
                else
                {
                    Console.WriteLine("Please enter the PIN encryption key.");
                    Console.WriteLine("You may have changed the key using the function BS2_SetDataEncryptKey.");
                    Console.Write(">> ");
                    string keyString = Console.ReadLine();
                    byte[] buff = Encoding.UTF8.GetBytes(keyString);

                    BS2EncryptKey keyInfo = Util.AllocateStructure<BS2EncryptKey>();
                    Array.Clear(keyInfo.key, 0, BS2Environment.BS2_ENC_KEY_SIZE);
                    Array.Copy(buff, 0, keyInfo.key, 0, keyString.Length);

                    Console.WriteLine("Enter the PIN code:");
                    Console.Write(">> ");
                    string pinString = Console.ReadLine();
                    if (BS2Environment.BS2_PIN_HASH_SIZE < pinString.Length)
                    {
                        Console.WriteLine("PIN code is too long");
                        return;
                    }

                    IntPtr ptrChar = Marshal.StringToHGlobalAnsi(pinString);
                    IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    sdkResult = (BS2ErrorCode)API.BS2_MakePinCodeWithKey(sdkContext, ptrChar, pinCode, ref keyInfo);
                    if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                    {
                        Console.WriteLine("BS2_MakePinCodeWithKey call failed: {0}", sdkResult);
                        return;
                    }

                    Marshal.Copy(pinCode, userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    Marshal.FreeHGlobal(ptrChar);
                    Marshal.FreeHGlobal(pinCode);
                }
	        }

            userBlob[0].setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.NONE;
            userBlob[0].setting.cardAuthMode = (byte)BS2CardAuthModeEnum.NONE;
            userBlob[0].setting.idAuthMode = (byte)BS2IDAuthModeEnum.NONE;

            Console.WriteLine("Do you want to register private auth mode? [y/n]");
            Console.Write(">> ");
            if (Util.IsYes())
	        {
		        if (fingerScanSupported || faceScanSupported)
		        {
			        Console.WriteLine("Enter the biometric authentication mode");
			        Console.WriteLine(" 0: Not use");
			        Console.WriteLine(" 1: Biometric only");
			        Console.WriteLine(" 2: Biometric+PIN");
                    Console.Write(">> ");
			        int fingerAuthMode = Util.GetInput(0);
			        switch (fingerAuthMode)
			        {
			        case 1:
				        userBlob[0].setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.BIOMETRIC_ONLY;
				        break;
			        case 2:
				        userBlob[0].setting.fingerAuthMode = pinSupported ? (byte)BS2FingerAuthModeEnum.BIOMETRIC_PIN : (byte)BS2FingerAuthModeEnum.BIOMETRIC_ONLY;
				        break;
			        default:
				        userBlob[0].setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.NONE;
				        break;
			        }
		        }

		        if (0 < deviceInfo.cardSupported)
		        {
			        Console.WriteLine("Enter the card authentication mode");
			        Console.WriteLine(" 0: Not use");
			        Console.WriteLine(" 1: Card only");
			        Console.WriteLine(" 2: Card+Biometric");
			        Console.WriteLine(" 3: Card+PIN");
			        Console.WriteLine(" 4: Card+(Biometric/PIN)");
			        Console.WriteLine(" 5: Card+Biometric+PIN");
                    Console.Write(">> ");
			        int cardAuthMode = Util.GetInput(0);
			        switch (cardAuthMode)
			        {
			        case 1:
				        userBlob[0].setting.cardAuthMode = (byte)BS2CardAuthModeEnum.CARD_ONLY;
				        break;
			        case 2:
				        userBlob[0].setting.cardAuthMode = (fingerScanSupported || faceScanSupported) ? (byte)BS2CardAuthModeEnum.CARD_BIOMETRIC : (byte)BS2CardAuthModeEnum.CARD_ONLY;
				        break;
			        case 3:
				        userBlob[0].setting.cardAuthMode = pinSupported ? (byte)BS2CardAuthModeEnum.CARD_PIN : (byte)BS2CardAuthModeEnum.CARD_ONLY;
				        break;
			        case 4:
				        userBlob[0].setting.cardAuthMode = (fingerScanSupported || faceScanSupported || pinSupported) ? (byte)BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN : (byte)BS2CardAuthModeEnum.CARD_ONLY;
				        break;
			        case 5:
				        userBlob[0].setting.cardAuthMode = (fingerScanSupported || faceScanSupported || pinSupported) ? (byte)BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN : (byte)BS2CardAuthModeEnum.CARD_ONLY;
				        break;
			        default:
				        userBlob[0].setting.cardAuthMode = (byte)BS2CardAuthModeEnum.NONE;
				        break;
			        }
		        }

		        {
			        Console.WriteLine("Enter the ID authentication mode");
			        Console.WriteLine(" 0: Not use");
			        Console.WriteLine(" 1: ID+Biometric");
			        Console.WriteLine(" 2: ID+PIN");
			        Console.WriteLine(" 3: ID+(Biometric/PIN)");
			        Console.WriteLine(" 4: ID+Biometric+PIN");
                    Console.Write(">> ");
			        int idAuthMode = Util.GetInput(0);
			        switch (idAuthMode)
			        {
			        case 1:
				        userBlob[0].setting.idAuthMode = (fingerScanSupported || faceScanSupported) ? (byte)BS2IDAuthModeEnum.ID_BIOMETRIC : (byte)BS2IDAuthModeEnum.NONE;
				        break;
			        case 2:
				        userBlob[0].setting.idAuthMode = pinSupported ? (byte)BS2IDAuthModeEnum.ID_PIN : (byte)BS2IDAuthModeEnum.NONE;
				        break;
			        case 3:
				        userBlob[0].setting.idAuthMode = (fingerScanSupported || faceScanSupported || pinSupported) ? (byte)BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN : (byte)BS2IDAuthModeEnum.NONE;
				        break;
			        case 4:
				        userBlob[0].setting.idAuthMode = (fingerScanSupported || faceScanSupported || pinSupported) ? (byte)BS2IDAuthModeEnum.ID_BIOMETRIC_PIN : (byte)BS2IDAuthModeEnum.NONE;
				        break;
			        default:
				        userBlob[0].setting.idAuthMode = (byte)BS2IDAuthModeEnum.NONE;
				        break;
			        }
		        }
	        }

            userBlob[0].settingEx.faceAuthMode = (byte)BS2ExtFaceAuthModeEnum.NONE;
            userBlob[0].settingEx.fingerprintAuthMode = (byte)BS2ExtFingerprintAuthModeEnum.NONE;
            userBlob[0].settingEx.cardAuthMode = (byte)BS2ExtCardAuthModeEnum.NONE;
            userBlob[0].settingEx.idAuthMode = (byte)BS2ExtIDAuthModeEnum.NONE;

            Console.WriteLine("Do you want to register private auth-ex mode? [y/n]");
            Console.Write(">> ");
            if (Util.IsYes())
	        {
		        if (faceExScanSupported)
		        {
			        Console.WriteLine("Enter the FaceEx authentication mode");
			        Console.WriteLine(" 0: Not use");
			        Console.WriteLine(" 1: Face");
			        Console.WriteLine(" 2: Face + Fingerprint");
			        Console.WriteLine(" 3: Face + PIN");
			        Console.WriteLine(" 4: Face + Fingerprint / PIN");
			        Console.WriteLine(" 5: Face + Fingerprint + PIN");
                    Console.Write(">> ");
			        int faceAuthMode = Util.GetInput(0);
			        switch (faceAuthMode)
			        {
			        case 1:
				        userBlob[0].settingEx.faceAuthMode = (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY;
				        break;
			        case 2:
				        userBlob[0].settingEx.faceAuthMode = fingerScanSupported ? (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT : (byte)BS2ExtFaceAuthModeEnum.NONE;
				        break;
			        case 3:
				        userBlob[0].settingEx.faceAuthMode = pinSupported ? (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_PIN : (byte)BS2ExtFaceAuthModeEnum.NONE;
				        break;
			        case 4:
				        userBlob[0].settingEx.faceAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT_OR_PIN : (byte)BS2ExtFaceAuthModeEnum.NONE;
				        break;
			        case 5:
				        userBlob[0].settingEx.faceAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT_PIN : (byte)BS2ExtFaceAuthModeEnum.NONE;
				        break;
			        default:
				        userBlob[0].settingEx.faceAuthMode = (byte)BS2ExtFaceAuthModeEnum.NONE;
				        break;
			        }
		        }

		        if (fingerScanSupported)
		        {
			        Console.WriteLine("Enter the Fingerprint authentication mode");;
			        Console.WriteLine(" 0: Not use");
			        Console.WriteLine(" 1: Fingerprint");
			        Console.WriteLine(" 2: Fingerprint + Face");
			        Console.WriteLine(" 3: Fingerprint + PIN");
			        Console.WriteLine(" 4: Fingerprint + Face/PIN");
			        Console.WriteLine(" 5: Fingerprint + Face + PIN");
                    Console.Write(">> ");
			        int fingerAuthMode = Util.GetInput(0);
			        switch (fingerAuthMode)
			        {
			        case 1:
				        userBlob[0].settingEx.fingerprintAuthMode = (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY;
				        break;
			        case 2:
				        userBlob[0].settingEx.fingerprintAuthMode = faceExScanSupported ? (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE : (byte)BS2ExtFingerprintAuthModeEnum.NONE;
				        break;
			        case 3:
				        userBlob[0].settingEx.fingerprintAuthMode = pinSupported ? (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_PIN : (byte)BS2ExtFingerprintAuthModeEnum.NONE;
				        break;
			        case 4:
				        userBlob[0].settingEx.fingerprintAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE_OR_PIN : (byte)BS2ExtFingerprintAuthModeEnum.NONE;
				        break;
			        case 5:
				        userBlob[0].settingEx.fingerprintAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE_PIN : (byte)BS2ExtFingerprintAuthModeEnum.NONE;
				        break;
			        default:
				        userBlob[0].settingEx.fingerprintAuthMode = (byte)BS2ExtFingerprintAuthModeEnum.NONE;
				        break;
			        }
		        }

		        if (cardSupported)
		        {
			        Console.WriteLine("Enter the Card authentication mode");
			        Console.WriteLine(" 0: Not use");
			        Console.WriteLine(" 1: Card");
			        Console.WriteLine(" 2: Card + Face");
			        Console.WriteLine(" 3: Card + Fingerprint");
			        Console.WriteLine(" 4: Card + PIN");
			        Console.WriteLine(" 5: Card + Face/Fingerprint");
			        Console.WriteLine(" 6: Card + Face/PIN");
			        Console.WriteLine(" 7: Card + Fingerprint/PIN");
			        Console.WriteLine(" 8: Card + Face/Fingerprint/PIN");
			        Console.WriteLine(" 9: Card + Face + Fingerprint");
			        Console.WriteLine("10: Card + Face + PIN");
			        Console.WriteLine("11: Card + Fingerprint + Face");
			        Console.WriteLine("12: Card + Fingerprint + PIN");
			        Console.WriteLine("13: Card + Face/Fingerprint + PIN");
			        Console.WriteLine("14: Card + Face + Fingerprint/PIN");
			        Console.WriteLine("15: Card + Fingerprint + Face/PIN");
                    Console.Write(">> ");
			        int cardAuthMode = Util.GetInput(0);
			        switch (cardAuthMode)
			        {
			        case 1:
				        userBlob[0].settingEx.cardAuthMode = (byte)BS2ExtCardAuthModeEnum.EXT_CARD_ONLY;
				        break;
			        case 2:
				        userBlob[0].settingEx.cardAuthMode = faceExScanSupported ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 3:
				        userBlob[0].settingEx.cardAuthMode = fingerScanSupported ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 4:
				        userBlob[0].settingEx.cardAuthMode = pinSupported ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 5:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 6:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 7:
				        userBlob[0].settingEx.cardAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 8:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 9:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_FINGERPRINT : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 10:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 11:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_FACE : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 12:
				        userBlob[0].settingEx.cardAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 13:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 14:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_FINGERPRINT_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        case 15:
				        userBlob[0].settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_FACE_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
				        break;
			        default:
				        userBlob[0].settingEx.cardAuthMode = (byte)BS2ExtCardAuthModeEnum.NONE;
                        break;
			        }
		        }	// cardAuthMode

		        {
			        Console.WriteLine("Enter the ID authentication mode");
			        Console.WriteLine(" 0: Not use");
			        Console.WriteLine(" 1: ID + Face");
			        Console.WriteLine(" 2: ID + Fingerprint");
			        Console.WriteLine(" 3: ID + PIN");
			        Console.WriteLine(" 4: ID + Face/Fingerprint");
			        Console.WriteLine(" 5: ID + Face/PIN");
			        Console.WriteLine(" 6: ID + Fingerprint/PIN");
			        Console.WriteLine(" 7: ID + Face/Fingerprint/PIN");
			        Console.WriteLine(" 8: ID + Face + Fingerprint");
			        Console.WriteLine(" 9: ID + Face + PIN");
			        Console.WriteLine("10: ID + Fingerprint + Face");
			        Console.WriteLine("11: ID + Fingerprint + PIN");
			        Console.WriteLine("12: ID + Face/Fingerprint + PIN");
			        Console.WriteLine("13: ID + Face + Fingerprint/PIN");
			        Console.WriteLine("14: ID + Fingerprint + Face/PIN");
                    Console.Write(">> ");
			        int idAuthMode = Util.GetInput(0);
			        switch (idAuthMode)
			        {
			        case 1:
				        userBlob[0].settingEx.idAuthMode = faceExScanSupported ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 2:
				        userBlob[0].settingEx.idAuthMode = fingerScanSupported ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 3:
				        userBlob[0].settingEx.idAuthMode = pinSupported ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 4:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 5:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 6:
				        userBlob[0].settingEx.idAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 7:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 8:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_FINGERPRINT : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 9:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 10:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_FACE : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 11:
				        userBlob[0].settingEx.idAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 12:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 13:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_FINGERPRINT_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        case 14:
				        userBlob[0].settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_FACE_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        default:
				        userBlob[0].settingEx.idAuthMode = (byte)BS2ExtIDAuthModeEnum.NONE;
				        break;
			        }
		        }	// idAuthMode
	        }

	        {
		        Console.WriteLine("Enter the security level for this user");
		        Console.WriteLine("[0: Default, 1: Lower, 2: Low, 3: Normal, 4: High, 5, Higher]");
                Console.Write(">> ");
                userBlob[0].setting.securityLevel = Util.GetInput((byte)BS2UserSecurityLevelEnum.DEFAULT);
	        }

            Array.Clear(userBlob[0].accessGroupId, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER);
            Console.WriteLine("Do you want to register access group ID? [y/n]");
            Console.Write(">> ");
            if (Util.IsYes())
	        {
		        Console.WriteLine("Please enter access group IDs. ex)ID1 ID2 ID3 ...");
                Console.Write(">> ");
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
                            userBlob[0].accessGroupId[accessGroupIdIndex++] = item;
                        }
                    }
                }
	        }

	        {
		        Console.WriteLine("Please enter a authentication group ID.");
		        Console.WriteLine("This is used for face authentication. [0: Not using]");
                Console.Write(">> ");
		        userBlob[0].user.authGroupID = (UInt32)Util.GetInput(0);
	        }

	        {
		        Console.WriteLine("Do you want to overwrite the user if it exist? [y/n]");
                Console.Write(">> ");
                userBlob[0].user.flag = (byte)(Util.IsYes() ? BS2UserFlagEnum.CREATED | BS2UserFlagEnum.UPDATED : BS2UserFlagEnum.CREATED);
	        }

            userBlob[0].user.numCards = 0;
            userBlob[0].user.numFingers = 0;
            userBlob[0].user.numFaces = 0;

	        if (cardSupported)
	        {
		        Console.WriteLine("Do you want to scan card? [y/n]");
                Console.Write(">> ");
		        if (Util.IsYes())
		        {
			        Console.WriteLine("How many cards would you like to register?");
                    Console.Write(">> ");
                    int numOfCard = Util.GetInput(1);
                    if (0 < numOfCard)
                    {
                        int structSize = Marshal.SizeOf(typeof(BS2CSNCard));
                        BS2Card card = Util.AllocateStructure<BS2Card>();
			            userBlob[0].cardObjs = Marshal.AllocHGlobal(structSize + numOfCard);
                        IntPtr curCardObjs = userBlob[0].cardObjs;
                        cbCardOnReadyToScan = new API.OnReadyToScan(ReadyToScanForCard);

				        for (int index = 0; index < numOfCard;)
				        {
					        sdkResult = (BS2ErrorCode)API.BS2_ScanCard(sdkContext, deviceID, out card, cbCardOnReadyToScan);
					        if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
						        Console.WriteLine("BS2_ScanCard call failed: %d", sdkResult);
					        else
					        {
						        if (Convert.ToBoolean(card.isSmartCard))
						        {
							        Console.WriteLine("CSN card only supported.");
						        }
						        else
						        {
							        Marshal.Copy(card.cardUnion, 0, curCardObjs, structSize);
                                    curCardObjs += structSize;
                                    userBlob[0].user.numCards++;
						            index++;
						        }
					        }
				        }
                        cbCardOnReadyToScan = null;
			        }
		        }
	        }

	        if (fingerScanSupported)
	        {
		        Console.WriteLine("Do you want to scan fingerprint? [y/n]");
                Console.Write(">> ");
		        if (Util.IsYes())
		        {
			        Console.WriteLine("How many fingers would you like to register?");
                    Console.Write(">> ");
                    int numOfFinger = Util.GetInput(1);
			        if (0 < numOfFinger)
                    {
                        int structSize = Marshal.SizeOf(typeof(BS2Fingerprint));
			            BS2Fingerprint fingerprint = Util.AllocateStructure<BS2Fingerprint>();
			            userBlob[0].fingerObjs = Marshal.AllocHGlobal(structSize * numOfFinger);
                        IntPtr curFingerObjs = userBlob[0].fingerObjs;
                        cbFingerOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFinger);

				        for (int index = 0; index < numOfFinger; index++)
				        {
					        for (UInt32 templateIndex = 0; templateIndex < BS2Environment.BS2_TEMPLATE_PER_FINGER;)
					        {
						        sdkResult = (BS2ErrorCode)API.BS2_ScanFingerprint(sdkContext, deviceID, ref fingerprint, templateIndex, (UInt32)BS2FingerprintQualityEnum.QUALITY_HIGHEST, (byte)BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA, cbFingerOnReadyToScan);
						        if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
							        Console.WriteLine("BS2_ScanFingerprint call failed: %d", sdkResult);
						        else
							        templateIndex++;
					        }

                            userBlob[0].user.numFingers++;
                            fingerprint.index = (byte)index;

                            Marshal.StructureToPtr(fingerprint, curFingerObjs, false);
                            curFingerObjs += structSize;
				        }

                        cbFingerOnReadyToScan = null;
			        }
		        }
	        }

            bool unwarpedMemory = false;
	        if (faceScanSupported)
	        {
			    Console.WriteLine("Do you want to scan face? [y/n]");
                Console.Write(">> ");
		        if (Util.IsYes())
                {
                    Console.WriteLine("How many face would you like to register?");
                    Console.Write(">> ");
                    int numOfFace = Util.GetInput(1);
                    if (0 < numOfFace)
                    {
                        int structSize = Marshal.SizeOf(typeof(BS2Face));
                        BS2Face[] face = Util.AllocateStructureArray<BS2Face>(1);
                        userBlob[0].faceObjs = Marshal.AllocHGlobal(structSize * numOfFace);
			            IntPtr curFaceObjs = userBlob[0].faceObjs;
			            cbFaceOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFace);

                        for (int index = 0; index < numOfFace;)
				        {
					        sdkResult = (BS2ErrorCode)API.BS2_ScanFace(sdkContext, deviceID, face, (byte)BS2FaceEnrollThreshold.THRESHOLD_DEFAULT, cbFaceOnReadyToScan);
					        if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
						        Console.WriteLine("BS2_ScanFace call failed: %d", sdkResult);
					        else
					        {
                                userBlob[0].user.numFaces++;
						        index++;
                                face[0].faceIndex = (byte)index;
                                Marshal.StructureToPtr(face[0], curFaceObjs, false);
                                curFaceObjs += structSize;

                                Thread.Sleep(100);
					        }
				        }

                        cbFaceOnReadyToScan = null;
			        }
		        }
	        }
	        else if (faceExScanSupported)
	        {
		        Console.WriteLine("Do you want to scan faceEx? [y/n]");
		        Console.Write(">> ");
                if (Util.IsYes())
		        {
			        Console.WriteLine("How many faceEx would you like to register?");
		            Console.Write(">> ");
			        int numOfFace = Util.GetInput(1);
			        if (0 < numOfFace)
			        {
                        int structSize = Marshal.SizeOf(typeof(BS2FaceExWarped));
                        BS2FaceExWarped[] faceEx = Util.AllocateStructureArray<BS2FaceExWarped>(1);
				        userBlob[0].faceExObjs = Marshal.AllocHGlobal(structSize * numOfFace);
                        IntPtr curFaceExObjs = userBlob[0].faceExObjs;
                        cbFaceOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFace);

				        for (int index = 0; index < numOfFace;)
				        {
					        sdkResult = (BS2ErrorCode)API.BS2_ScanFaceEx(sdkContext, deviceID, faceEx, (byte)BS2FaceEnrollThreshold.THRESHOLD_DEFAULT, cbFaceOnReadyToScan);
					        if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
						        Console.WriteLine("BS2_ScanFaceEx call failed: %d", sdkResult);
					        else
					        {
                                Console.WriteLine("Do you want to save warpped image? [y/n]");
                                Console.Write(">> ");
                                if (Util.IsYes())
                                {
                                    int written = 0;
                                    int size = (int)faceEx[index].imageLen;
                                    IntPtr imgPtr = Marshal.AllocHGlobal(size);
                                    Marshal.Copy(faceEx[index].imageData, 0, imgPtr, size);
                                    FileStream file = new FileStream(String.Format("{0}-{1}.jpg", userID, index), FileMode.Create, FileAccess.Write);
                                    WriteFile(file.Handle, imgPtr, (int)faceEx[index].imageLen, out written, IntPtr.Zero);
                                    file.Close();
                                    Marshal.FreeHGlobal(imgPtr);
                                }
                                userBlob[0].user.numFaces++;
						        index++;
                                faceEx[0].faceIndex = (byte)index;
                                Marshal.StructureToPtr(faceEx[0], curFaceExObjs, false);
                                curFaceExObjs += structSize;

                                Thread.Sleep(100);
					        }
				        }

                        cbFaceOnReadyToScan = null;
			        }
		        }
		        else
		        {
			        Console.WriteLine("Do you want to register from image? [y/n]");
			        Console.Write(">> ");
			        if (Util.IsYes())
			        {
				        Console.WriteLine("Enter the face image path and name:");
			            Console.Write(">> ");
				        string imagePath = Console.ReadLine();

                        if (!File.Exists(imagePath))
                        {
                            Console.WriteLine("Invalid file path");
                            return;
                        }

                        Image faceImage = Image.FromFile(imagePath);
                        if (!faceImage.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        {
                            Console.WriteLine("Invalid image file format");
                            return;
                        }
                        
                        IntPtr imageData = IntPtr.Zero;
				        UInt32 imageLen = 0;
                        if (Util.LoadBinary(imagePath, out imageData, out imageLen))
                        {
                            if (0 == imageLen)
                            {
                                Console.WriteLine("Empty image file");
                                return;
                            }

                            int structHeaderSize = Marshal.SizeOf(typeof(BS2FaceExUnwarped));
                            int totalSize = structHeaderSize + (int)imageLen;
                            userBlob[0].faceExObjs = Marshal.AllocHGlobal(totalSize);
                            IntPtr curFaceExObjs = userBlob[0].faceExObjs;

                            BS2FaceExUnwarped unwarped = Util.AllocateStructure<BS2FaceExUnwarped>();
                            unwarped.flag = 0;
                            unwarped.imageLen = imageLen;

				            Marshal.StructureToPtr(unwarped, curFaceExObjs, false);
				            curFaceExObjs += structHeaderSize;

                            Util.CopyMemory(curFaceExObjs, imageData, imageLen);

                            userBlob[0].user.numFaces = 1;
                            unwarpedMemory = true;
				        }
			        }
		        }
	        }

	        sdkResult = (BS2ErrorCode)API.BS2_EnrollUserFaceEx(sdkContext, deviceID, userBlob, 1, 1);
	        if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
		        Console.WriteLine("BS2_EnrollUserFaceEx call failed {0}", sdkResult);

	        if (userBlob[0].cardObjs != IntPtr.Zero)
		        Marshal.FreeHGlobal(userBlob[0].cardObjs);

	        if (userBlob[0].fingerObjs != IntPtr.Zero)
		        Marshal.FreeHGlobal(userBlob[0].fingerObjs);

	        if (userBlob[0].faceObjs != IntPtr.Zero)
		        Marshal.FreeHGlobal(userBlob[0].faceObjs);

	        if (userBlob[0].faceExObjs != IntPtr.Zero)
	        {
                //if (unwarpedMemory)
                    Marshal.FreeHGlobal(userBlob[0].faceExObjs);
	        }
        }

        BS2_USER_MASK getMaskForUpdateUser()
        {
            List<BS2_USER_MASK> userUpdateItems = new List<BS2_USER_MASK>();
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.SETTING);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.SETTING_EX);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.NAME);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.PHOTO);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.PIN);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.ACCESS_GROUP);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.JOB);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.PHRASE);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.CARD);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.FINGER);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.FACE);
            userUpdateItems.Add((BS2_USER_MASK)BS2UserMaskEnum.FACE_EX);

	        Console.WriteLine("Select all of your items to update.  Ex)0, 1, 2, ...");
	        Console.WriteLine("  0: Setting");
	        Console.WriteLine("  1: SettingEx");
	        Console.WriteLine("  2: Name");
	        Console.WriteLine("  3: Photo");
	        Console.WriteLine("  4: PIN");
	        Console.WriteLine("  5: Access group");
	        Console.WriteLine("  6: Job");
	        Console.WriteLine("  7: Phrase");
	        Console.WriteLine("  8: Card");
	        Console.WriteLine("  9: Finger");
	        Console.WriteLine(" 10: Face");
	        Console.WriteLine(" 11: FaceEx");
            Console.Write(">> ");
            
            BS2_USER_MASK mask = 0;
            char[] delim = {' ', ',', ':', '.', '\t'};
            string[] selectedStrs = Console.ReadLine().Split(delim);
            foreach (string str in selectedStrs)
            {
                if (0 < str.Length)
                {
                    int selected;
                    if (int.TryParse(str, out selected))
                        mask |= userUpdateItems[selected];
                }
            }

            return mask;
        }

        BS2ErrorCode getUserBlobUserID(ref BS2User user, out string userID)
        {
            Console.WriteLine("Please enter a user ID:");
            Console.Write(">> ");
            userID = Console.ReadLine();
            if (userID.Length == 0)
            {
                Console.WriteLine("The user id can not be empty.");
                return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
            }
            else if (userID.Length > BS2Environment.BS2_USER_ID_SIZE)
            {
                Console.WriteLine("The user id should less than {0} words.", BS2Environment.BS2_USER_ID_SIZE);
                return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
            }
            else
            {
                //TODO Alphabet user id is not implemented yet.
                UInt32 uid;
                if (!UInt32.TryParse(userID, out uid))
                {
                    Console.WriteLine("The user id should be a numeric.");
                    return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
                }

                byte[] userIDArray = Encoding.UTF8.GetBytes(userID);
                Array.Clear(user.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(userIDArray, user.userID, userIDArray.Length);
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobExpiryDate(ref BS2UserSetting setting)
        {
            Console.WriteLine("When is this user valid from? [default(Today), yyyy-MM-dd HH:mm:ss]");
            Console.Write(">> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out setting.startTime))
            {
                return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
            }

            Console.WriteLine("When is this user valid to? [default(Today), yyyy-MM-dd HH:mm:ss]");
            Console.Write(">> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out setting.endTime))
            {
                return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobPrivateAuthMode(ref BS2UserSetting setting)
        {
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            bool cardSupported = Convert.ToBoolean(deviceInfo.cardSupported);
            bool fingerScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN;
            bool faceScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN;
            bool faceExScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN;

            Console.WriteLine("Do you want to register private auth mode? [Y/n]");
            Console.Write(">> ");
            if (Util.IsYes())
            {
                if (fingerScanSupported || faceScanSupported)
                {
                    Console.WriteLine("Enter the biometric authentication mode");
                    Console.WriteLine(" 0: Not use");
                    Console.WriteLine(" 1: Biometric only");
                    Console.WriteLine(" 2: Biometric+PIN");
                    Console.Write(">> ");
                    int fingerAuthMode = Util.GetInput(0);
                    switch (fingerAuthMode)
                    {
                        case 1:
                            setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.BIOMETRIC_ONLY;
                            break;
                        case 2:
                            setting.fingerAuthMode = pinSupported ? (byte)BS2FingerAuthModeEnum.BIOMETRIC_PIN : (byte)BS2FingerAuthModeEnum.BIOMETRIC_ONLY;
                            break;
                        default:
                            setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.NONE;
                            break;
                    }
                }

                if (cardSupported)
                {
                    Console.WriteLine("Enter the card authentication mode");
                    Console.WriteLine(" 0: Not use");
                    Console.WriteLine(" 1: Card only");
                    Console.WriteLine(" 2: Card+Biometric");
                    Console.WriteLine(" 3: Card+PIN");
                    Console.WriteLine(" 4: Card+(Biometric/PIN)");
                    Console.WriteLine(" 5: Card+Biometric+PIN");
                    Console.Write(">> ");
                    int cardAuthMode = Util.GetInput(0);
                    switch (cardAuthMode)
                    {
                        case 1:
                            setting.cardAuthMode = (byte)BS2CardAuthModeEnum.CARD_ONLY;
                            break;
                        case 2:
                            setting.cardAuthMode = (fingerScanSupported || faceScanSupported) ? (byte)BS2CardAuthModeEnum.CARD_BIOMETRIC : (byte)BS2CardAuthModeEnum.CARD_ONLY;
                            break;
                        case 3:
                            setting.cardAuthMode = pinSupported ? (byte)BS2CardAuthModeEnum.CARD_PIN : (byte)BS2CardAuthModeEnum.CARD_ONLY;
                            break;
                        case 4:
                            setting.cardAuthMode = (fingerScanSupported || faceScanSupported || pinSupported) ? (byte)BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN : (byte)BS2CardAuthModeEnum.CARD_ONLY;
                            break;
                        case 5:
                            setting.cardAuthMode = (fingerScanSupported || faceScanSupported || pinSupported) ? (byte)BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN : (byte)BS2CardAuthModeEnum.CARD_ONLY;
                            break;
                        default:
                            setting.cardAuthMode = (byte)BS2CardAuthModeEnum.NONE;
                            break;
                    }
                }

                {
                    Console.WriteLine("Enter the ID authentication mode");
                    Console.WriteLine(" 0: Not use");
                    Console.WriteLine(" 1: ID+Biometric");
                    Console.WriteLine(" 2: ID+PIN");
                    Console.WriteLine(" 3: ID+(Biometric/PIN)");
                    Console.WriteLine(" 4: ID+Biometric+PIN");
                    Console.Write(">> ");
                    int idAuthMode = Util.GetInput(0);
                    switch (idAuthMode)
                    {
                        case 1:
                            setting.idAuthMode = (fingerScanSupported || faceScanSupported) ? (byte)BS2IDAuthModeEnum.ID_BIOMETRIC : (byte)BS2IDAuthModeEnum.NONE;
                            break;
                        case 2:
                            setting.idAuthMode = pinSupported ? (byte)BS2IDAuthModeEnum.ID_PIN : (byte)BS2IDAuthModeEnum.NONE;
                            break;
                        case 3:
                            setting.idAuthMode = (fingerScanSupported || faceScanSupported || pinSupported) ? (byte)BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN : (byte)BS2IDAuthModeEnum.NONE;
                            break;
                        case 4:
                            setting.idAuthMode = (fingerScanSupported || faceScanSupported || pinSupported) ? (byte)BS2IDAuthModeEnum.ID_BIOMETRIC_PIN : (byte)BS2IDAuthModeEnum.NONE;
                            break;
                        default:
                            setting.idAuthMode = (byte)BS2IDAuthModeEnum.NONE;
                            break;
                    }
                }
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobSecurityLevel(ref BS2UserSetting setting)
        {
            Console.WriteLine("Enter the security level for this user");
            Console.WriteLine("[0: Default, 1: Lower, 2: Low, 3: Normal, 4: High, 5, Higher]");
            Console.Write(">> ");
            setting.securityLevel = Util.GetInput((byte)BS2UserSecurityLevelEnum.DEFAULT);

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobPrivateAuthModeEx(ref BS2UserSettingEx settingEx)
        {
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            bool cardSupported = Convert.ToBoolean(deviceInfo.cardSupported);
            bool fingerScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN;
            bool faceScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN;
            bool faceExScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN;

            Console.WriteLine("Do you want to register private auth-ex mode? [Y/n]");
            Console.Write(">> ");
            if (Util.IsYes())
            {
                if (faceExScanSupported)
                {
                    Console.WriteLine("Enter the FaceEx authentication mode");
                    Console.WriteLine(" 0: Not use");
                    Console.WriteLine(" 1: Face");
                    Console.WriteLine(" 2: Face + Fingerprint");
                    Console.WriteLine(" 3: Face + PIN");
                    Console.WriteLine(" 4: Face + Fingerprint / PIN");
                    Console.WriteLine(" 5: Face + Fingerprint + PIN");
                    Console.Write(">> ");
                    int faceAuthMode = Util.GetInput(0);
                    switch (faceAuthMode)
                    {
                        case 1:
                            settingEx.faceAuthMode = (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY;
                            break;
                        case 2:
                            settingEx.faceAuthMode = fingerScanSupported ? (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT : (byte)BS2ExtFaceAuthModeEnum.NONE;
                            break;
                        case 3:
                            settingEx.faceAuthMode = pinSupported ? (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_PIN : (byte)BS2ExtFaceAuthModeEnum.NONE;
                            break;
                        case 4:
                            settingEx.faceAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT_OR_PIN : (byte)BS2ExtFaceAuthModeEnum.NONE;
                            break;
                        case 5:
                            settingEx.faceAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT_PIN : (byte)BS2ExtFaceAuthModeEnum.NONE;
                            break;
                        default:
                            settingEx.faceAuthMode = (byte)BS2ExtFaceAuthModeEnum.NONE;
                            break;
                    }
                }

                if (fingerScanSupported)
                {
                    Console.WriteLine("Enter the Fingerprint authentication mode"); ;
                    Console.WriteLine(" 0: Not use");
                    Console.WriteLine(" 1: Fingerprint");
                    Console.WriteLine(" 2: Fingerprint + Face");
                    Console.WriteLine(" 3: Fingerprint + PIN");
                    Console.WriteLine(" 4: Fingerprint + Face/PIN");
                    Console.WriteLine(" 5: Fingerprint + Face + PIN");
                    Console.Write(">> ");
                    int fingerAuthMode = Util.GetInput(0);
                    switch (fingerAuthMode)
                    {
                        case 1:
                            settingEx.fingerprintAuthMode = (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY;
                            break;
                        case 2:
                            settingEx.fingerprintAuthMode = faceExScanSupported ? (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE : (byte)BS2ExtFingerprintAuthModeEnum.NONE;
                            break;
                        case 3:
                            settingEx.fingerprintAuthMode = pinSupported ? (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_PIN : (byte)BS2ExtFingerprintAuthModeEnum.NONE;
                            break;
                        case 4:
                            settingEx.fingerprintAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE_OR_PIN : (byte)BS2ExtFingerprintAuthModeEnum.NONE;
                            break;
                        case 5:
                            settingEx.fingerprintAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE_PIN : (byte)BS2ExtFingerprintAuthModeEnum.NONE;
                            break;
                        default:
                            settingEx.fingerprintAuthMode = (byte)BS2ExtFingerprintAuthModeEnum.NONE;
                            break;
                    }
                }

                if (cardSupported)
                {
                    Console.WriteLine("Enter the Card authentication mode");
                    Console.WriteLine(" 0: Not use");
                    Console.WriteLine(" 1: Card");
                    Console.WriteLine(" 2: Card + Face");
                    Console.WriteLine(" 3: Card + Fingerprint");
                    Console.WriteLine(" 4: Card + PIN");
                    Console.WriteLine(" 5: Card + Face/Fingerprint");
                    Console.WriteLine(" 6: Card + Face/PIN");
                    Console.WriteLine(" 7: Card + Fingerprint/PIN");
                    Console.WriteLine(" 8: Card + Face/Fingerprint/PIN");
                    Console.WriteLine(" 9: Card + Face + Fingerprint");
                    Console.WriteLine("10: Card + Face + PIN");
                    Console.WriteLine("11: Card + Fingerprint + Face");
                    Console.WriteLine("12: Card + Fingerprint + PIN");
                    Console.WriteLine("13: Card + Face/Fingerprint + PIN");
                    Console.WriteLine("14: Card + Face + Fingerprint/PIN");
                    Console.WriteLine("15: Card + Fingerprint + Face/PIN");
                    Console.Write(">> ");
                    int cardAuthMode = Util.GetInput(0);
                    switch (cardAuthMode)
                    {
                        case 1:
                            settingEx.cardAuthMode = (byte)BS2ExtCardAuthModeEnum.EXT_CARD_ONLY;
                            break;
                        case 2:
                            settingEx.cardAuthMode = faceExScanSupported ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 3:
                            settingEx.cardAuthMode = fingerScanSupported ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 4:
                            settingEx.cardAuthMode = pinSupported ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 5:
                            settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 6:
                            settingEx.cardAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 7:
                            settingEx.cardAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 8:
                            settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 9:
                            settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_FINGERPRINT : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 10:
                            settingEx.cardAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 11:
                            settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_FACE : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 12:
                            settingEx.cardAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 13:
                            settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 14:
                            settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_FINGERPRINT_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        case 15:
                            settingEx.cardAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_FACE_OR_PIN : (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                        default:
                            settingEx.cardAuthMode = (byte)BS2ExtCardAuthModeEnum.NONE;
                            break;
                    }
                }	// cardAuthMode

                {
                    Console.WriteLine("Enter the ID authentication mode");
                    Console.WriteLine(" 0: Not use");
                    Console.WriteLine(" 1: ID + Face");
                    Console.WriteLine(" 2: ID + Fingerprint");
                    Console.WriteLine(" 3: ID + PIN");
                    Console.WriteLine(" 4: ID + Face/Fingerprint");
                    Console.WriteLine(" 5: ID + Face/PIN");
                    Console.WriteLine(" 6: ID + Fingerprint/PIN");
                    Console.WriteLine(" 7: ID + Face/Fingerprint/PIN");
                    Console.WriteLine(" 8: ID + Face + Fingerprint");
                    Console.WriteLine(" 9: ID + Face + PIN");
                    Console.WriteLine("10: ID + Fingerprint + Face");
                    Console.WriteLine("11: ID + Fingerprint + PIN");
                    Console.WriteLine("12: ID + Face/Fingerprint + PIN");
                    Console.WriteLine("13: ID + Face + Fingerprint/PIN");
                    Console.WriteLine("14: ID + Fingerprint + Face/PIN");
                    Console.Write(">> ");
                    int idAuthMode = Util.GetInput(0);
                    switch (idAuthMode)
                    {
                        case 1:
                            settingEx.idAuthMode = faceExScanSupported ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 2:
                            settingEx.idAuthMode = fingerScanSupported ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 3:
                            settingEx.idAuthMode = pinSupported ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 4:
                            settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 5:
                            settingEx.idAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 6:
                            settingEx.idAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 7:
                            settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 8:
                            settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_FINGERPRINT : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 9:
                            settingEx.idAuthMode = (faceExScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 10:
                            settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_FACE : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 11:
                            settingEx.idAuthMode = (fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 12:
                            settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 13:
                            settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FACE_FINGERPRINT_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        case 14:
                            settingEx.idAuthMode = (faceExScanSupported && fingerScanSupported && pinSupported) ? (byte)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_FACE_OR_PIN : (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                        default:
                            settingEx.idAuthMode = (byte)BS2ExtIDAuthModeEnum.NONE;
                            break;
                    }
                }	// idAuthMode
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobUserName(ref byte[] userName)
        {
            bool nameSupported = Convert.ToBoolean(deviceInfo.userNameSupported);

            if (nameSupported)
            {
                Console.WriteLine("Enter your name:");
                Console.Write(">> ");
                string name = Console.ReadLine();
                if (name.Length > BS2Environment.BS2_USER_NAME_LEN)
                {
                    Console.WriteLine("The user name should less than {0} words.", BS2Environment.BS2_USER_NAME_LEN);
                    return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
                }
                else
                {
                    byte[] nameArray = Encoding.UTF8.GetBytes(name);
                    Array.Copy(nameArray, userName, nameArray.Length);
                }
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobProfileImage(ref BS2UserFaceExBlob userBlob)
        {
            return getProfileImage(ref userBlob.user_photo_obj);
        }

        BS2ErrorCode getProfileImage(ref IntPtr user_photo_obj)
        {
            bool photoSupported = Convert.ToBoolean(deviceInfo.userPhotoSupported);

            if (photoSupported)
            {
                Console.WriteLine("Do you want to set profile image? [Y/n]");
                Console.Write(">> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("Enter the jpg file path for this user.");
                    Console.Write(">> ");
                    string imagePath = Console.ReadLine();

                    if (!File.Exists(imagePath))
                    {
                        Console.WriteLine("Invalid file path");
                        return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
                    }

                    Image profileImage = Image.FromFile(imagePath);
                    if (!profileImage.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                    {
                        Console.WriteLine("Invalid image file format");
                        return BS2ErrorCode.BS_SDK_ERROR_INVALID_FILE_FORMAT;
                    }

                    IntPtr imageData = IntPtr.Zero;
                    UInt32 imageDataLen = 0;

                    if (Util.LoadBinary(imagePath, out imageData, out imageDataLen))
                    {
                        if (imageDataLen == 0)
                        {
                            Console.WriteLine("Empty image file");
                            return BS2ErrorCode.BS_SDK_ERROR_EXIST;
                        }
                        else if (imageDataLen > BS2Environment.BS2_USER_PHOTO_SIZE)
                        {
                            Console.WriteLine("The profile image should less than {0} bytes.", BS2Environment.BS2_USER_PHOTO_SIZE);
                            return BS2ErrorCode.BS_SDK_ERROR_USER_IMAGE_SIZE_TOO_BIG;
                        }

                        user_photo_obj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2UserPhoto)));
                        IntPtr curObj = user_photo_obj;

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
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobPINCode(IntPtr sdkContext, ref byte[] cipherText)
        {
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            BS2ErrorCode sdkResult = BS2ErrorCode.BS_SDK_SUCCESS;

            if (pinSupported)
            {
                Console.WriteLine("Do you want to encrypt the PIN code with a custom key and apply it? [y/N]");
                Console.Write(">> ");
                if (Util.IsNo())
                {
                    // Default
                    Console.WriteLine("Enter the PIN code:");
                    Console.Write(">> ");
                    string pinString = Console.ReadLine();
                    if (BS2Environment.BS2_PIN_HASH_SIZE < pinString.Length)
                    {
                        Console.WriteLine("PIN code is too long");
                        return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
                    }

                    IntPtr ptrChar = Marshal.StringToHGlobalAnsi(pinString);
                    IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    sdkResult = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, ptrChar, pinCode);
                    if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                    {
                        Console.WriteLine("BS2_MakePinCode call failed: {0}", sdkResult);
                        return sdkResult;
                    }

                    Marshal.Copy(pinCode, cipherText, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    Marshal.FreeHGlobal(ptrChar);
                    Marshal.FreeHGlobal(pinCode);
                }
                else
                {
                    Console.WriteLine("Please enter the PIN encryption key.");
                    Console.WriteLine("You may have changed the key using the function BS2_SetDataEncryptKey.");
                    Console.Write(">> ");
                    string keyString = Console.ReadLine();
                    byte[] buff = Encoding.UTF8.GetBytes(keyString);

                    BS2EncryptKey keyInfo = Util.AllocateStructure<BS2EncryptKey>();
                    Array.Clear(keyInfo.key, 0, BS2Environment.BS2_ENC_KEY_SIZE);
                    Array.Copy(buff, 0, keyInfo.key, 0, keyString.Length);

                    Console.WriteLine("Enter the PIN code:");
                    Console.Write(">> ");
                    string pinString = Console.ReadLine();
                    if (BS2Environment.BS2_PIN_HASH_SIZE < pinString.Length)
                    {
                        Console.WriteLine("PIN code is too long");
                        return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
                    }

                    IntPtr ptrChar = Marshal.StringToHGlobalAnsi(pinString);
                    IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    sdkResult = (BS2ErrorCode)API.BS2_MakePinCodeWithKey(sdkContext, ptrChar, pinCode, ref keyInfo);
                    if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                    {
                        Console.WriteLine("BS2_MakePinCodeWithKey call failed: {0}", sdkResult);
                        return sdkResult;
                    }

                    Marshal.Copy(pinCode, cipherText, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    Marshal.FreeHGlobal(ptrChar);
                    Marshal.FreeHGlobal(pinCode);
                }
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobJobCode(ref BS2Job job)
        {
            Console.WriteLine("How many job codes would you like to register?");
            Console.Write(">> ");
            job.numJobs = Util.GetInput((byte)1);

            if (0 < job.numJobs)
                Console.WriteLine(">> Please enter job codes.");

            for (int i = 0; i <job.numJobs; i++)
            {
                Console.WriteLine(">> Code:");
                Console.Write(">>>> ");
                job.jobs[i].code = Util.GetInput((UInt32)0);

                Console.WriteLine(">> Label:");
                Console.Write(">>>> ");
                string label = Console.ReadLine();
                byte[] labelArr = Encoding.UTF8.GetBytes(label);
                Array.Copy(labelArr, job.jobs[i].label, labelArr.Length);
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobAccessGroupID(ref UInt32[] accessGroupId)
        {
            Console.WriteLine("Do you want to register access group ID? [Y/n]");
            Console.Write(">> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Please enter access group IDs. ex)ID1 ID2 ID3 ...");
                Console.Write(">> ");
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
                            accessGroupId[accessGroupIdIndex++] = item;
                        }
                    }
                }
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobPhrase(ref byte[] phrase)
        {
            bool phraseSupported = Convert.ToBoolean(deviceInfo.phraseCodeSupported);
            if (phraseSupported)
            {
                Console.WriteLine("Enter user phrase:");
                Console.Write(">> ");
                string temp = Console.ReadLine();
                if (BS2Environment.BS2_USER_PHRASE_SIZE < temp.Length)
                {
                    Console.WriteLine("The user phrase should less than {0} words.", BS2Environment.BS2_USER_PHRASE_SIZE);
                    return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
                }
                else
                {
                    byte[] tempArr = Encoding.UTF8.GetBytes(temp);
                    Array.Copy(tempArr, phrase, tempArr.Length);
                }
            }

            return BS2ErrorCode.BS_SDK_SUCCESS;
        }

        BS2ErrorCode getUserBlobCardInfo(IntPtr sdkContext, UInt32 deviceID, ref IntPtr cardObjs, ref byte numCards)
        {
            bool cardSupported = Convert.ToBoolean(deviceInfo.cardSupported);
            BS2ErrorCode sdkResult = BS2ErrorCode.BS_SDK_SUCCESS;

            if (cardSupported)
            {
                Console.WriteLine("Do you want to scan card? [Y/n]");
                Console.Write(">> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("How many cards would you like to register?");
                    Console.Write(">> ");
                    int numOfCard = Util.GetInput(1);
                    if (0 < numOfCard)
                    {
                        int structSize = Marshal.SizeOf(typeof(BS2CSNCard));
                        BS2Card card = Util.AllocateStructure<BS2Card>();
                        cardObjs = Marshal.AllocHGlobal(structSize + numOfCard);
                        IntPtr curCardObjs = cardObjs;
                        cbCardOnReadyToScan = new API.OnReadyToScan(ReadyToScanForCard);

                        for (int index = 0; index < numOfCard; )
                        {
                            sdkResult = (BS2ErrorCode)API.BS2_ScanCard(sdkContext, deviceID, out card, cbCardOnReadyToScan);
                            if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                                Console.WriteLine("BS2_ScanCard call failed: %d", sdkResult);
                            else
                            {
                                if (Convert.ToBoolean(card.isSmartCard))
                                {
                                    Console.WriteLine("CSN card only supported.");
                                }
                                else
                                {
                                    Marshal.Copy(card.cardUnion, 0, curCardObjs, structSize);
                                    curCardObjs += structSize;
                                    numCards++;
                                    index++;
                                }
                            }
                        }
                        cbCardOnReadyToScan = null;
                    }
                }
            }

            return sdkResult;
        }

        BS2ErrorCode getUserBlobFingerprintInfo(IntPtr sdkContext, UInt32 deviceID, ref IntPtr fingerObjs, ref byte numFingers)
        {
            bool fingerScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN;
            BS2ErrorCode sdkResult = BS2ErrorCode.BS_SDK_SUCCESS;

            if (fingerScanSupported)
            {
                Console.WriteLine("Do you want to scan fingerprint? [Y/n]");
                Console.Write(">> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("How many fingers would you like to register?");
                    Console.Write(">> ");
                    int numOfFinger = Util.GetInput(1);
                    if (0 < numOfFinger)
                    {
                        int structSize = Marshal.SizeOf(typeof(BS2Fingerprint));
                        BS2Fingerprint fingerprint = Util.AllocateStructure<BS2Fingerprint>();
                        fingerObjs = Marshal.AllocHGlobal(structSize * numOfFinger);
                        IntPtr curFingerObjs = fingerObjs;
                        cbFingerOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFinger);

                        for (int index = 0; index < numOfFinger; index++)
                        {
                            for (UInt32 templateIndex = 0; templateIndex < BS2Environment.BS2_TEMPLATE_PER_FINGER; )
                            {
                                sdkResult = (BS2ErrorCode)API.BS2_ScanFingerprint(sdkContext, deviceID, ref fingerprint, templateIndex, (UInt32)BS2FingerprintQualityEnum.QUALITY_HIGHEST, (byte)BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA, cbFingerOnReadyToScan);
                                if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                                    Console.WriteLine("BS2_ScanFingerprint call failed: %d", sdkResult);
                                else
                                    templateIndex++;
                            }

                            numFingers++;
                            fingerprint.index = (byte)index;

                            Marshal.StructureToPtr(fingerprint, curFingerObjs, false);
                            curFingerObjs += structSize;
                        }

                        cbFingerOnReadyToScan = null;
                    }
                }
            }

            return sdkResult;
        }

        BS2ErrorCode getUserBlobFaceInfo(IntPtr sdkContext, UInt32 deviceID, ref IntPtr faceObjs, ref byte numFaces)
        {
            bool faceScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN;
            BS2ErrorCode sdkResult = BS2ErrorCode.BS_SDK_SUCCESS;

            if (faceScanSupported)
            {
                Console.WriteLine("How many face would you like to register?");
                Console.Write(">> ");
                int numOfFace = Util.GetInput(1);
                if (0 < numOfFace)
                {
                    int structSize = Marshal.SizeOf(typeof(BS2Face));
                    BS2Face[] face = Util.AllocateStructureArray<BS2Face>(1);
                    faceObjs = Marshal.AllocHGlobal(structSize * numOfFace);
                    IntPtr curFaceObjs = faceObjs;
                    cbFaceOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFace);

                    for (int index = 0; index < numOfFace; )
                    {
                        sdkResult = (BS2ErrorCode)API.BS2_ScanFace(sdkContext, deviceID, face, (byte)BS2FaceEnrollThreshold.THRESHOLD_DEFAULT, cbFaceOnReadyToScan);
                        if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                            Console.WriteLine("BS2_ScanFace call failed: %d", sdkResult);
                        else
                        {
                            numFaces++;
                            index++;
                            face[0].faceIndex = (byte)index;
                            Marshal.StructureToPtr(face[0], curFaceObjs, false);
                            curFaceObjs += structSize;

                            Thread.Sleep(100);
                        }
                    }

                    cbFaceOnReadyToScan = null;
                }
            }

            return sdkResult;
        }

        BS2ErrorCode getUserBlobFaceExInfo(IntPtr sdkContext, UInt32 deviceID, ref IntPtr faceExObjs, ref byte numFaces
#if _SAVE_TO_FILE
            , string userID
#endif
            )
        {
            bool faceExScanSupported = (deviceInfoEx.supported & (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN) == (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN;
            BS2ErrorCode sdkResult = BS2ErrorCode.BS_SDK_SUCCESS;

            if (faceExScanSupported)
            {
                Console.WriteLine("Do you want to scan faceEx? [y/n]");
                Console.Write(">> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("How many faceEx would you like to register?");
                    Console.Write(">> ");
                    int numOfFace = Util.GetInput(1);
                    if (0 < numOfFace)
                    {
                        int structSize = Marshal.SizeOf(typeof(BS2FaceExWarped));
                        BS2FaceExWarped[] faceEx = Util.AllocateStructureArray<BS2FaceExWarped>(1);
                        faceExObjs = Marshal.AllocHGlobal(structSize * numOfFace);
                        IntPtr curFaceExObjs = faceExObjs;
                        cbFaceOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFace);

                        for (int index = 0; index < numOfFace; )
                        {
                            sdkResult = (BS2ErrorCode)API.BS2_ScanFaceEx(sdkContext, deviceID, faceEx, (byte)BS2FaceEnrollThreshold.THRESHOLD_DEFAULT, cbFaceOnReadyToScan);
                            if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                                Console.WriteLine("BS2_ScanFaceEx call failed: %d", sdkResult);
                            else
                            {
#if _SAVE_TO_FILE
                                Console.WriteLine("Do you want to save warpped image? [y/n]");
                                Console.Write(">> ");
                                if (Util.IsYes())
                                {
                                    int written = 0;
                                    int size = (int)faceEx[index].imageLen;
                                    IntPtr imgPtr = Marshal.AllocHGlobal(size);
                                    Marshal.Copy(faceEx[index].imageData, 0, imgPtr, size);
                                    FileStream file = new FileStream(String.Format("{0}-{1}.jpg", userID, index), FileMode.Create, FileAccess.Write);
                                    WriteFile(file.Handle, imgPtr, (int)faceEx[index].imageLen, out written, IntPtr.Zero);
                                    file.Close();
                                    Marshal.FreeHGlobal(imgPtr);
                                }
#endif
                                numFaces++;
                                index++;
                                faceEx[0].faceIndex = (byte)index;
                                Marshal.StructureToPtr(faceEx[0], curFaceExObjs, false);
                                curFaceExObjs += structSize;

                                Thread.Sleep(100);
                            }
                        }

                        cbFaceOnReadyToScan = null;
                    }
                }
                else
                {
                    Console.WriteLine("Do you want to register from image? [y/n]");
                    Console.Write(">> ");
                    if (Util.IsYes())
                    {
                        Console.WriteLine("Enter the face image path and name:");
                        Console.Write(">> ");
                        string imagePath = Console.ReadLine();

                        if (!File.Exists(imagePath))
                        {
                            Console.WriteLine("Invalid file path");
                            return BS2ErrorCode.BS_SDK_ERROR_INVALID_PARAM;
                        }

                        Image faceImage = Image.FromFile(imagePath);
                        if (!faceImage.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        {
                            Console.WriteLine("Invalid image file format");
                            return BS2ErrorCode.BS_SDK_ERROR_INVALID_FILE_FORMAT;
                        }

                        IntPtr imageData = IntPtr.Zero;
                        UInt32 imageLen = 0;
                        if (Util.LoadBinary(imagePath, out imageData, out imageLen))
                        {
                            if (0 == imageLen)
                            {
                                Console.WriteLine("Empty image file");
                                return BS2ErrorCode.BS_SDK_ERROR_EXIST;
                            }

                            int structHeaderSize = Marshal.SizeOf(typeof(BS2FaceExUnwarped));
                            int totalSize = structHeaderSize + (int)imageLen;
                            faceExObjs = Marshal.AllocHGlobal(totalSize);
                            IntPtr curFaceExObjs = faceExObjs;

                            BS2FaceExUnwarped unwarped = Util.AllocateStructure<BS2FaceExUnwarped>();
                            unwarped.flag = 0;
                            unwarped.imageLen = imageLen;

                            Marshal.StructureToPtr(unwarped, curFaceExObjs, false);
                            curFaceExObjs += structHeaderSize;

                            Util.CopyMemory(curFaceExObjs, imageData, imageLen);

                            numFaces = 1;
                        }
                    }
                }
            }

            return sdkResult;
        }

        public void partialUpdateFaceEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            bool pinSupported = Convert.ToBoolean(deviceInfo.pinSupported);
            bool nameSupported = Convert.ToBoolean(deviceInfo.userNameSupported);
            bool phraseSupported = Convert.ToBoolean(deviceInfo.phraseCodeSupported);

            BS2_USER_MASK mask = getMaskForUpdateUser();

            Console.WriteLine("How many users do you want to update?");
            Console.Write(">> ");
            int numOfUser = Util.GetInput(1);
            if (numOfUser <= 0)
                return;

            BS2ErrorCode sdkResult = BS2ErrorCode.BS_SDK_SUCCESS;
            BS2UserFaceExBlob[] userBlob = Util.AllocateStructureArray<BS2UserFaceExBlob>(numOfUser);

            for (UInt32 idx = 0; idx < numOfUser; idx++)
            {
                userBlob[idx].cardObjs = IntPtr.Zero;
                userBlob[idx].fingerObjs = IntPtr.Zero;
                userBlob[idx].faceObjs = IntPtr.Zero;
                userBlob[idx].user_photo_obj = IntPtr.Zero;
                userBlob[idx].faceExObjs = IntPtr.Zero;

                userBlob[idx].setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.NONE;
                userBlob[idx].setting.cardAuthMode = (byte)BS2CardAuthModeEnum.NONE;
                userBlob[idx].setting.idAuthMode = (byte)BS2IDAuthModeEnum.NONE;

                userBlob[idx].settingEx.faceAuthMode = (byte)BS2ExtFaceAuthModeEnum.NONE;
                userBlob[idx].settingEx.fingerprintAuthMode = (byte)BS2ExtFingerprintAuthModeEnum.NONE;
                userBlob[idx].settingEx.cardAuthMode = (byte)BS2ExtCardAuthModeEnum.NONE;
                userBlob[idx].settingEx.idAuthMode = (byte)BS2ExtIDAuthModeEnum.NONE;

                string userID;
                if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobUserID(ref userBlob[idx].user, out userID)))
                    return;

                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.SETTING) == (BS2_USER_MASK)BS2UserMaskEnum.SETTING)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobExpiryDate(ref userBlob[idx].setting)))
                        return;

                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobPrivateAuthMode(ref userBlob[idx].setting)))
                        return;

                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobSecurityLevel(ref userBlob[idx].setting)))
                        return;
                }

                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.SETTING_EX) == (BS2_USER_MASK)BS2UserMaskEnum.SETTING_EX)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobPrivateAuthModeEx(ref userBlob[idx].settingEx)))
                        return;
                }

                //Array.Clear(userBlob[idx].name, 0, BS2Environment.BS2_USER_NAME_LEN);
                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.NAME) == (BS2_USER_MASK)BS2UserMaskEnum.NAME)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobUserName(ref userBlob[idx].name)))
                        return;

                    userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.NAME;
                }

                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.PHOTO) == (BS2_USER_MASK)BS2UserMaskEnum.PHOTO)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobProfileImage(ref userBlob[idx])))
                        return;

                    userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.PHOTO;
                }

                //Array.Clear(userBlob[idx].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.PIN) == (BS2_USER_MASK)BS2UserMaskEnum.PIN)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobPINCode(sdkContext, ref userBlob[idx].pin)))
                        return;

                    userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.PIN;
                }

                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.JOB) == (BS2_USER_MASK)BS2UserMaskEnum.JOB)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobJobCode(ref userBlob[idx].job)))
                        return;

                    userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.JOB_CODE;
                }

                Array.Clear(userBlob[idx].accessGroupId, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER);
                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.ACCESS_GROUP) == (BS2_USER_MASK)BS2UserMaskEnum.ACCESS_GROUP)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobAccessGroupID(ref userBlob[idx].accessGroupId)))
                        return;
                }

                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.PHRASE) == (BS2_USER_MASK)BS2UserMaskEnum.PHRASE)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobPhrase(ref userBlob[idx].phrase)))
                        return;
                    
                    userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.PHRASE;
                }

                userBlob[idx].user.numCards = 0;
                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.CARD) == (BS2_USER_MASK)BS2UserMaskEnum.CARD)
                {
                    Console.WriteLine("Do you want to change/delete #{0} cards? (0:Keep, 1:Change, 2:Delete)", userID);
                    Console.Write(">> ");
                    int selected = Util.GetInput(0);
                    switch (selected)
                    {
                        case 0:     // Keep cards on the device
                        default:
                            userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.CARD;
                            break;

                        case 1:     // Change cards
                            if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobCardInfo(sdkContext, deviceID, ref userBlob[idx].cardObjs, ref userBlob[idx].user.numCards)))
                                return;

                            userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.CARD;
                            break;

                        case 2:     // Delete cards on the device
                            // unmasking and numCards = 0;
                            break;
                    }
                }
                else
                {
                    userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.CARD;
                }

                userBlob[idx].user.numFingers = 0;
                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.FINGER) == (BS2_USER_MASK)BS2UserMaskEnum.FINGER)
                {
                    Console.WriteLine("Do you want to change/delete #{0} fingerprints? (0:Keep, 1:Change, 2:Delete)", userID);
                    Console.Write(">> ");
                    int selected = Util.GetInput(0);
                    switch (selected)
                    {
                        case 0:     // Keep fingerprints on the device
                        default:
                            userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.FINGER;
                            break;

                        case 1:     // Change fingerprints
                            if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobFingerprintInfo(sdkContext, deviceID, ref userBlob[idx].fingerObjs, ref userBlob[idx].user.numFingers)))
                                return;

                            userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.FINGER;
                            break;

                        case 2:     // Delete fingerprints on the device
                            // unmasking and numFingers = 0;
                            break;
                    }
                }
                else
                {
                    userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.FINGER;
                }

                userBlob[idx].user.numFaces = 0;
                if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.FACE) == (BS2_USER_MASK)BS2UserMaskEnum.FACE)
                {
                    Console.WriteLine("Do you want to change/delete #{0} faces? (0:Keep, 1:Change, 2:Delete)", userID);
                    Console.Write(">> ");
                    int selected = Util.GetInput(0);
                    switch (selected)
                    {
                        case 0:     // Keep faces on the device
                        default:
                            userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.FACE;
                            break;

                        case 1:     // Change faces
                            if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobFaceInfo(sdkContext, deviceID, ref userBlob[idx].faceObjs, ref userBlob[idx].user.numFaces)))
                                return;

                            userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.FACE;
                            break;

                        case 2:     // Delete faces on the device
                            break;
                    }
                }
                else
                {
                    if ((mask & (BS2_USER_MASK)BS2UserMaskEnum.FACE_EX) == (BS2_USER_MASK)BS2UserMaskEnum.FACE_EX)
                    {
                        Console.WriteLine("Do you want to change/delete #{0} faceExs? (0:Keep, 1:Change, 2:Delete)");
                        Console.Write(">> ");
                        int selected = Util.GetInput(0);
                        switch (selected)
                        {
                            case 0:     // Keep faceExs on the device
                            default:
                                userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.FACE;
                                break;

                            case 1:     // Change faceExs
                                if (BS2ErrorCode.BS_SDK_SUCCESS != (sdkResult = getUserBlobFaceExInfo(sdkContext, deviceID, ref userBlob[idx].faceExObjs, ref userBlob[idx].user.numFaces)))
                                userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.FACE;
                                break;

                            case 2:     // Delete faceExs on the device
                                break;
                        }
                    }
                    else
                    {
                        userBlob[idx].user.infoMask |= (byte)BS2UserInfoMaskEnum.FACE;
                    }
                }
            } // for

            sdkResult = (BS2ErrorCode)API.BS2_PartialUpdateUserFaceEx(sdkContext, deviceID, mask, userBlob, (UInt32)numOfUser);
            if (BS2ErrorCode.BS_SDK_SUCCESS != sdkResult)
                Console.WriteLine("BS2_PartialUpdateUserFaceEx call failed {0}", sdkResult);

            foreach (BS2UserFaceExBlob blob in userBlob)
            {
                if (blob.cardObjs != IntPtr.Zero)
                    Marshal.FreeHGlobal(blob.cardObjs);

                if (blob.fingerObjs != IntPtr.Zero)
                    Marshal.FreeHGlobal(blob.fingerObjs);

                if (blob.faceObjs != IntPtr.Zero)
                    Marshal.FreeHGlobal(blob.faceObjs);

                if (blob.faceExObjs != IntPtr.Zero)
                    Marshal.FreeHGlobal(blob.faceExObjs);
            }
        }

        public void getUserStatistic(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2UserStatistic statistic;
            Console.WriteLine("Trying to get user statistic");
            BS2ErrorCode sdkResult = (BS2ErrorCode)API.BS2_GetUserStatistic(sdkContext, deviceID, out statistic);
            if (sdkResult != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", sdkResult);
                return;
            }

            print(statistic);
        }


        void print(BS2UserBlob userBlob)
        {
            Console.WriteLine(">>>> BS2UserBlob");
            Console.WriteLine("     +--user");
            Console.WriteLine("     |  +--userID[{0}]", Encoding.Default.GetString(userBlob.user.userID));
            Console.WriteLine("     |  |--flag[{0}]", userBlob.user.flag);
            Console.WriteLine("     |  |--numCards[{0}]", userBlob.user.numCards);
            Console.WriteLine("     |  |--numFingers[{0}]", userBlob.user.numFingers);
            Console.WriteLine("     |  +--numFaces[{0}]", userBlob.user.numFaces);
            Console.WriteLine("     +--setting");
            Console.WriteLine("     |  +--startTime[{0}]", userBlob.setting.startTime);
            Console.WriteLine("     |  |--endTime[{0}]", userBlob.setting.endTime);
            Console.WriteLine("     |  |--fingerAuthMode[{0}]", userBlob.setting.fingerAuthMode);
            Console.WriteLine("     |  |--cardAuthMode[{0}]", userBlob.setting.cardAuthMode);
            Console.WriteLine("     |  |--idAuthMode[{0}]", userBlob.setting.idAuthMode);
            Console.WriteLine("     |  +--securityLevel[{0}]", userBlob.setting.securityLevel);
            Console.WriteLine("     +--name[{0}]", Encoding.Default.GetString(userBlob.name));
            Console.WriteLine("     +--pin[{0}]", BitConverter.ToString(userBlob.pin));
            Console.WriteLine("     +--card");
            printCard(userBlob.cardObjs, userBlob.user.numCards);
            Console.WriteLine("     +--finger");
            printFinger(userBlob.fingerObjs, userBlob.user.numFingers);
            Console.WriteLine("     +--face");
            printFace(userBlob.faceObjs, userBlob.user.numFaces);
        }

        void print(BS2UserFaceExBlob userBlob)
        {
            Console.WriteLine(">>>> BS2UserFaceExBlob");
            Console.WriteLine("     +--user");
            Console.WriteLine("     |  +--userID[{0}]", Encoding.Default.GetString(userBlob.user.userID));
            Console.WriteLine("     |  |--flag[{0}]", userBlob.user.flag);
            Console.WriteLine("     |  |--numCards[{0}]", userBlob.user.numCards);
            Console.WriteLine("     |  |--numFingers[{0}]", userBlob.user.numFingers);
            Console.WriteLine("     |  +--numFaces[{0}]", userBlob.user.numFaces);
            Console.WriteLine("     +--setting");
            Console.WriteLine("     |  +--startTime[{0}]", userBlob.setting.startTime);
            Console.WriteLine("     |  |--endTime[{0}]", userBlob.setting.endTime);
            Console.WriteLine("     |  |--fingerAuthMode[{0}]", userBlob.setting.fingerAuthMode);
            Console.WriteLine("     |  |--cardAuthMode[{0}]", userBlob.setting.cardAuthMode);
            Console.WriteLine("     |  |--idAuthMode[{0}]", userBlob.setting.idAuthMode);
            Console.WriteLine("     |  +--securityLevel[{0}]", userBlob.setting.securityLevel);
            Console.WriteLine("     +--name[{0}]", Encoding.Default.GetString(userBlob.name));
            Console.WriteLine("     +--pin[{0}]", BitConverter.ToString(userBlob.pin));
            Console.WriteLine("     +--card");
            printCard(userBlob.cardObjs, userBlob.user.numCards);
            Console.WriteLine("     +--finger");
            printFinger(userBlob.fingerObjs, userBlob.user.numFingers);
            Console.WriteLine("     +--face");
            printFace(userBlob.faceObjs, userBlob.user.numFaces);
            Console.WriteLine("     +--settingEx");
            Console.WriteLine("     |  +--faceAuthMode[{0}]", userBlob.settingEx.faceAuthMode);
            Console.WriteLine("     |  |--fingerprintAuthMode[{0}]", userBlob.settingEx.fingerprintAuthMode);
            Console.WriteLine("     |  |--cardAuthMode[{0}]", userBlob.settingEx.cardAuthMode);
            Console.WriteLine("     |  +--idAuthMode[{0}]", userBlob.settingEx.idAuthMode);
            Console.WriteLine("     +--faceEx");
            printFaceEx(userBlob.faceExObjs, userBlob.user.numFaces);
        }

        void printCard(IntPtr cardObjs, byte numOfCard)
        {
            if (cardObjs != IntPtr.Zero && 0 < numOfCard)
            {
                Type structType = typeof(BS2CSNCard);
                int structSize = Marshal.SizeOf(structType);
                IntPtr curItem = cardObjs;
                for (int i = 0; i < numOfCard; i++)
                {
                    BS2CSNCard card = (BS2CSNCard)Marshal.PtrToStructure(curItem, structType);

                    Console.WriteLine("     |  +--type_{0}[{1}]", i, card.type);
                    Console.WriteLine("     |  |--size_{0}[{1}]", i, card.size);
                    Console.WriteLine("     |  +--data_{0}[{1}]", i, BitConverter.ToString(card.data));

                    curItem += structSize;
                }

                return;
            }

            Console.WriteLine("     |  +--empty");
        }

        void printFinger(IntPtr fingerObjs, byte numOfFinger)
        {
            if (fingerObjs != IntPtr.Zero && 0 < numOfFinger)
            {
                Type structType = typeof(BS2Fingerprint);
                int structSize = Marshal.SizeOf(structType);
                IntPtr curItem = fingerObjs;
                for (int i = 0; i < numOfFinger; i++)
                {
                    BS2Fingerprint item = (BS2Fingerprint)Marshal.PtrToStructure(curItem, structType);

                    Console.WriteLine("     |  +--index_{0}[{1}]", i, item.index);
                    Console.WriteLine("     |  +--flag_{0}[{1}]", i, item.flag);

                    curItem += structSize;
                }

                return;
            }

            Console.WriteLine("     |  +--empty");
        }

        void printFace(IntPtr faceObjs, byte numOfFace)
        {
            if (faceObjs != IntPtr.Zero && 0 < numOfFace)
            {
                Type structType = typeof(BS2Face);
                int structSize = Marshal.SizeOf(structType);
                IntPtr curItem = faceObjs;
                for (int i = 0; i < numOfFace; i++)
                {
                    BS2Face item = (BS2Face)Marshal.PtrToStructure(curItem, structType);

                    Console.WriteLine("     |  +--faceIndex_{0}[{1}]", i, item.faceIndex);
                    Console.WriteLine("     |  |--numOfTemplate_{0}[{1}]", i, item.numOfTemplate);
                    Console.WriteLine("     |  |--flag_{0}[{1}]", i, item.flag);
                    Console.WriteLine("     |  +--imageLen_{0}[{1}]", i, item.imageLen);

                    curItem += structSize;
                }

                return;
            }

            Console.WriteLine("     |  +--empty");
        }

        void printFaceEx(IntPtr faceExObjs, byte numOfFace)
        {
            if (faceExObjs != IntPtr.Zero && 0 < numOfFace)
            {
                Type structType = typeof(BS2FaceExWarped);
                int structSize = Marshal.SizeOf(structType);
                IntPtr curItem = faceExObjs;
                for (int i = 0; i < numOfFace; i++)
                {
                    BS2FaceExWarped item = (BS2FaceExWarped)Marshal.PtrToStructure(curItem, structType);

                    Console.WriteLine("     |  +--faceIndex_{0}[{1}]", i, item.faceIndex);
                    Console.WriteLine("     |  |--numOfTemplate_{0}[{1}]", i, item.numOfTemplate);
                    Console.WriteLine("     |  |--flag_{0}[{1}]", i, item.flag);
                    Console.WriteLine("     |  |--imageLen_{0}[{1}]", i, item.imageLen);
                    Console.WriteLine("     |  +--irImageLen_{0}[{1}]", i, item.irImageLen);

                    curItem += structSize;
                }

                return;
            }

            Console.WriteLine("     |  +--empty");
        }

        public void onUserPhrase(UInt32 deviceId, UInt16 seq, string userID)
        {
            string privateMsg = "SvrMsg-" + userID;
            byte[] uidArray = Util.StringToByte(BS2Environment.BS2_USER_ID_SIZE, privateMsg);

            IntPtr uid = Marshal.AllocHGlobal(BS2Environment.BS2_USER_ID_SIZE);
            Marshal.Copy(uidArray, 0, uid, uidArray.Length);

            int handleResult = (int)BS2ErrorCode.BS_SDK_SUCCESS;
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_ResponseUserPhrase(sdkContext, deviceId, seq, handleResult, uid);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            Marshal.FreeHGlobal(uid);
        }

        public void activateUserPhrase(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2DisplayConfig config;
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDisplayConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(config);
            
            config.useUserPhrase = Convert.ToByte(true);
            config.queryUserPhrase = Convert.ToByte(true);

            result = (BS2ErrorCode)API.BS2_SetDisplayConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            this.sdkContext = sdkContext;
            cbOnUserPhrase = new API.OnUserPhrase(onUserPhrase);
            result = (BS2ErrorCode)API.BS2_SetUserPhraseHandler(sdkContext, cbOnUserPhrase);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Try authentication.");
        }

        public void deactivateUserPhrase(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            cbOnUserPhrase = null;
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetUserPhraseHandler(sdkContext, cbOnUserPhrase);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        void print(BS2DisplayConfig config)
        {
            Console.WriteLine(">>>> Display configuration ");            
            Console.WriteLine("     |--useUserPhrase : {0}", config.useUserPhrase);            
            Console.WriteLine("     |--queryUserPhrase : {0}", config.queryUserPhrase);
            Console.WriteLine("<<<< ");
        }

        void print(BS2UserStatistic statistic)
        {
            Console.WriteLine(">>>> User statistic ");
            Console.WriteLine("    |--numUsers : {0}", statistic.numUsers);
            Console.WriteLine("    |--numCards : {0}",  statistic.numCards);
            Console.WriteLine("    |--numFingerprints : {0}", statistic.numFingerprints);
            Console.WriteLine("    |--numFaces : {0}",  statistic.numFaces);
            Console.WriteLine("    |--numNames : {0}",  statistic.numNames);
            Console.WriteLine("    |--numImages : {0}", statistic.numImages);
            Console.WriteLine("    |--numPhrases: {0}", statistic.numPhrases);
        }

        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(IntPtr hFile, IntPtr lpBuffer, int NumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr lpOverlapped);
    }
}
