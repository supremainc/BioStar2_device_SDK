/**
 *  Holiday Definitions
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

#ifndef __BS2_HOLIDAY_H__
#define __BS2_HOLIDAY_H__

#include "../BS2Types.h"

/**
 *	BS2Holiday constants
 */
enum {
	BS2_MAX_HOLIDAYS_PER_GROUP		= 128,
	BS2_MAX_HOLIDAY_GROUP_NAME_LEN	= 48 * 3,

	BS2_INVALID_HOLIDAY_GROUP_ID = 0,
};

/**
 *  BS2_HOLIDAY_RECURRENCE
 */
enum {
	BS2_HOLIDAY_RECURRENCE_NONE		= 0,
	BS2_HOLIDAY_RECURRENCE_YEARLY	= 1,
	BS2_HOLIDAY_RECURRENCE_MONTHLY	= 2,	///< not used yet
	BS2_HOLIDAY_RECURRENCE_WEEKLY	= 3,	///< not used yet
};

typedef uint8_t BS2_HOLIDAY_RECURRENCE;

/**
 *  BS2Holiday
 */
typedef struct {
	BS2_DATETIME			date;
	BS2_HOLIDAY_RECURRENCE	recurrence;
} BS2Holiday;

/**
 *  BS2HolidayGroup
 */
typedef struct {
	BS2_HOLIDAY_GROUP_ID	id;
	char					name[BS2_MAX_HOLIDAY_GROUP_NAME_LEN];
	uint8_t					numHolidays;
	uint8_t					reserved[3];
	BS2Holiday				holidays[BS2_MAX_HOLIDAYS_PER_GROUP];
} BS2HolidayGroup;

#endif	// __BS2_HOLIDAY_H__
