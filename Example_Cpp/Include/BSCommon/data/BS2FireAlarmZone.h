/**
 *  Fire Alarm Zone
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

#ifndef __BS2_FIRE_ALARM_ZONE_H__
#define __BS2_FIRE_ALARM_ZONE_H__

#include "../BS2Types.h"
#include "BS2Action.h"
#include "BS2Zone.h"
#include "BS2Trigger.h"

/**
 *  Constants
 */
enum {
	BS2_MAX_FIRE_SENSORS_PER_FIRE_ALARM_ZONE = 8,
	BS2_MAX_FIRE_ALARM_ACTION = 5,
	BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE	= 32,		// #doors + #lifts
};

#if 0
/**
 *  BS2_FIRE_SWITCH
 */
enum {
	BS2_FIRE_SWITCH_NORMAL_OPEN,
	BS2_FIRE_SWITCH_NORMAL_CLOSE,
};
#else
// Use BS2_SWITCH_TYPE instead.
#endif

typedef uint8_t BS2_FIRE_SWITCH;

typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	uint8_t			port;		///< 1 byte
	BS2_SWITCH_TYPE			switchType;		///< 1 byte
	uint16_t			duration;			///< 2 bytes
} BS2FireSensor;

typedef struct {
	BS2_ZONE_ID		zoneID;			///< 4 bytes
	char			name[BS2_MAX_ZONE_NAME_LEN];		///< 144 bytes

	uint8_t			numSensors;		///< 1 byte
	union {
		uint8_t			numDoors;		///< 1 byte
		uint8_t			numLifts;		///< 1 byte
	};
	BS2_BOOL 		alarmed;		///< 1 byte
	BS2_BOOL			disabled;		///< 1 byte

	uint8_t			reserved[8];		///< 8 bytes (packing)

	BS2FireSensor	sensor[BS2_MAX_FIRE_SENSORS_PER_FIRE_ALARM_ZONE];		///< 8 * 8 bytes
	BS2Action		alarm[BS2_MAX_FIRE_ALARM_ACTION];		///< 32 * 5 bytes

	uint8_t			reserved2[32];		///< 32 bytes (packing)

	union {
		BS2_DOOR_ID		doorIDs[BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE];		///< 4 * 32 bytes
		BS2_LIFT_ID		liftIDs[BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE];		///< 4 * 32 bytes
	};
} BS2FireAlarmZone;

#endif	// __BS2_FIRE_ALARM_ZONE_H__
