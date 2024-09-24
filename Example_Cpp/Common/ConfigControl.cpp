#include "stdafx.h"
#include <iostream>
#include <map>
#include "ConfigControl.h"
#include "BS_Errno.h"
#include "Utility.h"
#include "UserControl.h"


extern void TRACE(const char* fmt, ...);
using namespace std;


const map<BS2_TRIGGER_TYPE, string> kTRIGGER_TYPE = {
	{BS2_TRIGGER_NONE,     "none"},
	{BS2_TRIGGER_EVENT,    "event"},
	{BS2_TRIGGER_INPUT,    "input"},
	{BS2_TRIGGER_SCHEDULE, "schedule"},
};

const map<BS2_ACTION_TYPE, string> kACTION_TYPE = {
	{BS2_ACTION_NONE,				"none"},

	{BS2_ACTION_LOCK_DEVICE,		"lock device"},
	{BS2_ACTION_UNLOCK_DEVICE,		"unlock device"},
	{BS2_ACTION_REBOOT_DEVICE,		"reboot device"},
	{BS2_ACTION_RELEASE_ALARM,		"release alarm"},
	{BS2_ACTION_GENERAL_INPUT,		"general input"},

	{BS2_ACTION_RELAY,				"relay"},
	{BS2_ACTION_TTL,				"output"},
	{BS2_ACTION_SOUND,				"sound"},
	{BS2_ACTION_DISPLAY,			"display"},
	{BS2_ACTION_BUZZER,				"buzzer"},
	{BS2_ACTION_LED,				"led"},

	{BS2_ACTION_FIRE_ALARM_INPUT,	"fire alarm input"},

	{BS2_ACTION_AUTH_SUCCESS,		"auth success"},
	{BS2_ACTION_AUTH_FAIL,			"auth fail"},

	{BS2_ACTION_LIFT,				"lift"},
};

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

int ConfigControl::setSystemConfig(BS2_DEVICE_ID id, const BS2SystemConfig& config)
{
	int sdkResult = BS2_SetSystemConfig(context_, id, const_cast<BS2SystemConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetSystemConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getTimezone(BS2_DEVICE_ID id, int& timezone)
{
	BS2SystemConfig systemConfig;
	memset(&systemConfig, 0x0, sizeof(systemConfig));
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

	return sdkResult;
}

int ConfigControl::setDisplayConfig(BS2_DEVICE_ID id, const BS2DisplayConfig& config)
{
	int sdkResult = BS2_SetDisplayConfig(context_, id, const_cast<BS2DisplayConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetDisplayConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getIPConfig(BS2_DEVICE_ID id, BS2IpConfig& config) const
{
	int sdkResult = BS2_GetIPConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetIPConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setIPConfig(BS2_DEVICE_ID id, const BS2IpConfig& config)
{
	int sdkResult = BS2_SetIPConfig(context_, id, const_cast<BS2IpConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetIPConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getFactoryConfig(BS2_DEVICE_ID id, BS2FactoryConfig& config)
{
	int sdkResult = BS2_GetFactoryConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetFactoryConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getInputConfig(BS2_DEVICE_ID id, BS2InputConfig& config)
{
	int sdkResult = BS2_GetInputConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetInputConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getStatusConfig(BS2_DEVICE_ID id, BS2StatusConfig& config) const
{
	int sdkResult = BS2_GetStatusConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetStatusConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setStatusConfig(BS2_DEVICE_ID id, const BS2StatusConfig& config) const
{
	int sdkResult = BS2_SetStatusConfig(context_, id, const_cast<BS2StatusConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetStatusConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getFingerprintConfig(BS2_DEVICE_ID id, BS2FingerprintConfig& config)
{
	int sdkResult = BS2_GetFingerprintConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetFingerprintConfig call failed: %d", sdkResult);

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

	return sdkResult;
}

int ConfigControl::setDesFireCardConfigEx(BS2_DEVICE_ID id, const BS2DesFireCardConfigEx& config)
{
	int sdkResult = BS2_SetDesFireCardConfigEx(context_, id, const_cast<BS2DesFireCardConfigEx*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetDesFireCardConfigEx call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getAuthConfig(BS2_DEVICE_ID id, BS2AuthConfig& config)
{
	int sdkResult = BS2_GetAuthConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetAuthConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setAuthConfig(BS2_DEVICE_ID id, const BS2AuthConfig& config)
{
	int sdkResult = BS2_SetAuthConfig(context_, id, const_cast<BS2AuthConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetAuthConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getAuthConfigEx(BS2_DEVICE_ID id, BS2AuthConfigExt& config)
{
	int sdkResult = BS2_GetAuthConfigExt(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetAuthConfigExt call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setAuthConfigEx(BS2_DEVICE_ID id, const BS2AuthConfigExt& config)
{
	int sdkResult = BS2_SetAuthConfigExt(context_, id, const_cast<BS2AuthConfigExt*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetAuthConfigExt call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getFaceConfigEx(BS2_DEVICE_ID id, BS2FaceConfigExt& config)
{
	int sdkResult = BS2_GetFaceConfigExt(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetFaceConfigExt call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setFaceConfigEx(BS2_DEVICE_ID id, const BS2FaceConfigExt& config)
{
	int sdkResult = BS2_SetFaceConfigExt(context_, id, const_cast<BS2FaceConfigExt*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetFaceConfigExt call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getThermalCameraConfig(BS2_DEVICE_ID id, BS2ThermalCameraConfig& config)
{
	int sdkResult = BS2_GetThermalCameraConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetThermalCameraConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setThermalCameraConfig(BS2_DEVICE_ID id, const BS2ThermalCameraConfig& config)
{
	int sdkResult = BS2_SetThermalCameraConfig(context_, id, const_cast<BS2ThermalCameraConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetThermalCameraConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getEventConfig(BS2_DEVICE_ID id, BS2EventConfig& config)
{
	int sdkResult = BS2_GetEventConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetEventConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setEventConfig(BS2_DEVICE_ID id, const BS2EventConfig& config)
{
	int sdkResult = BS2_SetEventConfig(context_, id, const_cast<BS2EventConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetEventConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getTriggerActionConfig(BS2_DEVICE_ID id, BS2TriggerActionConfig& config)
{
	int sdkResult = BS2_GetTriggerActionConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetTriggerActionConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setTriggerActionConfig(BS2_DEVICE_ID id, const BS2TriggerActionConfig& config)
{
	int sdkResult = BS2_SetTriggerActionConfig(context_, id, const_cast<BS2TriggerActionConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetTriggerActionConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getBarcodeConfig(BS2_DEVICE_ID id, BS2BarcodeConfig& config)
{
	int sdkResult = BS2_GetBarcodeConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetBarcodeConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setBarcodeConfig(BS2_DEVICE_ID id, const BS2BarcodeConfig& config)
{
	int sdkResult = BS2_SetBarcodeConfig(context_, id, const_cast<BS2BarcodeConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetBarcodeConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getRS485Config(BS2_DEVICE_ID id, BS2Rs485Config& config)
{
	int sdkResult = BS2_GetRS485Config(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetRS485Config call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setRS485Config(BS2_DEVICE_ID id, const BS2Rs485Config& config)
{
	int sdkResult = BS2_SetRS485Config(context_, id, const_cast<BS2Rs485Config*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetRS485Config call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getRS485ConfigEx(BS2_DEVICE_ID id, BS2Rs485ConfigEX& config)
{
	int sdkResult = BS2_GetRS485ConfigEx(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetRS485ConfigEx call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getInputConfigEx(BS2_DEVICE_ID id, BS2InputConfigEx& config)
{
	int sdkResult = BS2_GetInputConfigEx(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetInputConfigEx call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setInputConfigEx(BS2_DEVICE_ID id, const BS2InputConfigEx& config)
{
	int sdkResult = BS2_SetInputConfigEx(context_, id, const_cast<BS2InputConfigEx*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetInputConfigEx call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getRelayActionConfig(BS2_DEVICE_ID id, BS2RelayActionConfig& config)
{
	int sdkResult = BS2_GetRelayActionConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetRelayActionConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setRelayActionConfig(BS2_DEVICE_ID id, const BS2RelayActionConfig& config)
{
	int sdkResult = BS2_SetRelayActionConfig(context_, id, const_cast<BS2RelayActionConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetRelayActionConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getWLANConfig(BS2_DEVICE_ID id, BS2WlanConfig& config)
{
	int sdkResult = BS2_GetWlanConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetWlanConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setWLANConfig(BS2_DEVICE_ID id, const BS2WlanConfig& config)
{
	int sdkResult = BS2_SetWlanConfig(context_, id, const_cast<BS2WlanConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetWlanConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getCardConfig(BS2_DEVICE_ID id, BS2CardConfig& config) const
{
	int sdkResult = BS2_GetCardConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetCardConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setCardConfig(BS2_DEVICE_ID id, const BS2CardConfig& config) const
{
	int sdkResult = BS2_SetCardConfig(context_, id, const_cast<BS2CardConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetCardConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getWiegandConfig(BS2_DEVICE_ID id, BS2WiegandConfig& config) const
{
	int sdkResult = BS2_GetWiegandConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetWiegandConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setWiegandConfig(BS2_DEVICE_ID id, const BS2WiegandConfig& config) const
{
	int sdkResult = BS2_SetWiegandConfig(context_, id, const_cast<BS2WiegandConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetWiegandConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getWiegandMultiConfig(BS2_DEVICE_ID id, BS2WiegandMultiConfig& config) const
{
	int sdkResult = BS2_GetWiegandMultiConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetWiegandMultiConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setWiegandMultiConfig(BS2_DEVICE_ID id, const BS2WiegandMultiConfig& config) const
{
	int sdkResult = BS2_SetWiegandMultiConfig(context_, id, const_cast<BS2WiegandMultiConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetWiegandMultiConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getVoipConfigExt(BS2_DEVICE_ID id, BS2VoipConfigExt& config) const
{
	int sdkResult = BS2_GetVoipConfigExt(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetVoipConfigExt call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setVoipConfigExt(BS2_DEVICE_ID id, const BS2VoipConfigExt& config) const
{
	int sdkResult = BS2_SetVoipConfigExt(context_, id, const_cast<BS2VoipConfigExt*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetVoipConfigExt call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getRtspConfig(BS2_DEVICE_ID id, BS2RtspConfig& config) const
{
	int sdkResult = BS2_GetRtspConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetRtspConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setRtspConfig(BS2_DEVICE_ID id, const BS2RtspConfig& config) const
{
	int sdkResult = BS2_SetRtspConfig(context_, id, const_cast<BS2RtspConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetRtspConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getLicenseConfig(BS2_DEVICE_ID id, BS2LicenseConfig& config) const
{
	int sdkResult = BS2_GetLicenseConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetLicenseConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getOsdpStandardConfig(BS2_DEVICE_ID id, BS2OsdpStandardConfig& config) const
{
	int sdkResult = BS2_GetOsdpStandardConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetOsdpStandardConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getOsdpStandardActionConfig(BS2_DEVICE_ID id, BS2OsdpStandardActionConfig& config) const
{
	int sdkResult = BS2_GetOsdpStandardActionConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetOsdpStandardActionConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setOsdpStandardActionConfig(BS2_DEVICE_ID id, const BS2OsdpStandardActionConfig& config) const
{
	int sdkResult = BS2_SetOsdpStandardActionConfig(context_, id, const_cast<BS2OsdpStandardActionConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetOsdpStandardActionConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getCustomCardConfig(BS2_DEVICE_ID id, BS2CustomCardConfig& config) const
{
	int sdkResult = BS2_GetCustomCardConfig(context_, id, &config);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetCustomCardConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::setCustomCardConfig(BS2_DEVICE_ID id, const BS2CustomCardConfig& config) const
{
	int sdkResult = BS2_SetCustomCardConfig(context_, id, const_cast<BS2CustomCardConfig*>(&config));
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetCustomCardConfig call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::updateConnectionModeViaUDP(BS2_DEVICE_ID id, BS2_CONNECTION_MODE mode)
{
	BS2IpConfig config;
	memset(&config, 0x0, sizeof(config));
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

int ConfigControl::updateConnectModeDevice2Server(BS2_DEVICE_ID id, string serverIP, BS2_PORT serverPort)
{
	BS2IpConfig config;
	memset(&config, 0x0, sizeof(config));
	int sdkResult = getIPConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.connectionMode = BS2_CONNECTION_MODE_DEVICE_TO_SERVER;
	config.serverPort = serverPort;
	strcpy(config.serverAddr, serverIP.c_str());

	return setIPConfig(id, config);
}

int ConfigControl::updateConnectModeServer2Device(BS2_DEVICE_ID id)
{
	BS2IpConfig config;
	memset(&config, 0x0, sizeof(config));
	int sdkResult = getIPConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.connectionMode = BS2_CONNECTION_MODE_SERVER_TO_DEVICE;

	return setIPConfig(id, config);
}

int ConfigControl::updateRS485OperationMode(BS2_DEVICE_ID id, BS2_RS485_MODE mode)
{
	BS2Rs485Config config;
	memset(&config, 0x0, sizeof(config));
	int sdkResult = getRS485Config(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.mode = mode;

	return setRS485Config(id, config);
}

int ConfigControl::resetConfigExceptNetInfo(BS2_DEVICE_ID id, bool includeDB)
{
	uint8_t includeDBInfo = includeDB ? 1 : 0;
	int sdkResult = BS2_ResetConfigExceptNetInfo(context_, id, includeDBInfo);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_ResetConfigExceptNetInfo call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::getOperatorInAuthConfig(BS2_DEVICE_ID id, vector<BS2AuthOperatorLevel>& oprList)
{
	// Operators in the old FW devices
	BS2AuthConfig config;
	memset(&config, 0x0, sizeof(config));
	int sdkResult = getAuthConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	for (int idx = 0; idx < config.numOperators; idx++)
	{
		BS2AuthOperatorLevel opr;
		memcpy(&config.operators[idx], &opr, sizeof(BS2AuthOperatorLevel));
		oprList.push_back(opr);
	}

	return sdkResult;
}

int ConfigControl::setOperatorInAuthConfig(BS2_DEVICE_ID id, const vector<BS2AuthOperatorLevel>& oprList)
{
	// Operators in the old FW devices
	BS2AuthConfig config;
	memset(&config, 0x0, sizeof(config));

	int sdkResult = getAuthConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	int numOfOperators = config.numOperators;
	bool overwrited = false;
	for (auto opr : oprList)
	{
		for (int idx = 0; idx < config.numOperators; idx++)
		{
			if (string(opr.userID) == config.operators[idx].userID)
			{
				memcpy(&opr, &config.operators[idx], sizeof(BS2AuthOperatorLevel));
				overwrited = true;
				break;
			}
		}

		if (!overwrited)
		{
			if (config.numOperators < BS2_MAX_OPERATORS)
				memcpy(&opr, &config.operators[numOfOperators++], sizeof(BS2AuthOperatorLevel));
			else
				TRACE("Number of operators are MAX %d.", BS2_MAX_OPERATORS);
		}
	}

	return setAuthConfig(id, config);
}

int ConfigControl::getAuthOperatorLevelEx(BS2_DEVICE_ID id, const vector<string>& oprIDs, vector<BS2AuthOperatorLevel>& oprList)
{
	BS2UIDArray arr(oprIDs);
	uint32_t numOfOpr(0);
	BS2AuthOperatorLevel* oprLevelObj = NULL;

	//int sdkResult = BS2_GetAuthOperatorLevelEx(context_, id, (BS2_USER_ID*)oprIDs.data(), oprIDs.size(), &oprLevelObj, &numOfOpr);
	int sdkResult = BS2_GetAuthOperatorLevelEx(context_, id, (BS2_USER_ID*)arr.getPtr(), arr.getSize(), &oprLevelObj, &numOfOpr);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetAuthOperatorLevelEx call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < numOfOpr; index++)
		oprList.push_back(oprLevelObj[index]);

	if (oprLevelObj)
		BS2_ReleaseObject(oprLevelObj);

	return sdkResult;
}

int ConfigControl::getAllAuthOperatorLevelEx(BS2_DEVICE_ID id, vector<BS2AuthOperatorLevel>& oprList)
{
	uint32_t numOfOpr(0);
	BS2AuthOperatorLevel* oprLevelObj = NULL;

	int sdkResult = BS2_GetAllAuthOperatorLevelEx(context_, id, &oprLevelObj, &numOfOpr);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetAllAuthOperatorLevelEx call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t index = 0; index < numOfOpr; index++)
		oprList.push_back(oprLevelObj[index]);

	if (oprLevelObj)
		BS2_ReleaseObject(oprLevelObj);

	return sdkResult;
}

int ConfigControl::setAuthOperatorLevelEx(BS2_DEVICE_ID id, const vector<BS2AuthOperatorLevel>& oprList)
{
	int sdkResult = BS2_SetAuthOperatorLevelEx(context_, id, const_cast<BS2AuthOperatorLevel*>(oprList.data()), oprList.size());
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetAuthOperatorLevelEx call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::removeAuthOperatorLevelEx(BS2_DEVICE_ID id, const vector<string>& oprIDs)
{
	BS2UIDArray arr(oprIDs);

	int sdkResult = BS2_RemoveAuthOperatorLevelEx(context_, id, (BS2_USER_ID*)arr.getPtr(), arr.getSize());
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_RemoveAuthOperatorLevelEx call failed: %d", sdkResult);

	return sdkResult;
}

int ConfigControl::removeAllAuthOperatorLevelEx(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_RemoveAllAuthOperatorLevelEx(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_RemoveAllAuthOperatorLevelEx call failed: %d", sdkResult);

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
	TRACE("   |--CUSTOM_DESFIRE_EV1 : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_CUSTOM_DESFIRE_EV1) == CARD_OPERATION_MASK_CUSTOM_DESFIRE_EV1);
	TRACE("   |--CUSTOM_CLASSIC_PLUS : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_CUSTOM_CLASSIC_PLUS) == CARD_OPERATION_MASK_CUSTOM_CLASSIC_PLUS);
	TRACE("   |--BLE : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_BLE) == CARD_OPERATION_MASK_BLE);
	TRACE("   |--NFC : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_NFC) == CARD_OPERATION_MASK_NFC);
	TRACE("   |--SEOS : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_SEOS) == CARD_OPERATION_MASK_SEOS);
	TRACE("   |--SR_SE : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_SR_SE) == CARD_OPERATION_MASK_SR_SE);
	TRACE("   |--DESFIRE_EV1 : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_DESFIRE_EV1) == CARD_OPERATION_MASK_DESFIRE_EV1);
	TRACE("   |--CLASSIC_PLUS : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_CLASSIC_PLUS) == CARD_OPERATION_MASK_CLASSIC_PLUS);
	TRACE("   |--ICLASS : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_ICLASS) == CARD_OPERATION_MASK_ICLASS);
	TRACE("   |--MIFARE_FELICA : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_MIFARE_FELICA) == CARD_OPERATION_MASK_MIFARE_FELICA);
	TRACE("   |--HIDPROX : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_HIDPROX) == CARD_OPERATION_MASK_HIDPROX);
	TRACE("   +--EM : %u", (config.useCardOperationMask & CARD_OPERATION_MASK_EM) == CARD_OPERATION_MASK_EM);
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
	TRACE("queryUserPhrase:%u", config.queryUserPhrase);

	for (int idx = 0; idx < BS2_MAX_SHORTCUT_HOME; idx++)
		TRACE("shortcutHome[%d]:%d", idx, config.shortcutHome[idx]);
	for (int idx = 0; idx < BS2_MAX_TNA_KEY; idx++)
		TRACE("tnaIcon[%d]:%d", idx, config.tnaIcon[idx]);
	TRACE("useScreenSaver:%u", config.useScreenSaver);
	TRACE("showOsdpResult:%u", config.showOsdpResult);

	TRACE("authMsgUserId:%u", config.authMsgUserId);
	TRACE("authMsgUserName:%u", config.authMsgUserName);
	TRACE("scrambleKeyboardMode:%u", config.scrambleKeyboardMode);
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

void ConfigControl::print(const BS2StatusConfig& config)
{
	TRACE("==[BS2StatusConfig]==");
	for (uint32_t idx = 0; idx < BS2_DEVICE_STATUS_NUM; idx++)
	{
		TRACE("LED Index: %u, enabled: %u, count: %u", idx, config.led[idx].enabled, config.led[idx].count);
		for (uint32_t sigidx = 0; sigidx < BS2_LED_SIGNAL_NUM; sigidx++)
			TRACE(" -- Signal %u [color: %u, duration: %u, delay: %u]",
				sigidx,
				config.led[idx].signal[sigidx].color,
				config.led[idx].signal[sigidx].duration,
				config.led[idx].signal[sigidx].delay);
	}
	
	for (uint32_t idx = 0; idx < BS2_DEVICE_STATUS_NUM; idx++)
	{
		TRACE("Buzzer Index: %u, enabled: %u, count: %u", idx, config.buzzer[idx].enabled, config.buzzer[idx].count);
		for (uint32_t sigidx = 0; sigidx < BS2_BUZZER_SIGNAL_NUM; sigidx++)
			TRACE(" -- Signal %u [tone: %u, fadeOut: %u, duration: %u, delay: %u]",
				sigidx,
				config.buzzer[idx].signal[sigidx].tone,
				config.buzzer[idx].signal[sigidx].fadeout,
				config.buzzer[idx].signal[sigidx].duration,
				config.buzzer[idx].signal[sigidx].delay);
	}
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

	// FSF2 supported
	TRACE("operationMode:%u", config.operationMode);
	TRACE("maxRotation:%u", config.maxRotation);
	TRACE("faceWidth.min:%u, faceWidth.max:%u", config.faceWidth.min, config.faceWidth.max);
	TRACE("searchRange.x:%u, searchRange.width:%u", config.searchRange.x, config.searchRange.width);

	// BS3 supported
	TRACE("detectDistance.min:%u, detectDistance.max:%u", config.detectDistance.min, config.detectDistance.max);
	TRACE("wideSearch:%u", config.wideSearch);

	TRACE("unableToSaveImageOfVisualFace:%u", config.unableToSaveImageOfVisualFace);
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

void ConfigControl::print(const BS2AuthConfigExt& config)
{
	TRACE("==[BS2AuthConfigExt]==");
	TRACE("+--extAuthSchedule");
	TRACE("   +--Face : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FACE_ONLY]);
	TRACE("   |--Face + Fingerprint : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FACE_FINGERPRINT]);
	TRACE("   |--Face + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FACE_PIN]);
	TRACE("   |--Face + Fingerprint/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FACE_FINGERPRINT_OR_PIN]);
	TRACE("   |--Face + Fingerprint + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FACE_FINGERPRINT_PIN]);
	TRACE("   +--Fingerprint : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FINGERPRINT_ONLY]);
	TRACE("   |--Fingerprint + Face : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FINGERPRINT_FACE]);
	TRACE("   |--Fingerprint + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FINGERPRINT_PIN]);
	TRACE("   |--Fingerprint + Face/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FINGERPRINT_FACE_OR_PIN]);
	TRACE("   |--Fingerprint + Face + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_FINGERPRINT_FACE_PIN]);
	TRACE("   +--Card : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_ONLY]);
	TRACE("   |--Card + Face : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FACE]);
	TRACE("   |--Card + Fingerprint : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FINGERPRINT]);
	TRACE("   |--Card + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_PIN]);
	TRACE("   |--Card + Face/Fingerprint : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FACE_OR_FINGERPRINT]);
	TRACE("   |--Card + Face/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FACE_OR_PIN]);
	TRACE("   |--Card + Fingerprint/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_OR_PIN]);
	TRACE("   |--Card + Face/Fingerprint/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FACE_OR_FINGERPRINT_OR_PIN]);
	TRACE("   |--Card + Face + Fingerprint : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FACE_FINGERPRINT]);
	TRACE("   |--Card + Face + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FACE_PIN]);
	TRACE("   |--Card + Fingerprint + Face : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_FACE]);
	TRACE("   |--Card + Fingerprint + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_PIN]);
	TRACE("   |--Card + Face/Fingerprint + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FACE_OR_FINGERPRINT_PIN]);
	TRACE("   |--Card + Face + Fingerprint/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FACE_FINGERPRINT_OR_PIN]);
	TRACE("   |--Card + Fingerprint + Face/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_FACE_OR_PIN]);
	TRACE("   +--ID + Face : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FACE]);
	TRACE("   |--ID + Fingerprint : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FINGERPRINT]);
	TRACE("   |--ID + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_PIN]);
	TRACE("   |--ID + Face/Fingerprint : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FACE_OR_FINGERPRINT]);
	TRACE("   |--ID + Face/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FACE_OR_PIN]);
	TRACE("   |--ID + Fingerprint/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FINGERPRINT_OR_PIN]);
	TRACE("   |--ID + Face/Fingerprint/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FACE_OR_FINGERPRINT_OR_PIN]);
	TRACE("   |--ID + Face + Fingerprint : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FACE_FINGERPRINT]);
	TRACE("   |--ID + Face + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FACE_PIN]);
	TRACE("   |--ID + Fingerprint + Face : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FINGERPRINT_FACE]);
	TRACE("   |--ID + Fingerprint + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FINGERPRINT_PIN]);
	TRACE("   |--ID + Face/Fingerprint + PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FACE_OR_FINGERPRINT_PIN]);
	TRACE("   |--ID + Face + Fingerprint/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FACE_FINGERPRINT_OR_PIN]);
	TRACE("   |--ID + Fingerprint + Face/PIN : %u", config.extAuthSchedule[BS2_EXT_AUTH_MODE_ID_FINGERPRINT_FACE_OR_PIN]);
	TRACE("+--useGlobalAPB : %u", config.useGlobalAPB);
	TRACE("|--globalAPBFailAction : %u", config.globalAPBFailAction);
	TRACE("|--useGroupMatching : %u", config.useGroupMatching);
	TRACE("|--usePrivateAuth : %u", config.usePrivateAuth);
	TRACE("|--faceDetectionLevel : %u", config.faceDetectionLevel);
	TRACE("|--useServerMatching : %u", config.useServerMatching);
	TRACE("|--useFullAccess : %u", config.useFullAccess);
	TRACE("|--matchTimeout : %u", config.matchTimeout);
	TRACE("|--authTimeout : %u", config.authTimeout);
	TRACE("+--numOperators : %u", config.numOperators);
}

void ConfigControl::print(const BS2FaceConfigExt& config)
{
	TRACE("==[BS2FaceConfigExt]==");
	TRACE("+--thermalCheckMode : %u", config.thermalCheckMode);
	TRACE("|--maskCheckMode : %u", config.maskCheckMode);
	TRACE("|--thermalFormat : %u", config.thermalFormat);
#if VER_272_OR_HIGHER
	float temperLow = (float)config.thermalThresholdLow / (float)100.0;
	float temperHigh = (float)config.thermalThresholdHigh / (float)100.0;
	TRACE("|--thermalThresholdLow : %.2f", temperLow);
	TRACE("|--thermalThresholdHigh : %.2f", temperHigh);
#else
	float temper = (float)config.thermalThreshold / (float)100.0;
	TRACE("|--thermalThreshold : %.2f", temper);
#endif
	TRACE("|--maskDetectionLevel : %u", config.maskDetectionLevel);
	TRACE("|--auditTemperature : %u", config.auditTemperature);
	TRACE("|--useRejectSound : %u", config.useRejectSound);
	TRACE("|--useOverlapThermal : %u", config.useOverlapThermal);
#if VER_272_OR_HIGHER
	TRACE("|--useDynamicROI : %u", config.useDynamicROI);
#endif
	TRACE("+--faceCheckOrder : %u", config.faceCheckOrder);
}

void ConfigControl::print(const BS2ThermalCameraConfig& config)
{
	TRACE("==[BS2ThermalCameraConfig]==");
	TRACE("+--distance : %u", config.distance);
	TRACE("|--emissionRate : %u", config.emissionRate);
	TRACE("+--roi");
	TRACE("   |--x : %u", config.roi.x);
	TRACE("   |--y : %u", config.roi.y);
	TRACE("   |--width : %u", config.roi.width);
	TRACE("   +--height : %u", config.roi.height);
	TRACE("+--useBodyCompensation : %u", config.useBodyCompensation);
	TRACE("+--compensationTemperature : %d", config.compensationTemperature);
}

void ConfigControl::print(const BS2EventConfig& config)
{
	TRACE("==[BS2EventConfig]==");
	TRACE("+--numImageEventFilter : %u", config.numImageEventFilter);
	for (uint32_t idx = 0; idx < config.numImageEventFilter; idx++)
	{
		TRACE("+--imageEventFilter");
		TRACE("   |--mainEventCode : %u", config.imageEventFilter[idx].mainEventCode);
		TRACE("   +--scheduleID : %u", config.imageEventFilter[idx].scheduleID);
	}
}

void ConfigControl::print(const BS2InputConfig& config)
{
	TRACE("==[BS2InputConfig]==");
	TRACE("+--numInputs : %u", config.numInputs);
	TRACE("+--Aux");
	TRACE("   |--Aux index of Tamper : %s", config.aux.field.tamperAuxIndex == BS2_INPUT_AUX0 ? "Aux0" : "Aux1");
	TRACE("   |--Aux index of ACFail : %s", config.aux.field.acFailAuxIndex == BS2_INPUT_AUX0 ? "Aux0" : "Aux1");
	TRACE("   |--Aux0 Type : %s", config.aux.field.aux0Type == BS2_INPUT_AUXTYPENO ? "NO" : "NC");
	TRACE("   |--Aux1 Type : %s", config.aux.field.aux1Type == BS2_INPUT_AUXTYPENO ? "NO" : "NC");
	TRACE("|--numSupervised : %u", config.numSupervised);

	for (uint8_t idx = 0; idx < config.numSupervised; idx++)
	{
		TRACE("+--Port(%u)", idx);
		TRACE("   |--portIndex : %u", config.supervised_inputs[idx].portIndex);
		TRACE("   |--enabled : %u", config.supervised_inputs[idx].enabled);
		TRACE("   |--supervised_index : %u", config.supervised_inputs[idx].supervised_index);
		print(config.supervised_inputs[idx].config);
	}
}

void ConfigControl::print(const BS2SupervisedInputConfig& config)
{
	TRACE("   +--[BS2SupervisedInputConfig]");
	print(config.shortInput);
	print(config.openInput);
	print(config.onInput);
	print(config.offInput);
}

void ConfigControl::print(const BS2SVInputRange& inputRange)
{
	TRACE("      +--[BS2SVInputRange]");
	TRACE("         |--minValue : %u", inputRange.minValue);
	TRACE("         +--maxValue : %u", inputRange.maxValue);
}

void ConfigControl::print(const BS2TriggerActionConfig& config)
{
	TRACE("==[BS2TriggerActionConfig]==");
	for (int idx = 0; idx < config.numItems; idx++)
	{
		TRACE("+--BS2TriggerAction[%d]", idx);
		print(config.items[idx].trigger);
		print(config.items[idx].action);
	}
}

void ConfigControl::print(const BS2Trigger& trigger)
{
	TRACE("==[BS2Trigger]==");
	TRACE("   +--device : %u", trigger.deviceID);
	TRACE("   |--type : %s", kTRIGGER_TYPE.at(trigger.type).c_str());
	TRACE("   |--ignoreSignalTime : %u", trigger.ignoreSignalTime);
	switch (trigger.type)
	{
	case BS2_TRIGGER_EVENT:
		TRACE("   +--code : %u", trigger.event.code);
		break;
	case BS2_TRIGGER_INPUT:
		TRACE("   |--port : %u", trigger.input.port);
		TRACE("   |--switchType : %u", trigger.input.switchType);
		TRACE("   |--duration : %u", trigger.input.duration);
		TRACE("   +--scheduleID : %u", trigger.input.scheduleID);
		break;
	case BS2_TRIGGER_SCHEDULE:
		TRACE("   |--scheduleType : %u", trigger.schedule.type);
		TRACE("   +--scheduleID : %u", trigger.schedule.scheduleID);
		break;
	case BS2_TRIGGER_NONE:
		break;
	default:
		break;
	}
}

void ConfigControl::print(const BS2Action& action)
{
	TRACE("==[BS2Action]==");
	TRACE("   +--device : %u", action.deviceID);
	TRACE("   |--type : %s", kACTION_TYPE.at(action.type).c_str());
	TRACE("   |--stopFlag : %u", action.stopFlag);
	TRACE("   |--delay : %u", action.delay);
	switch (action.type)
	{
	case BS2_ACTION_RELAY:
		TRACE("   |--relayIndex : %u", action.relay.relayIndex);
		TRACE("   |--signalID : %u", action.relay.signal.signalID);
		TRACE("   |--count : %u", action.relay.signal.count);
		TRACE("   |--onDuration : %u", action.relay.signal.onDuration);
		TRACE("   |--offDuration : %u", action.relay.signal.offDuration);
		TRACE("   +--delay : %u", action.relay.signal.delay);
		break;
	case BS2_ACTION_TTL:
		TRACE("   |--relayIndex : %u", action.outputPort.portIndex);
		TRACE("   |--signalID : %u", action.outputPort.signal.signalID);
		TRACE("   |--count : %u", action.outputPort.signal.count);
		TRACE("   |--onDuration : %u", action.outputPort.signal.onDuration);
		TRACE("   |--offDuration : %u", action.outputPort.signal.offDuration);
		TRACE("   +--delay : %u", action.outputPort.signal.delay);
		break;
	case BS2_ACTION_DISPLAY:
		TRACE("   |--duration : %u", action.display.duration);
		TRACE("   |--displayID : %u", action.display.displayID);
		TRACE("   +--resourceID : %u", action.display.resourceID);
		break;
	case BS2_ACTION_SOUND:
		TRACE("   |--count : %u", action.sound.count);
		TRACE("   |--soundIndex : %u", action.sound.soundIndex);
		TRACE("   +--delay : %u", action.sound.delay);
		break;
	case BS2_ACTION_LED:
		TRACE("   |--count : %u", action.led.count);
		for (int idx = 0; idx < BS2_LED_SIGNAL_NUM; idx++)
		{
			TRACE("   |--led[%u].color : %u", idx, action.led.signal[idx].color);
			TRACE("   |--led[%u].duration : %u", idx, action.led.signal[idx].duration);
			TRACE("   +--led[%u].delay : %u", idx, action.led.signal[idx].delay);
		}
		break;
	case BS2_ACTION_BUZZER:
		TRACE("   |--count : %u", action.buzzer.count);
		for (int idx = 0; idx < BS2_BUZZER_SIGNAL_NUM; idx++)
		{
			TRACE("   |--buzzer[%u].tone : %u", idx, action.buzzer.signal[idx].tone);
			TRACE("   |--buzzer[%u].fadeout : %u", idx, action.buzzer.signal[idx].fadeout);
			TRACE("   |--buzzer[%u].duration : %u", idx, action.buzzer.signal[idx].duration);
			TRACE("   +--buzzer[%u].delay : %u", idx, action.buzzer.signal[idx].delay);
		}
		break;
	case BS2_ACTION_LIFT:
		TRACE("   |--liftID : %u", action.lift.liftID);
		TRACE("   +--type : %u", action.lift.type);
		break;
	case BS2_ACTION_UNLOCK_DEVICE:
	case BS2_ACTION_REBOOT_DEVICE:
	case BS2_ACTION_LOCK_DEVICE:
	case BS2_ACTION_RELEASE_ALARM:
	case BS2_ACTION_GENERAL_INPUT:
	case BS2_ACTION_FIRE_ALARM_INPUT:
	case BS2_ACTION_AUTH_SUCCESS:
	case BS2_ACTION_AUTH_FAIL:
		break;
	case BS2_ACTION_NONE:
		break;
	default:
		break;
	}
}

void ConfigControl::print(const BS2BarcodeConfig& config)
{
	TRACE("==[BS2BarcodeConfig]==");
	TRACE("+--useBarcode : %u", config.useBarcode);
	TRACE("+--scanTimeout : %u", config.scanTimeout);
	TRACE("+--bypassData : %u", config.bypassData);
	TRACE("+--treatAsCSN : %u", config.treatAsCSN);
	TRACE("+--useVisualBarcode : %u", config.useVisualBarcode);
	TRACE("+--motionSensitivity : %u", config.motionSensitivity);
	TRACE("+--visualCameraScanTimeout : %u", config.visualCameraScanTimeout);
}


void ConfigControl::print(const BS2Rs485Config& config)
{
	TRACE("==[BS2Rs485Config]==");
	TRACE("+--mode : %u", config.mode);
	TRACE("|--numOfChannels : %u", config.numOfChannels);
	for (int index = 0; index < config.numOfChannels; index++)
	{
		TRACE("+--channels[%u]", index);
		print(config.channels[index]);
	}
	TRACE("+--intelligentInfo");
	TRACE("|  |--supportConfig : %u", config.intelligentInfo.supportConfig);
	TRACE("|  |--useExceptionCode : %u", config.intelligentInfo.useExceptionCode);

	string temp((char*)config.intelligentInfo.exceptionCode, sizeof(config.intelligentInfo.exceptionCode));
	TRACE("|  |--exceptionCode : 0x%s", Utility::convertHexByte2String(temp).c_str());
	TRACE("|  |--outputFormat : %u", config.intelligentInfo.outputFormat);
	TRACE("+  +--osdpID : %u", config.intelligentInfo.osdpID);
}

void ConfigControl::print(const BS2Rs485Channel& channel)
{
	TRACE("|  |--baudRate : %u", channel.baudRate);
	TRACE("|  |--channelIndex : %u", channel.channelIndex);
	TRACE("|  |--useRegistance : %u", channel.useRegistance);
	TRACE("|  |--numOfDevices : %u", channel.numOfDevices);
	for (int index = 0; index < channel.numOfDevices; index++)
	{
		TRACE("|  +--slaveDevices[%u]", index);
		print(channel.slaveDevices[index]);
	}
}

void ConfigControl::print(const BS2Rs485SlaveDevice& device)
{
	TRACE("|  |  |--deviceID : %u", device.deviceID);
	TRACE("|  |  |--deviceType : %u", device.deviceType);
	TRACE("|  |  |--enableOSDP : %u", device.enableOSDP);
	TRACE("|  |  |--connected : %u", device.connected);
}

void ConfigControl::printRS485Status(const BS2Rs485Config& config)
{
	for (uint32_t cidx = 0; cidx < config.numOfChannels; cidx++)
	{
		for (uint32_t didx = 0; didx < config.channels[cidx].numOfDevices; didx++)
		{
			TRACE("-- Channel: %u, Mode: %u, Baudrate: %u, Device: %u, Connected: %u, Enable: %u",
				cidx,
				config.mode,
				config.channels[cidx].baudRate,
				config.channels[cidx].slaveDevices[didx].deviceID,
				config.channels[cidx].slaveDevices[didx].connected,
				config.channels[cidx].slaveDevices[didx].enableOSDP);
		}
	}
}

void ConfigControl::printRS485Status(const BS2Rs485ConfigEX& config)
{
	for (uint32_t cidx = 0; cidx < config.numOfChannels; cidx++)
	{
		for (uint32_t didx = 0; didx < config.channels[cidx].numOfDevices; didx++)
		{
			TRACE("-- Channel: %u, Mode: %u, Baudrate: %u, ChannelType: %u, Device: %u, Connected: %u, Enable: %u",
				cidx,
				config.mode[cidx],
				config.channels[cidx].baudRate,
				config.channels[cidx].channelType,
				config.channels[cidx].slaveDevices[didx].deviceID,
				config.channels[cidx].slaveDevices[didx].connected,
				config.channels[cidx].slaveDevices[didx].enableOSDP);
		}
	}
}

void ConfigControl::print(const BS2InputConfigEx& config)
{
	TRACE("==[BS2InputConfigEx]==");
	TRACE("+--numInputs : %u", config.numInputs);
	TRACE("|--numSupervised : %u", config.numSupervised);

	for (uint8_t idx = 0; idx < config.numSupervised; idx++)
	{
		TRACE("+--inputs (%u)", idx);
		TRACE("   |--portIndex : %u", config.inputs[idx].portIndex);
		TRACE("   |--switchType : %u", config.inputs[idx].switchType);
		TRACE("   |--duration : %u", config.inputs[idx].duration);
		TRACE("   |--supervisedResistor : %u", config.inputs[idx].supervisedResistor);
	}
}

void ConfigControl::print(const BS2RelayActionConfig& config)
{
	TRACE("==[BS2RelayActionConfig]==");
	TRACE("+--deviceID : %u", config.deviceID);

	for (uint8_t idxRelay = 0; idxRelay < BS2_MAX_RELAY_ACTION; idxRelay++)
	{
		TRACE("+--relay (%u)", idxRelay);
		TRACE("   |--port : %u", config.relay[idxRelay].port);
		TRACE("   |--disconnEnabled : %u", config.relay[idxRelay].disconnEnabled);

		for (uint8_t idxInput = 0; idxInput < BS2_MAX_RELAY_ACTION_INPUT; idxInput++)
		{
			TRACE("   |--input(%u) -> port: %u, type: %u, mask: %u",
				idxInput,
				config.relay[idxRelay].input[idxInput].port,
				config.relay[idxRelay].input[idxInput].type,
				config.relay[idxRelay].input[idxInput].mask);
		}
	}
}

void ConfigControl::print(const BS2WlanConfig& config)
{
	TRACE("==[BS2WlanConfig]==");
	TRACE("+--enabled : %u", config.enabled);
	TRACE("+--operationMode : %u", config.operationMode);
	TRACE("+--authType : %u", config.authType);
	TRACE("+--encryptionType : %u", config.encryptionType);
	TRACE("+--essid : %s", config.essid);
	TRACE("+--authKey : %s", config.authKey);
}

void ConfigControl::print(const std::vector<BS2AuthOperatorLevel>& list)
{
	TRACE("==[BS2AuthOperatorLevel]==");
	for (uint32_t idx = 0; idx < list.size(); idx++)
	{
		ostringstream str;
		str << "userID: " << list[idx].userID;
		switch (list[idx].level)
		{
		case BS2_OPERATOR_LEVEL_ADMIN:
			str << ", level: Admin";
			break;
		case BS2_OPERATOR_LEVEL_CONFIG:
			str << ", level: Config-Operator";
			break;
		case BS2_OPERATOR_LEVEL_USER:
			str << ", level: User-Operator";
			break;
		}

		TRACE("[%d] %s", idx, str.str().c_str());
	}
}

void ConfigControl::print(const BS2AuthOperatorLevel& opr)
{
	TRACE("==[BS2AuthOperatorLevel]==");
	TRACE("userID : %s", opr.userID);
	string strLevel;
	switch (opr.level)
	{
	case BS2_OPERATOR_LEVEL_ADMIN:
		strLevel = "Admin";
		break;
	case BS2_OPERATOR_LEVEL_CONFIG:
		strLevel = "Config-Operator";
		break;
	case BS2_OPERATOR_LEVEL_USER:
		strLevel = "User-Operator";
		break;
	}
	TRACE("level : %s", strLevel.c_str());
}

void ConfigControl::printCard(const BS2MifareCard& card)
{
	TRACE("+--mifare.primaryKey : %s", Utility::getHexaString(card.primaryKey, sizeof(card.primaryKey)).c_str());
	TRACE("+--mifare.secondaryKey : %s", Utility::getHexaString(card.secondaryKey, sizeof(card.secondaryKey)).c_str());
	TRACE("+--mifare.startBlockIndex : %u", card.startBlockIndex);
}

void ConfigControl::printCard(const BS2IClassCard& card)
{
	TRACE("+--iclass.primaryKey : %s", Utility::getHexaString(card.primaryKey, sizeof(card.primaryKey)).c_str());
	TRACE("+--iclass.secondaryKey : %s", Utility::getHexaString(card.secondaryKey, sizeof(card.secondaryKey)).c_str());
	TRACE("+--iclass.startBlockIndex : %u", card.startBlockIndex);
}

void ConfigControl::printCard(const BS2DesFireCard& card)
{
	TRACE("+--desfire.primaryKey : %s", Utility::getHexaString(card.primaryKey, sizeof(card.primaryKey)).c_str());
	TRACE("+--desfire.secondaryKey : %s", Utility::getHexaString(card.secondaryKey, sizeof(card.secondaryKey)).c_str());
	TRACE("+--desfire.appID : %s", Utility::getHexaString(card.appID, sizeof(card.appID)).c_str());
	TRACE("+--desfire.fileID : %u", card.fileID);
	TRACE("+--desfire.encryptionType : %u", card.encryptionType);
	TRACE("+--desfire.operationMode : %u", card.operationMode);
}

void ConfigControl::print(const BS2CardConfig& config)
{
	TRACE("==[BS2CardConfig]==");
	TRACE("+--byteOrder : %u", config.byteOrder);
	TRACE("|--useWiegandFormat : %u", config.useWiegandFormat);
	TRACE("|--dataType : %u", config.dataType);
	TRACE("|--useSecondaryKey : %u", config.useSecondaryKey);
	printCard(config.mifare);
	printCard(config.iclass);
	printCard(config.desfire);
	TRACE("|--formatID : %u", config.formatID);
	TRACE("|--cipher : %u", config.cipher);
	TRACE("|--formatID : %u", config.formatID);
	TRACE("|--smartCardByteOrder : %u", config.smartCardByteOrder);
}

void ConfigControl::print(const BS2WiegandConfig& config)
{
	TRACE("==[BS2WiegandConfig]==");
	TRACE("+--mode : %u", config.mode);
	TRACE("|--useWiegandBypass : %u", config.useWiegandBypass);
	TRACE("|--useFailCode : %u", config.useFailCode);
	TRACE("|--failCode : %u", config.failCode);
	TRACE("|--outPulseWidth : %u", config.outPulseWidth);
	TRACE("|--outPulseInterval : %u", config.outPulseInterval);
	TRACE("|--formatID : %u", config.formatID);
	printWiegandFormat(config.format);
	TRACE("|--wiegandInputMask : %u", config.wiegandInputMask);
	TRACE("|--wiegandCardMask : %u", config.wiegandCardMask);
	TRACE("|--wiegandCSNIndex : %u", config.wiegandCSNIndex);
	TRACE("+--useWiegandUserID : %u", config.useWiegandUserID);
}

void ConfigControl::printWiegandFormat(const BS2WiegandFormat& format)
{
	TRACE("|--format");
	TRACE("+--+--length : %u", format.length);
	for (uint32_t idx = 0; idx < BS2_WIEGAND_MAX_FIELDS; idx++)
		TRACE("   |--idFields[%u] : %s", idx, Utility::convertArrayToString<uint8_t>(format.idFields[idx], BS2_WIEGAND_FIELD_SIZE, ',').c_str());
	for (uint32_t idx = 0; idx < BS2_WIEGAND_MAX_FIELDS; idx++)
		TRACE("   |--parityFields[%u] : %s", idx, Utility::convertArrayToString<uint8_t>(format.parityFields[idx], BS2_WIEGAND_FIELD_SIZE, ',').c_str());
	TRACE("   |--parityType : %s", Utility::convertArrayToString<BS2_WIEGAND_PARITY>(format.parityType, BS2_WIEGAND_MAX_PARITIES, ',').c_str());
	TRACE("   |--parityPos : %s", Utility::convertArrayToString<uint8_t>(format.parityPos, BS2_WIEGAND_MAX_PARITIES, ',').c_str());
}

void ConfigControl::printExtPhoneNumber(const BS2ExtensionNumber& extNumber)
{
	TRACE("  +--phoneNumber : %s", extNumber.phoneNumber);
	TRACE("  +--description : %s", extNumber.description);
}

void ConfigControl::print(const BS2VoipConfigExt& config)
{
	TRACE("==[BS2VoipConfigExt]==");
	TRACE("+--enabled : %u", config.enabled);
	TRACE("+--registrationDuration : %u", config.registrationDuration);
	TRACE("+--address : %s", config.address);
	TRACE("+--transport : %u", config.transport);
	TRACE("+--port : %u", config.port);
	TRACE("+--speaker : %u", config.volume.speaker);
	TRACE("+--mic : %u", config.volume.mic);
	TRACE("+--id : %s", config.id);
	TRACE("+--password : %s", config.password);
	TRACE("+--authorizationCode : %s", config.authorizationCode);
	TRACE("+--useOutboundProxy : %u", config.useOutboundProxy);
	if (config.useOutboundProxy) {
		TRACE("+--outboundProxy");
		TRACE("   +--address : %s", config.outboundProxy.address);
		TRACE("   +--port : %u", config.outboundProxy.port);
	}
	TRACE("+--exitButton : %c", config.exitButton);
	TRACE("+--showExtensionNumber : %u", config.showExtensionNumber);
	TRACE("+--numPhoneBook : %u", config.numPhoneBook);
	for (int idx = 0; idx < config.numPhoneBook; idx++)
	{
		TRACE("+--phonebook[%d]", idx);
		printExtPhoneNumber(config.phonebook[idx]);
	}
	TRACE("+--resolution : %u", config.resolution);
}

void ConfigControl::print(const BS2RtspConfig& config)
{
	TRACE("==[BS2RtspConfig]==");
	TRACE("+--id : %s", config.id);
	TRACE("+--password : %s", config.password);
	TRACE("+--address : %s", config.address);
	TRACE("+--port : %u", config.port);
	TRACE("+--enabled : %u", config.enabled);
	TRACE("+--resolution : %u", config.resolution);
}

void ConfigControl::print(const BS2License& license)
{

	TRACE("  +--index : %u",         license.index);
	TRACE("  +--hasCapability : %u", license.hasCapability);
	TRACE("  +--enable : %u",        license.enable);
	TRACE("  +--licenseType : %u",   license.licenseType);
	TRACE("  +--licenseSubType : %u",license.licenseSubType);
	TRACE("  +--enableTime : %u",    license.enableTime);
	TRACE("  +--expiredTime : %u",   license.expiredTime);
	TRACE("  +--issueNumber : %u",   license.issueNumber);
	TRACE("  +--name : %s",          license.name);
}

void ConfigControl::print(const BS2LicenseConfig& config)
{
	TRACE("==[BS2LicenseConfig]==");
	TRACE("+--version : %u",		config.version);
	TRACE("+--numOfLicense : %u",	config.numOfLicense);
	for (uint32_t idx = 0; idx <	config.numOfLicense; idx++)
	{
		TRACE("+--license[%u]",		idx);
		print(config.license[idx]);
    }
}

void ConfigControl::print(const BS2OsdpStandardConfig& config)
{
	TRACE("==[BS2OsdpStandardConfig]==");
	TRACE("+--numOfChannels : %u", config.numOfChannels);

	for (int idx = 0; idx < config.numOfChannels; idx++)
	{
		TRACE("+--channel[%d] - mode : %u", idx, config.mode[idx]);
		TRACE("   |--baudRate : %u", config.channels[idx].baudRate);
		TRACE("   |--channelIndex : %u", config.channels[idx].channelIndex);
		TRACE("   |--useRegistance : %u", config.channels[idx].useRegistance);
		TRACE("   |--numOfDevices : %u", config.channels[idx].numOfDevices);
		TRACE("   |--channelType : %u", config.channels[idx].channelType);

		for (int sidx = 0; sidx < config.channels[idx].numOfDevices; sidx++)
		{
			TRACE("  +--slaveDevice[%d]", sidx);
			TRACE("     |--deviceID : %u", config.channels[idx].slaveDevices[sidx].deviceID);
			TRACE("     |--deviceType : %u", config.channels[idx].slaveDevices[sidx].deviceType);
			TRACE("     |--enableOSDP : %u", config.channels[idx].slaveDevices[sidx].enableOSDP);
			TRACE("     |--connected : %u", config.channels[idx].slaveDevices[sidx].connected);
			TRACE("     |--channelInfo : %u", config.channels[idx].slaveDevices[sidx].channelInfo);
			TRACE("     |--osdpID : %u", config.channels[idx].slaveDevices[sidx].osdpID);
			TRACE("     |--supremaSearch : %u", config.channels[idx].slaveDevices[sidx].supremaSearch);
			TRACE("     |--activate : %u", config.channels[idx].slaveDevices[sidx].activate);
			TRACE("     |--useSecure : %u", config.channels[idx].slaveDevices[sidx].useSecure);
			TRACE("     |--vendorCode : %u", Utility::getHexaString(config.channels[idx].slaveDevices[sidx].vendorCode, sizeof(config.channels[idx].slaveDevices[sidx].vendorCode)));
			TRACE("     |--fwVersion : %u", config.channels[idx].slaveDevices[sidx].fwVersion);
			TRACE("     |--modelNumber : %u", config.channels[idx].slaveDevices[sidx].modelNumber);
			TRACE("     |--modelVersion : %u", config.channels[idx].slaveDevices[sidx].modelVersion);
			TRACE("     |--readInfo : %u", config.channels[idx].slaveDevices[sidx].readInfo);
		}
	}
}

void ConfigControl::print(const BS2OsdpStandardActionConfig& config)
{
	TRACE("==[BS2OsdpStandardActionConfig]==");
	TRACE("+--version : %u", config.version);

	for (int idx = 0; idx < BS2_OSDP_STANDARD_ACTION_TYPE_COUNT; idx++)
	{
		TRACE("+--actions[%u]", idx);
		TRACE("    |--type : %u", config.actions[idx].actionType);
		for (int ledidx = 0; ledidx < BS2_OSDP_STANDARD_ACTION_MAX_LED; ledidx++)
		{
			TRACE("    +--led[%u]", ledidx);
			TRACE("        |--use : %u", config.actions[idx].led[ledidx].use);
			TRACE("        |--readerNumber : %u", config.actions[idx].led[ledidx].readerNumber);
			TRACE("        |--ledNumber : %u", config.actions[idx].led[ledidx].ledNumber);

			TRACE("        |--tempCommand : %u", config.actions[idx].led[ledidx].tempCommand);
			TRACE("        |--tempOnTime (*100): %u", config.actions[idx].led[ledidx].tempOnTime);
			TRACE("        |--tempOffTime (*100): %u", config.actions[idx].led[ledidx].tempOffTime);
			TRACE("        |--tempOnColor : %u", config.actions[idx].led[ledidx].tempOnColor);
			TRACE("        |--tempOffColor : %u", config.actions[idx].led[ledidx].tempOffColor);
			TRACE("        |--tempRunTime (*100): %u", config.actions[idx].led[ledidx].tempRunTime);

			TRACE("        |--permCommand : %u", config.actions[idx].led[ledidx].permCommand);
			TRACE("        |--permOnTime (*100): %u", config.actions[idx].led[ledidx].permOnTime);
			TRACE("        |--permOffTime (*100): %u", config.actions[idx].led[ledidx].permOffTime);
			TRACE("        |--permOnColor : %u", config.actions[idx].led[ledidx].permOnColor);
			TRACE("        |--permOffColor : %u", config.actions[idx].led[ledidx].permOffColor);
		}

		TRACE("    +--buzzer");
		TRACE("        |--use : %u", config.actions[idx].buzzer.use);
		TRACE("        |--readerNumber : %u", config.actions[idx].buzzer.readerNumber);
		TRACE("        |--tone : %u", config.actions[idx].buzzer.tone);
		TRACE("        |--onTime (*100): %u", config.actions[idx].buzzer.onTime);
		TRACE("        |--offTime (*100): %u", config.actions[idx].buzzer.offTime);
		TRACE("        |--numOfCycle : %u", config.actions[idx].buzzer.numOfCycle);
	}
}

void ConfigControl::print(const BS2CustomMifareCard& card)
{
	TRACE("+--mifare.primaryKey : %s", Utility::getHexaString(card.primaryKey, sizeof(card.primaryKey)).c_str());
	TRACE("+--mifare.secondaryKey : %s", Utility::getHexaString(card.secondaryKey, sizeof(card.secondaryKey)).c_str());
	TRACE("+--mifare.startBlockIndex : %u", card.startBlockIndex);
	TRACE("+--mifare.dataSize : %u", card.dataSize);
	TRACE("+--mifare.skipBytes : %u", card.skipBytes);
}

void ConfigControl::print(const BS2CustomDesFireCard& card)
{
	TRACE("+--desfire.primaryKey : %s", Utility::getHexaString(card.primaryKey, sizeof(card.primaryKey)).c_str());
	TRACE("+--desfire.secondaryKey : %s", Utility::getHexaString(card.secondaryKey, sizeof(card.secondaryKey)).c_str());
	TRACE("+--desfire.appID : %s", Utility::getHexaString(card.appID, sizeof(card.appID)).c_str());
	TRACE("+--desfire.fileID : %u", card.fileID);
	TRACE("+--desfire.encryptionType : %u", card.encryptionType);
	TRACE("+--desfire.operationMode : %u", card.operationMode);
	TRACE("+--desfire.dataSize : %u", card.dataSize);
	TRACE("+--desfire.skipBytes : %u", card.skipBytes);
	print(card.desfireAppKey);
}

void ConfigControl::print(const BS2DesFireAppLevelKey& key)
{
	TRACE("+--desfire.desfireAppKey.appMasterKey:%s", Utility::getHexaString(key.appMasterKey, 16).c_str());	// maybe 0
	TRACE("+--desfire.desfireAppKey.fileReadKey:%s", Utility::getHexaString(key.fileReadKey, 16).c_str());	// maybe 0
	TRACE("+--desfire.desfireAppKey.fileWriteKey:%s", Utility::getHexaString(key.fileWriteKey, 16).c_str());	// maybe 0
	TRACE("+--desfire.desfireAppKey.fileReadKeyNumber:%u", key.fileReadKeyNumber);
	TRACE("+--desfire.desfireAppKey.fileWriteKeyNumber:%u", key.fileWriteKeyNumber);
}

void ConfigControl::print(const BS2CustomCardConfig& config)
{
	TRACE("==[BS2CustomCardConfig]==");
	TRACE("+--dataType : %u", config.dataType);
	TRACE("|--useSecondaryKey : %u", config.useSecondaryKey);
	print(config.mifare);
	print(config.desfire);
	TRACE("|--smartCardByteOrder : %u", config.smartCardByteOrder);
	TRACE("+--formatID : %u", config.formatID);
}

uint32_t ConfigControl::printOSDPDeviceID(const BS2OsdpStandardConfig& config)
{
	uint32_t numOfActivated(0);
	for (int idx = 0; idx < config.numOfChannels; idx++)
	{
		for (int sidx = 0; sidx < config.channels[idx].numOfDevices; sidx++)
		{
			TRACE("[%d-%d] %u", idx, sidx, config.channels[idx].slaveDevices[sidx].deviceID);
			numOfActivated++;
		}
	}

	return numOfActivated;
}

bool ConfigControl::getOsdpID(const BS2OsdpStandardConfig& config, uint32_t osdpDeviceID, uint8_t& osdpID)
{
	for (uint32_t idx = 0; idx < config.numOfChannels; idx++)
	{
		for (uint32_t sidx = 0; sidx < config.channels[idx].numOfDevices; sidx++)
		{
			if (osdpDeviceID == config.channels[idx].slaveDevices[sidx].deviceID)
			{
				osdpID = config.channels[idx].slaveDevices[sidx].osdpID;
				return true;
			}
		}
	}

	return false;
}
