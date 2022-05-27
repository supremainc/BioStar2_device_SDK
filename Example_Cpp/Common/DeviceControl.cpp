#include "stdafx.h"
#include <iostream>
#include <sstream>
#include <cassert>
#include <memory>
#include "DeviceControl.h"
#include "UserControl.h"
#include "BS_Errno.h"
#include "../Common/Utility.h"
#include "../Common/ConfigControl.h"

#define CAST_UINT32(x)				static_cast<uint32_t>(x)


extern void TRACE(const char* fmt, ...);
using namespace std;

void* DeviceControl::context_ = NULL;

DeviceControl::DeviceControl(void* sdkContext)
{
	context_ = sdkContext;
}


DeviceControl::~DeviceControl()
{
}

int DeviceControl::getDeviceInfo(BS2_DEVICE_ID id)
{
	BS2SimpleDeviceInfo info = { 0, };

	int sdkResult = getDeviceInfo(id, info);
	if (BS_SDK_SUCCESS == sdkResult)
		print(info);

	return sdkResult;
}

int DeviceControl::getDeviceInfo(BS2_DEVICE_ID id, BS2SimpleDeviceInfo& info)
{
	int sdkResult = BS2_GetDeviceInfo(context_, id, &info);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetDeviceInfo call failed: %d", sdkResult);

	return sdkResult;
}

int DeviceControl::getDeviceInfoEx(BS2_DEVICE_ID id)
{
	BS2SimpleDeviceInfo info = { 0, };
	BS2SimpleDeviceInfoEx infoEx = { 0, };

	int sdkResult = BS2_GetDeviceInfoEx(context_, id, &info, &infoEx);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetDeviceInfoEx call failed: %d", sdkResult);
	else
	{
		print(info);
		print(infoEx);
	}

	return sdkResult;
}

int DeviceControl::getSpecifiedDeviceInfo(BS2_DEVICE_ID id)
{
	int sdkResult = BS_SDK_SUCCESS;
	uint32_t inInfoSize(0);
	uint32_t outInfoSize(0);
	BS2SimpleDeviceInfo info = { 0, };
	BS2SimpleDeviceInfoEx infoEx = { 0, };
	BS2IPv6DeviceInfo infoV6 = { 0, };
	stringstream msg;

	msg << "Retrieve device info. What do you want?\n";
	msg << "[1 : BS2SimpleDeviceInfo, 2 : BS2SimpleDeviceInfoEx, 3 : BS2IPv6DeviceInfo]";
	uint32_t selected = Utility::getInput<uint32_t>(msg.str());
	switch (selected)
	{
	case BS2_SPECIFIED_DEVICE_INFO_SIMPLE:
		inInfoSize = sizeof(BS2SimpleDeviceInfo);
		sdkResult = BS2_GetSpecifiedDeviceInfo(context_, id, BS2_SPECIFIED_DEVICE_INFO_SIMPLE, &info, inInfoSize, &outInfoSize);
		assert(inInfoSize == outInfoSize);
		if (BS_SDK_SUCCESS == sdkResult)
			print(info);
		break;

	case BS2_SPECIFIED_DEVICE_INFO_SIMPLE_EX:
		inInfoSize = sizeof(BS2SimpleDeviceInfoEx);
		sdkResult = BS2_GetSpecifiedDeviceInfo(context_, id, BS2_SPECIFIED_DEVICE_INFO_SIMPLE_EX, &infoEx, inInfoSize, &outInfoSize);
		assert(inInfoSize == outInfoSize);
		if (BS_SDK_SUCCESS == sdkResult)
			print(infoEx);
		break;

	case BS2_SPECIFIED_DEVICE_INFO_IPV6:
		inInfoSize = sizeof(BS2IPv6DeviceInfo);
		sdkResult = BS2_GetSpecifiedDeviceInfo(context_, id, BS2_SPECIFIED_DEVICE_INFO_IPV6, &infoV6, inInfoSize, &outInfoSize);
		assert(inInfoSize == outInfoSize);
		if (BS_SDK_SUCCESS == sdkResult)
			print(infoV6);
		break;
	}

	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetSpecifiedDeviceInfo call failed: %d", sdkResult);
	}

	return sdkResult;
}

int DeviceControl::getDeviceTime(BS2_DEVICE_ID id)
{
	BS2_TIMESTAMP currTime = 0;

	int sdkResult = BS2_GetDeviceTime(context_, id, &currTime);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetDeviceTime call failed: %d", sdkResult);

	cout << "Device: " << id << ", Time: " << Utility::convertTimeUTC2String(currTime) << endl;

	return sdkResult;
}

int DeviceControl::setDeviceTime(BS2_DEVICE_ID id)
{
	string inputTime = Utility::getLine("Please enter a time [YYYY-MM-DD HH:MM:SS] ?");
	BS2_TIMESTAMP currTime = Utility::convertTimeString2UTC(inputTime);

	int sdkResult = BS2_SetDeviceTime(context_, id, currTime);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetDeviceTime call failed: %d", sdkResult);

	return sdkResult;
}

int DeviceControl::getDeviceTime(BS2_DEVICE_ID id, BS2_TIMESTAMP& currTime)
{
	int sdkResult = BS2_GetDeviceTime(context_, id, &currTime);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_GetDeviceTime call failed: %d", sdkResult);

	return sdkResult;
}

int DeviceControl::setDeviceTime(BS2_DEVICE_ID id, BS2_TIMESTAMP& currTime)
{
	int sdkResult = BS2_SetDeviceTime(context_, id, currTime);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_SetDeviceTime call failed: %d", sdkResult);

	return sdkResult;
}

int DeviceControl::clearDatabase(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_ClearDatabase(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_ConnectDeviceViaIP call failed: %d", sdkResult);

	return sdkResult;
}

int DeviceControl::factoryReset(BS2_DEVICE_ID id)
{
	{
		int sdkResult = BS2_SetDefaultResponseTimeout(context_, 1800 * 1000);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_SetDefaultResponseTimeout call failed: %d", sdkResult);
			return sdkResult;
		}

		sdkResult = BS2_SetKeepAliveTimeout(context_, 1800 * 1000);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_SetKeepAliveTimeout call failed: %d", sdkResult);
			return sdkResult;
		}
	}

	int sdkResult = BS2_FactoryReset(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_FactoryReset call failed: %d", sdkResult);
	else
		TRACE("BS2_FactoryReset call succeed: %u", id);

	return sdkResult;
}

int DeviceControl::rebootDevice(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_RebootDevice(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_RebootDevice call failed: %d", sdkResult);

	return sdkResult;
}

int DeviceControl::lockDevice(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_LockDevice(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_LockDevice call failed: %d", sdkResult);

	return sdkResult;
}

int DeviceControl::unlockDevice(BS2_DEVICE_ID id)
{
	int sdkResult = BS2_UnlockDevice(context_, id);
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_UnlockDevice call failed: %d", sdkResult);

	return sdkResult;
}

int DeviceControl::upgradeFirmware(BS2_DEVICE_ID id)
{
	string file = Utility::getLine("Enter the path and name of firmware:");

	int sdkResult = BS_SDK_SUCCESS;
	uint32_t fileLen = Utility::getResourceSize(file);
	shared_ptr<uint8_t> buffer(new uint8_t[fileLen], ArrayDeleter<uint8_t>());
	if (0 < fileLen && Utility::getResourceFromFile(file, buffer, fileLen))
	{
		sdkResult = BS2_UpgradeFirmware(context_, id, buffer.get(), fileLen, 0, onUpgrade);
		if (BS_SDK_SUCCESS != sdkResult)
			TRACE("BS2_UpgradeFirmware call failed: %d", sdkResult);
	}
	else
	{
		TRACE("The file could not be read.(%s)", file.c_str());
		return BS_SDK_ERROR_FILE_IO;
	}

	return sdkResult;
}

int DeviceControl::upgradeFirmware(const vector<BS2_DEVICE_ID>& devices)
{
	string file = Utility::getLine("Enter the path and name of firmware:");

	int sdkResult = BS_SDK_SUCCESS;
	uint32_t fileLen = Utility::getResourceSize(file);
	shared_ptr<uint8_t> buffer(new uint8_t[fileLen], ArrayDeleter<uint8_t>());
	if (0 < fileLen && Utility::getResourceFromFile(file, buffer, fileLen))
	{
		for (auto id : devices)
		{
			sdkResult = BS2_UpgradeFirmware(context_, id, buffer.get(), fileLen, 0, onUpgrade);
			if (BS_SDK_SUCCESS != sdkResult)
				TRACE("BS2_UpgradeFirmware call failed: %d", sdkResult);
		}
	}
	else
	{
		TRACE("The file could not be read.(%s)", file.c_str());
		return BS_SDK_ERROR_FILE_IO;
	}

	return sdkResult;
}

void DeviceControl::onUpgrade(BS2_DEVICE_ID id, uint32_t percent)
{
	TRACE("%u%% upgraded (id:%u)", percent, id);
}

int DeviceControl::updateResource(BS2_DEVICE_ID id)
{
	ConfigControl cc(context_);
	BS2DisplayConfig config = { 0, };

	int sdkResult = cc.getDisplayConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	config.background = BS2_BG_LOGO;
	config.bgTheme = BS2_BG_THEME_03;	// Background image or slide show
	sdkResult = cc.setDisplayConfig(id, config);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;
	
	string sampleImage = ".\\sampleImage_480x854.png";
	uint32_t fileLen = Utility::getResourceSize(sampleImage);
	shared_ptr<uint8_t> buffer(new uint8_t[fileLen], ArrayDeleter<uint8_t>());
	if (0 < fileLen && Utility::getResourceFromFile(sampleImage, buffer, fileLen))
	{
		BS2ResourceElement res = { 0, };
		res.type = BS2_RESOURCE_TYPE_IMAGE;
		res.numResData = 1;

		res.resData[0].index = 0;
		res.resData[0].data = buffer.get();
		res.resData[0].dataLen = fileLen;

		sdkResult = BS2_UpdateResource(context_, id, &res, 1, onUpgrade);
		if (BS_SDK_ERROR_NOT_SUPPORTED == sdkResult)
			sdkResult = BS2_UpdateResource(context_, id, &res, 0, onUpgrade);
	}

	return sdkResult;
}

int DeviceControl::getDeviceCapabilities(BS2_DEVICE_ID id, BS2DeviceCapabilities& cap)
{
	int sdkResult = BS2_GetDeviceCapabilities(context_, id, &cap);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetDeviceCapabilities call failed: %d", sdkResult);
		return sdkResult;
	}

	return sdkResult;
}

void DeviceControl::print(const BS2SimpleDeviceInfo& info)
{
	TRACE("==[BS2SimpleDeviceInfo]==");
	TRACE("id : %u", info.id);
	TRACE("type : %u", info.type);
	TRACE("connectionMode : %u", info.connectionMode);
	TRACE("ipv4Address : %u", info.ipv4Address);
	TRACE("port : %u", info.port);
	TRACE("maxNumOfUser : %u", info.maxNumOfUser);
	TRACE("userNameSupported : %u", info.userNameSupported);
	TRACE("userPhotoSupported : %u", info.userPhotoSupported);
	TRACE("pinSupported : %u", info.pinSupported);
	TRACE("cardSupported : %u", info.cardSupported);
	TRACE("fingerSupported : %u", info.fingerSupported);
	TRACE("faceSupported : %u", info.faceSupported);
	TRACE("wlanSupported : %u", info.wlanSupported);
	TRACE("tnaSupported : %u", info.tnaSupported);
	TRACE("triggerActionSupported : %u", info.triggerActionSupported);
	TRACE("wiegandSupported : %u", info.wiegandSupported);
	TRACE("imageLogSupported : %u", info.imageLogSupported);
	TRACE("dnsSupported : %u", info.dnsSupported);
	TRACE("jobCodeSupported : %u", info.jobCodeSupported);
	TRACE("wiegandMultiSupported : %u", info.wiegandMultiSupported);
	TRACE("rs485Mode : %u", info.rs485Mode);
	TRACE("sslSupported : %u", info.sslSupported);
	TRACE("rootCertExist : %u", info.rootCertExist);
	TRACE("dualIDSupported : %u", info.dualIDSupported);
	TRACE("useAlphanumericID : %u", info.useAlphanumericID);
	TRACE("connectedIP : %u", info.connectedIP);
	TRACE("phraseSupported : %u", info.phraseSupported);
	TRACE("card1xSupported : %u", info.card1xSupported);
	TRACE("systemExtSupported : %u", info.systemExtSupported);
	TRACE("voipSupported : %u", info.voipSupported);
	TRACE("rs485ExSupported : %u", info.rs485ExSupported);
	TRACE("cardExSupported : %u", info.cardExSupported);
	TRACE("============================>>");
}

void DeviceControl::print(const BS2SimpleDeviceInfoEx& info)
{
	TRACE("==[BS2SimpleDeviceInfoEx]==");
	TRACE("rs485Ex : %d", (info.supported & BS2SimpleDeviceInfoEx::BS2_SUPPORT_RS485EX));
	TRACE("cardEx : %d", (info.supported & BS2SimpleDeviceInfoEx::BS2_SUPPORT_CARDEX));
	TRACE("DST : %d", (info.supported & BS2SimpleDeviceInfoEx::BS2_SUPPORT_DST));
	TRACE("============================>>");
}

void DeviceControl::print(const BS2IPv6DeviceInfo& info)
{
	TRACE("==[BS2IPv6DeviceInfo]==");
	TRACE("id : %u", info.id);
	TRACE("bIPv6Mode : %u", info.bIPv6Mode);
	TRACE("ipv6Address : %s", info.ipv6Address);
	TRACE("portV6 : %u", info.portV6);
	TRACE("connectedIPV6 : %s", info.connectedIPV6);
	TRACE("numOfAllocatedAddressV6 : %u", info.numOfAllocatedAddressV6);
	for (uint32_t index = 0; index < BS2_MAX_IPV6_ALLOCATED_ADDR; index++)
	{
		TRACE("allocatedIpAddressV6[%u] : %s", index, info.allocatedIpAddressV6[index]);
	}
}

void DeviceControl::print(const BS2DeviceCapabilities& info)
{
	TRACE("==[BS2DeviceCapabilities]==");
	TRACE("maxUsers : %u", info.maxUsers);
	TRACE("maxEventLogs : %u", info.maxEventLogs);
	TRACE("maxImageLogs : %u", info.maxImageLogs);
	TRACE("maxBlacklists : %u", info.maxBlacklists);
	TRACE("maxOperators : %u", info.maxOperators);
	TRACE("maxCards : %u", info.maxCards);
	TRACE("maxFaces : %u", info.maxFaces);
	TRACE("maxFingerprints : %u", info.maxFingerprints);
	TRACE("maxUserNames : %u", info.maxUserNames);
	TRACE("maxUserImages : %u", info.maxUserImages);
	TRACE("maxUserJobs : %u", info.maxUserJobs);
	TRACE("maxUserPhrases : %u", info.maxUserPhrases);
	TRACE("maxCardsPerUser : %u", info.maxCardsPerUser);
	TRACE("maxFacesPerUser : %u", info.maxFacesPerUser);
	TRACE("maxFingerprintsPerUser : %u", info.maxFingerprintsPerUser);
	TRACE("maxInputPorts : %u", info.maxInputPorts);
	TRACE("maxOutputPorts : %u", info.maxOutputPorts);
	TRACE("maxRelays : %u", info.maxRelays);
	TRACE("maxRS485Channels : %u", info.maxRS485Channels);

	TRACE("cameraSupported : %u", info.cameraSupported);
	TRACE("tamperSupported : %u", info.tamperSupported);
	TRACE("wlanSupported : %u", info.wlanSupported);
	TRACE("displaySupported : %u", info.displaySupported);
	TRACE("thermalSupported : %u", info.thermalSupported);
	TRACE("maskSupported : %u", info.maskSupported);
	TRACE("faceExSupported : %u", info.faceExSupported);

	TRACE("[Card Supported]");
	TRACE("mask : 0x%04x", info.cardSupported.mask);
	TRACE("MifareFelica : %u", info.cardSupported.HIDProx);
	TRACE("BLE : %u", info.cardSupported.BLE);

	TRACE("[ExtendedMode] : %u", info.authSupported.extendedMode);
	TRACE("(Credentials)");
	TRACE("mask : 0x%01x", info.authSupported.credentials.mask);
	TRACE("card : %u", info.authSupported.credentials.card);
	TRACE("face : %u", info.authSupported.credentials.face);

	if (!info.authSupported.extendedMode)
	{
		TRACE("(Legacy)");
		TRACE("biometricAuth.mask : 0x%01x", info.authSupported.legacy.biometricAuth.mask);
		TRACE("cardAuth.mask : 0x%01x", info.authSupported.legacy.cardAuth.mask);
		TRACE("idAuth.mask : 0x%01x", info.authSupported.legacy.idAuth.mask);
	}
	else
	{
		TRACE("(Extended)");
		TRACE("faceAuth.mask : 0x%04x", info.authSupported.extended.faceAuth.mask);
		TRACE("fingerprintAuth.mask : 0x%04x", info.authSupported.extended.fingerprintAuth.mask);
		TRACE("cardAuth.mask : 0x%04x", info.authSupported.extended.cardAuth.mask);
		TRACE("idAuth.mask : 0x%04x", info.authSupported.extended.idAuth.mask);
		TRACE("faceAuth : 0x%04x", info.authSupported.extended.faceAuth.mask);
	}

	TRACE("intelligentPDSupported : %u", info.intelligentPDSupported);
}

#if 0
void DeviceControl::print(const BS2AuthOperatorLevel& opr)
{
	TRACE("==[]==");
	TRACE(" : %u", opr.);
}
#endif
