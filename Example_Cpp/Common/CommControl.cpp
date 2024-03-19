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

int CommControl::setSearchTimeout(uint32_t sec)
{
	int sdkResult = BS2_SetDeviceSearchingTimeout(context_, sec);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetDeviceSearchingTimeout call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::searchDevices(vector<BS2_DEVICE_ID>& devices)
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
		devices.push_back(devicesIDObj[index]);
	}

	if (devicesIDObj)
		BS2_ReleaseObject(devicesIDObj);

	return sdkResult;
}

int CommControl::searchDevices(vector<BS2SimpleDeviceInfo>& devices, string hostIP)
{
	BS2_DEVICE_ID* devicesIDObj = NULL;
	uint32_t numOfDevices(0);

	int sdkResult = (0 == hostIP.size()) ?
		BS2_SearchDevices(context_) :
		BS2_SearchDevicesEx(context_, hostIP.c_str());

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
		BS2SimpleDeviceInfo deviceInfo;
		memset(&deviceInfo, 0x0, sizeof(deviceInfo));
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

int CommControl::addSlaveDevice(BS2_DEVICE_ID id, const vector<BS2Rs485SlaveDevice>& slaveList)
{
	int sdkResult = BS2_SetSlaveDevice(context_, id, const_cast<BS2Rs485SlaveDevice*>(slaveList.data()), slaveList.size());
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetSlaveDevice call failed: %d", sdkResult);
	}

	return sdkResult;
}

#if _NOT_SUPPORTED
int CommControl::setSlaveBaudrate(BS2_DEVICE_ID masterID, BS2_DEVICE_ID slaveID, uint32_t baudrate)
{
	int sdkResult = BS2_SetSlaveBaudrate(context_, masterID, slaveID, baudrate);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetSlaveBaudrate call failed: %d", sdkResult);
	}

	return sdkResult;
}
#endif

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

	if (slaveDeviceObj)
	{
		slaveList.clear();
		for (uint32_t index = 0; index < slaveDeviceCount; index++)
		{
			slaveList.push_back(slaveDeviceObj[index]);
		}

		BS2_ReleaseObject(slaveDeviceObj);
	}

	return sdkResult;
}

int CommControl::addCSTSlaveDevice(BS2_DEVICE_ID id, uint32_t channelPort, const vector<BS2Rs485SlaveDeviceEX>& slaveList)
{
	int sdkResult = BS2_SetSlaveExDevice(context_, id, channelPort, const_cast<BS2Rs485SlaveDeviceEX*>(slaveList.data()), slaveList.size());
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetSlaveExDevice call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::addOsdpStandardDevice(BS2_DEVICE_ID id, uint32_t channelIndex, const BS2OsdpStandardDeviceAdd& osdpDevice, uint32_t& outChannelIndex)
{
	int sdkResult = BS2_AddOsdpStandardDevice(context_, id, channelIndex, &osdpDevice, &outChannelIndex);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_AddOsdpStandardDevice call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::getOsdpStandardDevice(BS2_DEVICE_ID id, BS2OsdpStandardDevice& osdpDevice)
{
	int sdkResult = BS2_GetOsdpStandardDevice(context_, id, &osdpDevice);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetOsdpStandardDevice call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::getAvailableOsdpStandardDevice(BS2_DEVICE_ID id, BS2OsdpStandardDeviceAvailable& osdpDevice)
{
	int sdkResult = BS2_GetAvailableOsdpStandardDevice(context_, id, &osdpDevice);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetAvailableOsdpStandardDevice call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::updateOsdpStandardDevice(BS2_DEVICE_ID id, const vector<BS2OsdpStandardDeviceUpdate>& osdpDevices, vector<BS2OsdpStandardDeviceResult>& result)
{
	BS2OsdpStandardDeviceResult* outResultObj = NULL;
	uint32_t outNumOfResult(0);
	int sdkResult = BS2_UpdateOsdpStandardDevice(context_, id, const_cast<BS2OsdpStandardDeviceUpdate*>(osdpDevices.data()), osdpDevices.size(), &outResultObj, &outNumOfResult);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_UpdateOsdpStandardDevice call failed: %d", sdkResult);
	}

	if (outResultObj)
	{
		result.clear();
		for (uint32_t idx = 0; idx < outNumOfResult; idx++)
		{
			result.push_back(outResultObj[idx]);
		}

		BS2_ReleaseObject(outResultObj);
	}
	
	return sdkResult;
}

int CommControl::removeOsdpStandardDevice(BS2_DEVICE_ID id, const vector<BS2_DEVICE_ID>& osdpDeviceIDs, vector<BS2OsdpStandardDeviceResult>& result)
{
	BS2OsdpStandardDeviceResult* outResultObj = NULL;
	uint32_t outNumOfResult(0);
	int sdkResult = BS2_RemoveOsdpStandardDevice(context_, id, const_cast<BS2_DEVICE_ID*>(osdpDeviceIDs.data()), osdpDeviceIDs.size(), &outResultObj, &outNumOfResult);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_RemoveOsdpStandardDevice call failed: %d", sdkResult);
	}

	if (outResultObj)
	{
		result.clear();
		for (uint32_t idx = 0; idx < outNumOfResult; idx++)
		{
			result.push_back(outResultObj[idx]);
		}

		BS2_ReleaseObject(outResultObj);
	}

	return sdkResult;
}

int CommControl::getOsdpStandardDeviceCapability(BS2_DEVICE_ID id, BS2OsdpStandardDeviceCapability& capability)
{
	int sdkResult = BS2_GetOsdpStandardDeviceCapability(context_, id, &capability);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetOsdpStandardDeviceCapability call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::setOsdpStandardDeviceSecurityKey(BS2_DEVICE_ID id, const BS2OsdpStandardDeviceSecurityKey* key)
{
	int sdkResult = BS2_SetOsdpStandardDeviceSecurityKey(context_, id, key);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetOsdpStandardDeviceSecurityKey call failed: %d", sdkResult);
	}

	return sdkResult;
}

int CommControl::setOsdpStandardDeviceStatusListener(OnOsdpStandardDeviceStatusChanged fpOsdpStandardDeviceStatusChanged)
{
	int sdkResult = BS2_SetOsdpStandardDeviceStatusListener(context_, fpOsdpStandardDeviceStatusChanged);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_SetOsdpStandardDeviceStatusListener call failed: %d", sdkResult);
	}

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
	int enable = Utility::isYes("Do you want to enable auto connection?");

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
	int enable = Utility::isYes("Do you want to enable IPV4 connection mode?");

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
	int enable = Utility::isYes("Do you want to enable IPV6 connection mode?");

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

int CommControl::getSocketRetryCount(uint32_t& count)
{
	int sdkResult = BS2_GetSocketRetryCount(context_, &count);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetSocketRetryCount call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::setSocketRetryCount(uint32_t count)
{
	int sdkResult = BS2_SetSocketRetryCount(context_, count);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetSocketRetryCount call failed: %d", sdkResult);

	return sdkResult;
}


int CommControl::getSocketSSLRetryCount(uint32_t& count)
{
	int sdkResult = BS2_GetSocketSSLRetryCount(context_, &count);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetSocketSSLRetryCount call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::setSocketSSLRetryCount(uint32_t count)
{
	int sdkResult = BS2_SetSocketSSLRetryCount(context_, count);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetSocketSSLRetryCount call failed: %d", sdkResult);

	return sdkResult;
}

int CommControl::disableSSL(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_DisableSSL(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_DisableSSL call failed: %d", sdkResult);

	return sdkResult;
}

void CommControl::print(const BS2OsdpStandardDeviceAvailable& devices)
{
	TRACE("==[BS2OsdpStandardDeviceAvailable]==");
	TRACE("+--numOfChannel : %u", devices.numOfChannel);

	for (int idx = 0; idx < devices.numOfChannel; idx++)
	{
		TRACE("+--channel[%d]", idx);
		TRACE("   |--channelType : %u", devices.channels[idx].channelType);
		TRACE("   |--channelIndex : %u", devices.channels[idx].channelIndex);
		TRACE("   |--maxOsdpDevice : %u", devices.channels[idx].maxOsdpDevice);
		TRACE("   |--numOsdpAvailableDevice : %u", devices.channels[idx].numOsdpAvailableDevice);

		for (int didx = 0; didx < devices.channels[idx].numOsdpAvailableDevice; didx++)
		{
			TRACE("  |--deviceIDs[%d] : %u", didx, devices.channels[idx].deviceIDs[didx]);
		}
	}
}

void CommControl::print(const BS2OsdpStandardDevice& device)
{
	TRACE("==[BS2OsdpStandardDevice]==");
	TRACE("+--deviceID : %u", device.deviceID);
	TRACE("|--deviceType : %u", device.deviceType);
	TRACE("|--enableOSDP : %u", device.enableOSDP);
	TRACE("|--connected : %u", device.connected);
	TRACE("|--channelInfo : %u", device.channelInfo);
	TRACE("|--osdpID : %u", device.osdpID);
	TRACE("|--supremaSearch : %u", device.supremaSearch);
	TRACE("|--activate : %u", device.activate);
	TRACE("|--useSecure : %u", device.useSecure);
	TRACE("|--vendorCode : %u", Utility::getHexaString(device.vendorCode, sizeof(device.vendorCode)));
	TRACE("|--fwVersion : %u", device.fwVersion);
	TRACE("|--modelNumber : %u", device.modelNumber);
	TRACE("|--modelVersion : %u", device.modelVersion);
	TRACE("|--readInfo : %u", device.readInfo);
}

void CommControl::print(const BS2OsdpStandardDeviceNotify& device)
{
	TRACE("==[BS2OsdpStandardDeviceNotify]==");
	TRACE("+--deviceID : %u", device.deviceID);
	TRACE("|--deviceType : %u", device.deviceType);
	TRACE("|--enableOSDP : %u", device.enableOSDP);
	TRACE("|--connected : %u", device.connected);
	TRACE("|--channelInfo : %u", device.channelInfo);
	TRACE("|--osdpID : %u", device.osdpID);
	TRACE("|--supremaSearch : %u", device.supremaSearch);
	TRACE("|--activate : %u", device.activate);
	TRACE("|--useSecure : %u", device.useSecure);
	TRACE("|--vendorCode : %u", Utility::getHexaString(device.vendorCode, sizeof(device.vendorCode)));
	TRACE("|--fwVersion : %u", device.fwVersion);
	TRACE("|--modelNumber : %u", device.modelNumber);
	TRACE("|--modelVersion : %u", device.modelVersion);
	TRACE("|--readInfo : %u", device.readInfo);
}

void CommControl::print(const vector<BS2OsdpStandardDeviceResult>& result)
{
	uint32_t idx(0);
	TRACE("==[BS2OsdpStandardDeviceResult]==");
	for (auto item : result)
	{
		TRACE("+--[%u]", idx++);
		TRACE("    |--deviceID : %u", item.deviceID);
		TRACE("    |--result : %u", item.result);
	}
}

void CommControl::print(const BS2OsdpStandardDeviceCapability& capability)
{
	TRACE("==[BS2OsdpStandardDeviceCapability]==");
	TRACE("+--input");
	TRACE("    |--compliance : %u", capability.input.compliance);
	TRACE("    |--count : %u", capability.input.count);
	TRACE("+--output");
	TRACE("    |--compliance : %u", capability.output.compliance);
	TRACE("    |--count : %u", capability.output.count);
	TRACE("+--led");
	TRACE("    |--compliance : %u", capability.led.compliance);
	TRACE("    |--count : %u", capability.led.count);
	TRACE("+--audio");
	TRACE("    |--compliance : %u", capability.audio.compliance);
	TRACE("    |--count : %u", capability.audio.count);
	TRACE("+--textOutput");
	TRACE("    |--compliance : %u", capability.textOutput.compliance);
	TRACE("    |--count : %u", capability.textOutput.count);
	TRACE("+--reader");
	TRACE("    |--compliance : %u", capability.reader.compliance);
	TRACE("    |--count : %u", capability.reader.count);
	TRACE("+--recvBufferSize : %u", capability.recvBufferSize);
	TRACE("+--largeMsgSize : %u", capability.largeMsgSize);
	TRACE("+--osdpVersion : %u", capability.osdpVersion);
	TRACE("+--cardFormat : %u", capability.cardFormat);
	TRACE("+--timeKeeping : %u", capability.timeKeeping);
	TRACE("+--canCommSecure : %u", capability.canCommSecure);
	TRACE("+--crcSupport : %u", capability.crcSupport);
	TRACE("+--smartCardSupport : %u", capability.smartCardSupport);
	TRACE("+--biometricSupport : %u", capability.biometricSupport);
	TRACE("+--securePinEntrySupport : %u", capability.securePinEntrySupport);
}
