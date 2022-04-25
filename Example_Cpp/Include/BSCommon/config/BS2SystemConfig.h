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
#include "../../BS_Deprecated.h"

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
	CARD_OPERATION_MASK_EM				= 0x00000001,

	CARD_OPERATION_MASK_NONE						= 0x0000,
	CARD_OPERATION_MASK_LF_EM 						= 0x0001,
	CARD_OPERATION_MASK_LF_PROX 					= 0x0002,
	CARD_OPERATION_MASK_HF_CSN_MIFARE 				= 0x0004, //Mifare/Felica/Desfire/1`5693
	CARD_OPERATION_MASK_HF_CSN_ICLASS 				= 0x0008,
	CARD_OPERATION_MASK_HF_SMART_MIFARE 			= 0x0010, //Classic/Plus
	CARD_OPERATION_MASK_HF_SMART_MIFARE_DESFIRE 	= 0x0020, //Desfire(EV1 Æ÷CO)
	CARD_OPERATION_MASK_HF_SMART_ICLASS 			= 0x0040, //SR/SE
	CARD_OPERATION_MASK_HF_SMART_ICLASS_SEOS 		= 0x0080,
	CARD_OPERATION_MASK_MOBILE_NFC 					= 0x0100,
	CARD_OPERATION_MASK_MOBILE_BLE 					= 0x0200,
	CARD_OPERATION_MASK_HF_CSN_OTHERS 				= 0x0400,
};

// Redefinition V2.8.3
#define CARD_OPERATION_MASK_BLE				(DEPRECATED_ENUM)CARD_OPERATION_MASK_BLE
#define CARD_OPERATION_MASK_NFC				(DEPRECATED_ENUM)CARD_OPERATION_MASK_NFC
#define CARD_OPERATION_MASK_SEOS			(DEPRECATED_ENUM)CARD_OPERATION_MASK_SEOS
#define CARD_OPERATION_MASK_SR_SE			(DEPRECATED_ENUM)CARD_OPERATION_MASK_SR_SE
#define CARD_OPERATION_MASK_DESFIRE_EV1		(DEPRECATED_ENUM)CARD_OPERATION_MASK_DESFIRE_EV1
#define CARD_OPERATION_MASK_CLASSIC_PLUS	(DEPRECATED_ENUM)CARD_OPERATION_MASK_CLASSIC_PLUS
#define CARD_OPERATION_MASK_ICLASS			(DEPRECATED_ENUM)CARD_OPERATION_MASK_ICLASS
#define CARD_OPERATION_MASK_MIFARE_FELICA	(DEPRECATED_ENUM)CARD_OPERATION_MASK_MIFARE_FELICA
#define CARD_OPERATION_MASK_HIDPROX			(DEPRECATED_ENUM)CARD_OPERATION_MASK_HIDPROX
#define CARD_OPERATION_MASK_EM				(DEPRECATED_ENUM)CARD_OPERATION_MASK_EM

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
