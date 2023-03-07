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

    public static class CommonControl
    {
        public static bool getLicenseConfig(IntPtr sdkContext, UInt32 deviceID, out BS2LicenseConfig config)
        {
            Console.WriteLine("Trying to get LicenseConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetLicenseConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool getOsdpStandardConfig(IntPtr sdkContext, UInt32 deviceID, out BS2OsdpStandardConfig config)
        {
            Console.WriteLine("Trying to get OsdpStandardConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetOsdpStandardConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool getOsdpStandardActionConfig(IntPtr sdkContext, UInt32 deviceID, out BS2OsdpStandardActionConfig config)
        {
            Console.WriteLine("Trying to get OsdpStandardActionConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetOsdpStandardActionConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool setOsdpStandardActionConfig(IntPtr sdkContext, UInt32 deviceID, ref BS2OsdpStandardActionConfig config)
        {
            bool activate = false;
            for (int idx = 0; idx < (int)BS2OsdpStandardActionType.COUNT; idx++)
            {
                Console.WriteLine("Do you want to modify the {0} action type? [Y/n]", (BS2OsdpStandardActionType)config.actions[idx].actionType);
                Console.Write(">> ");
                if (Util.IsYes())
                {
                    for (int ledidx = 0; ledidx < BS2Environment.BS2_OSDP_STANDARD_ACTION_MAX_LED; ledidx++)
                    {
                        string tempStr = String.Format("LED#{0}", ledidx);
                        string msgStr = String.Format("Do you want to activate for {0} action? [Y/n]", tempStr);
                        Util.HighlightLine(msgStr, tempStr);
                        Console.Write(">> ");
                        activate = Util.IsYes();
                        config.actions[idx].led[ledidx].use = Convert.ToByte(activate);

                        if (activate)
                        {
                            Util.HighlightLine("  Please enter your reader number. Default(0).", "reader number");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].readerNumber = Util.GetInput((byte)0);
                            Util.HighlightLine("  Please enter a led number of the reader. Default(0).", "led number");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].ledNumber = Util.GetInput((byte)0);

                            Util.HighlightLine("  Please enter a temporary command (0: NOP, 1: Cancel, 2: Set)", "temporary command");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].tempCommand = Util.GetInput((byte)BS2OsdpStandardLEDCommand.NOP);
                            Util.HighlightLine("  Please enter the temporary command on time in 100 ms.", "on time");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].tempOnTime = Util.GetInput((byte)10);
                            Util.HighlightLine("  Please enter the temporary command off time in 100 ms.", "off time");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].tempOffTime = Util.GetInput((byte)0);
                            Util.HighlightLine("  Please enter the temporary command on color. (0: Black, 1: Red, 2: Green, 3: Amber, 4: Blue, 5: Magenta, 6: Cyan, 7: White)", "on color");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].tempOnColor = Util.GetInput((byte)BS2OsdpStandardColor.GREEN);
                            Util.HighlightLine("  Please enter the temporary command off color. (0: Black, 1: Red, 2: Green, 3: Amber, 4: Blue, 5: Magenta, 6: Cyan, 7: White)", "off color");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].tempOffColor = Util.GetInput((byte)BS2OsdpStandardColor.BLACK);
                            Util.HighlightLine("  Please enter the temporary run time in 100 ms.", "run time");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].tempRunTime = Util.GetInput((UInt16)10);

                            Util.HighlightLine("  Please enter a permanent command (0: NOP, 1: Cancel, 2: Set)", "permanent command");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].permCommand = Util.GetInput((byte)BS2OsdpStandardLEDCommand.NOP);
                            Util.HighlightLine("  Please enter the permanent on time in 100 ms.", "on time");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].permOnTime = Util.GetInput((byte)0);
                            Util.HighlightLine("  Please enter the permanent off time in 100 ms.", "off time");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].permOffTime = Util.GetInput((byte)0);
                            Util.HighlightLine("  Please enter the permanent on color. (0: Black, 1: Red, 2: Green, 3: Amber, 4: Blue, 5: Magenta, 6: Cyan, 7: White)", "on color");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].permOnColor = Util.GetInput((byte)BS2OsdpStandardColor.BLACK);
                            Util.HighlightLine("  Please enter the permanent off color. (0: Black, 1: Red, 2: Green, 3: Amber, 4: Blue, 5: Magenta, 6: Cyan, 7: White)", "off color");
                            Console.Write("  >>>> ");
                            config.actions[idx].led[ledidx].permOffColor = Util.GetInput((byte)BS2OsdpStandardColor.BLACK);
                        }
                    }

                    Util.HighlightLine("Do you want to activate for buzzer action? [Y/n]", "buzzer");
                    Console.Write(">> ");
                    activate = Util.IsYes();
                    config.actions[idx].buzzer.use = Convert.ToByte(activate);
                    if (activate)
                    {
                        Util.HighlightLine("  Please enter your reader number. Default(0).", "reader number");
                        Console.Write("  >>>> ");
                        config.actions[idx].buzzer.readerNumber = Util.GetInput((byte)0);

                        Util.HighlightLine("  Please enter a tone type (0: None, 1: Off, 2: On)", "tone");
                        Console.Write("  >>>> ");
                        config.actions[idx].buzzer.tone = Util.GetInput((byte)BS2OsdpStandardTone.ON);
                        Util.HighlightLine("  Please enter the buzzer turn-on time in 100 ms.", "on time");
                        Console.Write("  >>>> ");
                        config.actions[idx].buzzer.onTime = Util.GetInput((byte)2);
                        Util.HighlightLine("  Please enter the buzzer turn-off time in 100 ms.", "off time");
                        Console.Write("  >>>> ");
                        config.actions[idx].buzzer.offTime = Util.GetInput((byte)0);
                        Util.HighlightLine("  Please enter the number of cycle the buzzer on and off.", "number of cycle");
                        Console.Write("  >>>> ");
                        config.actions[idx].buzzer.numOfCycle = Util.GetInput((byte)1);
                    }
                }
            }

            Console.WriteLine("Trying to set OsdpStandardActionConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetOsdpStandardActionConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool addOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, UInt32 channelIndex, ref BS2OsdpStandardDeviceAdd osdpDevice, out UInt32 outChannelIndex)
        {
            Console.WriteLine("Trying to add OsdpStandardDevice");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_AddOsdpStandardDevice(sdkContext, deviceID, channelIndex, ref osdpDevice, out outChannelIndex);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool getOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, out BS2OsdpStandardDevice osdpDevice)
        {
            Console.WriteLine("Trying to get OsdpStandardDevice");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetOsdpStandardDevice(sdkContext, deviceID, out osdpDevice);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool getAvailableOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, out BS2OsdpStandardDeviceAvailable osdpDevices)
        {
            Console.WriteLine("Trying to get all OsdpStandardDevice");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAvailableOsdpStandardDevice(sdkContext, deviceID, out osdpDevices);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool updateOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, ref BS2OsdpStandardDeviceUpdate[] osdpDevices, ref List<BS2OsdpStandardDeviceResult> listResult)
        {
            Console.WriteLine("Trying to update OsdpStandardDevice");

            int structSize = Marshal.SizeOf(typeof(BS2OsdpStandardDeviceUpdate));
            IntPtr ptrArray = Marshal.AllocHGlobal(structSize * osdpDevices.Length);
            long ptrCurrent = ptrArray.ToInt64();
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            try
            {
                for (int idx = 0; idx < osdpDevices.Length; idx++)
                {
                    IntPtr ptrTemp = new IntPtr(ptrCurrent);
                    Marshal.StructureToPtr(osdpDevices[idx], ptrTemp, false);
                    ptrCurrent += structSize;
                }

                IntPtr outResultObj = IntPtr.Zero;
                UInt32 numOfResult = 0;
                result = (BS2ErrorCode)API.BS2_UpdateOsdpStandardDevice(sdkContext, deviceID, ptrArray, (UInt32)osdpDevices.Length, out outResultObj, out numOfResult);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else
                {
                    IntPtr curResult = outResultObj;
                    int resultSize = Marshal.SizeOf(typeof(BS2OsdpStandardDeviceResult));
                    for (UInt32 resultIdx = 0; resultIdx < numOfResult; resultIdx++)
                    {
                        BS2OsdpStandardDeviceResult item = (BS2OsdpStandardDeviceResult)Marshal.PtrToStructure(curResult, typeof(BS2OsdpStandardDeviceResult));
                        //print(ref item, resultIdx);
                        listResult.Add(item);
                        curResult += resultSize;
                    }

                    API.BS2_ReleaseObject(outResultObj);
                    Console.WriteLine("Call success.");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptrArray);
            }

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool removeOsdpStandardDevice(IntPtr sdkContext, UInt32 deviceID, UInt32[] osdpDeviceIDs, ref List<BS2OsdpStandardDeviceResult> listResult)
        {
            Console.WriteLine("Trying to remove OsdpStandardDevice");

            int structSize = Marshal.SizeOf(typeof(UInt32));
            IntPtr ptrArray = Marshal.AllocHGlobal(structSize * osdpDeviceIDs.Length);
            long ptrCurrent = ptrArray.ToInt64();
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            try
            {
                int[] tempIDs = Array.ConvertAll(osdpDeviceIDs, Convert.ToInt32);
                Marshal.Copy(tempIDs, 0, ptrArray, tempIDs.Length);

                IntPtr outResultObj = IntPtr.Zero;
                UInt32 numOfResult = 0;
                result = (BS2ErrorCode)API.BS2_RemoveOsdpStandardDevice(sdkContext, deviceID, ptrArray, (UInt32)tempIDs.Length, out outResultObj, out numOfResult);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else
                {
                    IntPtr curResult = outResultObj;
                    int resultSize = Marshal.SizeOf(typeof(BS2OsdpStandardDeviceResult));
                    for (UInt32 resultIdx = 0; resultIdx < numOfResult; resultIdx++)
                    {
                        BS2OsdpStandardDeviceResult item = (BS2OsdpStandardDeviceResult)Marshal.PtrToStructure(curResult, typeof(BS2OsdpStandardDeviceResult));
                        //print(ref item, resultIdx);
                        listResult.Add(item);
                        curResult += resultSize;
                    }

                    API.BS2_ReleaseObject(outResultObj);
                    Console.WriteLine("Call success.");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptrArray);
            }

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool getOsdpStandardDeviceCapability(IntPtr sdkContext, UInt32 deviceID, out BS2OsdpStandardDeviceCapability capability)
        {
            Console.WriteLine("Trying to get OsdpStandardDeviceCapability");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetOsdpStandardDeviceCapability(sdkContext, deviceID, out capability);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool setOsdpStandardDeviceSecurityKey(IntPtr sdkContext, UInt32 deviceID, IntPtr key)
        {
            Console.WriteLine("Trying to set OsdpStandardDeviceSecurityKey");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetOsdpStandardDeviceSecurityKey(sdkContext, deviceID, key);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static void print(ref BS2LicenseConfig config)
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

        public static void print(ref BS2OsdpStandardConfig config)
        {
            Console.WriteLine(">>>> OSDP standard configuration");
            Console.WriteLine("     |--numOfChannels : {0}", config.numOfChannels);

            for (int idx = 0; idx < config.numOfChannels; idx++)
            {
                Console.WriteLine("     +--channel[{0}] - mode : {1}", idx, config.mode[idx]);
                Console.WriteLine("         |--baudRate : {0}", config.channels[idx].baudRate);
                Console.WriteLine("         |--channelIndex : {0}", config.channels[idx].channelIndex);
                Console.WriteLine("         |--useRegistance : {0}", config.channels[idx].useRegistance);
                Console.WriteLine("         |--numOfDevices : {0}", config.channels[idx].numOfDevices);
                Console.WriteLine("         |--channelType : {0}", config.channels[idx].channelType);
                for (int sidx = 0; sidx < config.channels[idx].numOfDevices; sidx++)
                {
                    Console.WriteLine("         +--slaveDevice[{0}]", sidx);
                    Console.WriteLine("             |--deviceID : {0}", config.channels[idx].slaveDevices[sidx].deviceID);
                    Console.WriteLine("             |--deviceType : {0}", config.channels[idx].slaveDevices[sidx].deviceType);
                    Console.WriteLine("             |--enableOSDP : {0}", config.channels[idx].slaveDevices[sidx].enableOSDP);
                    Console.WriteLine("             |--connected : {0}", config.channels[idx].slaveDevices[sidx].connected);
                    Console.WriteLine("             |--channelInfo : {0}", config.channels[idx].slaveDevices[sidx].channelInfo);
                    Console.WriteLine("             |--osdpID : {0}", config.channels[idx].slaveDevices[sidx].osdpID);
                    Console.WriteLine("             |--supremaSearch : {0}", config.channels[idx].slaveDevices[sidx].supremaSearch);
                    Console.WriteLine("             |--activate : {0}", config.channels[idx].slaveDevices[sidx].activate);
                    Console.WriteLine("             |--useSecure : {0}", config.channels[idx].slaveDevices[sidx].useSecure);
                    Console.WriteLine("             |--vendorCode : {0}", BitConverter.ToString(config.channels[idx].slaveDevices[sidx].vendorCode));
                    Console.WriteLine("             |--fwVersion : {0}", config.channels[idx].slaveDevices[sidx].fwVersion);
                    Console.WriteLine("             |--modelNumber : {0}", config.channels[idx].slaveDevices[sidx].modelNumber);
                    Console.WriteLine("             |--modelVersion : {0}", config.channels[idx].slaveDevices[sidx].modelVersion);
                    Console.WriteLine("             |--readInfo : {0}", config.channels[idx].slaveDevices[sidx].readInfo);
                }
            }
        }

        public static void print(ref BS2OsdpStandardDevice device)
        {
            Console.WriteLine(">>>> OSDP standard device");
            Console.WriteLine("     |--deviceID : {0}", device.deviceID);
            Console.WriteLine("     |--deviceType : {0}", device.deviceType);
            Console.WriteLine("     |--enableOSDP : {0}", device.enableOSDP);
            Console.WriteLine("     |--connected : {0}", device.connected);
            Console.WriteLine("     |--channelInfo : {0}", device.channelInfo);
            Console.WriteLine("     |--osdpID : {0}", device.osdpID);
            Console.WriteLine("     |--supremaSearch : {0}", device.supremaSearch);
            Console.WriteLine("     |--activate : {0}", device.activate);
            Console.WriteLine("     |--useSecure : {0}", device.useSecure);
            Console.WriteLine("     |--vendorCode : {0}", BitConverter.ToString(device.vendorCode));
            Console.WriteLine("     |--fwVersion : {0}", device.fwVersion);
            Console.WriteLine("     |--modelNumber : {0}", device.modelNumber);
            Console.WriteLine("     |--modelVersion : {0}", device.modelVersion);
            Console.WriteLine("     |--readInfo : {0}", device.readInfo);
        }

        public static void print(ref BS2OsdpStandardDeviceNotify device)
        {
            Console.WriteLine("<<<< OSDP standard device changed");
            Console.WriteLine("     |--deviceID : {0}", device.deviceID);
            Console.WriteLine("     |--deviceType : {0}", device.deviceType);
            Console.WriteLine("     |--enableOSDP : {0}", device.enableOSDP);
            Console.WriteLine("     |--connected : {0}", device.connected);
            Console.WriteLine("     |--channelInfo : {0}", device.channelInfo);
            Console.WriteLine("     |--osdpID : {0}", device.osdpID);
            Console.WriteLine("     |--supremaSearch : {0}", device.supremaSearch);
            Console.WriteLine("     |--activate : {0}", device.activate);
            Console.WriteLine("     |--useSecure : {0}", device.useSecure);
            Console.WriteLine("     |--vendorCode : {0}", BitConverter.ToString(device.vendorCode));
            Console.WriteLine("     |--fwVersion : {0}", device.fwVersion);
            Console.WriteLine("     |--modelNumber : {0}", device.modelNumber);
            Console.WriteLine("     |--modelVersion : {0}", device.modelVersion);
            Console.WriteLine("     |--readInfo : {0}", device.readInfo);
        }

        public static void print(ref BS2OsdpStandardActionConfig config)
        {
            Console.WriteLine(">>>> OSDP standard action configuration");
            Console.WriteLine("     |--version : {0}", config.version);

            for (int idx = 0; idx < (int)BS2OsdpStandardActionType.COUNT; idx++)
            {
                Console.WriteLine("     +--actions[{0}]", idx);
                Console.WriteLine("         +--type : {0}", (BS2OsdpStandardActionType)config.actions[idx].actionType);
                for (int ledidx = 0; ledidx < BS2Environment.BS2_OSDP_STANDARD_ACTION_MAX_LED; ledidx++)
                {
                    Console.WriteLine("         +--led[{0}]", ledidx);
                    Console.WriteLine("             |--use : {0}", config.actions[idx].led[ledidx].use);
                    Console.WriteLine("             |--readerNumber : {0}", config.actions[idx].led[ledidx].readerNumber);
                    Console.WriteLine("             |--ledNumber : {0}", config.actions[idx].led[ledidx].ledNumber);

                    Console.WriteLine("             |--tempCommand : {0}", config.actions[idx].led[ledidx].tempCommand);
                    Console.WriteLine("             |--tempOnTime (*100): {0}", config.actions[idx].led[ledidx].tempOnTime);
                    Console.WriteLine("             |--tempOffTime (*100): {0}", config.actions[idx].led[ledidx].tempOffTime);
                    Console.WriteLine("             |--tempOnColor : {0}", config.actions[idx].led[ledidx].tempOnColor);
                    Console.WriteLine("             |--tempOffColor : {0}", config.actions[idx].led[ledidx].tempOffColor);
                    Console.WriteLine("             |--tempRunTime (*100): {0}", config.actions[idx].led[ledidx].tempRunTime);

                    Console.WriteLine("             |--permCommand : {0}", config.actions[idx].led[ledidx].permCommand);
                    Console.WriteLine("             |--permOnTime (*100): {0}", config.actions[idx].led[ledidx].permOnTime);
                    Console.WriteLine("             |--permOffTime (*100): {0}", config.actions[idx].led[ledidx].permOffTime);
                    Console.WriteLine("             |--permOnColor : {0}", config.actions[idx].led[ledidx].permOnColor);
                    Console.WriteLine("             |--permOffColor : {0}", config.actions[idx].led[ledidx].permOffColor);
                }

                Console.WriteLine("         +--buzzer");
                Console.WriteLine("             |--use : {0}", config.actions[idx].buzzer.use);
                Console.WriteLine("             |--readerNumber : {0}", config.actions[idx].buzzer.readerNumber);
                Console.WriteLine("             |--tone : {0}", config.actions[idx].buzzer.tone);
                Console.WriteLine("             |--onTime (*100): {0}", config.actions[idx].buzzer.onTime);
                Console.WriteLine("             |--offTime (*100): {0}", config.actions[idx].buzzer.offTime);
                Console.WriteLine("             |--numOfCycle : {0}", config.actions[idx].buzzer.numOfCycle);
            }
        }

        public static void print(ref BS2OsdpStandardDeviceAvailable devices)
        {
            Console.WriteLine(">>>> OSDP channels");
            Console.WriteLine("     |--numOfChannel : {0}", devices.numOfChannel);

            for (int idx = 0; idx < devices.numOfChannel; idx++)
            {
                Console.WriteLine("     +--channel[{0}]", idx);
                Console.WriteLine("         |--channelType : {0}", devices.channels[idx].channelType);
                Console.WriteLine("         |--channelIndex : {0}", devices.channels[idx].channelIndex);
                Console.WriteLine("         |--maxOsdpDevice : {0}", devices.channels[idx].maxOsdpDevice);
                Console.WriteLine("         |--numOsdpAvailableDevice : {0}", devices.channels[idx].numOsdpAvailableDevice);
                for (int didx = 0; didx < devices.channels[idx].numOsdpAvailableDevice; didx++)
                {
                    Console.WriteLine("         |--deviceIDs[{0}] : {1}", didx, devices.channels[idx].deviceIDs[didx]);
                }
            }
        }

        public static void print (ref BS2OsdpStandardDeviceCapability capability)
        {
            Console.WriteLine(">>>> OSDP standard device capability");
            Console.WriteLine("     +--input");
            Console.WriteLine("         |--compliance : {0}", capability.input.compliance);
            Console.WriteLine("         |--count : {0}",      capability.input.count);
            Console.WriteLine("     +--output");
            Console.WriteLine("         |--compliance : {0}", capability.output.compliance);
            Console.WriteLine("         |--count : {0}",      capability.output.count);
            Console.WriteLine("     +--led");
            Console.WriteLine("         |--compliance : {0}", capability.led.compliance);
            Console.WriteLine("         |--count : {0}",      capability.led.count);
            Console.WriteLine("     +--audio");
            Console.WriteLine("         |--compliance : {0}", capability.audio.compliance);
            Console.WriteLine("         |--count : {0}",      capability.audio.count);
            Console.WriteLine("     +--textOutput");
            Console.WriteLine("         |--compliance : {0}", capability.textOutput.compliance);
            Console.WriteLine("         |--count : {0}",      capability.textOutput.count);
            Console.WriteLine("     +--reader");
            Console.WriteLine("         |--compliance : {0}", capability.reader.compliance);
            Console.WriteLine("         |--count : {0}",      capability.reader.count);
            Console.WriteLine("     +--recvBufferSize : {0}", capability.recvBufferSize);
            Console.WriteLine("     +--largeMsgSize : {0}", capability.largeMsgSize);
            Console.WriteLine("     +--osdpVersion : {0}", capability.osdpVersion);
            Console.WriteLine("     +--cardFormat : {0}", capability.cardFormat);
            Console.WriteLine("     +--timeKeeping : {0}", capability.timeKeeping);
            Console.WriteLine("     +--canCommSecure : {0}", capability.canCommSecure);
            Console.WriteLine("     +--crcSupport : {0}", capability.crcSupport);
            Console.WriteLine("     +--smartCardSupport : {0}", capability.smartCardSupport);
            Console.WriteLine("     +--biometricSupport : {0}", capability.biometricSupport);
            Console.WriteLine("     +--securePinEntrySupport : {0}", capability.securePinEntrySupport);
        }

        public static void print(BS2OsdpStandardDeviceResult result, UInt32 index)
        {
            Console.WriteLine("<<<< OsdpStandard device result[{0}]", index);
            Console.WriteLine("     |--deviceID : {0}", result.deviceID);
            Console.WriteLine("     |--result : {0}", result.result);
        }

    }
}
