/**
 *  Access Level Definitions
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

#ifndef __BS2_ACCESS_LEVEL_H__
#define __BS2_ACCESS_LEVEL_H__

#include "../BS2Types.h"

enum {
	BS2_MAX_ACCESS_LEVEL_ITEMS		= 128,
	BS2_MAX_ACCESS_LEVEL_NAME_LEN	= 48 * 3,

	BS2_INVALID_ACCESS_LEVEL_ID = 0,
};

/**
 *	BS2DoorSchedule
 */
typedef struct {
	BS2_DOOR_ID		doorID;		///< 4 bytes
	BS2_SCHEDULE_ID	scheduleID;		///< 4 bytes
} BS2DoorSchedule;

/**
 *	BS2AccessLevel
 */
typedef struct {
	BS2_ACCESS_LEVEL_ID		id;		///< 4 bytes, id < 32768 (BS2_FLOOR_LEVEL_ID >= 32768)
	char					name[BS2_MAX_ACCESS_LEVEL_NAME_LEN];		///< 144 bytes
	uint8_t					numDoorSchedules;		///< 1 byte
	uint8_t					reserved[3];			///< 3 bytes (packing)
	BS2DoorSchedule			doorSchedules[BS2_MAX_ACCESS_LEVEL_ITEMS];		///< 8 * 128 bytes
} BS2AccessLevel;

#endif	// __BS2_ACCESS_LEVEL_H__
