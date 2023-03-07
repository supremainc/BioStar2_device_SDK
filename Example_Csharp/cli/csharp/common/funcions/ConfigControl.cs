using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    using System.Net;
    using BS2_CONFIG_MASK = UInt32;

    public class ConfigControl : FunctionModule
    {
        private API.OnBarcodeScanned cbOnBarcodeScanned = null;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get AllConfig", getAllConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get supported Config Mask", getConfigMask));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get auth group", getAuthGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove auth group", removeAuthGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set auth group", setAuthGroup));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("disable ssl", disbleSSL));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get DataEncryptKey", getDataEncryptKey));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set DataEncryptKey", setDataEncryptKey));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove DataEncryptKey", removeDataEncryptKey));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Card1xConfig", getCard1xConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Card1xConfig", setCard1xConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get SystemExtConfig", getSystemExtConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set SystemExtConfig", setSystemExtConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get VoipConfig", getVoipConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set VoipConfig", setVoipConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get FaceConfig", getFaceConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set FaceConfig", setFaceConfig));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get RS485Config", getRS485Config));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set RS485Config", setRS485Config));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get RS485ConfigEx", getRS485ConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set RS485ConfigEx", setRS485ConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get CardConfigEx", getCardConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set CardConfigEx", setCardConfigEx));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get DstConfig", getDstConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set DstConfig", setDstConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get DesFireCardConfigEx", getDesFireCardConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set DesFireCardConfigEx", setDesFireCardConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get SystemConfig", getSystemConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set SystemConfig", setSystemConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get InputConfig", getInputConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set InputConfig", setInputConfig));

            //[IPv6] 
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get IPConfig", getIPConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get IPV6Config", getIPV6Config));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set IPV6Config", setIPV6Config));
            //<=

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get AuthConfig", getAuthConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set AuthConfig", setAuthConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get AuthConfigEx", getAuthConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set AuthConfigEx", setAuthConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get FaceConfigEx", getFaceConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set FaceConfigEx", setFaceConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get ThermalCameraConfig", getThermalCameraConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set ThermalCameraConfig", setThermalCameraConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get BarcodeConfig", getBarcodeConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set BarcodeConfig", setBarcodeConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get VoipConfigExt", getVoipConfigExt));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set VoipConfigExt", setVoipConfigExt));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get RtspConfig", getRtspConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set RtspConfig", setRtspConfig));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get LicenseConfig", getLicenseConfig));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get OsdpStandardConfig", getOsdpStandardConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get OsdpStandardActionConfig", getOsdpStandardActionConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set OsdpStandardActionConfig", setOsdpStandardActionConfig));

            return functionList;
        }

        //[IPv6]
        void print(IntPtr sdkContext, BS2IpConfig config)
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
                print(sdkContext, config);
            }
        }

        void print(IntPtr sdkContext, BS2IPV6Config config)
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
                Console.WriteLine("     |--allocatedGatewayV6[{0}] : {1}]", idx, Encoding.UTF8.GetString(tempIPV6), BitConverter.ToString(tempIPV6));
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
                print(sdkContext, config);
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
                print(sdkContext, config);
            }

            do
            {
                Console.WriteLine("useDhcpV6 ? [Y/N, (Blank:{0})]", config.useDhcpV6);
                Console.Write(">>>> ");
                bool bInput = config.useDhcpV6 != 0;
                if (bInput)
                    bInput = Util.IsYes();
                else
                    bInput = !Util.IsNo();
                config.useDhcpV6 = (byte)(bInput ? 1 : 0);

                Console.WriteLine("useDnsV6 ? [Y/N, (Blank:{0})]", config.useDnsV6);
                Console.Write(">>>> ");
                bInput = config.useDnsV6 != 0;
                if (bInput)
                    bInput = Util.IsYes();
                else
                    bInput = !Util.IsNo();
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
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / N(clear), (Blank:Y)]");
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
                        Console.WriteLine("   Do you want to keep the value? [Y(keep) / N(clear), (Blank:Y)]");
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
                    Console.WriteLine("   Do you want to keep the value? [Y(keep) / N(clear), (Blank:Y)]");
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

            Console.WriteLine("Trying to get Changed IPV6Config");
            result = (BS2ErrorCode)API.BS2_GetIPV6Config(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }
        }
        //<=

        public void getConfigMask(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2_CONFIG_MASK configMask = 0;

            Console.WriteLine("Trying to get supported config mask");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSupportedConfigMask(sdkContext, deviceID, out configMask);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Supported config Mask: 0x{0:X}", configMask);
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        void getAllConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2Configs configs = Util.AllocateStructure<BS2Configs>();
            configs.configMask = (uint)BS2ConfigMaskEnum.ALL;
            Console.WriteLine("Trying to get AllConfig");

            Type structureType = typeof(BS2Configs);
            int structSize = Marshal.SizeOf(structureType);

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetConfig(sdkContext, deviceID, ref configs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("BS2_GetConfig failed. Error : {0}", result);
            }
            else
            {
                Console.WriteLine("BS2_GetConfig Success  : {0}", configs.factoryConfig.deviceID);
            }
        }

        void getCard1xConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS1CardConfig config;
            Console.WriteLine("Trying to get Card1xConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetCard1xConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }
        }

        #region Card1x Utils
        void entercard1xKey(byte[] dst)
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
        #endregion

        public void setCard1xConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS1CardConfig card1xConfig = Util.AllocateStructure<BS1CardConfig>();

            card1xConfig.magicNo = 0;
            card1xConfig.disabled = 0;
            card1xConfig.useCSNOnly = 0;
            card1xConfig.bioentryCompatible = 0;
            card1xConfig.useSecondaryKey = 1;

            card1xConfig.cisIndex = 0;
            card1xConfig.numOfTemplate = 1;
            card1xConfig.templateSize = 32;
            card1xConfig.templateStartBlock[0] = 4;
            card1xConfig.templateStartBlock[1] = 8;
            card1xConfig.templateStartBlock[2] = 12;
            card1xConfig.templateStartBlock[3] = 0;

            Console.WriteLine("Trying to set card1x configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetCard1xConfig(sdkContext, deviceID, ref card1xConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void getSystemExtConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2SystemConfigExt config;
            Console.WriteLine("Trying to get SystemExtConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSystemExtConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }
        }


        public void setSystemExtConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2SystemConfigExt config = Util.AllocateStructure<BS2SystemConfigExt>();

            config.primarySecureKey[0] = 0x01;
            config.primarySecureKey[1] = 0xFE;

            config.secondarySecureKey[0] = 0x01;
            config.secondarySecureKey[1] = 0x1E;

            Console.WriteLine("Trying to set SystemExt configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetSystemExtConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void getVoipConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2VoipConfig config;
            Console.WriteLine("Trying to get VoipConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetVoipConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }
        }

        public void setVoipConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2VoipConfig config = Util.AllocateStructure<BS2VoipConfig>();

            string url = "192.168.0.1";
            byte[] str = Encoding.UTF8.GetBytes(url);
            Array.Clear(config.serverUrl, 0, BS2Environment.BS2_URL_SIZE);
            Array.Copy(str, 0, config.serverUrl, 0, str.Length);

            config.serverPort = 5061;

            string userId = "홍길동";
            byte[] str2 = Encoding.UTF8.GetBytes(userId);
            Array.Clear(config.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
            Array.Copy(str2, 0, config.userID, 0, str2.Length);

            string pwd = "123456";
            byte[] str3 = Encoding.UTF8.GetBytes(pwd);
            Array.Clear(config.userPW, 0, BS2Environment.BS2_USER_ID_SIZE);
            Array.Copy(str3, 0, config.userPW, 0, str3.Length);

            config.bUse = 1;
            config.dtmfMode = 0;
            config.exitButton = 1;

            config.numPhonBook = 2;
            string phoneNumber = "010-1234-5678";
            byte[] str4 = Encoding.UTF8.GetBytes(phoneNumber);
            Array.Clear(config.phonebook[0].phoneNumber, 0, BS2Environment.BS2_USER_ID_SIZE);
            Array.Copy(str4, 0, config.phonebook[0].phoneNumber, 0, str4.Length);

            string descript = "홍길동 나아가신다.";
            byte[] str5 = Encoding.UTF8.GetBytes(descript);
            Array.Clear(config.phonebook[0].descript, 0, BS2Environment.BS2_MAX_DESCRIPTION_NAME_LEN);
            Array.Copy(str5, 0, config.phonebook[0].descript, 0, str5.Length);

            string phoneNumber2 = "010-9874-1234";
            byte[] str6 = Encoding.UTF8.GetBytes(phoneNumber2);
            Array.Clear(config.phonebook[1].phoneNumber, 0, BS2Environment.BS2_USER_ID_SIZE);
            Array.Copy(str6, 0, config.phonebook[1].phoneNumber, 0, str6.Length);

            string descript2 = "사임당 나아가신다.";
            byte[] str7 = Encoding.UTF8.GetBytes(descript2);
            Array.Clear(config.phonebook[1].descript, 0, BS2Environment.BS2_MAX_DESCRIPTION_NAME_LEN);
            Array.Copy(str7, 0, config.phonebook[1].descript, 0, str7.Length);


            Console.WriteLine("Trying to set Voip configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetVoipConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void getFaceConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2FaceConfig config;
            Console.WriteLine("Trying to get FaceConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetFaceConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }
        }

        public void setFaceConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2FaceConfig config = Util.AllocateStructure<BS2FaceConfig>();

            //const UInt32 faceExMask = (UInt32)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX;

            Console.WriteLine("Insert securityLevel. (0: Basic, 1: Highly secure, 2: Most highly secure)");
            Console.Write(">> ");
            config.securityLevel = Util.GetInput((byte)0);

            Console.WriteLine("Insert lightCondition. (0: Normal, 1: High, 3: Not used)");
            Console.Write(">> ");
            config.lightCondition = Util.GetInput((byte)0);

            Console.WriteLine("Insert enrollThreshold. (0: Most strict - 9: Least strict, 4: Default)");
            Console.Write(">> ");
            config.enrollThreshold = Util.GetInput((byte)4);

            Console.WriteLine("Insert detectSensitivity. (0: Off, 1: Low, 2: Basic, 3: High)");
            Console.Write(">> ");
            config.detectSensitivity = Util.GetInput((byte)2);

            ushort defaultEnrollTimeout = 20;
            byte defaultLFD = 0;
            bool needInput = true;
            switch ((BS2DeviceTypeEnum)deviceInfo.type)
            {
            case BS2DeviceTypeEnum.FACESTATION_2:
            case BS2DeviceTypeEnum.FACELITE:
                defaultEnrollTimeout = 60;
                defaultLFD = 0;
                needInput = true;
                break;
            case BS2DeviceTypeEnum.FACESTATION_F2_FP:
            case BS2DeviceTypeEnum.FACESTATION_F2:
            case BS2DeviceTypeEnum.BIOSTATION_3:
                defaultEnrollTimeout = 20;
                defaultLFD = 1;
                needInput = true;
                break;
            default:
                needInput = false;
                break;
            }

            if (needInput)
            {
                Console.WriteLine("Insert enrollTimeout. (default: {0}s)", defaultEnrollTimeout);
                Console.Write(">> ");
                config.enrollTimeout = Util.GetInput(defaultEnrollTimeout);

                Console.WriteLine("Insert lfdLevel. (0: Not use, 1: Strict, 2: More Strict, 3: Most Strict... (default: {0}))", defaultLFD);
                Console.Write(">> ");
                config.lfdLevel = Util.GetInput(defaultLFD);
            }
            else
            {
                config.enrollTimeout = 0;
                config.lfdLevel = 0;
            }

            Console.WriteLine("Insert quickEnrollment. (0: 3-step enrollment(High quality), 1: 1-step enrollment(Quick))");
            Console.Write(">> ");
            config.quickEnrollment = Util.GetInput((byte)1);

            Console.WriteLine("Insert previewOption. (0: Not used, 1: 1/2 stage, 2: All stages)");
            Console.Write(">> ");
            config.previewOption = Util.GetInput((byte)1);

            Console.WriteLine("Insert checkDuplicate. (0 or 1)");
            Console.Write(">> ");
            config.checkDuplicate = Util.GetInput((byte)0);

            Console.WriteLine("Insert operationMode. (0: Fusion, 1: Visual, 2: Visual (+IR detect))");
            Console.Write(">> ");
            config.operationMode = Util.GetInput((byte)0);

            Console.WriteLine("Insert maxRotation. (default: 15)");
            Console.Write(">> ");
            config.maxRotation = Util.GetInput((byte)15);

            if ((BS2DeviceTypeEnum)deviceInfo.type == BS2DeviceTypeEnum.FACESTATION_F2_FP ||
                (BS2DeviceTypeEnum)deviceInfo.type == BS2DeviceTypeEnum.FACESTATION_F2)
            {
                Console.WriteLine("Insert min value of faceWidth. (default: {0})", BS2Environment.BS2_FACE_WIDTH_MIN_DEFAULT);
                Console.Write(">> ");
                config.faceWidth.min = Util.GetInput(BS2Environment.BS2_FACE_WIDTH_MIN_DEFAULT);

                Console.WriteLine("Insert max value of faceWidth. (default: {0})", BS2Environment.BS2_FACE_WIDTH_MAX_DEFAULT);
                Console.Write(">> ");
                config.faceWidth.max = Util.GetInput(BS2Environment.BS2_FACE_WIDTH_MAX_DEFAULT);

                Console.WriteLine("Insert x value of searchRange. (default: {0})", BS2Environment.BS2_FACE_SEARCH_RANGE_X_DEFAULT);
                Console.Write(">> ");
                config.searchRange.x = Util.GetInput(BS2Environment.BS2_FACE_SEARCH_RANGE_X_DEFAULT);

                Console.WriteLine("Insert width value of searchRange. (default: {0})", BS2Environment.BS2_FACE_SEARCH_RANGE_WIDTH_DEFAULT);
                Console.Write(">> ");
                config.searchRange.width = Util.GetInput(BS2Environment.BS2_FACE_SEARCH_RANGE_WIDTH_DEFAULT);
            }
            else if ((BS2DeviceTypeEnum)deviceInfo.type == BS2DeviceTypeEnum.BIOSTATION_3)
            {
                Console.WriteLine("Insert min value of detectDistance. ({0}~{1}, default: {2})",
                    BS2Environment.BS2_FACE_DETECT_DISTANCE_MIN_MIN,
                    BS2Environment.BS2_FACE_DETECT_DISTANCE_MIN_MAX,
                    BS2Environment.BS2_FACE_DETECT_DISTANCE_MIN_DEFAULT);
                Console.Write(">> ");
                config.detectDistance.min = Util.GetInput(BS2Environment.BS2_FACE_DETECT_DISTANCE_MIN_DEFAULT);

                Console.WriteLine("Insert max value of detectDistance. ({0}~{1}, default: {2}, infinite: {3})",
                    BS2Environment.BS2_FACE_DETECT_DISTANCE_MAX_MIN,
                    BS2Environment.BS2_FACE_DETECT_DISTANCE_MAX_MAX,
                    BS2Environment.BS2_FACE_DETECT_DISTANCE_MAX_DEFAULT,
                    BS2Environment.BS2_FACE_DETECT_DISTANCE_MAX_INF);
                Console.Write(">> ");
                config.detectDistance.max = Util.GetInput(BS2Environment.BS2_FACE_DETECT_DISTANCE_MAX_DEFAULT);

                Console.WriteLine("Do you want to turn on the wideSearch? [y/N]");
                Console.Write(">> ");
                config.wideSearch = Convert.ToByte(!Util.IsNo());
            }

            Console.WriteLine("Trying to set FaceConfig configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetFaceConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        #region Auth Group
        public void getAuthGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            IntPtr authGroupObj = IntPtr.Zero;
            UInt32 numAuthGroup = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all auth groups? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all auth groups from device.");
                result = (BS2ErrorCode)API.BS2_GetAllAuthGroup(sdkContext, deviceID, out authGroupObj, out numAuthGroup);
            }
            else
            {
                Console.WriteLine("Enter the ID of the access group which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] authGroupIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> authGroupIDList = new List<UInt32>();

                foreach (string authGroupID in authGroupIDs)
                {
                    if (authGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(authGroupID, out item))
                        {
                            authGroupIDList.Add(item);
                        }
                    }
                }

                if (authGroupIDList.Count > 0)
                {
                    IntPtr authGroupIDObj = Marshal.AllocHGlobal(4 * authGroupIDList.Count);
                    IntPtr curAuthGroupIDObj = authGroupIDObj;
                    foreach (UInt32 item in authGroupIDList)
                    {
                        Marshal.WriteInt32(curAuthGroupIDObj, (Int32)item);
                        curAuthGroupIDObj = (IntPtr)((long)curAuthGroupIDObj + 4);
                    }

                    Console.WriteLine("Trying to get auth groups from device.");
                    result = (BS2ErrorCode)API.BS2_GetAuthGroup(sdkContext, deviceID, authGroupIDObj, (UInt32)authGroupIDList.Count, out authGroupObj, out numAuthGroup);

                    Marshal.FreeHGlobal(authGroupIDObj);
                }
                else
                {
                    Console.WriteLine("Invalid parameter");
                }
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numAuthGroup > 0)
            {
                IntPtr curAuthGroupObj = authGroupObj;
                int structSize = Marshal.SizeOf(typeof(BS2AuthGroup));

                for (int idx = 0; idx < numAuthGroup; ++idx)
                {
                    BS2AuthGroup item = (BS2AuthGroup)Marshal.PtrToStructure(curAuthGroupObj, typeof(BS2AuthGroup));
                    print(sdkContext, item);
                    curAuthGroupObj = (IntPtr)((long)curAuthGroupObj + structSize);
                }

                API.BS2_ReleaseObject(authGroupObj);
            }
            else
            {
                Console.WriteLine(">>> There is no auth group in the device.");
            }
        }

        public void removeAuthGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all auth groups? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all auth groups from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllAuthGroup(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the auth group which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] authGroupIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> authGroupIDList = new List<UInt32>();

                foreach (string authGroupID in authGroupIDs)
                {
                    if (authGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(authGroupID, out item))
                        {
                            authGroupIDList.Add(item);
                        }
                    }
                }

                if (authGroupIDList.Count > 0)
                {
                    IntPtr authGroupIDObj = Marshal.AllocHGlobal(4 * authGroupIDList.Count);
                    IntPtr curAuthGroupIDObj = authGroupIDObj;
                    foreach (UInt32 item in authGroupIDList)
                    {
                        Marshal.WriteInt32(curAuthGroupIDObj, (Int32)item);
                        curAuthGroupIDObj = (IntPtr)((long)curAuthGroupIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove auth groups from device.");
                    result = (BS2ErrorCode)API.BS2_RemoveAuthGroup(sdkContext, deviceID, authGroupIDObj, (UInt32)authGroupIDList.Count);

                    Marshal.FreeHGlobal(authGroupIDObj);
                }
                else
                {
                    Console.WriteLine("Invalid parameter");
                }
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void setAuthGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many auth groups do you want to set? [1(default)-128]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2AuthGroup> authGroupList = new List<BS2AuthGroup>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2AuthGroup authGroup = Util.AllocateStructure<BS2AuthGroup>();

                Console.WriteLine("Enter a value for auth group[{0}]", idx);
                Console.WriteLine("  Enter the ID for the auth group which you want to set");
                Console.Write("  >>>> ");
                authGroup.id = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the auth group which you want to set");
                Console.Write("  >>>> ");
                string authGroupName = Console.ReadLine();
                if (authGroupName.Length == 0)
                {
                    Console.WriteLine("  [Warning] auth group name will be displayed as empty.");
                }
                else if (authGroupName.Length > BS2Environment.BS2_MAX_AUTH_GROUP_NAME_LEN)
                {
                    Console.WriteLine("  Name of auth group should less than {0} words.", BS2Environment.BS2_MAX_AUTH_GROUP_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] authGroupArray = Encoding.UTF8.GetBytes(authGroupName);
                    Array.Clear(authGroup.name, 0, BS2Environment.BS2_MAX_AUTH_GROUP_NAME_LEN);
                    Array.Copy(authGroupArray, authGroup.name, authGroupArray.Length);
                }

                authGroupList.Add(authGroup);
            }

            int structSize = Marshal.SizeOf(typeof(BS2AuthGroup));
            IntPtr authGroupListObj = Marshal.AllocHGlobal(structSize * authGroupList.Count);
            IntPtr curAuthGroupListObj = authGroupListObj;
            foreach (BS2AuthGroup item in authGroupList)
            {
                Marshal.StructureToPtr(item, curAuthGroupListObj, false);
                curAuthGroupListObj = (IntPtr)((long)curAuthGroupListObj + structSize);
            }

            Console.WriteLine("Trying to set auth groups to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAuthGroup(sdkContext, deviceID, authGroupListObj, (UInt32)authGroupList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(authGroupListObj);
        }
        #endregion

        public void disbleSSL(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to disable ssl");

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_DisableSSL(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void getRS485Config(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2Rs485Config config;
            Console.WriteLine("Trying to get RS485Config");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetRS485Config(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }
        }

        public void setRS485Config(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2Rs485Config config = Util.AllocateStructure<BS2Rs485Config>();
            
            BS2DeviceTypeEnum type = (BS2DeviceTypeEnum)deviceInfo.type;
            byte numOfChannels = (byte)(BS2DeviceTypeEnum.CORESTATION_40 == type ? BS2Environment.BS2_RS485_MAX_CHANNELS : 1);

            Console.WriteLine("Please set the mode. ({0}(0), {1}(1), {2}(2), {3}(3))",
                BS2RS485ModeEnum.DISABLED,
	            BS2RS485ModeEnum.MASTER,
                BS2RS485ModeEnum.SLAVE,
                BS2RS485ModeEnum.STANDALONE);
            Console.Write(">> ");
            config.mode = Util.GetInput((byte)BS2RS485ModeEnum.STANDALONE);

            Console.WriteLine("How many RS485 channels do you want to set up? (0 ~ {0})", numOfChannels);
            Console.Write(">> ");
            config.numOfChannels = Util.GetInput(numOfChannels);

            for (int idx = 0; idx < config.numOfChannels; idx++)
            {
                Console.WriteLine("Please insert baud rate. (Default: 115200)");
                Console.Write(">> ");
                config.channels[idx].baudRate = Util.GetInput((UInt32)115200);

                Console.WriteLine("Please insert channel index.");
                Console.Write(">> ");
                config.channels[idx].channelIndex = Util.GetInput((byte)0);

                Console.WriteLine("Please insert useRegistance.");
                Console.Write(">> ");
                config.channels[idx].useRegistance = Util.GetInput((byte)0);

                Console.WriteLine("Please insert number of devices.");
                Console.Write(">> ");
                config.channels[idx].numOfDevices = Util.GetInput((byte)1);

                for (int slaveIdx = 0; slaveIdx < config.channels[idx].numOfDevices; slaveIdx++)
                {
                    Console.WriteLine("Please insert #{0} deviceID.", slaveIdx);
                    Console.Write(">> ");
                    config.channels[idx].slaveDevices[slaveIdx].deviceID = Util.GetInput((UInt32)0);

                    Console.WriteLine("Please insert #{0} deviceType.", slaveIdx);
                    Console.Write(">> ");
                    config.channels[idx].slaveDevices[slaveIdx].deviceType = Util.GetInput((UInt16)BS2DeviceTypeEnum.BIOENTRY_W2);

                    Console.WriteLine("Please insert #{0} enableOSDP.", slaveIdx);
                    Console.Write(">> ");
                    config.channels[idx].slaveDevices[slaveIdx].enableOSDP = Util.GetInput((byte)0);

                    Console.WriteLine("Please insert #{0} connected.", slaveIdx);
                    Console.Write(">> ");
                    config.channels[idx].slaveDevices[slaveIdx].connected = Util.GetInput((byte)0);;
                }
            }

            Console.WriteLine("Would you like to use IntelligentPD-related settings? [y/N]");
            Console.Write(">> ");
            bool supportConfig = Util.IsNo() != true;
            config.intelligentInfo.supportConfig = Convert.ToByte(supportConfig);

            if (supportConfig)
            {
                Console.WriteLine("Would you like to use an exception code? [Y/n]");
                Console.Write(">> ");
                bool useExceptionCode = Util.IsYes();
                config.intelligentInfo.useExceptionCode = Convert.ToByte(useExceptionCode);

                if (useExceptionCode)
                {
                    Console.WriteLine("Please enter the exception code in 8 bytes hexa.");
                    Console.Write(">> 0x");
                    string exceptionCode = Console.ReadLine();

                    byte[] byteCode = Util.ConvertString2HexByte(exceptionCode);

                    Array.Clear(config.intelligentInfo.exceptionCode, 0, BS2Environment.BS2_RS485_MAX_EXCEPTION_CODE_LEN);
                    Array.Copy(byteCode, config.intelligentInfo.exceptionCode, BS2Environment.BS2_RS485_MAX_EXCEPTION_CODE_LEN);
                }

                Console.WriteLine("Please enter the output format. CardID({0}), UserID({1})",
                    BS2Environment.BS2_IPD_OUTPUT_CARDID, BS2Environment.BS2_IPD_OUTPUT_USERID);
                Console.Write(">> ");
                config.intelligentInfo.outputFormat = Util.GetInput((byte)BS2Environment.BS2_IPD_OUTPUT_CARDID);

                Console.WriteLine("Please enter the OSDP ID.");
                Console.Write(">> ");
                config.intelligentInfo.osdpID = Util.GetInput((byte)0);
            }

            Console.WriteLine("Trying to set RS485Config configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetRS485Config(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void getRS485ConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2Rs485ConfigEX config;
            Console.WriteLine("Trying to get RS485ConfigEx");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetRS485ConfigEx(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }

        }

        public void setRS485ConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {

            BS2Rs485ConfigEX config = Util.AllocateStructure<BS2Rs485ConfigEX>();
            Console.WriteLine("Trying to get RS485ConfigEx");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetRS485ConfigEx(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }

            Console.WriteLine("Do you want to change settings of device? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                for (int i = 0; i < 5; i++)
                {
                    config.mode[i] = 1;
                }

                config.numOfChannels = 5;
                config.channels[0].baudRate = 115200;
                config.channels[0].channelIndex = 0;
                config.channels[0].useRegistance = 0;
                config.channels[0].numOfDevices = 1;
                config.channels[0].slaveDevices[0].deviceID = 541531029;
                config.channels[0].slaveDevices[0].deviceType = 9;
                config.channels[0].slaveDevices[0].enableOSDP = 0;
                config.channels[0].slaveDevices[0].connected = 1;
                config.channels[0].slaveDevices[0].channelInfo = 0;
            }

            Console.WriteLine("Trying to set RS485ConfigEx configuration.");
            result = (BS2ErrorCode)API.BS2_SetRS485ConfigEx(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

        }

        void getCardConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2CardConfigEx config;
            Console.WriteLine("Trying to get CardConfigEx");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetCardConfigEx(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }
        }

        public void setCardConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2CardConfigEx config = Util.AllocateStructure<BS2CardConfigEx>();

            config.seos.oid_ADF[0] = 0x01;
            config.seos.oid_ADF[1] = 0x02;

            config.seos.oid_DataObjectID[0] = 0xD0;
            config.seos.oid_DataObjectID[1] = 0xD1;

            config.seos.size_DataObject[0] = 90;
            config.seos.size_DataObject[1] = 100;

            config.seos.primaryKeyAuth[0] = 0x01;
            config.seos.primaryKeyAuth[1] = 0xFE;

            config.seos.secondaryKeyAuth[0] = 0x01;
            config.seos.secondaryKeyAuth[1] = 0x1E;

            Console.WriteLine("Trying to set CardConfigEx configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetCardConfigEx(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

        }

        public void getDstConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2DstConfig config;

            Console.WriteLine("Trying to get daylight saving time configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDstConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(sdkContext, config);
        }

        public void setDstConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many daylight saving time schedules do you want to set? [1(default)-2]");
            Console.Write(">>>> ");
            BS2DstConfig config = Util.AllocateStructure<BS2DstConfig>();
            config.numSchedules = (byte)Util.GetInput(1);
            if (config.numSchedules < 0 || config.numSchedules > 2)
            {
                Console.WriteLine("Invalid parameter");
                return;
            }

            for (int idx = 0; idx < config.numSchedules; ++idx)
            {
                Console.WriteLine("Configure DST schedule #{0}", idx);
                Console.WriteLine("   Enter the OFFSET of the time in seconds. [Ex) 3600 means it will add 1 hour after the DST starts.]");
                Console.Write(">>>");
                config.schedules[idx].timeOffset = (int)Util.GetInput();

                Console.WriteLine("   Please enter the value for the STARTING TIME.");
                Console.WriteLine("      Enter the YEAR to start the DST schedule #{0}. [0(default) means every year]", idx);
                Console.Write("   >>>");
                config.schedules[idx].startTime.year = Util.GetInput((UInt16)0);
                Console.WriteLine("      Enter the MONTH to start the DST schedule #{0}. [0(Jan), 1(Feb), ... , 11(Dec)]", idx);
                Console.Write("   >>>");
                config.schedules[idx].startTime.month = (byte)Util.GetInput();
                Console.WriteLine("      Enter the ORDINAL of the WEEK to start the DST schedule #{0}. [0(1st week), 1(2nd week), ... , -1(Last week)]", idx);
                Console.WriteLine("      The start of the week is based on Monday.");
                Console.Write("   >>>");
                config.schedules[idx].startTime.ordinal = (sbyte)Util.GetInput();
                Console.WriteLine("      Enter the DAY of the WEEK to start the DST schedule #{0}. [0(Sun), 1(Mon), ... , 6(Sat)]", idx);
                Console.Write("   >>>");
                config.schedules[idx].startTime.weekDay = (byte)Util.GetInput();
                Console.WriteLine("      Enter the HOUR to start the DST schedule #{0}. [0 ~ 23]", idx);
                Console.Write("   >>>");
                config.schedules[idx].startTime.hour = (byte)Util.GetInput();
                Console.WriteLine("      Enter the MINUTE to start the DST schedule #{0}. [0 ~ 59]", idx);
                Console.Write("   >>>");
                config.schedules[idx].startTime.minute = (byte)Util.GetInput();
                Console.WriteLine("      Enter the SECOND to start the DST schedule #{0}. [0 ~ 59]", idx);
                Console.Write("   >>>");
                config.schedules[idx].startTime.second = (byte)Util.GetInput();

                Console.WriteLine("   Please enter the value for the ENDING TIME.");
                Console.WriteLine("      Enter the YEAR to end the DST schedule #{0}. [0(default) means every year]", idx);
                Console.Write("   >>>");
                config.schedules[idx].endTime.year = Util.GetInput((UInt16)0);
                Console.WriteLine("      Enter the MONTH to end the DST schedule #{0}. [0(Jan), 1(Feb), ... , 11(Dec)]", idx);
                Console.Write("   >>>");
                config.schedules[idx].endTime.month = (byte)Util.GetInput();
                Console.WriteLine("      Enter the ORDINAL of the WEEK to end the DST schedule #{0}. [0(1st week), 1(2nd week), ... , -1(Last week)]", idx);
                Console.WriteLine("      The start of the week is based on Monday.");
                Console.Write("   >>>");
                config.schedules[idx].endTime.ordinal = (sbyte)Util.GetInput();
                Console.WriteLine("      Enter the DAY of the WEEK to end the DST schedule #{0}. [0(Sun), 1(Mon), ... , 6(Sat)]", idx);
                Console.Write("   >>>");
                config.schedules[idx].endTime.weekDay = (byte)Util.GetInput();
                Console.WriteLine("      Enter the HOUR to end the DST schedule #{0}. [0 ~ 23]", idx);
                Console.Write("   >>>");
                config.schedules[idx].endTime.hour = (byte)Util.GetInput();
                Console.WriteLine("      Enter the MINUTE to end the DST schedule #{0}. [0 ~ 59]", idx);
                Console.Write("   >>>");
                config.schedules[idx].endTime.minute = (byte)Util.GetInput();
                Console.WriteLine("      Enter the SECOND to end the DST schedule #{0}. [0 ~ 59]", idx);
                Console.Write("   >>>");
                config.schedules[idx].endTime.second = (byte)Util.GetInput();
            }

            Console.WriteLine("Trying to set Daylight Saving Time configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDstConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
            else
            {
                Console.WriteLine("Set DstConfig Succeeded");
            }
        }

        public void getDesFireCardConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2DesFireCardConfigEx config;

            Console.WriteLine("Trying to get DesFire card configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDesFireCardConfigEx(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(config);
        }

        public void setDesFireCardConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2DesFireCardConfigEx config = Util.AllocateStructure<BS2DesFireCardConfigEx>();

            Console.WriteLine("Enter the hexadecimal application master key for DesFireCardConfigEx. [KEY1-KEY2-...-KEY16]");
            Console.Write(">>>> ");
            enterSmartcardKey(config.desfireAppKey.appMasterKey);

            Console.WriteLine("Enter the hexadecimal file read key. [KEY1-KEY2-...-KEY16]");
            Console.Write(">>>> ");
            enterSmartcardKey(config.desfireAppKey.fileReadKey);

            Console.WriteLine("Enter the file read key index.");
            Console.Write(">>>> ");
            config.desfireAppKey.fileReadKeyNumber = (byte)Util.GetInput();

            Console.WriteLine("Enter the hexadecimal file write key. [KEY1-KEY2-...-KEY16]");
            Console.Write(">>>> ");
            enterSmartcardKey(config.desfireAppKey.fileWriteKey);

            Console.WriteLine("Enter the file write key index.");
            Console.Write(">>>> ");
            config.desfireAppKey.fileWriteKeyNumber = (byte)Util.GetInput();

            Console.WriteLine("Trying to set DesFire card configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDesFireCardConfigEx(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
            else
            {
                Console.WriteLine("Set DesFire card configuration succeeded");
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

        void getSystemConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2SystemConfig config;
            Console.WriteLine("Trying to get SystemConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSystemConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(config);
            }
        }

        public void setSystemConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2SystemConfig config = Util.AllocateStructure<BS2SystemConfig>();

            Console.WriteLine("Enter a timezone in seconds. [Ex. Seoul: 32400 (= 9 * 60 * 60)]");
            Console.Write(">>>> ");
            config.timezone = Util.GetInput();

            Console.WriteLine("Do you want to synchronize time with server? [y/n]");
            Console.Write(">>>> ");
            config.syncTime = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Do you want to use interphone? [y/n]");
            Console.Write(">>>> ");
            config.useInterphone = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Do you want to use OSDP secure key? [y/n]");
            Console.Write(">>>> ");
            config.keyEncrypted = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Do you want to use job codes? [y/n]");
            Console.Write(">>>> ");
            config.useJobCode = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Do you want to use alphanumeric ID? [y/n]");
            Console.Write(">>>> ");
            config.useAlphanumericID = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Enter frequency of camera [1: 50Hz, 2: 60Hz]");
            Console.Write(">>>> ");
            config.cameraFrequency = (UInt32)Util.GetInput();

            Console.WriteLine("Do you want to use security tamper? [y/n]");
            Console.Write(">>>> ");
            config.secureTamper = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Do you want to change the type of card the device reads? [y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Enter the card combination you wish to set.");
                Console.WriteLine("    0xFFFFFFFF : DEFAULT");
                Console.WriteLine("    0x00000000 : NONE");
                Console.WriteLine("    0x00000001 : (LowFrequency)  EM");
                Console.WriteLine("    0x00000002 : (LowFrequency)  PROX");
                Console.WriteLine("    0x00000004 : (HighFrequency) CSN_MIFARE");
                Console.WriteLine("    0x00000008 : (HighFrequency) CSN_ICLASS");
                Console.WriteLine("    0x00000010 : (HighFrequency) SMART_MIFARE");
                Console.WriteLine("    0x00000020 : (HighFrequency) SMART_MIFARE_DESFIRE");
                Console.WriteLine("    0x00000040 : (HighFrequency) SMART_ICLASS");
                Console.WriteLine("    0x00000080 : (HighFrequency) SMART_ICLASS_SEOS");
                Console.WriteLine("    0x00000100 : (Mobile)        NFC");
                Console.WriteLine("    0x00000200 : (Mobile)        BLE");
                Console.WriteLine("    0x00000400 : (HighFrequency) CSN_OTHERS");
                Console.Write(">>>> ");

                UInt32 defaultMask = 0xFFFFFFFF;
                config.useCardOperationMask = (UInt32)Util.GetInput(defaultMask);
                config.useCardOperationMask |= (UInt32)BS2SystemConfigCardOperationMask.CARD_OPERATION_USE; // Card operation apply
            }
            else
            {
                config.useCardOperationMask = (UInt32)BS2SystemConfigCardOperationMask.CARD_OPERATION_MASK_DEFAULT;
            }

            Console.WriteLine("Trying to set System configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetSystemConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void getInputConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2InputConfig config;

            Console.WriteLine("Trying to get input configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetInputConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(sdkContext, config);
        }

        public void setInputConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2InputConfig config = Util.AllocateStructure<BS2InputConfig>();

            Console.WriteLine("Trying to get input configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetInputConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(sdkContext, config);

            Console.WriteLine("Do you want to change settings of device? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Please enter Number of inputs.");
                Console.Write(">>>> ");
                config.numInputs = (byte)Util.GetInput();
                if (config.numInputs < 0 || config.numInputs > 10)
                {
                    Console.WriteLine("Invalid parameter (numInputs)");
                    return;
                }

                Console.WriteLine("Please enter Number of supervised-inputs.");
                Console.Write(">>>> ");
                config.numSupervised = (byte)Util.GetInput();
                if (config.numSupervised < 0 || config.numSupervised > 8)
                {
                    Console.WriteLine("Invalid parameter (numSupervised)");
                    return;
                }

                for (int idx = 0; idx < BS2Environment.BS2_MAX_INPUT_NUM; ++idx)
                {
                    if (idx < config.numSupervised)
                    {
                        Console.WriteLine(">>>> supervised_inputs[{0}]", idx);

                        config.supervised_inputs[idx].portIndex = (byte)idx;

                        Console.Write("    Please enter enabled (0, 1) : ");
                        config.supervised_inputs[idx].enabled = (byte)Util.GetInput();

                        Console.Write("    Please enter superviced_index : ");
                        config.supervised_inputs[idx].supervised_index = (byte)Util.GetInput();

                        if (255 == config.supervised_inputs[idx].supervised_index)
                        {
                            Console.Write("    Please enter shortInput.minValue : ");
                            config.supervised_inputs[idx].config.shortInput.minValue = (UInt16)Util.GetInput();
                            Console.Write("    Please enter shortInput.maxValue : ");
                            config.supervised_inputs[idx].config.shortInput.maxValue = (UInt16)Util.GetInput();

                            Console.Write("    Please enter openInput.minValue : ");
                            config.supervised_inputs[idx].config.openInput.minValue = (UInt16)Util.GetInput();
                            Console.Write("    Please enter openInput.maxValue : ");
                            config.supervised_inputs[idx].config.openInput.maxValue = (UInt16)Util.GetInput();

                            Console.Write("    Please enter onInput.minValue : ");
                            config.supervised_inputs[idx].config.onInput.minValue = (UInt16)Util.GetInput();
                            Console.Write("    Please enter onInput.maxValue : ");
                            config.supervised_inputs[idx].config.onInput.maxValue = (UInt16)Util.GetInput();

                            Console.Write("    Please enter offInput.minValue : ");
                            config.supervised_inputs[idx].config.offInput.minValue = (UInt16)Util.GetInput();
                            Console.Write("    Please enter offInput.maxValue : ");
                            config.supervised_inputs[idx].config.offInput.maxValue = (UInt16)Util.GetInput();
                        }
                        else
                        {
                            config.supervised_inputs[idx].config.shortInput.minValue = 0;
                            config.supervised_inputs[idx].config.shortInput.maxValue = 0;
                            config.supervised_inputs[idx].config.openInput.minValue = 0;
                            config.supervised_inputs[idx].config.openInput.maxValue = 0;
                            config.supervised_inputs[idx].config.onInput.minValue = 0;
                            config.supervised_inputs[idx].config.onInput.maxValue = 0;
                            config.supervised_inputs[idx].config.offInput.minValue = 0;
                            config.supervised_inputs[idx].config.offInput.maxValue = 0;
                        }
                    }
                    else
                    {
                        config.supervised_inputs[idx].portIndex = (byte)idx;
                        config.supervised_inputs[idx].enabled = 0;
                        config.supervised_inputs[idx].supervised_index = 1;
                        config.supervised_inputs[idx].config.shortInput.minValue = 0;
                        config.supervised_inputs[idx].config.shortInput.maxValue = 0;
                        config.supervised_inputs[idx].config.openInput.minValue = 0;
                        config.supervised_inputs[idx].config.openInput.maxValue = 0;
                        config.supervised_inputs[idx].config.onInput.minValue = 0;
                        config.supervised_inputs[idx].config.onInput.maxValue = 0;
                        config.supervised_inputs[idx].config.offInput.minValue = 0;
                        config.supervised_inputs[idx].config.offInput.maxValue = 0;
                    }
                }
            }

            Console.WriteLine("Trying to set input configuration.");
            result = (BS2ErrorCode)API.BS2_SetInputConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
            else
            {
                Console.WriteLine("Set InputConfig Succeeded");
            }
        }

        public void getDataEncryptKey(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to get data encrypt key.");

            BS2EncryptKey keyInfo;
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDataEncryptKey(sdkContext, deviceID, out keyInfo);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(keyInfo);
        }

        public void setDataEncryptKey(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to set data encrypt key.");

            Console.WriteLine("If the key change is successful, all users of the device will be deleted.");
            Console.WriteLine("Do you want to continue? [y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                BS2EncryptKey keyInfo = Util.AllocateStructure<BS2EncryptKey>();

                Console.WriteLine("Write please key string.");
                Console.Write(">>>> ");
                string keyString = Console.ReadLine();
                byte[] buff = Encoding.UTF8.GetBytes(keyString);

                Array.Clear(keyInfo.key, 0, BS2Environment.BS2_ENC_KEY_SIZE);
                Array.Copy(buff, 0, keyInfo.key, 0, keyString.Length);

                BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDataEncryptKey(sdkContext, deviceID, ref keyInfo);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }
            }
        }

        public void removeDataEncryptKey(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to remove data encrypt key.");

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_RemoveDataEncryptKey(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public void getAuthConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2AuthConfig authConfig;
            Console.WriteLine("Trying to get authentication configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, deviceID, out authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(authConfig);
        }

        public void setAuthConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2AuthConfig authConfig = Util.AllocateStructure<BS2AuthConfig>();
            bool stop = false;

            do
            {
                Console.WriteLine("Select auth mode.");
                Console.WriteLine("  0. Biometric Only");
                Console.WriteLine("  1. Biometric + PIN");
                Console.WriteLine("  2. Card Only");
                Console.WriteLine("  3. Card + Biometric");
                Console.WriteLine("  4. Card + PIN");
                Console.WriteLine("  5. Card + Biometric/PIN");
                Console.WriteLine("  6. Card + Biometric + PIN");
                Console.WriteLine("  7. ID + Biometric");
                Console.WriteLine("  8. ID + PIN");
                Console.WriteLine("  9. ID + Biometric/PIN");
                Console.WriteLine(" 10. ID + Biometric + PIN");
                Console.WriteLine("999. No more changes.");
                Console.Write(">> ");
                UInt32 mode = (UInt32)Util.GetInput();
                if ((UInt32)BS2IDAuthModeEnum.ID_BIOMETRIC_PIN < mode && 999 != mode)
                {
                    Console.WriteLine("Invalid auth mode");
                    return;
                }

                if (999 == mode)
                {
                    stop = true;
                }
                else
                {
                    Console.WriteLine("0. Off (No time)");
                    Console.WriteLine("1. On (Always)");
                    Console.Write(">> ");
                    UInt32 onoff = (UInt32)Util.GetInput();
                    if (0 == onoff || 1 == onoff)
                    {
                        authConfig.authSchedule[mode] = onoff;
                    }
                }
            } while (!stop);

            Console.WriteLine("Insert global APB option. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfig.useGlobalAPB = Util.GetInput((byte)0);

            Console.WriteLine("Insert global APB fail action. (0: Not use, 1: Soft APB, 2: Hard APB)");
            Console.Write(">> ");
            authConfig.globalAPBFailAction = Util.GetInput((byte)0);

            Console.WriteLine("Insert private authentication. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfig.usePrivateAuth = Util.GetInput((byte)0);

            Console.WriteLine("Insert face detection level. (0: Not use, 1: Normal mode, 2: Strict mode)");
            Console.Write(">> ");
            authConfig.faceDetectionLevel = Util.GetInput((byte)0);

            Console.WriteLine("Insert server matching option. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfig.useServerMatching = Util.GetInput((byte)0);

            Console.WriteLine("Using full access. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfig.useFullAccess = Util.GetInput((byte)0);

            Console.WriteLine("Insert matching timeout in seconds");
            Console.Write(">> ");
            authConfig.matchTimeout = Util.GetInput((byte)5);

            Console.WriteLine("Insert authentication timeout in seconds");
            Console.Write(">> ");
            authConfig.authTimeout = Util.GetInput((byte)10);

            authConfig.numOperators = 0;

            Console.WriteLine("Trying to set authentication configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAuthConfig(sdkContext, deviceID, ref authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void getAuthConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2AuthConfigExt authConfigExt;
            Console.WriteLine("Trying to get authentication ext configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthConfigExt(sdkContext, deviceID, out authConfigExt);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(authConfigExt);
        }

        public void setAuthConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2AuthConfigExt authConfigExt = Util.AllocateStructure<BS2AuthConfigExt>();
            UInt32 mode;

            Console.WriteLine("Register FaceEx authentication mode");
            do
            {
                Console.WriteLine(" 11. Face");
                Console.WriteLine(" 12. Face + Fingerprint");
                Console.WriteLine(" 13. Face + PIN");
                Console.WriteLine(" 14. Face + Fingerprint/PIN");
                Console.WriteLine(" 15. Face + Fingerprint + PIN");
                Console.WriteLine("999. No more changes.");
                Console.Write(">> ");

                mode = (UInt32)Util.GetInput();
                if ((UInt32)BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY <= mode &&
                    mode < (UInt32)BS2ExtFaceAuthModeEnum.NUM_OF_EXT_FACE_AUTH_MODE)
                {
                    Console.WriteLine("0. Off (No time)");
                    Console.WriteLine("1. On (Always)");
                    Console.Write(">> ");
                    UInt32 onoff = (UInt32)Util.GetInput();
                    if (0 == onoff || 1 == onoff)
                    {
                        authConfigExt.extAuthSchedule[mode] = onoff;
                    }
                }
            } while (mode != 999);

            Console.WriteLine("Register Fingerprint authentication mode");
            do
            {
                Console.WriteLine(" 16. Fingerprint");
                Console.WriteLine(" 17. Fingerprint + Face");
                Console.WriteLine(" 18. Fingerprint + PIN");
                Console.WriteLine(" 19. Fingerprint + Face/PIN");
                Console.WriteLine(" 20. Fingerprint + Face + PIN");
                Console.WriteLine("999. No more changes.");
                Console.Write(">> ");

                mode = (UInt32)Util.GetInput();
                if ((UInt32)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY <= mode &&
                    mode < (UInt32)BS2ExtFingerprintAuthModeEnum.NUM_OF_EXT_FINGERPRINT_AUTH_MODE)
                {
                    Console.WriteLine("0. Off (No time)");
                    Console.WriteLine("1. On (Always)");
                    Console.Write(">> ");
                    UInt32 onoff = (UInt32)Util.GetInput();
                    if (0 == onoff || 1 == onoff)
                    {
                        authConfigExt.extAuthSchedule[mode] = onoff;
                    }
                }
            } while (mode != 999);

            Console.WriteLine("Register Card authentication mode");
            do
            {
                Console.WriteLine(" 21. Card");
                Console.WriteLine(" 22. Card + Face");
                Console.WriteLine(" 23. Card + Fingerprint");
                Console.WriteLine(" 24. Card + PIN");
                Console.WriteLine(" 25. Card + Face/Fingerprint");
                Console.WriteLine(" 26. Card + Face/PIN");
                Console.WriteLine(" 27. Card + Fingerprint/PIN");
                Console.WriteLine(" 28. Card + Face/Fingerprint/PIN");
                Console.WriteLine(" 29. Card + Face + Fingerprint");
                Console.WriteLine(" 30. Card + Face + PIN");
                Console.WriteLine(" 31. Card + Fingerprint + Face");
                Console.WriteLine(" 32. Card + Fingerprint + PIN");
                Console.WriteLine(" 33. Card + Face/Fingerprint + PIN");
                Console.WriteLine(" 34. Card + Face + Fingerprint/PIN");
                Console.WriteLine(" 35. Card + Fingerprint + Face/PIN");
                Console.WriteLine("999. No more changes.");
                Console.Write(">> ");

                mode = (UInt32)Util.GetInput();
                if ((UInt32)BS2ExtCardAuthModeEnum.EXT_CARD_ONLY <= mode &&
                    mode < (UInt32)BS2ExtCardAuthModeEnum.NUM_OF_EXT_CARD_AUTH_MODE)
                {
                    Console.WriteLine("0. Off (No time)");
                    Console.WriteLine("1. On (Always)");
                    Console.Write(">> ");
                    UInt32 onoff = (UInt32)Util.GetInput();
                    if (0 == onoff || 1 == onoff)
                    {
                        authConfigExt.extAuthSchedule[mode] = onoff;
                    }
                }
            } while (mode != 999);

            Console.WriteLine("Register ID authentication mode");
            do
            {
                Console.WriteLine(" 36. ID + Face");
                Console.WriteLine(" 37. ID + Fingerprint");
                Console.WriteLine(" 38. ID + PIN");
                Console.WriteLine(" 39. ID + Face/Fingerprint");
                Console.WriteLine(" 40. ID + Face/PIN");
                Console.WriteLine(" 41. ID + Fingerprint/PIN");
                Console.WriteLine(" 42. ID + Face/Fingerprint/PIN");
                Console.WriteLine(" 43. ID + Face + Fingerprint");
                Console.WriteLine(" 44. ID + Face + PIN");
                Console.WriteLine(" 45. ID + Fingerprint + Face");
                Console.WriteLine(" 46. ID + Fingerprint + PIN");
                Console.WriteLine(" 47. ID + Face/Fingerprint + PIN");
                Console.WriteLine(" 48. ID + Face + Fingerprint/PIN");
                Console.WriteLine(" 49. ID + Fingerprint + Face/PIN");
                Console.WriteLine("999. No more changes.");
                Console.Write(">> ");

                mode = (UInt32)Util.GetInput();
                if ((UInt32)BS2ExtIDAuthModeEnum.EXT_ID_FACE <= mode &&
                    mode < (UInt32)BS2ExtIDAuthModeEnum.NUM_OF_EXT_ID_AUTH_MODE)
                {
                    Console.WriteLine("0. Off (No time)");
                    Console.WriteLine("1. On (Always)");
                    Console.Write(">> ");
                    UInt32 onoff = (UInt32)Util.GetInput();
                    if (0 == onoff || 1 == onoff)
                    {
                        authConfigExt.extAuthSchedule[mode] = onoff;
                    }
                }
            } while (mode != 999);

            Console.WriteLine("Insert global APB option. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfigExt.useGlobalAPB = Util.GetInput((byte)0);

            Console.WriteLine("Insert global APB fail action. (0: Not use, 1: Soft APB, 2: Hard APB)");
            Console.Write(">> ");
            authConfigExt.globalAPBFailAction = Util.GetInput((byte)0);

            Console.WriteLine("Using group matching. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfigExt.useGroupMatching = Util.GetInput((byte)0);

            Console.WriteLine("Insert private authentication. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfigExt.usePrivateAuth = Util.GetInput((byte)0);

            Console.WriteLine("Insert face detection level. (0: Not use, 1: Normal mode, 2: Strict mode)");
            Console.Write(">> ");
            authConfigExt.faceDetectionLevel = Util.GetInput((byte)0);

            Console.WriteLine("Insert server matching option. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfigExt.useServerMatching = Util.GetInput((byte)0);

            Console.WriteLine("Using full access. (0: Not use, 1: Use)");
            Console.Write(">> ");
            authConfigExt.useFullAccess = Util.GetInput((byte)0);

            Console.WriteLine("Insert matching timeout in seconds");
            Console.Write(">> ");
            authConfigExt.matchTimeout = Util.GetInput((byte)5);

            Console.WriteLine("Insert authentication timeout in seconds");
            Console.Write(">> ");
            authConfigExt.authTimeout = Util.GetInput((byte)10);

            authConfigExt.numOperators = 0;

            Console.WriteLine("Trying to set authentication ext configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAuthConfigExt(sdkContext, deviceID, ref authConfigExt);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void getFaceConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2FaceConfigExt faceConfigExt;
            Console.WriteLine("Trying to get face ext configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetFaceConfigExt(sdkContext, deviceID, out faceConfigExt);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(faceConfigExt);
        }

        public void setFaceConfigEx(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2FaceConfigExt faceConfigExt = Util.AllocateStructure<BS2FaceConfigExt>();

            Console.WriteLine("Insert thermal check mode. (0: Not use, 1: Hard, 2: Soft)");
            Console.Write(">> ");
            faceConfigExt.thermalCheckMode = Util.GetInput((byte)0);

            Console.WriteLine("Insert mask check mode. (0: Not use, 1: Hard, 2: Soft)");
            Console.Write(">> ");
            faceConfigExt.maskCheckMode = Util.GetInput((byte)0);

            Console.WriteLine("Insert thermal format. (0: Fahrenheit, 1: Celsius)");
            Console.Write(">> ");
            faceConfigExt.thermalFormat = Util.GetInput((byte)1);

            const float THERMAL_THRESHOLD_DEFAULT_LOW = 32.0F;
            const float THERMAL_THRESHOLD_DEFAULT_HIGH = 38.0F;
            Console.WriteLine("Insert low value of high temperature range in Celsius. (1.0 ~ 45.0℃)");
            Console.Write(">> ");
            float thresholdLow = Util.GetInput(THERMAL_THRESHOLD_DEFAULT_LOW);
            faceConfigExt.thermalThresholdLow = (UInt16)(thresholdLow * 100);

            Console.WriteLine("Insert high value of high temperature range in Celsius. (1.0 ~ 45.0℃)");
            Console.Write(">> ");
            float thresholdHigh = Util.GetInput(THERMAL_THRESHOLD_DEFAULT_HIGH);
            faceConfigExt.thermalThresholdHigh = (UInt16)(thresholdHigh * 100);

            Console.WriteLine("Insert mask detection level. (0: Not use, 1: Normal, 2: High, 3: Very high)");
            Console.Write(">> ");
            faceConfigExt.maskDetectionLevel = Util.GetInput((byte)0);

            Console.WriteLine("Do you want to record the temperature in the event log? [y/n]");
            Console.Write(">> ");
            faceConfigExt.auditTemperature = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Do you want to use reject sound? [y/n]");
            Console.Write(">> ");
            faceConfigExt.useRejectSound = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Do you want to use overlapped thermal? [y/n]");
            Console.Write(">> ");
            faceConfigExt.useOverlapThermal = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Do you want to use dynamic ROI? [y/n]");
            Console.Write(">> ");
            faceConfigExt.useDynamicROI = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Insert face check order.");
            Console.WriteLine(" 0: Face check after auth [default]");
            Console.WriteLine(" 1: Face check before auth");
            Console.WriteLine(" 2: Face check without auth");
            Console.Write(">> ");
            faceConfigExt.faceCheckOrder = Util.GetInput((byte)0);

            Console.WriteLine("Trying to set face ext configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetFaceConfigExt(sdkContext, deviceID, ref faceConfigExt);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void getThermalCameraConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ThermalCameraConfig config;
            Console.WriteLine("Trying to get thermal camera configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetThermalCameraConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(config);
        }

        public void setThermalCameraConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ThermalCameraConfig config = Util.AllocateStructure<BS2ThermalCameraConfig>();

            Console.WriteLine("Insert camera distance from user. (cm. default: 100)");
            Console.Write(">> ");
            config.distance = Util.GetInput((byte)BS2Environment.BS2_THERMAL_CAMERA_DISTANCE_DEFAULT);

            Console.WriteLine("Insert emission rate. (95/97/98, default: 98)");
            Console.Write(">> ");
            config.emissionRate = Util.GetInput((byte)BS2Environment.BS2_THERMAL_CAMERA_EMISSIVITY_DEFAULT);

            Console.WriteLine("Insert ROI(Region of interest).");
            Console.WriteLine("  x");
            Console.Write("  >> ");
            config.roi.x = Util.GetInput((ushort)BS2Environment.BS2_THERMAL_CAMERA_ROI_X_DEFAULT);
            Console.WriteLine("  y");
            Console.Write("  >> ");
            config.roi.y = Util.GetInput((ushort)BS2Environment.BS2_THERMAL_CAMERA_ROI_Y_DEFAULT);
            Console.WriteLine("  width");
            Console.Write("  >> ");
            config.roi.width = Util.GetInput((ushort)BS2Environment.BS2_THERMAL_CAMERA_ROI_WIDTH_DEFAULT);
            Console.WriteLine("  height");
            Console.Write("  >> ");
            config.roi.height = Util.GetInput((ushort)BS2Environment.BS2_THERMAL_CAMERA_ROI_HEIGHT_DEFAULT);

            Console.WriteLine("Do you want to use body compensation [y/n]");
            Console.Write(">> ");
            config.useBodyCompensation = Util.IsYes() ? (byte)1 : (byte)0;

            Console.WriteLine("Insert compensation temperature *10. If you want -4.5℃, it is -45. (-50 ~ 50)");
            Console.Write(">> ");
            config.compensationTemperature = (sbyte)Util.GetInput();

            Console.WriteLine("Trying to set thermal camera configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetThermalCameraConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void getBarcodeConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2BarcodeConfig config;
            Console.WriteLine("Trying to get barcode configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetBarcodeConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(config);
        }

        public void setBarcodeConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2BarcodeConfig config = Util.AllocateStructure<BS2BarcodeConfig>();

            Console.WriteLine("Trying to set barcode configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetBarcodeConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Would you like to use barcode function? [Y/n]");
            Console.Write(">> ");
            bool useBarcode = Util.IsYes();

            switch ((BS2DeviceTypeEnum)deviceInfo.type)
            {
                case BS2DeviceTypeEnum.XSTATION_2_QR:
                    config.useBarcode = Convert.ToByte(useBarcode);
                    if (useBarcode)
                    {
                        Console.WriteLine("Set the barcode scan timeout in seconds. ({0}~{1})", BS2Environment.BS2_BARCODE_TIMEOUT_MIN, BS2Environment.BS2_BARCODE_TIMEOUT_MAX);
                        Console.Write(">> ");
                        config.scanTimeout = Util.GetInput((byte)BS2Environment.BS2_BARCODE_TIMEOUT_DEFAULT);
                    }
                    break;
                case BS2DeviceTypeEnum.XSTATION_2_FP:   // Supported V1.2.0
                case BS2DeviceTypeEnum.XSTATION_2:      // Supported V1.2.0
                case BS2DeviceTypeEnum.BIOSTATION_3:    // Supported V1.1.0
                    config.useVisualBarcode = Convert.ToByte(useBarcode);
                    if (useBarcode)
                    {
                        Console.WriteLine("Set the visual barcode scan timeout in seconds. ({0}~{1})", BS2Environment.BS2_VISUAL_BARCODE_TIMEOUT_MIN, BS2Environment.BS2_VISUAL_BARCODE_TIMEOUT_MAX);
                        Console.Write(">> ");
                        config.visualCameraScanTimeout = Util.GetInput((byte)BS2Environment.BS2_VISUAL_BARCODE_TIMEOUT_DEFAULT);

                        Console.WriteLine("Set the motion sensitivity. ({0}~{1})", (int)BS2MotionSensitivity.LOW, (int)BS2MotionSensitivity.HIGH);
                        Console.Write(">> ");
                        config.motionSensitivity = Util.GetInput((byte)BS2MotionSensitivity.NORMAL);
                    }
                    break;
                default:
                    return;
            }

            Console.WriteLine("Would you like to use QR-bypass? [Y/n]");
            Console.Write(">> ");
            bool useQRbypass = Util.IsYes();
            config.bypassData = Convert.ToByte(useQRbypass);
            cbOnBarcodeScanned = useQRbypass ? (new API.OnBarcodeScanned(barcodeScanned)) : null;
            result = (BS2ErrorCode)API.BS2_SetBarcodeScanListener(sdkContext, cbOnBarcodeScanned);
            if (BS2ErrorCode.BS_SDK_SUCCESS != result)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Do you want the barcode to use only number? like CSN? [Y/n]");
            Console.Write(">> ");
            config.treatAsCSN = Convert.ToByte(Util.IsYes());

            Console.WriteLine("Trying to set barcode configuration.");
            result = (BS2ErrorCode)API.BS2_SetBarcodeConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        private void barcodeScanned(UInt32 deviceId, string barcode)
		{
            Console.WriteLine("Device: {0}, Scanned: {1}", deviceId, barcode);
	    }

        void getVoipConfigExt(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2VoipConfigExt config;
            Console.WriteLine("Trying to get VoipConfigExt");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetVoipConfigExt(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }
        }

        public void setVoipConfigExt(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2VoipConfigExt config;
            Console.WriteLine("Trying to get VoipConfigExt");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetVoipConfigExt(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Do you want to use the VoIP Extension? [Y/n]");
            Console.Write(">> ");
            bool useVOIPExt = Util.IsYes();
            if (useVOIPExt)
            {
                config.enabled = Convert.ToByte(1);

                Console.WriteLine("Do you want to use Outbound proxy? [Y/n]");
                Console.Write(">> ");
                config.useOutboundProxy = Convert.ToByte(Util.IsYes());

                Console.WriteLine("Enter the interval in seconds to update the information on the SIP server. (60~600)");
                Console.Write(">> ");
                config.registrationDuration = Util.GetInput((ushort)300);

                Console.WriteLine("Enter the IP address of the SIP server.");
                Console.Write(">> ");
                string strIpAddr = Console.ReadLine();
                byte[] arrIpAddr = Encoding.UTF8.GetBytes(strIpAddr);
                Array.Clear(config.address, 0, BS2Environment.BS2_URL_SIZE);
                Array.Copy(arrIpAddr, 0, config.address, 0, arrIpAddr.Length);

                Console.WriteLine("Enter the port of the SIP server. (default: 5060)");
                Console.Write(">> ");
                config.port = Util.GetInput((ushort)5060);

                Console.WriteLine("Enter the intercom speaker volume between 0 and 100. (default: 50)");
                Console.Write(">> ");
                config.volume.speaker = Util.GetInput((byte)50);

                Console.WriteLine("Enter the intercom microphone volume between 0 and 100. (default: 50)");
                Console.Write(">> ");
                config.volume.mic = Util.GetInput((byte)50);

                Console.WriteLine("Enter the ID to connect to the SIP server.");
                Console.Write(">> ");
                string strSIPID = Console.ReadLine();
                byte[] arrSIPID = Encoding.UTF8.GetBytes(strSIPID);
                Array.Clear(config.id, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(arrSIPID, 0, config.id, 0, arrSIPID.Length);

                Console.WriteLine("Enter the password to connect to the SIP server.");
                Console.Write(">> ");
                string strSIPPW = Console.ReadLine();
                byte[] arrSIPPW = Encoding.UTF8.GetBytes(strSIPPW);
                Array.Clear(config.password, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(arrSIPPW, 0, config.password, 0, arrSIPPW.Length);

                Console.WriteLine("Enter the authorization code to connect to the SIP server.");
                Console.Write(">> ");
                string strAuthCode = Console.ReadLine();
                byte[] arrAuthCode = Encoding.UTF8.GetBytes(strAuthCode);
                Array.Clear(config.authorizationCode, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(arrAuthCode, 0, config.authorizationCode, 0, arrAuthCode.Length);

                Console.WriteLine("Enter the address of the Outbound proxy server.");
                Console.Write(">> ");
                string strProxyAddr = Console.ReadLine();
                byte[] arrProxyAddr = Encoding.UTF8.GetBytes(strProxyAddr);
                Array.Clear(config.outboundProxy.address, 0, BS2Environment.BS2_URL_SIZE);
                Array.Copy(arrProxyAddr, 0, config.outboundProxy.address, 0, arrProxyAddr.Length);

                Console.WriteLine("Enter the port of the Outbound proxy server.");
                Console.Write(">> ");
                config.outboundProxy.port = Util.GetInput((ushort)0);

                Console.WriteLine("Select the button symbol to be used as the exit button. (*, #, 0 ~ 9)");
                Console.Write(">> ");
                config.exitButton = (byte)Util.GetInput((char)'0');

                Console.WriteLine("Do you want to show the extension phone book? [Y/n]");
                Console.Write(">> ");
                config.showExtensionNumber = Convert.ToByte(Util.IsYes());

                Console.WriteLine("How many extension numbers would you like to register? (MAX: 128)");
                Console.Write(">> ");
                config.numPhoneBook = Util.GetInput((byte)0);

                for (byte idx = 0; idx < config.numPhoneBook; idx++)
                {
                    Console.WriteLine(" - Enter the extension phone number #{0}", idx);
                    Console.Write(">>>> ");
                    string strPhoneNum = Console.ReadLine();
                    byte[] arrPhoneNum = Encoding.UTF8.GetBytes(strPhoneNum);
                    Array.Clear(config.phonebook[idx].phoneNumber, 0, BS2Environment.BS2_USER_ID_SIZE);
                    Array.Copy(arrPhoneNum, 0, config.phonebook[idx].phoneNumber, 0, arrPhoneNum.Length);

                    Console.WriteLine(" - Enter the extension phone number #{0} description", idx);
                    Console.Write(">>>> ");
                    string strPhoneDesc = Console.ReadLine();
                    byte[] arrPhoneDesc = Encoding.UTF8.GetBytes(strPhoneDesc);
                    Array.Clear(config.phonebook[idx].description, 0, BS2Environment.BS2_VOIP_MAX_DESCRIPTION_LEN_EXT);
                    Array.Copy(arrPhoneDesc, 0, config.phonebook[idx].description, 0, arrPhoneDesc.Length);
                }
            }
            else
            {
                config.enabled = Convert.ToByte(0);
            }

            Console.WriteLine("Trying to set VoipConfigExt");
            result = (BS2ErrorCode)API.BS2_SetVoipConfigExt(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void getRtspConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2RtspConfig config;
            Console.WriteLine("Trying to get RtspConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetRtspConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, config);
            }
        }

        public void setRtspConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2RtspConfig config;
            Console.WriteLine("Trying to set RtspConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetRtspConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Do you want to use the RTSP server? [Y/n]");
            Console.Write(">> ");
            bool useRTSP = Util.IsYes();
            if (useRTSP)
            {
                config.enabled = Convert.ToByte(1);

                Console.WriteLine("Enter the account for the RTSP server.");
                Console.Write(">> ");
                string strRTSPID = Console.ReadLine();
                byte[] arrRTSPID = Encoding.UTF8.GetBytes(strRTSPID);
                Array.Clear(config.id, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(arrRTSPID, 0, config.id, 0, arrRTSPID.Length);

                Console.WriteLine("Enter the password for the RTSP server.");
                Console.Write(">> ");
                string strRTSPPW = Console.ReadLine();
                byte[] arrRTSPPW = Encoding.UTF8.GetBytes(strRTSPPW);
                Array.Clear(config.password, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(arrRTSPPW, 0, config.password, 0, arrRTSPPW.Length);

                Console.WriteLine("Enter the address of the RTSP server.");
                Console.Write(">> ");
                string strIpAddr = Console.ReadLine();
                byte[] arrIpAddr = Encoding.UTF8.GetBytes(strIpAddr);
                Array.Clear(config.address, 0, BS2Environment.BS2_URL_SIZE);
                Array.Copy(arrIpAddr, 0, config.address, 0, arrIpAddr.Length);

                Console.WriteLine("Enter the port of the RTSP server. (default: 554)");
                Console.Write(">> ");
                config.port = Util.GetInput((ushort)554);
            }
            else
            {
                config.enabled = Convert.ToByte(0);
            }

            Console.WriteLine("Trying to set RtspConfig");
            result = (BS2ErrorCode)API.BS2_SetRtspConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void getLicenseConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2LicenseConfig config;
            if (CommonControl.getLicenseConfig(sdkContext, deviceID, out config))
                CommonControl.print(ref config);
        }

        void getOsdpStandardConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardConfig config;
            if (CommonControl.getOsdpStandardConfig(sdkContext, deviceID, out config))
                CommonControl.print(ref config);
        }

        void getOsdpStandardActionConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardActionConfig config;
            if (CommonControl.getOsdpStandardActionConfig(sdkContext, deviceID, out config))
                CommonControl.print(ref config);
        }

        void setOsdpStandardActionConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardActionConfig config;
            if (!CommonControl.getOsdpStandardActionConfig(sdkContext, deviceID, out config))
                return;

            CommonControl.setOsdpStandardActionConfig(sdkContext, deviceID, ref config);
        }

        void print(IntPtr sdkContext, BS2DstConfig config)
        {
            Console.WriteLine(">>>> Daylight saving time configuration ");
            Console.WriteLine("     |--numSchedules : {0}", config.numSchedules);
            for (int idx = 0; idx < BS2Environment.BS2_MAX_DST_SCHEDULE; idx++)
            {
                Console.WriteLine("     |--schedules[{0}]", idx);
                Console.WriteLine("         |--timeOffset : {0}", config.schedules[idx].timeOffset);
                Console.WriteLine("         |--startTime");
                Console.WriteLine("             |--year : {0}", config.schedules[idx].startTime.year);
                Console.WriteLine("             |--month : {0}", config.schedules[idx].startTime.month);
                Console.WriteLine("             |--ordinal : {0}", config.schedules[idx].startTime.ordinal);
                Console.WriteLine("             |--weekDay : {0}", config.schedules[idx].startTime.weekDay);
                Console.WriteLine("             |--hour : {0}", config.schedules[idx].startTime.hour);
                Console.WriteLine("             |--minute : {0}", config.schedules[idx].startTime.minute);
                Console.WriteLine("             |--second : {0}", config.schedules[idx].startTime.second);
                Console.WriteLine("         |--endTime");
                Console.WriteLine("             |--year : {0}", config.schedules[idx].endTime.year);
                Console.WriteLine("             |--month : {0}", config.schedules[idx].endTime.month);
                Console.WriteLine("             |--ordinal : {0}", config.schedules[idx].endTime.ordinal);
                Console.WriteLine("             |--weekDay : {0}", config.schedules[idx].endTime.weekDay);
                Console.WriteLine("             |--hour : {0}", config.schedules[idx].endTime.hour);
                Console.WriteLine("             |--minute : {0}", config.schedules[idx].endTime.minute);
                Console.WriteLine("             |--second : {0}", config.schedules[idx].endTime.second);
            }
            Console.WriteLine("<<<< ");
        }

        void print(BS2DesFireCardConfigEx config)
        {
            Console.WriteLine(">>>> DesFire card configuration ");
            Console.WriteLine("     |--appMasterKey : {0}", config.desfireAppKey.appMasterKey);
            Console.WriteLine("     |--fileReadKey : {0}", config.desfireAppKey.fileReadKey);
            Console.WriteLine("     |--fileWriteKey : {0}", config.desfireAppKey.fileWriteKey);
            Console.WriteLine("     |--fileReadKeyNumber : {0}", config.desfireAppKey.fileReadKeyNumber);
            Console.WriteLine("     +--fileWriteKeyNumber : {0}", config.desfireAppKey.fileWriteKeyNumber);
            Console.WriteLine("<<<< ");
        }

        void print(BS2SystemConfig config)
        {
            Console.WriteLine(">>>> System configuration ");
            Console.WriteLine("     |--timezone : {0}", config.timezone);
            Console.WriteLine("     |--syncTime : {0}", config.syncTime);
            Console.WriteLine("     |--serverSync : {0}", config.serverSync);
            Console.WriteLine("     |--deviceLocked : {0}", config.deviceLocked);
            Console.WriteLine("     |--useInterphone : {0}", config.useInterphone);
            //Console.WriteLine("     |--useUSBConnection : {0}", config.useUSBConnection);
            Console.WriteLine("     |--keyEncrypted : {0}", config.keyEncrypted);
            Console.WriteLine("     |--useJobCode : {0}", config.useJobCode);
            Console.WriteLine("     |--useAlphanumericID : {0}", config.useAlphanumericID);
            Console.WriteLine("     |--cameraFrequency : {0}", config.cameraFrequency);
            Console.WriteLine("     |--secureTamper : {0}", config.secureTamper);
            Console.WriteLine("     +--useCardOperationMask : {0}", config.useCardOperationMask);
            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS2InputConfig config)
        {
            Console.WriteLine(">>>> Input configuration ");
            Console.WriteLine("     |--numInputs     : {0}", config.numInputs);
            Console.WriteLine("     |--numSupervised : {0}", config.numSupervised);
            for (int idx = 0; idx < BS2Environment.BS2_MAX_INPUT_NUM; idx++)
            {
                Console.WriteLine("     +--supervised_inputs[{0}]", idx);
                Console.WriteLine("     |--    portIndex        : {0}", config.supervised_inputs[idx].portIndex);
                Console.WriteLine("     |--    enabled          : {0}", config.supervised_inputs[idx].enabled);
                Console.WriteLine("     |--    supervised_index : {0}", config.supervised_inputs[idx].supervised_index);
                Console.WriteLine("     |--    config.shortInput.minValue : {0}", config.supervised_inputs[idx].config.shortInput.minValue);
                Console.WriteLine("     |--    config.shortInput.maxValue : {0}", config.supervised_inputs[idx].config.shortInput.maxValue);
                Console.WriteLine("     |--    config.openInput.minValue  : {0}", config.supervised_inputs[idx].config.openInput.minValue);
                Console.WriteLine("     |--    config.openInput.maxValue  : {0}", config.supervised_inputs[idx].config.openInput.maxValue);
                Console.WriteLine("     |--    config.onInput.minValue    : {0}", config.supervised_inputs[idx].config.onInput.minValue);
                Console.WriteLine("     |--    config.onInput.maxValue    : {0}", config.supervised_inputs[idx].config.onInput.maxValue);
                Console.WriteLine("     |--    config.offInput.minValue   : {0}", config.supervised_inputs[idx].config.offInput.minValue);
                Console.WriteLine("     +--    config.offInput.maxValue   : {0}", config.supervised_inputs[idx].config.offInput.maxValue);
            }
            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS1CardConfig config)
        {
            Console.WriteLine(">>>> BS1Card configuration ");
            Console.WriteLine("     |--magicNo : {0}", config.magicNo);
            Console.WriteLine("     |--disabled : {0}", config.disabled);
            Console.WriteLine("     |--useCSNOnly : {0}", config.useCSNOnly);
            Console.WriteLine("     |--bioentryCompatible : {0}", config.bioentryCompatible);
            Console.WriteLine("     |--useSecondaryKey : {0}", config.useSecondaryKey);
            Console.WriteLine("     |--primaryKey : {0}", BitConverter.ToString(config.primaryKey));
            Console.WriteLine("     |--secondaryKey : {0}", BitConverter.ToString(config.secondaryKey));
            Console.WriteLine("     |--cisIndex : {0}", config.cisIndex);
            Console.WriteLine("     |--numOfTemplate : {0}", config.numOfTemplate);
            Console.WriteLine("     |--templateSize : {0}", config.templateSize);
            Console.WriteLine("     |--templateStartBlock : {0},{1},{2},{3}", config.templateStartBlock[0], config.templateStartBlock[1], config.templateStartBlock[2], config.templateStartBlock[3]);
            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS2SystemConfigExt config)
        {
            Console.WriteLine(">>>> SystemExt configuration ");
            Console.WriteLine("     |--primarySecureKey : {0}", BitConverter.ToString(config.primarySecureKey));
            Console.WriteLine("     |--secondarySecureKey : {0}", BitConverter.ToString(config.secondarySecureKey));
            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS2VoipConfig config)
        {
            Console.WriteLine(">>>> Voip configuration ");
            Console.WriteLine("     |--serverUrl : {0}", Encoding.UTF8.GetString(config.serverUrl).TrimEnd('\0'));
            Console.WriteLine("     |--serverPort : {0}", config.serverPort);
            Console.WriteLine("     |--userID : {0}", Encoding.UTF8.GetString(config.userID).TrimEnd('\0'));
            Console.WriteLine("     |--userPW : {0}", Encoding.UTF8.GetString(config.userPW).TrimEnd('\0'));
            Console.WriteLine("     |--exitButton : {0}", config.exitButton);
            Console.WriteLine("     |--dtmfMode : {0}", config.dtmfMode);
            Console.WriteLine("     |--bUse : {0}", config.bUse);
            Console.WriteLine("     |--reseverd : {0}", config.reseverd[0]);
            Console.WriteLine("     |--numPhonBook : {0}", config.numPhonBook);
            for (int idx = 0; idx < config.numPhonBook; ++idx)
            {
                Console.WriteLine("     |++PhoneItem[{0}]", idx);
                Console.WriteLine("         |--phoneNumber : {0}", Encoding.UTF8.GetString(config.phonebook[idx].phoneNumber).TrimEnd('\0'));
                Console.WriteLine("         |--descript : {0}", Encoding.UTF8.GetString(config.phonebook[idx].descript).TrimEnd('\0'));
            }


            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS2FaceConfig config)
        {
            Console.WriteLine(">>>> Face configuration ");
            Console.WriteLine("     |--securityLevel : {0}", config.securityLevel);
            Console.WriteLine("     |--lightCondition : {0}", config.lightCondition);
            Console.WriteLine("     |--enrollThreshold : {0}", config.enrollThreshold);
            Console.WriteLine("     |--detectSensitivity : {0}", config.detectSensitivity);
            Console.WriteLine("     |--enrollTimeout : {0}", config.enrollTimeout);
            Console.WriteLine("     |--lfdLevel : {0}", config.lfdLevel);
            Console.WriteLine("     |--quickEnrollment : {0}", config.quickEnrollment);
            Console.WriteLine("     |--checkDuplicate : {0}", config.checkDuplicate);
            Console.WriteLine("     |--previewOption : {0}", config.previewOption);

            // FSF2 supported
            Console.WriteLine("     |--operationMode : {0}", config.operationMode);
            Console.WriteLine("     |--maxRotation : {0}", config.maxRotation);
            Console.WriteLine("     |--faceWidth.min : {0}, faceWidth.max : {1}", config.faceWidth.min, config.faceWidth.max);
            Console.WriteLine("     |--searchRange.x : {0}, searchRange.width : {1}", config.searchRange.x, config.searchRange.width);

            // BS3 supported
            Console.WriteLine("     |--detectDistance.min : {0}, detectDistance.max : {1}", config.detectDistance.min, config.detectDistance.max);
            Console.WriteLine("     |--wideSearch : {0}", config.wideSearch);

            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS2CardConfigEx config)
        {
            Console.WriteLine(">>>> CardEx configuration ");
            Console.WriteLine("     |--oid_ADF : {0}", BitConverter.ToString(config.seos.oid_ADF));
            Console.WriteLine("     |--size_ADF : {0}", config.seos.size_ADF);
            Console.WriteLine("     |--oid_DataObjectID : {0}", BitConverter.ToString(config.seos.oid_DataObjectID));
            Console.WriteLine("     |++size_DataObject");
            for (int i = 0; i < 8; ++i)
            {
                Console.WriteLine("     |--size{0} : {1}", i, config.seos.size_DataObject[i]);
            }

            Console.WriteLine("     |--primaryKeyAuth : {0}", BitConverter.ToString(config.seos.primaryKeyAuth));
            Console.WriteLine("     |--secondaryKeyAuth : {0}", BitConverter.ToString(config.seos.secondaryKeyAuth));

            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS2AuthGroup authGroup)
        {
            Console.WriteLine(">>>> AuthGroup id[{0}] name[{1}]", authGroup.id, Encoding.UTF8.GetString(authGroup.name).TrimEnd('\0'));
        }

        void print(IntPtr sdkContext, BS2DisplayConfig config)
        {
            Console.WriteLine(">>>> Display configuration ");
            Console.WriteLine("     |--useUserPhrase : {0}", config.useUserPhrase);
            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS2Rs485ConfigEX config)
        {
            Console.WriteLine(">>>> Rs485ConfigEX configuration ");
            Console.WriteLine("     |--mode : {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", config.mode[0], config.mode[1], config.mode[2], config.mode[3], config.mode[4], config.mode[5], config.mode[6], config.mode[7]);
            Console.WriteLine("     |--numOfChannels : {0}", config.numOfChannels);

            for (int idx = 0; idx < config.numOfChannels; ++idx)
            {
                Console.WriteLine("     |++channels[{0}]", idx);
                Console.WriteLine("         |--baudRate : {0}", config.channels[idx].baudRate);
                Console.WriteLine("         |--channelIndex : {0}", config.channels[idx].channelIndex);
                Console.WriteLine("         |--useRegistance : {0}", config.channels[idx].useRegistance);
                Console.WriteLine("         |--numOfDevices : {0}", config.channels[idx].numOfDevices);
                Console.WriteLine("         |--channelType : {0}", config.channels[idx].channelType);

                for (int idx2 = 0; idx2 < config.channels[idx].numOfDevices; ++idx2)
                {
                    Console.WriteLine("          |++slaveDevices[{0}]", idx2);
                    Console.WriteLine("                  |--deviceID : {0}", config.channels[idx].slaveDevices[idx2].deviceID);
                    Console.WriteLine("                  |--deviceType : {0}", config.channels[idx].slaveDevices[idx2].deviceType);
                    Console.WriteLine("                  |--enableOSDP : {0}", config.channels[idx].slaveDevices[idx2].enableOSDP);
                    Console.WriteLine("                  |--connected : {0}", config.channels[idx].slaveDevices[idx2].connected);
                    Console.WriteLine("                  |--channelInfo : {0}", config.channels[idx].slaveDevices[idx2].channelInfo);
                }
            }

            Console.WriteLine("<<<< ");
        }

        void print(BS2EncryptKey keyInfo)
        {
            Console.WriteLine(">>>> EncryptKey Information");
            Console.WriteLine("     +--key : {0}", BitConverter.ToString(keyInfo.key));
            //Console.WriteLine("     +--key : {0}", Encoding.UTF8.GetString(keyInfo.key).TrimEnd('\0'));

            Console.WriteLine("<<<< ");
        }

        void print(BS2AuthConfig config)
        {
            Console.WriteLine(">>>> AuthConfig configuration ");
            Console.WriteLine("     +--authSchedule");
            Console.WriteLine("        +--Biometric Only : {0}", config.authSchedule[(int)BS2FingerAuthModeEnum.BIOMETRIC_ONLY]);
            Console.WriteLine("        |--Biometric + PIN : {0}", config.authSchedule[(int)BS2FingerAuthModeEnum.BIOMETRIC_PIN]);
            Console.WriteLine("        |--Card Only : {0}", config.authSchedule[(int)BS2CardAuthModeEnum.CARD_ONLY]);
            Console.WriteLine("        |--Card + Biometric : {0}", config.authSchedule[(int)BS2CardAuthModeEnum.CARD_BIOMETRIC]);
            Console.WriteLine("        |--Card + PIN : {0}", config.authSchedule[(int)BS2CardAuthModeEnum.CARD_PIN]);
            Console.WriteLine("        |--Card + Biometric/PIN : {0}", config.authSchedule[(int)BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN]);
            Console.WriteLine("        |--Card + Biometric + PIN : {0}", config.authSchedule[(int)BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN]);
            Console.WriteLine("        |--ID + Biometric : {0}", config.authSchedule[(int)BS2IDAuthModeEnum.ID_BIOMETRIC]);
            Console.WriteLine("        |--ID + PIN : {0}", config.authSchedule[(int)BS2IDAuthModeEnum.ID_PIN]);
            Console.WriteLine("        |--ID + Biometric/PIN : {0}", config.authSchedule[(int)BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN]);
            Console.WriteLine("        +--ID + Biometric + PIN : {0}", config.authSchedule[(int)BS2IDAuthModeEnum.ID_BIOMETRIC_PIN]);
            Console.WriteLine("     +--useGlobalAPB : {0}", config.useGlobalAPB);
            Console.WriteLine("     |--globalAPBFailAction : {0}", config.globalAPBFailAction);
            Console.WriteLine("     |--usePrivateAuth : {0}", config.usePrivateAuth);
            Console.WriteLine("     |--faceDetectionLevel : {0}", config.faceDetectionLevel);
            Console.WriteLine("     |--useServerMatching : {0}", config.useServerMatching);
            Console.WriteLine("     |--useFullAccess : {0}", config.useFullAccess);
            Console.WriteLine("     |--matchTimeout : {0}", config.matchTimeout);
            Console.WriteLine("     |--authTimeout : {0}", config.authTimeout);
            Console.WriteLine("     +--numOperators : {0}", config.numOperators);
            Console.WriteLine("<<<< ");
        }

        void print(BS2AuthConfigExt config)
        {
            Console.WriteLine(">>>> AuthConfigExt configuration ");
            Console.WriteLine("     +--extAuthSchedule");
            Console.WriteLine("        +--Face : {0}", config.extAuthSchedule[(int)BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY]);
            Console.WriteLine("        |--Face + Fingerprint : {0}", config.extAuthSchedule[(int)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT]);
            Console.WriteLine("        |--Face + PIN : {0}", config.extAuthSchedule[(int)BS2ExtFaceAuthModeEnum.EXT_FACE_PIN]);
            Console.WriteLine("        |--Face + Fingerprint/PIN : {0}", config.extAuthSchedule[(int)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT_OR_PIN]);
            Console.WriteLine("        |--Face + Fingerprint + PIN : {0}", config.extAuthSchedule[(int)BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT_PIN]);
            Console.WriteLine("        +--Fingerprint : {0}", config.extAuthSchedule[(int)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY]);
            Console.WriteLine("        |--Fingerprint + Face : {0}", config.extAuthSchedule[(int)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE]);
            Console.WriteLine("        |--Fingerprint + PIN : {0}", config.extAuthSchedule[(int)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_PIN]);
            Console.WriteLine("        |--Fingerprint + Face/PIN : {0}", config.extAuthSchedule[(int)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE_OR_PIN]);
            Console.WriteLine("        |--Fingerprint + Face + PIN : {0}", config.extAuthSchedule[(int)BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE_PIN]);
            Console.WriteLine("        +--Card : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_ONLY]);
            Console.WriteLine("        |--Card + Face : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FACE]);
            Console.WriteLine("        |--Card + Fingerprint : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT]);
            Console.WriteLine("        |--Card + PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_PIN]);
            Console.WriteLine("        |--Card + Face/Fingerprint : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT]);
            Console.WriteLine("        |--Card + Face/PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_PIN]);
            Console.WriteLine("        |--Card + Fingerprint/PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_OR_PIN]);
            Console.WriteLine("        |--Card + Face/Fingerprint/PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_OR_PIN]);
            Console.WriteLine("        |--Card + Face + Fingerprint : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_FINGERPRINT]);
            Console.WriteLine("        |--Card + Face + PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_PIN]);
            Console.WriteLine("        |--Card + Fingerprint + Face : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_FACE]);
            Console.WriteLine("        |--Card + Fingerprint + PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_PIN]);
            Console.WriteLine("        |--Card + Face/Fingerprint + PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_PIN]);
            Console.WriteLine("        |--Card + Face + Fingerprint/PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FACE_FINGERPRINT_OR_PIN]);
            Console.WriteLine("        |--Card + Fingerprint + Face/PIN : {0}", config.extAuthSchedule[(int)BS2ExtCardAuthModeEnum.EXT_CARD_FINGERPRINT_FACE_OR_PIN]);
            Console.WriteLine("        +--ID + Face : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FACE]);
            Console.WriteLine("        |--ID + Fingerprint : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT]);
            Console.WriteLine("        |--ID + PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_PIN]);
            Console.WriteLine("        |--ID + Face/Fingerprint : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT]);
            Console.WriteLine("        |--ID + Face/PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_PIN]);
            Console.WriteLine("        |--ID + Fingerprint/PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_OR_PIN]);
            Console.WriteLine("        |--ID + Face/Fingerprint/PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_OR_PIN]);
            Console.WriteLine("        |--ID + Face + Fingerprint : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FACE_FINGERPRINT]);
            Console.WriteLine("        |--ID + Face + PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FACE_PIN]);
            Console.WriteLine("        |--ID + Fingerprint + Face : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_FACE]);
            Console.WriteLine("        |--ID + Fingerprint + PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_PIN]);
            Console.WriteLine("        |--ID + Face/Fingerprint + PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_PIN]);
            Console.WriteLine("        |--ID + Face + Fingerprint/PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FACE_FINGERPRINT_OR_PIN]);
            Console.WriteLine("        |--ID + Fingerprint + Face/PIN : {0}", config.extAuthSchedule[(int)BS2ExtIDAuthModeEnum.EXT_ID_FINGERPRINT_FACE_OR_PIN]);
            Console.WriteLine("     +--useGlobalAPB : {0}", config.useGlobalAPB);
            Console.WriteLine("     |--globalAPBFailAction : {0}", config.globalAPBFailAction);
            Console.WriteLine("     |--useGroupMatching : {0}", config.useGroupMatching);
            Console.WriteLine("     |--usePrivateAuth : {0}", config.usePrivateAuth);
            Console.WriteLine("     |--faceDetectionLevel : {0}", config.faceDetectionLevel);
            Console.WriteLine("     |--useServerMatching : {0}", config.useServerMatching);
            Console.WriteLine("     |--useFullAccess : {0}", config.useFullAccess);
            Console.WriteLine("     |--matchTimeout : {0}", config.matchTimeout);
            Console.WriteLine("     |--authTimeout : {0}", config.authTimeout);
            Console.WriteLine("     +--numOperators : {0}", config.numOperators);
            Console.WriteLine("<<<< ");
        }

        void print(BS2FaceConfigExt config)
        {
            Console.WriteLine(">>>> FaceConfigExt configuration ");
            Console.WriteLine("     +--thermalCheckMode : {0}", config.thermalCheckMode);
            Console.WriteLine("     |--maskCheckMode : {0}", config.maskCheckMode);
            Console.WriteLine("     |--thermalFormat : {0}", config.thermalFormat);
            float thresholdLow = (float)config.thermalThresholdLow / 100.0F;
            Console.WriteLine("     |--thermalThresholdLow : {0}", thresholdLow);
            float thresholdHigh = (float)config.thermalThresholdHigh / 100.0F;
            Console.WriteLine("     |--thermalThresholdHigh : {0}", thresholdHigh);
            Console.WriteLine("     |--maskDetectionLevel : {0}", config.maskDetectionLevel);
            Console.WriteLine("     |--auditTemperature : {0}", config.auditTemperature);
            Console.WriteLine("     |--useRejectSound : {0}", config.useRejectSound);
            Console.WriteLine("     |--useOverlapThermal : {0}", config.useOverlapThermal);
            Console.WriteLine("     |--useDynamicROI : {0}", config.useDynamicROI);
            Console.WriteLine("     +--faceCheckOrder : {0}", config.faceCheckOrder);
            Console.WriteLine("<<<< ");
        }

        void print(BS2ThermalCameraConfig config)
        {
            Console.WriteLine(">>>> ThermalCamera configuration ");
            Console.WriteLine("     +--distance : {0}", config.distance);
            Console.WriteLine("     |--emissionRate : {0}", config.emissionRate);
            Console.WriteLine("     +--roi");
            Console.WriteLine("        |--x : {0}", config.roi.x);
            Console.WriteLine("        |--y : {0}", config.roi.y);
            Console.WriteLine("        |--width : {0}", config.roi.width);
            Console.WriteLine("        |--height : {0}", config.roi.height);
            Console.WriteLine("     +--useBodyCompensation : {0}", config.useBodyCompensation);
            Console.WriteLine("     +--compensationTemperature : {0}", config.compensationTemperature);
        }

        void print(BS2BarcodeConfig config)
        {
            Console.WriteLine(">>>> Barcode configuration ");
            Console.WriteLine("     +--useBarcode : {0}", config.useBarcode);
            Console.WriteLine("     +--scanTimeout : {0}", config.scanTimeout);
            Console.WriteLine("     +--bypassData : {0}", config.bypassData);
            Console.WriteLine("     +--treatAsCSN : {0}", config.treatAsCSN);

            Console.WriteLine("     +--useVisualBarcode : {0}", config.useVisualBarcode);
            Console.WriteLine("     +--motionSensitivity : {0}", config.motionSensitivity);
            Console.WriteLine("     +--visualCameraScanTimeout : {0}", config.visualCameraScanTimeout);
        }

        void print(BS2Rs485Config config)
        {
            Console.WriteLine(">>>> RS485 configuration ");
            Console.WriteLine("     +--mode : {0}", config.mode);
            Console.WriteLine("     |--numOfChannels : {0}", config.numOfChannels);
            for (int index = 0; index < config.numOfChannels; index++)
                print(config.channels[index], index);

            print(config.intelligentInfo);
        }

        void print(BS2Rs485Channel channel, int index)
        {
            Console.WriteLine("     +--channels[{0}]", index);
            Console.WriteLine("     |  |--baudRate : {0}", channel.baudRate);
            Console.WriteLine("     |  |--channelIndex : {0}", channel.channelIndex);
            Console.WriteLine("     |  |--useRegistance : {0}", channel.useRegistance);
            Console.WriteLine("     |  |--numOfDevices : {0}", channel.numOfDevices);
            for (int idx = 0; idx < channel.numOfDevices; idx++)
            {
                print(channel.slaveDevices[idx], idx);
            }
        }

        void print(BS2Rs485SlaveDevice device, int index)
        {
            Console.WriteLine("     |  +--slaveDevices[{0}]", index);
            Console.WriteLine("     |  |  |--deviceID : {0}", device.deviceID);
            Console.WriteLine("     |  |  |--deviceType : {0}", device.deviceType);
            Console.WriteLine("     |  |  |--enableOSDP : {0}", device.enableOSDP);
            Console.WriteLine("     |  |  |--connected : {0}", device.connected);
        }

        void print(BS2IntelligentPDInfo info)
        {
            Console.WriteLine("     +--intelligentInfo");
            Console.WriteLine("     |  |--supportConfig : {0}", info.supportConfig);
            Console.WriteLine("     |  |--useExceptionCode : {0}", info.useExceptionCode);
            //Console.WriteLine("     |  |--exceptionCode : {0}", BitConverter.ToString(info.exceptionCode));
            Console.WriteLine("     |  |--exceptionCode : {0}", Util.ConvertHexByte2String(info.exceptionCode));
            Console.WriteLine("     |  |--outputFormat : {0}", info.outputFormat);
            Console.WriteLine("     +--+--osdpID : {0}", info.osdpID);
        }

        void print(IntPtr sdkContext, BS2VoipConfigExt config)
        {
            Console.WriteLine(">>>> VoipExt configuration ");
            Console.WriteLine("     |--enabled : {0}", config.enabled);
            Console.WriteLine("     |--useOutboundProxy : {0}", config.useOutboundProxy);
            Console.WriteLine("     |--registrationDuration : {0}", config.registrationDuration);
            Console.WriteLine("     |--address : {0}", Encoding.UTF8.GetString(config.address).TrimEnd('\0'));
            Console.WriteLine("     |--port : {0}", config.port);
            Console.WriteLine("     |--speaker : {0}", config.volume.speaker);
            Console.WriteLine("     |--mic : {0}", config.volume.mic);
            Console.WriteLine("     |--id : {0}", Encoding.UTF8.GetString(config.id).TrimEnd('\0'));
            Console.WriteLine("     |--password : {0}", Encoding.UTF8.GetString(config.password).TrimEnd('\0'));
            Console.WriteLine("     |--authorizationCode : {0}", Encoding.UTF8.GetString(config.authorizationCode).TrimEnd('\0'));
            Console.WriteLine("     +--outboundProxy");
            Console.WriteLine("        |--address : {0}", Encoding.UTF8.GetString(config.outboundProxy.address).TrimEnd('\0'));
            Console.WriteLine("        +--port : {0}", config.outboundProxy.port);
            Console.WriteLine("     |--exitButton : {0}", config.exitButton);
            Console.WriteLine("     |--numPhoneBook : {0}", config.numPhoneBook);
            Console.WriteLine("     |--showExtensionNumber : {0}", config.showExtensionNumber);
            Console.WriteLine("     +--phonebook : {0}", config.numPhoneBook);
            for (int idx = 0; idx < config.numPhoneBook; ++idx)
            {
                Console.WriteLine("         |--phoneNumber[{0}] : {1}", idx, Encoding.UTF8.GetString(config.phonebook[idx].phoneNumber).TrimEnd('\0'));
                Console.WriteLine("         |--description[{0}] : {1}", idx, Encoding.UTF8.GetString(config.phonebook[idx].description).TrimEnd('\0'));
            }

            Console.WriteLine("<<<< ");
        }

        void print(IntPtr sdkContext, BS2RtspConfig config)
        {
            Console.WriteLine(">>>> Rtsp configuration");
            Console.WriteLine("     |--id : {0}", Encoding.UTF8.GetString(config.id).TrimEnd('\0'));
            Console.WriteLine("     |--password : {0}", Encoding.UTF8.GetString(config.password).TrimEnd('\0'));
            Console.WriteLine("     |--address : {0}", Encoding.UTF8.GetString(config.address).TrimEnd('\0'));
            Console.WriteLine("     |--port : {0}", config.port);
            Console.WriteLine("     |--enabled : {0}", config.enabled);
        }
    }
}
