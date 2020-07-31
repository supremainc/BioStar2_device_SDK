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

namespace Suprema
{
    using BS2_USER_MASK = UInt32;

    //[802.1x][ET]
    public enum BS2Set8021xTypeMaskEnum : uint
    {
        BS2_SET_8021X_TYPE_ROOT_CA = 0,
        BS2_SET_8021X_TYPE_CLIENT_CA = 1,
        BS2_SET_8021X_TYPE_CSR_INFO = 2,
        BS2_SET_8021X_TYPE_PRIVATE_KEY = 3,
        BS2_SET_8021X_TYPE_WIRED_INFO = 4,
        BS2_SET_8021X_TYPE_WPA_GET_LOG = 5,

        BS2_SET_8021X_TYPE_UNKNOWN = 0xFF,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct BS28021xWiredInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] identity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] password;
    }


    //[WriteCardToC][ET]
    public static class CONST_BS1SmartCardHeader
    {
        public const int MAX_TEMPLATE = 4;
        public const int MAX_ACCESS_GROUP = 4;
        public const int MAX_NAME_LEN = 16;
        public const int PASSWORD_LEN = 8;

        public const int MIFARE_VER_1_0 = 0x10;

        // security level
        public const int USER_SECURITY_DEFAULT = 0;
        public const int USER_SECURITY_LOWER = 1;
        public const int USER_SECURITY_LOW = 2;
        public const int USER_SECURITY_NORMAL = 3;
        public const int USER_SECURITY_HIGH = 4;
        public const int USER_SECURITY_HIGHER = 5;

        // admin level
        public const int USER_LEVEL_NORMAL = 0;
        public const int USER_LEVEL_ADMIN = 1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BS1SmartCardHeader
    {
        public UInt32 userID;
        public byte version;
        public byte numOfTemplate;
        public byte adminLevel;
        public byte securityLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = CONST_BS1SmartCardHeader.MAX_TEMPLATE)]
        public byte[] duress;
        public byte isBypassCard;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = CONST_BS1SmartCardHeader.MAX_ACCESS_GROUP)]
        public byte[] accessGroup;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = CONST_BS1SmartCardHeader.MAX_NAME_LEN)]
        public byte[] userName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = CONST_BS1SmartCardHeader.PASSWORD_LEN)]
        public byte[] password;
        public Int32 startTime;
        public Int32 expiryTime;
        public UInt16 hdrCRC;
        public UInt16 reserved3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BS1SmartCardData
    {
        public BS1SmartCardHeader header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_SMART_CARD_MAX_TEMPLATE_COUNT * BS2Environment.BS2_FINGER_TEMPLATE_SIZE)]
        public byte[] templateData;
    }

    //[IPv6][ET]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2IpConfigExt_ET
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV4_ADDR_SIZE)]
        public byte[] dnsAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_URL_SIZE)]
        public byte[] serverUrl;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] ipv6Addr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] ipv6ServerAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ipv6SubLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] ipv6GateWayAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] dnsAddrIpv6;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SEosCard_ET
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_primaryKeyENC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_primaryKeyMAC;	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_primaryKeyAuth;	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_secondaryKeyENC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_secondaryKeyMAC;	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_secondaryKeyAuth;	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_configCardKeyENC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_configCardKeyMAC;	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_configCardKeyAuth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	    public byte[] oid_ADF;
	    public byte size_ADF;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public byte[] oid_DataObjectID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public byte[] size_DataObject;
	    public byte referenceDataQualifier;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] primaryKeyENC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] primaryKeyMAC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] primaryKeyAuth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] secondaryKeyENC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] secondaryKeyMAC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	    public byte[] secondaryKeyAuth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
	    public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2CardConfigEx_ET
    {
        public byte byteOrder;
        public byte useWiegandFormat;
        public byte dataType;
        public byte useSecondaryKey;

        public BS2MifareCard mifare;
        public BS2IClassCard iclass;
        public BS2DesFireCard desfire;

        public UInt32 formatID;
        public byte turnOffRF;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
        public byte[] reserved;

        public BS2SEosCard_ET seos;
    }

    static class API_ET
    {
        //[SSL][ET]
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetSSLExpiryTime_ET(IntPtr context, UInt32 deviceId, out UInt32 gmtTime);

        //[WriteCardToC][ET]
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_WriteCardForV1_ET(IntPtr context, UInt32 deviceId, ref BS1SmartCardData smartCard);

        //[802.1x][ET]
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetNetworkFor802_1x_ET(IntPtr context, UInt32 deviceId, BS2Set8021xTypeMaskEnum type, IntPtr data, int dataLen, IntPtr resData, out UInt32 resLen);

        //[IPv6][ET]
        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_IsDeviceOf_ET(IntPtr context, UInt32 deviceId, out int poIs);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetIPConfigExt_ET(IntPtr context, UInt32 deviceId, out BS2IpConfigExt_ET config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetIPConfigExt_ET(IntPtr context, UInt32 deviceId, ref BS2IpConfigExt_ET config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_GetCardConfigEx_ET(IntPtr context, UInt32 deviceId, out BS2CardConfigEx_ET config);

        [DllImport("BS_SDK_V2.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int BS2_SetCardConfigEx_ET(IntPtr context, UInt32 deviceId, ref BS2CardConfigEx_ET config);
    }


    public class V2_6_3_ET_Control : FunctionModule
    {
        private const int USER_PAGE_SIZE = 1024;

        private API.OnReadyToScan cbCardOnReadyToScan = null;
        private API.OnReadyToScan cbFingerOnReadyToScan = null;
        private API.OnReadyToScan cbFaceOnReadyToScan = null;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("[ET] Get IPv6", getIPv6_ET));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("[ET] Set IPv6", setIPv6_ET));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("[ET] Set 802.1x", set8021xNetwork));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("[ET] Get SSL Expiry time of Root CA", getSSLExpiryTime));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("[ET] Write card to V1 ToC", writeCardForV1));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("[ET] Get CardConfigEx", getCardConfig_ET));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("[ET] Set CardConfigEx", setCardConfig_ET));

            return functionList;
        }

        
        public void set8021xNetwork(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            Console.WriteLine("Enter the path of root ca file.");
            Console.Write(">>>> ");
            string firmwarePath = Console.ReadLine();
            UInt32 outLen = 0;

            if (!File.Exists(firmwarePath))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            IntPtr firmwareData = IntPtr.Zero;
            IntPtr outData = IntPtr.Zero;
            UInt32 firmwareDataLen = 0;

            if (Util.LoadBinary(firmwarePath, out firmwareData, out firmwareDataLen))
            {
                Console.WriteLine("Send 802.1x Root CA");

                result = (BS2ErrorCode)API_ET.BS2_SetNetworkFor802_1x_ET(sdkContext, deviceID, BS2Set8021xTypeMaskEnum.BS2_SET_8021X_TYPE_ROOT_CA, firmwareData, (int)firmwareDataLen, outData, out outLen);
                Marshal.FreeHGlobal(firmwareData);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
            }            

            Console.WriteLine("Enter the path of clientCA file.");
            Console.Write(">>>> ");
            firmwarePath = Console.ReadLine();
            outLen = 0;

            if (!File.Exists(firmwarePath))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            if (Util.LoadBinary(firmwarePath, out firmwareData, out firmwareDataLen))
            {
                Console.WriteLine("Send 802.1x Client CA");

                result = (BS2ErrorCode)API_ET.BS2_SetNetworkFor802_1x_ET(sdkContext, deviceID, BS2Set8021xTypeMaskEnum.BS2_SET_8021X_TYPE_CLIENT_CA, firmwareData, (int)firmwareDataLen, outData, out outLen);
                Marshal.FreeHGlobal(firmwareData);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
            }

            Console.WriteLine("Enter the path of Private Key file.");
            Console.Write(">>>> ");
            firmwarePath = Console.ReadLine();
            outLen = 0;

            if (!File.Exists(firmwarePath))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            if (Util.LoadBinary(firmwarePath, out firmwareData, out firmwareDataLen))
            {
                Console.WriteLine("Send 802.1x Root Private Key");

                result = (BS2ErrorCode)API_ET.BS2_SetNetworkFor802_1x_ET(sdkContext, deviceID, BS2Set8021xTypeMaskEnum.BS2_SET_8021X_TYPE_PRIVATE_KEY, firmwareData, (int)firmwareDataLen, outData, out outLen);
                Marshal.FreeHGlobal(firmwareData);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
            }

            Console.WriteLine("Enter the path of CSR Info file.");
            Console.Write(">>>> ");
            firmwarePath = Console.ReadLine();
            outLen = 0;
            string saveFilePath = "d:\\csr_recv.csr";

            if (!File.Exists(firmwarePath))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            if (Util.LoadBinary(firmwarePath, out firmwareData, out firmwareDataLen))
            {
                Console.WriteLine("Send 802.1x CSR Info");

                outData = Marshal.AllocHGlobal(4096);
                result = (BS2ErrorCode)API_ET.BS2_SetNetworkFor802_1x_ET(sdkContext, deviceID, BS2Set8021xTypeMaskEnum.BS2_SET_8021X_TYPE_CSR_INFO, firmwareData, (int)firmwareDataLen, outData, out outLen);
                Marshal.FreeHGlobal(firmwareData);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                //file write
                Console.WriteLine("CSR file size({0})", outLen);
                Console.Write(outData);

                SaveFile(saveFilePath, outData, outLen);
                Marshal.FreeHGlobal(outData);
            }

            //Console.WriteLine("Enter the path of wired config file.");
            //string firmwarePath = Console.ReadLine();
            outLen = 0;

            BS28021xWiredInfo WiredInfoData = Util.AllocateStructure<BS28021xWiredInfo>();
            byte[] RawData = Encoding.ASCII.GetBytes("test");
            Array.Copy(RawData, 0, WiredInfoData.identity, 0, RawData.Length);
            byte[] RawData2 = Encoding.ASCII.GetBytes("suprema!1");
            Array.Copy(RawData2, 0, WiredInfoData.password, 0, RawData2.Length);

            int structSize = Marshal.SizeOf(typeof(BS28021xWiredInfo));
            IntPtr dataPtr = Marshal.AllocHGlobal(structSize);
            Marshal.StructureToPtr(WiredInfoData, dataPtr, false);

            Console.WriteLine("Get Wired info");
            result = (BS2ErrorCode)API_ET.BS2_SetNetworkFor802_1x_ET(sdkContext, deviceID, BS2Set8021xTypeMaskEnum.BS2_SET_8021X_TYPE_WIRED_INFO, dataPtr, structSize, outData, out outLen);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(dataPtr);


            Console.WriteLine("WPA Get Log");
            // int outLen = 0;
            saveFilePath = "d:\\dbg_recv.txt";

            outData = Marshal.AllocHGlobal(40960);

            result = (BS2ErrorCode)API_ET.BS2_SetNetworkFor802_1x_ET(sdkContext, deviceID, BS2Set8021xTypeMaskEnum.BS2_SET_8021X_TYPE_WPA_GET_LOG, IntPtr.Zero, 0, outData, out outLen);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS && result != BS2ErrorCode.BS_SDK_ERROR_CANNOT_OPEN_FILE)
            {
                Console.WriteLine("Got error({0}).", result);
            }


            //file write
            Console.WriteLine("Get Log file size({0})", outLen);
            Console.Write(outData);
            SaveFile(saveFilePath, outData, outLen);
        }

        public static bool SaveFile(string filePath, IntPtr binaryData, UInt32 binaryDataLen)
        {
            bool handled = false;
            FileStream fs = null;

            try
            {
                fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                int fileSize = (int)binaryDataLen;

                Byte[] readBuffer = new byte[fileSize];
                Byte[] asciiByte = Encoding.ASCII.GetBytes("ASCII string to byte!");

                Marshal.Copy(binaryData, readBuffer, 0, fileSize);
                fs.Write(readBuffer, 0, readBuffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error write file {0}. Message = {1}", filePath, e.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return handled;
        }

        public void getSSLExpiryTime(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            {
                int isET = 0;
                Console.WriteLine("Trying to get GetIPConfigExt_ET");
                BS2ErrorCode result_ = (BS2ErrorCode)API_ET.BS2_IsDeviceOf_ET(sdkContext, deviceID, out isET);
                if (result_ != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result_);
                }

                if (isET == 0)
                {
                    Console.WriteLine("It is not an ET device. Do you want to proceed ? [Y/n] ");
                    if (!Util.IsYes())
                        return;
                }
            }

            UInt32 timestamp = 0;
            Console.WriteLine("Trying to get ssl expiry time");
            BS2ErrorCode result = (BS2ErrorCode)API_ET.BS2_GetSSLExpiryTime_ET(sdkContext, deviceID, out timestamp);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                DateTime currentTime = Util.ConvertFromUnixTimestamp(timestamp);
                Console.WriteLine(">>>> SSL Expiry time : " + currentTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        void print(BS2IpConfigExt_ET config)
        {
            Console.WriteLine(">>>> IPExt_ET configuration ");
            Console.WriteLine("     |--dnsAddr : {0}", Encoding.UTF8.GetString(config.dnsAddr), BitConverter.ToString(config.dnsAddr));
            Console.WriteLine("     |--serverUrl : {0}", Encoding.UTF8.GetString(config.serverUrl), BitConverter.ToString(config.serverUrl));
            Console.WriteLine("     |--ipv6Addr : {0}", Encoding.UTF8.GetString(config.ipv6Addr), BitConverter.ToString(config.ipv6Addr));
            Console.WriteLine("     |--ipv6ServerAddr : {0}", Encoding.UTF8.GetString(config.ipv6ServerAddr), BitConverter.ToString(config.ipv6ServerAddr));
            Console.WriteLine("     |--ipv6SubLen : {0}", Encoding.UTF8.GetString(config.ipv6SubLen), BitConverter.ToString(config.ipv6SubLen));
            Console.WriteLine("     |--ipv6GateWayAddr : {0}", Encoding.UTF8.GetString(config.ipv6GateWayAddr), BitConverter.ToString(config.ipv6GateWayAddr));
            Console.WriteLine("     |--dnsAddrIpv6 : {0}", Encoding.UTF8.GetString(config.dnsAddrIpv6), BitConverter.ToString(config.dnsAddrIpv6));            
            Console.WriteLine("<<<< ");
        }


        public void getIPv6_ET(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            {
                int isET = 0;
                Console.WriteLine("Trying to get GetIPConfigExt_ET");
                BS2ErrorCode result_ = (BS2ErrorCode)API_ET.BS2_IsDeviceOf_ET(sdkContext, deviceID, out isET);
                if (result_ != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result_);
                }

                if (isET == 0)
                {                    
                    Console.WriteLine("It is not an ET device. Do you want to proceed ? [Y/n] ");
                    if (!Util.IsYes())
                        return;
                }
            }

            BS2IpConfigExt_ET config;
            Console.WriteLine("Trying to get GetIPConfigExt_ET");
            BS2ErrorCode result = (BS2ErrorCode)API_ET.BS2_GetIPConfigExt_ET(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }
        }

        public void setIPv6_ET(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            {
                int isET = 0;
                Console.WriteLine("Trying to get GetIPConfigExt_ET");
                BS2ErrorCode result_ = (BS2ErrorCode)API_ET.BS2_IsDeviceOf_ET(sdkContext, deviceID, out isET);
                if (result_ != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result_);
                }

                if (isET == 0)
                {
                    Console.WriteLine("It is not an ET device. Do you want to proceed ? [Y/n] ");
                    if (!Util.IsYes())
                        return;
                }
            }

            BS2IpConfigExt_ET config;
            Console.WriteLine("Trying to get IPConfigExt_ET");
            BS2ErrorCode result = (BS2ErrorCode)API_ET.BS2_GetIPConfigExt_ET(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }


            Console.WriteLine("Enter DNS Address: ");
            string strInput = Console.ReadLine();
            Array.Copy(Encoding.ASCII.GetBytes(strInput), 0, config.dnsAddr, 0, Encoding.ASCII.GetBytes(strInput).Length);

            Console.WriteLine("Enter Server URL: ");
            strInput = Console.ReadLine();
            Array.Copy(Encoding.ASCII.GetBytes(strInput), 0, config.serverUrl, 0, Encoding.ASCII.GetBytes(strInput).Length);

            Console.WriteLine("Enter IPv6 Address: ");
            strInput = Console.ReadLine();
            Array.Copy(Encoding.ASCII.GetBytes(strInput), 0, config.ipv6Addr, 0, Encoding.ASCII.GetBytes(strInput).Length);

            Console.WriteLine("Enter IPv6 Submask prefix len: ");
            strInput = Console.ReadLine();
            Array.Copy(Encoding.ASCII.GetBytes(strInput), 0, config.ipv6SubLen, 0, Encoding.ASCII.GetBytes(strInput).Length);

            Console.WriteLine("Enter IPv6 Gateway Address: ");
            strInput = Console.ReadLine();
            Array.Copy(Encoding.ASCII.GetBytes(strInput), 0, config.ipv6GateWayAddr, 0, Encoding.ASCII.GetBytes(strInput).Length);

            Console.WriteLine("Enter IPv6 Server Address (IPv6): ");
            strInput = Console.ReadLine();
            Array.Copy(Encoding.ASCII.GetBytes(strInput), 0, config.ipv6ServerAddr, 0, Encoding.ASCII.GetBytes(strInput).Length);

            Console.WriteLine("Enter IPv6 DNS Address: ");
            strInput = Console.ReadLine();
            Array.Copy(Encoding.ASCII.GetBytes(strInput), 0, config.dnsAddrIpv6, 0, Encoding.ASCII.GetBytes(strInput).Length);

            Console.WriteLine("Trying to set IPConfigExt_ET");
            result = (BS2ErrorCode)API_ET.BS2_SetIPConfigExt_ET(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            BS2IpConfigExt_ET changedconfig = Util.AllocateStructure<BS2IpConfigExt_ET>();
            Console.WriteLine("Trying to get Changed IPConfigExt_ET");
            result = (BS2ErrorCode)API_ET.BS2_GetIPConfigExt_ET(sdkContext, deviceID, out changedconfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(changedconfig);
            }
        }

        void print(BS1CardConfig config)
        {
            Console.WriteLine(">>>> BS1 Card Config");
            Console.WriteLine("     |--magicNo[{0}]", config.magicNo);
            Console.WriteLine("     |--disabled[{0}]", config.disabled);
            Console.WriteLine("     |--useCSNOnly[{0}]", config.useCSNOnly);
            Console.WriteLine("     |--bioentryCompatible[{0}]", config.bioentryCompatible);
            Console.WriteLine("     |--useSecondaryKey[{0}]", config.useSecondaryKey);
            Console.WriteLine("     |--primaryKey[{0}]", BitConverter.ToString(config.primaryKey));
            Console.WriteLine("     |--secondaryKey[{0}]", BitConverter.ToString(config.secondaryKey));
            Console.WriteLine("     |--cisIndex[{0}]", config.cisIndex);
            Console.WriteLine("     |--numOfTemplate[{0}]", config.numOfTemplate);
            Console.WriteLine("     |--templateSize[{0}]", config.templateSize);           
            Console.WriteLine("     |--templateStartBlock[0] = [{0}]", config.templateStartBlock[0]);
            Console.WriteLine("     |--templateStartBlock[1] = [{0}]", config.templateStartBlock[1]);
            Console.WriteLine("     |--templateStartBlock[2] = [{0}]", config.templateStartBlock[2]);
            Console.WriteLine("     |--templateStartBlock[3] = [{0}]", config.templateStartBlock[3]);
            Console.WriteLine("<<<< ");
        }

        BS2ErrorCode computeCRCForV1(BS1SmartCardHeader smartCard, out UInt16 hdrCRC)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            IntPtr smartCardObj = Marshal.AllocHGlobal(Marshal.SizeOf(smartCard));

            //int cardCRCOffset = (int)Marshal.OffsetOf(typeof(BS1SmartCardHeader), "userID");

            Marshal.StructureToPtr(smartCard, smartCardObj, false);

            hdrCRC = 0xFFFF;

            result = (BS2ErrorCode)API.BS2_ComputeCRC16CCITT(smartCardObj, (UInt32)(Marshal.SizeOf(typeof(BS1SmartCardHeader)) - 4), ref hdrCRC);

            Marshal.FreeHGlobal(smartCardObj);
            return result;
        }

        public void writeCardForV1(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2SimpleDeviceInfo deviceInfo;
            BS1SmartCardData smartCard = Util.AllocateStructure<BS1SmartCardData>();

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceInfo(sdkContext, deviceID, out deviceInfo);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't get device information(errorCode : {0}).", result);
                return;
            }

            BS1CardConfig cardConfig;

            Console.WriteLine("Trying to get card configuration for v1.x.");
            result = (BS2ErrorCode)API.BS2_GetCard1xConfig(sdkContext, deviceID, out cardConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(cardConfig);
            }


            // Console.WriteLine("choose the format for the card: [2: SecureCredential(default), 3: AccessOn]");
            // Console.Write(">>>> ");
            //smartCard.header. = Util.GetInput((byte)BS2CardTypeEnum.SECURE);

            Array.Clear(smartCard.header.reserved2, 0, 3);
            Array.Clear(smartCard.header.accessGroup, 0, CONST_BS1SmartCardHeader.MAX_ACCESS_GROUP);
            Array.Clear(smartCard.header.userName, 0, CONST_BS1SmartCardHeader.MAX_NAME_LEN);
            Array.Clear(smartCard.header.password, 0, CONST_BS1SmartCardHeader.PASSWORD_LEN);

            if (Convert.ToBoolean(deviceInfo.pinSupported))
            {
                Console.WriteLine("Do you want to set pin code? [y/N]");
                Console.Write(">>>> ");
                if (!Util.IsNo())
                {
                    Console.WriteLine("Enter the pin code which you want to set");
                    Console.Write(">>>> ");
                    string pinCodeStr = Console.ReadLine();
                    IntPtr pinChar = Marshal.StringToHGlobalAnsi(pinCodeStr);
                    IntPtr pinCode = Marshal.AllocHGlobal(CONST_BS1SmartCardHeader.PASSWORD_LEN);
                    //result = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, pinCodeStr, pinCode);
                    result = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, pinChar, pinCode);

                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Got error({0}).", result);
                        Marshal.FreeHGlobal(pinCode);
                        return;
                    }

                    Marshal.Copy(pinCode, smartCard.header.password, 0, CONST_BS1SmartCardHeader.PASSWORD_LEN);
                    Marshal.FreeHGlobal(pinCode);
                }
            }

            if (Convert.ToBoolean(deviceInfo.fingerSupported))
            {
                Console.WriteLine("How many fingerprint templates do you want to set? [0(default)-{0}]", BS2Environment.BS2_SMART_CARD_MAX_TEMPLATE_COUNT);
                Console.Write(">>>> ");
                smartCard.header.numOfTemplate = Util.GetInput((byte)0);
            }
            else
            {
                smartCard.header.numOfTemplate = 0;
            }

            Array.Clear(smartCard.templateData, 0, BS2Environment.BS2_SMART_CARD_MAX_TEMPLATE_COUNT * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
            if (smartCard.header.numOfTemplate > 0)
            {
                BS2Fingerprint fingerprint = Util.AllocateStructure<BS2Fingerprint>();
                fingerprint.index = 0;

                for (byte idx = 0; idx < smartCard.header.numOfTemplate; ++idx)
                {
                    Console.WriteLine("Place your finger on the device for fingerprint template[{0}] extraction", idx);
                    result = (BS2ErrorCode)API.BS2_ScanFingerprint(sdkContext, deviceID, ref fingerprint, 0, (UInt32)BS2FingerprintQualityEnum.QUALITY_STANDARD, (byte)BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA, null);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        if (result == BS2ErrorCode.BS_SDK_ERROR_EXTRACTION_LOW_QUALITY || result == BS2ErrorCode.BS_SDK_ERROR_CAPTURE_LOW_QUALITY)
                        {
                            Console.WriteLine("Bad fingerprint quality. Tty agin.");
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("Got error({0}).", result);
                            return;
                        }
                    }

                    Array.Copy(fingerprint.data, 0, smartCard.templateData, idx * cardConfig.templateSize, cardConfig.templateSize);
                }

                Console.WriteLine("Is it duress finger? [0 : Normal(default), 1 : Duress]");
                Console.Write(">>>> ");
                if (!Util.IsNo())
                {
                    Console.WriteLine("Enter the index of duress finger[0(default)]");
                    Console.Write(">>>> ");
                    int duressIndex = Util.GetInput((UInt16)0);
                    smartCard.header.duress[duressIndex] = 1;
                }
                else
                {
                    Array.Clear(smartCard.header.duress, 0, BS2Environment.BS2_SMART_CARD_MAX_TEMPLATE_COUNT);
                }
            }
            //admin level
            Console.WriteLine("Enter the adminLevel[0(default)]");
            Console.Write(">>>> ");
            smartCard.header.adminLevel = Util.GetInput((byte)0); // 1 : admin level
            //securityLevel level
            Console.WriteLine("Enter the securityLevel[0(default)]");
            Console.Write(">>>> ");
            smartCard.header.securityLevel = Util.GetInput(0); // normal security

            Console.WriteLine("Enter start time which you want to set. [default(Today), yyyy-MM-dd HH:mm:ss]");
            Console.Write(">>>> ");
            UInt32 inputTime;
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out inputTime))
            {
                return;
            }
            smartCard.header.startTime = (int)inputTime;
            Console.WriteLine("Enter end time which you want to set. [default(Today), yyyy-MM-dd HH:mm:ss]");
            Console.Write(">>>> ");
            if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out inputTime))
            {
                return;
            }
            smartCard.header.expiryTime = (int)inputTime;

            Console.WriteLine("Enter the user id which you want to write on the card");
            Console.Write(">>>> ");

            UInt32 cardID = 0;
            string cardIDStr = Console.ReadLine();
            if (cardIDStr.Length > 10)
            {
                Console.WriteLine("Card id should less than 10 words.");
                return;
            }
            else if (!UInt32.TryParse(cardIDStr, out cardID) || cardID == 0)
            {
                Console.WriteLine("Invalid card id");
                return;
            }
            else
            {
                smartCard.header.userID = cardID;
            }

            result = computeCRCForV1(smartCard.header, out smartCard.header.hdrCRC);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't compute CRC16({0})", result);
            }
            else
            {
                Console.WriteLine("Trying to write card({0}).", smartCard.header.hdrCRC);
                result = (BS2ErrorCode)API_ET.BS2_WriteCardForV1_ET(sdkContext, deviceID, ref smartCard);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else
                {
                    Console.WriteLine("Successfully write TOC card");
                }
            }
        }

        void enterSmartcardKey(byte[] dst)
        {
            int index = 0;
            string[] keys = Console.ReadLine().Split('-');
            foreach (string key in keys)
            {
                dst[index++] = Convert.ToByte(key, 16);
                if (index > dst.Length)
                {
                    return;
                }
            }

            for (; index < dst.Length; ++index)
            {
                dst[index] = 0xFF;
            }
        }

        public void getCardConfig_ET(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            int isET = 0;
            Console.WriteLine("Trying to get CardConfigEx_ET");
            BS2ErrorCode result_ = (BS2ErrorCode)API_ET.BS2_IsDeviceOf_ET(sdkContext, deviceID, out isET);
            if (result_ != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result_);
            }

            if (isET == 0)
            {                    
                Console.WriteLine("It is not an ET device. Do you want to proceed ? [Y/n] ");
                if (!Util.IsYes())
                    return;
            }

            BS2CardConfigEx_ET config;
            BS2ErrorCode result = (BS2ErrorCode)API_ET.BS2_GetCardConfigEx_ET(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }
        }

        public void setCardConfig_ET(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            int isET = 0;
            BS2ErrorCode result_ = (BS2ErrorCode)API_ET.BS2_IsDeviceOf_ET(sdkContext, deviceID, out isET);
            if (result_ != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result_);
            }

            if (isET == 0)
            {
                Console.WriteLine("It is not an ET device. Do you want to proceed ? [Y/n] ");
                if (!Util.IsYes())
                    return;
            }

            BS2CardConfigEx_ET config;
            BS2ErrorCode result = (BS2ErrorCode)API_ET.BS2_GetCardConfigEx_ET(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }

            Console.WriteLine("Current byte order: {0}, [0: MSB, 1: LSB]", config.byteOrder);
            Console.WriteLine("Do you want to change byte order? [y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                config.byteOrder = 
                    (byte)BS2CardByteOrderEnum.MSB == config.byteOrder ? (byte)BS2CardByteOrderEnum.LSB : (byte)BS2CardByteOrderEnum.MSB;
            }

            Console.WriteLine("Trying to set CardConfigEx_ET");
            result = (BS2ErrorCode)API_ET.BS2_SetCardConfigEx_ET(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            BS2CardConfigEx_ET changedconfig;
            Console.WriteLine("Trying to get Changed CardConfigEx_ET");
            result = (BS2ErrorCode)API_ET.BS2_GetCardConfigEx_ET(sdkContext, deviceID, out changedconfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(changedconfig);
            }
        }

        void print(BS2CardConfigEx_ET config)
        {
            Console.WriteLine(">>>> BS2CardConfigEx_ET");
            Console.WriteLine("     |--byteOrder[{0}]", (BS2CardByteOrderEnum)config.byteOrder);
            Console.WriteLine("     |--useWiegandFormat[{0}]", Convert.ToBoolean(config.useWiegandFormat));
            Console.WriteLine("     |--dataType[{0}]", (BS2CardDataTypeEnum)config.dataType);
            Console.WriteLine("     |--useSecondaryKey[{0}]", Convert.ToBoolean(config.useSecondaryKey));
            Console.WriteLine("     +--BS2MifareCard");
            Console.WriteLine("     |  |--primaryKey[{0}]", BitConverter.ToString(config.mifare.primaryKey));
            Console.WriteLine("     |  |--secondaryKey[{0}]", BitConverter.ToString(config.mifare.secondaryKey));
            Console.WriteLine("     |  +--startBlockIndex[{0}]", config.mifare.startBlockIndex);
            Console.WriteLine("     +--BS2IClassCard");
            Console.WriteLine("     |-----primaryKey[{0}]", BitConverter.ToString(config.iclass.primaryKey));
            Console.WriteLine("     |-----secondaryKey[{0}]", BitConverter.ToString(config.iclass.secondaryKey));
            Console.WriteLine("     |-----startBlockIndex[{0}]", config.iclass.startBlockIndex);
            Console.WriteLine("     +--BS2DesFireCard");
            Console.WriteLine("     |-----primaryKey[{0}]", BitConverter.ToString(config.desfire.primaryKey));
            Console.WriteLine("     |-----secondaryKey[{0}]", BitConverter.ToString(config.desfire.secondaryKey));
            Console.WriteLine("     |-----appID[{0}]", BitConverter.ToString(config.desfire.appID));
            Console.WriteLine("     |-----fileID[{0}]", config.desfire.fileID);
            Console.WriteLine("     |-----encryptionType[{0}]", config.desfire.encryptionType);           
            Console.WriteLine("     |-----operationMode[{0}]", config.desfire.operationMode);
            Console.WriteLine("     +--formatID[{0}]", config.formatID);
            Console.WriteLine("     |--turnOffRF[{0}]", config.turnOffRF);
            Console.WriteLine("     +--BS2SEosCard_ET");
            Console.WriteLine("     |-----oid_primaryKeyENC[{0}]", BitConverter.ToString(config.seos.oid_primaryKeyENC));
            Console.WriteLine("     |-----oid_primaryKeyMAC[{0}]", BitConverter.ToString(config.seos.oid_primaryKeyMAC));
            Console.WriteLine("     |-----oid_primaryKeyAuth[{0}]", BitConverter.ToString(config.seos.oid_primaryKeyAuth));
            Console.WriteLine("     |-----oid_secondaryKeyENC[{0}]", BitConverter.ToString(config.seos.oid_secondaryKeyENC));
            Console.WriteLine("     |-----oid_secondaryKeyMAC[{0}]", BitConverter.ToString(config.seos.oid_secondaryKeyMAC));
            Console.WriteLine("     |-----oid_secondaryKeyAuth[{0}]", BitConverter.ToString(config.seos.oid_secondaryKeyAuth));
            Console.WriteLine("     |-----oid_configCardKeyENC[{0}]", BitConverter.ToString(config.seos.oid_configCardKeyENC));
            Console.WriteLine("     |-----oid_configCardKeyMAC[{0}]", BitConverter.ToString(config.seos.oid_configCardKeyMAC));
            Console.WriteLine("     |-----oid_configCardKeyAuth[{0}]", BitConverter.ToString(config.seos.oid_configCardKeyAuth));
            Console.WriteLine("     |-----oid_ADF[{0}]", BitConverter.ToString(config.seos.oid_ADF));
            Console.WriteLine("     |-----size_ADF[{0}]", config.seos.size_ADF);
            Console.WriteLine("     |-----oid_DataObjectID[{0}]", config.seos.oid_DataObjectID);
            Console.WriteLine("     |-----referenceDataQualifier[{0}]", config.seos.referenceDataQualifier);
            Console.WriteLine("     |-----primaryKeyENC[{0}]", BitConverter.ToString(config.seos.primaryKeyENC));
            Console.WriteLine("     |-----primaryKeyMAC[{0}]", BitConverter.ToString(config.seos.primaryKeyMAC));
            Console.WriteLine("     |-----primaryKeyAuth[{0}]", BitConverter.ToString(config.seos.primaryKeyAuth));
            Console.WriteLine("     |-----secondaryKeyENC[{0}]", BitConverter.ToString(config.seos.secondaryKeyENC));
            Console.WriteLine("     |-----secondaryKeyMAC[{0}]", BitConverter.ToString(config.seos.secondaryKeyMAC));
            Console.WriteLine("     +-----secondaryKeyAuth[{0}]", BitConverter.ToString(config.seos.secondaryKeyAuth));
            Console.WriteLine("<<<< ");
       }
    }
}
