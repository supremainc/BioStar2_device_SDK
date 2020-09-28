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
#include "BS2Context.h"
#include "BS_API.h"
#include "BS_Errno.h"


BS2Context* BS2Context::sdk_ = NULL;

using namespace std;

void TRACE(const char* fmt, ...)
{
	char buffer[0x160] = { 0, };
	va_list fmtList;
	va_start(fmtList, fmt);
	vsprintf(buffer, fmt, fmtList);
	va_end(fmtList);
	buffer[strlen(buffer)] = '\n';
#if defined(OS_WIN32) || defined(OS_WIN64)
	::OutputDebugStringA(buffer);
#else
	cout << buffer;
#endif
}


BS2Context::BS2Context() : context_(NULL)
{
}


BS2Context::~BS2Context()
{
	if (context_)
		BS2_ReleaseContext(context_);
}


BS2Context* BS2Context::getInstance()
{
	if (!sdk_)
		sdk_ = new BS2Context;

	return sdk_;
}


void BS2Context::releaseInstance()
{
	if (sdk_)
	{
		delete sdk_;
		sdk_ = NULL;
	}
}


void* BS2Context::initSDK(BS2_PORT port)
{
	TRACE("Version:%s", BS2_Version());

	context_ = BS2_AllocateContext();
	if (!context_)
	{
		TRACE("BS2_AllocateContext call failed");
		return NULL;
	}

	int sdkResult = setServerPort(port);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		BS2_ReleaseContext(context_);
		context_ = NULL;
		return NULL;
	}

	sdkResult = BS2_Initialize(context_);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_Initialize call failed: %d", sdkResult);
		BS2_ReleaseContext(context_);
		context_ = NULL;
		return NULL;
	}

	return context_;
}


void BS2Context::setDebugFileLog(uint32_t level, uint32_t module, const char* path)
{
	BS2_SetDebugFileLog(level, module, path);
	//BS2_SetDebugFileLog(DEBUG_LOG_OPERATION_ALL, DEBUG_MODULE_ALL, ".");
}


int BS2Context::setDeviceEventListener(OnDeviceAccepted fpAccepted, OnDeviceConnected fpConnected, OnDeviceDisconnected fpDisconnected)
{
	int sdkResult = BS2_SetDeviceEventListener(context_, NULL, fpAccepted, fpConnected, fpDisconnected);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetDeviceEventListener call failed: %d", sdkResult);
	}

	return sdkResult;
}

int BS2Context::setServerPort(BS2_PORT port)
{
	int sdkResult = BS2_SetServerPort(context_, port);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetServerPort call failed: %d", sdkResult);
	}

	return sdkResult;
}