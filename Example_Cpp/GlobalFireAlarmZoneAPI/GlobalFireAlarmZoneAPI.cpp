#include "stdafx.h"
#include <sstream>
#include "GlobalFireAlarmZoneAPI.h"
#include "../Common/Utility.h"
#include "../Common/CommControl.h"
#include "../Common/LogControl.h"
#include "../Common/UserControl.h"
#include "../Common/AccessControl.h"
#include "../Common/DoorControl.h"
#include <memory>

extern void TRACE(const char* fmt, ...);
using namespace std;

static void* sdkContext = NULL;
static BS2_DEVICE_ID connectedID = 0;
static DeviceList deviceList;
static DoorList doorList;
static map<BS2_DEVICE_ID, BS2_DEVICE_ID> listAction;

void onLogReceived(BS2_DEVICE_ID id, const BS2Event* event)
{
	int32_t timezone = deviceList.getTimezone(id);
	cout << LogControl::getEventString(id, *event, timezone) << endl;
}

// Thermal supported callback
void onLogReceivedEx(BS2_DEVICE_ID id, const BS2Event* event, BS2_TEMPERATURE temperature)
{
	int32_t timezone = deviceList.getTimezone(id);
	cout << LogControl::getEventString(id, *event, timezone, temperature) << endl;
}


void onDeviceAccepted(BS2_DEVICE_ID id)
{
	TRACE("Device(%d) accepted", id);

	int sdkResult = BS2_ConnectDevice(sdkContext, id);
	if (sdkResult != BS_SDK_SUCCESS)
		TRACE("BS2_ConnectDevice call failed: %d", sdkResult);
}


void onDeviceConnected(BS2_DEVICE_ID id)
{
	int32_t timezone = 0;
	DeviceControl dc(sdkContext);

#if RETRIVE_BULK_LOGS
	if (!getDeviceLogs(id, timezone))
		return;
#endif

#if VER_271_OR_HIGHER
  #if REALTIME_LOG_TEMPERATURE
	int sdkResult = BS2_StartMonitoringLogEx(sdkContext, id, onLogReceivedEx);
  #else
	int sdkResult = BS2_StartMonitoringLog(sdkContext, id, onLogReceived);
  #endif
#else
	int sdkResult = BS2_StartMonitoringLog(sdkContext, id, onLogReceived);
#endif
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_StartMonitoringLogEx call failed: %d", sdkResult);

	if (!deviceList.findDevice(id))
	{
		// Device to server connection
		BS2SimpleDeviceInfo info = { 0, };
		if (BS_SDK_SUCCESS != (sdkResult = dc.getDeviceInfo(id, info)))
			BS2_DisconnectDevice(sdkContext, id);

		deviceList.appendDevice(id, info.type, info.ipv4Address, info.port, timezone);
	}

	deviceList.updateStatus(id, true);

	TRACE("Device(%d) connected", id);
}


void onDeviceDisconnected(BS2_DEVICE_ID id)
{
	deviceList.updateStatus(id, false);

	TRACE("Device(%d) disconnected", id);
}


void onInputDetected(BS2_DEVICE_ID id, const BS2Event* event)
{
	int timezone = deviceList.getTimezone(id);
	cout << LogControl::getEventString(id, *event, timezone) << endl;

	switch (event->code)
	{
	case BS2_EVENT_DEVICE_INPUT_DETECTED:
		postUnlockDoor(id);
		postUnlockDoor(listAction[id]);

		// Optional
		if (listAction.find(id) != listAction.end())
		{
			BS2Action action = { 0, };
			BS2_DEVICE_ID targetID = listAction[id];

			action.deviceID = targetID;
			action.type = BS2_ACTION_RELAY;
			action.stopFlag = 0;
			action.relay.relayIndex = 1;
			action.relay.signal.signalID = 1;
			action.relay.signal.count = 10;
			action.relay.signal.onDuration = 200;
			action.relay.signal.offDuration = 100;
			action.relay.signal.delay = 50;
			action.delay = 100;

			postActionCommand(targetID, action);
		}
		break;
	default:
		break;
	}
}


bool getDeviceLogs(BS2_DEVICE_ID id, int& timezone)
{
	ConfigControl cc(sdkContext);

	if (deviceList.findDevice(id))
		timezone = deviceList.getTimezone(id);
	else
		cc.getTimezone(id, timezone);

	int sdkResult = sdkResult = Utility::getAllLogsFromDevice(sdkContext, id, timezone);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("An error occurred while receiving bulk logs from device: %d", sdkResult);
		BS2_DisconnectDevice(sdkContext, id);
		return false;
	}

	return true;
}

int main(int argc, char* argv[])
{
	// Set debugging SDK log (to current working directory)
	BS2Context::setDebugFileLog(DEBUG_LOG_ALL, DEBUG_MODULE_ALL, ".", 100);

	TRACE("Version: %s", BS2_Version());

	sdkContext = BS2Context::getInstance()->getContext();

	bool sslMode = Utility::isYes("Do you want to connect your device with SSL?");
	if (sslMode)
	{
		if (BS_SDK_SUCCESS != setSSLHandler())
		{
			BS2Context::getInstance()->releaseInstance();
			return -1;
		}
	}

	// Create SDK context and initialize
	if (BS_SDK_SUCCESS != BS2Context::getInstance()->initSDK())
	{
		BS2Context::getInstance()->releaseInstance();
		return -1;
	}

	BS2Context::getInstance()->setNotificationListener(NULL, onInputDetected, NULL);

	BS2Context::getInstance()->setDeviceEventListener(onDeviceAccepted, onDeviceConnected, onDeviceDisconnected);

	connectTestDevice(sdkContext, deviceList);

	BS2Context::getInstance()->releaseInstance();
	return 0;
}

int setSSLHandler()
{
#ifndef _DEBUG
	if (Utility::isYes("Do you want to listen by SSL connection?"))
#endif
	{
		return BS2Context::getInstance()->setSSLHandler();
	}

	return BS_SDK_SUCCESS;
}

void connectTestDevice(void* context, DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	bool menuBreak = false;
	while (!menuBreak)
	{
		uint32_t selected = Utility::showMenu(menuInfoTop);
		switch (selected)
		{
		case MENU_TOP_BREAK:
			menuBreak = true;
			break;
		case MENU_TOP_SEARCH_N_CONN:
			sdkResult = Utility::searchAndConnect(context, deviceList);
			break;
		case MENU_TOP_DIRECT_IPADDR:
			sdkResult = Utility::connectViaIP(context, deviceList);
			break;
		case MENU_TOP_VIEW_DEVICE:
			Utility::displayConnectedDevices(deviceList, true, true);
			break;

		default:
			break;
		}
	}

	if (BS_SDK_SUCCESS != sdkResult)
		return;

	runAPIs(context, deviceList);
}

int runAPIs(void* context, const DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);
	CommControl cm(context);
	ConfigControl cc(context);
	LogControl lc(context);
	UserControl uc(context);

	cout << endl << endl << "== CommunicationAPI Test ==" << endl;
	BS2_DEVICE_ID id = 0;

	while (BS_SDK_SUCCESS == sdkResult && MENU_COMM_BREAK != (selectedTop = Utility::showMenu(menuInfoCommAPI)))
	{
		switch (selectedTop)
		{
		case MENU_COMM_BREAK:
			return BS_SDK_SUCCESS;
		case MENU_COMM_GET_DOOR:
			sdkResult = getDoors(context, deviceList, doorList);
			break;
		case MENU_COMM_SET_DOOR:
			break;
		case MENU_COMM_SET_DOOR_FOR_FIREALARM:
			sdkResult = makeFAZDoors(context, deviceList, doorList);
			break;
		case MENU_COMM_DEL_DOOR:
			sdkResult = removeDoors(context, deviceList);
			break;
		case MENU_COMM_GET_CONFIG_TRIGGERACTION:
			sdkResult = getTriggerActionConfig(context, deviceList);
			break;
		case  MENU_COMM_SET_CONFIG_TRIGGERACTION:
			break;
		case MENU_COMM_SET_ZONE_GLOBAL_FIREALARM:
			sdkResult = setGlobalFireAlarmZone(context, deviceList);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}

int getDoors(void* context, const DeviceList& devices, DoorList& doors)
{
	vector<BS2Door> vecDoors;
	int sdkResult = BS_SDK_SUCCESS;
	DoorControl dc(context);

	cout << "Now get doors from device..." << endl;
	BS2_DEVICE_ID selectedDevice = Utility::selectDeviceID(devices);

	if (BS_SDK_SUCCESS != (sdkResult = dc.getAllDoor(selectedDevice, vecDoors)))
		return sdkResult;

	if (0 < vecDoors.size())
	{
		for (auto door : vecDoors)
			dc.print(door);
	}
	else
		cout << selectedDevice << " has not door" << endl;

	doors[selectedDevice] = vecDoors;

	return sdkResult;
}

int makeFAZDoors(void* context, const DeviceList& devices, DoorList& doors)
{
	stringstream msg;
	vector<BS2Door> vecDoors;
	int sdkResult = BS_SDK_SUCCESS;
	DoorControl dc(context);

	cout << "Which device would you like to set as the TRIGGER device?" << endl;
	BS2_DEVICE_ID selectedDevice1 = Utility::selectDeviceID(devices);

	setDoor(selectedDevice1, vecDoors);
	sdkResult = dc.setDoor(selectedDevice1, vecDoors);
	if (BS_SDK_SUCCESS == sdkResult)
		doors[selectedDevice1] = vecDoors;
	else
		return sdkResult;

	vecDoors.clear();

	cout << "Which device would you like to set as the ACTION device?" << endl;
	BS2_DEVICE_ID selectedDevice2 = Utility::selectDeviceID(devices);

	if (selectedDevice1 == selectedDevice2)
		return BS_SDK_ERROR_INTERNAL;

	setDoor(selectedDevice2, vecDoors);
	sdkResult = dc.setDoor(selectedDevice2, vecDoors);

	doors[selectedDevice2] = vecDoors;

	return sdkResult;
}

void setDoor(BS2_DEVICE_ID id, vector<BS2Door>& vecDoors)
{
	stringstream msg;
	BS2Door door = { 0, };
	msg << "Please enter the door ID:";
	BS2_DOOR_ID doorID = Utility::getInput<BS2_DOOR_ID>(msg.str());

	string name = Utility::getInput<string>("Please enter the door name:");
	door.doorID = doorID;
	sprintf(door.name, "%s", name.c_str());
	door.entryDeviceID = id;
	door.exitDeviceID = 0;

	door.relay.deviceID = id;
	door.relay.port = 0;
	door.sensor.deviceID = 0;
	door.sensor.port = 0;
	door.sensor.switchType = BS2_SWITCH_TYPE_NORMAL_OPEN;
	door.sensor.apbUseDoorSensor = 0;
	door.button.deviceID = 0;
	door.button.port = 0;
	door.button.switchType = BS2_SWITCH_TYPE_NORMAL_OPEN;

	door.autoLockTimeout = 3;
	door.heldOpenTimeout = 3;
	door.instantLock = false;
	door.unlockFlags = BS2_DOOR_FLAG_EMERGENCY;
	door.lockFlags = BS2_DOOR_FLAG_NONE;
	door.unconditionalLock = false;

	//door.forcedOpenAlarm
	//door.heldOpenAlarm
	door.dualAuthScheduleID = 0;
	door.dualAuthDevice = BS2_DUAL_AUTH_NO_DEVICE;
	door.dualAuthApprovalType = BS2_DUAL_AUTH_APPROVAL_NONE;
	door.dualAuthTimeout = 1;
	door.numDualAuthApprovalGroups = 0;

	//door.apbZone

	vecDoors.push_back(door);
}

int removeDoors(void* context, const DeviceList& devices)
{
	DoorControl dc(context);

	int sdkResult = BS_SDK_SUCCESS;
	BS2_DEVICE_ID id = Utility::selectDeviceID(devices);
	if (Utility::isYes("All door settings on the device will be erased."))
		sdkResult = dc.removeAllDoor(id);

	return sdkResult;
}

int setGlobalFireAlarmZone(void* context, const DeviceList& devices)
{
	BS2TriggerActionConfig config = { 0, };
	ConfigControl cc(context);

	int sdkResult = BS_SDK_SUCCESS;
	cout << "[Trigger device]" << endl;
	BS2_DEVICE_ID idTrigger = Utility::selectDeviceID(devices);

	config.numItems = 1;
	BS2Trigger& trigger = config.items[0].trigger;
	BS2Action& action = config.items[0].action;

	trigger.deviceID = idTrigger;
	trigger.type = BS2_TRIGGER_INPUT;
	trigger.input.port = 0;
	trigger.input.switchType = BS2_SWITCH_TYPE_NORMAL_OPEN;
	trigger.input.duration = 500;
	trigger.input.scheduleID = BS2_SCHEDULE_ALWAYS_ID;

	action.deviceID = idTrigger;
	action.type = BS2_ACTION_FIRE_ALARM_INPUT;
	action.stopFlag = BS2_STOP_NONE;
	action.delay = 0;

	sdkResult = cc.setTriggerActionConfig(idTrigger, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	cout << "[Action device]" << endl;
	BS2_DEVICE_ID idAction = Utility::selectDeviceID(devices);

	listAction[idTrigger] = idAction;

	return sdkResult;
}

int getTriggerActionConfig(void* context, const DeviceList& devices)
{
	BS2TriggerActionConfig config = { 0, };
	ConfigControl cc(context);

	int sdkResult = BS_SDK_SUCCESS;
	BS2_DEVICE_ID id = Utility::selectDeviceID(devices);

	sdkResult = cc.getTriggerActionConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		if (0 < config.numItems)
			ConfigControl::print(config);
		else
			cout << id << " has not trigger action config" << endl;
	}

	return sdkResult;
}

int setTriggerActionConfig(void* context, const DeviceList& devices)
{
	BS2TriggerActionConfig config = { 0, };
	ConfigControl cc(context);

	int sdkResult = BS_SDK_SUCCESS;
	BS2_DEVICE_ID id = Utility::selectDeviceID(devices);

	config.numItems = 1;
	BS2Trigger& trigger = config.items[0].trigger;
	BS2Action& action = config.items[0].action;

	trigger.deviceID = id;
	trigger.type = BS2_TRIGGER_INPUT;
	trigger.input.port = 0;
	trigger.input.switchType = BS2_SWITCH_TYPE_NORMAL_OPEN;
	trigger.input.duration = 500;
	trigger.input.scheduleID = BS2_SCHEDULE_ALWAYS_ID;

	action.deviceID = id;
	action.type = BS2_ACTION_FIRE_ALARM_INPUT;
	action.stopFlag = BS2_STOP_NONE;
	action.delay = 0;

	return cc.setTriggerActionConfig(id, config);
}

void postActionCommand(BS2_DEVICE_ID id, BS2Action& action)
{
	DWORD thID;
	BS2Action* actionItem = new BS2Action;
	memcpy(actionItem, &action, sizeof(BS2Action));

	HANDLE handle = CreateThread(NULL, 0, onPostAction, actionItem, 0, &thID);
}

DWORD WINAPI onPostAction(LPVOID lpParam)
{
	shared_ptr<BS2Action> ptr(reinterpret_cast<BS2Action*>(lpParam));
	if (ptr)
	{
		BS2_DEVICE_ID targetID = ptr->deviceID;
		int sdkResult = BS2_RunAction(sdkContext, targetID, ptr.get());
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_RunAction call failed: %d", sdkResult);
			return -1;
		}

		cout << "Run action completed : " << targetID << endl;
	}

	return 0;
}

void postUnlockDoor(BS2_DEVICE_ID id)
{
	DWORD thID;
	BS2_DEVICE_ID* pID = new BS2_DEVICE_ID;
	*pID = id;

	HANDLE handle = CreateThread(NULL, 0, onPostUnlockDoor, pID, 0, &thID);
}

DWORD WINAPI onPostUnlockDoor(LPVOID lpParam)
{
	shared_ptr<BS2_DEVICE_ID> ptr(reinterpret_cast<BS2_DEVICE_ID*>(lpParam));
	if (ptr)
	{
		BS2_DEVICE_ID id = *ptr;
		vector<BS2_DOOR_ID> vecDoorIDs;
		if (findDoor(id, vecDoorIDs))
		{
			int sdkResult = BS2_UnlockDoor(sdkContext, id, BS2_DOOR_FLAG_EMERGENCY, vecDoorIDs.data(), vecDoorIDs.size());
			if (BS_SDK_SUCCESS != sdkResult)
			{
				TRACE("BS2_UnlockDoor call failed: [%u, %d]", id, sdkResult);
				return -1;
			}

			cout << "Door unlock completed : " << id << endl;
		}
		else
			cout << "Unmanaged device : " << id << endl;
	}

	return 0;
}

bool findDoor(BS2_DEVICE_ID deviceID, vector<BS2_DOOR_ID>& doorIDs)
{
	auto doors = doorList.find(deviceID);
	if (doors != doorList.end())
	{
		for (BS2Door& door : doors->second)
		{
			doorIDs.push_back(door.doorID);
		}
		return true;
	}

	return false;
}
