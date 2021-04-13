/**
 *  Wiegand Configuration Definitions
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

#ifndef __BS2_WIEGAND_CONFIG_H__
#define __BS2_WIEGAND_CONFIG_H__

#include "../BS2Types.h"
#include "../../BS_Deprecated.h"

/**
 *  Card Config constants
 */
enum {
	BS2_WIEGAND_FIELD_SIZE		= 32,	///< 256 bits
	BS2_WIEGAND_FIELD_MASK_SIZE = BS2_WIEGAND_FIELD_SIZE * 8,
	BS2_WIEGAND_MAX_FIELDS		= 4,
	BS2_WIEGAND_MAX_PARITIES	= 4,
};

/**
 *
 */
enum {
	BS2_WIEGAND_FMT_H10301_26BIT		= 0,
	BS2_WIEGAND_FMT_H10302_37BIT		= 1,
	BS2_WIEGAND_FMT_H10304_37BIT		= 2,
	BS2_WIEGAND_FMT_CORPORATE1000_35BIT	= 3,
};

/**
 *  BS2_WIEGAND_MODE
 */
enum {
	BS2_WIEGAND_IN			= 0,
	BS2_WIEGAND_OUT			= 1,
	BS2_WIEGAND_IN_ONLY		= 0,
	BS2_WIEGAND_OUT_ONLY	= 1,

	BS2_WIEGAND_IN_OUT		= 2,
};

// Deprecated in V2.6.0. Instead, use the BS2_WIEGAND_IN_ONLY or BS2_WIEGAND_OUT_ONLY
#define BS2_WIEGAND_IN		(DEPRECATED_ENUM)BS2_WIEGAND_IN
#define BS2_WIEGAND_OUT		(DEPRECATED_ENUM)BS2_WIEGAND_OUT

// Deprecated in V2.6.0. Instead, use the BS2_WIEGAND_MODE
//typedef DEPRECATED_TYPEDEF(uint8_t) BS2_WIEGAND_INOUT;

typedef uint8_t BS2_WIEGAND_MODE;

/**
 *	BS2_WIEGAND_PARITY
 */
enum {
	BS2_WIEGAND_PARITY_NONE	= 0,
	BS2_WIEGAND_PARITY_ODD	= 1,
	BS2_WIEGAND_PARITY_EVEN	= 2,
};

typedef uint8_t BS2_WIEGAND_PARITY;

typedef enum {
	BS2_WIEGAND_INPUT_DATA,
	BS2_WIEGAND_CARD_DATA,
	BS2_WIEGAND_CSN_DATA
} BS2_WIEGAND_DATA_TYPE;

/**
 *  BS2WiegandFormat
 */
typedef struct {
	uint32_t	length;											///< 4 bytes
	uint8_t		idFields[BS2_WIEGAND_MAX_FIELDS][BS2_WIEGAND_FIELD_SIZE];		///< 4 * 32 bytes: 00000............................0000
	uint8_t		parityFields[BS2_WIEGAND_MAX_PARITIES][BS2_WIEGAND_FIELD_SIZE];	///< 4 * 32 bytes: 00000.....001111111111111000000000000
	BS2_WIEGAND_PARITY	parityType[BS2_WIEGAND_MAX_PARITIES];	///< 4 bytes: 0 = none, 1 = odd, 2 = even
	uint8_t		parityPos[BS2_WIEGAND_MAX_PARITIES];			///< 4 bytes: position of parity bit (0 ~ 255)
} BS2WiegandFormat;

/**
 *  BS2WiegandConfig
 */
typedef struct {
	BS2_WIEGAND_MODE mode;			///< 1 byte - BS2_WIEGAND_IN_ONLY/BS2_WIEGAND_OUT_ONLY/BS2_WIEGAND_IN_OUT
	BS2_BOOL useWiegandBypass;		///< 1 byte
	BS2_BOOL useFailCode;			///< 1 byte
	uint8_t failCode;				///< 1 byte

	uint16_t outPulseWidth;			///< 2 bytes (20 ~ 100 us, default = 40)
	uint16_t outPulseInterval;		///< 2 bytes (200 ~ 20000 us, default = 10000)

	BS2_UID formatID;				///< 4 bytes (wiegand format ID)
	BS2WiegandFormat format;		///< 268 bytes

	uint16_t	wiegandInputMask;	///< 2 bytes (bitmask , no use 0 postion bit, 1~15 bit)
	uint16_t	wiegandCardMask;	///< 2 bytes (bitmask , no use 0 postion bit, 1~15 bit)

	uint8_t		wiegandCSNIndex;	///< 1 Bytes (1~15)

	uint8_t useWiegandUserID;     	///< 1 byte (0 - Not use, 1 - CardID, 2 - UserID)
	uint8_t reserved[26];			///< 26 bytes (reserved)
} BS2WiegandConfig;

#endif	// __BS2_WIEGAND_CONFIG_H__
