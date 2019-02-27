using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class UpgradeControl : FunctionModule
    {
        private const UInt32 MAX_PERCENT = 40;
        private API.OnProgressChanged cbOnProgressChanged = null;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Upgrade firmware", upgradeFirmware));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Upgrade language pack", upgradeLanguage));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Update background image", updateBackgroundImage));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Update notice message", updateNoticeMessage));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Update slide image", updateSlideImage));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Update sound resource", updateSoundResource));

            return functionList;
        }

        public void upgradeFirmware(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the path of firmware file which you want to upgrade.");
            Console.Write(">>>> ");
            string firmwarePath = Console.ReadLine();

            if (!File.Exists(firmwarePath))
            {
                Console.WriteLine("Invalid firmware path");
                return;
            }

            IntPtr firmwareData = IntPtr.Zero;
            UInt32 firmwareDataLen = 0;

            if(Util.LoadBinary(firmwarePath, out firmwareData, out firmwareDataLen))
            {
                Console.WriteLine("Trying to upgrade firmware.");
                cbOnProgressChanged = new API.OnProgressChanged(FirmwareProgressChanged);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_UpgradeFirmware(sdkContext, deviceID, firmwareData, firmwareDataLen, 0, cbOnProgressChanged);
                Marshal.FreeHGlobal(firmwareData);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                cbOnProgressChanged = null;
            }
        }

        public void upgradeLanguage(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the path of language pack which you want to upgrade.");
            Console.Write(">>>> ");
            string firmwarePath = Console.ReadLine();

            if (!File.Exists(firmwarePath))
            {
                Console.WriteLine("Invalid language pack path");
                return;
            }

            IntPtr languageData = IntPtr.Zero;
            UInt32 languageDataLen = 0;

            if (Util.LoadBinary(firmwarePath, out languageData, out languageDataLen))
            {
                BS2ResourceElement resourceElement = Util.AllocateStructure<BS2ResourceElement>();

                resourceElement.type = (byte)BS2ResourceTypeEnum.BS2_RESOURCE_TYPE_UI;
                resourceElement.numResData = 1;
                resourceElement.resData[0].index = 0;
                resourceElement.resData[0].data = languageData;
                resourceElement.resData[0].dataLen = languageDataLen;
                cbOnProgressChanged = new API.OnProgressChanged(LanguageProgressChanged);

                Console.WriteLine("Trying to upgrade language pack.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_UpdateResource(sdkContext, deviceID, ref resourceElement, 0, cbOnProgressChanged);
                Marshal.FreeHGlobal(languageData);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                cbOnProgressChanged = null;
            }
        }

        public void updateBackgroundImage(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the path of background image(*.png) which you want to update.");
            Console.Write(">>>> ");
            string firmwarePath = Console.ReadLine();

            if (!File.Exists(firmwarePath))
            {
                Console.WriteLine("Invalid image file path");
                return;
            }

            IntPtr imageData = IntPtr.Zero;
            UInt32 imageDataLen = 0;

            if (Util.LoadBinary(firmwarePath, out imageData, out imageDataLen))
            {
                BS2ResourceElement resourceElement = Util.AllocateStructure<BS2ResourceElement>();

                resourceElement.type = (byte)BS2ResourceTypeEnum.BS2_RESOURCE_TYPE_IMAGE;
                resourceElement.numResData = 1;
                resourceElement.resData[0].index = 0;
                resourceElement.resData[0].data = imageData;
                resourceElement.resData[0].dataLen = imageDataLen;
                cbOnProgressChanged = new API.OnProgressChanged(GeneralProgressChanged);

                Console.WriteLine("Trying to update background image.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_UpdateResource(sdkContext, deviceID, ref resourceElement, 0, cbOnProgressChanged);
                Marshal.FreeHGlobal(imageData);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else
                {
                    Console.WriteLine("Do you need to confirm background image? [Y/n]");
                    Console.Write(">>>> ");
                    if (Util.IsYes())
                    {
                        BS2DisplayConfig displayConfig;

                        Console.WriteLine("Trying to get display configuration.");
                        result = (BS2ErrorCode)API.BS2_GetDisplayConfig(sdkContext, deviceID, out displayConfig);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                        }
                        else if((BS2BackgroundEnum)displayConfig.background != BS2BackgroundEnum.LOGO || 
                                (BS2BGThemeEnum)displayConfig.bgTheme != BS2BGThemeEnum.THEME3)
                        {
                            displayConfig.background = (byte)BS2BackgroundEnum.LOGO;
                            displayConfig.bgTheme = (byte)BS2BGThemeEnum.THEME3;
                            Console.WriteLine("Trying to set display configuration.");
                            result = (BS2ErrorCode)API.BS2_SetDisplayConfig(sdkContext, deviceID, ref displayConfig);

                            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                            {
                                Console.WriteLine("Got error({0}).", result);
                            }
                        }
                    }
                }

                cbOnProgressChanged = null;
            }
        }

        public void updateNoticeMessage(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the notice message which you want to update.");
            Console.Write(">>>> ");
            string noticeMessage = Console.ReadLine();

            if (noticeMessage.Length == 0)
            {
                Console.WriteLine("Empty notice message");
                return;
            }

            byte[] noticeMessageArray = Encoding.UTF8.GetBytes(noticeMessage);
            BS2ResourceElement resourceElement = Util.AllocateStructure<BS2ResourceElement>();

            resourceElement.type = (byte)BS2ResourceTypeEnum.BS2_RESOURCE_TYPE_NOTICE;
            resourceElement.numResData = 1;
            resourceElement.resData[0].index = 0;
            resourceElement.resData[0].data = Marshal.AllocHGlobal(noticeMessageArray.Length);
            resourceElement.resData[0].dataLen = (UInt32)noticeMessageArray.Length;
            Marshal.Copy(noticeMessageArray, 0, resourceElement.resData[0].data, noticeMessageArray.Length);
            cbOnProgressChanged = new API.OnProgressChanged(GeneralProgressChanged);

            Console.WriteLine("Trying to update notice message.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_UpdateResource(sdkContext, deviceID, ref resourceElement, 0, cbOnProgressChanged);
            Marshal.FreeHGlobal(resourceElement.resData[0].data);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                Console.WriteLine("Do you need to confirm notice message? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    BS2DisplayConfig displayConfig;

                    Console.WriteLine("Trying to get display configuration.");
                    result = (BS2ErrorCode)API.BS2_GetDisplayConfig(sdkContext, deviceID, out displayConfig);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Got error({0}).", result);
                    }
                    else if ((BS2BackgroundEnum)displayConfig.background != BS2BackgroundEnum.NOTICE ||
                            (BS2BGThemeEnum)displayConfig.bgTheme != BS2BGThemeEnum.THEME2)
                    {
                        displayConfig.background = (byte)BS2BackgroundEnum.NOTICE;
                        displayConfig.bgTheme = (byte)BS2BGThemeEnum.THEME2;
                        Console.WriteLine("Trying to set display configuration.");
                        result = (BS2ErrorCode)API.BS2_SetDisplayConfig(sdkContext, deviceID, ref displayConfig);

                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                        }
                    }
                }
            }

            cbOnProgressChanged = null;
        }

        public void updateSlideImage(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ResourceElement resourceElement = Util.AllocateStructure<BS2ResourceElement>();

            resourceElement.type = (byte)BS2ResourceTypeEnum.BS2_RESOURCE_TYPE_SLIDE;
            Console.WriteLine("How many slide images do you want to set? [1(default)-10]");
            Console.Write(">>>> ");
            resourceElement.numResData = Util.GetInput(1);

            for (UInt32 idx = 0; idx < resourceElement.numResData; ++idx)
            {
                Console.WriteLine("Enter a value for slide image[{0}]", idx);
                resourceElement.resData[idx].index = (byte)idx;
                Console.WriteLine("  Enter the path of slide image(*.png) which you want to update.");
                Console.Write("  >>>> ");
                string pngFilePath = Console.ReadLine();
                if (!File.Exists(pngFilePath))
                {
                    Console.WriteLine("Invalid image file path");
                    return;
                }

                IntPtr pngData = IntPtr.Zero;
                UInt32 pngDataLen = 0;

                if (Util.LoadBinary(pngFilePath, out pngData, out pngDataLen))
                {
                    resourceElement.resData[idx].data = pngData;
                    resourceElement.resData[idx].dataLen = pngDataLen;
                }
                else
                {
                    return;
                }
            }

            Console.WriteLine("Trying to update slide image.");
            cbOnProgressChanged = new API.OnProgressChanged(GeneralProgressChanged);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_UpdateResource(sdkContext, deviceID, ref resourceElement, 0, cbOnProgressChanged);

            for (UInt32 idx = 0; idx < resourceElement.numResData; ++idx)
            {
                Marshal.FreeHGlobal(resourceElement.resData[idx].data);
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                Console.WriteLine("Do you need to confirm slide image? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    BS2DisplayConfig displayConfig;

                    Console.WriteLine("Trying to get display configuration.");
                    result = (BS2ErrorCode)API.BS2_GetDisplayConfig(sdkContext, deviceID, out displayConfig);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Got error({0}).", result);
                    }
                    else if ((BS2BackgroundEnum)displayConfig.background != BS2BackgroundEnum.SLIDE ||
                            (BS2BGThemeEnum)displayConfig.bgTheme != BS2BGThemeEnum.THEME3)
                    {
                        displayConfig.background = (byte)BS2BackgroundEnum.SLIDE;
                        displayConfig.bgTheme = (byte)BS2BGThemeEnum.THEME3;
                        Console.WriteLine("Trying to set display configuration.");
                        result = (BS2ErrorCode)API.BS2_SetDisplayConfig(sdkContext, deviceID, ref displayConfig);

                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            Console.WriteLine("Got error({0}).", result);
                        }
                    }
                }
            }

            cbOnProgressChanged = null;
        }

        public void updateSoundResource(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ResourceElement resourceElement = Util.AllocateStructure<BS2ResourceElement>();

            resourceElement.type = (byte)BS2ResourceTypeEnum.BS2_RESOURCE_TYPE_SOUND;
            Console.WriteLine("How many sound resource do you want to set? [1(default)-3]");
            Console.Write(">>>> ");
            resourceElement.numResData = Util.GetInput(1);            

            for (UInt32 idx = 0; idx < resourceElement.numResData; ++idx)
            {
                Console.WriteLine("Enter a value for sound resource[{0}]", idx);
                Console.WriteLine("  Enter index of the sound resource which you want to set [{0} - {1}]", BS2SoundIndexEnum.WELCOME, BS2SoundIndexEnum.AUTH_FAIL);
                Console.Write("  >>>> ");
                resourceElement.resData[idx].index = (byte)Util.GetInput();
                Console.WriteLine("  Enter the path of wave file(*.wav) which you want to update.");
                Console.Write("  >>>> ");
                string waveFilePath = Console.ReadLine();
                if (!File.Exists(waveFilePath))
                {
                    Console.WriteLine("Invalid image file path");
                    return;
                }

                IntPtr waveData = IntPtr.Zero;
                UInt32 waveDataLen = 0;

                if (Util.LoadBinary(waveFilePath, out waveData, out waveDataLen))
                {
                    resourceElement.resData[idx].data = waveData;
                    resourceElement.resData[idx].dataLen = waveDataLen;
                }
                else
                {
                    return;
                }
            }

            Console.WriteLine("Trying to update sound resource.");
            cbOnProgressChanged = new API.OnProgressChanged(GeneralProgressChanged);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_UpdateResource(sdkContext, deviceID, ref resourceElement, 0, cbOnProgressChanged);

            for (UInt32 idx = 0; idx < resourceElement.numResData; ++idx)
            {
                Marshal.FreeHGlobal(resourceElement.resData[idx].data);
            }

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            cbOnProgressChanged = null;
        }              

        void FirmwareProgressChanged(UInt32 deviceId, UInt32 progressPercentage)
        {
            UInt32 curProgress = progressPercentage*MAX_PERCENT/100;

            Console.Write("\r>>>> [");
            for (UInt32 idx = 0; idx < curProgress; ++idx)
            {
                Console.Write("#");
            }

            for (UInt32 idx = curProgress; idx < MAX_PERCENT; ++idx)
            {
                Console.Write(" ");
            }

            Console.Write("] {0, 3}%", progressPercentage);

            if (progressPercentage < 99)
            {
                Console.Write(" Firmware downloading");
            }
            else if (progressPercentage == 99)
            {
                Console.Write(" Firmware flashing    ");
            }
            else
            {
                Console.WriteLine(" Done                                       ");
                Console.WriteLine(">>>> Your device[{0}] has been upgraded.", deviceId);
                Console.WriteLine(">>>> The device[{0}] will restart.", deviceId);
            }
        }

        void LanguageProgressChanged(UInt32 deviceId, UInt32 progressPercentage)
        {
            UInt32 curProgress = progressPercentage * MAX_PERCENT / 100;

            Console.Write("\r>>>> [");
            for (UInt32 idx = 0; idx < curProgress; ++idx)
            {
                Console.Write("#");
            }

            for (UInt32 idx = curProgress; idx < MAX_PERCENT; ++idx)
            {
                Console.Write(" ");
            }

            Console.Write("] {0, 3}%", progressPercentage);

            if (progressPercentage < 99)
            {
                Console.Write(" Language pack downloading");
            }
            else if (progressPercentage == 99)
            {
                Console.Write(" Language pack flashing    ");
            }
            else
            {
                Console.WriteLine(" Done                                       ");
                Console.WriteLine(">>>> The device[{0}] will restart.", deviceId);
            }
        }

        void GeneralProgressChanged(UInt32 deviceId, UInt32 progressPercentage)
        {
            UInt32 curProgress = progressPercentage * MAX_PERCENT / 100;

            Console.Write("\r>>>> [");
            for (UInt32 idx = 0; idx < curProgress; ++idx)
            {
                Console.Write("#");
            }

            for (UInt32 idx = curProgress; idx < MAX_PERCENT; ++idx)
            {
                Console.Write(" ");
            }

            Console.Write("] {0, 3}%", progressPercentage);

            if (progressPercentage == 100)
            {
                Console.WriteLine();
            }
        }
    }
}
