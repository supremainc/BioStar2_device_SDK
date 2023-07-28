#include "stdafx.h"
#include <iostream>
#include <sstream>
#include <cassert>
#include <memory>
#include "DoorControl.h"
#include "BS_Errno.h"
#include "../Common/Utility.h"

#if defined(OS_LINUX)
#pragma GCC diagnostic ignored "-Wunused-parameter"
#endif

#define CAST_UINT32(x)				static_cast<uint32_t>(x)


extern void TRACE(const char* fmt, ...);
using namespace std;

void* DoorControl::context_ = NULL;

DoorControl::DoorControl(void* sdkContext)
{
	context_ = sdkContext;
}


DoorControl::~DoorControl()
{
}

int DoorControl::getAllDoor(BS2_DEVICE_ID id, std::vector<BS2Door>& doors)
{
	BS2Door* doorObjs = NULL;
	uint32_t numOfDoors(0);

	int sdkResult = BS2_GetAllDoor(context_, id, &doorObjs, &numOfDoors);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetAllDoor call failed: %d", sdkResult);
		return sdkResult;
	}

	doors.clear();

	for (uint32_t idx = 0; idx < numOfDoors; idx++)
	{
		doors.push_back(doorObjs[idx]);
	}

	BS2_ReleaseObject(doorObjs);

	return sdkResult;
}

int DoorControl::setDoor(BS2_DEVICE_ID id, std::vector<BS2Door>& doors)
{
	if (0 == doors.size())
		return BS_SDK_SUCCESS;

	BS2Door* ptrDoor = doors.data();
	int sdkResult = BS2_SetDoor(context_, id, ptrDoor, doors.size());
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetDoor call failed: %d", sdkResult);

	return sdkResult;
}

int DoorControl::removeAllDoor(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_RemoveAllDoor(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_RemoveAllDoor call failed: %d", sdkResult);

	return sdkResult;
}

int DoorControl::lockDoor(BS2_DEVICE_ID id)
{
	int sdkResult = BS_SDK_SUCCESS;
	return sdkResult;
}

int DoorControl::unlockDoor(BS2_DEVICE_ID id, std::vector<BS2_DOOR_ID>& doorIDs)
{
	int sdkResult = BS_SDK_SUCCESS;
	return sdkResult;
}

void DoorControl::print(const BS2Door& door)
{
	TRACE("==[BS2Door]==");
	TRACE("doorID: %u", door.doorID);
	TRACE("name : %s", door.name);
	TRACE("entryDeviceID : %u", door.entryDeviceID);
	TRACE("exitDeviceID : %u", door.exitDeviceID);
#if 0
	TRACE("relay");
	print(door.relay);
	TRACE("sensor");
	print(door.sensor);
	TRACE("button");
	print(door.button);

	TRACE("autoLockTimeout : %u", door.autoLockTimeout);
	TRACE("heldOpenTimeout : %u", door.heldOpenTimeout);
	TRACE("instantLock : %u", door.instantLock);
	TRACE("unlockFlags : %u", door.unlockFlags);
	TRACE("lockFlags : %u", door.lockFlags);
	TRACE("unconditionalLock : %u", door.unconditionalLock);

	for (int idx = 0; idx < BS2_MAX_FORCED_OPEN_ALARM_ACTION; idx++)
	{
		TRACE("forcedOpenAlarm[%d]", idx);
		print(door.forcedOpenAlarm[idx]);
	}

	for (int idx = 0; idx < BS2_MAX_HELD_OPEN_ALARM_ACTION; idx++)
	{
		TRACE("heldOpenAlarm[%d]", idx);
		print(door.heldOpenAlarm[idx]);
	}

	TRACE("dualAuthScheduleID : %u", door.dualAuthScheduleID);
	TRACE("dualAuthDevice : %u", door.dualAuthDevice);
	TRACE("dualAuthApprovalType : %u", door.dualAuthApprovalType);
	TRACE("dualAuthTimeout : %u", door.dualAuthTimeout);
	TRACE("numDualAuthApprovalGroups : %u", door.numDualAuthApprovalGroups);
	for (int idx = 0; idx < door.numDualAuthApprovalGroups; idx++)
		TRACE("dualAuthApprovalGroupID[%d] : %u", idx, door.dualAuthApprovalGroupID[idx]);
	//TRACE("apbZone : %u", door.apbZone);
#endif
}

void DoorControl::print(const BS2DoorRelay& relay)
{
	TRACE(" -deviceID : %u", relay.deviceID);
	TRACE(" -port : %u", relay.port);
}

void DoorControl::print(const BS2DoorSensor& sensor)
{
	TRACE(" -deviceID : %u", sensor.deviceID);
	TRACE(" -port : %u", sensor.port);
	TRACE(" -switchType : %u", sensor.switchType);
	TRACE(" -apbUseDoorSensor : %u", sensor.apbUseDoorSensor);
}

void DoorControl::print(const BS2ExitButton& button)
{
	TRACE(" -deviceID: %u", button.deviceID);
	TRACE(" -port : %u", button.port);
	TRACE(" -switchType : %u", button.switchType);
}

void DoorControl::print(const BS2Action& action)
{
	TRACE("   -deviceID : %u", action.deviceID);
	TRACE("   -type : %u", action.type);
	TRACE("   -stopFlag : %u", action.stopFlag);
	TRACE("   -delay : %u", action.delay);

	switch (action.type)
	{
	case BS2_ACTION_RELAY:
		TRACE("    relay");
		print(action.relay);
		break;

	case BS2_ACTION_TTL:
		TRACE("    outputPort");
		print(action.outputPort);
		break;

	case BS2_ACTION_DISPLAY:
		TRACE("    display");
		print(action.display);
		break;

	case BS2_ACTION_SOUND:
		TRACE("    sound");
		print(action.sound);
		break;

	case BS2_ACTION_LED:
		TRACE("    led");
		print(action.led);
		break;

	case BS2_ACTION_BUZZER:
		TRACE("    buzzer");
		print(action.buzzer);
		break;

	case BS2_ACTION_LIFT:
		TRACE("    lift");
		print(action.lift);
		break;

	case BS2_ACTION_UNLOCK_DEVICE:
	case BS2_ACTION_REBOOT_DEVICE:
	case BS2_ACTION_LOCK_DEVICE:
	case BS2_ACTION_RELEASE_ALARM:
	case BS2_ACTION_GENERAL_INPUT:
	case BS2_ACTION_FIRE_ALARM_INPUT:
	case BS2_ACTION_AUTH_SUCCESS:
	case BS2_ACTION_AUTH_FAIL:
		// Works with default
		break;
	case BS2_ACTION_NONE:
	default:
		break;
	}
}

void DoorControl::print(const BS2RelayAction& relay)
{
	TRACE("    -relayIndex : %u", relay.relayIndex);
	TRACE("       signal");
	print(relay.signal);
}

void DoorControl::print(const BS2OutputPortAction& output)
{
	TRACE("     -relayIndex : %u", output.portIndex);
	TRACE("       signal");
	print(output.signal);
}

void DoorControl::print(const BS2DisplayAction& display)
{
	TRACE("     -duration : %u", display.duration);
	TRACE("     -displayID : %u", display.displayID);
	TRACE("     -resourceID : %u", display.resourceID);
}

void DoorControl::print(const BS2SoundAction& sound)
{
	//TRACE(" - : %u", sound.);
}

void DoorControl::print(const BS2LedAction& led)
{
	//TRACE(" - : %u", sound.);
}

void DoorControl::print(const BS2BuzzerAction& buzzer)
{
	//TRACE(" - : %u", buzzer.);
}

void DoorControl::print(const BS2LiftAction& lift)
{
}

void DoorControl::print(const BS2Signal& signal)
{
	TRACE("     -signalID : %u", signal.signalID);
	TRACE("     -count : %u", signal.count);
	TRACE("     -onDuration : %u", signal.onDuration);
	TRACE("     -offDuration : %u", signal.offDuration);
	TRACE("     -delay : %u", signal.delay);
}
