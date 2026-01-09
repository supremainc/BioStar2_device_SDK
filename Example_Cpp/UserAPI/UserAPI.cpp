#include "stdafx.h"
#include <sstream>
#include <utility>
//#include <gtest/gtest.h>
#include "UserAPI.h"
#include "../Common/Utility.h"
#include "../Common/CommControl.h"
#include "../Common/LogControl.h"


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

//TEST(SampleTest, SampeTest_Bool)
//{
//	EXPECT_EQ(1, true);
//}

int main(int argc, char* argv[])
{
	//::testing::InitGoogleTest();
	//return RUN_ALL_TESTS();

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
	memset(&deviceInfo, 0x0, sizeof(DeviceInfo));
	int sdkResult = Utility::connectViaIP(context, deviceInfo);
	if (BS_SDK_SUCCESS != sdkResult)
		return;

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

int runAPIs(void* context, const DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);
	UserControl uc(context);
	DoorControl dc(context);

	cout << endl << endl << "== UserAPI Test ==" << endl;

	while (/*BS_SDK_SUCCESS == sdkResult && */MENU_USR_BREAK != (selectedTop = Utility::showMenu(menuInfoDeviceAPI)))
	{
		if (!device.connected_)
		{
			TRACE("No device connected");
			return BS_SDK_ERROR_CANNOT_CONNECT_SOCKET;
		}

		BS2_DEVICE_ID id = Utility::getSelectedDeviceID(device);
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
			sdkResult = uc.enrollUser(id, &device);
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
		case MENU_USR_ACT_USR:
			sdkResult = uc.activateUser(id);
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
		case MENU_USR_UPD_USR:
			sdkResult = updateUser(context, id);
			break;
		case MENU_USR_GET_STATISTIC:
			sdkResult = getUserStatistic(context, id);
			break;

		case MENU_USR_GET_OPERATOR:
			sdkResult = getOperators(context, id);
			break;
		case MENU_USR_SET_OPERATOR:
			sdkResult = setOperators(context, id);
			break;
		case MENU_USR_REM_OPERATOR:
			sdkResult = removeOperators(context, id);
			break;

		case MENU_USR_GET_MASTER_ADMIN:
			sdkResult = uc.getMasterAdmin(id);
			break;
		case MENU_USR_SET_MASTER_ADMIN:
			sdkResult = uc.setMasterAdmin(id);
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

		case MENU_USR_GET_LOCK_OVERRIDE:
			sdkResult = getLockOverride(uc, id);
			break;
		case MENU_USR_SET_LOCK_OVERRIDE:
			sdkResult = setLockOverride(uc, id);
			break;
		case MENU_USR_REM_LOCK_OVERRIDE:
			sdkResult = removeLockOverride(uc, id);
			break;
		case MENU_USR_CFG_USER_OVERRIDE:
			sdkResult = setUserOverrideConfig(dc, id);
			break;
		case MENU_USR_GET_USER_OVERRIDE:
			sdkResult = getUserOverride(uc, id);
			break;
		case MENU_USR_SET_USER_OVERRIDE:
			sdkResult = setUserOverride(uc, id);
			break;
		case MENU_USR_REM_USER_OVERRIDE:
			sdkResult = removeUserOverride(uc, id);
			break;
		default:
			break;
		}
	}

	return sdkResult;
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

int getOperators(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	vector<BS2Operator> oprList;
	int sdkResult = BS_SDK_SUCCESS;

	if (Utility::isYes("Are you sure you want to get ALL operators?"))
	{
		sdkResult = cc.getAllAuthOperatorLevelEx(id, oprList);
	}
	else
	{
		ostringstream msg;
		msg << "Please enter operators UID. ex)ID1 ID2 ID3 ..." << endl;
		string arrOprIDs = Utility::getLine(msg.str());
		if (0 == arrOprIDs.size())
			return BS_SDK_SUCCESS;

		vector<string> oprIDs = Utility::tokenizeString(arrOprIDs);

		sdkResult = cc.getAuthOperatorLevelEx(id, oprIDs, oprList);
	}

	switch (sdkResult)
	{
	case BS_SDK_SUCCESS:				// New version FW
		break;

	case BS_SDK_ERROR_NOT_SUPPORTED:	// Old version FW
		sdkResult = cc.getOperatorInAuthConfig(id, oprList);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;
		break;

	default:
		return sdkResult;
	}

	ConfigControl::print(oprList);
	cout << "Count of operator: " << oprList.size() << endl;

	return sdkResult;
}

int setOperators(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	vector<BS2Operator> oprList;

	while (true)
	{
		BS2Operator opr = { 0, };
		string userID = Utility::getInput<string>("Please enter the userID to set as the operator. [0:Stop]:");
		if (userID == "0")
		{
			break;
		}
		else if (BS2_USER_ID_SIZE < userID.size())
		{
			TRACE("Size of userID is %d", BS2_USER_ID_SIZE);
			continue;
		}

		strcpy(opr.userID, userID.c_str());
		ostringstream msg;
		msg << "Please enter the level of the operator:" << endl;
		msg << " (0: Not operator, 1: Admin, 2: Config operator, 3: User operator)" << endl;
		BS2_OPERATOR_LEVEL level = (BS2_OPERATOR_LEVEL)Utility::getInput<uint32_t>(msg.str());
		opr.level = level;
		oprList.push_back(opr);
	}

	int sdkResult = BS_SDK_SUCCESS;
	if (0 < oprList.size())
	{
		sdkResult = cc.setAuthOperatorLevelEx(id, oprList);
	}

	return sdkResult;
}

int removeOperators(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	int sdkResult = BS_SDK_SUCCESS;

	if (Utility::isYes("Are you sure you want to delete ALL operators?"))
	{
		sdkResult = cc.removeAllAuthOperatorLevelEx(id);
	}
	else
	{
		ostringstream msg;
		msg << "Please enter operators UID. ex)ID1 ID2 ID3 ..." << endl;
		string arrOprIDs = Utility::getLine(msg.str());
		if (0 == arrOprIDs.size())
			return BS_SDK_SUCCESS;

		vector<string> oprIDs = Utility::tokenizeString(arrOprIDs);

		sdkResult = cc.removeAuthOperatorLevelEx(id, oprIDs);
	}

	return sdkResult;
}

int scanCard(UserControl& uc, BS2_DEVICE_ID id)
{
	BS2Card card = { 0, };
	int sdkResult = uc.scanCard(id, &card);
	if (BS_SDK_SUCCESS == sdkResult)
		UserControl::print(card);

	return sdkResult;
}

int writeCard(UserControl& uc, BS2_DEVICE_ID id)
{
	int sdkResult = BS_SDK_SUCCESS;
	BS2SmartCardData card = { 0, };

	uint32_t uid(0);
	uint8_t cardBuff[BS2_CARD_DATA_SIZE] = { 0, };
	uint8_t cardType = BS2_CARD_TYPE_UNKNOWN;
	unsigned char* ptrUID = NULL;
	string msg = "Please select a card input type. [1. Scan from device, 2: Set user ID]";
	uint32_t cardInput = Utility::getInput<uint32_t>(msg);
	switch (cardInput)
	{
	case 1:
		cout << "Now scan your card." << endl;
		sdkResult = uc.scanCard(id, cardBuff, cardType);
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
	BS2TemplateEx templateEx = { 0, };

	if (Utility::isYes("Do you want to include fingerprint templates?"))
	{
		cout << "Now scan your fingerprint." << endl;
		sdkResult = uc.scanTemplate(id, fpTemplate);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		card.header.numOfTemplate = 1;
		card.header.numOfFaceTemplate = 0;
		card.header.cardAuthMode = BS2_AUTH_MODE_CARD_BIOMETRIC_OR_PIN;
		card.header.cardAuthModeEx = BS2_AUTH_MODE_NONE;
	}
	else if (Utility::isYes("Do you want to include VisualFace template?"))
	{
		cout << "Now scan your face." << endl;

		uint8_t imageData[BS2_MAX_WARPED_IMAGE_LENGTH] = { 0, };
		uint32_t imageLen = 0;

		int sdkResult = uc.getNormalizedImage(id, imageData, imageLen);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		sdkResult = uc.extractTemplateFaceEx(id, imageData, imageLen, templateEx);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		card.header.numOfTemplate = 0;
		card.header.numOfFaceTemplate = 1;
		card.header.cardAuthMode = BS2_AUTH_MODE_NONE;
		card.header.cardAuthModeEx = BS2_EXT_AUTH_MODE_CARD_FACE_OR_PIN;
	}
	else
	{
		card.header.numOfTemplate = 0;
		card.header.numOfFaceTemplate = 0;
		card.header.cardAuthMode = BS2_AUTH_MODE_CARD_PIN;
		card.header.cardAuthModeEx = BS2_AUTH_MODE_NONE;
	}

	if (0 < card.header.numOfTemplate)
	{
		memcpy(card.credentials.templateData, fpTemplate, BS2_FINGER_TEMPLATE_SIZE);
		card.header.templateSize = BS2_FINGER_TEMPLATE_SIZE;
	}
	else if (0 < card.header.numOfFaceTemplate)
	{
		memcpy(card.credentials.templateData, templateEx.data, BS2_FACE_EX_TEMPLATE_SIZE);
		card.header.templateSize = BS2_FACE_EX_TEMPLATE_SIZE;
	}

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

	string inputTime = Utility::getLine("Please enter start time [YYYY-MM-DD HH:MM:SS] ?", string("2000-01-01 00:00:00"));
	BS2_TIMESTAMP startTime = Utility::convertTimeString2UTC(inputTime);
	card.accessOnData.startTime = startTime;

	inputTime = Utility::getLine("Please enter end time [YYYY-MM-DD HH:MM:SS] ?", string("2030-12-31 23:59:59"));
	BS2_TIMESTAMP endTime = Utility::convertTimeString2UTC(inputTime);
	card.accessOnData.endTime = endTime;

	card.header.cardType = BS2_CARD_TYPE_ACCESS;
	card.header.issueCount = 1;
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

	uint8_t imageData[BS2_MAX_WARPED_IMAGE_LENGTH] = { 0, };
	uint32_t imageLen = 0;

	int sdkResult = uc.getNormalizedImage(id, imageData, imageLen);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	BS2TemplateEx templateEx = { 0, };
	sdkResult = uc.extractTemplateFaceEx(id, imageData, imageLen, templateEx);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	return sdkResult;
}

int getNormalizedImageFaceEx(void* context, BS2_DEVICE_ID id)
{
	UserControl uc(context);

	uint8_t imageData[BS2_MAX_WARPED_IMAGE_LENGTH] = { 0, };
	uint32_t imageLen = 0;

	int sdkResult = uc.getNormalizedImage(id, imageData, imageLen);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		if (Utility::isYes("Do you want to enroll a user with the warped image?"))
		{
			BS2FaceEx faceEx = { 0, };
			faceEx.faceIndex = 0;
			faceEx.numOfTemplate = 1;
			faceEx.flag = BS2_FACE_EX_FLAG_WARPED;
			faceEx.imageLen = imageLen;
			faceEx.irImageLen = 0;
			memcpy(faceEx.imageData, imageData, faceEx.imageLen);
			sdkResult = uc.enrollUserFaceEx(id, NULL, NULL, NULL, &faceEx);
		}
	}

	return sdkResult;
}

int updateUser(void* context, BS2_DEVICE_ID id)
{
	DeviceControl dc(context);
	UserControl uc(context);

	BS2SimpleDeviceInfo deviceInfo = { 0, };
	BS2SimpleDeviceInfoEx deviceInfoEx = { 0, };
	int sdkResult = dc.getDeviceInfoEx(id, deviceInfo, deviceInfoEx);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	ostringstream msg;
	BS2_USER_MASK mask = getMaskForUpdateUser();
	vector<BS2UserFaceExBlob> userList;

	do {
		BS2UserFaceExBlob userBlob = { 0, };
		BS2User& user = userBlob.user;
		BS2UserSetting& setting = userBlob.setting;
		BS2UserSettingEx& settingEx = userBlob.settingEx;

		setting.fingerAuthMode = BS2_AUTH_MODE_NONE;
		setting.cardAuthMode = BS2_AUTH_MODE_NONE;
		setting.idAuthMode = BS2_AUTH_MODE_NONE;

		settingEx.faceAuthMode = BS2_AUTH_MODE_NONE;
		settingEx.fingerprintAuthMode = BS2_AUTH_MODE_NONE;
		settingEx.cardAuthMode = BS2_AUTH_MODE_NONE;
		settingEx.idAuthMode = BS2_AUTH_MODE_NONE;

		if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobUserID(user)))
			return sdkResult;

		if ((mask & BS2_USER_MASK_SETTING) == BS2_USER_MASK_SETTING)
		{
			if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobExpiryDate(setting)))
				return sdkResult;

			if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobPrivateAuthMode(setting, deviceInfo, deviceInfoEx)))
				return sdkResult;

			if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobSecurityLevel(setting)))
				return sdkResult;
		}

		if ((mask & BS2_USER_MASK_SETTING_EX) == BS2_USER_MASK_SETTING_EX)
		{
			if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobPrivateAuthModeEx(settingEx, deviceInfo, deviceInfoEx)))
				return sdkResult;
		}

		if ((mask & BS2_USER_MASK_NAME) == BS2_USER_MASK_NAME)
		{
			msg.str("");
			msg << "Do you want to change/delete #" << user.userID << " name? (0:Change, 1:Delete)";
			uint32_t selected = Utility::getInput<uint32_t>(msg.str());
			switch (selected)
			{
			case 0:
				if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobUserName(userBlob.user_name, deviceInfo)))
					return sdkResult;
				user.infoMask |= BS2_USER_INFO_MASK_NAME;
				break;

			case 1:
			default:
				mask &= ~BS2_USER_MASK_NAME;
				break;
			}
		}
		else
		{
			// Keep
			user.infoMask |= BS2_USER_INFO_MASK_NAME;
		}

		if ((mask & BS2_USER_MASK_PHOTO) == BS2_USER_MASK_PHOTO)
		{
			msg.str("");
			msg << "Do you want to change/delete #" << user.userID << " profile image? (0:Change, 1:Delete)";
			uint32_t selected = Utility::getInput<uint32_t>(msg.str());
			switch (selected)
			{
			case 0:
				if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobProfileImage(userBlob, deviceInfo)))
					return sdkResult;
				user.infoMask |= BS2_USER_INFO_MASK_PHOTO;
				break;

			case 1:
			default:
				mask &= ~BS2_USER_MASK_PHOTO;
				break;
			}
		}
		else
		{
			// Keep
			user.infoMask |= BS2_USER_INFO_MASK_PHOTO;
		}

		if ((mask & BS2_USER_MASK_PIN) == BS2_USER_MASK_PIN)
		{
			msg.str("");
			msg << "Do you want to change/delete #" << user.userID << " PIN? (0:Change, 1:Delete)";
			uint32_t selected = Utility::getInput<uint32_t>(msg.str());
			switch (selected)
			{
			case 0:
				if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobPINCode(userBlob.pin, deviceInfo)))
					return sdkResult;
				user.infoMask |= BS2_USER_INFO_MASK_PIN;
				break;

			case 1:
			default:
				mask &= ~BS2_USER_MASK_PIN;
				break;
			}
		}
		else
		{
			// Keep
			user.infoMask |= BS2_USER_INFO_MASK_PIN;
		}

		if ((mask & BS2_USER_MASK_JOB) == BS2_USER_MASK_JOB)
		{
			msg.str("");
			msg << "Do you want to change/delete #" << user.userID << " jobs? (0:Change, 1:Delete)";
			uint32_t selected = Utility::getInput<uint32_t>(msg.str());
			switch (selected)
			{
			case 0:
				if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobJobCode(userBlob.job)))
					return sdkResult;
				user.infoMask |= BS2_USER_INFO_MASK_JOB_CODE;
				break;

			case 1:
			default:
				mask &= ~BS2_USER_MASK_JOB;
				break;
			}
		}
		else
		{
			// Keep
			user.infoMask |= BS2_USER_INFO_MASK_JOB_CODE;
		}

		if ((mask & BS2_USER_MASK_ACCESS_GROUP) == BS2_USER_MASK_ACCESS_GROUP)
		{
			if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobAccessGroupID(userBlob.accessGroupId)))
				return sdkResult;
		}

		if ((mask & BS2_USER_MASK_PHRASE) == BS2_USER_MASK_PHRASE)
		{
			msg.str("");
			msg << "Do you want to change/delete #" << user.userID << " private message? (0:Change, 1:Delete)";
			uint32_t selected = Utility::getInput<uint32_t>(msg.str());
			switch (selected)
			{
			case 0:
				if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobPhrase(userBlob.phrase, deviceInfo)))
					return sdkResult;
				user.infoMask |= BS2_USER_INFO_MASK_PHRASE;
				break;

			case 1:
			default:
				mask &= ~BS2_USER_MASK_PHRASE;
				break;
			}
		}
		else
		{
			// Keep
			user.infoMask |= BS2_USER_INFO_MASK_PHRASE;
		}

		user.numCards = 0;
		if ((mask & BS2_USER_MASK_CARD) == BS2_USER_MASK_CARD)
		{
			msg.str("");
			msg << "Do you want to change/delete #" << user.userID << " cards? (0:Change, 1:Delete)";
			uint32_t selected = Utility::getInput<uint32_t>(msg.str());
			switch (selected)
			{
			case 0:
				if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobCardInfo(&userBlob.cardObjs, user.numCards, id, deviceInfo, deviceInfoEx)))
					return sdkResult;
				user.infoMask |= BS2_USER_INFO_MASK_CARD;
				break;

			case 1:
			default:
				// unmasking and numCards = 0;
				mask &= ~BS2_USER_MASK_CARD;
				break;
			}
		}
		else
		{
			// Keep
			user.infoMask |= BS2_USER_INFO_MASK_CARD;
		}

		user.numFingers = 0;
		if ((mask & BS2_USER_MASK_FINGER) == BS2_USER_MASK_FINGER)
		{
			msg.str("");
			msg << "Do you want to change/delete #" << user.userID << " fingerprints? (0:Change, 1:Delete)";
			uint32_t selected = Utility::getInput<uint32_t>(msg.str());
			switch (selected)
			{
			case 0:
				if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobFingerprintInfo(&userBlob.fingerObjs, user.numFingers, id, deviceInfoEx)))
					return sdkResult;
				user.infoMask |= BS2_USER_INFO_MASK_FINGER;
				break;

			case 1:
			default:
				// unmasking and numFingers = 0;
				mask &= ~BS2_USER_MASK_FINGER;
				break;
			}
		}
		else
		{
			// Keep
			user.infoMask |= BS2_USER_INFO_MASK_FINGER;
		}

		user.numFaces = 0;
		if ((mask & BS2_USER_MASK_FACE) == BS2_USER_MASK_FACE)
		{
			msg.str("");
			msg << "Do you want to change/delete #" << user.userID << " face? (0:Change, 1:Delete)";
			uint32_t selected = Utility::getInput<uint32_t>(msg.str());
			switch (selected)
			{
			case 0:
				if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobFaceInfo(&userBlob.faceObjs, user.numFaces, id, deviceInfoEx)))
					return sdkResult;
				user.infoMask |= BS2_USER_INFO_MASK_FACE;
				break;

			case 1:
			default:
				// unmasking and numFace = 0;
				mask &= ~BS2_USER_MASK_FACE;
				break;
			}
		}
		else
		{
			if ((mask & BS2_USER_MASK_FACE_EX) == BS2_USER_MASK_FACE_EX)
			{
				msg.str("");
				msg << "Do you want to change/delete #" << user.userID << " visual face? (0:Change, 1:Delete)";
				uint32_t selected = Utility::getInput<uint32_t>(msg.str());
				switch (selected)
				{
				case 0:
					if (BS_SDK_SUCCESS != (sdkResult = uc.getUserBlobFaceInfo(&userBlob.faceExObjs, user.numFaces, id, deviceInfoEx)))
						return sdkResult;
					user.infoMask |= BS2_USER_INFO_MASK_FACE;
					break;

				case 1:
				default:
					// unmasking and numFace = 0;
					mask &= ~BS2_USER_MASK_FACE_EX;
					break;
				}
			}
			else
			{
				// Keep
				user.infoMask |= BS2_USER_INFO_MASK_FACE;
			}
		}
		
		userList.push_back(userBlob);

		msg.str("");
		msg << "Do you want other users to update too?";
	} while (Utility::isYes(msg.str()));

	sdkResult = uc.updateUser(id, mask, userList);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("User update failed: %d", sdkResult);

	for (auto item : userList)
	{
		if (item.cardObjs)
			delete[] item.cardObjs;

		if (item.fingerObjs)
			delete[] item.fingerObjs;

		if (item.faceObjs)
			delete[] item.faceObjs;

		if (item.faceExObjs)
			delete[] item.faceExObjs;
	}

	return sdkResult;
}

BS2_USER_MASK getMaskForUpdateUser()
{
	const vector<uint32_t> userUpdateItems = {
		BS2_USER_MASK_SETTING,
		BS2_USER_MASK_SETTING_EX,
		BS2_USER_MASK_NAME,
		BS2_USER_MASK_PHOTO,
		BS2_USER_MASK_PIN,
		BS2_USER_MASK_ACCESS_GROUP,
		BS2_USER_MASK_JOB,
		BS2_USER_MASK_PHRASE,
		BS2_USER_MASK_CARD,
		BS2_USER_MASK_FINGER,
		BS2_USER_MASK_FACE,
		BS2_USER_MASK_FACE_EX,
	};

	ostringstream msg;
	msg << "Select all of your items to update.  Ex)0, 1, 2, ..." << endl;
	msg << "  0: Setting" << endl;
	msg << "  1: SettingEx" << endl;
	msg << "  2: Name" << endl;
	msg << "  3: Photo" << endl;
	msg << "  4: PIN" << endl;
	msg << "  5: Access group" << endl;
	msg << "  6: Job" << endl;
	msg << "  7: Phrase" << endl;
	msg << "  8: Card" << endl;
	msg << "  9: Finger" << endl;
	msg << " 10: Face" << endl;
	msg << " 11: FaceEx" << endl;
	auto selectedNums = Utility::getLineNumbers<uint32_t>(msg.str());

	BS2_USER_MASK mask(0);
	for (auto item : selectedNums)
	{
		mask |= userUpdateItems[item];
	}

	TRACE("Selected mask: %u", mask);
	return mask;
}

int getUserStatistic(void* context, BS2_DEVICE_ID id)
{
	UserControl uc(context);

	BS2UserStatistic statistic = { 0, };
	int sdkResult = uc.getUserStatistic(id, statistic);
	if (BS_SDK_SUCCESS == sdkResult)
		UserControl::print(statistic);

	return sdkResult;
}

int selectUserID(const vector<array<char, BS2_USER_ID_SIZE>>& listUserIDs)
{
	cout << "--- Select User ID ---" << endl;
	for (uint32_t idx = 0; idx < listUserIDs.size(); idx++)
	{
		string strID(listUserIDs[idx].data(), BS2_USER_ID_SIZE);
		cout << idx << ": UserID[" << strID << "]" << endl;
	}

	cout << "999: N/A" << endl;
	int selected = Utility::getInput<int>("Enter the INDEX of the userID to select:");
	selected = (0 > selected || listUserIDs.size() < selected) ? -1 : selected;
	return selected;
}

int getLockOverride(UserControl& uc, BS2_DEVICE_ID id)
{
	vector<BS2LockOverride> request, response;

	if (!Utility::isYes("Do you want to get all Lock Overrides?"))
	{
		uint32_t numOfOverrides = Utility::getInput<uint32_t>("  Enter number of Lock Overrides to get:");
		BS2LockOverride item = { 0, };

		for (uint32_t idx = 0; idx < numOfOverrides; idx++)
		{
			string strCard = Utility::getLine("    Enter the CardID (Ex: E43DD05C)");
			if (BS2_CARD_DATA_SIZE * 2 < strCard.size())
			{
				TRACE("Card id should less than %u.", BS2_CARD_DATA_SIZE * 2);
				return BS_SDK_ERROR_INVALID_PARAM;
			}

			vector<unsigned char> cardIDArray = UserControl::HexStringToByteArray(strCard, BS2_CARD_DATA_SIZE);
			memcpy(item.cardID, cardIDArray.data(), BS2_CARD_DATA_SIZE);

			item.issueCount = Utility::getInput<uint32_t>("    Enter the Issue Count:");

			request.push_back(item);
		}
	}

	int sdkResult = uc.getLockOverrides(id, request, response);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		for (uint32_t idx = 0; idx < response.size(); idx++)
			uc.print(response[idx], idx);
	}

	return sdkResult;
}

int setLockOverride(UserControl& uc, BS2_DEVICE_ID id)
{
	vector<BS2LockOverride> listOverrides;
	int sdkResult = BS_SDK_SUCCESS;

	uint32_t numOfOverrides = Utility::getInput<uint32_t>("Enter number of Lock Overrides to set:");
	for (uint32_t idx = 0; idx < numOfOverrides; idx++)
	{
		BS2LockOverride item = { 0, };

		if (!Utility::isNo("Do you like to set the userID?"))
		{
			vector<array<char, BS2_USER_ID_SIZE>> listUserIDs;
			sdkResult = uc.getUserList(id, NULL, listUserIDs);

			if (BS_SDK_SUCCESS == sdkResult && 0 < listUserIDs.size())
			{
				int selected = selectUserID(listUserIDs);
				if (-1 != selected)
					memcpy(item.userID, listUserIDs[selected].data(), BS2_USER_ID_SIZE);
			}
		}

		cout << "Scanning card for Lock Override entry " << idx + 1 << "/" << numOfOverrides << "from device:" << id << "..." << endl;
		uint8_t cardBuff[BS2_CARD_DATA_SIZE] = { 0, };
		uint8_t cardType = BS2_CARD_TYPE_UNKNOWN;
		sdkResult = uc.scanCard(id, cardBuff, cardType);
		if (sdkResult != BS_SDK_SUCCESS)
			return sdkResult;

		memcpy(item.cardID, cardBuff, BS2_CARD_DATA_SIZE);
		item.size = BS2_CARD_DATA_SIZE;
		item.type = cardType;

		item.issueCount = (uint16_t)Utility::getInput<uint32_t>("Enter the Issue Count:");

		listOverrides.push_back(item);
	}

	return uc.setLockOverrides(id, listOverrides);
}

int removeLockOverride(UserControl& uc, BS2_DEVICE_ID id)
{
	vector<BS2LockOverride> listDelete;
	int sdkResult = BS_SDK_SUCCESS;

	if (!Utility::isYes("Do you want to remove all Lock Overrides?"))
	{
		vector<BS2LockOverride> requestAll;
		vector<BS2LockOverride> listResult;

		sdkResult = uc.getLockOverrides(id, requestAll, listResult);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		for (uint32_t idx = 0; idx < listResult.size(); idx++)
			uc.print(listResult[idx], idx);

		int delIndex = Utility::getInput<int>("Select the Lock Override INDEX to delete:");
		listDelete.push_back(listResult[delIndex]);
	}

	return uc.removeLockOverrides(id, listDelete);
}

int setUserOverrideConfig(DoorControl& dc, BS2_DEVICE_ID id)
{
	vector<BS2Door> listDoors;

	stringstream msg;
	BS2Door door = { 0, };
	msg << "Please enter the door ID:";
	door.doorID = Utility::getInput<BS2_DOOR_ID>(msg.str());
	sprintf(door.name, "TEST DOOR %u", door.doorID);
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

	door.dualAuthScheduleID = 0;
	door.dualAuthDevice = BS2_DUAL_AUTH_NO_DEVICE;
	door.dualAuthApprovalType = BS2_DUAL_AUTH_APPROVAL_NONE;
	door.extendedAutoLockTimeout = (uint16_t)Utility::getDefaultInput<uint32_t>("Please enter the extended door open time in seconds:", BS2_DEFAULT_EXTENDED_AUTO_LOCK_TIMEOUT);
	door.dualAuthTimeout = 1;
	door.numDualAuthApprovalGroups = 0;

	listDoors.push_back(door);

	int sdkResult = dc.setDoor(id, listDoors);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		vector<BS2Door> listResult;
		sdkResult = dc.getAllDoor(id, listResult);
		if (BS_SDK_SUCCESS == sdkResult)
		{
			for (auto item : listResult)
				dc.print(item);
		}
	}

	return sdkResult;
}

int getUserOverride(UserControl& uc, BS2_DEVICE_ID id)
{
	vector<array<char, BS2_USER_ID_SIZE>> listUserIDs, request;
	vector<BS2UserOverride> response;
	int sdkResult = BS_SDK_SUCCESS;

	if (!Utility::isYes("Do you want to get all User Overrides?"))
	{
		uint32_t numOfOverides = Utility::getInput<uint32_t>("  Enter number of User Overrides to get:");

		sdkResult = uc.getUserList(id, NULL, listUserIDs);
		if (BS_SDK_SUCCESS == sdkResult && 0 < listUserIDs.size())
		{
			do
			{
				int selected = selectUserID(listUserIDs);
				if (-1 != selected)
					request.push_back(listUserIDs[selected]);
			} while (request.size() < numOfOverides);
		}
	}

	sdkResult = uc.getUserOverrides(id, request, response);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		for (uint32_t idx = 0; idx < response.size(); idx++)
			uc.print(response[idx], idx);
	}

	return sdkResult;
}

int setUserOverride(UserControl& uc, BS2_DEVICE_ID id)
{
	vector<BS2UserOverride> listOverrides;
	int sdkResult = BS_SDK_SUCCESS;

	uint32_t numOfOverrides = Utility::getInput<uint32_t>("Enter number of User Overrides to set:");
	while (listOverrides.size() < numOfOverrides)
	{
		BS2UserOverride item = { 0, };
		vector<array<char, BS2_USER_ID_SIZE>> listUserIDs;

		sdkResult = uc.getUserList(id, NULL, listUserIDs);
		if (BS_SDK_SUCCESS == sdkResult && 0 < listUserIDs.size())
		{
			int selected = selectUserID(listUserIDs);
			if (-1 != selected)
			{
				memcpy(item.userID, listUserIDs[selected].data(), BS2_USER_ID_SIZE);
				item.useExtendedAutoLockTimeout = Utility::isYes("Do you want to use Extended Door Open Time?");
			}
		}

		listOverrides.push_back(item);
	}

	return uc.setUserOverrides(id, listOverrides);
}

int removeUserOverride(UserControl& uc, BS2_DEVICE_ID id)
{
	vector<array<char, BS2_USER_ID_SIZE>> listDelete;
	int sdkResult = BS_SDK_SUCCESS;

	if (!Utility::isYes("Do you want to remove all Lock Overrides?"))
	{
		vector<array<char, BS2_USER_ID_SIZE>> requestAll;
		vector<BS2UserOverride> listResult;

		sdkResult = uc.getUserOverrides(id, requestAll, listResult);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		for (uint32_t idx = 0; idx < listResult.size(); idx++)
			uc.print(listResult[idx], idx);

		int delIndex = Utility::getInput<int>("Select the User Override INDEX to delete:");
		array<char, BS2_USER_ID_SIZE> item;
		memcpy(item.data(), listResult[delIndex].userID, BS2_USER_ID_SIZE);
		listDelete.push_back(item);
	}

	return uc.removeUserOverrides(id, listDelete);
}
