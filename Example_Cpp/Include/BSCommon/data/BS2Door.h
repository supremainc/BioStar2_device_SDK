/**
 *  Door
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

#ifndef __BS2_DOOR_H__
#define __BS2_DOOR_H__

#include "../BS2Types.h"
#include "BS2Action.h"
#include "BS2AntiPassbackZone.h"

/**
 *  Door Constants
 */
enum {
	BS2_MAX_DOOR_NAME_LEN				= 48 * 3,

	BS2_MAX_DUAL_AUTH_APPROVAL_GROUP	= 16,
	BS2_DEFAULT_AUTO_LOCK_TIMEOUT		= 3,	///< in seconds
	BS2_DEFAULT_HELD_OPEN_TIMEOUT		= 10,	///< in seconds
	BS2_DEFAULT_DUAL_AUTH_TIMEOUT		= 15,	///< in seconds
	BS2_INVALID_DOOR_ID					= 0,

	BS2_MAX_HELD_OPEN_ALARM_ACTION		= 5,
	BS2_MAX_FORCED_OPEN_ALARM_ACTION	= 5,
};

/**
 *	BS2_DUAL_AUTH_APPROVAL_TYPE
 */
enum {
	BS2_DUAL_AUTH_APPROVAL_NONE			= 0,
	BS2_DUAL_AUTH_APPROVAL_LAST			= 1,
//	BS2_DUAL_AUTH_APPROVAL_BOTH			= 2,
};

typedef uint8_t BS2_DUAL_AUTH_APPROVAL;

/**
 *	BS2_DUAL_AUTH_TYPE
 */
enum {
	BS2_DUAL_AUTH_NO_DEVICE				= 0,
	BS2_DUAL_AUTH_ENTRY_DEVICE_ONLY		= 1,
	BS2_DUAL_AUTH_EXIT_DEVICE_ONLY		= 2,
	BS2_DUAL_AUTH_BOTH_DEVICE			= 3,
};

typedef uint8_t BS2_DUAL_AUTH_DEVICE;

#if 0
/**
 *  BS2_DOOR_SWITCH
 */
enum {
	BS2_DOOR_SWITCH_NORMAL_OPEN,
	BS2_DOOR_SWITCH_NORMAL_CLOSE,
};

typedef uint8_t BS2_DOOR_SWITCH;
#else
// Use BS2_SWITCH_TYPE instead.
#endif

/**
 *	BS2DoorRelay
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	uint8_t			port;			///< 1 byte : 1 ~ 16
	uint8_t			reserved[3];	///< 3 bytes (packing)
} BS2DoorRelay;

/**
 *	BS2DoorSensor
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	uint8_t			port;			///< 1 byte : 1 ~ 16
	BS2_SWITCH_TYPE			switchType;		///< 0 = N/O, 1 = N/C	
	uint8_t					apbUseDoorSensor;	
	uint8_t			reserved[1];	///< 2 bytes (packing)
} BS2DoorSensor;

/**
 *	BS2ExitButton
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	uint8_t			port;			///< 1 byte : 1 ~ 16
	BS2_SWITCH_TYPE			switchType;		///< 0 = N/O, 1 = N/C
	uint8_t			reserved[2];	///< 2 bytes (packing)
} BS2ExitButton;

/**
 *  BS2_DOOR_ALARM
 */
enum {
	BS2_DOOR_ALARM_NONE			= 0,
	BS2_DOOR_ALARM_HELD_OPEN	= 1,
	BS2_DOOR_ALARM_FORCED_OPEN	= 2,
};

typedef uint8_t BS2_DOOR_ALARM;

#if 0
/**
 *  BS2_DOOR_RELAY
 */
enum {
	BS2_DOOR_RELAY_OFF		= 0,
	BS2_DOOR_RELAY_ON		= 1,
};

typedef uint8_t BS2_DOOR_RELAY;
#endif

/**
 *	BS2_DOOR_FLAG
 */
enum {
	BS2_DOOR_FLAG_NONE = 0x00,
	BS2_DOOR_FLAG_SCHEDULE = 0x01,
	BS2_DOOR_FLAG_OPERATOR = 0x04,
	BS2_DOOR_FLAG_EMERGENCY = 0x02,
};

typedef uint8_t BS2_DOOR_FLAG;

/**
 *	BS2_DOOR_ALARM_FLAG
 */
enum {
	BS2_DOOR_ALARM_FLAG_NONE = 0x00,
	BS2_DOOR_ALARM_FLAG_FORCED_OPEN = 0x01,
	BS2_DOOR_ALARM_FLAG_HELD_OPEN = 0x02,
	BS2_DOOR_ALARM_FLAG_APB = 0x04,
};

typedef uint8_t BS2_DOOR_ALARM_FLAG;

/**
 *  BS2DoorStatus
 */
typedef struct {
	BS2_DOOR_ID				id;				///< 4 bytes
	BS2_BOOL				opened;			///< 1 byte
	BS2_BOOL				unlocked;		///< 1 byte
	BS2_BOOL				heldOpened;		///< 1 byte
	BS2_DOOR_FLAG			unlockFlags;	///< 1 byte
	BS2_DOOR_FLAG			lockFlags;		///< 1 byte
	BS2_DOOR_ALARM_FLAG	alarmFlags;		///< 1 byte
	uint8_t					reserved[2];	///< 2 bytes (packing)
	BS2_TIMESTAMP			lastOpenTime;	///< 4 bytes
} BS2DoorStatus;

/**
 *	BS2Door
 */
typedef struct {
	BS2_DOOR_ID		doorID;				///< 4 bytes
	char			name[BS2_MAX_DOOR_NAME_LEN];

	BS2_DEVICE_ID	entryDeviceID;		///< 4 bytes
	BS2_DEVICE_ID	exitDeviceID;		///< 4 bytes

	BS2DoorRelay	relay;				///< 8 bytes
	BS2DoorSensor	sensor;				///< 8 bytes
	BS2ExitButton	button;				///< 8 bytes

	uint32_t		autoLockTimeout;	///< 4 bytes (in seconds)
	uint32_t		heldOpenTimeout;	///< 4 bytes (in seconds)

	BS2_BOOL		instantLock;		///< 1 byte	- The door will be locked when it is closed, even though autoLock timeout is not expired.
	BS2_DOOR_FLAG			unlockFlags;	///< 1 byte
	BS2_DOOR_FLAG			lockFlags;		///< 1 byte
#if 0		// TIIDFI-521, ENTRYIIDII-257
	BS2_DOOR_ALARM_FLAG					alarmFlags;				///< 1 byte (packing)
#else
	BS2_BOOL	unconditionalLock;				///< 1 byte -  The door will be locked after autoLock timeout, even though it is open.
#endif

	BS2Action		forcedOpenAlarm[BS2_MAX_FORCED_OPEN_ALARM_ACTION];
	BS2Action		heldOpenAlarm[BS2_MAX_HELD_OPEN_ALARM_ACTION];

	BS2_SCHEDULE_ID			dualAuthScheduleID;			///< 4 bytes
	BS2_DUAL_AUTH_DEVICE	dualAuthDevice;				///< 1 byte
	BS2_DUAL_AUTH_APPROVAL	dualAuthApprovalType;		///< 1 byte
	uint32_t				dualAuthTimeout;			///< 4 bytes
	uint8_t					numDualAuthApprovalGroups;	///< 1 byte
	uint8_t					reserved2[1];				///< 1 byte (packing)
	BS2_ACCESS_GROUP_ID		dualAuthApprovalGroupID[BS2_MAX_DUAL_AUTH_APPROVAL_GROUP];

	BS2AntiPassbackZone		apbZone;
} BS2Door;

#endif	// __BS2_DOOR_H__
