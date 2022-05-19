#pragma once

#if defined(_WIN32) || defined(_WIN64)
#include <Windows.h>
#endif

#include <iostream>
#include <string>
#include <vector>
#include <memory>
#include <array>
#include "PreDefined.h"
#include "BS_Errno.h"
#include "../Common/BS2Context.h"
#include "../Common/DeviceControl.h"
#include "../Common/ConfigControl.h"
#include "../Common/CommControl.h"
#include "../Common/DeviceList.h"

#define MENU_BREAK				0


typedef struct
{
	int			index;
	std::string disc;
} MENU_ITEM;

enum EN_MENU_TOP
{
	MENU_TOP_BREAK = MENU_BREAK,
	MENU_TOP_SEARCH_N_CONN = 1,
	MENU_TOP_DIRECT_IPADDR = 2,
	MENU_TOP_VIEW_DEVICE = 5,
};

std::vector<MENU_ITEM> menuInfoTop =
{
	{MENU_TOP_BREAK,			"Exit and API test"},
	{MENU_TOP_SEARCH_N_CONN,	"Connection (Discover and connect)"},
	{MENU_TOP_DIRECT_IPADDR,	"Connection (Direct connect with IP/Port)"},
	{MENU_TOP_VIEW_DEVICE,		"View (Connected devices)"},
};

enum EN_MENU_COMM
{
	MENU_COMM_BREAK = MENU_BREAK,

	MENU_COMM_GET_DOOR,
	MENU_COMM_SET_DOOR,
	MENU_COMM_SET_DOOR_FOR_FIREALARM,
	MENU_COMM_DEL_DOOR,
	MENU_COMM_GET_CONFIG_TRIGGERACTION,
	MENU_COMM_SET_CONFIG_TRIGGERACTION,
	MENU_COMM_SET_ZONE_GLOBAL_FIREALARM,
};

std::vector<MENU_ITEM> menuInfoCommAPI =
{
	{MENU_COMM_BREAK,						"Exit"},
	{MENU_COMM_GET_DOOR,					"Get door"},
	{MENU_COMM_SET_DOOR,					"Set door"},
	{MENU_COMM_SET_DOOR_FOR_FIREALARM,		"Set FireAlarm door" },
	{MENU_COMM_DEL_DOOR,					"Remove door"},
	{MENU_COMM_GET_CONFIG_TRIGGERACTION,	"Get TriggerAction config"},
	{MENU_COMM_SET_CONFIG_TRIGGERACTION,	"Set TriggerAction config"},
	{MENU_COMM_SET_ZONE_GLOBAL_FIREALARM,	"Set Global FireAlarm zone"},
};

using DoorList = std::map<BS2_DEVICE_ID, std::vector<BS2Door>>;


int setSSLHandler();
bool getDeviceLogs(BS2_DEVICE_ID id, int& timezone);
void connectTestDevice(void* context, DeviceList& deviceList);
uint32_t showMenu(std::vector<MENU_ITEM>& info);
uint32_t getSelectedIndex();
int searchAndConnect(void* context, DeviceList& deviceList);
int connectViaIP(void* context, DeviceList& deviceList);
void displayDeviceList(const std::vector<BS2SimpleDeviceInfo>& devices);
void displaySlaveList(const std::vector<BS2Rs485SlaveDevice>& devices);
void displayCSTSlaveList(const std::vector<BS2Rs485SlaveDeviceEX>& devices);
void displayWiegandList(const std::vector<BS2_DEVICE_ID>& devices);
BS2_DEVICE_ID selectDeviceID(const DeviceList& deviceList, bool includeSlave = false, bool includeWiegand = false);
void selectDeviceIDs(const DeviceList& deviceList, BS2_DEVICE_ID& masterID, std::vector<BS2_DEVICE_ID>& selectedDevices, bool includeSlave, bool includeWiegand);
int runAPIs(void* context, const DeviceList& deviceList);
int getAllLogsFromDevice(void* context, BS2_DEVICE_ID id, int32_t timezone);
int getLogsFromDevice(void* context, BS2_DEVICE_ID id, int& latestIndex, int timezone);
int getImageLog(void* context, BS2_DEVICE_ID id, BS2_EVENT_ID eventID, uint8_t* imageBuf, uint32_t& imageSize);
void displayConnectedDevices(const DeviceList& devices, bool includeSlave = false, bool includeWiegand = false);
int getDoors(void* context, const DeviceList& devices, DoorList& doors);
int makeFAZDoors(void* context, const DeviceList& devices, DoorList& doors);
void setDoor(BS2_DEVICE_ID id, std::vector<BS2Door>& vecDoors);
int removeDoors(void* context, const DeviceList& devices);
int setGlobalFireAlarmZone(void* context, const DeviceList& devices);
int getTriggerActionConfig(void* context, const DeviceList& devices);
int setTriggerActionConfig(void* context, const DeviceList& devices);
void postActionCommand(BS2_DEVICE_ID id, BS2Action& action);
void postUnlockDoor(BS2_DEVICE_ID id);
DWORD WINAPI onPostAction(LPVOID lpParam);
DWORD WINAPI onPostUnlockDoor(LPVOID lpParam);
bool findDoor(BS2_DEVICE_ID deviceID, std::vector<BS2_DOOR_ID>& doorIDs);
