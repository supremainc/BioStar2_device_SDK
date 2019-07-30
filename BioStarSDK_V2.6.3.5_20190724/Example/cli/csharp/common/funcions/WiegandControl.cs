using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class WiegandControl : FunctionModule
    {
        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Search wiegand device", searchWiegandDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get wiegand device", getWiegandDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Add wiegand device", addWiegandDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove wiegand device", removeWiegandDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set predefined wiegand formats", setPredefinedWiegandFormats));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set wiegand card mask", setWiegandConfig));
            
            return functionList;
        }

        public void searchWiegandDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr wiegandDeviceObj = IntPtr.Zero;
            UInt32 numWiegandDevice = 0;

            Console.WriteLine("Trying to search the wiegand devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SearchWiegandDevices(sdkContext, deviceID, out wiegandDeviceObj, out numWiegandDevice);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numWiegandDevice > 0)
            {
                for (int idx = 0; idx < numWiegandDevice; ++idx)
                {
                    UInt32 wiegandDeviceID = Convert.ToUInt32(Marshal.ReadInt32(wiegandDeviceObj, (int)idx * sizeof(UInt32)));
                    Console.WriteLine(">>>> WiegandDevice id[{0, 10}]", wiegandDeviceID);
                }

                API.BS2_ReleaseObject(wiegandDeviceObj);
            }
            else
            {
                Console.WriteLine(">>> There is no wiegand device in the device.");
            }
        }

        public void getWiegandDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr wiegandDeviceObj = IntPtr.Zero;
            UInt32 numWiegandDevice = 0;

            Console.WriteLine("Trying to get the wiegand devices.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetWiegandDevices(sdkContext, deviceID, out wiegandDeviceObj, out numWiegandDevice);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numWiegandDevice > 0)
            {
                for (int idx = 0; idx < numWiegandDevice; ++idx)
                {
                    UInt32 wiegandDeviceID = Convert.ToUInt32(Marshal.ReadInt32(wiegandDeviceObj, (int)idx * sizeof(UInt32)));
                    Console.WriteLine(">>>> WiegandDevice id[{0, 10}]", wiegandDeviceID);
                }

                API.BS2_ReleaseObject(wiegandDeviceObj);
            }
            else
            {
                Console.WriteLine(">>> There is no wiegand device in the device.");
            }
        }

        public void addWiegandDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the wiegand device which you want to add: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] wiegandDeviceIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> wiegandDeviceIDList = new List<UInt32>();

            foreach (string holidayGroupID in wiegandDeviceIDs)
            {
                if (holidayGroupID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(holidayGroupID, out item))
                    {
                        wiegandDeviceIDList.Add(item);
                    }
                }
            }

            if (wiegandDeviceIDList.Count > 0)
            {
                IntPtr wiegandDeviceIDObj = Marshal.AllocHGlobal(sizeof(UInt32) * wiegandDeviceIDList.Count);
                for (int idx = 0; idx < wiegandDeviceIDList.Count; ++idx)
                {
                    Marshal.WriteInt32(wiegandDeviceIDObj, idx * sizeof(UInt32), (int)wiegandDeviceIDList[idx]);
                }

                Console.WriteLine("Trying to add the wiegand devices.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_AddWiegandDevices(sdkContext, deviceID, wiegandDeviceIDObj, (UInt32)wiegandDeviceIDList.Count);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(wiegandDeviceIDObj);
            }
        }

        public void removeWiegandDevice(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the wiegand device which you want to remove: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] wiegandDeviceIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> wiegandDeviceIDList = new List<UInt32>();

            foreach (string holidayGroupID in wiegandDeviceIDs)
            {
                if (holidayGroupID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(holidayGroupID, out item))
                    {
                        wiegandDeviceIDList.Add(item);
                    }
                }
            }

            if (wiegandDeviceIDList.Count > 0)
            {
                IntPtr wiegandDeviceIDObj = Marshal.AllocHGlobal(sizeof(UInt32) * wiegandDeviceIDList.Count);
                for (int idx = 0; idx < wiegandDeviceIDList.Count; ++idx)
                {
                    Marshal.WriteInt32(wiegandDeviceIDObj, idx * sizeof(UInt32), (int)wiegandDeviceIDList[idx]);
                }

                Console.WriteLine("Trying to remove the wiegand devices.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_RemoveWiegandDevices(sdkContext, deviceID, wiegandDeviceIDObj, (UInt32)wiegandDeviceIDList.Count);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(wiegandDeviceIDObj);
            }
        }

        public void setPredefinedWiegandFormats(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Do you want to set the predefined wiegand formats to the device?[Y/N]");
            Console.Write(">>>> ");
            if (!Util.IsYes())
            {
                return;
            }

            BS2WiegandMultiConfig configWiegandMulti = Util.AllocateStructure<BS2WiegandMultiConfig>();

            /*=======================H10301 26 bit format=======================*/
            configWiegandMulti.formats[0].formatID = 1;
            configWiegandMulti.formats[0].format.length = 26;
            configWiegandMulti.formats[0].format.idFields = new byte[128];
            configWiegandMulti.formats[0].format.parityFields = new byte[128];
            configWiegandMulti.formats[0].format.parityType = new byte[4];
            configWiegandMulti.formats[0].format.parityPos = new byte[4];

            configWiegandMulti.formats[0].format.idFields[28] = 0x01;
            configWiegandMulti.formats[0].format.idFields[29] = 0xFE;
            configWiegandMulti.formats[0].format.idFields[61] = 0x01;
            configWiegandMulti.formats[0].format.idFields[62] = 0xFF;
            configWiegandMulti.formats[0].format.idFields[63] = 0xFE;

            configWiegandMulti.formats[0].format.parityType[0] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_EVEN;
            configWiegandMulti.formats[0].format.parityType[1] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_ODD;

            configWiegandMulti.formats[0].format.parityPos[0] = 0;
            configWiegandMulti.formats[0].format.parityPos[1] = 25;

            configWiegandMulti.formats[0].format.parityFields[28] = 0x01;
            configWiegandMulti.formats[0].format.parityFields[29] = 0xFF;
            configWiegandMulti.formats[0].format.parityFields[30] = 0xE0;

            configWiegandMulti.formats[0].format.parityFields[62] = 0x1F;
            configWiegandMulti.formats[0].format.parityFields[63] = 0xFE;
            


            /*=======================H10302 37 bit format=======================*/
            configWiegandMulti.formats[1].formatID = 2;
            configWiegandMulti.formats[1].format.length = 37;                            //Set the length of the format. H10302 is a 37 bit format
            configWiegandMulti.formats[1].format.idFields = new byte[128];
            configWiegandMulti.formats[1].format.parityFields = new byte[128];
            configWiegandMulti.formats[1].format.parityType = new byte[4];
            configWiegandMulti.formats[1].format.parityPos = new byte[4];

            // H10302 uses only 1 field. The first field is from [0] ~ [31]
            // If you convert the bits used on the field in binary, it is as below. 35 bit as card ID.
            // 0000 1111 / 1111 1111 / 1111 1111 / 1111 1111 / 1111 1110        -> 0F / FF / FF / FF / FE
            //     27    /     28    /     29    /    30     /    31
            configWiegandMulti.formats[1].format.idFields[27] = 0x0F;
            configWiegandMulti.formats[1].format.idFields[28] = 0xFF;
            configWiegandMulti.formats[1].format.idFields[29] = 0xFF;
            configWiegandMulti.formats[1].format.idFields[30] = 0xFF;
            configWiegandMulti.formats[1].format.idFields[31] = 0xFE;


            configWiegandMulti.formats[1].format.parityType[0] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_EVEN;                        // The first parity bit is an even parity bit.
            configWiegandMulti.formats[1].format.parityType[1] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_ODD;                         // The second parity bit is an odd parity bit.

            configWiegandMulti.formats[1].format.parityPos[0] = 0;                         // The first parity bit's position is the first bit. So, it will be 0.
            configWiegandMulti.formats[1].format.parityPos[1] = 36;                        // The second parity bit's position is the last bit. So, it will be 36.


            // According to H10302, the first even parity calculates the bits starting from 1 ~ 18
            // 000 0<parity bit 1111 / 1111 1111 / 1111 1100                -> 0F / FF / FC
            //           27         /     28    /     29
            configWiegandMulti.formats[1].format.parityFields[27] = 0x0F;
            configWiegandMulti.formats[1].format.parityFields[28] = 0xFF;
            configWiegandMulti.formats[1].format.parityFields[29] = 0xFC;

            // The second parity calculates the bits starting from 18 ~ 35. Since this is for the second parity bit,
            // parityFields[32] ~ [63] is used.
            // 0000 0111 / 1111 1111 / 1111 111 0<parity bit
            //     61    /     62    /     63
            configWiegandMulti.formats[1].format.parityFields[61] = 0x07;
            configWiegandMulti.formats[1].format.parityFields[62] = 0xFF;
            configWiegandMulti.formats[1].format.parityFields[63] = 0xFE;


            
            /*=======================H10304 37 bit format=======================*/
            configWiegandMulti.formats[2].formatID = 3;
            configWiegandMulti.formats[2].format.length = 37;
            configWiegandMulti.formats[2].format.idFields = new byte[128];
            configWiegandMulti.formats[2].format.parityFields = new byte[128];
            configWiegandMulti.formats[2].format.parityType = new byte[4];
            configWiegandMulti.formats[2].format.parityPos = new byte[4];

            configWiegandMulti.formats[2].format.idFields[59] = 0x0F;
            configWiegandMulti.formats[2].format.idFields[60] = 0xFF;
            configWiegandMulti.formats[2].format.idFields[61] = 0xF0;

            configWiegandMulti.formats[2].format.idFields[29] = 0x0F;
            configWiegandMulti.formats[2].format.idFields[30] = 0xFF;
            configWiegandMulti.formats[2].format.idFields[31] = 0xFE;

            configWiegandMulti.formats[2].format.parityType[0] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_EVEN;
            configWiegandMulti.formats[2].format.parityType[1] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_ODD;

            configWiegandMulti.formats[2].format.parityPos[0] = 0;
            configWiegandMulti.formats[2].format.parityPos[1] = 36;

            configWiegandMulti.formats[2].format.parityFields[27] = 0x0F;
            configWiegandMulti.formats[2].format.parityFields[28] = 0xFF;
            configWiegandMulti.formats[2].format.parityFields[29] = 0xFC;
 
            configWiegandMulti.formats[2].format.parityFields[61] = 0x07;
            configWiegandMulti.formats[2].format.parityFields[62] = 0xFF;
            configWiegandMulti.formats[2].format.parityFields[63] = 0xFE;


            /*=======================Corporate 1000 35 bit format=======================*/
            configWiegandMulti.formats[3].formatID = 4;
            configWiegandMulti.formats[3].format.length = 35;
            configWiegandMulti.formats[3].format.idFields = new byte[128];
            configWiegandMulti.formats[3].format.parityFields = new byte[128];
            configWiegandMulti.formats[3].format.parityType = new byte[4];
            configWiegandMulti.formats[3].format.parityPos = new byte[4];

            configWiegandMulti.formats[3].format.idFields[27] = 0x01;
            configWiegandMulti.formats[3].format.idFields[28] = 0xFF;
            configWiegandMulti.formats[3].format.idFields[29] = 0xE0;

            configWiegandMulti.formats[3].format.idFields[61] = 0x1F;
            configWiegandMulti.formats[3].format.idFields[62] = 0xFF;
            configWiegandMulti.formats[3].format.idFields[63] = 0xFE;

            configWiegandMulti.formats[3].format.parityType[0] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_EVEN;
            configWiegandMulti.formats[3].format.parityType[1] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_ODD;
            configWiegandMulti.formats[3].format.parityType[2] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_ODD;

            configWiegandMulti.formats[3].format.parityPos[0] = 1;
            configWiegandMulti.formats[3].format.parityPos[1] = 34;
            configWiegandMulti.formats[3].format.parityPos[2] = 0;

            configWiegandMulti.formats[3].format.parityFields[27] = 0x01;
            configWiegandMulti.formats[3].format.parityFields[28] = 0xB6;
            configWiegandMulti.formats[3].format.parityFields[29] = 0xDB;
            configWiegandMulti.formats[3].format.parityFields[30] = 0x6D;
            configWiegandMulti.formats[3].format.parityFields[31] = 0xB6;

            configWiegandMulti.formats[3].format.parityFields[59] = 0x03;
            configWiegandMulti.formats[3].format.parityFields[60] = 0x6D;
            configWiegandMulti.formats[3].format.parityFields[61] = 0xB6;
            configWiegandMulti.formats[3].format.parityFields[62] = 0xDB;
            configWiegandMulti.formats[3].format.parityFields[63] = 0x6C;

            configWiegandMulti.formats[3].format.parityFields[91] = 0x03;
            configWiegandMulti.formats[3].format.parityFields[92] = 0xFF;
            configWiegandMulti.formats[3].format.parityFields[93] = 0xFF;
            configWiegandMulti.formats[3].format.parityFields[94] = 0xFF;
            configWiegandMulti.formats[3].format.parityFields[95] = 0xFF;



            /*=======================Corporate 1000 48 bit format=======================*/
            configWiegandMulti.formats[4].formatID = 5;
            configWiegandMulti.formats[4].format.length = 48;
            configWiegandMulti.formats[4].format.idFields = new byte[128];
            configWiegandMulti.formats[4].format.parityFields = new byte[128];
            configWiegandMulti.formats[4].format.parityType = new byte[4];
            configWiegandMulti.formats[4].format.parityPos = new byte[4];

            configWiegandMulti.formats[4].format.idFields[26] = 0x3F;
            configWiegandMulti.formats[4].format.idFields[27] = 0xFF;
            configWiegandMulti.formats[4].format.idFields[28] = 0xFF;

            configWiegandMulti.formats[4].format.idFields[61] = 0xFF;
            configWiegandMulti.formats[4].format.idFields[62] = 0xFF;
            configWiegandMulti.formats[4].format.idFields[63] = 0xFE;

            configWiegandMulti.formats[4].format.parityType[0] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_EVEN;
            configWiegandMulti.formats[4].format.parityType[1] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_ODD;
            configWiegandMulti.formats[4].format.parityType[2] = (byte)BS2ParityTypeEnum.BS2_WIEGAND_PARITY_ODD;

            configWiegandMulti.formats[4].format.parityPos[0] = 1;
            configWiegandMulti.formats[4].format.parityPos[1] = 47;
            configWiegandMulti.formats[4].format.parityPos[2] = 0;

            configWiegandMulti.formats[4].format.parityFields[26] = 0x1B;
            configWiegandMulti.formats[4].format.parityFields[27] = 0x6D;
            configWiegandMulti.formats[4].format.parityFields[28] = 0xB6;
            configWiegandMulti.formats[4].format.parityFields[29] = 0xDB;
            configWiegandMulti.formats[4].format.parityFields[30] = 0x6D;
            configWiegandMulti.formats[4].format.parityFields[31] = 0xB6;

            configWiegandMulti.formats[4].format.parityFields[58] = 0x36;
            configWiegandMulti.formats[4].format.parityFields[59] = 0xDB;
            configWiegandMulti.formats[4].format.parityFields[60] = 0x6D;
            configWiegandMulti.formats[4].format.parityFields[61] = 0xB6;
            configWiegandMulti.formats[4].format.parityFields[62] = 0xDB;
            configWiegandMulti.formats[4].format.parityFields[63] = 0x6C;

            configWiegandMulti.formats[4].format.parityFields[90] = 0x7F;
            configWiegandMulti.formats[4].format.parityFields[91] = 0xFF;
            configWiegandMulti.formats[4].format.parityFields[92] = 0xFF;
            configWiegandMulti.formats[4].format.parityFields[93] = 0xFF;
            configWiegandMulti.formats[4].format.parityFields[94] = 0xFF;
            configWiegandMulti.formats[4].format.parityFields[95] = 0xFF;

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetWiegandMultiConfig(sdkContext, deviceID, ref configWiegandMulti);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            
        }

        public void setWiegandConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Please run menu #5 \"Set predefined wiegand formats\" before running this menu.");
            BS2WiegandConfig wiegandConfig;

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetWiegandConfig(sdkContext, deviceID, out wiegandConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
            Console.WriteLine("Select the wiegand mode for the device[0({0}:Default), 1({1}), 2({2} Supported only for BS2, BSA2, FS2)]", BS2WiegandModeEnum.BS2_WIEGAND_IN_ONLY,
                                                                                                                                          BS2WiegandModeEnum.BS2_WIEGAND_OUT_ONLY,
                                                                                                                                          BS2WiegandModeEnum.BS2_WIEGAND_IN_OUT);
            Console.Write("  >>>> ");
            wiegandConfig.mode = (byte)Util.GetInput();

            if (wiegandConfig.mode == (byte)BS2WiegandModeEnum.BS2_WIEGAND_OUT_ONLY || wiegandConfig.mode == (byte)BS2WiegandModeEnum.BS2_WIEGAND_IN_OUT)
            {
                Console.WriteLine("   Do you want the device to work as wiegand bypass?[Y/N]");
                Console.Write("     >>>> ");
                if(Util.IsYes())
                {
                    wiegandConfig.useWiegandBypass = 1;
                }
                else
                {
                    Console.WriteLine("   Do you want the device to use fail code?[Y/N]");
                    Console.Write("     >>>> ");
                    if(Util.IsYes())
                    {
                        wiegandConfig.useFailCode = 1;
                        Console.WriteLine("   Select the fail code type[0(0x00), 1(0xFF:Default)]");
                        byte failCodeType = (byte)Util.GetInput(1);
                        if(failCodeType == 0)
                        {
                            wiegandConfig.failCode = 0x00;
                        }
                        else
                        {
                            wiegandConfig.failCode = 0xFF;
                        }
                    }
                }
            }

            if (wiegandConfig.mode == (byte)BS2WiegandModeEnum.BS2_WIEGAND_IN_ONLY || wiegandConfig.mode == (byte)BS2WiegandModeEnum.BS2_WIEGAND_IN_OUT)
            {
                Console.WriteLine("   Please set the Wiegand Card mask for the WIEGAND INPUT. The device will accept WIEGNAD SIGNALS that matches the configured formats.");
                wiegandConfig.wiegandInputMask = InputWiegandFormatType();
                
            }

            Console.WriteLine("   Do you want the device to proccess CSN cards with wiegand formats?[Y/N]");
            Console.Write("     >>>> ");
            if (Util.IsYes())
            {
                BS2CardConfig cardConfig;
                result = (BS2ErrorCode)API.BS2_GetCardConfig(sdkContext, deviceID, out cardConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }

                cardConfig.useWiegandFormat = 1;

                result = (BS2ErrorCode)API.BS2_SetCardConfig(sdkContext, deviceID, ref cardConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }

                Console.WriteLine("   Please set the CSN Wiegand Card index for the device.");
                Console.WriteLine("   Select a format type. [{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}]",
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_H10301_26, BS2WiegandFormatEnum.BS2_WIEGAND_H10301_26,
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_H10302_37, BS2WiegandFormatEnum.BS2_WIEGAND_H10302_37,
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_H10304_37, BS2WiegandFormatEnum.BS2_WIEGAND_H10304_37,
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_C1000_35, BS2WiegandFormatEnum.BS2_WIEGAND_C1000_35,
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_C1000_48, BS2WiegandFormatEnum.BS2_WIEGAND_C1000_48);
                wiegandConfig.wiegandCSNIndex = (byte)(Util.GetInput() + 1);
            }
            else
            {
                BS2CardConfig cardConfig;
                result = (BS2ErrorCode)API.BS2_GetCardConfig(sdkContext, deviceID, out cardConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }

                cardConfig.useWiegandFormat = 0;

                result = (BS2ErrorCode)API.BS2_SetCardConfig(sdkContext, deviceID, ref cardConfig);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }
            }

            Console.WriteLine("   Please set the Wiegand Card mask for the DEVICE. The device will accept CARDS that matches the configured formats.");
            wiegandConfig.wiegandCardMask = InputWiegandFormatType();

            Console.WriteLine("   Enter the pulse width(µs) for the wiegand signal.(20(Min) ~ 100(Max) 40[Default]");
            Console.Write("     >>>> ");
            wiegandConfig.outPulseWidth = Util.GetInput(40);

            Console.WriteLine("   Enter the pulse interval(µs) for the wiegand signal.(200(Min) ~ 20000(Max) 10000[Default]");
            Console.Write("     >>>> ");
            wiegandConfig.outPulseInterval = Util.GetInput(10000);
            
            result = (BS2ErrorCode)API.BS2_SetWiegandConfig(sdkContext, deviceID, ref wiegandConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }
        }

        public ushort InputWiegandFormatType()
        {
            int formatType = 0;
            Console.WriteLine("   Select the format types to configure. [{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}] [Type1, Type2 ...]",
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_H10301_26, BS2WiegandFormatEnum.BS2_WIEGAND_H10301_26,
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_H10302_37, BS2WiegandFormatEnum.BS2_WIEGAND_H10302_37,
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_H10304_37, BS2WiegandFormatEnum.BS2_WIEGAND_H10304_37,
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_C1000_35, BS2WiegandFormatEnum.BS2_WIEGAND_C1000_35,
                (ushort)BS2WiegandFormatEnum.BS2_WIEGAND_C1000_48, BS2WiegandFormatEnum.BS2_WIEGAND_C1000_48);
            Console.Write("     >>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] strFormatTypes = Console.ReadLine().Split(delimiterChars);
            foreach (string strFormatType in strFormatTypes)
            {
                int item;
                if (int.TryParse(strFormatType, out item))
                {
                    formatType |= ((int)0x01 << item);
                }
            }
            formatType = formatType << 0x01; //0th bit is not used.

            return (ushort)formatType;
        }
    }
}
