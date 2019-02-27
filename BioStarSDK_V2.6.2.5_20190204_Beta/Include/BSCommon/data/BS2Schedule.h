/**
 *  Access Schedule Definitions
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

#ifndef __BS2_SCHEDULE_H__
#define __BS2_SCHEDULE_H__

#include "BS2Types.h"
#include "BS2DaySchedule.h"
#include "BS2Holiday.h"

/**
 *  Schedule constants
 */
enum {
	BS2_MAX_HOLIDAY_GROUPS_PER_SCHEDULE	= 4,
	BS2_MAX_DAYS_PER_DAILY_SCHEDULE		= 90,

	BS2_MAX_SCHEDULE_NAME_LEN			= 48 * 3,

	BS2_INVALID_SCHEDULE_ID = 0,

	BS2_SCHEDULE_NEVER_ID				= 0,
	BS2_SCHEDULE_ALWAYS_ID				= 1,
};

/**
 *	WeeklySchedule
 */
typedef struct {
	BS2DaySchedule	schedule[BS2_NUM_WEEKDAYS];
} BS2WeeklySchedule;

typedef struct {
	BS2_TIMESTAMP	startDate;		///< 4 bytes
	uint8_t			numDays;		///< 1 byte
	uint8_t			reserved[3];	///< 3 bytes (packing)
	BS2DaySchedule	schedule[BS2_MAX_DAYS_PER_DAILY_SCHEDULE];
} BS2DailySchedule;

/**
 *	BS2HolidaySchedule
 */
typedef struct {
	BS2_HOLIDAY_GROUP_ID	id;		///< 4 bytes
	BS2DaySchedule			schedules;
} BS2HolidaySchedule;

/**
 *  BS2Schedule
 */
typedef struct {
	BS2_SCHEDULE_ID	id;						///< 4 bytes
	char			name[BS2_MAX_SCHEDULE_NAME_LEN];

	BS2_BOOL		isDaily;				///< 1 byte
	uint8_t			numHolidaySchedules;	///< 1 byte
	uint8_t			reserved[2];			///< 2 bytes (packing)
	union {
		BS2WeeklySchedule	weekly;
		BS2DailySchedule	daily;
	} schedule;

	BS2HolidaySchedule	holidaySchedules[BS2_MAX_HOLIDAY_GROUPS_PER_SCHEDULE];
} BS2Schedule;

#endif	// __BS2_SCHEDULE_H__
