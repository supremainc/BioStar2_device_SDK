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

void LogControl::print(const BS2EventSmallBlob* logs, uint32_t numLog)
{
	for (uint32_t index = 0; index < numLog; index++)
	{
		TRACE("==[BS2EventSmallBlob(%u)]==", index);
		TRACE("eventMask : %u", logs[index].eventMask);
		TRACE("id : %u", logs[index].id);
		print(logs[index].info);
		TRACE("userID : %s", logs[index].userID);
		TRACE("tnaKey : %u", logs[index].tnaKey);
		TRACE("jobCode : %u", logs[index].jobCode);
		TRACE("imageSize : %u", logs[index].imageSize);
		TRACE("imageObj : %p", logs[index].imageObj);
		TRACE("============================>>");
	}
}

void LogControl::print(const BS2EventExtInfo& info)
{
	TRACE("==[BS2EventExtInfo]==");
	TRACE("dateTime : %u", info.dateTime);
	TRACE("deviceID : %u", info.deviceID);
	TRACE("mainCode : %u", info.mainCode);
	TRACE("subCode : %u", info.subCode);
	TRACE("============================>>");
}
