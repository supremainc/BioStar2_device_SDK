/**
 *  Factory Configuration Definitions
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


#ifndef __BS2_FACTORY_CONFIG_H__
#define __BS2_FACTORY_CONFIG_H__

#include "../BS2Types.h"

/**
 *	Constants for BS2FactoryConfig
 */
enum {
	BS2_MAC_ADDR_LEN		= 6,
	BS2_MODEL_NAME_LEN		= 32,
	BS2_FIRMWARE_REV_LEN	= 32,
	BS2_BSCORE_REV_LEN	= 32,
	BS2_KERNEL_REV_LEN	= 32,

	BS2_DEVICE_ID_MIN = 0x01000000,
	BS2_DEVICE_ID_MAX = 0x3FFFFFFF,
};

typedef struct {
	uint8_t major;			///< 1 byte
	uint8_t minor;			///< 1 byte
	uint8_t ext;			///< 1 byte
	uint8_t reserved[1];	///< 1 byte (packing)
} Version;

/**
 *	BS2FactoryConfig
 */
typedef struct {
	BS2_DEVICE_ID deviceID;					///< 4 bytes
	uint8_t macAddr[BS2_MAC_ADDR_LEN];		///< 6 bytes
	uint8_t reserved[2];					///< 2 bytes (packing)
	char modelName[BS2_MODEL_NAME_LEN];		///< 32 bytes
	Version boardVer;						///< 4 bytes
	Version kernelVer;						///< 4 bytes
	Version bscoreVer;					///< 4 bytes
	Version firmwareVer;					///< 4 bytes
	char kernelRev[BS2_KERNEL_REV_LEN];	///< 32 bytes
	char bscoreRev[BS2_BSCORE_REV_LEN];	///< 32 bytes
	char firmwareRev[BS2_FIRMWARE_REV_LEN];	///< 32 bytes

	uint8_t reserved2[32];				///< 32 bytes (reserved)
} BS2FactoryConfig;

#endif	// __BS2_FACTORY_CONFIG_H__
