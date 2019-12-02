#include "stdafx.h"
#include <iostream>
#include "ConfigControl.h"
#include "BS_Errno.h"
#include "Utils.h"


extern void TRACE(const char* fmt, ...);
using namespace std;


ConfigControl::ConfigControl(void* sdkContext) : context_(sdkContext)
{
}


ConfigControl::~ConfigControl()
{
}


int ConfigControl::getConfig(BS2_DEVICE_ID id, BS2Configs& config)
{
	return BS2_GetConfig(context_, id, &config);
}

int ConfigControl::getSystemConfig(BS2_DEVICE_ID id, BS2SystemConfig& config)
{
	int sdkResult = BS2_GetSystemConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetSystemConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getTimezone(BS2_DEVICE_ID id, int& timezone)
{
	BS2SystemConfig systemConfig = { 0, };
	int sdkResult = getSystemConfig(id, systemConfig);
	if (BS_SDK_SUCCESS == sdkResult)
		timezone = systemConfig.timezone;

	return sdkResult;
}

int ConfigControl::getDisplayConfig(BS2_DEVICE_ID id, BS2DisplayConfig& config)
{
	int sdkResult = BS2_GetDisplayConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetDisplayConfig call failed: %d", sdkResult);
	else
		print(config);

	return sdkResult;
}

int ConfigControl::setDisplayConfig(BS2_DEVICE_ID id, const BS2DisplayConfig& config)
{
	int sdkResult = BS2_SetDisplayConfig(context_, id, const_cast<BS2DisplayConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetDisplayConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getIPConfig(BS2_DEVICE_ID id, BS2IpConfig& config)
{
	int sdkResult = BS2_GetIPConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetIPConfig call failed: %d", sdkResult);
	else
		print(config);

	return sdkResult;
}

int ConfigControl::getFactoryConfig(BS2_DEVICE_ID id, BS2FactoryConfig& config)
{
	int sdkResult = BS2_GetFactoryConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetFactoryConfig call failed: %d", sdkResult);
	else
		print(config);

	return sdkResult;
}

int ConfigControl::updateConnectionModeViaUDP(BS2_DEVICE_ID id, BS2_CONNECTION_MODE mode)
{
	BS2IpConfig config = { 0, };
	int sdkResult = BS2_GetIPConfigViaUDP(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetIPConfigViaUDP call failed: %d", sdkResult);
		return sdkResult;
	}

	config.connectionMode = mode;

	sdkResult = BS2_SetIPConfigViaUDP(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetIPConfigViaUDP call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::updateRS485OperationMode(BS2_DEVICE_ID id, BS2_RS485_MODE mode)
{
	BS2Rs485Config config = { 0, };
	int sdkResult = BS2_GetRS485Config(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetRS485Config call failed: %d", sdkResult);
		return sdkResult;
	}

	config.mode = mode;

	sdkResult = BS2_SetRS485Config(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetRS485Config call failed: %d", sdkResult);

	return sdkResult;
}

void ConfigControl::print(const BS2DisplayConfig& config)
{
	TRACE("==[BS2DisplayConfig]==");
	TRACE("language:%u", config.language);
	TRACE("background:%u", config.background);
	TRACE("volume:%u", config.volume);
	TRACE("bgTheme:%u", config.bgTheme);
	TRACE("dateFormat:%u", config.dateFormat);

	TRACE("menuTimeout:%u", config.menuTimeout);
	TRACE("msgTimeout:%u", config.msgTimeout);
	TRACE("backlightTimeout:%u", config.backlightTimeout);
	TRACE("displayDateTime:%u", config.displayDateTime);
	TRACE("useVoice:%u", config.useVoice);

	TRACE("timeFormat:%u", config.timeFormat);
	TRACE("homeFormation:%u", config.homeFormation);
	TRACE("useUserPhrase:%u", config.useUserPhrase);

	//uint8_t shortcutHome[BS2_MAX_SHORTCUT_HOME];	///< 8 bytes
	//uint8_t tnaIcon[BS2_MAX_TNA_KEY];		///< 16 bytes : tnaIcon
}

void ConfigControl::print(const BS2IpConfig& config)
{
	TRACE("==[BS2IpConfig]==");
	TRACE("connectionMode:%s", config.connectionMode ? "D2S" : "S2D");
	TRACE("useDHCP:%u", config.useDHCP);
	TRACE("useDNS:%u", config.useDNS);
	TRACE("ipAddress:%s", config.ipAddress);
	TRACE("gateway:%s", config.gateway);
	TRACE("subnetMask:%s", config.subnetMask);
	TRACE("serverAddr:%s", config.serverAddr);
	TRACE("port:%u", config.port);
	TRACE("mtuSize:%u", config.mtuSize);
	TRACE("baseband:%u", config.baseband);
	TRACE("sslServerPort:%u", config.sslServerPort);
}


void ConfigControl::print(const BS2FactoryConfig& config)
{
	TRACE("==[BS2FactoryConfig]==");
	TRACE("deviceID:%u", config.deviceID);
	TRACE("macAddr:%02x:%02x:%02x:%02x:%02x:%02x",
		config.macAddr[0], config.macAddr[1], config.macAddr[2],
		config.macAddr[3], config.macAddr[4], config.macAddr[5]);
	TRACE("modelName:%s", config.modelName);
	TRACE("boardVer:%d.%d.%d",
		config.boardVer.major, config.boardVer.minor, config.boardVer.ext);
	TRACE("kernelVer:%d.%d.%d(%s)",
		config.kernelVer.major, config.kernelVer.minor, config.kernelVer.ext, config.kernelRev);
	TRACE("bscoreVer:%d.%d.%d(%s)",
		config.bscoreVer.major, config.bscoreVer.minor, config.bscoreVer.ext, config.bscoreRev);
	TRACE("firmwareVer:%d.%d.%d(%s)",
		config.firmwareVer.major, config.firmwareVer.minor, config.firmwareVer.ext, config.firmwareRev);
}