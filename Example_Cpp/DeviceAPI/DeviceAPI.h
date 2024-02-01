#pragma once
#include <iostream>
#include <string>
#include <vector>
#include <memory>
#include <array>
#include <Windows.h>
#include "PreDefined.h"
#include "BS_Errno.h"
#include "../Common/BS2Context.h"
#include "../Common/DeviceControl.h"
#include "../Common/ConfigControl.h"
#include "../Common/CommControl.h"
#include "../Common/DeviceList.h"


enum EN_MENU_DEV
{
	MENU_DEV_BREAK = MENU_BREAK,
	MENU_DEV_GET_DEVINF,
	MENU_DEV_GET_DEVINFEX,

	MENU_DEV_GET_DEVTIME,
	MENU_DEV_SET_DEVTIME,

	MENU_DEV_CLR_DATABASE,
	MENU_DEV_FACTORY_RESET,
	MENU_DEV_REBOOT_DEV,

	MENU_DEV_LOCK_DEV,
	MENU_DEV_UNLOCK_DEV,

	MENU_DEV_UPG_FIRMWARE,
	MENU_DEV_UPD_RESOURCE,

	MENU_DEV_GET_SPCDEVINFO,

	MENU_DEV_RST_CONFIG_EXCEPT_NETINFO,
	MENU_DEV_GET_DEVICECAPABILITIES,

	MENU_DEV_GET_FINGERPRINTCONFIG,
	MENU_DEV_SET_FINGERPRINTCONFIG,

	MENU_DEV_GET_FACECONFIG,
	MENU_DEV_SET_FACECONFIG,

	MENU_DEV_GET_DISPLAYCONFIG,
	MENU_DEV_SET_DISPLAYCONFIG,

	MENU_DEV_GET_SYSTEMCONFIG,
	MENU_DEV_SET_SYSTEMCONFIG,

	MENU_DEV_GET_DESFIRECONFIGEX,
	MENU_DEV_SET_DESFIRECONFIGEX,

	MENU_DEV_GET_AUTHCONFIGEX,
	MENU_DEV_SET_AUTHCONFIGEX,

	MENU_DEV_GET_FACECONFIGEX,
	MENU_DEV_SET_FACECONFIGEX,

	MENU_DEV_GET_IPCONFIG,
	MENU_DEV_SET_IPCONFIG,

	MENU_DEV_GET_THERMALCAMERACONFIG,
	MENU_DEV_SET_THERMALCAMERACONFIG,

	MENU_DEV_GET_EVENTCONFIG,
	MENU_DEV_SET_EVENTCONFIG,

	MENU_DEV_GET_INPUTCONFIG,

	MENU_DEV_GET_TRIGGERACTIONCONFIG,
	MENU_DEV_SET_TRIGGERACTIONCONFIG,
	MENU_DEV_REM_TRIGGERACTIONCONFIG,

	MENU_DEV_GET_BARCODECONFIG,
	MENU_DEV_SET_BARCODECONFIG,

	MENU_DEV_GET_RS485CONFIG,
	MENU_DEV_SET_RS485CONFIG,

	MENU_DEV_GET_INPUTCONFIGEX,
	MENU_DEV_SET_INPUTCONFIGEX,

	MENU_DEV_GET_RELAYACTIONCONFIG,
	MENU_DEV_SET_RELAYACTIONCONFIG,

	MENU_DEV_GET_WLANCONFIG,
	MENU_DEV_SET_WLANCONFIG,

	MENU_DEV_SET_WIEGANDMULTICONFIG,

	MENU_DEV_GET_WIEGANDCONFIG,
	MENU_DEV_SET_WIEGANDCONFIG,

	MENU_DEV_GET_VOIPCONFIGEXT,
	MENU_DEV_SET_VOIPCONFIGEXT,

	MENU_DEV_GET_RTSPCONFIG,
	MENU_DEV_SET_RTSPCONFIG,

	MENU_DEV_GET_LICENSECONFIG,

	MENU_DEV_GET_OSDPSTANDARDCONFIG,
	MENU_DEV_GET_OSDPSTANDARDACTIONCONFIG,
	MENU_DEV_SET_OSDPSTANDARDACTIONCONFIG,

	MENU_DEV_GET_CUSTOMCARDCONFIG,
	MENU_DEV_SET_CUSTOMCARDCONFIG,

	MENU_DEV_UPD_DEVICE_VOLUME,

	MENU_DEV_TURNON_QRBYPASS,
	MENU_DEV_TURNOFF_QRBYPASS,
};

std::vector<MENU_ITEM> menuInfoDeviceAPI =
{
	{MENU_DEV_BREAK,					"Exit"},
	{MENU_DEV_GET_DEVINF,				"BS2_GetDeviceInfo" },
	{MENU_DEV_GET_DEVINFEX,				"BS2_GetDeviceInfoEx" },

	{MENU_DEV_GET_DEVTIME,				"BS2_GetDeviceTime" },
	{MENU_DEV_SET_DEVTIME,				"BS2_SetDeviceTime" },

	{MENU_DEV_CLR_DATABASE,				"BS2_ClearDatabase" },
	{MENU_DEV_FACTORY_RESET,			"BS2_FactoryReset" },
	{MENU_DEV_REBOOT_DEV,				"BS2_RebootDevice" },

	{MENU_DEV_LOCK_DEV,					"BS2_LockDevice" },
	{MENU_DEV_UNLOCK_DEV,				"BS2_UnlockDevice" },

	{MENU_DEV_UPG_FIRMWARE,				"BS2_UpgradeFirmware" },
	{MENU_DEV_UPD_RESOURCE,				"BS2_UpdateResource" },

	{MENU_DEV_GET_SPCDEVINFO,			"BS2_GetSpecifiedDeviceInfo" },

	{MENU_DEV_RST_CONFIG_EXCEPT_NETINFO,"BS2_ResetConfigExceptNetInfo"},
	{MENU_DEV_GET_DEVICECAPABILITIES,	"BS2_GetDeviceCapabilities"},

	{MENU_DEV_GET_FINGERPRINTCONFIG,	"BS2_GetFingerprintConfig"},
	{MENU_DEV_SET_FINGERPRINTCONFIG,	"BS2_SetFingerprintConfig"},

	{MENU_DEV_GET_FACECONFIG,			"BS2_GetFaceConfig"},
	{MENU_DEV_SET_FACECONFIG,			"BS2_SetFaceConfig"},

	{MENU_DEV_GET_DISPLAYCONFIG,		"BS2_GetDisplayConfig"},
	{MENU_DEV_SET_DISPLAYCONFIG,		"BS2_SetDisplayConfig"},

	{MENU_DEV_GET_IPCONFIG,				"BS2_GetIpConfig"},
	{MENU_DEV_SET_IPCONFIG,				"BS2_SetIpConfig"},

	{MENU_DEV_GET_SYSTEMCONFIG,			"BS2_GetSystemConfig"},
	{MENU_DEV_SET_SYSTEMCONFIG,			"BS2_SetSystemConfig"},

	{MENU_DEV_GET_DESFIRECONFIGEX,		"BS2_GetDesFireCardConfigEx"},
	{MENU_DEV_SET_DESFIRECONFIGEX,		"BS2_SetDesFireCardConfigEx"},

	{MENU_DEV_GET_AUTHCONFIGEX,			"BS2_GetAuthConfigEx"},
	{MENU_DEV_SET_AUTHCONFIGEX,			"BS2_SetAuthConfigEx"},

	{MENU_DEV_GET_FACECONFIGEX,			"BS2_GetFaceConfigEx"},
	{MENU_DEV_SET_FACECONFIGEX,			"BS2_SetFaceConfigEx"},

	{MENU_DEV_GET_THERMALCAMERACONFIG,	"BS2_GetThermalCameraConfig"},
	{MENU_DEV_SET_THERMALCAMERACONFIG,	"BS2_SetThermalCameraConfig"},

	{MENU_DEV_GET_EVENTCONFIG,			"BS2_GetEventConfig"},
	{MENU_DEV_SET_EVENTCONFIG,			"BS2_SetEventConfig"},

	{MENU_DEV_GET_INPUTCONFIG,			"BS2_GetInputConfig"},

	{MENU_DEV_GET_TRIGGERACTIONCONFIG,	"BS2_GetTriggerActionConfig"},
	{MENU_DEV_SET_TRIGGERACTIONCONFIG,	"BS2_SetTriggerActionConfig"},
	{MENU_DEV_REM_TRIGGERACTIONCONFIG,	"Remove trigger action config"},

	{MENU_DEV_GET_BARCODECONFIG,		"BS2_GetBarcodeConfig"},
	{MENU_DEV_SET_BARCODECONFIG,		"BS2_SetBarcodeConfig"},

	{MENU_DEV_GET_RS485CONFIG,			"BS2_GetRS485Config"},
	{MENU_DEV_SET_RS485CONFIG,			"BS2_SetRS485Config"},

	{MENU_DEV_GET_INPUTCONFIGEX,		"BS2_GetInputConfigEx"},
	{MENU_DEV_SET_INPUTCONFIGEX,		"BS2_SetInputConfigEx"},

	{MENU_DEV_GET_RELAYACTIONCONFIG,	"BS2_GetRelayActionConfig"},
	{MENU_DEV_SET_RELAYACTIONCONFIG,	"BS2_SetRelayActionConfig"},

	{MENU_DEV_GET_WLANCONFIG,			"BS2_GetWlanConfig"},
	{MENU_DEV_SET_WLANCONFIG,			"BS2_SetWlanConfig"},

	{MENU_DEV_SET_WIEGANDMULTICONFIG,	"BS2_SetWiegandMultiConfig"},

	{MENU_DEV_GET_WIEGANDCONFIG,		"BS2_GetWiegandConfig"},
	{MENU_DEV_SET_WIEGANDCONFIG,		"BS2_SetWiegandConfig"},

	{MENU_DEV_GET_VOIPCONFIGEXT,		"BS2_GetVoipConfigExt"},
	{MENU_DEV_SET_VOIPCONFIGEXT,		"BS2_SetVoipConfigExt"},

	{MENU_DEV_GET_RTSPCONFIG,			"BS2_GetRtspConfig"},
	{MENU_DEV_SET_RTSPCONFIG,			"BS2_SetRtspConfig"},

	{MENU_DEV_GET_LICENSECONFIG,		"BS2_GetLicenseConfig"},

	{MENU_DEV_GET_OSDPSTANDARDCONFIG,		"BS2_GetOsdpStandardConfig"},
	{MENU_DEV_GET_OSDPSTANDARDACTIONCONFIG,	"BS2_GetOsdpStandardActionConfig"},
	{MENU_DEV_SET_OSDPSTANDARDACTIONCONFIG,	"BS2_SetOsdpStandardActionConfig"},

	{MENU_DEV_GET_CUSTOMCARDCONFIG,		"BS2_GetCustomCardConfig"},
	{MENU_DEV_SET_CUSTOMCARDCONFIG,		"BS2_SetCustomCardConfig"},

	{MENU_DEV_UPD_DEVICE_VOLUME,		"Update device volume"},

	{MENU_DEV_TURNON_QRBYPASS,			"Turn On QR Bypass"},
	{MENU_DEV_TURNOFF_QRBYPASS,			"Turn Off QR Bypass"},
};


void connectTestDevice(void* context);
int runAPIs(void* context, const DeviceInfo& device);
int getDeviceInfo(void* context, const DeviceInfo& device);
int getDeviceInfoEx(void* context, const DeviceInfo& device);
int getFingerprintConfig(void* context, const DeviceInfo& device);
int setFingerprintConfig(void* context, const DeviceInfo& device);
int getFaceConfig(void* context, const DeviceInfo& device);
int setFaceConfig(void* context, const DeviceInfo& device);
int getDisplayConfig(void* context, const DeviceInfo& device);
int setDisplayConfig(void* context, const DeviceInfo& device);
int getIPConfig(void* context, const DeviceInfo& device);
int setIPConfig(void* context, const DeviceInfo& device);
int getSystemConfig(void* context, const DeviceInfo& device);
int setSystemConfig(void* context, const DeviceInfo& device);
int getDesFireCardConfigEx(void* context, const DeviceInfo& device);
int setDesFireCardConfigEx(void* context, const DeviceInfo& device);
int getAuthConfigEx(void* context, const DeviceInfo& device);
int setAuthConfigEx(void* context, const DeviceInfo& device);
int getFaceConfigEx(void* context, const DeviceInfo& device);
int setFaceConfigEx(void* context, const DeviceInfo& device);
int getThermalCameraConfig(void* context, const DeviceInfo& device);
int setThermalCameraConfig(void* context, const DeviceInfo& device);
int getEventConfig(void* context, const DeviceInfo& device);
int setEventConfig(void* context, const DeviceInfo& device);
int getInputConfig(void* context, const DeviceInfo& device);
int getTriggerActionConfig(void* context, const DeviceInfo& device);
int setTriggerActionConfig(void* context, const DeviceInfo& device);
int removeTriggerActionConfig(void* context, const DeviceInfo& device);
int updateDeviceVolume(void* context, const DeviceInfo& device);
int getBarcodeConfig(void* context, const DeviceInfo& device);
int setBarcodeConfig(void* context, const DeviceInfo& device);
int turnOnQRBypass(void* context, const DeviceInfo& device);
int turnOffQRBypass(void* context, const DeviceInfo& device);
int getRS485Config(void* context, const DeviceInfo& device);
int setRS485Config(void* context, const DeviceInfo& device);
int getDeviceCapabilities(void* context, const DeviceInfo& device);
int getInputConfigEx(void* context, const DeviceInfo& device);
int setInputConfigEx(void* context, const DeviceInfo& device);
int getRelayActionConfig(void* context, const DeviceInfo& device);
int setRelayActionConfig(void* context, const DeviceInfo& device);
int getWLANConfig(void* context, const DeviceInfo& device);
int setWLANConfig(void* context, const DeviceInfo& device);
int getWiegandConfig(void* context, const DeviceInfo& device);
int setWiegandConfig(void* context, const DeviceInfo& device);
uint16_t selectWiegandFormat(std::ostringstream& oss);
int setWiegandMultiConfigWithPreset(void* context, const DeviceInfo& device);
int getVoipConfigExt(void* context, const DeviceInfo& device);
int setVoipConfigExt(void* context, const DeviceInfo& device);
int getRtspConfig(void* context, const DeviceInfo& device);
int setRtspConfig(void* context, const DeviceInfo& device);
int getLicenseConfig(void* context, const DeviceInfo& device);
int getOsdpStandardConfig(void* context, const DeviceInfo& device);
int getOsdpStandardActionConfig(void* context, const DeviceInfo& device);
int setOsdpStandardActionConfig(void* context, const DeviceInfo& device);
int getCustomCardConfig(void* context, const DeviceInfo& device);
int setCustomCardConfig(void* context, const DeviceInfo& device);
void setTriggerAction(const DeviceInfo& device, BS2TriggerAction& triggerAction);
void setTrigger(const DeviceInfo& device, BS2Trigger& trigger);
void setEventTrigger(BS2EventTrigger& eventTrigger);
void setInputTrigger(BS2InputTrigger& inputTrigger);
void setScheduleTrigger(BS2ScheduleTrigger& scheduleTrigger);
void setAction(const DeviceInfo& device, BS2Action& action);
void setRelayAction(BS2RelayAction& relayAction);
void setOutputPortAction(BS2OutputPortAction& outputPortAction);
void setSoundAction(BS2SoundAction& soundAction);
void setDisplayAction(BS2DisplayAction& displayAction);
void setBuzzerAction(BS2BuzzerAction& buzzerAction);
void setLedAction(BS2LedAction& ledAction);
void setLiftAction(BS2LiftAction& liftAction);