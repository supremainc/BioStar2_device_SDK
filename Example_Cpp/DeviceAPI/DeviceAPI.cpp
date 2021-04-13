#include "stdafx.h"
#include <sstream>
#include "DeviceAPI.h"
#include "../Common/Utility.h"
#include "../Common/CommControl.h"

#pragma warning(disable:4800)

extern void TRACE(const char* fmt, ...);
using namespace std;

static void* sdkContext = NULL;
static BS2_DEVICE_ID connectedID = 0;
static DeviceInfo deviceInfo = { 0, 0, 0, 51211, 0 };


void onLogReceived(BS2_DEVICE_ID id, const BS2Event* event)
{
	if (deviceInfo.id_ == id)
	{
		int32_t timezone = deviceInfo.timezone_;
		cout << Utility::getEventString(id, *event, timezone) << endl;
	}
}

// Thermal supported callback
void onLogReceivedEx(BS2_DEVICE_ID id, const BS2Event* event, BS2_TEMPERATURE temperature)
{
	if (deviceInfo.id_ == id)
	{
		int32_t timezone = deviceInfo.timezone_;
		cout << Utility::getEventStringWithThermal(id, *event, timezone, temperature) << endl;
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
	BS2Context::setDebugFileLog(DEBUG_LOG_OPERATION_ALL, DEBUG_MODULE_ALL, ".");

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
	memset(&deviceInfo, 0x0, sizeof(DeviceInfo));
	int sdkResult = connectViaIP(context, deviceInfo);
	if (BS_SDK_SUCCESS != sdkResult)
		return;

	// Retrieve bulk logs.
	CommControl cm(context);
#if RETRIVE_BULK_LOGS
	sdkResult = getAllLogsFromDevice(sdkContext, deviceInfo.id_, deviceInfo.timezone_);
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

	connectSlave(context, deviceInfo);
	connectWiegand(context, deviceInfo);

	runAPIs(context, deviceInfo);
}

uint32_t showMenu(vector<MENU_ITEM>& info)
{
	for (const auto& item : info)
	{
		cout << item.index << ") " << item.disc << endl;
	}

	return getSelectedIndex();
}

int connectViaIP(void* context, DeviceInfo& device)
{
	DeviceControl dc(context);
	ConfigControl cc(context);
	CommControl cm(context);
	string ip = Utility::getInput<string>("Device IP:");
	BS2_PORT port = Utility::getInput<BS2_PORT>("Port:");
	BS2_DEVICE_ID id = 0;

	TRACE("Now connect to device (IP:%s, Port:%u)", ip.c_str(), port);

	int sdkResult = cm.connectDevice(id, ip, port);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	int timezone(0);
	if (BS_SDK_SUCCESS != (sdkResult = cc.getTimezone(id, timezone)))
	{
		cm.disconnectDevice(id);
		return sdkResult;
	}

	BS2SimpleDeviceInfo info = { 0, };
	if (BS_SDK_SUCCESS != (sdkResult = dc.getDeviceInfo(id, info)))
	{
		cm.disconnectDevice(id);
		return sdkResult;
	}

	device.id_ = id;
	device.type_ = info.type;
	device.ip_ = info.ipv4Address;
	device.port_ = port;
	device.timezone_ = timezone;
	device.connected_ = true;

	return sdkResult;
}

int connectSlave(void* context, DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	char selected = Utility::getInput<char>("Do you want to find slave devices? [y/n]");
	if ('y' == selected || 'Y' == selected)
	{
		BS2_DEVICE_ID slaveID = 0;
		ConfigControl cc(context);

		switch (device.type_)
		{
		case BS2_DEVICE_TYPE_CORESTATION_40:
			sdkResult = searchCSTSlave(context, device.id_, slaveID);
			break;

		default:
			sdkResult = cc.updateRS485OperationMode(device.id_, BS2_RS485_MODE_MASTER);
			if (BS_SDK_SUCCESS == sdkResult)
				sdkResult = searchSlave(context, device.id_, slaveID);
			break;
		}

		if (BS_SDK_SUCCESS == sdkResult && 0 < slaveID)
			device.slaveDevices_.push_back(slaveID);
	}

	return sdkResult;
}

int connectWiegand(void* context, DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	char selected = Utility::getInput<char>("Do you want to find wiegand devices? [y/n]");
	if ('y' == selected || 'Y' == selected)
	{
		BS2_DEVICE_ID wiegandID = 0;
		int sdkResult = searchWiegand(context, device.id_, wiegandID);
		if (BS_SDK_SUCCESS == sdkResult)
			device.wiegandDevices_.push_back(wiegandID);
	}

	return sdkResult;
}

uint32_t getSelectedIndex()
{
	return Utility::getInput<uint32_t>("Select ID:");
}

int searchSlave(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& slaveID)
{
	CommControl cm(context);
	vector<BS2Rs485SlaveDevice> slaveList;
	int sdkResult = cm.searchSlaveDevice(masterID, slaveList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	displaySlaveList(slaveList);

	uint32_t selected(0);
	if (MENU_BREAK != (selected = getSelectedIndex()) && selected <= slaveList.size())
	{
		BS2_DEVICE_ID id = slaveList[selected - 1].deviceID;

		TRACE("Now connect to slave device (Host:%u, Slave:%u)", masterID, id);

		sdkResult = cm.addSlaveDevice(masterID, id);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		slaveID = id;
		cout << "Added slave " << slaveID << endl;
	}

	return sdkResult;
}

int searchCSTSlave(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& slaveID)
{
	stringstream msg;
	msg << "Please select a channel to search. [0, 1, 2, 3, 4(All)]";
	uint32_t chSelected = Utility::getInput<uint32_t>(msg.str());
	switch (chSelected)
	{
	case RS485_HOST_CH_0:
	case RS485_HOST_CH_1:
	case RS485_HOST_CH_2:
	case RS485_HOST_CH_3:
		break;
	case 4:
	default:
		chSelected = RS485_HOST_CH_ALL;
		break;
	}

	CommControl cm(context);
	vector<BS2Rs485SlaveDeviceEX> slaveList;
	int sdkResult = cm.searchCSTSlaveDevice(masterID, chSelected, slaveList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	displayCSTSlaveList(slaveList);

	uint32_t selected(0);
	if (MENU_BREAK != (selected = getSelectedIndex()) && selected <= slaveList.size())
	{
		BS2_DEVICE_ID id = slaveList[selected - 1].deviceID;
		uint8_t chPort = slaveList[selected - 1].channelInfo;

		TRACE("Now connect to slave device (Host:%u, Slave:%u, Channel:%u)", masterID, id, chPort);

		sdkResult = cm.addCSTSlaveDevice(masterID, chPort, id);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		slaveID = id;
		cout << "Added slave " << slaveID << endl;
	}

	return sdkResult;
}


int searchWiegand(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& wiegandID)
{
	CommControl cm(context);
	vector<BS2_DEVICE_ID> wiegandList;
	int sdkResult = cm.searchWiegandDevice(masterID, wiegandList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	displayWiegandList(wiegandList);

	uint32_t selected(0);
	if (MENU_BREAK != (selected = getSelectedIndex()) && selected <= wiegandList.size())
	{
		BS2_DEVICE_ID id = wiegandList[selected - 1];

		TRACE("Now connect to wiegand device (Host:%u, Slave:%u)", masterID, id);

		sdkResult = cm.addWiegandDevice(masterID, id);
		if (BS_SDK_SUCCESS == sdkResult)
		{
			wiegandID = id;
			cout << "Added wiegand slave " << wiegandID << endl;
		}
	}

	return sdkResult;
}


int runAPIs(void* context, const DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);
	DeviceControl dc(context);
	ConfigControl cc(context);

	cout << endl << endl << "== DeviceAPI Test ==" << endl;
	BS2_DEVICE_ID id = getSelectedDeviceID(device);

	while (BS_SDK_SUCCESS == sdkResult && MENU_DEV_BREAK != (selectedTop = showMenu(menuInfoDeviceAPI)))
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
			sdkResult = dc.getDeviceInfo(id);
			if (BS_SDK_SUCCESS == sdkResult)
			break;
		case MENU_DEV_GET_DEVINFEX:
			sdkResult = dc.getDeviceInfoEx(id);
			if (BS_SDK_SUCCESS == sdkResult)
			break;
		case MENU_DEV_GET_DEVTIME:
			sdkResult = dc.getDeviceTime(id);
			break;
		case MENU_DEV_SET_DEVTIME:
			sdkResult = dc.setDeviceTime(id);
			break;
		case MENU_DEV_CLR_DATABASE:
			sdkResult = dc.clearDatabase(id);
			break;
		case MENU_DEV_FACTORY_RESET:
			sdkResult = dc.factoryReset(id);
			break;
		case MENU_DEV_REBOOT_DEV:
			sdkResult = dc.rebootDevice(id);
			break;
		case MENU_DEV_LOCK_DEV:
			sdkResult = dc.lockDevice(id);
			// try a fingerprint verification test.
			break;
		case MENU_DEV_UNLOCK_DEV:
			sdkResult = dc.unlockDevice(id);
			// try a fingerprint verification test.
			break;
		case MENU_DEV_UPG_FIRMWARE:
			sdkResult = dc.upgradeFirmware(id);
			break;
		case MENU_DEV_UPD_RESOURCE:
			sdkResult = dc.updateResource(id);
			break;
		case MENU_DEV_GET_SPCDEVINFO:
			sdkResult = dc.getSpecifiedDeviceInfo(id);
			break;
		case MENU_DEV_GET_AUTH_OPRLEVELEX:
			sdkResult = dc.getAuthOperatorLevelEx(id);
			break;
		case MENU_DEV_GET_ALLAUTH_OPRLEVELEX:
			break;
		case MENU_DEV_SET_AUTH_OPRLEVELEX:
			break;
		case MENU_DEV_REM_AUTH_OPRLEVELEX:
			break;
		case MENU_DEV_REM_ALLAUTH_OPRLEVELEX:
			break;

		case MENU_DEV_GET_FINGERPRINTCONFIG:
			sdkResult = getFingerprintConfig(context, id);
			break;
		case MENU_DEV_SET_FINGERPRINTCONFIG: 
			sdkResult = setFingerprintConfig(context, id);
			break;
		case MENU_DEV_GET_FACECONFIG:
			sdkResult = getFaceConfig(context, id);
			break;
		case MENU_DEV_SET_FACECONFIG:
			sdkResult = setFaceConfig(context, id);
			break;
		case MENU_DEV_GET_SYSTEMCONFIG:
			sdkResult = getSystemConfig(context, id);
			break;
		case MENU_DEV_SET_SYSTEMCONFIG:
			sdkResult = setSystemConfig(context, id);
			break;
		case MENU_DEV_GET_DESFIRECONFIGEX:
			sdkResult = getDesFireCardConfigEx(context, id);
			break;
		case MENU_DEV_SET_DESFIRECONFIGEX:
			sdkResult = setDesFireCardConfigEx(context, id);
			break;
		case MENU_DEV_GET_AUTHCONFIGEX:
			sdkResult = getAuthConfigEx(context, id);
			break;
		case MENU_DEV_SET_AUTHCONFIGEX:
			sdkResult = setAuthConfigEx(context, id);
			break;
		case MENU_DEV_GET_FACECONFIGEX:
			sdkResult = getFaceConfigEx(context, id);
			break;
		case MENU_DEV_SET_FACECONFIGEX:
			sdkResult = setFaceConfigEx(context, id);
			break;
		case MENU_DEV_GET_THERMALCAMERACONFIG:
			sdkResult = getThermalCameraConfig(context, id);
			break;
		case MENU_DEV_SET_THERMALCAMERACONFIG:
			sdkResult = setThermalCameraConfig(context, id);
			break;
		case MENU_DEV_GET_EVENTCONFIG:
			sdkResult = getEventConfig(context, id);
			break;
		case MENU_DEV_SET_EVENTCONFIG:
			sdkResult = setEventConfig(context, id);
			break;
		case MENU_DEV_EXTRACT_TEMPLATE_FACEEX:
			sdkResult = extractTemplateFaceEx(context, id);
			break;
		case MENU_DEV_GET_INPUTCONFIG:
			sdkResult = getInputConfig(context, id);
			break;
		case MENU_DEV_GET_TRIGGERACTIONCONFIG:
			sdkResult = getTriggerActionConfig(context, id);
			break;
		case MENU_DEV_SET_TRIGGERACTIONCONFIG:
			sdkResult = setTriggerActionConfig(context, id);
			break;
		case MENU_DEV_REM_TRIGGERACTIONCONFIG:
			sdkResult = removeTriggerActionConfig(context, id);
			break;
		case MENU_DEV_UPD_DEVICE_VOLUME:
			sdkResult = updateDeviceVolume(context, id);
			break;
		case MENU_DEV_RST_CONFIG_EXCEPT_NETINFO:
			sdkResult = cc.resetConfigExceptNetInfo(id);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}


void displayDeviceList(const vector<BS2SimpleDeviceInfo>& devices)
{
	int index = 0;
	printf("%2u - Exit\n", index);
	for (const auto& device : devices)
	{
		const BS2SimpleDeviceInfo& info = device;
		printf("%2u - Device:%10u, IP:%-15s, Port:%u, Connected:%-15s, Mode:%s, Type:%-10s, DualID:%u\n",
			++index,
			info.id,
			Utility::getIPAddress(info.ipv4Address).c_str(),
			info.port,
			(info.connectedIP == 0xFFFFFFFF) ? "" : Utility::getIPAddress(info.connectedIP).c_str(),
			CONNECT_MODE[info.connectionMode],
			DEVICE_NAME[info.type],
			info.dualIDSupported);
	}
}

void displaySlaveList(const vector<BS2Rs485SlaveDevice>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		const BS2Rs485SlaveDevice& info = device;
		printf("%2u - Device:%10u, Type:%-10s, OSDP:%d, Connected:%d\n",
			++index,
			info.deviceID,
			DEVICE_NAME[info.deviceType],
			info.enableOSDP,
			info.connected);
	}
}

void displayCSTSlaveList(const vector<BS2Rs485SlaveDeviceEX>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		const BS2Rs485SlaveDeviceEX& info = device;
		printf("%2u - Device:%10u, Type:%-10s, OSDP:%d, Connected:%d, Channel:%u\n",
			++index,
			info.deviceID,
			DEVICE_NAME[info.deviceType],
			info.enableOSDP,
			info.connected,
			info.channelInfo);
	}
}

void displayWiegandList(const vector<BS2_DEVICE_ID>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		printf("%2u - Device:%u\n", ++index, device);
	}
}

int getAllLogsFromDevice(void* context, BS2_DEVICE_ID id, int32_t timezone)
{
	int logIndex = 0;
	int sdkResult = BS_SDK_SUCCESS;

	// 1. Get the last log index from the database.
	// logIndex = ????

	// 2. Retrieve all bulk logs when disconnected
	if (BS_SDK_SUCCESS == (sdkResult = getLogsFromDevice(context, id, logIndex, timezone)))
	{
		// 3. Retrieve logs that may have occurred during bulk log reception
		sdkResult = getLogsFromDevice(context, id, logIndex, timezone);
	}

	return sdkResult;
}

int getLogsFromDevice(void* context, BS2_DEVICE_ID id, int& latestIndex, int timezone)
{
	int sdkResult = BS_SDK_SUCCESS;
	char buffer[1024] = { 0, };
	BS2Event* logObj = NULL;
	uint32_t numOfLog = 0;

	do
	{
		sdkResult = BS2_GetLog(context, id, latestIndex, MAX_RECV_LOG_AMOUNT, &logObj, &numOfLog);
		if (BS_SDK_SUCCESS == sdkResult)
		{
			for (uint32_t index = 0; index < numOfLog; ++index)
			{
				BS2Event& event = logObj[index];
				latestIndex = event.id;
				cout << Utility::getEventString(id, event, timezone) << endl;

				if (event.image & 0x01)
				{
					uint32_t imageSize(0);
					uint8_t* imageBuf = new uint8_t[MAX_SIZE_IMAGE_LOG];
					memset(imageBuf, 0x0, sizeof(uint8_t) * MAX_SIZE_IMAGE_LOG);
					if (BS_SDK_SUCCESS == getImageLog(context, id, event.id, imageBuf, imageSize))
					{
						// Your job.
						cout << "Image log received from " << id << " dateTime:" << event.dateTime + timezone
							<< " Event:" << event.id << endl;
					}

					delete[] imageBuf;
				}
			}

			if (logObj)
			{
				BS2_ReleaseObject(logObj);
				logObj = NULL;
			}
		}
		else
		{
			TRACE("BS2_GetLog call failed: %d", sdkResult);
			return sdkResult;
		}
	} while (MAX_RECV_LOG_AMOUNT <= numOfLog);

	return sdkResult;
}

int getImageLog(void* context, BS2_DEVICE_ID id, BS2_EVENT_ID eventID, uint8_t* imageBuf, uint32_t& imageSize)
{
	if (!imageBuf)
		return BS_SDK_ERROR_NULL_POINTER;

	uint8_t* imageObj = NULL;
	uint32_t size(0);
	int sdkResult = BS2_GetImageLog(context, id, eventID, &imageObj, &size);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		memcpy(imageBuf, imageObj, size);
		imageSize = size;
		if (imageObj)
			BS2_ReleaseObject(imageObj);
	}

	return sdkResult;
}

BS2_DEVICE_ID getSelectedDeviceID(const DeviceInfo& info)
{
	printf("%u - (M)\n", info.id_);
	for (uint32_t index = 0; index < info.slaveDevices_.size(); index++)
		printf("%u - (S)\n", info.slaveDevices_[index]);

	return Utility::getInput<BS2_DEVICE_ID>("Select ID:");
}

int getFingerprintConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2FingerprintConfig config = { 0, };

	return cc.getFingerprintConfig(id, config);
}

int setFingerprintConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2FingerprintConfig config = { 0, };

	int sdkResult = cc.getFingerprintConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		char checkDuplicate = Utility::getInput<char>("Do you want to turn on the checkDuplicate option? [y/n]");
		config.checkDuplicate = ('y' == checkDuplicate || 'Y' == checkDuplicate);
		sdkResult = cc.setFingerprintConfig(id, config);
	}

	return sdkResult;
}

int getFaceConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2FaceConfig config = { 0, };
	return cc.getFaceConfig(id, config);
}

int setFaceConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2FaceConfig config = { 0, };
	int sdkResult = cc.getFaceConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		char checkDuplicate = Utility::getInput<char>("Do you want to turn on the checkDuplicate option? [y/n]");
		config.checkDuplicate = ('y' == checkDuplicate || 'Y' == checkDuplicate);
		sdkResult = cc.setFaceConfig(id, config);
	}

	return sdkResult;
}

int getSystemConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2SystemConfig config = { 0, };
	return cc.getSystemConfig(id, config);
}

int setSystemConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2SystemConfig config = {0,};
	int sdkResult = cc.getSystemConfig(id, config);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		string msg = "Please enter the card combination you wish to set.\r\n";
		msg += "    DEFAULT : 0xFFFFFFFF\r\n";
		msg += "    BLE : 0x00000200\r\n";
		msg += "    NFC : 0x00000100\r\n";
		msg += "    SEOS : 0x00000080\r\n";
		msg += "    SR_SE : 0x00000040\r\n";
		msg += "    DESFIRE_EV1 : 0x00000020\r\n";
		msg += "    CLASSIC_PLUS : 0x00000010\r\n";
		msg += "    ICLASS : 0x00000008\r\n";
		msg += "    MIFARE_FELICA : 0x00000004\r\n";
		msg += "    HIDPROX : 0x00000002\r\n";
		msg += "    EM : 0x00000001\r\n";
		uint32_t cardTypes = Utility::getInput<uint32_t>(msg);
		cardTypes |= CARD_OPERATION_USE;		// Card operation apply
		config.useCardOperationMask = cardTypes;

		TRACE("CardType:0x%08x", config.useCardOperationMask);

		sdkResult = cc.setSystemConfig(id, config);
	}

	return sdkResult;
}

int getDesFireCardConfigEx(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2DesFireCardConfigEx config = { 0, };
	return cc.getDesFireCardConfigEx(id, config);
}

int setDesFireCardConfigEx(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2DesFireCardConfigEx config = { 0, };
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

int getAuthConfigEx(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2AuthConfigExt config = { 0, };
	return cc.getAuthConfigEx(id, config);
}

int setAuthConfigEx(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2AuthConfigExt config = { 0, };
	const int EXIT_MENU = 999;
	uint32_t mode(0);

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

	config.numOperators = 0;

	return cc.setAuthConfigEx(id, config);
}

int getFaceConfigEx(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2FaceConfigExt config = { 0, };
	return cc.getFaceConfigEx(id, config);
}

int setFaceConfigEx(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2FaceConfigExt config = { 0, };
	string msg;
	stringstream strmsg;

	msg = "Insert thermal check mode. (0: Not use, 1: Hard, 2: Soft)";
	config.thermalCheckMode = (BS2_FACE_CHECK_MODE)Utility::getInput<uint32_t>(msg);

	msg = "Insert mask check mode. (0: Not use, 1: Hard, 2: Soft)";
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

	msg = "Do you want to record the temperature in the event log? [y/n]";
	config.auditTemperature = Utility::isYes(msg);

	msg = "Do you want to use reject sound? [y/n]";
	config.useRejectSound = Utility::isYes(msg);

	msg = "Do you want to use overlapped thermal? [y/n]";
	config.useOverlapThermal = Utility::isYes(msg);

	msg = "Do you want to use dynamic ROI? [y/n]";
	config.useDynamicROI = Utility::isYes(msg);


	strmsg << "Insert face check order." << endl;
	strmsg << " 0: Face check after auth [default]" << endl;
	strmsg << " 1: Face check before auth" << endl;
	strmsg << " 2: Face check without auth";
	config.faceCheckOrder = (BS2_FACE_CHECK_ORDER)Utility::getInput<uint32_t>(strmsg.str());

	return cc.setFaceConfigEx(id, config);
}

int getThermalCameraConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2ThermalCameraConfig config = { 0, };
	return cc.getThermalCameraConfig(id, config);
}

int setThermalCameraConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2ThermalCameraConfig config = { 0, };
	string msg;

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

	msg = "Do you want to use body compensation [y/n]";
	config.useBodyCompensation = Utility::isYes(msg);

	msg = "Insert compensation temperature *10. If you want -4.5, it is -45. (-50 ~ 50)";
	config.compensationTemperature = (int8_t)Utility::getInput<int32_t>(msg);

	return cc.setThermalCameraConfig(id, config);
}

int getEventConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2EventConfig config = { 0, };
	return cc.getEventConfig(id, config);
}

int setEventConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2EventConfig config = { 0, };
	string msg;

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

	//msg = "Do you want to use body compensation [y/n]";
	//config.useBodyCompensation = Utility::isYes(msg);

	//msg = "Insert compensation temperature *10. If you want -4.5, it is -45. (-50 ~ 50)";
	//config.compensationTemperature = (int8_t)Utility::getInput<int32_t>(msg);

	return cc.setEventConfig(id, config);
}

int extractTemplateFaceEx(void* context, BS2_DEVICE_ID id)
{
	DeviceControl dc(context);
	BS2TemplateEx templateEx = {0,};
	return dc.extractTemplateFaceEx(id, templateEx);
}

int getInputConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2InputConfig config = { 0, };
	return cc.getInputConfig(id, config);
}

int getTriggerActionConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2TriggerActionConfig config = { 0, };
	return cc.getTriggerActionConfig(id, config);
}

int setTriggerActionConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2TriggerActionConfig config = { 0, };
	string msg;

	msg = "How many trigger-action do you want to register?";
	config.numItems = (uint8_t)Utility::getInput<uint32_t>(msg);

	for (uint8_t idx = 0; idx < config.numItems; idx++)
	{
		BS2Trigger& trigger = config.items[idx].trigger;

		msg = "[Trigger] Please insert device ID.";
		trigger.deviceID = (BS2_DEVICE_ID)Utility::getInput<uint32_t>(msg);

		trigger.type = BS2_TRIGGER_INPUT;
		BS2InputTrigger& inputTrigger = trigger.input;

		msg = "[Trigger] Please insert input port No.";
		inputTrigger.port = (uint8_t)Utility::getInput<uint32_t>(msg);

		msg = "[Trigger] Please insert switchType (N/O:0, N/C:1).";
		BS2_SWITCH_TYPE sw = (BS2_SWITCH_TYPE)Utility::getInput<uint32_t>(msg);
		inputTrigger.switchType = (sw == BS2_SWITCH_TYPE_NORMAL_OPEN) ? BS2_SWITCH_TYPE_NORMAL_OPEN : BS2_SWITCH_TYPE_NORMAL_CLOSED;

		msg = "[Trigger] Please insert duration.";
		inputTrigger.duration = (uint16_t)Utility::getInput<uint32_t>(msg);
		inputTrigger.scheduleID = BS2_SCHEDULE_ALWAYS_ID;


		BS2Action& action = config.items[idx].action;
		msg = "[Action] Please insert device ID.";
		action.deviceID = (BS2_DEVICE_ID)Utility::getInput<uint32_t>(msg);

		action.type = BS2_ACTION_RELAY;
		action.stopFlag = BS2_STOP_NONE;

		msg = "[Action] Please insert delay of relay.";
		action.delay = (uint8_t)Utility::getInput<uint32_t>(msg);

		BS2RelayAction& relayAction = action.relay;
		msg = "[Action] Please insert relay index.";
		relayAction.relayIndex = (uint8_t)Utility::getInput<uint32_t>(msg);

		BS2Signal& relaySignal = relayAction.signal;
		msg = "[Action] Please insert signal ID.";
		relaySignal.signalID = (BS2_UID)Utility::getInput<uint32_t>(msg);

		msg = "[Action] Please insert signal count.";
		relaySignal.count = (uint16_t)Utility::getInput<uint32_t>(msg);

		msg = "[Action] Please insert signal On-Duration.";
		relaySignal.onDuration = (uint16_t)Utility::getInput<uint32_t>(msg);

		msg = "[Action] Please insert signal Off-Duration.";
		relaySignal.offDuration = (uint16_t)Utility::getInput<uint32_t>(msg);

		msg = "[Action] Please insert signal delay.";
		relaySignal.delay = (uint16_t)Utility::getInput<uint32_t>(msg);
	}

	return cc.setTriggerActionConfig(id, config);
}

int removeTriggerActionConfig(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2TriggerActionConfig config = { 0, };

	return cc.setTriggerActionConfig(id, config);
}

int updateDeviceVolume(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	BS2DisplayConfig config = { 0, };

	int sdkResult = cc.getDisplayConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.volume = 10;

	return cc.setDisplayConfig(id, config);
}