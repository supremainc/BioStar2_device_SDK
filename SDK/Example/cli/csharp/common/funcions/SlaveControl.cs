using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class SlaveControl : FunctionModule
    {
        private API.OnReadyToScan cbCardOnReadyToScan = null;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get slave device", getSlaveDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set slave device", setSlaveDevice));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get slaveEx device", getSlaveExDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set slaveEx device", setSlaveExDevice));

            //functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Turn on CST slave AuthMode", turnOnAuthMode));
            //functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Turn off CST slave AuthMode", turnOffAuthMode));

            return functionList;
        }

        public void getSlaveDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr slaveDeviceObj = IntPtr.Zero;
            UInt32 slaveDeviceCount = 0;

            Console.WriteLine("Trying to get the slave devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSlaveDevice(sdkContext, deviceID, out slaveDeviceObj, out slaveDeviceCount);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (slaveDeviceCount > 0)
            {
                List<BS2Rs485SlaveDevice> slaveDeviceList = new List<BS2Rs485SlaveDevice>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDevice));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }

                API.BS2_ReleaseObject(slaveDeviceObj);

                foreach (BS2Rs485SlaveDevice slaveDevice in slaveDeviceList)
                {
                    print(sdkContext, slaveDevice);
                }

                slaveControl(sdkContext, slaveDeviceList);
            }
            else
            {
                Console.WriteLine(">>> There is no slave device in the device.");
            }
        }

        public void setSlaveDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr slaveDeviceObj = IntPtr.Zero;
            UInt32 slaveDeviceCount = 0;

            Console.WriteLine("Trying to get the slave devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSlaveDevice(sdkContext, deviceID, out slaveDeviceObj, out slaveDeviceCount);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (slaveDeviceCount > 0)
            {
                List<BS2Rs485SlaveDevice> slaveDeviceList = new List<BS2Rs485SlaveDevice>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDevice));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }                

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                for (UInt32 idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice slaveDevice = slaveDeviceList[(int)idx];
                    Console.WriteLine("[{0:000}] ==> SlaveDevice id[{1, 10}] type[{2, 3}] model[{3, 16}] enable[{4}], connected[{5}]",
                                idx,
                                slaveDevice.deviceID,
                                slaveDevice.deviceType,
                                API.productNameDictionary[(BS2DeviceTypeEnum)slaveDevice.deviceType],
                                Convert.ToBoolean(slaveDevice.enableOSDP),
                                Convert.ToBoolean(slaveDevice.connected));
                }
                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("Enter the index of the slave device which you want to connect: [INDEX_1,INDEX_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] slaveDeviceIndexs = Console.ReadLine().Split(delimiterChars);
                HashSet<UInt32> connectSlaveDevice = new HashSet<UInt32>();

                if (slaveDeviceIndexs.Length == 0)
                {
                    Console.WriteLine("All of the slave device will be disabled.");
                }
                else
                {
                    foreach (string slaveDeviceIndex in slaveDeviceIndexs)
                    {
                        if (slaveDeviceIndex.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(slaveDeviceIndex, out item))
                            {
                                if (item < slaveDeviceCount)
                                {
                                    connectSlaveDevice.Add(slaveDeviceList[(int)item].deviceID);
                                }
                            }
                        }
                    }
                }

                curSlaveDeviceObj = slaveDeviceObj;
                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));

                    if (connectSlaveDevice.Contains(item.deviceID))
                    {
                        if (item.enableOSDP != 1)
                        {
                            item.enableOSDP = 1;
                            Marshal.StructureToPtr(item, curSlaveDeviceObj, false);
                        }
                    }
                    else
                    {
                        if (item.enableOSDP != 0)
                        {
                            item.enableOSDP = 0;
                            Marshal.StructureToPtr(item, curSlaveDeviceObj, false);
                        }
                    }

                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }

                Console.WriteLine("Trying to set the slave devices.");
                result = (BS2ErrorCode)API.BS2_SetSlaveDevice(sdkContext, deviceID, slaveDeviceObj, slaveDeviceCount);

                API.BS2_ReleaseObject(slaveDeviceObj);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else
                {
                    slaveControl(sdkContext, slaveDeviceList);
                }
            }
            else
            {
                Console.WriteLine(">>> There is no slave device in the device.");
            }
        }

        public void getSlaveExDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr slaveDeviceObj = IntPtr.Zero;
            UInt32 slaveDeviceCount = 0;
            UInt32 outchannelport = 0;

            Console.WriteLine("Trying to get the slave devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSlaveExDevice(sdkContext, deviceID, 0xFF, out slaveDeviceObj, out outchannelport, out slaveDeviceCount);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (slaveDeviceCount > 0)
            {
                List<BS2Rs485SlaveDeviceEX> slaveDeviceList = new List<BS2Rs485SlaveDeviceEX>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDeviceEX));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDeviceEX item = (BS2Rs485SlaveDeviceEX)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDeviceEX));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }

                API.BS2_ReleaseObject(slaveDeviceObj);

                foreach (BS2Rs485SlaveDeviceEX slaveExDevice in slaveDeviceList)
                {
                    print(sdkContext, slaveExDevice);
                }

                //slaveExControl(sdkContext, slaveDeviceList);
            }
            else
            {
                Console.WriteLine(">>> There is no slave device in the device.");
            }
        }

        public void setSlaveExDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr slaveDeviceObj = IntPtr.Zero;
            UInt32 slaveDeviceCount = 0;
            UInt32 outchannelport = 0;

            Console.WriteLine("Choose the RS485 port where the device is connected. [0(default), 1, 2, 3, 4]");
            Console.Write(">>>> ");
            int selchannel = Util.GetInput(0);

            Console.WriteLine("Trying to get the slave devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetSlaveExDevice(sdkContext, deviceID, (uint)selchannel, out slaveDeviceObj, out outchannelport, out slaveDeviceCount);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (slaveDeviceCount > 0)
            {
                List<BS2Rs485SlaveDeviceEX> slaveDeviceList = new List<BS2Rs485SlaveDeviceEX>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDeviceEX));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDeviceEX item = (BS2Rs485SlaveDeviceEX)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDeviceEX));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }

                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                for (UInt32 idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDeviceEX slaveDevice = slaveDeviceList[(int)idx];
                    Console.WriteLine("[{0:000}] ==> SlaveDevice id[{1, 10}] channel[{2}] type[{3, 3}] model[{4, 16}] enable[{5}], connected[{6}]",
                                idx,
                                slaveDevice.deviceID,
                                slaveDevice.channelInfo,
                                slaveDevice.deviceType,
                                API.productNameDictionary[(BS2DeviceTypeEnum)slaveDevice.deviceType],
                                Convert.ToBoolean(slaveDevice.enableOSDP),
                                Convert.ToBoolean(slaveDevice.connected));
                }
                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("Enter the index of the slave device which you want to connect: [INDEX_1,INDEX_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] slaveDeviceIndexs = Console.ReadLine().Split(delimiterChars);
                HashSet<UInt32> connectSlaveDevice = new HashSet<UInt32>();

                if (slaveDeviceIndexs.Length == 0)
                {
                    Console.WriteLine("All of the slave device will be disabled.");
                }
                else
                {
                    foreach (string slaveDeviceIndex in slaveDeviceIndexs)
                    {
                        if (slaveDeviceIndex.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(slaveDeviceIndex, out item))
                            {
                                if (item < slaveDeviceCount)
                                {
                                    connectSlaveDevice.Add(slaveDeviceList[(int)item].deviceID);
                                }
                            }
                        }
                    }
                }

                curSlaveDeviceObj = slaveDeviceObj;
                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDeviceEX item = (BS2Rs485SlaveDeviceEX)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDeviceEX));

                    if (connectSlaveDevice.Contains(item.deviceID))
                    {
                        if (item.enableOSDP != 1)
                        {
                            item.enableOSDP = 1;
                            Marshal.StructureToPtr(item, curSlaveDeviceObj, false);
                        }
                    }
                    else
                    {
                        if (item.enableOSDP != 0)
                        {
                            item.enableOSDP = 0;
                            Marshal.StructureToPtr(item, curSlaveDeviceObj, false);
                        }
                    }

                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                }

                Console.WriteLine("Trying to set the slave devices.");
                result = (BS2ErrorCode)API.BS2_SetSlaveExDevice(sdkContext, deviceID, (uint)selchannel, slaveDeviceObj, slaveDeviceCount);

                API.BS2_ReleaseObject(slaveDeviceObj);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else
                {
                    //slaveExControl(sdkContext, slaveDeviceList);
                    Console.WriteLine("Do you want update config of CST slaves? [y/n]");
                    Console.Write(">>>> ");
                    if (Util.IsYes())
                        slaveExConfig(sdkContext, deviceID, slaveDeviceList);
                }
            }
            else
            {
                Console.WriteLine(">>> There is no slave device in the device.");
            }
        }

        void slaveControl(IntPtr sdkContext, List<BS2Rs485SlaveDevice> slaveDeviceList)
        {
            //TODO implement this section.
        }

        //void slaveExControl(IntPtr sdkContext, List<BS2Rs485SlaveDeviceEX> slaveDeviceList)
        //{
        //    //TODO implement this section.
        //    foreach (BS2Rs485SlaveDeviceEX slaveDevice in slaveDeviceList)
        //    {
        //        BS2AuthConfig authConfig;
        //        Console.WriteLine("Getting auth config. reader[{0}]", slaveDevice.deviceID);
        //        BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, slaveDevice.deviceID, out authConfig);
        //        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
        //        {
        //            Console.WriteLine("Get auth config. reader[{0}]-error[{1}].", slaveDevice.deviceID, result);
        //            return;
        //        }
        //        Console.WriteLine("Get auth config. reader[{0}]-AuthMode[{1}].", slaveDevice.deviceID, getAuthInfo(authConfig));
        //    }
        //}

        public void getSlaveExAuthMode(IntPtr sdkContext, UInt32 slaveID)
        {
            BS2AuthConfig authConfig;
            Console.WriteLine("Getting auth config. reader[{0}]", slaveID);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, slaveID, out authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Get auth config. reader[{0}]-error[{1}].", slaveID, result);
                return;
            }
            Console.WriteLine("Get auth config. reader[{0}]-AuthMode[{1}].", slaveID, getAuthInfo(authConfig));
        }

        public void setSlaveExAuthMode(IntPtr sdkContext, UInt32 slaveID)
        {
            BS2AuthConfig authConfig;
            Console.WriteLine("Getting auth config.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, slaveID, out authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Get auth config. reader[{0}]-error[{1}].", slaveID, result);
                return;
            }

            bool stop = false;
            do {
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
                Console.Write(">>>> ");
                UInt32 mode = (UInt32)Util.GetInput();
                if (10 < mode && 999 != mode)
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
                    Console.Write(">>>> ");
                    UInt32 onoff = (UInt32)Util.GetInput();
                    if (0 == onoff || 1 == onoff)
                    {
                        authConfig.authSchedule[mode] = onoff;
                    }
                }
            } while (!stop);

            result = (BS2ErrorCode)API.BS2_SetAuthConfig(sdkContext, slaveID, ref authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            BS2AuthConfig configResult;
            result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, slaveID, out configResult);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(configResult);
        }

        public void getSlaveExCardConfig(IntPtr sdkContext, UInt32 slaveID)
        {
            BS2CardConfig cardConfig;
            Console.WriteLine("Getting card config. reader[{0}]", slaveID);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetCardConfig(sdkContext, slaveID, out cardConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Get card config. reader[{0}]-error[{1}].", slaveID, result);
                return;
            }
            Console.WriteLine("Get card config. reader[{0}]", slaveID);
            print(cardConfig);
        }

        public void setSlaveExCardConfig(IntPtr sdkContext, UInt32 deviceID, UInt32 slaveID)
        {
            // Enroll test user
            enrollUserEx(sdkContext, deviceID, slaveID, "testuser", "1", "1122");

            BS2ErrorCode result;
            Console.WriteLine("Do you want update card config? [y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Input card ID with keypad");
                Console.WriteLine("0. Off");
                Console.WriteLine("1. On");
                Console.Write(">>>> ");
                int mode = Util.GetInput();

                insertCardIDWithKeypad(sdkContext, slaveID, (mode != 0) ? (byte)1 : (byte)0);
            }
        }

        void slaveExConfig(IntPtr sdkContext, UInt32 deviceID, List<BS2Rs485SlaveDeviceEX> slaveList)
        {
            if (0 == slaveList.Count())
                return;

            UInt32 slaveID = 0;
            Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
            Console.WriteLine("1. Get slave auth mode for AuthConfig");
            Console.WriteLine("2. Set slave auth mode for AuthConfig");
            Console.WriteLine("3. Get slave card config");
            Console.WriteLine("4. Set slave card config");
            Console.WriteLine("5. Exit");
            Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
            Console.WriteLine("Please, choose a number.");
            Console.Write(">>>> ");
            switch (Util.GetInput((UInt16)0))
            {
                case 1:
                    slaveID = printSlaveList(slaveList);
                    if (0 < slaveID)
                        getSlaveExAuthMode(sdkContext, slaveID);
                    break;

                case 2:
                    slaveID = printSlaveList(slaveList);
                    if (0 < slaveID)
                        setSlaveExAuthMode(sdkContext, slaveID);
                    break;

                case 3:
                    slaveID = printSlaveList(slaveList);
                    if (0 < slaveID)
                        getSlaveExCardConfig(sdkContext, slaveID);
                    break;

                case 4:
                    slaveID = printSlaveList(slaveList);
                    if (0 < slaveID)
                        setSlaveExCardConfig(sdkContext, deviceID, slaveID);
                    break;

                default:
                    return;
            }
        }

        UInt32 printSlaveList(List<BS2Rs485SlaveDeviceEX> slaveList)
        {
            UInt16 index = 0;
            Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
            foreach (BS2Rs485SlaveDeviceEX slaveDevice in slaveList)
            {
                Console.WriteLine("{0:000} => ID[{1, 10}] Model[{2, 16}]",
                    index,
                    slaveDevice.deviceID,
                    API.productNameDictionary[(BS2DeviceTypeEnum)slaveDevice.deviceType]);
            }
            Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
            Console.WriteLine("Please, choose a number.");
            Console.Write(">>>> ");
            UInt16 num = Util.GetInput((UInt16)0);
            if ((0 <= num) && (num < slaveList.Count()))
            {
                return slaveList[num].deviceID;
            }

            return 0;
        }

        void enrollUserEx(IntPtr sdkContext, UInt32 deviceID, UInt32 slaveID, string userName, string userID, string pin)
        {
            BS2ErrorCode result;
            Console.WriteLine("Do you want Enroll user for test? [y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {

                BS2UserBlobEx[] user = Util.AllocateStructureArray<BS2UserBlobEx>(1);
                user[0].user.version = 0;
                user[0].user.formatVersion = 0;
                user[0].user.faceChecksum = 0;
                user[0].user.authGroupID = 0;
                user[0].user.numCards = 1;
                user[0].user.numFingers = 0;
                user[0].user.numFaces = 0;
                user[0].user.flag = 0;

                user[0].cardObjs = IntPtr.Zero;
                user[0].fingerObjs = IntPtr.Zero;
                user[0].faceObjs = IntPtr.Zero;

                Console.WriteLine("Enter the ID for the User which you want to enroll");
                Console.Write(">>>> ");
                byte[] userIDArray = Encoding.UTF8.GetBytes(userID);
                Array.Clear(user[0].user.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(userIDArray, user[0].user.userID, userIDArray.Length);

                user[0].setting.startTime = 946684800;
                user[0].setting.endTime = 1924991999;
                user[0].setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.NONE;
                user[0].setting.idAuthMode = (byte)BS2IDAuthModeEnum.NONE;
                user[0].setting.cardAuthMode = (byte)BS2CardAuthModeEnum.NONE;

                Array.Clear(user[0].name, 0, BS2Environment.BS2_USER_NAME_LEN);
                byte[] userNameArray = Encoding.UTF8.GetBytes(userName);
                Array.Copy(userNameArray, user[0].name, userNameArray.Length);

                Array.Clear(user[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                IntPtr pinChar = Marshal.StringToHGlobalAnsi(pin);
                IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                //result = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, pin, pinCode);
                result = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, pinChar, pinCode);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Can't generate pin code.");
                    return;
                }
                Marshal.Copy(pinCode, user[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                Marshal.FreeHGlobal(pinChar);
                Marshal.FreeHGlobal(pinCode);

                int structSize = Marshal.SizeOf(typeof(BS2CSNCard));
                BS2Card card = Util.AllocateStructure<BS2Card>();
                user[0].cardObjs = Marshal.AllocHGlobal(structSize * user[0].user.numCards);
                IntPtr curCardObjs = user[0].cardObjs;
                cbCardOnReadyToScan = new API.OnReadyToScan(ReadyToScanForCard);

                Console.WriteLine("Trying to scan card.");
                result = (BS2ErrorCode)API.BS2_ScanCard(sdkContext, slaveID, out card, cbCardOnReadyToScan);
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
                }
                cbCardOnReadyToScan = null;

                result = (BS2ErrorCode)API.BS2_EnrolUserEx(sdkContext, deviceID, user, 1, 1);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Enroll user failed.");
                    return;
                }

                Console.WriteLine("Enroll user succeed.");
            }
        }

        void insertCardIDWithKeypad(IntPtr sdkContext, UInt32 slaveID, byte onoff)
        {
            BS2CardConfig cardConfig;
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetCardConfig(sdkContext, slaveID, out cardConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Get card config. reader[{0}]-error[{1}].", slaveID, result);
                return;
            }

            cardConfig.cipher = onoff;

            result = (BS2ErrorCode)API.BS2_SetCardConfig(sdkContext, slaveID, ref cardConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(cardConfig);
        }

        public void changeAuthMode(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice, bool turnOn)
        {
            Console.WriteLine("Get slave device ID");
            Console.Write(">>>> ");
            UInt32 slaveID = (UInt32)Util.GetInput();

            BS2AuthConfig authConfig;
            Console.WriteLine("Getting auth config.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, slaveID, out authConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Select auth mode.");
            Console.WriteLine("  0.  Biometric Only");
            Console.WriteLine("  1.  Biometric + PIN");
            Console.WriteLine("  2.  Card Only");
            Console.WriteLine("  3.  Card + Biometric");
            Console.WriteLine("  4.  Card + PIN");
            Console.WriteLine("  5.  Card + Biometric/PIN");
            Console.WriteLine("  6.  Card + Biometric + PIN");
            Console.WriteLine("  7.  ID + Biometric");
            Console.WriteLine("  8.  ID + PIN");
            Console.WriteLine("  9.  ID + Biometric/PIN");
            Console.WriteLine("  10. ID + Biometric + PIN");
            Console.Write(">>>> ");
            UInt32 mode = (UInt32)Util.GetInput();
            if (10 < mode)
            {
                Console.WriteLine("Invalid auth mode");
                return;
            }
            else
            {
                authConfig.authSchedule[mode] = turnOn ? 1U : 0U;
                Console.WriteLine("Turn {0} auth mode.", turnOn ? "on" : "off");
                result = (BS2ErrorCode)API.BS2_SetAuthConfig(sdkContext, slaveID, ref authConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }
            }

            BS2AuthConfig configResult;
            result = (BS2ErrorCode)API.BS2_GetAuthConfig(sdkContext, slaveID, out configResult);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(configResult);
        }

        public void turnOnAuthMode(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            changeAuthMode(sdkContext, deviceID, isMasterDevice, true);
        }

        public void turnOffAuthMode(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            changeAuthMode(sdkContext, deviceID, isMasterDevice, false);
        }

        void print(IntPtr sdkContext, BS2Rs485SlaveDevice slaveDevice)
        {
            Console.WriteLine(">>>> SlaveDevice id[{0, 10}] type[{1, 3}] model[{2, 16}] enable[{3}], connected[{4}]", 
                                slaveDevice.deviceID, 
                                slaveDevice.deviceType,
                                API.productNameDictionary[(BS2DeviceTypeEnum)slaveDevice.deviceType],
                                Convert.ToBoolean(slaveDevice.enableOSDP),
                                Convert.ToBoolean(slaveDevice.connected));
        }

        void print(IntPtr sdkContext, BS2Rs485SlaveDeviceEX slaveExDevice)
        {
            Console.WriteLine(">>>> SlaveDevice id[{0, 10}] channel[{1}] type[{2, 3}] model[{3, 16}] enable[{4}], connected[{5}]",
                                slaveExDevice.deviceID,
                                slaveExDevice.channelInfo,
                                slaveExDevice.deviceType,
                                API.productNameDictionary[(BS2DeviceTypeEnum)slaveExDevice.deviceType],
                                Convert.ToBoolean(slaveExDevice.enableOSDP),
                                Convert.ToBoolean(slaveExDevice.connected));
        }

        string getAuthInfo(BS2AuthConfig config)
        {
            string result = "";
            bool first = true;
            for (BS2AuthModeEnum index = BS2AuthModeEnum.BS2_AUTH_MODE_BIOMETRIC_ONLY;
                index < BS2AuthModeEnum.BS2_NUM_OF_AUTH_MODE; index++)
            {
                if (0 < config.authSchedule[(uint)index])
                {
                    switch (index)
                    {
                    case BS2AuthModeEnum.BS2_AUTH_MODE_BIOMETRIC_ONLY:
                        result += !first ? ", " : "";
                        result += "Biometric";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_BIOMETRIC_PIN:
                        result += !first ? ", " : "";
                        result += "Biometric+Pin";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_CARD_ONLY:
                        result += !first ? ", " : "";
                        result += "Card";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_CARD_BIOMETRIC:
                        result += !first ? ", " : "";
                        result += "Card+Biometric";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_CARD_PIN:
                        result += !first ? ", " : "";
                        result += "Card+PIN";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_CARD_BIOMETRIC_OR_PIN:
                        result += !first ? ", " : "";
                        result += "Card+Biometric/PIN";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_CARD_BIOMETRIC_PIN:
                        result += !first ? ", " : "";
                        result += "Card+Biometric+PIN";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_ID_BIOMETRIC:
                        result += !first ? ", " : "";
                        result += "ID+Biometric";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_ID_PIN:
                        result += !first ? ", " : "";
                        result += "ID+PIN";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_ID_BIOMETRIC_OR_PIN:
                        result += !first ? ", " : "";
                        result += "ID+Biometric/PIN";
                        first = false;
                        break;
                    case BS2AuthModeEnum.BS2_AUTH_MODE_ID_BIOMETRIC_PIN:
                        result += !first ? ", " : "";
                        result += "ID+Biometric+PIN";
                        first = false;
                        break;
                    default:
                        break;
                    }
                }
            }

            return result;
        }

        void print(BS2AuthConfig config)
        {
            Console.WriteLine(">>>> Auth Configuration");
            Console.WriteLine("     +- Biometric ----------");
            Console.WriteLine("     |--- Biometric Only : {0}", config.authSchedule[0]);
            Console.WriteLine("     |--- Biometric + PIN : {0}", config.authSchedule[1]);
            Console.WriteLine("     +- Card ---------------");
            Console.WriteLine("     |--- Card Only : {0}", config.authSchedule[2]);
            Console.WriteLine("     |--- Card + Biometric : {0}", config.authSchedule[3]);
            Console.WriteLine("     |--- Card + PIN : {0}", config.authSchedule[4]);
            Console.WriteLine("     |--- Card + Biometric/PIN : {0}", config.authSchedule[5]);
            Console.WriteLine("     |--- Card + Biometric + PIN : {0}", config.authSchedule[6]);
            Console.WriteLine("     +- ID -----------------");
            Console.WriteLine("     |--- ID + Biometric : {0}", config.authSchedule[7]);
            Console.WriteLine("     |--- ID + PIN : {0}", config.authSchedule[8]);
            Console.WriteLine("     |--- ID + Biometric/PIN : {0}", config.authSchedule[9]);
            Console.WriteLine("     |--- ID + Biometric + PIN : {0}", config.authSchedule[10]);
        }

        void print(BS2CardConfig config)
        {
            Console.WriteLine(">>>> Card configuration formatID[{0}]", config.formatID);
            Console.WriteLine("     +--byteOrder[{0}]", (BS2CardByteOrderEnum)config.byteOrder);
            Console.WriteLine("     |--useWiegandFormat[{0}]", Convert.ToBoolean(config.useWiegandFormat));
            Console.WriteLine("     |--dataType[{0}]", (BS2CardDataTypeEnum)config.dataType);
            Console.WriteLine("     |--useSecondaryKey[{0}]", Convert.ToBoolean(config.useSecondaryKey));
            Console.WriteLine("     +--mifare");
            Console.WriteLine("     |  |--primaryKey[{0}]", BitConverter.ToString(config.mifare.primaryKey));
            Console.WriteLine("     |  |--secondaryKey[{0}]", BitConverter.ToString(config.mifare.secondaryKey));
            Console.WriteLine("     |  |--startBlockIndex[{0}]", config.mifare.startBlockIndex);
            Console.WriteLine("     +--iclass");
            Console.WriteLine("     |  |--primaryKey[{0}]", BitConverter.ToString(config.iclass.primaryKey));
            Console.WriteLine("     |  |--secondaryKey[{0}]", BitConverter.ToString(config.iclass.secondaryKey));
            Console.WriteLine("     |  |--startBlockIndex[{0}]", config.iclass.startBlockIndex);
            Console.WriteLine("     +--desfire");
            Console.WriteLine("     |  |--primaryKey[{0}]", BitConverter.ToString(config.desfire.primaryKey));
            Console.WriteLine("     |  |--secondaryKey[{0}]", BitConverter.ToString(config.desfire.secondaryKey));
            Console.WriteLine("     |  |--appID[{0}]", BitConverter.ToString(config.desfire.appID));
            Console.WriteLine("     |  |--fileID[{0}]", config.desfire.fileID);
            Console.WriteLine("     |  |--encryptionType[{0}]", config.desfire.encryptionType);
            Console.WriteLine("     +--cipher[{0}]", Convert.ToBoolean(config.cipher));
        }

        void ReadyToScanForCard(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your card on the device.");
        }
    }
}
