#pragma once

#include <vector>
#include <string>
#include "BS_API.h"


class DeviceControl
{
public:
	DeviceControl(void* sdkContext);
	virtual ~DeviceControl();


public:
	int getDeviceInfo(BS2_DEVICE_ID id, BS2SimpleDeviceInfo& info);
	int getDeviceInfoEx(BS2_DEVICE_ID id, BS2SimpleDeviceInfo& info, BS2SimpleDeviceInfoEx& infoEx);
	int getSpecifiedDeviceInfo(BS2_DEVICE_ID id);
	int getDeviceTime(BS2_DEVICE_ID id);
	int setDeviceTime(BS2_DEVICE_ID id);
	int getDeviceTime(BS2_DEVICE_ID id, BS2_TIMESTAMP& currTime);
	int setDeviceTime(BS2_DEVICE_ID id, BS2_TIMESTAMP& currTime);
	int clearDatabase(BS2_DEVICE_ID id);
	int factoryReset(BS2_DEVICE_ID id);
	int rebootDevice(BS2_DEVICE_ID id);
	int lockDevice(BS2_DEVICE_ID id);
	int unlockDevice(BS2_DEVICE_ID id);
	int upgradeFirmware(BS2_DEVICE_ID id);
	int upgradeFirmware(const std::vector<BS2_DEVICE_ID>& devices);
	int updateResource(BS2_DEVICE_ID id);
	int getDeviceCapabilities(BS2_DEVICE_ID id, BS2DeviceCapabilities& cap);

	static void print(const BS2SimpleDeviceInfo& info);
	static void print(const BS2SimpleDeviceInfoEx& info);
	static void print(const BS2IPv6DeviceInfo& info);
	static void print(const BS2DeviceCapabilities& info);

private:
	static void onUpgrade(BS2_DEVICE_ID id, uint32_t percent);


private:
	static void* context_;
};
