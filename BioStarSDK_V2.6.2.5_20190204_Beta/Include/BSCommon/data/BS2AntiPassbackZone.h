/**
 *  Anti-Passback Zone
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

#ifndef __BS2_ANTI_PASSBACK_ZONE_H__
#define __BS2_ANTI_PASSBACK_ZONE_H__

#include "BS2Types.h"
#include "BS2Action.h"
#include "BS2Zone.h"

/**
 *  Constants
 */
enum {
	BS2_MAX_READERS_PER_APB_ZONE		= 64,
	BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE	= 16,
	BS2_MAX_APB_ALARM_ACTION			= 5,

	BS2_RESET_DURATION_DEFAULT			= 86400,	///< 1 day
};

/**
 *  BS2_APB_ZONE_TYPE
 */
enum {
	BS2_APB_ZONE_HARD		= 0x00,
	BS2_APB_ZONE_SOFT		= 0x01,
};

typedef uint8_t BS2_APB_ZONE_TYPE;

/**
 *  BS2_APB_ZONE_READER_TYPE
 */
enum {
	BS2_APB_ZONE_READER_NONE	= -1,
	BS2_APB_ZONE_READER_ENTRY	= 0,
	BS2_APB_ZONE_READER_EXIT	= 1,
};

typedef uint8_t BS2_APB_ZONE_READER_TYPE;

typedef struct {
	BS2_DEVICE_ID deviceID;			///< 4 bytes
	BS2_APB_ZONE_READER_TYPE type;	///< 1 byte
	uint8_t reserved[3];			///< 3 bytes (packing)
} BS2ApbMember;

/**
 *  BS2AntiPassbackZone
 */
typedef struct {
	BS2_ZONE_ID zoneID;					///< 4 bytes
	char name[BS2_MAX_ZONE_NAME_LEN];		///< 48 * 3 bytes

	BS2_APB_ZONE_TYPE type;				///< 1 byte
	uint8_t numReaders;					///< 1 byte
	uint8_t numBypassGroups;				///< 1 byte
	BS2_BOOL disabled;				///< 1 byte

	BS2_BOOL alarmed;				///< 1 byte
	uint8_t reserved[3];				///< 3 bytes (packing)

	uint32_t resetDuration;				///< 4 bytes: in seconds, 0: no reset

	BS2Action alarm[BS2_MAX_APB_ALARM_ACTION];		///< 32 * 5 bytes

	BS2ApbMember readers[BS2_MAX_READERS_PER_APB_ZONE];		///< 8 * 64 bytes
	uint8_t reserved2[8 * 64];		///< 8 * 64 bytes (packing)

	BS2_ACCESS_GROUP_ID bypassGroupIDs[BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE];		///< 4 * 16 bytes
} BS2AntiPassbackZone;

#endif	// __BS2_ANTI_PASSBACK_ZONE_H__
