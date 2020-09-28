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
	MENU_TOP_SEARCH_SLAVE = 3,
	MENU_TOP_SEARCH_WIEGAND = 4,
	MENU_TOP_VIEW_DEVICE = 5,
};

std::vector<MENU_ITEM> menuInfoTop =
{
	{MENU_TOP_BREAK,			"Exit and API test"},
	{MENU_TOP_SEARCH_N_CONN,	"Connection (Discover and connect)"},
	{MENU_TOP_DIRECT_IPADDR,	"Connection (Direct connect with IP/Port)"},
	{MENU_TOP_SEARCH_SLAVE,		"Discover and add slave device"},
	{MENU_TOP_SEARCH_WIEGAND,	"Discover and add wiegand device" },
	{MENU_TOP_VIEW_DEVICE,		"View all devices"},
};

enum EN_MENU_COMM
{
	MENU_COMM_BREAK = MENU_BREAK,
	MENU_COMM_SET_KEEP_ALIVE = 1,
	MENU_COMM_GET_SERVER_PORT,
	MENU_COMM_SET_SERVER_PORT,
	MENU_COMM_GET_SERVER_SSLPORT,
	MENU_COMM_SET_SERVER_SSLPORT,
	MENU_COMM_IS_CONNECTED,
	MENU_COMM_IS_AUTO_CONNECT,
	MENU_COMM_SET_AUTO_CONNECT,
	MENU_COMM_GET_ENABLEIPV4,
	MENU_COMM_SET_ENABLEIPV4,
	MENU_COMM_GET_ENABLEIPV6,
	MENU_COMM_SET_ENABLEIPV6,
	MENU_COMM_GET_SERVERPORTV6,
	MENU_COMM_SET_SERVERPORTV6,
	MENU_COMM_GET_SERVERSSLPORTV6,
	MENU_COMM_SET_SERVERSSLPORTV6,
	MENU_COMM_GET_DEF_RES_TIMEOUT,
	MENU_COMM_SET_DEF_RES_TIMEOUT,
	MENU_CONF_GET_FACCONFIG,
	MENU_ELOG_GET_EVENTSMALLBLOB,
	MENU_ELOG_GET_EVENTSMALLBLOBEX,
	MENU_USER_ENROLL_FACE,
	MENU_CONF_UPD_DEVICE_2_SERVER,
	MENU_CONF_UPD_SERVER_2_DEVICE,
};

std::vector<MENU_ITEM> menuInfoCommAPI =
{
	{MENU_COMM_BREAK,				"Exit"},
	{MENU_COMM_SET_KEEP_ALIVE,		"BS2_SetKeepAliveTimeout"},
	{MENU_COMM_GET_SERVER_PORT,		"BS2_GetServerPort"},
	{MENU_COMM_SET_SERVER_PORT,		"BS2_SetServerPort"},
	{MENU_COMM_GET_SERVER_SSLPORT,	"BS2_GetSSLServerPort"},
	{MENU_COMM_SET_SERVER_SSLPORT,	"BS2_SetSSLServerPort"},
	{MENU_COMM_IS_CONNECTED,		"BS2_IsConnected"},
	{MENU_COMM_IS_AUTO_CONNECT,		"BS2_IsAutoConnection"},
	{MENU_COMM_SET_AUTO_CONNECT,	"BS2_SetAutoConnection"},
	{MENU_COMM_GET_ENABLEIPV4,		"BS2_GetEnableIPV4"},
	{MENU_COMM_SET_ENABLEIPV4,		"BS2_SetEnableIPV4"},
	{MENU_COMM_GET_ENABLEIPV6,		"BS2_GetEnableIPV6"},
	{MENU_COMM_SET_ENABLEIPV6,		"BS2_SetEnableIPV6"},
	{MENU_COMM_GET_SERVERPORTV6,	"BS2_GetServerPortIPV6"},
	{MENU_COMM_SET_SERVERPORTV6,	"BS2_SetServerPortIPV6"},
	{MENU_COMM_GET_SERVERSSLPORTV6,	"BS2_GetSSLServerPortIPV6"},
	{MENU_COMM_SET_SERVERSSLPORTV6,	"BS2_SetSSLServerPortIPV6"},
	{MENU_COMM_GET_DEF_RES_TIMEOUT,	"BS2_GetDefaultResponseTimeout"},
	{MENU_COMM_SET_DEF_RES_TIMEOUT,	"BS2_SetDefaultResponseTimeout"},
	{MENU_CONF_GET_FACCONFIG,		"BS2_GetFactoryConfig" },
	{MENU_ELOG_GET_EVENTSMALLBLOB,	"BS2_GetEventSmallBlob"},
	{MENU_ELOG_GET_EVENTSMALLBLOBEX,	"BS2_GetEventSmallBlobEx"},
	{MENU_USER_ENROLL_FACE,			"BS2_EnrolUser"},
	{MENU_CONF_UPD_DEVICE_2_SERVER,	"Update device to server connection"},
	{MENU_CONF_UPD_SERVER_2_DEVICE,	"Update server to device connection"},
};


bool getDeviceLogs(BS2_DEVICE_ID id, int& timezone);
void connectTestDevice(void* context, DeviceList& deviceList);
uint32_t showMenu(std::vector<MENU_ITEM>& info);
uint32_t getSelectedIndex();
int searchAndConnect(void* context, DeviceList& deviceList);
int connectViaIP(void* context, DeviceList& deviceList);
int connectSlave(void* context, DeviceList& deviceList);
int connectWiegand(void* context, DeviceList& deviceList);
int searchSlave(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& slaveID);
int searchCSTSlave(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& slaveID);
int searchWiegand(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& wiegandID);
void displayDeviceList(const std::vector<BS2SimpleDeviceInfo>& devices);
void displaySlaveList(const std::vector<BS2Rs485SlaveDevice>& devices);
void displayCSTSlaveList(const std::vector<BS2Rs485SlaveDeviceEX>& devices);
void displayWiegandList(const std::vector<BS2_DEVICE_ID>& devices);
BS2_DEVICE_ID selectDeviceID(const DeviceList& deviceList, bool includeSlave = false, bool includeWiegand = false);
int runAPIs(void* context, const DeviceList& deviceList);
int getAllLogsFromDevice(void* context, BS2_DEVICE_ID id, int32_t timezone);
int getLogsFromDevice(void* context, BS2_DEVICE_ID id, int& latestIndex, int timezone);
int getImageLog(void* context, BS2_DEVICE_ID id, BS2_EVENT_ID eventID, uint8_t* imageBuf, uint32_t& imageSize);
void displayConnectedDevices(const DeviceList& devices, bool includeSlave = false, bool includeWiegand = false);
int updateConnectModeDevice2Server(void* context, BS2_DEVICE_ID id);
int updateConnectModeServer2Device(void* context, BS2_DEVICE_ID id);
#if 0
DWORD WINAPI onWaiting(LPVOID lpParam);
void waitForConnection();
#endif