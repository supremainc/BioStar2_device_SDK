/**
 *  System Configuration Definitions
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

#ifndef __BS2_SYSTEM_CONFIG_H__
#define __BS2_SYSTEM_CONFIG_H__

#include "../BS2Types.h"
#include "BS2DisplayConfig.h"

enum {
    CARD_OPERATION_MASK_DEFAULT			= 0xFFFFFFFF,
	CARD_OPERATION_USE					= 0x80000000,
	CARD_OPERATION_MASK_BLE				= 0x00000200,
	CARD_OPERATION_MASK_NFC				= 0x00000100,
	CARD_OPERATION_MASK_SEOS			= 0x00000080,
	CARD_OPERATION_MASK_SR_SE			= 0x00000040,
	CARD_OPERATION_MASK_DESFIRE_EV1		= 0x00000020,
	CARD_OPERATION_MASK_CLASSIC_PLUS	= 0x00000010,
	CARD_OPERATION_MASK_ICLASS			= 0x00000008,
	CARD_OPERATION_MASK_MIFARE_FELICA	= 0x00000004,
	CARD_OPERATION_MASK_HIDPROX			= 0x00000002,
	CARD_OPERATION_MASK_EM				= 0x00000001
};

/**
 *	BS2SystemConfig
 */
typedef struct {
#if 0
	char tnaLabel[BS2_MAX_TNA_KEY][BS2_MAX_TNA_LABEL_LEN];
#else
	uint8_t notUsed[16 * 16 * 3];
#endif

	int32_t timezone;                                    ///< 4 bytes (offset of GMT in second)

	BS2_BOOL syncTime;                                   ///< 1 byte (sync time with server)
	BS2_BOOL serverSync;                                 ///< 1 byte
	BS2_BOOL deviceLocked;                               /// 1 byte
	BS2_BOOL useInterphone;                              ///< 1 byte

	BS2_BOOL useUSBConnection;                           ///< 1 byte
	BS2_BOOL keyEncrypted;                               ///< 1 byte
	BS2_BOOL useJobCode;                                 ///< 1 byte
	BS2_BOOL useAlphanumericID;                          ///< 1 byte

	uint32_t cameraFrequency;                            ///< 4 bytes

	BS2_BOOL secureTamper;                               ///< 1 byte
	BS2_BOOL reserved0;                                  ///< 1 byte   (write protected)
	uint8_t reserved[2];                                 ///< 2 bytes
	uint32_t useCardOperationMask;			   			 ///< 4 Bytes (bitmask , no use 0 postion bit - 0~30 bit, valid data- 31 bit)
	uint8_t reserved2[16];                               ///< 16 bytes (reserved)
} BS2SystemConfig;

#endif	// __BS2_SYSTEM_CONFIG_H__
