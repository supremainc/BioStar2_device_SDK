using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class CardControl : FunctionModule
    {
        private API.OnReadyToScan cbCardOnReadyToScan = null;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get card configuration", getCardConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set card configuration", setCardConfig));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Scan card", scanCard));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Write card", writeCard));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Erase card", eraseCard));

            if (isMasterDevice)
            {
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get blacklist", getBlacklist));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove blacklist", removeBlacklist));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set blacklist", setBlacklist));
            }

            return functionList;
        }

        public void getCardConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2CardConfig cardConfig;

            Console.WriteLine("Trying to get card configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetCardConfig(sdkContext, deviceID, out cardConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(sdkContext, cardConfig);
            }
        }

        public void setCardConfig(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2FactoryConfig factoryConfig;

            Console.WriteLine("Trying to get factory config");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetFactoryConfig(sdkContext, deviceID, out factoryConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            UInt16 cardModel = 0;
            IntPtr ptrModel = Marshal.StringToHGlobalAnsi(Encoding.UTF8.GetString(factoryConfig.modelName).TrimEnd('\0'));
            result = (BS2ErrorCode)API.BS2_GetCardModel(ptrModel, out cardModel);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            BS2CardConfig cardConfig = Util.AllocateStructure<BS2CardConfig>();            

            Console.WriteLine("Choose byte order: [0: MSB(default), 1: LSB]");
            Console.Write(">>>> ");
            cardConfig.byteOrder = Util.GetInput((byte)BS2CardByteOrderEnum.MSB);
            Console.WriteLine("Do you want to use wiegand format? [y/N]");
            Console.Write(">>>> ");
            if (Util.IsNo())
            {
                cardConfig.useWiegandFormat = 0;
            }
            else
            {
                cardConfig.useWiegandFormat = 1;
            }

            BS2CardModelEnum cardModelEnum = (BS2CardModelEnum)cardModel;
            if (cardModelEnum == BS2CardModelEnum.OMPW || cardModelEnum == BS2CardModelEnum.OIPW)
            {
                Console.WriteLine("Enter the card format id [0(default)]");
                Console.Write(">>>> ");
                cardConfig.formatID = Util.GetInput((UInt32)0);

                Console.WriteLine("Choose card data type: [0: Binary(default), 1: ASCII, 2: UTF16, 3: BCD]");
                Console.Write(">>>> ");
                cardConfig.dataType = Util.GetInput((byte)BS2CardDataTypeEnum.BINARY);
                Console.WriteLine("Do you want to use secondary key? [y/N]");
                Console.Write(">>>> ");
                if (Util.IsNo())
                {
                    cardConfig.useSecondaryKey = 0;
                }
                else
                {
                    cardConfig.useSecondaryKey = 1;
                }

                if (cardModelEnum == BS2CardModelEnum.OMPW) // mifare card
                {
                    Console.WriteLine("Enter the start block index for mifare card [0(default)]");
                    Console.Write(">>>> ");
                    cardConfig.mifare.startBlockIndex = Util.GetInput((UInt16)0);
                    Console.WriteLine("Enter the hexadecimal primary key for mifare card. [KEY1-KEY2-...-KEY6]");
                    Console.Write(">>>> ");
                    enterSmartcardKey(cardConfig.mifare.primaryKey);
                    Console.WriteLine("Enter the hexadecimal secondary key for mifare card. [KEY1-KEY2-...-KEY6]");
                    Console.Write(">>>> ");
                    enterSmartcardKey(cardConfig.mifare.secondaryKey);

                    Console.WriteLine("Enter the app id for desfire card. [ID1-ID2-ID3]");
                    Console.Write(">>>> ");
                    enterSmartcardKey(cardConfig.desfire.appID);
                    Console.WriteLine("Enter the file id for desfire card [0(default)]");
                    Console.Write(">>>> ");
                    cardConfig.desfire.fileID = Util.GetInput((byte)0);

#if false //Not yet implemented
                    Console.WriteLine("Enter the encryption type for desfire card [0: DES(default), 1: AES]");
                    Console.Write(">>>> ");
                    cardConfig.desfire.encryptionType = Util.GetInput((byte)0);
#else
                    cardConfig.desfire.encryptionType = 0;
#endif
                    Console.WriteLine("Enter the hexadecimal primary key for desfire card. [KEY1-KEY2-...-KEY16]");
                    Console.Write(">>>> ");
                    enterSmartcardKey(cardConfig.desfire.primaryKey);
                    Console.WriteLine("Enter the hexadecimal secondary key for desfire card. [KEY1-KEY2-...-KEY16]");
                    Console.Write(">>>> ");
                    enterSmartcardKey(cardConfig.desfire.secondaryKey);
                }
                else // iclass card
                {
                    Console.WriteLine("Enter the start block index for iclass card [0(default)]");
                    Console.Write(">>>> ");
                    cardConfig.iclass.startBlockIndex = Util.GetInput((UInt16)0);
                    Console.WriteLine("Enter the hexadecimal primary key for iclass card. [KEY1-KEY2-...-KEY8]");
                    Console.Write(">>>> ");
                    enterSmartcardKey(cardConfig.iclass.primaryKey);
                    Console.WriteLine("Enter the hexadecimal secondary key for iclass card. [KEY1-KEY2-...-KEY8]");
                    Console.Write(">>>> ");
                    enterSmartcardKey(cardConfig.iclass.secondaryKey);
                }
            }

            Console.WriteLine("Trying to set card configuration.");
            result = (BS2ErrorCode)API.BS2_SetCardConfig(sdkContext, deviceID, ref cardConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void scanCard(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2Card card;

            cbCardOnReadyToScan = new API.OnReadyToScan(ReadyToScanForCard);
            Console.WriteLine("Trying to scan card.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_ScanCard(sdkContext, deviceID, out card, cbCardOnReadyToScan);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                if (Convert.ToBoolean(card.isSmartCard))
                {
                    UInt16 hdrCRC;
                    UInt16 cardCRC;
                    BS2SmartCardData smartCard = Util.ConvertTo<BS2SmartCardData>(card.cardUnion);

                    result = computeCRC(smartCard, out hdrCRC, out cardCRC);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Can't compute CRC16({0})", result);
                    }
                    else if (smartCard.header.hdrCRC != hdrCRC)
                    {
                        Console.WriteLine("Get a header crc mismatch(expected[{0}] computed[{1}])", smartCard.header.hdrCRC, hdrCRC);
                    }
                    else if (smartCard.header.cardCRC != cardCRC)
                    {
                        Console.WriteLine("Get a card crc mismatch(expected[{0}] computed[{1}])", smartCard.header.cardCRC, cardCRC);
                    }
                    else
                    {
                        print(sdkContext, smartCard);
                    }
                }
                else
                {
                    BS2CSNCard csnCard = Util.ConvertTo<BS2CSNCard>(card.cardUnion);
                    print(sdkContext, csnCard);
                }
            }

            cbCardOnReadyToScan = null;
        }

        public void writeCard(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2SimpleDeviceInfo deviceInfo;
            BS2SmartCardData smartCard = Util.AllocateStructure<BS2SmartCardData>();

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceInfo(sdkContext, deviceID, out deviceInfo);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't get device information(errorCode : {0}).", result);
                return;
            }

            Console.WriteLine("choose the format for the card: [2: SecureCredential(default), 3: AccessOn]");
            Console.Write(">>>> ");
            smartCard.header.cardType = Util.GetInput((byte)BS2CardTypeEnum.SECURE);

            if(Convert.ToBoolean(deviceInfo.pinSupported))
            {
                Console.WriteLine("Do you want to set pin code? [y/N]");
                Console.Write(">>>> ");
                if(!Util.IsNo())
                {
                    Console.WriteLine("Enter the pin code which you want to set");
                    Console.Write(">>>> ");
                    string pinCodeStr = Console.ReadLine();
                    IntPtr pinChar = Marshal.StringToHGlobalAnsi(pinCodeStr);
                    IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    //result = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, pinCodeStr, pinCode);
                    result = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, pinChar, pinCode);

                    if(result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Got error({0}).", result);
                        Marshal.FreeHGlobal(pinCode);
                        return;
                    }

                    Marshal.Copy(pinCode, smartCard.credentials.pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    Marshal.FreeHGlobal(pinChar);
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

            Array.Clear(smartCard.credentials.templateData, 0, BS2Environment.BS2_SMART_CARD_MAX_TEMPLATE_COUNT * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
            if(smartCard.header.numOfTemplate > 0)
            {
                Console.WriteLine("Enter the size of template which you want to set. [{0}(default)-{1}]", BS2Environment.BS2_SMART_CARD_MIN_TEMPLATE_SIZE, BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                Console.Write(">>>> ");
                smartCard.header.templateSize = Util.GetInput((UInt16)BS2Environment.BS2_SMART_CARD_MIN_TEMPLATE_SIZE);

                BS2Fingerprint fingerprint = Util.AllocateStructure<BS2Fingerprint>();
                fingerprint.index = 0;

                UInt32 outquality;
                for(byte idx = 0 ; idx < smartCard.header.numOfTemplate ; ++idx)
                {
                    Console.WriteLine("Place your finger on the device for fingerprint template[{0}] extraction", idx);
                    result = (BS2ErrorCode)API.BS2_ScanFingerprintEx(sdkContext, deviceID, ref fingerprint, 0, (UInt32)BS2FingerprintQualityEnum.QUALITY_STANDARD, (byte)BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA, out outquality, null);
                    if(result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        if(result == BS2ErrorCode.BS_SDK_ERROR_EXTRACTION_LOW_QUALITY || result == BS2ErrorCode.BS_SDK_ERROR_CAPTURE_LOW_QUALITY)
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

                    Array.Copy(fingerprint.data, 0, smartCard.credentials.templateData, idx*smartCard.header.templateSize, smartCard.header.templateSize);
                }

                Console.WriteLine("Is it duress finger? [0 : Normal(default), 1 : Duress]");
                Console.Write(">>>> ");
                smartCard.header.duressMask = Util.GetInput((byte)BS2FingerprintFlagEnum.NORMAL);
            }
            else
            {
                smartCard.header.templateSize = (UInt16)BS2Environment.BS2_SMART_CARD_MIN_TEMPLATE_SIZE;
                smartCard.header.duressMask = (byte)BS2FingerprintFlagEnum.NORMAL;
            }

            Console.WriteLine("Enter the issue count which you want to set");
            Console.Write(">>>> ");
            smartCard.header.issueCount = (UInt16)Util.GetInput();

            Console.WriteLine("Enter the card id which you want to write on the card");
            Console.Write(">>>> ");

            UInt64 cardID = 0;
            string cardIDStr = Console.ReadLine();            
            if (cardIDStr.Length > BS2Environment.BS2_CARD_DATA_SIZE)
            {
                Console.WriteLine("Card id should less than {0} words.", BS2Environment.BS2_CARD_DATA_SIZE);
                return;
            }
            else if (!UInt64.TryParse(cardIDStr, out cardID) || cardID == 0)
            {
                Console.WriteLine("Invalid card id");
                return;
            }
            else
            {
                byte[] cardIDArray = BitConverter.GetBytes(cardID);
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(cardIDArray);
                }

                Array.Clear(smartCard.cardID, 0, BS2Environment.BS2_CARD_DATA_SIZE);

                for (int idx = 0; idx < cardIDArray.Length; ++idx)
                {
                    smartCard.cardID[BS2Environment.BS2_CARD_DATA_SIZE - idx - 1] = cardIDArray[idx];
                }
            }

            if ((BS2CardTypeEnum)smartCard.header.cardType == BS2CardTypeEnum.ACCESS)
            {
                Console.WriteLine("Enter the ID of the access group which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);
                int idx = 0;
                Array.Clear(smartCard.accessOnData.accessGroupID, 0, BS2Environment.BS2_SMART_CARD_MAX_ACCESS_GROUP_COUNT);
                
                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt16 item;
                        if (UInt16.TryParse(accessGroupID, out item))
                        {
                            smartCard.accessOnData.accessGroupID[idx++] = item;
                            if (idx >= BS2Environment.BS2_SMART_CARD_MAX_ACCESS_GROUP_COUNT)
                            {
                                break;
                            }
                        }
                    }
                }
                
                Console.WriteLine("Enter start time which you want to set. [default(Today), yyyy-MM-dd HH:mm:ss]");
                Console.Write(">>>> ");
                if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out smartCard.accessOnData.startTime))
                {
                    return;
                }

                Console.WriteLine("Enter end time which you want to set. [default(Today), yyyy-MM-dd HH:mm:ss]");
                Console.Write(">>>> ");
                if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out smartCard.accessOnData.endTime))
                {
                    return;
                }

                // card auth mode
                //Console.WriteLine("Enter card authentication mode which you want to set:");
                //Console.WriteLine(" [2: Card only, 3: Card+Biometric, 4: Card+PIN, 5: Card+Biometric/PIN, 6: Card+Biometric+PIN]");
                //Console.Write(">>>> ");
                //smartCard.header.cardAuthMode = Util.GetInput((byte)BS2CardAuthModeEnum.NONE);
            }

            result = computeCRC(smartCard, out smartCard.header.hdrCRC, out smartCard.header.cardCRC);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Can't compute CRC16({0})", result);
            }
            else
            {
                Console.WriteLine("Trying to write card.");
                result = (BS2ErrorCode)API.BS2_WriteCard(sdkContext, deviceID, ref smartCard);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
            }
        }

        public void eraseCard(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Trying to erase card.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_EraseCard(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        public void getBlacklist(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr blacklistObj = IntPtr.Zero;
            UInt32 numBlacklist = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Trying to get blacklist from device.");
            result = (BS2ErrorCode)API.BS2_GetAllBlackList(sdkContext, deviceID, out blacklistObj, out numBlacklist);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numBlacklist > 0)
            {
                IntPtr curAccessGroupObj = blacklistObj;
                int structSize = Marshal.SizeOf(typeof(BS2BlackList));

                for (int idx = 0; idx < numBlacklist; ++idx)
                {
                    BS2BlackList item = (BS2BlackList)Marshal.PtrToStructure(curAccessGroupObj, typeof(BS2BlackList));
                    print(sdkContext, item);
                    curAccessGroupObj = (IntPtr)((long)curAccessGroupObj + structSize);
                }

                API.BS2_ReleaseObject(blacklistObj);
            }
            else
            {
                Console.WriteLine(">>> There is no blacklist in the device.");
            }
        }

        public void removeBlacklist(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all blacklist? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all blacklist from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllBlackList(sdkContext, deviceID);
            }
            else
            {
                IntPtr blacklistObj;
                UInt32 numBlacklist;

                Console.WriteLine("Trying to get blacklists from device.");
                result = (BS2ErrorCode)API.BS2_GetAllBlackList(sdkContext, deviceID, out blacklistObj, out numBlacklist);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else if (numBlacklist > 0)
                {
                    List<BS2BlackList> blackList = new List<BS2BlackList>();
                    IntPtr curBlacklistObj = blacklistObj;
                    int structSize = Marshal.SizeOf(typeof(BS2BlackList));

                    for (int idx = 0; idx < numBlacklist; ++idx)
                    {
                        BS2BlackList item = (BS2BlackList)Marshal.PtrToStructure(curBlacklistObj, typeof(BS2BlackList));
                        blackList.Add(item);
                        curBlacklistObj = (IntPtr)((long)curBlacklistObj + structSize);
                    }                    

                    Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                    for (int idx = 0; idx < numBlacklist; ++idx)
                    {
                        Console.WriteLine("[{0:000}] ==> Blacklist issueCount[{0}] cardID[{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}]",
                                            idx,
                                            blackList[idx].issueCount,
                                            blackList[idx].cardID[BS2Environment.BS2_CARD_DATA_SIZE - 1],
                                            blackList[idx].cardID[BS2Environment.BS2_CARD_DATA_SIZE - 2],
                                            blackList[idx].cardID[BS2Environment.BS2_CARD_DATA_SIZE - 3],
                                            blackList[idx].cardID[BS2Environment.BS2_CARD_DATA_SIZE - 4],
                                            blackList[idx].cardID[BS2Environment.BS2_CARD_DATA_SIZE - 5],
                                            blackList[idx].cardID[BS2Environment.BS2_CARD_DATA_SIZE - 6],
                                            blackList[idx].cardID[BS2Environment.BS2_CARD_DATA_SIZE - 7],
                                            blackList[idx].cardID[BS2Environment.BS2_CARD_DATA_SIZE - 8]);
                    }
                    Console.WriteLine("+----------------------------------------------------------------------------------------------------------+");
                    Console.WriteLine("Enter the index of the blacklist which you want to remove: [INDEX_1,INDEX_2 ...]");
                    Console.Write(">>>> ");
                    char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                    string[] blackListIndexs = Console.ReadLine().Split(delimiterChars);
                    HashSet<int> blackListSet = new HashSet<int>();

                    if (blackListIndexs.Length == 0)
                    {
                        Console.WriteLine("Invalid parameter.");
                    }
                    else
                    {
                        foreach (string index in blackListIndexs)
                        {
                            if (index.Length > 0)
                            {
                                UInt32 item;
                                if (UInt32.TryParse(index, out item))
                                {
                                    if (item < numBlacklist)
                                    {
                                        blackListSet.Add((int)item);
                                    }
                                }
                            }
                        }

                        if (blackListSet.Count > 0)
                        {
                            foreach (int index in blackListSet)
                            {
                                Marshal.StructureToPtr(blackList[index], blacklistObj + structSize, false);
                            }

                            Console.WriteLine("Trying to remove blacklist from device.");
                            result = (BS2ErrorCode)API.BS2_RemoveBlackList(sdkContext, deviceID, blacklistObj, (UInt32)blackListSet.Count);
                            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                            {
                                Console.WriteLine("Got error({0}).", result);
                            }
                        }
                    }

                    API.BS2_ReleaseObject(blacklistObj);
                }
                else
                {
                    Console.WriteLine(">>> There is no blacklist in the device.");
                }
            }
        }

        public void setBlacklist(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            Console.WriteLine("How many Blacklists do you want to set? [1(default)-1000]");
            Console.Write(">>>> ");
            int amount = Util.GetInput(1);
            List<BS2BlackList> blackList = new List<BS2BlackList>();

            scanAndAddBlacklist(sdkContext, deviceID, blackList, amount);

            int structSize = Marshal.SizeOf(typeof(BS2BlackList));
            IntPtr blacklistObj = Marshal.AllocHGlobal(structSize * blackList.Count);
            IntPtr curBlacklistObj = blacklistObj;
            foreach (BS2BlackList item in blackList)
            {
                Marshal.StructureToPtr(item, curBlacklistObj, false);
                curBlacklistObj = (IntPtr)((long)curBlacklistObj + structSize);
            }

            Console.WriteLine("Trying to set blacklists to device.");
            result = (BS2ErrorCode)API.BS2_SetBlackList(sdkContext, deviceID, blacklistObj, (UInt32)blackList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(blacklistObj);
        }

        #region Callbacks
        void ReadyToScanForCard(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your card on the device.");
        }
        #endregion

        #region SmartCard Utils
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

        BS2ErrorCode computeCRC(BS2SmartCardData smartCard, out UInt16 hdrCRC, out UInt16 cardCRC)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            IntPtr smartCardObj = Marshal.AllocHGlobal(Marshal.SizeOf(smartCard));            
            int cardTypeOffset = (int)Marshal.OffsetOf(typeof(BS2SmartCardHeader), "cardType");
            int cardCRCOffset = (int)Marshal.OffsetOf(typeof(BS2SmartCardHeader), "cardCRC");
            IntPtr cardDataObj = smartCardObj + cardTypeOffset;

            Marshal.StructureToPtr(smartCard, smartCardObj, false);

            cardCRC = 0xFFFF;
            hdrCRC = 0xFFFF;

            result = (BS2ErrorCode)API.BS2_ComputeCRC16CCITT(cardDataObj, (UInt32)(Marshal.SizeOf(typeof(BS2SmartCardData)) - cardTypeOffset), ref cardCRC);
            if(result ==BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Marshal.WriteInt16(smartCardObj, cardCRCOffset, (Int16)cardCRC);    
                IntPtr cardCrcObj = smartCardObj + cardCRCOffset;
                result = (BS2ErrorCode)API.BS2_ComputeCRC16CCITT(cardCrcObj, (UInt32)(Marshal.SizeOf(typeof(BS2SmartCardHeader)) - cardCRCOffset), ref hdrCRC);
            }
            
            Marshal.FreeHGlobal(smartCardObj);
            return result;
        }
        #endregion

        #region Blacklist Utils
        void scanAndAddBlacklist(IntPtr sdkContext, UInt32 deviceID, List<BS2BlackList> blackList, int amount)
        {
            cbCardOnReadyToScan = new API.OnReadyToScan(ReadyToScanForCard);
            for (int idx = 0; idx < amount; ++idx)
            {
                BS2Card card = Util.AllocateStructure<BS2Card>();
                BS2BlackList blacklistItem = Util.AllocateStructure<BS2BlackList>();
                Console.WriteLine("Trying to scan card.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_ScanCard(sdkContext, deviceID, out card, cbCardOnReadyToScan);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                    return;
                }
                else if (Convert.ToBoolean(card.isSmartCard))
                {
                    BS2SmartCardData smartCard = Util.ConvertTo<BS2SmartCardData>(card.cardUnion);
                    Array.Copy(smartCard.cardID, blacklistItem.cardID, BS2Environment.BS2_CARD_DATA_SIZE);
                    blacklistItem.issueCount = smartCard.header.issueCount;
                }
                else
                {
                    BS2CSNCard csnCard = Util.ConvertTo<BS2CSNCard>(card.cardUnion);
                    Array.Copy(csnCard.data, blacklistItem.cardID, BS2Environment.BS2_CARD_DATA_SIZE);
                    blacklistItem.issueCount = 0; //should set count of issue to 0
                }
                
                bool isDuplicated = false;
                foreach (BS2BlackList item in blackList)
                {
                    if (blacklistItem.cardID.SequenceEqual(item.cardID))
                    {
                        isDuplicated = true;
                        break;
                    }
                }

                if (isDuplicated)
                {
                    Console.WriteLine("Already exist blacklist. try again");
                    --idx;
                    continue;
                }            
                
                blackList.Add(blacklistItem);
            }

            cbCardOnReadyToScan = null;
        }
        #endregion

        void print(IntPtr sdkContext, BS2CardConfig cardConfig)
        {
            Console.WriteLine(">>>> Card configuration formatID[{0}]", cardConfig.formatID);
            Console.WriteLine("     |--byteOrder[{0}]", (BS2CardByteOrderEnum)cardConfig.byteOrder);
            Console.WriteLine("     |--useWiegandFormat[{0}]", Convert.ToBoolean(cardConfig.useWiegandFormat));
            Console.WriteLine("     |--dataType[{0}]", (BS2CardDataTypeEnum)cardConfig.dataType);
            Console.WriteLine("     |--useSecondaryKey[{0}]", Convert.ToBoolean(cardConfig.useSecondaryKey));
            Console.WriteLine("     |--mifare");
            Console.WriteLine("     |  |--primaryKey[{0}]", BitConverter.ToString(cardConfig.mifare.primaryKey));
            Console.WriteLine("     |  |--secondaryKey[{0}]", BitConverter.ToString(cardConfig.mifare.secondaryKey));
            Console.WriteLine("     |  |--startBlockIndex[{0}]", cardConfig.mifare.startBlockIndex);
            Console.WriteLine("     |--iclass");
            Console.WriteLine("     |  |--primaryKey[{0}]", BitConverter.ToString(cardConfig.iclass.primaryKey));
            Console.WriteLine("     |  |--secondaryKey[{0}]", BitConverter.ToString(cardConfig.iclass.secondaryKey));
            Console.WriteLine("     |  |--startBlockIndex[{0}]", cardConfig.iclass.startBlockIndex);
            Console.WriteLine("     |--desfire");
            Console.WriteLine("     |  |--primaryKey[{0}]", BitConverter.ToString(cardConfig.desfire.primaryKey));
            Console.WriteLine("     |  |--secondaryKey[{0}]", BitConverter.ToString(cardConfig.desfire.secondaryKey));
            Console.WriteLine("     |  |--appID[{0}]", BitConverter.ToString(cardConfig.desfire.appID));
            Console.WriteLine("     |  |--fileID[{0}]", cardConfig.desfire.fileID);
            Console.WriteLine("     |  |--encryptionType[{0}]", cardConfig.desfire.encryptionType);
        }

        void print(IntPtr sdkContext, BS2SmartCardData smartCard)
        {
            byte[] cardIDArray = new byte[8];
            for (int idx = 0; idx < 8; ++idx)
            {
                cardIDArray[idx] = smartCard.cardID[BS2Environment.BS2_CARD_DATA_SIZE - idx - 1];
            }

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(cardIDArray);
            }

            UInt64 cardID = BitConverter.ToUInt64(cardIDArray, 0);
            BS2CardTypeEnum cardType = (BS2CardTypeEnum)smartCard.header.cardType;

            Console.WriteLine(">>>> SmartCard type[{0}]", cardType);
            Console.WriteLine("     |--cardID[{0}]", cardID);
            Console.WriteLine("     |--numOfTemplate[{0}]", smartCard.header.numOfTemplate);
            Console.WriteLine("     |--templateSize[{0}]", smartCard.header.templateSize);
            Console.WriteLine("     |--issueCount[{0}]", smartCard.header.issueCount);
            Console.WriteLine("     |--duressMask[{0}]", (BS2FingerprintFlagEnum)smartCard.header.duressMask);

            if (cardType == BS2CardTypeEnum.ACCESS)
            {
                Console.WriteLine("     |--accessOnCard");
                Console.WriteLine("     |  |--accessGroups");
                for (int idx = 0; idx < BS2Environment.BS2_SMART_CARD_MAX_ACCESS_GROUP_COUNT; ++idx)
                {
                    if (smartCard.accessOnData.accessGroupID[idx] != 0)
                    {
                        Console.WriteLine("     |  |  |--accessGroupID[{0}]", smartCard.accessOnData.accessGroupID[idx]);
                    }
                    else
                    {
                        break;
                    }
                }

                Console.WriteLine("     |  |--startTime[{0}]", Util.ConvertFromUnixTimestamp(smartCard.accessOnData.startTime).ToString("yyyy-MM-dd HH:mm:ss"));
                Console.WriteLine("     |  |--endTime[{0}]", Util.ConvertFromUnixTimestamp(smartCard.accessOnData.endTime).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        void print(IntPtr sdkContext, BS2CSNCard csnCard)
        {
            Console.WriteLine(">>>> CSN Card type[{0}] size[{1,2}] data[{2}]", (BS2CardTypeEnum)csnCard.type, csnCard.size, BitConverter.ToString(csnCard.data));
        }

        void print(IntPtr sdkContext, BS2BlackList blacklist)
        {
            Console.WriteLine(">>>> Blacklist issueCount[{0}] cardID[{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}]",
                                        blacklist.issueCount,
                                        blacklist.cardID[BS2Environment.BS2_CARD_DATA_SIZE - 1],
                                        blacklist.cardID[BS2Environment.BS2_CARD_DATA_SIZE - 2],
                                        blacklist.cardID[BS2Environment.BS2_CARD_DATA_SIZE - 3],
                                        blacklist.cardID[BS2Environment.BS2_CARD_DATA_SIZE - 4],
                                        blacklist.cardID[BS2Environment.BS2_CARD_DATA_SIZE - 5],
                                        blacklist.cardID[BS2Environment.BS2_CARD_DATA_SIZE - 6],
                                        blacklist.cardID[BS2Environment.BS2_CARD_DATA_SIZE - 7],
                                        blacklist.cardID[BS2Environment.BS2_CARD_DATA_SIZE - 8]);
        }
    }
}
