#include "stdafx.h"
#include <sstream>
#include "CommunicationAPI.h"
#include "../Common/Utility.h"
#include "../Common/CommControl.h"
#include "../Common/LogControl.h"
#include "../Common/UserControl.h"
#include "../Common/AccessControl.h"
#include "../Common/DeviceControl.h"
#include "../Common/BS2UsbContext.h"
#include <memory>

//std::shared_ptr<int> ptr;

extern void TRACE(const char* fmt, ...);
using namespace std;

static void* sdkContext = NULL;
static DeviceList deviceList;


void onLogReceived(BS2_DEVICE_ID id, const BS2Event* event)
{
	int32_t timezone = deviceList.getTimezone(id);
	cout << LogControl::getEventString(id, *event, timezone) << endl;
}

// Thermal supported callback
void onLogReceivedEx(BS2_DEVICE_ID id, const BS2Event* event, BS2_TEMPERATURE temperature)
{
	int32_t timezone = deviceList.getTimezone(id);
	cout << LogControl::getEventStringWithThermal(id, *event, timezone, temperature) << endl;
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
		BS2SimpleDeviceInfo info;
		memset(&info, 0x0, sizeof(info));
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

bool getDeviceLogs(BS2_DEVICE_ID id, int& timezone)
{
	ConfigControl cc(sdkContext);

	if (deviceList.findDevice(id))
		timezone = deviceList.getTimezone(id);
	else
		cc.getTimezone(id, timezone);

	int sdkResult = Utility::getAllLogsFromDevice(sdkContext, id, timezone);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("An error occurred while receiving bulk logs from device: %d", sdkResult);
		BS2_DisconnectDevice(sdkContext, id);
		return false;
	}

	return true;
}

int main()
{
	// Set debugging SDK log (to current working directory)
	BS2Context::setDebugFileLog(DEBUG_LOG_ALL, DEBUG_MODULE_ALL, ".", 100);
	//BS2Context::setDebugCallbackLog(DEBUG_LOG_ALL, DEBUG_MODULE_ALL);

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

	BS2Context::getInstance()->setDeviceEventListener(onDeviceAccepted, onDeviceConnected, onDeviceDisconnected);

	connectTestDevice(sdkContext, deviceList);

	BS2Context::getInstance()->releaseInstance();
	return 0;
}

#if TEST_CODE
int main(int argc, char* argv[])
{
	// Set debugging SDK log (to current working directory)
	BS2Context::setDebugFileLog(DEBUG_LOG_ALL, DEBUG_MODULE_ALL, ".");

	TRACE("Version: %s", BS2_Version());

	const int RETRY_TIMES = 1000;
	for (int idx = 0; idx < RETRY_TIMES; idx++)
	{
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

		BS2Context::getInstance()->setDeviceEventListener(onDeviceAccepted, NULL, onDeviceDisconnected);

		connectTestDevice2(sdkContext);
		//connectTestDevice3(sdkContext);

		BS2Context::getInstance()->releaseInstance();

		//Sleep(30 * 1000);
		TRACE("Call finished %d times", idx + 1);
	}
	return 0;
}
#endif

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
		case MENU_TOP_SEARCH_SLAVE:
			sdkResult = slaveMenu(context, deviceList);
			break;
		case MENU_TOP_OSDP_STANDARD:
			sdkResult = osdpMenu(context, deviceList);
			break;
		case MENU_TOP_SEARCH_WIEGAND:
			sdkResult = Utility::connectWiegand(context, deviceList);
			break;
		case MENU_TOP_VIEW_DEVICE:
			Utility::displayConnectedDevices(deviceList, true, true);
			break;
		case MENU_TOP_CONNECT_USB:
			runUSBAPIs();
			return;

		default:
			break;
		}
	}

	if (BS_SDK_SUCCESS != sdkResult)
		return;

	runAPIs(context, deviceList);
}

int slaveMenu(void* context, DeviceList& deviceList)
{
	DeviceControl dc(context);
	BS2_DEVICE_ID masterID(0);
	BS2_DEVICE_TYPE masterType(0);
	int sdkResult = BS_SDK_SUCCESS;
	vector<BS2_DEVICE_ID> selectedIDs;
	bool selectedOrder = false;

	bool menuBreak = false;
	while (!menuBreak)
	{
		uint32_t selected = Utility::showMenu(menuInfoSlave);
		switch (selected)
		{
		case MENU_SLV_BREAK:
			menuBreak = true;
			break;
		case MENU_SLV_GET_CONFIG_RS485EX:
			if (Utility::selectDeviceIDAndType(deviceList, false, masterID, masterType))
			{
				sdkResult = Utility::getSlaveConnectionStatus(context, masterID, masterType);
			}
			break;
		case MENU_SLV_SEARCH_DEVICE:
			sdkResult = Utility::searchAndAddSlave(context, deviceList);
			break;
		case MENU_SLV_UPG_FIRMWARE:
			if (!selectedOrder)
			{
				Utility::selectDeviceIDs(deviceList, masterID, selectedIDs, true, false);
				selectedOrder = true;
			}
			sdkResult = dc.upgradeFirmware(selectedIDs);
			break;
		case MENU_SLV_GET_CONFIG_FACTORY:
			if (!selectedOrder)
			{
				Utility::selectDeviceIDs(deviceList, masterID, selectedIDs, true, false);
				selectedOrder = true;
			}
			sdkResult = getFactoryConfigMulti(context, selectedIDs);
			break;
		case MENU_SLV_GET_CONFIG_STATUS:
			if (!selectedOrder)
			{
				Utility::selectDeviceIDs(deviceList, masterID, selectedIDs, true, false);
				selectedOrder = true;
			}
			sdkResult = getStatusConfigMulti(context, selectedIDs);
			break;
		case MENU_SLV_SET_CONFIG_STATUS:
			if (!selectedOrder)
			{
				Utility::selectDeviceIDs(deviceList, masterID, selectedIDs, true, false);
				selectedOrder = true;
			}
			sdkResult = setStatusConfigMulti(context, selectedIDs);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}

int osdpMenu(void* context, DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;

	bool menuBreak = false;
	while (!menuBreak)
	{
		uint32_t selected = Utility::showMenu(menuInfoOsdp);
		switch (selected)
		{
		case MENU_OSDP_BREAK:
			menuBreak = true;
			break;
		case MENU_OSDP_GET_OSDPSTANDARDCONFIG:
			sdkResult = getOsdpStandardConfig(context, deviceList);
			break;
		case MENU_OSDP_GET_OSDPSTANDARDACTIONCONFIG:
			sdkResult = getOsdpStandardActionConfig(context, deviceList);
			break;
		case MENU_OSDP_SET_OSDPSTANDARDACTIONCONFIG:
			sdkResult = setOsdpStandardActionConfig(context, deviceList);
			break;
		case MENU_OSDP_GET_AVAILABLE_DEVICE:
			sdkResult = getAvailableOsdpStandardDevice(context, deviceList);
			break;
		case MENU_OSDP_GET_DEVICE:
			sdkResult = getOsdpStandardDevice(context, deviceList);
			break;
		case MENU_OSDP_ADD_DEVICE:
			sdkResult = addOsdpStandardDevice(context, deviceList);
			break;
		case MENU_OSDP_UPD_DEVICE:
			sdkResult = updateOsdpStandardDevice(context, deviceList);
			break;
		case MENU_OSDP_REM_DEVICE:
			sdkResult = removeOsdpStandardDevice(context, deviceList);
			break;
		case MENU_OSDP_GET_CAPABILITY:
			sdkResult = getOsdpStandardDeviceCapability(context, deviceList);
			break;
		case MENU_OSDP_SET_SECURITYKEY:
			sdkResult = setOsdpStandardDeviceSecurityKey(context, deviceList);
			break;
		case MENU_OSDP_START_STATUS_MONITOR:
			sdkResult = startMonitorOsdpStandardDeviceStatus(context);
			break;
		case  MENU_OSDP_STOP_STATUS_MONITOR:
			sdkResult = stopMonitorOsdpStandardDeviceStatus(context);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}

int runAPIs(void* context, const DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);
	CommControl cm(context);
	ConfigControl cc(context);
	LogControl lc(context);
	UserControl uc(context);
	DeviceControl dc(context);

	cout << endl << endl << "== CommunicationAPI Test ==" << endl;
	BS2_DEVICE_ID id = 0;

	while (/*BS_SDK_SUCCESS == sdkResult && */MENU_COMM_BREAK != (selectedTop = Utility::showMenu(menuInfoCommAPI)))
	{
		switch (selectedTop)
		{
		case MENU_COMM_BREAK:
			return BS_SDK_SUCCESS;
		case MENU_COMM_SET_KEEP_ALIVE:
			sdkResult = cm.setKeepAliveTimeout();
			break;
		case MENU_COMM_GET_SERVER_PORT:
			sdkResult = cm.getServerPort();
			break;
		case MENU_COMM_SET_SERVER_PORT:
			sdkResult = cm.setServerPort();
			break;
		case MENU_COMM_GET_SERVER_SSLPORT:
			sdkResult = cm.getSSLServerPort();
			break;
		case MENU_COMM_SET_SERVER_SSLPORT:
			sdkResult = cm.setSSLServerPort();
			break;
		case MENU_COMM_IS_CONNECTED:
			id = Utility::selectDeviceID(deviceList);
			sdkResult = cm.isConnected(id);
			break;
		case MENU_COMM_IS_AUTO_CONNECT:
			sdkResult = cm.isAutoConnection();
			break;
		case MENU_COMM_SET_AUTO_CONNECT:
			sdkResult = cm.setAutoConnection();
			break;
		case MENU_COMM_GET_ENABLEIPV4:
			sdkResult = cm.getEnableIPV4();
			break;
		case MENU_COMM_SET_ENABLEIPV4:
			sdkResult = cm.setEnableIPV4();
			break;
		case MENU_COMM_GET_ENABLEIPV6:
			sdkResult = cm.getEnableIPV6();
			break;
		case MENU_COMM_SET_ENABLEIPV6:
			sdkResult = cm.setEnableIPV6();
			break;
		case MENU_COMM_GET_SERVERPORTV6:
			sdkResult = cm.getServerPortIPV6();
			break;
		case MENU_COMM_SET_SERVERPORTV6:
			sdkResult = cm.setServerPortIPV6();
			break;
		case MENU_COMM_GET_SERVERSSLPORTV6:
			sdkResult = cm.getSSLServerPortIPV6();
			break;
		case MENU_COMM_SET_SERVERSSLPORTV6:
			sdkResult = cm.setSSLServerPortIPV6();
			break;
		case MENU_COMM_GET_DEF_RES_TIMEOUT:
			sdkResult = cm.getDefaultResponseTimeout();
			break;
		case MENU_COMM_SET_DEF_RES_TIMEOUT:
			sdkResult = cm.setDefaultResponseTimeout();
			break;
		case MENU_COMM_GET_SOCKET_RETRY_COUNT:
			sdkResult = getSocketRetryCount(context);
			break;
		case MENU_COMM_SET_SOCKET_RETRY_COUNT:
			sdkResult = setSocketRetryCount(context);
			break;
		case MENU_COMM_GET_SOCKETSSL_RETRY_COUNT:
			sdkResult = getSocketSSLRetryCount(context);
			break;
		case MENU_COMM_SET_SOCKETSSL_RETRY_COUNT:
			sdkResult = setSocketSSLRetryCount(context);
			break;
		case MENU_COMM_DEL_SSL:
			id = Utility::selectDeviceID(deviceList);
			sdkResult = cm.disableSSL(id);
			break;
		case MENU_COMM_UPG_FIRMWARE:
			id = Utility::selectDeviceID(deviceList, true, false);
			sdkResult = dc.upgradeFirmware(id);
			break;
		case MENU_CONF_GET_FACCONFIG:
			id = Utility::selectDeviceID(deviceList, true, false);
			sdkResult = getFactoryConfig(context, id);
			break;
		case MENU_ELOG_GET_EVENTSMALLBLOB:
			id = Utility::selectDeviceID(deviceList, true, false);
			sdkResult = lc.getLogSmallBlob(id);
			break;
		case MENU_ELOG_GET_EVENTSMALLBLOBEX:
			id = Utility::selectDeviceID(deviceList, true, false);
			sdkResult = lc.getLogSmallBlobEx(id);
			break;
		case MENU_USER_ENROLL_FACE:
			id = Utility::selectDeviceID(deviceList, true, false);
			sdkResult = uc.enrollUser(id);
			break;
		case MENU_USER_ENROLL_FACEEX:
			sdkResult = enrollUserFaceEx_2_CS40(context, deviceList);
			break;
		case MENU_CONF_UPD_DEVICE_2_SERVER:
			id = Utility::selectDeviceID(deviceList, false, false);
			sdkResult = updateConnectModeDevice2Server(context, id);
			break;
		case MENU_CONF_UPD_SERVER_2_DEVICE:
			id = Utility::selectDeviceID(deviceList, false, false);
			sdkResult = updateConnectModeServer2Device(context, id);
			break;
		case MENU_USER_ENROLL_MULTIPLE:
			sdkResult = enrollMultipleUsers(context, deviceList);
			break;
		case MENU_COMM_SET_DEVICE_LICENSE:
			id = Utility::selectDeviceID(deviceList, true, false);
			sdkResult = setDeviceLicense(context, id);
			break;
		case MENU_COMM_DEL_DEVICE_LICENSE:
			id = Utility::selectDeviceID(deviceList, true, false);
			sdkResult = deleteDeviceLicense(context, id);
			break;
		case MENU_COMM_GET_DEVICE_LICENSE:
			id = Utility::selectDeviceID(deviceList, true, false);
			sdkResult = getDeviceLicense(context, id);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}

int enrollUserFaceEx_2_CS40(void* context, const DeviceList& deviceList)
{
	const string fnFaceTemplate = "C:\\Temp\\user01.dat";
	BS2Face face, faceTemp;
	memset(&face, 0x0, sizeof(face));
	memset(&faceTemp, 0x0, sizeof(faceTemp));
	uint8_t numOfScanFace(0);
	UserControl uc(context);
	int sdkResult = BS_SDK_SUCCESS;
	FILE* fp = NULL;

	bool isWrite = Utility::isYes("Do you want to write face template to file?");
	if (isWrite)
	{
		Utility::displayConnectedDevices(deviceList, true);
		BS2_DEVICE_ID scanDeviceID = Utility::getInput<BS2_DEVICE_ID>("Please enter the DEVICE ID for the scan face:");
		sdkResult = uc.scanFace(scanDeviceID, &faceTemp, numOfScanFace);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("Face scan failed from device: %u, error: %d", scanDeviceID, sdkResult);
			return sdkResult;
		}

		fp = fopen(fnFaceTemplate.c_str(), "wb");
		if (fp)
		{
			size_t written = fwrite(&faceTemp, sizeof(BS2Face), 1, fp);
			TRACE("Template: %u, Written: %u", sizeof(BS2Face), written);
		}

		fclose(fp);
	}

	fp = fopen(fnFaceTemplate.c_str(), "rb");
	if (fp)
	{
		size_t readData = fread(&face, sizeof(BS2Face), 1, fp);
		TRACE("Template: %u, Read: %u", sizeof(BS2Face), readData);
	}

	fclose(fp);

	if (isWrite && (memcmp(&face, &faceTemp, sizeof(BS2Face)) != 0))
	{
		TRACE("Different template read and written");
		return BS_SDK_ERROR_INTERNAL;
	}

	Utility::displayConnectedDevices(deviceList, true);
	BS2_DEVICE_ID enrollDeviceID = Utility::getInput<BS2_DEVICE_ID>("Please enter the DEVICE ID for the enroll:");

	sdkResult = uc.enrollUserFaceEx(enrollDeviceID, NULL, NULL, &face, NULL);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("Face enroll failed to device: %u, error: %d", enrollDeviceID, sdkResult);
	
	return sdkResult;
}

int updateConnectModeDevice2Server(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	string msg;

	msg = "Insert server IP";
	string ip = Utility::getInput<string>(msg);

	msg = "Insert server port";
	BS2_PORT port = (BS2_PORT)Utility::getInput<uint32_t>(msg);

	return cc.updateConnectModeDevice2Server(id, ip, port);
}

int updateConnectModeServer2Device(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);

	return cc.updateConnectModeServer2Device(id);
}

int getSocketRetryCount(void* context)
{
	CommControl cc(context);

	uint32_t count(0);
	int sdkResult = cc.getSocketRetryCount(count);
	if (BS_SDK_SUCCESS == sdkResult)
		cout << "Socket retry count:" << count << endl;

	return sdkResult;
}

int setSocketRetryCount(void* context)
{
	CommControl cc(context);

	uint32_t count = Utility::getInput<uint32_t>("Enter the number of retries.");
	return cc.setSocketRetryCount(count);
}

int getSocketSSLRetryCount(void* context)
{
	CommControl cc(context);

	uint32_t count(0);
	int sdkResult = cc.getSocketSSLRetryCount(count);
	if (BS_SDK_SUCCESS == sdkResult)
		cout << "SocketSSL retry count:" << count << endl;

	return sdkResult;
}

int setSocketSSLRetryCount(void* context)
{
	CommControl cc(context);

	uint32_t count = Utility::getInput<uint32_t>("Enter the number of retries.");
	return cc.setSocketSSLRetryCount(count);
}

int getFactoryConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2FactoryConfig config;
	memset(&config, 0x0, sizeof(config));

	int sdkResult = cc.getFactoryConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int getFactoryConfigMulti(void* context, const vector<BS2_DEVICE_ID>& devices)
{
	int sdkResult = BS_SDK_SUCCESS;

	for (auto id : devices)
	{
		sdkResult = getFactoryConfig(context, id);
	}

	return sdkResult;
}

// int getRS485ExConfig(void* context, const vector<BS2_DEVICE_ID>& devices)
// {
// 	ConfigControl cc(context);
// 	int sdkResult = BS_SDK_SUCCESS;

// 	for (auto id : devices)
// 	{
// 		BS2Rs485ConfigEX config;
//		memset(&config, 0x0, sizeof(config));
// 		sdkResult = cc.getRS485ConfigEx(id, config);
// 		if (BS_SDK_SUCCESS == sdkResult)
// 			ConfigControl::print(config);
// 	}

// 	return sdkResult;
// }

int getStatusConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2StatusConfig config;
	memset(&config, 0x0, sizeof(config));

	int sdkResult = cc.getStatusConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);
	
	return sdkResult;
}

int getStatusConfigMulti(void* context, const vector<BS2_DEVICE_ID>& devices)
{
	int sdkResult = BS_SDK_SUCCESS;

	for (auto id : devices)
	{
		sdkResult = getStatusConfig(context, id);
	}

	return sdkResult;
}

int setStatusConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2StatusConfig config;
	memset(&config, 0x0, sizeof(config));

	int sdkResult = cc.getStatusConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	for (uint32_t idx = 0; idx < BS2_DEVICE_STATUS_NUM; idx++)
	{
		cout << "LED" << idx << " - enabled:" << config.led[idx].enabled << ", count:" << config.led[idx].count << endl;
		ostringstream oss;
		oss << "Do you want to enable LED #" << idx << "/" << BS2_DEVICE_STATUS_NUM << "?" << endl;
		config.led[idx].enabled = (BS2_BOOL)Utility::getInput<uint32_t>(oss.str());
		if (config.led[idx].enabled)
		{
			string msg = "How many times do you want to repeat the LED signal? (0: Infinite)";
			config.led[idx].count = (uint16_t)Utility::getInput<uint32_t>(msg);

			for (uint32_t lidx = 0; lidx < BS2_LED_SIGNAL_NUM; lidx++)
			{
				if (Utility::isNo("Add LED signal?"))
					break;

				oss.str("");
				oss << "Please select the LED color." << endl;
				oss << " - 0: Off" << endl;
				oss << " - 1: Red" << endl;
				oss << " - 2: Yellow" << endl;
				oss << " - 3: Green" << endl;
				oss << " - 4: Cyan" << endl;
				oss << " - 5: Blue" << endl;
				oss << " - 6: Magenta" << endl;
				oss << " - 7: White" << endl;
				config.led[idx].signal[lidx].color = (uint8_t)Utility::getInput<uint32_t>(oss.str());

				msg = "Please insert the LED duration.";
				config.led[idx].signal[lidx].duration = (uint16_t)Utility::getInput<uint32_t>(msg);

				msg = "Please insert the LED delay.";
				config.led[idx].signal[lidx].delay = (uint16_t)Utility::getInput<uint32_t>(msg);
			}
		}
	}
	
	for (uint32_t idx = 0; idx < BS2_DEVICE_STATUS_NUM; idx++)
	{
		cout << "Buzzer" << idx << " - enabled:" << config.buzzer[idx].enabled << ", count:" << config.buzzer[idx].count << endl;
		ostringstream oss;
		oss << "Do you want to enable BUZZER #" << idx << "/" << BS2_DEVICE_STATUS_NUM << "?" << endl;
		config.buzzer[idx].enabled = (BS2_BOOL)Utility::getInput<uint32_t>(oss.str());
		if (config.buzzer[idx].enabled)
		{
			string msg = "How many times do you want to repeat the BUZZER signal? (0: Infinite)";
			config.buzzer[idx].count = (uint16_t)Utility::getInput<uint32_t>(msg);

			for (uint32_t bidx = 0; bidx < BS2_BUZZER_SIGNAL_NUM; bidx++)
			{
				if (Utility::isNo("Add BUZZER signal?"))
					break;

				oss.str("");
				oss << "Please select the BUZZER tone." << endl;
				oss << " - 0: Off" << endl;
				oss << " - 1: Low" << endl;
				oss << " - 2: Middle" << endl;
				oss << " - 3: High" << endl;
				config.buzzer[idx].signal[bidx].tone = (BS2_BUZZER_TONE)Utility::getInput<uint32_t>(oss.str());

				msg = "Do you want to use fadeout option?";
				config.buzzer[idx].signal[bidx].fadeout = (BS2_BOOL)Utility::getInput<uint32_t>(msg);

				msg = "Please insert the BUZZER duration.";
				config.buzzer[idx].signal[bidx].duration = (uint16_t)Utility::getInput<uint32_t>(msg);

				msg = "Please insert the BUZZER delay.";
				config.buzzer[idx].signal[bidx].delay = (uint16_t)Utility::getInput<uint32_t>(msg);
			}
		}
	}

	return cc.setStatusConfig(id, config);
}

int setStatusConfigValue(void* context, BS2_DEVICE_ID id, int value)
{
	ConfigControl cc(context);
	BS2StatusConfig config;
	memset(&config, 0x0, sizeof(config));

	int sdkResult = cc.getStatusConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.led[0].enabled = 1;
	config.led[0].count = value;
	for (uint32_t lidx = 0; lidx < BS2_LED_SIGNAL_NUM; lidx++)
	{
		config.led[0].signal[lidx].color = (BS2_LED_COLOR)value;
		config.led[0].signal[lidx].duration = (uint16_t)value;
		config.led[0].signal[lidx].delay = (uint16_t)value;
	}
	for (uint32_t idx = 1; idx < BS2_DEVICE_STATUS_NUM; idx++)
	{
		config.led[idx].enabled = 0;
		config.led[idx].count = 0;
	}

	config.buzzer[0].enabled = 1;
	config.buzzer[0].count = value;
	for (uint32_t bidx = 0; bidx < BS2_BUZZER_SIGNAL_NUM; bidx++)
	{
		config.buzzer[0].signal[bidx].tone = (BS2_BUZZER_TONE)value;
		config.buzzer[0].signal[bidx].fadeout = (BS2_BOOL)value;
		config.buzzer[0].signal[bidx].duration = (uint16_t)value;
		config.buzzer[0].signal[bidx].delay = (uint16_t)value;
	}
	for (uint32_t idx = 0; idx < BS2_DEVICE_STATUS_NUM; idx++)
	{
		config.buzzer[idx].enabled = 0;
		config.buzzer[idx].count = 0;
	}

	return cc.setStatusConfig(id, config);
}

int setStatusConfigMulti(void* context, const vector<BS2_DEVICE_ID>& devices)
{
	int sdkResult = BS_SDK_SUCCESS;

#ifdef _FOR_TEST
	for (auto id : devices)
	{
		sdkResult = setStatusConfigValue(context, id, 2);
	}
#endif

	return sdkResult;
}

#if 0
DWORD WINAPI onWaiting(LPVOID lpParam)
{
	int* standby = (int*)lpParam;
	cout << "Is the connection completed from the device?" << endl;
	while (true)
	{
		Sleep(1000);
		char cont = Utility::getInput<char>("Press 'c' continue calling APIs");

		if (cont == 'q' || cont == 'Q')
			break;
	}

	*standby = 0;

	return 0;
}

void waitForConnection()
{
	int standby = 1;
	DWORD thID;
	HANDLE handle = CreateThread(NULL, 0, onWaiting, &standby, 0, &thID);
	if (handle)
	{
		while (standby) { Sleep(1); }
	}
}

#endif

int enrollMultipleUsers(void* context, const DeviceList& devices)
{
	uint32_t numOfUser = Utility::getInput<uint32_t>("How many users would you like to create:");

	uint32_t uid = 1;
	string name = "test";
	BS2_TIMESTAMP startTime = Utility::convertTimeString2UTC("2010-01-01 00:00:00");
	BS2_TIMESTAMP endTime = Utility::convertTimeString2UTC("2030-01-01 00:00:00");
	string pinString = "1234";
	BS2_USER_PIN pin = { 0, };
	int sdkResult = BS2_MakePinCode(context, const_cast<char*>(pinString.c_str()), pin);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_MakePinCode call failed: %d", sdkResult);
		return sdkResult;
	}
	const string fnFaceTemplate = "C:\\Temp\\user01.dat";
	BS2Face face;
	memset(&face, 0x0, sizeof(face));
	FILE* fp = fopen(fnFaceTemplate.c_str(), "rb");
	if (fp)
	{
		size_t readData = fread(&face, sizeof(BS2Face), 1, fp);
		TRACE("Template: %u, Read: %u", sizeof(BS2Face), readData);
	}
	else
	{
		TRACE("Not found template");
		return BS_SDK_SUCCESS;
	}

	BS2UserBlob* userBlob = new BS2UserBlob[numOfUser];
	memset(userBlob, 0x0, sizeof(BS2UserBlob) * numOfUser);
	for (uint32_t idx = 0; idx < numOfUser; idx++)
	{
		BS2User& user = userBlob[idx].user;
		BS2UserSetting& setting = userBlob[idx].setting;

		stringstream msg;

		uint32_t userID = uid + idx;
		sprintf(user.userID, "%u", userID);
		sprintf(reinterpret_cast<char*>(userBlob[idx].user_name), "%s%u", name.c_str(), userID);
		setting.startTime = startTime;
		setting.endTime = endTime;
		memcpy(userBlob[idx].pin, pin, sizeof(BS2_USER_PIN));
		setting.fingerAuthMode = BS2_AUTH_MODE_BIOMETRIC_ONLY;
		setting.cardAuthMode = BS2_AUTH_MODE_CARD_ONLY;
		setting.idAuthMode = BS2_AUTH_MODE_ID_BIOMETRIC;
		setting.securityLevel = BS2_USER_SECURITY_LEVEL_DEFAULT;
		user.flag = BS2_USER_FLAG_CREATED;

		user.numFingers = 0;
		user.numCards = 0;
		user.numFaces = 1;
		userBlob[idx].faceObjs = new BS2Face;
		memcpy(userBlob[idx].faceObjs, &face, sizeof(BS2Face));
	}

#if _CHECK_RUNTIME
	unsigned long tm1 = GetTickCount();
	TRACE("Tick 1: %u", tm1);
#endif
	auto allDevices = devices.getAllDevices();
	for (auto id : allDevices)
	{
		sdkResult = BS2_EnrolUser(context, id.first, userBlob, numOfUser, 1);
		if (BS_SDK_SUCCESS != sdkResult)
			TRACE("BS2_EnrolUser call failed: %d", sdkResult);
	}
#if _CHECK_RUNTIME
	unsigned long tm2 = GetTickCount();
	TRACE("Tick 2: %u", tm2);
	TRACE("Run time: %u.%u", (tm2 - tm1) / 1000, (tm2 - tm1) % 1000);
#endif

	for (uint32_t idx = 0; idx < numOfUser; idx++)
	{
		if (userBlob[idx].cardObjs)
			delete[] userBlob[idx].cardObjs;

		if (userBlob[idx].fingerObjs)
			delete[] userBlob[idx].fingerObjs;

		if (userBlob[idx].faceObjs)
			delete[] userBlob[idx].faceObjs;
	}

	return sdkResult;
}


int getAllAccessSchedule(void* context, BS2_DEVICE_ID id)
{
	AccessControl ac(context);
	vector<BS2Schedule> schedules;
	int sdkResult = ac.getAllAccessSchedule(id, schedules);
	return sdkResult;
}

#if TEST_CODE
int connectTestDevice2(void* context)
{
	CommControl cm(context);

	string ip = "192.168.13.123";
	BS2_PORT port = 51211;
	BS2_DEVICE_ID id = 0;

	TRACE("Now connect to device (IP:%s, Port:%u)", ip.c_str(), port);

	int sdkResult = cm.connectDevice(id, ip, port);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	string inputTime = "2021-03-01 00:00:00";
	BS2_TIMESTAMP currTime = Utility::convertTimeString2UTC(inputTime);

	sdkResult = BS2_SetDeviceTime(context, id, currTime);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetDeviceTime call failed: %d", sdkResult);
		return sdkResult;
	}

	sdkResult = cm.disconnectDevice(id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("Device disconnect failed: %d", sdkResult);

	return sdkResult;
}

int connectTestDevice3(void* context)
{
	CommControl cm(context);

	string ip = "192.168.13.123";
	BS2_PORT port = 51211;
	BS2_DEVICE_ID id = 0;

	TRACE("Now connect to device (IP:%s, Port:%u)", ip.c_str(), port);

	int sdkResult = cm.connectDevice(id, ip, port);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	UserControl uc(context);
	sdkResult = uc.removeAllUser(id);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	const uint32_t numOfRetry = 1000;
	uint32_t idx = 0;
	//for (uint32_t idx = 0; idx < numOfRetry; idx++)
	while (true)
	{
		sdkResult = uc.enrollUserFaceEx_1User(id, idx);
		//sdkResult = uc.enrollUserFaceEx_WithImage_1User(id);
		//sdkResult = uc.enrollUserFaceEx_WithImage_Multi(id);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		sdkResult = getAllAccessSchedule(context, id);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		TRACE("Call finished %d times", idx + 1);
		idx++;
	}

	sdkResult = cm.disconnectDevice(id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("Device disconnect failed: %d", sdkResult);

	return sdkResult;
}
#endif

int runUSBAPIs()
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);

	cout << endl << endl << "== USB Test ==" << endl;

	while (/*BS_SDK_SUCCESS == sdkResult && */MENU_USB_BREAK != (selectedTop = Utility::showMenu(menuInfoUSBAPI)))
	{
		switch (selectedTop)
		{
		case MENU_USB_BREAK:
			return BS_SDK_SUCCESS;
		case MENU_USB_GET_USR_DBINFO_FROMDIR:
			sdkResult = getUserDatabaseInfoFromDir();
			break;
		case MENU_USB_GET_USR_LIST_FROMDIR:
			sdkResult = getUserListFromDir();
			break;
		case MENU_USB_GET_USR_INFO_FROMDIR:
		case MENU_USB_GET_USR_INFOEX_FROMDIR:
		case MENU_USB_GET_USRSMALL_INFO_FROMDIR:
		case MENU_USB_GET_USRSMALL_INFOEX_FROMDIR:
		case MENU_USB_GET_USRFACEEX_INFO_FROMDIR:
			sdkResult = getUserInfosFromDir(selectedTop);
			break;
		case MENU_USB_GET_USR_DATA_FROMDIR:
		case MENU_USB_GET_USR_DATAEX_FROMDIR:
		case MENU_USB_GET_USRSMALL_DATA_FROMDIR:
		case MENU_USB_GET_USRSMALL_DATAEX_FROMDIR:
		case MENU_USB_GET_USRFACEEX_DATA_FROMDIR:
			sdkResult = getUserDatasFromDir(selectedTop);
			break;

		case MENU_USB_GET_FILTEREDLOG_FROMDIR:
			sdkResult = getFilteredLogFromDir();
			break;
		case MENU_USB_GET_LOG_FROMDIR:
			sdkResult = getLogFromDir();
			break;
		case MENU_USB_GET_LOG_BLOB_FROMDIR:
			sdkResult = getLogBlobFromDir();
			break;
		case MENU_USB_GET_LOG_SMALLBLOB_FROMDIR:
			sdkResult = getLogSmallBlobFromDir();
			break;
		case MENU_USB_GET_LOG_SMALLBLOBEX_FROMDIR:
			sdkResult = getLogSmallBlobExFromDir();
			break;
		default:
			break;
		}
	}

	return sdkResult;
}

vector<string> userIDList;
int onIsAcceptableUID(const char* uid)
{
	for (auto item : userIDList)
	{
		if (item == uid)
			return 1;
	}
	return 0;
}

int getUserDatabaseInfoFromDir()
{
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	IsAcceptableUserID ptrAccept = NULL;
	userIDList.clear();
	if (Utility::isYes("Do you want to filter users?"))
	{
		cout << "Enter the user ID to allow. [0: Stop]" << endl;
		while (true)
		{
			string uid = Utility::getInput<string>("UserID:");
			if (uid == "0")
				break;
			else if (BS2_USER_ID_SIZE < uid.size())
				continue;

			userIDList.push_back(uid);
		}

		ptrAccept = onIsAcceptableUID;
	}

	uint32_t numUsers(0), numCards(0), numFingers(0), numFaces(0);

	BS2UsbContext usbCtx(usbPath);
	int sdkResult = usbCtx.getUserDatabaseInfoFromDir(numUsers, numCards, numFingers, numFaces, ptrAccept);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		cout << "Num of users:" << numUsers << endl;
		cout << "Num of cards:" << numCards << endl;
		cout << "Num of Fingers:" << numFingers << endl;
		cout << "Num of Faces:" << numFaces << endl;
	}

	return sdkResult;
}

int getUserListFromDir()
{
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	IsAcceptableUserID ptrAccept = NULL;
	userIDList.clear();
	if (Utility::isYes("Do you want to filter users?"))
	{
		cout << "Enter the user ID to allow. [0: Stop]" << endl;
		while (true)
		{
			string uid = Utility::getInput<string>("UserID:");
			if (uid == "0")
				break;
			else if (BS2_USER_ID_SIZE < uid.size())
				continue;

			userIDList.push_back(uid);
		}

		ptrAccept = onIsAcceptableUID;
	}

	vector<string> userIDs;

	BS2UsbContext usbCtx(usbPath);
	int sdkResult = usbCtx.getUserListFromDir(ptrAccept, userIDs);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		uint32_t count(0);
		uint32_t total(userIDs.size());
		for (auto uid : userIDs)
		{
			cout << "User(" << ++count << "/" << total << "): " << uid << endl;
			if (uid == "0")
				break;
		}
	}

	return sdkResult;
}

int getUserInfosFromDir(int menuItem)
{
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	vector<string> userIDs;
	cout << "Enter the user ID to import from the USB device. [0: Stop]" << endl;
	while (true)
	{
		string uid = Utility::getInput<string>("UserID:");
		if (uid == "0")
			break;
		else if (BS2_USER_ID_SIZE < uid.size())
			continue;

		uid.resize(BS2_USER_ID_SIZE);
		userIDs.push_back(uid);
	}
	if (0 == userIDs.size())
	{
		cout << "User not found." << endl;
		return BS_SDK_SUCCESS;
	}

	BS2UsbContext usbCtx(usbPath);
	int sdkResult = BS_SDK_SUCCESS;

	switch (menuItem)
	{
	case MENU_USB_GET_USR_INFO_FROMDIR:
		{
			vector<BS2UserBlob> userBlob;
			sdkResult = usbCtx.getUserInfosFromDir(userIDs, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				for (auto user : userBlob)
					UserControl::print(user);
			}
		}
		break;

	case MENU_USB_GET_USR_INFOEX_FROMDIR:
		{
			vector<BS2UserBlobEx> userBlob;
			sdkResult = usbCtx.getUserInfosExFromDir(userIDs, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				for (auto user : userBlob)
					UserControl::print(user);
			}
		}
		break;

	case MENU_USB_GET_USRSMALL_INFO_FROMDIR:
		{
			vector<BS2UserSmallBlob> userBlob;
			sdkResult = usbCtx.getUserSmallInfosFromDir(userIDs, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				//for (auto user : userBlob)
				//	UserControl::print(user);
			}
		}
		break;

	case MENU_USB_GET_USRSMALL_INFOEX_FROMDIR:
		{
			vector<BS2UserSmallBlobEx> userBlob;
			sdkResult = usbCtx.getUserSmallInfosExFromDir(userIDs, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				//for (auto user : userBlob)
				//	UserControl::print(user);
			}
		}
		break;

	case MENU_USB_GET_USRFACEEX_INFO_FROMDIR:
		{
			vector<BS2UserFaceExBlob> userBlob;
			sdkResult = usbCtx.getUserInfosFaceExFromDir(userIDs, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				for (auto user : userBlob)
					UserControl::print(user);
			}
		}
		break;

	default:
		return BS_SDK_SUCCESS;
	}

	return sdkResult;
}

int getUserDatasFromDir(int menuItem)
{
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	vector<string> userIDs;
	cout << "Enter the user ID to import from the USB device. [0: Stop]" << endl;
	while (true)
	{
		string uid = Utility::getInput<string>("UserID:");
		if (uid == "0")
			break;
		else if (BS2_USER_ID_SIZE < uid.size())
			continue;

		uid.resize(BS2_USER_ID_SIZE);
		userIDs.push_back(uid);
	}

	if (0 == userIDs.size())
	{
		cout << "User not found." << endl;
		return BS_SDK_SUCCESS;
	}

	BS2_USER_MASK userMask = BS2_USER_MASK_DATA | BS2_USER_MASK_SETTING;
	cout << "Enter the user mask you need." << endl;
	if (Utility::isYes("Do you want access group?"))
		userMask |= BS2_USER_MASK_ACCESS_GROUP;
	if (Utility::isYes("Do you want user name?"))
		userMask |= BS2_USER_MASK_NAME;
	if (Utility::isYes("Do you want user PIN?"))
		userMask |= BS2_USER_MASK_PIN;
	if (Utility::isYes("Do you want user image?"))
		userMask |= BS2_USER_MASK_PHOTO;
	if (Utility::isYes("Do you want cards?"))
		userMask |= BS2_USER_MASK_CARD;
	if (Utility::isYes("Do you want fingerprints?"))
		userMask |= BS2_USER_MASK_FINGER;
	if (Utility::isYes("Do you want faces?"))
	{
		ostringstream msg;
		msg << " What device type is it?" << endl;
		msg << "  0. FaceStation 2" << endl;
		msg << "  1. FaceStation F2" << endl;
		msg << "  2. No face device" << endl;
		int type = Utility::getInput<int>(msg.str());
		switch (type)
		{
		case 0:
			userMask |= BS2_USER_MASK_FACE;
			break;
		case 1:
			userMask |= BS2_USER_MASK_FACE_EX;
			break;
		default:
			break;
		}
	}

	BS2UsbContext usbCtx(usbPath);
	int sdkResult = BS_SDK_SUCCESS;

	switch (menuItem)
	{
	case MENU_USB_GET_USR_DATA_FROMDIR:
		{
			vector<BS2UserBlob> userBlob;
			sdkResult = usbCtx.getUserDatasFromDir(userIDs, userMask, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				for (auto user : userBlob)
					UserControl::print(user);
			}
		}
		break;

	case MENU_USB_GET_USR_DATAEX_FROMDIR:
		{
			vector<BS2UserBlobEx> userBlob;
			sdkResult = usbCtx.getUserDatasExFromDir(userIDs, userMask, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				for (auto user : userBlob)
					UserControl::print(user);
			}
		}
		break;

	case MENU_USB_GET_USRSMALL_DATA_FROMDIR:
		{
			vector<BS2UserSmallBlob> userBlob;
			sdkResult = usbCtx.getUserSmallDatasFromDir(userIDs, userMask, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				//for (auto user : userBlob)
				//	UserControl::print(user);
			}
		}
		break;

	case MENU_USB_GET_USRSMALL_DATAEX_FROMDIR:
		{
			vector<BS2UserSmallBlobEx> userBlob;
			sdkResult = usbCtx.getUserSmallDatasExFromDir(userIDs, userMask, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				//for (auto user : userBlob)
				//	UserControl::print(user);
			}
		}
		break;

	case MENU_USB_GET_USRFACEEX_DATA_FROMDIR:
		{
			vector<BS2UserFaceExBlob> userBlob;
			sdkResult = usbCtx.getUserDatasFaceExFromDir(userIDs, userMask, userBlob);
			if (BS_SDK_SUCCESS == sdkResult)
			{
				for (auto user : userBlob)
					UserControl::print(user);
			}
		}
		break;

	default:
		return BS_SDK_SUCCESS;
	}

	return sdkResult;
}


int getFilteredLogFromDir()
{
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	string userID = Utility::getInput<string>("Please enter a user ID [0: All]");
	if (BS2_USER_ID_SIZE < userID.size())
	{
		TRACE("User ID is too big.");
		return BS_SDK_ERROR_INVALID_PARAM;
	}

	char* uid = NULL;
	if (userID != "0")
	{
		userID.resize(BS2_USER_ID_SIZE);
		uid = const_cast<char*>(userID.c_str());
	}

	BS2_EVENT_CODE eventCode = (BS2_EVENT_CODE)Utility::getInput<uint32_t>("Which event do you want to get? [0: All]");

//#ifdef _DEBUG
//	BS2_TIMESTAMP startTime = 0;
//	BS2_TIMESTAMP endTime = 0;
//#else
	string inputTime = Utility::getLine("Please enter start time [YYYY-MM-DD HH:MM:SS] ?");
	BS2_TIMESTAMP startTime = Utility::convertTimeString2UTC(inputTime);

	inputTime = Utility::getLine("Please enter end time [YYYY-MM-DD HH:MM:SS] ?");
	BS2_TIMESTAMP endTime = Utility::convertTimeString2UTC(inputTime);
//#endif
	
	uint8_t tnaKey = (uint8_t)Utility::getInput<uint32_t>("Which tnaKey do you want to get? [0: All, 1-16]");
	if (BS2_MAX_TNA_KEY < tnaKey)
	{
		TRACE("Invalid tnaKey: %u", tnaKey);
		return BS_SDK_ERROR_INVALID_PARAM;
	}

	BS2UsbContext usbCtx(usbPath);
	vector<BS2Event> eventList;
	int sdkResult = usbCtx.getFilteredLogFromDir(uid, eventCode, startTime, endTime, tnaKey, eventList);
	LogControl::print(eventList.data(), eventList.size());

	cout << "Count: " << eventList.size() << endl;
	return sdkResult;
}

int getLogFromDir()
{
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	BS2_EVENT_ID eventID = Utility::getInput<BS2_EVENT_ID>("What is the ID of the last log which you have? [0: None]");

	uint32_t amount = Utility::getInput<uint32_t>("How many logs do you want to get? [0: All]");

	BS2UsbContext usbCtx(usbPath);
	vector<BS2Event> eventList;
	int sdkResult = usbCtx.getLogFromDir(eventID, amount, eventList);
	LogControl::print(eventList.data(), eventList.size());

	cout << "Count: " << eventList.size() << endl;
	return sdkResult;
}

int getLogBlobFromDir()
{
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	BS2_EVENT_ID eventID = Utility::getInput<BS2_EVENT_ID>("What is the ID of the last log which you have? [0: None]");

	uint32_t amount = Utility::getInput<uint32_t>("How many logs do you want to get? [0: All]");

	BS2UsbContext usbCtx(usbPath);
	vector<BS2EventBlob> eventList;
	int sdkResult = usbCtx.getLogBlobFromDir(BS2_EVENT_MASK_ALL, eventID, amount, eventList);
	LogControl::print(eventList.data(), eventList.size());

	return sdkResult;
}

int getLogSmallBlobFromDir()
{
	stringstream msg;
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	BS2_EVENT_ID eventID = Utility::getInput<BS2_EVENT_ID>("What is the ID of the last log which you have? [0: None]");

	uint32_t amount = Utility::getInput<uint32_t>("How many logs do you want to get? [0: All]");
	
	BS2UsbContext usbCtx(usbPath);
	vector<BS2EventSmallBlob> eventList;
	int sdkResult = usbCtx.getLogSmallBlobFromDir(BS2_EVENT_MASK_ALL, eventID, amount, eventList);
	LogControl::print(eventList.data(), eventList.size());

	for (auto item : eventList)
	{
		if (0 < item.imageSize && item.imageObj)
			BS2_ReleaseObject(item.imageObj);
	}

	return sdkResult;
}

int getLogSmallBlobExFromDir()
{
	string usbPath = BS2UsbContext::getUSBPath();
	if (0 == usbPath.size())
		return BS_SDK_SUCCESS;

	BS2_EVENT_ID eventID = Utility::getInput<BS2_EVENT_ID>("What is the ID of the last log which you have? [0: None]");

	uint32_t amount = Utility::getInput<uint32_t>("How many logs do you want to get? [0: All]");

	BS2UsbContext usbCtx(usbPath);
	vector<BS2EventSmallBlobEx> eventList;
	int sdkResult = usbCtx.getLogSmallBlobExFromDir(BS2_EVENT_MASK_ALL, eventID, amount, eventList);
	LogControl::print(eventList.data(), eventList.size());

	for (auto item : eventList)
	{
		if (0 < item.imageSize && item.imageObj)
			BS2_ReleaseObject(item.imageObj);
	}

	return sdkResult;
}

int setDeviceLicense(void* context, BS2_DEVICE_ID id)
{
	DeviceControl dc(context);
	BS2LicenseBlob licenseBlob;
	memset(&licenseBlob, 0x0, sizeof(licenseBlob));
	vector<BS2_DEVICE_ID> deviceIDs;
	vector<BS2LicenseResult> licenseResult;
	int sdkResult = BS_SDK_SUCCESS;

	licenseBlob.licenseType = (BS2_LICENSE_TYPE)Utility::getInput<uint32_t>("Enter the license type. (0: None, 1: Visual QR)");
	licenseBlob.numOfDevices = (uint16_t)Utility::getInput<uint32_t>("How many devices do you want to register?");
	if (0 < licenseBlob.numOfDevices)
	{
		// Device ID
		for (uint16_t idx = 0; idx < licenseBlob.numOfDevices; idx++)
		{
			BS2_DEVICE_ID deviceID = (BS2_DEVICE_ID)Utility::getInput<uint32_t>("Enter a device ID:");
			deviceIDs.push_back(deviceID);
		}

		licenseBlob.deviceIDObjs = deviceIDs.data();

		string pathName = Utility::getLine("Enter the path and name of license.");
		licenseBlob.licenseLen = Utility::getResourceSize(pathName);
		shared_ptr<uint8_t> buffer(new uint8_t[licenseBlob.licenseLen], ArrayDeleter<uint8_t>());
		if (0 < licenseBlob.licenseLen && Utility::getResourceFromFile(pathName, buffer, licenseBlob.licenseLen))
		{
			licenseBlob.licenseObj = buffer.get();

			sdkResult = dc.enableDeviceLicense(id, &licenseBlob, licenseResult);
			if (BS_SDK_SUCCESS == sdkResult)
				DeviceControl::print(licenseResult);
		}
	}

	return sdkResult;
}

int deleteDeviceLicense(void* context, BS2_DEVICE_ID id)
{
	DeviceControl dc(context);
	BS2LicenseBlob licenseBlob;
	memset(&licenseBlob, 0x0, sizeof(licenseBlob));
	vector<BS2_DEVICE_ID> deviceIDs;
	vector<BS2LicenseResult> licenseResult;
	int sdkResult = BS_SDK_SUCCESS;

	licenseBlob.licenseType = (BS2_LICENSE_TYPE)Utility::getInput<uint32_t>("Enter the license type. (0: None, 1: Visual QR)");
	licenseBlob.numOfDevices = (uint16_t)Utility::getInput<uint32_t>("How many devices do you want to register?");
	if (0 < licenseBlob.numOfDevices)
	{
		// Device ID
		for (uint16_t idx = 0; idx < licenseBlob.numOfDevices; idx++)
		{
			BS2_DEVICE_ID deviceID = (BS2_DEVICE_ID)Utility::getInput<uint32_t>("Enter a device ID:");
			deviceIDs.push_back(deviceID);
		}

		licenseBlob.deviceIDObjs = deviceIDs.data();

		sdkResult = dc.disableDeviceLicense(id, &licenseBlob, licenseResult);
		if (BS_SDK_SUCCESS == sdkResult)
			DeviceControl::print(licenseResult);
	}

	return sdkResult;
}

int getDeviceLicense(void* context, BS2_DEVICE_ID id)
{
	DeviceControl dc(context);
	BS2LicenseBlob licenseBlob;
	memset(&licenseBlob, 0x0, sizeof(licenseBlob));
	vector<BS2LicenseResult> licenseResult;
	int sdkResult = BS_SDK_SUCCESS;

	BS2_LICENSE_TYPE licenseType = (BS2_LICENSE_TYPE)Utility::getInput<uint32_t>("Enter the license type. (0: None, 1: Visual QR)");
	sdkResult = dc.queryDeviceLicense(id, licenseType, licenseResult);
	if (BS_SDK_SUCCESS == sdkResult)
		DeviceControl::print(licenseResult);

	return sdkResult;
}

int getOsdpStandardConfig(void* context, const DeviceList& deviceList)
{
	ConfigControl cc(context);
	BS2OsdpStandardConfig config;
	memset(&config, 0x0, sizeof(config));

	BS2_DEVICE_ID id = Utility::selectDeviceID(deviceList, false, false);
	int sdkResult = cc.getOsdpStandardConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int getOsdpStandardActionConfig(void* context, const DeviceList& deviceList)
{
	ConfigControl cc(context);
	BS2OsdpStandardActionConfig config;
	memset(&config, 0x0, sizeof(config));

	BS2_DEVICE_ID id = Utility::selectDeviceID(deviceList, false, false);
	int sdkResult = cc.getOsdpStandardActionConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setOsdpStandardActionConfig(void* context, const DeviceList& deviceList)
{
	ConfigControl cc(context);
	BS2OsdpStandardActionConfig config;
	memset(&config, 0x0, sizeof(config));

	BS2_DEVICE_ID id = Utility::selectDeviceID(deviceList, false, false);
	int sdkResult = cc.getOsdpStandardActionConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	bool activate = false;
	for (int idx = 0; idx < BS2_OSDP_STANDARD_ACTION_TYPE_COUNT; idx++)
	{
		string msg;
		switch (config.actions[idx].actionType)
		{
		case BS2_OSDP_STANDARD_ACTION_TYPE_SUCCESS:
			msg = "SUCCESS";
			break;
		case BS2_OSDP_STANDARD_ACTION_TYPE_FAIL:
			msg = "FAIL";
			break;
		case BS2_OSDP_STANDARD_ACTION_TYPE_WAIT_INPUT:
			msg = "WAIT_INPUT";
			break;
		case BS2_OSDP_STANDARD_ACTION_TYPE_NONE:
		default:
			msg = "NONE";
			break;
		}
		if (Utility::isYes("Do you want to modify the %s action type?", msg.c_str()))
		{
			for (int ledidx = 0; ledidx < BS2_OSDP_STANDARD_ACTION_MAX_LED; ledidx++)
			{
				string msg;
				activate = Utility::isYes("Do you want to activate for LED#%d action?", ledidx);
				config.actions[idx].led[ledidx].use = activate;

				if (activate)
				{
					msg = "  Please enter your reader number. Default(0).";
					config.actions[idx].led[ledidx].readerNumber = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter a led number of the reader. Default(0).";
					config.actions[idx].led[ledidx].ledNumber = (uint8_t)Utility::getInput<uint32_t>(msg);

					msg = "  Please enter a temporary command (0: NOP, 1: Cancel, 2: Set)";
					config.actions[idx].led[ledidx].tempCommand = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the temporary command on time in 100 ms.";
					config.actions[idx].led[ledidx].tempOnTime = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the temporary command off time in 100 ms.";
					config.actions[idx].led[ledidx].tempOffTime = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the temporary command on color. (0: Black, 1: Red, 2: Green, 3: Amber, 4: Blue, 5: Magenta, 6: Cyan, 7: White)";
					config.actions[idx].led[ledidx].tempOnColor = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the temporary command off color. (0: Black, 1: Red, 2: Green, 3: Amber, 4: Blue, 5: Magenta, 6: Cyan, 7: White)";
					config.actions[idx].led[ledidx].tempOffColor = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the temporary run time in 100 ms.";
					config.actions[idx].led[ledidx].tempRunTime = (uint16_t)Utility::getInput<uint32_t>(msg);

					msg = "  Please enter a permanent command (0: NOP, 1: Cancel, 2: Set)";
					config.actions[idx].led[ledidx].permCommand = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the permanent on time in 100 ms.";
					config.actions[idx].led[ledidx].permOnTime = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the permanent off time in 100 ms.";
					config.actions[idx].led[ledidx].permOffTime = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the permanent on color. (0: Black, 1: Red, 2: Green, 3: Amber, 4: Blue, 5: Magenta, 6: Cyan, 7: White)";
					config.actions[idx].led[ledidx].permOnColor = (uint8_t)Utility::getInput<uint32_t>(msg);
					msg = "  Please enter the permanent off color. (0: Black, 1: Red, 2: Green, 3: Amber, 4: Blue, 5: Magenta, 6: Cyan, 7: White)";
					config.actions[idx].led[ledidx].permOffColor = (uint8_t)Utility::getInput<uint32_t>(msg);
				}
			}

			activate = Utility::isYes("Do you want to activate for buzzer action?");
			config.actions[idx].buzzer.use = activate;
			if (activate)
			{
				msg = "  Please enter your reader number. Default(0).";
				config.actions[idx].buzzer.readerNumber = (uint8_t)Utility::getInput<uint32_t>(msg);
				msg = "  Please enter a tone type (0: None, 1: Off, 2: On)";
				config.actions[idx].buzzer.tone = (uint8_t)Utility::getInput<uint32_t>(msg);
				msg = "  Please enter the buzzer turn-on time in 100 ms.";
				config.actions[idx].buzzer.onTime = (uint8_t)Utility::getInput<uint32_t>(msg);
				msg = "  Please enter the buzzer turn-off time in 100 ms.";
				config.actions[idx].buzzer.offTime = (uint8_t)Utility::getInput<uint32_t>(msg);
				msg = "  Please enter the number of cycle the buzzer on and off.";
				config.actions[idx].buzzer.numOfCycle = (uint8_t)Utility::getInput<uint32_t>(msg);
			}
		}
	}

	return cc.setOsdpStandardActionConfig(id, config);
}

int getAvailableOsdpStandardDevice(void* context, DeviceList& deviceList)
{
	CommControl cc(context);
	BS2OsdpStandardDeviceAvailable availDevice;
	memset(&availDevice, 0x0, sizeof(availDevice));
	int sdkResult = BS_SDK_SUCCESS;

	BS2_DEVICE_ID id = Utility::selectDeviceID(deviceList, false, false);

	sdkResult = cc.getAvailableOsdpStandardDevice(id, availDevice);
	if (BS_SDK_SUCCESS == sdkResult)
		CommControl::print(availDevice);

	return sdkResult;
}

int getOsdpStandardDevice(void* context, DeviceList& deviceList)
{
	ConfigControl cc(context);
	CommControl mc(context);
	BS2OsdpStandardConfig config;
	BS2OsdpStandardDevice osdpDevice;
	memset(&config, 0x0, sizeof(config));
	memset(&osdpDevice, 0x0, sizeof(osdpDevice));

	BS2_DEVICE_ID id = Utility::selectDeviceID(deviceList, false, false);
	int sdkResult = cc.getOsdpStandardConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	uint32_t numOfActivated = cc.printOSDPDeviceID(config);
	if (0 < numOfActivated)
	{
		BS2_DEVICE_ID osdpID = Utility::selectSlaveID();

		sdkResult = mc.getOsdpStandardDevice(osdpID, osdpDevice);
		if (BS_SDK_SUCCESS == sdkResult)
			CommControl::print(osdpDevice);
	}

	return sdkResult;
}

int addOsdpStandardDevice(void* context, DeviceList& deviceList)
{
	CommControl mc(context);
	BS2OsdpStandardDeviceAvailable availDevice;
	BS2OsdpStandardDeviceAdd addDevice;
	memset(&availDevice, 0x0, sizeof(availDevice));
	memset(&addDevice, 0x0, sizeof(addDevice));

	int sdkResult = BS_SDK_SUCCESS;

	BS2_DEVICE_ID masterID = Utility::selectDeviceID(deviceList, false, false);
	if (mc.getAvailableOsdpStandardDevice(masterID, availDevice))
		CommControl::print(availDevice);

	cout << "Now add a OSDP device." << endl;
	addDevice.deviceID = Utility::selectSlaveID();

	uint32_t channelIndex = 0;
	for (int idx = 0; idx < availDevice.numOfChannel; idx++)
	{
		for (int didx = 0; didx < availDevice.channels[idx].numOsdpAvailableDevice; didx++)
		{
			if (availDevice.channels[idx].deviceIDs[didx] == addDevice.deviceID)
				channelIndex = availDevice.channels[idx].channelIndex;
		}
	}

	addDevice.osdpID = (uint8_t)Utility::getInput<uint32_t>("Please enter the OSDP ID. [0 ~ 126]");
	addDevice.useSecureSession = Utility::isYes("Does the OSDP device use secure communication?");
	addDevice.deviceType = BS2_DEVICE_TYPE_3RD_OSDP_DEVICE;
	addDevice.activate = 1;

	uint32_t outChannelIndex(0);
	sdkResult = mc.addOsdpStandardDevice(masterID, channelIndex, addDevice, outChannelIndex);
	if (sdkResult == BS_SDK_SUCCESS)
	{
		cout << "Successfully added OSDP device to channel " << outChannelIndex << "." << endl;
	}

	return sdkResult;
}

int updateOsdpStandardDevice(void* context, DeviceList& deviceList)
{
	ConfigControl cc(context);
	CommControl mc(context);
	BS2OsdpStandardConfig config;
	memset(&config, 0x0, sizeof(config));
	vector<BS2OsdpStandardDeviceUpdate> updateData;

	BS2_DEVICE_ID id = Utility::selectDeviceID(deviceList, false, false);
	int sdkResult = cc.getOsdpStandardConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	uint32_t numOfActivated = cc.printOSDPDeviceID(config);
	uint32_t numOfDevice = Utility::getInput<uint32_t>("How many devices do you want to update? (0~%u)", numOfActivated);
	if (0 < numOfDevice)
	{
		for (uint32_t idx = 0; idx < numOfDevice; idx++)
		{
			BS2OsdpStandardDeviceUpdate item;
			memset(&item, 0x0, sizeof(item));
			item.deviceID = (BS2_DEVICE_ID)Utility::getInput<uint32_t>("[%u] Please enter the slave ID to be updated.", idx + 1);

			if (!ConfigControl::getOsdpID(config, item.deviceID, item.osdpID))
			{
				cout << "The OSDP ID could not be found." << endl;
				return BS_SDK_ERROR_INTERNAL;
			}

			if (Utility::isYes("Do you want to change the OSDP ID? (CurrentID: %u)", item.osdpID))
			{
				item.osdpID = (uint8_t)Utility::getInput<uint32_t>("Please enter the OSDP ID. [0 ~ 126]");
			}

			item.activate = Utility::isYes("Do you like to enable the OSDP device?");
			item.useSecureSession = Utility::isYes("Does the OSDP device use secure communication?");
			item.deviceType = BS2_DEVICE_TYPE_3RD_OSDP_DEVICE;

			updateData.push_back(item);
		}

		vector<BS2OsdpStandardDeviceResult> listResult;
		if (BS_SDK_SUCCESS == (sdkResult = mc.updateOsdpStandardDevice(id, updateData, listResult)))
		{
			CommControl::print(listResult);
		}
	}

	return sdkResult;
}

int removeOsdpStandardDevice(void* context, DeviceList& deviceList)
{
	ConfigControl cc(context);
	CommControl mc(context);
	BS2OsdpStandardConfig config;
	memset(&config, 0x0, sizeof(config));
	vector<BS2_DEVICE_ID> removeData;

	BS2_DEVICE_ID id = Utility::selectDeviceID(deviceList, false, false);
	int sdkResult = cc.getOsdpStandardConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	uint32_t numOfActivated = cc.printOSDPDeviceID(config);
	uint32_t numOfDevice = Utility::getInput<uint32_t>("How many devices do you want to remove? (0~%u)", numOfActivated);
	if (0 < numOfDevice)
	{
		for (uint32_t idx = 0; idx < numOfDevice; idx++)
		{
			BS2_DEVICE_ID slaveID = (BS2_DEVICE_ID)Utility::getInput<uint32_t>("[%u] Please enter the slave ID to be removed.", idx + 1);
			removeData.push_back(slaveID);
		}

		vector<BS2OsdpStandardDeviceResult> listResult;
		if (BS_SDK_SUCCESS == (sdkResult = mc.removeOsdpStandardDevice(id, removeData, listResult)))
		{
			CommControl::print(listResult);
		}
	}

	return sdkResult;
}

int getOsdpStandardDeviceCapability(void* context, DeviceList& deviceList)
{
	ConfigControl cc(context);
	CommControl mc(context);
	BS2OsdpStandardConfig config;
	BS2OsdpStandardDeviceCapability capability;
	memset(&config, 0x0, sizeof(config));
	memset(&capability, 0x0, sizeof(capability));

	BS2_DEVICE_ID id = Utility::selectDeviceID(deviceList, false, false);
	int sdkResult = cc.getOsdpStandardConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	uint32_t numOfActivated = cc.printOSDPDeviceID(config);
	if (0 < numOfActivated)
	{
		BS2_DEVICE_ID id = Utility::selectSlaveID();

		int sdkResult = mc.getOsdpStandardDeviceCapability(id, capability);
		if (BS_SDK_SUCCESS == sdkResult)
			CommControl::print(capability);
	}

	return sdkResult;
}

int setOsdpStandardDeviceSecurityKey(void* context, DeviceList& deviceList)
{
	CommControl mc(context);
	int sdkResult = BS_SDK_SUCCESS;

	bool useMaster = true;
	BS2_DEVICE_ID id = (BS2_DEVICE_ID)Utility::selectMasterOrSlaveID(deviceList, useMaster);
	if (useMaster)
	{
		BS2OsdpStandardDeviceSecurityKey key;
		memset(&key, 0x0, sizeof(key));
		string keyInfo = Utility::getInput<string>("Please enter the OSDP security key.");
		memcpy(key.key, keyInfo.c_str(), min(keyInfo.size(), (size_t)BS2_OSDP_STANDARD_KEY_SIZE));

		sdkResult = mc.setOsdpStandardDeviceSecurityKey(id, &key);
	}
	else
	{
		sdkResult = mc.setOsdpStandardDeviceSecurityKey(id, NULL);
	}

	if (BS_SDK_SUCCESS == sdkResult)
		cout << "Set success" << endl;

	return sdkResult;
}

void onOsdpStandardDeviceStatusChanged(BS2_DEVICE_ID /*deviceId*/, const BS2OsdpStandardDeviceNotify* notifyData)
{
	if (notifyData)
		CommControl::print(*notifyData);
}

int startMonitorOsdpStandardDeviceStatus(void* context)
{
	CommControl mc(context);

	return mc.setOsdpStandardDeviceStatusListener(onOsdpStandardDeviceStatusChanged);
}

int stopMonitorOsdpStandardDeviceStatus(void* context)
{
	CommControl mc(context);

	return mc.setOsdpStandardDeviceStatusListener(NULL);
}