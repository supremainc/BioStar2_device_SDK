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

string LogControl::getEventString(BS2_DEVICE_ID id, const BS2Event& event, int32_t timezone)
{
	char buffer[1024] = { 0, };
	switch (event.code & BS2_EVENT_MASK)
	{
	case BS2_EVENT_USER_ENROLL_SUCCESS:
	case BS2_EVENT_USER_ENROLL_FAIL:
	case BS2_EVENT_USER_UPDATE_SUCCESS:
	case BS2_EVENT_USER_UPDATE_FAIL:
	case BS2_EVENT_USER_DELETE_SUCCESS:
	case BS2_EVENT_USER_DELETE_FAIL:
	case BS2_EVENT_VERIFY_SUCCESS:
	case BS2_EVENT_VERIFY_DURESS:
	case BS2_EVENT_IDENTIFY_SUCCESS:
	case BS2_EVENT_IDENTIFY_DURESS:
	case BS2_EVENT_DUAL_AUTH_SUCCESS:
	case BS2_EVENT_ACCESS_DENIED:
	case BS2_EVENT_FAKE_FINGER_DETECTED:
	case BS2_EVENT_ACCESS_GRANTED:
#if VER_272_OR_HIGHER
	case BS2_EVENT_ABNORMAL_TEMPERATURE_DETECTED:
#endif
	case BS2_EVENT_UNMASKED_FACE_DETECTED:
		sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d) userID(%s) where(%s)",
			id, event.mainCode, event.subCode, event.dateTime + timezone, event.deviceID, event.userID, event.param ? "Device" : "Server");
		break;

	case BS2_EVENT_RELAY_ACTION_ON:
	case BS2_EVENT_RELAY_ACTION_OFF:
	case BS2_EVENT_RELAY_ACTION_KEEP:
		sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d) relayPort(%u) inputPort(%u)",
			id, event.mainCode, event.subCode, event.dateTime + timezone, event.deviceID, event.relayAction.relayPort, event.relayAction.inputPort);
		break;

	default:
		sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d)",
			id, event.mainCode, event.subCode, event.dateTime + timezone, event.deviceID);
		break;
	}

	return buffer;
}

string LogControl::getEventStringWithThermal(BS2_DEVICE_ID id, const BS2Event& event, int32_t timezone, BS2_TEMPERATURE temperature)
{
	char buffer[1024] = { 0, };
	float temper = (float)temperature / (float)100.0;
	switch (event.code & BS2_EVENT_MASK)
	{
	case BS2_EVENT_USER_ENROLL_SUCCESS:
	case BS2_EVENT_USER_ENROLL_FAIL:
	case BS2_EVENT_USER_UPDATE_SUCCESS:
	case BS2_EVENT_USER_UPDATE_FAIL:
	case BS2_EVENT_USER_DELETE_SUCCESS:
	case BS2_EVENT_USER_DELETE_FAIL:
	case BS2_EVENT_VERIFY_SUCCESS:
	case BS2_EVENT_VERIFY_DURESS:
	case BS2_EVENT_IDENTIFY_SUCCESS:
	case BS2_EVENT_IDENTIFY_DURESS:
	case BS2_EVENT_DUAL_AUTH_SUCCESS:
	case BS2_EVENT_ACCESS_DENIED:
	case BS2_EVENT_FAKE_FINGER_DETECTED:
	case BS2_EVENT_ACCESS_GRANTED:
#if VER_272_OR_HIGHER
	case BS2_EVENT_ABNORMAL_TEMPERATURE_DETECTED:
#endif
	case BS2_EVENT_UNMASKED_FACE_DETECTED:
		sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d) userID(%s) where(%s) temperature(%.2f)",
			id, event.mainCode, event.subCode, event.dateTime + timezone, event.deviceID, event.userID, event.param ? "Device" : "Server", temper);
		break;

	default:
		sprintf(buffer, "Device(%u), mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d) temperature(%.2f)",
			id, event.mainCode, event.subCode, event.dateTime + timezone, event.deviceID, temper);
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
	uint16_t mask = BS2_EVENT_MASK_USER_ID | BS2_EVENT_MASK_TEMPERATURE;
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
	TRACE("id:%u, dateTime:%u, deviceID:%u, userID:%s, code:%u, param:%u, image:%u",
		log.id, log.dateTime, log.deviceID, log.userID, log.code, log.param, log.image);
}

void LogControl::print(const BS2Event* logs, uint32_t numLog)
{
	for (uint32_t index = 0; index < numLog; index++)
		print(logs[index]);
}

void LogControl::print(const BS2EventBlob& log)
{
	TRACE("eventMask:%u, id:%u, dateTime:%u, deviceID:%u, mainCode:%u, subCode:%u, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u",
		log.eventMask, log.id, log.info.dateTime, log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize);
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
		TRACE("eventMask:%u, id:%u, dateTime:%u, deviceID:%u, mainCode:%u, subCode:%u, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u, imageObj:%p",
			log.eventMask, log.id, log.info.dateTime, log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize, log.imageObj);
	}
	else
	{
		TRACE("eventMask:%u, id:%u, dateTime:%u, deviceID:%u, mainCode:%u, subCode:%u, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u",
			log.eventMask, log.id, log.info.dateTime, log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize);
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
		TRACE("eventMask:%u, id:%u, dateTime:%u, deviceID:%u, mainCode:%u, subCode:%u, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u, imageObj:%p, temperature : %.2f",
			log.eventMask, log.id, log.info.dateTime, log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize, log.imageObj, temper);
	}
	else
	{
		TRACE("eventMask:%u, id:%u, dateTime:%u, deviceID:%u, mainCode:%u, subCode:%u, userID:%s, tnaKey:%u, jobCode:%u, imageSize:%u, temperature : %.2f",
			log.eventMask, log.id, log.info.dateTime, log.info.deviceID, log.info.mainCode, log.info.subCode, log.userID, log.tnaKey, log.jobCode, log.imageSize, temper);
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
	TRACE("  mainCode : %u", info.mainCode);
	TRACE("  subCode : %u", info.subCode);
}
