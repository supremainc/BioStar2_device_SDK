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
    class GlobalAPBZoneManager
    {
        public UInt32 zoneID = 0;
        public SortedSet<UInt32> inDeviceIDs = new SortedSet<UInt32>();
        public SortedSet<UInt32> outDeviceIDs = new SortedSet<UInt32>();
        public BS2APBZoneTypeEnum APBZoneType = BS2APBZoneTypeEnum.SOFT;

        private Dictionary<string, UInt32> lastDevicePerUser = new Dictionary<string, UInt32>();

        public BS2ErrorCode checkAPBZoneRule(UInt32 deviceID, string userID_1, string userID_2, bool isDualAuth, out UInt32 zoneID)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            if (!inDeviceIDs.Contains(deviceID) && !outDeviceIDs.Contains(deviceID))
            {
                zoneID = 0;
                result = BS2ErrorCode.BS_SDK_SUCCESS;
            }
            else
            { 
                //BS_SDK_ERROR_HARD_APB_VIOLATION;
                //BS_SDK_ERROR_SOFT_APB_VIOLATION;
                //BS_SDK_ERROR_HARD_TIMED_APB_VIOLATION;
                //BS_SDK_ERROR_SOFT_TIMED_APB_VIOLATION;
                if (lastDevicePerUser.ContainsKey(userID_1))
                {
                    if (inDeviceIDs.Contains(lastDevicePerUser[userID_1]) && inDeviceIDs.Contains(deviceID))
                        result = APBZoneType == BS2APBZoneTypeEnum.SOFT ? BS2ErrorCode.BS_SDK_ERROR_SOFT_APB_VIOLATION : BS2ErrorCode.BS_SDK_ERROR_HARD_APB_VIOLATION;
                    else if (outDeviceIDs.Contains(lastDevicePerUser[userID_1]) && outDeviceIDs.Contains(deviceID))
                        result = APBZoneType == BS2APBZoneTypeEnum.SOFT ? BS2ErrorCode.BS_SDK_ERROR_SOFT_APB_VIOLATION : BS2ErrorCode.BS_SDK_ERROR_HARD_APB_VIOLATION;
                    else
                        result = BS2ErrorCode.BS_SDK_SUCCESS;
                }
                else
                {
                    //First 
                    result = BS2ErrorCode.BS_SDK_SUCCESS;
                }

                zoneID = this.zoneID;

                switch ((BS2ErrorCode)result)
                {
                    case BS2ErrorCode.BS_SDK_SUCCESS:
                    case BS2ErrorCode.BS_SDK_ERROR_SOFT_APB_VIOLATION:
                    case BS2ErrorCode.BS_SDK_ERROR_SOFT_TIMED_APB_VIOLATION:
                        lastDevicePerUser[userID_1] = deviceID;
                        break;
                }
            }

            return result;
        }
        
    }

    abstract class BaseTask
    {
        public abstract void execute();
    }

    class CheckGlobalAPBViolationTask: BaseTask
    {
        GlobalAPBZoneManager zoneManager;
        IntPtr sdkContext;
        UInt32 deviceID;
        UInt16 seq;
        string userID_1;
        string userID_2;
        bool isDualAuth;

        public CheckGlobalAPBViolationTask(IntPtr sdkContext, UInt32 deviceID, UInt16 seq, string userID_1, string userID_2, bool isDualAuth, GlobalAPBZoneManager zoneManager)
        {
            this.sdkContext = sdkContext;
            this.deviceID = deviceID;
            this.seq = seq;
            this.userID_1 = userID_1;
            this.userID_2 = userID_2;
            this.isDualAuth = isDualAuth;
            this.zoneManager = zoneManager;
        }

        public override void execute()
        {
            UInt32 zoneID = 0;
            BS2ErrorCode handleResult = zoneManager.checkAPBZoneRule(deviceID, userID_1, userID_2, isDualAuth, out zoneID);
            Console.WriteLine("[Server] responded with a status of {0}, zoneID[{6}] : device[{1}] seq[{2}], userID_1[{3}], userID_2[{4}], isDualAuth[{5}]", handleResult, deviceID, seq, userID_1, userID_2, isDualAuth, zoneID);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_CheckGlobalAPBViolation(sdkContext, deviceID, seq, (int)handleResult, zoneID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }
    }
    
    class CheckGlobalAPBViolationTaskExecutor : IDisposable
    {
        private bool running;
        private Thread thread;
        private readonly object locker = new object();
        private EventWaitHandle eventWaitHandle = new AutoResetEvent(false);
        private Queue<BaseTask> taskQueue = new Queue<BaseTask>();

        public CheckGlobalAPBViolationTaskExecutor()
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

    public class GlobalAPBZoneControl : FunctionModule
    {
        private API.OnCheckGlobalAPBViolation cbCheckGlobalAPBViolation = null;
        private CheckGlobalAPBViolationTaskExecutor taskExecutor = null;
        private GlobalAPBZoneManager zoneManager = null;
        private IntPtr sdkContext;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Start global apb zone", StarGlobalAPBZone));
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
        
        public void StarGlobalAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            zoneManager = new GlobalAPBZoneManager();

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
                    if (!zoneManager.inDeviceIDs.Contains(connectedDeviceIDs[indexOfReader]))
                        zoneManager.outDeviceIDs.Add(connectedDeviceIDs[indexOfReader]);
                    else
                    {
                        Console.WriteLine("Got error(index({0}) is IN device already.).", readerIndexs[idx]);
                        return;
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
                BS2AuthConfig authConfig;
                Console.WriteLine("Getting auth config. reader[{0}]", zoneReaderID);
                result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, zoneReaderID, out authConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }
                authConfig.useGlobalAPB = 1;
                authConfig.globalAPBFailAction = (byte)networkFailAction;
                Console.WriteLine("Setting auth config. reader[{0}]", zoneReaderID);
                result = (BS2ErrorCode)API.BS2_SetAuthConfig(sdkContext, zoneReaderID, ref authConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }
            }

            this.sdkContext = sdkContext;
            taskExecutor = new CheckGlobalAPBViolationTaskExecutor();

            Console.WriteLine("Attaching a global apb zone handler.");
            cbCheckGlobalAPBViolation = new API.OnCheckGlobalAPBViolation(onCheckGlobalAPBViolation);
            result = (BS2ErrorCode)API.BS2_SetCheckGlobalAPBViolationHandler(sdkContext, cbCheckGlobalAPBViolation);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            taskExecutor.start();

            Console.WriteLine("Press ESC to stop global apb zone.");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Thread.Sleep(100);
            }

            taskExecutor.stop();

            Console.WriteLine("Detaching a global apb zone handler.");
            result = (BS2ErrorCode)API.BS2_SetCheckGlobalAPBViolationHandler(sdkContext, null);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            cbCheckGlobalAPBViolation = null;
            taskExecutor = null;
            zoneManager = null;
        }

        public void onCheckGlobalAPBViolation(UInt32 deviceId, UInt16 seq, string userID_1, string userID_2, bool isDualAuth)
        {
            CheckGlobalAPBViolationTask task = new CheckGlobalAPBViolationTask(sdkContext, deviceId, seq, userID_1, userID_2, isDualAuth, zoneManager);
            taskExecutor.enqueue(task);
        }        
    }
}
