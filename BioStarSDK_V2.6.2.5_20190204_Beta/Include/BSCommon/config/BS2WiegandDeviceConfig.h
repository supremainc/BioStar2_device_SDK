/*
 * BS2WiegandDeviceConfig.h
 *
 *  Created on: 2015. 3. 16.
 *      Author: yhlee
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
 
#ifndef __BS2_WIEGAND_DEVICE_CONFIG_H__
#define __BS2_WIEGAND_DEVICE_CONFIG_H__

#include "BS2Types.h"

/**
 *  BS2WiegandTamper
 */
typedef struct{
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	BS2_PORT		port;			///< 2 byte
	uint8_t			switchType;		///< 1 byte : 0 = N/O, 1 = N/C
	uint8_t			reserved[1];	///< 1 bytes (packing)
} BS2WiegandTamperInput;

enum{
	BS2_WIEGAND_DEVICE_BUZZER_NUM = 3,
};

/**
 *  BS2WiegandLedStatus
 */
typedef struct{
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	BS2_PORT		port;		///< 2 byte
	uint8_t			reserved[10];	///< 10 bytes (packing)
} BS2WiegandLedOutput;

/**
 *  BS2WiegandBuzzerStatus
 */
typedef struct{
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	BS2_PORT		port;		///< 2 byte
	
	uint8_t			reserved[34];	///< 34 bytes (packing)
} BS2WiegandBuzzerOutput;

enum {
	BS2_WIEGAND_STATUS_GREEN,
	BS2_WIEGAND_STATUS_RED,
	BS2_WIEGAND_STATUS_NUM,
};

/**
 *  BS2WiegandDeviceConfig
 */
typedef struct{
	BS2WiegandTamperInput	tamper;
	BS2WiegandLedOutput		led[BS2_WIEGAND_STATUS_NUM];
	BS2WiegandBuzzerOutput	buzzer;
	uint32_t				reserved[32];
} BS2WiegandDeviceConfig;

#endif	// __BS2_WIEGAND_DEVICE_CONFIG_H__
