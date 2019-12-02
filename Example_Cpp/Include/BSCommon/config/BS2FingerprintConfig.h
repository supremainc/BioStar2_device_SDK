/**
 *  Device Configuration Definitions
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

#ifndef __BS2_FINGERPRINT_CONFIG_H__
#define __BS2_FINGERPRINT_CONFIG_H__

#include "../BS2Types.h"

/**
 *	Fingerprint Security Level
 */
enum {
	BS2_FINGER_SECURITY_NORMAL		= 0x00,
	BS2_FINGER_SECURITY_SECURE		= 0x01,
	BS2_FINGER_SECURITY_MORE_SECURE	= 0x02,

	BS2_FINGER_SECURITY_DEFAULT		= BS2_FINGER_SECURITY_NORMAL,

	BS2_FINGER_SCAN_TIMEOUT_MIN = 1,
	BS2_FINGER_SCAN_TIMEOUT_MAX = 20,
	BS2_FINGER_SCAN_TIMEOUT_DEFAULT = 10,
};

typedef uint8_t BS2_FINGER_SECURITY_LEVEL;

/**
 *	Fingerprint Fast Mode
 */
enum {
	BS2_FINGER_FAST_MODE_AUTO		= 0x00,
	BS2_FINGER_FAST_MODE_NORMAL		= 0x01,
	BS2_FINGER_FAST_MODE_FASTER		= 0x02,
	BS2_FINGER_FAST_MODE_FASTEST	= 0x03,

	BS2_FINGER_FAST_MODE_DEFAULT	= BS2_FINGER_FAST_MODE_AUTO,
};

typedef uint8_t BS2_FINGER_FAST_MODE;

/**
 *	Fingerprint Sensor Sensitivity
 */
enum {
	BS2_FINGER_SENSOR_LEAST_SENSITIVE	= 0x00,
	BS2_FINGER_SENSOR_SENSITIVE1		= 0x01,
	BS2_FINGER_SENSOR_SENSITIVE2		= 0x02,
	BS2_FINGER_SENSOR_SENSITIVE3		= 0x03,
	BS2_FINGER_SENSOR_SENSITIVE4		= 0x04,
	BS2_FINGER_SENSOR_SENSITIVE5		= 0x05,
	BS2_FINGER_SENSOR_SENSITIVE6		= 0x06,
	BS2_FINGER_SENSOR_MOST_SENSITIVE	= 0x07,

	BS2_FINGER_SENSOR_SENSITIVITY_DEFAULT	= BS2_FINGER_SENSOR_MOST_SENSITIVE,
};

typedef uint8_t BS2_FINGER_SENSITIVITY;

/**
 *	Fingerprint Format
 */
enum {
	BS2_FINGER_TEMPLATE_FORMAT_UNKNOWN	= 0xFF,
	BS2_FINGER_TEMPLATE_FORMAT_SUPREMA	= 0x00,
	BS2_FINGER_TEMPLATE_FORMAT_ISO		= 0x01,
	BS2_FINGER_TEMPLATE_FORMAT_ANSI		= 0x02,

	BS2_FINGER_TEMPLATE_FORMAT_DEFAULT	= BS2_FINGER_TEMPLATE_FORMAT_SUPREMA,
};

typedef uint8_t BS2_FINGER_TEMPLATE_FORMAT;

/**
 *	BS2_FINGER_SENSOR_MODE
 */
enum {
	BS2_FINGER_SENSOR_MODE_ALWAYS_ON,
	BS2_FINGER_SENSOR_MODE_PROXIMITY,
};

typedef uint8_t BS2_FINGER_SENSOR_MODE;

/**
 *	Fingerprint Quality Threshold
 */
enum {
	BS2_FINGER_TEMPLATE_QUALITY_LOW = 20,
	BS2_FINGER_TEMPLATE_QUALITY_STANDARD = 40,
	BS2_FINGER_TEMPLATE_QUALITY_HIGH = 60,
	BS2_FINGER_TEMPLATE_QUALITY_HIGHEST = 80,
};

typedef uint16_t BS2_FINGER_TEMPLATE_QUALITY;

enum {
	BS2_FINGER_LFD_LEVEL_OFF,
	BS2_FINGER_LFD_LEVEL_LOW,
	BS2_FINGER_LFD_LEVEL_MIDDLE,
	BS2_FINGER_LFD_LEVEL_HIGH,

	BS2_FINGER_LFD_LEVEL_DEFAULT = BS2_FINGER_LFD_LEVEL_OFF,
};

typedef uint8_t BS2_FINGER_LFD_LEVEL;

typedef struct {
	BS2_FINGER_SECURITY_LEVEL securityLevel;	///< 1 byte
	BS2_FINGER_FAST_MODE fastMode;				///< 1 byte
	BS2_FINGER_SENSITIVITY sensitivity;			///< 1 byte
	BS2_FINGER_SENSOR_MODE sensorMode;			///< 1 byte

	BS2_FINGER_TEMPLATE_FORMAT templateFormat;	///< 1 byte
	uint8_t reserved;		///< 1 byte (padding)
	uint16_t scanTimeout;						///< 2 bytes

	BS2_BOOL successiveScan;		///< 1 byte
	BS2_BOOL advancedEnrollment;						///< 1 byte
	BS2_BOOL showImage;							///< 1 byte
	BS2_FINGER_LFD_LEVEL lfdLevel;		///< 1 byte [0: off, 1~3: on]

	uint8_t reserved3[32];						///< 32 bytes (reserved)
} BS2FingerprintConfig;

#endif	// __BS2_FINGERPRINT_CONFIG_H__
