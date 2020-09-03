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
	int getLastFingerprintImage(BS2_DEVICE_ID id, uint8_t** imageObj, uint32_t* width, uint32_t* height);
	int removeUser(BS2_DEVICE_ID id);
	int removeAllUser(BS2_DEVICE_ID id);
	int getUserFaceEx(BS2_DEVICE_ID id);
	int enrollUserFaceEx(BS2_DEVICE_ID id);

	int updateCardTypeCRC(BS2SmartCardData& card);
	int updateCardCRC(BS2SmartCardData& card);
	int getPinCode(std::string plainText, uint8_t* cipherText);
	int scanTemplate(BS2_DEVICE_ID id, uint8_t* fpTemplate);
	int scanCard(BS2_DEVICE_ID id, uint8_t* card);
	int scanCard(BS2_DEVICE_ID id, BS2Card* card);
	int writeCard(BS2_DEVICE_ID id, const BS2SmartCardData* card);
	int eraseCard(BS2_DEVICE_ID id);

	void print(const BS2Card& card);

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

	void printCardSmartHeader(const BS2SmartCardHeader& header);
	void printCardSmartCredential(const BS2SmartCardCredentials& cred);
	void printCardAOC(const BS2AccessOnCardData& aoc);
	void printCardSmart(const BS2SmartCardData& card);
	void printCardCSN(const BS2CSNCard& card);


private:
	static void* context_;
};
