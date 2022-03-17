using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.Data.SQLite;

namespace Suprema
{
    public class USBControl : FunctionModule
    {
        private API.OnLogReceived cbOnLogReceived = null; //To prevent garbage collection

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get log", getLog));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Filtering log", getFilteredLog));
            //functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get image log", getImageLog));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get logBlob", getLogBlob));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get logSmallBlobEx", getLogSmallBlobEx));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get database Info", getDatabaseInfo));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list only", listOnlyUser));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list with Infos", listUserInfos));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list with Datas", listUserDatas));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list with InfosEx", listUserInfosEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get user list with DatasEx", listUserDatasEx));

            return functionList;
        }

        void getLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            const UInt32 defaultLogPageSize = 1024;
            Type structureType = typeof(BS2Event);
            int structSize = Marshal.SizeOf(structureType);
            bool getAllLog = false;
            UInt32 lastEventId = 0;
            UInt32 amount;
            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;
            cbOnLogReceived = new API.OnLogReceived(NormalLogReceived);
            
            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine("What is the ID of the last log which you have? [0: None]");
            Console.Write(">>>> ");
            lastEventId = Util.GetInput((UInt32)0);
            Console.WriteLine("How many logs do you want to get? [0: All]");
            Console.Write(">>>> ");
            amount = Util.GetInput((UInt32)0);

            if (amount == 0)
            {
                getAllLog = true;
                amount = defaultLogPageSize;
            }

            do
            {
                outEventLogObjs = IntPtr.Zero;
                IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetLogFromDir(sdkContext, ptrDir, lastEventId, amount, out outEventLogObjs, out outNumEventLogs);
                Marshal.FreeHGlobal(ptrDir);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    break;
                }

                if (outNumEventLogs > 0)
                {
                    IntPtr curEventLogObjs = outEventLogObjs;
                    for (UInt32 idx = 0; idx < outNumEventLogs; idx++)
                    {
                        BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(curEventLogObjs, structureType);
                        Console.WriteLine(Util.GetLogMsg(eventLog));
                        curEventLogObjs += structSize;
                        lastEventId = eventLog.id;
                    }

                    API.BS2_ReleaseObject(outEventLogObjs);
                }

                if (outNumEventLogs < defaultLogPageSize)
                {
                    break;
                }
            }
            while (getAllLog);
        }

        void getFilteredLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Type structureType = typeof(BS2Event);
            int structSize = Marshal.SizeOf(structureType);
            IntPtr uid = IntPtr.Zero;
            UInt16 eventCode = 0;
            UInt32 start = 0;
            UInt32 end = 0;
            byte tnaKey = 0;
            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;

            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine("Which event do you want to get? [0: All]");
            Console.Write(">>>> ");
            eventCode = Util.GetInput((UInt16)BS2EventCodeEnum.ALL);

            Console.WriteLine("When do you want to get the log from? [yyyy-MM-dd HH:mm:ss]");
            Console.Write(">>>> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out start))
            {
                return;
            }

            Console.WriteLine("When do you want to get the log to? [yyyy-MM-dd HH:mm:ss]");
            Console.Write(">>>> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out end))
            {
                return;
            }

            Console.WriteLine("Which tnaKey do you want to get? [0: All(default), 1-16]");
            Console.Write(">>>> ");
            tnaKey = Util.GetInput(0);

            if (tnaKey > BS2Environment.BS2_MAX_TNA_KEY)
            {
                Console.WriteLine("Invalid tnaKey : {0}", tnaKey);
                return;
            }

            Console.WriteLine("Which user do you want to the log for? [userID]");
            Console.Write(">>>> ");
            string userIDStr = Console.ReadLine();
            if (userIDStr.Length > 0)
            {
                byte[] uidArray = Encoding.ASCII.GetBytes(userIDStr);
                byte[] outUidArray = new byte[BS2Environment.BS2_USER_ID_SIZE];

                uid = Marshal.AllocHGlobal(BS2Environment.BS2_USER_ID_SIZE);
                for (int idx = 0; idx < BS2Environment.BS2_USER_ID_SIZE; idx++)
                {
                    outUidArray[idx] = 0;
                }

                Array.Copy(uidArray, outUidArray, uidArray.Length);
                Marshal.Copy(outUidArray, 0, uid, BS2Environment.BS2_USER_ID_SIZE);
            }

            IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetFilteredLogFromDir(sdkContext, ptrDir, uid, eventCode, start, end, tnaKey, out outEventLogObjs, out outNumEventLogs);
            Marshal.FreeHGlobal(ptrDir);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (outNumEventLogs > 0)
            {
                IntPtr curEventLogObjs = outEventLogObjs;
                for (int idx = 0; idx < outNumEventLogs; idx++)
                {
                    BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(curEventLogObjs, structureType);
                    Console.WriteLine(Util.GetLogMsg(eventLog));
                    curEventLogObjs = (IntPtr)((long)curEventLogObjs + structSize);
                }

                API.BS2_ReleaseObject(outEventLogObjs);
            }
            else
            {
                Console.WriteLine("There are no matching logs.");
            }

            if (uid != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(uid);
            }
        }

        void getImageLog(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2SimpleDeviceInfo deviceInfo;
            int structSize = Marshal.SizeOf(typeof(BS2Event));
            UInt16 imageLogEventCode = (UInt16)BS2EventCodeEnum.DEVICE_TCP_CONNECTED;
            BS2EventConfig eventConfig = Util.AllocateStructure<BS2EventConfig>();
            eventConfig.numImageEventFilter = 1;
            eventConfig.imageEventFilter[0].mainEventCode = (byte)(imageLogEventCode >> 8);
            eventConfig.imageEventFilter[0].scheduleID = (UInt32)BS2ScheduleIDEnum.ALWAYS;

            Console.WriteLine("Trying to get the device[{0}] information.", deviceID);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceInfo(sdkContext, deviceID, out deviceInfo);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't get device information(errorCode : {0}).", result);
                return;
            }

            Console.WriteLine("Trying to activate image log.");
            result = (BS2ErrorCode)API.BS2_SetEventConfig(sdkContext, deviceID, ref eventConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Trying to clear log for quick test.");
            result = (BS2ErrorCode)API.BS2_ClearLog(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Trying to disconnect device[{0}] for quick test.", deviceID);
            result = (BS2ErrorCode)API.BS2_DisconnectDevice(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Thread.Sleep(500); //waiting for socket close

            Console.WriteLine("Trying to connect device[{0}].", deviceID);
            IntPtr ptrIPAddr = Marshal.StringToHGlobalAnsi(new IPAddress(BitConverter.GetBytes(deviceInfo.ipv4Address)).ToString());
            result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, ptrIPAddr, deviceInfo.port, out deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
            Marshal.FreeHGlobal(ptrIPAddr);

            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;

            result = (BS2ErrorCode)API.BS2_GetLog(sdkContext, deviceID, 0, 1024, out outEventLogObjs, out outNumEventLogs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            if (outNumEventLogs > 0)
            {
                IntPtr curEventLogObjs = outEventLogObjs;
                for (int idx = 0; idx < outNumEventLogs; idx++)
                {
                    BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(curEventLogObjs, typeof(BS2Event));
                    //if (Convert.ToBoolean(eventLog.image))
                    bool hasImage = Convert.ToBoolean(eventLog.image & (byte)BS2EventImageBitPos.BS2_IMAGEFIELD_POS_IMAGE);
                    if (hasImage)
                    {
                        Console.WriteLine("Trying to get image log[{0}].", eventLog.id);

                        IntPtr imageObj = IntPtr.Zero;
                        UInt32 imageSize = 0;

                        result = (BS2ErrorCode)API.BS2_GetImageLog(sdkContext, deviceID, eventLog.id, out imageObj, out imageSize);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                        }
                        else
                        {
                            int written = 0;
                            FileStream file = new FileStream(String.Format("{0}.jpg", eventLog.id), FileMode.Create, FileAccess.Write);

                            Console.WriteLine("Trying to save image log[{0}].", eventLog.id);
                            WriteFile(file.Handle, imageObj, (int)imageSize, out written, IntPtr.Zero);
                            file.Close();

                            if (written != imageSize)
                            {
                                Console.WriteLine("Got error({0}).", result);
                            }
                            else
                            {
                                Console.WriteLine("Successfully saved the image log[{0}].", eventLog.id);
                                Process.Start(file.Name);
                            }
                        }
                        break;
                    }

                    curEventLogObjs = (IntPtr)((long)curEventLogObjs + structSize);
                }

                API.BS2_ReleaseObject(outEventLogObjs);
            }

            eventConfig.numImageEventFilter = 0;

            Console.WriteLine("Trying to deactivate image log.");
            result = (BS2ErrorCode)API.BS2_SetEventConfig(sdkContext, deviceID, ref eventConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        void getLogBlob(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            const UInt32 defaultLogPageSize = 1024;
            Type structureType = typeof(BS2EventBlob);
            int structSize = Marshal.SizeOf(structureType);
            bool getAllLog = false;
            UInt32 lastEventId = 0;
            UInt32 amount;
            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;
            cbOnLogReceived = new API.OnLogReceived(NormalLogReceived);

            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine("What is the ID of the last log which you have? [0: None]");
            Console.Write(">>>> ");
            lastEventId = Util.GetInput((UInt32)0);
            Console.WriteLine("How many logs do you want to get? [0: All]");
            Console.Write(">>>> ");
            amount = Util.GetInput((UInt32)0);

            if (amount == 0)
            {
                getAllLog = true;
                amount = defaultLogPageSize;
            }

            do
            {
                outEventLogObjs = IntPtr.Zero;
                IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetLogBlobFromDir(sdkContext, ptrDir, (ushort)BS2EventMaskEnum.ALL, lastEventId, amount, out outEventLogObjs, out outNumEventLogs);
                Marshal.FreeHGlobal(ptrDir);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    break;
                }

                if (outNumEventLogs > 0)
                {
                    IntPtr curEventLogObjs = outEventLogObjs;
                    for (UInt32 idx = 0; idx < outNumEventLogs; idx++)
                    {
                        BS2EventBlob eventLog = (BS2EventBlob)Marshal.PtrToStructure(curEventLogObjs, structureType);


                        DateTime eventTime = Util.ConvertFromUnixTimestamp(eventLog.info.dateTime);

                        byte[] userID = new byte[BS2Environment.BS2_USER_ID_SIZE];
                        Array.Clear(userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                        Array.Copy(eventLog.objectID, userID, userID.Length);

                        Console.WriteLine("Got log(idx[{0}], timestamp[{1}], event id[{2}], userID[{3}], jobcode[{4}])."
                                                    , idx
                                                    , eventTime.ToString("yyyy-MM-dd HH:mm:ss")
                                                    , eventLog.id
                                                    , System.Text.Encoding.ASCII.GetString(userID).TrimEnd('\0')
                                                    , eventLog.jobCode);


                        curEventLogObjs += structSize;
                        lastEventId = eventLog.id;
                    }

                    API.BS2_ReleaseObject(outEventLogObjs);
                }

                if (outNumEventLogs < defaultLogPageSize)
                {
                    break;
                }
            }
            while (getAllLog);
        }

        void getLogSmallBlobEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            const UInt32 defaultLogPageSize = 1024;
            Type structureType = typeof(BS2EventSmallBlobEx);
            int structSize = Marshal.SizeOf(structureType);
            bool getAllLog = false;
            UInt32 lastEventId = 0;
            UInt32 amount;
            IntPtr outEventLogObjs = IntPtr.Zero;
            UInt32 outNumEventLogs = 0;

            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine("What is the ID of the last log which you have? [0: None]");
            Console.Write(">>>> ");
            lastEventId = Util.GetInput((UInt32)0);
            Console.WriteLine("How many logs do you want to get? [0: All]");
            Console.Write(">>>> ");
            amount = Util.GetInput((UInt32)0);

            if (amount == 0)
            {
                getAllLog = true;
                amount = defaultLogPageSize;
            }

            do
            {
                outEventLogObjs = IntPtr.Zero;
                IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetLogSmallBlobExFromDir(sdkContext, ptrDir, (ushort)BS2EventMaskEnum.ALL, lastEventId, amount, out outEventLogObjs, out outNumEventLogs);
                Marshal.FreeHGlobal(ptrDir);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    break;
                }

                if (outNumEventLogs > 0)
                {
                    IntPtr curEventLogObjs = outEventLogObjs;
                    for (UInt32 idx = 0; idx < outNumEventLogs; idx++)
                    {
                        BS2EventSmallBlobEx eventLog = (BS2EventSmallBlobEx)Marshal.PtrToStructure(curEventLogObjs, structureType);

                        print(idx, eventLog);

                        curEventLogObjs += structSize;
                        lastEventId = eventLog.id;
                    }

                    API.BS2_ReleaseObject(outEventLogObjs);
                }

                if (outNumEventLogs < defaultLogPageSize)
                {
                    break;
                }
            }
            while (getAllLog);
        }

        private void NormalLogReceived(UInt32 deviceID, IntPtr log)
        {
            if (log != IntPtr.Zero)
            {
                BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(log, typeof(BS2Event));
                Console.WriteLine(Util.GetLogMsg(eventLog));
            }
        }

        private int cbAcceptableUserID(string uid)
        {
            return 1;
        }

        void print(UInt32 idx, BS2UserBlob userblob)
        {
            Console.WriteLine(">>>> User idx[{4, 10}] id[{0}] numCards[{1}] numFingers[{2}] numFaces[{3}] name[{5}], startTime[{6}], endTime[{7}]",
                                Encoding.UTF8.GetString(userblob.user.userID).TrimEnd('\0'),
                                userblob.user.numCards,
                                userblob.user.numFingers,
                                userblob.user.numFaces,
                                idx,
                                Encoding.UTF8.GetString(userblob.name).TrimEnd('\0'),
                                userblob.setting.startTime,
                                userblob.setting.endTime
                                );
        }

        void print(UInt32 idx, BS2UserBlobEx userblob)
        {
            Console.WriteLine(">>>> User idx[{4, 10}] id[{0}] numCards[{1}] numFingers[{2}] numFaces[{3}] name[{5}], startTime[{6}], endTime[{7}], numJobs[{8}]",
                                Encoding.UTF8.GetString(userblob.user.userID).TrimEnd('\0'),
                                userblob.user.numCards,
                                userblob.user.numFingers,
                                userblob.user.numFaces,
                                idx,
                                Encoding.UTF8.GetString(userblob.name).TrimEnd('\0'),
                                userblob.setting.startTime,
                                userblob.setting.endTime,
                                userblob.job.numJobs
                                );
        }

        public void getDatabaseInfo(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr outUidObjs = IntPtr.Zero;
            //UInt32 numUserIds = 0;
            API.IsAcceptableUserID cbIsAcceptableUserID = new API.IsAcceptableUserID(cbAcceptableUserID); // we don't need to user id filtering

            Console.WriteLine("Enter the path of exported usb directory which you want to get.");
            Console.Write(">>>> ");
            string strDir = Console.ReadLine();

            if (!Directory.Exists(strDir))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            UInt32 numUsers = 0;
            UInt32 numCards = 0;
            UInt32 numFingers = 0;
            UInt32 numFaces = 0;
            Console.WriteLine("Trying to get database Info.");
            IntPtr ptrDir = Marshal.StringToHGlobalAnsi(strDir);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetUserDatabaseInfoFromDir(sdkContext, ptrDir, out numUsers, out numCards, out numFingers, out numFaces, cbIsAcceptableUserID);
            Marshal.FreeHGlobal(ptrDir);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Number of users : ({0}).", numUsers);
                Console.WriteLine("Number of cards : ({0}).", numCards);
                Console.WriteLine("Number of fingers : ({0}).", numFingers);
                Console.WriteLine("Number of faces : ({0}).", numFaces);
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void listOnlyUser(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
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
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds; ++idx)
                    {
                        Marshal.Copy(curUidObjs, bufUserID, 0, bufUserID.Length);
                        Console.WriteLine("idx[{0, 10}] User ID: {1}", idx, Encoding.UTF8.GetString(bufUserID).TrimEnd('\0'));
                        curUidObjs += (int)BS2Environment.BS2_USER_ID_SIZE;
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

        private const int USER_PAGE_SIZE = 5000;
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
                    BS2UserBlob[] userBlobs = new BS2UserBlob[USER_PAGE_SIZE];
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds;)
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        IntPtr ptrDir2 = Marshal.StringToHGlobalAnsi(strDir);
                        result = (BS2ErrorCode)API.BS2_GetUserInfosFromDir(sdkContext, ptrDir2, curUidObjs, available, userBlobs);
                        Marshal.FreeHGlobal(ptrDir2);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(idx+loop, userBlobs[loop]);
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
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
                    BS2UserBlob[] userBlobs = new BS2UserBlob[USER_PAGE_SIZE];
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds;)
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        IntPtr ptrDir2 = Marshal.StringToHGlobalAnsi(strDir);
                        result = (BS2ErrorCode)API.BS2_GetUserDatasFromDir(sdkContext, ptrDir2, curUidObjs, available, userBlobs, (UInt32)BS2UserMaskEnum.DATA | (UInt32)BS2UserMaskEnum.NAME);
                        Marshal.FreeHGlobal(ptrDir2);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(idx + loop, userBlobs[loop]);
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
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
                    BS2UserBlobEx[] userBlobs = new BS2UserBlobEx[USER_PAGE_SIZE];
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds;)
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        IntPtr ptrDir2 = Marshal.StringToHGlobalAnsi(strDir);
                        result = (BS2ErrorCode)API.BS2_GetUserInfosExFromDir(sdkContext, ptrDir2, curUidObjs, available, userBlobs);
                        Marshal.FreeHGlobal(ptrDir2);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(idx + loop, userBlobs[loop]);
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
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
                    BS2UserBlobEx[] userBlobs = new BS2UserBlobEx[USER_PAGE_SIZE];
                    byte[] bufUserID = new byte[BS2Environment.BS2_USER_ID_SIZE];

                    Console.WriteLine("Number of users : ({0}).", numUserIds);
                    for (UInt32 idx = 0; idx < numUserIds;)
                    {
                        UInt32 available = numUserIds - idx;
                        if (available > USER_PAGE_SIZE)
                        {
                            available = USER_PAGE_SIZE;
                        }

                        IntPtr ptrDir2 = Marshal.StringToHGlobalAnsi(strDir);
                        result = (BS2ErrorCode)API.BS2_GetUserDatasExFromDir(sdkContext, ptrDir2, curUidObjs, available, userBlobs, (UInt32)BS2UserMaskEnum.DATA | (UInt32)BS2UserMaskEnum.NAME);
                        Marshal.FreeHGlobal(ptrDir2);
                        if (result == BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            for (UInt32 loop = 0; loop < available; ++loop)
                            {
                                print(idx + loop, userBlobs[loop]);
                                if (userBlobs[loop].cardObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].cardObjs);
                                if (userBlobs[loop].fingerObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].fingerObjs);
                                if (userBlobs[loop].faceObjs != IntPtr.Zero)
                                    API.BS2_ReleaseObject(userBlobs[loop].faceObjs);
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

        void print(UInt32 idx, BS2EventSmallBlobEx eventLog)
        {
            DateTime eventTime = Util.ConvertFromUnixTimestamp(eventLog.info.dateTime);

            byte[] userID = new byte[BS2Environment.BS2_USER_ID_SIZE];
            Array.Clear(userID, 0, BS2Environment.BS2_USER_ID_SIZE);
            Array.Copy(eventLog.objectID, userID, userID.Length);

            Console.WriteLine("Got log(idx[{0}], timestamp[{1}], event id[{2}], userID[{3}], jobcode[{4}]).",
                                        idx,
                                        eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                        eventLog.id,
                                        System.Text.Encoding.ASCII.GetString(userID).TrimEnd('\0'),
                                        eventLog.jobCode);
        }

        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(IntPtr hFile, IntPtr lpBuffer, int NumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr lpOverlapped);
    }
}
