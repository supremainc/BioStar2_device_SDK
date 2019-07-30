/**
 *  Floor Level Definitions
 *
 *  @author smlee@suprema.co.kr
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

#ifndef __BS2_FLOOR_LEVEL_H__
#define __BS2_FLOOR_LEVEL_H__

#include "../BS2Types.h"

enum {
	BS2_MAX_FLOOR_LEVEL_ITEMS		= 128,
	BS2_MAX_FLOOR_LEVEL_NAME_LEN	= 48 * 3,

	BS2_INVALID_FLOOR_LEVEL_ID = 0,
};

/**
 *	BS2FloorSchedule
 */
typedef struct {
	BS2_LIFT_ID		liftID;
	uint16_t					floorIndex;
	uint8_t					reserved[2];
	BS2_SCHEDULE_ID	scheduleID;
} BS2FloorSchedule;

/**
 *	BS2FloorLevel
 */
typedef struct {
	BS2_FLOOR_LEVEL_ID		id;		// id >= 32768 (BS2_ACCESS_LEVEL_ID < 32768)
	char					name[BS2_MAX_FLOOR_LEVEL_NAME_LEN];
	uint8_t					numFloorSchedules;
	uint8_t					reserved[3];
	BS2FloorSchedule			floorSchedules[BS2_MAX_FLOOR_LEVEL_ITEMS];
} BS2FloorLevel;

#endif	// __BS2_FLOOR_LEVEL_H__
