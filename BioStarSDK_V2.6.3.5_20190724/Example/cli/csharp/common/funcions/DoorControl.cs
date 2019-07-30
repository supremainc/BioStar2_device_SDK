using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class DoorControl : FunctionModule
    {
        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get door", getDoor));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get door status", getDoorStatus));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove door", removeDoor));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set door", setDoor));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set door alarm", setDoorAlarm));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Release door", releaseDoor));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Lock door", lockDoor));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Unlock door", unlockDoor));
            
            return functionList;
        }

        public void getDoor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            IntPtr doorObj = IntPtr.Zero;
            UInt32 numDoor = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all doors? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all doors from device.");
                result = (BS2ErrorCode)API.BS2_GetAllDoor(sdkContext, deviceID, out doorObj, out numDoor);
            }
            else
            {
                Console.WriteLine("Enter the ID of the door which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] doorIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> doorIDList = new List<UInt32>();

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

                if (doorIDList.Count > 0)
                {
                    IntPtr doorIDObj = Marshal.AllocHGlobal(4 * doorIDList.Count);
                    IntPtr curDoorIDObj = doorIDObj;
                    foreach (UInt32 item in doorIDList)
                    {
                        Marshal.WriteInt32(curDoorIDObj, (Int32)item);
                        curDoorIDObj = (IntPtr)((long)curDoorIDObj + 4);
                    }

                    Console.WriteLine("Trying to get doors from device.");
                    result = (BS2ErrorCode)API.BS2_GetDoor(sdkContext, deviceID, doorIDObj, (UInt32)doorIDList.Count, out doorObj, out numDoor);

                    Marshal.FreeHGlobal(doorIDObj);
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
            else if (numDoor > 0)
            {
                IntPtr curDoorObj = doorObj;
                int structSize = Marshal.SizeOf(typeof(BS2Door));

                for (int idx = 0; idx < numDoor; ++idx)
                {
                    BS2Door item = (BS2Door)Marshal.PtrToStructure(curDoorObj, typeof(BS2Door));
                    print(sdkContext, item);
                    curDoorObj = (IntPtr)((long)curDoorObj + structSize);
                }

                API.BS2_ReleaseObject(doorObj);
            }
            else
            {
                Console.WriteLine(">>> There is no door in the device.");
            }
        }

        public void getDoorStatus(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the door which you want to get its status: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] doorIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> doorIDList = new List<UInt32>();

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

            if (doorIDList.Count > 0)
            {
                IntPtr doorStatusObj = IntPtr.Zero;
                UInt32 numDoorStatus = 0;
                IntPtr doorIDObj = Marshal.AllocHGlobal(4 * doorIDList.Count);
                IntPtr curDoorIDObj = doorIDObj;
                foreach (UInt32 item in doorIDList)
                {
                    Marshal.WriteInt32(curDoorIDObj, (Int32)item);
                    curDoorIDObj = (IntPtr)((long)curDoorIDObj + 4);
                }

                Console.WriteLine("Trying to get the status of the doors from device.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDoorStatus(sdkContext, deviceID, doorIDObj, (UInt32)doorIDList.Count, out doorStatusObj, out numDoorStatus);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }
                else if (numDoorStatus > 0)
                {
                    IntPtr curDoorStatusObj = doorStatusObj;
                    int structSize = Marshal.SizeOf(typeof(BS2DoorStatus));

                    for (int idx = 0; idx < numDoorStatus; ++idx)
                    {
                        BS2DoorStatus item = (BS2DoorStatus)Marshal.PtrToStructure(curDoorStatusObj, typeof(BS2DoorStatus));
                        print(sdkContext, item);
                        curDoorStatusObj = (IntPtr)((long)curDoorStatusObj + structSize);
                    }

                    API.BS2_ReleaseObject(doorStatusObj);
                }
                else
                {
                    Console.WriteLine(">>> There is no such door in the device.");
                }

                Marshal.FreeHGlobal(doorIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        public void removeDoor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all doors? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all doors from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllDoor(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the door which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] doorIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> doorIDList = new List<UInt32>();

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

                if (doorIDList.Count > 0)
                {
                    IntPtr doorIDObj = Marshal.AllocHGlobal(4 * doorIDList.Count);
                    IntPtr curDoorIDObj = doorIDObj;
                    foreach (UInt32 item in doorIDList)
                    {
                        Marshal.WriteInt32(curDoorIDObj, (Int32)item);
                        curDoorIDObj = (IntPtr)((long)curDoorIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove doors from device.");
                    result = (BS2ErrorCode)API.BS2_RemoveDoor(sdkContext, deviceID, doorIDObj, (UInt32)doorIDList.Count);

                    Marshal.FreeHGlobal(doorIDObj);
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

        public void setDoor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many doors do you want to set? [1(default)-128]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2Door> doorList = new List<BS2Door>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2Door door = Util.AllocateStructure<BS2Door>();

                Console.WriteLine("Enter a value for door[{0}]", idx);
                Console.WriteLine("  Enter the ID for the door which you want to set");
                Console.Write("  >>>> ");
                door.doorID = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the door which you want to set");
                Console.Write("  >>>> ");
                string doorName = Console.ReadLine();
                if (doorName.Length == 0)
                {
                    Console.WriteLine("  [Warning] door name will be displayed as empty.");
                }
                else if (doorName.Length > BS2Environment.BS2_MAX_DOOR_NAME_LEN)
                {
                    Console.WriteLine("  Name of door should less than {0} words.", BS2Environment.BS2_MAX_DOOR_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] doorArray = Encoding.UTF8.GetBytes(doorName);
                    Array.Clear(door.name, 0, BS2Environment.BS2_MAX_DOOR_NAME_LEN);
                    Array.Copy(doorArray, door.name, doorArray.Length);
                }

                Console.WriteLine("  Enter the ID of the Reader for entrance");
                Console.Write("  >>>> ");
                door.entryDeviceID = Util.GetInput((UInt32)0);

                Console.WriteLine("  Enter the ID of the Reader for exit");
                Console.Write("  >>>> ");
                door.exitDeviceID = Util.GetInput((UInt32)0);

                Console.WriteLine("  Enter the AutoLock timeout in seconds: [3(default)]");
                Console.Write("  >>>> ");
                door.autoLockTimeout = Util.GetInput((UInt32)3);

                Console.WriteLine("  Enter the HeldOpen timeout in seconds: [3(default)]");
                Console.Write("  >>>> ");
                door.heldOpenTimeout = Util.GetInput((UInt32)3);

                Console.WriteLine("  Should this Door be locked instantly when it is closed? [Y/n]");
                Console.Write("  >>>> ");
                if(Util.IsYes())
                {
                    door.instantLock = 1;
                }
                else
                {
                    door.instantLock = 0;
                }                

                Console.WriteLine("  Does this door has a relay? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("  Enter the device id for the relay on this door.");
                    Console.Write("  >>>> ");
                    door.relay.deviceID = (UInt32)Util.GetInput();

                    Console.WriteLine("  Enter the the port of the relay on this door.[0(default)]");
                    Console.Write("  >>>> ");
                    door.relay.port = Util.GetInput(0);
                }

                Console.WriteLine("  Does this door has a door sensor? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("  Enter the device id of the door sensor on this door.");
                    Console.Write("  >>>> ");
                    door.sensor.deviceID = (UInt32)Util.GetInput();

                    Console.WriteLine("  Enter the the port of the door sensor on this door.[0(default)]");
                    Console.Write("  >>>> ");
                    door.sensor.port = Util.GetInput(0);

                    Console.WriteLine("  Enter the switch type of the door sensor on this door: [0: normally open, 1: normally closed].");
                    Console.Write("  >>>> ");
                    door.sensor.switchType = Util.GetInput(0);
                }

                Console.WriteLine("  Does this door has a exit button? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    Console.WriteLine("  Enter the device id of the exit button on this door.");
                    Console.Write("  >>>> ");
                    door.button.deviceID = (UInt32)Util.GetInput();

                    Console.WriteLine("  Enter the the port of the exit button on this door.[0(default)]");
                    Console.Write("  >>>> ");
                    door.button.port = Util.GetInput(0);

                    Console.WriteLine("  Enter the switch type of the exit button on this door: [0: normally open, 1: normally closed].");
                    Console.Write("  >>>> ");
                    door.button.switchType = Util.GetInput(0);
                }

                Console.WriteLine("  How to act at lock door? [0: None(default), 1: Schedule, 2: Emergency, 4: Operator]");
                Console.Write("  >>>> ");
                door.lockFlags = Util.GetInput((byte)BS2DoorFlagEnum.NONE);

                Console.WriteLine("  How to act at unlock door? [0: None(default), 1: Schedule, 2: Emergency, 4: Operator]");
                Console.Write("  >>>> ");
                door.unlockFlags = Util.GetInput((byte)BS2DoorFlagEnum.NONE);

                //BS2DoorAlarmFlagEnum doorAlarmFlag = BS2DoorAlarmFlagEnum.NONE;
                for (int loop = 0; loop < BS2Environment.BS2_MAX_FORCED_OPEN_ALARM_ACTION; ++loop)
                {
                    door.forcedOpenAlarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                }

                for (int loop = 0; loop < BS2Environment.BS2_MAX_HELD_OPEN_ALARM_ACTION; ++loop)
                {
                    door.heldOpenAlarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                }

#if false //please refer to ZoneControl
                Console.WriteLine("  Does this door need to forced open door alarm? [y/N]");
                if (!Util.IsNo())
                {
                    Console.WriteLine("  How many forced open door alarm do you want to set? [1(default)-{0}]", BS2Environment.BS2_MAX_FORCED_OPEN_ALARM_ACTION);
                    Console.Write("  >>>> ");
                    int alarmCount = Util.GetInput(1);

                    if (alarmCount > 0)
                    {
                        doorAlarmFlag |= BS2DoorAlarmFlagEnum.FORCED_OPEN;

                        for (int loop = 0; loop < alarmCount; ++loop)
                        {
                            Console.WriteLine("  Enter the action type which you want to set [6(default) : relay, 7 : ttl, 8 : sound, 9: display, 10 : buzzer, 11: led]");
                            Console.Write("  >>>> ");
                            door.forcedOpenAlarm[loop].type = Util.GetInput((byte)BS2ActionTypeEnum.RELAY);

                            switch ((BS2ActionTypeEnum)door.forcedOpenAlarm[loop].type)
                            {
                                case BS2ActionTypeEnum.RELAY:
                                    {
                                        BS2RelayAction relay = Util.AllocateStructure<BS2RelayAction>();

                                        Console.WriteLine("  Enter the the port of the relay on this door.[0(default)]");
                                        Console.Write("  >>>> ");
                                        relay.relayIndex = Util.GetInput(0);                                        
                                    }
                                    break;
                            }
                        }
                    }
                }
#endif

                //door.unconditionalLock = (byte)doorAlarmFlag;
                Console.WriteLine("  Should this Door be locked after autoLock timeout? [Y/n]");
                Console.Write("  >>>> ");
                if (Util.IsYes())
                {
                    door.unconditionalLock = 1;
                }
                else
                {
                    door.unconditionalLock = 0;
                }                

                Console.WriteLine("  Does this door need to dual authentication? [y/N]");
                Console.Write("  >>>> ");
                if (Util.IsNo())
                {
                    door.dualAuthDevice = (byte)BS2DualAuthDeviceEnum.NO_DEVICE;
                    door.dualAuthScheduleID = (UInt32)BS2ScheduleIDEnum.NEVER;
                    door.dualAuthTimeout = 0;
                    door.dualAuthApprovalType = (byte)BS2DualAuthApprovalEnum.NONE;
                    door.numDualAuthApprovalGroups = 0;
                }
                else
                {
                    Console.WriteLine("  Which reader requires dual authentication? [1: Entrance Only(default), 2: Exit Only, 3: Both]");
                    Console.Write("  >>>> ");
                    door.dualAuthDevice = Util.GetInput((byte)BS2DualAuthDeviceEnum.ENTRY_DEVICE_ONLY);

                    Console.WriteLine("  Enter the id of access schedule for dual authentication: [0: Never, 1: Always(default), or the other schedule id]");
                    Console.Write("  >>>> ");
                    door.dualAuthScheduleID = Util.GetInput((UInt32)BS2ScheduleIDEnum.ALWAYS);

                    Console.WriteLine("  Enter the dual authentication timeout in seconds: [5(default)]");
                    Console.Write("  >>>> ");
                    door.dualAuthTimeout = Util.GetInput((UInt32)5);

                    Console.WriteLine("  Who should be the dual authentication approver for this door? [0: Not required(default), 1: Second user] ");
                    Console.Write("  >>>> ");
                    door.dualAuthApprovalType = Util.GetInput((byte)BS2DualAuthApprovalEnum.NONE);

                    Console.WriteLine("  Enter the ID of access groups for dual authentication approval: [ID_1,ID_2 ...]");
                    Console.Write("  >>>> ");
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

                    door.numDualAuthApprovalGroups = (byte)accessGroupIDList.Count;
                    for (int loop = 0; loop < accessGroupIDList.Count; ++loop)
                    {
                        door.dualAuthApprovalGroupID[loop] = accessGroupIDList[loop];
                    }
                    
                    //If you want to set up one door apb zone, please refer to ZoneControl section.
                }

                doorList.Add(door);
            }

            int structSize = Marshal.SizeOf(typeof(BS2Door));
            IntPtr doorListObj = Marshal.AllocHGlobal(structSize * doorList.Count);
            IntPtr curDoorListObj = doorListObj;
            foreach (BS2Door item in doorList)
            {
                Marshal.StructureToPtr(item, curDoorListObj, false);
                curDoorListObj = (IntPtr)((long)curDoorListObj + structSize);
            }

            Console.WriteLine("Trying to set doors to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDoor(sdkContext, deviceID, doorListObj, (UInt32)doorList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(doorListObj);
        }

        public void setDoorAlarm(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the door which you want to set its alarm: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] doorIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> doorIDList = new List<UInt32>();

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

            if (doorIDList.Count > 0)
            {
                IntPtr doorIDObj = Marshal.AllocHGlobal(4 * doorIDList.Count);
                IntPtr curDoorIDObj = doorIDObj;
                foreach (UInt32 item in doorIDList)
                {
                    Marshal.WriteInt32(curDoorIDObj, (Int32)item);
                    curDoorIDObj = (IntPtr)((long)curDoorIDObj + 4);
                }

                Console.WriteLine("Choose the flag for the doors to be released: [0: None(default), 1: Forced open, 2: Held open, 4: APB]");
                Console.Write(">>>> ");
                byte validDoorAlarmFlag = (byte)(BS2DoorAlarmFlagEnum.NONE | BS2DoorAlarmFlagEnum.FORCED_OPEN | BS2DoorAlarmFlagEnum.HELD_OPEN | BS2DoorAlarmFlagEnum.APB);
                byte doorAlarmFlag = Util.GetInput((byte)BS2DoorAlarmFlagEnum.NONE);

                if ((doorAlarmFlag & ~validDoorAlarmFlag) != 0)
                {
                    Console.WriteLine("Invalid parameter");
                    Marshal.FreeHGlobal(doorIDObj);
                    return;
                }

                Console.WriteLine("Trying to set door alarm.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDoorAlarm(sdkContext, deviceID, doorAlarmFlag, doorIDObj, (UInt32)doorIDList.Count);

                Marshal.FreeHGlobal(doorIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        public void releaseDoor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the door which you want to release: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] doorIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> doorIDList = new List<UInt32>();

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

            if (doorIDList.Count > 0)
            {
                IntPtr doorIDObj = Marshal.AllocHGlobal(4 * doorIDList.Count);
                IntPtr curDoorIDObj = doorIDObj;
                foreach (UInt32 item in doorIDList)
                {
                    Marshal.WriteInt32(curDoorIDObj, (Int32)item);
                    curDoorIDObj = (IntPtr)((long)curDoorIDObj + 4);
                }

                Console.WriteLine("Choose the flag for the doors to be released: [0: None, 1: By Schedule, 2: By Emergency, 4: By Operator(default)]");
                Console.Write(">>>> ");
                byte validDoorFlag = (byte)(BS2DoorFlagEnum.NONE | BS2DoorFlagEnum.SCHEDULE | BS2DoorFlagEnum.EMERGENCY | BS2DoorFlagEnum.OPERATOR);
                byte doorFlag = Util.GetInput((byte)BS2DoorFlagEnum.OPERATOR);

                if ((doorFlag & ~validDoorFlag) != 0)
                {
                    Console.WriteLine("Invalid parameter");
                    Marshal.FreeHGlobal(doorIDObj);
                    return;
                }

                Console.WriteLine("Trying to release doors.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_ReleaseDoor(sdkContext, deviceID, doorFlag, doorIDObj, (UInt32)doorIDList.Count);

                Marshal.FreeHGlobal(doorIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        public void lockDoor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the door which you want to lock: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] doorIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> doorIDList = new List<UInt32>();

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

            if (doorIDList.Count > 0)
            {
                IntPtr doorIDObj = Marshal.AllocHGlobal(4 * doorIDList.Count);
                IntPtr curDoorIDObj = doorIDObj;
                foreach (UInt32 item in doorIDList)
                {
                    Marshal.WriteInt32(curDoorIDObj, (Int32)item);
                    curDoorIDObj = (IntPtr)((long)curDoorIDObj + 4);
                }

                Console.WriteLine("Choose the flag for the doors to be locked: [0: None, 1: By Schedule, 2: By Emergency, 4: By Operator(default)]");
                Console.Write(">>>> ");
                byte validDoorFlag = (byte)(BS2DoorFlagEnum.NONE | BS2DoorFlagEnum.SCHEDULE | BS2DoorFlagEnum.EMERGENCY | BS2DoorFlagEnum.OPERATOR);
                byte doorFlag = Util.GetInput((byte)BS2DoorFlagEnum.OPERATOR);

                if ((doorFlag & ~validDoorFlag) != 0)
                {
                    Console.WriteLine("Invalid parameter");
                    Marshal.FreeHGlobal(doorIDObj);
                    return;
                }

                Console.WriteLine("Trying to lock doors.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_LockDoor(sdkContext, deviceID, doorFlag, doorIDObj, (UInt32)doorIDList.Count);

                Marshal.FreeHGlobal(doorIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        public void unlockDoor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Enter the ID of the door which you want to unlock: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] doorIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> doorIDList = new List<UInt32>();

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

            if (doorIDList.Count > 0)
            {
                IntPtr doorIDObj = Marshal.AllocHGlobal(4 * doorIDList.Count);
                IntPtr curDoorIDObj = doorIDObj;
                foreach (UInt32 item in doorIDList)
                {
                    Marshal.WriteInt32(curDoorIDObj, (Int32)item);
                    curDoorIDObj = (IntPtr)((long)curDoorIDObj + 4);
                }

                Console.WriteLine("Choose the flag for the doors to be unlocked: [0: None, 1: By Schedule, 2: By Emergency, 4: By Operator(default)]");
                Console.Write(">>>> ");
                byte validDoorFlag = (byte)(BS2DoorFlagEnum.NONE | BS2DoorFlagEnum.SCHEDULE | BS2DoorFlagEnum.EMERGENCY | BS2DoorFlagEnum.OPERATOR);
                byte doorFlag = Util.GetInput((byte)BS2DoorFlagEnum.OPERATOR);

                if ((doorFlag & ~validDoorFlag) != 0)
                {
                    Console.WriteLine("Invalid parameter");
                    Marshal.FreeHGlobal(doorIDObj);
                    return;
                }

                Console.WriteLine("Trying to unlock doors.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_UnlockDoor(sdkContext, deviceID, doorFlag, doorIDObj, (UInt32)doorIDList.Count);

                Marshal.FreeHGlobal(doorIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        void print(IntPtr sdkContext, BS2Door door)
        {
            Console.WriteLine(">>>> Door ID[{0, 10}] name[{1}]", door.doorID, Encoding.UTF8.GetString(door.name).TrimEnd('\0'));
            Console.WriteLine("     |--entryDeviceID[{0}]", door.entryDeviceID);
            Console.WriteLine("     |--exitDeviceID[{0}]", door.exitDeviceID);
            Console.WriteLine("     |--relay[ioDeviceID[{0}] port[{1}]]", door.relay.deviceID, door.relay.port);
            Console.WriteLine("     |--sensor[ioDeviceID[{0}] port[{1}] switchType[{2}]]", door.sensor.deviceID, door.sensor.port, (BS2SwitchTypeEnum)door.sensor.switchType);
            Console.WriteLine("     |--exitButton[ioDeviceID[{0}] port[{1}] switchType[{2}]]", door.button.deviceID, door.button.port, (BS2SwitchTypeEnum)door.button.switchType);
            Console.WriteLine("     |--autoLockTimeout[{0}ms]", door.autoLockTimeout);
            Console.WriteLine("     |--heldOpenTimeout[{0}ms]", door.heldOpenTimeout);
            Console.WriteLine("     |--unlockFlags[{0}]", (BS2DoorFlagEnum)door.unlockFlags);
            Console.WriteLine("     |--lockFlags[{0}]", (BS2DoorFlagEnum)door.lockFlags);
            Console.WriteLine("     |--unconditionalLock[{0}]", (BS2DoorAlarmFlagEnum)door.unconditionalLock);
            Console.WriteLine("     |--forcedOpenAlarm");            
            for (int idx = 0; idx < BS2Environment.BS2_MAX_FORCED_OPEN_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)door.forcedOpenAlarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", door.forcedOpenAlarm[idx].deviceID, (BS2ActionTypeEnum)door.forcedOpenAlarm[idx].type, Util.getActionMsg(door.forcedOpenAlarm[idx]));
            }

            Console.WriteLine("     |--heldOpenAlarm");
            for (int idx = 0; idx < BS2Environment.BS2_MAX_HELD_OPEN_ALARM_ACTION; ++idx)
            {
                BS2ActionTypeEnum actionType = (BS2ActionTypeEnum)door.heldOpenAlarm[idx].type;
                Console.WriteLine("     |  |--ID[{0}] Type[{1}] {2}", door.heldOpenAlarm[idx].deviceID, (BS2ActionTypeEnum)door.heldOpenAlarm[idx].type, Util.getActionMsg(door.heldOpenAlarm[idx]));
            }
            Console.WriteLine("     |--dualAuthScheduleID[{0}]", door.dualAuthScheduleID);
            Console.WriteLine("     |--dualAuthDevice[{0}]", door.dualAuthDevice);
            Console.WriteLine("     |--dualAuthApprovalType[{0}]", (BS2DualAuthApprovalEnum)door.dualAuthApprovalType);
            Console.WriteLine("     |--dualAuthTimeout[{0}ms]", door.dualAuthTimeout);
            if (door.numDualAuthApprovalGroups > 0)
            {
                Console.WriteLine("     |--dualAuthApprovalGroupID");
                for (int idx = 0; idx < door.numDualAuthApprovalGroups; ++idx)
                {
                    Console.WriteLine("     |  |--dual auth approval group id[{0}]", door.dualAuthApprovalGroupID[idx]);
                }
            }
        }

        void print(IntPtr sdkContext, BS2DoorStatus doorStatus)
        {
            Console.WriteLine(">>>> Door status ID[{0}]", doorStatus.id);
            Console.WriteLine("     |--opened[{0}]", Convert.ToBoolean(doorStatus.opened));
            Console.WriteLine("     |--unlocked[{0}]", Convert.ToBoolean(doorStatus.unlocked));
            Console.WriteLine("     |--heldOpened[{0}]", Convert.ToBoolean(doorStatus.heldOpened));
            Console.WriteLine("     |--unlockFlags[{0}]", (BS2DoorFlagEnum)doorStatus.unlockFlags);
            Console.WriteLine("     |--lockFlags[{0}]", (BS2DoorFlagEnum)doorStatus.lockFlags);
            Console.WriteLine("     |--alarmFlags[{0}]", (BS2DoorAlarmFlagEnum)doorStatus.alarmFlags);
            Console.WriteLine("     |--lastOpenTime[{0}]", Util.ConvertFromUnixTimestamp(doorStatus.lastOpenTime).ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
