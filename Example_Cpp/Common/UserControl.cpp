#include "stdafx.h"
#include <iostream>
#include <sstream>
#include <cassert>
#include <memory>
#include <algorithm>
#include "UserControl.h"
#include "BS_Errno.h"
#include "../Common/Utils.h"
#include "../Common/ConfigControl.h"


extern void TRACE(const char* fmt, ...);
using namespace std;

void* UserControl::context_ = NULL;

UserControl::UserControl(void* sdkContext)
{
	context_ = sdkContext;
}


UserControl::~UserControl()
{
}

int UserControl::isAcceptableUID(const char* uid)
{
	const size_t idLen = strlen(uid);
	const string idMax = MAX_SUPPORTED_UID;
	if (idMax.size() < idLen)
		return 0;

	if (idMax.size() == idLen && idMax.compare(uid) < 0)
		return 0;

	return 1;
}

void UserControl::onReadyToScan(BS2_DEVICE_ID id, uint32_t sequence)
{
	cout << "Place your card(/finger/face) on the device. [" << id << ", " << "Seq:" << sequence << "]" << endl;
}

int UserControl::getUser(BS2_DEVICE_ID id)
{
	string uid = Utils::getInput<string>("Please enter a user ID :");
	if (BS2_USER_ID_SIZE < uid.size())
	{
		TRACE("User ID is too big.");
		return BS_SDK_ERROR_INVALID_PARAM;
	}

	BS2SimpleDeviceInfo deviceInfo = { 0, };
	BS2AuthConfig authConfig = { 0, };
	stringstream msg;
	msg << "What do you want :\n";
	msg << "[1: User header, 2: Card, 3: Finger, 4: Face]";
	short maskType = Utils::getInput<short>(msg.str());

	int sdkResult = BS2_GetDeviceInfo(context_, id, &deviceInfo);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		sdkResult = BS2_GetAuthConfig(context_, id, &authConfig);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_GetAuthConfig call failed: %d", sdkResult);
			return sdkResult;
		}
	}
	else
	{
		TRACE("BS2_GetDeviceInfo call failed: %d", sdkResult);
		return sdkResult;
	}

	uint32_t numUser = 1;
	BS2_USER_MASK userMask = 0;
	size_t dataSize = 0;
	//shared_ptr<uint8_t> ptrUID(new uint8_t[BS2_USER_ID_SIZE], ArrayDeleter<uint8_t>());

	userMask = BS2_USER_MASK_DATA | BS2_USER_MASK_SETTING;
	switch (maskType)
	{
	case BS_USER_HEADER_ONLY:
		userMask |= BS2_USER_MASK_ACCESS_GROUP;
		if (deviceInfo.userNameSupported)
			userMask |= BS2_USER_MASK_NAME;
		if (deviceInfo.pinSupported)
			userMask |= BS2_USER_MASK_PIN;
		if (deviceInfo.userPhotoSupported)
			userMask |= BS2_USER_MASK_PHOTO;
		break;
	case BS_USER_CRED_CARD:
		if (deviceInfo.cardSupported)
			userMask |= BS2_USER_MASK_CARD;
		break;
	case BS_USER_CRED_FINGER:
		if (deviceInfo.fingerSupported)
			userMask |= BS2_USER_MASK_FINGER;
		break;
	case BS_USER_CRED_FACE:
		if (deviceInfo.faceSupported)
			userMask |= BS2_USER_MASK_FACE;
		break;
	default:
		return BS_SDK_ERROR_INVALID_PARAM;
	}

	BS2UserBlob userBlob = { 0, };
	sdkResult = BS2_GetUserDatas(context_, id, const_cast<char*>(uid.c_str()), numUser, &userBlob, userMask);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserDatas call failed: %d", sdkResult);
		return sdkResult;
	}

	switch (maskType)
	{
	case BS_USER_HEADER_ONLY:
		printHeader(userBlob);
		break;
	case BS_USER_CRED_CARD:
		print(userBlob.cardObjs, userBlob.user.numCards);
		if (0 < userBlob.user.numCards)
			BS2_ReleaseObject(userBlob.cardObjs);
		break;
	case BS_USER_CRED_FINGER:
		print(userBlob.fingerObjs, userBlob.user.numFingers);
		if (0 < userBlob.user.numFingers)
			BS2_ReleaseObject(userBlob.fingerObjs);
		break;
	case BS_USER_CRED_FACE:
		print(userBlob.faceObjs, userBlob.user.numFaces);
		if (0 < userBlob.user.numFaces)
			BS2_ReleaseObject(userBlob.faceObjs);
		break;
	}

	return sdkResult;
}

int UserControl::getAllUser(BS2_DEVICE_ID id)
{
	char* uidObj = NULL;
	uint32_t numUID = 0;

	int sdkResult = BS2_GetUserList(context_, id, &uidObj, &numUID, isAcceptableUID);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserList call failed: %d", sdkResult);
		return sdkResult;
	}

	const int MAX_USER_PAGE_COUNT = 2048;
	vector<BS2UserBlob> userBlob(MAX_USER_PAGE_COUNT);
	BS2_USER_MASK userMask = BS2_USER_MASK_ALL;

	for (uint32_t idx = 0; idx < numUID; idx += MAX_USER_PAGE_COUNT)
	{
		uint32_t availUserCount = min<uint32_t>(MAX_USER_PAGE_COUNT, numUID - idx);

		int sdkResult = BS2_GetUserDatas(context_, id, uidObj + BS2_USER_ID_SIZE * idx, availUserCount, &userBlob[0], userMask);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_GetUserDatas call failed: %d", sdkResult);
			if (uidObj)
				BS2_ReleaseObject(uidObj);
			return sdkResult;
		}

		for (uint32_t pageIdx = 0; pageIdx < availUserCount; pageIdx++)
		{
			print(userBlob[pageIdx]);

			if (0 < userBlob[pageIdx].user.numCards && userBlob[pageIdx].cardObjs)
				BS2_ReleaseObject(userBlob[pageIdx].cardObjs);
			if (0 < userBlob[pageIdx].user.numFingers && userBlob[pageIdx].fingerObjs)
				BS2_ReleaseObject(userBlob[pageIdx].fingerObjs);
			if (0 < userBlob[pageIdx].user.numFaces && userBlob[pageIdx].faceObjs)
				BS2_ReleaseObject(userBlob[pageIdx].faceObjs);
		}
	}

	if (uidObj)
		BS2_ReleaseObject(uidObj);

	return sdkResult;
}

int UserControl::enrollUser(BS2_DEVICE_ID id)
{
	BS2SimpleDeviceInfo deviceInfo = { 0, };
	BS2AuthConfig authConfig = { 0, };

	int sdkResult = BS2_GetDeviceInfo(context_, id, &deviceInfo);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		sdkResult = BS2_GetAuthConfig(context_, id, &authConfig);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_GetAuthConfig call failed: %d", sdkResult);
			return sdkResult;
		}
	}
	else
	{
		TRACE("BS2_GetDeviceInfo call failed: %d", sdkResult);
		return sdkResult;
	}

	BS2UserBlob userBlob = { 0, };
	BS2User& user = userBlob.user;
	BS2UserSetting& setting = userBlob.setting;
	BS2UserPhoto& photo = userBlob.user_photo;
	stringstream msg;

	string uid = Utils::getInput<string>("Please enter a user ID:");
	if (BS2_USER_ID_SIZE < uid.size())
	{
		TRACE("User ID is too big.");
		return BS_SDK_ERROR_INVALID_PARAM;
	}
	strcpy(user.userID, uid.c_str());

	if (deviceInfo.userNameSupported)
	{
		string name = Utils::getInput<string>("Enter your name:");
		if (BS2_USER_NAME_SIZE < uid.size())
		{
			TRACE("User name is too long.");
			return BS_SDK_ERROR_INVALID_PARAM;
		}
		strcpy(reinterpret_cast<char*>(userBlob.user_name), name.c_str());
	}

	{
		string inputTime = Utils::getLine("Please enter start time [YYYY-MM-DD HH:MM:SS] ?");
		BS2_TIMESTAMP startTime = Utils::convertTimeString2UTC(inputTime);
		setting.startTime = startTime;

		inputTime = Utils::getLine("Please enter end time [YYYY-MM-DD HH:MM:SS] ?");
		BS2_TIMESTAMP endTime = Utils::convertTimeString2UTC(inputTime);
		setting.endTime = endTime;
	}

	if (deviceInfo.pinSupported)
	{
		string pinString = Utils::getInput<string>("Enter the PIN code:");
		if (BS2_USER_PIN_SIZE < pinString.size())
		{
			TRACE("PIN code is too long");
			return BS_SDK_ERROR_INVALID_PARAM;
		}

		sdkResult = BS2_MakePinCode(context_, const_cast<char*>(pinString.c_str()), userBlob.pin);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_MakePinCode call failed: %d", sdkResult);
			return sdkResult;
		}
	}
	if (deviceInfo.fingerSupported || deviceInfo.faceSupported)
	{
		msg.str("");
		msg << "Enter the biometric authentication mode\n";
		msg << "[1: Biometric only, 2: Biometric+PIN]";
		int fingerAuthMode = Utils::getInput<int>(msg.str());
		switch (fingerAuthMode)
		{
		case 1:
			setting.fingerAuthMode = BS2_AUTH_MODE_BIOMETRIC_ONLY;
			break;
		case 2:
			setting.fingerAuthMode = deviceInfo.pinSupported ? BS2_AUTH_MODE_BIOMETRIC_PIN : BS2_AUTH_MODE_BIOMETRIC_ONLY;
			break;
		default:
			setting.fingerAuthMode = BS2_AUTH_MODE_NONE;
			break;
		}
	}

	if (deviceInfo.cardSupported)
	{
		msg.str("");
		msg << "Enter the card authentication mode\n";
		msg << "[1: Card only, 2: Card+Biometric, 3: Card+PIN, 4: Card+(Biometric/PIN), 5: Card+Biometric+PIN]";
		int cardAuthMode = Utils::getInput<int>(msg.str());
		switch (cardAuthMode)
		{
		case 1:
			setting.cardAuthMode = BS2_AUTH_MODE_CARD_ONLY;
			break;
		case 2:
			setting.cardAuthMode = (deviceInfo.fingerSupported || deviceInfo.faceSupported) ? BS2_AUTH_MODE_CARD_BIOMETRIC : BS2_AUTH_MODE_CARD_ONLY;
			break;
		case 3:
			setting.cardAuthMode = deviceInfo.pinSupported ? BS2_AUTH_MODE_CARD_PIN : BS2_AUTH_MODE_CARD_ONLY;
			break;
		case 4:
			setting.cardAuthMode = (deviceInfo.fingerSupported || deviceInfo.faceSupported || deviceInfo.pinSupported) ? BS2_AUTH_MODE_CARD_BIOMETRIC_OR_PIN : BS2_AUTH_MODE_CARD_ONLY;
			break;
		case 5:
			setting.cardAuthMode = (deviceInfo.fingerSupported || deviceInfo.faceSupported || deviceInfo.pinSupported) ? BS2_AUTH_MODE_CARD_BIOMETRIC_PIN : BS2_AUTH_MODE_CARD_ONLY;
			break;
		default:
			setting.cardAuthMode = BS2_AUTH_MODE_NONE;
			break;
		}
	}

	{
		msg.str("");
		msg << "Enter the ID authentication mode\n";
		msg << "[1: ID+Biometric, 2: ID+PIN, 3: ID+(Biometric/PIN), 4: ID+Biometric+PIN]";
		int idAuthMode = Utils::getInput<int>(msg.str());
		switch (idAuthMode)
		{
		case 1:
			setting.idAuthMode = (deviceInfo.fingerSupported || deviceInfo.faceSupported) ? BS2_AUTH_MODE_ID_BIOMETRIC : BS2_AUTH_MODE_NONE;
			break;
		case 2:
			setting.idAuthMode = deviceInfo.pinSupported ? BS2_AUTH_MODE_ID_PIN : BS2_AUTH_MODE_NONE;
			break;
		case 3:
			setting.idAuthMode = (deviceInfo.fingerSupported || deviceInfo.faceSupported || deviceInfo.pinSupported) ? BS2_AUTH_MODE_ID_BIOMETRIC_OR_PIN : BS2_AUTH_MODE_NONE;
			break;
		case 4:
			setting.idAuthMode = (deviceInfo.fingerSupported || deviceInfo.faceSupported || deviceInfo.pinSupported) ? BS2_AUTH_MODE_ID_BIOMETRIC_PIN : BS2_AUTH_MODE_NONE;
			break;
		default:
			setting.idAuthMode = BS2_AUTH_MODE_NONE;
			break;
		}
	}

	{
		msg.str("");
		msg << "Enter the security level for this user\n";
		msg << "[0: Default, 1: Lower, 2: Low, 3: Normal, 4: High, 5, Higher]";
		int securityLevel = Utils::getInput<int>(msg.str());
		switch (securityLevel)
		{
		case BS2_USER_SECURITY_LEVEL_DEFAULT:
		case BS2_USER_SECURITY_LEVEL_LOWER:
		case BS2_USER_SECURITY_LEVEL_LOW:
		case BS2_USER_SECURITY_LEVEL_NORMAL:
		case BS2_USER_SECURITY_LEVEL_HIGH:
		case BS2_USER_SECURITY_LEVEL_HIGHER:
			setting.securityLevel = securityLevel;
			break;
		default:
			setting.securityLevel = BS2_USER_SECURITY_LEVEL_DEFAULT;
			break;
		}
	}

	if (deviceInfo.userPhotoSupported)
	{
		char profileImage = Utils::getInput<char>("Do you want to register a profile image? [y/n]");
		if ('y' == profileImage || 'Y' == profileImage)
		{
			string imagePath = Utils::getInput<string>("Enter the profile image path and name:");
			uint32_t size = Utils::getResourceSize(imagePath);
			shared_ptr<uint8_t> buffer(new uint8_t[size], ArrayDeleter<uint8_t>());

			while (BS2_USER_PHOTO_SIZE < size)
			{
				msg.str("");
				msg << "Image is to big.\n";
				msg << "Re-enter an image smaller than 16384 byte:";
				imagePath = Utils::getInput<string>(msg.str());
				size = Utils::getResourceSize(imagePath);
			}

			if (Utils::getResourceFromFile(imagePath, buffer, size))
			{
				photo.size = size;
				memcpy(photo.data, buffer.get(), size);
			}
		}
	}

	char flag = Utils::getInput<char>("Do you want register access group ID? [y/n]");
	if ('y' == flag || 'Y' == flag)
	{
		msg.str("");
		msg << "Please enter access group IDs. ex)ID1 ID2 ID3 ...\n";
		string inStrAGID = Utils::getLine(msg.str());
		if (0 == inStrAGID.size())
			return BS_SDK_ERROR_CANNOT_FIND_ACCESS_GROUP;

		vector<string> listID = Utils::tokenizeString(inStrAGID);
		if (listID.size() < BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER)
		{
			uint32_t index(0);
			for (auto groupID : listID)
			{
				BS2_ACCESS_GROUP_ID gid = atoi(groupID.c_str());
				userBlob.accessGroupId[index++] = gid;
			}
		}
	}

	{
		msg.str("");
		msg << "Please enter a authentication group ID.\n";
		msg << "This is used for face authentication. [0: Not using]";
		uint32_t authGroupID = Utils::getInput<uint32_t>(msg.str());
		user.authGroupID = authGroupID;
	}

	{
		flag = Utils::getInput<char>("Do you want to overwrite the user if it exist? [y/n]");
		user.flag = (flag == 'y' || flag == 'Y') ? BS2_USER_FLAG_CREATED | BS2_USER_FLAG_UPDATED : BS2_USER_FLAG_CREATED;
	}

	user.numFingers = 0;
	user.numCards = 0;
	user.numFaces = 0;

	if (deviceInfo.cardSupported)
	{
		flag = Utils::getInput<char>("Do you want scan card? [y/n]");
		if ('y' == flag || 'Y' == flag)
		{
			uint32_t numCard = Utils::getInput<uint32_t>("How many cards would you like to register?");
			BS2CSNCard* ptrCard = new BS2CSNCard[numCard];
			if (ptrCard)
			{
				userBlob.cardObjs = ptrCard;
				for (uint32_t index = 0; index < numCard;)
				{
					BS2Card card = { 0, };
					sdkResult = BS2_ScanCard(context_, id, &card, onReadyToScan);
					if (BS_SDK_SUCCESS != sdkResult)
						TRACE("BS2_ScanCard call failed: %d", sdkResult);
					else
					{
						if (card.isSmartCard)
						{
							TRACE("CSN card only supported.");
						}
						else
						{
							memcpy(&ptrCard[index], &card.card, sizeof(BS2CSNCard));
						}
						user.numCards++;
						index++;
					}
				}
			}
		}
	}

	if (deviceInfo.fingerSupported)
	{
		flag = Utils::getInput<char>("Do you want scan fingerprint? [y/n]");
		if ('y' == flag || 'Y' == flag)
		{
			uint32_t numFinger = Utils::getInput<uint32_t>("How many fingers would you like to register?");
			BS2Fingerprint* ptrFinger = new BS2Fingerprint[numFinger];
			if (ptrFinger)
			{
				userBlob.fingerObjs = ptrFinger;
				for (uint32_t index = 0; index < numFinger; index++)
				{
					for (uint32_t templateIndex = 0; templateIndex < BS2_TEMPLATE_PER_FINGER;)
					{
						sdkResult = BS2_ScanFingerprint(context_, id, &ptrFinger[index], templateIndex, BS2_FINGER_TEMPLATE_QUALITY_HIGHEST, BS2_FINGER_TEMPLATE_FORMAT_SUPREMA, onReadyToScan);
						if (BS_SDK_SUCCESS != sdkResult)
							TRACE("BS2_ScanFingerprint call failed: %d", sdkResult);
						else
							templateIndex++;
					}
					user.numFingers++;
				}
			}
		}
	}

	if (deviceInfo.faceSupported)
	{
		flag = Utils::getInput<char>("Do you want scan face? [y/n]");
		if ('y' == flag || 'Y' == flag)
		{
			uint32_t numFace = Utils::getInput<uint32_t>("How many face would you like to register?");
			BS2Face* ptrFace = new BS2Face[numFace];
			if (ptrFace)
			{
				userBlob.faceObjs = ptrFace;
				for (uint32_t index = 0; index < numFace;)
				{
					sdkResult = BS2_ScanFace(context_, id, &ptrFace[index], BS2_FACE_ENROLL_THRESHOLD_DEFAULT, onReadyToScan);
					if (BS_SDK_SUCCESS != sdkResult)
						TRACE("BS2_ScanFace call failed: %d", sdkResult);
					else
					{
						user.numFaces++;
						index++;
					}
				}
			}
		}
	}

	sdkResult = BS2_EnrolUser(context_, id, &userBlob, 1, 1);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_EnrolUser call failed: %d", sdkResult);

	if (userBlob.cardObjs)
		delete[] userBlob.cardObjs;

	if (userBlob.fingerObjs)
		delete[] userBlob.fingerObjs;

	if (userBlob.faceObjs)
		delete[] userBlob.faceObjs;

	return sdkResult;
}

int UserControl::getLastFingerprintImage(BS2_DEVICE_ID id, uint8_t** imageObj, uint32_t* width, uint32_t* height)
{
	int sdkResult = BS2_GetLastFingerprintImage(context_, id, imageObj, width, height);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetLastFingerprintImage call failed: %d", sdkResult);

	return sdkResult;
}

int UserControl::removeUser(BS2_DEVICE_ID id)
{
	string uid = Utils::getInput<string>("Please enter a user ID:");
	if (BS2_USER_ID_SIZE < uid.size())
	{
		TRACE("User ID is too big.");
		return BS_SDK_ERROR_INVALID_PARAM;
	}

	int sdkResult = BS2_RemoveUser(context_, id, const_cast<char*>(uid.c_str()), 1);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_RemoveUser call failed: %d", sdkResult);

	return sdkResult;
}

int UserControl::removeAllUser(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_RemoveAllUser(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_RemoveAllUser call failed: %d", sdkResult);

	return sdkResult;
}

void UserControl::print(const BS2UserBlob& userBlob)
{
	TRACE("==[BS2UserBlob]==");
	print(userBlob.user);
	print(userBlob.setting);
	TRACE("user_name:%s", userBlob.user_name);
	print(userBlob.user_photo);
	print(userBlob.cardObjs, userBlob.user.numCards);
	print(userBlob.fingerObjs, userBlob.user.numFingers);
	print(userBlob.faceObjs, userBlob.user.numFaces);
	for (int i = 0; i < BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER; i++)
	{
		TRACE("[%d] accessGroupId:%u", i, userBlob.accessGroupId[i]);
	}
}

void UserControl::printHeader(const BS2UserBlob& userBlob)
{
	TRACE("==[BS2UserBlob Header]==");
	print(userBlob.user);
	print(userBlob.setting);
	TRACE("user_name:%s", userBlob.user_name);
	print(userBlob.user_photo);
	for (int i = 0; i < BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER; i++)
	{
		TRACE("[%d] accessGroupId:%u", i, userBlob.accessGroupId[i]);
	}
}

void UserControl::print(const BS2User& user)
{
	TRACE("==[BS2User]==");
	TRACE("userID:%s", user.userID);
	TRACE("formatVersion:%u", user.formatVersion);
	TRACE("flag:%u", user.flag);
	TRACE("version:%u", user.version);
	TRACE("numCards:%u", user.numCards);
	TRACE("numFingers:%u", user.numFingers);
	TRACE("numFaces:%u", user.numFaces);
	TRACE("authGroupID:%u", user.authGroupID);
	TRACE("faceChecksum:%u", user.faceChecksum);
}

void UserControl::print(const BS2UserSetting& settings)
{
	TRACE("==[BS2UserSetting]==");
	TRACE("startTime:%u", settings.startTime);
	TRACE("endTime:%u", settings.endTime);
	TRACE("fingerAuthMode:%u", settings.fingerAuthMode);
	TRACE("cardAuthMode:%u", settings.cardAuthMode);
	TRACE("idAuthMode:%u", settings.idAuthMode);
	TRACE("securityLevel:%u", settings.securityLevel);
}

void UserControl::print(const BS2UserPhoto& photo)
{
	TRACE("==[BS2UserPhoto]==");
	TRACE("size:%u", photo.size);

	if (0 < photo.size)
	{
		shared_ptr<uint8_t> buffer(new uint8_t[photo.size], ArrayDeleter<uint8_t>());
		memcpy(buffer.get(), photo.data, photo.size);
		Utils::setResourceToFile("c:\\temp\\temp.jpg", buffer, photo.size);
	}
}

void UserControl::print(const BS2CSNCard* card, uint8_t numCard)
{
	TRACE("==[BS2CSNCard]==");
	for (int i = 0; i < numCard; i++)
	{
		TRACE("[%d] type:%u", i, card[i].type);
		TRACE("[%d] size:%u", i, card[i].size);
		//TRACE([%d] "data:%s", i, card[i].data);
	}
}

void UserControl::print(const BS2Fingerprint* finger, uint8_t numFinger)
{
	TRACE("==[BS2Fingerprint]==");
	for (int i = 0; i < numFinger; i++)
	{
		TRACE("[%d] index:%u", i, finger[i].index);
		TRACE("[%d] flag:%u", i, finger[i].flag);
		//TRACE("[%d] data:%s", i, finger[i].data[0]);
		//TRACE("[%d] data:%s", i, finger[i].data[1]);
	}
}

void UserControl::print(const BS2Face* face, uint8_t numFace)
{
	TRACE("==[BS2Face]==");
	for (int i = 0; i < numFace; i++)
	{
		TRACE("[%d] faceIndex:%u", i, face[i].faceIndex);
		TRACE("[%d] numOfTemplate:%u", i, face[i].numOfTemplate);
		TRACE("[%d] flag:%u", i, face[i].flag);
		TRACE("[%d] imageLen:%u", i, face[i].imageLen);

		//for (int row = 0; row < BS2_TEMPLATE_PER_FACE; row++)
		//{
		//	TRACE("[%d] %s", i, face[i].templateData[row]);
		//	//for (int col = 0; col < BS2_FACE_TEMPLATE_LENGTH; col++)
		//	//{
		//	//	//TRACE("[%d][%d] %s", i, row, face[i].templateData[row][col]);
		//	//}
		//}
	}
}