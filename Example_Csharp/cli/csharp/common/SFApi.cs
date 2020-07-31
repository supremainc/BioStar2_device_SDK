using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Security;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace Suprema
{
    using BS2_SCHEDULE_ID = UInt32;
    using BS2_CONFIG_MASK = UInt32;
    using BS2_USER_MASK = UInt32;

    static class API
    {
        public static Dictionary<BS2DeviceTypeEnum, string> productNameDictionary = new Dictionary<BS2DeviceTypeEnum, string>()
        {
            {BS2DeviceTypeEnum.UNKNOWN,         "Unknown Device"},
            {BS2DeviceTypeEnum.BIOENTRY_PLUS,   "BioEntry Plus"},
            {BS2DeviceTypeEnum.BIOENTRY_W,      "BioEntry W"},
            {BS2DeviceTypeEnum.BIOLITE_NET,     "BioLite Net"},
            {BS2DeviceTypeEnum.XPASS,           "Xpass"},
            {BS2DeviceTypeEnum.XPASS_S2,        "Xpass S2"},
            {BS2DeviceTypeEnum.SECURE_IO_2,     "Secure IO 2"},
            {BS2DeviceTypeEnum.DOOR_MODULE_20,  "Door module 20"},
            {BS2DeviceTypeEnum.BIOSTATION_2,    "BioStation 2"},
            {BS2DeviceTypeEnum.BIOSTATION_A2,   "BioStation A2"},
            {BS2DeviceTypeEnum.FACESTATION_2,   "FaceStation 2"},
            {BS2DeviceTypeEnum.IO_DEVICE,       "IO device"},
            {BS2DeviceTypeEnum.BIOSTATION_L2,   "BioStation L2"},
            {BS2DeviceTypeEnum.BIOENTRY_W2,     "BioEntry W2"},
            //{BS2DeviceTypeEnum.CORESTATION,     "CoreStation" },		// Deprecated 2.6.0
            {BS2DeviceTypeEnum.CORESTATION_40,  "CoreStation40" },
            {BS2DeviceTypeEnum.OUTPUT_MODULE,   "Output Module"},
            {BS2DeviceTypeEnum.INPUT_MODULE,    "Inout Module"},      
            {BS2DeviceTypeEnum.BIOENTRY_P2,     "BioEntry P2"},
            {BS2DeviceTypeEnum.BIOLITE_N2,      "BioLite N2"},
            {BS2DeviceTypeEnum.XPASS2,          "XPass 2"},
            {BS2DeviceTypeEnum.XPASS_S3,        "XPass S3"},
            {BS2DeviceTypeEnum.BIOENTRY_R2,     "BioEntry R2"},
            {BS2DeviceTypeEnum.XPASS_D2,        "XPass D2"},
            {BS2DeviceTypeEnum.DOOR_MODULE_21,  "DoorModule 21"},
            {BS2DeviceTypeEnum.XPASS_D2_KEYPAD, "XPass D2 Keypad"},
            {BS2DeviceTypeEnum.FACELITE,        "FaceLite"},
            {BS2DeviceTypeEnum.XPASS2_KEYPAD,   "XPass 2 Keypad"},
            {BS2DeviceTypeEnum.XPASS_D2_REV,    "XPass D2 Rev"},
            {BS2DeviceTypeEnum.XPASS_D2_KEYPAD_REV, "XPass D2 Keypad Rev"},
        };

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDeviceFound(UInt32 deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDeviceAccepted(UInt32 deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDeviceConnected(UInt32 deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDeviceDisconnected(UInt32 deviceId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnReadyToScan(UInt32 deviceId, UInt32 sequence);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnProgressChanged(UInt32 deviceId, UInt32 progressPercentage);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnLogReceived(UInt32 deviceId, IntPtr log);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnAlarmFired(UInt32 deviceId, IntPtr log);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnInputDetected(UInt32 deviceId, IntPtr log);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnConfigChanged(UInt32 deviceId, UInt32 configMask);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnVerifyUser(UInt32 deviceId, UInt16 seq, byte isCard, byte cardType, IntPtr data, UInt32 dataLen);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnIdentifyUser(UInt32 deviceId, UInt16 seq, byte format, IntPtr templateData, UInt32 templateSize);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnUserPhrase(UInt32 deviceId, UInt16 seq, string userID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int IsAcceptableUserID(string uid);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UInt32 PreferMethod(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate string GetRootCaFilePath(UInt32 deviceID);
        public delegate IntPtr GetRootCaFilePath(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate string GetServerCaFilePath(UInt32 deviceID);
        public delegate IntPtr GetServerCaFilePath(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate string GetServerPrivateKeyFilePath(UInt32 deviceID);
        public delegate IntPtr GetServerPrivateKeyFilePath(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate string GetPassword(UInt32 deviceID);
        public delegate IntPtr GetPassword(UInt32 deviceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnErrorOccured(UInt32 deviceID, int errCode);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnSendRootCA(int result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnCheckGlobalAPBViolation(UInt32 deviceId, UInt16 seq, string userID_1, string userID_2, bool isDualAuth);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnCheckGlobalAPBViolationByDoorOpen(UInt32 deviceId, UInt16 seq, string userID_1, string userID_2, bool isDualAuth);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnUpdateGlobalAPBViolationByDoorOpen(UInt32 deviceId, UInt16 seq, string userID_1, string userID_2, bool isDualAuth);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static IntPtr BS2_AllocateContext();

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_Initialize(IntPtr context);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceSearchingTimeout(IntPtr context, UInt32 second);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetMaxThreadCount(IntPtr context, UInt32 maxThreadCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_IsAutoConnection(IntPtr context, ref int enable);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAutoConnection(IntPtr context, int enable);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceEventListener(IntPtr context, OnDeviceFound cbOnDeviceFound, OnDeviceAccepted cbOnDeviceAccepted, OnDeviceConnected cbOnDeviceConnected, OnDeviceDisconnected cbOnDeviceDisconnected);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetNotificationListener(IntPtr context, OnAlarmFired cbOnAlarmFired, OnInputDetected cbOnInputDetected, OnConfigChanged cbOnConfigChanged);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void BS2_ReleaseContext(IntPtr context);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void BS2_ReleaseObject(IntPtr obj);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSSLHandler(IntPtr context, PreferMethod cbPreferMethod, GetRootCaFilePath cbGetRootCaFilePath, GetServerCaFilePath cbGetServerCaFilePath, GetServerPrivateKeyFilePath cbGetServerPrivateKeyFilePath, GetPassword cbGetPassword, OnErrorOccured cbOnErrorOccured); 

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetServerPort(IntPtr context, UInt16 serverPort);                               

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SearchDevices(IntPtr context);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_SearchDevicesEx(IntPtr context, string hostipAddr);
        extern public static int BS2_SearchDevicesEx(IntPtr context, IntPtr hostipAddr);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDevices(IntPtr context, out IntPtr deviceListObj, out UInt32 numDevice);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceInfo(IntPtr context, UInt32 deviceId, out BS2SimpleDeviceInfo deviceInfo);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ConnectDevice(IntPtr context, UInt32 deviceId);        

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_ConnectDeviceViaIP(IntPtr context, string deviceAddress, UInt16 devicePort, out UInt32 deviceId);
        extern public static int BS2_ConnectDeviceViaIP(IntPtr context, IntPtr deviceAddress, UInt16 devicePort, out UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_DisconnectDevice(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceTopology(IntPtr context, UInt32 deviceId, out IntPtr networkNodeObj, out UInt32 numNetworkNode);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceTopology(IntPtr context, UInt32 deviceId, IntPtr networkNode, UInt32 numNetworkNode);


        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< AccessControl API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAccessGroup(IntPtr context, UInt32 deviceId, IntPtr accessGroupIds, UInt32 accessGroupIdCount, out IntPtr accessGroupObj, out UInt32 numAccessGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAccessGroup(IntPtr context, UInt32 deviceId, out IntPtr accessGroupObj, out UInt32 numAccessGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAccessGroup(IntPtr context, UInt32 deviceId, IntPtr accessGroups, UInt32 accessGroupCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAccessGroup(IntPtr context, UInt32 deviceId, IntPtr accessGroupIds, UInt32 accessGroupIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAccessGroup(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAccessLevel(IntPtr context, UInt32 deviceId, IntPtr accessLevelIds, UInt32 accessLevelIdCount, out IntPtr accessLevelObj, out UInt32 numAccessLevel);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAccessLevel(IntPtr context, UInt32 deviceId, out IntPtr accessLevelObj, out UInt32 numAccessLevel);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAccessLevel(IntPtr context, UInt32 deviceId, IntPtr accessLevels, UInt32 accessLevelCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAccessLevel(IntPtr context, UInt32 deviceId, IntPtr accessLevelIds, UInt32 accessLevelIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAccessLevel(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAccessSchedule(IntPtr context, UInt32 deviceId, IntPtr accessScheduleIds, UInt32 accessScheduleIdCount, out IntPtr accessScheduleObj, out UInt32 numAccessSchedule);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAccessSchedule(IntPtr context, UInt32 deviceId, out IntPtr accessScheduleObj, out UInt32 numAccessSchedule);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAccessSchedule(IntPtr context, UInt32 deviceId, IntPtr accessSchedules, UInt32 accessScheduleCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAccessSchedule(IntPtr context, UInt32 deviceId, IntPtr accessScheduleIds, UInt32 accessScheduleIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAccessSchedule(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetHolidayGroup(IntPtr context, UInt32 deviceId, IntPtr holidayGroupIds, UInt32 holidayGroupIdCount, out IntPtr holidayGroupObj, out UInt32 numHolidayGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllHolidayGroup(IntPtr context, UInt32 deviceId, out IntPtr holidayGroupObj, out UInt32 numHolidayGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetHolidayGroup(IntPtr context, UInt32 deviceId, IntPtr holidayGroups, UInt32 holidayGroupCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveHolidayGroup(IntPtr context, UInt32 deviceId, IntPtr holidayGroupIds, UInt32 holidayGroupIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllHolidayGroup(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Blacklist API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetBlackList(IntPtr context, UInt32 deviceId, IntPtr blacklists, UInt32 blacklistCount, out IntPtr blacklistObj, out UInt32 numBlacklist);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllBlackList(IntPtr context, UInt32 deviceId, out IntPtr blacklistObj, out UInt32 numBlacklist);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetBlackList(IntPtr context, UInt32 deviceId, IntPtr blacklists, UInt32 blacklistCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveBlackList(IntPtr context, UInt32 deviceId, IntPtr blacklists, UInt32 blacklistCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllBlackList(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Card API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ScanCard(IntPtr context, UInt32 deviceId, out BS2Card card, OnReadyToScan cbReadyToScan);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_WriteCard(IntPtr context, UInt32 deviceId, ref BS2SmartCardData smartCard);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EraseCard(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Config API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearDatabase(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ResetConfig(IntPtr context, UInt32 deviceId, [MarshalAs(UnmanagedType.I1)] bool includingDB);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetConfig(IntPtr context, UInt32 deviceId, ref BS2Configs configs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetConfig(IntPtr context, UInt32 deviceId, ref BS2Configs configs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFactoryConfig(IntPtr context, UInt32 deviceId, out BS2FactoryConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSystemConfig(IntPtr context, UInt32 deviceId, out BS2SystemConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSystemConfig(IntPtr context, UInt32 deviceId, ref BS2SystemConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAuthConfig(IntPtr context, UInt32 deviceId, out BS2AuthConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAuthConfig(IntPtr context, UInt32 deviceId, ref BS2AuthConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetStatusConfig(IntPtr context, UInt32 deviceId, out BS2StatusConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetStatusConfig(IntPtr context, UInt32 deviceId, ref BS2StatusConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDisplayConfig(IntPtr context, UInt32 deviceId, out BS2DisplayConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDisplayConfig(IntPtr context, UInt32 deviceId, ref BS2DisplayConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPConfig(IntPtr context, UInt32 deviceId, out BS2IpConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPConfigExt(IntPtr context, UInt32 deviceId, out BS2IpConfigExt config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPConfigViaUDP(IntPtr context, UInt32 deviceId, out BS2IpConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPConfig(IntPtr context, UInt32 deviceId, ref BS2IpConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPConfigExt(IntPtr context, UInt32 deviceId, ref BS2IpConfigExt config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPConfigViaUDP(IntPtr context, UInt32 deviceId, ref BS2IpConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetTNAConfig(IntPtr context, UInt32 deviceId, out BS2TNAConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetTNAConfig(IntPtr context, UInt32 deviceId, ref BS2TNAConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetCardConfig(IntPtr context, UInt32 deviceId, out BS2CardConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetCardConfig(IntPtr context, UInt32 deviceId, ref BS2CardConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFingerprintConfig(IntPtr context, UInt32 deviceId, out BS2FingerprintConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetFingerprintConfig(IntPtr context, UInt32 deviceId, ref BS2FingerprintConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetRS485Config(IntPtr context, UInt32 deviceId, out BS2Rs485Config config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetRS485Config(IntPtr context, UInt32 deviceId, ref BS2Rs485Config config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWiegandConfig(IntPtr context, UInt32 deviceId, out BS2WiegandConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetWiegandConfig(IntPtr context, UInt32 deviceId, ref BS2WiegandConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWiegandDeviceConfig(IntPtr context, UInt32 deviceId, out BS2WiegandDeviceConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetWiegandDeviceConfig(IntPtr context, UInt32 deviceId, ref BS2WiegandDeviceConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetInputConfig(IntPtr context, UInt32 deviceId, out BS2InputConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetInputConfig(IntPtr context, UInt32 deviceId, ref BS2InputConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWlanConfig(IntPtr context, UInt32 deviceId, out BS2WlanConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetWlanConfig(IntPtr context, UInt32 deviceId, ref BS2WlanConfig config);        

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetTriggerActionConfig(IntPtr context, UInt32 deviceId, out BS2TriggerActionConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetTriggerActionConfig(IntPtr context, UInt32 deviceId, ref BS2TriggerActionConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetEventConfig(IntPtr context, UInt32 deviceId, out BS2EventConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetEventConfig(IntPtr context, UInt32 deviceId, ref BS2EventConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWiegandMultiConfig(IntPtr context, UInt32 deviceId, out BS2WiegandMultiConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetWiegandMultiConfig(IntPtr context, UInt32 deviceId, ref BS2WiegandMultiConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetCard1xConfig(IntPtr context, UInt32 deviceId, out BS1CardConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetCard1xConfig(IntPtr context, UInt32 deviceId, ref BS1CardConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSystemExtConfig(IntPtr context, UInt32 deviceId, out BS2SystemConfigExt config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSystemExtConfig(IntPtr context, UInt32 deviceId, ref BS2SystemConfigExt config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetVoipConfig(IntPtr context, UInt32 deviceId, out BS2VoipConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetVoipConfig(IntPtr context, UInt32 deviceId, ref BS2VoipConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFaceConfig(IntPtr context, UInt32 deviceId, out BS2FaceConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetFaceConfig(IntPtr context, UInt32 deviceId, ref BS2FaceConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetCardConfigEx(IntPtr context, UInt32 deviceId, out BS2CardConfigEx config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetCardConfigEx(IntPtr context, UInt32 deviceId, ref BS2CardConfigEx config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetRS485ConfigEx(IntPtr context, UInt32 deviceId, out BS2Rs485ConfigEX config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetRS485ConfigEx(IntPtr context, UInt32 deviceId, ref BS2Rs485ConfigEX config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDstConfig(IntPtr context, UInt32 deviceId, out BS2DstConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDstConfig(IntPtr context, UInt32 deviceId, ref BS2DstConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDesFireCardConfigEx(IntPtr context, UInt32 deviceId, out BS2DesFireCardConfigEx config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDesFireCardConfigEx(IntPtr context, UInt32 deviceId, ref BS2DesFireCardConfigEx config);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Door API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDoor(IntPtr context, UInt32 deviceId, IntPtr doorIds, UInt32 doorIdCount, out IntPtr doorObj, out UInt32 numDoor);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllDoor(IntPtr context, UInt32 deviceId, out IntPtr doorObj, out UInt32 numDoor);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDoorStatus(IntPtr context, UInt32 deviceId, IntPtr doorIds, UInt32 doorIdCount, out IntPtr doorStatusObj, out UInt32 numDoorStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllDoorStatus(IntPtr context, UInt32 deviceId, out IntPtr doorStatusObj, out UInt32 numDoorStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDoor(IntPtr context, UInt32 deviceId, IntPtr doors, UInt32 doorCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDoorAlarm(IntPtr context, UInt32 deviceId, byte flag, IntPtr doorIds, UInt32 doorIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveDoor(IntPtr context, UInt32 deviceId, IntPtr doors, UInt32 doorCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllDoor(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ReleaseDoor(IntPtr context, UInt32 deviceId, byte flag, IntPtr doorIds, UInt32 doorIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_LockDoor(IntPtr context, UInt32 deviceId, byte flag, IntPtr doorIds, UInt32 doorIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_UnlockDoor(IntPtr context, UInt32 deviceId, byte flag, IntPtr doorIds, UInt32 doorIdCount);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Fingerprint API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLastFingerprintImage(IntPtr context, UInt32 deviceId, out IntPtr imageObj, out UInt32 imageWidth, out UInt32 imageHeight);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ScanFingerprint(IntPtr context, UInt32 deviceId, ref BS2Fingerprint finger, UInt32 templateIndex, UInt32 quality, byte templateFormat, OnReadyToScan cbReadyToScan);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ScanFingerprintEx(IntPtr context, UInt32 deviceId, ref BS2Fingerprint finger, UInt32 templateIndex, UInt32 quality, byte templateFormat, out UInt32 outquality, OnReadyToScan cbReadyToScan);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_VerifyFingerprint(IntPtr context, UInt32 deviceId, ref BS2Fingerprint finger);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFingerTemplateQuality(IntPtr templateBuffer, UInt32 templateSize, out Int32 score);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Log API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLog(IntPtr context, UInt32 deviceId, UInt32 eventId, UInt32 amount, out IntPtr logObjs, out UInt32 numLog);
        
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFilteredLog(IntPtr context, UInt32 deviceId, IntPtr uid, UInt16 eventCode, UInt32 start, UInt32 end, byte tnakey, out IntPtr logObjs, out UInt32 numLog);
        
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFilteredLogSinceEventId(IntPtr context, UInt32 deviceId, IntPtr uid, UInt16 eventCode, UInt32 start, UInt32 end, byte tnakey, UInt32 lastEventId, UInt32 amount, out IntPtr logObjs, out UInt32 numLog);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetImageLog(IntPtr context, UInt32 deviceId, UInt32 eventId, out IntPtr imageObj, out UInt32 imageSize);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearLog(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_StartMonitoringLog(IntPtr context, UInt32 deviceId, OnLogReceived cbOnLogReceived);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_StopMonitoringLog(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLogBlob(IntPtr context, UInt32 deviceId, UInt16 eventMask, UInt32 eventId, UInt32 amount, out IntPtr logObjs, out UInt32 numLog);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< MISC API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_FactoryReset(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RebootDevice(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_LockDevice(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_UnlockDevice(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceTime(IntPtr context, UInt32 deviceId, out UInt32 gmtTime);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceTime(IntPtr context, UInt32 deviceId, UInt32 gmtTime);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_UpgradeFirmware(IntPtr context, UInt32 deviceId, IntPtr firmwareData, UInt32 firmwareDataLen, byte keepVerifyingSlaveDevice, OnProgressChanged cbProgressChanged);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_UpdateResource(IntPtr context, UInt32 deviceId, ref BS2ResourceElement resourceElement, byte keepVerifyingSlaveDevice, OnProgressChanged cbProgressChanged);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static void BS2_SetKeepAliveTimeout(IntPtr context, long ms);

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_MakePinCode(IntPtr context, string salt, [In, Out] IntPtr pinCode);
        extern public static int BS2_MakePinCode(IntPtr context, IntPtr salt, [In, Out] IntPtr pinCode);

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ComputeCRC16CCITT(IntPtr data, UInt32 dataLen, ref UInt16 crc);

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetCardModel(string modelName, out UInt16 cardModel);
        extern public static int BS2_GetCardModel(IntPtr modelName, out UInt16 cardModel);

        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static IntPtr BS2_Version();

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDataEncryptKey(IntPtr context, UInt32 deviceId, out BS2EncryptKey keyInfo);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDataEncryptKey(IntPtr context, UInt32 deviceId, ref BS2EncryptKey keyInfo);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveDataEncryptKey(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Slave Control API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSlaveDevice(IntPtr context, UInt32 deviceId, out IntPtr slaveDeviceObj, out UInt32 slaveDeviceCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSlaveDevice(IntPtr context, UInt32 deviceId, IntPtr slaveDeviceObj, UInt32 slaveDeviceCount);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Server Matching API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetServerMatchingHandler(IntPtr context, OnVerifyUser cbOnVerifyUser, OnIdentifyUser cbOnIdentifyUser);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_VerifyUser(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserBlob userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_IdentifyUser(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserBlob userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_VerifyUserEx(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserBlobEx userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_IdentifyUserEx(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserBlobEx userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetUserPhraseHandler(IntPtr context, OnUserPhrase cbOnQuery);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ResponseUserPhrase(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, IntPtr userPhrase);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< User API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserDatabaseInfo(IntPtr context, UInt32 deviceId, out UInt32 numUsers, out UInt32 numCards, out UInt32 numFingers, out UInt32 numFaces, IsAcceptableUserID cbIsAcceptableUserID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserList(IntPtr context, UInt32 deviceId, out IntPtr outUidObjs, out UInt32 outNumUids, IsAcceptableUserID cbIsAcceptableUserID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserInfos(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlob[] userBlobs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserDatas(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlob[] userBlobs, UInt32 userMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EnrolUser(IntPtr context, UInt32 deviceId, [In, Out] BS2UserBlob[] userBlobs, UInt32 uidCount, byte overwrite);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EnrollUser(IntPtr context, UInt32 deviceId, [In, Out] BS2UserBlob[] userBlobs, UInt32 uidCount, byte overwrite);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveUser(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllUser(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserInfosEx(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlobEx[] userBlobs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserDatasEx(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlobEx[] userBlobs, UInt32 userMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EnrolUserEx(IntPtr context, UInt32 deviceId, [In, Out] BS2UserBlobEx[] userBlobs, UInt32 uidCount, byte overwrite);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EnrollUserEx(IntPtr context, UInt32 deviceId, [In, Out] BS2UserBlobEx[] userBlobs, UInt32 uidCount, byte overwrite);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Wiegand Control API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SearchWiegandDevices(IntPtr context, UInt32 deviceId, out IntPtr wiegandDeviceObj, out UInt32 numWiegandDevice);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetWiegandDevices(IntPtr context, UInt32 deviceId, out IntPtr wiegandDeviceObj, out UInt32 numWiegandDevice);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_AddWiegandDevices(IntPtr context, UInt32 deviceId, IntPtr wiegandDevice, UInt32 numWiegandDevice);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveWiegandDevices(IntPtr context, UInt32 deviceId, IntPtr wiegandDevice, UInt32 numWiegandDevice);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Zone Control API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAntiPassbackZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAntiPassbackZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAntiPassbackZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, UInt32 zoneID, IntPtr uids, UInt32 uidCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearAllAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, UInt32 zoneID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetCheckGlobalAPBViolationHandler(IntPtr context, OnCheckGlobalAPBViolation ptrCheckGlobalAPBViolation);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_CheckGlobalAPBViolation(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, UInt32 zoneID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetGlobalAPBViolationByDoorOpenHandler(IntPtr context, OnCheckGlobalAPBViolationByDoorOpen ptrCheck, OnUpdateGlobalAPBViolationByDoorOpen ptrUpdate);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_CheckGlobalAPBViolationByDoorOpen(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, UInt32 zoneID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetTimedAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllTimedAntiPassbackZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetTimedAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllTimedAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetTimedAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetTimedAntiPassbackZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveTimedAntiPassbackZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllTimedAntiPassbackZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearTimedAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, UInt32 zoneID, IntPtr uids, UInt32 uidCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearAllTimedAntiPassbackZoneStatus(IntPtr context, UInt32 deviceId, UInt32 zoneID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFireAlarmZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllFireAlarmZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFireAlarmZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllFireAlarmZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetFireAlarmZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetFireAlarmZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveFireAlarmZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllFireAlarmZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetScheduledLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllScheduledLockUnlockZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetScheduledLockUnlockZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllScheduledLockUnlockZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetScheduledLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetScheduledLockUnlockZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveScheduledLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllScheduledLockUnlockZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLiftLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllLiftLockUnlockZone(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLiftLockUnlockZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllLiftLockUnlockZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetLiftLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zones, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetLiftLockUnlockZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveLiftLockUnlockZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllLiftLockUnlockZone(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Face API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ScanFace(IntPtr context, UInt32 deviceId, [In, Out] BS2Face[] face, byte erollmentThreshold, OnReadyToScan cbReadyToScan);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Lift API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLift(IntPtr context, UInt32 deviceId, IntPtr LiftIds, UInt32 LiftIdCount, out IntPtr LiftObj, out UInt32 numLift);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllLift(IntPtr context, UInt32 deviceId, out IntPtr LiftObj, out UInt32 numLift);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetLiftStatus(IntPtr context, UInt32 deviceId, IntPtr LiftIds, UInt32 LiftIdCount, out IntPtr LiftObj, out UInt32 numLift);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllLiftStatus(IntPtr context, UInt32 deviceId, out IntPtr LiftObj, out UInt32 numLift);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetLift(IntPtr context, UInt32 deviceId, IntPtr Lifts, UInt32 LiftCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetLiftAlarm(IntPtr context, UInt32 deviceId, byte alarmFlag, IntPtr Lifts, UInt32 LiftCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveLift(IntPtr context, UInt32 deviceId, IntPtr LiftIds, UInt32 LiftIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllLift(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ReleaseFloor(IntPtr context, UInt32 deviceId, byte floorFlag, UInt32 liftID, IntPtr FloorIndexs, byte floorIndexCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ActivateFloor(IntPtr context, UInt32 deviceId, byte floorFlag, UInt32 liftID, IntPtr FloorIndexs, byte floorIndexCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_DeActivateFloor(IntPtr context, UInt32 deviceId, byte floorFlag, UInt32 liftID, IntPtr FloorIndexs, byte floorIndexCount);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< SSL API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSSLServerPort(IntPtr context, UInt16 sslServerPort);                               

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceSSLEventListener(IntPtr context, OnSendRootCA cbOnSendRootCA);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_DisableSSL(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< AuthGroup API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAuthGroup(IntPtr context, UInt32 deviceId, IntPtr authGroupIds, UInt32 authGroupIdCount, out IntPtr authGroupObj, out UInt32 numAuthGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAuthGroup(IntPtr context, UInt32 deviceId, out IntPtr authGroupObj, out UInt32 numAuthGroup);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAuthGroup(IntPtr context, UInt32 deviceId, IntPtr authGroups, UInt32 authGroupCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAuthGroup(IntPtr context, UInt32 deviceId, IntPtr authGroupIds, UInt32 authGroupIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAuthGroup(IntPtr context, UInt32 deviceId);        

		/* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Floor Level API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetFloorLevel(IntPtr context, UInt32 deviceId, IntPtr floorLevelIds, UInt32 floorLevelIdCount, out IntPtr floorLevelObj, out UInt32 numFloorLevel);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllFloorLevel(IntPtr context, UInt32 deviceId, out IntPtr floorLevelObj, out UInt32 numFloorLevel);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetFloorLevel(IntPtr context, UInt32 deviceId, IntPtr floorLevels, UInt32 floorLevelCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveFloorLevel(IntPtr context, UInt32 deviceId, IntPtr floorLevelIds, UInt32 floorLevelIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllFloorLevel(IntPtr context, UInt32 deviceId);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< USB Exported API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserDatabaseInfoFromDir(IntPtr context, string szDir, out UInt32 numUsers, out UInt32 numCards, out UInt32 numFingers, out UInt32 numFaces, IsAcceptableUserID cbIsAcceptableUserID);
        extern public static int BS2_GetUserDatabaseInfoFromDir(IntPtr context, IntPtr szDir, out UInt32 numUsers, out UInt32 numCards, out UInt32 numFingers, out UInt32 numFaces, IsAcceptableUserID cbIsAcceptableUserID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserListFromDir(IntPtr context, string szDir, out IntPtr outUidObjs, out UInt32 outNumUids, IsAcceptableUserID cbIsAcceptableUserID);
        extern public static int BS2_GetUserListFromDir(IntPtr context, IntPtr szDir, out IntPtr outUidObjs, out UInt32 outNumUids, IsAcceptableUserID cbIsAcceptableUserID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserInfosFromDir(IntPtr context, string szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlob[] userBlobs);
        extern public static int BS2_GetUserInfosFromDir(IntPtr context, IntPtr szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlob[] userBlobs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserDatasFromDir(IntPtr context, string szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlob[] userBlobs, UInt32 userMask);
        extern public static int BS2_GetUserDatasFromDir(IntPtr context, IntPtr szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlob[] userBlobs, UInt32 userMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserInfosExFromDir(IntPtr context, string szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlobEx[] userBlobs);
        extern public static int BS2_GetUserInfosExFromDir(IntPtr context, IntPtr szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlobEx[] userBlobs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserDatasExFromDir(IntPtr context, string szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlobEx[] userBlobs, UInt32 userMask);
        extern public static int BS2_GetUserDatasExFromDir(IntPtr context, IntPtr szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserBlobEx[] userBlobs, UInt32 userMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetLogFromDir(IntPtr context, string szDir, UInt32 eventId, UInt32 amount, out IntPtr logObjs, out UInt32 numLog);
        extern public static int BS2_GetLogFromDir(IntPtr context, IntPtr szDir, UInt32 eventId, UInt32 amount, out IntPtr logObjs, out UInt32 numLog);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetFilteredLogFromDir(IntPtr context, string szDir, IntPtr uid, UInt16 eventCode, UInt32 start, UInt32 end, byte tnakey, out IntPtr logObjs, out UInt32 numLog);
        extern public static int BS2_GetFilteredLogFromDir(IntPtr context, IntPtr szDir, IntPtr uid, UInt16 eventCode, UInt32 start, UInt32 end, byte tnakey, out IntPtr logObjs, out UInt32 numLog);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetLogBlobFromDir(IntPtr context, string szDir, UInt16 eventMask, UInt32 eventId, UInt32 amount, out IntPtr logObjs, out UInt32 numLog);
        extern public static int BS2_GetLogBlobFromDir(IntPtr context, IntPtr szDir, UInt16 eventMask, UInt32 eventId, UInt32 amount, out IntPtr logObjs, out UInt32 numLog);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< WRAPPER >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */        
        public static BS2ErrorCode CSP_BS2_GetAllAccessSchedule(IntPtr context, UInt32 deviceId, out CSP_BS2Schedule[] accessScheduleObj, out UInt32 numAccessSchedule)
        {
            return Util.CSP_BS2_GetAll<CSP_BS2Schedule, CXX_BS2Schedule>(context, deviceId, out accessScheduleObj, out numAccessSchedule, API.BS2_GetAllAccessSchedule);
        }

        public static BS2ErrorCode CSP_BS2_GetAccessSchedule(IntPtr context, UInt32 deviceId, BS2_SCHEDULE_ID[] accessScheduleIds, UInt32 accessScheduleIdCount, out CSP_BS2Schedule[] accessScheduleObj, out UInt32 numAccessSchedule)
        {
            return Util.CSP_BS2_GetItems<BS2_SCHEDULE_ID, CSP_BS2Schedule, BS2_SCHEDULE_ID, CXX_BS2Schedule>(context, deviceId, accessScheduleIds, accessScheduleIdCount, out accessScheduleObj, out numAccessSchedule, API.BS2_GetAccessSchedule);
        }

        public static BS2ErrorCode CSP_BS2_RemoveAccessSchedule(IntPtr context, UInt32 deviceId, BS2_SCHEDULE_ID[] accessScheduleIds, UInt32 accessScheduleIdCount)
        {
            return Util.CSP_BS2_RemoveItems<BS2_SCHEDULE_ID, BS2_SCHEDULE_ID>(context, deviceId, accessScheduleIds, accessScheduleIdCount, API.BS2_RemoveAccessSchedule);
        }

        public static BS2ErrorCode CSP_BS2_SetAccessSchedule(IntPtr context, UInt32 deviceId, CSP_BS2Schedule[] accessSchedules, UInt32 accessScheduleCount)
        {
            return Util.CSP_BS2_SetItems<CSP_BS2Schedule, CXX_BS2Schedule>(context, deviceId, accessSchedules, accessScheduleCount, API.BS2_SetAccessSchedule);
        }

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< SlaveEx Control API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSlaveExDevice(IntPtr context, UInt32 deviceId, UInt32 channelport, out IntPtr slaveDeviceObj, out UInt32 outchannelport, out UInt32 slaveDeviceCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSlaveExDevice(IntPtr context, UInt32 deviceId, UInt32 channelport, IntPtr slaveDeviceObj, UInt32 slaveDeviceCount);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< IntrusionAlarmZone API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIntrusionAlarmZone(IntPtr context, UInt32 deviceId, [In, Out] BS2IntrusionAlarmZoneBlob[] zoneBlobs, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIntrusionAlarmZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllIntrusionAlarmZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIntrusionAlarmZone(IntPtr context, UInt32 deviceId, [In, Out] BS2IntrusionAlarmZoneBlob[] zoneBlobs, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIntrusionAlarmZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveIntrusionAlarmZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllIntrusionAlarmZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIntrusionAlarmZoneArm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        #region DEVICE_ZONE_SUPPORTED
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceZoneMasterConfig(IntPtr context, UInt32 deviceId, out BS2DeviceZoneMasterConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceZoneMasterConfig(IntPtr context, UInt32 deviceId, ref BS2DeviceZoneMasterConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveDeviceZoneMasterConfig(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceZone(IntPtr context, UInt32 deviceId, IntPtr Ids, UInt32 IdCount, out IntPtr deviceZoneObj, out UInt32 numDeviceZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllDeviceZone(IntPtr context, UInt32 deviceId, out IntPtr deviceZoneObj, out UInt32 numDeviceZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceZone(IntPtr context, UInt32 deviceId, IntPtr deviceZones, UInt32 deviceZoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveDeviceZone(IntPtr context, UInt32 deviceId, IntPtr Ids, UInt32 IdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllDeviceZone(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceZoneConfig(IntPtr context, UInt32 deviceId, out BS2DeviceZoneConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceZoneConfig(IntPtr context, UInt32 deviceId, ref BS2DeviceZoneConfig config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearDeviceZoneAccessRecord(IntPtr context, UInt32 deviceId, UInt32 zoneID, IntPtr uids, UInt32 uidCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ClearAllDeviceZoneAccessRecord(IntPtr context, UInt32 deviceId, UInt32 zoneID);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDeviceZoneAGEntranceLimit(IntPtr context, UInt32 deviceId, /*UInt32[]*/IntPtr Ids, UInt32 IdCount, out /*BS2DeviceZoneAGEntranceLimit[]*/IntPtr deviceZoneAGEntranceLimitObj, out UInt32 numDeviceZoneAGEntranceLimit);
        
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllDeviceZoneAGEntranceLimit(IntPtr context, UInt32 deviceId, out /*BS2DeviceZoneAGEntranceLimit[]*/ IntPtr deviceZoneAGEntranceLimitObj, out UInt32 numDeviceZoneAGEntranceLimit);
        
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDeviceZoneAGEntranceLimit(IntPtr context, UInt32 deviceId, /*BS2DeviceZoneAGEntranceLimit[]*/IntPtr deviceZoneAGEntranceLimits, UInt32 deviceZoneAGEntranceLimitCount);
        
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveDeviceZoneAGEntranceLimit(IntPtr context, UInt32 deviceId, /*UInt32[]*/IntPtr Ids, UInt32 IdCount);
        
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllDeviceZoneAGEntranceLimit(IntPtr context, UInt32 deviceId);

        #endregion

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSupportedConfigMask(IntPtr context, UInt32 deviceId, out BS2_CONFIG_MASK configMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSupportedUserMask(IntPtr context, UInt32 deviceId, out BS2_USER_MASK userMask);

        //Debugging
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CBDebugPrint(string msg);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDebugLevel(CBDebugPrint ptrCBDebugPrint, UInt32 debugLevel);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CBDebugExPrint(UInt32 level, UInt32 module, string msg);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDebugExCallback(CBDebugExPrint ptrCBDebugExPrint, UInt32 level, UInt32 module);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_SetDebugFileLog(UInt32 level, UInt32 module, string logPath);
        extern public static int BS2_SetDebugFileLog(UInt32 level, UInt32 module, IntPtr logPath);

        /* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< InterlockZone API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetInterlockZone(IntPtr context, UInt32 deviceId, [In, Out] BS2InterlockZoneBlob[] zoneBlob, out UInt32 numZone);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetInterlockZoneStatus(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllInterlockZoneStatus(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetInterlockZone(IntPtr context, UInt32 deviceId, [In, Out] BS2InterlockZoneBlob[] zoneBlobs, UInt32 zoneCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetInterlockZoneAlarm(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveInterlockZone(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllInterlockZone(IntPtr context, UInt32 deviceId);

        //=> [IPv6]
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetIPConfigViaUDPEx(IntPtr context, UInt32 deviceId, out BS2IpConfig config, string hostipAddr);
        extern public static int BS2_GetIPConfigViaUDPEx(IntPtr context, UInt32 deviceId, out BS2IpConfig config, IntPtr hostipAddr);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_SetIPConfigViaUDPEx(IntPtr context, UInt32 deviceId, ref BS2IpConfig config, string hostipAddr);
        extern public static int BS2_SetIPConfigViaUDPEx(IntPtr context, UInt32 deviceId, ref BS2IpConfig config, IntPtr hostipAddr);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPV6Config(IntPtr context, UInt32 deviceId, out BS2IPV6Config config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPV6Config(IntPtr context, UInt32 deviceId, ref BS2IPV6Config config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPV6ConfigViaUDP(IntPtr context, UInt32 deviceId, out BS2IPV6Config config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPV6ConfigViaUDP(IntPtr context, UInt32 deviceId, ref BS2IPV6Config config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetIPV6ConfigViaUDPEx(IntPtr context, UInt32 deviceId, out BS2IPV6Config config, string hostipAddr);
        extern public static int BS2_GetIPV6ConfigViaUDPEx(IntPtr context, UInt32 deviceId, out BS2IPV6Config config, IntPtr hostipAddr);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_SetIPV6ConfigViaUDPEx(IntPtr context, UInt32 deviceId, ref BS2IPV6Config config, string hostipAddr);
        extern public static int BS2_SetIPV6ConfigViaUDPEx(IntPtr context, UInt32 deviceId, ref BS2IPV6Config config, IntPtr hostipAddr);
        //<=

        //=> [IPv6]
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetEnableIPV4(IntPtr context, out int enable);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetEnableIPV4(IntPtr context, int enable);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetEnableIPV6(IntPtr context, out int enable);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetEnableIPV6(IntPtr context, int enable);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetServerPortIPV6(IntPtr context, UInt16 serverPort);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetServerPortIPV6(IntPtr context, out UInt16 serverPort);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetSSLServerPortIPV6(IntPtr context, UInt16 sslServerPort);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSSLServerPortIPV6(IntPtr context, out UInt16 sslServerPort);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSpecifiedDeviceInfo(IntPtr context, UInt32 deviceId, UInt32 specifiedDeviceInfo, IntPtr pOutDeviceInfo, UInt32 nDeviceInfoSize, out UInt32 pOutDeviceInfoSize);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_ConnectDeviceIPV6(IntPtr context, UInt32 deviceId);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_SearchDevicesCoreStationEx(IntPtr context, string hostipAddr);
        extern public static int BS2_SearchDevicesCoreStationEx(IntPtr context, IntPtr hostipAddr);
        //<=

        //Beta => [Get ServerPort]
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetServerPort(IntPtr context, out UInt16 serverPort);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSSLServerPort(IntPtr context, out UInt16 sslServerPort);
        //Beta <=

        //=> [Admin 1000]
		/* <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Auth Operator Level Ex API >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> */
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAuthOperatorLevelEx(IntPtr context, UInt32 deviceId, IntPtr userIDs, UInt32 userIDCount, out IntPtr operatorlevelObj, out UInt32 numOperatorlevel);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetAllAuthOperatorLevelEx(IntPtr context, UInt32 deviceId, out IntPtr operatorlevelObj, out UInt32 numOperatorlevel);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetAuthOperatorLevelEx(IntPtr context, UInt32 deviceId, IntPtr operatorlevels, UInt32 operatorlevelCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAuthOperatorLevelEx(IntPtr context, UInt32 deviceId, IntPtr userIDs, UInt32 userIDCount);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_RemoveAllAuthOperatorLevelEx(IntPtr context, UInt32 deviceId);
		//<=
        
        //=> [Set/Get default response wait timeout]
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetDefaultResponseTimeout(IntPtr context, Int32 ms);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetDefaultResponseTimeout(IntPtr context, out Int32 poMs);
        //<=

        //UserSmall
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_VerifyUserSmall(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserSmallBlob userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_IdentifyUserSmall(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserSmallBlob userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserSmallInfos(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlob[] userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserSmallDatas(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlob[] userBlob, BS2_USER_MASK userMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EnrollUserSmall(IntPtr context, UInt32 deviceId, [In, Out] BS2UserSmallBlob[] userBlob, UInt32 userCount, byte overwrite);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_VerifyUserSmallEx(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserSmallBlobEx userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_IdentifyUserSmallEx(IntPtr context, UInt32 deviceId, UInt16 seq, int handleResult, ref BS2UserSmallBlobEx userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserSmallInfosEx(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlobEx[] userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetUserSmallDatasEx(IntPtr context, UInt32 deviceId, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlobEx[] userBlob, BS2_USER_MASK userMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_EnrollUserSmallEx(IntPtr context, UInt32 deviceId, [In, Out] BS2UserSmallBlobEx[] userBlob, UInt32 userCount, byte overwrite);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserSmallInfosFromDir(IntPtr context, string szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlob[] userBlob);
        extern public static int BS2_GetUserSmallInfosFromDir(IntPtr context, IntPtr szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlob[] userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserSmallDatasFromDir(IntPtr context, string szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlob[] userBlob, BS2_USER_MASK userMask);
        extern public static int BS2_GetUserSmallDatasFromDir(IntPtr context, IntPtr szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlob[] userBlob, BS2_USER_MASK userMask);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserSmallInfosExFromDir(IntPtr context, string szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlobEx[] userBlob);
        extern public static int BS2_GetUserSmallInfosExFromDir(IntPtr context, IntPtr szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlobEx[] userBlob);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        //extern public static int BS2_GetUserSmallDatasExFromDir(IntPtr context, string szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlobEx[] userBlob, BS2_USER_MASK userMask);
        extern public static int BS2_GetUserSmallDatasExFromDir(IntPtr context, IntPtr szDir, IntPtr uids, UInt32 uidCount, [In, Out] BS2UserSmallBlobEx[] userBlob, BS2_USER_MASK userMask);

    }
}
