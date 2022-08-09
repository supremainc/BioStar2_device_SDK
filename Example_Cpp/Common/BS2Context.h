#pragma once

#include "BS_API.h"
#include <fstream>


class BS2InstantContext
{
public:
	BS2InstantContext();
	~BS2InstantContext();
	void* getContext();
	int init();
	int setDeviceEventListener(OnDeviceFound cbFound, OnDeviceAccepted cbAccepted, OnDeviceConnected cbConnected, OnDeviceDisconnected);


protected:
	void* context_;
};


class BS2Context
{
public:
	static BS2Context* getInstance();
	void releaseInstance();

	static void setDebugCallbackLog(uint32_t level, uint32_t module, bool writeToFile = false);
	static void setDebugFileLog(uint32_t level, uint32_t module, const char* path, int maxFileSize = 0);

	static void onDebugMessage(uint32_t level, uint32_t module, const char* msg);
	int initSDK(BS2_PORT port = BS2_TCP_SERVER_PORT_DEFAULT);
	void* getContext() {return context_;}
	int setDeviceEventListener(OnDeviceAccepted fpAccepted, OnDeviceConnected fpConnected, OnDeviceDisconnected fpDisconnected);
	int setNotificationListener(OnAlarmFired fpAlarmFired, OnInputDetected fpInputDetected, OnConfigChanged fpConfigChanged);
	int setBarcodeScanListener(OnBarcodeScanned fpBarcodeScan);
	int setServerPort(BS2_PORT port);
	int setSSLHandler();

	static uint32_t OnPreferMethod(BS2_DEVICE_ID deviceID);
	static const char* OnGetRootCaFilePath(BS2_DEVICE_ID deviceID);
	static const char* OnGetServerCaFilePath(BS2_DEVICE_ID deviceID);
	static const char* OnGetServerPrivateKeyFilePath(BS2_DEVICE_ID deviceID);
	static const char* OnGetPassword(BS2_DEVICE_ID deviceID);
	static void onErrorOccured(BS2_DEVICE_ID deviceID, int errCode);
	static void OnSendRootCA(BS2_DEVICE_ID deviceID, int result);


private:
	BS2Context();
	virtual ~BS2Context();


private:
	static BS2Context* sdk_;
	void* context_;
	static std::ofstream file_;
	static bool writeToFile_;
};
