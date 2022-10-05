using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class ZoneControl : FunctionModule
    {
        private API.OnReadyToScan cbCardOnReadyToScan = null;

        delegate int ClearZoneStatusDelegate(IntPtr context, UInt32 deviceId, UInt32 zoneID, IntPtr uids, UInt32 uidCount);
        delegate int ClearAllZoneStatusDelegate(IntPtr context, UInt32 deviceId, UInt32 zoneID);
        delegate int GetZoneDelegate(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneObj, out UInt32 numZone);
        delegate int GetZoneStatusDelegate(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);
        delegate int GetALLZoneDelegate(IntPtr context, UInt32 deviceId, out IntPtr zoneObj, out UInt32 numZone);
        delegate int GetZoneAllStatusDelegate(IntPtr context, UInt32 deviceId, out IntPtr zoneStatusObj, out UInt32 numZoneStatus);
        delegate int SetZoneAlarmDelegate(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);
        delegate int SetZoneArmDelegate(IntPtr context, UInt32 deviceId, byte alarmed, IntPtr zoneIds, UInt32 zoneIdCount);
        delegate int RemoveZoneDelegate(IntPtr context, UInt32 deviceId, IntPtr zoneIds, UInt32 zoneIdCount);
        delegate int RemoveAllZoneDelegate(IntPtr context, UInt32 deviceId);
        delegate void PrintDelegate<T>(IntPtr context, T item);

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (isMasterDevice)
            {
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get anti-passback zone", getAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get anti-passback zone status", getAPBZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set anti-passback zone", setAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set anti-passback zone alarm", setAPBZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove anti-passback zone", removeAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Clear anti-passback zone status", clearAPBZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get timed anti-passback zone", getTimedAPBZone));
                
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get timed anti-passback zone status", getTimedAPBZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set timed anti-passback zone", setTimedAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set timed anti-passback zone alarm", setTimedAPBZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove timed anti-passback zone", removeTimedAPBZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Clear timed anti-passback zone status", clearTimedAPBZoneStatus));
                
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get fire alarm zone", getFireAlarmZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get fire alarm zone status", getFireAlarmZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set fire alarm zone", setFireAlarmZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set fire alarm zone alarm", setFireAlarmZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove fire alarm zone", removeFireAlarmZone));
               
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get scheduled lock/unlock zone", getScheduledLockUnlockZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get scheduled lock/unlock zone status", getScheduledLockUnlockZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set scheduled lock/unlock zone", setScheduledLockUnlockZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set scheduled lock/unlock zone alarm", setScheduledLockUnlockZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove scheduled lock/unlock zone", removeScheduledLockUnlockZone));

                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Intrusion alarm zone", getIntrusionAlarmZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Intrusion alarm zone status", getIntrusionAlarmZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Intrusion alarm zone", setIntrusionAlarmZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Intrusion alarm zone alarm", setIntrusionAlarmZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove Intrusion alarm zone", removeIntrusionAlarmZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Intrusion alarm zone arm", setIntrusionAlarmZoneArm));

                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Interlock zone", getInterlockZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Interlock zone status", getInterlockZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get All Interlock zone status", getAllInterlockZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Interlock zone", setInterlockZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Interlock zone alarm", setInterlockZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove Interlock zone", removeInterlockZone));

                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get lift scheduled lock/unlock zone", getLiftLockUnlockZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get lift scheduled lock/unlock zone status", getLiftLockUnlockZoneStatus));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set lift scheduled lock/unlock zone", setLiftLockUnlockZone));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set lift scheduled lock/unlock zone alarm", setLiftLockUnlockZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove lift scheduled lock/unlock zone", removeLiftLockUnlockZone));
            }

            return functionList;
        }

        void getAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2AntiPassbackZone>(sdkContext, deviceID, "anti-passback", API.BS2_GetAllAntiPassbackZone, API.BS2_GetAntiPassbackZone, print);            
        }

        void getAPBZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "anti-passback", API.BS2_GetAntiPassbackZoneStatus);
        }

        void setAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many anti-passback zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2AntiPassbackZone> apbList = new List<BS2AntiPassbackZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2AntiPassbackZone zone = Util.AllocateStructure<BS2AntiPassbackZone>();

                Console.WriteLine("Enter a value for anti-passback zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Environment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Which type is this anti-passback zone? [{0} : {1}, {2} : {3}(default)", (byte)BS2APBZoneTypeEnum.HARD, BS2APBZoneTypeEnum.HARD, (byte)BS2APBZoneTypeEnum.SOFT, BS2APBZoneTypeEnum.SOFT);
                Console.Write("  >>>> ");
                zone.type = Util.GetInput((byte)BS2APBZoneTypeEnum.SOFT);

                Console.WriteLine("  Do you want to activate this anti-passback zone? [Y/n]");
                Console.Write("  >>>> ");
                if(Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                Console.WriteLine("  How many seconds should this anti-passback zone be reset after? [0(default): No Reset]");
                Console.Write("  >>>> ");
                zone.resetDuration = Util.GetInput((UInt32)0);

                for (int loop = 0; loop < BS2Environment.BS2_MAX_APB_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                    zone.alarm[loop].delay = 0;
                    zone.alarm[loop].stopFlag = 0;
                    
                }

                Console.WriteLine("  How many alarms for this anti-passback zone do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_APB_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                BS2RelayAction relay = Util.AllocateStructure<BS2RelayAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming relay control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.RELAY;                    

                    Console.WriteLine("  Enter the relay port on this device.[0(default)]");
                    Console.Write("  >>>> ");
                    relay.relayIndex = Util.GetInput(0);

                    relay.signal.signalID = 1;
                    Console.WriteLine("  How many play the relay action on this device?[1(default)]");
                    Console.Write("  >>>> ");
                    
                    relay.signal.count = Util.GetInput((UInt16)1);

                    Console.WriteLine("  Enter the active duration.[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.onDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  Enter the deactive duration.[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.offDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  How many waiting for to a next action?[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.delay = Util.GetInput((UInt16)100);     
                           
                    byte[] inputActionArray = Util.ConvertTo<BS2RelayAction>(ref relay);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                zone.numReaders = 0;
                Console.WriteLine("  Enter the ID of the readers which are used to enter into this anti-passback zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] readerIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string readerID in readerIDs)
                {
                    if (readerID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(readerID, out item))
                        {
                            if (zone.numReaders + 1 >= BS2Environment.BS2_MAX_READERS_PER_APB_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of reader id should less than {0}.", BS2Environment.BS2_MAX_READERS_PER_APB_ZONE);
                                break;
                            }

                            zone.readers[zone.numReaders].deviceID = item;
                            zone.readers[zone.numReaders].type = (byte)BS2APBZoneReaderTypeEnum.ENTRY;
                            zone.numReaders++;
                        }
                    }
                }

                if (zone.numReaders + 1 < BS2Environment.BS2_MAX_READERS_PER_APB_ZONE)
                {
                    Console.WriteLine("  Enter the ID of the readers which are used to exit into this anti-passback zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    readerIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string readerID in readerIDs)
                    {
                        if (readerID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(readerID, out item))
                            {
                                if (zone.numReaders + 1 >= BS2Environment.BS2_MAX_READERS_PER_APB_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of reader should less than {0}.", BS2Environment.BS2_MAX_READERS_PER_APB_ZONE);
                                    break;
                                }

                                zone.readers[zone.numReaders].deviceID = item;
                                zone.readers[zone.numReaders].type = (byte)BS2APBZoneReaderTypeEnum.EXIT;
                                zone.numReaders++;
                            }
                        }
                    }
                }

                zone.numBypassGroups = 0;
                Console.WriteLine("  Enter the ID of the access group which can bypass this anti-passback zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            if (zone.numBypassGroups + 1 >= BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of bypass access group should less than {0}.", BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE);
                                break;
                            }

                            zone.bypassGroupIDs[zone.numBypassGroups] = item;
                            zone.numBypassGroups++;
                        }
                    }
                }

                apbList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2AntiPassbackZone));
            IntPtr apbListObj = Marshal.AllocHGlobal(structSize * apbList.Count);
            IntPtr curApbListObj = apbListObj;
            foreach (BS2AntiPassbackZone item in apbList)
            {
                Marshal.StructureToPtr(item, curApbListObj, false);
                curApbListObj = (IntPtr)((long)curApbListObj + structSize);
            }

            Console.WriteLine("Trying to set anti-passback zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAntiPassbackZone(sdkContext, deviceID, apbListObj, (UInt32)apbList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(apbListObj);
        }

        void setAPBZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "anti-passback", API.BS2_SetAntiPassbackZoneAlarm);
        }

        void removeAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "anti-passback", API.BS2_RemoveAllAntiPassbackZone, API.BS2_RemoveAntiPassbackZone);
        }

        void clearAPBZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            clearZoneStatus(sdkContext, deviceID, "anti-passback", API.BS2_ClearAllAntiPassbackZoneStatus, API.BS2_ClearAntiPassbackZoneStatus);
        }

        void getTimedAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2TimedAntiPassbackZone>(sdkContext, deviceID, "timed anti-passback", API.BS2_GetAllTimedAntiPassbackZone, API.BS2_GetTimedAntiPassbackZone, print);            
        }

        void getTimedAPBZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "timed anti-passback", API.BS2_GetTimedAntiPassbackZoneStatus);
        }

        void setTimedAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many timed anti-passback zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2TimedAntiPassbackZone> apbList = new List<BS2TimedAntiPassbackZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2TimedAntiPassbackZone zone = Util.AllocateStructure<BS2TimedAntiPassbackZone>();

                Console.WriteLine("Enter a value for timed anti-passback zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Environment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Which type is this timed anti-passback zone? [{0} : {1}, {2} : {3}(default)", (byte)BS2APBZoneTypeEnum.HARD, BS2APBZoneTypeEnum.HARD, (byte)BS2APBZoneTypeEnum.SOFT, BS2APBZoneTypeEnum.SOFT);
                Console.Write("  >>>> ");
                zone.type = Util.GetInput((byte)BS2APBZoneTypeEnum.SOFT);

                Console.WriteLine("  Do you want to activate this timed anti-passback zone? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                Console.WriteLine("  How many seconds should this timed anti-passback zone be reset after? [0(default): No Reset]");
                Console.Write("  >>>> ");
                zone.resetDuration = Util.GetInput((UInt32)0);

                for (int loop = 0; loop < BS2Environment.BS2_MAX_APB_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                    zone.alarm[loop].stopFlag = 0;
                    zone.alarm[loop].delay = 0;
                }

                Console.WriteLine("  How many alarms for this timed anti-passback zone do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_TIMED_APB_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                //BS2OutputPortAction ttl = Util.AllocateStructure<BS2OutputPortAction>();
                BS2RelayAction relay = Util.AllocateStructure<BS2RelayAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming ttl control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.RELAY;

                    Console.WriteLine("  Enter the relay port on this device.[0(default)]");
                    Console.Write("  >>>> ");
                    relay.relayIndex = Util.GetInput(0);

                    relay.signal.signalID = 1;
                    Console.WriteLine("  How many play the relay action on this device?[1(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.count = Util.GetInput((UInt16)1);

                    Console.WriteLine("  Enter the active duration.[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.onDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  Enter the deactive duration.[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.offDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  How many waiting for to a next action?[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.delay = Util.GetInput((UInt16)100);

                    byte[] inputActionArray = Util.ConvertTo<BS2RelayAction>(ref relay);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);

                    //byte[] inputActionArray = Util.ConvertTo<BS2OutputPortAction>(ref ttl);
                    //Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    //Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                zone.numReaders = 0;
                Console.WriteLine("  Enter the ID of the readers which are used to enter into this timed anti-passback zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] readerIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string readerID in readerIDs)
                {
                    if (readerID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(readerID, out item))
                        {
                            if (zone.numReaders + 1 >= BS2Environment.BS2_MAX_READERS_PER_TIMED_APB_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of reader id should less than {0}.", BS2Environment.BS2_MAX_READERS_PER_TIMED_APB_ZONE);
                                break;
                            }

                            zone.readers[zone.numReaders].deviceID = item;
                            zone.numReaders++;
                        }
                    }
                }

                /*
                if (zone.numReaders + 1 < BS2Environment.BS2_MAX_READERS_PER_TIMED_APB_ZONE)
                {
                    Console.WriteLine("  Enter the ID of the readers which are used to exit into this timed anti-passback zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    readerIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string readerID in readerIDs)
                    {
                        if (readerID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(readerID, out item))
                            {
                                if (zone.numReaders + 1 >= BS2Environment.BS2_MAX_READERS_PER_TIMED_APB_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of reader id should less than {0}.", BS2Environment.BS2_MAX_READERS_PER_TIMED_APB_ZONE);
                                    break;
                                }

                                zone.readers[zone.numReaders].deviceID = item;
                                zone.readers[zone.numReaders].type = (byte)BS2APBZoneReaderTypeEnum.EXIT;
                                zone.numReaders++;
                            }
                        }
                    }
                }
                */

                zone.numBypassGroups = 0;
                Console.WriteLine("  Enter the ID of the access group which can bypass this timed anti-passback zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            if (zone.numBypassGroups + 1 >= BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_TIMED_APB_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of bypass access group should less than {0}.", BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_TIMED_APB_ZONE);
                                break;
                            }

                            zone.bypassGroupIDs[zone.numBypassGroups] = item;
                            zone.numBypassGroups++;
                        }
                    }
                }

                apbList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2TimedAntiPassbackZone));
            IntPtr apbListObj = Marshal.AllocHGlobal(structSize * apbList.Count);
            IntPtr curApbListObj = apbListObj;
            foreach (BS2TimedAntiPassbackZone item in apbList)
            {
                Marshal.StructureToPtr(item, curApbListObj, false);
                curApbListObj = (IntPtr)((long)curApbListObj + structSize);
            }

            Console.WriteLine("Trying to set timed anti-passback zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetTimedAntiPassbackZone(sdkContext, deviceID, apbListObj, (UInt32)apbList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(apbListObj);
        }

        void setTimedAPBZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "timed anti-passback", API.BS2_SetTimedAntiPassbackZoneAlarm);
        }

        void removeTimedAPBZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "timed anti-passback", API.BS2_RemoveAllTimedAntiPassbackZone, API.BS2_RemoveTimedAntiPassbackZone);
        }

        void clearTimedAPBZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            clearZoneStatus(sdkContext, deviceID, "timed anti-passback", API.BS2_ClearAllTimedAntiPassbackZoneStatus, API.BS2_ClearTimedAntiPassbackZoneStatus);
        }

        void getFireAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2FireAlarmZone>(sdkContext, deviceID, "fire alarm", API.BS2_GetAllFireAlarmZone, API.BS2_GetFireAlarmZone, print);            
        }

        void getFireAlarmZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "fire alarm", API.BS2_GetFireAlarmZoneStatus);
        }

        void setFireAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many fire alarm zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2FireAlarmZone> fazList = new List<BS2FireAlarmZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2FireAlarmZone zone = Util.AllocateStructure<BS2FireAlarmZone>();

                Console.WriteLine("Enter a value for fire alarm zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Environment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Do you want to activate this fire alarm zone? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                for (int loop = 0; loop < BS2Environment.BS2_MAX_FIRE_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                    zone.alarm[loop].stopFlag = 0;
                    zone.alarm[loop].delay = 0;
                    
                }

                Console.WriteLine("  How many alarms for this fire alarm zone do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_FIRE_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                BS2SoundAction sound = Util.AllocateStructure<BS2SoundAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming sound control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.SOUND;

                    Console.WriteLine("  How many play the sound action on this device?[1(default)]");
                    Console.Write("  >>>> ");
                    sound.count = Util.GetInput((byte)1);

                    Console.WriteLine("  Enter the sound index.[{0}(default) : {1}, {2} : {3}, {4} : {5}]", (UInt16)BS2SoundIndexEnum.WELCOME, BS2SoundIndexEnum.WELCOME, (UInt16)BS2SoundIndexEnum.AUTH_SUCCESS, BS2SoundIndexEnum.AUTH_SUCCESS, (UInt16)BS2SoundIndexEnum.AUTH_FAIL, BS2SoundIndexEnum.AUTH_FAIL);
                    Console.Write("  >>>> ");
                    sound.soundIndex = Util.GetInput((UInt16)BS2SoundIndexEnum.WELCOME);

                    byte[] inputActionArray = Util.ConvertTo<BS2SoundAction>(ref sound);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                Console.WriteLine("  How many fire sensors for this fire alarm zone do you want to set? [1(default)-{0}]", BS2Environment.BS2_MAX_FIRE_SENSORS_PER_FIRE_ALARM_ZONE);
                Console.Write("  >>>> ");
                zone.numSensors = Util.GetInput((byte)1);

                for (byte loop = 0; loop < zone.numSensors; ++loop)
                {
                    Console.WriteLine("  Enter the ID of the fire sensor for this fire alarm zone:");
                    Console.Write("  >>>> ");
                    zone.sensor[loop].deviceID = (UInt32)Util.GetInput();

                    Console.WriteLine("  Enter the port of the fire sensor for this fire alarm zone: [0(default)]");
                    Console.Write("  >>>> ");
                    zone.sensor[loop].port = (byte)Util.GetInput((byte)0);

                    Console.WriteLine("  Enter the switch type of the fire sensor for this fire alarm zone: [{0}(default) : {1}, {2} : {3}]", (byte)BS2SwitchTypeEnum.NORMAL_CLOSE, BS2SwitchTypeEnum.NORMAL_CLOSE, (byte)BS2SwitchTypeEnum.NORMAL_OPEN, BS2SwitchTypeEnum.NORMAL_OPEN);
                    Console.Write("  >>>> ");
                    zone.sensor[loop].switchType = (byte)Util.GetInput((byte)0);

                    Console.WriteLine("  Enter the duration of the fire sensor for this fire alarm zone: [{100}(default)]");
                    Console.Write("  >>>> ");
                    zone.sensor[loop].duration = (UInt16)Util.GetInput((UInt16)100);
                }

                zone.numDoors = 0;
                Console.WriteLine("  Enter the ID of the door which belong to this fire alarm zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] doorIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string doorID in doorIDs)
                {
                    if (doorID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(doorID, out item))
                        {
                            if (zone.numDoors + 1 >= BS2Environment.BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of doorID should less than {0}.", BS2Environment.BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE);
                                break;
                            }

                            zone.doorIDs[zone.numDoors] = item;
                            zone.numDoors++;
                        }
                    }
                }

                fazList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2FireAlarmZone));
            IntPtr fazListObj = Marshal.AllocHGlobal(structSize * fazList.Count);
            IntPtr curFazListObj = fazListObj;
            foreach (BS2FireAlarmZone item in fazList)
            {
                Marshal.StructureToPtr(item, curFazListObj, false);
                curFazListObj = (IntPtr)((long)curFazListObj + structSize);
            }

            Console.WriteLine("Trying to set fire alarm zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetFireAlarmZone(sdkContext, deviceID, fazListObj, (UInt32)fazList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(fazListObj);
        }

        void setFireAlarmZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "fire alarm", API.BS2_SetFireAlarmZoneAlarm);
        }

        void removeFireAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "fire alarm", API.BS2_RemoveAllFireAlarmZone, API.BS2_RemoveFireAlarmZone);
        }

        void getScheduledLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2ScheduledLockUnlockZone>(sdkContext, deviceID, "scheduled lock/unlock", API.BS2_GetAllScheduledLockUnlockZone, API.BS2_GetScheduledLockUnlockZone, print);            
        }

        void getScheduledLockUnlockZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "scheduled lock/unlock", API.BS2_GetScheduledLockUnlockZoneStatus);
        }

        void setScheduledLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many scheduled lock/unlock zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2ScheduledLockUnlockZone> slulList = new List<BS2ScheduledLockUnlockZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2ScheduledLockUnlockZone zone = Util.AllocateStructure<BS2ScheduledLockUnlockZone>();

                Console.WriteLine("Enter a value for scheduled lock/unlock zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Environment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Enter the ID of access schedule to lock this scheduled lock/unlock zone: [{0}(default) : {1}]", (UInt32)BS2ScheduleIDEnum.ALWAYS, BS2ScheduleIDEnum.ALWAYS);
                Console.Write("  >>>> ");
                zone.lockScheduleID = Util.GetInput((UInt32)BS2ScheduleIDEnum.ALWAYS);

                Console.WriteLine("  Enter the ID of access schedule to unlock this scheduled lock/unlock zone: [{0}(default) : {1}]", (UInt32)BS2ScheduleIDEnum.NEVER, BS2ScheduleIDEnum.NEVER);
                Console.Write("  >>>> ");
                zone.unlockScheduleID = Util.GetInput((UInt32)BS2ScheduleIDEnum.NEVER);                

                Console.WriteLine("  Do you want to activate this scheduled lock/unlock zone? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                for (int loop = 0; loop < BS2Environment.BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                    zone.alarm[loop].stopFlag = 0;
                    zone.alarm[loop].delay = 0;
                }

                Console.WriteLine("  How many alarms for this scheduled lock/unlock do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                BS2BuzzerAction buzzer = Util.AllocateStructure<BS2BuzzerAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming buzzer control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.BUZZER;

                    buzzer.count = 5;
                    Console.WriteLine("  Enter the type of buzzer tone.[{0} : {1}, {2} : {3}, {4} : {5}, {6}(default) : {7}]", 
                                    (byte)BS2BuzzerToneEnum.OFF,
                                    BS2BuzzerToneEnum.OFF,
                                    (byte)BS2BuzzerToneEnum.LOW,
                                    BS2BuzzerToneEnum.LOW,
                                    (byte)BS2BuzzerToneEnum.MIDDLE,
                                    BS2BuzzerToneEnum.MIDDLE,
                                    (byte)BS2BuzzerToneEnum.HIGH,
                                    BS2BuzzerToneEnum.HIGH);
                    Console.Write("  >>>> ");
                    buzzer.signal[0].tone = Util.GetInput((byte)BS2BuzzerToneEnum.HIGH);

                    Console.WriteLine("  Do you want to set the fade out effect for this scheduled lock/unlock zone? [y/N]");
                    Console.Write("  >>>> ");
                    if (Util.IsNo())
                    {
                        buzzer.signal[0].fadeout = 0;
                    }
                    else
                    {
                        buzzer.signal[0].fadeout = 1;
                    }

                    Console.WriteLine("  Enter the duration of buzzer for this scheduled lock/unlock zone: [{100}(default)]");
                    Console.Write("  >>>> ");
                    buzzer.signal[0].duration = (UInt16)Util.GetInput((UInt16)100);

                    Console.WriteLine("  How many waiting for to a next action?[100(default)]");
                    Console.Write("  >>>> ");
                    buzzer.signal[0].delay = Util.GetInput((UInt16)100);

                    byte[] inputActionArray = Util.ConvertTo<BS2BuzzerAction>(ref buzzer);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                zone.numDoors = 0;
                Console.WriteLine("  Enter the ID of the door which belong to this scheduled lock/unlock zone: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] doorIDs = Console.ReadLine().Split(delimiterChars);

                foreach (string doorID in doorIDs)
                {
                    if (doorID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(doorID, out item))
                        {
                            if (zone.numDoors + 1 >= BS2Environment.BS2_MAX_DOORS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)
                            {
                                Console.WriteLine("[Warning] The count of doorID should less than {0}.", BS2Environment.BS2_MAX_DOORS_IN_SCHEDULED_LOCK_UNLOCK_ZONE);
                                break;
                            }

                            zone.doorIDs[zone.numDoors] = item;
                            zone.numDoors++;
                        }
                    }
                }

                zone.bidirectionalLock = 0;
                zone.numBypassGroups = 0;
                if (zone.lockScheduleID > (UInt32)BS2ScheduleIDEnum.NEVER)
                {
                    Console.WriteLine("  Should it's door be locked bi-directionally when this scheduled lock/unlock zone is on the lock schedule? [Y/n]");
                    Console.Write("  >>>> ");
                    if (Util.IsYes())
                    {
                        zone.bidirectionalLock = 1;
                    }

                    Console.WriteLine("  Enter the ID of the access group which can bypass this scheduled lock/unlock zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string accessGroupID in accessGroupIDs)
                    {
                        if (accessGroupID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(accessGroupID, out item))
                            {
                                if (zone.numDoors + 1 >= BS2Environment.BS2_MAX_BYPASS_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of access group ID should less than {0}.", BS2Environment.BS2_MAX_BYPASS_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE);
                                    break;
                                }

                                zone.bypassGroupIDs[zone.numBypassGroups] = item;
                                zone.numBypassGroups++;
                            }
                        }
                    }
                }

                zone.numUnlockGroups = 0;
                if (zone.unlockScheduleID > (UInt32)BS2ScheduleIDEnum.NEVER)
                {
                    Console.WriteLine("  Enter the ID of the access group which can unlock this scheduled lock/unlock zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string accessGroupID in accessGroupIDs)
                    {
                        if (accessGroupID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(accessGroupID, out item))
                            {
                                if (zone.numDoors + 1 >= BS2Environment.BS2_MAX_UNLOCK_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of access group ID should less than {0}.", BS2Environment.BS2_MAX_UNLOCK_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE);
                                    break;
                                }

                                zone.unlockGroupIDs[zone.numUnlockGroups] = item;
                                zone.numUnlockGroups++;
                            }
                        }
                    }
                }

                slulList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2ScheduledLockUnlockZone));
            IntPtr slulListObj = Marshal.AllocHGlobal(structSize * slulList.Count);
            IntPtr curSlulListObj = slulListObj;
            foreach (BS2ScheduledLockUnlockZone item in slulList)
            {
                Marshal.StructureToPtr(item, curSlulListObj, false);
                curSlulListObj = (IntPtr)((long)curSlulListObj + structSize);
            }

            Console.WriteLine("Trying to set scheduled lock/unlock zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetScheduledLockUnlockZone(sdkContext, deviceID, slulListObj, (UInt32)slulList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(slulListObj);
        }

        void setScheduledLockUnlockZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "scheduled lock/unlock", API.BS2_SetScheduledLockUnlockZoneAlarm);
        }

        void removeScheduledLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "scheduled lock/unlock", API.BS2_RemoveAllScheduledLockUnlockZone, API.BS2_RemoveScheduledLockUnlockZone);
        }

        //IntrusionAlarmZone
        void getIntrusionAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2IntrusionAlarmZoneBlob[] zoneBlobs = new BS2IntrusionAlarmZoneBlob[32];
            UInt32 outNumZones = 0;

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetIntrusionAlarmZone(sdkContext, deviceID, zoneBlobs, out outNumZones);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (outNumZones > 0)
                {
                    for (UInt32 loop = 0; loop < outNumZones; ++loop)
                    {
                        print(sdkContext, zoneBlobs[loop].IntrusionAlarmZone);

                        if (zoneBlobs[loop].memberObjs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[loop].memberObjs);
                        if (zoneBlobs[loop].inputObjs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[loop].inputObjs);
                        if (zoneBlobs[loop].outputObjs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[loop].outputObjs);
                        if (zoneBlobs[loop].cardObjs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[loop].cardObjs);
                        if (zoneBlobs[loop].doorIDs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[loop].doorIDs);
                        if (zoneBlobs[loop].groupIDs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[loop].groupIDs);
                    }
                }
                else
                {
                    Console.WriteLine(">>> There is no Intrusion Alarm zone in the device.");
                }

            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
            }

        }

        void getIntrusionAlarmZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "IntrusionAlarm", API.BS2_GetIntrusionAlarmZoneStatus);
        }

        void setIntrusionAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2IntrusionAlarmZoneBlob[] zoneBlob = Util.AllocateStructureArray<BS2IntrusionAlarmZoneBlob>(1);

            Util.HighlightLine("Enter the ID of the new intrusion alarm zone", "ID");
            Console.Write(">> ");
            zoneBlob[0].IntrusionAlarmZone.zoneID = Util.GetInput((UInt32)100);

            Util.HighlightLine("Enter the zone name", "name");
            Console.Write(">> ");
            //string name = "Test Zone";
            string name = Console.ReadLine();
            Array.Clear(zoneBlob[0].IntrusionAlarmZone.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
            byte[] zoneName = Encoding.UTF8.GetBytes(name);
            Array.Copy(zoneName, 0, zoneBlob[0].IntrusionAlarmZone.name, 0, zoneName.Length);

            Util.HighlightLine("Enter the arm delay time", "arm delay", ConsoleColor.Red);
            Console.Write(">> ");
            zoneBlob[0].IntrusionAlarmZone.armDelay = Util.GetInput((byte)10);
            Util.HighlightLine("Enter the alarm delay time", "alarm delay");
            Console.Write(">> ");
            zoneBlob[0].IntrusionAlarmZone.alarmDelay = Util.GetInput((byte)10);
            Util.HighlightLine("Do you want to activate the intrusion alarm zone? [Y/n]", "activate");
            Console.Write(">> ");
            bool zoneActivate = Util.IsYes();
            zoneBlob[0].IntrusionAlarmZone.disabled = Convert.ToByte(!zoneActivate);

            Util.HighlightLine("How many devices do you want to configure the member with?", "How many devices");
            Console.Write(">> ");
            int numOfReaders = Util.GetInput(1);
            zoneBlob[0].IntrusionAlarmZone.numReaders = Convert.ToByte(numOfReaders);
            zoneBlob[0].memberObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2AlarmZoneMember)) * zoneBlob[0].IntrusionAlarmZone.numReaders);
            IntPtr curmemberObjs = zoneBlob[0].memberObjs;

            for (int idx = 0; idx < numOfReaders; idx++)
            {
                BS2AlarmZoneMember member = Util.AllocateStructure<BS2AlarmZoneMember>();
                Util.HighlightLine("  Enter the device ID which you want to run Alarm Zone Member", "device ID");
                Console.Write("  >>>> ");
                member.deviceID = (UInt32)Util.GetInput();

                Util.HighlightLine("  Enter the input type. [0: None, 1: Card, 2: Key, 255: All]", "input type");
                Console.Write("  >>>> ");
                member.inputType = Convert.ToByte(Util.GetInput((byte)255));

                Util.HighlightLine("  Enter the operation type. [0: None, 1: Arm, 2: Disarm, 3: Toggle, 4: Alarm, 8: Alarm clear]", "operation type");
                Console.Write("  >>>> ");
                member.operationType = Convert.ToByte(Util.GetInput((byte)3));

                Marshal.StructureToPtr(member, curmemberObjs, false);
                curmemberObjs += Marshal.SizeOf(typeof(BS2AlarmZoneMember));
            }

            Util.HighlightLine("How many cards do you want to register?", "How many cards");
            Console.Write(">> ");
            int numOfCards = Util.GetInput(0);
            zoneBlob[0].IntrusionAlarmZone.numCards = Convert.ToByte(numOfCards);
            zoneBlob[0].cardObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2CSNCard)) * numOfCards);
            IntPtr curCardObj = zoneBlob[0].cardObjs;

            Util.HighlightLine("Enter the device ID from which to read the card", "device ID");
            Console.Write(">> ");
            UInt32 cardReadDevice = (UInt32)Util.GetInput();
            cbCardOnReadyToScan = new API.OnReadyToScan(ReadyToScanForCard);
            int structSize = Marshal.SizeOf(typeof(BS2CSNCard));

            for (int idx = 0; idx < numOfCards; idx++)
            {
                BS2Card card = Util.AllocateStructure<BS2Card>();
                Console.WriteLine("  Trying to scan card.");
                BS2ErrorCode res = (BS2ErrorCode)API.BS2_ScanCard(sdkContext, cardReadDevice, out card, cbCardOnReadyToScan);
                if (res != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", res);
                    return;
                }
                else if (Convert.ToBoolean(card.isSmartCard))
                {
                    Console.WriteLine("CSN card is only available. Try again");
                }
                else
                {
                    Marshal.Copy(card.cardUnion, 0, curCardObj, structSize);
                    curCardObj += structSize;
                }
            }

            cbCardOnReadyToScan = null;

            Util.HighlightLine("How many access groups do you want to register?", "access groups");
            Console.Write(">> ");
            int numOfGroups = Util.GetInput(0);
            zoneBlob[0].IntrusionAlarmZone.numGroups =Convert.ToByte(numOfGroups);
            zoneBlob[0].groupIDs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UInt32)) * numOfGroups);
            IntPtr curGroup = zoneBlob[0].groupIDs;

            for (int idx = 0; idx < numOfGroups; idx++)
            {
                Util.HighlightLine("  Enter the access group ID", "access group ID");
                Console.Write("  >>>> ");
                int gID = Util.GetInput();

                Marshal.WriteInt32(curGroup, gID);
                curGroup += Marshal.SizeOf(typeof(UInt32));
            }

            Console.WriteLine("Trying to IntrusionAlarm zone.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetIntrusionAlarmZone(sdkContext, deviceID, zoneBlob, 1);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            if (zoneBlob[0].memberObjs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(zoneBlob[0].memberObjs);
            }

            if (zoneBlob[0].inputObjs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(zoneBlob[0].inputObjs);
            }

            if (zoneBlob[0].outputObjs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(zoneBlob[0].outputObjs);
            }

            if (zoneBlob[0].cardObjs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(zoneBlob[0].cardObjs);
            }

            if (zoneBlob[0].doorIDs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(zoneBlob[0].doorIDs);
            }

            if (zoneBlob[0].groupIDs != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(zoneBlob[0].groupIDs);
            }
        }

        void setIntrusionAlarmZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "IntrusionAlarm", API.BS2_SetIntrusionAlarmZoneAlarm);
        }

        void removeIntrusionAlarmZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "IntrusionAlarm", API.BS2_RemoveAllIntrusionAlarmZone, API.BS2_RemoveIntrusionAlarmZone);
        }

        void setIntrusionAlarmZoneArm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneArm(sdkContext, deviceID, "IntrusionAlarm Arm", API.BS2_SetIntrusionAlarmZoneArm);

        }

        void getZone<T>(IntPtr sdkContext, UInt32 deviceID, string zoneType, GetALLZoneDelegate getALLZoneDelegate, GetZoneDelegate getZoneDelegate, PrintDelegate<T> printDelegate)
        {
            IntPtr zoneObj = IntPtr.Zero;
            UInt32 numZone = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all {0} zones? [Y/n]", zoneType);
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all {0} zones from device.", zoneType);
                result = (BS2ErrorCode)getALLZoneDelegate(sdkContext, deviceID, out zoneObj, out numZone);             
            }
            else
            {
                Console.WriteLine("Enter the ID of the zone which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> zoneIDList = new List<UInt32>();

                foreach (string zoneID in zoneIDs)
                {
                    if (zoneID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(zoneID, out item))
                        {
                            zoneIDList.Add(item);
                        }
                    }
                }

                if (zoneIDList.Count > 0)
                {
                    IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                    IntPtr curZoneIDObj = zoneIDObj;
                    foreach (UInt32 item in zoneIDList)
                    {
                        Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                        curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                    }

                    Console.WriteLine("Trying to get {0} zones from device.", zoneType);
                    result = (BS2ErrorCode)getZoneDelegate(sdkContext, deviceID, zoneIDObj, (UInt32)zoneIDList.Count, out zoneObj, out numZone);             

                    Marshal.FreeHGlobal(zoneIDObj);
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
            else if (numZone > 0)
            {
                IntPtr curZoneObj = zoneObj;
                int structSize = Marshal.SizeOf(typeof(T));

                for (int idx = 0; idx < numZone; ++idx)
                {
                    T item = (T)Marshal.PtrToStructure(curZoneObj, typeof(T));
                    printDelegate(sdkContext, item);
                    curZoneObj = (IntPtr)((long)curZoneObj + structSize);
                }

                API.BS2_ReleaseObject(zoneObj);
            }
            else
            {
                Console.WriteLine(">>> There is no {0} zone in the device.", zoneType);
            }
        }

        void getZoneStatus(IntPtr sdkContext, UInt32 deviceID, string zoneType, GetZoneStatusDelegate getZoneStatusDelegate)
        {
            Console.WriteLine("Enter the ID of the zone which you want to get its status: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> zoneIDList = new List<UInt32>();

            foreach (string zoneID in zoneIDs)
            {
                if (zoneID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(zoneID, out item))
                    {
                        zoneIDList.Add(item);
                    }
                }
            }

            if (zoneIDList.Count > 0)
            {
                IntPtr zoneStatusObj = IntPtr.Zero;
                UInt32 numZoneStatus = 0;
                IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                IntPtr curZoneIDObj = zoneIDObj;
                foreach (UInt32 item in zoneIDList)
                {
                    Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                    curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                }

                Console.WriteLine("Trying to get the status of the {0} zone from device.", zoneType);
                BS2ErrorCode result = (BS2ErrorCode)getZoneStatusDelegate(sdkContext, deviceID, zoneIDObj, (UInt32)zoneIDList.Count, out zoneStatusObj, out numZoneStatus);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else if (numZoneStatus > 0)
                {
                    IntPtr curZoneStatusObj = zoneStatusObj;
                    int structSize = Marshal.SizeOf(typeof(BS2ZoneStatus));

                    for (int idx = 0; idx < numZoneStatus; ++idx)
                    {
                        BS2ZoneStatus item = (BS2ZoneStatus)Marshal.PtrToStructure(curZoneStatusObj, typeof(BS2ZoneStatus));
                        print(sdkContext, item);
                        curZoneStatusObj = (IntPtr)((long)curZoneStatusObj + structSize);
                    }

                    API.BS2_ReleaseObject(zoneStatusObj);
                }
                else
                {
                    Console.WriteLine(">>> There is no such {0} zone in the device.", zoneType);                    
                }

                Marshal.FreeHGlobal(zoneIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        void getAllZoneStatus(IntPtr sdkContext, UInt32 deviceID, string zoneType, GetZoneAllStatusDelegate getAllZoneStatusDelegate)
        {
            IntPtr zoneStatusObj = IntPtr.Zero;
            UInt32 numZoneStatus = 0;
            Console.WriteLine("Trying to get the status of all zones from device.", zoneType);
            BS2ErrorCode result = (BS2ErrorCode)getAllZoneStatusDelegate(sdkContext, deviceID, out zoneStatusObj, out numZoneStatus);

            IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * (int)numZoneStatus);
            IntPtr curZoneIDObj = zoneIDObj;

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else if (numZoneStatus > 0)
            {
                IntPtr curZoneStatusObj = zoneStatusObj;
                int structSize = Marshal.SizeOf(typeof(BS2ZoneStatus));

                for (int idx = 0; idx < numZoneStatus; ++idx)
                {
                    BS2ZoneStatus item = (BS2ZoneStatus)Marshal.PtrToStructure(curZoneStatusObj, typeof(BS2ZoneStatus));
                    print(sdkContext, item);
                    curZoneStatusObj = (IntPtr)((long)curZoneStatusObj + structSize);
                }

                API.BS2_ReleaseObject(zoneStatusObj);
            }
            else
            {
                Console.WriteLine(">>> There is no such {0} zone in the device.", zoneType);
            }

            Marshal.FreeHGlobal(zoneIDObj);
            
        }

        void setZoneAlarm(IntPtr sdkContext, UInt32 deviceID, string zoneType, SetZoneAlarmDelegate setZoneAlarmDelegate)
        {
            byte alarmed = 1;
            Console.WriteLine("Do you want to release the {0} zone alarm? [Y/n]", zoneType);
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                alarmed = 0;
            }

            Console.WriteLine("Enter the ID of the zone which you want to set: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> zoneIDList = new List<UInt32>();

            foreach (string zoneID in zoneIDs)
            {
                if (zoneID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(zoneID, out item))
                    {
                        zoneIDList.Add(item);
                    }
                }
            }

            if (zoneIDList.Count > 0)
            {
                IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                IntPtr curZoneIDObj = zoneIDObj;
                foreach (UInt32 item in zoneIDList)
                {
                    Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                    curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                }

                Console.WriteLine("Trying to set the alarm of the {0} zone from device.", zoneType);
                BS2ErrorCode result = (BS2ErrorCode)setZoneAlarmDelegate(sdkContext, deviceID, alarmed, zoneIDObj, (UInt32)zoneIDList.Count);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(zoneIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        void removeZone(IntPtr sdkContext, UInt32 deviceID, string zoneType, RemoveAllZoneDelegate removeAllZoneDelegate, RemoveZoneDelegate removeZoneDelegate)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all {0} zones? [Y/n]", zoneType);
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all {0} zones from device.", zoneType);
                result = (BS2ErrorCode)removeAllZoneDelegate(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the zone which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> zoneIDList = new List<UInt32>();

                foreach (string zoneID in zoneIDs)
                {
                    if (zoneID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(zoneID, out item))
                        {
                            zoneIDList.Add(item);
                        }
                    }
                }

                if (zoneIDList.Count > 0)
                {
                    IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                    IntPtr curZoneIDObj = zoneIDObj;
                    foreach (UInt32 item in zoneIDList)
                    {
                        Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                        curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove {0} zones from device.", zoneType);
                    result = (BS2ErrorCode)removeZoneDelegate(sdkContext, deviceID, zoneIDObj, (UInt32)zoneIDList.Count);

                    Marshal.FreeHGlobal(zoneIDObj);
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

        void clearZoneStatus(IntPtr sdkContext, UInt32 deviceID, string zoneType, ClearAllZoneStatusDelegate clearAllZoneStatusDelegate, ClearZoneStatusDelegate clearZoneStatusDelegate)
        {
            Console.WriteLine("Enter the ID of the zone which you want to clear.");
            Console.Write(">>>> ");
            UInt32 zoneID = (UInt32)Util.GetInput();

            if (zoneID == 0)
            {
                Console.WriteLine("The zone id should be greater than 0.");
                return;
            }

            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to clear all users? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to clear all user.");
                result = (BS2ErrorCode)clearAllZoneStatusDelegate(sdkContext, deviceID, zoneID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the user which you want to clear: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] userIDs = Console.ReadLine().Split(delimiterChars);
                List<string> userIDList = new List<string>();

                foreach (string userID in userIDs)
                {
                    if (userID.Length == 0)
                    {
                        Console.WriteLine("The user id can not be empty.");
                        return;
                    }
                    else if (userID.Length > BS2Environment.BS2_USER_ID_SIZE)
                    {
                        Console.WriteLine("The user id should less than {0} words.", BS2Environment.BS2_USER_ID_SIZE);
                        return;
                    }
                    else
                    {
                        userIDList.Add(userID);
                    }
                }

                if (userIDList.Count > 0)
                {
                    byte[] targetUid = new byte[BS2Environment.BS2_USER_ID_SIZE];
                    IntPtr userIDObj = Marshal.AllocHGlobal(BS2Environment.BS2_USER_ID_SIZE * userIDList.Count);
                    IntPtr curUserIDObj = userIDObj;
                    foreach (string item in userIDList)
                    {
                        byte[] uid = Encoding.UTF8.GetBytes(item);
                        Array.Clear(targetUid, 0, BS2Environment.BS2_USER_ID_SIZE);
                        Array.Copy(uid, 0, targetUid, 0, uid.Length);

                        Marshal.Copy(targetUid, 0, curUserIDObj, BS2Environment.BS2_USER_ID_SIZE);
                        curUserIDObj = (IntPtr)((long)curUserIDObj + BS2Environment.BS2_USER_ID_SIZE);
                    }

                    Console.WriteLine("Trying to clear user.");
                    result = (BS2ErrorCode)clearZoneStatusDelegate(sdkContext, deviceID, zoneID, userIDObj, (UInt32)userIDList.Count);

                    Marshal.FreeHGlobal(userIDObj);
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

        void setZoneArm(IntPtr sdkContext, UInt32 deviceID, string zoneType, SetZoneArmDelegate setZoneArmDelegate)
        {
            byte armed = 0;
            Console.WriteLine("Do you want to arm the {0} zone? [Y/n]", zoneType);
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                armed = 1;
            }


            Console.WriteLine("Enter the ID of the zone which you want to set: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] zoneIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> zoneIDList = new List<UInt32>();

            foreach (string zoneID in zoneIDs)
            {
                if (zoneID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(zoneID, out item))
                    {
                        zoneIDList.Add(item);
                    }
                }
            }

            if (zoneIDList.Count > 0)
            {
                IntPtr zoneIDObj = Marshal.AllocHGlobal(4 * zoneIDList.Count);
                IntPtr curZoneIDObj = zoneIDObj;
                foreach (UInt32 item in zoneIDList)
                {
                    Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                    curZoneIDObj = (IntPtr)((long)curZoneIDObj + 4);
                }

                Console.WriteLine("Trying to set the arm of the {0} zone from device.", zoneType);
                BS2ErrorCode result = (BS2ErrorCode)setZoneArmDelegate(sdkContext, deviceID, armed, zoneIDObj, (UInt32)zoneIDList.Count);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(zoneIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        void ReadyToScanForCard(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your card on the device.");
        }

        void getInterlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2InterlockZoneBlob[] zoneBlobs = new BS2InterlockZoneBlob[32];
            UInt32 outNumZones = 0;

            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetInterlockZone(sdkContext, deviceID, zoneBlobs, out outNumZones);
            if (result == BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (outNumZones > 0)
                {
                    for (int idx = 0; idx < outNumZones; idx++)
                    {
                        Console.WriteLine(">>>> InterlockZone Zone #{0}", idx);
                        print(sdkContext, zoneBlobs[idx]);

                        if (zoneBlobs[idx].doorIDs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[idx].doorIDs);
                        if (zoneBlobs[idx].inputObjs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[idx].inputObjs);
                        if (zoneBlobs[idx].outputObjs != IntPtr.Zero)
                            API.BS2_ReleaseObject(zoneBlobs[idx].outputObjs);
                    }
                }
                else
                {
                    Console.WriteLine(">>> There is no Interlock zone in the device.");
                }
            }
            else
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }

        void getInterlockZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "Interlock", API.BS2_GetInterlockZoneStatus);
        }

        void getAllInterlockZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getAllZoneStatus(sdkContext, deviceID, "Interlock", API.BS2_GetAllInterlockZoneStatus);
        }

        void setInterlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many Interlock zones do you want to set? [1(Default), ... , 32(Max)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            uint amount = Util.GetInput(1);
            if(amount > BS2Environment.BS2_MAX_INTERLOCK_ZONE)
            {
                Console.WriteLine("You cannot create more than 32 Interlock zones.");
                return;
            }

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2InterlockZoneBlob[] zoneBlob = Util.AllocateStructureArray<BS2InterlockZoneBlob>((int)amount);

                Console.WriteLine("Enter a value for the Interlock zone #{0}", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zoneBlob[idx].InterlockZone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Environment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zoneBlob[idx].InterlockZone.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zoneBlob[idx].InterlockZone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Do you want the zone {zoneID:" + zoneBlob[idx].InterlockZone.zoneID + "} to be activated  ? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    zoneBlob[idx].InterlockZone.disabled = 0;
                }
                else
                {
                    zoneBlob[idx].InterlockZone.disabled = 1;
                }

                Console.WriteLine("  How many doors do you want to add to the interlock zone?[2(Default/Min), ... , {0}(Max)", BS2Environment.BS2_MAX_DOORS_IN_INTERLOCK_ZONE);
                Console.Write("  >>>> ");
                zoneBlob[idx].InterlockZone.numDoors = Util.GetInput(2);
                if (zoneBlob[idx].InterlockZone.numDoors > BS2Environment.BS2_MAX_DOORS_IN_INTERLOCK_ZONE)
                {
                    Console.WriteLine("You cannot add more than 4 doors to the Interlock zone.");
                    return;
                }
                Console.WriteLine("  Enter the ID of the doors which you want to set: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] doorIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> doorIDList = new List<UInt32>();
                IntPtr doorIDObj = IntPtr.Zero;

                foreach (string doorID in doorIDs)
                {
                    if (doorID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(doorID, out item))
                        {
                            doorIDList.Add(item);
                        }
                    }
                }

                if (doorIDList.Count == (int)zoneBlob[idx].InterlockZone.numDoors)
                {
                    doorIDObj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UInt32)) * doorIDList.Count);
                    IntPtr curZoneIDObj = doorIDObj;
                    foreach (UInt32 item in doorIDList)
                    {
                        Marshal.WriteInt32(curZoneIDObj, (Int32)item);
                        curZoneIDObj += Marshal.SizeOf(typeof(Int32));
                    }
                    zoneBlob[idx].doorIDs = doorIDObj;
                }
                else if (doorIDList.Count < 0)
                {
                    Console.WriteLine("  Invalid parameter");
                    return;
                }
                else
                {
                    Console.WriteLine("  Please match the number of doors you have selected.");
                    return;
                }

                Console.WriteLine("  How many inputs do you want to set to the interlock zone?[0(Default), ... , {0}(Max)", BS2Environment.BS2_MAX_INPUTS_IN_INTERLOCK_ZONE);
                Console.Write("  >>>> ");
                zoneBlob[idx].InterlockZone.numInputs = Util.GetInput(0);
                if (zoneBlob[idx].InterlockZone.numInputs > BS2Environment.BS2_MAX_INPUTS_IN_INTERLOCK_ZONE)
                {
                    Console.WriteLine("   You cannot add more than {0} doors to the Interlock zone.", BS2Environment.BS2_MAX_INPUTS_IN_INTERLOCK_ZONE);
                    return;
                }

                IntPtr inputObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2InterlockZoneInput)) * zoneBlob[idx].InterlockZone.numInputs);
                IntPtr curInputObjs = inputObjs;

                for (int inputIdx = 0; inputIdx < zoneBlob[idx].InterlockZone.numInputs; inputIdx++)
                {
                    BS2InterlockZoneInput input = Util.AllocateStructure<BS2InterlockZoneInput>();
                    Console.WriteLine("  Enter the ID of the device which will be used for the input.");
                    Console.Write("  >>>> ");
                    input.deviceID = (UInt32)Util.GetInput();
                    Console.WriteLine("  Enter the index of the input port which will be used for the input.[0(Default)]");
                    Console.Write("  >>>> ");
                    input.port = (byte)Util.GetInput(0);
                    Console.WriteLine("  Enter the minimum contact duration(ms) of the input port.[100(Default)]");
                    Console.Write("  >>>> ");
                    input.duration = (byte)Util.GetInput(100);
                    Console.WriteLine("  Enter the switch type of the input port selected above.[0({0}:Default), 1({1})]", (BS2SwitchTypeEnum)BS2SwitchTypeEnum.NORMAL_OPEN, (BS2SwitchTypeEnum)BS2SwitchTypeEnum.NORMAL_CLOSE);
                    Console.Write("  >>>> ");
                    input.switchType = (byte)Util.GetInput(0);
                    Console.WriteLine("  Enter the operation type of the input port selected above.[0({0}), 1({1}), 2({2}), 255({3}:Default)]", BS2OperationTypeEnum.INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_NONE,
                                                                                                                                                BS2OperationTypeEnum.INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_ENRTY,
                                                                                                                                                BS2OperationTypeEnum.INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_EXIT,
                                                                                                                                                BS2OperationTypeEnum.INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_ALL);
                    Console.Write("  >>>> ");
                    input.operationType = (byte)Util.GetInput((byte)255);

                    Marshal.StructureToPtr(input, curInputObjs, true);
                    curInputObjs += Marshal.SizeOf(typeof(BS2InterlockZoneInput));
                }
                zoneBlob[idx].inputObjs = inputObjs;

                Console.WriteLine("  How many outputs do you want to set to the interlock zone?[0(Default), ... , {0}(Max)", BS2Environment.BS2_MAX_OUTPUTS_IN_INTERLOCK_ZONE);
                Console.Write("  >>>> ");
                zoneBlob[idx].InterlockZone.numOutputs = Util.GetInput(0);
                if (zoneBlob[idx].InterlockZone.numOutputs > BS2Environment.BS2_MAX_OUTPUTS_IN_INTERLOCK_ZONE)
                {
                    Console.WriteLine("  You cannot add more than {0} doors to the Interlock zone.", BS2Environment.BS2_MAX_OUTPUTS_IN_INTERLOCK_ZONE);
                    return;
                }

                IntPtr outputObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2InterlockZoneOutput)) * zoneBlob[idx].InterlockZone.numOutputs);
                IntPtr curOutputObjs = outputObjs;

                for (int outputIdx = 0; outputIdx < zoneBlob[idx].InterlockZone.numOutputs; outputIdx++)
                {
                    BS2InterlockZoneOutput output = Util.AllocateStructure<BS2InterlockZoneOutput>();
                    Console.WriteLine("  Enter the number of the eventcode which will trigger the output.[41472({0}:Default) 41728({1})]", BS2EventCodeEnum.ZONE_INTERLOCK_ALARM_DOOR_OPEN_DENIED,
                                                                                                                                            BS2EventCodeEnum.ZONE_INTERLOCK_ALARM_INDOOR_DENIED);
                    Console.Write("  >>>> ");
                    output.eventcode = (UInt16)Util.GetInput(41472);

                    Console.WriteLine("  Enter the ID of the device which will be used for the output action.");
                    Console.Write("  >>>> ");
                    output.action.deviceID = (UInt32)Util.GetInput();
                    Console.WriteLine("  Enter the action type of the output.[6({0}:Default), 7({1})]", BS2ActionTypeEnum.RELAY, BS2ActionTypeEnum.TTL);
                    Console.Write("  >>>> ");
                    output.action.type = (byte)Util.GetInput(6);
                    Console.WriteLine("  Enter the stopFlag of the output.[0(Default)]");
                    Console.Write("  >>>> ");
                    output.action.stopFlag = (byte)Util.GetInput(0);
                    Console.WriteLine("  Enter the delay time(ms) before the output action starts.[100(Default)]");
                    Console.Write("  >>>> ");
                    output.action.delay = (byte)Util.GetInput(100);

                    if (output.action.type == (byte)BS2ActionTypeEnum.RELAY)
                    {
                        BS2RelayAction relayAction = Util.AllocateStructure<BS2RelayAction>();
                        Console.WriteLine("  Enter the index of the relay port which will be used for the relay action.[0(Default)]");
                        Console.Write("  >>>> ");
                        relayAction.relayIndex = (byte)Util.GetInput(0);
                        Console.WriteLine("  Enter the ID of the signal which will be used for the relay action.");
                        Console.Write("  >>>> ");
                        relayAction.signal.signalID = (byte)Util.GetInput();
                        Console.WriteLine("  Enter the count of the signal which will be used for the relay action.[3(Default)]");
                        Console.Write("  >>>> ");
                        relayAction.signal.count = (UInt16)Util.GetInput(3);
                        Console.WriteLine("  Enter the onDuration of the signal which will be used for the relay action.[1000(Default)]");
                        Console.Write("  >>>> ");
                        relayAction.signal.onDuration = (UInt16)Util.GetInput(1000);
                        Console.WriteLine("  Enter the offDuration of the signal which will be used for the relay action.[1000(Default)]");
                        Console.Write("  >>>> ");
                        relayAction.signal.offDuration = (UInt16)Util.GetInput(1000);
                        Console.WriteLine("  Enter the delay of the signal which will be used for the relay action.[100(Default)]");
                        Console.Write("  >>>> ");
                        relayAction.signal.delay = (UInt16)Util.GetInput(100);
                        IntPtr tempRelayObj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2RelayAction)));
                        Marshal.StructureToPtr(relayAction, tempRelayObj, true);
                        Marshal.Copy(tempRelayObj, output.action.actionUnion, 0, Marshal.SizeOf(typeof(BS2RelayAction)));
                    }

                    if (output.action.type == (byte)BS2ActionTypeEnum.TTL)
                    {
                        BS2OutputPortAction outputPortAction = Util.AllocateStructure<BS2OutputPortAction>();
                        Console.WriteLine("  Enter the index of the relay port which will be used for the TTL output action.[0(Default)]");
                        Console.Write("  >>>> ");
                        outputPortAction.portIndex = (byte)Util.GetInput(0);
                        Console.WriteLine("  Enter the ID of the signal which will be used for the TTL output action");
                        Console.Write("  >>>> ");
                        outputPortAction.signal.signalID = (byte)Util.GetInput();
                        Console.WriteLine("  Enter the count of the signal which will be used for the TTL output action.[3(Default)]");
                        Console.Write("  >>>> ");
                        outputPortAction.signal.count = (UInt16)Util.GetInput(3);
                        Console.WriteLine("  Enter the onDuration of the signal which will be used for the TTL output action.[1000(Default)]");
                        Console.Write("  >>>> ");
                        outputPortAction.signal.onDuration = (UInt16)Util.GetInput(1000);
                        Console.WriteLine("  Enter the offDuration of the signal which will be used for the TTL output action.[1000(Default)]");
                        Console.Write("  >>>> ");
                        outputPortAction.signal.offDuration = (UInt16)Util.GetInput(1000);
                        Console.WriteLine("  Enter the delay of the signal which will be used for the TTL output action.[100(Default)]");
                        Console.Write("  >>>> ");
                        outputPortAction.signal.delay = (UInt16)Util.GetInput(100);
                        IntPtr tempOutputObj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2OutputPortAction)));
                        Marshal.StructureToPtr(outputPortAction, tempOutputObj, true);
                        Marshal.Copy(tempOutputObj, output.action.actionUnion, 0, Marshal.SizeOf(typeof(BS2OutputPortAction)));
                    }

                    Marshal.StructureToPtr(output, curOutputObjs, true);
                    curOutputObjs += Marshal.SizeOf(typeof(BS2InterlockZoneOutput));
                }
                zoneBlob[idx].outputObjs = outputObjs;

                BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetInterlockZone(sdkContext, deviceID, zoneBlob, amount);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(outputObjs);
                Marshal.FreeHGlobal(inputObjs);
                Marshal.FreeHGlobal(doorIDObj);
            }
        }

        void setInterlockZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "Interlock", API.BS2_SetInterlockZoneAlarm);
        }

        void removeInterlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "Interlock", API.BS2_RemoveAllInterlockZone, API.BS2_RemoveInterlockZone);
        }

        void getLiftLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZone<BS2LiftLockUnlockZone>(sdkContext, deviceID, "Lift lock/unlock", API.BS2_GetAllLiftLockUnlockZone, API.BS2_GetLiftLockUnlockZone, print);
        }

        void getLiftLockUnlockZoneStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            getZoneStatus(sdkContext, deviceID, "Lift lock/unlock", API.BS2_GetLiftLockUnlockZoneStatus);
        }

        void setLiftLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many Lift lock/unlock zones do you want to set? [1(default)]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2LiftLockUnlockZone> slulList = new List<BS2LiftLockUnlockZone>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2LiftLockUnlockZone zone = Util.AllocateStructure<BS2LiftLockUnlockZone>();

                Console.WriteLine("Enter a value for Lift lock/unlock zone[{0}]", idx);
                Console.WriteLine("  Enter the ID for the zone which you want to set");
                Console.Write("  >>>> ");
                zone.zoneID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the zone which you want to set");
                Console.Write("  >>>> ");
                string zoneName = Console.ReadLine();
                if (zoneName.Length == 0)
                {
                    Console.WriteLine("  [Warning] Name of zone will be displayed as empty.");
                }
                else if (zoneName.Length > BS2Environment.BS2_MAX_ZONE_NAME_LEN)
                {
                    Console.WriteLine("  Name of zone should less than {0} words.", BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                    Array.Clear(zone.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                    Array.Copy(zoneNameArray, zone.name, zoneNameArray.Length);
                }

                Console.WriteLine("  Enter the ID of access schedule to lock this Lift lock/unlock zone: [{0}(default) : {1}]", (UInt32)BS2ScheduleIDEnum.ALWAYS, BS2ScheduleIDEnum.ALWAYS);
                Console.Write("  >>>> ");
                zone.lockScheduleID = Util.GetInput((UInt32)BS2ScheduleIDEnum.ALWAYS);

                Console.WriteLine("  Enter the ID of access schedule to unlock this Lift lock/unlock zone: [{0}(default) : {1}]", (UInt32)BS2ScheduleIDEnum.NEVER, BS2ScheduleIDEnum.NEVER);
                Console.Write("  >>>> ");
                zone.unlockScheduleID = Util.GetInput((UInt32)BS2ScheduleIDEnum.NEVER);

                Console.WriteLine("  Do you want to activate this Lift lock/unlock zone? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    zone.disabled = 0;
                }
                else
                {
                    zone.disabled = 1;
                }

                zone.alarmed = 0;

                for (int loop = 0; loop < BS2Environment.BS2_MAX_LIFT_LOCK_UNLOCK_ALARM_ACTION; ++loop)
                {
                    zone.alarm[loop].deviceID = 0;
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                    zone.alarm[loop].stopFlag = 0;
                    zone.alarm[loop].delay = 0;
                }

                Console.WriteLine("  How many alarms for this Lift lock/unlock do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_LIFT_LOCK_UNLOCK_ALARM_ACTION);
                Console.Write("  >>>> ");
                int alarmCount = Util.GetInput(0);
                BS2BuzzerAction buzzer = Util.AllocateStructure<BS2BuzzerAction>();

                for (int loop = 0; loop < alarmCount; ++loop)
                {
                    Console.WriteLine("  Enter the device ID which you want to run this alarm");
                    Console.Write("  >>>> ");
                    zone.alarm[loop].deviceID = (UInt32)Util.GetInput();

                    // We are assuming buzzer control. Of course you can do the other action.
                    zone.alarm[loop].type = (byte)BS2ActionTypeEnum.BUZZER;

                    buzzer.count = 5;
                    Console.WriteLine("  Enter the type of buzzer tone.[{0} : {1}, {2} : {3}, {4} : {5}, {6}(default) : {7}]",
                                    (byte)BS2BuzzerToneEnum.OFF,
                                    BS2BuzzerToneEnum.OFF,
                                    (byte)BS2BuzzerToneEnum.LOW,
                                    BS2BuzzerToneEnum.LOW,
                                    (byte)BS2BuzzerToneEnum.MIDDLE,
                                    BS2BuzzerToneEnum.MIDDLE,
                                    (byte)BS2BuzzerToneEnum.HIGH,
                                    BS2BuzzerToneEnum.HIGH);
                    Console.Write("  >>>> ");
                    buzzer.signal[0].tone = Util.GetInput((byte)BS2BuzzerToneEnum.HIGH);

                    Console.WriteLine("  Do you want to set the fade out effect for this Lift lock/unlock zone? [y/N]");
                    Console.Write("  >>>> ");
                    if (Util.IsNo())
                    {
                        buzzer.signal[0].fadeout = 0;
                    }
                    else
                    {
                        buzzer.signal[0].fadeout = 1;
                    }

                    Console.WriteLine("  Enter the duration of buzzer for this Lift lock/unlock zone: [{100}(default)]");
                    Console.Write("  >>>> ");
                    buzzer.signal[0].duration = (UInt16)Util.GetInput((UInt16)100);

                    Console.WriteLine("  How many waiting for to a next action?[100(default)]");
                    Console.Write("  >>>> ");
                    buzzer.signal[0].delay = Util.GetInput((UInt16)100);

                    byte[] inputActionArray = Util.ConvertTo<BS2BuzzerAction>(ref buzzer);
                    Array.Clear(zone.alarm[loop].actionUnion, 0, zone.alarm[loop].actionUnion.Length);
                    Array.Copy(inputActionArray, zone.alarm[loop].actionUnion, inputActionArray.Length);
                }

                Console.WriteLine("  Enter the number of Lift-Floor informations that make up the zone.");
                Console.Write("  >>>> ");
                zone.numLifts = Util.GetInput((byte)1);
                for (int index = 0; index < zone.numLifts; index++)
                {
                    Console.WriteLine("  Enter the lift ID of the floors.");
                    Console.Write("  >>>> ");
                    zone.lifts[index].liftID = (UInt32)Util.GetInput();

                    Console.WriteLine("  Enter all the floor indices (array index) to be composed of the zone among the floors of the currently set lift.");
                    Console.WriteLine("  Perhaps you have set floors by calling BS2_SetLift. [POS_1, POS_2, ...]");
                    Console.Write("  >>>> ");
                    string[] floorIndices = Console.ReadLine().Split(delimiterChars);

                    foreach (string pos in floorIndices)
                    {
                        if (0 < pos.Length)
                        {
                            byte item;
                            if (byte.TryParse(pos, out item))
                            {
                                if (zone.lifts[index].numFloors + 1 >= 256)
                                {
                                    Console.WriteLine("[Warning] The count of floors should less than {0}.", 255);
                                    break;
                                }
                                zone.lifts[index].floorIndices[zone.lifts[index].numFloors] = item;
                                zone.lifts[index].numFloors++;
                            }
                        }
                    }
                }

                zone.numBypassGroups = 0;
                if (zone.lockScheduleID > (UInt32)BS2ScheduleIDEnum.NEVER)
                {
                    Console.WriteLine("  Enter the ID of the access group which can bypass this Lift lock/unlock zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string accessGroupID in accessGroupIDs)
                    {
                        if (accessGroupID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(accessGroupID, out item))
                            {
                                if (zone.numBypassGroups + 1 >= BS2Environment.BS2_MAX_BYPASS_GROUPS_IN_LIFT_LOCK_UNLOCK_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of access group ID should less than {0}.", BS2Environment.BS2_MAX_BYPASS_GROUPS_IN_LIFT_LOCK_UNLOCK_ZONE);
                                    break;
                                }

                                zone.bypassGroupIDs[zone.numBypassGroups] = item;
                                zone.numBypassGroups++;
                            }
                        }
                    }
                }

                zone.numUnlockGroups = 0;
                if (zone.unlockScheduleID > (UInt32)BS2ScheduleIDEnum.NEVER)
                {
                    Console.WriteLine("  Enter the ID of the access group which can unlock this Lift lock/unlock zone: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
                    string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                    foreach (string accessGroupID in accessGroupIDs)
                    {
                        if (accessGroupID.Length > 0)
                        {
                            UInt32 item;
                            if (UInt32.TryParse(accessGroupID, out item))
                            {
                                if (zone.numUnlockGroups + 1 >= BS2Environment.BS2_MAX_UNLOCK_GROUPS_IN_LIFT_LOCK_UNLOCK_ZONE)
                                {
                                    Console.WriteLine("[Warning] The count of access group ID should less than {0}.", BS2Environment.BS2_MAX_UNLOCK_GROUPS_IN_LIFT_LOCK_UNLOCK_ZONE);
                                    break;
                                }

                                zone.unlockGroupIDs[zone.numUnlockGroups] = item;
                                zone.numUnlockGroups++;
                            }
                        }
                    }
                }

                slulList.Add(zone);
            }

            int structSize = Marshal.SizeOf(typeof(BS2LiftLockUnlockZone));
            IntPtr slulListObj = Marshal.AllocHGlobal(structSize * slulList.Count);
            IntPtr curSlulListObj = slulListObj;
            foreach (BS2LiftLockUnlockZone item in slulList)
            {
                Marshal.StructureToPtr(item, curSlulListObj, false);
                curSlulListObj = (IntPtr)((long)curSlulListObj + structSize);
            }

            Console.WriteLine("Trying to set Lift lock/unlock zone to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetLiftLockUnlockZone(sdkContext, deviceID, slulListObj, (UInt32)slulList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(slulListObj);
        }

        void setLiftLockUnlockZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setZoneAlarm(sdkContext, deviceID, "Lift lock/unlock", API.BS2_SetLiftLockUnlockZoneAlarm);
        }

        void removeLiftLockUnlockZone(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            removeZone(sdkContext, deviceID, "Lift lock/unlock", API.BS2_RemoveAllLiftLockUnlockZone, API.BS2_RemoveLiftLockUnlockZone);
        }

        void print(IntPtr sdkContext, BS2AntiPassbackZone zone)
        {
            Console.WriteLine(">>>> APB Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--type[{0}]", (BS2APBZoneTypeEnum)zone.type);
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--resetDuration[{0}]", zone.resetDuration);
            Console.WriteLine("     |--alarm");

            for (int idx = 0; idx < BS2Environment.BS2_MAX_APB_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }
            Console.WriteLine("     |--readers");
            for (byte idx = 0; idx < zone.numReaders; ++idx)
            {
                Console.WriteLine("     |  |--deviceID[{0}] type[{1}]", zone.readers[idx].deviceID, (BS2APBZoneReaderTypeEnum)zone.readers[idx].type);
            }
            Console.WriteLine("     |--bypassGroupIDs");
            for (byte idx = 0; idx < zone.numBypassGroups; ++idx)
            {
                Console.WriteLine("     |  |--access group ID[{0}]", zone.bypassGroupIDs[idx]);
            }
        }

        void print(IntPtr sdkContext, BS2TimedAntiPassbackZone zone)
        {
            Console.WriteLine(">>>> Timed APB Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--type[{0}]", (BS2APBZoneTypeEnum)zone.type);
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--resetDuration[{0}]", zone.resetDuration);
            Console.WriteLine("     |--alarm");
            for (int idx = 0; idx < BS2Environment.BS2_MAX_TIMED_APB_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }
            Console.WriteLine("     |--readers");
            for (byte idx = 0; idx < zone.numReaders; ++idx)
            {
                Console.WriteLine("     |  |--deviceID[{0}]]", zone.readers[idx].deviceID);
            }
            Console.WriteLine("     |--bypassGroupIDs");
            for (byte idx = 0; idx < zone.numBypassGroups; ++idx)
            {
                Console.WriteLine("     |  |--access group ID[{0}]", zone.bypassGroupIDs[idx]);
            }
        }

        void print(IntPtr sdkContext, BS2FireAlarmZone zone)
        {
            Console.WriteLine(">>>> Fire Alarm Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--sensor");
            for (byte idx = 0; idx < zone.numSensors; ++idx)
            {
                Console.WriteLine("     |  |--deviceID[{0}] port[{1}] switchType[{2}]", zone.sensor[idx].deviceID, zone.sensor[idx].port, (BS2SwitchTypeEnum)zone.sensor[idx].switchType);
            }
            Console.WriteLine("     |--alarm");
            for (int idx = 0; idx < BS2Environment.BS2_MAX_FIRE_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }            
            Console.WriteLine("     |--doorIDs");
            for (byte idx = 0; idx < zone.numDoors; ++idx)
            {
                Console.WriteLine("     |  |--door ID[{0}]", zone.doorIDs[idx]);
            }
        }

        void print(IntPtr sdkContext, BS2ScheduledLockUnlockZone zone)
        {
            Console.WriteLine(">>>> Scheduled Lock/Unlock Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--lockScheduleID[{0}]", zone.lockScheduleID);
            Console.WriteLine("     |--unlockScheduleID[{0}]", zone.unlockScheduleID);
            Console.WriteLine("     |--bidirectionalLock[{0}]", Convert.ToBoolean(zone.bidirectionalLock));
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--alarm");
            for (int idx = 0; idx < BS2Environment.BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }
            Console.WriteLine("     |--doorIDs");
            for (byte idx = 0; idx < zone.numDoors; ++idx)
            {
                Console.WriteLine("     |  |--door ID[{0}]", zone.doorIDs[idx]);
            }
            Console.WriteLine("     |--bypassGroupIDs");
            for (byte idx = 0; idx < zone.numBypassGroups; ++idx)
            {
                Console.WriteLine("     |  |--bypass group ID[{0}]", zone.bypassGroupIDs[idx]);
            }
            Console.WriteLine("     |--unlockGroupIDs");
            for (byte idx = 0; idx < zone.numUnlockGroups; ++idx)
            {
                Console.WriteLine("     |  |--unlock group ID[{0}]", zone.unlockGroupIDs[idx]);
            }
        }

        void print(IntPtr sdkContext, BS2ZoneStatus zoneStatus)
        {
            Console.WriteLine(">>>> Zone ID[{0, 10}] status[{1}] disabled[{2}]", zoneStatus.id, (BS2ZoneStatusEnum)zoneStatus.status, Convert.ToBoolean(zoneStatus.disabled));            
        }

        void print(IntPtr sdkContext, BS2IntrusionAlarmZone zone)
        {
            Console.WriteLine(">>>> IntrusionAlarmZone Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--armDelay[{0}]", zone.armDelay);
            Console.WriteLine("     |--alarmDelay[{0}]", zone.alarmDelay);
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));

            Console.WriteLine("     |--numReaders[{0}]", zone.numReaders);
            Console.WriteLine("     |--numInputs[{0}]", zone.numInputs);
            Console.WriteLine("     |--numOutputs[{0}]", zone.numOutputs);
            Console.WriteLine("     |--numCards[{0}]", zone.numCards);
            Console.WriteLine("     |--numDoors[{0}]", zone.numDoors);
            Console.WriteLine("     |--numGroups[{0}]", zone.numGroups);

        }

        void print(IntPtr sdkContext, BS2InterlockZoneBlob zoneBlob)
        {
            Console.WriteLine("   |-- InterlockZone Zone ID[{0, 10}] name[{1}]", zoneBlob.InterlockZone.zoneID, Encoding.UTF8.GetString(zoneBlob.InterlockZone.name).TrimEnd('\0'));
            Console.WriteLine("      |--disabled[{0}]", zoneBlob.InterlockZone.disabled);
            Console.WriteLine("      |--numDoors[{0}]", zoneBlob.InterlockZone.numDoors);
            Console.WriteLine("      |--numInputs[{0}]", zoneBlob.InterlockZone.numInputs);
            Console.WriteLine("      |--numOutputs[{0}]", zoneBlob.InterlockZone.numOutputs);

            BS2InterlockZoneInput input;
            IntPtr curInputObj = zoneBlob.inputObjs;
            for (int idx = 0; idx < zoneBlob.InterlockZone.numInputs; idx++)
            {
                input = (BS2InterlockZoneInput)Marshal.PtrToStructure(curInputObj, typeof(BS2InterlockZoneInput));
                print(ref input, 1, idx);
                curInputObj += Marshal.SizeOf(typeof(BS2InterlockZoneInput));
            }
            
            BS2InterlockZoneOutput output;
            IntPtr curOutputObj = zoneBlob.outputObjs;
            for (int idx = 0; idx < zoneBlob.InterlockZone.numOutputs; idx++)
            {
                output = (BS2InterlockZoneOutput)Marshal.PtrToStructure(curOutputObj, typeof(BS2InterlockZoneOutput));
                print(ref output, 1, idx);
                curOutputObj += Marshal.SizeOf(typeof(BS2InterlockZoneOutput));
            }
        }

        void print(ref BS2InterlockZoneInput data, int Indented, int idx)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + "|Input #{0}", idx);
            Console.WriteLine(preIndented + "|-- deviceID: {0}", data.deviceID);
            Console.WriteLine(preIndented + "|-- port: {0}", data.port);
            Console.WriteLine(preIndented + "|-- switchType: {0}", (BS2SwitchTypeEnum)data.switchType);
            Console.WriteLine(preIndented + "|-- duration: {0}", data.duration);
            Console.WriteLine(preIndented + "|-- operationType: {0}", (BS2OperationTypeEnum)data.operationType);
            Console.WriteLine();
        }

        void print(ref BS2InterlockZoneOutput data, int Indented, int idx)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + "|Output #{0}", idx);
            Console.WriteLine(preIndented + "|-- eventcode: {0}", (BS2EventCodeEnum)data.eventcode);
            Console.WriteLine(preIndented + "|-- BS2Action");
            if (data.action.deviceID == UInt32.MaxValue)
                Console.WriteLine(preIndented + "   |-- deviceID: ALL Member");
            else
                Console.WriteLine(preIndented + "   |-- deviceID: {0}", data.action.deviceID);
            Console.WriteLine(preIndented + "   |-- type: {0}", (BS2ActionTypeEnum)data.action.type);
            Console.WriteLine(preIndented + "   |-- stopFlag: {0}", data.action.stopFlag);
            Console.WriteLine(preIndented + "   |-- delay: {0}", data.action.delay);
            if ((BS2ActionTypeEnum)data.action.type == BS2ActionTypeEnum.SOUND)
            {
                int size = Marshal.SizeOf(typeof(BS2SoundAction));
                byte[] arrSA = new byte[size];
                Array.Copy(data.action.actionUnion, arrSA, size);
                BS2SoundAction sa = Util.ConvertTo<BS2SoundAction>(arrSA);
                print(ref sa, Indented + 1);
            }
            if ((BS2ActionTypeEnum)data.action.type == BS2ActionTypeEnum.RELAY)
            {
                int size = Marshal.SizeOf(typeof(BS2RelayAction));
                byte[] arrRA = new byte[size];
                Array.Copy(data.action.actionUnion, arrRA, size);
                BS2RelayAction ra = Util.ConvertTo<BS2RelayAction>(arrRA);
                print(ref ra, Indented + 1);
            }
            if ((BS2ActionTypeEnum)data.action.type == BS2ActionTypeEnum.TTL)
            {
                int size = Marshal.SizeOf(typeof(BS2OutputPortAction));
                byte[] arrOA = new byte[size];
                Array.Copy(data.action.actionUnion, arrOA, size);
                BS2OutputPortAction oa = Util.ConvertTo<BS2OutputPortAction>(arrOA);
                print(ref oa, Indented + 1);
            }
            Console.WriteLine();
        }

        void print(ref BS2SoundAction data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + "|-- BS2SoundAction");
            Console.WriteLine(preIndented + "   |-- count: {0}", data.count);
            Console.WriteLine(preIndented + "   |-- soundIndex: {0}", (BS2SoundIndexEnum)data.soundIndex);
        }

        void print(ref BS2RelayAction data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + "|-- BS2RelayAction");
            Console.WriteLine(preIndented + "   |-- relayIndex: {0}", data.relayIndex);
            Console.WriteLine(preIndented + "   |-- BS2Signal");
            Console.WriteLine(preIndented + "      |-- signalID: {0}", data.signal.signalID);
            Console.WriteLine(preIndented + "      |-- count: {0}", data.signal.count);
            Console.WriteLine(preIndented + "      |-- onDuration: {0}", data.signal.onDuration);
            Console.WriteLine(preIndented + "      |-- offDuration: {0}", data.signal.offDuration);
            Console.WriteLine(preIndented + "      |-- delay: {0}", data.signal.delay);
        }

        void print(ref BS2OutputPortAction data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + "|-- BS2RelayAction");
            Console.WriteLine(preIndented + "   |-- portIndex: {0}", data.portIndex);
            Console.WriteLine(preIndented + "   |-- BS2Signal");
            Console.WriteLine(preIndented + "      |-- signalID: {0}", data.signal.signalID);
            Console.WriteLine(preIndented + "      |-- count: {0}", data.signal.count);
            Console.WriteLine(preIndented + "      |-- onDuration: {0}", data.signal.onDuration);
            Console.WriteLine(preIndented + "      |-- offDuration: {0}", data.signal.offDuration);
            Console.WriteLine(preIndented + "      |-- delay: {0}", data.signal.delay);
        }

        void print(IntPtr sdkContext, BS2LiftLockUnlockZone zone)
        {
            Console.WriteLine(">>>> Lift Lock/Unlock Zone ID[{0, 10}] name[{1}]", zone.zoneID, Encoding.UTF8.GetString(zone.name).TrimEnd('\0'));
            Console.WriteLine("     |--lockScheduleID[{0}]", zone.lockScheduleID);
            Console.WriteLine("     |--unlockScheduleID[{0}]", zone.unlockScheduleID);
            Console.WriteLine("     |--disabled[{0}]", Convert.ToBoolean(zone.disabled));
            Console.WriteLine("     |--alarmed[{0}]", Convert.ToBoolean(zone.alarmed));
            Console.WriteLine("     |--alarm");
            for (int idx = 0; idx < BS2Environment.BS2_MAX_LIFT_LOCK_UNLOCK_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)zone.alarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", zone.alarm[idx].deviceID, (BS2ActionTypeEnum)zone.alarm[idx].type, Util.getActionMsg(zone.alarm[idx]));
            }
            Console.WriteLine("     |--lift floors");
            for (byte idx = 0; idx < zone.numLifts; ++idx)
            {
                Console.WriteLine("     |  |--lift ID[{0}]", zone.lifts[idx]);
                for (byte fidx = 0; fidx < zone.lifts[idx].numFloors; fidx++)
                    Console.WriteLine("     |  |  |--floor array pos[{0}]", zone.lifts[idx].floorIndices[fidx]);
            }
            Console.WriteLine("     |--bypassGroupIDs");
            for (byte idx = 0; idx < zone.numBypassGroups; ++idx)
            {
                Console.WriteLine("     |  |--bypass group ID[{0}]", zone.bypassGroupIDs[idx]);
            }
            Console.WriteLine("     |--unlockGroupIDs");
            for (byte idx = 0; idx < zone.numUnlockGroups; ++idx)
            {
                Console.WriteLine("     |  |--unlock group ID[{0}]", zone.unlockGroupIDs[idx]);
            }
        }
    }
}
