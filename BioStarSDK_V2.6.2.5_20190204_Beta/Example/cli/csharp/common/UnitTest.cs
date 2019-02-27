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
                    //API.BS2_ReleaseContext(sdkContext); //중복
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
                return;
            }

            /*
            if (bSsl)
            {
                cbOnSendRootCA = new API.OnSendRootCA(SendRootCA);
                result = (BS2ErrorCode)API.BS2_SetDeviceSSLEventListener(sdkContext, cbOnSendRootCA);
            }
            */

            cbDebugExPrint = null;
            Console.WriteLine("Do you want print debug message? [y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                cbDebugExPrint = new API.CBDebugExPrint(DebugExPrint);
                result = (BS2ErrorCode)API.BS2_SetDebugExCallback(cbDebugExPrint, Constants.DEBUG_LOG_ALL, Constants.DEBUG_MODULE_ALL);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
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
                            API.productNameDictionary[(BS2DeviceTypeEnum)deviceInfo.type],
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

           
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, deviceIpAddress, port, out deviceID);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't connect to device(errorCode : {0}).", result);
                return false;
            }           

            Console.WriteLine(">>>> Successfully connected to the device[{0}].", deviceID);
            return true;
        }

        bool ConnectToDeviceSSL(string deviceIpAddress, ref UInt32 deviceID)
        {                
            UInt16 port = Util.GetInput((UInt16)BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT);

            int nCnt = 0;
            while (true)
            {
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, deviceIpAddress, port, out deviceID);

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
            Console.WriteLine("|  Type                              : {0}({1})", API.productNameDictionary[(BS2DeviceTypeEnum)deviceInfo.type], deviceInfo.type);
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

        string GetRootCaFilePathHandle(UInt32 deviceID)
        {
            return ssl_server_root_crt;
        }

        string GetServerCaFilePathHandle(UInt32 deviceID)
        {
            return ssl_server_crt;
        }

        string GetServerPrivateKeyFilePathHandle(UInt32 deviceID)
        {
            return ssl_server_pem;
        }

        string GetPasswordHandle(UInt32 deviceID)
        {
            return ssl_server_passwd;
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
            Trace.WriteLine(printmsg);
            //Console.WriteLine(printmsg);
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
    }
}
