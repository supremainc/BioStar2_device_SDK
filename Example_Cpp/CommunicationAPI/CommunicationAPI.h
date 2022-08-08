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
	MENU_TOP_CONNECT_USB = 6,
};

std::vector<MENU_ITEM> menuInfoTop =
{
	{MENU_TOP_BREAK,			"Exit and API test"},
	{MENU_TOP_SEARCH_N_CONN,	"Connection (Discover and connect)"},
	{MENU_TOP_DIRECT_IPADDR,	"Connection (Direct connect with IP/Port)"},
	{MENU_TOP_SEARCH_SLAVE,		"Slave (Discover and add)"},
	{MENU_TOP_SEARCH_WIEGAND,	"Wiegand (Discover and add)" },
	{MENU_TOP_VIEW_DEVICE,		"View (Connected devices)"},
	{MENU_TOP_CONNECT_USB,		"Connection (USB)"},
};

enum EN_MENU_SLAVE
{
	MENU_SLV_BREAK = MENU_BREAK,
	MENU_SLV_GET_CONFIG_RS485EX = 1,
	MENU_SLV_SEARCH_DEVICE,
	MENU_SLV_UPG_FIRMWARE,
	MENU_SLV_GET_CONFIG_FACTORY,
	MENU_SLV_GET_CONFIG_STATUS,
	MENU_SLV_SET_CONFIG_STATUS,
};

std::vector<MENU_ITEM> menuInfoSlave =
{
	{MENU_SLV_BREAK,				"Exit"},
	{MENU_SLV_GET_CONFIG_RS485EX,	"Connected status (of slave devices)"},
	{MENU_SLV_SEARCH_DEVICE,		"Discover and add (slave devices)"},
	{MENU_SLV_UPG_FIRMWARE,			"Upgrade firmware"},
	{MENU_SLV_GET_CONFIG_FACTORY,	"Get config (Factory)"},
	{MENU_SLV_GET_CONFIG_STATUS,	"Get config (Status)"},
	{MENU_SLV_SET_CONFIG_STATUS,	"Set config (Status)"},
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
	MENU_USER_ENROLL_FACEEX,
	MENU_USER_ENROLL_MULTIPLE,

	MENU_CONF_UPD_DEVICE_2_SERVER,
	MENU_CONF_UPD_SERVER_2_DEVICE,
};

enum EN_MENU_USB
{
	MENU_USB_BREAK = MENU_BREAK,
	MENU_USB_GET_USR_DBINFO_FROMDIR,
	MENU_USB_GET_USR_LIST_FROMDIR,
	MENU_USB_GET_USR_INFO_FROMDIR,
	MENU_USB_GET_USR_DATA_FROMDIR,
	MENU_USB_GET_USR_INFOEX_FROMDIR,
	MENU_USB_GET_USR_DATAEX_FROMDIR,
	MENU_USB_GET_USRSMALL_INFO_FROMDIR,
	MENU_USB_GET_USRSMALL_DATA_FROMDIR,
	MENU_USB_GET_USRSMALL_INFOEX_FROMDIR,
	MENU_USB_GET_USRSMALL_DATAEX_FROMDIR,
	MENU_USB_GET_USRFACEEX_INFO_FROMDIR,
	MENU_USB_GET_USRFACEEX_DATA_FROMDIR,

	MENU_USB_GET_FILTEREDLOG_FROMDIR,
	MENU_USB_GET_LOG_FROMDIR,
	MENU_USB_GET_LOG_BLOB_FROMDIR,
	MENU_USB_GET_LOG_SMALLBLOB_FROMDIR,
	MENU_USB_GET_LOG_SMALLBLOBEX_FROMDIR,
};

std::vector<MENU_ITEM> menuInfoCommAPI =
{
	{MENU_COMM_BREAK,						"Exit"},
	{MENU_COMM_SET_KEEP_ALIVE,				"BS2_SetKeepAliveTimeout"},

	{MENU_COMM_GET_SERVER_PORT,				"BS2_GetServerPort"},
	{MENU_COMM_SET_SERVER_PORT,				"BS2_SetServerPort"},

	{MENU_COMM_GET_SERVER_SSLPORT,			"BS2_GetSSLServerPort"},
	{MENU_COMM_SET_SERVER_SSLPORT,			"BS2_SetSSLServerPort"},

	{MENU_COMM_IS_CONNECTED,				"BS2_IsConnected"},

	{MENU_COMM_IS_AUTO_CONNECT,				"BS2_IsAutoConnection"},
	{MENU_COMM_SET_AUTO_CONNECT,			"BS2_SetAutoConnection"},

	{MENU_COMM_GET_ENABLEIPV4,				"BS2_GetEnableIPV4"},
	{MENU_COMM_SET_ENABLEIPV4,				"BS2_SetEnableIPV4"},

	{MENU_COMM_GET_ENABLEIPV6,				"BS2_GetEnableIPV6"},
	{MENU_COMM_SET_ENABLEIPV6,				"BS2_SetEnableIPV6"},

	{MENU_COMM_GET_SERVERPORTV6,			"BS2_GetServerPortIPV6"},
	{MENU_COMM_SET_SERVERPORTV6,			"BS2_SetServerPortIPV6"},

	{MENU_COMM_GET_SERVERSSLPORTV6,			"BS2_GetSSLServerPortIPV6"},
	{MENU_COMM_SET_SERVERSSLPORTV6,			"BS2_SetSSLServerPortIPV6"},

	{MENU_COMM_GET_DEF_RES_TIMEOUT,			"BS2_GetDefaultResponseTimeout"},
	{MENU_COMM_SET_DEF_RES_TIMEOUT,			"BS2_SetDefaultResponseTimeout"},

	{MENU_CONF_GET_FACCONFIG,				"BS2_GetFactoryConfig" },

	{MENU_ELOG_GET_EVENTSMALLBLOB,			"BS2_GetEventSmallBlob"},
	{MENU_ELOG_GET_EVENTSMALLBLOBEX,		"BS2_GetEventSmallBlobEx"},

	{MENU_USER_ENROLL_FACE,					"BS2_EnrolUser"},
	{MENU_USER_ENROLL_FACEEX,				"BS2_EnrollUserFaceEx to CS40"},
	{MENU_USER_ENROLL_MULTIPLE,				"Enroll multiple users"},

	{MENU_CONF_UPD_DEVICE_2_SERVER,			"Update device to server connection"},
	{MENU_CONF_UPD_SERVER_2_DEVICE,			"Update server to device connection"},
};

std::vector<MENU_ITEM> menuInfoUSBAPI =
{
	{MENU_COMM_BREAK,						"Exit"},
	{MENU_USB_GET_USR_DBINFO_FROMDIR,		"BS2_GetUserDatabaseInfoFromDir"},
	{MENU_USB_GET_USR_LIST_FROMDIR,			"BS2_GetUserListFromDir"},
	{MENU_USB_GET_USR_INFO_FROMDIR,			"BS2_GetUserInfosFromDir"},
	{MENU_USB_GET_USR_DATA_FROMDIR,			"BS2_GetUserDatasFromDir"},
	{MENU_USB_GET_USR_INFOEX_FROMDIR,		"BS2_GetUserInfosExFromDir"},
	{MENU_USB_GET_USR_DATAEX_FROMDIR,		"BS2_GetUserDatasExFromDir"},
	{MENU_USB_GET_USRSMALL_INFO_FROMDIR,	"BS2_GetUserSmallInfosFromDir"},
	{MENU_USB_GET_USRSMALL_DATA_FROMDIR,	"BS2_GetUserSmallDatasFromDir"},
	{MENU_USB_GET_USRSMALL_INFOEX_FROMDIR,	"BS2_GetUserSmallInfosExFromDir"},
	{MENU_USB_GET_USRSMALL_DATAEX_FROMDIR,	"BS2_GetUserSmallDatasExFromDir"},
	{MENU_USB_GET_USRFACEEX_INFO_FROMDIR,	"BS2_GetUserFaceExInfosFromDir"},
	{MENU_USB_GET_USRFACEEX_DATA_FROMDIR,	"BS2_GetUserFaceExDatasFromDir"},

	{MENU_USB_GET_FILTEREDLOG_FROMDIR,		"BS2_GetFilteredLogFromDir"},
	{MENU_USB_GET_LOG_FROMDIR,				"BS2_GetLogFromDir"},
	{MENU_USB_GET_LOG_BLOB_FROMDIR,			"BS2_GetLogBlobFromDir"},
	{MENU_USB_GET_LOG_SMALLBLOB_FROMDIR,	"BS2_GetLogSmallBlobFromDir",},
	{MENU_USB_GET_LOG_SMALLBLOBEX_FROMDIR,	"BS2_GetLogSmallBlobExFromDir",},
};


int setSSLHandler();
bool getDeviceLogs(BS2_DEVICE_ID id, int& timezone);
void connectTestDevice(void* context, DeviceList& deviceList);
uint32_t showMenu(std::vector<MENU_ITEM>& info);
uint32_t getSelectedIndex();
int searchAndConnect(void* context, DeviceList& deviceList);
int connectViaIP(void* context, DeviceList& deviceList);
int slaveMenu(void* context, DeviceList& deviceList);
int searchAndAddSlave(void* context, DeviceList& deviceList);
int connectWiegand(void* context, DeviceList& deviceList);
int searchSlave(void* context, DeviceList& deviceList, BS2_DEVICE_ID& masterID);
int searchCSTSlave(void* context, DeviceList& deviceList, BS2_DEVICE_ID& masterID);
int searchWiegand(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& wiegandID);
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
int enrollUserFaceEx_2_CS40(void* context, const DeviceList& deviceList);
int updateConnectModeDevice2Server(void* context, BS2_DEVICE_ID id);
int updateConnectModeServer2Device(void* context, BS2_DEVICE_ID id);
int getFactoryConfig(void* context, BS2_DEVICE_ID id);
int getFactoryConfigMulti(void* context, const std::vector<BS2_DEVICE_ID>& devices);
int getSlaveConnectionStatus(void* context, BS2_DEVICE_ID id);
int getStatusConfig(void* context, BS2_DEVICE_ID id);
int getStatusConfigMulti(void* context, const std::vector<BS2_DEVICE_ID>& devices);
int setStatusConfig(void* context, BS2_DEVICE_ID id);
int setStatusConfigValue(void* context, BS2_DEVICE_ID id, int value);
int setStatusConfigMulti(void* context, const std::vector<BS2_DEVICE_ID>& devices);
#if 0
DWORD WINAPI onWaiting(LPVOID lpParam);
void waitForConnection();
#endif
int enrollMultipleUsers(void* context, const DeviceList& devices);
int getAllAccessSchedule(void* context, BS2_DEVICE_ID id);
#if TEST_CODE
int connectTestDevice2(void* context);
int connectTestDevice3(void* context);
#endif
int runUSBAPIs();
int getUserDatabaseInfoFromDir();
int getUserListFromDir();
int getUserInfosFromDir(int menuItem);
int getUserDatasFromDir(int menuItem);
int getFilteredLogFromDir();
int getLogFromDir();
int getLogBlobFromDir();
int getLogSmallBlobFromDir();
int getLogSmallBlobExFromDir();
