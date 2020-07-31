using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Data.SQLite;

namespace Suprema
{
    class GlobalAPBByDoorOpenManager
    {
        public UInt32 zoneID = 0;
        public SortedSet<UInt32> inDeviceIDs = new SortedSet<UInt32>();
        public SortedSet<UInt32> outDeviceIDs = new SortedSet<UInt32>();
        public BS2APBZoneTypeEnum APBZoneType = BS2APBZoneTypeEnum.SOFT;

        private Dictionary<string, UInt32> lastDevicePerUser = new Dictionary<string, UInt32>();

        public BS2ErrorCode checkAPBByDoorOpen(UInt32 deviceID, string userID1, string userID2, bool isDualAuth, bool isCheck, out UInt32 zoneID)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            if (!inDeviceIDs.Contains(deviceID) && !outDeviceIDs.Contains(deviceID))
            {
                zoneID = 0;
                result = BS2ErrorCode.BS_SDK_SUCCESS;
            }
            else
            {
                if (isCheck)
                {
                    if (lastDevicePerUser.ContainsKey(userID1))
                    {
                        if (inDeviceIDs.Contains(lastDevicePerUser[userID1]) && inDeviceIDs.Contains(deviceID))
                            result = APBZoneType == BS2APBZoneTypeEnum.SOFT ? BS2ErrorCode.BS_SDK_ERROR_SOFT_APB_VIOLATION : BS2ErrorCode.BS_SDK_ERROR_HARD_APB_VIOLATION;
                        else if (outDeviceIDs.Contains(lastDevicePerUser[userID1]) && outDeviceIDs.Contains(deviceID))
                            result = APBZoneType == BS2APBZoneTypeEnum.SOFT ? BS2ErrorCode.BS_SDK_ERROR_SOFT_APB_VIOLATION : BS2ErrorCode.BS_SDK_ERROR_HARD_APB_VIOLATION;
                        else
                            result = BS2ErrorCode.BS_SDK_SUCCESS;
                    }
                    else
                    {
                        result = BS2ErrorCode.BS_SDK_SUCCESS;
                    }

                    zoneID = this.zoneID;

                    switch ((BS2ErrorCode)result)
                    {
                        case BS2ErrorCode.BS_SDK_SUCCESS:
                        case BS2ErrorCode.BS_SDK_ERROR_SOFT_APB_VIOLATION:
                        case BS2ErrorCode.BS_SDK_ERROR_SOFT_TIMED_APB_VIOLATION:
                            if (!isDualAuth)
                                Console.WriteLine("[==>CHECK({0})] User1:{1}, result:{2}", deviceID, userID1, result);
                            else
                                Console.WriteLine("[==>CHECK({0})] User1:{1}, User2:{2} result:{3}", deviceID, userID1, userID2, result);
                            //lastDevicePerUser[userID1] = deviceID;
                            break;
                        default:
                            if (!isDualAuth)
                                Console.WriteLine("[==>FAIL({0})] User1:{1}, result:{2}", deviceID, userID1, result);
                            else
                                Console.WriteLine("[==>FAIL({0})] User1:{1}, User2:{2} result:{3}", deviceID, userID1, userID2, result);
                            break;
                    }
                }
                else
                {
                    if (!isDualAuth)
                        Console.WriteLine("[==>UPDATE({0})] User1:{1}, result:{2}", deviceID, userID1, result);
                    else
                        Console.WriteLine("[==>UPDATE({0})] User1:{1}, User2:{2} result:{3}", deviceID, userID1, userID2, result);
                    // Real update for user
                    lastDevicePerUser[userID1] = deviceID;
                    zoneID = this.zoneID;
                    result = BS2ErrorCode.BS_SDK_SUCCESS;
                }
            }

            return result;
        }
    }

    abstract class BaseTask
    {
        public abstract void execute();
    }

    class CheckGlobalAPBByDoorOpenTask : BaseTask
    {
        GlobalAPBByDoorOpenManager zoneManager;
        IntPtr sdkContext;
        UInt32 deviceID;
        UInt16 seq;
        string userID1;
        string userID2;
        bool isDualAuth;
        bool isCheck;

        public CheckGlobalAPBByDoorOpenTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, string userID1, string userID2, bool isDualAuth, bool isCheck, GlobalAPBByDoorOpenManager zoneManager)
        {
            this.sdkContext = sdkContext;
            this.deviceID = deviceID;
            this.seq = seq;
            this.userID1 = userID1;
            this.userID2 = userID2;
            this.isDualAuth = isDualAuth;
            this.isCheck = isCheck;
            this.zoneManager = zoneManager;
        }

        public override void execute()
        {
            UInt32 zoneID = 0;
            BS2ErrorCode checkResult = zoneManager.checkAPBByDoorOpen(deviceID, userID1, userID2, isDualAuth, isCheck, out zoneID);
            Console.WriteLine("[Server] responded with a status of {0}, zoneID[{7}] : device[{1}], seq[{2}], userID_1[{3}], userID_2[{4}], isDualAuth[{5}], isCheck[{6}]", checkResult, deviceID, seq, userID1, userID2, isDualAuth, isCheck, zoneID);
            if (isCheck)
            {
                // Response to the device for global APB by door open
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_CheckGlobalAPBViolationByDoorOpen(sdkContext, deviceID, seq, (int)checkResult, zoneID);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
            }
        }
    }

    class CheckGlobalAPBByDoorOpenTaskExecutor : IDisposable
    {
        private bool running;
        private Thread thread;
        private readonly object locker = new object();
        private EventWaitHandle handleWait = new AutoResetEvent(false);
        private Queue<BaseTask> qTask = new Queue<BaseTask>();

        public CheckGlobalAPBByDoorOpenTaskExecutor()
        {
            thread = new Thread(run);
        }

        public void enqueue(BaseTask task)
        {
            lock(locker)
            {
                qTask.Enqueue(task);
            }
            handleWait.Set();
        }

        public void Dispose()
        {
            stop();
        }

        public void start()
        {
            if (!running)
            {
                thread.Start();
                running = true;
            }
        }

        public void stop()
        {
            if (running)
            {
                lock (locker)
                {
                    qTask.Clear();
                }
                handleWait.Set();
                thread.Join();
                handleWait.Close();
                running = false;
            }
        }

        public void run()
        {
            while (running)
            {
                BaseTask task = null;

                lock (locker)
                {
                    if (0 < qTask.Count)
                        task = qTask.Dequeue();
                }

                if (task != null)
                    task.execute();
                else
                    handleWait.WaitOne();
            }
        }
    }

    public class GlobalAPBZoneByDoorOpenControl : FunctionModule
    {
        private API.OnCheckGlobalAPBViolationByDoorOpen cbCheck = null;
        private API.OnUpdateGlobalAPBViolationByDoorOpen cbUpdate = null;
        private CheckGlobalAPBByDoorOpenTaskExecutor taskExecutor = null;
        private GlobalAPBByDoorOpenManager zoneManager = null;
        private IntPtr sdkContext;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Start Global APB zone by door open", StartGlobalAPBByDoorOpen));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Show connected device list", ShowConnectionDeviceList));

            return functionList;
        }

        public void ShowConnectionDeviceList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            uint numDevice = 0;
            IntPtr deviceObjList = IntPtr.Zero;
            API.BS2_GetDevices(sdkContext, out deviceObjList, out numDevice);

            Console.WriteLine("Number of connected device: " + numDevice);
            if (numDevice > 0)
            {
                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                for (UInt32 idx = 0; idx < numDevice; ++idx)
                {
                    deviceID = Convert.ToUInt32(Marshal.ReadInt32(deviceObjList, (int)idx * sizeof(UInt32)));
                    Console.WriteLine("[{0:000}] ==> ID[{1, 10}]",
                            idx,
                            deviceID);
                }
                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
            }

            API.BS2_ReleaseObject(deviceObjList);
        }
        
        public void StartGlobalAPBByDoorOpen(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            zoneManager = new GlobalAPBByDoorOpenManager();

            UInt32[] connectedDeviceIDs = null;
            uint numDevice = 0;
            IntPtr deviceObjList = IntPtr.Zero;
            API.BS2_GetDevices(sdkContext, out deviceObjList, out numDevice);
            connectedDeviceIDs = new UInt32[numDevice];
            for (int idx = 0; idx < numDevice; ++idx)
                connectedDeviceIDs[idx] = Convert.ToUInt32(Marshal.ReadInt32(deviceObjList, (int)idx * sizeof(UInt32)));
            API.BS2_ReleaseObject(deviceObjList);

            if (connectedDeviceIDs.Length < 2)
            {
                Console.WriteLine("Number of connected device is {0}. At least two terminals must be connected.", connectedDeviceIDs.Length);
                return;
            }

            Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
            for (UInt32 idx = 0; idx < numDevice; ++idx)
            {
                Console.WriteLine("[{0:000}] ==> ID[{1, 10}]",
                        idx,
                        connectedDeviceIDs[idx]);
            }
            Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");

            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            Console.WriteLine("Enter index of the readers which are used as >>>>> IN <<<<< device of global apb zone: [0,1,2 ...]");
            Console.Write(">>>> ");
            string[] readerIndexs = Console.ReadLine().Split(delimiterChars);
            for (int idx = 0; idx < readerIndexs.Length; ++idx)
            {
                UInt32 indexOfReader;
                if (UInt32.TryParse(readerIndexs[idx], out indexOfReader) && indexOfReader < numDevice)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS == (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, connectedDeviceIDs[indexOfReader]))
                        zoneManager.inDeviceIDs.Add(connectedDeviceIDs[indexOfReader]);
                }
                else
                {
                    Console.WriteLine("Got error(Invaild index({0})).", readerIndexs[idx]);
                    return;
                }
            }

            Console.WriteLine("Enter index of the readers which are used as <<<<< OUT >>>>> device of global apb zone: [0,1,2 ...]");
            Console.Write(">>>> ");
            readerIndexs = Console.ReadLine().Split(delimiterChars);
            for (int idx = 0; idx < readerIndexs.Length; ++idx)
            {
                UInt32 indexOfReader;
                if (UInt32.TryParse(readerIndexs[idx], out indexOfReader) && indexOfReader < numDevice)
                {
                    if (BS2ErrorCode.BS_SDK_SUCCESS == (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, connectedDeviceIDs[indexOfReader]))
                    {
                        if (!zoneManager.inDeviceIDs.Contains(connectedDeviceIDs[indexOfReader]))
                            zoneManager.outDeviceIDs.Add(connectedDeviceIDs[indexOfReader]);
                        else
                        {
                            Console.WriteLine("Got error(index({0}) is IN device already.).", readerIndexs[idx]);
                            return;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Got error(Invaild index({0})).", readerIndexs[idx]);
                    return;
                }
            }

            Console.WriteLine("Which type is this global apb zone? [{0} : {1}, {2} : {3}(default)]", (byte)BS2APBZoneTypeEnum.HARD, BS2APBZoneTypeEnum.HARD, (byte)BS2APBZoneTypeEnum.SOFT, BS2APBZoneTypeEnum.SOFT);
            Console.Write(">>>> ");
            zoneManager.APBZoneType = (BS2APBZoneTypeEnum)Util.GetInput((byte)BS2APBZoneTypeEnum.SOFT);

            BS2GlobalAPBFailActionTypeEnum networkFailAction = BS2GlobalAPBFailActionTypeEnum.NONE;
            Console.WriteLine("Which network fail action type is this global apb zone? [{0} : {1}(default), {2} : {3}, {4} : {5}]", (byte)BS2GlobalAPBFailActionTypeEnum.NONE, BS2GlobalAPBFailActionTypeEnum.NONE, (byte)BS2GlobalAPBFailActionTypeEnum.SOFT, BS2GlobalAPBFailActionTypeEnum.SOFT, (byte)BS2GlobalAPBFailActionTypeEnum.HARD, BS2GlobalAPBFailActionTypeEnum.HARD);
            Console.Write(">>>> ");
            networkFailAction = (BS2GlobalAPBFailActionTypeEnum)Util.GetInput((byte)BS2GlobalAPBFailActionTypeEnum.NONE);

            Console.WriteLine("Enter the ID for the zone which you want to set");
            Console.Write(">>>> ");
            zoneManager.zoneID = (UInt32)Util.GetInput();

            SortedSet<UInt32> zoneReaders = new SortedSet<UInt32>();
            zoneReaders.UnionWith(zoneManager.inDeviceIDs);
            zoneReaders.UnionWith(zoneManager.outDeviceIDs);
            BS2ErrorCode result = 0;
            foreach (UInt32 zoneReaderID in zoneReaders)
            {
                Console.WriteLine("Now check if the useGlobalAPB option is turned on in the AuthConfig settings.");
                if (!checkAPBOptionInAuthConfig(sdkContext, zoneReaderID, Convert.ToByte(networkFailAction)))
                    return;

                Console.WriteLine("Now check if the apbUseDoorSensor option is turned on in the door settings.");
                if (!checkAPBOptionInDoor(sdkContext, zoneReaderID))
                    return;
            }

            this.sdkContext = sdkContext;
            taskExecutor = new CheckGlobalAPBByDoorOpenTaskExecutor();

            Console.WriteLine("Attaching a global apb by door open handler.");
            cbCheck = new API.OnCheckGlobalAPBViolationByDoorOpen(onCheckGlobalAPBViolationByDoorOpen);
            cbUpdate = new API.OnUpdateGlobalAPBViolationByDoorOpen(onUpdateGlobalAPBViolationByDoorOpen);
            result = (BS2ErrorCode)API.BS2_SetGlobalAPBViolationByDoorOpenHandler(sdkContext, cbCheck, cbUpdate);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            taskExecutor.start();

            Console.WriteLine("Press ESC to stop global apb by door open.");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Thread.Sleep(100);
            }

            taskExecutor.stop();

            Console.WriteLine("Detaching a global apb by door open handler.");
            result = (BS2ErrorCode)API.BS2_SetGlobalAPBViolationByDoorOpenHandler(sdkContext, null, null);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            cbCheck = null;
            taskExecutor = null;
            zoneManager = null;
        }

        public void onCheckGlobalAPBViolationByDoorOpen(UInt32 deviceId, UInt16 seq, string userID1, string userID2, bool isDualAuth)
        {
            CheckGlobalAPBByDoorOpenTask task = new CheckGlobalAPBByDoorOpenTask(sdkContext, deviceId, seq, userID1, userID2, isDualAuth, true, zoneManager);
            taskExecutor.enqueue(task);
        }

        public void onUpdateGlobalAPBViolationByDoorOpen(UInt32 deviceId, UInt16 seq, string userID1, string userID2, bool isDualAuth)
        {
            CheckGlobalAPBByDoorOpenTask task = new CheckGlobalAPBByDoorOpenTask(sdkContext, deviceId, seq, userID1, userID2, isDualAuth, false, zoneManager);
            taskExecutor.enqueue(task);
        }

        public bool checkAPBOptionInAuthConfig(IntPtr sdkContext, UInt32 deviceID, byte networkFailAction)
        {
            BS2AuthConfig authConfig;
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, deviceID, out authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return false;
            }

            const byte On = 1;
            if (authConfig.useGlobalAPB != On)
            {
                authConfig.useGlobalAPB = On;
                authConfig.globalAPBFailAction = networkFailAction;
                result = (BS2ErrorCode)API.BS2_SetAuthConfig(sdkContext, deviceID, ref authConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return false;
                }
            }

            return true;
        }

        public bool checkAPBOptionInDoor(IntPtr sdkContext, UInt32 deviceID)
        {
            IntPtr doorObj = IntPtr.Zero;
            UInt32 numDoor = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Device:{0}, Enter the ID of the door which you want to get: [ID_1,ID_2 ...]", deviceID);
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] doorIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> doorIDList = new List<UInt32>();

            foreach (string doorID in doorIDs)
            {
                if (doorID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(doorID, out item))
                    {
                        doorIDList.Add(item);
                    }
                }
            }

            if (doorIDList.Count > 0)
            {
                IntPtr doorIDObj = Marshal.AllocHGlobal(4 * doorIDList.Count);
                IntPtr curDoorIDObj = doorIDObj;
                foreach (UInt32 item in doorIDList)
                {
                    Marshal.WriteInt32(curDoorIDObj, (Int32)item);
                    curDoorIDObj = (IntPtr)((long)curDoorIDObj + 4);
                }

                Console.WriteLine("Trying to get doors from device.");
                result = (BS2ErrorCode)API.BS2_GetDoor(sdkContext, deviceID, doorIDObj, (UInt32)doorIDList.Count, out doorObj, out numDoor);

                Marshal.FreeHGlobal(doorIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numDoor > 0)
            {
                IntPtr curDoorObj = doorObj;
                int structSize = Marshal.SizeOf(typeof(BS2Door));

                for (int idx = 0; idx < numDoor; ++idx)
                {
                    BS2Door item = (BS2Door)Marshal.PtrToStructure(curDoorObj, typeof(BS2Door));
                    print(deviceID, item);

                    const byte On = 1;
                    if (item.sensor.apbUseDoorSensor != On)
                    {
                        item.sensor.apbUseDoorSensor = On;

                        result = (BS2ErrorCode)API.BS2_SetDoor(sdkContext, deviceID, curDoorObj, 1);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                        }
                    }

                    curDoorObj = (IntPtr)((long)curDoorObj + structSize);
                }

                API.BS2_ReleaseObject(doorObj);

                return true;
            }
            else
            {
                Console.WriteLine(">>> There is no door in the device.");
                Console.WriteLine("    Set the door 1st in the DoorControl.");
                Console.WriteLine("    Set the access group and access level 2nd in the AccessControl.");
                Console.WriteLine("    Set a user 3rd in the UserControl.");
            }

            return false;
        }

        public void print(UInt32 deviceID, BS2Door door)
        {
            Console.WriteLine(">>>> Device[{0}] DoorID[{1}] name:[{2}]", deviceID, door.doorID, Encoding.UTF8.GetString(door.name).TrimEnd('\0'));
            Console.WriteLine("     +--sensor");
            Console.WriteLine("        |--ioDeviceID[{0}]", door.sensor.deviceID);
            Console.WriteLine("        |--port[{0}]]", door.sensor.port);
            Console.WriteLine("        |--switchType[{0}]]", (BS2SwitchTypeEnum)door.sensor.switchType);
            Console.WriteLine("        +--apbUseDoorSensor[{0}]", door.sensor.apbUseDoorSensor);
        }
    }
}
