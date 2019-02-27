/**
 *  Day Schedule Definitions
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

#ifndef __BS2_DAY_SCHEDULE_H__
#define __BS2_DAY_SCHEDULE_H__

#include "BS2Types.h"

/**
 *  Day Schedule constants
 */
enum {
	BS2_MAX_TIME_PERIODS_PER_DAY	= 5,
};

/**
 *  BS2TimePeriod
 */
typedef struct {
	int16_t		startTime;	///< in minutes: negative value means time of the day before
	int16_t		endTime;	///< in minutes: positive value over 1440 means time of the day after
} BS2TimePeriod;

/**
 *  BS2DaySchedule
 */
typedef struct {
	uint8_t			numPeriods;		///< 1 byte
	uint8_t			reserved[3];	///< 3 bytes (packing)
	BS2TimePeriod	periods[BS2_MAX_TIME_PERIODS_PER_DAY];
} BS2DaySchedule;

#endif	// __BS2_DAY_SCHEDULE_H__
