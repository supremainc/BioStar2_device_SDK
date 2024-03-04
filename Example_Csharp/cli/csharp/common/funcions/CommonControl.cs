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
        public static readonly string[] kTRIGGER_TYPE = {"none", "event", "input", "schedule"};
        public static readonly string[] kACTION_TYPE = {"none","lock device", "unlock device", "reboot device", "release alarm", "general input", "relay", "output", "sound", "display", "buzzer", "led", "fire alarm input", "auth success", "auth fail", "lift"};

        public static bool getTriggerActionConfig(IntPtr sdkContext, UInt32 deviceID, out BS2TriggerActionConfig config)
        {
            Console.WriteLine("Trying to get TriggerActionConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetTriggerActionConfig(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool setTriggerActionConfig(IntPtr sdkContext, UInt32 deviceID, ref BS2TriggerActionConfig config)
        {
            Console.WriteLine("How many trigger-action do you want to register?");
            Console.Write(">> ");
            config.numItems = Util.GetInput((byte)0); ;

            for (int idx = 0; idx < (int)config.numItems; idx++)
            {
                setTriggerAction(ref config.items[idx]);
            }
            Console.WriteLine("Trying to set TriggerActionConfig");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetTriggerActionConfig(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static void setTriggerAction(ref BS2TriggerAction triggerAction)
        {
	        setTrigger(ref triggerAction.trigger);
	        setAction(ref triggerAction.action);

            if ((BS2TriggerTypeEnum)triggerAction.trigger.type == BS2TriggerTypeEnum.INPUT &&
                ((BS2ActionTypeEnum)triggerAction.action.type == BS2ActionTypeEnum.AUTH_SUCCESS ||
                (BS2ActionTypeEnum)triggerAction.action.type == BS2ActionTypeEnum.AUTH_FAIL ||
                (BS2ActionTypeEnum)triggerAction.action.type == BS2ActionTypeEnum.LED))
            {
                Util.HighlightLine("[Trigger] Enter the interval(millisec) to ignore the input signal. (ignore input).", "ignore the input signal");
                Console.Write(">> ");
                triggerAction.trigger.ignoreSignalTime = Util.GetInput((UInt16)0);
            }
            else
            {
                triggerAction.trigger.ignoreSignalTime = 0;
            }
        }

        // Trigger
        public static void setTrigger(ref BS2Trigger trigger)
        {
            Util.HighlightLine("Please enter the device ID.", "device ID");
            Console.Write(">> ");
	        trigger.deviceID = Util.GetInput((UInt32)0);

            Util.HighlightLine("[Trigger] Select trigger type. (0: None, 1: Event, 2: Input, 3: Schedule)", "trigger type");
            Console.Write(">> ");
	        trigger.type = Util.GetInput((byte)0);

	        switch ((BS2TriggerTypeEnum)trigger.type)
	        {
	        case BS2TriggerTypeEnum.EVENT:
		        setEventTrigger(ref trigger.triggerUnion);
		        break;
	        case BS2TriggerTypeEnum.INPUT:
                setInputTrigger(ref trigger.triggerUnion);
		        break;
	        case BS2TriggerTypeEnum.SCHEDULE:
                setScheduleTrigger(ref trigger.triggerUnion);
		        break;
	        case BS2TriggerTypeEnum.NONE:
	        default:
		        break;
	        }
        }

        public static void setEventTrigger(ref byte[] triggerUnion)
        {
            BS2EventTrigger eventTrigger = Util.AllocateStructure<BS2EventTrigger>();
	        Util.HighlightLine("[EventTrigger] Enter the event code.", "event code");
            Console.Write(">> ");
	        eventTrigger.code = Util.GetInput((ushort)0);

            byte[] arr = Util.ConvertTo<BS2EventTrigger>(ref eventTrigger);
            Array.Clear(triggerUnion, 0, triggerUnion.Length);
            Array.Copy(arr, triggerUnion, arr.Length);
        }

        public static void setInputTrigger(ref byte[] triggerUnion)
        {
            BS2InputTrigger inputTrigger = Util.AllocateStructure<BS2InputTrigger>();
	        Util.HighlightLine("[InputTrigger] Enter the input port No.", "input port No.");
            Console.Write(">> ");
	        inputTrigger.port = Util.GetInput((byte)0);

	        Util.HighlightLine("[InputTrigger] Enter the switchType (0: N/O, 1: N/C).", "switchType");
            Console.Write(">> ");
	        inputTrigger.switchType = Util.GetInput((byte)0);

	        Util.HighlightLine("[InputTrigger] Enter the duration.", "duration");
            Console.Write(">> ");
	        inputTrigger.duration = Util.GetInput((UInt16)0);

	        Util.HighlightLine("[InputTrigger] Enter the schedule ID. (1: Always)", "schedule ID");
            Console.Write(">> ");
            inputTrigger.scheduleID = Util.GetInput((UInt32)1);

            byte[] arr = Util.ConvertTo<BS2InputTrigger>(ref inputTrigger);
            Array.Clear(triggerUnion, 0, triggerUnion.Length);
            Array.Copy(arr, triggerUnion, arr.Length);
        }

        public static void setScheduleTrigger(ref byte[] triggerUnion)
        {
            BS2ScheduleTrigger scheduleTrigger = Util.AllocateStructure<BS2ScheduleTrigger>();
	        Util.HighlightLine("[ScheduleTrigger] Enter the schedule trigger type. (0: On start, 1: On end)", "trigger type");
            Console.Write(">> ");
	        scheduleTrigger.type = Util.GetInput((UInt32)0);

	        Util.HighlightLine("[ScheduleTrigger] Enter the schedule ID. (1: Always)", "schedule ID");
            Console.Write(">> ");
            scheduleTrigger.scheduleID = Util.GetInput((UInt32)1);

            byte[] arr = Util.ConvertTo<BS2ScheduleTrigger>(ref scheduleTrigger);
            Array.Clear(triggerUnion, 0, triggerUnion.Length);
            Array.Copy(arr, triggerUnion, arr.Length);
        }

        // Action
        public static void setAction(ref BS2Action action)
        {
            Util.HighlightLine("Please enter the device ID.", "device ID");
            Console.Write(">> ");
	        action.deviceID = Util.GetInput((UInt32)0);

	        Util.HighlightLine("[Action] Select action type.", "action type");
	        Console.WriteLine("    0 : BS2_ACTION_NONE");
	        Console.WriteLine("    1 : BS2_ACTION_LOCK_DEVICE");
	        Console.WriteLine("    2 : BS2_ACTION_UNLOCK_DEVICE");
	        Console.WriteLine("    3 : BS2_ACTION_REBOOT_DEVICE");
	        Console.WriteLine("    4 : BS2_ACTION_RELEASE_ALARM");
	        Console.WriteLine("    5 : BS2_ACTION_GENERAL_INPUT");
	        Console.WriteLine("    6 : BS2_ACTION_RELAY");
	        Console.WriteLine("    7 : BS2_ACTION_TTL");
	        Console.WriteLine("    8 : BS2_ACTION_SOUND");
	        Console.WriteLine("    9 : BS2_ACTION_DISPLAY");
	        Console.WriteLine("   10 : BS2_ACTION_BUZZER");
	        Console.WriteLine("   11 : BS2_ACTION_LED");
	        Console.WriteLine("   12 : BS2_ACTION_FIRE_ALARM_INPUT");
	        Console.WriteLine("   13 : BS2_ACTION_AUTH_SUCCESS");
	        Console.WriteLine("   14 : BS2_ACTION_AUTH_FAIL");
	        Console.WriteLine("   15 : BS2_ACTION_LIFT");
	        action.type = Util.GetInput((byte)0);

	        Util.HighlightLine("[Action] Enter the stop flag. (0: None, 1: When door closed, 2: By command action)", "stop flag");
            Console.Write(">> ");
	        action.stopFlag = Util.GetInput((byte)0);

	        Util.HighlightLine("[Action] Enter the action delay", "delay");
            Console.Write(">> ");
            action.delay = Util.GetInput((UInt16)0);

	        switch ((BS2ActionTypeEnum)action.type)
	        {
            case BS2ActionTypeEnum.RELAY:
		        setRelayAction(ref action.actionUnion);
		        break;
            case BS2ActionTypeEnum.TTL:
                setOutputPortAction(ref action.actionUnion);
		        break;
	        case BS2ActionTypeEnum.SOUND:
                setSoundAction(ref action.actionUnion);
		        break;
	        case BS2ActionTypeEnum.DISPLAY:
                setDisplayAction(ref action.actionUnion);
		        break;
	        case BS2ActionTypeEnum.BUZZER:
                setBuzzerAction(ref action.actionUnion);
		        break;
	        case BS2ActionTypeEnum.LED:
                setLedAction(ref action.actionUnion);
		        break;
	        case BS2ActionTypeEnum.LIFT:
                setLiftAction(ref action.actionUnion);
		        break;
	        case BS2ActionTypeEnum.LOCK_DEVICE:
	        case BS2ActionTypeEnum.UNLOCK_DEVICE:
	        case BS2ActionTypeEnum.REBOOT_DEVICE:
	        case BS2ActionTypeEnum.RELEASE_ALARM:
	        case BS2ActionTypeEnum.GENERAL_INPUT:
	        case BS2ActionTypeEnum.FIRE_ALARM_INPUT:
	        case BS2ActionTypeEnum.AUTH_SUCCESS:
	        case BS2ActionTypeEnum.AUTH_FAIL:
		        break;
	        case BS2ActionTypeEnum.NONE:
	        default:
		        break;
	        }
        }

        public static void setRelayAction(ref byte[] actionUnion)
        {
            BS2RelayAction relayAction = Util.AllocateStructure<BS2RelayAction>();
	        Util.HighlightLine("[RelayAction] Enter the relay index.", "relay index");
            Console.Write(">> ");
	        relayAction.relayIndex = Util.GetInput((byte)0);

	        Util.HighlightLine("[RelayAction] Enter the signal ID.", "signal ID");
            Console.Write(">> ");
	        relayAction.signal.signalID = Util.GetInput((UInt32)0);

	        Util.HighlightLine("[RelayAction] Enter the signal count.", "signal count");
            Console.Write(">> ");
	        relayAction.signal.count = Util.GetInput((UInt16)0);

	        Util.HighlightLine("[RelayAction] Enter the signal On-duration.", "On-duration");
            Console.Write(">> ");
	        relayAction.signal.onDuration = Util.GetInput((UInt16)0);

	        Util.HighlightLine("[RelayAction] Enter the signal Off-duration.", "Off-duration");
            Console.Write(">> ");
	        relayAction.signal.offDuration = Util.GetInput((UInt16)0);

	        Util.HighlightLine("[RelayAction] Enter the signal delay.", "signal delay");
            Console.Write(">> ");
            relayAction.signal.delay = Util.GetInput((UInt16)0);

            byte[] arr = Util.ConvertTo<BS2RelayAction>(ref relayAction);
            Array.Clear(actionUnion, 0, actionUnion.Length);
            Array.Copy(arr, actionUnion, arr.Length);
        }

        public static void setOutputPortAction(ref byte[] actionUnion)
        {
            BS2OutputPortAction outputPortAction = Util.AllocateStructure<BS2OutputPortAction>();
	        Util.HighlightLine("[OutputPortAction] Enter the port index.", "port index");
            Console.Write(">> ");
	        outputPortAction.portIndex = Util.GetInput((byte)0);

	        Util.HighlightLine("[OutputPortAction] Enter the signal ID.", "signal ID");
            Console.Write(">> ");
	        outputPortAction.signal.signalID = Util.GetInput((UInt32)0);

	        Util.HighlightLine("[OutputPortAction] Enter the signal count.", "signal count");
            Console.Write(">> ");
	        outputPortAction.signal.count = Util.GetInput((UInt16)0);

	        Util.HighlightLine("[OutputPortAction] Enter the signal On-duration.", "On-duration");
            Console.Write(">> ");
	        outputPortAction.signal.onDuration = Util.GetInput((UInt16)0);

	        Util.HighlightLine("[OutputPortAction] Enter the signal Off-duration.", "Off-duration");
            Console.Write(">> ");
	        outputPortAction.signal.offDuration = Util.GetInput((UInt16)0);

	        Util.HighlightLine("[OutputPortAction] Enter the signal delay.", "signal delay");
            Console.Write(">> ");
            outputPortAction.signal.delay = Util.GetInput((UInt16)0);

            byte[] arr = Util.ConvertTo<BS2OutputPortAction>(ref outputPortAction);
            Array.Clear(actionUnion, 0, actionUnion.Length);
            Array.Copy(arr, actionUnion, arr.Length);
        }

        public static void setSoundAction(ref byte[] actionUnion)
        {
            BS2SoundAction soundAction = Util.AllocateStructure<BS2SoundAction>();
	        Util.HighlightLine("[SoundAction] Enter the count.", "count");
            Console.Write(">> ");
	        soundAction.count = Util.GetInput((byte)0);

	        Util.HighlightLine("[SoundAction] Enter the sound index. (0: Welcome, 1: Auth success, 2: Auth fail, 3: Alarm1, 4: Alarm2)", "sound index");
            Console.Write(">> ");
	        soundAction.soundIndex = Util.GetInput((UInt16)0);

	        Util.HighlightLine("[SoundAction] Enter the delay.", "delay");
            Console.Write(">> ");
	        soundAction.delay = Util.GetInput((UInt16)0);

            byte[] arr = Util.ConvertTo<BS2SoundAction>(ref soundAction);
            Array.Clear(actionUnion, 0, actionUnion.Length);
            Array.Copy(arr, actionUnion, arr.Length);
        }

        public static void setDisplayAction(ref byte[] actionUnion)
        {
            BS2DisplayAction displayAction = Util.AllocateStructure<BS2DisplayAction>();
	        Util.HighlightLine("[DisplayAction] Enter the duration.", "duration");
            Console.Write(">> ");
	        displayAction.duration = Util.GetInput((byte)0);

	        Util.HighlightLine("[DisplayAction] Enter the display ID.", "display ID");
            Console.Write(">> ");
	        displayAction.displayID = Util.GetInput((UInt32)0);

	        Util.HighlightLine("[DisplayAction] Enter the resource ID.", "resource ID");
            Console.Write(">> ");
	        displayAction.resourceID = Util.GetInput((UInt32)0);

            byte[] arr = Util.ConvertTo<BS2DisplayAction>(ref displayAction);
            Array.Clear(actionUnion, 0, actionUnion.Length);
            Array.Copy(arr, actionUnion, arr.Length);
        }

        public static void setBuzzerAction(ref byte[] actionUnion)
        {
            BS2BuzzerAction buzzerAction = Util.AllocateStructure<BS2BuzzerAction>();
	        Util.HighlightLine("[BuzzerAction] Enter the repeat count. (0: Infinite)", "repeat count");
            Console.Write(">> ");
	        buzzerAction.count = Util.GetInput((UInt16)0);

	        for (uint idx = 0; idx < BS2Environment.BS2_BUZZER_SIGNAL_NUM; idx++)
	        {
	            Util.HighlightLine("[BuzzerAction] Enter the buzzer tone. (0: Off, 1: Low, 2: Middle, 3: High)", "buzzer tone");
                Console.Write(">> ");
		        buzzerAction.signal[idx].tone = Util.GetInput((byte)0);

                Util.HighlightLine("[BuzzerAction] Do you want to enable fade out. [Y/n]", "enable fade out");
                Console.Write(">> ");
		        buzzerAction.signal[idx].fadeout = Convert.ToByte(Util.IsYes());

	            Util.HighlightLine("[BuzzerAction] Enter the buzzer duration.", "buzzer duration");
                Console.Write(">> ");
		        buzzerAction.signal[idx].duration = Util.GetInput((UInt16)0);

	            Util.HighlightLine("[BuzzerAction] Enter the buzzer delay.", "buzzer delay");
                Console.Write(">> ");
		        buzzerAction.signal[idx].delay = Util.GetInput((UInt16)0);
	        }

            byte[] arr = Util.ConvertTo<BS2BuzzerAction>(ref buzzerAction);
            Array.Clear(actionUnion, 0, actionUnion.Length);
            Array.Copy(arr, actionUnion, arr.Length);
        }

        public static void setLedAction(ref byte[] actionUnion)
        {
            BS2LedAction ledAction = Util.AllocateStructure<BS2LedAction>();
	        Util.HighlightLine("[LedAction] Enter the repeat count. (0: Infinite)", "repeat count");
            Console.Write(">> ");
	        ledAction.count = Util.GetInput((UInt16)0);

	        for (uint idx = 0; idx < BS2Environment.BS2_LED_SIGNAL_NUM; idx++)
	        {
	            Util.HighlightLine("[LedAction] Select the buzzer color", "buzzer color");
		        Console.WriteLine("    0 : BS2_LED_COLOR_OFF");
		        Console.WriteLine("    1 : BS2_LED_COLOR_RED");
		        Console.WriteLine("    2 : BS2_LED_COLOR_YELLOW");
		        Console.WriteLine("    3 : BS2_LED_COLOR_GREEN");
		        Console.WriteLine("    4 : BS2_LED_COLOR_CYAN");
		        Console.WriteLine("    5 : BS2_LED_COLOR_BLUE");
		        Console.WriteLine("    6 : BS2_LED_COLOR_MAGENTA");
		        Console.WriteLine("    7 : BS2_LED_COLOR_WHITE");
		        Console.WriteLine("    8 : BS2_LED_COLOR_BLUE_BREATHING");
                Console.Write(">> ");
		        ledAction.signal[idx].color = Util.GetInput((byte)0);

	            Util.HighlightLine("[LedAction] Enter the duration.", "duration");
                Console.Write(">> ");
		        ledAction.signal[idx].duration = Util.GetInput((UInt16)0);

	            Util.HighlightLine("[LedAction] Enter the delay.", "delay");
                Console.Write(">> ");
		        ledAction.signal[idx].delay = Util.GetInput((UInt16)0);
	        }

            byte[] arr = Util.ConvertTo<BS2LedAction>(ref ledAction);
            Array.Clear(actionUnion, 0, actionUnion.Length);
            Array.Copy(arr, actionUnion, arr.Length);
        }

        public static void setLiftAction(ref byte[] actionUnion)
        {
            BS2LiftAction liftAction = Util.AllocateStructure<BS2LiftAction>();
	        Util.HighlightLine("[LiftAction] Enter the lift ID.", "lift ID");
            Console.Write(">> ");
	        liftAction.liftID = Util.GetInput((UInt32)0);

	        Util.HighlightLine("[LiftAction] Enter the lift action type. (0: Activate floors, 1: Deactivate floors, 2: Release floors)", "lift action type");
            Console.Write(">> ");
	        liftAction.type = Util.GetInput((byte)0);

            byte[] arr = Util.ConvertTo<BS2LiftAction>(ref liftAction);
            Array.Clear(actionUnion, 0, actionUnion.Length);
            Array.Copy(arr, actionUnion, arr.Length);
        }

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

        public static bool getDeviceCapabilities(IntPtr sdkContext, UInt32 deviceID, out BS2DeviceCapabilities capabilities)
        {
            Console.WriteLine("Trying to get device capabilities");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceCapabilities(sdkContext, deviceID, out capabilities);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool getDesFireCardConfigEx(IntPtr sdkContext, UInt32 deviceID, out BS2DesFireCardConfigEx config)
        {
            Console.WriteLine("Trying to get DesFire card ext configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDesFireCardConfigEx(sdkContext, deviceID, out config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Call success.");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static bool setDesFireCardConfigEx(IntPtr sdkContext, UInt32 deviceID, ref BS2DesFireCardConfigEx config)
        {
            Console.WriteLine("Enter the hexadecimal application master key for DesFireCardConfigEx. [KEY1-KEY2-...-KEY16]");
            Console.Write(">>>> ");
            enterSmartcardKey(config.desfireAppKey.appMasterKey);

            Console.WriteLine("Enter the hexadecimal file read key. [KEY1-KEY2-...-KEY16]");
            Console.Write(">>>> ");
            enterSmartcardKey(config.desfireAppKey.fileReadKey);

            Console.WriteLine("Enter the file read key index.");
            Console.Write(">>>> ");
            config.desfireAppKey.fileReadKeyNumber = (byte)Util.GetInput();

            Console.WriteLine("Enter the hexadecimal file write key. [KEY1-KEY2-...-KEY16]");
            Console.Write(">>>> ");
            enterSmartcardKey(config.desfireAppKey.fileWriteKey);

            Console.WriteLine("Enter the file write key index.");
            Console.Write(">>>> ");
            config.desfireAppKey.fileWriteKeyNumber = (byte)Util.GetInput();

            Console.WriteLine("Trying to set DesFire card configuration.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDesFireCardConfigEx(sdkContext, deviceID, ref config);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                Console.WriteLine("Got error({0}).", result);
            else
                Console.WriteLine("Set DesFire card configuration succeeded");

            return BS2ErrorCode.BS_SDK_SUCCESS == result;
        }

        public static void enterSmartcardKey(byte[] dst)
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

        public static void print(ref BS2TriggerActionConfig config)
        {
            Console.WriteLine(">>>> BS2TriggerAction configuration");
	        for (int idx = 0; idx < config.numItems; idx++)
	        {
		        Console.WriteLine("     |--BS2TriggerAction[{0}]", idx);
                print(ref config.items[idx].trigger);
		        print(ref config.items[idx].action);
	        }
        }

        public static void print(ref BS2Trigger trigger)
        {
            Console.WriteLine("  >> BS2Trigger");
	        Console.WriteLine("     |--device : {0}", trigger.deviceID);
            Console.WriteLine("     |--type : {0}", kTRIGGER_TYPE[trigger.type]);
            Console.WriteLine("     |--ignoreSignalTime : {0}", trigger.ignoreSignalTime);
	        switch ((BS2TriggerTypeEnum)trigger.type)
	        {
	        case BS2TriggerTypeEnum.EVENT:
                BS2EventTrigger eventTrigger = Util.ConvertTo<BS2EventTrigger>(trigger.triggerUnion);
		        Console.WriteLine("     |--code : {0}", eventTrigger.code);
		        break;
	        case BS2TriggerTypeEnum.INPUT:
                BS2InputTrigger inputTrigger = Util.ConvertTo<BS2InputTrigger>(trigger.triggerUnion);
		        Console.WriteLine("     |--port : {0}", inputTrigger.port);
		        Console.WriteLine("     |--switchType : {0}", inputTrigger.switchType);
		        Console.WriteLine("     |--duration : {0}", inputTrigger.duration);
		        Console.WriteLine("     |--scheduleID : {0}", inputTrigger.scheduleID);
		        break;
	        case BS2TriggerTypeEnum.SCHEDULE:
                BS2ScheduleTrigger scheduleTrigger = Util.ConvertTo<BS2ScheduleTrigger>(trigger.triggerUnion);
		        Console.WriteLine("     |--scheduleType : {0}", scheduleTrigger.type);
		        Console.WriteLine("     |--scheduleID : {0}", scheduleTrigger.scheduleID);
		        break;
	        case BS2TriggerTypeEnum.NONE:
		        break;
	        default:
		        break;
	        }
        }

        public static void print(ref BS2Action action)
        {
            Console.WriteLine("  >> BS2Action");
	        Console.WriteLine("     |--device : {0}", action.deviceID);
            Console.WriteLine("     |--type : {0}", kACTION_TYPE[action.type]);
	        Console.WriteLine("     |--stopFlag : {0}", action.stopFlag);
	        Console.WriteLine("     |--delay : {0}", action.delay);
	        switch ((BS2ActionTypeEnum)action.type)
	        {
	        case BS2ActionTypeEnum.RELAY:
                BS2RelayAction relayAction = Util.ConvertTo<BS2RelayAction>(action.actionUnion);
		        Console.WriteLine("     |--relayIndex : {0}", relayAction.relayIndex);
		        Console.WriteLine("     |--signalID : {0}", relayAction.signal.signalID);
		        Console.WriteLine("     |--count : {0}", relayAction.signal.count);
		        Console.WriteLine("     |--onDuration : {0}", relayAction.signal.onDuration);
		        Console.WriteLine("     |--offDuration : {0}", relayAction.signal.offDuration);
		        Console.WriteLine("     |--delay : {0}", relayAction.signal.delay);
		        break;
	        case BS2ActionTypeEnum.TTL:
                BS2OutputPortAction outputPortAction = Util.ConvertTo<BS2OutputPortAction>(action.actionUnion);
		        Console.WriteLine("     |--relayIndex : {0}", outputPortAction.portIndex);
		        Console.WriteLine("     |--signalID : {0}", outputPortAction.signal.signalID);
		        Console.WriteLine("     |--count : {0}", outputPortAction.signal.count);
		        Console.WriteLine("     |--onDuration : {0}", outputPortAction.signal.onDuration);
		        Console.WriteLine("     |--offDuration : {0}", outputPortAction.signal.offDuration);
		        Console.WriteLine("     |--delay : {0}", outputPortAction.signal.delay);
		        break;
	        case BS2ActionTypeEnum.DISPLAY:
                BS2DisplayAction displayAction = Util.ConvertTo<BS2DisplayAction>(action.actionUnion);
		        Console.WriteLine("     |--duration : {0}", displayAction.duration);
		        Console.WriteLine("     |--displayID : {0}", displayAction.displayID);
		        Console.WriteLine("     |--resourceID : {0}", displayAction.resourceID);
		        break;
	        case BS2ActionTypeEnum.SOUND:
                BS2SoundAction soundAction = Util.ConvertTo<BS2SoundAction>(action.actionUnion);
                Console.WriteLine("     |--count : {0}", soundAction.count);
                Console.WriteLine("     |--soundIndex : {0}", soundAction.soundIndex);
                Console.WriteLine("     |--delay : {0}", soundAction.delay);
		        break;
	        case BS2ActionTypeEnum.LED:
                BS2LedAction ledAction = Util.ConvertTo<BS2LedAction>(action.actionUnion);
                Console.WriteLine("     |--count : {0}", ledAction.count);
                for (int idx = 0; idx < BS2Environment.BS2_LED_SIGNAL_NUM; idx++)
		        {
                    Console.WriteLine("     |--led[{0}].color : {1}", idx, ledAction.signal[idx].color);
                    Console.WriteLine("     |--led[{0}].duration : {1}", idx, ledAction.signal[idx].duration);
                    Console.WriteLine("     |--led[{0}].delay : {1}", idx, ledAction.signal[idx].delay);
		        }
		        break;
	        case BS2ActionTypeEnum.BUZZER:
                BS2BuzzerAction buzzerAction = Util.ConvertTo<BS2BuzzerAction>(action.actionUnion);
                Console.WriteLine("     |--count : {0}", buzzerAction.count);
                for (int idx = 0; idx < BS2Environment.BS2_BUZZER_SIGNAL_NUM; idx++)
		        {
                    Console.WriteLine("     |--buzzer[{0}].tone : {1}", idx, buzzerAction.signal[idx].tone);
                    Console.WriteLine("     |--buzzer[{0}].fadeout : {1}", idx, buzzerAction.signal[idx].fadeout);
                    Console.WriteLine("     |--buzzer[{0}].duration : {1}", idx, buzzerAction.signal[idx].duration);
                    Console.WriteLine("     |--buzzer[{0}].delay : {1}", idx, buzzerAction.signal[idx].delay);
		        }
		        break;
	        case BS2ActionTypeEnum.LIFT:
                BS2LiftAction liftAction = Util.ConvertTo<BS2LiftAction>(action.actionUnion);
		        Console.WriteLine("     |--liftID : {0}", liftAction.liftID);
		        Console.WriteLine("     |--type : {0}", liftAction.type);
		        break;
	        case BS2ActionTypeEnum.UNLOCK_DEVICE:
	        case BS2ActionTypeEnum.REBOOT_DEVICE:
	        case BS2ActionTypeEnum.LOCK_DEVICE:
	        case BS2ActionTypeEnum.RELEASE_ALARM:
	        case BS2ActionTypeEnum.GENERAL_INPUT:
	        case BS2ActionTypeEnum.FIRE_ALARM_INPUT:
	        case BS2ActionTypeEnum.AUTH_SUCCESS:
	        case BS2ActionTypeEnum.AUTH_FAIL:
		        break;
	        case BS2ActionTypeEnum.NONE:
		        break;
	        default:
		        break;
	        }
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

        public static void print(ref BS2Card card)
        {
            if (Convert.ToBoolean(card.isSmartCard))
            {
                BS2SmartCardData smartCard = Util.ConvertTo<BS2SmartCardData>(card.cardUnion);
                print(ref smartCard);
            }
            else
            {
                BS2CSNCard csnCard = Util.ConvertTo<BS2CSNCard>(card.cardUnion);
                print(ref csnCard);
            }
        }

        public static void print(ref BS2SmartCardData smartCard)
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

        public static void print(ref BS2CSNCard csnCard)
        {
            Console.WriteLine(">>>> CSN Card type[{0}] size[{1,2}] data[{2}]", (BS2CardTypeEnum)csnCard.type, csnCard.size, BitConverter.ToString(csnCard.data));
        }

        public static void print(ref BS2DeviceCapabilities info)
        {
            Console.WriteLine(">>>> Device capabilities");
            Console.WriteLine("     |--maxUsers : {0}", info.maxUsers);
            Console.WriteLine("     |--maxEventLogs : {0}", info.maxEventLogs);
            Console.WriteLine("     |--maxImageLogs : {0}", info.maxImageLogs);
            Console.WriteLine("     |--maxBlacklists : {0}", info.maxBlacklists);
            Console.WriteLine("     |--maxOperators : {0}", info.maxOperators);
            Console.WriteLine("     |--maxCards : {0}", info.maxCards);
            Console.WriteLine("     |--maxFaces : {0}", info.maxFaces);
            Console.WriteLine("     |--maxFingerprints : {0}", info.maxFingerprints);
            Console.WriteLine("     |--maxUserNames : {0}", info.maxUserNames);
            Console.WriteLine("     |--maxUserImages : {0}", info.maxUserImages);
            Console.WriteLine("     |--maxUserJobs : {0}", info.maxUserJobs);
            Console.WriteLine("     |--maxUserPhrases : {0}", info.maxUserPhrases);
            Console.WriteLine("     |--maxCardsPerUser : {0}", info.maxCardsPerUser);
            Console.WriteLine("     |--maxFacesPerUser : {0}", info.maxFacesPerUser);
            Console.WriteLine("     |--maxFingerprintsPerUser : {0}", info.maxFingerprintsPerUser);
            Console.WriteLine("     |--maxInputPorts : {0}", info.maxInputPorts);
            Console.WriteLine("     |--maxOutputPorts : {0}", info.maxOutputPorts);
            Console.WriteLine("     |--maxRelays : {0}", info.maxRelays);
            Console.WriteLine("     |--maxRS485Channels : {0}", info.maxRS485Channels);

            Console.WriteLine("     |--cameraSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_CAMERA));
            Console.WriteLine("     |--tamperSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_TAMPER));
            Console.WriteLine("     |--wlanSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_WLAN));
            Console.WriteLine("     |--displaySupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_DISPLAY));
            Console.WriteLine("     |--thermalSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_THERMAL));
            Console.WriteLine("     |--maskSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_MASK));
            Console.WriteLine("     |--faceExSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_FACEEX));
            Console.WriteLine("     |--voipExSupported : {0}", Convert.ToBoolean(info.systemSupported & (byte)BS2CapabilitySystemSupport.SYSTEM_SUPPORT_VOIPEX));

            Console.WriteLine("     |--[Card Supported]");
            Console.WriteLine("     |--mask : 0x{0}", Convert.ToString(info.cardSupportedMask, 16));
            Console.WriteLine("     |--EM : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_EM));
            Console.WriteLine("     |--HIDProx : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_HIDPROX));
            Console.WriteLine("     |--MifareFelica : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_MIFAREFELICA));
            Console.WriteLine("     |--iClass : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_ICLASS));
            Console.WriteLine("     |--ClassicPlus : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_CLASSICPLUS));
            Console.WriteLine("     |--DesFireEV1 : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_DESFIREEV1));
            Console.WriteLine("     |--SRSE : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_SRSE));
            Console.WriteLine("     |--SEOS : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_SEOS));
            Console.WriteLine("     |--NFC : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_NFC));
            Console.WriteLine("     |--BLE : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_BLE));
            Console.WriteLine("     |--CustomClassicPlus : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_CUSTOMCLASSICPLUS));
            Console.WriteLine("     |--CustomDesFireEV1 : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_CUSTOMDESFIREEV1));
            Console.WriteLine("     |--TOM_NFC : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_TOM_NFC));
            Console.WriteLine("     |--TOM_BLE : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_TOM_BLE));
            Console.WriteLine("     |--CustomFelica : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_CUSTOMFELICA));
            Console.WriteLine("     |--useCardOperation : {0}", Convert.ToBoolean(info.cardSupportedMask & (UInt32)BS2CapabilityCardSupport.CARD_SUPPORT_USECARDOPERATION));

            bool extMode = Convert.ToBoolean(info.authSupported.extendedMode);
            Console.WriteLine("     |--[ExtendedMode] : {0}", extMode);
            Console.WriteLine("     |--(Credentials)");
            Console.WriteLine("     |--mask : 0x{0}", Convert.ToString(info.authSupported.credentialsMask, 16));

            if (!extMode)
            {
                BS2LagacyAuth auth = Util.BytesToStruct<BS2LagacyAuth>(ref info.authSupported.auth);

                Console.WriteLine("     |--(Legacy)");
                Console.WriteLine("     |--biometricAuthMask : 0x{0}", Convert.ToString(auth.biometricAuthMask, 16));
                Console.WriteLine("     |--cardAuthMask : 0x{0}", Convert.ToString(auth.cardAuthMask, 16));
                Console.WriteLine("     |--idAuthMask : 0x{0}", Convert.ToString(auth.idAuthMask, 16));
            }
            else
            {
                BS2ExtendedAuth auth = Util.BytesToStruct<BS2ExtendedAuth>(ref info.authSupported.auth);

                Console.WriteLine("     |--(Extended)");
                Console.WriteLine("     |--faceAuthMask : 0x{0}", Convert.ToString(auth.faceAuthMask, 16));
                Console.WriteLine("     |--fingerprintAuthMask : 0x{0}", Convert.ToString(auth.fingerprintAuthMask, 16));
                Console.WriteLine("     |--cardAuthMask : 0x{0}", Convert.ToString(auth.cardAuthMask, 16));
                Console.WriteLine("     |--idAuthMask : 0x{0}", Convert.ToString(auth.idAuthMask, 16));
                Console.WriteLine("     |--faceAuth : 0x{0}", Convert.ToString(auth.faceAuthMask, 16));
            }

            Console.WriteLine("     |--intelligentPDSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_INTELLIGENTPD));
            Console.WriteLine("     |--updateUserSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_UPDATEUSER));
            Console.WriteLine("     |--simulatedUnlockSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_SIMULATEDUNLOCK));
            Console.WriteLine("     |--smartCardByteOrderSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_SMARTCARDBYTEORDER));
            Console.WriteLine("     |--treatAsCSNSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_TREATASCSN));
            Console.WriteLine("     |--rtspSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_RTSP));
            Console.WriteLine("     |--lfdSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_LFD));
            Console.WriteLine("     |--visualQRSupported : {0}", Convert.ToBoolean(info.functionSupported & (byte)BS2CapabilityFunctionSupport.FUNCTION_SUPPORT_VISUALQR));

            Console.WriteLine("     |--maxVoipExtensionNumbers : {0}", info.maxVoipExtensionNumbers);

            Console.WriteLine("     |--osdpStandardCentralSupported : {0}", Convert.ToBoolean(info.functionSupported2 & (byte)BS2CapabilityFunctionSupport2.FUNCTION2_SUPPORT_OSDPSTANDARDCENTRAL));
            Console.WriteLine("     |--enableLicenseFuncSupported : {0}", Convert.ToBoolean(info.functionSupported2 & (byte)BS2CapabilityFunctionSupport2.FUNCTION2_SUPPORT_ENABLELICENSE));
            Console.WriteLine("     |--keypadBacklightSupported : {0}", Convert.ToBoolean(info.functionSupported2 & (byte)BS2CapabilityFunctionSupport2.FUNCTION2_SUPPORT_KEYPADBACKLIGHT));
            Console.WriteLine("     |--uzWirelessLockDoorSupported : {0}", Convert.ToBoolean(info.functionSupported2 & (byte)BS2CapabilityFunctionSupport2.FUNCTION2_SUPPORT_UZWIRELESSLOCKDOOR));
            Console.WriteLine("     |--customSmartCardSupported : {0}", Convert.ToBoolean(info.functionSupported2 & (byte)BS2CapabilityFunctionSupport2.FUNCTION2_SUPPORT_CUSTOMSMARTCARD));
            Console.WriteLine("     |--tomSupported : {0}", Convert.ToBoolean(info.functionSupported2 & (byte)BS2CapabilityFunctionSupport2.FUNCTION2_SUPPORT_TOM));
            Console.WriteLine("     |--tomEnrollSupported : {0}", Convert.ToBoolean(info.functionSupported2 & (byte)BS2CapabilityFunctionSupport2.FUNCTION2_SUPPORT_TOMENROLL));
            Console.WriteLine("     |--showOsdpResultbyLED : {0}", Convert.ToBoolean(info.functionSupported2 & (byte)BS2CapabilityFunctionSupport2.FUNCTION2_SUPPORT_SHOWOSDPRESULTBYLED));

            Console.WriteLine("     |--customSmartCardFelicaSupported : {0}", Convert.ToBoolean(info.functionSupported3 & (byte)BS2CapabilityFunctionSupport3.FUNCTION3_SUPPORT_CUSTOMSMARTCARDFELICA));
            Console.WriteLine("     |--ignoreInputAfterWiegandOut : {0}", Convert.ToBoolean(info.functionSupported3 & (byte)BS2CapabilityFunctionSupport3.FUNCTION3_SUPPORT_IGNOREINPUTAFTERWIEGANDOUT));
            Console.WriteLine("     |--setSlaveBaudrateSupported : {0}", Convert.ToBoolean(info.functionSupported3 & (byte)BS2CapabilityFunctionSupport3.FUNCTION3_SUPPORT_SETSLAVEBAUDRATE));

            Console.WriteLine("     |--visualFaceTemplateVersion : {0}", info.visualFaceTemplateVersion);
        }

        public static void print(ref BS2DesFireCardConfigEx config)
        {
            Console.WriteLine(">>>> DesFire card configuration ");
            Console.WriteLine("     |--appMasterKey : {0}", config.desfireAppKey.appMasterKey);
            Console.WriteLine("     |--fileReadKey : {0}", config.desfireAppKey.fileReadKey);
            Console.WriteLine("     |--fileWriteKey : {0}", config.desfireAppKey.fileWriteKey);
            Console.WriteLine("     |--fileReadKeyNumber : {0}", config.desfireAppKey.fileReadKeyNumber);
            Console.WriteLine("     +--fileWriteKeyNumber : {0}", config.desfireAppKey.fileWriteKeyNumber);
            Console.WriteLine("<<<< ");
        }
    }
}
