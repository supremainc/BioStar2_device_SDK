#pragma once
#include <iostream>
#include <string>
#include <vector>
#include <memory>
#include <array>
#include <Windows.h>
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

enum EN_MENU_DEV
{
	MENU_DEV_BREAK = MENU_BREAK,
	MENU_DEV_GET_DEVINF,
	MENU_DEV_GET_DEVINFEX,
	MENU_DEV_GET_DEVTIME,
	MENU_DEV_SET_DEVTIME,
	MENU_DEV_CLR_DATABASE,
	MENU_DEV_FACTORY_RESET,
	MENU_DEV_REBOOT_DEV,
	MENU_DEV_LOCK_DEV,
	MENU_DEV_UNLOCK_DEV,
	MENU_DEV_UPG_FIRMWARE,
	MENU_DEV_UPD_RESOURCE,
	MENU_DEV_GET_SPCDEVINFO,
	MENU_DEV_GET_AUTH_OPRLEVELEX,
	MENU_DEV_GET_ALLAUTH_OPRLEVELEX,
	MENU_DEV_SET_AUTH_OPRLEVELEX,
	MENU_DEV_REM_AUTH_OPRLEVELEX,
	MENU_DEV_REM_ALLAUTH_OPRLEVELEX,
};

std::vector<MENU_ITEM> menuInfoDeviceAPI =
{
	{MENU_DEV_BREAK,				"Exit"},
	{MENU_DEV_GET_DEVINF,			"BS2_GetDeviceInfo" },
	{MENU_DEV_GET_DEVINFEX,			"BS2_GetDeviceInfoEx" },
	{MENU_DEV_GET_DEVTIME,			"BS2_GetDeviceTime" },
	{MENU_DEV_SET_DEVTIME,			"BS2_SetDeviceTime" },
	{MENU_DEV_CLR_DATABASE,			"BS2_ClearDatabase" },
	{MENU_DEV_FACTORY_RESET,		"BS2_FactoryReset" },
	{MENU_DEV_REBOOT_DEV,			"BS2_RebootDevice" },
	{MENU_DEV_LOCK_DEV,				"BS2_LockDevice" },
	{MENU_DEV_UNLOCK_DEV,			"BS2_UnlockDevice" },
	{MENU_DEV_UPG_FIRMWARE,			"BS2_UpgradeFirmware" },
	{MENU_DEV_UPD_RESOURCE,			"BS2_UpdateResource" },
	{MENU_DEV_GET_SPCDEVINFO,		"BS2_GetSpecifiedDeviceInfo" },
	{MENU_DEV_GET_AUTH_OPRLEVELEX,	"BS2_GetAuthOperatorLevelEx" },
	{MENU_DEV_GET_ALLAUTH_OPRLEVELEX,"BS2_GetAllAuthOperatorLevelEx" },
	{MENU_DEV_SET_AUTH_OPRLEVELEX,	"BS2_SetAuthOperatorLevelEx" },
	{MENU_DEV_REM_AUTH_OPRLEVELEX,	"BS2_RemoveAuthOperatorLevelEx" },
	{MENU_DEV_REM_ALLAUTH_OPRLEVELEX,"BS2_RemoveAllAuthOperatorLevelEx" },
};


void connectTestDevice(void* context);
uint32_t showMenu(std::vector<MENU_ITEM>& info);
uint32_t getSelectedIndex();
int connectViaIP(void* context, DeviceInfo& device);
int connectSlave(void* context, DeviceInfo& device);
int connectWiegand(void* context, DeviceInfo& device);
int searchSlave(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& slaveID);
int searchCSTSlave(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& slaveID);
int searchWiegand(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& wiegandID);
void displayDeviceList(const std::vector<BS2SimpleDeviceInfo>& devices);
void displaySlaveList(const std::vector<BS2Rs485SlaveDevice>& devices);
void displayCSTSlaveList(const std::vector<BS2Rs485SlaveDeviceEX>& devices);
void displayWiegandList(const std::vector<BS2_DEVICE_ID>& devices);
int runAPIs(void* context, const DeviceInfo& device);
int getAllLogsFromDevice(void* context, BS2_DEVICE_ID id, int32_t timezone);
int getLogsFromDevice(void* context, BS2_DEVICE_ID id, int& latestIndex, int timezone);
int getImageLog(void* context, BS2_DEVICE_ID id, BS2_EVENT_ID eventID, uint8_t* imageBuf, uint32_t& imageSize);
BS2_DEVICE_ID getSelectedDeviceID(const DeviceInfo& info);
