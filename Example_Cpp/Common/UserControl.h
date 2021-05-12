#pragma once

#include <vector>
#include <string>
#include "BS_API.h"

#define MAX_SUPPORTED_UID		"4294967295"

enum
{
	BS_USER_HEADER_ONLY = 1,
	BS_USER_CRED_CARD = 2,
	BS_USER_CRED_FINGER = 3,
	BS_USER_CRED_FACE = 4
};

typedef struct
{
	std::string fileName;
	uint32_t size;
} UserImageInfo;

class BS2UIDArray
{
public:
	BS2UIDArray() : uid(NULL) {}
	virtual ~BS2UIDArray() { if (uid) delete[] uid; }

	void append(const std::string& user) { listUser.push_back(user); }
	void append(const std::vector<std::string>& list) {for (auto id : list) append(id);}
	uint32_t getSize() { return static_cast<uint32_t>(listUser.size()); }
	char* getPtr() {
		int index = 0;
		uid = new char[BS2_USER_ID_SIZE * listUser.size()];
		for (auto op : listUser)
		{
			//TRACE("ID: %s", op.c_str());
			strcpy(uid + (BS2_USER_ID_SIZE * index++), op.c_str());
		}
		return uid;
	}

private:
	char* uid;
	std::vector<std::string> listUser;
};


class UserControl
{
public:
	UserControl(void* sdkContext);
	virtual ~UserControl();


public:
	static int isAcceptableUID(const char* uid);
	static void onReadyToScan(BS2_DEVICE_ID id, uint32_t sequence);
	int getUser(BS2_DEVICE_ID id);
	int getAllUser(BS2_DEVICE_ID id);
	int getAllUserFaceEx(BS2_DEVICE_ID id);
	int enrollUser(BS2_DEVICE_ID id);
	int enrollUserSmall(BS2_DEVICE_ID id);
	int getLastFingerprintImage(BS2_DEVICE_ID id, uint8_t** imageObj, uint32_t* width, uint32_t* height);
	int removeUser(BS2_DEVICE_ID id);
	int removeAllUser(BS2_DEVICE_ID id);
	int getUserFaceEx(BS2_DEVICE_ID id);
	int makeUserFaceEx(BS2_DEVICE_ID id, BS2UserFaceExBlob* userBlob);
	int makeUserFaceExWithImage(BS2_DEVICE_ID id, const BS2TemplateEx& templateEx, BS2UserFaceExBlob* userBlob);
	int enrollUserFaceEx(BS2_DEVICE_ID id, BS2CSNCard* card = NULL, BS2Fingerprint* finger = NULL, BS2Face* face = NULL, BS2FaceEx* faceEx = NULL);
	int enrollUserFaceExScanAndLoad(BS2_DEVICE_ID id);

	int updateCardTypeCRC(BS2SmartCardData& card);
	int updateCardCRC(BS2SmartCardData& card);
	int getPinCode(std::string plainText, uint8_t* cipherText);
	int scanTemplate(BS2_DEVICE_ID id, uint8_t* fpTemplate);
	int scanCard(BS2_DEVICE_ID id, uint8_t* card);
	int scanCard(BS2_DEVICE_ID id, BS2Card* card);
	int writeCard(BS2_DEVICE_ID id, const BS2SmartCardData* card);
	int eraseCard(BS2_DEVICE_ID id);

	void print(const BS2Card& card);

	int getUserBlobUserID(BS2User& user);
	int getUserBlobUserName(BS2UserBlob& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobUserName(BS2UserBlobEx& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobUserName(BS2UserSmallBlob& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobUserName(BS2UserSmallBlobEx& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobUserName(BS2UserFaceExBlob& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobExpiryDate(BS2UserSetting& setting);
	int getUserBlobPINCode(BS2UserBlob& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobPINCode(BS2UserBlobEx& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobPINCode(BS2UserSmallBlob& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobPINCode(BS2UserSmallBlobEx& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobPINCode(BS2UserFaceExBlob& userBlob, const BS2SimpleDeviceInfo& deviceInfo);

	int getUserBlobPrivateAuthMode(BS2UserSetting& setting, const BS2SimpleDeviceInfo& deviceInfo, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobPrivateAuthModeEx(BS2UserSettingEx& setting, const BS2SimpleDeviceInfo& deviceInfo, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobSecurityLevel(BS2UserSetting& setting);
	int getUserBlobProfileImage(BS2UserSmallBlob& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobProfileImage(BS2UserSmallBlobEx& userBlob, const BS2SimpleDeviceInfo& deviceInfo);
	int getUserBlobProfileImage(BS2UserFaceExBlob& userBlob, const BS2SimpleDeviceInfo& deviceInfo);

	int getUserBlobAccessGroupID(BS2UserBlob& userBlob);
	int getUserBlobAccessGroupID(BS2UserBlobEx& userBlob);
	int getUserBlobAccessGroupID(BS2UserSmallBlob& userBlob);
	int getUserBlobAccessGroupID(BS2UserSmallBlobEx& userBlob);
	int getUserBlobAccessGroupID(BS2UserFaceExBlob& userBlob);
	int getUserBlobFaceAuthGroupID(BS2User& user);
	int getUserBlobUserUpdate(BS2User& user);
	int getUserBlobCardInfo(BS2UserBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfo& deviceInfo, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobCardInfo(BS2UserBlobEx& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfo& deviceInfo, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobCardInfo(BS2UserSmallBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfo& deviceInfo, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobCardInfo(BS2UserSmallBlobEx& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfo& deviceInfo, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobCardInfo(BS2UserFaceExBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfo& deviceInfo, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobFingerprintInfo(BS2UserBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobFingerprintInfo(BS2UserBlobEx& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobFingerprintInfo(BS2UserSmallBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobFingerprintInfo(BS2UserSmallBlobEx& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobFingerprintInfo(BS2UserFaceExBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobFaceInfo(BS2UserSmallBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobFaceInfo(BS2UserFaceExBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getUserBlobFaceInfoEx(BS2UserFaceExBlob& userBlob, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);

	int getUserName(BS2_USER_NAME userName, const BS2SimpleDeviceInfo& deviceInfo);
	int makePinCode(BS2_USER_PIN chiperText, const BS2SimpleDeviceInfo& deviceInfo);
	int getProfileImage(BS2UserPhoto** photo, const BS2SimpleDeviceInfo& deviceInfo);
	int getAccessGroupID(BS2_ACCESS_GROUP_ID* accessGroupId);
	int getCardInfo(BS2CSNCard** cardObjs, uint8_t& numOfCards, BS2_DEVICE_ID id, const BS2SimpleDeviceInfo& deviceInfo, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int getFingerprintInfo(BS2Fingerprint** fingerObjs, uint8_t& numOfFingers, BS2_DEVICE_ID id, const BS2SimpleDeviceInfoEx& deviceInfoEx);
	int scanFace(BS2_DEVICE_ID id, BS2Face* ptrFace, uint8_t& numOfFace);
	int scanFaceEx(BS2_DEVICE_ID id, BS2FaceEx* ptrFace, uint8_t& numOfFace);
	int scanAndLoadFaceEx(BS2UserFaceExBlob& userBlob, BS2_DEVICE_ID id, uint32_t numOfScan, uint32_t numOfImage);
	int extractTemplateFaceEx(BS2_DEVICE_ID id, BS2TemplateEx& templateEx);
	int getNormalizedImageFaceEx(BS2_DEVICE_ID id);

#if TEST_CODE	
	int enrollUserFaceEx_1User(BS2_DEVICE_ID id, uint32_t idx);
	int enrollUserFaceEx_WithImage_1User(BS2_DEVICE_ID id);
	int enrollUserFaceEx_WithImage_Multi(BS2_DEVICE_ID id);
#endif

private:
	void dumpHexa(const uint8_t* data, uint32_t size);
	void print(const BS2UserBlob& userBlob);
	void print(const BS2UserFaceExBlob& userBlob);
	void printHeader(const BS2UserBlob& userBlob);
	void printHeaderFaceEx(const BS2UserFaceExBlob& userBlob);
	void print(const BS2User& user);
	void print(const BS2UserSetting& settings);
	void print(const BS2UserSettingEx& settings);
	void print(const BS2UserPhoto& photo);
	void print(const BS2CSNCard* card, uint8_t numCard);
	void print(const BS2Fingerprint* finger, uint8_t numFinger);
	void print(const BS2Face* face, uint8_t numFace);
	void print(const BS2FaceEx* face, uint8_t numFace);
	void print(const BS2TemplateEx& templateEx);

	void printCardSmartHeader(const BS2SmartCardHeader& header);
	void printCardSmartCredential(const BS2SmartCardCredentials& cred);
	void printCardAOC(const BS2AccessOnCardData& aoc);
	void printCardSmart(const BS2SmartCardData& card);
	void printCardCSN(const BS2CSNCard& card);


private:
	static void* context_;
};
