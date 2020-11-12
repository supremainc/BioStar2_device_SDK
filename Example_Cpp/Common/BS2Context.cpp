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


void BS2Context::onDebugMessage(uint32_t level, uint32_t module, const char* msg)
{
	string strLevel, strModule;
	switch (level)
	{
	case DEBUG_LOG_FATAL:  strLevel = "[FATAL] ";  break;
	case DEBUG_LOG_ERROR:  strLevel = "[ERROR] ";  break;
	case DEBUG_LOG_WARN:   strLevel = "[WARN]  ";  break;
	case DEBUG_LOG_INFO:   strLevel = "[INFO]  ";  break;
	case DEBUG_LOG_TRACE:  strLevel = "[TRACE] ";  break;
	}

	switch (module)
	{
	case DEBUG_MODULE_KEEP_ALIVE:       strModule = "[KeepAlive]       "; break;
	case DEBUG_MODULE_SOCKET_MANAGER:   strModule = "[SocketManager]   "; break;
	case DEBUG_MODULE_SOCKET_HANDLER:   strModule = "[SocketHandler]   "; break;
	case DEBUG_MODULE_DEVICE:           strModule = "[Device]          "; break;
	case DEBUG_MODULE_DEVICE_MANAGER:   strModule = "[DeviceManager]   "; break;
	case DEBUG_MODULE_EVENT_DISPATCHER: strModule = "[EventDispatcher] "; break;
	case DEBUG_MODULE_API:              strModule = "[API]             "; break;
	case DEBUG_MODULE_MISC:             strModule = "[Misc]            "; break;
	case DEBUG_MODULE_PACKET:           strModule = "[Packet]          "; break;
	}

	cout << strLevel << strModule << msg << endl;
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
#if OLD_CODE
	BS2_SetDebugExCallback(&BS2Context::onDebugMessage, level, module);
#else
	BS2_SetDebugFileLog(level, module, path);
#endif
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