#pragma once

#if defined(_WIN32) || defined(_WIN64)
#include <Windows.h>
#endif

#include <iostream>
#include <string>
#include <vector>
#include <memory>
#include <array>
#include "PreDefined.h"
#include "BS_Errno.h"
#include "../Common/BS2Context.h"
#include "../Common/DeviceControl.h"
#include "../Common/ConfigControl.h"
#include "../Common/CommControl.h"
#include "../Common/DeviceList.h"
#include "../Common/UserControl.h"


enum EN_MENU_DEV
{
	MENU_USR_BREAK = MENU_BREAK,
	MENU_USR_GET_USR,
	MENU_USR_GET_USR_FACEEX,
	MENU_USR_GET_ALLUSR,
	MENU_USR_GET_ALLUSR_FACEEX,
	MENU_USR_ENR_USR,
	MENU_USR_ENR_USR_SMALL,
	MENU_USR_ENR_USR_FACEEX,
	MENU_USR_ENR_USR_SCAN_N_LOAD,
	MENU_USR_EXTRACT_TEMPLATE_FACEEX,
	MENU_USR_GET_NORMALIZE_IMAGE_FACEEX,
	MENU_USR_GET_LASTFPIMAGE,
	MENU_USR_REM_USR,
	MENU_USR_REM_ALLUSR,
	MENU_USR_ACT_USR,
	MENU_USR_UPD_USR,
	MENU_USR_GET_STATISTIC,

	MENU_USR_GET_OPERATOR,
	MENU_USR_SET_OPERATOR,
	MENU_USR_REM_OPERATOR,

	MENU_USR_SMARTCARD_SCAN,
	MENU_USR_SMARTCARD_WRITE,
	MENU_USR_SMARTCARD_ERASE,
};

std::vector<MENU_ITEM> menuInfoDeviceAPI =
{
	{MENU_USR_BREAK,						"Exit"},
	{MENU_USR_GET_USR,						"Get user from device" },
	{MENU_USR_GET_USR_FACEEX,				"Get FaceEx user from device" },
	{MENU_USR_GET_ALLUSR,					"Get all users from device" },
	{MENU_USR_GET_ALLUSR_FACEEX,			"Get all users from FaceEx device" },
	{MENU_USR_ENR_USR,						"Enroll user to device (BS2UserBlob)" },
	{MENU_USR_ENR_USR_SMALL,				"Enroll user to device (BS2UserSmallBlob)" },
	{MENU_USR_ENR_USR_FACEEX,				"Enroll user to FaceEx device"},
	{MENU_USR_ENR_USR_SCAN_N_LOAD,			"Enroll user with scanned and loaded image for faceEx"},
	{MENU_USR_EXTRACT_TEMPLATE_FACEEX,		"Extract faceEx template from FSF2"},
	{MENU_USR_GET_NORMALIZE_IMAGE_FACEEX,	"Get normalize image and Enroll"},
	{MENU_USR_GET_LASTFPIMAGE,				"Get last fingerprint image from device"},
	{MENU_USR_REM_USR,						"Remove a user " },
	{MENU_USR_REM_ALLUSR,					"Remove all users" },
	{MENU_USR_ACT_USR,						"Enable/Disable user"},
	{MENU_USR_UPD_USR,						"Update user"},
	{MENU_USR_GET_STATISTIC,				"Get user statistic"},

	{MENU_USR_GET_OPERATOR,					"Get operators from device"},
	{MENU_USR_SET_OPERATOR,					"Set operators to device"},
	{MENU_USR_REM_OPERATOR,					"Remove operators in device"},

	{MENU_USR_SMARTCARD_SCAN,				"Smartcard scan"},
	{MENU_USR_SMARTCARD_WRITE,				"Smartcard write"},
	{MENU_USR_SMARTCARD_ERASE,				"Smartcard erase"},
};


void connectTestDevice(void* context);
int runAPIs(void* context, const DeviceInfo& device);
int getLastFingerprintImage(UserControl& uc, BS2_DEVICE_ID id);
int scanCard(UserControl& uc, BS2_DEVICE_ID id);
int writeCard(UserControl& uc, BS2_DEVICE_ID id);
int eraseCard(UserControl& uc, BS2_DEVICE_ID id);
int extractTemplateFaceEx(void* context, BS2_DEVICE_ID id);
int getNormalizedImageFaceEx(void* context, BS2_DEVICE_ID id);
int getOperators(void* context, BS2_DEVICE_ID id);
int setOperators(void* context, BS2_DEVICE_ID id);
int removeOperators(void* context, BS2_DEVICE_ID id);
int updateUser(void* context, BS2_DEVICE_ID id);
int getUserStatistic(void* context, BS2_DEVICE_ID id);
BS2_USER_MASK getMaskForUpdateUser();
