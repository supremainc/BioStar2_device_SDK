#include "stdafx.h"
#if defined(_WIN32) || defined(_WIN64)
#include <Windows.h>
#else
#include <stdarg.h>
#endif

#include <string>
#include <iostream>
#include <sstream>
#include <fstream>
#include "BS2UsbContext.h"
#include "BS_API.h"
#include "BS_Errno.h"
#include "Utility.h"


using namespace std;

extern void TRACE(const char*, ...);


BS2UsbContext::BS2UsbContext(string dir)
{
	if (0 == dir.size())
		return;

	int sdkResult = init();
	inited_ = sdkResult == BS_SDK_SUCCESS;

	if (!inited_)
	{
		TRACE("USB context init failed: %d", sdkResult);
		return;
	}

	dir_ = dir;
}


BS2UsbContext::~BS2UsbContext()
{
}

string BS2UsbContext::getUSBPath()
{
	stringstream msg;
	msg << "Enter the path of exported USB directory which you want to get." << endl;
	string usbPath = Utility::getInput<string>(msg.str());
	if (0 == usbPath.size())
		TRACE("Invalid path");

	return usbPath;
}

int BS2UsbContext::getUserDatabaseInfoFromDir(uint32_t& numUsers, uint32_t& numCards, uint32_t& numFingers, uint32_t& numFaces, IsAcceptableUserID cbAcceptableUID)
{
	int sdkResult = BS2_GetUserDatabaseInfoFromDir(context_, dir_.c_str(), &numUsers, &numCards, &numFingers, &numFaces, cbAcceptableUID);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserDatabaseInfoFromDir call failed: %d", sdkResult);
	}

	return sdkResult;
}

int BS2UsbContext::getUserListFromDir(IsAcceptableUserID cbAcceptableUID, vector<string>& userIDs)
{
	char* uidsObj = NULL;
	uint32_t numOfUID(0);
	int sdkResult = BS2_GetUserListFromDir(context_, dir_.c_str(), &uidsObj, &numOfUID, cbAcceptableUID);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserListFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	if (0 == numOfUID || !uidsObj)
	{
		TRACE("There are no users.");
		return sdkResult;
	}

	char* pos = uidsObj;
	for (uint32_t idx = 0; idx < numOfUID; idx++, pos += BS2_USER_ID_SIZE)
	{
		string uid(pos, BS2_USER_ID_SIZE);
		userIDs.push_back(uid);
	}

	if (uidsObj)
		BS2_ReleaseObject(uidsObj);

	return sdkResult;
}

int BS2UsbContext::getUserInfosFromDir(const vector<string>& userIDs, vector<BS2UserBlob>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserBlob* blobs = new BS2UserBlob[numOfUID];
	int sdkResult = BS2_GetUserInfosFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserInfosFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserDatasFromDir(const vector<string>& userIDs, BS2_USER_MASK userMask, vector<BS2UserBlob>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserBlob* blobs = new BS2UserBlob[numOfUID];
	int sdkResult = BS2_GetUserDatasFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs, userMask);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserDatasFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserInfosExFromDir(const vector<string>& userIDs, vector<BS2UserBlobEx>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserBlobEx* blobs = new BS2UserBlobEx[numOfUID];
	int sdkResult = BS2_GetUserInfosExFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserInfosExFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserDatasExFromDir(const vector<string>& userIDs, BS2_USER_MASK userMask, vector<BS2UserBlobEx>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserBlobEx* blobs = new BS2UserBlobEx[numOfUID];
	int sdkResult = BS2_GetUserDatasExFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs, userMask);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserDatasExFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserSmallInfosFromDir(const vector<string>& userIDs, vector<BS2UserSmallBlob>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserSmallBlob* blobs = new BS2UserSmallBlob[numOfUID];
	int sdkResult = BS2_GetUserSmallInfosFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserSmallInfosFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserSmallDatasFromDir(const vector<string>& userIDs, BS2_USER_MASK userMask, vector<BS2UserSmallBlob>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserSmallBlob* blobs = new BS2UserSmallBlob[numOfUID];
	int sdkResult = BS2_GetUserSmallDatasFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs, userMask);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserSmallDatasFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserSmallInfosExFromDir(const vector<string>& userIDs, vector<BS2UserSmallBlobEx>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserSmallBlobEx* blobs = new BS2UserSmallBlobEx[numOfUID];
	int sdkResult = BS2_GetUserSmallInfosExFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserSmallInfosExFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserSmallDatasExFromDir(const vector<string>& userIDs, BS2_USER_MASK userMask, vector<BS2UserSmallBlobEx>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserSmallBlobEx* blobs = new BS2UserSmallBlobEx[numOfUID];
	int sdkResult = BS2_GetUserSmallDatasExFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs, userMask);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserSmallDatasExFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserInfosFaceExFromDir(const vector<string>& userIDs, vector<BS2UserFaceExBlob>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserFaceExBlob* blobs = new BS2UserFaceExBlob[numOfUID];
	int sdkResult = BS2_GetUserInfosFaceExFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserInfosFaceExFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}

int BS2UsbContext::getUserDatasFaceExFromDir(const vector<string>& userIDs, BS2_USER_MASK userMask, vector<BS2UserFaceExBlob>& userBlob)
{
	uint32_t numOfUID = userIDs.size();
	vector<char> buffer;
	buffer.resize(BS2_USER_ID_SIZE * numOfUID);

	char* pos = &buffer[0];
	for (auto uid : userIDs)
	{
		memcpy(pos, uid.c_str(), uid.size());
		pos += BS2_USER_ID_SIZE;
	}

	BS2UserFaceExBlob* blobs = new BS2UserFaceExBlob[numOfUID];
	int sdkResult = BS2_GetUserDatasFaceExFromDir(context_, dir_.c_str(), reinterpret_cast<char*>(buffer.data()), numOfUID, blobs, userMask);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetUserDatasFaceExFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfUID; idx++)
	{
		string tempID(blobs[idx].user.userID, BS2_USER_ID_SIZE);
		if (0 < strlen(tempID.c_str()))
			userBlob.push_back(blobs[idx]);
	}

	if (blobs)
		delete[] blobs;

	return sdkResult;
}


int BS2UsbContext::getFilteredLogFromDir(char* uid, BS2_EVENT_CODE eventCode, BS2_TIMESTAMP startTime, BS2_TIMESTAMP endTime, uint8_t tnaKey, vector<BS2Event>& eventList)
{
	BS2Event* eventsObj = NULL;
	uint32_t numLog(0);
	int sdkResult = BS2_GetFilteredLogFromDir(context_, dir_.c_str(), uid, eventCode, startTime, endTime, tnaKey, &eventsObj, &numLog);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetFilteredLogFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numLog; idx++)
	{
		eventList.push_back(eventsObj[idx]);
	}

	BS2_ReleaseObject(eventsObj);

	return sdkResult;
}


int BS2UsbContext::getLogFromDir(BS2_EVENT_ID eventId, uint32_t amount, vector<BS2Event>& eventList)
{
	BS2Event* eventsObj = NULL;
	uint32_t numLog(0);
	int sdkResult = BS2_GetLogFromDir(context_, dir_.c_str(), eventId, amount, &eventsObj, &numLog);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetLogFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numLog; idx++)
	{
		eventList.push_back(eventsObj[idx]);
	}

	BS2_ReleaseObject(eventsObj);

	return sdkResult;
}


int BS2UsbContext::getLogBlobFromDir(uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, vector<BS2EventBlob>& eventList)
{
	BS2EventBlob* eventBlobObj = NULL;
	uint32_t numLog(0);
	int sdkResult = BS2_GetLogBlobFromDir(context_, dir_.c_str(), eventMask, eventId, amount, &eventBlobObj, &numLog);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetLogBlobFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numLog; idx++)
	{
		eventList.push_back(eventBlobObj[idx]);
	}

	BS2_ReleaseObject(eventBlobObj);

	return sdkResult;
}


int BS2UsbContext::getLogSmallBlobFromDir(uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, vector<BS2EventSmallBlob>& eventList)
{
	BS2EventSmallBlob* smallBlobObj = NULL;
	uint32_t numLog(0);
	int sdkResult = BS2_GetLogSmallBlobFromDir(context_, dir_.c_str(), eventMask, eventId, amount, &smallBlobObj, &numLog);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetLogSmallBlobFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numLog; idx++)
	{
		eventList.push_back(smallBlobObj[idx]);
	}

	BS2_ReleaseObject(smallBlobObj);

	return sdkResult;
}


int BS2UsbContext::getLogSmallBlobExFromDir(uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, vector<BS2EventSmallBlobEx>& eventList)
{
	BS2EventSmallBlobEx* smallBlobObj = NULL;
	uint32_t numLog(0);
	int sdkResult = BS2_GetLogSmallBlobExFromDir(context_, dir_.c_str(), eventMask, eventId, amount, &smallBlobObj, &numLog);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetLogSmallBlobExFromDir call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numLog; idx++)
	{
		eventList.push_back(smallBlobObj[idx]);
	}

	BS2_ReleaseObject(smallBlobObj);

	return sdkResult;
}
