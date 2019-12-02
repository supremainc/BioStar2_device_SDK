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
	int enrollUser(BS2_DEVICE_ID id);
	int removeUser(BS2_DEVICE_ID id);
	int removeAllUser(BS2_DEVICE_ID id);


private:
	void print(const BS2UserBlob& userBlob);
	void printHeader(const BS2UserBlob& userBlob);
	void print(const BS2User& user);
	void print(const BS2UserSetting& settings);
	void print(const BS2UserPhoto& photo);
	void print(const BS2CSNCard* card, uint8_t numCard);
	void print(const BS2Fingerprint* finger, uint8_t numFinger);
	void print(const BS2Face* face, uint8_t numFace);


private:
	static void* context_;
};
