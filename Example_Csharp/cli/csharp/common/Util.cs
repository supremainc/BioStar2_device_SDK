using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Suprema
{    
    class Util
    {
        public static T BytesToStruct<T>(ref byte[] source)
        {
            Type structType = typeof(T);
            int structSize = Marshal.SizeOf(structType);
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(source, 0, buffer, structSize);
            T instance = (T)Marshal.PtrToStructure(buffer, structType);
            Marshal.FreeHGlobal(buffer);

            return instance;
        }

        public static T BytesToStruct<T>(ref byte[] source, int startIndex)
        {
            Type structType = typeof(T);
            int structSize = Marshal.SizeOf(structType);
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(source, startIndex, buffer, structSize);
            T instance = (T)Marshal.PtrToStructure(buffer, structType);
            Marshal.FreeHGlobal(buffer);

            return instance;
        }

        public void CopyMemory<T>(ref IntPtr source, ref IntPtr target)
        {
            Type structType = typeof(T);
            int structSize = Marshal.SizeOf(structType);
            CopyMemory(target, source, (uint)structSize);
        }

        public static byte[] StructToBytes<T>(ref T source)
        {
            Type structType = typeof(T);
            int structSize = Marshal.SizeOf(structType);
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            Marshal.StructureToPtr(source, buffer, true);
            byte[] output = new byte[structSize];
            Marshal.Copy(buffer, output, 0, structSize);
            Marshal.FreeHGlobal(buffer);

            return output;
        }

        public static void TranslatePrimitive<TSource, TOutput>(ref TSource src, ref TOutput output)
        {
            Type typeSrc = typeof(TSource);
            Type typeOut = typeof(TOutput);

            TypedReference trSrc = __makeref(src);
            TypedReference trOut = __makeref(output);
            FieldInfo[] srcInfos = typeSrc.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo[] outInfos = typeOut.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo srcInfo in srcInfos)
            {
                IEnumerable<FieldInfo> matchs = outInfos.Where(x => x.Name == srcInfo.Name && x.FieldType == srcInfo.FieldType);
                foreach(FieldInfo outInfo in matchs)
                {
                    outInfo.SetValueDirect(trOut, srcInfo.GetValueDirect(trSrc));
                    break;
                }
            }
        }

        public delegate int FUNC_BS2_GetAll(IntPtr context, UInt32 deviceId, out IntPtr obj, out UInt32 numItem);
        public static BS2ErrorCode CSP_BS2_GetAll<CSP_T, CXX_T>(IntPtr context, UInt32 deviceId, out CSP_T[] ItemsObj, out UInt32 numItem, FUNC_BS2_GetAll func)
        {
            IntPtr _itemsObj = IntPtr.Zero;
            UInt32 _itemNum = 0;
            BS2ErrorCode result = (BS2ErrorCode)func(context, deviceId, out _itemsObj, out _itemNum);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                numItem = 0;
                ItemsObj = AllocateStructureArray<CSP_T>(0);
                return result;
            }

            CSP_T[] _items = AllocateStructureArray<CSP_T> ((int)_itemNum);

            Translator<CXX_T, CSP_T> transItem = new Translator<CXX_T, CSP_T>();
            int ItemSize = Marshal.SizeOf(typeof(CXX_T));
            IntPtr curItemObj = _itemsObj;
            for (int idx = 0; idx < _itemNum; ++idx)
            {
                CXX_T item = (CXX_T)Marshal.PtrToStructure(curItemObj, typeof(CXX_T));
                transItem.Translate(ref item, ref _items[idx]);
                curItemObj += ItemSize;
            }

            API.BS2_ReleaseObject(_itemsObj);

            numItem = _itemNum;
            ItemsObj = _items;
            return result;
        }
                
        public delegate int FUNC_BS2_GetItems(IntPtr context, UInt32 deviceId, IntPtr Ids, UInt32 IdCount, out IntPtr obj, out UInt32 numItem);
        public static BS2ErrorCode CSP_BS2_GetItems<CSP_ID_T, CSP_T, CXX_ID_T, CXX_T>(IntPtr context, UInt32 deviceId, CSP_ID_T[] Ids, UInt32 idCount, out CSP_T[] ItemsObj, out UInt32 numItem, FUNC_BS2_GetItems func)
        {
            Translator<CSP_ID_T, CXX_ID_T> transID = new Translator<CSP_ID_T, CXX_ID_T>();
            CXX_ID_T Id = Util.AllocateStructure<CXX_ID_T>();

            int IdSize = Marshal.SizeOf(typeof(CXX_ID_T));
            IntPtr _IDObj = Marshal.AllocHGlobal(IdSize * (int)idCount);
            IntPtr _curIDObj = _IDObj;
            for (int idx = 0; idx < idCount; ++idx)
            {
                transID.Translate(ref Ids[idx], ref Id);
                Marshal.StructureToPtr(Id, _curIDObj, true);
                _curIDObj += IdSize;
            }

            IntPtr _itemsObj = IntPtr.Zero;
            UInt32 _itemNum = 0;
            BS2ErrorCode result = (BS2ErrorCode)func(context, deviceId, _IDObj, idCount, out _itemsObj, out _itemNum);
            Marshal.FreeHGlobal(_IDObj);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                numItem = 0;
                ItemsObj = new CSP_T[0];
                return result;
            }

            CSP_T[] _items = AllocateStructureArray<CSP_T>((int)_itemNum);

            Translator<CXX_T, CSP_T> transItem = new Translator<CXX_T, CSP_T>();
            int ItemSize = Marshal.SizeOf(typeof(CXX_T));
            IntPtr curItemObj = _itemsObj;
            for (int idx = 0; idx < _itemNum; ++idx)
            {
                CXX_T item = (CXX_T)Marshal.PtrToStructure(curItemObj, typeof(CXX_T));
                transItem.Translate(ref item, ref _items[idx]);
                curItemObj += ItemSize;
            }

            API.BS2_ReleaseObject(_itemsObj);

            numItem = _itemNum;
            ItemsObj = _items;
            return result;
        }

        public delegate int FUNC_BS2_RemoveItems(IntPtr context, UInt32 deviceId, IntPtr Ids, UInt32 IdCount);
        public static BS2ErrorCode CSP_BS2_RemoveItems<CSP_ID_T, CXX_ID_T>(IntPtr context, UInt32 deviceId, CSP_ID_T[] Ids, UInt32 idCount, FUNC_BS2_RemoveItems func)
        {
            Translator<CSP_ID_T, CXX_ID_T> transID = new Translator<CSP_ID_T, CXX_ID_T>();
            CXX_ID_T Id = Util.AllocateStructure<CXX_ID_T>();

            int IdSize = Marshal.SizeOf(typeof(CXX_ID_T));
            IntPtr _IDObj = Marshal.AllocHGlobal(IdSize * (int)idCount);
            IntPtr _curIDObj = _IDObj;
            for (int idx = 0; idx < idCount; ++idx)
            {
                transID.Translate(ref Ids[idx], ref Id);
                Marshal.StructureToPtr(Id, _curIDObj, true);
                _curIDObj += IdSize;
            }

            BS2ErrorCode result = (BS2ErrorCode)func(context, deviceId, _IDObj, idCount);
            Marshal.FreeHGlobal(_IDObj);
            return result;
        }


        public delegate int FUNC_BS2_SetItems(IntPtr context, UInt32 deviceId, IntPtr ItemsObj, UInt32 ItemCount);
        public static BS2ErrorCode CSP_BS2_SetItems<CSP_T, CXX_T>(IntPtr context, UInt32 deviceId, CSP_T[] ItemsObj, UInt32 ItemCount, FUNC_BS2_SetItems func)
        {
            Translator<CSP_T, CXX_T> transItem = new Translator<CSP_T, CXX_T>();
            CXX_T Item = Util.AllocateStructure<CXX_T>();

            int ItemSize = Marshal.SizeOf(typeof(CXX_T));
            IntPtr _ItemsObj = Marshal.AllocHGlobal(ItemSize * (int)ItemCount);
            IntPtr _curItemObj = _ItemsObj;
            for (int idx = 0; idx < ItemCount; ++idx)
            {
                transItem.Translate(ref ItemsObj[idx], ref Item);
                Marshal.StructureToPtr(Item, _curItemObj, false);
                _curItemObj += ItemSize;
            }

            BS2ErrorCode result = (BS2ErrorCode)func(context, deviceId, _ItemsObj, ItemCount);
            Marshal.FreeHGlobal(_ItemsObj);
            return result;
        }


        public static T AllocateStructure<T>()
        {
            int structSize = Marshal.SizeOf(typeof(T));
            byte[] empty = new byte[structSize];
            Array.Clear(empty, 0, empty.Length);
            IntPtr buffer = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(empty, 0, buffer, structSize);
            T instance = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);

            return instance;

        }

        public static T[] AllocateStructureArray<T>(int count)
        {
            T[] result = new T[count];
            int structSize = Marshal.SizeOf(typeof(T));
            byte[] empty = new byte[structSize * count];
            Array.Clear(empty, 0, empty.Length);
            IntPtr buffer = Marshal.AllocHGlobal(structSize * count);
            IntPtr curBuffer = buffer;
            Marshal.Copy(empty, 0, buffer, structSize * count);
            for (int idx = 0; idx < count; idx++)
            {
                result[idx] = (T)Marshal.PtrToStructure(curBuffer, typeof(T));
                curBuffer = (IntPtr)((long)curBuffer + structSize);
            }

            Marshal.FreeHGlobal(buffer);
            return result;

        }

        public static byte[] StringToByte(int allocSize, string source)
        {
            byte[] result = new byte[allocSize];
            Array.Clear(result, 0, result.Length);
            byte[] sourceByte = Encoding.UTF8.GetBytes(source);
            int copySize = Math.Min(allocSize, sourceByte.Length);
            Buffer.BlockCopy(sourceByte, 0, result, 0, copySize);
            return result;
        }

        public static T ConvertTo<T>(byte[] src)
        {
            if (src.Length < Marshal.SizeOf(typeof(T)))
            {
                throw new ArgumentException("array size is less than object size", "src");
            }

            IntPtr buffer = Marshal.AllocHGlobal(src.Length);
            Marshal.Copy(src, 0, buffer, src.Length);
            T item = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);

            return item;
        }

        public static byte[] ConvertTo<T>(ref T instance)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(instance, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public static bool GetInput(out int input)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Int32.TryParse(inputStr, out input);
            }
            else
            {
                input = 0;
                return false;
            }
        }

        public static int GetInput()
        {
            do
            {
                string inputStr = Console.ReadLine();
                if (inputStr.Length > 0)
                {
                    return Convert.ToInt32(inputStr);
                }
            } while (true);
        }

        public static UInt32 GetInputHexa()
        {
            do
            {
                string inputStr = Console.ReadLine();
                if (inputStr.Length > 0)
                {
                    try
                    {
                        return Convert.ToUInt32(inputStr, 16);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Please enter the correct value in hexadecimal.");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("The input value is out of range.");
                    }
                }
            } while (true);
        }


        public static char GetInput(char defaultValue)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Convert.ToChar(inputStr);
            }

            return defaultValue;
        }

        public static byte GetInput(byte defaultValue)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Convert.ToByte(inputStr);
            }

            return defaultValue;
        }

        public static UInt16 GetInput(UInt16 defaultValue)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Convert.ToUInt16(inputStr);
            }

            return defaultValue;
        }

        public static UInt32 GetInput(UInt32 defaultValue)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return Convert.ToUInt32(inputStr);
            }

            return defaultValue;
        }

        public static float GetInput(float defaultValue)
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length > 0)
            {
                return float.Parse(inputStr);
            }

            return defaultValue;
        }

        public static string ConvertHexByte2String(byte[] convertArr)
        {
            string converted = string.Empty;
            converted = string.Concat(Array.ConvertAll(convertArr, byt => byt.ToString("X2")));
            return converted;
        }

        public static byte[] ConvertString2HexByte(string convertStr)
        {
            byte[] converted = new byte[convertStr.Length / 2];
            for (int i = 0; i < converted.Length; i++)
            {
                converted[i] = Convert.ToByte(convertStr.Substring(i * 2, 2), 16);
            }
            return converted;
        }

        public static bool GetTimestamp(string formatString, UInt32 defaultValue, out UInt32 timestamp)
        {
            string inputStr = Console.ReadLine();

            if (defaultValue == 0)
            {
                defaultValue = Convert.ToUInt32(Util.ConvertToUnixTimestamp(DateTime.Now));
            }

            timestamp = defaultValue;
            if (inputStr.Length > 0)
            {
                DateTime dateTime;
                if (!DateTime.TryParseExact(inputStr, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    Console.WriteLine("Invalid datetime : {0}", inputStr);
                    return false;
                }
                else
                {
                    //timestamp = Convert.ToUInt32(Util.ConvertToUnixTimestamp(dateTime));
                    timestamp = Convert.ToUInt32(Util.GetPosixTime(dateTime));
                }
            }

            return true;
        }

        public static bool GetTime(string formatString, UInt32 defaultValue, out UInt32 time)
        {
            string inputStr = Console.ReadLine();
            time = 0;
            if (inputStr.Length > 0)
            {
                DateTime dateTime;
                if (!DateTime.TryParseExact(inputStr, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    Console.WriteLine("Invalid datetime : {0}", inputStr);
                    return false;
                }
                else
                {
                    time = (UInt32)(dateTime.Hour * 60 * 60 + dateTime.Minute * 60 + dateTime.Second);
                }
            }

            return true;
        }

        public static bool IsYes()
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length == 0 || String.Compare(inputStr, "Y", true) == 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsNo()
        {
            string inputStr = Console.ReadLine();
            if (inputStr.Length == 0 || String.Compare(inputStr, "N", true) == 0)
            {
                return true;
            }

            return false;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() + (TimeZoneInfo.Local.IsDaylightSavingTime(date) ? TimeZoneInfo.Local.BaseUtcOffset : TimeSpan.Zero) - origin.ToUniversalTime();
            return Math.Floor(diff.TotalSeconds);
        }

        public static double GetPosixTime(DateTime date)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // FISF-934 Set 0 problem when converted posix time
            //DateTime currDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime currDate = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc);
            // FISF-934 Set 0 problem when converted posix time
            return currDate.Subtract(baseDate).TotalSeconds;
        }

        public static bool LoadBinary(string filePath, out IntPtr binaryData, out UInt32 binaryDataLen)
        {
            bool handled = false;
            FileStream fs = null;
                        
            binaryData = IntPtr.Zero;
            binaryDataLen = 0;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                int fileSize = (int)fs.Length;
                int totalReadCount = 0;
                byte[] readBuffer = new byte[fileSize];               

                while (totalReadCount < fileSize)
                {
                    int readCount = fs.Read(readBuffer, totalReadCount, (fileSize - totalReadCount));
                    if (readCount > 0)
                    {
                        totalReadCount += readCount;
                    }
                    else
                    {
                        Console.WriteLine("I/O error occurred while reading firmware file.");
                        break;
                    }
                }

                if (totalReadCount == fileSize)
                {
                    binaryData = Marshal.AllocHGlobal(fileSize);
                    Marshal.Copy(readBuffer, 0, binaryData, fileSize);
                    binaryDataLen = (UInt32)fileSize;
                    handled = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading from {0}. Message = {1}", filePath, e.Message);
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

        public static string getActionMsg(BS2Action action)
        {
            BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)action.type;

            switch (actionType)
            {
                case BS2ActionTypeEnum.NONE:
                    return "Not specified";
                case BS2ActionTypeEnum.RELAY:
                    {
                        BS2RelayAction relay = Util.ConvertTo<BS2RelayAction>(action.actionUnion);
                        return String.Format("RelayAction relayIndex[{0}] signalID[{1}] count[{2}] onDuration[{3}ms] offDuration[{4}ms] delay[{5}ms]",
                                            relay.relayIndex,
                                            relay.signal.signalID,
                                            relay.signal.count,
                                            relay.signal.onDuration,
                                            relay.signal.offDuration,
                                            relay.signal.delay);
                    }
                case BS2ActionTypeEnum.TTL:
                    {
                        BS2OutputPortAction outputPort = Util.ConvertTo<BS2OutputPortAction>(action.actionUnion);
                        return String.Format("OutputPortAction relayIndex[{0}] signalID[{1}] count[{2}] onDuration[{3}ms] offDuration[{4}ms] delay[{5}ms]",
                                            outputPort.portIndex,
                                            outputPort.signal.signalID,
                                            outputPort.signal.count,
                                            outputPort.signal.onDuration,
                                            outputPort.signal.offDuration,
                                            outputPort.signal.delay);
                    }
                case BS2ActionTypeEnum.DISPLAY:
                    {
                        BS2DisplayAction display = Util.ConvertTo<BS2DisplayAction>(action.actionUnion);
                        return String.Format("DisplayAction displayID[{0}] resourceID[{1}] delay[{2}ms]",
                                            display.displayID,
                                            display.resourceID,
                                            display.duration);
                    }
                case BS2ActionTypeEnum.SOUND:
                    {
                        BS2SoundAction sound = Util.ConvertTo<BS2SoundAction>(action.actionUnion);
                        return String.Format("SoundAction soundIndex[{0}] count[{1}]", sound.soundIndex, sound.count);
                    }
                case BS2ActionTypeEnum.LED:
                    {
                        BS2LedAction led = Util.ConvertTo<BS2LedAction>(action.actionUnion);
                        string ledSignalStr = "";
                        for (int idx = 0; idx < BS2Environment.BS2_LED_SIGNAL_NUM; ++idx)
                        {
                            ledSignalStr += String.Format("[color[{0}] duration[{1}ms] delay[{2}ms]]",
                                                            (BS2LedColorEnum)led.signal[idx].color,
                                                            led.signal[idx].duration,
                                                            led.signal[idx].delay);

                            if (idx + 1 < BS2Environment.BS2_LED_SIGNAL_NUM)
                            {
                                ledSignalStr += ", ";
                            }
                        }

                        return String.Format("LedAction count[{0}] {1}", led.count, ledSignalStr);
                    }
                case BS2ActionTypeEnum.BUZZER:
                    {
                        BS2BuzzerAction buzzer = Util.ConvertTo<BS2BuzzerAction>(action.actionUnion);
                        string buzzerSignalStr = "";
                        for (int idx = 0; idx < BS2Environment.BS2_BUZZER_SIGNAL_NUM; ++idx)
                        {
                            buzzerSignalStr += String.Format("[tone[{0}] fadeout[{1}] duration[{2}ms] delay[{3}ms]]",
                                                            (BS2BuzzerToneEnum)buzzer.signal[idx].tone,
                                                            Convert.ToBoolean(buzzer.signal[idx].fadeout),
                                                            buzzer.signal[idx].duration,
                                                            buzzer.signal[idx].delay);

                            if (idx + 1 < BS2Environment.BS2_BUZZER_SIGNAL_NUM)
                            {
                                buzzerSignalStr += ", ";
                            }
                        }

                        return String.Format("BuzzerAction count[{0}] {1}", buzzer.count, buzzerSignalStr);
                    }
                case BS2ActionTypeEnum.LIFT:
                    {
                        BS2LiftAction lift = Util.ConvertTo<BS2LiftAction>(action.actionUnion);
                        return String.Format("LiftAction deviceID[{0}] type[{1}]", lift.liftID, (BS2LiftActionTypeEnum)lift.type);
                    }
                default:
                    return "Not implemented yet.";
            }
        }

        public static string GetLogMsg(BS2Event eventLog)
        {
#if false
            return "eventlog : ";
#else
            switch (((BS2EventCodeEnum)eventLog.code & BS2EventCodeEnum.MASK))
            {
                case BS2EventCodeEnum.DOOR_LOCKED:
                case BS2EventCodeEnum.DOOR_UNLOCKED:
                case BS2EventCodeEnum.DOOR_CLOSED:
                case BS2EventCodeEnum.DOOR_OPENED:
                case BS2EventCodeEnum.DOOR_FORCED_OPEN:
                case BS2EventCodeEnum.DOOR_FORCED_OPEN_ALARM:
                case BS2EventCodeEnum.DOOR_FORCED_OPEN_ALARM_CLEAR:
                case BS2EventCodeEnum.DOOR_HELD_OPEN:
                case BS2EventCodeEnum.DOOR_HELD_OPEN_ALARM:
                case BS2EventCodeEnum.DOOR_HELD_OPEN_ALARM_CLEAR:
                case BS2EventCodeEnum.DOOR_APB_ALARM:
                case BS2EventCodeEnum.DOOR_APB_ALARM_CLEAR:
                    return GetDoorIdMsg(eventLog);
                case BS2EventCodeEnum.ZONE_APB_ALARM:
                case BS2EventCodeEnum.ZONE_APB_ALARM_CLEAR:
                case BS2EventCodeEnum.ZONE_TIMED_APB_ALARM:
                case BS2EventCodeEnum.ZONE_TIMED_APB_ALARM_CLEAR:
                case BS2EventCodeEnum.ZONE_FIRE_ALARM:
                case BS2EventCodeEnum.ZONE_FIRE_ALARM_CLEAR:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_VIOLATION:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_START:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_END:
                case BS2EventCodeEnum.ZONE_SCHEDULED_UNLOCK_START:
                case BS2EventCodeEnum.ZONE_SCHEDULED_UNLOCK_END:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_ALARM:
                case BS2EventCodeEnum.ZONE_SCHEDULED_LOCK_ALARM_CLEAR:
                    return GetZoneIdMsg(eventLog);
                case BS2EventCodeEnum.SUPERVISED_INPUT_OPEN:
                case BS2EventCodeEnum.SUPERVISED_INPUT_SHORT:
                case BS2EventCodeEnum.DEVICE_INPUT_DETECTED:
                    return GetIOInfoMsg(eventLog);
                case BS2EventCodeEnum.USER_ENROLL_SUCCESS:
                case BS2EventCodeEnum.USER_ENROLL_FAIL:
                case BS2EventCodeEnum.USER_UPDATE_SUCCESS:
                case BS2EventCodeEnum.USER_UPDATE_FAIL:
                case BS2EventCodeEnum.USER_DELETE_SUCCESS:
                case BS2EventCodeEnum.USER_DELETE_FAIL:
                case BS2EventCodeEnum.USER_ISSUE_AOC_SUCCESS:
                case BS2EventCodeEnum.USER_DUPLICATE_CREDENTIAL:
                case BS2EventCodeEnum.USER_UPDATE_PARTIAL_SUCCESS:
                case BS2EventCodeEnum.USER_UPDATE_PARTIAL_FAIL:
                    return GetUserIdMsg(eventLog);
                case BS2EventCodeEnum.VERIFY_SUCCESS:
                case BS2EventCodeEnum.VERIFY_FAIL:
                case BS2EventCodeEnum.VERIFY_DURESS:
                case BS2EventCodeEnum.IDENTIFY_SUCCESS:
                case BS2EventCodeEnum.IDENTIFY_FAIL:
                case BS2EventCodeEnum.IDENTIFY_DURESS:
                    return GetUserIdAndTnaKeyMsg(eventLog);
                case BS2EventCodeEnum.RELAY_ACTION_ON:
                case BS2EventCodeEnum.RELAY_ACTION_OFF:
                case BS2EventCodeEnum.RELAY_ACTION_KEEP:
                    return GetRelayActionMsg(eventLog);
                default:
                    return GetGeneralMsg(eventLog);
            }
#endif
        }

        private static string GetDoorIdMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            BS2EventDetail eventDetail = ConvertTo<BS2EventDetail>(eventLog.userID);

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] doorID[{4}] image[{5}]", 
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                eventDetail.doorID,
                                Convert.ToBoolean(eventLog.image & (byte)BS2EventImageBitPos.BS2_IMAGEFIELD_POS_IMAGE));
        }

        private static string GetZoneIdMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            BS2EventDetail eventDetail = ConvertTo<BS2EventDetail>(eventLog.userID);

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] zoneID[{4}] image[{5}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                eventDetail.zoneID,
                                Convert.ToBoolean(eventLog.image & (byte)BS2EventImageBitPos.BS2_IMAGEFIELD_POS_IMAGE));
        }

        private static string GetIOInfoMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            BS2EventDetail eventDetail = ConvertTo<BS2EventDetail>(eventLog.userID);

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] device[{4, 10}] port[{5}] value[{6}] image[{7}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                eventDetail.ioDeviceID,
                                eventDetail.port,
                                eventDetail.value,
                                Convert.ToBoolean(eventLog.image & (byte)BS2EventImageBitPos.BS2_IMAGEFIELD_POS_IMAGE));
        }

        private static string GetUserIdMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            string userID = System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0');
            if (userID.Length == 0)
            {
                userID = "unknown";
            }

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] userID[{4}] image[{5}] where[{6}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                userID,
                                Convert.ToBoolean(eventLog.image & (byte)BS2EventImageBitPos.BS2_IMAGEFIELD_POS_IMAGE),
                                Convert.ToBoolean(eventLog.param) ? "Device" : "Server");
        }

        private static string GetUserIdAndTnaKeyMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            string userID = System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0');            

            if (userID.Length == 0)
            {
                userID = "unknown";
            }

            string subMsg = "";
            if ((BS2EventCodeEnum)eventLog.code != BS2EventCodeEnum.VERIFY_FAIL_CARD)
            {
                BS2TNAKeyEnum tnaKeyEnum = (BS2TNAKeyEnum)eventLog.param;
                if (tnaKeyEnum != BS2TNAKeyEnum.UNSPECIFIED)
                {
                    subMsg = String.Format("userID[{0}] T&A[{1}]", userID, tnaKeyEnum.ToString());
                }
                else
                {
                    subMsg = String.Format("userID[{0}]", userID);
                }
            }
            else
            {
                subMsg = String.Format("cardID[{0}]", userID);
            }

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] {4} image[{5}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                subMsg,
                                Convert.ToBoolean(eventLog.image & (byte)BS2EventImageBitPos.BS2_IMAGEFIELD_POS_IMAGE));
        }

        private static string GetGeneralMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] image[{4}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                Convert.ToBoolean(eventLog.image & (byte)BS2EventImageBitPos.BS2_IMAGEFIELD_POS_IMAGE));
        }

        private static string GetRelayActionMsg(BS2Event eventLog)
        {
            DateTime eventTime = ConvertFromUnixTimestamp(eventLog.dateTime);
            BS2EventDetail eventDetail = ConvertTo<BS2EventDetail>(eventLog.userID);

            return String.Format("Log => device[{0, 10}] : timestamp[{1}] event id[{2, 10}] event code[{3}] inputPort[{4}] relayPort[{5}]",
                                eventLog.deviceID,
                                eventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                eventLog.id,
                                (BS2EventCodeEnum)eventLog.code,
                                eventDetail.relayActionInputPort,
                                eventDetail.relayActionRelayPort);
        }

        public static void HighlightLine(string fullStr, string highlightStr, ConsoleColor highlightColor = ConsoleColor.Green)
        {
            int first = fullStr.IndexOf(highlightStr);
            int len = highlightStr.Length;
            int second = first + len;
            var oldColor = Console.ForegroundColor;

            Console.Write(fullStr.Substring(0, first));
            Console.ForegroundColor = highlightColor;
            Console.Write(fullStr.Substring(first, len));
            Console.ForegroundColor = oldColor;
            Console.WriteLine(fullStr.Substring(second));
        }

        public static void HighlightLineMulti(string fullStr, params object[] highlightStrObj)
        {
            int before = 0;
            var oldColor = Console.ForegroundColor;
            foreach (object str in highlightStrObj)
            {
                string tempStr = str.ToString();
                int first = fullStr.IndexOf(tempStr);
                int len = tempStr.ToString().Length;

                Console.ForegroundColor = oldColor;
                Console.Write(fullStr.Substring(before, first - before));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(fullStr.Substring(first, len));
                before = first + len;
            }

            Console.ForegroundColor = oldColor;
            Console.WriteLine(fullStr.Substring(before));
        }

        public static UInt32 GetInputMasterOrSlaveID(UInt32 masterID)
        {
            UInt32 id = 0;

            Console.WriteLine(">>>> Do you want to process it with master ID? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                id = masterID;
            }
            else
            {
                id = GetInputSlaveID();
            }

            return id;
        }

        public static UInt32 GetInputSlaveID()
        {
            UInt32 id = 0;

            Console.WriteLine(">>>> Select the slave ID:");
            Console.Write(">>>> ");
            id = (UInt32)Util.GetInput();

            return id;
        }


        public static List<string> GetHostIPAddresses()
        {
            List<string> ipAddrs = new List<string>();
            NetworkInterface[] allInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface netInterface in allInterfaces)
            {
                if (netInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    netInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    netInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipProp = netInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipInfo in ipProp.UnicastAddresses)
                    {
                        if (ipInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            ipAddrs.Add(ipInfo.Address.ToString());
                    }
                }
            }

            return ipAddrs;
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
    }
}
