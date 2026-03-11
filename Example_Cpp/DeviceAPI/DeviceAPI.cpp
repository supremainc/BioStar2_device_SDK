#include "stdafx.h"
#include <sstream>
#include <random>
#include "DeviceAPI.h"
#include "../Common/Utility.h"
#include "../Common/CommControl.h"
#include "../Common/LogControl.h"
#include "../Common/UserControl.h"
#include "../Common/DoorControl.h"

#pragma warning(disable:4800)

extern void TRACE(const char* fmt, ...);
using namespace std;

static void* sdkContext = NULL;
static BS2_DEVICE_ID connectedID = 0;
static DeviceInfo deviceInfo = { 0, 0, 0, 51211, 0 };
static DeviceList deviceList;

// Wiegand format preset
enum
{
	BS2_WIEGAND_H10301_26,
	BS2_WIEGAND_H10302_37,
	BS2_WIEGAND_H10304_37,
	BS2_WIEGAND_C1000_35,
	BS2_WIEGAND_C1000_48,
};

const vector<pair<uint32_t, string>> WIEGAND_FORMAT_PRESET = {
	{BS2_WIEGAND_H10301_26, "BS2_WIEGAND_H10301_26"},
	{BS2_WIEGAND_H10302_37, "BS2_WIEGAND_H10302_37"},
	{BS2_WIEGAND_H10304_37, "BS2_WIEGAND_H10304_37"},
	{BS2_WIEGAND_C1000_35,  "BS2_WIEGAND_C1000_35"},
	{BS2_WIEGAND_C1000_48,  "BS2_WIEGAND_C1000_48"},
};


void onLogReceived(BS2_DEVICE_ID id, const BS2Event* event)
{
	if (deviceInfo.id_ == id)
	{
		int32_t timezone = deviceInfo.timezone_;
		cout << LogControl::getEventString(id, *event, timezone) << endl;
	}
}

// Thermal supported callback
void onLogReceivedEx(BS2_DEVICE_ID id, const BS2Event* event, BS2_TEMPERATURE temperature)
{
	if (deviceInfo.id_ == id)
	{
		int32_t timezone = deviceInfo.timezone_;
		cout << LogControl::getEventString(id, *event, timezone, temperature) << endl;
	}
}


void onDeviceConnected(BS2_DEVICE_ID id)
{
	if (deviceInfo.id_ == id)
		deviceInfo.connected_ = true;

	TRACE("Device(%d) connected", id);
}


void onDeviceDisconnected(BS2_DEVICE_ID id)
{
	deviceInfo.connected_ = false;

	TRACE("Device(%d) disconnected", id);
}


int main(int argc, char* argv[])
{
	// Set debugging SDK log (to current working directory)
	BS2Context::setDebugFileLog(DEBUG_LOG_ALL, DEBUG_MODULE_ALL, ".", 100);

	TRACE("Version: %s", BS2_Version());

	sdkContext = BS2Context::getInstance()->getContext();

	// Create SDK context and initialize
	if (BS_SDK_SUCCESS != BS2Context::getInstance()->initSDK())
	{
		BS2Context::getInstance()->releaseInstance();
		return -1;
	}

	BS2Context::getInstance()->setDeviceEventListener(NULL, onDeviceConnected, onDeviceDisconnected);

	connectTestDevice(sdkContext);

	BS2Context::getInstance()->releaseInstance();
	return 0;
}

void connectTestDevice(void* context)
{
	int sdkResult = BS_SDK_SUCCESS;
	bool isDirectConnect = Utility::isNo("Would you like to search for devices?");

	if (isDirectConnect)
	{
		memset(&deviceInfo, 0x0, sizeof(DeviceInfo));
		sdkResult = Utility::connectViaIP(context, deviceInfo);
		if (BS_SDK_SUCCESS != sdkResult)
			return;
	}
	else 
	{		
		sdkResult = Utility::searchAndConnect(sdkContext, deviceList);
		if (BS_SDK_SUCCESS != sdkResult)
			return;
		deviceInfo = *deviceList.getAllDevices().begin()->second.get();
	}

	// Retrieve bulk logs.
	CommControl cm(context);
#if RETRIVE_BULK_LOGS
	sdkResult = Utility::getAllLogsFromDevice(sdkContext, deviceInfo.id_, deviceInfo.timezone_);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("An error occurred while receiving bulk logs from device: %d", sdkResult);
		cm.disconnectDevice(deviceInfo.id_);
		return;
	}
#endif

	// Set callback for realtime logs
	//sdkResult = BS2_StartMonitoringLog(sdkContext, deviceInfo.id_, onLogReceived);
	sdkResult = BS2_StartMonitoringLogEx(sdkContext, deviceInfo.id_, onLogReceivedEx);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_StartMonitoringLogEx call failed: %d", sdkResult);

	Utility::connectSlave(context, deviceInfo);
	Utility::connectWiegand(context, deviceInfo);
	
	runAPIs(context, deviceInfo);
}

int runAPIs(void* context, DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;

	cout << endl << endl << "== Top ==" << endl;

	bool menuBreak = false;
	while (!menuBreak)
	{
		uint32_t selected = Utility::showMenu(menuInfoTop);
		switch (selected)
		{
		case MENU_TOP_BREAK:
			menuBreak = true;
			break;
		case MENU_TOP_DEVICE_TEST:
			sdkResult = runDeviceAPIs(context, device);
			break;
		case MENU_TOP_DOOR_TEST:
			sdkResult = runDoorAPIs(context, device);
			break;
		case MENU_TOP_LOG_TEST:
			sdkResult = runLogAPIs(context, device);
			break;
		default:
			break;
		}

		//if (BS_SDK_SUCCESS != sdkResult)
		//	return;
	}

	return sdkResult;
}

int runDeviceAPIs(void* context, DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);
	DeviceControl dc(context);
	ConfigControl cc(context);

	cout << endl << endl << "== DeviceAPI Test ==" << endl;

	while (/*BS_SDK_SUCCESS == sdkResult && */MENU_DEV_BREAK != (selectedTop = Utility::showMenu(menuInfoDeviceAPI)))
	{
		if (!device.connected_)
		{
			TRACE("No device connected");
			return BS_SDK_ERROR_CANNOT_CONNECT_SOCKET;
		}

		switch (selectedTop)
		{
		case MENU_DEV_BREAK:
			return BS_SDK_SUCCESS;

		case MENU_DEV_GET_DEVINF:
			sdkResult = getDeviceInfo(context, device);
			break;
		case MENU_DEV_GET_DEVINFEX:
			sdkResult = getDeviceInfoEx(context, device);
			break;
		case MENU_DEV_GET_DEVTIME:
			sdkResult = dc.getDeviceTime(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_SET_DEVTIME:
			sdkResult = dc.setDeviceTime(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_CLR_DATABASE:
			sdkResult = dc.clearDatabase(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_FACTORY_RESET:
			sdkResult = dc.factoryReset(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_REBOOT_DEV:
			sdkResult = dc.rebootDevice(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_LOCK_DEV:
			sdkResult = dc.lockDevice(Utility::getSelectedDeviceID(device));
			// try a fingerprint verification test.
			break;
		case MENU_DEV_UNLOCK_DEV:
			sdkResult = dc.unlockDevice(Utility::getSelectedDeviceID(device));
			// try a fingerprint verification test.
			break;
		case MENU_DEV_UPG_FIRMWARE:
			sdkResult = dc.upgradeFirmware(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_UPD_RESOURCE:
			sdkResult = dc.updateResource(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_GET_SPCDEVINFO:
			sdkResult = dc.getSpecifiedDeviceInfo(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_RST_CONFIG_EXCEPT_NETINFO:
			sdkResult = cc.resetConfigExceptNetInfo(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DEV_GET_DEVICECAPABILITIES:
			sdkResult = getDeviceCapabilities(context, device);
			break;

		case MENU_DEV_GET_FINGERPRINTCONFIG:
			sdkResult = getFingerprintConfig(context, device);
			break;
		case MENU_DEV_SET_FINGERPRINTCONFIG: 
			sdkResult = setFingerprintConfig(context, device);
			break;
		case MENU_DEV_GET_FACECONFIG:
			sdkResult = getFaceConfig(context, device);
			break;
		case MENU_DEV_SET_FACECONFIG:
			sdkResult = setFaceConfig(context, device);
			break;
		case MENU_DEV_GET_DISPLAYCONFIG:
			sdkResult = getDisplayConfig(context, device);
			break;
		case MENU_DEV_SET_DISPLAYCONFIG:
			sdkResult = setDisplayConfig(context, device);
			break;
		case MENU_DEV_GET_IPCONFIG:
			sdkResult = getIPConfig(context, device);
			break;
		case MENU_DEV_SET_IPCONFIG:
			sdkResult = setIPConfig(context, device);
			break;
		case MENU_DEV_GET_SYSTEMCONFIG:
			sdkResult = getSystemConfig(context, device);
			break;
		case MENU_DEV_SET_SYSTEMCONFIG:
			sdkResult = setSystemConfig(context, device);
			break;
		case MENU_DEV_GET_DESFIRECONFIGEX:
			sdkResult = getDesFireCardConfigEx(context, device);
			break;
		case MENU_DEV_SET_DESFIRECONFIGEX:
			sdkResult = setDesFireCardConfigEx(context, device);
			break;
		case MENU_DEV_GET_AUTHCONFIGEX:
			sdkResult = getAuthConfigEx(context, device);
			break;
		case MENU_DEV_SET_AUTHCONFIGEX:
			sdkResult = setAuthConfigEx(context, device);
			break;
		case MENU_DEV_GET_FACECONFIGEX:
			sdkResult = getFaceConfigEx(context, device);
			break;
		case MENU_DEV_SET_FACECONFIGEX:
			sdkResult = setFaceConfigEx(context, device);
			break;
		case MENU_DEV_GET_THERMALCAMERACONFIG:
			sdkResult = getThermalCameraConfig(context, device);
			break;
		case MENU_DEV_SET_THERMALCAMERACONFIG:
			sdkResult = setThermalCameraConfig(context, device);
			break;
		case MENU_DEV_GET_EVENTCONFIG:
			sdkResult = getEventConfig(context, device);
			break;
		case MENU_DEV_SET_EVENTCONFIG:
			sdkResult = setEventConfig(context, device);
			break;
		case MENU_DEV_GET_INPUTCONFIG:
			sdkResult = getInputConfig(context, device);
			break;
		case MENU_DEV_GET_TRIGGERACTIONCONFIG:
			sdkResult = getTriggerActionConfig(context, device);
			break;
		case MENU_DEV_SET_TRIGGERACTIONCONFIG:
			sdkResult = setTriggerActionConfig(context, device);
			break;
		case MENU_DEV_REM_TRIGGERACTIONCONFIG:
			sdkResult = removeTriggerActionConfig(context, device);
			break;
		case MENU_DEV_GET_BARCODECONFIG:
			sdkResult = getBarcodeConfig(context, device);
			break;
		case MENU_DEV_SET_BARCODECONFIG:
			sdkResult = setBarcodeConfig(context, device);
			break;
		case MENU_DEV_GET_RS485CONFIG:
			sdkResult = getRS485Config(context, device);
			break;
		case MENU_DEV_SET_RS485CONFIG:
			sdkResult = setRS485Config(context, device);
			break;
		case MENU_DEV_GET_RS485CONFIGEX:
			sdkResult = getRS485ConfigEx(context, device);
			break;
		case MENU_DEV_SET_RS485CONFIGEX:
			sdkResult = setRS485ConfigEx(context, device);
			break;
		case MENU_DEV_GET_RS485CONFIGEXDYNAMIC:
			sdkResult = getRS485ConfigExDynamic(context, device);
			break;
		case MENU_DEV_SET_RS485CONFIGEXDYNAMIC:
			sdkResult = setRS485ConfigExDynamic(context, device);
			break;
		case MENU_DEV_GET_RS485CONFIGEXDYNAMICREMOVE:
			sdkResult = getRS485ConfigExDynamicAndRemove(context, device);
			break;
		case MENU_DEV_GET_INPUTCONFIGEX:
			sdkResult = getInputConfigEx(context, device);
			break;
		case MENU_DEV_SET_INPUTCONFIGEX:
			sdkResult = setInputConfigEx(context, device);
			break;
		case MENU_DEV_GET_RELAYACTIONCONFIG:
			sdkResult = getRelayActionConfig(context, device);
			break;
		case MENU_DEV_SET_RELAYACTIONCONFIG:
			sdkResult = setRelayActionConfig(context, device);
			break;
		case MENU_DEV_GET_WLANCONFIG:
			sdkResult = getWLANConfig(context, device);
			break;
		case MENU_DEV_SET_WLANCONFIG:
			sdkResult = setWLANConfig(context, device);
			break;
		case MENU_DEV_SET_WIEGANDMULTICONFIG:
			sdkResult = setWiegandMultiConfigWithPreset(context, device);
			break;
		case MENU_DEV_GET_WIEGANDCONFIG:
			sdkResult = getWiegandConfig(context, device);
			break;
		case MENU_DEV_SET_WIEGANDCONFIG:
			sdkResult = setWiegandConfig(context, device);
			break;
		case MENU_DEV_GET_VOIPCONFIGEXT:
			sdkResult = getVoipConfigExt(context, device);
			break;
		case MENU_DEV_SET_VOIPCONFIGEXT:
			sdkResult = setVoipConfigExt(context, device);
			break;
		case MENU_DEV_GET_RTSPCONFIG:
			sdkResult = getRtspConfig(context, device);
			break;
		case MENU_DEV_SET_RTSPCONFIG:
			sdkResult = setRtspConfig(context, device);
			break;
		case MENU_DEV_GET_LICENSECONFIG:
			sdkResult = getLicenseConfig(context, device);
			break;
		case MENU_DEV_GET_OSDPSTANDARDCONFIG:
			sdkResult = getOsdpStandardConfig(context, device);
			break;
		case MENU_DEV_GET_OSDPSTANDARDACTIONCONFIG:
			sdkResult = getOsdpStandardActionConfig(context, device);
			break;
		case MENU_DEV_SET_OSDPSTANDARDACTIONCONFIG:
			sdkResult = setOsdpStandardActionConfig(context, device);
			break;
		case MENU_DEV_GET_CARDCONFIG:
			sdkResult = getCardConfig(context, device);
			break;
		case MENU_DEV_SET_CARDCONFIG:
			sdkResult = setCardConfig(context, device);
			break;		
		case MENU_DEV_GET_CUSTOMCARDCONFIG:
			sdkResult = getCustomCardConfig(context, device);
			break;
		case MENU_DEV_SET_CUSTOMCARDCONFIG:
			sdkResult = setCustomCardConfig(context, device);
			break;
		case MENU_DEV_UPD_DEVICE_VOLUME:
			sdkResult = updateDeviceVolume(context, device);
			break;
		case MENU_DEV_RUN_ACTION:
			sdkResult = runAction(context, device);
			break;
		case MENU_DEV_TURNON_QRBYPASS:
			sdkResult = turnOnQRBypass(context, device);
			break;
		case MENU_DEV_TURNOFF_QRBYPASS:
			sdkResult = turnOffQRBypass(context, device);
			break;
		case MENU_DEV_SEARCH_SLAVE:
			sdkResult = searchSlave(context, device);
			break;
		case MENU_DEV_DISPLAY_SLAVE:
			displaySlave(context, device);
			break;
		case MENU_DEV_GET_FACILITYCODECONFIG:
			sdkResult = getFacilityCodeConfig(context, device);
			break;
		case MENU_DEV_SET_FACILITYCODECONFIG:
			sdkResult = setFacilityCodeConfig(context, device);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}

int runDoorAPIs(void* context, DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);
	DoorControl dc(context);

	cout << endl << endl << "== DoorAPI Test ==" << endl;

	while (/*BS_SDK_SUCCESS == sdkResult && */MENU_DOOR_BREAK != (selectedTop = Utility::showMenu(menuInfoDoorAPI)))
	{
		if (!device.connected_)
		{
			TRACE("No device connected");
			return BS_SDK_ERROR_CANNOT_CONNECT_SOCKET;
		}

		switch (selectedTop)
		{
		case MENU_DOOR_BREAK:
			return BS_SDK_SUCCESS;

		case MENU_DOOR_GET_DOOR:
			sdkResult = getDoors(context, device);
			break;
		case MENU_DOOR_SET_DOOR:
			sdkResult = setDoors(context, device);
			break;
		case MENU_DOOR_REM_DOORALL:
			sdkResult = dc.removeAllDoor(Utility::getSelectedDeviceID(device));
			break;
		case MENU_DOOR_LOCK:
			sdkResult = lockDoor(context, device);
			break;
		case MENU_DOOR_UNLOCK:
			sdkResult = unlockDoor(context, device);
			break;
		case MENU_DOOR_GETSTATUS:
			sdkResult = getStatus(context, device);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}

int runLogAPIs(void* context, DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);
	LogControl lc(context);

	cout << endl << endl << "== LogAPI Test ==" << endl;

	while (/*BS_SDK_SUCCESS == sdkResult && */MENU_LOG_BREAK != (selectedTop = Utility::showMenu(menuInfoLogAPI)))
	{
		if (!device.connected_)
		{
			TRACE("No device connected");
			return BS_SDK_ERROR_CANNOT_CONNECT_SOCKET;
		}

		switch (selectedTop)
		{
		case MENU_LOG_BREAK:
			return BS_SDK_SUCCESS;

		case MENU_LOG_GET_LOG:
			sdkResult = lc.getLogSmallBlob(Utility::getSelectedDeviceID(device));
			break;
		case MENU_LOG_GET_LOGEX:
			sdkResult = lc.getLogSmallBlobEx(Utility::getSelectedDeviceID(device));
			break;
		case MENU_LOG_GET_DEVICEIO_STATUS:
			sdkResult = getDeviceIOStatus(context, device);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}

int searchSlave(void* context, DeviceInfo& device)
{
	bool isSlave = false;
	bool found = false;
	int sdkResult = BS_SDK_ERROR_CANNOT_FIND_DEVICE;

	BS2_DEVICE_ID selected = Utility::getSelectedDeviceID(device);
	if (device.id_ == selected) {
		isSlave = false;
		found = true;
	}
	else {
		auto& slaveList = device.slaveDevices_;
		auto it = find_if(slaveList.begin(), slaveList.end(), [selected](const BS2_DEVICE_ID_TYPE idType) { return idType.id == selected; });
		if (it != slaveList.end()) {
			isSlave = true;
			found = true;
		}
	}

	if (found) sdkResult = Utility::connectSlave(context, device, isSlave, selected);

	return sdkResult;
}

 void displaySlave(void* context, const DeviceInfo& device)
 {
	Utility::displayConnectedSlaves(device);
 }


 int getFacilityCodeConfig(void* context, const DeviceInfo& device)
 {
	 //ConfigControl cc(context);
	 //BS2FacilityCodeConfig cofig;
	 //
	 //BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	 //int sdkResult = cc.getFacilityCodeConfig(id, cofig);
	 //if (BS_SDK_SUCCESS == sdkResult)
		// ConfigControl::print(cofig);
	 return 1;
 }
 
 int setFacilityCodeConfig(void* context, const DeviceInfo& device)
 {
	// ConfigControl cc(context);
	// BS2FacilityCodeConfig config;
	// 
	// BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	// int sdkResult = cc.getFacilityCodeConfig(id, config);
	//if (BS_SDK_SUCCESS != sdkResult)
	//	return sdkResult;

	//string msg = "Enter the number of facility code";
	//config.numFacilityCode = Utility::getInput<uint32_t>(msg);
	//if (config.numFacilityCode > BS2_MAX_NUMBER_FACILITY_CODE)
	//	config.numFacilityCode = BS2_MAX_NUMBER_FACILITY_CODE;

	//for (uint8_t i = 0; i < config.numFacilityCode; i++)
	//{
	//	msg = "Enter the facility code";
	//	uint32_t inputValue = Utility::getInput<uint32_t>(msg);
	//	if (Utility::isBigEndianSystem())
	//		*(uint32_t*)config.facilityCodes[i].code = inputValue;
	//	else
	//		*(uint32_t*)config.facilityCodes[i].code = ((inputValue & 0xFF000000) >> 24) |
	//									   ((inputValue & 0x00FF0000) >> 8) |
	//									   ((inputValue & 0x0000FF00) << 8) |
	//									   ((inputValue & 0x000000FF) << 24);
	//}	
	//sdkResult = cc.setFacilityCodeConfig(id, config);

	 return 1;
 }

int getDeviceInfo(void* context, const DeviceInfo& device)
{
	DeviceControl dc(context);
	BS2SimpleDeviceInfo info = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = dc.getDeviceInfo(id, info);
	if (BS_SDK_SUCCESS == sdkResult)
		DeviceControl::print(info);

	return sdkResult;
}

int getDeviceInfoEx(void* context, const DeviceInfo& device)
{
	DeviceControl dc(context);
	BS2SimpleDeviceInfo info = { 0, };
	BS2SimpleDeviceInfoEx infoEx = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = dc.getDeviceInfoEx(id, info, infoEx);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		DeviceControl::print(info);
		DeviceControl::print(infoEx);
	}

	return sdkResult;
}

int getFingerprintConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2FingerprintConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getFingerprintConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setFingerprintConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	UserControl uc(context);
	BS2FingerprintConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getFingerprintConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	bool removeFirst = false;
	string msg = "Select a fingerprint authentication security level. (0: Basic, 1: Highly secure, 2: The most highly secure)";
	config.securityLevel = (BS2_FINGER_SECURITY_LEVEL)Utility::getInput<uint32_t>(msg);
	msg = "Select a matching speed. (0: Automatic, 1: Basic, 2: High, 3: Very high)";
	config.fastMode = (BS2_FINGER_FAST_MODE)Utility::getInput<uint32_t>(msg);
	msg = "Select a sensitivity of the fingerprint sensor. (0: Lowest, 1-6: Level 1~6, 7: Highest)";
	config.sensitivity = (BS2_FINGER_SENSITIVITY)Utility::getInput<uint32_t>(msg);
	msg = "Select a sensor mode. (0: Always on, 1: Finger approach detection)";
	config.sensorMode = (BS2_FINGER_SENSOR_MODE)Utility::getInput<uint32_t>(msg);
	msg = "Select a fingerprint template format. (0: Suprema, 1: ISO, 2: ANSI)";
	BS2_FINGER_TEMPLATE_FORMAT tempFormat = (BS2_FINGER_TEMPLATE_FORMAT)Utility::getInput<uint32_t>(msg);
	if (config.templateFormat != tempFormat)
	{
		ostringstream strm;
		strm << "If the fingerprint format is changed," << endl;
		strm << " the user's fingerprint must also be re-enrolled." << endl;
		strm << " Do you want to proceed after deleting all users?" << endl;
		strm << " - Y: Remove all users and set config" << endl;
		strm << " - N: Keep previous template format";
		if (Utility::isYes(strm.str()))
		{
			removeFirst = true;
			config.templateFormat = tempFormat;
		}
	}
	msg = "Enter the fingerprint scanning timeout in seconds";
	config.scanTimeout = (uint16_t)Utility::getInput<uint32_t>(msg);
	msg = "Do you want to turn on the advancedEnrollment option?";
	config.advancedEnrollment = Utility::isYes(msg);
	msg = "Do you want to turn on the showImage option?";
	config.showImage = Utility::isYes(msg);
	msg = "Select a LFD level. (0, Not use, 1: Strict, 2: More strict, 3: Most strict)";
	config.lfdLevel = (BS2_FINGER_LFD_LEVEL)Utility::getInput<uint32_t>(msg);
	msg = "Do you want to turn on the checkDuplicate option?";
	config.checkDuplicate = Utility::isYes(msg);

	if (removeFirst)
	{
		sdkResult = uc.removeAllUser(id);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;
	}

	sdkResult = cc.setFingerprintConfig(id, config);

	return sdkResult;
}

int getFaceConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2FaceConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getFaceConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setFaceConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2FaceConfig config = { 0, };

	BS2_DEVICE_ID id(0);
	BS2_DEVICE_TYPE type(0);
	if (!Utility::getSelectedDeviceID(device, id, type))
		return BS_SDK_SUCCESS;

	int sdkResult = cc.getFaceConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		string msg = "Insert securityLevel. (0: Basic, 1: Highly secure, 2: Most highly secure)";
		config.securityLevel = (BS2_FACE_SECURITY_LEVEL)Utility::getInput<uint32_t>(msg);

		msg = "Insert lightCondition. (0: Normal, 1: High, 3: Not used)";
		config.lightCondition = (BS2_FACE_LIGHT_CONDITION)Utility::getInput<uint32_t>(msg);

		msg = "Insert enrollThreshold. (0: Most strict - 9: Least strict, 4: Default)";
		config.enrollThreshold = (BS2_FACE_ENROLL_THRESHOLD)Utility::getInput<uint32_t>(msg);

		msg = "Insert detectSensitivity. (0: Off, 1: Low, 2: Basic, 3: High)";
		config.detectSensitivity = (BS2_FACE_DETECT_SENSITIVITY)Utility::getInput<uint32_t>(msg);

		int defaultEnrollTimeout(0), defaultLFD(0);
		bool needInput = false;
		switch (type)
		{
		case BS2_DEVICE_TYPE_FACESTATION_2:
		case BS2_DEVICE_TYPE_FACELITE:
		case BS2_DEVICE_TYPE_CORESTATION_40:
			defaultEnrollTimeout = 60;
			defaultLFD = 0;
			needInput = true;
			break;
		case BS2_DEVICE_TYPE_FACESTATION_F2_FP:
		case BS2_DEVICE_TYPE_FACESTATION_F2:
		case BS2_DEVICE_TYPE_BIOSTATION_3:
		case BS2_DEVICE_TYPE_BIOENTRY_W3:
		//case BS2_DEVICE_TYPE_BIOSTATION_3_MAX:
		//case BS2_DEVICE_TYPE_BIOSTATION_3_MAX_FP:
			defaultEnrollTimeout = 20;
			defaultLFD = 1;
			needInput = true;
			break;
		default:
			break;
		}

		if (needInput)
		{
			ostringstream strm;
			strm << "Insert enrollTimeout. (default: " << defaultEnrollTimeout << "s)";
			config.enrollTimeout = (uint16_t)Utility::getInput<uint32_t>(strm.str());

			strm.str("");
			strm << "Insert lfdLevel. (0: Not use, 1: Strict, 2: More Strict, 3: Most Strict... (default: " << defaultLFD << "))";
			config.lfdLevel = (BS2_FACE_LFD_LEVEL)Utility::getInput<uint32_t>(strm.str());
		}
		else
		{
			config.enrollTimeout = 0;
			config.lfdLevel = 0;
		}

		msg = "Do you want to turn on the quickEnrollment? (Y: 1-step enrollment(Quick), N: 3-step enrollment(High quality))";
		config.quickEnrollment = Utility::isYes(msg);

		msg = "Insert previewOption. (0: Not used, 1: 1/2 stage, 2: All stages)";
		config.previewOption = (BS2_FACE_PREVIEW_OPTION)Utility::getInput<uint32_t>(msg);

		msg = "Do you want to turn on the checkDuplicate?";
		config.checkDuplicate = Utility::isYes(msg);

		msg = "Insert operationMode. (0: Fusion, 1: Visual, 2: Visual (+IR detect))";
		config.operationMode = (BS2_FACE_OPERATION_MODE)Utility::getInput<uint32_t>(msg);

		msg = "Insert maxRotation. (default: 15)";
		config.maxRotation = (uint8_t)Utility::getInput<uint32_t>(msg);

		char buf[128] = { 0, };
		switch (type)
		{
		case BS2_DEVICE_TYPE_FACESTATION_F2_FP:
		case BS2_DEVICE_TYPE_FACESTATION_F2:
			sprintf(buf, "Insert min value of faceWidth. (default: %d)", BS2_FACE_WIDTH_MIN_DEFAULT);
			config.faceWidth.min = (uint16_t)Utility::getInput<uint32_t>(buf);

			sprintf(buf, "Insert max value of faceWidth. (default: %d)", BS2_FACE_WIDTH_MAX_DEFAULT);
			config.faceWidth.max = (uint16_t)Utility::getInput<uint32_t>(buf);

			sprintf(buf, "Insert x value of searchRange. (default: %d)", BS2_FACE_SEARCH_RANGE_X_DEFAULT);
			config.searchRange.x = (uint16_t)Utility::getInput<uint32_t>(buf);

			sprintf(buf, "Insert width value of searchRange. (default: %d)", BS2_FACE_SEARCH_RANGE_WIDTH_DEFAULT);
			config.searchRange.width = (uint16_t)Utility::getInput<uint32_t>(buf);
			break;

		case BS2_DEVICE_TYPE_BIOSTATION_3:
		case BS2_DEVICE_TYPE_BIOENTRY_W3:
		//case BS2_DEVICE_TYPE_BIOSTATION_3_MAX:
		//case BS2_DEVICE_TYPE_BIOSTATION_3_MAX_FP:
			sprintf(buf, "Insert min value of detectDistance. (%d~%d, default: %d)",
				BS2_FACE_DETECT_DISTANCE_MIN_MIN,
				BS2_FACE_DETECT_DISTANCE_MIN_MAX,
				BS2_FACE_DETECT_DISTANCE_MIN_DEFAULT);
			config.detectDistance.min = (uint8_t)Utility::getInput<uint32_t>(buf);

			sprintf(buf, "Insert max value of detectDistance. (%d~%d, default: %d, infinite: %d)",
				BS2_FACE_DETECT_DISTANCE_MAX_MIN,
				BS2_FACE_DETECT_DISTANCE_MAX_MAX,
				BS2_FACE_DETECT_DISTANCE_MAX_DEFAULT,
				BS2_FACE_DETECT_DISTANCE_MAX_INF);
			config.detectDistance.max = (uint8_t)Utility::getInput<uint32_t>(buf);

			msg = "Do you want to turn on the wideSearch?";
			config.wideSearch = Utility::isYes(msg);
			break;

		default:
			break;
		}

		msg = "Would you like to have facial images stored on the device?. (0: Store facial images, 1: Store template only)";
		config.unableToSaveImageOfVisualFace = (uint8_t)Utility::getInput<uint32_t>(msg);

		sdkResult = cc.setFaceConfig(id, config);
	}

	return sdkResult;
}

int getDisplayConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2DisplayConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getDisplayConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setDisplayConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2DisplayConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getDisplayConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	string msg = "Please select a language. (0: Korean, 1: English, 2: Custom)";
	config.language = (BS2_LANGUAGE)Utility::getInput<uint32_t>(msg);

	msg = "Please select a background style. (0: Logo, 1: Notice, 2: Slide, 3: PDF)";
	config.background = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "Enter the volume. (0 ~ 100)";
	config.volume = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "Please select a background theme. (0: Theme1, 1: Theme2, 2: Theme3, 3: Theme4)";
	config.bgTheme = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "Please select a date format. (0: YYYY/MM/DD, 1: MM/DD/YYYY, 2: DD/MM/YYYY)";
	config.dateFormat = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "Please select a menu timeout. (0, 10, 20, 30, 40, 50, 60)";
	config.menuTimeout = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "Please select a message timeout in millisec. (500, 1000, 2000, 3000, 4000, 5000)";
	config.msgTimeout = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "Please select a backlight timeout. (0, 10, 20, 30, 40, 50, 60)";
	config.backlightTimeout = (uint16_t)Utility::getInput<uint32_t>(msg);

	config.displayDateTime = Utility::isYes("Would you like to display the clock?");

	config.useVoice = Utility::isYes("Would you like to use voice instruction?");

	msg = "Please select a time format. (0: 12hour, 1: 24hour)";
	config.timeFormat = (uint8_t)Utility::getInput<uint32_t>(msg);

	//config.homeFormation = 0;
	config.useUserPhrase = Utility::isYes("Would you like to use a personal authentication message? (Using device user message)");
	config.queryUserPhrase = Utility::isYes("Would you like to use a personal authentication message? (Ask the server)");
	config.useScreenSaver = Utility::isYes("Would you like to use screen saver?");

	msg = "Display authentication result from Controller.";
	config.showOsdpResult = Utility::isYes(msg) ? BS2_SHOW_OSDP_RESULT_ON : BS2_SHOW_OSDP_RESULT_OFF;

	//msg = "Display User name in Auth result message? (0: All, 1: Partial, 2: No)";
	//config.authMsgUserName = (uint8_t)Utility::getInput<uint32_t>(msg);

	//msg = "Display User ID in Auth result message? (0: All, 1: Partial, 2: No)";
	//config.authMsgUserId = (uint8_t)Utility::getInput<uint32_t>(msg);

	//msg = "Would you like to use scramble keyboard mode for pin input (0:scramble 1:non-scramble)?";
	//config.scrambleKeyboardMode = (uint8_t)Utility::getInput<uint32_t>(msg);

	return cc.setDisplayConfig(id, config);
}

int getIPConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2IpConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getIPConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setIPConfig(void* context, const DeviceInfo& device)
{
#if 0
	ConfigControl cc(context);
	BS2IpConfig config = { 0, };

	BS2_DEVICE_ID id(0);
	BS2_DEVICE_TYPE type(0);
	if (!Utility::getSelectedDeviceID(device, id, type))
		return BS_SDK_SUCCESS;

	int sdkResult = cc.getIPConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		string msg = "Enter the connection mode. (0: Server->Device, 1: Device->Server)";
		config.connectionMode = Utility::getInput<uint32_t>(msg);

		msg = "Do you want to use the DHCP?";
		config.useDHCP = Utility::isYes(msg);

		string addr;
		if (!config.useDHCP)
		{
			memset(config.ipAddress, 0x0, sizeof(config.ipAddress));
			memset(config.gateway, 0x0, sizeof(config.gateway));
			memset(config.subnetMask, 0x0, sizeof(config.gateway));

			addr = Utility::getInput<string>("Enter the IP address.");
			memcpy(config.ipAddress, addr.c_str(), addr.size());

			addr = Utility::getInput<string>("Enter the gateway address.");
			memcpy(config.gateway, addr.c_str(), addr.size());

			addr = Utility::getInput<string>("Enter the subnet mask.");
			memcpy(config.gateway, addr.c_str(), addr.size());

			config.port = (BS2_PORT)Utility::getInput<uint32_t>("Enter the port number.");
		}

		config.useDNS = Utility::isYes("Do you want to use DNS?");

		memset(config.serverAddr, 0x0, sizeof(config.serverAddr));
		addr = Utility::getInput<string>("Enter the server address.");
		memcpy(config.serverAddr, addr.c_str(), addr.size());

		config.serverPort = (BS2_PORT)Utility::getInput<uint32_t>("Enter the server port.");
		config.sslServerPort = (BS2_PORT)Utility::getInput<uint32_t>("Enter the SSL server port.");

		ostringstream oss;
		oss << "Enter the MTU size. (Current: " << (uint32_t)config.mtuSize << ")" << endl;
		config.mtuSize = (uint16_t)Utility::getInput<uint32_t>(oss.str());

		oss.str("");
		oss << "Enter the baseband. (Current: " << (uint32_t)config.baseband << ")" << endl;
		config.baseband = (BS2_TCP_BASEBAND)Utility::getInput<uint32_t>(oss.str());

		sdkResult = cc.setIPConfig(id, config);
	}

	return sdkResult;
#else
	return BS_SDK_SUCCESS;
#endif
}

int getSystemConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2SystemConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getSystemConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setSystemConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2SystemConfig config = {0,};

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getSystemConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		ostringstream strm;
		if (Utility::isYes("Update card operation mask?"))
		{
			strm << "Please enter the card combination you wish to set." << endl;
			strm << "    0x00000800 : CUSTOM_DESFIRE_EV1" << endl;
			strm << "    0x00000400 : CUSTOM_CLASSIC_PLUS" << endl;
			strm << "    0x00000200 : BLE" << endl;
			strm << "    0x00000100 : NFC" << endl;
			strm << "    0x00000080 : SEOS" << endl;
			strm << "    0x00000040 : SR_SE" << endl;
			strm << "    0x00000020 : DESFIRE_EV1" << endl;
			strm << "    0x00000010 : CLASSIC_PLUS" << endl;
			strm << "    0x00000008 : ICLASS" << endl;
			strm << "    0x00000004 : MIFARE_FELICA" << endl;
			strm << "    0x00000002 : HIDPROX" << endl;
			strm << "    0x00000001 : EM" << endl;

			uint32_t cardTypes = Utility::getInputHexaChar<uint32_t>(strm.str());
			cardTypes |= CARD_OPERATION_USE;		// Card operation apply
			config.useCardOperationMask = cardTypes;

			TRACE("CardType:0x%08x", config.useCardOperationMask);
		}

		strm.str("");
		strm << "Update field usbJobCode? (current:" << config.useJobCode << ")";
		if (Utility::isYes(strm.str()))
		{
			config.useJobCode = (BS2_BOOL)Utility::getInput<uint32_t>("Please insert useJobCode. (0: false, 1: true)");
		}

		sdkResult = cc.setSystemConfig(id, config);
	}

	return sdkResult;
}

int getDesFireCardConfigEx(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2DesFireCardConfigEx config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getDesFireCardConfigEx(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setDesFireCardConfigEx(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2DesFireCardConfigEx config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getDesFireCardConfigEx(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		config.desfireAppKey.appMasterKey[0] = 0x01;
		config.desfireAppKey.appMasterKey[1] = 0xFE;
		config.desfireAppKey.fileReadKeyNumber = 1;
		config.desfireAppKey.fileReadKey[0] = 0x01;
		config.desfireAppKey.fileReadKey[1] = 0xFE;
		config.desfireAppKey.fileWriteKeyNumber = 2;
		config.desfireAppKey.fileWriteKey[0] = 0x01;
		config.desfireAppKey.fileWriteKey[1] = 0xFE;

		sdkResult = cc.setDesFireCardConfigEx(id, config);
	}

	return sdkResult;
}

int getAuthConfigEx(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2AuthConfigExt config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	int sdkResult = cc.getAuthConfigEx(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setAuthConfigEx(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2AuthConfigExt config = { 0, };
	const int EXIT_MENU = 999;
	uint32_t mode(0);

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	int sdkResult = cc.getAuthConfigEx(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	do
	{
		stringstream msg;
		msg << "Register FaceEx authentication mode" << endl;
		msg << " 11. Face" << endl;
		msg << " 12. Face + Fingerprint" << endl;
		msg << " 13. Face + PIN" << endl;
		msg << " 14. Face + Fingerprint/PIN" << endl;
		msg << " 15. Face + Fingerprint + PIN" << endl;
		msg << "999. No more changes." << endl;
		msg << ">> ";

		mode = Utility::getInput<uint32_t>(msg.str());
		if (BS2_EXT_AUTH_MODE_FACE_ONLY <= mode && mode <= BS2_EXT_AUTH_MODE_FACE_FINGERPRINT_PIN)
		{
			stringstream msgSel;
			msgSel << "Set On/Off" << endl;
			msgSel << " 0. Off (No time)" << endl;
			msgSel << " 1. On (Always)" << endl;
			msgSel << " >> ";

			uint32_t onoff = Utility::getInput<uint32_t>(msgSel.str());
			switch (onoff)
			{
			case 0:
			case 1:
				config.extAuthSchedule[mode] = onoff;
				break;
			}
		}
	} while (mode != EXIT_MENU);

	do
	{
		stringstream msg;
		msg << "Register Fingerprint authentication mode" << endl;
		msg << " 16. Fingerprint" << endl;
		msg << " 17. Fingerprint + Face" << endl;
		msg << " 18. Fingerprint + PIN" << endl;
		msg << " 19. Fingerprint + Face/PIN" << endl;
		msg << " 20. Fingerprint + Face + PIN" << endl;
		msg << "999. No more changes." << endl;
		msg << ">> ";

		mode = Utility::getInput<uint32_t>(msg.str());
		if (BS2_EXT_AUTH_MODE_FINGERPRINT_ONLY <= mode && mode <= BS2_EXT_AUTH_MODE_FINGERPRINT_FACE_PIN)
		{
			stringstream msgSel;
			msgSel << "Set On/Off" << endl;
			msgSel << " 0. Off (No time)" << endl;
			msgSel << " 1. On (Always)" << endl;
			msgSel << " >> ";

			uint32_t onoff = Utility::getInput<uint32_t>(msgSel.str());
			switch (onoff)
			{
			case 0:
			case 1:
				config.extAuthSchedule[mode] = onoff;
				break;
			}
		}
	} while (mode != EXIT_MENU);

	do
	{
		stringstream msg;
		msg << "Register Card authentication mode" << endl;
		msg << " 21. Card" << endl;
		msg << " 22. Card + Face" << endl;
		msg << " 23. Card + Fingerprint" << endl;
		msg << " 24. Card + PIN" << endl;
		msg << " 25. Card + Face/Fingerprint" << endl;
		msg << " 26. Card + Face/PIN" << endl;
		msg << " 27. Card + Fingerprint/PIN" << endl;
		msg << " 28. Card + Face/Fingerprint/PIN" << endl;
		msg << " 29. Card + Face + Fingerprint" << endl;
		msg << " 30. Card + Face + PIN" << endl;
		msg << " 31. Card + Fingerprint + Face" << endl;
		msg << " 32. Card + Fingerprint + PIN" << endl;
		msg << " 33. Card + Face/Fingerprint + PIN" << endl;
		msg << " 34. Card + Face + Fingerprint/PIN" << endl;
		msg << " 35. Card + Fingerprint + Face/PIN" << endl;
		msg << "999. No more changes." << endl;
		msg << ">> ";

		mode = Utility::getInput<uint32_t>(msg.str());
		if (BS2_EXT_AUTH_MODE_CARD_ONLY <= mode && mode <= BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_FACE_OR_PIN)
		{
			stringstream msgSel;
			msgSel << "Set On/Off" << endl;
			msgSel << " 0. Off (No time)" << endl;
			msgSel << " 1. On (Always)" << endl;
			msgSel << " >> ";

			uint32_t onoff = Utility::getInput<uint32_t>(msgSel.str());
			switch (onoff)
			{
			case 0:
			case 1:
				config.extAuthSchedule[mode] = onoff;
				break;
			}
		}
	} while (mode != EXIT_MENU);

	do
	{
		stringstream msg;
		msg << "Register ID authentication mode" << endl;
		msg << " 36. ID + Face" << endl;
		msg << " 37. ID + Fingerprint" << endl;
		msg << " 38. ID + PIN" << endl;
		msg << " 39. ID + Face/Fingerprint" << endl;
		msg << " 40. ID + Face/PIN" << endl;
		msg << " 41. ID + Fingerprint/PIN" << endl;
		msg << " 42. ID + Face/Fingerprint/PIN" << endl;
		msg << " 43. ID + Face + Fingerprint" << endl;
		msg << " 44. ID + Face + PIN" << endl;
		msg << " 45. ID + Fingerprint + Face" << endl;
		msg << " 46. ID + Fingerprint + PIN" << endl;
		msg << " 47. ID + Face/Fingerprint + PIN" << endl;
		msg << " 48. ID + Face + Fingerprint/PIN" << endl;
		msg << " 49. ID + Fingerprint + Face/PIN" << endl;
		msg << "999. No more changes." << endl;
		msg << ">> ";

		mode = Utility::getInput<uint32_t>(msg.str());
		if (BS2_EXT_AUTH_MODE_ID_FACE <= mode && mode <= BS2_EXT_AUTH_MODE_ID_FINGERPRINT_FACE_OR_PIN)
		{
			stringstream msgSel;
			msgSel << "Set On/Off" << endl;
			msgSel << " 0. Off (No time)" << endl;
			msgSel << " 1. On (Always)" << endl;
			msgSel << " >> ";

			uint32_t onoff = Utility::getInput<uint32_t>(msgSel.str());
			switch (onoff)
			{
			case 0:
			case 1:
				config.extAuthSchedule[mode] = onoff;
				break;
			}
		}
	} while (mode != EXIT_MENU);

	string msg = "Insert global APB option. (0: Not use, 1: Use)";
	config.useGlobalAPB = (BS2_BOOL)Utility::getInput<uint32_t>(msg);

	msg = "Insert global APB fail action. (0: Not use, 1: Soft APB, 2: Hard APB)";
	config.globalAPBFailAction = (BS2_GLOBAL_APB_FAIL_ACTION_TYPE)Utility::getInput<uint32_t>(msg);

	msg = "Using group matching. (0: Not use, 1: Use)";
	config.useGroupMatching = (BS2_BOOL)Utility::getInput<uint32_t>(msg);

	msg = "Insert private authentication. (0: Not use, 1: Use)";
	config.usePrivateAuth = (BS2_BOOL)Utility::getInput<uint32_t>(msg);

	msg = "Insert face detection level. (0: Not use, 1: Normal mode, 2: Strict mode)";
	config.faceDetectionLevel = (BS2_FACE_DETECTION_LEVEL)Utility::getInput<uint32_t>(msg);

	msg = "Insert server matching option. (0: Not use, 1: Use)";
	config.useServerMatching = (BS2_BOOL)Utility::getInput<uint32_t>(msg);

	msg = "Using full access. (0: Not use, 1: Use)";
	config.useFullAccess = (BS2_BOOL)Utility::getInput<uint32_t>(msg);

	msg = "Insert matching timeout in seconds";
	config.matchTimeout = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "Insert authentication timeout in seconds";
	config.authTimeout = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "How many Operator ID do you want to register?";
	config.numOperators = (uint8_t)Utility::getInput<uint32_t>(msg);
	if (config.numOperators > BS2_MAX_OPERATORS)
	{
		TRACE("Exceed the maximum number of operators.");
		config.numOperators = BS2_MAX_OPERATORS;
	}

	for (size_t i = 0; i < config.numOperators; i++)
	{
		msg = "Insert Operator ID";
		string userID = Utility::getInput<string>(msg);
		if (BS2_USER_NAME_SIZE < userID.size())
		{
			TRACE("User ID is too long.");
			return BS_SDK_ERROR_INVALID_PARAM;
		}
		strcpy(reinterpret_cast<char*>(config.operators[i].userID), userID.c_str());

		msg = "Insert Operator Level. (0: none, 1: Admin, 2:Config, 3:User)";
		config.operators[i].level = (BS2_OPERATOR_LEVEL)Utility::getInput<uint32_t>(msg);
	}

	return cc.setAuthConfigEx(id, config);
}

int getFaceConfigEx(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2FaceConfigExt config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getFaceConfigEx(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setFaceConfigEx(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2FaceConfigExt config = { 0, };
	string msg;
	stringstream strmsg;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	msg = "Insert thermal check mode. (0: Not use, 1: Hard, 2: Soft)";
	config.thermalCheckMode = (BS2_FACE_CHECK_MODE)Utility::getInput<uint32_t>(msg);

	msg = "Insert mask check mode. (0: Not use, 1: Hard, 2: Soft 3: DenyMask)";
	config.maskCheckMode = (BS2_FACE_CHECK_MODE)Utility::getInput<uint32_t>(msg);

	msg = "Insert thermal format. (0: Fahrenheit, 1: Celsius)";
	config.thermalFormat = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "Insert low value of high temperature range in Celsius. (1.0 ~ 45.0)";
	float thresholdLow = Utility::getInput<float>(msg);
	config.thermalThresholdLow = (uint16_t)(thresholdLow * 100);

	msg = "Insert high value of high temperature range in Celsius. (1.0 ~ 45.0)";
	float thresholdHigh = Utility::getInput<float>(msg);
	config.thermalThresholdHigh = (uint16_t)(thresholdHigh * 100);

	msg = "Insert mask detection level. (0: Not use, 1: Normal, 2: High, 3: Very high)";
	config.maskDetectionLevel = (BS2_MASK_DETECTION_LEVEL)Utility::getInput<uint32_t>(msg);

	msg = "Do you want to record the temperature in the event log?";
	config.auditTemperature = Utility::isYes(msg);

	msg = "Do you want to use reject sound?";
	config.useRejectSound = Utility::isYes(msg);

	msg = "Do you want to use overlapped thermal?";
	config.useOverlapThermal = Utility::isYes(msg);

	msg = "Do you want to use dynamic ROI?";
	config.useDynamicROI = Utility::isYes(msg);


	strmsg << "Insert face check order." << endl;
	strmsg << " 0: Face check after auth [default]" << endl;
	strmsg << " 1: Face check before auth" << endl;
	strmsg << " 2: Face check without auth";
	config.faceCheckOrder = (BS2_FACE_CHECK_ORDER)Utility::getInput<uint32_t>(strmsg.str());

	return cc.setFaceConfigEx(id, config);
}

int getThermalCameraConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2ThermalCameraConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getThermalCameraConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setThermalCameraConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2ThermalCameraConfig config = { 0, };
	string msg;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	msg = "Insert camera distance from user. (cm. Recommend: 70)";
	config.distance = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "Insert emission rate. (95/97/98, Recommend: 98)";
	config.emissionRate = (uint8_t)Utility::getInput<uint32_t>(msg);

	cout << "Insert ROI(Region of interest)." << endl;
	msg = "x:";
	config.roi.x = (uint16_t)Utility::getInput<uint32_t>(msg);
	msg = "y:";
	config.roi.y = (uint16_t)Utility::getInput<uint32_t>(msg);
	msg = "width:";
	config.roi.width = (uint16_t)Utility::getInput<uint32_t>(msg);
	msg = "height:";
	config.roi.height = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "Do you want to use body compensation";
	config.useBodyCompensation = Utility::isYes(msg);

	msg = "Insert compensation temperature *10. If you want -4.5, it is -45. (-50 ~ 50)";
	config.compensationTemperature = (int8_t)Utility::getInput<int32_t>(msg);

	return cc.setThermalCameraConfig(id, config);
}

int getEventConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2EventConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getEventConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setEventConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2EventConfig config = { 0, };
	string msg;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	//msg = "Insert camera distance from user. (cm. Recommend: 70)";
	//config.distance = (uint8_t)Utility::getInput<uint32_t>(msg);

	//msg = "Insert emission rate. (95/97/98, Recommend: 98)";
	//config.emissionRate = (uint8_t)Utility::getInput<uint32_t>(msg);

	//cout << "Insert ROI(Region of interest)." << endl;
	//msg = "x:";
	//config.roi.x = (uint16_t)Utility::getInput<uint32_t>(msg);
	//msg = "y:";
	//config.roi.y = (uint16_t)Utility::getInput<uint32_t>(msg);
	//msg = "width:";
	//config.roi.width = (uint16_t)Utility::getInput<uint32_t>(msg);
	//msg = "height:";
	//config.roi.height = (uint16_t)Utility::getInput<uint32_t>(msg);

	//msg = "Do you want to use body compensation";
	//config.useBodyCompensation = Utility::isYes(msg);

	//msg = "Insert compensation temperature *10. If you want -4.5, it is -45. (-50 ~ 50)";
	//config.compensationTemperature = (int8_t)Utility::getInput<int32_t>(msg);

	return cc.setEventConfig(id, config);
}

int getInputConfig(void* context, const DeviceInfo& device)
{
	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	DeviceControl dc(context);
	BS2SimpleDeviceInfo info = { 0, };
	int sdkResult = dc.getDeviceInfo(id, info);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	switch (info.type)
	{
	case BS2_DEVICE_TYPE_CORESTATION_40:
	case BS2_DEVICE_TYPE_DOOR_MODULE_20:
		break;
	case BS2_DEVICE_TYPE_IM_120:
		//break;
	default:
		return BS_SDK_ERROR_NOT_SUPPORTED;
	}

	ConfigControl cc(context);
	BS2InputConfig config = { 0, };

	sdkResult = cc.getInputConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int getTriggerActionConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2TriggerActionConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getTriggerActionConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setTriggerActionConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2TriggerActionConfig config = { 0, };
	string msg;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	msg = "How many trigger-action do you want to register?";
	config.numItems = (uint8_t)Utility::getInput<uint32_t>(msg);

	for (uint8_t idx = 0; idx < config.numItems; idx++)
	{
		BS2TriggerAction& triggerAction = config.items[idx];
		setTriggerAction(device, triggerAction);
	}

	return cc.setTriggerActionConfig(id, config);
}

void setTriggerAction(const DeviceInfo& device, BS2TriggerAction& triggerAction)
{
	setTrigger(device, triggerAction.trigger);
	setAction(device, triggerAction.action);

	if (triggerAction.trigger.type == BS2_TRIGGER_INPUT &&
		(triggerAction.action.type == BS2_ACTION_AUTH_SUCCESS ||
			triggerAction.action.type == BS2_ACTION_AUTH_FAIL ||
			triggerAction.action.type == BS2_ACTION_LED))
	{
		string msg = "[Trigger] Enter the interval(millisec) to ignore the input signal. (ignore input).";
		triggerAction.trigger.ignoreSignalTime = (uint16_t)Utility::getInput<uint32_t>(msg);
	}
	else
	{
		triggerAction.trigger.ignoreSignalTime = 0;
	}
}

void setTrigger(const DeviceInfo& device, BS2Trigger& trigger)
{
	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	trigger.deviceID = id;

	string msg = "[Trigger] Select trigger type. (0: None, 1: Event, 2: Input, 3: Schedule)";
	trigger.type = (BS2_TRIGGER_TYPE)Utility::getInput<uint32_t>(msg);

	switch (trigger.type)
	{
	case BS2_TRIGGER_EVENT:
		setEventTrigger(trigger.event);
		break;
	case BS2_TRIGGER_INPUT:
		setInputTrigger(trigger.input);
		break;
	case BS2_TRIGGER_SCHEDULE:
		setScheduleTrigger(trigger.schedule);
		break;
	case BS2_TRIGGER_NONE:
	default:
		break;
	}
}

void setEventTrigger(BS2EventTrigger& eventTrigger)
{
	string msg = "[EventTrigger] Enter the event code.";
	eventTrigger.code = (BS2_EVENT_CODE)Utility::getInput<uint32_t>(msg);
}

void setInputTrigger(BS2InputTrigger& inputTrigger)
{
	string msg = "[InputTrigger] Enter the input port No.";
	inputTrigger.port = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "[InputTrigger] Enter the switchType (0: N/O, 1: N/C).";
	BS2_SWITCH_TYPE sw = (BS2_SWITCH_TYPE)Utility::getInput<uint32_t>(msg);
	inputTrigger.switchType = (sw == BS2_SWITCH_TYPE_NORMAL_OPEN) ? BS2_SWITCH_TYPE_NORMAL_OPEN : BS2_SWITCH_TYPE_NORMAL_CLOSED;

	msg = "[InputTrigger] Enter the duration.";
	inputTrigger.duration = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "[InputTrigger] Enter the schedule ID. (1: Always)";
	inputTrigger.scheduleID = (BS2_SCHEDULE_ID)Utility::getInput<uint32_t>(msg);
}

void setScheduleTrigger(BS2ScheduleTrigger& scheduleTrigger)
{
	string msg = "[ScheduleTrigger] Enter the schedule trigger type. (0: On start, 1: On end)";
	scheduleTrigger.type = (BS2_SCHEDULE_TRIGGER_TYPE)Utility::getInput<uint32_t>(msg);

	msg = "[ScheduleTrigger] Enter the schedule ID. (1: Always)";
	scheduleTrigger.scheduleID = (BS2_SCHEDULE_ID)Utility::getInput<uint32_t>(msg);
}

void setAction(const DeviceInfo& device, BS2Action& action)
{
	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	action.deviceID = id;

	ostringstream ss;
	ss << "[Action] Select action type." << endl;
	ss << "    0 : BS2_ACTION_NONE" << endl;
	ss << "    1 : BS2_ACTION_LOCK_DEVICE" << endl;
	ss << "    2 : BS2_ACTION_UNLOCK_DEVICE" << endl;
	ss << "    3 : BS2_ACTION_REBOOT_DEVICE" << endl;
	ss << "    4 : BS2_ACTION_RELEASE_ALARM" << endl;
	ss << "    5 : BS2_ACTION_GENERAL_INPUT" << endl;
	ss << "    6 : BS2_ACTION_RELAY" << endl;
	ss << "    7 : BS2_ACTION_TTL" << endl;
	ss << "    8 : BS2_ACTION_SOUND" << endl;
	ss << "    9 : BS2_ACTION_DISPLAY" << endl;
	ss << "   10 : BS2_ACTION_BUZZER" << endl;
	ss << "   11 : BS2_ACTION_LED" << endl;
	ss << "   12 : BS2_ACTION_FIRE_ALARM_INPUT" << endl;
	ss << "   13 : BS2_ACTION_AUTH_SUCCESS" << endl;
	ss << "   14 : BS2_ACTION_AUTH_FAIL" << endl;
	ss << "   15 : BS2_ACTION_LIFT" << endl;
	action.type = (BS2_ACTION_TYPE)Utility::getInput<uint32_t>(ss.str());

	string msg = "[Action] Enter the stop flag. (0: None, 1: When door closed, 2: By command action)";
	action.stopFlag = (BS2_STOP_FLAG)Utility::getInput<uint32_t>(msg);

	msg = "[Action] Enter the action delay";
	action.delay = (uint16_t)Utility::getInput<uint32_t>(msg);

	switch (action.type)
	{
	case BS2_ACTION_RELAY:
		setRelayAction(action.relay);
		break;
	case BS2_ACTION_TTL:
		setOutputPortAction(action.outputPort);
		break;
	case BS2_ACTION_SOUND:
		setSoundAction(action.sound);
		break;
	case BS2_ACTION_DISPLAY:
		setDisplayAction(action.display);
		break;
	case BS2_ACTION_BUZZER:
		setBuzzerAction(action.buzzer);
		break;
	case BS2_ACTION_LED:
		setLedAction(action.led);
		break;
	case BS2_ACTION_LIFT:
		setLiftAction(action.lift);
		break;
	case BS2_ACTION_LOCK_DEVICE:
	case BS2_ACTION_UNLOCK_DEVICE:
	case BS2_ACTION_REBOOT_DEVICE:
	case BS2_ACTION_RELEASE_ALARM:
	case BS2_ACTION_GENERAL_INPUT:
	case BS2_ACTION_FIRE_ALARM_INPUT:
	case BS2_ACTION_AUTH_SUCCESS:
	case BS2_ACTION_AUTH_FAIL:
		break;
	case BS2_ACTION_NONE:
	default:
		break;
	}
}

void setRelayAction(BS2RelayAction& relayAction)
{
	string msg = "[RelayAction] Enter the relay index.";
	relayAction.relayIndex = (uint8_t)Utility::getInput<uint32_t>(msg);

	BS2Signal& relaySignal = relayAction.signal;
	msg = "[RelayAction] Enter the signal ID.";
	relaySignal.signalID = (BS2_UID)Utility::getInput<uint32_t>(msg);

	msg = "[RelayAction] Enter the signal count.";
	relaySignal.count = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "[RelayAction] Enter the signal On-duration.";
	relaySignal.onDuration = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "[RelayAction] Enter the signal Off-duration.";
	relaySignal.offDuration = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "[RelayAction] Enter the signal delay.";
	relaySignal.delay = (uint16_t)Utility::getInput<uint32_t>(msg);
}

void setOutputPortAction(BS2OutputPortAction& outputPortAction)
{
	string msg = "[OutputPortAction] Enter the port index.";
	outputPortAction.portIndex = (uint8_t)Utility::getInput<uint32_t>(msg);

	BS2Signal& outputPortSignal = outputPortAction.signal;
	msg = "[OutputPortAction] Enter the signal ID.";
	outputPortSignal.signalID = (BS2_UID)Utility::getInput<uint32_t>(msg);

	msg = "[OutputPortAction] Enter the signal count.";
	outputPortSignal.count = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "[OutputPortAction] Enter the signal On-duration.";
	outputPortSignal.onDuration = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "[OutputPortAction] Enter the signal Off-duration.";
	outputPortSignal.offDuration = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "[OutputPortAction] Enter the signal delay.";
	outputPortSignal.delay = (uint16_t)Utility::getInput<uint32_t>(msg);
}

void setSoundAction(BS2SoundAction& soundAction)
{
	string msg = "[SoundAction] Enter the count.";
	soundAction.count = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "[SoundAction] Enter the sound index. (0: Welcome, 1: Auth success, 2: Auth fail, 3: Alarm1, 4: Alarm2)";
	soundAction.soundIndex = (BS2_SOUND_INDEX)Utility::getInput<uint32_t>(msg);

	msg = "[SoundAction] Enter the delay.";
	soundAction.delay = (uint16_t)Utility::getInput<uint32_t>(msg);
}

void setDisplayAction(BS2DisplayAction& displayAction)
{
	string msg = "[DisplayAction] Enter the duration.";
	displayAction.duration = (uint8_t)Utility::getInput<uint32_t>(msg);

	msg = "[DisplayAction] Enter the display ID.";
	displayAction.displayID = (BS2_UID)Utility::getInput<uint32_t>(msg);

	msg = "[DisplayAction] Enter the resource ID.";
	displayAction.resourceID = (BS2_UID)Utility::getInput<uint32_t>(msg);
}

void setBuzzerAction(BS2BuzzerAction& buzzerAction)
{
	string msg = "[BuzzerAction] Enter the repeat count. (0: Infinite)";
	buzzerAction.count = (uint16_t)Utility::getInput<uint32_t>(msg);

	for (uint32_t idx = 0; idx < BS2_BUZZER_SIGNAL_NUM; idx++)
	{
		BS2BuzzerSignal& buzzerSignal = buzzerAction.signal[idx];
		msg = "[BuzzerAction] Enter the buzzer tone. (0: Off, 1: Low, 2: Middle, 3: High)";
		buzzerSignal.tone = (BS2_BUZZER_TONE)Utility::getInput<uint32_t>(msg);

		msg = "[BuzzerAction] Do you want to enable fade out.";
		buzzerSignal.fadeout = Utility::isYes(msg);

		msg = "[BuzzerAction] Enter the buzzer duration.";
		buzzerSignal.duration = (uint16_t)Utility::getInput<uint32_t>(msg);

		msg = "[BuzzerAction] Enter the buzzer delay.";
		buzzerSignal.delay = (uint16_t)Utility::getInput<uint32_t>(msg);
	}
}

void setLedAction(BS2LedAction& ledAction)
{
	string msg = "[LedAction] Enter the repeat count. (0: Infinite)";
	ledAction.count = (uint16_t)Utility::getInput<uint32_t>(msg);

	for (uint32_t idx = 0; idx < BS2_LED_SIGNAL_NUM; idx++)
	{
		BS2LedSignal& ledSignal = ledAction.signal[idx];

		ostringstream ss;
		ss << "[LedAction] Select the buzzer color" << endl;
		ss << "    0 : BS2_LED_COLOR_OFF" << endl;
		ss << "    1 : BS2_LED_COLOR_RED" << endl;
		ss << "    2 : BS2_LED_COLOR_YELLOW" << endl;
		ss << "    3 : BS2_LED_COLOR_GREEN" << endl;
		ss << "    4 : BS2_LED_COLOR_CYAN" << endl;
		ss << "    5 : BS2_LED_COLOR_BLUE" << endl;
		ss << "    6 : BS2_LED_COLOR_MAGENTA" << endl;
		ss << "    7 : BS2_LED_COLOR_WHITE" << endl;
		ss << "    8 : BS2_LED_COLOR_BLUE_BREATHING" << endl;
		ledSignal.color = (BS2_LED_COLOR)Utility::getInput<uint32_t>(ss.str());

		msg = "[LedAction] Enter the duration.";
		ledSignal.duration = (uint16_t)Utility::getInput<uint32_t>(msg);

		msg = "[LedAction] Enter the delay.";
		ledSignal.delay = (uint16_t)Utility::getInput<uint16_t>(msg);
	}
}

void setLiftAction(BS2LiftAction& liftAction)
{
	string msg = "[LiftAction] Enter the lift ID.";
	liftAction.liftID = (BS2_LIFT_ID)Utility::getInput<uint32_t>(msg);

	msg = "[LiftAction] Enter the lift action type. (0: Activate floors, 1: Deactivate floors, 2: Release floors)";
	liftAction.type = (BS2_LIFT_ACTION_TYPE)Utility::getInput<uint32_t>(msg);
}

int removeTriggerActionConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2TriggerActionConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	return cc.setTriggerActionConfig(id, config);
}

int updateDeviceVolume(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2DisplayConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getDisplayConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	ConfigControl::print(config);

#if 1
	config.volume = (uint8_t)Utility::getInput<uint32_t>("Please insert volumn of the device.");
	config.backlightTimeout = (uint16_t)Utility::getInput<uint32_t>("Please insert backlight timeout.");

	return cc.setDisplayConfig(id, config);
#else
	std::random_device rd;
	std::mt19937 engine(rd());
	std::uniform_int_distribution<int> dist(0, 20);

	for (uint16_t idx = 0; idx < 3000; idx++)
	{
		config.menuTimeout = (uint16_t)dist(engine);
		config.msgTimeout = (uint16_t)dist(engine);
		config.backlightTimeout = (uint16_t)dist(engine);

		sdkResult = cc.setDisplayConfig(id, config);

		Sleep(1 * 1000);
	}

	return sdkResult;
#endif
}

int getBarcodeConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2BarcodeConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getBarcodeConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

void onBarcodeScanned(BS2_DEVICE_ID id, const char* barcode)
{
	cout << "Device:" << id << ", Scanned barcode:" << barcode << endl;
}

int setBarcodeConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2BarcodeConfig config = { 0, };
	string msg;
	char buf[128] = { 0, };

	BS2_DEVICE_ID id(0);
	BS2_DEVICE_TYPE type(0);
	if (!Utility::getSelectedDeviceID(device, id, type))
		return BS_SDK_SUCCESS;

	int sdkResult = cc.getBarcodeConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	bool useBarcode = Utility::isYes("Would you like to use barcode function?");
	switch (type)
	{
	case BS2_DEVICE_TYPE_XSTATION_2_QR:
		config.useBarcode = useBarcode;
		if (useBarcode)
		{
			sprintf(buf, "Set the barcode scan timeout in seconds. (%u~%u)", BS2_BARCODE_TIMEOUT_MIN, BS2_BARCODE_TIMEOUT_MAX);
			msg = buf;
			config.scanTimeout = (uint8_t)Utility::getInput<uint32_t>(msg);
		}
		break;

	case BS2_DEVICE_TYPE_XSTATION_2_FP:		// Supported V1.2.0
	case BS2_DEVICE_TYPE_XSTATION_2:		// Supported V1.2.0
	case BS2_DEVICE_TYPE_BIOSTATION_3:		// Supported V1.1.0
	//case BS2_DEVICE_TYPE_BIOSTATION_3_MAX:
	//case BS2_DEVICE_TYPE_BIOSTATION_3_MAX_FP:
		config.useVisualBarcode = useBarcode;
		if (useBarcode)
		{
			sprintf(buf, "Set the visual barcode scan timeout in seconds. (%u~%u)", BS2_VISUAL_BARCODE_TIMEOUT_MIN, BS2_VISUAL_BARCODE_TIMEOUT_MAX);
			msg = buf;
			config.visualCameraScanTimeout = (uint8_t)Utility::getInput<uint32_t>(msg);

			sprintf(buf, "Set the motion sensitivity. (%u~%u)", BS2_MOTION_SENSITIVITY_LOW, BS2_MOTION_SENSITIVITY_HIGH);
			msg = buf;
			config.motionSensitivity = (uint8_t)Utility::getInput<uint32_t>(msg);
		}
		break;

	case BS2_DEVICE_TYPE_BIOENTRY_W3:
	default:
		return sdkResult;
	}

	config.bypassData = Utility::isYes("Would you like to use QR-bypass?");
	sdkResult = BS2Context::getInstance()->setBarcodeScanListener(config.bypassData ? onBarcodeScanned : NULL);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.treatAsCSN = Utility::isYes("Do you want the barcode to use only number like CSN?");

	return cc.setBarcodeConfig(id, config);
}

int turnOffQRBypass(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2BarcodeConfig config = { 0, };
	string msg;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getBarcodeConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.bypassData = false;

	sdkResult = cc.setBarcodeConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	return BS2Context::getInstance()->setBarcodeScanListener(NULL);
}

int turnOnQRBypass(void* context, const DeviceInfo& device)
{
	int sdkResult = BS2Context::getInstance()->setBarcodeScanListener(onBarcodeScanned);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	ConfigControl cc(context);
	BS2BarcodeConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	sdkResult = cc.getBarcodeConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.bypassData = true;

	return cc.setBarcodeConfig(id, config);
}

int getRS485Config(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2Rs485Config config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getRS485Config(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int getRS485ConfigEx(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2Rs485ConfigEX config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getRS485ConfigEx(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int getRS485ConfigExDynamic(void* context, const DeviceInfo& device)
{
	//ConfigControl cc(context);
	//BS2Rs485ConfigEXDynamic config = { 0, };

	//BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	//int sdkResult = cc.getRS485ConfigExDynamic(id, config);
	//if (BS_SDK_SUCCESS == sdkResult) {
	//	ConfigControl::print(config);

	//	//deallocating memory
	//	for(auto& channel: config.channels)
	//	{
	//		if (channel.slaveDevices != nullptr)
	//		{
	//			BS2_ReleaseObject(channel.slaveDevices);				
	//		}
	//	}
	//}

	return 1;
}

int getRS485ConfigExDynamicAndRemove(void* context, const DeviceInfo& device)
{
//	ConfigControl cc(context);
//	BS2Rs485ConfigEXDynamic config = { 0, };
//
//	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
//	int sdkResult = cc.getRS485ConfigExDynamic(id, config);
//	if (BS_SDK_SUCCESS == sdkResult) {
//		ConfigControl::print(config);
//
//		BS2Rs485SlaveDeviceEX rmSlaveList[BS2_RS485_MAX_SLAVES_PER_CHANNEL] = { 0, };
//		uint32_t rmCnt = 0;
//
//		for(uint32_t ch = 0; ch <  config.numOfChannels; ch++)
//		{
//			auto& channel = config.channels[ch];
//			rmCnt = 0;
//
//			for(uint32_t idx = 0; idx <  channel.numOfDevices; idx++)
//			{
//				rmSlaveList[rmCnt] = channel.slaveDevices[idx];
//				rmSlaveList[rmCnt].enableOSDP = 0;
//				rmCnt++;
//			}
//
//			for(uint32_t idx = 0; idx < rmCnt; idx++)
//			{				
//				TRACE("SlaveList to be removed");
//				TRACE("- ch:%d idx[%d] deviceID: %u, deviceType: %s", ch, idx,
//				rmSlaveList[idx].deviceID, Utility::getStringOfDeviceType(rmSlaveList[idx].deviceType).c_str());
//			}
//
//			TRACE("BS2_SetSlaveExDevice start, rmCnt[%d]", rmCnt);
//			BS2_SetSlaveExDevice(context, id,  0, rmSlaveList, rmCnt);
//			TRACE("BS2_SetSlaveExDevice end, rmCnt[%d]",rmCnt);
//}
//
//		//deallocating memory
//		for(uint32_t ch = 0; ch <  config.numOfChannels; ch++)
//		{
//			auto& channel = config.channels[ch];
//			if (channel.slaveDevices != nullptr)
//			{
//				BS2_ReleaseObject(channel.slaveDevices);
//			}
//		}
//	}

	return 1;
}

int setRS485Config(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	DeviceControl dc(context);
	BS2Rs485Config config = { 0, };
	string msg;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	BS2SimpleDeviceInfo info = {0,};
	int sdkResult = dc.getDeviceInfo(id, info);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	msg = "Please set the mode. Disable(%u), Master(%u), Slave(%u), Standalone(%u)";
	config.mode = (BS2_RS485_MODE)Utility::getInput<uint32_t>(msg, BS2_RS485_MODE_DISABLED, BS2_RS485_MODE_MASTER, BS2_RS485_MODE_SLAVE, BS2_RS485_MODE_STANDALONE);

	uint32_t numOfChannels =
		(BS2_DEVICE_TYPE_CORESTATION_40 == info.type) ? BS2_RS485_MAX_CHANNELS : 1;

	msg = "How many RS485 channels do you want to set up? (0 ~ %u)";
	config.numOfChannels = (uint8_t)Utility::getInput<uint32_t>(msg, numOfChannels);

	// The device applies only baudRate.
	for (uint8_t idx = 0; idx < config.numOfChannels; idx++)
	{
		msg = "Please insert baud rate. (Default: 115200)";
		config.channels[idx].baudRate = Utility::getInput<uint32_t>(msg);

		msg = "Please insert channel index.";
		config.channels[idx].channelIndex = (uint8_t)Utility::getInput<uint32_t>(msg);

		msg = "Please insert useRegistance.";
		config.channels[idx].useRegistance = (uint8_t)Utility::getInput<uint32_t>(msg);

		msg = "Please insert number of devices.";
		config.channels[idx].numOfDevices = (uint8_t)Utility::getInput<uint32_t>(msg);

		for (uint8_t slaveIdx = 0; slaveIdx < config.channels[idx].numOfDevices; slaveIdx++)
		{
			BS2Rs485SlaveDevice& slaveDevice = config.channels[idx].slaveDevices[slaveIdx];

			msg = "Please insert #%u deviceID.";
			slaveDevice.deviceID = Utility::getInput<BS2_DEVICE_ID>(msg, slaveIdx);

			msg = "Please insert #%u deviceType.";
			slaveDevice.deviceType = (uint16_t)Utility::getInput<uint32_t>(msg, slaveIdx);

			msg = "Please insert #%u enableOSDP.";
			slaveDevice.enableOSDP = (uint8_t)Utility::getInput<uint32_t>(msg, slaveIdx);

			msg = "Please insert #%u connected.";
			slaveDevice.connected = (uint8_t)Utility::getInput<uint32_t>(msg, slaveIdx);
		}
	}

	msg = "Would you like to use IntelligentPD-related settings?";
	config.intelligentInfo.supportConfig = Utility::isYes(msg);

	if (config.intelligentInfo.supportConfig)
	{
		msg = "Would you like to use an exception code?";
		config.intelligentInfo.useExceptionCode = Utility::isYes(msg);
		if (config.intelligentInfo.useExceptionCode)
		{
			stringstream streamMsg;
			streamMsg << "Please enter the exception code in 8 bytes hexa." << endl;
			streamMsg << ">> 0x";
			string enteredCode = Utility::getInput<string>(streamMsg.str());
			string exceptionCode = Utility::convertString2HexByte(enteredCode);
			memcpy(config.intelligentInfo.exceptionCode, exceptionCode.c_str(),
				min(BS2_RS485_MAX_EXCEPTION_CODE_LEN, exceptionCode.size()));
		}

		msg = "Please enter the output format. CardID(%u), UserID(%u)";
		config.intelligentInfo.outputFormat = (uint8_t)Utility::getInput<uint32_t>(msg, BS2_IPD_OUTPUT_CARDID, BS2_IPD_OUTPUT_USERID);

		msg = "Please enter the OSDP ID.";
		config.intelligentInfo.osdpID = (uint8_t)Utility::getInput<uint32_t>(msg);
	}

	return cc.setRS485Config(id, config);
}

int setRS485ConfigEx(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	DeviceControl dc(context);
	BS2Rs485ConfigEX config = { 0, };
	string msg;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	BS2SimpleDeviceInfo info = {0,};
	int sdkResult = dc.getDeviceInfo(id, info);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	uint32_t numOfChannels =
		(BS2_DEVICE_TYPE_CORESTATION_40 == info.type) ? BS2_RS485_MAX_CHANNELS : 1;

	msg = "How many RS485 channels do you want to set up? (0 ~ %u)";
	config.numOfChannels = (uint8_t)Utility::getInput<uint32_t>(msg, numOfChannels);


	// numOfChannels and mode are fixed base on the device type.
	// Use BS2_GetSlaveDevice()/BS2_SetSlaveDevice() for slave device control.

	for (uint8_t idx = 0; idx < config.numOfChannels; idx++)
	{
		msg = "Please insert baud rate. (Default: 115200)";
		config.channels[idx].baudRate = Utility::getInput<uint32_t>(msg);
	
		config.mode[idx] = BS2_RS485_MODE_MASTER;
#if 0
		msg = "Please insert channel index.";
		config.channels[idx].channelIndex = (uint8_t)Utility::getInput<uint32_t>(msg);

		msg = "Please insert useRegistance.";
		config.channels[idx].useRegistance = (uint8_t)Utility::getInput<uint32_t>(msg);
		
		msg = "Please insert channelType.";
		config.channels[idx].channelType = (uint8_t)Utility::getInput<uint32_t>(msg);

		msg = "Please insert number of devices.";
		config.channels[idx].numOfDevices = (uint8_t)Utility::getInput<uint32_t>(msg);

		for (uint8_t slaveIdx = 0; slaveIdx < config.channels[idx].numOfDevices; slaveIdx++)
		{
			BS2Rs485SlaveDeviceEX& slaveDevice = config.channels[idx].slaveDevices[slaveIdx];

			msg = "Please insert #%u deviceID.";
			slaveDevice.deviceID = Utility::getInput<BS2_DEVICE_ID>(msg, slaveIdx);

			msg = "Please insert #%u deviceType.";
			slaveDevice.deviceType = (uint16_t)Utility::getInput<uint32_t>(msg, slaveIdx);

			msg = "Please insert #%u enableOSDP.";
			slaveDevice.enableOSDP = (uint8_t)Utility::getInput<uint32_t>(msg, slaveIdx);

			msg = "Please insert #%u connected.";
			slaveDevice.connected = (uint8_t)Utility::getInput<uint32_t>(msg, slaveIdx);
			
			msg = "Please insert #%u connected.";
			slaveDevice.connected = (uint8_t)Utility::getInput<uint32_t>(msg, slaveIdx);
		}
#endif
	}

	return cc.setRS485ConfigEx(id, config);
}

int setRS485ConfigExDynamic(void* context, const DeviceInfo& device)
{
//	ConfigControl cc(context);
//	DeviceControl dc(context);
//	BS2Rs485ConfigEXDynamic config = { 0, };
//	string msg;
//
//	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
//
//	BS2SimpleDeviceInfo info = {0,};
//	int sdkResult = dc.getDeviceInfo(id, info);
//	if (BS_SDK_SUCCESS != sdkResult)
//		return sdkResult;
//
//	TRACE("deviceType : %s(0x%02X)",  Utility::getStringOfDeviceType(info.type).c_str(), info.type);
//
//	sdkResult =	cc.getRS485ConfigExDynamic(id, config);
//	if (BS_SDK_SUCCESS != sdkResult)
//		return sdkResult;
//	ConfigControl::print(config);
//
//	// the device only cares about baudrates and ignores the rest.
//	// Use BS2_GetSlaveDevice()/BS2_SetSlaveDevice() to control slave device connection.
//#if 0
//	// get channel count from BS2DeviceCapabilities.maxRS485Channels
//	switch (info.type)
//	{
//	case BS2_DEVICE_TYPE_CORESTATION_20:
//	case BS2_DEVICE_TYPE_DOOR_INTERFACE_24:
//		config.numOfChannels = 3;
//		break;
//	default:
//		config.numOfChannels = 1;
//		break;
//	}
//#endif
//
//	for (uint8_t ch = 0; ch < config.numOfChannels; ch++)
//	{
//#if 0
//		if (info.type == BS2_DEVICE_TYPE_DOOR_INTERFACE_24)
//		{			
//			if (ch < (config.numOfChannels - 1)) config.mode[ch] = BS2_RS485_MODE_MASTER;
//			else config.mode[ch] = BS2_RS485_MODE_SLAVE;
//		}
//		else if (info.type == BS2_DEVICE_TYPE_CORESTATION_20)
//		{			
//			config.mode[ch] = BS2_RS485_MODE_MASTER;
//		}
//		TRACE("channel %u mode: %u", ch, config.mode[ch]);
//
//		config.channels[ch].channelIndex = (uint8_t)ch;		
//		config.channels[ch].numOfDevices = 0;
//		config.channels[ch].slaveDevices = nullptr;	
//#endif
//		msg = "Please insert baud rate (Default: 115200)";
//		config.channels[ch].baudRate = Utility::getInput<uint32_t>(msg);
//	}
//
	return 1;
}

int getDeviceCapabilities(void* context, const DeviceInfo& device)
{
	DeviceControl dc(context);
	BS2DeviceCapabilities cap = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = dc.getDeviceCapabilities(id, cap);
	if (BS_SDK_SUCCESS == sdkResult)
		DeviceControl::print(cap);

	return sdkResult;
}

int getInputConfigEx(void* context, const DeviceInfo& device)
{
	// As of 2021.08.03, only IM-120 is supported
	//ConfigControl cc(context);
	//BS2InputConfigEx config = { 0, };

	//BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	//int sdkResult = cc.getInputConfigEx(id, config);
	//if (BS_SDK_SUCCESS == sdkResult)
	//	ConfigControl::print(config);

	return 1;
}

int setInputConfigEx(void* context, const DeviceInfo& device)
{
	//// As of 2021.08.03, only IM-120 is supported
	//ConfigControl cc(context);
	//BS2InputConfigEx config = { 0, };
	//string msg;

	//BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	//const int STOP_N_SET = -1;

	//int sdkResult = cc.getInputConfigEx(id, config);
	//if (BS_SDK_SUCCESS != sdkResult)
	//	return sdkResult;

	//msg = "Please enter number of inputs.";
	//config.numInputs = (uint8_t)Utility::getInput<uint32_t>(msg);

	//msg = "Please enter number of supervised inputs.";
	//config.numSupervised = (uint8_t)Utility::getInput<uint32_t>(msg);

	//while (true)
	//{
	//	msg = "What input port would you like to set? [-1(Exit), 0, ..., %d]";
	//	int idx = Utility::getInput<int>(msg, config.numSupervised - 1);
	//	if (STOP_N_SET == idx)
	//		break;

	//	config.inputs[idx].portIndex = (uint8_t)idx;

	//	msg = "Please enter the switch type. (N/O: 0, N/C: 1)";
	//	config.inputs[idx].switchType = (BS2_SWITCH_TYPE)Utility::getInput<uint32_t>(msg);

	//	msg = "Please enter the duration.";
	//	config.inputs[idx].duration = (uint16_t)Utility::getInput<uint32_t>(msg);

	//	stringstream strmMsg;
	//	strmMsg << "Please enter the type of resistance value for supervised input." << endl;
	//	strmMsg << "[0: 1K, 1: 2.2K, 2: 4.7K, 3: 10K, 254: Unsupervised]";
	//	config.inputs[idx].supervisedResistor = (uint8_t)Utility::getInput<uint32_t>(strmMsg.str());

	//	if (config.inputs[idx].supervisedResistor == SUPERVISED_REG_CUSTOM)
	//	{
	//		msg = "Please enter shortInput.minValue.";
	//		config.inputs[idx].supervisedConfig.shortInput.minValue = (uint16_t)Utility::getInput<uint32_t>(msg);
	//		msg = "Please enter shortInput.maxValue.";
	//		config.inputs[idx].supervisedConfig.shortInput.maxValue = (uint16_t)Utility::getInput<uint32_t>(msg);

	//		msg = "Please enter openInput.minValue.";
	//		config.inputs[idx].supervisedConfig.openInput.minValue = (uint16_t)Utility::getInput<uint32_t>(msg);
	//		msg = "Please enter openInput.maxValue.";
	//		config.inputs[idx].supervisedConfig.openInput.maxValue = (uint16_t)Utility::getInput<uint32_t>(msg);

	//		msg = "Please enter onInput.minValue.";
	//		config.inputs[idx].supervisedConfig.onInput.minValue = (uint16_t)Utility::getInput<uint32_t>(msg);
	//		msg = "Please enter onInput.maxValue.";
	//		config.inputs[idx].supervisedConfig.onInput.maxValue = (uint16_t)Utility::getInput<uint32_t>(msg);

	//		msg = "Please enter offInput.minValue.";
	//		config.inputs[idx].supervisedConfig.offInput.minValue = (uint16_t)Utility::getInput<uint32_t>(msg);
	//		msg = "Please enter offInput.maxValue.";
	//		config.inputs[idx].supervisedConfig.offInput.maxValue = (uint16_t)Utility::getInput<uint32_t>(msg);
	//	}
	//}

	////Aux index
	//msg = "Please enter acFailAuxIndex (0~2) : ";
	//uint16_t value = (uint16_t)Utility::getInput<uint32_t>(msg);
	//if (value > 2)
	//{
	//	cout << "Invalid parameter" << endl;
	//	return BS_SDK_ERROR_INVALID_CONFIG;
	//}
	//config.aux.field.acFailAuxIndex = value + 1;

	//msg = "Please enter TamperAuxIndex (0~2) : ";
	//value = (uint16_t)Utility::getInput<uint32_t>(msg);
	//if (value > 2)
	//{
	//	cout << "Invalid parameter" << endl;
	//	return BS_SDK_ERROR_INVALID_CONFIG;
	//}
	//config.aux.field.tamperAuxIndex = value + 1;

	//msg = "Please enter FireAuxIndex (0~2) : ";
	//value = (uint16_t)Utility::getInput<uint32_t>(msg);
	//if (value > 2)
	//{
	//	cout << "Invalid parameter" << endl;
	//	return BS_SDK_ERROR_INVALID_CONFIG;
	//}
	//config.aux.field.fireAuxIndex = value + 1;

	////Aux Type
	//msg = "Please enter aux0 switchType (0:NO, 1:NC) : ";
	//value = (uint16_t)Utility::getInput<uint32_t>(msg);
	//config.aux.field.aux0Type = value;
	//if (value > 1)
	//{
	//	cout << "Invalid parameter" << endl;
	//	return BS_SDK_ERROR_INVALID_CONFIG;
	//}
	//
	//msg = "Please enter aux1 switchType (0:NO, 1:NC) : ";
	//value = (uint16_t)Utility::getInput<uint32_t>(msg);
	//config.aux.field.aux1Type = value;
	//if (value > 1)
	//{
	//	cout << "Invalid parameter" << endl;
	//	return BS_SDK_ERROR_INVALID_CONFIG;
	//}

	//msg = "Please enter aux2 switchType (0:NO, 1:NC) : ";
	//value = (uint16_t)Utility::getInput<uint32_t>(msg);
	//config.aux.field.aux2Type = value;
	//if (value > 1)
	//{
	//	cout << "Invalid parameter" << endl;
	//	return BS_SDK_ERROR_INVALID_CONFIG;
	//}

	return 1;
}

int getRelayActionConfig(void* context, const DeviceInfo& device)
{
	// As of 2021.08.03, only IM-120 is supported
	ConfigControl cc(context);
	BS2RelayActionConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getRelayActionConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setRelayActionConfig(void* context, const DeviceInfo& device)
{
	// As of 2021.08.03, only IM-120 is supported
	ConfigControl cc(context);
	BS2RelayActionConfig config = { 0, };
	string msg;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	const int STOP_N_SET = -1;

	int sdkResult = cc.getRelayActionConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.deviceID = id;

	while (true)
	{
		msg = "What relay port would you like to set? [-1(Exit), 0, ..., %d]";
		int idxRelay = Utility::getInput<int>(msg, BS2_MAX_RELAY_ACTION - 1);
		if (STOP_N_SET == idxRelay)
			break;

		config.relay[idxRelay].port = (uint8_t)idxRelay;

		msg = "Do you want to set an alarm for RS485 disconnection?";
		config.relay[idxRelay].disconnEnabled = Utility::isYes(msg);

		while (true)
		{
			msg = "What input port would you like to set? [-1(Exit), 0, ..., %d]";
			int idxInput = Utility::getInput<int>(msg, BS2_MAX_RELAY_ACTION_INPUT - 1);
			if (STOP_N_SET == idxInput)
				break;

			config.relay[idxRelay].input[idxInput].port = (uint8_t)idxInput;

			msg = "Please enter the type of relay action input [0: None, 1: Linkage]";
			config.relay[idxRelay].input[idxInput].type = (BS2_RELAY_ACTION_INPUT_TYPE)Utility::getInput<uint32_t>(msg);

			msg = "Please enter the mask of relay action input [0: None, 0x01: Alarm, 0x02: Fault]";
			config.relay[idxRelay].input[idxInput].mask = (BS2_RELAY_ACTION_INPUT_MASK)Utility::getInput<uint32_t>(msg);
		}
	}

	return cc.setRelayActionConfig(id, config);
}

int getWLANConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2WlanConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getWLANConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setWLANConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2WlanConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getWLANConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	string msg = "Do you want to use the WLAN?";
	if (Utility::isYes(msg))
	{
		config.enabled = true;

		msg = "Select the operation mode of the WLAN. [0: Infrastructure, 1: Ad-hoc]";
		config.operationMode = (BS2_WLAN_OPMODE)Utility::getInput<uint32_t>(msg);

		ostringstream methodMsg;
		methodMsg << "Select the WLAN authentication method." << endl;
		methodMsg << " - 0: Open authentication" << endl;
		methodMsg << " - 1: Shared authentication" << endl;
		methodMsg << " - 2: WPA-PSK" << endl;
		methodMsg << " - 3: WPA2-PSK" << endl;
		config.authType = (BS2_WLAN_AUTH_TYPE)Utility::getInput<uint32_t>(methodMsg.str());

		ostringstream encMsg;
		encMsg << "Select the WLAN encryption method." << endl;
		encMsg << " - 0: None" << endl;
		encMsg << " - 1: WEP" << endl;
		encMsg << " - 2: TKIP/AES" << endl;
		encMsg << " - 3: AES" << endl;
		encMsg << " - 4: TKIP" << endl;
		config.encryptionType = (BS2_WLAN_ENC_TYPE)Utility::getInput<uint32_t>(encMsg.str());

ESSID_AGAIN:
		msg = "Enter the ESSID of the WLAN?";
		string essID = Utility::getInput<string>(msg);
		if (BS2_WLAN_SSID_SIZE < essID.size())
		{
			cout << "Max ESSID size is " << BS2_WLAN_SSID_SIZE << endl;
			goto ESSID_AGAIN;
		}
		memset(config.essid, 0x0, BS2_WLAN_SSID_SIZE);
		strcpy(config.essid, essID.c_str());

AUTHKEY_AGAIN:
		msg = "Enter the authentication key of the WLAN?";
		string authKey = Utility::getInput<string>(msg);
		if (BS2_WLAN_KEY_SIZE < authKey.size())
		{
			cout << "Max Authentication key size is " << BS2_WLAN_KEY_SIZE << endl;
			goto AUTHKEY_AGAIN;
		}
		memset(config.authKey, 0x0, BS2_WLAN_KEY_SIZE);
		strcpy(config.authKey, authKey.c_str());
	}
	else
	{
		config.enabled = false;
	}

	return cc.setWLANConfig(id, config);
}

int getWiegandConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2WiegandConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getWiegandConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setWiegandConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2WiegandConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getWiegandConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	string msg = "Select the wiegand I/O mode. (0: Input, 1: Output, 2: Input/Output)";
	config.mode = (BS2_WIEGAND_MODE)Utility::getInput<uint32_t>(msg);
	if (config.mode == BS2_WIEGAND_OUT_ONLY || config.mode == BS2_WIEGAND_IN_OUT)
	{
		msg = "Do you want to use the WiegandBypass?";
		config.useWiegandBypass = Utility::isYes(msg);

		if (!config.useWiegandBypass)
		{
			msg = "Do you want to use the FailCode?";
			config.useFailCode = Utility::isYes(msg);
			if (config.useFailCode)
			{
				msg = "Enter the FAILCODE in hexa-decimal 1byte like 0xFF.";
				config.failCode = Utility::getInputHexaChar<uint8_t>(msg);
			}
		}
	}

	msg = "Enter the outPulseWidth. (20~100 us, Default = 40)";
	config.outPulseWidth = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "Enter the outPulseInterval. (200~20000 us, Default = 10000)";
	config.outPulseInterval = (uint16_t)Utility::getInput<uint32_t>(msg);

	msg = "Enter the wiegand format ID.";
	config.formatID = (BS2_UID)Utility::getInput<uint32_t>(msg);

	// Format
	msg = "Enter the LENGTH of the wiegand card format.";
	config.format.length = Utility::getInput<uint32_t>(msg);

	ostringstream iss;
	iss << "Enter the ID FIELDs of the wiegand card. (Max. 32bytes)" << endl;
	iss << "If you type 01 FE 00 00, " << endl;
	iss << "then I will help insert to '0000................000001FE0000'";

	uint32_t numOfField = Utility::getInput<uint32_t>("How many ID fields would you like to register?");
	for (uint32_t idx = 0; idx < numOfField; idx++)
	{
		memset(config.format.idFields[idx], 0x0, BS2_WIEGAND_FIELD_SIZE);
		ostringstream oss;
		oss << iss.str() << endl;
		oss << "[" << idx << "] ";
		Utility::getLineWiegandBits<uint8_t>(oss.str(), config.format.idFields[idx], BS2_WIEGAND_FIELD_SIZE);
	}

	ostringstream pss;
	pss << "Enter the PARITY FIELDs of the wiegand card. (Max. 32bytes)" << endl;
	pss << "If you type 01 FF E0 00, " << endl;
	pss << "then I will help insert to '0000................000001FFE000'";

	numOfField = Utility::getInput<uint32_t>("How many parity fields would you like to register?");
	for (uint32_t idx = 0; idx < numOfField; idx++)
	{
		memset(config.format.parityFields[idx], 0x0, BS2_WIEGAND_FIELD_SIZE);
		ostringstream oss;
		oss << pss.str() << endl;
		oss << "[" << idx << "] ";
		Utility::getLineWiegandBits<uint8_t>(oss.str(), config.format.parityFields[idx], BS2_WIEGAND_FIELD_SIZE);

		msg = "Select the PARITY TYPE. (0: No check, 1: Check odd parity, 2: Check even parity)";
		oss.str("");
		oss << "[" << idx << "] " << msg;
		config.format.parityType[idx] = (BS2_WIEGAND_PARITY)Utility::getInput<uint32_t>(oss.str());

		msg = "Enter the PARITY POS.";
		oss.str("");
		oss << "[" << idx << "] " << msg;
		config.format.parityPos[idx] = (uint8_t)Utility::getInput<uint32_t>(oss.str());
	}

	if (config.mode == BS2_WIEGAND_IN_ONLY || config.mode == BS2_WIEGAND_IN_OUT)
	{
		ostringstream oss;
		oss << "Enter the WIEGAND CARD MASK for the wiegand input." << endl;
		oss << "The device will accept wiegand signals that the configured formats." << endl;
		config.wiegandInputMask = selectWiegandFormat(oss);
	}

	BS2CardConfig cardConfig = { 0, };
	sdkResult = cc.getCardConfig(id, cardConfig);
	if (sdkResult != BS_SDK_SUCCESS)
		return sdkResult;

	msg = "Do you want the device to process CSN cards with wiegand formats?";
	if (Utility::isYes(msg))
	{
		cardConfig.useWiegandFormat = true;
		cc.setCardConfig(id, cardConfig);

		ostringstream oss;
		oss << "Enter the CSN WIEGAND CARD INDEX for the device." << endl;
		for (const auto& item : WIEGAND_FORMAT_PRESET)
			oss << "  " << item.first << ": " << item.second << endl;

		config.wiegandCSNIndex = (uint8_t)(Utility::getInput<uint32_t>(oss.str()) + 1);
	}
	else
	{
		cardConfig.useWiegandFormat = false;
		cc.setCardConfig(id, cardConfig);
	}

	{
		ostringstream oss;
		oss << "Enter the WIEGAND CARD MASK for the device." << endl;
		oss << "The device will accept CARDs that matches the configured formats." << endl;
		config.wiegandCardMask = selectWiegandFormat(oss);
	}

	if (config.mode == BS2_WIEGAND_OUT_ONLY || config.mode == BS2_WIEGAND_IN_OUT)
	{
		msg = "Select the Wiegand data output FLAG. (0: None, 1: CardID, 2: UserID)";
		config.useWiegandUserID = (uint8_t)Utility::getInput<uint32_t>(msg);
	}

	return cc.setWiegandConfig(id, config);
}

uint16_t selectWiegandFormat(ostringstream& oss)
{
	uint32_t mask(0);
	oss << "Select 0, 1, 2, ..." << endl;
	for (const auto& item : WIEGAND_FORMAT_PRESET)
		oss << "  " << item.first << ": " << item.second << endl;

	auto inputDatas = Utility::getLineNumbers<uint32_t>(oss.str(), ',');
	for (auto item : inputDatas)
		mask |= (0x01 << item);

	mask = mask << 0x01;	// Not using 0th bit.

	return (uint16_t)mask;
}


int setWiegandMultiConfigWithPreset(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2WiegandMultiConfig config = { 0, };

	//////////////////////////////////////////////////////////////////////////
	// H10301 26 bit format
	config.formats[0].formatID = 1;
	config.formats[0].format.length = 26;

	config.formats[0].format.idFields[0][28] = 0x01;
	config.formats[0].format.idFields[0][29] = 0xFE;

	config.formats[0].format.idFields[1][29] = 0x01;
	config.formats[0].format.idFields[1][30] = 0xFF;
	config.formats[0].format.idFields[1][31] = 0xFE;

	config.formats[0].format.parityFields[0][28] = 0x01;
	config.formats[0].format.parityFields[0][29] = 0xFF;
	config.formats[0].format.parityFields[0][30] = 0xE0;
	config.formats[0].format.parityType[0] = BS2_WIEGAND_PARITY_EVEN;
	config.formats[0].format.parityPos[0] = 0;

	config.formats[0].format.parityFields[1][30] = 0x1F;
	config.formats[0].format.parityFields[1][31] = 0xFE;
	config.formats[0].format.parityType[1] = BS2_WIEGAND_PARITY_ODD;
	config.formats[0].format.parityPos[1] = 25;

	//////////////////////////////////////////////////////////////////////////
	// H10302 37 bit format
	config.formats[1].formatID = 2;
	config.formats[1].format.length = 37;

	// H10302 uses only 1 field. The first field is from [0] ~ [31]
	// If you convert the bits used on the field in binary, it is as below. 35 bit as card ID.
	// 0000 1111 / 1111 1111 / 1111 1111 / 1111 1111 / 1111 1110        -> 0F / FF / FF / FF / FE
	//     27    /     28    /     29    /    30     /    31
	config.formats[1].format.idFields[0][27] = 0x0F;
	config.formats[1].format.idFields[0][28] = 0xFF;
	config.formats[1].format.idFields[0][29] = 0xFF;
	config.formats[1].format.idFields[0][30] = 0xFF;
	config.formats[1].format.idFields[0][31] = 0xFE;

	// According to H10302, the first even parity calculates the bits starting from 1 ~ 18
	// 000 0<parity bit 1111 / 1111 1111 / 1111 1100                -> 0F / FF / FC
	//           27         /     28    /     29
	config.formats[1].format.parityFields[0][27] = 0x0F;
	config.formats[1].format.parityFields[0][28] = 0xFF;
	config.formats[1].format.parityFields[0][29] = 0xFC;
	config.formats[1].format.parityPos[0] = 0;
	config.formats[1].format.parityType[0] = BS2_WIEGAND_PARITY_EVEN;

	// The second parity calculates the bits starting from 18 ~ 35. Since this is for the second parity bit,
	// parityFields[1][0] ~ [1][31] is used.
	// 0000 0111 / 1111 1111 / 1111 111 0<parity bit
	//     29    /     30    /     31
	config.formats[1].format.parityFields[1][29] = 0x07;
	config.formats[1].format.parityFields[1][30] = 0xFF;
	config.formats[1].format.parityFields[1][31] = 0xFE;
	config.formats[1].format.parityType[1] = BS2_WIEGAND_PARITY_ODD;
	config.formats[1].format.parityPos[1] = 36;

	//////////////////////////////////////////////////////////////////////////
	// H10304 37 bit format
	config.formats[2].formatID = 3;
	config.formats[2].format.length = 37;

	config.formats[2].format.idFields[0][27] = 0x0F;
	config.formats[2].format.idFields[0][28] = 0xFF;
	config.formats[2].format.idFields[0][29] = 0xF0;

	config.formats[2].format.idFields[1][29] = 0x0F;
	config.formats[2].format.idFields[1][30] = 0xFF;
	config.formats[2].format.idFields[1][31] = 0xFE;

	config.formats[2].format.parityFields[0][27] = 0x0F;
	config.formats[2].format.parityFields[0][28] = 0xFF;
	config.formats[2].format.parityFields[0][29] = 0xFC;
	config.formats[2].format.parityType[0] = BS2_WIEGAND_PARITY_EVEN;
	config.formats[2].format.parityPos[0] = 0;

	config.formats[2].format.parityFields[1][29] = 0x07;
	config.formats[2].format.parityFields[1][30] = 0xFF;
	config.formats[2].format.parityFields[1][31] = 0xFE;
	config.formats[2].format.parityType[1] = BS2_WIEGAND_PARITY_ODD;
	config.formats[2].format.parityPos[1] = 36;

	//////////////////////////////////////////////////////////////////////////
	// Corporate 1000 35 bit format
	config.formats[3].formatID = 4;
	config.formats[3].format.length = 35;

	config.formats[3].format.idFields[0][27] = 0x01;
	config.formats[3].format.idFields[0][28] = 0xFF;
	config.formats[3].format.idFields[0][29] = 0xE0;

	config.formats[3].format.idFields[1][29] = 0x1F;
	config.formats[3].format.idFields[1][30] = 0xFF;
	config.formats[3].format.idFields[1][31] = 0xFE;

	config.formats[3].format.parityFields[0][27] = 0x01;
	config.formats[3].format.parityFields[0][28] = 0xB6;
	config.formats[3].format.parityFields[0][29] = 0xDB;
	config.formats[3].format.parityFields[0][30] = 0x6D;
	config.formats[3].format.parityFields[0][31] = 0xB6;
	config.formats[3].format.parityType[0] = BS2_WIEGAND_PARITY_EVEN;
	config.formats[3].format.parityPos[0] = 1;

	config.formats[3].format.parityFields[1][27] = 0x03;
	config.formats[3].format.parityFields[1][28] = 0x6D;
	config.formats[3].format.parityFields[1][29] = 0xB6;
	config.formats[3].format.parityFields[1][30] = 0xDB;
	config.formats[3].format.parityFields[1][31] = 0x6C;
	config.formats[3].format.parityType[1] = BS2_WIEGAND_PARITY_ODD;
	config.formats[3].format.parityPos[1] = 34;

	config.formats[3].format.parityFields[2][27] = 0x03;
	config.formats[3].format.parityFields[2][28] = 0xFF;
	config.formats[3].format.parityFields[2][29] = 0xFF;
	config.formats[3].format.parityFields[2][30] = 0xFF;
	config.formats[3].format.parityFields[2][31] = 0xFF;
	config.formats[3].format.parityType[2] = BS2_WIEGAND_PARITY_ODD;
	config.formats[3].format.parityPos[2] = 0;

	//////////////////////////////////////////////////////////////////////////
	// Corporate 1000 48 bit format
	config.formats[4].formatID = 5;
	config.formats[4].format.length = 48;

	config.formats[4].format.idFields[0][26] = 0x3F;
	config.formats[4].format.idFields[0][27] = 0xFF;
	config.formats[4].format.idFields[0][28] = 0xFF;

	config.formats[4].format.idFields[1][29] = 0xFF;
	config.formats[4].format.idFields[1][30] = 0xFF;
	config.formats[4].format.idFields[1][31] = 0xFE;

	config.formats[4].format.parityFields[0][26] = 0x1B;
	config.formats[4].format.parityFields[0][27] = 0x6D;
	config.formats[4].format.parityFields[0][28] = 0xB6;
	config.formats[4].format.parityFields[0][29] = 0xDB;
	config.formats[4].format.parityFields[0][30] = 0x6D;
	config.formats[4].format.parityFields[0][31] = 0xB6;
	config.formats[4].format.parityType[0] = BS2_WIEGAND_PARITY_EVEN;
	config.formats[4].format.parityPos[0] = 1;

	config.formats[4].format.parityFields[1][26] = 0x36;
	config.formats[4].format.parityFields[1][27] = 0xDB;
	config.formats[4].format.parityFields[1][28] = 0x6D;
	config.formats[4].format.parityFields[1][29] = 0xB6;
	config.formats[4].format.parityFields[1][30] = 0xDB;
	config.formats[4].format.parityFields[1][31] = 0x6C;
	config.formats[4].format.parityType[1] = BS2_WIEGAND_PARITY_ODD;
	config.formats[4].format.parityPos[1] = 47;

	config.formats[4].format.parityFields[2][26] = 0x7F;
	config.formats[4].format.parityFields[2][27] = 0xFF;
	config.formats[4].format.parityFields[2][28] = 0xFF;
	config.formats[4].format.parityFields[2][29] = 0xFF;
	config.formats[4].format.parityFields[2][30] = 0xFF;
	config.formats[4].format.parityFields[2][31] = 0xFF;
	config.formats[4].format.parityType[2] = BS2_WIEGAND_PARITY_ODD;
	config.formats[4].format.parityPos[2] = 0;

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	return cc.setWiegandMultiConfig(id, config);
}

int getVoipConfigExt(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2VoipConfigExt config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getVoipConfigExt(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setVoipConfigExt(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2VoipConfigExt config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getVoipConfigExt(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	string msg = "Do you want to use the VoIP Extension?";
	if (Utility::isYes(msg))
	{
		config.enabled = true;

		msg = "Enter the interval in seconds to update the information on the SIP server. (60~600)";
		config.registrationDuration = (uint16_t)Utility::getInput<uint32_t>(msg);

		msg = "Enter the IP address of the SIP server.";
		string ipAddr = Utility::getInput<string>(msg);
		memset(config.address, 0x0, BS2_URL_SIZE);
		memcpy(config.address, ipAddr.c_str(), ipAddr.size());

		msg = "Enter the port of the SIP server. (default: 5060)";
		config.port = (BS2_PORT)Utility::getInput<uint32_t>(msg);

		msg = "Enter the intercom speaker volume between 0 and 100. (default: 50)";
		config.volume.speaker = (uint8_t)Utility::getInput<uint32_t>(msg);

		msg = "Enter the intercom speaker microphone between 0 and 100. (default: 50)";
		config.volume.mic = (uint8_t)Utility::getInput<uint32_t>(msg);

		msg = "Enter the ID to connect to the SIP server.";
		string sipID = Utility::getInput<string>(msg);
		memset(config.id, 0x0, BS2_USER_ID_SIZE);
		memcpy(config.id, sipID.c_str(), sipID.size());

		msg = "Enter the password to connect to the SIP server.";
		string sipPW = Utility::getInput<string>(msg);
		memset(config.password, 0x0, BS2_USER_ID_SIZE);
		memcpy(config.password, sipPW.c_str(), sipPW.size());

		msg = "Enter the authorization code to connect to the SIP server.";
		string authCode = Utility::getInput<string>(msg);
		memset(config.authorizationCode, 0x0, BS2_USER_ID_SIZE);
		memcpy(config.authorizationCode, authCode.c_str(), authCode.size());

		msg = "Do you want to use Outbound proxy?";
		config.useOutboundProxy = (BS2_BOOL)Utility::isYes(msg);

		if (config.useOutboundProxy) {
			msg = "Enter the address of the Outbound proxy server.";
			string proxyAddr = Utility::getInput<string>(msg);
			memset(config.outboundProxy.address, 0x0, BS2_URL_SIZE);
			memcpy(config.outboundProxy.address, proxyAddr.c_str(), proxyAddr.size());

			msg = "Enter the port of the Outbound proxy server.";
			config.outboundProxy.port = (BS2_PORT)Utility::getInput<uint32_t>(msg);
		}

		msg = "Select the button symbol to be used as the exit button. (*, #, 0 ~ 9)";
		config.exitButton = (uint8_t)Utility::getInput<char>(msg);

		msg = "Do you want to show the extension phone book?";
		config.showExtensionNumber = (BS2_BOOL)Utility::isYes(msg);

		msg = "How many extension numbers would you like to register? (MAX: 128)";
		config.numPhoneBook = (uint8_t)Utility::getInput<uint32_t>(msg);

		if (config.numPhoneBook) {
			memset(config.phonebook, 0x0, sizeof(config.phonebook));
			for (uint8_t idx = 0; idx < config.numPhoneBook; idx++)
			{
				ostringstream msgStrm;
				msgStrm << "Enter the extension phone number #" << static_cast<int>(idx);
				string phoneNum = Utility::getInput<string>(msgStrm.str());
				memcpy(config.phonebook[idx].phoneNumber, phoneNum.c_str(), phoneNum.size());

				msgStrm.str("");
				msgStrm << "Enter the extension phone number #" << static_cast<int>(idx) << " description";
				string phoneDesc = Utility::getInput<string>(msgStrm.str());
				memcpy(config.phonebook[idx].description, phoneDesc.c_str(), phoneDesc.size());
			}
		}

		//msg = "Select video resolution (0:360x640, 1: 720x480, default:0)";
		//config.resolution = (uint8_t)Utility::getInput<uint32_t>(msg);

		//msg = "Select the transport (0:UDP, 1:TCP, 2:TLS, default:0)";
		//config.transport = (uint8_t)Utility::getInput<uint32_t>(msg);
	}
	else
	{
		config.enabled = false;
	}

	return cc.setVoipConfigExt(id, config);
}

int getRtspConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2RtspConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getRtspConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setRtspConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2RtspConfig config = { 0, };

	BS2_DEVICE_ID id = 0;
	BS2_DEVICE_TYPE type = BS2_DEVICE_TYPE_UNKNOWN;
	if (!Utility::getSelectedDeviceID(device, id, type))
		return BS_SDK_SUCCESS;

	int sdkResult = cc.getRtspConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	string msg = "Do you want to use the RTSP server?";
	if (Utility::isYes(msg))
	{
		config.enabled = true;

		msg = "Enter the account for the RTSP server.";
		string acc = Utility::getInput<string>(msg);
		memset(config.id, 0x0, BS2_USER_ID_SIZE);
		memcpy(config.id, acc.c_str(), acc.size());

		msg = "Enter the password for the RTSP server.";
		string pw = Utility::getInput<string>(msg);
		memset(config.password, 0x0, BS2_USER_ID_SIZE);
		memcpy(config.password, pw.c_str(), pw.size());

		/* No need to set address which is updated by the device.
		msg = "Enter the address of the RTSP server.";
		string addr = Utility::getInput<string>(msg);
		memset(config.address, 0x0, BS2_URL_SIZE);
		memcpy(config.address, addr.c_str(), addr.size());*/

		msg = "Enter the port of the RTSP server. (default: 554)";
		config.port = (BS2_PORT)Utility::getInput<uint32_t>(msg);

		uint32_t defaultResolution = 0;
		switch (type)
		{
		case BS2_DEVICE_TYPE_BIOSTATION_3:
		case BS2_DEVICE_TYPE_BIOENTRY_W3:
			//defaultResolution = RTSP_RESOLUTION_TYPE_2;
			break;
		case BS2_DEVICE_TYPE_FACESTATION_F2:
		case BS2_DEVICE_TYPE_FACESTATION_F2_FP:
		//case BS2_DEVICE_TYPE_BIOSTATION_3_MAX:
		//case BS2_DEVICE_TYPE_BIOSTATION_3_MAX_FP:
			//defaultResolution = RTSP_RESOLUTION_TYPE_3;
			break;
		default:
			//defaultResolution = RTSP_RESOLUTION_TYPE_1;
			break;
		}
		msg = "Select video resolution. (0:180x320, 1:720x480, 2:360x640)";
		//config.resolution = (uint8_t)Utility::getDefaultInput<uint32_t>(msg, defaultResolution);
	}
	else
	{
		config.enabled = false;
	}

	return cc.setRtspConfig(id, config);
}

int getLicenseConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2LicenseConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getLicenseConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int getOsdpStandardConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2OsdpStandardConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getOsdpStandardConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int getOsdpStandardActionConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2OsdpStandardActionConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
	int sdkResult = cc.getOsdpStandardActionConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);

	return sdkResult;
}

int setOsdpStandardActionConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	BS2OsdpStandardActionConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
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

int getCardConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	DeviceControl dc(context);
	BS2DeviceCapabilities capabilies = { 0, };
	BS2CardConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	int sdkResult = dc.getDeviceCapabilities(id, capabilies);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	if (!capabilies.customSmartCardSupported)
	{
		TRACE("Not supported function.");
		return BS_SDK_ERROR_NOT_SUPPORTED;
	}

	sdkResult = cc.getCardConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);
	else
		return sdkResult;

	if (Utility::isYes("Do you want to scan card test?"))
	{
		UserControl uc(context);
		BS2Card cardID;

		sdkResult = uc.scanCard(id, &cardID);
		if (BS_SDK_SUCCESS == sdkResult)
			UserControl::print(cardID);
	}

	return sdkResult;
}

int setCardConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	DeviceControl dc(context);
	BS2DeviceCapabilities capabilies = { 0, };
	BS2CardConfig config = { 0, };
	//BS2MifareCardConfigEx mfConfig = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	int sdkResult = dc.getDeviceCapabilities(id, capabilies);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	if (!capabilies.customSmartCardSupported)
	{
		TRACE("Not supported function.");
		return BS_SDK_ERROR_NOT_SUPPORTED;
	}

	sdkResult = cc.getCardConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	string msg = "Please enter a data type of cards. (0: Binary, 1: ASCII, 2: UTF16, 3: BCS)";
	config.dataType = (BS2_CARD_DATA_TYPE)Utility::getInput<uint32_t>(msg);
	
	config.useSecondaryKey = Utility::isYes("Do you want to use secondary key?");

	ostringstream oss;

	//msg = "Please enter the encryption type of mifare card (0:CRYPTO1, 1:AES128).";
	//config.mifareEncType = (BS2_MIFARE_ENCRYPTION)Utility::getInput<uint32_t>(msg);


	
	if (Utility::isYes("Do you want to change mifare card settings?"))
	{
		memset(&config.mifare.primaryKey, 0x0, sizeof(config.mifare.primaryKey));
		oss << "Please enter the hexadecimal " << sizeof(config.mifare.primaryKey) << "-bytes primary key for mifare card." << endl;
		oss << " [Like 12 34 56 ... EF]" << endl;
		Utility::getLineHexaString<uint8_t>(oss.str(), config.mifare.primaryKey, sizeof(config.mifare.primaryKey));

		if (config.useSecondaryKey)
		{
			memset(&config.mifare.secondaryKey, 0x0, sizeof(config.mifare.secondaryKey));
			oss.str("");
			oss << "Please enter the hexadecimal " << sizeof(config.mifare.secondaryKey) << "-bytes secondary key for mifare card." << endl;
			oss << " [Like 12 34 56 ... EF]" << endl;
			Utility::getLineHexaString<uint8_t>(oss.str(), config.mifare.secondaryKey, sizeof(config.mifare.secondaryKey));
		}

		msg = "Please enter the start block index of mifare card.";
		config.mifare.startBlockIndex = (uint16_t)Utility::getInput<uint32_t>(msg);
	}

	if (Utility::isYes("Do you want to change desfire card settings?"))
	{
		msg = "Please enter a operation mode for desfire card. (0: Legacy, 1: Advanced(AppLevelKey))";
		config.desfire.operationMode = (uint8_t)Utility::getInput<uint32_t>(msg);

		if (DESFIRECARD_OPERATION_MODE_LEGACY == config.desfire.operationMode)
		{
			memset(&config.desfire.primaryKey, 0x0, sizeof(config.desfire.primaryKey));
			oss.str("");
			oss << "Please enter the hexadecimal " << sizeof(config.desfire.primaryKey) << "-bytes primary key for desfire card." << endl;
			oss << " [Like 12 34 56 ... EF]" << endl;
			Utility::getLineHexaString<uint8_t>(oss.str(), config.desfire.primaryKey, sizeof(config.desfire.primaryKey));

			if (config.useSecondaryKey)
			{
				memset(&config.desfire.secondaryKey, 0x0, sizeof(config.desfire.secondaryKey));
				oss.str("");
				oss << "Please enter the hexadecimal " << sizeof(config.desfire.secondaryKey) << "-bytes secondary key for desfire card." << endl;
				oss << " [Like 12 34 56 ... EF]" << endl;
				Utility::getLineHexaString<uint8_t>(oss.str(), config.desfire.secondaryKey, sizeof(config.desfire.secondaryKey));
			}
		}

	}

	msg = "Please enter a smart card byte order. (0: MSB, 1: LSB)";
	config.smartCardByteOrder = (BS2_CARD_BYTE_ORDER)Utility::getInput<uint32_t>(msg);
	msg = "Please enter a formatID.";
	config.formatID = (BS2_UID)Utility::getInput<uint32_t>(msg);

	sdkResult = cc.setCardConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;


	// MifareCardConfigEx
	//if (config.mifareEncType == MIFARE_ENCRYPTION_AES128)
	//{
	//	if (Utility::isYes("Do you want to change mifare card Extention settings?"))
	//	{
	//		sdkResult = cc.getCardConfig(id, config);
	//		if (BS_SDK_SUCCESS != sdkResult)
	//		return sdkResult;

	//		memset(&mfConfig.mifareEx.primaryKey, 0x0, sizeof(mfConfig.mifareEx.primaryKey));
	//		oss << "Please enter the hexadecimal " << sizeof(mfConfig.mifareEx.primaryKey) << "-bytes primary key for mifare card." << endl;
	//		oss << " [Like 12 34 56 ... EF]" << endl;
	//		Utility::getLineHexaString<uint8_t>(oss.str(), mfConfig.mifareEx.primaryKey, sizeof(mfConfig.mifareEx.primaryKey));

	//		if (config.useSecondaryKey)
	//		{
	//			memset(&mfConfig.mifareEx.secondaryKey, 0x0, sizeof(mfConfig.mifareEx.secondaryKey));
	//			oss.str("");
	//			oss << "Please enter the hexadecimal " << sizeof(mfConfig.mifareEx.secondaryKey) << "-bytes secondary key for mifare card." << endl;
	//			oss << " [Like 12 34 56 ... EF]" << endl;
	//			Utility::getLineHexaString<uint8_t>(oss.str(), mfConfig.mifareEx.secondaryKey, sizeof(mfConfig.mifareEx.secondaryKey));
	//		}

	//		msg = "Please enter the start block index of mifare card.";
	//		mfConfig.mifareEx.startBlockIndex = (uint16_t)Utility::getInput<uint32_t>(msg);
	//	}
	//}
	
	// SystemConfig
	oss.str("");
	oss << "Do you want to change the card operation mode?";
	if (Utility::isYes(oss.str()))
	{
		BS2SystemConfig sysConfig = { 0, };
		sdkResult = cc.getSystemConfig(id, sysConfig);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		uint32_t preMask = sysConfig.useCardOperationMask;
		sysConfig.useCardOperationMask |= (uint32_t)CARD_OPERATION_MASK_CLASSIC_PLUS;
		sysConfig.useCardOperationMask |= (uint32_t)CARD_OPERATION_MASK_DESFIRE_EV1;
		sysConfig.useCardOperationMask &= ~(uint32_t)CARD_OPERATION_MASK_SR_SE;
		sysConfig.useCardOperationMask &= ~(uint32_t)CARD_OPERATION_MASK_SEOS;

		// Apply
		sysConfig.useCardOperationMask |= (uint32_t)CARD_OPERATION_USE;

		sdkResult = cc.setSystemConfig(id, sysConfig);
		if (BS_SDK_SUCCESS == sdkResult)
			TRACE("Card operation mode was changed 0x08d => 0x08d", preMask, sysConfig.useCardOperationMask);
	}

	return sdkResult;
}

int getCustomCardConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	DeviceControl dc(context);
	BS2DeviceCapabilities capabilies = { 0, };
	BS2CustomCardConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	int sdkResult = dc.getDeviceCapabilities(id, capabilies);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	if (!capabilies.customSmartCardSupported)
	{
		TRACE("Not supported function.");
		return BS_SDK_ERROR_NOT_SUPPORTED;
	}

	sdkResult = cc.getCustomCardConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
		ConfigControl::print(config);
	else
		return sdkResult;

	if (Utility::isYes("Do you want to scan card test?"))
	{
		UserControl uc(context);
		BS2Card cardID;

		sdkResult = uc.scanCard(id, &cardID);
		if (BS_SDK_SUCCESS == sdkResult)
			UserControl::print(cardID);
	}

	return sdkResult;
}

int setCustomCardConfig(void* context, const DeviceInfo& device)
{
	ConfigControl cc(context);
	DeviceControl dc(context);
	BS2DeviceCapabilities capabilies = { 0, };
	BS2CustomCardConfig config = { 0, };

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	int sdkResult = dc.getDeviceCapabilities(id, capabilies);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	if (!capabilies.customSmartCardSupported)
	{
		TRACE("Not supported function.");
		return BS_SDK_ERROR_NOT_SUPPORTED;
	}

	sdkResult = cc.getCustomCardConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	std::cout << "Current dataType:" << config.dataType << std::endl;
	string msg = "Please enter a data type of cards. (0: Binary, 1: ASCII, 2: UTF16, 3: BCS)";
	config.dataType = (BS2_CARD_DATA_TYPE)Utility::getInput<uint32_t>(msg);
	config.useSecondaryKey = Utility::isYes("Do you want to use secondary key?");

	ostringstream oss;

//	msg = "Please enter the encryption type of mifare card (0:CRYPTO1, 1:AES128).";
//	config.mifareEncType = (BS2_MIFARE_ENCRYPTION)Utility::getInput<uint32_t>(msg);
//
//	if (Utility::isYes("Do you want to change mifare custom card settings?"))
//	{
//		memset(&config.mifare.primaryKey, 0x0, sizeof(config.mifare.primaryKey));
//		oss << "Please enter the hexadecimal " << sizeof(config.mifare.primaryKey) << "-bytes primary key for mifare card." << endl;
//		oss << " [Like 12 34 56 ... EF]" << endl;
//		Utility::getLineHexaString<uint8_t>(oss.str(), config.mifare.primaryKey, sizeof(config.mifare.primaryKey));
//
//		if (config.useSecondaryKey)
//		{
//			memset(&config.mifare.secondaryKey, 0x0, sizeof(config.mifare.secondaryKey));
//			oss.str("");
//			oss << "Please enter the hexadecimal " << sizeof(config.mifare.secondaryKey) << "-bytes secondary key for mifare card." << endl;
//			oss << " [Like 12 34 56 ... EF]" << endl;
//			Utility::getLineHexaString<uint8_t>(oss.str(), config.mifare.secondaryKey, sizeof(config.mifare.secondaryKey));
//		}
//
//		msg = "Please enter the start block index of mifare card.";
//		config.mifare.startBlockIndex = (uint16_t)Utility::getInput<uint32_t>(msg);
//		msg = "Please enter the card data size of mifare card.";
//		config.mifare.dataSize = (uint8_t)Utility::getInput<uint32_t>(msg);
//		msg = "Please enter the skip bytes of mifare card.";
//		config.mifare.skipBytes = (uint8_t)Utility::getInput<uint32_t>(msg);
//	}
//	
//	if (Utility::isYes("Do you want to change mifare custom card Extention settings?"))
//	{
//		memset(&config.mifareEx.primaryKey, 0x0, sizeof(config.mifareEx.primaryKey));
//		oss << "Please enter the hexadecimal " << sizeof(config.mifareEx.primaryKey) << "-bytes primary key for mifare card." << endl;
//		oss << " [Like 12 34 56 ... EF]" << endl;
//		Utility::getLineHexaString<uint8_t>(oss.str(), config.mifareEx.primaryKey, sizeof(config.mifareEx.primaryKey));
//
//		if (config.useSecondaryKey)
//		{
//			memset(&config.mifareEx.secondaryKey, 0x0, sizeof(config.mifareEx.secondaryKey));
//			oss.str("");
//			oss << "Please enter the hexadecimal " << sizeof(config.mifareEx.secondaryKey) << "-bytes secondary key for mifare card." << endl;
//			oss << " [Like 12 34 56 ... EF]" << endl;
//			Utility::getLineHexaString<uint8_t>(oss.str(), config.mifareEx.secondaryKey, sizeof(config.mifareEx.secondaryKey));
//		}
//
//		msg = "Please enter the start block index of mifare card.";
//		config.mifareEx.startBlockIndex = (uint16_t)Utility::getInput<uint32_t>(msg);
//		msg = "Please enter the card data size of mifare card.";
//		config.mifareEx.dataSize = (uint8_t)Utility::getInput<uint32_t>(msg);
//		msg = "Please enter the skip bytes of mifare card.";
//		config.mifareEx.skipBytes = (uint8_t)Utility::getInput<uint32_t>(msg);
//	}
//
//	if (Utility::isYes("Do you want to change desfire custom card settings?"))
//	{
//		msg = "Please enter a operation mode for desfire card. (0: Legacy, 1: Advanced(AppLevelKey))";
//		config.desfire.operationMode = (uint8_t)Utility::getInput<uint32_t>(msg);
//
//		if (DESFIRECARD_OPERATION_MODE_LEGACY == config.desfire.operationMode)
//		{
//			memset(&config.desfire.primaryKey, 0x0, sizeof(config.desfire.primaryKey));
//			oss.str("");
//			oss << "Please enter the hexadecimal " << sizeof(config.desfire.primaryKey) << "-bytes primary key for desfire card." << endl;
//			oss << " [Like 12 34 56 ... EF]" << endl;
//			Utility::getLineHexaString<uint8_t>(oss.str(), config.desfire.primaryKey, sizeof(config.desfire.primaryKey));
//
//			if (config.useSecondaryKey)
//			{
//				memset(&config.desfire.secondaryKey, 0x0, sizeof(config.desfire.secondaryKey));
//				oss.str("");
//				oss << "Please enter the hexadecimal " << sizeof(config.desfire.secondaryKey) << "-bytes secondary key for desfire card." << endl;
//				oss << " [Like 12 34 56 ... EF]" << endl;
//				Utility::getLineHexaString<uint8_t>(oss.str(), config.desfire.secondaryKey, sizeof(config.desfire.secondaryKey));
//			}
//		}
//		else	// DESFIRECARD_OPERATION_MODE_APPLEVELKEY
//		{
//			memset(&config.desfire.desfireAppKey.appMasterKey, 0x0, sizeof(config.desfire.desfireAppKey.appMasterKey));
//			memset(&config.desfire.desfireAppKey.fileReadKey, 0x0, sizeof(config.desfire.desfireAppKey.fileReadKey));
//			memset(&config.desfire.desfireAppKey.fileWriteKey, 0x0, sizeof(config.desfire.desfireAppKey.fileWriteKey));
//
//			oss.str("");
//			oss << "Please enter the hexadecimal " << sizeof(config.desfire.desfireAppKey.appMasterKey) << "-bytes appMasterKey for desfire card." << endl;
//			oss << " [Like 12 34 56 ... EF]" << endl;
//			Utility::getLineHexaString<uint8_t>(oss.str(), config.desfire.desfireAppKey.appMasterKey, sizeof(config.desfire.desfireAppKey.appMasterKey));
//
//			oss.str("");
//			oss << "Please enter the hexadecimal " << sizeof(config.desfire.desfireAppKey.fileReadKey) << "-bytes fileReadKey for desfire card." << endl;
//			oss << " [Like 12 34 56 ... EF]" << endl;
//			Utility::getLineHexaString<uint8_t>(oss.str(), config.desfire.desfireAppKey.fileReadKey, sizeof(config.desfire.desfireAppKey.fileReadKey));
//
//			msg = "Please enter the fileReadKeyNumber of desfire card.";
//			config.desfire.desfireAppKey.fileReadKeyNumber = (uint8_t)Utility::getInput<uint32_t>(msg);
//
//#if DO_NOT_NEED
//			oss.str("");
//			oss << "Please enter the hexadecimal " << sizeof(config.desfire.desfireAppKey.fileWriteKey) << "-bytes fileWriteKey for desfire card." << endl;
//			oss << " [Like 12 34 56 ... EF]" << endl;
//			Utility::getLineHexaString<uint8_t>(oss.str(), config.desfire.desfireAppKey.fileWriteKey, sizeof(config.desfire.desfireAppKey.fileWriteKey));
//
//			msg = "Please enter the fileWriteKeyNumber of desfire card.";
//			config.desfire.desfireAppKey.fileWriteKeyNumber = (uint8_t)Utility::getInput<uint32_t>(msg);
//#endif
//		}
//
//		oss.str("");
//		oss << "Please enter the hexadecimal " << sizeof(config.desfire.appID) << "-bytes appID for desfire card." << endl;
//		oss << " [Like 12 34 EF]" << endl;
//		Utility::getLineHexaString<uint8_t>(oss.str(), config.desfire.appID, sizeof(config.desfire.appID));
//
//		msg = "Please enter the fileID for desfire card.";
//		config.desfire.fileID = (uint8_t)Utility::getInput<uint32_t>(msg);
//		msg = "Please enter a encryption type for desfire card. (0: DES/3DES, 1: AES)";
//		config.desfire.encryptionType = (uint8_t)Utility::getInput<uint32_t>(msg);
//		msg = "Please enter the card data size of desfire card.";
//		config.desfire.dataSize = (uint8_t)Utility::getInput<uint32_t>(msg);
//		msg = "Please enter the skip bytes of desfire card.";
//		config.desfire.skipBytes = (uint8_t)Utility::getInput<uint32_t>(msg);
//	}
//
//	msg = "Please enter a smart card byte order. (0: MSB, 1: LSB)";
//	config.smartCardByteOrder = (BS2_CARD_BYTE_ORDER)Utility::getInput<uint32_t>(msg);
//	msg = "Please enter a formatID.";
//	config.formatID = (BS2_UID)Utility::getInput<uint32_t>(msg);
//
//	sdkResult = cc.setCustomCardConfig(id, config);
//	if (BS_SDK_SUCCESS != sdkResult)
//		return sdkResult;
//
	oss.str("");
	oss << "To use the custom smart card function, you must turn off the Suprema smart card function." << endl;
	oss << "Do you want to change the card operation mode?";
	if (Utility::isYes(oss.str()))
	{
		BS2SystemConfig sysConfig = { 0, };
		sdkResult = cc.getSystemConfig(id, sysConfig);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		uint32_t preMask = sysConfig.useCardOperationMask;

		// Turn off Suprema smart card
		sysConfig.useCardOperationMask &= ~(uint32_t)CARD_OPERATION_MASK_CLASSIC_PLUS;
		sysConfig.useCardOperationMask &= ~(uint32_t)CARD_OPERATION_MASK_DESFIRE_EV1;
		sysConfig.useCardOperationMask &= ~(uint32_t)CARD_OPERATION_MASK_SR_SE;
		sysConfig.useCardOperationMask &= ~(uint32_t)CARD_OPERATION_MASK_SEOS;

		// Turn on Custom smart card
		sysConfig.useCardOperationMask |= (uint32_t)CARD_OPERATION_MASK_CUSTOM_CLASSIC_PLUS;
		sysConfig.useCardOperationMask |= (uint32_t)CARD_OPERATION_MASK_CUSTOM_DESFIRE_EV1;

		// Apply
		sysConfig.useCardOperationMask |= (uint32_t)CARD_OPERATION_USE;

		sdkResult = cc.setSystemConfig(id, sysConfig);
		if (BS_SDK_SUCCESS == sdkResult)
			TRACE("Card operation mode was changed 0x08d => 0x08d", preMask, sysConfig.useCardOperationMask);
	}

	return sdkResult;
}

int runAction(void* context, const DeviceInfo& device)
{
	DeviceControl dc(context);

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	BS2Action action = { 0, };
	action.deviceID = id;
	action.type = BS2_ACTION_SOUND;
	action.stopFlag = BS2_STOP_NONE;
	action.sound.count = 1;
	action.sound.soundIndex = 0;
	action.sound.delay = 50;

	uint8_t buf[32] = { 0, };
	memcpy(buf, &action, 32);
	//action.type = BS2_ACTION_RELAY;
	//action.stopFlag = 0;
	//action.relay.relayIndex = 1;
	//action.relay.signal.signalID = 1;
	//action.relay.signal.count = 10;
	//action.relay.signal.onDuration = 200;
	//action.relay.signal.offDuration = 100;
	//action.relay.signal.delay = 50;
	//action.delay = 100;

	return dc.runAction(id, action);
}

int getDoors(void* context, const DeviceInfo& device)
{
	vector<BS2Door> vecDoors;
	int sdkResult = BS_SDK_SUCCESS;
	DoorControl dc(context);

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	if (BS_SDK_SUCCESS != (sdkResult = dc.getAllDoor(id, vecDoors)))
		return sdkResult;

	if (0 < vecDoors.size())
	{
		for (auto door : vecDoors)
			dc.print(door);
	}

	return sdkResult;
}

int setDoors(void* context, const DeviceInfo& device)
{
	vector<BS2Door> vecDoors;
	stringstream msg;
	DoorControl dc(context);

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	uint32_t numOfDoors = Utility::getInput<uint32_t>("How many doors do you want to register?");
	for (uint32_t idx = 0; idx < numOfDoors; idx++)
	{
		BS2Door door = { 0, };
		BS2_DOOR_ID doorID = Utility::getInput<BS2_DOOR_ID>("Please enter the door ID:");
		door.doorID = doorID;
		sprintf(door.name, "Door%u", doorID);

		door.entryDeviceID = (BS2_DEVICE_ID)Utility::getDefaultInput<uint32_t>("Enter the device ID for entry.", id);
		door.exitDeviceID = (BS2_DEVICE_ID)Utility::getDefaultInput<uint32_t>("Enter the device ID for exit.", id);

		door.relay.deviceID = (BS2_DEVICE_ID)Utility::getDefaultInput<uint32_t>("Enter the device ID to be used as a relay.", id);
		if (0 < door.relay.deviceID)
		{
			msg.str("");
			msg << "Enter the port of " << door.relay.deviceID << " relay.";
			door.relay.port = (uint8_t)Utility::getDefaultInput<uint32_t>(msg.str(), 0);
		}
		else
		{
			door.relay.port = 0;
		}

		door.sensor.deviceID = (BS2_DEVICE_ID)Utility::getDefaultInput<uint32_t>("Enter the device ID to be used as a sensor.", id);
		if (0 < door.sensor.deviceID)
		{
			msg.str("");
			msg << "Enter the port of " << door.sensor.deviceID << " sensor.";
			door.sensor.port = (uint8_t)Utility::getDefaultInput<uint32_t>(msg.str(), 0);
			msg.str("");
			msg << "Enter the switch type of " << door.sensor.deviceID << " sensor.";
			door.sensor.switchType = (BS2_SWITCH_TYPE)Utility::getDefaultInput<uint32_t>(msg.str(), BS2_SWITCH_TYPE_NORMAL_OPEN);
			door.sensor.apbUseDoorSensor = (uint8_t)!Utility::isNo("When using Global APB, do you want to use the door sensor to check whether the user has entered or not?");
		}
		else
		{
			door.sensor.port = 0;
			door.sensor.switchType = BS2_SWITCH_TYPE_NORMAL_OPEN;
			door.sensor.apbUseDoorSensor = 0;
		}

		door.button.deviceID = (BS2_DEVICE_ID)Utility::getDefaultInput<uint32_t>("Enter the device ID to be used as a RTE button.", id);
		if (0 < door.button.deviceID)
		{
			msg.str("");
			msg << "Enter the port of " << door.button.deviceID << " RTE button.";
			door.button.port = (uint8_t)Utility::getDefaultInput<uint32_t>(msg.str(), 1);
			msg.str("");
			msg << "Enter the switch type of " << door.button.deviceID << " RTE button.";
			door.button.switchType = (BS2_SWITCH_TYPE)Utility::getDefaultInput<uint32_t>(msg.str(), BS2_SWITCH_TYPE_NORMAL_OPEN);
		}
		else
		{
			door.button.port = 0;
			door.button.switchType = BS2_SWITCH_TYPE_NORMAL_OPEN;
		}

		door.autoLockTimeout = Utility::getDefaultInput<uint32_t>("Enter the auto lock timeout in seconds.", BS2_DEFAULT_AUTO_LOCK_TIMEOUT);
		door.heldOpenTimeout = Utility::getDefaultInput<uint32_t>("Enter the held open timeout in seconds.", BS2_DEFAULT_HELD_OPEN_TIMEOUT);

		door.instantLock = !Utility::isNo("Should this door be locked instantly when it is closed?");

		door.lockFlags = (BS2_DOOR_FLAG)Utility::getDefaultInput<uint32_t>("How to act at lock door? [0: None, 1: Schedule, 2: Emergency, 4: Operator] ", BS2_DOOR_FLAG_NONE);
		door.unlockFlags = (BS2_DOOR_FLAG)Utility::getDefaultInput<uint32_t>("How to act at unlock door? [0: None, 1: Schedule, 2: Emergency, 4: Operator] ", BS2_DOOR_FLAG_NONE);
		door.unconditionalLock = !Utility::isNo("Should this Door be locked after autoLock timeout?");

		bool useDualAuth = !Utility::isNo("Does this door need to dual authentication?");
		if (useDualAuth)
		{
			door.dualAuthScheduleID = (BS2_SCHEDULE_ID)Utility::getDefaultInput<uint32_t>("Enter the id of access schedule for dual authentication: [0: Never, 1: Always, or the other schedule id] ", BS2_SCHEDULE_ALWAYS_ID);
			door.dualAuthDevice = (BS2_DUAL_AUTH_DEVICE)Utility::getDefaultInput<uint32_t>("Which reader requires dual authentication? [1: Entrance Only, 2: Exit Only, 3: Both] ", BS2_DUAL_AUTH_ENTRY_DEVICE_ONLY);
			door.dualAuthTimeout = Utility::getDefaultInput<uint32_t>("Enter the dual authentication timeout in seconds: ", BS2_DEFAULT_DUAL_AUTH_TIMEOUT);
			door.dualAuthApprovalType = (BS2_DUAL_AUTH_APPROVAL)Utility::getDefaultInput<uint32_t>("Who should be the dual authentication approver for this door? [0: Not required, 1: Second user] ", BS2_DUAL_AUTH_APPROVAL_NONE);
			door.numDualAuthApprovalGroups = 0;
			// ...
		}
		else
		{
			door.dualAuthScheduleID = BS2_SCHEDULE_NEVER_ID;
			door.dualAuthDevice = BS2_DUAL_AUTH_NO_DEVICE;
			door.dualAuthTimeout = 0;
			door.dualAuthApprovalType = BS2_DUAL_AUTH_APPROVAL_NONE;
			door.numDualAuthApprovalGroups = 0;
		}

		vecDoors.push_back(door);
	}

	if (0 < vecDoors.size())
		return dc.setDoor(id, vecDoors);

	return BS_SDK_SUCCESS;
}

int lockDoor(void* context, const DeviceInfo& device)
{
	vector<BS2_DOOR_ID> listDoors;
	DoorControl dc(context);

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	uint32_t numOfDoors = Utility::getDefaultInput<uint32_t>("How many doors do you want to lock?", 1);
	for (uint32_t idx = 0; idx < numOfDoors; idx++)
	{
		BS2_DOOR_ID doorID = Utility::getInput<BS2_DOOR_ID>("Enter a door ID:");
		listDoors.push_back(doorID);
	}

	if (0 < listDoors.size())
	{
		BS2_DOOR_FLAG doorFlag = (BS2_DOOR_FLAG)Utility::getDefaultInput<uint32_t>("Enter the flag for the doors to be locked: [0: None, 1: By Schedule, 2: By Emergency, 4: By Operator] ", BS2_DOOR_FLAG_OPERATOR);

		uint32_t timeout = Utility::getDefaultInput<uint32_t>("Enter the normalize timer in seconds [0: Infinite, 1~86400: Sec] ", 0);

		return dc.lockDoor(id, listDoors, doorFlag, timeout);
	}

	return BS_SDK_SUCCESS;
}

int unlockDoor(void* context, const DeviceInfo& device)
{
	vector<BS2_DOOR_ID> listDoors;
	DoorControl dc(context);

	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	uint32_t numOfDoors = Utility::getDefaultInput<uint32_t>("How many doors do you want to unlock?", 1);
	for (uint32_t idx = 0; idx < numOfDoors; idx++)
	{
		BS2_DOOR_ID doorID = Utility::getInput<BS2_DOOR_ID>("Enter a door ID:");
		listDoors.push_back(doorID);
	}

	if (0 < listDoors.size())
	{
		BS2_DOOR_FLAG doorFlag = (BS2_DOOR_FLAG)Utility::getDefaultInput<uint32_t>("Enter the flag for the doors to be unlocked: [0: None, 1: By Schedule, 2: By Emergency, 4: By Operator] ", BS2_DOOR_FLAG_OPERATOR);

		uint32_t timeout = Utility::getDefaultInput<uint32_t>("Enter the normalize timer in seconds [0: Infinite, 1~86400: Sec] ", 0);

		return dc.unlockDoor(id, listDoors, doorFlag, timeout);
	}

	return BS_SDK_SUCCESS;
}

int getStatus(void* context, const DeviceInfo& device)
{
	vector<BS2_DOOR_ID> listDoors;
	vector<BS2DoorStatus> listStatus;
	int sdkResult = BS_SDK_SUCCESS;
	DoorControl dc(context);

#ifdef _DEBUG
	BS2_DEVICE_ID id = device.id_;
#else
	BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
#endif

#ifdef _DEBUG
	listDoors.push_back(1);
#else
	uint32_t numOfDoors = Utility::getDefaultInput<uint32_t>("How many door status do you want to get?", 1);
	for (uint32_t idx = 0; idx < numOfDoors; idx++);
	{
		BS2_DOOR_ID doorID = Utility::getInput<BS2_DOOR_ID>("Enter a door ID:");
		listDoors.push_back(doorID);
	}
#endif

	if (0 < listDoors.size())
	{
		sdkResult = dc.getDoorStatus(id, listDoors, listStatus);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;
	}

	uint32_t idx(0);
	for (auto item : listStatus)
		dc.print(item, idx++);

	return sdkResult;
}

int getDeviceIOStatus(void* context, const DeviceInfo& device)
{
	//LogControl lc(context);
	//vector<BS2_DEVICE_ID> request;
	//vector<BS2IOStatus> response;

	//BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);

	//if (!Utility::isYes("Do you want to get IO states of all currently connected devices?"))
	//{
	//	request = Utility::getLineNumbers<uint32_t>("  Enter the deviceIDs to get [ID_1, ID_2 ...] ", ',');
	//}

	//int sdkResult = lc.getDeviceIOStatus(id, request, response);
	//if (BS_SDK_SUCCESS == sdkResult)
	//{
	//	for (uint32_t idx = 0; idx < response.size(); idx++)
	//		lc.print(response[idx]);
	//}

	return 1;
}