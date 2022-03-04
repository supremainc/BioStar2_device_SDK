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

const string ssl_server_root_crt = "../resource/server/ssl_server_root.crt";
const string ssl_server_crt = "../resource/server/ssl_server.crt";
const string ssl_server_pem = "../resource/server/ssl_server.pem";
const string ssl_server_passwd = "supremaserver";


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


BS2InstantContext::BS2InstantContext()
{
	context_ = BS2_AllocateContext();
}

BS2InstantContext::~BS2InstantContext()
{
	BS2_ReleaseContext(context_);
}

void* BS2InstantContext::getContext()
{
	return context_;
}

int BS2InstantContext::init()
{
	return BS2_Initialize(context_);
}

int BS2InstantContext::setDeviceEventListener(OnDeviceFound cbFound, OnDeviceAccepted cbAccepted, OnDeviceConnected cbConnected, OnDeviceDisconnected cbDisconnected)
{
	return BS2_SetDeviceEventListener(context_, cbFound, cbAccepted, cbConnected, cbDisconnected);
}


uint32_t BS2Context::OnPreferMethod(BS2_DEVICE_ID deviceID)
{
	return BS2_SSL_METHOD_MASK_TLS1 | BS2_SSL_METHOD_MASK_TLS1_1 | BS2_SSL_METHOD_MASK_TLS1_2;
}

const char* BS2Context::OnGetRootCaFilePath(BS2_DEVICE_ID deviceID)
{
	return ssl_server_root_crt.c_str();
}

const char* BS2Context::OnGetServerCaFilePath(BS2_DEVICE_ID deviceID)
{
	return ssl_server_crt.c_str();
}

const char* BS2Context::OnGetServerPrivateKeyFilePath(BS2_DEVICE_ID deviceID)
{
	return ssl_server_pem.c_str();
}

const char* BS2Context::OnGetPassword(BS2_DEVICE_ID deviceID)
{
	return ssl_server_passwd.c_str();
}

void BS2Context::onErrorOccured(BS2_DEVICE_ID deviceID, int errCode)
{
	TRACE("Error occured on SSL - Device:%u, Error:%d", deviceID, errCode);
}

void BS2Context::OnSendRootCA(BS2_DEVICE_ID deviceID, int result)
{
	TRACE("RootCA send result - Device:%u, Result:%d", deviceID, result);
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
	case DEBUG_MODULE_MOBILEACCESS:		strModule = "[MobileAccess]    "; break;
	case DEBUG_MODULE_NOTIFY_MANAGER:   strModule = "[NotifyManager]   "; break;
	case DEBUG_MODULE_EVENT:			strModule = "[Event]           "; break;
	case DEBUG_MODULE_USB:				strModule = "[USB]             "; break;
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
	{
		sdk_ = new BS2Context;

		sdk_->context_ = BS2_AllocateContext();
		if (!sdk_->context_)
		{
			TRACE("BS2_AllocateContext call failed");
			return NULL;
		}
	}

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


int BS2Context::initSDK(BS2_PORT port)
{
	int sdkResult = setServerPort(port);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		BS2_ReleaseContext(context_);
		context_ = NULL;
	}
	else
	{
		sdkResult = BS2_Initialize(context_);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_Initialize call failed: %d", sdkResult);
			BS2_ReleaseContext(context_);
			context_ = NULL;
		}
	}

	return sdkResult;
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

int BS2Context::setSSLHandler()
{
	int sdkResult = BS2_SetSSLHandler(context_, BS2Context::OnPreferMethod, BS2Context::OnGetRootCaFilePath, BS2Context::OnGetServerCaFilePath, BS2Context::OnGetServerPrivateKeyFilePath, BS2Context::OnGetPassword, BS2Context::onErrorOccured);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetSSLHandler call failed: %d", sdkResult);
		return sdkResult;
	}

	sdkResult = BS2_SetDeviceSSLEventListener(context_, BS2Context::OnSendRootCA);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetDeviceSSLEventListener call failed: %d", sdkResult);
	}

	return sdkResult;
}

int BS2Context::setNotificationListener(OnAlarmFired fpAlarmFired, OnInputDetected fpInputDetected, OnConfigChanged fpConfigChanged)
{
	int sdkResult = BS2_SetNotificationListener(context_, fpAlarmFired, fpInputDetected, fpConfigChanged);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetNotificationListener call failed: %d", sdkResult);
	}

	return sdkResult;
}

int BS2Context::setBarcodeScanListener(OnBarcodeScanned fpBarcodeScan)
{
	int sdkResult = BS2_SetBarcodeScanListener(context_, fpBarcodeScan);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetBarcodeScanListener call failed: %d", sdkResult);
	}

	return sdkResult;
}