#include "stdafx.h"
#include <iostream>
#include "ConfigControl.h"
#include "BS_Errno.h"
#include "Utility.h"


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

	print(config);

	return sdkResult;
}

int ConfigControl::setSystemConfig(BS2_DEVICE_ID id, const BS2SystemConfig& config)
{
	int sdkResult = BS2_SetSystemConfig(context_, id, const_cast<BS2SystemConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetSystemConfig call failed: %d", sdkResult);

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

int ConfigControl::getFingerprintConfig(BS2_DEVICE_ID id, BS2FingerprintConfig& config)
{
	int sdkResult = BS2_GetFingerprintConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetFingerprintConfig call failed: %d", sdkResult);
	else
		print(config);

	return sdkResult;
}

int ConfigControl::setFingerprintConfig(BS2_DEVICE_ID id, const BS2FingerprintConfig& config)
{
	int sdkResult = BS2_SetFingerprintConfig(context_, id, const_cast<BS2FingerprintConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetFingerprintConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getFaceConfig(BS2_DEVICE_ID id, BS2FaceConfig& config)
{
	int sdkResult = BS2_GetFaceConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetFaceConfig call failed: %d", sdkResult);
	else
		print(config);

	return sdkResult;
}

int ConfigControl::setFaceConfig(BS2_DEVICE_ID id, const BS2FaceConfig& config)
{
	int sdkResult = BS2_SetFaceConfig(context_, id, const_cast<BS2FaceConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetFaceConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getDesFireCardConfigEx(BS2_DEVICE_ID id, BS2DesFireCardConfigEx& config)
{
	int sdkResult = BS2_GetDesFireCardConfigEx(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetDesFireCardConfigEx call failed: %d", sdkResult);
	else
		print(config);

	return sdkResult;
}

int ConfigControl::setDesFireCardConfigEx(BS2_DEVICE_ID id, const BS2DesFireCardConfigEx& config)
{
	int sdkResult = BS2_SetDesFireCardConfigEx(context_, id, const_cast<BS2DesFireCardConfigEx*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetDesFireCardConfigEx call failed: %d", sdkResult);

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

void ConfigControl::print(const BS2SystemConfig& config)
{
	TRACE("==[BS2SystemConfig]==");
	TRACE("timezone:%d", config.timezone);
	TRACE("syncTime:%d", config.syncTime);
	TRACE("serverSync:%d", config.serverSync);
	TRACE("deviceLocked:%d", config.deviceLocked);
	TRACE("useInterphone:%d", config.useInterphone);

	//TRACE("useUSBConnection:%d", config.useUSBConnection);
	TRACE("keyEncrypted:%d", config.keyEncrypted);
	TRACE("useJobCode:%d", config.useJobCode);
	TRACE("useAlphanumericID:%d", config.useAlphanumericID);

	TRACE("cameraFrequency:%u", config.cameraFrequency);
	TRACE("secureTamper:%d", config.secureTamper);
	TRACE("useCardOperationMask:0x%08x", config.useCardOperationMask);
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

void ConfigControl::print(const BS2FingerprintConfig& config)
{
	TRACE("==[BS2FingerprintConfig]==");
	TRACE("securityLevel:%u", config.securityLevel);
	TRACE("fastMode:%u", config.fastMode);
	TRACE("sensitivity:%u", config.sensitivity);
	TRACE("sensorMode:%u", config.sensorMode);
	TRACE("templateFormat:%u", config.templateFormat);
	TRACE("scanTimeout:%u", config.scanTimeout);
	TRACE("successiveScan:%u", config.successiveScan);
	TRACE("advancedEnrollment:%u", config.advancedEnrollment);
	TRACE("showImage;%u", config.showImage);
	TRACE("lfdLevel:%u", config.lfdLevel);
	TRACE("checkDuplicate:%u", config.checkDuplicate);
}

void ConfigControl::print(const BS2FaceConfig& config)
{
	TRACE("==[BS2FaceConfig]==");
	TRACE("securityLevel:%u", config.securityLevel);
	TRACE("lightCondition:%u", config.lightCondition);
	TRACE("enrollThreshold:%u", config.enrollThreshold);
	TRACE("detectSensitivity:%u", config.detectSensitivity);
	TRACE("enrollTimeout:%u", config.enrollTimeout);
	TRACE("lfdLevel:%u", config.lfdLevel);
	TRACE("quickEnrollment:%u", config.quickEnrollment);
	TRACE("previewOption:%u", config.previewOption);
	TRACE("checkDuplicate:%u", config.checkDuplicate);
}

void ConfigControl::print(const BS2DesFireCardConfigEx& config)
{
	TRACE("==[BS2DesFireCardConfigEx]==");
	TRACE("appMasterKey:%s", Utility::getHexaString(config.desfireAppKey.appMasterKey, 16).c_str());	// maybe 0
	TRACE("fileReadKey:%s", Utility::getHexaString(config.desfireAppKey.fileReadKey, 16).c_str());	// maybe 0
	TRACE("fileWriteKey:%s", Utility::getHexaString(config.desfireAppKey.fileWriteKey, 16).c_str());	// maybe 0
	TRACE("fileReadKeyNumber:%u", config.desfireAppKey.fileReadKeyNumber);
	TRACE("fileWriteKeyNumber:%u", config.desfireAppKey.fileWriteKeyNumber);
}
