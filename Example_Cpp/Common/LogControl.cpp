#include "stdafx.h"
#include <iostream>
#include <sstream>
#include <cassert>
#include <memory>
#include "LogControl.h"
#include "BS_Errno.h"
#include "../Common/Utility.h"

#define CAST_UINT32(x)				static_cast<uint32_t>(x)


extern void TRACE(const char* fmt, ...);
using namespace std;

void* LogControl::context_ = NULL;

LogControl::LogControl(void* sdkContext)
{
	context_ = sdkContext;
}

LogControl::~LogControl()
{
}

string LogControl::getEventString(BS2_DEVICE_ID id, const BS2Event& event, int32_t timezone, BS2_TEMPERATURE temperature)
{
	char buffer[1024] = { 0, };
	float temper = 0.0;
	if (0 < temperature)
	{
		temper = (float)temperature / (float)100.0;
	}

	switch (event.code & BS2_EVENT_MASK)
	{
	case BS2_EVENT_USER_ENROLL_SUCCESS:
	case BS2_EVENT_USER_ENROLL_FAIL:
	case BS2_EVENT_USER_UPDATE_SUCCESS:
	case BS2_EVENT_USER_UPDATE_FAIL:
	case BS2_EVENT_USER_DELETE_SUCCESS:
	case BS2_EVENT_USER_DELETE_FAIL:
	case BS2_EVENT_USER_DELETE_ALL_SUCCESS:
#ifndef _NEW_CODE
		if (0 < temperature)
		{
			sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%s) deviceID(%d) userID(%s) where(%s) temperature(%.2f)",
				id, event.mainCode, event.subCode, Utility::convertTimeUTC2String(event.dateTime + timezone).c_str(), event.deviceID, event.userID, event.param ? "Device" : "Server", temper);
		}
		else
		{
			sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%s) deviceID(%d) userID(%s) where(%s)",
				id, event.mainCode, event.subCode, Utility::convertTimeUTC2String(event.dateTime + timezone).c_str(), event.deviceID, event.userID, event.param ? "Device" : "Server");
		}
#else
		if (0 < temperature)
		{
			sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d) userID(%s) where(%s) temperature(%.2f)",
				id, event.mainCode, event.subCode, Utility::convertTimeUTC2String(event.dateTime + timezone).c_str(), event.deviceID, event.userID, (event.subCode == BS2_SUB_EVENT_USER_BY_DEVICE) ? "Device" : "Server", temper);
		}
		else
		{
			sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d) userID(%s) where(%s)",
				id, event.mainCode, event.subCode, Utility::convertTimeUTC2String(event.dateTime + timezone).c_str(), event.deviceID, event.userID, (event.subCode == BS2_SUB_EVENT_USER_BY_DEVICE) ? "Device" : "Server");
		}
#endif
		break;

	case BS2_EVENT_RELAY_ACTION_ON:
	case BS2_EVENT_RELAY_ACTION_OFF:
	case BS2_EVENT_RELAY_ACTION_KEEP:
		sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%s) deviceID(%d) relayPort(%u) inputPort(%u)",
			id, event.mainCode, event.subCode, Utility::convertTimeUTC2String(event.dateTime + timezone).c_str(), event.deviceID, event.relayAction.relayPort, event.relayAction.inputPort);
		break;

	default:
		if (0 < temperature)
		{
			sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%s) deviceID(%d) temperature(%.2f)",
				id, event.mainCode, event.subCode, Utility::convertTimeUTC2String(event.dateTime + timezone).c_str(), event.deviceID, temper);
		}
		else
		{
			sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%s) deviceID(%d)",
				id, event.mainCode, event.subCode, Utility::convertTimeUTC2String(event.dateTime + timezone).c_str(), event.deviceID);
		}
		break;
	}

	return buffer;
}

int LogControl::getLogSmallBlob(BS2_DEVICE_ID id)
{
	uint16_t mask = BS2_EVENT_MASK_USER_ID;
	const uint32_t FROM_FIRST = 0;
	BS2_EVENT_ID eID = FROM_FIRST;
	uint32_t amount(0), numLog(0);
	BS2EventSmallBlob* blobObj = NULL;

	int sdkResult = BS2_GetLogSmallBlob(context_, id, mask, eID, amount, &blobObj, &numLog);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetLogSmallBlob call failed: %d", sdkResult);

	print(blobObj, numLog);

	if (blobObj)
	{
		if (blobObj->imageObj)
			BS2_ReleaseObject(blobObj->imageObj);

		BS2_ReleaseObject(blobObj);
	}
	return sdkResult;
}

int LogControl::getLogSmallBlobEx(BS2_DEVICE_ID id)
{
	//uint16_t mask = BS2_EVENT_MASK_USER_ID | BS2_EVENT_MASK_TEMPERATURE;
	uint16_t mask = BS2_EVENT_MASK_ALL;	 
	const uint32_t FROM_FIRST = 0;
	BS2_EVENT_ID eID = FROM_FIRST;
	uint32_t amount(0), numLog(0);
	BS2EventSmallBlobEx* blobObj = NULL;

	int sdkResult = BS2_GetLogSmallBlobEx(context_, id, mask, eID, amount, &blobObj, &numLog);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetLogSmallBlobEx call failed: %d", sdkResult);

	print(blobObj, numLog);

	if (blobObj)
	{
		if (blobObj->imageObj)
			BS2_ReleaseObject(blobObj->imageObj);

		BS2_ReleaseObject(blobObj);
	}
	return sdkResult;
}

void LogControl::print(const BS2Event& log)
{
	TRACE("id:%u, dateTime:%s, deviceID:%u, userID:%s, code:%u, param:%u, image:%u",
		log.id, Utility::convertTimeUTC2String(log.dateTime).c_str(), log.deviceID, log.userID, log.code, log.param, log.image);
}

void LogControl::print(const BS2Event* logs, uint32_t numLog)
{
	for (uint32_t index = 0; index < numLog; index++)
		print(logs[index]);
}

void LogControl::print(const BS2EventBlob& log)
{
	TRACE("eventMask:0x%04x, id:%u, dateTime:%s, deviceID:%u, mainCode:%x, subCode:%x, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u",
		log.eventMask, log.id,
		Utility::convertTimeUTC2String(log.info.dateTime).c_str(),
		log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize);
}

void LogControl::print(const BS2EventBlob* logs, uint32_t numLog)
{
	for (uint32_t index = 0; index < numLog; index++)
		print(logs[index]);
}

#ifdef OLD_CODE
void LogControl::print(const BS2EventSmallBlob& log)
{
	TRACE("eventMask : %u", log.eventMask);
	TRACE("id : %u", log.id);
	print(log.info);
	TRACE("userID : %s", log.userID);
	TRACE("tnaKey : %u", log.tnaKey);
	TRACE("jobCode : %u", log.jobCode);
	if (0 < log.imageSize)
	{
		TRACE("imageSize : %u", log.imageSize);
		TRACE("imageObj : %p", log.imageObj);
	}
}
#else
void LogControl::print(const BS2EventSmallBlob& log)
{
	if (0 < log.imageSize)
	{
		TRACE("eventMask:0x%04x, id:%u, dateTime:%s, deviceID:%u, mainCode:%x, subCode:%x, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u, imageObj:%p",
			log.eventMask, log.id,
			Utility::convertTimeUTC2String(log.info.dateTime).c_str(),
			log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize, log.imageObj);
	}
	else
	{
		TRACE("eventMask:0x%04x, id:%u, dateTime:%s, deviceID:%u, mainCode:%x, subCode:%x, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u",
			log.eventMask, log.id,
			Utility::convertTimeUTC2String(log.info.dateTime).c_str(),
			log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize);
	}
}
#endif

void LogControl::print(const BS2EventSmallBlob* logs, uint32_t numLog)
{
	for (uint32_t index = 0; index < numLog; index++)
		print(logs[index]);
}

#ifdef OLD_CODE
void LogControl::print(const BS2EventSmallBlobEx& log)
{
	TRACE("eventMask : %u", log.eventMask);
	TRACE("id : %u", log.id);
	print(log.info);
	TRACE("userID : %s", log.userID);
	TRACE("tnaKey : %u", log.tnaKey);
	TRACE("jobCode : %u", log.jobCode);
	if (0 < log.imageSize)
	{
		TRACE("imageSize : %u", log.imageSize);
		TRACE("imageObj : %p", log.imageObj);
	}
	float temper = (float)log.temperature / (float)100.0;
	TRACE("temperature : %.2f", temper);
}
#else
void LogControl::print(const BS2EventSmallBlobEx& log)
{
	float temper = (float)log.temperature / (float)100.0;
	if (0 < log.imageSize)
	{
		TRACE("eventMask:0x%04x, id:%u, dateTime:%s, deviceID:%u, mainCode:%x, subCode:%x, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u, imageObj:%p, temperature : %.2f",
			log.eventMask, log.id, 
			Utility::convertTimeUTC2String(log.info.dateTime).c_str(),
			log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize, log.imageObj, temper);
	}
	else
	{
		TRACE("eventMask:0x%04x, id:%u, dateTime:%s, deviceID:%u, mainCode:%x, subCode:%x, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u, temperature : %.2f",
			log.eventMask, log.id,
			Utility::convertTimeUTC2String(log.info.dateTime).c_str(),
			log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize, temper);
	}
}
#endif

void LogControl::print(const BS2EventSmallBlobEx* logs, uint32_t numLog)
{
	for (uint32_t index = 0; index < numLog; index++)
		print(logs[index]);
}

void LogControl::print(const BS2EventExtInfo& info)
{
	TRACE("==[BS2EventExtInfo]==");
	TRACE("  dateTime : %u", info.dateTime);
	TRACE("  deviceID : %u", info.deviceID);
	TRACE("  mainCode : %x", info.mainCode);
	TRACE("  subCode : %x", info.subCode);
}
