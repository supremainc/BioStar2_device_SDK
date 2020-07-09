#include "stdafx.h"
#include <iostream>
#include "CommControl.h"
#include "BS_Errno.h"
#include "../Common/Utility.h"


extern void TRACE(const char* fmt, ...);
using namespace std;


CommControl::CommControl(void* sdkContext) : context_(sdkContext)
{
}

CommControl::~CommControl()
{
}

int CommControl::searchDevices(vector<BS2SimpleDeviceInfo>& devices)
{
	BS2_DEVICE_ID* devicesIDObj = NULL;
	uint32_t numOfDevices(0);

	int sdkResult = BS2_SearchDevices(context_);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SearchDevices call failed: %d", sdkResult);
		return sdkResult;
	}

	sdkResult = BS2_GetDevices(context_, &devicesIDObj, &numOfDevices);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetDevices call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < numOfDevices; index++)
	{
		BS2SimpleDeviceInfo deviceInfo = { 0, };
		sdkResult = BS2_GetDeviceInfo(context_, devicesIDObj[index], &deviceInfo);
		if (BS_SDK_SUCCESS == sdkResult)
		{
			devices.push_back(deviceInfo);
		}
		else
			TRACE("BS2_GetDeviceInfo call failed: %d (id:%u)", sdkResult, devicesIDObj[index]);
	}

	if (devicesIDObj)
		BS2_ReleaseObject(devicesIDObj);

	return sdkResult;
}

int CommControl::connectDevice(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_ConnectDevice(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_ConnectDevice call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::connectDevice(BS2_DEVICE_ID& id, string ip, BS2_PORT port)
{
	//int sdkResult = BS2_ConnectDeviceViaIP(context_, ip.c_str(), BS2_TCP_DEVICE_PORT_DEFAULT, &id);
	int sdkResult = BS2_ConnectDeviceViaIP(context_, ip.c_str(), port, &id);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_ConnectDeviceViaIP call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::disconnectDevice(BS2_DEVICE_ID id)
{
	return BS2_DisconnectDevice(context_, id);
}

int CommControl::searchSlaveDevice(BS2_DEVICE_ID id, vector<BS2Rs485SlaveDevice>& slaveList)
{
	BS2Rs485SlaveDevice* slaveDeviceObj = NULL;
	uint32_t slaveDeviceCount = 0;

	int sdkResult = BS2_GetSlaveDevice(context_, id, &slaveDeviceObj, &slaveDeviceCount);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetSlaveDevice call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < slaveDeviceCount; ++index)
	{
		slaveList.push_back(slaveDeviceObj[index]);
	}

	if (slaveDeviceObj)
		BS2_ReleaseObject(slaveDeviceObj);

	return sdkResult;
}

int CommControl::addSlaveDevice(BS2_DEVICE_ID id, BS2_DEVICE_ID slaveID)
{
	BS2Rs485SlaveDevice* slaveDeviceObj = NULL;
	uint32_t slaveDeviceCount = 0;
	bool found(false);

	int sdkResult = BS2_GetSlaveDevice(context_, id, &slaveDeviceObj, &slaveDeviceCount);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetSlaveDevice call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < slaveDeviceCount; index++)
	{
		if (slaveDeviceObj[index].deviceID == slaveID)
		{
			if (slaveDeviceObj[index].enableOSDP == false)
			{
				slaveDeviceObj[index].enableOSDP = true;

				sdkResult = BS2_SetSlaveDevice(context_, id, slaveDeviceObj, slaveDeviceCount);
			}

			found = true;
			break;
		}
	}

	if (!found)
	{
		TRACE("The slave device(%u) could not be found under (%u).", slaveID, id);
		sdkResult = BS_SDK_ERROR_CANNOT_FIND_DEVICE;
	}

	if (slaveDeviceObj)
		BS2_ReleaseObject(slaveDeviceObj);

	return sdkResult;
}

int CommControl::deleteSlaveDevice(BS2_DEVICE_ID id, BS2_DEVICE_ID slaveID)
{
	BS2Rs485SlaveDevice* slaveDeviceObj = NULL;
	uint32_t slaveDeviceCount = 0;
	bool found(false);

	int sdkResult = BS2_GetSlaveDevice(context_, id, &slaveDeviceObj, &slaveDeviceCount);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		for (uint32_t index = 0; index < slaveDeviceCount; index++)
		{
			if (slaveDeviceObj[index].deviceID == slaveID)
			{
				if (slaveDeviceObj[index].enableOSDP == true)
				{
					slaveDeviceObj[index].enableOSDP = false;

					sdkResult = BS2_SetSlaveDevice(context_, id, slaveDeviceObj, slaveDeviceCount);
				}

				found = true;
				break;
			}
		}

		if (!found)
		{
			TRACE("The slave device(%u) could not be found under (%u).", slaveID, id);
			sdkResult = BS_SDK_ERROR_CANNOT_FIND_DEVICE;
		}
	}
	else if (BS_SDK_ERROR_CANNOT_FIND_DEVICE == sdkResult)
	{
		sdkResult = BS2_RemoveSlaveDevice(context_, slaveID);
	}
	else
	{
		TRACE("BS2_GetSlaveDevice call failed: %d", sdkResult);
		sdkResult = BS_SDK_ERROR_CANNOT_FIND_DEVICE;
	}

	if (slaveDeviceObj)
		BS2_ReleaseObject(slaveDeviceObj);

	return sdkResult;
}

int CommControl::searchCSTSlaveDevice(BS2_DEVICE_ID id, uint32_t channelPort, vector<BS2Rs485SlaveDeviceEX>& slaveList)
{
	BS2Rs485SlaveDeviceEX* slaveDeviceObj = NULL;
	uint32_t slaveDeviceCount = 0;
	uint32_t outputPort = 0;

	int sdkResult = BS2_GetSlaveExDevice(context_, id, channelPort, &slaveDeviceObj, &outputPort, &slaveDeviceCount);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetSlaveExDevice call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < slaveDeviceCount; index++)
	{
		slaveList.push_back(slaveDeviceObj[index]);
	}

	if (slaveDeviceObj)
		BS2_ReleaseObject(slaveDeviceObj);

	return sdkResult;
}

int CommControl::addCSTSlaveDevice(BS2_DEVICE_ID id, uint32_t channelPort, BS2_DEVICE_ID slaveID)
{
	BS2Rs485SlaveDeviceEX* slaveDeviceObj = NULL;
	uint32_t slaveDeviceCount = 0;
	uint32_t outputPort = 0;
	bool found(false);

	int sdkResult = BS2_GetSlaveExDevice(context_, id, channelPort, &slaveDeviceObj, &outputPort, &slaveDeviceCount);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetSlaveExDevice call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < slaveDeviceCount; index++)
	{
		if (slaveDeviceObj[index].deviceID == slaveID)
		{
			if (slaveDeviceObj[index].enableOSDP == false)
			{
				slaveDeviceObj[index].enableOSDP = true;

				sdkResult = BS2_SetSlaveExDevice(context_, id, outputPort, slaveDeviceObj, slaveDeviceCount);
			}

			found = true;
			break;
		}
	}

	if (!found)
	{
		TRACE("The slave device(%u) could not be found under CST(%u).", slaveID, id);
		sdkResult = BS_SDK_ERROR_CANNOT_FIND_DEVICE;
	}

	if (slaveDeviceObj)
		BS2_ReleaseObject(slaveDeviceObj);

	return sdkResult;
}


int CommControl::deleteCSTSlaveDevice(BS2_DEVICE_ID id, uint32_t channelPort, BS2_DEVICE_ID slaveID)
{
	BS2Rs485SlaveDeviceEX *slaveDeviceObj = NULL;
	uint32_t slaveDeviceCount = 0;
	uint32_t outputPort = 0;
	bool found(false);

	int sdkResult = BS2_GetSlaveExDevice(context_, id, channelPort, &slaveDeviceObj, &outputPort, &slaveDeviceCount);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		for (uint32_t index = 0; index < slaveDeviceCount; index++)
		{
			if (slaveDeviceObj[index].deviceID == slaveID)
			{
				slaveDeviceObj[index].enableOSDP = false;

				sdkResult = BS2_SetSlaveExDevice(context_, id, outputPort, slaveDeviceObj, slaveDeviceCount);
			}

			found = true;
			break;
		}
	}
	else
	{
		TRACE("BS2_GetSlaveExDevice call failed: %d", sdkResult);
		sdkResult = BS_SDK_ERROR_CANNOT_FIND_DEVICE;
	}

	if (slaveDeviceObj)
		BS2_ReleaseObject(slaveDeviceObj);

	return sdkResult;
}

int CommControl::searchWiegandDevice(BS2_DEVICE_ID id, vector<BS2_DEVICE_ID>& wiegandList)
{
	BS2_DEVICE_ID* wiegandDeviceObj = NULL;
	uint32_t wiegandDeviceCount(0);

	int sdkResult = BS2_SearchWiegandDevices(context_, id, &wiegandDeviceObj, &wiegandDeviceCount);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SearchWiegandDevices call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < wiegandDeviceCount; index++)
	{
		wiegandList.push_back(wiegandDeviceObj[index]);
	}

	if (wiegandDeviceObj)
		BS2_ReleaseObject(wiegandDeviceObj);

	return sdkResult;
}

int CommControl::addWiegandDevice(BS2_DEVICE_ID id, BS2_DEVICE_ID wiegandID)
{
	return BS2_AddWiegandDevices(context_, id, &wiegandID, 1);
}

int CommControl::getWiegandDevice(BS2_DEVICE_ID id, vector<BS2_DEVICE_ID>& wiegandList)
{
	BS2_DEVICE_ID* wiegandDeviceObj = NULL;
	uint32_t wiegandDeviceCount(0);

	int sdkResult = BS2_GetWiegandDevices(context_, id, &wiegandDeviceObj, &wiegandDeviceCount);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetWiegandDevices call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < wiegandDeviceCount; index++)
	{
		wiegandList.push_back(wiegandDeviceObj[index]);
	}

	if (wiegandDeviceObj)
		BS2_ReleaseObject(wiegandDeviceObj);

	return sdkResult;
}

int CommControl::deleteWiegandDevice(BS2_DEVICE_ID id, BS2_DEVICE_ID wiegandID)
{
	return BS2_RemoveWiegandDevices(context_, id, &wiegandID, 1);
}

int CommControl::setKeepAliveTimeout()
{
	const long MILLI_SEC = 1000;

	long sec = Utility::getInput<long>("How many seconds?");
	sec *= MILLI_SEC;

	int sdkResult = BS2_SetKeepAliveTimeout(context_, sec);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetKeepAliveTimeout call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::getServerPort()
{
	BS2_PORT port = 0;

	int sdkResult = BS2_GetServerPort(context_, &port);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetServerPort call failed: %d", sdkResult);
	else
		cout << "Server V4 port is " << port << endl;

	return sdkResult;
}

int CommControl::setServerPort()
{
	BS2_PORT port = Utility::getInput<BS2_PORT>("Server V4 port:");

	int sdkResult = BS2_SetServerPort(context_, port);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetServerPort call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::getSSLServerPort()
{
	BS2_PORT port = 0;

	int sdkResult = BS2_GetSSLServerPort(context_, &port);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetSSLServerPort call failed: %d", sdkResult);
	else
		cout << "Server SSL V4 port is " << port << endl;

	return sdkResult;
}

int CommControl::setSSLServerPort()
{
	BS2_PORT port = Utility::getInput<BS2_PORT>("Server SSL V4 port:");

	int sdkResult = BS2_SetSSLServerPort(context_, port);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetSSLServerPort call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::isConnected(BS2_DEVICE_ID id)
{
	int connected = 0;

	int sdkResult = BS2_IsConnected(context_, id, &connected);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_IsConnected call failed: %d", sdkResult);
	else
		cout << "Device (" << id << ") is " << (connected ? "connected" : "not connected") << endl;

	return sdkResult;
}

int CommControl::isAutoConnection()
{
	int enable = 0;

	int sdkResult = BS2_IsAutoConnection(context_, &enable);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_IsAutoConnection call failed: %d", sdkResult);
	else
		cout << "Auto connection is " << enable << endl;

	return sdkResult;
}

int CommControl::setAutoConnection()
{
	char input = Utility::getInput<char>("Do you want to enable auto connection? [y/n]");
	int enable = (input == 'y' || input == 'Y') ? 1 : 0;

	int sdkResult = BS2_SetAutoConnection(context_, enable);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetAutoConnection call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::getEnableIPV4()
{
	int enable = 0;

	int sdkResult = BS2_GetEnableIPV4(context_, &enable);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetEnableIPV4 call failed: %d", sdkResult);
	else
		cout << "IPV4 connection mode is " << enable << endl;

	return sdkResult;
}

int CommControl::setEnableIPV4()
{
	char input = Utility::getInput<char>("Do you want to enable IPV4 connection mode? [y/n]");
	int enable = (input == 'y' || input == 'Y') ? 1 : 0;

	int sdkResult = BS2_SetEnableIPV4(context_, enable);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetEnableIPV4 call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::getEnableIPV6()
{
	int enable = 0;

	int sdkResult = BS2_GetEnableIPV6(context_, &enable);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetEnableIPV6 call failed: %d", sdkResult);
	else
		cout << "IPV6 connection mode is " << enable << endl;

	return sdkResult;
}

int CommControl::setEnableIPV6()
{
	char input = Utility::getInput<char>("Do you want to enable IPV6 connection mode? [y/n]");
	int enable = (input == 'y' || input == 'Y') ? 1 : 0;

	int sdkResult = BS2_SetEnableIPV6(context_, enable);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetEnableIPV6 call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::getServerPortIPV6()
{
	BS2_PORT port = 0;

	int sdkResult = BS2_GetServerPortIPV6(context_, &port);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetServerPortIPV6 call failed: %d", sdkResult);
	else
		cout << "Server V6 port is " << port << endl;

	return sdkResult;
}

int CommControl::setServerPortIPV6()
{
	BS2_PORT port = Utility::getInput<BS2_PORT>("Server V6 port:");

	int sdkResult = BS2_SetServerPortIPV6(context_, port);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetServerPortIPV6 call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::getSSLServerPortIPV6()
{
	BS2_PORT port = 0;

	int sdkResult = BS2_GetSSLServerPortIPV6(context_, &port);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetSSLServerPortIPV6 call failed: %d", sdkResult);
	else
		cout << "Server SSL V6 port is " << port << endl;

	return sdkResult;
}

int CommControl::setSSLServerPortIPV6()
{
	BS2_PORT port = Utility::getInput<BS2_PORT>("Server SSL V6 port:");

	int sdkResult = BS2_SetSSLServerPortIPV6(context_, port);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetSSLServerPortIPV6 call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::getDefaultResponseTimeout()
{
	long ms = 0L;

	int sdkResult = BS2_GetDefaultResponseTimeout(context_, &ms);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetDefaultResponseTimeout call failed: %d", sdkResult);
	}
	else
	{
		ms /= 1000;
		cout << "DefaultResponseTimeout is " << ms << endl;
	}

	return sdkResult;
}

int CommControl::setDefaultResponseTimeout()
{
	const long MILLI_SEC = 1000;

	long sec = Utility::getInput<long>("How many seconds?");
	sec *= MILLI_SEC;

	int sdkResult = BS2_SetDefaultResponseTimeout(context_, sec);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetDefaultResponseTimeout call failed: %d", sdkResult);

	return sdkResult;
}
