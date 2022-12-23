/**
 *  RS485 Config
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

#ifndef __BS2_RS485_CONFIG_H__
#define __BS2_RS485_CONFIG_H__

#include "../BS2Types.h"
#include "../data/BS2Rs485Channel.h"

/**
 *	Constants for BS2Rs485Config
 */
enum {
	BS2_RS485_MAX_CHANNELS		= 4,
	BS2_RS485_MAX_CHANNELS_EX	= 8,
};

/**
 *	BS2_RS485_MODE
 */
enum {
	BS2_RS485_MODE_DISABLED		= 0,
	BS2_RS485_MODE_MASTER		= 1,
	BS2_RS485_MODE_SLAVE		= 2,
	BS2_RS485_MODE_STANDALONE	= 3,

	BS2_RS485_MODE_DEFAULT		= BS2_RS485_MODE_STANDALONE,
};

enum {
	BS2_RS485_MAX_EXCEPTION_CODE_LEN = 8,

	BS2_IPD_OUTPUT_CARDID		= 0,
	BS2_IPD_OUTPUT_USERID		= 1,
};

typedef struct {
	BS2_BOOL supportConfig;								///< 1 byte
	BS2_BOOL useExceptionCode;							///< 1 byte
	uint8_t exceptionCode[BS2_RS485_MAX_EXCEPTION_CODE_LEN];	///< 8 byte
	uint8_t outputFormat;     							///< 1 byte (0 - CardID, 1 - UserID)
	uint8_t osdpID;										///< 1 byte 0 ~ 127
	uint8_t reserved[4];								///< 4 bytes (packing)
} BS2IntelligentPDInfo;

typedef uint8_t BS2_RS485_MODE;

/**
 *	BS2Rs485Config
 */
typedef struct {
	BS2_RS485_MODE mode;		///< 1 byte
	uint8_t numOfChannels;		///< 1 byte
	uint8_t reserved[2];		///< 2 bytes (packing)
	BS2IntelligentPDInfo intelligentInfo;			///< 1 byte
	uint8_t reserved1[16];		///< 16 bytes (reserved)

	BS2Rs485Channel channels[BS2_RS485_MAX_CHANNELS]; 	///< 72 bytes
} BS2Rs485Config;

/**
 *	BS2Rs485ConfigEX
 */
typedef struct {
	BS2_RS485_MODE mode[BS2_RS485_MAX_CHANNELS_EX];		///< 8 byte
	uint16_t numOfChannels;		///< 2 byte
	uint8_t reserved[2];		///< 2 bytes (packing)

	uint8_t reserved1[32];		///< 32 bytes (reserved)

	BS2Rs485ChannelEX channels[BS2_RS485_MAX_CHANNELS_EX]; 	///< 72 * 8 bytes
} BS2Rs485ConfigEX;

#endif	// __BS2_RS485_CONFIG_H__

