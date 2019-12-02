#pragma once

#include <vector>
#include <string>
#include "BS_API.h"


class ConfigControl
{
public:
	ConfigControl(void* sdkContext);
	virtual ~ConfigControl();


public:
	int getConfig(BS2_DEVICE_ID id, BS2Configs& config);
	int getSystemConfig(BS2_DEVICE_ID id, BS2SystemConfig& config);
	int getTimezone(BS2_DEVICE_ID id, int& timezone);
	int getDisplayConfig(BS2_DEVICE_ID id, BS2DisplayConfig& config);
	int setDisplayConfig(BS2_DEVICE_ID id, const BS2DisplayConfig& config);
	int getIPConfig(BS2_DEVICE_ID id, BS2IpConfig& config);
	int getFactoryConfig(BS2_DEVICE_ID id, BS2FactoryConfig& config);

	int updateConnectionModeViaUDP(BS2_DEVICE_ID id, BS2_CONNECTION_MODE mode = BS2_CONNECTION_MODE_SERVER_TO_DEVICE);
	int updateRS485OperationMode(BS2_DEVICE_ID id, BS2_RS485_MODE mode = BS2_RS485_MODE_DEFAULT);

private:
	void print(const BS2DisplayConfig& config);
	void print(const BS2IpConfig& config);
	void print(const BS2FactoryConfig& config);


private:
	void* context_;
};
