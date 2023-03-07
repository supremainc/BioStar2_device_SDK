/**
 *  Lift
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

#ifndef __BS2_LIFT_H__
#define __BS2_LIFT_H__

#include "../BS2Types.h"
#include "data/BS2Action.h"

/**
 *  Lift Constants
 */
enum {
	BS2_MAX_LIFT_NAME_LEN				= 48 * 3,

	BS2_MAX_DEVICES_ON_LIFT = 4,
	BS2_MAX_FLOORS_ON_LIFT		= 255,

	BS2_MAX_ALARMS_ON_LIFT = 2,
	BS2_MAX_DUAL_AUTH_APPROVAL_GROUP_ON_LIFT	= 16,

	BS2_DEFAULT_ACTIVATE_TIMEOUT_ON_LIFT	= 10,	///< in seconds
	BS2_DEFAULT_DUAL_AUTH_TIMEOUT_ON_LIFT		= 15,	///< in seconds
	BS2_INVALID_LIFT_ID	= 0,
};

/**
 *	BS2_DUAL_AUTH_APPROVAL_TYPE
 */
enum {
	BS2_DUAL_AUTH_APPROVAL_NONE_ON_LIFT			= 0,
	BS2_DUAL_AUTH_APPROVAL_LAST_ON_LIFT			= 1,
//	BS2_DUAL_AUTH_APPROVAL_BOTH			= 2,
};

typedef uint8_t BS2_DUAL_AUTH_APPROVAL;

/**
 *	BS2_FLOOR_FLAG
 */
enum {
	BS2_FLOOR_FLAG_NONE = 0x00,
	BS2_FLOOR_FLAG_SCHEDULE = 0x01,
	BS2_FLOOR_FLAG_OPERATOR = 0x04,
	BS2_FLOOR_FLAG_ACTION = 0x08,
	BS2_FLOOR_FLAG_EMERGENCY = 0x02,
};

typedef uint8_t BS2_FLOOR_FLAG;

/**
 *  BS2FloorStatus
 */
typedef struct {
	BS2_BOOL				activated;			///< 1 byte
	BS2_FLOOR_FLAG			activateFlags;	///< 1 byte
	BS2_FLOOR_FLAG			deactivateFlags;		///< 1 byte
} BS2FloorStatus;

/**
 *	BS2LiftFloor
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	uint8_t			port;			///< 1 byte : 1 ~ 16
	BS2FloorStatus status;		///< 3 bytes
} BS2LiftFloor;

/**
 *	BS2LiftSensor
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	uint8_t			port;		///< 1 byte
	BS2_SWITCH_TYPE			switchType;		///< 1 byte
	uint16_t			duration;			///< 2 bytes
	BS2_SCHEDULE_ID		scheduleID;		///< 4 bytes
} BS2LiftSensor;

typedef struct {
	BS2LiftSensor sensor;		///< 12 bytes
	BS2Action action;		///< 32 bytes
} BS2LiftAlarm;

/**
 * BS2_LIFT_ALARM_FLAG
 */
enum {
	BS2_LIFT_ALARM_FLAG_NONE = 0x00,
	BS2_LIFT_ALARM_FLAG_FIRST = 0x01,
	BS2_LIFT_ALARM_FLAG_SECOND = 0x02,
	BS2_LIFT_ALARM_FLAG_TAMPER = 0x04,
};

typedef uint8_t BS2_LIFT_ALARM_FLAG;

/**
 *  BS2LiftStatus
 */
typedef struct {
	BS2_LIFT_ID liftID;		///< 4 bytes
	uint16_t numFloors;		///< 2 bytes
	BS2_LIFT_ALARM_FLAG alarmFlags;			///< 1 byte
	BS2_BOOL tamperOn;			///< 1 byte
	BS2FloorStatus floors[BS2_MAX_FLOORS_ON_LIFT];		///< 3 * 255 bytes
	uint8_t reserved[3];			///< 3 bytes padding
} BS2LiftStatus;

/**
 *	BS2Lift
 */
typedef struct {
	BS2_LIFT_ID		liftID;				///< 4 bytes
	char			name[BS2_MAX_LIFT_NAME_LEN];		///< 144 bytes

	BS2_DEVICE_ID	deviceID[BS2_MAX_DEVICES_ON_LIFT];		///< 4 * 4 bytes

	uint32_t		activateTimeout;		///< 4 bytes (in seconds)
	uint32_t		dualAuthTimeout;			///< 4 bytes

	uint8_t	numFloors;		///< 1 byte
	uint8_t	numDualAuthApprovalGroups;	///< 1 byte
	BS2_DUAL_AUTH_APPROVAL	dualAuthApprovalType;		///< 1 byte
	BS2_BOOL	tamperOn;		///< 1 byte

	BS2_BOOL dualAuthRequired[BS2_MAX_DEVICES_ON_LIFT];			///< 1 * 4 byte
	BS2_SCHEDULE_ID			dualAuthScheduleID;			///< 4 bytes

	BS2LiftFloor		floor[BS2_MAX_FLOORS_ON_LIFT];				///< 8 * 255 bytes
	BS2_ACCESS_GROUP_ID		dualAuthApprovalGroupID[BS2_MAX_DUAL_AUTH_APPROVAL_GROUP_ON_LIFT];		///< 4 * 16 bytes

	BS2LiftAlarm alarm[BS2_MAX_ALARMS_ON_LIFT];		///< 44 * 2 bytes
	BS2LiftAlarm tamper;			///< 44 bytes

	BS2_LIFT_ALARM_FLAG	alarmFlags;		///< 1 byte
	uint8_t reserved[3];			///< 3 bytes (packing)
} BS2Lift;

#endif	// __BS2_LIFT_H__
