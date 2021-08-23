#include "stdafx.h"
#include <sstream>
//#include <gtest/gtest.h>
#include "UserAPI.h"
#include "../Common/Utility.h"
#include "../Common/CommControl.h"


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

//TEST(SampleTest, SampeTest_Bool)
//{
//	EXPECT_EQ(1, true);
//}

int main(int argc, char* argv[])
{
	//::testing::InitGoogleTest();
	//return RUN_ALL_TESTS();

	// Set debugging SDK log (to current working directory)
	BS2Context::setDebugFileLog(DEBUG_LOG_ALL, DEBUG_MODULE_ALL, ".");

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
		TRACE("BS2_StartMonitoringLog call failed: %d", sdkResult);

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
	if (Utility::isYes("Do you want to find slave devices?"))					  
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
	if (Utility::isYes("Do you want to find wiegand devices?"))
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
	return Utility::getInput<uint32_t>("Select number:");
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
	UserControl uc(context);

	cout << endl << endl << "== UserAPI Test ==" << endl;
	BS2_DEVICE_ID id = getSelectedDeviceID(device);

	while (BS_SDK_SUCCESS == sdkResult && MENU_USR_BREAK != (selectedTop = showMenu(menuInfoDeviceAPI)))
	{
		if (!device.connected_)
		{
			TRACE("No device connected");
			return BS_SDK_ERROR_CANNOT_CONNECT_SOCKET;
		}

		switch (selectedTop)
		{
		case MENU_USR_BREAK:
			return BS_SDK_SUCCESS;
		case MENU_USR_GET_USR:
			sdkResult = uc.getUser(id);
			break;
		case MENU_USR_GET_USR_FACEEX:
			sdkResult = uc.getUserFaceEx(id);
			break;
		case MENU_USR_GET_ALLUSR:
			sdkResult = uc.getAllUser(id);
			break;
		case MENU_USR_GET_ALLUSR_FACEEX:
			sdkResult = uc.getAllUserFaceEx(id);
			break;
		case MENU_USR_ENR_USR:
			sdkResult = uc.enrollUser(id);
			break;
		case MENU_USR_ENR_USR_SMALL:
			sdkResult = uc.enrollUserSmall(id);
			break;
		case MENU_USR_ENR_USR_FACEEX:
			sdkResult = uc.enrollUserFaceEx(id);
			break;
		case MENU_USR_ENR_USR_SCAN_N_LOAD:
			sdkResult = uc.enrollUserFaceExScanAndLoad(id);
			break;
		case MENU_USR_EXTRACT_TEMPLATE_FACEEX:
			sdkResult = extractTemplateFaceEx(context, id);
			break;
		case MENU_USR_GET_NORMALIZE_IMAGE_FACEEX:
			sdkResult = getNormalizedImageFaceEx(context, id);
			break;
		case MENU_USR_GET_LASTFPIMAGE:
			sdkResult = getLastFingerprintImage(uc, id);
			break;
		case MENU_USR_REM_USR:
			sdkResult = uc.removeUser(id);
			break;
		case MENU_USR_REM_ALLUSR:
			sdkResult = uc.removeAllUser(id);
			break;
		case MENU_USR_SMARTCARD_SCAN:
			sdkResult = scanCard(uc, id);
			break;
		case MENU_USR_SMARTCARD_WRITE:
			sdkResult = writeCard(uc, id);
			break;
		case MENU_USR_SMARTCARD_ERASE:
			sdkResult = eraseCard(uc, id);
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
	BS2Event* logObj = NULL;
	uint32_t numOfLog = 0;

	do
	{
		numOfLog = 0;
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

	return Utility::getInput<BS2_DEVICE_ID>("Please enter the device ID:");
}

int getLastFingerprintImage(UserControl& uc, BS2_DEVICE_ID id)
{
	uint8_t* imageObj = NULL;
	uint32_t width = 0;
	uint32_t height = 0;

	int sdkResult = uc.getLastFingerprintImage(id, &imageObj, &width, &height);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	if (0 < width * height)
	{
		string fileName = Utility::getInput<string>("Please insert image file name:");
		if (0 < fileName.size())
		{
			FILE* fp = fopen(fileName.c_str(), "wb");
			if (NULL != fp)
			{
				if (0 < Utility::saveBMP(fp, imageObj, width, height))
					TRACE("File write success: %s", fileName.c_str());
				else
					TRACE("File write failed: %s", fileName.c_str());
			}
			else
				TRACE("File open failed: %s", fileName.c_str());

			fclose(fp);
		}
	}
	else
	{
		TRACE("File has not size.");
	}

	if (imageObj)
	{
		BS2_ReleaseObject(imageObj);
	}

	return sdkResult;
}

int scanCard(UserControl& uc, BS2_DEVICE_ID id)
{
	BS2Card card = { 0, };
	int sdkResult = uc.scanCard(id, &card);
	if (BS_SDK_SUCCESS == sdkResult)
		uc.print(card);

	return sdkResult;
}

int writeCard(UserControl& uc, BS2_DEVICE_ID id)
{
	int sdkResult = BS_SDK_SUCCESS;
	BS2SmartCardData card = { 0, };

	uint32_t uid(0);
	uint8_t cardBuff[BS2_CARD_DATA_SIZE] = { 0, };
	unsigned char* ptrUID = NULL;
	string msg = "Please select a card input type. [1. Scan from device, 2: Set user ID]";
	uint32_t cardInput = Utility::getInput<uint32_t>(msg);
	switch (cardInput)
	{
	case 1:
		cout << "Now scan your card." << endl;
		sdkResult = uc.scanCard(id, cardBuff);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		memcpy(card.cardID, cardBuff, BS2_CARD_DATA_SIZE);
		break;

	case 2:
	default:
		uid = Utility::getInput<uint32_t>("Please enter a user ID:");
		ptrUID = (unsigned char*)&uid;
		for (int i = 0; i < sizeof(uid); i++)
		{
			card.cardID[BS2_CARD_DATA_SIZE - i - 1] = ptrUID[i];
		}
		break;
	}

	string pinString = Utility::getInput<string>("Enter the PIN code:");
	if (BS2_USER_PIN_SIZE < pinString.size())
	{
		TRACE("PIN code is too long");
		return BS_SDK_ERROR_INVALID_PARAM;
	}

	sdkResult = uc.getPinCode(pinString, card.credentials.pin);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	uint8_t fpTemplate[BS2_FINGER_TEMPLATE_SIZE] = { 0, };
	cout << "Now scan your fingerprint." << endl;
	sdkResult = uc.scanTemplate(id, fpTemplate);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	memcpy(card.credentials.templateData, fpTemplate, BS2_FINGER_TEMPLATE_SIZE);
	card.header.duressMask = false;

	if (Utility::isYes("Do you want register access group ID?"))
	{
		stringstream msg;
		msg << "Please enter access group IDs. ex)ID1 ID2 ID3 ...\n";
		string inStrAGID = Utility::getLine(msg.str());
		if (0 == inStrAGID.size())
			return BS_SDK_ERROR_CANNOT_FIND_ACCESS_GROUP;

		vector<string> listID = Utility::tokenizeString(inStrAGID);
		if (listID.size() < BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER)
		{
			uint32_t index(0);
			for (auto groupID : listID)
			{
				BS2_ACCESS_GROUP_ID gid = atoi(groupID.c_str());
				card.accessOnData.accessGroupID[index++] = gid;
			}
		}
	}

	string inputTime = Utility::getLine("Please enter start time [YYYY-MM-DD HH:MM:SS] ?");
	BS2_TIMESTAMP startTime = Utility::convertTimeString2UTC(inputTime);
	card.accessOnData.startTime = startTime;

	inputTime = Utility::getLine("Please enter end time [YYYY-MM-DD HH:MM:SS] ?");
	BS2_TIMESTAMP endTime = Utility::convertTimeString2UTC(inputTime);
	card.accessOnData.endTime = endTime;

	card.header.cardType = BS2_CARD_TYPE_ACCESS;
	card.header.numOfTemplate = 1;
	card.header.templateSize = BS2_FINGER_TEMPLATE_SIZE;
	card.header.issueCount = 1;
	card.header.cardAuthMode = BS2_AUTH_MODE_CARD_BIOMETRIC_OR_PIN;
	card.header.useAlphanumericID = false;

	if (BS_SDK_SUCCESS != uc.updateCardTypeCRC(card) ||
		BS_SDK_SUCCESS != uc.updateCardCRC(card))
		return sdkResult;

	return uc.writeCard(id, &card);
}

int eraseCard(UserControl& uc, BS2_DEVICE_ID id)
{
	cout << "Now erase your card" << endl;
	return uc.eraseCard(id);
}

int extractTemplateFaceEx(void* context, BS2_DEVICE_ID id)
{
	UserControl uc(context);
	BS2TemplateEx templateEx = { 0, };
	return uc.extractTemplateFaceEx(id, templateEx);
}

int getNormalizedImageFaceEx(void* context, BS2_DEVICE_ID id)
{
	UserControl uc(context);
	return uc.getNormalizedImageFaceEx(id);
}
