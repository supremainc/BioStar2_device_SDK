using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    using BS2_CONFIG_MASK = UInt32;

    public class ConfigControl : FunctionModule
    {
        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get AllConfig", getAllConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Card1xConfig", getCard1xConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Card1xConfig", setCard1xConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get SystemExtConfig", getSystemExtConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set SystemExtConfig", setSystemExtConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get VoipConfig", getVoipConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set VoipConfig", setVoipConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get FaceConfig", getFaceConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set FaceConfig", setFaceConfig));            
            
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get auth group", getAuthGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove auth group", removeAuthGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set auth group", setAuthGroup));            

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("disable ssl", disbleSSL));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get RS485ConfigEx", getRS485ConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set RS485ConfigEx", setRS485ConfigEx));      

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get CardConfigEx", getCardConfigEx));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set CardConfigEx", setCardConfigEx));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get supported Config Mask", getConfigMask));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get DstConfig", getDstConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set DstConfig", setDstConfig));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get InputConfig", getInputConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set InputConfig", setInputConfig));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get DataEncryptKey", getDataEncryptKey));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set DataEncryptKey", setDataEncryptKey));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove DataEncryptKey", removeDataEncryptKey));

            return functionList;
        }

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
            Console.WriteLine("Trying to get SystemExtConfig");
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
                Console.WriteLine("Trying to get all auth gruops from device.");
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

                    Console.WriteLine("Trying to get auth gruops from device.");
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
                Console.WriteLine("Trying to remove all auth gruops from device.");
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

                    Console.WriteLine("Trying to remove auth gruops from device.");
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

            Console.WriteLine("Trying to set RS485ConfigEx configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetRS485ConfigEx(sdkContext, deviceID, ref config);
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

            Console.WriteLine("Trying to set input configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetInputConfig(sdkContext, deviceID, ref config);
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
    }
}
