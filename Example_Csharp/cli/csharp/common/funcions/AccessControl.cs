using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Suprema
{
    public class AccessControl : FunctionModule
    {
        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get access group", getAccessGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove access group", removeAccessGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set access group", setAccessGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get access level", getAccessLevel));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove access level", removeAccessLevel));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set access level", setAccessLevel));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get access schedule", getAccessSchedule));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove access schedule", removeAccessSchedule));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set access schedule", setAccessSchedule));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get holiday group", getHolidayGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove holiday group", removeHolidayGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set holiday group", setHolidayGroup));

            return functionList;
        }

        #region Access Group
        public void getAccessGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            IntPtr accessGroupObj = IntPtr.Zero;
            UInt32 numAccessGroup = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all access groups? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all access groups from device.");
                result = (BS2ErrorCode)API.BS2_GetAllAccessGroup(sdkContext, deviceID, out accessGroupObj, out numAccessGroup);
            }
            else
            {
                Console.WriteLine("Enter the ID of the access group which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> accessGroupIDList = new List<UInt32>();

                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            accessGroupIDList.Add(item);
                        }
                    }
                }

                if (accessGroupIDList.Count > 0)
                {
                    IntPtr accessGroupIDObj = Marshal.AllocHGlobal(4 * accessGroupIDList.Count);
                    IntPtr curAccessGroupIDObj = accessGroupIDObj;
                    foreach (UInt32 item in accessGroupIDList)
                    {
                        Marshal.WriteInt32(curAccessGroupIDObj, (Int32)item);
                        curAccessGroupIDObj = (IntPtr)((long)curAccessGroupIDObj + 4);
                    }

                    Console.WriteLine("Trying to get access groups from device.");
                    result = (BS2ErrorCode)API.BS2_GetAccessGroup(sdkContext, deviceID, accessGroupIDObj, (UInt32)accessGroupIDList.Count, out accessGroupObj, out numAccessGroup);

                    Marshal.FreeHGlobal(accessGroupIDObj);
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
            else if (numAccessGroup > 0)
            {
                IntPtr curAccessGroupObj = accessGroupObj;
                int structSize = Marshal.SizeOf(typeof(BS2AccessGroup));

                for (int idx = 0; idx < numAccessGroup; ++idx)
                {
                    BS2AccessGroup item = (BS2AccessGroup)Marshal.PtrToStructure(curAccessGroupObj, typeof(BS2AccessGroup));
                    print(sdkContext, item);
                    curAccessGroupObj = (IntPtr)((long)curAccessGroupObj + structSize);
                }

                API.BS2_ReleaseObject(accessGroupObj);
            }
            else
            {
                Console.WriteLine(">>> There is no access group in the device.");
            }
        }

        public void removeAccessGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all access groups? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all access groups from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllAccessGroup(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the access group which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> accessGroupIDList = new List<UInt32>();

                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            accessGroupIDList.Add(item);
                        }
                    }
                }

                if (accessGroupIDList.Count > 0)
                {
                    IntPtr accessGroupIDObj = Marshal.AllocHGlobal(4 * accessGroupIDList.Count);
                    IntPtr curAccessGroupIDObj = accessGroupIDObj;
                    foreach (UInt32 item in accessGroupIDList)
                    {
                        Marshal.WriteInt32(curAccessGroupIDObj, (Int32)item);
                        curAccessGroupIDObj = (IntPtr)((long)curAccessGroupIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove access groups from device.");
                    result = (BS2ErrorCode)API.BS2_RemoveAccessGroup(sdkContext, deviceID, accessGroupIDObj, (UInt32)accessGroupIDList.Count);

                    Marshal.FreeHGlobal(accessGroupIDObj);
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

        public void setAccessGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many access groups do you want to set? [1(default)-128]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2AccessGroup> accessGroupList = new List<BS2AccessGroup>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2AccessGroup accessGroup = Util.AllocateStructure<BS2AccessGroup>();

                Console.WriteLine("Enter a value for access group[{0}]", idx);
                Console.WriteLine("  Enter the ID for the access group which you want to set");
                Console.Write("  >>>> ");
                accessGroup.id = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the access group which you want to set");
                Console.Write("  >>>> ");
                string accessGroupName = Console.ReadLine();
                if (accessGroupName.Length == 0)
                {
                    Console.WriteLine("  [Warning] access group name will be displayed as empty.");
                }
                else if (accessGroupName.Length > BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN)
                {
                    Console.WriteLine("  Name of access group should less than {0} words.", BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] accessGroupArray = Encoding.UTF8.GetBytes(accessGroupName);
                    Array.Clear(accessGroup.name, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN);
                    Array.Copy(accessGroupArray, accessGroup.name, accessGroupArray.Length);
                }

                Console.WriteLine("  Enter the ID of the access level which you want to set: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);

                accessGroup.numOflevelUnion.numAccessLevels = 0;
                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            accessGroup.levelUnion.accessLevels[accessGroup.numOflevelUnion.numAccessLevels++] = item;
                        }
                    }
                }

                accessGroupList.Add(accessGroup);
            }

            int structSize = Marshal.SizeOf(typeof(BS2AccessGroup));
            IntPtr accessGroupListObj = Marshal.AllocHGlobal(structSize * accessGroupList.Count);
            IntPtr curAccessGroupListObj = accessGroupListObj;
            foreach (BS2AccessGroup item in accessGroupList)
            {
                Marshal.StructureToPtr(item, curAccessGroupListObj, false);
                curAccessGroupListObj = (IntPtr)((long)curAccessGroupListObj + structSize);
            }

            Console.WriteLine("Trying to set access groups to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAccessGroup(sdkContext, deviceID, accessGroupListObj, (UInt32)accessGroupList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(accessGroupListObj);
        }
        #endregion

        #region Access Level
        public void getAccessLevel(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            IntPtr accessLevelObj = IntPtr.Zero;
            UInt32 numAccessLevel = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all access levels? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all access levels from device.");
                result = (BS2ErrorCode)API.BS2_GetAllAccessLevel(sdkContext, deviceID, out accessLevelObj, out numAccessLevel);
            }
            else
            {
                Console.WriteLine("Enter the ID of the access level which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] accessLevelIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> accessLevelIDList = new List<UInt32>();

                foreach (string accessGroupID in accessLevelIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            accessLevelIDList.Add(item);
                        }
                    }
                }

                if (accessLevelIDList.Count > 0)
                {
                    IntPtr accessLevelIDObj = Marshal.AllocHGlobal(4 * accessLevelIDList.Count);
                    IntPtr curAccessLevelIDObj = accessLevelIDObj;
                    foreach (UInt32 item in accessLevelIDList)
                    {
                        Marshal.WriteInt32(curAccessLevelIDObj, (Int32)item);
                        curAccessLevelIDObj = (IntPtr)((long)curAccessLevelIDObj + 4);
                    }

                    Console.WriteLine("Trying to get access levels from device.");
                    result = (BS2ErrorCode)API.BS2_GetAccessLevel(sdkContext, deviceID, accessLevelIDObj, (UInt32)accessLevelIDList.Count, out accessLevelObj, out numAccessLevel);

                    Marshal.FreeHGlobal(accessLevelIDObj);
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
            else if (numAccessLevel > 0)
            {
                IntPtr curAccessLevelObj = accessLevelObj;
                int structSize = Marshal.SizeOf(typeof(BS2AccessLevel));

                for (int idx = 0; idx < numAccessLevel; ++idx)
                {
                    BS2AccessLevel item = (BS2AccessLevel)Marshal.PtrToStructure(curAccessLevelObj, typeof(BS2AccessLevel));
                    print(sdkContext, item);
                    curAccessLevelObj = (IntPtr)((long)curAccessLevelObj + structSize);
                }

                API.BS2_ReleaseObject(accessLevelObj);
            }
            else
            {
                Console.WriteLine(">>> There is no access level in the device.");
            }
        }

        public void removeAccessLevel(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all access levels? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all access levels from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllAccessLevel(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the access level which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] accessLevelIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> accessLevelIDList = new List<UInt32>();

                foreach (string accessLevelID in accessLevelIDs)
                {
                    if (accessLevelID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessLevelID, out item))
                        {
                            accessLevelIDList.Add(item);
                        }
                    }
                }

                if (accessLevelIDList.Count > 0)
                {
                    IntPtr accessLevelIDObj = Marshal.AllocHGlobal(4 * accessLevelIDList.Count);
                    IntPtr curAccessLevelIDObj = accessLevelIDObj;
                    foreach (UInt32 item in accessLevelIDList)
                    {
                        Marshal.WriteInt32(curAccessLevelIDObj, (Int32)item);
                        curAccessLevelIDObj = (IntPtr)((long)curAccessLevelIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove access levels from device.");
                    result = (BS2ErrorCode)API.BS2_RemoveAccessLevel(sdkContext, deviceID, accessLevelIDObj, (UInt32)accessLevelIDList.Count);

                    Marshal.FreeHGlobal(accessLevelIDObj);
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

        public void setAccessLevel(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many access levels do you want to set? [1(default)-128]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2AccessLevel> accessLevelList = new List<BS2AccessLevel>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2AccessLevel accessLevel = Util.AllocateStructure<BS2AccessLevel>();

                Console.WriteLine("Enter a value for access level[{0}]", idx);
                Console.WriteLine("  Enter the ID for the access level which you want to set [id < 32768]");
                Console.Write("  >>>> ");
                accessLevel.id = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the access level which you want to set");
                Console.Write("  >>>> ");
                string accessGroupName = Console.ReadLine();
                if (accessGroupName.Length == 0)
                {
                    Console.WriteLine("  [Warning] access level name will be displayed as empty.");
                }
                else if (accessGroupName.Length > BS2Environment.BS2_MAX_ACCESS_LEVEL_NAME_LEN)
                {
                    Console.WriteLine("  Name of access group should less than {0} words.", BS2Environment.BS2_MAX_ACCESS_LEVEL_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] accessGroupArray = Encoding.UTF8.GetBytes(accessGroupName);
                    Array.Clear(accessLevel.name, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN);
                    Array.Copy(accessGroupArray, accessLevel.name, accessGroupArray.Length);
                }

                Console.WriteLine("  How many door schedules do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_ACCESS_LEVEL_ITEMS);
                Console.Write("  >>>> ");
                accessLevel.numDoorSchedules = Util.GetInput(0);

                for (byte loop = 0; loop < accessLevel.numDoorSchedules; ++loop)
                {
                    Console.WriteLine("    Enter a value for door schedule[{0}]", loop);
                    Console.WriteLine("    Enter the door ID which you want to set");
                    Console.Write("    >>>> ");
                    accessLevel.doorSchedules[loop].doorID = (UInt32)Util.GetInput();
                    Console.WriteLine("    Enter the schedule ID which you want to set");
                    Console.Write("    >>>> ");
                    accessLevel.doorSchedules[loop].scheduleID = (UInt32)Util.GetInput();
                }

                accessLevelList.Add(accessLevel);
            }

            int structSize = Marshal.SizeOf(typeof(BS2AccessLevel));
            IntPtr accessLevelListObj = Marshal.AllocHGlobal(structSize * accessLevelList.Count);
            IntPtr curAccessLevelListObj = accessLevelListObj;
            foreach (BS2AccessLevel item in accessLevelList)
            {
                Marshal.StructureToPtr(item, curAccessLevelListObj, false);
                curAccessLevelListObj = (IntPtr)((long)curAccessLevelListObj + structSize);
            }

            Console.WriteLine("Trying to set access levels to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAccessLevel(sdkContext, deviceID, accessLevelListObj, (UInt32)accessLevelList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(accessLevelListObj);
        }
        #endregion

        #region Access Schedule
        public void getAccessSchedule(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            CSP_BS2Schedule[] accessScheduleObj = new CSP_BS2Schedule[0];
            UInt32 numAccessSchedule = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all access schedules? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all access schedules from device.");
                result = API.CSP_BS2_GetAllAccessSchedule(sdkContext, deviceID, out accessScheduleObj, out numAccessSchedule);
            }
            else
            {
                Console.WriteLine("Enter the ID of the access schedule which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] accessScheduleIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> accessScheduleIDList = new List<UInt32>();

                foreach (string accessScheduleID in accessScheduleIDs)
                {
                    if (accessScheduleID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessScheduleID, out item))
                        {
                            accessScheduleIDList.Add(item);
                        }
                    }
                }

                if (accessScheduleIDList.Count > 0)
                {
                    Console.WriteLine("Trying to get access schedules from device.");
                    result = API.CSP_BS2_GetAccessSchedule(sdkContext, deviceID, accessScheduleIDList.ToArray(), (UInt32)accessScheduleIDList.Count, out accessScheduleObj, out numAccessSchedule);
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
            else if (numAccessSchedule > 0)
            {
                for (int idx = 0; idx < numAccessSchedule; ++idx)
                {
                    print(sdkContext, accessScheduleObj[idx]);

                }
            }
            else
            {
                Console.WriteLine(">>> There is no access schedule in the device.");
            }
        }

        public void removeAccessSchedule(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all access schedules? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all access schedules from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllAccessSchedule(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the access schedule which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] accessScheduleIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> accessScheduleIDList = new List<UInt32>();

                foreach (string accessScheduleID in accessScheduleIDs)
                {
                    if (accessScheduleID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessScheduleID, out item))
                        {
                            accessScheduleIDList.Add(item);
                        }
                    }
                }

                if (accessScheduleIDList.Count > 0)
                {
                    result = (BS2ErrorCode)API.CSP_BS2_RemoveAccessSchedule(sdkContext, deviceID, accessScheduleIDList.ToArray(), (UInt32)accessScheduleIDList.Count);
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

        public void setAccessSchedule(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many access schedules do you want to set? [1(default)-128]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<CSP_BS2Schedule> accessScheduleList = new List<CSP_BS2Schedule>();

            for (int idx = 0; idx < amount; ++idx)
            {
                CSP_BS2Schedule accessSchedule = Util.AllocateStructure<CSP_BS2Schedule>();

                Console.WriteLine("Enter a value for access schedule[{0}]", idx);
                Console.WriteLine("  Enter the ID for the access schedule which you want to set");
                Console.Write("  >>>> ");
                accessSchedule.id = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the access schedule which you want to set");
                Console.Write("  >>>> ");
                string accessScheduleName = Console.ReadLine();
                if (accessScheduleName.Length == 0)
                {
                    Console.WriteLine("  [Warning] access schedule name will be displayed as empty.");
                }
                else if (accessScheduleName.Length > BS2Environment.BS2_MAX_SCHEDULE_NAME_LEN)
                {
                    Console.WriteLine("  Name of access schedule should less than {0} words.", BS2Environment.BS2_MAX_SCHEDULE_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] accessScheduleArray = Encoding.UTF8.GetBytes(accessScheduleName);
                    Array.Clear(accessSchedule.name, 0, BS2Environment.BS2_MAX_SCHEDULE_NAME_LEN);
                    Array.Copy(accessScheduleArray, accessSchedule.name, accessScheduleArray.Length);
                }

                Console.WriteLine("  How many holiday schedules do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_HOLIDAY_GROUPS_PER_SCHEDULE);
                Console.Write("  >>>> ");
                accessSchedule.numHolidaySchedules = Util.GetInput(0);

                for (byte loop = 0; loop < accessSchedule.numHolidaySchedules; ++loop)
                {
                    Console.WriteLine("    Enter a value for holiday group[{0}]", loop);
                    Console.WriteLine("    Enter the holiday group ID which you want to set");
                    Console.Write("    >>>> ");
                    accessSchedule.holidaySchedules[loop].id = (UInt32)Util.GetInput();

                    Console.WriteLine("    How many time period do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_TIME_PERIODS_PER_DAY);
                    Console.Write("    >>>> ");
                    accessSchedule.holidaySchedules[loop].schedule.numPeriods = (byte)Util.GetInput();

                    for (byte z = 0; z < accessSchedule.holidaySchedules[loop].schedule.numPeriods; ++z)
                    {
                        Console.WriteLine("      Enter a value for time period[{0}]", z);
                        Console.WriteLine("      Enter start time which you want to set(hh:mm)");
                        Console.Write("      >>>> ");
                        string[] startTimes = Console.ReadLine().Split(delimiterChars);

                        if (startTimes.Count() < 2)
                        {
                            Console.WriteLine("Invalid parameter");
                            return;
                        }

                        accessSchedule.holidaySchedules[loop].schedule.periods[z].startTime = (UInt16)(60 * Convert.ToUInt16(startTimes[0]) + Convert.ToUInt16(startTimes[1]));

                        Console.WriteLine("      Enter end time which you want to set(hh:mm)");
                        Console.Write("      >>>> ");
                        string[] endTimes = Console.ReadLine().Split(delimiterChars);

                        if (endTimes.Count() < 2)
                        {
                            Console.WriteLine("Invalid parameter");
                            return;
                        }

                        accessSchedule.holidaySchedules[loop].schedule.periods[z].endTime = (UInt16)(60 * Convert.ToUInt16(endTimes[0]) + Convert.ToUInt16(endTimes[1]));
                    }
                }

                Console.WriteLine("  Is daily schedule? [Y/n]");
                Console.Write("  >>>> ");
                bool isDailySchedule = true;
                string isDailyScheduleStr = Console.ReadLine();
                if (isDailyScheduleStr.Length != 0 && String.Compare(isDailyScheduleStr, "n", true) == 0)
                {
                    isDailySchedule = false;
                }

                accessSchedule.isDaily = Convert.ToByte(isDailySchedule);

                if (isDailySchedule)
                {
                    Console.WriteLine("    When is this schedule applied from? [default(Today), yyyy-MM-dd HH:mm:ss]");
                    Console.Write("    >>>> ");
                    if (!Util.GetTimestamp("yyyy-MM-dd HH:mm:ss", 0, out accessSchedule.scheduleUnion.daily.startDate))
                    {
                        return;
                    }

                    if (accessSchedule.scheduleUnion.daily.startDate == 0)
                    {
                        accessSchedule.scheduleUnion.daily.startDate = Convert.ToUInt32(Util.ConvertToUnixTimestamp(DateTime.Now));
                    }

                    Console.WriteLine("    How many days is this schedule applied for? [0(default)-{0}]", BS2Environment.BS2_MAX_DAYS_PER_DAILY_SCHEDULE);
                    Console.Write("    >>>> ");
                    accessSchedule.scheduleUnion.daily.numDays = Util.GetInput(0);

                    for (byte loop = 0; loop < accessSchedule.scheduleUnion.daily.numDays; ++loop)
                    {
                        Console.WriteLine("      Enter a value for daily schedule[{0}]", loop);
                        Console.WriteLine("      How many time period do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_TIME_PERIODS_PER_DAY);
                        Console.Write("      >>>> ");
                        accessSchedule.scheduleUnion.daily.schedule[loop].numPeriods = (byte)Util.GetInput();

                        for (byte z = 0; z < accessSchedule.scheduleUnion.daily.schedule[loop].numPeriods; ++z)
                        {
                            Console.WriteLine("        Enter a value for time period[{0}]", z);
                            Console.WriteLine("        Enter start time which you want to set(hh:mm)");
                            Console.Write("        >>>> ");
                            string[] startTimes = Console.ReadLine().Split(delimiterChars);

                            if (startTimes.Count() < 2)
                            {
                                Console.WriteLine("Invalid parameter");
                                return;
                            }

                            accessSchedule.scheduleUnion.daily.schedule[loop].periods[z].startTime = (UInt16)(60 * Convert.ToUInt16(startTimes[0]) + Convert.ToUInt16(startTimes[1]));

                            Console.WriteLine("        Enter end time which you want to set(hh:mm)");
                            Console.Write("        >>>> ");
                            string[] endTimes = Console.ReadLine().Split(delimiterChars);

                            if (endTimes.Count() < 2)
                            {
                                Console.WriteLine("Invalid parameter");
                                return;
                            }

                            accessSchedule.scheduleUnion.daily.schedule[loop].periods[z].endTime = (UInt16)(60 * Convert.ToUInt16(endTimes[0]) + Convert.ToUInt16(endTimes[1]));
                        }
                    }
                }
                else
                {
                    for (byte loop = 0; loop < BS2Environment.BS2_NUM_WEEKDAYS; ++loop)
                    {
                        Console.WriteLine("    Enter the weekly schedule for {0}", ((DayOfWeek)loop).ToString());
                        Console.WriteLine("    How many time period do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_TIME_PERIODS_PER_DAY);
                        Console.Write("    >>>> ");
                        accessSchedule.scheduleUnion.weekly.schedule[loop].numPeriods = (byte)Util.GetInput();

                        for (byte z = 0; z < accessSchedule.scheduleUnion.weekly.schedule[loop].numPeriods; ++z)
                        {
                            Console.WriteLine("      Enter a value for time period[{0}]", z);
                            Console.WriteLine("      Enter start time which you want to set(hh:mm)");
                            Console.Write("      >>>> ");
                            string[] startTimes = Console.ReadLine().Split(delimiterChars);

                            if (startTimes.Count() < 2)
                            {
                                Console.WriteLine("Invalid parameter");
                                return;
                            }

                            accessSchedule.scheduleUnion.weekly.schedule[loop].periods[z].startTime = (UInt16)(60 * Convert.ToUInt16(startTimes[0]) + Convert.ToUInt16(startTimes[1]));

                            Console.WriteLine("      Enter end time which you want to set(hh:mm)");
                            Console.Write("      >>>> ");
                            string[] endTimes = Console.ReadLine().Split(delimiterChars);

                            if (endTimes.Count() < 2)
                            {
                                Console.WriteLine("Invalid parameter");
                                return;
                            }

                            accessSchedule.scheduleUnion.weekly.schedule[loop].periods[z].endTime = (UInt16)(60 * Convert.ToUInt16(endTimes[0]) + Convert.ToUInt16(endTimes[1]));
                        }
                    }
                }

                accessScheduleList.Add(accessSchedule);
            }


            Console.WriteLine("Trying to set access schedules to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.CSP_BS2_SetAccessSchedule(sdkContext, deviceID, accessScheduleList.ToArray(), (UInt32)accessScheduleList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }
        }
        #endregion

        #region Holiday Group
        public void getHolidayGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            IntPtr holidayGroupObj = IntPtr.Zero;
            UInt32 numHolidayGroup = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all holiday groups? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all holiday groups from device.");
                result = (BS2ErrorCode)API.BS2_GetAllHolidayGroup(sdkContext, deviceID, out holidayGroupObj, out numHolidayGroup);
            }
            else
            {
                Console.WriteLine("Enter the ID of the holiday group which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] holidayGroupIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> holidayGroupIDList = new List<UInt32>();

                foreach (string holidayGroupID in holidayGroupIDs)
                {
                    if (holidayGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(holidayGroupID, out item))
                        {
                            holidayGroupIDList.Add(item);
                        }
                    }
                }

                if (holidayGroupIDList.Count > 0)
                {
                    IntPtr holidayGroupIDObj = Marshal.AllocHGlobal(4 * holidayGroupIDList.Count);
                    IntPtr curHolidayGroupIDObj = holidayGroupIDObj;
                    foreach (UInt32 item in holidayGroupIDList)
                    {
                        Marshal.WriteInt32(curHolidayGroupIDObj, (Int32)item);
                        curHolidayGroupIDObj = (IntPtr)((long)curHolidayGroupIDObj + 4);
                    }

                    Console.WriteLine("Trying to get holiday groups from device.");
                    result = (BS2ErrorCode)API.BS2_GetHolidayGroup(sdkContext, deviceID, holidayGroupIDObj, (UInt32)holidayGroupIDList.Count, out holidayGroupObj, out numHolidayGroup);

                    Marshal.FreeHGlobal(holidayGroupIDObj);
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
            else if (numHolidayGroup > 0)
            {
                IntPtr curHolidayGroupObj = holidayGroupObj;
                int structSize = Marshal.SizeOf(typeof(BS2HolidayGroup));

                for (int idx = 0; idx < numHolidayGroup; ++idx)
                {
                    BS2HolidayGroup item = (BS2HolidayGroup)Marshal.PtrToStructure(curHolidayGroupObj, typeof(BS2HolidayGroup));
                    print(sdkContext, item);
                    curHolidayGroupObj = (IntPtr)((long)curHolidayGroupObj + structSize);
                }

                API.BS2_ReleaseObject(holidayGroupObj);
            }
            else
            {
                Console.WriteLine(">>> There is no holiday group in the device.");
            }
        }

        public void removeHolidayGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all holiday groups? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all holiday groups from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllHolidayGroup(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the holiday group which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] holidayGroupIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> holidayGroupIDList = new List<UInt32>();

                foreach (string holidayGroupID in holidayGroupIDs)
                {
                    if (holidayGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(holidayGroupID, out item))
                        {
                            holidayGroupIDList.Add(item);
                        }
                    }
                }

                if (holidayGroupIDList.Count > 0)
                {
                    IntPtr holidayGroupIDObj = Marshal.AllocHGlobal(4 * holidayGroupIDList.Count);
                    IntPtr curHolidayGroupIDObj = holidayGroupIDObj;
                    foreach (UInt32 item in holidayGroupIDList)
                    {
                        Marshal.WriteInt32(curHolidayGroupIDObj, (Int32)item);
                        curHolidayGroupIDObj = (IntPtr)((long)curHolidayGroupIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove holiday groups from device.");
                    result = (BS2ErrorCode)API.BS2_RemoveHolidayGroup(sdkContext, deviceID, holidayGroupIDObj, (UInt32)holidayGroupIDList.Count);

                    Marshal.FreeHGlobal(holidayGroupIDObj);
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

        public void setHolidayGroup(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many holiday groups do you want to set? [1(default)-128]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2HolidayGroup> holidayGroupList = new List<BS2HolidayGroup>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2HolidayGroup holidayGroup = Util.AllocateStructure<BS2HolidayGroup>();

                Console.WriteLine("Enter a value for holiday group[{0}]", idx);
                Console.WriteLine("  Enter the ID for the holiday group which you want to set");
                Console.Write("  >>>> ");
                holidayGroup.id = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the holiday group which you want to set");
                Console.Write("  >>>> ");
                string holidayGroupName = Console.ReadLine();
                if (holidayGroupName.Length == 0)
                {
                    Console.WriteLine("  [Warning] holiday group name will be displayed as empty.");
                }
                else if (holidayGroupName.Length > BS2Environment.BS2_MAX_HOLIDAY_GROUP_NAME_LEN)
                {
                    Console.WriteLine("  Name of holiday group should less than {0} words.", BS2Environment.BS2_MAX_HOLIDAY_GROUP_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] holidayGroupArray = Encoding.UTF8.GetBytes(holidayGroupName);
                    Array.Clear(holidayGroup.name, 0, BS2Environment.BS2_MAX_HOLIDAY_GROUP_NAME_LEN);
                    Array.Copy(holidayGroupArray, holidayGroup.name, holidayGroupArray.Length);
                }

                Console.WriteLine("  How many holidays do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_HOLIDAYS_PER_GROUP);
                Console.Write("  >>>> ");
                holidayGroup.numHolidays = Util.GetInput(0);

                for (byte loop = 0; loop < holidayGroup.numHolidays; ++loop)
                {
                    Console.WriteLine("    Enter a value for holiday[{0}]", loop);
                    Console.WriteLine("    What day is this holiday? [YYYY-MM-DD]");
                    Console.Write("    >>>> ");
                    if (!Util.GetTimestamp("yyyy-MM-dd", 0, out holidayGroup.holidays[loop].date))
                    {
                        return;
                    }

                    Console.WriteLine("    Enter the recurrence type which you want to set [0(default) : none, 1 : every year, 2 : every month, 3: every week]");
                    Console.Write("    >>>> ");
                    holidayGroup.holidays[loop].recurrence = (byte)Util.GetInput();
                }

                holidayGroupList.Add(holidayGroup);
            }

            int structSize = Marshal.SizeOf(typeof(BS2HolidayGroup));
            IntPtr holidayGroupListObj = Marshal.AllocHGlobal(structSize * holidayGroupList.Count);
            IntPtr curHolidayGroupListObj = holidayGroupListObj;
            foreach (BS2HolidayGroup item in holidayGroupList)
            {
                Marshal.StructureToPtr(item, curHolidayGroupListObj, false);
                curHolidayGroupListObj = (IntPtr)((long)curHolidayGroupListObj + structSize);
            }

            Console.WriteLine("Trying to set holiday groups to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetHolidayGroup(sdkContext, deviceID, holidayGroupListObj, (UInt32)holidayGroupList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(holidayGroupListObj);
        }
        #endregion
       
        void print(IntPtr sdkContext, BS2AccessGroup accessGroup)
        {
            Console.WriteLine(">>>> AccessGroup id[{0}] name[{1}]", accessGroup.id, Encoding.UTF8.GetString(accessGroup.name).TrimEnd('\0'));
            Console.WriteLine("     |--accessLevels");
            for (byte loop = 0; loop < accessGroup.numOflevelUnion.numAccessLevels; ++loop)
            {
                Console.WriteLine("     |  |--accessLevelID[{0}]", accessGroup.levelUnion.accessLevels[loop]);
            }
        }

        void print(IntPtr sdkContext, BS2AccessLevel accessLevel)
        {
            Console.WriteLine(">>>> AccessLevel id[{0}] name[{1}]", accessLevel.id, Encoding.UTF8.GetString(accessLevel.name).TrimEnd('\0'));
            Console.WriteLine("     |--doorSchedules");
            for (byte loop = 0; loop < accessLevel.numDoorSchedules; ++loop)
            {
                Console.WriteLine("     |  |--doorID[{0}] scheduleID[{1}]", accessLevel.doorSchedules[loop].doorID, accessLevel.doorSchedules[loop].scheduleID);
            }
        }

        void print(IntPtr sdkContext, CSP_BS2Schedule schedule)
        {
            Console.WriteLine(">>>> Schedule id[{0}] name[{1}]", schedule.id, Encoding.UTF8.GetString(schedule.name).TrimEnd('\0'));
            if (Convert.ToBoolean(schedule.isDaily))
            {
                Console.WriteLine("     |--dailySchedules startDate[{0}] numDays[{1}]", Util.ConvertFromUnixTimestamp(schedule.scheduleUnion.daily.startDate).ToString("yyyy-MM-dd HH:mm:ss"), schedule.scheduleUnion.daily.numDays);
                for (byte loop = 0; loop < schedule.scheduleUnion.daily.numDays; ++loop)
                {
                    Console.Write("     |  |--schedule[{0, 2}] [", loop);
                    for (byte z = 0; z < schedule.scheduleUnion.daily.schedule[loop].numPeriods; ++z)
                    {
                        UInt32 startTime = (UInt32)60 * schedule.scheduleUnion.daily.schedule[loop].periods[z].startTime;
                        UInt32 endTime = (UInt32)60 * schedule.scheduleUnion.daily.schedule[loop].periods[z].endTime;

                        if (z + 1 < schedule.scheduleUnion.daily.schedule[loop].numPeriods)
                        {
                            Console.Write("{0}-{1}, ", Util.ConvertFromUnixTimestamp(startTime).ToString("HH:mm"), Util.ConvertFromUnixTimestamp(endTime).ToString("HH:mm"));
                        }
                        else
                        {
                            Console.Write("{0}-{1}", Util.ConvertFromUnixTimestamp(startTime).ToString("HH:mm"), Util.ConvertFromUnixTimestamp(endTime).ToString("HH:mm"));
                        }
                    }
                    Console.WriteLine("]");
                }
            }
            else
            {
                Console.WriteLine("     |--weeklySchedules");
                for (byte loop = 0; loop < BS2Environment.BS2_NUM_WEEKDAYS; ++loop)
                {
                    Console.Write("     |  |--schedule[{0, 10}] [", ((DayOfWeek)loop).ToString());
                    for (byte z = 0; z < schedule.scheduleUnion.weekly.schedule[loop].numPeriods; ++z)
                    {
                        UInt32 startTime = (UInt32)60 * schedule.scheduleUnion.weekly.schedule[loop].periods[z].startTime;
                        UInt32 endTime = (UInt32)60 * schedule.scheduleUnion.weekly.schedule[loop].periods[z].endTime;

                        if (z + 1 < schedule.scheduleUnion.weekly.schedule[loop].numPeriods)
                        {
                            Console.Write("{0}-{1}, ", Util.ConvertFromUnixTimestamp(startTime).ToString("HH:mm"), Util.ConvertFromUnixTimestamp(endTime).ToString("HH:mm"));
                        }
                        else
                        {
                            Console.Write("{0}-{1}", Util.ConvertFromUnixTimestamp(startTime).ToString("HH:mm"), Util.ConvertFromUnixTimestamp(endTime).ToString("HH:mm"));
                        }
                    }
                    Console.WriteLine("]");
                }
            }

            Console.WriteLine("     |--holidaySchedules numDays[{0}]", schedule.numHolidaySchedules);
            for (byte loop = 0; loop < schedule.numHolidaySchedules; ++loop)
            {
                Console.Write("     |  |-- id[{0}] schedule[", schedule.holidaySchedules[loop].id);

                for (byte z = 0; z < schedule.holidaySchedules[loop].schedule.numPeriods; ++z)
                {
                    UInt32 startTime = (UInt32)60 * schedule.holidaySchedules[loop].schedule.periods[z].startTime;
                    UInt32 endTime = (UInt32)60 * schedule.holidaySchedules[loop].schedule.periods[z].endTime;

                    if (z + 1 < schedule.holidaySchedules[loop].schedule.numPeriods)
                    {
                        Console.Write("{0}-{1}, ", Util.ConvertFromUnixTimestamp(startTime).ToString("HH:mm"), Util.ConvertFromUnixTimestamp(endTime).ToString("HH:mm"));
                    }
                    else
                    {
                        Console.Write("{0}-{1}", Util.ConvertFromUnixTimestamp(startTime).ToString("HH:mm"), Util.ConvertFromUnixTimestamp(endTime).ToString("HH:mm"));
                    }
                }
                Console.WriteLine("]");
            }
        }

        void print(IntPtr sdkContext, BS2HolidayGroup holidayGroup)
        {
            Console.WriteLine(">>>> HolidayGroup id[{0}] name[{1}]", holidayGroup.id, Encoding.UTF8.GetString(holidayGroup.name).TrimEnd('\0'));
            Console.WriteLine("     |--holidays");
            for (byte loop = 0; loop < holidayGroup.numHolidays; ++loop)
            {
                Console.WriteLine("     |  |--date[{0}] recurrence[{1}]", Util.ConvertFromUnixTimestamp(holidayGroup.holidays[loop].date).ToString("yyyy-MM-dd"), holidayGroup.holidays[loop].recurrence);
            }
        }
    }
}
