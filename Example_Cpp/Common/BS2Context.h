#pragma once

#include "BS_API.h"


class BS2Context
{
public:
	static BS2Context* getInstance();
	void releaseInstance();

	void* initSDK(BS2_PORT port = BS2_TCP_SERVER_PORT_DEFAULT);
	void* getContext() {return context_;}
	void setDebugFileLog(uint32_t level, uint32_t module, const char* path);
	int setDeviceEventListener(OnDeviceAccepted fpAccepted, OnDeviceConnected fpConnected, OnDeviceDisconnected fpDisconnected);
	int setServerPort(BS2_PORT port);


private:
	BS2Context();
	virtual ~BS2Context();


private:
	static BS2Context* sdk_;
	void* context_;
};
