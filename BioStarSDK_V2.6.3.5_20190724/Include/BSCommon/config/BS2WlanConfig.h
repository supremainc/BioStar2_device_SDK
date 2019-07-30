/**
 *  Wireless Lan Configuration
 *
 *  @author jylee@suprema.co.kr
 *  @see
 */

/*
 *  Copyright (c) 2014 Suprema Co., Ltd. All Rights Reserved.
 *
 *  This software is the confidential and proprietary information of
 *  Suprema Co., Ltd. ("Confidential Information").  You shall not
 *  disclose such Confidential Information and shall use it only in
 *  accordance with the terms of the license agreement you entered into
 *  with Suprema.
 */

#ifndef __BS2_WLAN_CONFIG_H__
#define __BS2_WLAN_CONFIG_H__

#include "../BS2Types.h"

/**
 *	Constants
 */
enum {
	BS2_WLAN_SSID_SIZE = 32,
	BS2_WLAN_KEY_SIZE = 64,
};

/**
 *	BS2_WLAN_OPMODE
 */
enum {
	BS2_WLAN_OPMODE_MANAGED,
	BS2_WLAN_OPMODE_ADHOC,

	BS2_WLAN_OPMODE_DEFAULT = BS2_WLAN_OPMODE_MANAGED,
};

typedef uint8_t BS2_WLAN_OPMODE;

/**
 *  BS2_WLAN_AUTH_TYPE
 */
enum {
	BS2_WLAN_AUTH_OPEN,
	BS2_WLAN_AUTH_SHARED,
	BS2_WLAN_AUTH_WPA_PSK,
	BS2_WLAN_AUTH_WPA2_PSK,

	BS2_WLAN_AUTH_DEFAULT = BS2_WLAN_AUTH_OPEN,
};

typedef uint8_t BS2_WLAN_AUTH_TYPE;

/**
 *  BS2_WLAN_ENC_TYPE
 */
enum {
	BS2_WLAN_ENC_NONE,
	BS2_WLAN_ENC_WEP,
	BS2_WLAN_ENC_TKIP_AES,
	BS2_WLAN_ENC_AES,
	BS2_WLAN_ENC_TKIP,

	BS2_WLAN_ENC_DEFAULT = BS2_WLAN_ENC_NONE,
};

typedef uint8_t BS2_WLAN_ENC_TYPE;

/**
 *	BS2IpConfig
 */
typedef struct {
	BS2_BOOL enabled;					///< 1 byte
	BS2_WLAN_OPMODE operationMode;		///< 1 byte
	BS2_WLAN_AUTH_TYPE authType;		///< 1 byte
	BS2_WLAN_ENC_TYPE encryptionType;	///< 1 byte

	char essid[BS2_WLAN_SSID_SIZE];		///< 32 bytes
	char authKey[BS2_WLAN_KEY_SIZE];	///< 64 bytes

	uint8_t reserved2[32];				///< 32 bytes (reserved)
} BS2WlanConfig;

#endif	// __BS2_WLAN_CONFIG_H__
