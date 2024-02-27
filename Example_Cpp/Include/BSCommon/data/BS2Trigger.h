/**
 *  Trigger Definitions
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

#ifndef __BS2_TRIGGER_H__
#define __BS2_TRIGGER_H__

#include "../BS2Types.h"
#include "BS2Event.h"

/**
 *  BS2_TRIGGER_TYPE
 */
enum {
	BS2_TRIGGER_NONE,

	BS2_TRIGGER_EVENT,
	BS2_TRIGGER_INPUT,
	BS2_TRIGGER_SCHEDULE,
};

typedef uint8_t	BS2_TRIGGER_TYPE;

/**
 *	BS2EventTrigger
 */
typedef struct {
	BS2_EVENT_CODE		code;		///< 2 bytes
	uint8_t reserved[2];		///< 2 bytes (packing)
} BS2EventTrigger;

/**
 *  BS2InputTrigger
 */
typedef struct {
	uint8_t				port;		///< 1 byte
	BS2_SWITCH_TYPE		switchType;		///< 1 byte
	uint16_t			duration;		///< 2 bytes
	BS2_SCHEDULE_ID		scheduleID;		///< 4 bytes
} BS2InputTrigger;

/**
*  BS2_SCHEDULE_TRIGGER_TYPE
*/
enum {
	BS2_SCHEDULE_TRIGGER_ON_START,
	BS2_SCHEDULE_TRIGGER_ON_END,
};

typedef uint32_t	BS2_SCHEDULE_TRIGGER_TYPE;

/**
*  BS2ScheduleTrigger
*/

typedef struct {
	BS2_SCHEDULE_TRIGGER_TYPE type;          ///< 4 bytes
	BS2_SCHEDULE_ID scheduleID;              ///< 4 bytes
} BS2ScheduleTrigger;

/**
 *  BS2Trigger
 */
typedef struct {
	BS2_DEVICE_ID		deviceID;		///< 4 bytes
	BS2_TRIGGER_TYPE	type;			///< 1 byte
	uint8_t				reserved;		///< 1 byte (packing)
	uint16_t			ignoreSignalTime;  ///< 2 bytes	

	union {
		BS2EventTrigger event;		///< 4 bytes
		BS2InputTrigger	input;		///< 8 bytes
		BS2ScheduleTrigger schedule;		///< 8 bytes
	};
} BS2Trigger;

#endif	// __BS2_TRIGGER_H__
