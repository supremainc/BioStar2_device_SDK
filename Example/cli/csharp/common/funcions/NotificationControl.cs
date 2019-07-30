using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Suprema
{
    public class NotificationControl : FunctionModule
    {
        private const UInt32 MAX_PERCENT = 40;
        private API.OnAlarmFired cbOnAlarmFired = null;
        private API.OnInputDetected cbOnInputDetected = null;
        private API.OnConfigChanged cbOnConfigChanged = null;

        protected override List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = new List<KeyValuePair<string, Action<IntPtr, uint, bool>>>();

            if (isMasterDevice)
            {
                functionList.Add(new KeyValuePair<string, Action<IntPtr, uint, bool>>("Set notification listener", setNotificationListener));
            }            

            return functionList;
        }

        public void setNotificationListener(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice)
        {
            //TODO Configure zone to test alarm notification.

            //To test input detect notification.
            BS2TriggerActionConfig triggerActionConfig;
            Console.WriteLine("Trying to get trigger-action config.");
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetTriggerActionConfig(sdkContext, deviceID, out triggerActionConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            triggerActionConfig.numItems = 1;
            triggerActionConfig.items[0].trigger.deviceID = deviceID;
            triggerActionConfig.items[0].trigger.type = (byte)BS2TriggerTypeEnum.INPUT;

            BS2InputTrigger inputTrigger = Util.AllocateStructure<BS2InputTrigger>();
            inputTrigger.port = 0;
            inputTrigger.switchType = (byte)BS2SwitchTypeEnum.NORMAL_OPEN;
            inputTrigger.duration = 10;
            inputTrigger.scheduleID = (UInt32)BS2ScheduleIDEnum.ALWAYS;

            byte[] inputTriggerArray = Util.ConvertTo<BS2InputTrigger>(ref inputTrigger);
            Array.Clear(triggerActionConfig.items[0].trigger.triggerUnion, 0, triggerActionConfig.items[0].trigger.triggerUnion.Length);
            Array.Copy(inputTriggerArray, triggerActionConfig.items[0].trigger.triggerUnion, inputTriggerArray.Length);

            triggerActionConfig.items[0].action.deviceID = deviceID;
            triggerActionConfig.items[0].action.type = (byte)BS2ActionTypeEnum.SOUND;

            BS2SoundAction soundAction = Util.AllocateStructure<BS2SoundAction>();
            soundAction.count = 1;
            soundAction.soundIndex = (UInt16)BS2SoundIndexEnum.WELCOME;
            Array.Clear(soundAction.reserved, 0, soundAction.reserved.Length);

            byte[] inputActionArray = Util.ConvertTo<BS2SoundAction>(ref soundAction);
            Array.Clear(triggerActionConfig.items[0].action.actionUnion, 0, triggerActionConfig.items[0].action.actionUnion.Length);
            Array.Copy(inputActionArray, triggerActionConfig.items[0].action.actionUnion, inputActionArray.Length);

            Console.WriteLine("Trying to set trigger-action config.");
            result = (BS2ErrorCode)API.BS2_SetTriggerActionConfig(sdkContext, deviceID, ref triggerActionConfig);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Trying to activate notification listening.");
            cbOnAlarmFired = new API.OnAlarmFired(AlarmFired);
            cbOnInputDetected = new API.OnInputDetected(InputDetected);
            cbOnConfigChanged = new API.OnConfigChanged(ConfigChanged);
            result = (BS2ErrorCode)API.BS2_SetNotificationListener(sdkContext, cbOnAlarmFired, cbOnInputDetected, cbOnConfigChanged);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
                return;
            }

            Console.WriteLine("Press ESC to stop notification listening.");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Trying to deactivate notification listening.");
            result = (BS2ErrorCode)API.BS2_SetNotificationListener(sdkContext, null, null, null);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                Console.WriteLine("Got error({0}).", result);
            }

            cbOnAlarmFired = null;
            cbOnInputDetected = null;
            cbOnConfigChanged = null;
        }

        private void AlarmFired(UInt32 deviceID, IntPtr log)
        {
            if (log != IntPtr.Zero)
            {
                BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(log, typeof(BS2Event));
                Console.WriteLine("Notification : " + Util.GetLogMsg(eventLog));
            }
        }

        private void InputDetected(UInt32 deviceID, IntPtr log)
        {
            if (log != IntPtr.Zero)
            {
                BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(log, typeof(BS2Event));
                Console.WriteLine("Notification : " + Util.GetLogMsg(eventLog));
            }
        }

        private void ConfigChanged(UInt32 deviceID, UInt32 configMask)
        {
            Console.WriteLine("Notification : The {0} configuration of the device[{1}] was changed.", (BS2ConfigMaskEnum)configMask, deviceID);
        }
    }
}
