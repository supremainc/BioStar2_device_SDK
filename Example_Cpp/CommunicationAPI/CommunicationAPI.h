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


enum EN_MENU_TOP
{
	MENU_TOP_BREAK = MENU_BREAK,
	MENU_TOP_SEARCH_N_CONN = 1,
	MENU_TOP_DIRECT_IPADDR,
	MENU_TOP_SEARCH_SLAVE,
	MENU_TOP_OSDP_STANDARD,
	MENU_TOP_SEARCH_WIEGAND,
	MENU_TOP_VIEW_DEVICE,
	MENU_TOP_CONNECT_USB,
};

std::vector<MENU_ITEM> menuInfoTop =
{
	{MENU_TOP_BREAK,			"Exit and API test"},
	{MENU_SEPARATOR,			""},
	{MENU_TOP_SEARCH_N_CONN,	"Connection (Discover and connect)"},
	{MENU_TOP_DIRECT_IPADDR,	"Connection (Direct connect with IP/Port)"},
	{MENU_SEPARATOR,			""},
	{MENU_TOP_SEARCH_SLAVE,		"Slave (Discover and add)"},
	{MENU_TOP_OSDP_STANDARD,	"OSDP device control"},
	{MENU_TOP_SEARCH_WIEGAND,	"Wiegand (Discover and add)" },
	{MENU_SEPARATOR,			""},
	{MENU_TOP_VIEW_DEVICE,		"View (Connected devices)"},
	{MENU_SEPARATOR,			""},
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
	{MENU_SEPARATOR,				""},
	{MENU_SLV_GET_CONFIG_RS485EX,	"Connected status (of slave devices)"},
	{MENU_SEPARATOR,				""},
	{MENU_SLV_SEARCH_DEVICE,		"Discover and add (slave devices)"},
	{MENU_SEPARATOR,				""},
	{MENU_SLV_UPG_FIRMWARE,			"Upgrade firmware"},
	{MENU_SEPARATOR,				""},
	{MENU_SLV_GET_CONFIG_FACTORY,	"Get config (Factory)"},
	{MENU_SLV_GET_CONFIG_STATUS,	"Get config (Status)"},
	{MENU_SLV_SET_CONFIG_STATUS,	"Set config (Status)"},
};

enum EN_MENU_OSDP
{
	MENU_OSDP_BREAK = MENU_BREAK,
	MENU_OSDP_GET_OSDPSTANDARDCONFIG,
	MENU_OSDP_GET_OSDPSTANDARDACTIONCONFIG,
	MENU_OSDP_SET_OSDPSTANDARDACTIONCONFIG,
	MENU_OSDP_GET_AVAILABLE_DEVICE,
	MENU_OSDP_GET_DEVICE,
	MENU_OSDP_ADD_DEVICE,
	MENU_OSDP_UPD_DEVICE,
	MENU_OSDP_REM_DEVICE,
	MENU_OSDP_GET_CAPABILITY,
	MENU_OSDP_SET_SECURITYKEY,
	MENU_OSDP_START_STATUS_MONITOR,
	MENU_OSDP_STOP_STATUS_MONITOR,
};

std::vector<MENU_ITEM> menuInfoOsdp =
{
	{MENU_SLV_BREAK,							"Exit" },
	{MENU_SEPARATOR,							"" },
	{MENU_OSDP_GET_OSDPSTANDARDCONFIG,			"Get OsdpStandardConfig"},
	{MENU_OSDP_GET_OSDPSTANDARDACTIONCONFIG,	"Get OsdpStandardActionConfig"},
	{MENU_OSDP_SET_OSDPSTANDARDACTIONCONFIG,	"Set OsdpStandardActionConfig"},
	{MENU_SEPARATOR,							"" },
	{MENU_OSDP_GET_AVAILABLE_DEVICE,			"Get available device"},
	{MENU_OSDP_GET_DEVICE,						"Get device"},
	{MENU_OSDP_ADD_DEVICE,						"Add device"},
	{MENU_OSDP_UPD_DEVICE,						"Update devices"},
	{MENU_OSDP_REM_DEVICE,						"Remove devices"},
	{MENU_OSDP_GET_CAPABILITY,					"Get capability"},
	{MENU_OSDP_SET_SECURITYKEY,					"Set security key"},
	{MENU_SEPARATOR,							""},
	{MENU_OSDP_START_STATUS_MONITOR,			"Start status monitor"},
	{MENU_OSDP_STOP_STATUS_MONITOR,				"Stop status monitor"},
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

	MENU_COMM_GET_SOCKET_RETRY_COUNT,
	MENU_COMM_SET_SOCKET_RETRY_COUNT,
	MENU_COMM_GET_SOCKETSSL_RETRY_COUNT,
	MENU_COMM_SET_SOCKETSSL_RETRY_COUNT,

	MENU_CONF_GET_FACCONFIG,

	MENU_ELOG_GET_EVENTSMALLBLOB,
	MENU_ELOG_GET_EVENTSMALLBLOBEX,

	MENU_USER_ENROLL_FACE,
	MENU_USER_ENROLL_FACEEX,
	MENU_USER_ENROLL_MULTIPLE,

	MENU_CONF_UPD_DEVICE_2_SERVER,
	MENU_CONF_UPD_SERVER_2_DEVICE,

	MENU_COMM_SET_DEVICE_LICENSE,
	MENU_COMM_DEL_DEVICE_LICENSE,
	MENU_COMM_GET_DEVICE_LICENSE,
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
	{MENU_SEPARATOR,						""},

	{MENU_COMM_SET_KEEP_ALIVE,				"BS2_SetKeepAliveTimeout"},

	{MENU_COMM_GET_SERVER_PORT,				"BS2_GetServerPort"},
	{MENU_COMM_SET_SERVER_PORT,				"BS2_SetServerPort"},

	{MENU_COMM_GET_SERVER_SSLPORT,			"BS2_GetSSLServerPort"},
	{MENU_COMM_SET_SERVER_SSLPORT,			"BS2_SetSSLServerPort"},

	{MENU_COMM_IS_CONNECTED,				"BS2_IsConnected"},

	{MENU_COMM_IS_AUTO_CONNECT,				"BS2_IsAutoConnection"},
	{MENU_COMM_SET_AUTO_CONNECT,			"BS2_SetAutoConnection"},
	{MENU_SEPARATOR,						""},

	{MENU_COMM_GET_ENABLEIPV4,				"BS2_GetEnableIPV4"},
	{MENU_COMM_SET_ENABLEIPV4,				"BS2_SetEnableIPV4"},

	{MENU_COMM_GET_ENABLEIPV6,				"BS2_GetEnableIPV6"},
	{MENU_COMM_SET_ENABLEIPV6,				"BS2_SetEnableIPV6"},

	{MENU_COMM_GET_SERVERPORTV6,			"BS2_GetServerPortIPV6"},
	{MENU_COMM_SET_SERVERPORTV6,			"BS2_SetServerPortIPV6"},

	{MENU_COMM_GET_SERVERSSLPORTV6,			"BS2_GetSSLServerPortIPV6"},
	{MENU_COMM_SET_SERVERSSLPORTV6,			"BS2_SetSSLServerPortIPV6"},
	{MENU_SEPARATOR,						""},

	{MENU_COMM_GET_DEF_RES_TIMEOUT,			"BS2_GetDefaultResponseTimeout"},
	{MENU_COMM_SET_DEF_RES_TIMEOUT,			"BS2_SetDefaultResponseTimeout"},

	{MENU_COMM_GET_SOCKET_RETRY_COUNT,		"BS2_GetSocketRetryCount"},
	{MENU_COMM_SET_SOCKET_RETRY_COUNT,		"BS2_SetSocketRetryCount"},
	{MENU_COMM_GET_SOCKETSSL_RETRY_COUNT,	"BS2_GetSocketSSLRetryCount"},
	{MENU_COMM_SET_SOCKETSSL_RETRY_COUNT,	"BS2_SetSocketSSLRetryCount"},
	{MENU_SEPARATOR,						""},

	{MENU_CONF_GET_FACCONFIG,				"BS2_GetFactoryConfig" },
	{MENU_SEPARATOR,						""},

	{MENU_ELOG_GET_EVENTSMALLBLOB,			"BS2_GetEventSmallBlob"},
	{MENU_ELOG_GET_EVENTSMALLBLOBEX,		"BS2_GetEventSmallBlobEx"},
	{MENU_SEPARATOR,						""},

	{MENU_USER_ENROLL_FACE,					"BS2_EnrolUser"},
	{MENU_USER_ENROLL_FACEEX,				"BS2_EnrollUserFaceEx to CS40"},
	{MENU_USER_ENROLL_MULTIPLE,				"Enroll multiple users"},
	{MENU_SEPARATOR,						""},

	{MENU_CONF_UPD_DEVICE_2_SERVER,			"Update device to server connection"},
	{MENU_CONF_UPD_SERVER_2_DEVICE,			"Update server to device connection"},
	{MENU_SEPARATOR,						""},

	{MENU_COMM_SET_DEVICE_LICENSE,			"BS2_EnableDeviceLicense"},
	{MENU_COMM_DEL_DEVICE_LICENSE,			"BS2_DisableDeviceLicense"},
	{MENU_COMM_GET_DEVICE_LICENSE,			"BS2_QueryDeviceLicense"},
};

std::vector<MENU_ITEM> menuInfoUSBAPI =
{
	{MENU_COMM_BREAK,						"Exit"},
	{MENU_SEPARATOR,						""},

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
	{MENU_SEPARATOR,						""},

	{MENU_USB_GET_FILTEREDLOG_FROMDIR,		"BS2_GetFilteredLogFromDir"},
	{MENU_USB_GET_LOG_FROMDIR,				"BS2_GetLogFromDir"},
	{MENU_USB_GET_LOG_BLOB_FROMDIR,			"BS2_GetLogBlobFromDir"},
	{MENU_USB_GET_LOG_SMALLBLOB_FROMDIR,	"BS2_GetLogSmallBlobFromDir",},
	{MENU_USB_GET_LOG_SMALLBLOBEX_FROMDIR,	"BS2_GetLogSmallBlobExFromDir",},
};


int setSSLHandler();
bool getDeviceLogs(BS2_DEVICE_ID id, int& timezone);
void connectTestDevice(void* context, DeviceList& deviceList);
int slaveMenu(void* context, DeviceList& deviceList);
int osdpMenu(void* context, DeviceList& deviceList);
int runAPIs(void* context, const DeviceList& deviceList);
int enrollUserFaceEx_2_CS40(void* context, const DeviceList& deviceList);
int updateConnectModeDevice2Server(void* context, BS2_DEVICE_ID id);
int updateConnectModeServer2Device(void* context, BS2_DEVICE_ID id);
int getSocketRetryCount(void* context);
int setSocketRetryCount(void* context);
int getSocketSSLRetryCount(void* context);
int setSocketSSLRetryCount(void* context);
int getFactoryConfig(void* context, BS2_DEVICE_ID id);
int getFactoryConfigMulti(void* context, const std::vector<BS2_DEVICE_ID>& devices);
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
int setDeviceLicense(void* context, BS2_DEVICE_ID id);
int deleteDeviceLicense(void* context, BS2_DEVICE_ID id);
int getDeviceLicense(void* context, BS2_DEVICE_ID id);
int getAvailableOsdpStandardDevice(void* context, DeviceList& deviceList);
int getOsdpStandardConfig(void* context, const DeviceList& deviceList);
int getOsdpStandardActionConfig(void* context, const DeviceList& deviceList);
int setOsdpStandardActionConfig(void* context, const DeviceList& deviceList);
int getOsdpStandardDevice(void* context, DeviceList& deviceList);
int addOsdpStandardDevice(void* context, DeviceList& deviceList);
int updateOsdpStandardDevice(void* context, DeviceList& deviceList);
int removeOsdpStandardDevice(void* context, DeviceList& deviceList);
int getOsdpStandardDeviceCapability(void* context, DeviceList& deviceList);
int setOsdpStandardDeviceSecurityKey(void* context, DeviceList& deviceList);
void onOsdpStandardDeviceStatusChanged(BS2_DEVICE_ID deviceId, const BS2OsdpStandardDeviceNotify* notifyData);
int startMonitorOsdpStandardDeviceStatus(void* context);
int stopMonitorOsdpStandardDeviceStatus(void* context);
