//#define SDK_AUTO_CONNECTION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Globalization;
using System.Diagnostics;
using System.Threading;

namespace Suprema
{
#if !SDK_AUTO_CONNECTION
    class ReconnectionTask : IDisposable
    {
        private IntPtr sdkContext;
        private bool running;
        private Thread thread;
        private readonly object locker = new object();
        private EventWaitHandle eventWaitHandle = new AutoResetEvent(false);
        private Queue<UInt32> deviceIDQueue = new Queue<UInt32>();

        public ReconnectionTask(IntPtr sdkContext)
        {
            this.sdkContext = sdkContext;
            thread = new Thread(run);
        }

        public void enqueue(UInt32 deviceID)
        {
            bool isAlreadyRequested = false;

            lock (locker)
            {
                foreach (UInt32 targetDeviceID in deviceIDQueue)
                {
                    if (targetDeviceID == deviceID)
                    {
                        isAlreadyRequested = true;
                        break;
                    }
                }

                if (!isAlreadyRequested)
                {
                    deviceIDQueue.Enqueue(deviceID);
                }
            }

            if (!isAlreadyRequested)
            {
                Console.WriteLine("enqueue Device[{0, 10}].", deviceID);
                eventWaitHandle.Set();
            }
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
                    deviceIDQueue.Clear();
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
                UInt32 deviceID = 0;

                lock (locker)
                {
                    if (deviceIDQueue.Count > 0)
                    {
                        deviceID = deviceIDQueue.Dequeue();
                    }
                }

                if (deviceID != 0)
                {
                    Console.WriteLine("trying to reconnect Device[{0, 10}].", deviceID);
                   
                    /*
                    BS2ErrorCode result = new BS2ErrorCode();
                    while (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        //result = (BS2ErrorCode)API.BS2_DisconnectDevice(sdkContext, deviceID);
                        result = (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, deviceID);
                        if (result != BS2ErrorCode.BS_SDK_ERROR_CANNOT_CONNECT_SOCKET)
                        {
                            Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                        }
                        else
                        {
                            enqueue(deviceID);
                        }

                    }
                    */
                    
                    /*
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        if (result != BS2ErrorCode.BS_SDK_ERROR_CANNOT_CONNECT_SOCKET)
                        {
                            //Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                            return;
                        }
                        else
                        {
                            enqueue(deviceID);
                        }
                    }
                    */            
                    
                    //원본
                    BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, deviceID);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        if (result != BS2ErrorCode.BS_SDK_ERROR_CANNOT_CONNECT_SOCKET)
                        {
                            Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                            return;
                        }
                        else
                        {
                            enqueue(deviceID);
                        }
                    }       
                    
                     
                }
                else
                {
                    eventWaitHandle.WaitOne();
                }
            }
        }
    }
#endif

    public abstract class UnitTest
    {
        private string title;
        private API.OnDeviceFound cbOnDeviceFound = null;
        private API.OnDeviceAccepted cbOnDeviceAccepted = null;
        private API.OnDeviceConnected cbOnDeviceConnected = null;
        private API.OnDeviceDisconnected cbOnDeviceDisconnected = null;
        protected IntPtr sdkContext = IntPtr.Zero;
#if !SDK_AUTO_CONNECTION
        private ReconnectionTask reconnectionTask = null;
#endif
        private UInt32 deviceIDForServerMode = 0;
        private EventWaitHandle eventWaitHandle = new AutoResetEvent(false);

        private API.PreferMethod cbPreferMethod = null;
        private API.GetRootCaFilePath cbGetRootCaFilePath = null;
        private API.GetServerCaFilePath cbGetServerCaFilePath = null;
        private API.GetServerPrivateKeyFilePath cbGetServerPrivateKeyFilePath = null;
        private API.GetPassword cbGetPassword = null;
        private API.OnErrorOccured cbOnErrorOccured = null;
        
        private string ssl_server_root_crt = "../../../../../resource/server/ssl_server_root.crt";
        private string ssl_server_crt = "../../../../../resource/server/ssl_server.crt";
        private string ssl_server_pem = "../../../../../resource/server/ssl_server.pem";
        private string ssl_server_passwd = "supremaserver";
        private API.OnSendRootCA cbOnSendRootCA = null;
        private API.CBDebugExPrint cbDebugExPrint = null;

        private IntPtr ptr_server_root_crt = IntPtr.Zero;
        private IntPtr ptr_server_crt = IntPtr.Zero;
        private IntPtr ptr_server_pem = IntPtr.Zero;
        private IntPtr ptr_server_passwd = IntPtr.Zero;

        protected abstract void runImpl(UInt32 deviceID);

        protected string Title {
            get
            {
                return title;
            }
            set
            {
                title = value;
                Console.Title = value;
            }
        }

        public UnitTest()
        {
            if (Console.WindowWidth < 150)
            {
                Console.WindowWidth = 150;
            }

            AppDomain.CurrentDomain.ProcessExit += (object s, EventArgs args) =>
            {
                if (sdkContext != IntPtr.Zero)
                {
                    API.BS2_ReleaseContext(sdkContext);
                    sdkContext = IntPtr.Zero;
                }
            };
        }

        ~UnitTest()
        {
            if (ptr_server_root_crt != IntPtr.Zero)
                Marshal.FreeHGlobal(ptr_server_root_crt);
            if (ptr_server_crt != IntPtr.Zero)
                Marshal.FreeHGlobal(ptr_server_crt);
            if (ptr_server_pem != IntPtr.Zero)
                Marshal.FreeHGlobal(ptr_server_pem);
            if (ptr_server_passwd != IntPtr.Zero)
                Marshal.FreeHGlobal(ptr_server_passwd);
        }

        void printStructureSize<T>()
        {
            Console.WriteLine("{0} size : {1}", typeof(T), Marshal.SizeOf(typeof(T)));
        }

        public void run()
        {            
            UInt32 deviceID = 0;
            IntPtr versionPtr = API.BS2_Version();
            //bool bSsl = false;

            if (title.Length > 0)
            {
                Console.Title = title;
            }

            Console.WriteLine("SDK version : " + Marshal.PtrToStringAnsi(versionPtr));
            
            sdkContext = API.BS2_AllocateContext();
            if (sdkContext == IntPtr.Zero)
            {
                Console.WriteLine("Can't allocate sdk context.");
                return;
            }

            Console.WriteLine("Do you want to set up ssl configuration? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                cbPreferMethod = new API.PreferMethod(PreferMethodHandle);
                cbGetRootCaFilePath = new API.GetRootCaFilePath(GetRootCaFilePathHandle);
                cbGetServerCaFilePath = new API.GetServerCaFilePath(GetServerCaFilePathHandle);
                cbGetServerPrivateKeyFilePath = new API.GetServerPrivateKeyFilePath(GetServerPrivateKeyFilePathHandle);
                cbGetPassword = new API.GetPassword(GetPasswordHandle);
                cbOnErrorOccured = new API.OnErrorOccured(OnErrorOccuredHandle);
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)SecurityProtocolType.Ssl3;

                BS2ErrorCode sdkResult = (BS2ErrorCode)API.BS2_SetSSLHandler(sdkContext, cbPreferMethod, cbGetRootCaFilePath, cbGetServerCaFilePath, cbGetServerPrivateKeyFilePath, cbGetPassword, null);
                if (sdkResult != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("BS2_SetSSLHandler failed with : {0}", sdkResult);
                    API.BS2_ReleaseContext(sdkContext);
                    sdkContext = IntPtr.Zero;
                    return;
                }
                else
                {
                    //bSsl = true;
                }

            }
            
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_Initialize(sdkContext);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("SDK initialization failed with : {0}", result);
                API.BS2_ReleaseContext(sdkContext);
                sdkContext = IntPtr.Zero;
                return;
            }

            cbOnDeviceFound = new API.OnDeviceFound(DeviceFound);
            cbOnDeviceAccepted = new API.OnDeviceAccepted(DeviceAccepted);
            cbOnDeviceConnected = new API.OnDeviceConnected(DeviceConnected);
            cbOnDeviceDisconnected = new API.OnDeviceDisconnected(DeviceDisconnected);

            result = (BS2ErrorCode)API.BS2_SetDeviceEventListener(sdkContext,
                                                                cbOnDeviceFound,
                                                                cbOnDeviceAccepted,
                                                                cbOnDeviceConnected,
                                                                cbOnDeviceDisconnected);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't register a callback function/method to a sdk.({0})", result);
                API.BS2_ReleaseContext(sdkContext);
                sdkContext = IntPtr.Zero;
                return;
            }

            /*
            if (bSsl)
            {
                cbOnSendRootCA = new API.OnSendRootCA(SendRootCA);
                result = (BS2ErrorCode)API.BS2_SetDeviceSSLEventListener(sdkContext, cbOnSendRootCA);
            }
            */

            //cbDebugExPrint = null;
            //Console.WriteLine("Do you want print debug message? [y/n]");
            Console.WriteLine("Do you want output debug message to file? [y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                //cbDebugExPrint = new API.CBDebugExPrint(DebugExPrint);
                //result = (BS2ErrorCode)API.BS2_SetDebugExCallback(cbDebugExPrint, Constants.DEBUG_LOG_ALL, Constants.DEBUG_MODULE_ALL);
                //if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                //{
                //    Console.WriteLine("Got error({0}).", result);
                //    return;
                //}

                const string CURRENT_DIR = ".";
                IntPtr ptrDir = Marshal.StringToHGlobalAnsi(CURRENT_DIR);
                result = (BS2ErrorCode)API.BS2_SetDebugFileLog(Constants.DEBUG_LOG_ALL, Constants.DEBUG_MODULE_ALL, ptrDir);
                Marshal.FreeHGlobal(ptrDir);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
	                API.BS2_ReleaseContext(sdkContext);
	                sdkContext = IntPtr.Zero;
                    return;
                }

                //Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                //Trace.AutoFlush = true;
                //Trace.Indent();
            }

#if SDK_AUTO_CONNECTION
            result = (BS2ErrorCode)API.BS2_SetAutoConnection(sdkContext, 1);
#endif

            Console.WriteLine("+-----------------------------------------------------------+");
            Console.WriteLine("| 1. Search and connect device                              |");
            Console.WriteLine("| 2. Connect to device via Ip                               |");
            Console.WriteLine("| 3. Server mode test                                       |");
            Console.WriteLine("+-----------------------------------------------------------+");
            Console.WriteLine("How to connect to device? [2(default)]");
            Console.Write(">>>> ");
            int selection = Util.GetInput(2);

            switch (selection)
            {
                case 1:
                    if (!SearchAndConnectDevice(ref deviceID))
                    {
                        deviceID = 0;
                    }
                    break;
                case 2:
                    if (!ConnectToDevice(ref deviceID))
                    {
                        deviceID = 0;
                    }
                    break;
                case 3:
                    {
                        if(deviceIDForServerMode == 0)
                        {
                            Console.WriteLine("Waiting for client connection");
                            eventWaitHandle.WaitOne();
                        }

                        
                        deviceID = deviceIDForServerMode;

                        /*
                        result = (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, deviceID);

                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                            deviceID = 0;
                        }
                        else
                        {
                            Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
                        }
                         */

                    }
                    break;
                default:
                    Console.WriteLine("Invalid parameter : {0}", selection);
                    break;
            }

            if (deviceID > 0)
            {                
                Console.Title = String.Format("{0} connected deviceID[{1}]", title, deviceID);

#if !SDK_AUTO_CONNECTION
                reconnectionTask = new ReconnectionTask(sdkContext);
                reconnectionTask.start();
#endif
                runImpl(deviceID);
#if !SDK_AUTO_CONNECTION
                reconnectionTask.stop();
                reconnectionTask = null;
#endif

                Console.WriteLine("Trying to discconect device[{0}].", deviceID);
                result = (BS2ErrorCode)API.BS2_DisconnectDevice(sdkContext, deviceID);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
            }


            eventWaitHandle.Close();
            API.BS2_ReleaseContext(sdkContext);
            sdkContext = IntPtr.Zero;

            cbOnDeviceFound = null;
            cbOnDeviceAccepted = null;
            cbOnDeviceConnected = null;
            cbOnDeviceDisconnected = null;
            cbOnSendRootCA = null;
        }

        public void runWithoutConnection()
        {
            UInt32 deviceID = 0;
            IntPtr versionPtr = API.BS2_Version();
            //bool bSsl = false;

            if (title.Length > 0)
            {
                Console.Title = title;
            }

            Console.WriteLine("SDK version : " + Marshal.PtrToStringAnsi(versionPtr));

            sdkContext = API.BS2_AllocateContext();
            if (sdkContext == IntPtr.Zero)
            {
                Console.WriteLine("Can't allocate sdk context.");
                return;
            }

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_Initialize(sdkContext);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("SDK initialization failed with : {0}", result);
                API.BS2_ReleaseContext(sdkContext);
                sdkContext = IntPtr.Zero;
                return;
            }

            runImpl(deviceID);

            API.BS2_ReleaseContext(sdkContext);
            sdkContext = IntPtr.Zero;

            cbOnDeviceFound = null;
            cbOnDeviceAccepted = null;
            cbOnDeviceConnected = null;
            cbOnDeviceDisconnected = null;
            cbOnSendRootCA = null;
        }

        bool SearchAndConnectDevice(ref UInt32 deviceID)
        {
            Console.WriteLine("Trying to broadcast on the network");           

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SearchDevices(sdkContext);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error : {0}.", result);
                return false;
            }

            IntPtr deviceListObj = IntPtr.Zero;
            UInt32 numDevice = 0;

            const UInt32 LONG_TIME_STANDBY_7S = 7;
            result = (BS2ErrorCode)API.BS2_SetDeviceSearchingTimeout(sdkContext, LONG_TIME_STANDBY_7S);
            if (BS2ErrorCode.BS_SDK_SUCCESS != result)
            {
                Console.WriteLine("Got error : {0}.", result);
                return false;
            }

            result = (BS2ErrorCode)API.BS2_GetDevices(sdkContext, out deviceListObj, out numDevice);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error : {0}.", result);
                return false;
            }

            if (numDevice > 0)
            {
                BS2SimpleDeviceInfo deviceInfo;

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                for (UInt32 idx = 0; idx < numDevice; ++idx)
                {
                    deviceID = Convert.ToUInt32(Marshal.ReadInt32(deviceListObj, (int)idx * sizeof(UInt32)));
                    result = (BS2ErrorCode)API.BS2_GetDeviceInfo(sdkContext, deviceID, out deviceInfo);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Can't get device information(errorCode : {0}).", result);
                        return false;
                    }

                    Console.WriteLine("[{0, 3:##0}] ==> ID[{1, 10}] Type[{2, 16}] Connection mode[{3}] Ip[{4, 16}] port[{5, 5}]",
                            idx,
                            deviceID,                            
                            API.productNameDictionary.ContainsKey((BS2DeviceTypeEnum)deviceInfo.type) ?  API.productNameDictionary[(BS2DeviceTypeEnum)deviceInfo.type] : (API.productNameDictionary[BS2DeviceTypeEnum.UNKNOWN] + "(" + deviceInfo.type + ")"),
                            (BS2ConnectionModeEnum)deviceInfo.connectionMode,
                            new IPAddress(BitConverter.GetBytes(deviceInfo.ipv4Address)).ToString(),
                            deviceInfo.port);
                }
                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("Please, choose the index of the Device which you want to connect to. [-1: quit]");
                Console.Write(">>>> ");

                deviceID = 0;
                Int32 selection = Util.GetInput();

                if (selection >= 0)
                {
                    if (selection < numDevice)
                    {
                        deviceID = Convert.ToUInt32(Marshal.ReadInt32(deviceListObj, (int)selection * sizeof(UInt32)));
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection[{0}]", selection);
                    }
                }

                API.BS2_ReleaseObject(deviceListObj);
                if (deviceID > 0)
                {
                    Console.WriteLine("Trying to connect to device[{0}]", deviceID);                    
                    result = (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, deviceID);                    

                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                        return false;
                    }

                    Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
                    return true;
                }
            }
            else
            {
                Console.WriteLine("There is no device to launch.");
            }

            return false;
        }

        bool ConnectToDevice(ref UInt32 deviceID)
        {
            Console.WriteLine("Enter the IP Address to connect device");
            Console.Write(">>>> ");
            string deviceIpAddress = Console.ReadLine();
            IPAddress ipAddress;

            if (!IPAddress.TryParse(deviceIpAddress, out ipAddress))
            {
                Console.WriteLine("Invalid ip : " + deviceIpAddress);
                return false;
            }

            Console.WriteLine("Enter the port number to connect device : default[{0}]", BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT);
            Console.Write(">>>> ");
            UInt16 port = Util.GetInput((UInt16)BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT);

            Console.WriteLine("Trying to connect to device [ip :{0}, port : {1}]", deviceIpAddress, port);


            IntPtr ptrIPAddr = Marshal.StringToHGlobalAnsi(deviceIpAddress);
            //BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, deviceIpAddress, port, out deviceID);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, ptrIPAddr, port, out deviceID);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                return false;
            }
            Marshal.FreeHGlobal(ptrIPAddr);

            Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
            return true;
        }

        bool ConnectToDeviceSSL(string deviceIpAddress, ref UInt32 deviceID)
        {                
            UInt16 port = Util.GetInput((UInt16)BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT);

            int nCnt = 0;
            while (true)
            {
                IntPtr ptrIPAddr = Marshal.StringToHGlobalAnsi(deviceIpAddress);
                //BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, deviceIpAddress, port, out deviceID);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, ptrIPAddr, port, out deviceID);
                Marshal.FreeHGlobal(ptrIPAddr);

                if(nCnt > 7)
                {
                    Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                    return false;
                }

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    nCnt++;
                    continue;
                }
                else
                    break;
            }

            Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
            return true;
        }       

        void PrintDeviceInfo(BS2SimpleDeviceInfo deviceInfo)
        {
            Console.WriteLine("                        <Device information>");
            Console.WriteLine("+-------------------------------------------------------------+");
            Console.WriteLine("|  ID                                : {0}", deviceInfo.id);
            Console.WriteLine("|  Type                              : {0}({1})", API.productNameDictionary.ContainsKey((BS2DeviceTypeEnum)deviceInfo.type) ? API.productNameDictionary[(BS2DeviceTypeEnum)deviceInfo.type] : API.productNameDictionary[BS2DeviceTypeEnum.UNKNOWN], deviceInfo.type);
            Console.WriteLine("|  Connection mode                   : {0}", (BS2ConnectionModeEnum)deviceInfo.connectionMode);
            Console.WriteLine("|  Ip address                        : {0}", new IPAddress(BitConverter.GetBytes(deviceInfo.ipv4Address)).ToString());
            Console.WriteLine("|  Port number                       : {0}", deviceInfo.port);
            Console.WriteLine("|  Maximum user                      : {0}", deviceInfo.maxNumOfUser);
            Console.WriteLine("|  Supporting user name              : {0}", Convert.ToBoolean(deviceInfo.userNameSupported));
            Console.WriteLine("|  Supporting user profile           : {0}", Convert.ToBoolean(deviceInfo.userPhotoSupported));
            Console.WriteLine("|  Supporting pin code               : {0}", Convert.ToBoolean(deviceInfo.pinSupported));
            Console.WriteLine("|  Supporting card                   : {0}", Convert.ToBoolean(deviceInfo.cardSupported));
            Console.WriteLine("|  Supporting fingerprint            : {0}", Convert.ToBoolean(deviceInfo.fingerSupported));
            Console.WriteLine("|  Supporting face recognition       : {0}", Convert.ToBoolean(deviceInfo.faceSupported));
            Console.WriteLine("|  Supporting wlan                   : {0}", Convert.ToBoolean(deviceInfo.wlanSupported));
            Console.WriteLine("|  Supporting T&A                    : {0}", Convert.ToBoolean(deviceInfo.tnaSupported));
            Console.WriteLine("|  Supporting trigger action         : {0}", Convert.ToBoolean(deviceInfo.triggerActionSupported));
            Console.WriteLine("|  Supporting wiegand                : {0}", Convert.ToBoolean(deviceInfo.wiegandSupported));
            Console.WriteLine("+-------------------------------------------------------------+");
        }

        void DeviceFound(UInt32 deviceID)
        {
            Console.WriteLine("[CB] Device[{0, 10}] has been found.", deviceID);
        }

        void DeviceAccepted(UInt32 deviceID)
        {
            Console.WriteLine("[CB] Device[{0, 10}] has been accepted.", deviceID);
            deviceIDForServerMode = deviceID;
            eventWaitHandle.Set();
        }

        void DeviceConnected(UInt32 deviceID)
        {
            Console.WriteLine("[CB] Device[{0, 10}] has been connected.", deviceID);
        }

        void DeviceDisconnected(UInt32 deviceID)
        {
            Console.WriteLine("[CB] Device[{0, 10}] has been disconnected.", deviceID);
#if !SDK_AUTO_CONNECTION
            if (reconnectionTask != null)
            {
                Console.WriteLine("enqueue");
                reconnectionTask.enqueue(deviceID);

            }
#endif
        }

        UInt32 PreferMethodHandle(UInt32 deviceID)
        {
            return (UInt32)(BS2SslMethodMaskEnum.TLS1 | BS2SslMethodMaskEnum.TLS1_1 | BS2SslMethodMaskEnum.TLS1_2);
        }

        IntPtr GetRootCaFilePathHandle(UInt32 deviceID)
        {
            //return ssl_server_root_crt;
            if (ptr_server_root_crt == IntPtr.Zero)
                ptr_server_root_crt = Marshal.StringToHGlobalAnsi(ssl_server_root_crt);
            return ptr_server_root_crt;
        }

        IntPtr GetServerCaFilePathHandle(UInt32 deviceID)
        {
            //return ssl_server_crt;
            if (ptr_server_crt == IntPtr.Zero)
                ptr_server_crt = Marshal.StringToHGlobalAnsi(ssl_server_crt);
            return ptr_server_crt;
        }

        IntPtr GetServerPrivateKeyFilePathHandle(UInt32 deviceID)
        {
            //return ssl_server_pem;
            if (ptr_server_pem == IntPtr.Zero)
                ptr_server_pem = Marshal.StringToHGlobalAnsi(ssl_server_pem);
            return ptr_server_pem;
        }

        IntPtr GetPasswordHandle(UInt32 deviceID)
        {
            //return ssl_server_passwd;
            if (ptr_server_passwd == IntPtr.Zero)
                ptr_server_passwd = Marshal.StringToHGlobalAnsi(ssl_server_passwd);
            return ptr_server_passwd;
        }

        void OnErrorOccuredHandle(UInt32 deviceID, int errCode)
        {
            Console.WriteLine("Got ssl error{0} Device[{1, 10}].", (BS2ErrorCode)errCode, deviceID);
        }

        void SendRootCA(int result)
        {
            if (result == 1)
                Console.WriteLine("send RootCA Success!!\n");
            else
                Console.WriteLine("send RootCA Fail!!\n");
            
        }

        private void DebugExPrint(UInt32 level, UInt32 module, string msg)
        {
            //string printmsg = String.Format("[{0}-{1}] {2}", getModuleName(module), getLevelName(level), msg);
            string printmsg = String.Format("{0}", msg);
            //Trace.WriteLine(printmsg);
            Console.WriteLine(printmsg);
        }

        private string getModuleName(UInt32 module)
        {
            switch (module)
            {
            case Constants.DEBUG_MODULE_KEEP_ALIVE:         return "KAV";
            case Constants.DEBUG_MODULE_SOCKET_MANAGER:     return "SOM";
            case Constants.DEBUG_MODULE_SOCKET_HANDLER:     return "SOH";
            case Constants.DEBUG_MODULE_DEVICE:             return "DEV";
            case Constants.DEBUG_MODULE_DEVICE_MANAGER:     return "DVM";
            case Constants.DEBUG_MODULE_EVENT_DISPATCHER:   return "DIS";
            case Constants.DEBUG_MODULE_API:                return "API";
            case Constants.DEBUG_MODULE_ALL:                return "ALL";
            }

            return "UnK";
        }

        private string getLevelName(UInt32 level)
        {
            switch (level)
            {
                case Constants.DEBUG_LOG_FATAL:             return "FAT";
                case Constants.DEBUG_LOG_ERROR:             return "ERR";
                case Constants.DEBUG_LOG_WARN:              return "WRN";
                case Constants.DEBUG_LOG_INFO:              return "INF";
                case Constants.DEBUG_LOG_TRACE:             return "TRC";
                case Constants.DEBUG_LOG_ALL:               return "ALL";
            }

            return "UnK";
        }


        private string ToStringYesNo(bool value)
        {
            return value ? "y" : "n";
        }

        public bool noConnectionMode = false;
        public void runWithIPv6()
        {            
            int delayTerminate = 0;
            UInt32 deviceID = 0;
            IntPtr versionPtr = API.BS2_Version();
            //bool bSsl = false;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            if (title.Length > 0)
            {
                Console.Title = title;
            }

            Console.WriteLine("SDK version : " + Marshal.PtrToStringAnsi(versionPtr));

            cbDebugExPrint = null;
            Console.WriteLine("Do you want print debug message? [y/N]");
            Console.Write(">>>> ");
            if (!Util.IsNo())
            {
                cbDebugExPrint = new API.CBDebugExPrint(DebugExPrint);
                result = (BS2ErrorCode)API.BS2_SetDebugExCallback(cbDebugExPrint, Constants.DEBUG_LOG_ALL, Constants.DEBUG_MODULE_ALL);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("SetDebugExCallback: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }
            }

            sdkContext = API.BS2_AllocateContext();
            if (sdkContext == IntPtr.Zero)
            {
                Console.WriteLine("Can't allocate sdk context.");
                ClearSDK(delayTerminate);
                return;
            }

            Int32 responseTimeoutMs = 0;
            result = (BS2ErrorCode)API.BS2_GetDefaultResponseTimeout(sdkContext, out responseTimeoutMs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("GetDefaultResponseTimeout: Got error({0}).", result);
                ClearSDK(delayTerminate);
                return;
            }
            Console.WriteLine("How long do you have to wait by default for response time? [{0} ms (Default)]", responseTimeoutMs);
            Console.Write(">>>> ");
            responseTimeoutMs = (Int32)(Util.GetInput((UInt32)responseTimeoutMs));
            result = (BS2ErrorCode)API.BS2_SetDefaultResponseTimeout(sdkContext, responseTimeoutMs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("SetDefaultResponseTimeout: Got error({0}).", result);
                ClearSDK(delayTerminate);
                return;
            }


            int IPv4 = 1;
            int IPv6 = 0;
            result = (BS2ErrorCode)API.BS2_GetEnableIPV4(sdkContext, out IPv4);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("GetEnableIPV4: Got error({0}).", result);
                ClearSDK(delayTerminate);
                return;
            }
            result = (BS2ErrorCode)API.BS2_GetEnableIPV6(sdkContext, out IPv6);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("GetEnableIPV4: Got error({0}).", result);
                ClearSDK(delayTerminate);
                return;
            }

            while (true) 
            { 
                Console.WriteLine("What do you want to be active between IPv4 and IPv6? [0(IPv4: Default), 1(IPv6), 2(Both)]");
                Console.Write(">>>> ");
                byte choiceIP = Util.GetInput((byte)0);
                if (choiceIP == 0)
                {
                    IPv4 = 1;
                    IPv6 = 0;
                }
                else if (choiceIP == 1)
                {
                    IPv4 = 0;
                    IPv6 = 1;
                }
                else if (choiceIP == 2)
                {
                    IPv4 = 1;
                    IPv6 = 1;
                }
                else
                {
                    Console.WriteLine("Wrong selection");
                    continue;
                }
                break;
            }

            if (IPv4 == 1)
            {
                UInt16 port = BS2Environment.BS2_TCP_SERVER_PORT_DEFAULT;
                result = (BS2ErrorCode)API.BS2_GetServerPort(sdkContext, out port);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("GetServerPort: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }

                Console.WriteLine("What server port number will you use in IPv4? [{0} Default]", port);
                Console.Write(">>>> ");
                port = Util.GetInput((UInt16)port);
                result = (BS2ErrorCode)API.BS2_SetServerPort(sdkContext, port);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("SetServerPort: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }
            }

            if (IPv6 == 1)
            {
                UInt16 port = BS2Environment.BS2_TCP_SERVER_PORT_DEFAULT_V6;
                result = (BS2ErrorCode)API.BS2_GetServerPortIPV6(sdkContext, out port);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("GetServerPortIPV6: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }

                Console.WriteLine("What server port number will you use in IPv6? [{0} Default]", port);
                Console.Write(">>>> ");
                port = Util.GetInput((UInt16)port);
                result = (BS2ErrorCode)API.BS2_SetServerPortIPV6(sdkContext, port);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("SetServerPortIPV6: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }
            }

	        result = (BS2ErrorCode)API.BS2_SetEnableIPV4(sdkContext, IPv4);
	        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
	        {
	            Console.WriteLine("SetEnableIPV4: Got error({0}).", result);
                ClearSDK(delayTerminate);
                return;
	        }

	        result = (BS2ErrorCode)API.BS2_SetEnableIPV6(sdkContext, IPv6);
	        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
	        {
	            Console.WriteLine("SetEnableIPV6: Got error({0}).", result);
                ClearSDK(delayTerminate);
                return;
	        }

            Console.WriteLine("Do you want to set up ssl configuration? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                if (IPv4 == 1)
                {
                    UInt16 sslPort = BS2Environment.BS2_TCP_SSL_SERVER_PORT_DEFAULT;
                    result = (BS2ErrorCode)API.BS2_GetSSLServerPort(sdkContext, out sslPort);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("GetSSLServerPort: Got error({0}).", result);
                        ClearSDK(delayTerminate);
                        return;
                    }

                    Console.WriteLine("What ssl server port number will you use in IPv4? [{0} Default]", sslPort);
                    Console.Write(">>>> ");
                    sslPort = Util.GetInput((UInt16)sslPort);
                    result = (BS2ErrorCode)API.BS2_SetSSLServerPort(sdkContext, sslPort);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("SetSSLServerPort: Got error({0}).", result);
                        ClearSDK(delayTerminate);
                        return;
                    }
                }

                if (IPv6 == 1)
                {
                    UInt16 sslPort = BS2Environment.BS2_TCP_SSL_SERVER_PORT_DEFAULT_V6;
                    result = (BS2ErrorCode)API.BS2_GetSSLServerPortIPV6(sdkContext, out sslPort);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("GetSSLServerPortIPV6: Got error({0}).", result);
                        ClearSDK(delayTerminate);
                        return;
                    }

                    Console.WriteLine("What ssl server port number will you use in IPv6? [{0} Default]", sslPort);
                    Console.Write(">>>> ");
                    sslPort = Util.GetInput((UInt16)sslPort);
                    result = (BS2ErrorCode)API.BS2_SetSSLServerPortIPV6(sdkContext, sslPort);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("SetSSLServerPortIPV6: Got error({0}).", result);
                        ClearSDK(delayTerminate);
                        return;
                    }
                }

                cbPreferMethod = new API.PreferMethod(PreferMethodHandle);
                cbGetRootCaFilePath = new API.GetRootCaFilePath(GetRootCaFilePathHandle);
                cbGetServerCaFilePath = new API.GetServerCaFilePath(GetServerCaFilePathHandle);
                cbGetServerPrivateKeyFilePath = new API.GetServerPrivateKeyFilePath(GetServerPrivateKeyFilePathHandle);
                cbGetPassword = new API.GetPassword(GetPasswordHandle);
                cbOnErrorOccured = new API.OnErrorOccured(OnErrorOccuredHandle);
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)SecurityProtocolType.Ssl3;

                result = (BS2ErrorCode)API.BS2_SetSSLHandler(sdkContext, cbPreferMethod, cbGetRootCaFilePath, cbGetServerCaFilePath, cbGetServerPrivateKeyFilePath, cbGetPassword, null);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("SetSSLHandler: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }
                else
                {
                    //bSsl = true;
                }
            }

            if (IPv4 == 1)
            { 
                UInt16 serverPort = 0;
                result = (BS2ErrorCode)API.BS2_GetServerPort(sdkContext, out serverPort);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("GetServerPort: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }
                Console.WriteLine("Server Port on IPv4: {0}", serverPort);

                UInt16 sslServerPort = 0;
                result = (BS2ErrorCode)API.BS2_GetSSLServerPort(sdkContext, out sslServerPort);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("GetSSLServerPort: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }
                Console.WriteLine("SSL Server Port on IPv4: {0}", sslServerPort);
            }

            if (IPv6 == 1)
            {
                UInt16 serverPort = 0;
                result = (BS2ErrorCode)API.BS2_GetServerPortIPV6(sdkContext, out serverPort);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("GetServerPortIPV6: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }
                Console.WriteLine("Server Port on IPv6: {0}", serverPort);

                UInt16 sslServerPort = 0;
                result = (BS2ErrorCode)API.BS2_GetSSLServerPortIPV6(sdkContext, out sslServerPort);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("GetSSLServerPort: Got error({0}).", result);
                    ClearSDK(delayTerminate);
                    return;
                }
                Console.WriteLine("SSL Server Port on IPv6: {0}", sslServerPort);
            }

            result = (BS2ErrorCode)API.BS2_Initialize(sdkContext);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("SDK initialization failed with : {0}", result);
                ClearSDK(delayTerminate);
                return;
            }

            cbOnDeviceFound = new API.OnDeviceFound(DeviceFound);
            cbOnDeviceAccepted = new API.OnDeviceAccepted(DeviceAccepted);
            cbOnDeviceConnected = new API.OnDeviceConnected(DeviceConnected);
            cbOnDeviceDisconnected = new API.OnDeviceDisconnected(DeviceDisconnected);

            result = (BS2ErrorCode)API.BS2_SetDeviceEventListener(sdkContext,
                                                                cbOnDeviceFound,
                                                                cbOnDeviceAccepted,
                                                                cbOnDeviceConnected,
                                                                cbOnDeviceDisconnected);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't register a callback function/method to a sdk.({0})", result);
                ClearSDK(delayTerminate);
                return;
            }

            /*
            if (bSsl)
            {
                cbOnSendRootCA = new API.OnSendRootCA(SendRootCA);
                result = (BS2ErrorCode)API.BS2_SetDeviceSSLEventListener(sdkContext, cbOnSendRootCA);
            }
            */

#if SDK_AUTO_CONNECTION
            result = (BS2ErrorCode)API.BS2_SetAutoConnection(sdkContext, 1);
#endif

            noConnectionMode = false;
            do
            {
                Console.WriteLine("+-----------------------------------------------------------+");
                Console.WriteLine("| 1. Search and connect device                              |");
                Console.WriteLine("| 2. Connect to device via Ip                               |");
                Console.WriteLine("| 3. Server mode test                                       |");
                Console.WriteLine("| 4. Get IP Config via UDP                                  |");
                Console.WriteLine("| 5. Set IP Config via UDP                                  |");
                Console.WriteLine("| 6. Get IPV6 Config via UDP                                |");
                Console.WriteLine("| 7. Set IPV6 Config via UDP                                |");
                Console.WriteLine("| 8. No Connection for USB                                  |");
                Console.WriteLine("+-----------------------------------------------------------+");
                Console.WriteLine("How to connect to device? [2(default)]");
                Console.Write(">>>> ");
                int selection = Util.GetInput(2);

                switch (selection)
                {
                    case 1:
                        if (!SearchAndConnectDeviceWithIPv6(ref deviceID))
                        {
                            deviceID = 0;
                        }
                        break;
                    case 2:
                        if (!ConnectToDeviceWithIPv6(ref deviceID))
                        {
                            deviceID = 0;
                        }
                        break;
                    case 3:
                        {
                            if (deviceIDForServerMode == 0)
                            {
                                Console.WriteLine("Waiting for client connection");
                                eventWaitHandle.WaitOne();
                            }


                            deviceID = deviceIDForServerMode;

                            /*
                            result = (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, deviceID);

                            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                            {
                                Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                                deviceID = 0;
                            }
                            else
                            {
                                Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
                            }
                             */

                        }
                        break;
                    case 4:
                        if (!GetIPConfigViaUDP(ref deviceID))
                        {
                            deviceID = 0;
                        }
                        break;
                    case 5:
                        SetIPConfigViaUDP();
                        break;
                    case 6:
                        if (!GetIPV6ConfigViaUDP(ref deviceID))
                        {
                            deviceID = 0;
                        }
                        break;
                    case 7:
                        SetIPV6ConfigViaUDP();
                        break;
                    case 8:
                        noConnectionMode = true;
                        break;
                    default:
                        Console.WriteLine("Invalid parameter : {0}", selection);
                        break;
                }
            } while (deviceID == 0 && noConnectionMode == false);

            if (noConnectionMode == false && deviceID > 0)
            {
                Console.Title = String.Format("{0} connected deviceID[{1}]", title, deviceID);

#if !SDK_AUTO_CONNECTION
                reconnectionTask = new ReconnectionTask(sdkContext);
                reconnectionTask.start();
#endif
                runImpl(deviceID);
#if !SDK_AUTO_CONNECTION
                reconnectionTask.stop();
                reconnectionTask = null;
#endif

                Console.WriteLine("Trying to discconect device[{0}].", deviceID);
                result = (BS2ErrorCode)API.BS2_DisconnectDevice(sdkContext, deviceID);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    ClearSDK(delayTerminate);
                }
            }
            else if (noConnectionMode == true)
            {
                Console.Title = String.Format("{0} No Connection Mode", title);

                runImpl(deviceID);
            }

            eventWaitHandle.Close();
            ClearSDK(delayTerminate);

            cbOnDeviceFound = null;
            cbOnDeviceAccepted = null;
            cbOnDeviceConnected = null;
            cbOnDeviceDisconnected = null;
            cbOnSendRootCA = null;

        }

        void ClearSDK(int delayTerminate)
        {
            if (sdkContext != IntPtr.Zero)
            { 
                API.BS2_ReleaseContext(sdkContext);
            }
            sdkContext = IntPtr.Zero;
            Thread.Sleep(delayTerminate);
        }
		
        bool SearchAndConnectDeviceWithIPv6(ref UInt32 deviceID)
        {
            bool IPv6 = true;
            bool IPv4 = true;
            Console.WriteLine("Which mode do you want to use between IPv4 and IPv6? [0(IPv4), 1(IPv6), 2(Both: Default)]");
            Console.Write(">>>> ");
            int choiceIP = Util.GetInput((int)2);
            if (choiceIP == 0)
            {
                IPv4 = true;
                IPv6 = false;
            }
            else if (choiceIP == 1)
            {
                IPv4 = false;
                IPv6 = true;
            }

            Console.WriteLine("Trying to broadcast on the network");

            IntPtr ptrV4Broad = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_BROADCAST_IPV4_ADDRESS);
            IntPtr ptrV6Multi = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_MULTICAST_IPV6_ADDRESS);

            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            if (IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_SearchDevices(sdkContext);
            else if (IPv4 && !IPv6)
                //result = (BS2ErrorCode)API.BS2_SearchDevicesEx(sdkContext, BS2Environment.DEFAULT_BROADCAST_IPV4_ADDRESS);
                result = (BS2ErrorCode)API.BS2_SearchDevicesEx(sdkContext, ptrV4Broad);
            else if (!IPv4 && IPv6)
                //result = (BS2ErrorCode)API.BS2_SearchDevicesEx(sdkContext, BS2Environment.DEFAULT_MULTICAST_IPV6_ADDRESS);
                result = (BS2ErrorCode)API.BS2_SearchDevicesEx(sdkContext, ptrV6Multi);

            Marshal.FreeHGlobal(ptrV4Broad);
            Marshal.FreeHGlobal(ptrV6Multi);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("SearchDevices?? : Got error : {0}.", result);
                return false;
            }

            IntPtr deviceListObj = IntPtr.Zero;
            UInt32 numDevice = 0;

            const UInt32 LONG_TIME_STANDBY_7S = 7;
            result = (BS2ErrorCode)API.BS2_SetDeviceSearchingTimeout(sdkContext, LONG_TIME_STANDBY_7S);
            if (BS2ErrorCode.BS_SDK_SUCCESS != result)
            {
                Console.WriteLine("SetDeviceSearchingTimeout: Got error : {0}.", result);
                return false;
            }

            result = (BS2ErrorCode)API.BS2_GetDevices(sdkContext, out deviceListObj, out numDevice);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("GetDevices: Got error : {0}.", result);
                return false;
            }

            if (numDevice > 0)
            {
                BS2SimpleDeviceInfo deviceInfo;
                Type structType = typeof(BS2IPv6DeviceInfo);
                int structSize = Marshal.SizeOf(structType);
                IntPtr buffer = Marshal.AllocHGlobal(structSize);
                UInt32 outStructSize = 0;

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                for (UInt32 idx = 0; idx < numDevice; ++idx)
                {
                    deviceID = Convert.ToUInt32(Marshal.ReadInt32(deviceListObj, (int)idx * sizeof(UInt32)));
                    
                    result = (BS2ErrorCode)API.BS2_GetSpecifiedDeviceInfo(sdkContext, deviceID, (UInt32)BS2SpecifiedDeviceInfo.BS2_SPECIFIED_DEVICE_INFO_IPV6, buffer, (UInt32)structSize, out outStructSize);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("GetSpecifiedDeviceInfo: Got error : {0}.", result);
                        Marshal.FreeHGlobal(buffer);
                        return false;
                    }
                    BS2IPv6DeviceInfo devicInfoIPv6 = (BS2IPv6DeviceInfo)Marshal.PtrToStructure(buffer, structType);

                    result = (BS2ErrorCode)API.BS2_GetDeviceInfo(sdkContext, deviceID, out deviceInfo);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("GetDeviceInfo: Got error : {0}.", result);
                        Marshal.FreeHGlobal(buffer);
                        return false;
                    }

                    Console.WriteLine("[{0, 3:##0}] ==> ID[{1, 10}] Type[{2, 16}] Connection mode[{3}] IPv4[{4}] IPv4-Port[{5}], IPv6[{6}] IPv6-Port[{7}]",
                            idx,
                            deviceID,
                            API.productNameDictionary.ContainsKey((BS2DeviceTypeEnum)deviceInfo.type) ? API.productNameDictionary[(BS2DeviceTypeEnum)deviceInfo.type] : (API.productNameDictionary[BS2DeviceTypeEnum.UNKNOWN] + "(" + deviceInfo.type + ")") ,
                            (BS2ConnectionModeEnum)deviceInfo.connectionMode,
                            new IPAddress(BitConverter.GetBytes(deviceInfo.ipv4Address)).ToString(),
                            deviceInfo.port
                            ,Encoding.UTF8.GetString(devicInfoIPv6.ipv6Address).TrimEnd('\0')
                            ,devicInfoIPv6.portV6
                            ); 
                }

                Marshal.FreeHGlobal(buffer);

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("Please, choose the index of the Device which you want to connect to. [-1: quit]");
                Console.Write(">>>> ");

                deviceID = 0;
                Int32 selection = Util.GetInput();

                if (selection >= 0)
                {
                    if (selection < numDevice)
                    {
                        deviceID = Convert.ToUInt32(Marshal.ReadInt32(deviceListObj, (int)selection * sizeof(UInt32)));
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection[{0}]", selection);
                    }
                }

                API.BS2_ReleaseObject(deviceListObj);
                if (deviceID > 0)
                {
                    IntPtr buffer1 = Marshal.AllocHGlobal(structSize);
                    result = (BS2ErrorCode)API.BS2_GetSpecifiedDeviceInfo(sdkContext, deviceID, (UInt32)BS2SpecifiedDeviceInfo.BS2_SPECIFIED_DEVICE_INFO_IPV6, buffer1, (UInt32)structSize, out outStructSize);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("GetSpecifiedDeviceInfo: Got error : {0}.", result);
                        Marshal.FreeHGlobal(buffer1);
                        return false;
                    }
                    bool connectIPv6 = false;
                    BS2IPv6DeviceInfo devicInfoIPv6 = (BS2IPv6DeviceInfo)Marshal.PtrToStructure(buffer1, structType);
                    IPAddress tempAddress;
                    bool bCanUseIPv6 = Encoding.UTF8.GetString(devicInfoIPv6.ipv6Address).TrimEnd('\0').Length > 0
                        && IPAddress.TryParse(Encoding.UTF8.GetString(devicInfoIPv6.ipv6Address).TrimEnd('\0'), out tempAddress)
                        && tempAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
                    if (bCanUseIPv6)
                    {
                        Console.WriteLine("Do you want to connect via IPv6? [Y/n]");
                        Console.Write(">>>>");
                        if (Util.IsYes())
                        {
                            connectIPv6 = true;
                        }
                    }
                    Marshal.FreeHGlobal(buffer1);

                    Console.WriteLine("Trying to connect to device[{0}]", deviceID);                    

                    if (connectIPv6)
                        result = (BS2ErrorCode)API.BS2_ConnectDeviceIPV6(sdkContext, deviceID);
                    else
                        result = (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, deviceID);                    

                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("BS2_ConnectDevice???: Got error : {0}.", result);
                        return false;
                    }

                    Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
                    return true;
                }
            }
            else
            {
                Console.WriteLine("There is no device to launch.");
            }

            return false;
        }

        bool ConnectToDeviceWithIPv6(ref UInt32 deviceID)
        {
            Console.WriteLine("Enter the IP Address to connect device");
            Console.Write(">>>> ");
            string deviceIpAddress = Console.ReadLine();
            IPAddress ipAddress;

            if (!IPAddress.TryParse(deviceIpAddress, out ipAddress))
            {
                Console.WriteLine("Invalid ip : " + deviceIpAddress);
                return false;
            }

            Console.WriteLine("Enter the port number to connect device : default[{0}]", ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 ? BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT_V6 : BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT); //[IPv6] <=
            Console.Write(">>>> ");
            UInt16 port = Util.GetInput((UInt16)(ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 ? BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT_V6 : BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT)); //[IPv6] <=

            Console.WriteLine("Trying to connect to device [ip :{0}, port : {1}]", deviceIpAddress, port);


            IntPtr ptrIPAddr = Marshal.StringToHGlobalAnsi(deviceIpAddress);
            //BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, deviceIpAddress, port, out deviceID);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, ptrIPAddr, port, out deviceID);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("BS2_ConnectDeviceViaIP: Got error : {0}.", result);
                return false;
            }
            Marshal.FreeHGlobal(ptrIPAddr);

            Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
            return true;
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

        bool GetIPConfigViaUDP(ref UInt32 deviceID)
        {
            Console.WriteLine("What is the ID of the device for which you want to get IP config?");
            Console.Write(">>>> ");
            UInt32 inputID = Util.GetInput((UInt32)0);
            if (inputID == 0)
            {
                Console.WriteLine("Invalid Device ID");
                return false;
            }

            bool IPv6 = true;
            bool IPv4 = true;
            Console.WriteLine("Which mode do you want to use between IPv4 and IPv6? [0(IPv4), 1(IPv6), 2(Both: Default)]");
            Console.Write(">>>> ");
            int choiceIP = Util.GetInput((int)2);
            if (choiceIP == 0)
            {
                IPv4 = true;
                IPv6 = false;
            }
            else if (choiceIP == 1)
            {
                IPv4 = false;
                IPv6 = true;
            }

            Console.WriteLine("Trying to send packet via UDP on the network");

            BS2IpConfig config;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            IntPtr ptrV4Broad = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_BROADCAST_IPV4_ADDRESS);
            IntPtr ptrV6Multi = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_MULTICAST_IPV6_ADDRESS);
            if (IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_GetIPConfigViaUDP(sdkContext, inputID, out config);
            else if (IPv4 && !IPv6)
                result = (BS2ErrorCode)API.BS2_GetIPConfigViaUDPEx(sdkContext, inputID, out config, ptrV4Broad);
            else if (!IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_GetIPConfigViaUDPEx(sdkContext, inputID, out config, ptrV6Multi);
            else
                config = default(BS2IpConfig);

            Marshal.FreeHGlobal(ptrV4Broad);
            Marshal.FreeHGlobal(ptrV6Multi);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("GetIPConfigViaUDP??: Got error : {0}.", result);
                return false;
            }
            else
            {
                print(config);

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");

                Console.WriteLine("==> ID[{0, 10}] Connection mode[{1}] IPv4[{2}] IPv4-Port[{3}]",
                        inputID,
                        (BS2ConnectionModeEnum)config.connectionMode,
                        Encoding.UTF8.GetString(config.ipAddress).TrimEnd('\0'),
                        config.port
                        );

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("Do you want to connect? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    
                    Console.WriteLine("Trying to connect to device[{0}]", inputID);

                    IntPtr ptrIPAddr = Marshal.StringToHGlobalAnsi(Encoding.UTF8.GetString(config.ipAddress).TrimEnd('\0'));
                    //result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, Encoding.UTF8.GetString(config.ipAddress).TrimEnd('\0'), config.port, out deviceID);
                    result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, ptrIPAddr, config.port, out deviceID);

                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("ConnectDeviceViaIP: Got error : {0}.", result);
                        return false;
                    }
                    Marshal.FreeHGlobal(ptrIPAddr);

                    Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
                    return true;
                }
                else
                    return false;
            }
        }

        bool GetIPV6ConfigViaUDP(ref UInt32 deviceID)
        {
            Console.WriteLine("What is the ID of the device for which you want to get IP config?");
            Console.Write(">>>> ");
            UInt32 inputID = Util.GetInput((UInt32)0);
            if (inputID == 0)
            {
                Console.WriteLine("Invalid Device ID");
                return false;
            }

            bool IPv6 = true;
            bool IPv4 = true;
            Console.WriteLine("Which mode do you want to use between IPv4 and IPv6? [0(IPv4), 1(IPv6), 2(Both: Default)]");
            Console.Write(">>>> ");
            int choiceIP = Util.GetInput((int)2);
            if (choiceIP == 0)
            {
                IPv4 = true;
                IPv6 = false;
            }
            else if (choiceIP == 1)
            {
                IPv4 = false;
                IPv6 = true;
            }

            Console.WriteLine("Trying to send packet via UDP on the network");

            BS2IPV6Config config;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            IntPtr ptrV4Broad = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_BROADCAST_IPV4_ADDRESS);
            IntPtr ptrV6Multi = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_MULTICAST_IPV6_ADDRESS);
            if (IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_GetIPV6ConfigViaUDP(sdkContext, inputID, out config);
            else if (IPv4 && !IPv6)
                result = (BS2ErrorCode)API.BS2_GetIPV6ConfigViaUDPEx(sdkContext, inputID, out config, ptrV4Broad);
            else if (!IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_GetIPV6ConfigViaUDPEx(sdkContext, inputID, out config, ptrV6Multi);
            else
                config = default(BS2IPV6Config);

            Marshal.FreeHGlobal(ptrV4Broad);
            Marshal.FreeHGlobal(ptrV6Multi);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("GetIPV6ConfigViaUDP??: Got error : {0}.", result);
                return false;
            }
            else
            {
                print(config);

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");

                byte[] allocatedIpAddressV6_0 = new byte[BS2Environment.BS2_IPV6_ADDR_SIZE];
                Array.Copy(config.allocatedIpAddressV6, 0, allocatedIpAddressV6_0, 0, BS2Environment.BS2_IPV6_ADDR_SIZE);
                Console.WriteLine("==> ID[{0, 10}] numOfAllocated[{1}] IPv6[{2}] IPv6-Port[{3}]",
                        inputID,
                        config.numOfAllocatedAddressV6,
                        Encoding.UTF8.GetString(allocatedIpAddressV6_0).TrimEnd('\0'),
                        config.portV6
                        );

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("Do you want to connect? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    String strIpAddressV6 = Encoding.UTF8.GetString(allocatedIpAddressV6_0).TrimEnd('\0');
                    if (strIpAddressV6.IndexOf('/') != -1)
                    {
                        strIpAddressV6 = strIpAddressV6.Substring(0, strIpAddressV6.IndexOf('/'));
                    }
                    Console.WriteLine("Trying to connect to device[{0}][{1}]", inputID, strIpAddressV6);

                    IntPtr ptrIPAddr = Marshal.StringToHGlobalAnsi(strIpAddressV6);
                    result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, ptrIPAddr, config.portV6, out deviceID);

                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("ConnectDeviceViaIP??: Got error : {0}.", result);
                        return false;
                    }
                    Marshal.FreeHGlobal(ptrIPAddr);

                    Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
                    return true;
                }
                else
                    return false;
            }
        }

        public void SetIPConfigViaUDP()
        {
            Console.WriteLine("What is the ID of the device for which you want to set IP config?");
            Console.Write(">>>> ");
            UInt32 inputID = Util.GetInput((UInt32)0);
            if (inputID == 0)
            {
                Console.WriteLine("Invalid Device ID");
                return;
            }

            bool IPv6 = true;
            bool IPv4 = true;
            Console.WriteLine("Which mode do you want to use between IPv4 and IPv6? [0(IPv4), 1(IPv6), 2(Both: Default)]");
            Console.Write(">>>> ");
            int choiceIP = Util.GetInput((int)2);
            if (choiceIP == 0)
            {
                IPv4 = true;
                IPv6 = false;
            }
            else if (choiceIP == 1)
            {
                IPv4 = false;
                IPv6 = true;
            }

            BS2IpConfig config;
            Console.WriteLine("Trying to get Current IPConfig via UDP");
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            if (IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_GetIPConfigViaUDP(sdkContext, inputID, out config);
            else if (IPv4 && !IPv6)
            {
                IntPtr ptrV4Broad = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_BROADCAST_IPV4_ADDRESS);
                result = (BS2ErrorCode)API.BS2_GetIPConfigViaUDPEx(sdkContext, inputID, out config, ptrV4Broad);
                Marshal.FreeHGlobal(ptrV4Broad);
            }
            else if (!IPv4 && IPv6)
            {
                IntPtr ptrV6Multi = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_MULTICAST_IPV6_ADDRESS);
                result = (BS2ErrorCode)API.BS2_GetIPConfigViaUDPEx(sdkContext, inputID, out config, ptrV6Multi);
                Marshal.FreeHGlobal(ptrV6Multi);
            }
            else
            {
                Console.WriteLine("Wrong selection");
                return;
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("GetIPConfigViaUDP??: Got error : {0}.", result);
                return;
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

            Console.WriteLine("Trying to set IPConfig via UDP");
            IntPtr ptrV4Broad2 = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_BROADCAST_IPV4_ADDRESS);
            IntPtr ptrV6Multi2 = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_MULTICAST_IPV6_ADDRESS);
            if (IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_SetIPConfigViaUDP(sdkContext, inputID, ref config);
            else if (IPv4 && !IPv6)
                result = (BS2ErrorCode)API.BS2_SetIPConfigViaUDPEx(sdkContext, inputID, ref config, ptrV4Broad2);
            else if (!IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_SetIPConfigViaUDPEx(sdkContext, inputID, ref config, ptrV6Multi2);
            Marshal.FreeHGlobal(ptrV4Broad2);
            Marshal.FreeHGlobal(ptrV6Multi2);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("SetIPConfigViaUDP??: Got error({0}).", result);
            }
            else
            {
                Console.WriteLine(">>>> Successfully set");
            }
        }

        public void SetIPV6ConfigViaUDP()
        {
            Console.WriteLine("What is the ID of the device for which you want to get IP config?");
            Console.Write(">>>> ");
            UInt32 inputID = Util.GetInput((UInt32)0);
            if (inputID == 0)
            {
                Console.WriteLine("Invalid Device ID");
                return;
            }

            bool IPv6 = true;
            bool IPv4 = true;
            Console.WriteLine("Which mode do you want to use between IPv4 and IPv6? [0(IPv4), 1(IPv6), 2(Both: Default)]");
            Console.Write(">>>> ");
            int choiceIP = Util.GetInput((int)2);
            if (choiceIP == 0)
            {
                IPv4 = true;
                IPv6 = false;
            }
            else if (choiceIP == 1)
            {
                IPv4 = false;
                IPv6 = true;
            }

            BS2IPV6Config config;
            Console.WriteLine("Trying to get Current IPV6Config via UDP");
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            if (IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_GetIPV6ConfigViaUDP(sdkContext, inputID, out config);
            else if (IPv4 && !IPv6)
            {
                IntPtr ptrV4Broad = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_BROADCAST_IPV4_ADDRESS);
                result = (BS2ErrorCode)API.BS2_GetIPV6ConfigViaUDPEx(sdkContext, inputID, out config, ptrV4Broad);
                Marshal.FreeHGlobal(ptrV4Broad);
            }
            else if (!IPv4 && IPv6)
            {
                IntPtr ptrV6Multi = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_MULTICAST_IPV6_ADDRESS);
                result = (BS2ErrorCode)API.BS2_GetIPV6ConfigViaUDPEx(sdkContext, inputID, out config, ptrV6Multi);
                Marshal.FreeHGlobal(ptrV6Multi);
            }
            else
            {
                Console.WriteLine("Wrong selection");
                return;
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("GetIPV6ConfigViaUDP??: Got error({0}).", result);
                return;
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

            Console.WriteLine("Trying to set IPV6Config via UDP");
            IntPtr ptrV4Broad2 = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_BROADCAST_IPV4_ADDRESS);
            IntPtr ptrV6Multi2 = Marshal.StringToHGlobalAnsi(BS2Environment.DEFAULT_MULTICAST_IPV6_ADDRESS);
            if (IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_SetIPV6ConfigViaUDP(sdkContext, inputID, ref config);
            else if (IPv4 && !IPv6)
                result = (BS2ErrorCode)API.BS2_SetIPV6ConfigViaUDPEx(sdkContext, inputID, ref config, ptrV4Broad2);
            else if (!IPv4 && IPv6)
                result = (BS2ErrorCode)API.BS2_SetIPV6ConfigViaUDPEx(sdkContext, inputID, ref config, ptrV6Multi2);

            Marshal.FreeHGlobal(ptrV4Broad2);
            Marshal.FreeHGlobal(ptrV6Multi2);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("SetIPV6ConfigViaUDP??: Got error({0}).", result);
            }
            else
            {
                Console.WriteLine(">>>> Successfully set");
            }
        }
    }
}
