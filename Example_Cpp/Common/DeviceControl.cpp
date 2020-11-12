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

int DeviceControl::getAuthOperatorLevelEx(BS2_DEVICE_ID id)
{
	stringstream msg;
	msg << "Please enter operators UID. ex)ID1 ID2 ID3 ...\n";
	string inStrArrOprs = Utility::getLine(msg.str());
	int sdkResult = BS_SDK_ERROR_CANNOT_FIND_OPERATOR;

	if (0 < inStrArrOprs.size())
	{
		BS2UIDArray arr;
		vector<string> listOprs = Utility::tokenizeString(inStrArrOprs);
		arr.append(listOprs);

		uint32_t inNumOfOpr = arr.getSize();
		uint32_t outNumOfOpr(0);
		BS2AuthOperatorLevel* oprLevelObj = NULL;

		sdkResult = BS2_GetAuthOperatorLevelEx(context_, id, reinterpret_cast<BS2_USER_ID*>(arr.getPtr()), inNumOfOpr, &oprLevelObj, &outNumOfOpr);
		if (BS_SDK_SUCCESS != sdkResult)
		{
			TRACE("BS2_GetAuthOperatorLevelEx call failed: %d", sdkResult);
			return sdkResult;
		}

		for (uint32_t index = 0; index < outNumOfOpr; index++)
			print(oprLevelObj[index]);

		if (oprLevelObj)
			BS2_ReleaseObject(oprLevelObj);
	}

	return sdkResult;
}

int DeviceControl::extractTemplateFaceEx(BS2_DEVICE_ID id, BS2TemplateEx& templateEx)
{
	BS2SimpleDeviceInfoEx deviceInfoEx = { 0, };

	int sdkResult = BS2_GetDeviceInfoEx(context_, id, NULL, &deviceInfoEx);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetDeviceInfoEx call failed: %d", sdkResult);
		return sdkResult;
	}

	bool faceExScanSupported = (deviceInfoEx.supported & BS2SimpleDeviceInfoEx::BS2_SUPPORT_FACE_EX_SCAN) == BS2SimpleDeviceInfoEx::BS2_SUPPORT_FACE_EX_SCAN;
	if (faceExScanSupported)
	{
		char flag = Utility::getInput<char>("Do you want to extract faceEx template from image? [y/n]");
		if ('y' == flag || 'Y' == flag)
		{
			string imagePath = Utility::getInput<string>("Enter the face image path and name:");
			uint32_t size = Utility::getResourceSize(imagePath);
			shared_ptr<uint8_t> buffer(new uint8_t[size], ArrayDeleter<uint8_t>());

			size_t dataOffset = offsetof(BS2FaceEx, rawImageData);
			size_t faceSize = dataOffset + size;
			if (Utility::getResourceFromFile(imagePath, buffer, size))
			{
				sdkResult = BS2_ExtractTemplateFaceEx(context_, id, buffer.get(), size, 0, &templateEx);
				if (BS_SDK_SUCCESS != sdkResult)
				{
					TRACE("BS2_ExtractTemplateFaceEx call failed: %d", sdkResult);
					return sdkResult;
				}

				print(templateEx);
			}
		}
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

void DeviceControl::print(const BS2AuthOperatorLevel& opr)
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

#if 0
void DeviceControl::print(const BS2AuthOperatorLevel& opr)
{
	TRACE("==[]==");
	TRACE(" : %u", opr.);
}
#endif

void DeviceControl::print(const BS2TemplateEx& templateEx)
{
	TRACE("==[BS2TemplateEx]==");
	TRACE("isIR : %u", templateEx.isIR);
	TRACE("data[0] : %x, data[551] : %x", templateEx.data[0], templateEx.data[551]);
}
