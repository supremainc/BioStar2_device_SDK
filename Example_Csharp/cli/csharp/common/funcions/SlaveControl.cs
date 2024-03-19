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
        private API.OnBarcodeScanned cbOnBarcodeScanned = null;
        private API.OnOsdpStandardDeviceStatusChanged cbOnOsdpStandardDeviceStatusChanged = null;
        private List<Tuple<UInt32, UInt16>> searchedSlave = new List<Tuple<UInt32, UInt16>>();

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

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get slaveEx device", getSlaveExDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set slaveEx device", setSlaveExDevice));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Update to master device", update485Config));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get BarcodeConfig", getBarcodeConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set BarcodeConfig", setBarcodeConfig));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));

            //functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Turn on CST slave AuthMode", turnOnAuthMode));
            //functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Turn off CST slave AuthMode", turnOffAuthMode));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get LicenseConfig", getLicenseConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Device License", setDeviceLicense));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Delete Device License", deleteDeviceLicense));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Device License", getDeviceLicense));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("-------------------------------", null));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get OsdpStandardConfig", getOsdpStandardConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get OsdpStandardActionConfig", getOsdpStandardActionConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set OsdpStandardActionConfig", setOsdpStandardActionConfig));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Add OsdpStandardDevice", addOsdpStandardDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get OsdpStandardDevice", getOsdpStandardDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get available OsdpStandardDevice", getAvailableOsdpStandardDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Update OsdpStandardDevice", updateOsdpStandardDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove OsdpStandardDevice", removeOsdpStandardDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get OsdpStandardDeviceCapability", getOsdpStandardDeviceCapability));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set OsdpStandardDeviceSecurityKey", setOsdpStandardDeviceSecurityKey));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Start monitor OsdpStandardDeviceStatus", startMonitorOsdpStandardDeviceStatus));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Stop monitor OsdpStandardDeviceStatus", stopMonitorOsdpStandardDeviceStatus));

            return functionList;
        }

        public void update485Config(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2Rs485Config config;
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetRS485Config(sdkContext, deviceID, out config);
            if (BS2ErrorCode.BS_SDK_SUCCESS != result)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Trying to set to master device.");
            config.mode = (byte)BS2RS485ModeEnum.MASTER;
            result = (BS2ErrorCode)API.BS2_SetRS485Config(sdkContext, deviceID, ref config);
            if (BS2ErrorCode.BS_SDK_SUCCESS != result)
            {
                Console.WriteLine("Set error({0}).", result);
                return;
            }
        }

        public void getLicenseConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            UInt32 id = Util.GetInputMasterOrSlaveID(deviceID);

            BS2LicenseConfig config;
            if (CommonControl.getLicenseConfig(sdkContext, id, out config))
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

        bool getOsdpID(ref BS2OsdpStandardConfig config, UInt32 osdpDeviceID, ref byte osdpID)
        {
            for (int idx = 0; idx < config.numOfChannels; idx++)
            {
                for (int sidx = 0; sidx < config.channels[idx].numOfDevices; sidx++)
                {
                    if (osdpDeviceID == config.channels[idx].slaveDevices[sidx].deviceID)
                    {
                        osdpID = config.channels[idx].slaveDevices[sidx].osdpID;
                        return true;
                    }
                }
            }

            return false;
        }

        void printOSDPDeviceID(ref BS2OsdpStandardConfig config, ref UInt32 numOfActivated)
        {
            numOfActivated = 0;
            for (int idx = 0; idx < config.numOfChannels; idx++)
            {
                for (int sidx = 0; sidx < config.channels[idx].numOfDevices; sidx++)
                {
                    if (Convert.ToBoolean(config.channels[idx].slaveDevices[sidx].activate))
                    {
                        Console.WriteLine("[{0}-{1}] {2}", idx, sidx, config.channels[idx].slaveDevices[sidx].deviceID);
                        numOfActivated++;
                    }
                }
            }
        }

        void addOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardDeviceAvailable allDevice;
            if (CommonControl.getAvailableOsdpStandardDevice(sdkContext, deviceID, out allDevice))
                CommonControl.print(ref allDevice);

            BS2OsdpStandardDeviceAdd addDevice = Util.AllocateStructure<BS2OsdpStandardDeviceAdd>();
            //BS2DeviceTypeEnum type = (BS2DeviceTypeEnum)deviceInfo.type;
            //byte numOfChannels = (byte)(BS2DeviceTypeEnum.CORESTATION_40 == type ? BS2Environment.BS2_RS485_MAX_CHANNELS : 1);

            Util.HighlightLine(">>>> Now add a OSDP device.", "add");
            //Console.WriteLine(">>>> Please enter the channel index. (0 ~ {0})", numOfChannels);
            addDevice.deviceID = Util.GetInputSlaveID();

            UInt32 channelIndex = 0;
            for (int idx = 0; idx < allDevice.numOfChannel; idx++)
            {
                for (int didx = 0; didx < allDevice.channels[idx].numOsdpAvailableDevice; didx++)
                {
                    if (allDevice.channels[idx].deviceIDs[didx] == addDevice.deviceID)
                        channelIndex = allDevice.channels[idx].channelIndex;
                }
            }

            Util.HighlightLine(">>>> Please enter the OSDP ID. [0 ~ 126]", "OSDP ID");
            Console.Write(">>>> ");
            addDevice.osdpID = Util.GetInput((byte)0);
            
            Util.HighlightLine(">>>> Does the OSDP device use secure communication? [Y/n]", "use secure communication");
            Console.Write(">>>> ");
            addDevice.useSecureSession = Convert.ToByte(Util.IsYes());

            addDevice.deviceType = Convert.ToByte(BS2DeviceTypeEnum.THIRD_OSDP_DEVICE);
            addDevice.activate = Convert.ToByte(true);

            UInt32 outChannelIndex = 0;
            if (CommonControl.addOsdpStandardDevice(sdkContext, deviceID, channelIndex, ref addDevice, out outChannelIndex))
            {
                Console.WriteLine("Successfully added OSDP device to channel {0}.", outChannelIndex);
            }
        }

        void getOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardConfig config;
            if (!CommonControl.getOsdpStandardConfig(sdkContext, deviceID, out config))
                return;

            UInt32 numOfActivated = 0;
            printOSDPDeviceID(ref config, ref numOfActivated);
            if (0 < numOfActivated)
            {
                UInt32 id = Util.GetInputSlaveID();

                BS2OsdpStandardDevice osdpDevice;
                if (CommonControl.getOsdpStandardDevice(sdkContext, id, out osdpDevice))
                    CommonControl.print(ref osdpDevice);
            }
        }

        void getAvailableOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardDeviceAvailable allDevice;
            if (CommonControl.getAvailableOsdpStandardDevice(sdkContext, deviceID, out allDevice))
                CommonControl.print(ref allDevice);
        }

        void updateOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardConfig config;
            if (!CommonControl.getOsdpStandardConfig(sdkContext, deviceID, out config))
                return;

            UInt32 numOfActivated = 0;
            printOSDPDeviceID(ref config, ref numOfActivated);

            string tempStr = String.Format("How many devices do you want to update? (0~{0})", numOfActivated);
            Util.HighlightLineMulti(tempStr, "How many", "update");
            Console.Write(">>>> ");
            int numOfDevice = Util.GetInput(1);
            if (0 < numOfDevice)
            {
                BS2OsdpStandardDeviceUpdate[] updateData = Util.AllocateStructureArray<BS2OsdpStandardDeviceUpdate>(numOfDevice);
                for (int idx = 0; idx < numOfDevice; idx++)
                {
                    tempStr = String.Format(">>>> [{0}] Please enter the slave ID to be updated.", idx + 1);
                    Util.HighlightLine(tempStr, "slave ID to be updated");
                    Console.Write(">>>> ");
                    updateData[idx].deviceID = Util.GetInput((UInt32)0);

                    if (!getOsdpID(ref config, updateData[idx].deviceID, ref updateData[idx].osdpID))
                    {
                        Console.WriteLine("The OSDP ID could not be found.");
                        return;
                    }

                    Console.WriteLine("Do you want to change the OSDP ID? (CurrentID: {0}) [Y/n]", updateData[idx].osdpID);
                    Console.Write(">>>> ");
                    if (Util.IsYes())
                    {
                        Util.HighlightLine(">>>> Please enter the OSDP ID. [0 ~ 126]", "OSDP ID");
                        Console.Write(">>>> ");
                        updateData[idx].osdpID = Util.GetInput((byte)0);
                    }

                    Util.HighlightLine(">>>> Do you like to enable the OSDP device? [Y/n]", "enable");
                    Console.Write(">>>> ");
                    updateData[idx].activate = Convert.ToByte(Util.IsYes());

                    Util.HighlightLine(">>>> Does the OSDP device use secure communication? [Y/n]", "use secure communication");
                    Console.Write(">>>> ");
                    updateData[idx].useSecureSession = Convert.ToByte(Util.IsYes());
                    updateData[idx].deviceType = Convert.ToByte(BS2DeviceTypeEnum.THIRD_OSDP_DEVICE);
                }

                List<BS2OsdpStandardDeviceResult> listResult = new List<BS2OsdpStandardDeviceResult>();
                if (CommonControl.updateOsdpStandardDevice(sdkContext, deviceID, ref updateData, ref listResult))
                {
                    UInt32 resultIdx = 0;
                    foreach (BS2OsdpStandardDeviceResult item in listResult)
                    {
                        CommonControl.print(item, resultIdx);
                        resultIdx++;
                    }
                }
            }
        }

        void removeOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardConfig config;
            if (!CommonControl.getOsdpStandardConfig(sdkContext, deviceID, out config))
                return;

            UInt32 numOfActivated = 0;
            printOSDPDeviceID(ref config, ref numOfActivated);

            string tempStr = String.Format("How many devices do you want to remove? (0~{0})", numOfActivated);
            Util.HighlightLineMulti(tempStr, "How many", "remove");
            Console.Write(">>>> ");
            int numOfDevice = Util.GetInput(1);
            if (0 < numOfDevice)
            {
                List<UInt32> removeIDs = new List<UInt32>();
                for (int idx = 0; idx < numOfDevice; idx++)
                {
                    tempStr = String.Format(">>>> [{0}] Please enter the slave ID to be removed.", idx + 1);
                    Util.HighlightLine(tempStr, "slave ID to be removed");
                    Console.Write(">>>> ");
                    removeIDs.Add(Util.GetInput((UInt32)0));
                }

                List<BS2OsdpStandardDeviceResult> listResult = new List<BS2OsdpStandardDeviceResult>();
                if (CommonControl.removeOsdpStandardDevice(sdkContext, deviceID, removeIDs.ToArray(), ref listResult))
                {
                    UInt32 resultIdx = 0;
                    foreach (BS2OsdpStandardDeviceResult item in listResult)
                    {
                        CommonControl.print(item, resultIdx);
                        resultIdx++;
                    }
                }
            }
        }

        void getOsdpStandardDeviceCapability(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2OsdpStandardConfig config;
            if (!CommonControl.getOsdpStandardConfig(sdkContext, deviceID, out config))
                return;

            UInt32 numOfActivated = 0;
            printOSDPDeviceID(ref config, ref numOfActivated);
            if (0 < numOfActivated)
            {
                UInt32 id = Util.GetInputSlaveID();

                BS2OsdpStandardDeviceCapability capability;
                if (CommonControl.getOsdpStandardDeviceCapability(sdkContext, id, out capability))
                    CommonControl.print(ref capability);
            }
        }

        void setOsdpStandardDeviceSecurityKey(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            UInt32 id = Util.GetInputMasterOrSlaveID(deviceID);

            if (id == deviceID)     // set key to master
            {
                BS2OsdpStandardDeviceSecurityKey keyInfo = Util.AllocateStructure<BS2OsdpStandardDeviceSecurityKey>();

                Util.HighlightLine(">>>> Please enter the OSDP security key.", "security key");
                Console.Write(">>>> ");
                string keyString = Console.ReadLine();
                byte[] buff = Encoding.UTF8.GetBytes(keyString);

                Array.Clear(keyInfo.key, 0, BS2Environment.BS2_OSDP_STANDARD_KEY_SIZE);
                Array.Copy(buff, 0, keyInfo.key, 0, keyString.Length);

                IntPtr ptrKey = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2OsdpStandardDeviceSecurityKey)));
                Marshal.StructureToPtr(keyInfo, ptrKey, false);

                CommonControl.setOsdpStandardDeviceSecurityKey(sdkContext, id, ptrKey);

                Marshal.FreeHGlobal(ptrKey);
            }
            else
            {
                CommonControl.setOsdpStandardDeviceSecurityKey(sdkContext, id, IntPtr.Zero);
            }
        }

        void onOSDPStandardDeviceStatusChanged(UInt32 deviceId, IntPtr notifyData)
        {
            if (notifyData != IntPtr.Zero)
            {
                BS2OsdpStandardDeviceNotify item = (BS2OsdpStandardDeviceNotify)Marshal.PtrToStructure(notifyData, typeof(BS2OsdpStandardDeviceNotify));
                CommonControl.print(ref item);
            }
        }

        void startMonitorOsdpStandardDeviceStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            cbOnOsdpStandardDeviceStatusChanged = new API.OnOsdpStandardDeviceStatusChanged(onOSDPStandardDeviceStatusChanged);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetOsdpStandardDeviceStatusListener(sdkContext, cbOnOsdpStandardDeviceStatusChanged);
            if (BS2ErrorCode.BS_SDK_SUCCESS != result)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
            Console.WriteLine("Start monitoring.");
        }

        void stopMonitorOsdpStandardDeviceStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetOsdpStandardDeviceStatusListener(sdkContext, null);

            cbOnOsdpStandardDeviceStatusChanged = null;
            if (BS2ErrorCode.BS_SDK_SUCCESS != result)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Stop monitoring.");
        }

        public void getBarcodeConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            UInt32 id = 0;

            Console.WriteLine(">>>> Do you want to get master settings? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                id = deviceID;
            }
            else
            {
                Console.WriteLine(">>>> Select the slave ID:");
                Console.Write(">>>> ");
                id = (UInt32)Util.GetInput();
            }

            BS2BarcodeConfig config;
            Console.WriteLine("Trying to get barcode configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetBarcodeConfig(sdkContext, id, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(config);
        }

        public void setBarcodeConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            UInt32 id = 0;
            UInt16 type = (UInt16)BS2DeviceTypeEnum.UNKNOWN;

            Console.WriteLine(">>>> Do you want to get master settings? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                id = deviceID;
                type = deviceInfo.type;
            }
            else
            {
                Console.WriteLine(">>>> Select the slave ID:");
                Console.Write(">>>> ");
                id = (UInt32)Util.GetInput();

                foreach(var slave in searchedSlave)
                {
                    if (slave.Item1 == id)
                    {
                        type = slave.Item2;
                        break;
                    }
                }
            }

            BS2BarcodeConfig config = Util.AllocateStructure<BS2BarcodeConfig>();

            Console.WriteLine("Trying to set barcode configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetBarcodeConfig(sdkContext, id, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Would you like to use barcode function? [Y/n]");
            Console.Write(">> ");
            bool useBarcode = Util.IsYes();

            switch ((BS2DeviceTypeEnum)type)
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
                case BS2DeviceTypeEnum.BIOENTRY_W3:
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
            result = (BS2ErrorCode)API.BS2_SetBarcodeConfig(sdkContext, id, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
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
                searchedSlave.Clear();
                List<BS2Rs485SlaveDevice> slaveDeviceList = new List<BS2Rs485SlaveDevice>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDevice));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                    searchedSlave.Add(new Tuple<UInt32, UInt16>(item.deviceID, item.deviceType));
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
                searchedSlave.Clear();
                List<BS2Rs485SlaveDevice> slaveDeviceList = new List<BS2Rs485SlaveDevice>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDevice));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                    searchedSlave.Add(new Tuple<UInt32, UInt16>(item.deviceID, item.deviceType));
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
                searchedSlave.Clear();
                List<BS2Rs485SlaveDeviceEX> slaveDeviceList = new List<BS2Rs485SlaveDeviceEX>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDeviceEX));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDeviceEX item = (BS2Rs485SlaveDeviceEX)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDeviceEX));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                    searchedSlave.Add(new Tuple<UInt32, UInt16>(item.deviceID, item.deviceType));
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
                searchedSlave.Clear();
                List<BS2Rs485SlaveDeviceEX> slaveDeviceList = new List<BS2Rs485SlaveDeviceEX>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDeviceEX));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDeviceEX item = (BS2Rs485SlaveDeviceEX)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDeviceEX));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
                    searchedSlave.Add(new Tuple<UInt32, UInt16>(item.deviceID, item.deviceType));
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
                UInt32 slaveID = 0;
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
                        slaveID = item.deviceID;
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
                //else
                //{
                //    //slaveExControl(sdkContext, slaveDeviceList);
                //    Console.WriteLine("Do you want update config of CST slaves? [y/n]");
                //    Console.Write(">>>> ");
                //    if (Util.IsYes())
                //        slaveExConfig(sdkContext, deviceID, slaveDeviceList);
                //}

                IntPtr wiegandDeviceObj = IntPtr.Zero;
                UInt32 wiegandDeviceCount = 0;
                Console.WriteLine("Trying to get the wiegand devices under the {0}", slaveID);
                result = (BS2ErrorCode)API.BS2_SearchWiegandDevices(sdkContext, slaveID, out wiegandDeviceObj, out wiegandDeviceCount);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }

                IntPtr tempPtr = wiegandDeviceObj;
                for (UInt32 idx = 0; idx < wiegandDeviceCount; idx++)
                {
                    UInt32 wiegandID = (UInt32)Marshal.PtrToStructure(tempPtr, typeof(UInt32));
                    Console.WriteLine(" *[{0}] {1}", idx, wiegandID);
                    tempPtr = (IntPtr)((long)tempPtr + sizeof(UInt32));
                }

                API.BS2_ReleaseObject(wiegandDeviceObj);
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

            while (true)
            {
                UInt32 slaveID = 0;
                Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                Console.WriteLine("1. Get slave auth mode for AuthConfig");
                Console.WriteLine("2. Set slave auth mode for AuthConfig");
                Console.WriteLine("3. Get slave card config");
                Console.WriteLine("4. Set slave card config");
                Console.WriteLine("5. Get InputConfigEx config");
                Console.WriteLine("6. Set InputConfigEx config");
                Console.WriteLine("7. Get RelayAction config");
                Console.WriteLine("8. Set RelayAction config");

                Console.WriteLine("9. Exit");
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

                    case 5:
                        slaveID = printSlaveList(slaveList);
                        if (0 < slaveID)
                            getInputConfigEx(sdkContext, slaveID);
                        break;

                    case 6:
                        slaveID = printSlaveList(slaveList);
                        if (0 < slaveID)
                            setInputConfigEx(sdkContext, slaveID);
                        break;

                    case 7:
                        slaveID = printSlaveList(slaveList);
                        if (0 < slaveID)
                            getRelayActionConfig(sdkContext, slaveID);
                        break;

                    case 8:
                        slaveID = printSlaveList(slaveList);
                        if (0 < slaveID)
                            setRelayActionConfig(sdkContext, slaveID);
                        break;

                    default:
                        return;
                }
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

        public void getInputConfigEx(IntPtr sdkContext, UInt32 slaveID)
        {
            BS2InputConfigEx config;
            Console.WriteLine("Trying to get InputConfigEx configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetInputConfigEx(sdkContext, slaveID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(config);
        }

        public void setInputConfigEx(IntPtr sdkContext, UInt32 slaveID)
        {
	        // As of 2021.08.03, only IM-120 is supported
	        BS2InputConfigEx config = Util.AllocateStructure<BS2InputConfigEx>();

	        const int STOP_N_SET = -1;

	        BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetInputConfigEx(sdkContext, slaveID, out config);
	        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

	        Console.WriteLine("Please enter number of inputs.");
            Console.Write(">>>> ");
            config.numInputs = Convert.ToByte(Util.GetInput());

	        Console.WriteLine("Please enter number of supervised inputs.");
            Console.Write(">>>> ");
            config.numSupervised = Convert.ToByte(Util.GetInput());

	        while (true)
	        {
		        Console.WriteLine("What input port would you like to set? [-1(Exit), 0, ..., {0}]", config.numSupervised - 1);
                Console.Write(">>>> ");
                int idx = Util.GetInput();
		        if (STOP_N_SET == idx)
			        break;

		        config.inputs[idx].portIndex = Convert.ToByte(idx);

		        Console.WriteLine("Please enter the switch type. (N/O: 0(default), N/C: 1)");
                Console.Write(">>>> ");
                config.inputs[idx].switchType = Util.GetInput((byte)BS2SwitchTypeEnum.NORMAL_OPEN);

		        Console.WriteLine("Please enter the duration.");
                Console.Write(">>>> ");
                config.inputs[idx].duration = Util.GetInput((UInt16)50);

		        Console.WriteLine("Please enter the type of resistance value for supervised input.");
		        Console.WriteLine("[0: 1K, 1: 2.2K, 2: 4.7K, 3: 10K, 254: Unsupervised]");
                Console.Write(">>>> ");
                config.inputs[idx].supervisedResistor = Util.GetInput((byte)BS2SupervisedResistor.SUPERVISED_RESISTOR_UNUSED);
	        }

            Console.WriteLine("Trying to set InputConfigEx configuration");
            result = (BS2ErrorCode)API.BS2_SetInputConfigEx(sdkContext, slaveID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void getRelayActionConfig(IntPtr sdkContext, UInt32 slaveID)
        {
            BS2RelayActionConfig config;
            Console.WriteLine("Trying to get RelayAction configuration");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetRelayActionConfig(sdkContext, slaveID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            print(config);
        }

        public void setRelayActionConfig(IntPtr sdkContext, UInt32 slaveID)
        {
            // As of 2021.08.03, only IM-120 is supported
            BS2RelayActionConfig config = Util.AllocateStructure<BS2RelayActionConfig>();

            const int STOP_N_SET = -1;

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetRelayActionConfig(sdkContext, slaveID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

   	        config.deviceID = slaveID;

            while (true)
            {
                Console.WriteLine("What relay port would you like to set? [-1(Exit), 0, ..., {0}]", BS2Environment.BS2_MAX_RELAY_ACTION - 1);
                Console.Write(">>>> ");
                int idxRelay = Util.GetInput();
                if (STOP_N_SET == idxRelay)
                    break;

                config.relay[idxRelay].port = Convert.ToByte(idxRelay);

                Console.WriteLine("Do you want to set an alarm for RS485 disconnection?");
                Console.Write(">>>> ");
                config.relay[idxRelay].disconnEnabled = Convert.ToByte(Util.IsYes());

	            while (true)
	            {
                    Console.WriteLine("What input port would you like to set? [-1(Exit), 0, ..., {0}]", BS2Environment.BS2_MAX_RELAY_ACTION_INPUT - 1);
                    Console.Write(">>>> ");
                    int idxInput = Util.GetInput();
                    if (STOP_N_SET == idxInput)
                        break;

		            config.relay[idxRelay].input[idxInput].port = Convert.ToByte(idxInput);

                    Console.WriteLine("Please enter the type of relay action input [0: None, 1: Linkage]");
                    Console.Write(">>>> ");
		            config.relay[idxRelay].input[idxInput].type =
                        Util.GetInput((byte)BS2RelayActionInputType.RELAY_ACTION_INPUT_TYPE_NONE);

                    Console.WriteLine("Please enter the mask of relay action input [0: None, 0x01: Alarm, 0x02: Fault]");
                    Console.Write(">>>> ");
		            config.relay[idxRelay].input[idxInput].mask =
                        Util.GetInput((byte)BS2RelayActionInputMask.RELAY_ACTION_INPUT_MASK_NONE);
                }
            }

            Console.WriteLine("Trying to set RelayActionConfig configuration");
            result = (BS2ErrorCode)API.BS2_SetRelayActionConfig(sdkContext, slaveID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void setDeviceLicense(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2LicenseBlob licenseBlob = Util.AllocateStructure<BS2LicenseBlob>();

            Console.WriteLine("Try adding a license");

            Console.WriteLine("Enter the license type. (0: None, 1: Visual QR)");
            Console.Write(">>>> ");
            licenseBlob.licenseType = Util.GetInput((UInt16)BS2LicenseType.VISUAL_QR_MASK);

            Console.WriteLine("How many devices do you want to register?");
            Console.Write(">>>> ");
            licenseBlob.numOfDevices = Util.GetInput((UInt16)1);

            if (0 < licenseBlob.numOfDevices)
            {
                // Device ID
                List<UInt32> listID = new List<UInt32>();
                UInt32 tempID = 0;
                for (UInt16 idx = 0; idx < licenseBlob.numOfDevices; idx++)
                {
                    Console.WriteLine("  Slave device ID #{0}", idx);
                    Console.Write("  >> ");
                    tempID = (UInt32)Util.GetInput();
                    listID.Add(tempID);
                }

                byte[] byteListID = listID.SelectMany(BitConverter.GetBytes).ToArray();
                int byteCount = Marshal.SizeOf(typeof(UInt32)) * licenseBlob.numOfDevices;

                licenseBlob.deviceIDObjs = Marshal.AllocHGlobal(byteCount);
                Marshal.Copy(byteListID, 0, licenseBlob.deviceIDObjs, byteCount);

                // License data
                Console.WriteLine("Enter the path and name of license.");
                Console.Write(">>>> ");
                string licensePath = Console.ReadLine();
                if (!File.Exists(licensePath))
                {
                    Console.WriteLine("Invalid license Path");
                    return;
                }

                if (Util.LoadBinary(licensePath, out licenseBlob.licenseObj, out licenseBlob.licenseLen))
                {
                    IntPtr resultObj = IntPtr.Zero;
                    UInt32 numOfResult = 0;

                    BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnableDeviceLicense(sdkContext, deviceID, ref licenseBlob, out resultObj, out numOfResult);
                    Marshal.FreeHGlobal(licenseBlob.licenseObj);

                    if (BS2ErrorCode.BS_SDK_SUCCESS != result)
                    {
                        Console.WriteLine("Got error({0}).", result);
                    }
                    else
                    {
                        IntPtr curResult = resultObj;
                        int resultSize = Marshal.SizeOf(typeof(BS2LicenseResult));
                        for (UInt32 idx = 0; idx < numOfResult; idx++)
                        {
                            BS2LicenseResult item = (BS2LicenseResult)Marshal.PtrToStructure(curResult, typeof(BS2LicenseResult));
                            print(item, idx);
                            curResult += resultSize;
                        }

                        API.BS2_ReleaseObject(resultObj);
                    }
                } // if (Util.LoadBinary(licensePath, out licenseBlob.licenseObj, out licenseBlob.licenseLen))
            } // if (0 < licenseBlob.numOfDevices)

            Marshal.FreeHGlobal(licenseBlob.deviceIDObjs);
        }

        public void deleteDeviceLicense(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2LicenseBlob licenseBlob = Util.AllocateStructure<BS2LicenseBlob>();

            Console.WriteLine("Try removing the license");

            Console.WriteLine("Enter the license type. (0: None, 1: Visual QR)");
            Console.Write(">>>> ");
            licenseBlob.licenseType = Util.GetInput((UInt16)BS2LicenseType.VISUAL_QR_MASK);

            Console.WriteLine("How many devices do you want to remove?");
            Console.Write(">>>> ");
            licenseBlob.numOfDevices = Util.GetInput((UInt16)1);

            if (0 < licenseBlob.numOfDevices)
            {
                // Device ID
                List<UInt32> listID = new List<UInt32>();
                UInt32 tempID = 0;
                for (UInt16 idx = 0; idx < licenseBlob.numOfDevices; idx++)
                {
                    Console.WriteLine("  Slave device ID #{0}", idx);
                    Console.Write("  >> ");
                    tempID = (UInt32)Util.GetInput();
                    listID.Add(tempID);
                }

                byte[] byteListID = listID.SelectMany(BitConverter.GetBytes).ToArray();
                int byteCount = Marshal.SizeOf(typeof(UInt32)) * licenseBlob.numOfDevices;

                licenseBlob.deviceIDObjs = Marshal.AllocHGlobal(byteCount);
                Marshal.Copy(byteListID, 0, licenseBlob.deviceIDObjs, byteCount);

                // License data
                licenseBlob.licenseLen = 0;
                licenseBlob.licenseObj = IntPtr.Zero;

                IntPtr resultObj = IntPtr.Zero;
                UInt32 numOfResult = 0;

                BS2ErrorCode result = (BS2ErrorCode)API.BS2_DisableDeviceLicense(sdkContext, deviceID, ref licenseBlob, out resultObj, out numOfResult);

                if (BS2ErrorCode.BS_SDK_SUCCESS != result)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else
                {
                    IntPtr curResult = resultObj;
                    int resultSize = Marshal.SizeOf(typeof(BS2LicenseResult));
                    for (UInt32 idx = 0; idx < numOfResult; idx++)
                    {
                        BS2LicenseResult item = (BS2LicenseResult)Marshal.PtrToStructure(curResult, typeof(BS2LicenseResult));
                        print(item, idx);
                        curResult += resultSize;
                    }

                    API.BS2_ReleaseObject(resultObj);
                }
            } // if (0 < licenseBlob.numOfDevices)

            Marshal.FreeHGlobal(licenseBlob.deviceIDObjs);
        }

        public void getDeviceLicense(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to get a license");

            Console.WriteLine("Enter the license type. (0: None, 1: Visual QR)");
            Console.Write(">>>> ");
            UInt16 licenseType = Util.GetInput((UInt16)BS2LicenseType.VISUAL_QR_MASK);

            IntPtr resultObj = IntPtr.Zero;
            UInt32 numOfResult = 0;

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_QueryDeviceLicense(sdkContext, deviceID, licenseType, out resultObj, out numOfResult);

            if (BS2ErrorCode.BS_SDK_SUCCESS != result)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                IntPtr curResult = resultObj;
                int resultSize = Marshal.SizeOf(typeof(BS2LicenseResult));
                for (UInt32 idx = 0; idx < numOfResult; idx++)
                {
                    BS2LicenseResult item = (BS2LicenseResult)Marshal.PtrToStructure(curResult, typeof(BS2LicenseResult));
                    print(item, idx);
                    curResult += resultSize;
                }

                API.BS2_ReleaseObject(resultObj);
            }
        }

        private void barcodeScanned(UInt32 deviceId, string barcode)
        {
            Console.WriteLine("Device: {0}, Scanned: {1}", deviceId, barcode);
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

        void print(BS2InputConfigEx config)
        {
            Console.WriteLine(">>>> InputConfigEx configuration");
            Console.WriteLine("     +--numInputs : {0}", config.numInputs);
            Console.WriteLine("     |--numSupervised : {0}", config.numSupervised);

            for (byte idx = 0; idx < config.numSupervised; idx++)
            {
                Console.WriteLine("     +--inputs ({0})", idx);
                Console.WriteLine("        |--portIndex : {0}", config.inputs[idx].portIndex);
                Console.WriteLine("        |--switchType : {0}", config.inputs[idx].switchType);
                Console.WriteLine("        |--duration : {0}", config.inputs[idx].duration);
                Console.WriteLine("        |--supervisedResistor : {0}", config.inputs[idx].supervisedResistor);
            }
        }

        void print(BS2RelayActionConfig config)
        {
            Console.WriteLine(">>>> RelayAction configuraion");
            Console.WriteLine("     +--deviceID : {0}", config.deviceID);

            for (byte idxRelay = 0; idxRelay < BS2Environment.BS2_MAX_RELAY_ACTION; idxRelay++)
            {
                Console.WriteLine("     +--relay ({0})", idxRelay);
                Console.WriteLine("        |--port : {0}", config.relay[idxRelay].port);
                Console.WriteLine("        |--disconnEnabled : {0}", config.relay[idxRelay].disconnEnabled);

                for (byte idxInput = 0; idxInput < BS2Environment.BS2_MAX_RELAY_ACTION_INPUT; idxInput++)
                {
                    Console.WriteLine("        |--input({0}) -> port: {1}, type: {2}, mask: {3}",
                        idxInput,
                        config.relay[idxRelay].input[idxInput].port,
                        config.relay[idxRelay].input[idxInput].type,
                        config.relay[idxRelay].input[idxInput].mask);
                }
            }
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

        void print(BS2LicenseConfig config)
        {
            Console.WriteLine(">>>> Device license configuration ");
            Console.WriteLine("     |--version : {0}", config.version);
            Console.WriteLine("     |--numOfLicense : {0}", config.numOfLicense);

            for (int idx = 0; idx < config.numOfLicense; idx++)
            {
                Console.WriteLine("     +--license[{0}]", idx);
                Console.WriteLine("         |--index : {0}", config.license[idx].index);
                Console.WriteLine("         |--hasCapability : {0}", config.license[idx].hasCapability);
                Console.WriteLine("         |--enable : {0}", config.license[idx].enable);
                Console.WriteLine("         |--licenseType : {0}", config.license[idx].licenseType);
                Console.WriteLine("         |--licenseSubType : {0}", config.license[idx].licenseSubType);
                Console.WriteLine("         |--enableTime : {0}", config.license[idx].enableTime);
                Console.WriteLine("         |--expiredTime : {0}", config.license[idx].expiredTime);
                Console.WriteLine("         |--issueNumber : {0}", config.license[idx].issueNumber);
                Console.WriteLine("         |--licenseName : {0}", Encoding.Default.GetString(config.license[idx].name));
            }
        }

        void print(BS2LicenseResult result, UInt32 index)
        {
            Console.WriteLine("<<<< License result[{0}]", index);
            Console.WriteLine("     |--deviceID : {0}", result.deviceID);
            Console.WriteLine("     |--status : {0}", result.status);
        }
    }
}
