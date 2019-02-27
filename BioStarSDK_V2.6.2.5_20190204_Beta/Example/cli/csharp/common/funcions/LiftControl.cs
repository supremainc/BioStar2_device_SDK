using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class LiftControl : FunctionModule
    {
        delegate int SetFloorStatusDelegate(IntPtr context, UInt32 deviceId, byte flag, UInt32 liftID, IntPtr floorIndexs, byte floorIndexCount);

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (!isMasterDevice)
            {
                Console.WriteLine("Not supported in slave device.");
                return functionList;
            }

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get Lift", getLift));                        
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set Lift", setLift));            
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove Lift", removeLift));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get LiftStatus", getLiftStatus));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Release Floor", releaseFloor));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("ActivateFloor", activateFloor));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("DeActivateFloor", deactivateFloor));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set LiftAlarm", setLiftAlarm));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get floor level", getFloorLevel));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove floor level", removeFloorLevel));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set floor level", setFloorLevel));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get access group", getAccessGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Remove access group", removeAccessGroup));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set access group", setAccessGroup));

            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Get slave device", getSlaveDevice));
            functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set slave device", setSlaveDevice));
            

            return functionList;
        }

        #region Lift
        public void getLift(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            IntPtr LiftObj = IntPtr.Zero;
            UInt32 numLift = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all Lift? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all Lift from device.");
                result = (BS2ErrorCode)API.BS2_GetAllLift(sdkContext, deviceID, out LiftObj, out numLift);
            }
            else
            {
                Console.WriteLine("Enter the ID of the lift which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] LiftIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> LiftIDList = new List<UInt32>();

                foreach (string LiftID in LiftIDs)
                {
                    if (LiftID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(LiftID, out item))
                        {
                            LiftIDList.Add(item);
                        }
                    }
                }

                if (LiftIDList.Count > 0)
                {
                    IntPtr LiftIDObj = Marshal.AllocHGlobal(4 * LiftIDList.Count);
                    IntPtr curLiftIDObj = LiftIDObj;
                    foreach (UInt32 item in LiftIDList)
                    {
                        Marshal.WriteInt32(curLiftIDObj, (Int32)item);
                        curLiftIDObj = (IntPtr)((long)curLiftIDObj + 4);
                    }

                    Console.WriteLine("Trying to get lift from device.");
                    result = (BS2ErrorCode)API.BS2_GetLift(sdkContext, deviceID, LiftIDObj, (UInt32)LiftIDList.Count, out LiftObj, out numLift);

                    Marshal.FreeHGlobal(LiftIDObj);
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
            else if (numLift > 0)
            {
                IntPtr curLiftObj = LiftObj;
                int structSize = Marshal.SizeOf(typeof(BS2Lift));

                for (int idx = 0; idx < numLift; ++idx)
                {
                    BS2Lift item = (BS2Lift)Marshal.PtrToStructure(curLiftObj, typeof(BS2Lift));
                    print(sdkContext, item);
                    curLiftObj = (IntPtr)((long)curLiftObj + structSize);
                }

                API.BS2_ReleaseObject(LiftObj);
            }
            else
            {
                Console.WriteLine(">>> There is no lift in the device.");
            }
        }

        public void getLiftStatus(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            IntPtr LiftObj = IntPtr.Zero;
            UInt32 numLift = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get status of all Lift ? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get status of all Lift from device.");
                result = (BS2ErrorCode)API.BS2_GetAllLiftStatus(sdkContext, deviceID, out LiftObj, out numLift);
            }
            else
            {
                Console.WriteLine("Enter the ID of the lift which you want to get its status: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] LiftIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> LiftIDList = new List<UInt32>();

                foreach (string LiftID in LiftIDs)
                {
                    if (LiftID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(LiftID, out item))
                        {
                            LiftIDList.Add(item);
                        }
                    }
                }

                if (LiftIDList.Count > 0)
                {
                    IntPtr LiftIDObj = Marshal.AllocHGlobal(4 * LiftIDList.Count);
                    IntPtr curLiftIDObj = LiftIDObj;
                    foreach (UInt32 item in LiftIDList)
                    {
                        Marshal.WriteInt32(curLiftIDObj, (Int32)item);
                        curLiftIDObj = (IntPtr)((long)curLiftIDObj + 4);
                    }

                    Console.WriteLine("Trying to get lift from device.");
                    result = (BS2ErrorCode)API.BS2_GetLiftStatus(sdkContext, deviceID, LiftIDObj, (UInt32)LiftIDList.Count, out LiftObj, out numLift);

                    Marshal.FreeHGlobal(LiftIDObj);
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
            else if (numLift > 0)
            {
                IntPtr curLiftObj = LiftObj;
                int structSize = Marshal.SizeOf(typeof(BS2LiftStatus));

                for (int idx = 0; idx < numLift; ++idx)
                {
                    BS2LiftStatus item = (BS2LiftStatus)Marshal.PtrToStructure(curLiftObj, typeof(BS2LiftStatus));
                    print(sdkContext, item);
                    curLiftObj = (IntPtr)((long)curLiftObj + structSize);
                }

                API.BS2_ReleaseObject(LiftObj);
            }
            else
            {
                Console.WriteLine(">>> There is no lift in the device.");
            }
        }        

        public void setLift(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("Do you want to Auto lift set? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                List<BS2Lift> liftList = new List<BS2Lift>();

                BS2Lift lift = Util.AllocateStructure<BS2Lift>();
                Array.Clear(lift.deviceID, 0, BS2Environment.BS2_MAX_DEVICES_ON_LIFT);
                Array.Clear(lift.floor, 0, BS2Environment.BS2_MAX_FLOORS_ON_LIFT);
                Array.Clear(lift.dualAuthApprovalGroupID, 0, BS2Environment.BS2_MAX_DUAL_AUTH_APPROVAL_GROUP_ON_LIFT);
                //Array.Clear(lift.alarm, 0, BS2Environment.BS2_MAX_ALARMS_ON_LIFT);

                //
                lift.liftID = 1004;

                string liftName = "천사";
                byte[] str = Encoding.UTF8.GetBytes(liftName);
                Array.Clear(lift.name, 0, BS2Environment.BS2_MAX_LIFT_NAME_LEN);
                Array.Copy(str, 0, lift.name, 0, str.Length);

                lift.deviceID[0] = 541531082; //connect device
                lift.activateTimeout = 5;
                lift.dualAuthTimeout = 10;
                lift.numFloors = 12;
                lift.numDualAuthApprovalGroups = 0;
                lift.dualAuthApprovalType = 1;
                lift.tamperOn = 0;
                lift.dualAuthRequired[0] = 0;
                lift.dualAuthRequired[1] = 0;
                lift.dualAuthRequired[2] = 0;
                lift.dualAuthRequired[3] = 0;
                lift.dualAuthScheduleID = 1;                
                lift.alarmFlags = 0;

                for (int i = 0; i < lift.numFloors-1; ++i)
                {
                    lift.floor[i].deviceID = 100000143;
                    lift.floor[i].port = (byte)i;
                }

                lift.alarm[0].sensor.deviceID = 541531082;
                lift.alarm[0].sensor.port = 0;
                lift.alarm[0].sensor.switchType = 1;
                lift.alarm[0].sensor.duration = 100;
                lift.alarm[0].sensor.scheduleID = 1;

                lift.alarm[0].action.deviceID = 100000143;
                lift.alarm[0].action.type = 15; //lift
                lift.alarm[0].action.stopFlag = 0;
                lift.alarm[0].action.delay = 100;
                BS2LiftAction liftaction = Util.AllocateStructure<BS2LiftAction>();
                liftaction.liftID = 1004;
                liftaction.type = 0; //Activate
                byte[] liftActionArray = Util.ConvertTo<BS2LiftAction>(ref liftaction);
                Array.Clear(lift.alarm[0].action.actionUnion, 0, lift.alarm[1].action.actionUnion.Length);
                Array.Copy(liftActionArray, lift.alarm[0].action.actionUnion, liftActionArray.Length);

                lift.alarm[1].sensor.deviceID = 541531082;
                lift.alarm[1].sensor.port = 1;
                lift.alarm[1].sensor.switchType = 1;
                lift.alarm[1].sensor.duration = 100;
                lift.alarm[1].sensor.scheduleID = 1;

                lift.alarm[1].action.deviceID = 100000143;
                lift.alarm[1].action.type = 6; //relay
                lift.alarm[1].action.stopFlag = 0;
                lift.alarm[1].action.delay = 100;
                BS2RelayAction relay = Util.AllocateStructure<BS2RelayAction>();
                relay.relayIndex = 11;
                relay.signal.signalID = 0;   
                relay.signal.count = 100;               
                relay.signal.onDuration = 11;                
                relay.signal.offDuration = 45;
                relay.signal.delay = 52;
                byte[] inputActionArray = Util.ConvertTo<BS2RelayAction>(ref relay);
                Array.Clear(lift.alarm[1].action.actionUnion, 0, lift.alarm[1].action.actionUnion.Length);
                Array.Copy(inputActionArray, lift.alarm[1].action.actionUnion, inputActionArray.Length);

                //tamper
                lift.tamper.sensor.deviceID = 100000143;
                lift.tamper.sensor.port = 0;
                lift.tamper.sensor.switchType = 0;
                lift.tamper.sensor.duration = 100;
                lift.tamper.sensor.scheduleID = 1;

                lift.tamper.action.deviceID = 100000143;
                lift.tamper.action.type = 15; //lift
                lift.tamper.action.stopFlag = 0;
                lift.tamper.action.delay = 100;
                byte[] liftActionArray2 = Util.ConvertTo<BS2LiftAction>(ref liftaction);
                Array.Clear(lift.tamper.action.actionUnion, 0, lift.tamper.action.actionUnion.Length);
                Array.Copy(liftActionArray2, lift.tamper.action.actionUnion, liftActionArray.Length);

                liftList.Add(lift);

                int structSize = Marshal.SizeOf(typeof(BS2Lift));
                IntPtr liftListObj = Marshal.AllocHGlobal(structSize * liftList.Count);
                IntPtr curLiftListObj = liftListObj;
                foreach (BS2Lift item in liftList)
                {
                    Marshal.StructureToPtr(item, curLiftListObj, false);
                    curLiftListObj = (IntPtr)((long)curLiftListObj + structSize);
                }

                Console.WriteLine("Trying to set lift to device.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetLift(sdkContext, deviceID, liftListObj, (UInt32)liftList.Count);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(liftListObj);
            }
            else 
            {
                Console.WriteLine("How many lift do you want to set? [1(default)-128]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                int amount = Util.GetInput(1);
                List<BS2Lift> liftList = new List<BS2Lift>();            

                for (int idx = 0; idx < amount; ++idx)
                {
                    BS2Lift lift = Util.AllocateStructure<BS2Lift>();
                    Array.Clear(lift.deviceID, 0, BS2Environment.BS2_MAX_DEVICES_ON_LIFT);
                    Array.Clear(lift.floor, 0, BS2Environment.BS2_MAX_FLOORS_ON_LIFT);
                    Array.Clear(lift.dualAuthApprovalGroupID, 0, BS2Environment.BS2_MAX_DUAL_AUTH_APPROVAL_GROUP_ON_LIFT);
                    //Array.Clear(lift.alarm, 0, BS2Environment.BS2_MAX_ALARMS_ON_LIFT);

                    Console.WriteLine("Enter a value for lift[{0}]", idx);
                    Console.WriteLine("  Enter the ID for the lift which you want to set");
                    Console.Write("  >>>> ");
                    lift.liftID = (UInt32)Util.GetInput();
                    Console.WriteLine("  Enter the name for the lift which you want to set");
                    Console.Write("  >>>> ");
                    string liftName = Console.ReadLine();
                    if (liftName.Length == 0)
                    {
                        Console.WriteLine("  [Warning] lift name will be displayed as empty.");
                    }
                    else if (liftName.Length > BS2Environment.BS2_MAX_LIFT_NAME_LEN)
                    {
                        Console.WriteLine("  Name of lift should less than {0} words.", BS2Environment.BS2_MAX_LIFT_NAME_LEN);
                        return;
                    }
                    else
                    {
                        byte[] liftArray = Encoding.UTF8.GetBytes(liftName);
                        Array.Clear(lift.name, 0, BS2Environment.BS2_MAX_LIFT_NAME_LEN);
                        Array.Copy(liftArray, lift.name, liftArray.Length);
                    }

                    Console.WriteLine("How many device do you want to set? [1(default)-4]");
                    Console.Write(">>>> ");
                    char[] delimiterCharsDevice = { ' ', ',', '.', ':', '\t' };
                    int amountdevice = Util.GetInput(1);

                    for (idx = 0; idx < amountdevice; ++idx)
                    {
                        Console.WriteLine("  Enter the deviceID[{0}]", idx);
                        Console.Write("  >>>> ");
                        lift.deviceID[idx] = (UInt32)Util.GetInput();
                    }

                    Console.WriteLine("  Enter the activateTimeout in seconds: [10(default)]");
                    Console.Write("  >>>> ");
                    lift.activateTimeout = Util.GetInput(10);

                    Console.WriteLine("  Enter the HeldOpen timeout in seconds: [15(default)]");
                    Console.Write("  >>>> ");
                    lift.dualAuthTimeout = Util.GetInput(15);

                    Console.WriteLine("  How many floors do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_FLOORS_ON_LIFT);
                    Console.Write("  >>>> ");
                    lift.numFloors = Util.GetInput(0);

                    for (byte loop = 0; loop < lift.numFloors; ++loop)
                    {
                        Console.WriteLine("    Enter a value for floor[{0}]", loop);

                        Console.WriteLine("  Enter the device id of the floor");
                        Console.Write("  >>>> ");
                        lift.floor[loop].deviceID = (UInt32)Util.GetInput();

                        Console.WriteLine("  Enter the the port of the floor.[0(default)]");
                        Console.Write("  >>>> ");
                        lift.floor[loop].port = Util.GetInput(0);

                        Console.WriteLine("  Does this floor has a activated? [Y/n]");
                        Console.Write("  >>>> ");
                        if (Util.IsYes())
                        {                        
                            lift.floor[loop].status.activated = 1;

                            Console.WriteLine("  Enter the activatedFlags of the floor.[0(default), 1(Schedule), 2(Emergency), 4(Operator)]");
                            Console.Write("  >>>> ");
                            lift.floor[loop].status.activateFlags = Util.GetInput(0);

                            Console.WriteLine("  Enter the deactivatedFlags of the floor.[0(default), 1(Schedule), 2(Emergency), 4(Operator)]");
                            Console.Write("  >>>> ");
                            lift.floor[loop].status.deactivateFlags = Util.GetInput(0);
                        }
                    }

                    Console.WriteLine("  How many dualAuthApprovalGroups do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_DUAL_AUTH_APPROVAL_GROUP_ON_LIFT);
                    Console.Write("  >>>> ");
                    lift.numDualAuthApprovalGroups = Util.GetInput(0);

                    for (byte loop = 0; loop < lift.numDualAuthApprovalGroups; ++loop)
                    {
                        Console.WriteLine("  Enter the DualAuthApprovalGroupID of the floor");
                        Console.Write("  >>>> ");
                        lift.dualAuthApprovalGroupID[loop] = (UInt32)Util.GetInput();
                    
                    }

                    Console.WriteLine("  Enter the dualAuthApprovalType: [0(default), 1(last)]");
                    Console.Write("  >>>> ");
                    lift.dualAuthApprovalType = Util.GetInput(0);

                    /* device tamperOn Status
                    Console.WriteLine("  Does this lift has a tamperOn? [Y/n]");
                    Console.Write("  >>>> ");
                    if (Util.IsYes())
                    {
                        lift.tamperOn = 1; 
                    }
                    */

                    for (idx = 0; idx < amountdevice; ++idx)
                    {
                        Console.WriteLine("  Does this devices[{0}] has a dualAuthRequired? [Y/n]", idx);
                        Console.Write("  >>>> ");
                        if (Util.IsYes())
                        {
                            lift.dualAuthRequired[idx] = 1; 
                        }
                    }

                    Console.WriteLine("  Enter the dualAuthScheduleID: [0(default), 1(Always], 2, 3, .....]");
                    Console.Write("  >>>> ");
                    lift.dualAuthScheduleID = (UInt32)Util.GetInput();

                    Console.WriteLine("  How many alarms for this lift do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_ALARMS_ON_LIFT);
                    Console.Write("  >>>> ");
                    int alarmCount = Util.GetInput(0);
                    BS2RelayAction relay = Util.AllocateStructure<BS2RelayAction>();

                    for (byte loop = 0; loop < alarmCount; ++loop)
                    {
                        Console.WriteLine("  Enter the device ID of the sensor which you want to run this lift");
                        Console.Write("  >>>> ");
                        lift.alarm[loop].sensor.deviceID = (UInt32)Util.GetInput();

                        Console.WriteLine("  Enter the the port of the sensor on this alarm[0(default)]");
                        Console.Write("  >>>> ");
                        lift.alarm[loop].sensor.port = Util.GetInput(0);

                        Console.WriteLine("  Enter the switch type of the alarm sensor on this lift: [0: normally open, 1: normally closed].");
                        Console.Write("  >>>> ");
                        lift.alarm[loop].sensor.switchType = Util.GetInput(0);

                        Console.WriteLine("  Enter duration of the alarm sensor on this lift.[100(default)]");
                        Console.Write("  >>>> ");
                        lift.alarm[loop].sensor.duration = Util.GetInput((UInt16)100);

                        Console.WriteLine("  Enter the ScheduleID of the alarm sensor on this lift: [0(default), 1(Always], 2, 3, .....]");
                        Console.Write("  >>>> ");
                        lift.alarm[loop].sensor.scheduleID = (UInt32)Util.GetInput(0);

                        // We are assuming relay control. Of course you can do the other action.
                        lift.alarm[loop].action.type = (byte)BS2ActionTypeEnum.RELAY;

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
                        Array.Clear(lift.alarm[loop].action.actionUnion, 0, lift.alarm[loop].action.actionUnion.Length);
                        Array.Copy(inputActionArray, lift.alarm[loop].action.actionUnion, inputActionArray.Length);
                    }

                    //tamper
                    Console.WriteLine("  Enter the device ID of the sensor which you want to run this lift");
                    Console.Write("  >>>> ");
                    lift.tamper.sensor.deviceID = (UInt32)Util.GetInput();

                    Console.WriteLine("  Enter the the port of the sensor on this alarm[0(default)]");
                    Console.Write("  >>>> ");
                    lift.tamper.sensor.port = Util.GetInput(0);

                    Console.WriteLine("  Enter the switch type of the alarm sensor on this lift: [0: normally open, 1: normally closed].");
                    Console.Write("  >>>> ");
                    lift.tamper.sensor.switchType = Util.GetInput(0);

                    Console.WriteLine("  Enter duration of the alarm sensor on this lift.[100(default)]");
                    Console.Write("  >>>> ");
                    lift.tamper.sensor.duration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  Enter the ScheduleID of the alarm sensor on this lift: [0(default), 1(Always], 2, 3, .....]");
                    Console.Write("  >>>> ");
                    lift.tamper.sensor.scheduleID = (UInt32)Util.GetInput(0);

                    // We are assuming relay control. Of course you can do the other action.
                    lift.tamper.action.type = (byte)BS2ActionTypeEnum.RELAY;

                    Console.WriteLine("  Enter the relay port on this device.[0(default)]");
                    Console.Write("  >>>> ");
                    relay.relayIndex = Util.GetInput(0);

                    relay.signal.signalID = 1;
                    Console.WriteLine("  How many play the relay action on this device?[1(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.count = Util.GetInput((UInt16)100);

                    Console.WriteLine("  Enter the active duration.[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.onDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  Enter the deactive duration.[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.offDuration = Util.GetInput((UInt16)100);

                    Console.WriteLine("  How many waiting for to a next action?[100(default)]");
                    Console.Write("  >>>> ");
                    relay.signal.delay = Util.GetInput((UInt16)100);

                    byte[] inputActionArray2 = Util.ConvertTo<BS2RelayAction>(ref relay);
                    Array.Clear(lift.tamper.action.actionUnion, 0, lift.tamper.action.actionUnion.Length);
                    Array.Copy(inputActionArray2, lift.tamper.action.actionUnion, inputActionArray2.Length);
                    //tamper end

                    /* lift alarmFlags Status
                    Console.WriteLine("  Enter the alarmFlags of the lift.[0(default), 1(first), 2(second), 4(tamper)]");
                    Console.Write("  >>>> ");
                    lift.alarmFlags = Util.GetInput(0);
                    */
                
                    liftList.Add(lift);
                }

                int structSize = Marshal.SizeOf(typeof(BS2Lift));
                IntPtr liftListObj = Marshal.AllocHGlobal(structSize * liftList.Count);
                IntPtr curLiftListObj = liftListObj;
                foreach (BS2Lift item in liftList)
                {
                    Marshal.StructureToPtr(item, curLiftListObj, false);
                    curLiftListObj = (IntPtr)((long)curLiftListObj + structSize);
                }

                Console.WriteLine("Trying to set lift to device.");
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetLift(sdkContext, deviceID, liftListObj, (UInt32)liftList.Count);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(liftListObj);
            }
        }

        public void setLiftAlarm(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
           
            Console.WriteLine("  Enter the alarmFlag.[0(None), 1(First), 2(Second), 4(Tamper)]");
            Console.Write("  >>>> ");
            byte alarmFlag = Util.GetInput(0);            

            Console.WriteLine("Enter the ID of the lift which you want to set: [ID_1,ID_2 ...]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] liftIDs = Console.ReadLine().Split(delimiterChars);
            List<UInt32> liftIDList = new List<UInt32>();

            foreach (string liftID in liftIDs)
            {
                if (liftID.Length > 0)
                {
                    UInt32 item;
                    if (UInt32.TryParse(liftID, out item))
                    {
                        liftIDList.Add(item);
                    }
                }
            }

            if (liftIDList.Count > 0)
            {
                IntPtr liftIDObj = Marshal.AllocHGlobal(4 * liftIDList.Count);
                IntPtr curLiftIDObj = liftIDObj;
                foreach (UInt32 item in liftIDList)
                {
                    Marshal.WriteInt32(curLiftIDObj, (Int32)item);
                    curLiftIDObj = (IntPtr)((long)curLiftIDObj + 4);
                }

                BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetLiftAlarm(sdkContext, deviceID, alarmFlag, liftIDObj, (UInt32)liftIDList.Count);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(liftIDObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }

        public void removeLift(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all lift? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all lift from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllLift(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the lift which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] liftIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> liftIDList = new List<UInt32>();

                foreach (string liftID in liftIDs)
                {
                    if (liftID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(liftID, out item))
                        {
                            liftIDList.Add(item);
                        }
                    }
                }

                if (liftIDList.Count > 0)
                {
                    IntPtr liftIDObj = Marshal.AllocHGlobal(4 * liftIDList.Count);
                    IntPtr curLiftIDObj = liftIDObj;
                    foreach (UInt32 item in liftIDList)
                    {
                        Marshal.WriteInt32(curLiftIDObj, (Int32)item);
                        curLiftIDObj = (IntPtr)((long)curLiftIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove lift from device.");
                    result = (BS2ErrorCode)API.BS2_RemoveLift(sdkContext, deviceID, liftIDObj, (UInt32)liftIDList.Count);

                    Marshal.FreeHGlobal(liftIDObj);
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

        void releaseFloor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setFloorStatus(sdkContext, deviceID, "releaseFloor", API.BS2_ReleaseFloor);
        }

        void activateFloor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setFloorStatus(sdkContext, deviceID, "activateFloor", API.BS2_ActivateFloor);
        }

        void deactivateFloor(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            setFloorStatus(sdkContext, deviceID, "deactivateFloor", API.BS2_DeActivateFloor);
        }

        void setFloorStatus(IntPtr sdkContext, UInt32 deviceID, string floorType, SetFloorStatusDelegate setFloorStatusDelegate)
        {
            Console.WriteLine("  Enter the floorFlag.[0(None), 2(Emergency), 4(Operator)]");
            Console.Write("  >>>> ");
            byte floorFlag = Util.GetInput(0);
            
            Console.WriteLine("  Enter the liftID");
            Console.Write("  >>>> ");
            UInt32 liftID = (UInt32)Util.GetInput();

            Console.WriteLine("Enter the floorIndex of the lift which you want to set: [0,1,2,...255]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] floorIndexs = Console.ReadLine().Split(delimiterChars);
            List<UInt16> floorIndexList = new List<UInt16>();

            foreach (string floorIndex in floorIndexs)
            {
                if (floorIndex.Length > 0)
                {
                    UInt16 item;
                    if (UInt16.TryParse(floorIndex, out item))
                    {
                        floorIndexList.Add(item);
                    }
                }
            }

            if (floorIndexList.Count > 0)
            {
                IntPtr floorIndexObj = Marshal.AllocHGlobal(2 * floorIndexList.Count);
                IntPtr curfloorIndexObj = floorIndexObj;
                foreach (UInt32 item in floorIndexList)
                {
                    Marshal.WriteInt16(curfloorIndexObj, (Int16)item);
                    curfloorIndexObj = (IntPtr)((long)curfloorIndexObj + 2);
                }

                Console.WriteLine("Trying to set the status of the {0} floor from device.", floorType);
                BS2ErrorCode result = (BS2ErrorCode)setFloorStatusDelegate(sdkContext, deviceID, floorFlag, liftID, floorIndexObj, (byte)floorIndexList.Count);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Got error({0}).", result);
                }

                Marshal.FreeHGlobal(floorIndexObj);
            }
            else
            {
                Console.WriteLine("Invalid parameter");
            }
        }
        #endregion

        #region Floor Level
        public void getFloorLevel(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            IntPtr floorLevelObj = IntPtr.Zero;
            UInt32 numFloorLevel = 0;
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to get all floor levels? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to get all floor levels from device.");
                result = (BS2ErrorCode)API.BS2_GetAllFloorLevel(sdkContext, deviceID, out floorLevelObj, out numFloorLevel);
            }
            else
            {
                Console.WriteLine("Enter the ID of the floor level which you want to get: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] floorLevelIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> floorLevelIDList = new List<UInt32>();

                foreach (string floorLevelID in floorLevelIDs)
                {
                    if (floorLevelID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(floorLevelID, out item))
                        {
                            floorLevelIDList.Add(item);
                        }
                    }
                }

                if (floorLevelIDList.Count > 0)
                {
                    IntPtr floorLevelIDObj = Marshal.AllocHGlobal(4 * floorLevelIDList.Count);
                    IntPtr curFloorLevelIDObj = floorLevelIDObj;
                    foreach (UInt32 item in floorLevelIDList)
                    {
                        Marshal.WriteInt32(curFloorLevelIDObj, (Int32)item);
                        curFloorLevelIDObj = (IntPtr)((long)curFloorLevelIDObj + 4);
                    }

                    Console.WriteLine("Trying to get floor levels from device.");
                    result = (BS2ErrorCode)API.BS2_GetFloorLevel(sdkContext, deviceID, floorLevelIDObj, (UInt32)floorLevelIDList.Count, out floorLevelObj, out numFloorLevel);

                    Marshal.FreeHGlobal(floorLevelIDObj);
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
            else if (numFloorLevel > 0)
            {
                IntPtr curFloorLevelObj = floorLevelObj;
                int structSize = Marshal.SizeOf(typeof(BS2FloorLevel));

                for (int idx = 0; idx < numFloorLevel; ++idx)
                {
                    BS2FloorLevel item = (BS2FloorLevel)Marshal.PtrToStructure(curFloorLevelObj, typeof(BS2FloorLevel));
                    print(sdkContext, item);
                    curFloorLevelObj = (IntPtr)((long)curFloorLevelObj + structSize);
                }

                API.BS2_ReleaseObject(floorLevelObj);
            }
            else
            {
                Console.WriteLine(">>> There is no access level in the device.");
            }
        }

        public void removeFloorLevel(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            Console.WriteLine("Do you want to remove all floor levels? [Y/n]");
            Console.Write(">>>> ");
            if (Util.IsYes())
            {
                Console.WriteLine("Trying to remove all floor levels from device.");
                result = (BS2ErrorCode)API.BS2_RemoveAllFloorLevel(sdkContext, deviceID);
            }
            else
            {
                Console.WriteLine("Enter the ID of the floor level which you want to remove: [ID_1,ID_2 ...]");
                Console.Write(">>>> ");
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] floorLevelIDs = Console.ReadLine().Split(delimiterChars);
                List<UInt32> floorLevelIDList = new List<UInt32>();

                foreach (string floorLevelID in floorLevelIDs)
                {
                    if (floorLevelID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(floorLevelID, out item))
                        {
                            floorLevelIDList.Add(item);
                        }
                    }
                }

                if (floorLevelIDList.Count > 0)
                {
                    IntPtr floorLevelIDObj = Marshal.AllocHGlobal(4 * floorLevelIDList.Count);
                    IntPtr curFloorLevelIDObj = floorLevelIDObj;
                    foreach (UInt32 item in floorLevelIDList)
                    {
                        Marshal.WriteInt32(curFloorLevelIDObj, (Int32)item);
                        curFloorLevelIDObj = (IntPtr)((long)curFloorLevelIDObj + 4);
                    }

                    Console.WriteLine("Trying to remove access levels from device.");
                    result = (BS2ErrorCode)API.BS2_RemoveFloorLevel(sdkContext, deviceID, floorLevelIDObj, (UInt32)floorLevelIDList.Count);

                    Marshal.FreeHGlobal(floorLevelIDObj);
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

        public void setFloorLevel(IntPtr sdkContext, uint deviceID, bool isMasterDevice)
        {
            Console.WriteLine("How many floor levels do you want to set? [1(default)-128]");
            Console.Write(">>>> ");
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            int amount = Util.GetInput(1);
            List<BS2FloorLevel> floorLevelList = new List<BS2FloorLevel>();

            for (int idx = 0; idx < amount; ++idx)
            {
                BS2FloorLevel floorLevel = Util.AllocateStructure<BS2FloorLevel>();

                Console.WriteLine("Enter a value for floor level[{0}]", idx);
                Console.WriteLine("  Enter the ID for the floor level which you want to set [id >= 32768]");
                Console.Write("  >>>> "); 
                floorLevel.id = (UInt32)Util.GetInput();
                Console.WriteLine("  Enter the name for the access level which you want to set ");
                Console.Write("  >>>> ");
                string floorName = Console.ReadLine();
                if (floorName.Length == 0)
                {
                    Console.WriteLine("  [Warning] floor level name will be displayed as empty.");
                }
                else if (floorName.Length > BS2Environment.BS2_MAX_FLOOR_LEVEL_NAME_LEN)
                {
                    Console.WriteLine("  Name of floor Level should less than {0} words.", BS2Environment.BS2_MAX_FLOOR_LEVEL_NAME_LEN);
                    return;
                }
                else
                {
                    byte[] floorLevelArray = Encoding.UTF8.GetBytes(floorName);
                    Array.Clear(floorLevel.name, 0, BS2Environment.BS2_MAX_FLOOR_LEVEL_NAME_LEN);
                    Array.Copy(floorLevelArray, floorLevel.name, floorLevelArray.Length);
                }

                Console.WriteLine("  How many floor schedules do you want to set? [0(default)-{0}]", BS2Environment.BS2_MAX_FLOOR_LEVEL_ITEMS);
                Console.Write("  >>>> ");
                floorLevel.numFloorSchedules = Util.GetInput(0);

                for (byte loop = 0; loop < floorLevel.numFloorSchedules; ++loop)
                {
                    Console.WriteLine("    Enter a value for floor schedule[{0}]", loop);
                    Console.WriteLine("    Enter the lift ID which you want to set");
                    Console.Write("    >>>> ");
                    floorLevel.floorSchedules[loop].liftID = (UInt32)Util.GetInput();

                    Console.WriteLine("    Enter the floorIndex which you want to set");
                    Console.Write("    >>>> ");
                    floorLevel.floorSchedules[loop].floorIndex = (UInt16)Util.GetInput();

                    Console.WriteLine("    Enter the schedule ID which you want to set");
                    Console.Write("    >>>> ");
                    floorLevel.floorSchedules[loop].scheduleID = (UInt32)Util.GetInput();
                }

                floorLevelList.Add(floorLevel);
            }

            int structSize = Marshal.SizeOf(typeof(BS2FloorLevel));
            IntPtr floorLevelListObj = Marshal.AllocHGlobal(structSize * floorLevelList.Count);
            IntPtr curFloorLevelListObj = floorLevelListObj;
            foreach (BS2FloorLevel item in floorLevelList)
            {
                Marshal.StructureToPtr(item, curFloorLevelListObj, false);
                curFloorLevelListObj = (IntPtr)((long)curFloorLevelListObj + structSize);
            }

            Console.WriteLine("Trying to set floor levels to device.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetFloorLevel(sdkContext, deviceID, floorLevelListObj, (UInt32)floorLevelList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            Marshal.FreeHGlobal(floorLevelListObj);
        }
        #endregion

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
                Console.WriteLine("Trying to get all access gruops from device.");
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

                    Console.WriteLine("Trying to get access gruops from device.");
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
                Console.WriteLine("Trying to remove all access gruops from device.");
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

                    Console.WriteLine("Trying to remove access gruops from device.");
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

                Console.WriteLine("  Enter the ID of the floor level which you want to set: [ID_1,ID_2 ...]");
                Console.Write("  >>>> ");
                string[] accessGroupIDs = Console.ReadLine().Split(delimiterChars);      
          
                accessGroup.numOflevelUnion.numFloorLevels = 0;
                foreach (string accessGroupID in accessGroupIDs)
                {
                    if (accessGroupID.Length > 0)
                    {
                        UInt32 item;
                        if (UInt32.TryParse(accessGroupID, out item))
                        {
                            accessGroup.levelUnion.floorLevels[accessGroup.numOflevelUnion.numFloorLevels++] = item;
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
                List<BS2Rs485SlaveDevice> slaveDeviceList = new List<BS2Rs485SlaveDevice>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDevice));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
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
                List<BS2Rs485SlaveDevice> slaveDeviceList = new List<BS2Rs485SlaveDevice>();
                IntPtr curSlaveDeviceObj = slaveDeviceObj;
                int structSize = Marshal.SizeOf(typeof(BS2Rs485SlaveDevice));

                for (int idx = 0; idx < slaveDeviceCount; ++idx)
                {
                    BS2Rs485SlaveDevice item = (BS2Rs485SlaveDevice)Marshal.PtrToStructure(curSlaveDeviceObj, typeof(BS2Rs485SlaveDevice));
                    slaveDeviceList.Add(item);
                    curSlaveDeviceObj = (IntPtr)((long)curSlaveDeviceObj + structSize);
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

        void slaveControl(IntPtr sdkContext, List<BS2Rs485SlaveDevice> slaveDeviceList)
        {
            //TODO implement this section.
        }

        void print(IntPtr sdkContext, BS2FloorLevel floorLevel)
        {
            Console.WriteLine(">>>> FloorLevel id[{0}] name[{1}]", floorLevel.id, Encoding.UTF8.GetString(floorLevel.name).TrimEnd('\0'));
            Console.WriteLine("     |--floorSchedules");
            Console.WriteLine("     |--numFloorSchedules[{0}]", floorLevel.numFloorSchedules);
            for (byte loop = 0; loop < floorLevel.numFloorSchedules; ++loop)
            {
                Console.WriteLine("     |  |--liftID[{0}] floorIndex[{1}] scheduleID[{2}]", floorLevel.floorSchedules[loop].liftID, floorLevel.floorSchedules[loop].floorIndex, floorLevel.floorSchedules[loop].scheduleID);
            }
        }

        void print(IntPtr sdkContext, BS2AccessGroup accessGroup)
        {
            Console.WriteLine(">>>> AccessGroup id[{0}] name[{1}]", accessGroup.id, Encoding.UTF8.GetString(accessGroup.name).TrimEnd('\0'));
            Console.WriteLine("     |--FloorLevels");
            Console.WriteLine("     |--numFloorLevels[{0}]", accessGroup.numOflevelUnion.numFloorLevels);
            for (byte loop = 0; loop < accessGroup.numOflevelUnion.numFloorLevels; ++loop)
            {
                Console.WriteLine("     |  |--floorLevelID[{0}]", accessGroup.levelUnion.floorLevels[loop]);
            }
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

        void print(IntPtr sdkContext, BS2Lift lift)
        {            
            Console.WriteLine(">>>> Lift ID[{0, 10}] name[{1}]", lift.liftID, Encoding.UTF8.GetString(lift.name).TrimEnd('\0'));
            Console.WriteLine("     |--deviceID[{0}, {1}, {2}, {3}]", lift.deviceID[0], lift.deviceID[1], lift.deviceID[2], lift.deviceID[3]);
            Console.WriteLine("     |--activateTimeout[{0}]", lift.activateTimeout);
            Console.WriteLine("     |--dualAuthTimeout[{0}]", lift.dualAuthTimeout);
            Console.WriteLine("     |--numFloors[{0}]", lift.numFloors);
            Console.WriteLine("     |--numDualAuthApprovalGroups[{0}]", lift.numDualAuthApprovalGroups);
            Console.WriteLine("     |--dualAuthApprovalType[{0}]", lift.dualAuthApprovalType);
            Console.WriteLine("     |--tamperOn[{0}]", Convert.ToBoolean(lift.tamperOn));
            Console.WriteLine("     |--dualAuthRequired[{0}, {1}, {2}, {3}]", Convert.ToBoolean(lift.dualAuthRequired[0]), Convert.ToBoolean(lift.dualAuthRequired[1]), Convert.ToBoolean(lift.dualAuthRequired[2]), Convert.ToBoolean(lift.dualAuthRequired[3]));
            Console.WriteLine("     |--dualAuthScheduleID[{0}]", lift.dualAuthScheduleID);

            for (int i = 0; i < lift.numDualAuthApprovalGroups; ++i )
            {
                Console.WriteLine("     |--dualAuthApprovalGroupID[{0}]", lift.dualAuthApprovalGroupID[i]);
            }

            for (int i = 0; i < lift.numFloors; ++i)
            {
                Console.WriteLine("     |++floor[{0}]", i);
                Console.WriteLine("         |--deviceID[{0}]", lift.floor[i].deviceID);
                Console.WriteLine("         |--port[{0}]", lift.floor[i].port);
                Console.WriteLine("         |--activated[{0}]", Convert.ToBoolean(lift.floor[i].status.activated));
                Console.WriteLine("         |--activateFlags[{0}]", lift.floor[i].status.activateFlags);
                Console.WriteLine("         |--deactivateFlags[{0}]", lift.floor[i].status.deactivateFlags);
            }

            for (int i = 0; i < 2; ++i)
            {
                Console.WriteLine("     |++alarm[{0}]", i);

                Console.WriteLine("         |--deviceID[{0}]", lift.alarm[i].sensor.deviceID);
                Console.WriteLine("         |--port[{0}]", lift.alarm[i].sensor.port);
                Console.WriteLine("         |--switchType[{0}]", lift.alarm[i].sensor.switchType);
                Console.WriteLine("         |--duration[{0}]", lift.alarm[i].sensor.duration);
                Console.WriteLine("         |--scheduleID[{0}]", lift.alarm[i].sensor.scheduleID);

                Console.WriteLine("         |++Action]", i);
                Console.WriteLine("             |--deviceID[{0}]", lift.alarm[i].action.deviceID);
                Console.WriteLine("             |--port[{0}]", lift.alarm[i].action.type);
                Console.WriteLine("             |--switchType[{0}]", lift.alarm[i].action.stopFlag);
                Console.WriteLine("             |--duration[{0}]", lift.alarm[i].action.delay);

                Console.WriteLine("             |--relay[{0}]", Util.getActionMsg(lift.alarm[i].action));               

            }

            for (int i = 0; i < 1; ++i)
            {
                Console.WriteLine("     |++tamper[{0}]", i);

                Console.WriteLine("         |--deviceID[{0}]", lift.tamper.sensor.deviceID);
                Console.WriteLine("         |--port[{0}]", lift.tamper.sensor.port);
                Console.WriteLine("         |--switchType[{0}]", lift.tamper.sensor.switchType);
                Console.WriteLine("         |--duration[{0}]", lift.tamper.sensor.duration);
                Console.WriteLine("         |--scheduleID[{0}]", lift.tamper.sensor.scheduleID);

                Console.WriteLine("         |++Action]", i);
                Console.WriteLine("             |--deviceID[{0}]", lift.tamper.action.deviceID);
                Console.WriteLine("             |--port[{0}]", lift.tamper.action.type);
                Console.WriteLine("             |--switchType[{0}]", lift.tamper.action.stopFlag);
                Console.WriteLine("             |--duration[{0}]", lift.tamper.action.delay);

                Console.WriteLine("             |--relay[{0}]", Util.getActionMsg(lift.tamper.action));

            }

            Console.WriteLine("     |--alarmFlags[{0}]", lift.alarmFlags);            
            
        }

        void print(IntPtr sdkContext, BS2LiftStatus liftStatus)
        {
            Console.WriteLine(">>>> liftStatus ID[{0}]", liftStatus.liftID);
            Console.WriteLine("     |--numFloors[{0}]", liftStatus.numFloors);
            Console.WriteLine("     |--alarmFlags[{0}]", (BS2LiftAlarmFlagEnum)liftStatus.alarmFlags);
            Console.WriteLine("     |--tamperOn[{0}]", Convert.ToBoolean(liftStatus.tamperOn));

            for (int i = 0; i < liftStatus.numFloors; ++i)
            {
                Console.WriteLine("     |++floor[{0}]", i);
                Console.WriteLine("         |--activated[{0}]", Convert.ToBoolean(liftStatus.floors[i].activated));
                Console.WriteLine("         |--activateFlags[{0}]", (BS2FloorFlagEnum)liftStatus.floors[i].activateFlags);
                Console.WriteLine("         |--deactivateFlags[{0}]", (BS2FloorFlagEnum)liftStatus.floors[i].deactivateFlags);                
            }
            
        }
    }

}