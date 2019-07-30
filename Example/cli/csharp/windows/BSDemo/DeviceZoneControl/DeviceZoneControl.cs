using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Net;

namespace Suprema
{
    #region DEVICE_ZONE_SUPPORTED
    public class DeviceZoneControl : FunctionModule
    {
        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (isMasterDevice)
            {
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Device Zone Master Config", getDeviceZoneMasterCfg));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Device Zone Master Config", setDeviceZoneMasterCfg));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Device Zone Config", getDeviceZoneCfg));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Device Zone Config", setDeviceZoneCfg));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Device Zone Alarm", setDeviceZoneAlarm));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Delete device zone in/out records of user", clearDeviceZoneAccessRecord));
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Device Zone Access Group Entrance Limit", getDeviceZoneAGEntranceLimit));
            }

            return functionList;
        }

        string ToStringTime(Array ary, int start, int length)
        {
            string str = "";
            for (int idx = start; idx < Math.Min(ary.Length, start + length); idx++)
            {
                if (str != "")
                    str += ",";
                UInt32 value = (UInt32)ary.GetValue(idx);
                UInt32 hour = (value / (60*60));
                UInt32 min = (value % (60 * 60)) / 60;
                UInt32 sec = (value % 60);
                str += string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
            }

            return str;
        }
        string ToString(Array ary, int start, int length)
        {
            string str = "";
            for (int idx = start; idx < Math.Min(ary.Length, start+length); idx++)
            {
                if (str != "")
                    str += ",";
                str += ary.GetValue(idx).ToString() ;
            }

            return str;
        }
        void print(ref BS2DeviceZoneMasterConfig data, int Indented)
        {
            String preIndented = new String(' ', Indented*3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneMasterConfig");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   enable: {0}", data.enable);
            Console.WriteLine(preIndented + "   listenPort: {0}", data.listenPort);
            Console.WriteLine(preIndented + "}");

            //string jString = JsonConvert.SerializeObject(deviceZoneMasterConfig, Formatting.Indented);
            //Console.WriteLine(jString);
        }
        void print(ref BS2DeviceZoneAGEntranceLimit data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneAGEntranceLimit");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   zoneID: {0}", data.zoneID);
            Console.WriteLine(preIndented + "   numAGEntranceLimit: {0}", data.numAGEntranceLimit);
            for (int i = 0; i < data.numAGEntranceLimit; i++)
            {
                Console.WriteLine(preIndented + "   [{0}]periodStart_s: {1}", i, ToStringTime(data.periodStart_s, i, 1));
                Console.WriteLine(preIndented + "   [{0}]periodEnd_s: {1}", i, ToStringTime(data.periodEnd_s, i, 1));
                Console.WriteLine(preIndented + "   [{0}]numEntry: {1}", i, data.numEntry[i]);
                Console.WriteLine(preIndented + "   [{0}]accessGroupID: {1}", i, ToString(data.accessGroupID, BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE*i, data.numEntry[i]));
                Console.WriteLine(preIndented + "   [{0}]maxEntry: {1}", i, ToString(data.maxEntry, BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE * i, data.numEntry[i]));

            }
            Console.WriteLine(preIndented + "}");
        }

        void print (ref BS2SoundAction data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + ">>>> BS2SoundAction");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   count: {0}", data.count);
            Console.WriteLine(preIndented + "   soundIndex: {0}", (BS2SoundIndexEnum)data.soundIndex);
            Console.WriteLine(preIndented + "}");
        }

        void print(ref BS2Action data, int Indented)
        {
            String preIndented = new String(' ', Indented*3);
            Console.WriteLine(preIndented + ">>>> BS2Action");
            Console.WriteLine(preIndented + "{");
            if (data.deviceID == UInt32.MaxValue)
                Console.WriteLine(preIndented + "   deviceID: ALL Member");
            else
                Console.WriteLine(preIndented + "   deviceID: {0}", data.deviceID);
            Console.WriteLine(preIndented + "   type: {0}", (BS2ActionTypeEnum)data.type);
            Console.WriteLine(preIndented + "   stopFlag: {0}", data.stopFlag);
            Console.WriteLine(preIndented + "   delay: {0}", data.delay);
            Console.WriteLine(preIndented + "}");
            if ((BS2ActionTypeEnum)data.type == BS2ActionTypeEnum.SOUND)
            {
                int size = Marshal.SizeOf(typeof(BS2SoundAction));
                byte[] arrSA = new byte[size];
                Array.Copy(data.actionUnion, arrSA, size);
                int a = Marshal.OffsetOf(typeof(BS2SoundAction), "count").ToInt32();
                int b = Marshal.OffsetOf(typeof(BS2SoundAction), "soundIndex").ToInt32();
                int c = Marshal.SizeOf(typeof(BS2SoundAction));
                BS2SoundAction sa = Util.ConvertTo<BS2SoundAction>(arrSA);
                print(ref sa, Indented + 1);
            }
        }

        #region ENTRANCE_LIMIT
        void print(ref BS2DeviceZoneEntranceLimitMemberInfo data, int Indented)
        {
            String preIndented = new String(' ', Indented*3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneEntranceLimitMemberInfo");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   readerID: {0}", data.readerID);
            Console.WriteLine(preIndented + "}");
        }

        void print(ref BS2DeviceZoneEntranceLimitMaster data, int Indented)
        {
            String preIndented = new String(' ', Indented*3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneEntranceLimitMaster");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   name: {0}", Encoding.UTF8.GetString(data.name).TrimEnd('\0'));
            Console.WriteLine(preIndented + "   type: {0}", (BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE)data.type);
            Console.WriteLine(preIndented + "   entryLimitInterval_s: {0}", data.entryLimitInterval_s);
            Console.WriteLine(preIndented + "   numEntranceLimit: {0}", data.numEntranceLimit);
            Console.WriteLine(preIndented + "   numReaders: {0}", data.numReaders);
            Console.WriteLine(preIndented + "   numAlarm: {0}", data.numAlarm);
            Console.WriteLine(preIndented + "   numBypassGroups: {0}", data.numBypassGroups);
            Console.WriteLine(preIndented + "   maxEntry: {0}", ToString(data.maxEntry, 0, data.numEntranceLimit));
            Console.WriteLine(preIndented + "   periodStart_s: {0}", ToStringTime(data.periodStart_s, 0, data.numEntranceLimit));
            Console.WriteLine(preIndented + "   periodEnd_s: {0}", ToStringTime(data.periodEnd_s, 0, data.numEntranceLimit));
            Console.WriteLine(preIndented + "   readers: ");
            for (int idx = 0; idx < data.numReaders; ++idx)
            {
                print(ref data.readers[idx], Indented + 1);
            }
            Console.WriteLine(preIndented + "   alarm: ");
            for (int idx = 0; idx < data.numAlarm; ++idx)
            {
                print(ref data.alarm[idx], Indented + 1);
            }
            Console.WriteLine(preIndented + "   bypassGroupIDs: {0}", ToString(data.bypassGroupIDs, 0, data.numBypassGroups));
            Console.WriteLine(preIndented + "}");
        }

        void print(ref BS2DeviceZoneEntranceLimitMember data, int Indented)
        {
            String preIndented = new String(' ', Indented*3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneEntranceLimitMember");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   masterPort: {0}", data.masterPort);
            Console.WriteLine(preIndented + "   actionInDisconnect: {0}", (BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_TYPE)data.actionInDisconnect);            
            Console.WriteLine(preIndented + "   masterIP: {0}", Encoding.UTF8.GetString(data.masterIP).TrimEnd('\0'));
            Console.WriteLine(preIndented + "}");
        }
        #endregion

        #region FIRE_ALARM
        void print(ref BS2DeviceZoneFireSensor data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneFireSensor");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   deviceID: {0}", data.deviceID);
            Console.WriteLine(preIndented + "   port: {0}", data.port);
            Console.WriteLine(preIndented + "   switchType: {0}", (BS2SwitchTypeEnum)data.switchType);
            Console.WriteLine(preIndented + "   duration: {0} ms", data.duration);
            Console.WriteLine(preIndented + "}");
        }

        void print(ref BS2DeviceZoneFireAlarmMemberInfo data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneFireAlarmMemberInfo");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   readerID: {0}", data.readerID);
            Console.WriteLine(preIndented + "}");
        }
        
        void print(ref BS2DeviceZoneFireAlarmMaster data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneFireAlarmMaster");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   name: {0}", Encoding.UTF8.GetString(data.name).TrimEnd('\0'));
            Console.WriteLine(preIndented + "   numReaders: {0}", data.numReaders);
            Console.WriteLine(preIndented + "   numAlarm: {0}", data.numAlarm);
            Console.WriteLine(preIndented + "   readers: ");
            for (int idx = 0; idx < data.numReaders; ++idx)
            {
                print(ref data.readers[idx], Indented + 1);
            }
            Console.WriteLine(preIndented + "   alarm: ");
            for (int idx = 0; idx < data.numAlarm; ++idx)
            {
                print(ref data.alarm[idx], Indented + 1);
            }
            Console.WriteLine(preIndented + "}");
        }

        void print(ref BS2DeviceZoneFireAlarmMember data, int Indented)
        {
            String preIndented = new String(' ', Indented * 3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneFireAlarmMember");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   masterPort: {0}", data.masterPort);
            Console.WriteLine(preIndented + "   masterIP: {0}", Encoding.UTF8.GetString(data.masterIP).TrimEnd('\0'));
            Console.WriteLine(preIndented + "   numSensors: {0}", data.numSensors);
            Console.WriteLine(preIndented + "   numDoors: {0}", data.numDoors);
            Console.WriteLine(preIndented + "   sensors: ");
            for (int idx = 0; idx < data.numSensors; ++idx)
            {
                print(ref data.sensor[idx], Indented + 1);
            }
            if (data.doorIDs[0] == UInt32.MaxValue)
                Console.WriteLine(preIndented + "   doorIDs: ALL Door");
            else
                Console.WriteLine(preIndented + "   doorIDs: {0}", ToString(data.doorIDs, 0, data.numDoors));           
            Console.WriteLine(preIndented + "}");
        }
        #endregion

        void print(ref BS2DeviceZone data, int Indented)
        {
            String preIndented = new String(' ', Indented*3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZone");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   zoneID: {0}", data.zoneID);
            Console.WriteLine(preIndented + "   zoneType: {0}", (BS2_DEVICE_ZONE_TYPE)data.zoneType);
            Console.WriteLine(preIndented + "   nodeType: {0}", (BS2_DEVICE_ZONE_NODE_TYPE)data.nodeType);
            Console.WriteLine(preIndented + "   enable: {0}", data.enable);
            switch((BS2_DEVICE_ZONE_TYPE)data.zoneType)
            {
                #region ENTRANCE_LIMIT
                case BS2_DEVICE_ZONE_TYPE.BS2_DEVICE_ZONE_TYPE_ENTRANCE_LIMIT:
                    if ((BS2_DEVICE_ZONE_NODE_TYPE)data.nodeType == BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MASTER)
                    {
                        Console.WriteLine(preIndented + "   BS2DeviceZoneEntranceLimitMaster:");
                        BS2DeviceZoneEntranceLimitMaster elm = Util.ConvertTo<BS2DeviceZoneEntranceLimitMaster>(data.zoneUnion);
                        print(ref elm, Indented + 1);
                    }
                    else
                    {
                        Console.WriteLine(preIndented + "   BS2DeviceZoneEntranceLimitMember:");
                        BS2DeviceZoneEntranceLimitMember elm = Util.ConvertTo<BS2DeviceZoneEntranceLimitMember>(data.zoneUnion);
                        print(ref elm, Indented + 1);
                    }
                    break;
                #endregion

                #region FIRE_ALARM
                case BS2_DEVICE_ZONE_TYPE.BS2_DEVICE_ZONE_TYPE_FIRE_ALARM:
                    if ((BS2_DEVICE_ZONE_NODE_TYPE)data.nodeType == BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MASTER)
                    {
                        Console.WriteLine(preIndented + "   BS2DeviceZoneFireAlarmMaster:");
                        BS2DeviceZoneFireAlarmMaster elm = Util.ConvertTo<BS2DeviceZoneFireAlarmMaster>(data.zoneUnion);
                        print(ref elm, Indented + 1);
                    }
                    else
                    {
                        Console.WriteLine(preIndented + "   BS2DeviceZoneFireAlarmMember:");
                        BS2DeviceZoneFireAlarmMember elm = Util.ConvertTo<BS2DeviceZoneFireAlarmMember>(data.zoneUnion);
                        print(ref elm, Indented + 1);
                    }
                    break;
                #endregion
            }
            Console.WriteLine(preIndented + "}");
        }

        void print(ref BS2DeviceZoneConfig data, int Indented)
        {
            String preIndented = new String(' ', Indented*3);
            Console.WriteLine(preIndented + ">>>> BS2DeviceZoneConfig");
            Console.WriteLine(preIndented + "{");
            Console.WriteLine(preIndented + "   numOfZones:{0}", data.numOfZones);
            for(int idx = 0; idx < data.numOfZones; ++idx)
            {
                print(ref data.zone[idx], Indented+1);
            }
            Console.WriteLine(preIndented + "}");
        }

        void getDeviceZoneMasterCfg(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2DeviceZoneMasterConfig config;
            Console.WriteLine("Trying to get DeviceZoneMasterConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceZoneMasterConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(ref config, 1);                
            }
        }

        void setDeviceZoneMasterCfg(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {            
            BS2DeviceZoneMasterConfig config = Util.AllocateStructure<BS2DeviceZoneMasterConfig>();

            Console.WriteLine("Enter listen port number of master device [{0}(default)]", (UInt16)BS2Environment.BS2_TCP_DEVICE_ZONE_MASTER_PORT_DEFAULT);
            Console.Write(">>>> ");
            config.listenPort = Util.GetInput((UInt16)BS2Environment.BS2_TCP_DEVICE_ZONE_MASTER_PORT_DEFAULT); ;

            Console.WriteLine("Do you want to activate this master device? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                config.enable = 1;               
            }
            else
            {
                config.enable = 0;
            }

            Console.WriteLine("Trying to set DeviceZoneMasterConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDeviceZoneMasterConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                getDeviceZoneMasterCfg(sdkContext, deviceID, true);
            }
        }

        void getDeviceZoneCfg(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2DeviceZoneConfig config;

            //Console.WriteLine("struct {");
            //Console.WriteLine("   numOfZones [{0:D}]", Marshal.OffsetOf(typeof(BS2DeviceZoneConfig), "numOfZones"));
            //Console.WriteLine("   zone [{0:D}]", Marshal.OffsetOf(typeof(BS2DeviceZoneConfig), "zone"));            
            //Console.WriteLine("}} BS2DeviceZoneConfig ({0:D})", Marshal.SizeOf(typeof(BS2DeviceZoneConfig)));
            

            Console.WriteLine("Trying to get getDeviceZoneCfg");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceZoneConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                print(ref config, 1);
            }
        }

        #region ENTRANCE_LIMIT
        void getDeviceZoneAGEntranceLimit(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr AGELsObj = IntPtr.Zero;
            UInt32 numAGELs = 0;

            Console.WriteLine("Trying to get GetAllDeviceZoneAGEntranceLimit");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetAllDeviceZoneAGEntranceLimit(sdkContext, deviceID, out AGELsObj, out numAGELs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                int structSize = Marshal.SizeOf(typeof(BS2DeviceZoneAGEntranceLimit));
                Type structureType = typeof(BS2DeviceZoneAGEntranceLimit);
                for (int i = 0; i < numAGELs; i++)
                {
                    IntPtr AGELObj = AGELsObj + structSize * i;
                    BS2DeviceZoneAGEntranceLimit agel = (BS2DeviceZoneAGEntranceLimit)Marshal.PtrToStructure(AGELObj, structureType);
                    print(ref agel, 1);
                }
                
            }
        }

        bool setEntranceLimit(ref BS2DeviceZone bs2DeviceZone, ref BS2DeviceZoneAGEntranceLimit AGEL)
        {
            Console.WriteLine("Which node type is this ? [{0}(default): BS2_DEVICE_ZONE_NODE_TYPE_MASTER, {1}: BS2_DEVICE_ZONE_NODE_TYPE_MEMBER]", (int)BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MASTER, (int)BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MEMBER);
            Console.Write(">>>> ");
            bs2DeviceZone.nodeType = Util.GetInput((int)BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MASTER);
            switch ((BS2_DEVICE_ZONE_NODE_TYPE)bs2DeviceZone.nodeType)
            {
                case BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MASTER:
                    {  
                        BS2DeviceZoneEntranceLimitMaster elm = Util.AllocateStructure<BS2DeviceZoneEntranceLimitMaster>();
                        Console.WriteLine("Enter the name for the zone which you want to set");
                        Console.Write(">>>> ");
                        string zoneName = Console.ReadLine();
                        if (zoneName.Length == 0)
                        {
                            Console.WriteLine("[Warning] Name of zone will be displayed as empty.");
                        }
                        else if (zoneName.Length > BS2Environment.BS2_MAX_ZONE_NAME_LEN)
                        {
                            Console.WriteLine("  Name of zone should less than {0} words.", BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                            return false;
                        }
                        else
                        {
                            byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                            Array.Clear(elm.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                            Array.Copy(zoneNameArray, elm.name, zoneNameArray.Length);
                        }

                        Console.WriteLine("Which type is this entrance limit zone? [{0}(default): BS2_DEVICE_ZONE_ENTRANCE_LIMIT_SOFT, {1} : BS2_DEVICE_ZONE_ENTRANCE_LIMIT_HARD", (byte)BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE.BS2_DEVICE_ZONE_ENTRANCE_LIMIT_SOFT, (byte)BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE.BS2_DEVICE_ZONE_ENTRANCE_LIMIT_HARD);
                        Console.Write(">>>> ");
                        elm.type = Util.GetInput((byte)BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE.BS2_DEVICE_ZONE_ENTRANCE_LIMIT_SOFT);

                        Console.WriteLine("How many seconds should you limit authentication on the same reader ? [0(default): no limit]");
                        Console.Write(">>>> ");
                        elm.entryLimitInterval_s = Util.GetInput((UInt32)0);

                        Console.WriteLine("How many entrance limit item do you want to set? [0(default)~24]");
                        Console.Write(">>>> ");
                        elm.numEntranceLimit = Util.GetInput(0);
                        if (elm.numEntranceLimit > 24)
                        {
                            Console.WriteLine("Wrong entrance limit item");
                            return false;
                        }
                        for (int idxEntranceLimitItem = 0; idxEntranceLimitItem < elm.numEntranceLimit; ++idxEntranceLimitItem)
                        {
                            Console.WriteLine("When do you want to limit {0} th entrance limit from ? [HH:mm:ss]", idxEntranceLimitItem);
                            Console.Write(">>>> ");
                            if (!Util.GetTime("HH:mm:ss", 0, out elm.periodStart_s[idxEntranceLimitItem]))
                            {
                                return false;
                            }

                            Console.WriteLine("When do you want to limit {0} th entrance limit to ? [HH:mm:ss]", idxEntranceLimitItem);
                            Console.Write(">>>> ");
                            if (!Util.GetTime("HH:mm:ss", 0, out elm.periodEnd_s[idxEntranceLimitItem]))
                            {
                                return false;
                            }

                            Console.WriteLine("How many entrance should you limit {0} th entrance limit? [0: ~ 253, {1}: Access Group]", idxEntranceLimitItem, BS2Environment.BS2_ENTRY_COUNT_FOR_ACCESS_GROUP_ENTRANCE_LIMIT);
                            Console.Write(">>>> ");
                            int value = Util.GetInput();
                            bool bAGEL = false;
                            unchecked
                            {
                                elm.maxEntry[idxEntranceLimitItem] = (byte)value;
                                bAGEL = elm.maxEntry[idxEntranceLimitItem] == (byte)BS2Environment.BS2_ENTRY_COUNT_FOR_ACCESS_GROUP_ENTRANCE_LIMIT;
                            }
                            if (bAGEL)
                            {
                                AGEL.zoneID = bs2DeviceZone.zoneID;
                                AGEL.periodStart_s[AGEL.numAGEntranceLimit] = elm.periodStart_s[idxEntranceLimitItem];
                                AGEL.periodEnd_s[AGEL.numAGEntranceLimit] = elm.periodEnd_s[idxEntranceLimitItem];
                                Console.WriteLine("How many access group item do you want to set? [1(default)~{0}]", BS2Environment.BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT);
                                Console.Write(">>>> ");
                                AGEL.numEntry[AGEL.numAGEntranceLimit] = Util.GetInput(1);
                                if (AGEL.numEntry[AGEL.numAGEntranceLimit] == 0 || AGEL.numEntry[AGEL.numAGEntranceLimit] > BS2Environment.BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT)
                                {
                                    Console.WriteLine("Wrong access group item count");
                                    return false;
                                }

                                for (int idxAG = 0; idxAG < AGEL.numEntry[AGEL.numAGEntranceLimit]; ++idxAG)
                                {
                                    int AGID = 0;
                                    int entry = 0;
                                    Console.WriteLine("Enter the ID of the access group [1~, {0}: otherwise Access Group ID]", BS2Environment.BS2_OTHERWISE_ACCESS_GROUP_ID);
                                    Console.Write(">>>> ");
                                    AGID = Util.GetInput();

                                    if (AGID == 0)
                                    {
                                        Console.WriteLine("Wrong access group id");
                                        return false;
                                    }

                                    Console.WriteLine("How many entrance should you limit [0(default): ~ 253, {0}: No Limit]", BS2Environment.BS2_ENTRY_COUNT_NO_LIMIT);
                                    Console.Write(">>>> ");
                                    entry = Util.GetInput();

                                    AGEL.accessGroupID[BS2Environment.BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT * AGEL.numAGEntranceLimit + idxAG] = (UInt32)AGID;
                                    AGEL.maxEntry[BS2Environment.BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT * AGEL.numAGEntranceLimit + idxAG] = (UInt16)entry;
                                }

                                AGEL.numAGEntranceLimit++;
                            }
                        }

                        char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                        elm.numReaders = 0;

                        Console.WriteLine("Enter the ID of the readers which are used as member of this entrance limit zone: [ID_1,ID_2 ...] (Max:{0})", BS2Environment.BS2_MAX_READERS_PER_DEVICE_ZONE_ENTRANCE_LIMIT);
                        Console.Write(">>>> ");
                        string[] readerIDs = Console.ReadLine().Split(delimiterChars);

                        foreach (string readerID in readerIDs)
                        {
                            if (readerID.Length > 0)
                            {
                                UInt32 item;
                                if (UInt32.TryParse(readerID, out item))
                                {
                                    if (elm.numReaders + 1 >= BS2Environment.BS2_MAX_READERS_PER_DEVICE_ZONE_ENTRANCE_LIMIT)
                                    {
                                        Console.WriteLine("[Warning] The count of reader id should less than {0}.", BS2Environment.BS2_MAX_READERS_PER_APB_ZONE);
                                        break;
                                    }
                                    elm.readers[elm.numReaders].readerID = item;
                                    elm.numReaders++;
                                }
                            }
                        }

                        Console.WriteLine("How many alarms for this entracne limit zone do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_DEVICE_ZONE_ENTRANCE_LIMIT_ALARM_ACTION);
                        Console.Write(">>>> ");
                        elm.numAlarm = Util.GetInput(0);
                        BS2SoundAction sound = Util.AllocateStructure<BS2SoundAction>();

                        for (int loop = 0; loop < elm.numAlarm; ++loop)
                        {
                            Console.WriteLine("Enter the device ID which you want to run this alarm [-1(default): ALL Member]");
                            Console.Write(">>>> ");
                            elm.alarm[loop].deviceID = (UInt32)Util.GetInput((UInt32)(UInt32.MaxValue));

                            // We are assuming sound control. Of course you can do the other action.
                            elm.alarm[loop].type = (byte)BS2ActionTypeEnum.SOUND;

                            Console.WriteLine("How many play the sound action on this device?[1(default)]");
                            Console.Write(">>>> ");
                            sound.count = Util.GetInput((byte)1);

                            Console.WriteLine("Enter the sound index.[{0}(default) : {1}, {2} : {3}, {4} : {5}]", (UInt16)BS2SoundIndexEnum.WELCOME, BS2SoundIndexEnum.WELCOME, (UInt16)BS2SoundIndexEnum.AUTH_SUCCESS, BS2SoundIndexEnum.AUTH_SUCCESS, (UInt16)BS2SoundIndexEnum.AUTH_FAIL, BS2SoundIndexEnum.AUTH_FAIL);
                            Console.Write(">>>> ");
                            sound.soundIndex = Util.GetInput((UInt16)BS2SoundIndexEnum.WELCOME);

                            byte[] inputActionArray = Util.ConvertTo<BS2SoundAction>(ref sound);
                            Array.Clear(elm.alarm[loop].actionUnion, 0, elm.alarm[loop].actionUnion.Length);
                            Array.Copy(inputActionArray, elm.alarm[loop].actionUnion, inputActionArray.Length);
                        }

                        elm.numBypassGroups = 0;
                        Console.WriteLine("Enter the ID of the access group which can bypass this entrace limit zone: [ID_1,ID_2 ...] (Max:{0})", BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_DEVICE_ZONE_ENTRANCE_LIMIT);
                        Console.Write(">>>> ");
                        string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                        foreach (string accessGroupID in accessGroupIDs)
                        {
                            if (accessGroupID.Length > 0)
                            {
                                UInt32 item;
                                if (UInt32.TryParse(accessGroupID, out item))
                                {
                                    if (elm.numBypassGroups + 1 >= BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_DEVICE_ZONE_ENTRANCE_LIMIT)
                                    {
                                        Console.WriteLine("[Warning] The count of bypass access group should less than {0}.", BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_DEVICE_ZONE_ENTRANCE_LIMIT);
                                        break;
                                    }

                                    elm.bypassGroupIDs[elm.numBypassGroups] = item;
                                    elm.numBypassGroups++;
                                }
                            }
                        }

                        byte[] elmArray = Util.ConvertTo<BS2DeviceZoneEntranceLimitMaster>(ref elm);
                        Array.Clear(bs2DeviceZone.zoneUnion, 0, bs2DeviceZone.zoneUnion.Length);
                        Array.Copy(elmArray, bs2DeviceZone.zoneUnion, elmArray.Length);
                    }
                    break;
                case BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MEMBER:
                    {
                        BS2DeviceZoneEntranceLimitMember elm = Util.AllocateStructure<BS2DeviceZoneEntranceLimitMember>();
                        Console.WriteLine("Which type is this entrance limit zone when was not connected? [{0}(default): BS2_DEVICE_ZONE_ENTRANCE_LIMIT_SOFT, {1} : BS2_DEVICE_ZONE_ENTRANCE_LIMIT_HARD", (byte)BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE.BS2_DEVICE_ZONE_ENTRANCE_LIMIT_SOFT, (byte)BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE.BS2_DEVICE_ZONE_ENTRANCE_LIMIT_HARD);
                        Console.Write(">>>> ");
                        elm.actionInDisconnect = Util.GetInput((byte)BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE.BS2_DEVICE_ZONE_ENTRANCE_LIMIT_SOFT);

                        Console.WriteLine("Enter the IP address of the master device of entrance limit?");
                        Console.Write(">>>> ");

                        string deviceIpAddress = Console.ReadLine();
                        IPAddress ipAddress;
                        if (!IPAddress.TryParse(deviceIpAddress, out ipAddress))
                        {
                            Console.WriteLine("Invalid ip : " + deviceIpAddress);
                            return false;
                        }
                        byte[] deviceIpAddressArray = Encoding.UTF8.GetBytes(deviceIpAddress);
                        Array.Clear(elm.masterIP, 0, BS2Environment.BS2_IPV4_ADDR_SIZE);
                        Array.Copy(deviceIpAddressArray, elm.masterIP, deviceIpAddressArray.Length);

                        Console.WriteLine("Enter the Port Number of the master device of entrance limit? : default[{0}]", BS2Environment.BS2_TCP_DEVICE_ZONE_MASTER_PORT_DEFAULT);
                        Console.Write(">>>> ");
                        elm.masterPort = Util.GetInput((UInt16)BS2Environment.BS2_TCP_DEVICE_ZONE_MASTER_PORT_DEFAULT);

                        byte[] elmArray = Util.ConvertTo<BS2DeviceZoneEntranceLimitMember>(ref elm);
                        Array.Clear(bs2DeviceZone.zoneUnion, 0, bs2DeviceZone.zoneUnion.Length);
                        Array.Copy(elmArray, bs2DeviceZone.zoneUnion, elmArray.Length);
                    }
                    break;
                default:
                    Console.WriteLine("Wrong Zone Node Type");
                    return false;
            }

            return true;
        }
        #endregion
		
        #region FIRE_ALARM
        bool setFireAlarm(ref BS2DeviceZone bs2DeviceZone)
        {
            Console.WriteLine("Which node type is this ? [{0}(default): BS2_DEVICE_ZONE_NODE_TYPE_MASTER, {1}: BS2_DEVICE_ZONE_NODE_TYPE_MEMBER]", (int)BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MASTER, (int)BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MEMBER);
            Console.Write(">>>> ");
            bs2DeviceZone.nodeType = Util.GetInput((int)BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MASTER);
            switch ((BS2_DEVICE_ZONE_NODE_TYPE)bs2DeviceZone.nodeType)
            {
                case BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MASTER:
                    {
                        BS2DeviceZoneFireAlarmMaster elm = Util.AllocateStructure<BS2DeviceZoneFireAlarmMaster>();
                        Console.WriteLine("Enter the name for the zone which you want to set");
                        Console.Write(">>>> ");
                        string zoneName = Console.ReadLine();
                        if (zoneName.Length == 0)
                        {
                            Console.WriteLine("[Warning] Name of zone will be displayed as empty.");
                        }
                        else if (zoneName.Length > BS2Environment.BS2_MAX_ZONE_NAME_LEN)
                        {
                            Console.WriteLine("  Name of zone should less than {0} words.", BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                            return false;
                        }
                        else
                        {
                            byte[] zoneNameArray = Encoding.UTF8.GetBytes(zoneName);
                            Array.Clear(elm.name, 0, BS2Environment.BS2_MAX_ZONE_NAME_LEN);
                            Array.Copy(zoneNameArray, elm.name, zoneNameArray.Length);
                        }

                        char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                        elm.numReaders = 0;

                        Console.WriteLine("Enter the ID of the readers which are used as member of this fire alarm zone: [ID_1,ID_2 ...] (Max:{0})", BS2Environment.BS2_MAX_READERS_PER_DEVICE_ZONE_ENTRANCE_LIMIT);
                        Console.Write(">>>> ");
                        string[] readerIDs = Console.ReadLine().Split(delimiterChars);

                        foreach (string readerID in readerIDs)
                        {
                            if (readerID.Length > 0)
                            {
                                UInt32 item;
                                if (UInt32.TryParse(readerID, out item))
                                {
                                    if (elm.numReaders + 1 >= BS2Environment.BS2_MAX_READERS_PER_DEVICE_ZONE_ENTRANCE_LIMIT)
                                    {
                                        Console.WriteLine("[Warning] The count of reader id should less than {0}.", BS2Environment.BS2_MAX_READERS_PER_APB_ZONE);
                                        break;
                                    }
                                    elm.readers[elm.numReaders].readerID = item;
                                    elm.numReaders++;
                                }
                            }
                        }

                        Console.WriteLine("How many alarms for this fire alarm zone do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_DEVICE_ZONE_ENTRANCE_LIMIT_ALARM_ACTION);
                        Console.Write(">>>> ");
                        elm.numAlarm = Util.GetInput(0);
                        BS2SoundAction sound = Util.AllocateStructure<BS2SoundAction>();

                        for (int loop = 0; loop < elm.numAlarm; ++loop)
                        {
                            Console.WriteLine("Enter the device ID which you want to run this alarm [-1(default): ALL Member]");
                            Console.Write(">>>> ");
                            elm.alarm[loop].deviceID = (UInt32)Util.GetInput((UInt32)(UInt32.MaxValue));

                            // We are assuming sound control. Of course you can do the other action.
                            elm.alarm[loop].type = (byte)BS2ActionTypeEnum.SOUND;

                            Console.WriteLine("How many play the sound action on this device?[1(default)]");
                            Console.Write(">>>> ");
                            sound.count = Util.GetInput((byte)1);

                            Console.WriteLine("Enter the sound index.[{0}(default) : {1}, {2} : {3}, {4} : {5}]", (UInt16)BS2SoundIndexEnum.WELCOME, BS2SoundIndexEnum.WELCOME, (UInt16)BS2SoundIndexEnum.AUTH_SUCCESS, BS2SoundIndexEnum.AUTH_SUCCESS, (UInt16)BS2SoundIndexEnum.AUTH_FAIL, BS2SoundIndexEnum.AUTH_FAIL);
                            Console.Write(">>>> ");
                            sound.soundIndex = Util.GetInput((UInt16)BS2SoundIndexEnum.WELCOME);

                            byte[] inputActionArray = Util.ConvertTo<BS2SoundAction>(ref sound);
                            Array.Clear(elm.alarm[loop].actionUnion, 0, elm.alarm[loop].actionUnion.Length);
                            Array.Copy(inputActionArray, elm.alarm[loop].actionUnion, inputActionArray.Length);
                        }

                        byte[] elmArray = Util.ConvertTo<BS2DeviceZoneFireAlarmMaster>(ref elm);
                        Array.Clear(bs2DeviceZone.zoneUnion, 0, bs2DeviceZone.zoneUnion.Length);
                        Array.Copy(elmArray, bs2DeviceZone.zoneUnion, elmArray.Length);
                    }
                    break;
                case BS2_DEVICE_ZONE_NODE_TYPE.BS2_DEVICE_ZONE_NODE_TYPE_MEMBER:
                    {
                        BS2DeviceZoneFireAlarmMember elm = Util.AllocateStructure<BS2DeviceZoneFireAlarmMember>();
                        Console.WriteLine("Enter the IP address of the master device of fire alarm zone?");
                        Console.Write(">>>> ");

                        string deviceIpAddress = Console.ReadLine();
                        IPAddress ipAddress;
                        if (!IPAddress.TryParse(deviceIpAddress, out ipAddress))
                        {
                            Console.WriteLine("Invalid ip : " + deviceIpAddress);
                            return false;
                        }
                        byte[] deviceIpAddressArray = Encoding.UTF8.GetBytes(deviceIpAddress);
                        Array.Clear(elm.masterIP, 0, BS2Environment.BS2_IPV4_ADDR_SIZE);
                        Array.Copy(deviceIpAddressArray, elm.masterIP, deviceIpAddressArray.Length);

                        Console.WriteLine("Enter the Port Number of the master device of fire alarm zone? : default[{0}]", BS2Environment.BS2_TCP_DEVICE_ZONE_MASTER_PORT_DEFAULT);
                        Console.Write(">>>> ");
                        elm.masterPort = Util.GetInput((UInt16)BS2Environment.BS2_TCP_DEVICE_ZONE_MASTER_PORT_DEFAULT);

                        
                        Console.WriteLine("How many fire sensors for member of this fire alarm zone do you want to set? [1(default)-{0}]", BS2Environment.BS2_MAX_FIRE_SENSORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER);
                        Console.Write(">>>> ");
                        elm.numSensors = Util.GetInput((byte)1);

                        for (byte loop = 0; loop < elm.numSensors; ++loop)
                        {
                            Console.WriteLine("Enter the ID of the fire sensor for member of this fire alarm zone:");
                            Console.Write(">>>> ");
                            elm.sensor[loop].deviceID = (UInt32)Util.GetInput();

                            Console.WriteLine("Enter the port of the fire sensor for member of this fire alarm zone: [0(default)]");
                            Console.Write(">>>> ");
                            elm.sensor[loop].port = (byte)Util.GetInput((byte)0);

                            Console.WriteLine("Enter the switch type of the fire sensor for member of this fire alarm zone: [{0}(default) : {1}, {2} : {3}]", (byte)BS2SwitchTypeEnum.NORMAL_CLOSE, BS2SwitchTypeEnum.NORMAL_CLOSE, (byte)BS2SwitchTypeEnum.NORMAL_OPEN, BS2SwitchTypeEnum.NORMAL_OPEN);
                            Console.Write(">>>> ");
                            elm.sensor[loop].switchType = (byte)Util.GetInput((byte)0);

                            Console.WriteLine("Enter the duration of the fire sensor for member of this fire alarm zone: [{100}(default)]");
                            Console.Write(">>>> ");
                            elm.sensor[loop].duration = (UInt16)Util.GetInput((UInt16)100);
                        }

                        elm.numDoors = 0;
                        Console.WriteLine("Enter the ID of the door which belong to member of this fire alarm zone: [-1: ALL Member] [ID_1,ID_2 ...]");
                        Console.Write(">>>> ");
                        char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                        string[] doorIDs = Console.ReadLine().Split(delimiterChars);

                        foreach (string doorID in doorIDs)
                        {
                            if (doorID.Length > 0)
                            {
                                UInt32 item = 0;
                                if (doorID == "-1" || UInt32.TryParse(doorID, out item))
                                {
                                    if (doorID == "-1")
                                        item = UInt32.MaxValue;
                                    if (elm.numDoors + 1 >= BS2Environment.BS2_MAX_DOORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER)
                                    {
                                        Console.WriteLine("[Warning] The count of doorID should less than {0}.", BS2Environment.BS2_MAX_DOORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER);
                                        break;
                                    }

                                    elm.doorIDs[elm.numDoors] = item;
                                    elm.numDoors++;
                                }
                            }
                        }

                        byte[] elmArray = Util.ConvertTo<BS2DeviceZoneFireAlarmMember>(ref elm);
                        Array.Clear(bs2DeviceZone.zoneUnion, 0, bs2DeviceZone.zoneUnion.Length);
                        Array.Copy(elmArray, bs2DeviceZone.zoneUnion, elmArray.Length);
                    }
                    break;
                default:
                    Console.WriteLine("Wrong Zone Node Type");
                    return false;
            }

            return true;
        }
        #endregion

        void setDeviceZoneCfg(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2DeviceZoneConfig config = Util.AllocateStructure<BS2DeviceZoneConfig>();

            Console.WriteLine("How many zones do you want to set? [1(default)~4]");
            Console.Write(">>>> ");
            config.numOfZones = Util.GetInput(1);
            if (config.numOfZones > 4)
            {
                Console.WriteLine("Wrong Zone Count");
                return;                
            }

            BS2DeviceZoneAGEntranceLimit[] AGELs = Util.AllocateStructureArray<BS2DeviceZoneAGEntranceLimit>(config.numOfZones);
            UInt32 numAGEL = 0;

            for (int idxZone = 0; idxZone < config.numOfZones; ++idxZone )
            {
                UInt32 zoneID = 100; // (UInt32)new Random().Next();
                Console.WriteLine("Enter the ID for the zone which you want to set [{0}(default)]", zoneID);
                Console.Write(">>>> ");
                config.zone[idxZone].zoneID = Util.GetInput(zoneID);

                Console.WriteLine("Which zone type is this zone ? [3(default): ENTRANCE_LIMIT, 5: FIRE_ALARM]");
                Console.Write(">>>> ");
                config.zone[idxZone].zoneType = Util.GetInput((int)BS2_DEVICE_ZONE_TYPE.BS2_DEVICE_ZONE_TYPE_ENTRANCE_LIMIT);

                switch((BS2_DEVICE_ZONE_TYPE)config.zone[idxZone].zoneType)
                {
                    #region ENTRANCE_LIMIT
                    case BS2_DEVICE_ZONE_TYPE.BS2_DEVICE_ZONE_TYPE_ENTRANCE_LIMIT:                        
                        if (false == setEntranceLimit(ref config.zone[idxZone], ref AGELs[numAGEL]))
                            return;
                        if (AGELs[numAGEL].numAGEntranceLimit > 0)
                            numAGEL++;
                        break;
                    #endregion

                    #region FIRE_ALARM
                    case BS2_DEVICE_ZONE_TYPE.BS2_DEVICE_ZONE_TYPE_FIRE_ALARM:
                        if (false == setFireAlarm(ref config.zone[idxZone]))
                            return;
                        break;
                    #endregion

                    default:
                        Console.WriteLine("Wrong Zone Type");
                        return;
                }

                Console.WriteLine("Do you want to activate the zone {zoneID:" + config.zone[idxZone].zoneID + "} ? [Y/n]");
                Console.Write(">>>> ");
                if (Util.IsYes())
                {
                    config.zone[idxZone].enable = 1;
                }
                else
                {
                    config.zone[idxZone].enable = 0;
                }
            }

            Console.WriteLine("Trying to set DeviceZoneConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDeviceZoneConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
            else
            {
                if (numAGEL > 0)
                {
                    int structSize = Marshal.SizeOf(typeof(BS2DeviceZoneAGEntranceLimit));
                    IntPtr AGELsObj = Marshal.AllocHGlobal(structSize * (int)numAGEL);
                    for (int i = 0; i < numAGEL; i++)
                    {
                        IntPtr curAGELObj = AGELsObj + structSize * i;
                        Marshal.StructureToPtr(AGELs[i], curAGELObj, false);
                    }
                    result = (BS2ErrorCode)API.BS2_SetDeviceZoneAGEntranceLimit(sdkContext, deviceID, AGELsObj, numAGEL);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        Console.WriteLine("Got error({0}).", result);
                    }
                    else
                    {
                        getDeviceZoneCfg(sdkContext, deviceID, true);
                    }

                    Marshal.FreeHGlobal(AGELsObj);
                }
            }
        }

        void setDeviceZoneAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            byte alarmed = 1;
            Console.WriteLine("Do you want to release the device zone alarm? [Y/n]");
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

                Console.WriteLine("Trying to set the alarm of the device zone from device.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDeviceZoneAlarm(sdkContext, deviceID, alarmed, zoneIDObj, (UInt32)zoneIDList.Count);

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

        #region ENTRANCE_LIMIT
        void clearDeviceZoneAccessRecord(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
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
                result = (BS2ErrorCode)API.BS2_ClearAllDeviceZoneAccessRecord(sdkContext, deviceID, zoneID);
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
                    result = (BS2ErrorCode)API.BS2_ClearDeviceZoneAccessRecord(sdkContext, deviceID, zoneID, userIDObj, (UInt32)userIDList.Count);

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
        #endregion

    }

    #endregion
}
