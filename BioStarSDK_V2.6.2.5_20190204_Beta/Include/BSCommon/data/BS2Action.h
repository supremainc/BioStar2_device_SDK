/**
 *  Action Definitions
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

#ifndef __BS2_ACTION_H__
#define __BS2_ACTION_H__

#include "BS2Types.h"
#include "BS2Resource.h"

/**
 *	Constants
 */
enum {
	BS2_BUZZER_SIGNAL_NUM	= 3,
	BS2_LED_SIGNAL_NUM		= 3,
};

/**
 *  BS2_ACTION_TYPE
 */
enum {
	BS2_ACTION_NONE = 0,

	BS2_ACTION_LOCK_DEVICE,
	BS2_ACTION_UNLOCK_DEVICE,
	BS2_ACTION_REBOOT_DEVICE,
	BS2_ACTION_RELEASE_ALARM,
	BS2_ACTION_GENERAL_INPUT,

	BS2_ACTION_RELAY,
	BS2_ACTION_TTL,
	BS2_ACTION_SOUND,
	BS2_ACTION_DISPLAY,
	BS2_ACTION_BUZZER,
	BS2_ACTION_LED,

	BS2_ACTION_FIRE_ALARM_INPUT,

	BS2_ACTION_AUTH_SUCCESS,
	BS2_ACTION_AUTH_FAIL,

	BS2_ACTION_LIFT,

	BS2_ACTION_NUM,
};

typedef uint8_t	BS2_ACTION_TYPE;

/**
 *  BS2Signal
 */
typedef struct {
	BS2_UID		signalID;		///< 4 bytes
	uint16_t	count;			///< 2 bytes
	uint16_t	onDuration;		///< 2 bytes
	uint16_t	offDuration;	///< 2 bytes
	uint16_t	delay;			///< 2 bytes
} BS2Signal;

/**
 *  BS2OutputPortAction
 */
typedef struct {
	uint8_t		portIndex;		///< 1 byte
	uint8_t		reserved[3];	///< 3 bytes (packing)
	BS2Signal	signal;				///< 12 bytes
} BS2OutputPortAction;

/**
 *  BS2RelayAction
 */
typedef struct {
	uint8_t		relayIndex;		///< 1 byte
	uint8_t		reserved[3];	///< 3 bytes (packing)
	BS2Signal	signal;				///< 12 bytes
} BS2RelayAction;

/**
 *	BS2ReleaseAlarmAction
 */
typedef struct {
	uint8_t targetType;			///< 1 byte : Device, Door, Zone
	uint8_t reserved[3];		///< 3 bytes
	union {
		BS2_DEVICE_ID deviceID;
		BS2_DOOR_ID doorID;
		BS2_ZONE_ID zoneID;
	};
} BS2ReleaseAlarmAction;

/**
 *	BS2_LED_COLOR
 */
enum {
	BS2_LED_COLOR_OFF,
	BS2_LED_COLOR_RED,
	BS2_LED_COLOR_YELLOW,
	BS2_LED_COLOR_GREEN,
	BS2_LED_COLOR_CYAN,
	BS2_LED_COLOR_BLUE,
	BS2_LED_COLOR_MAGENTA,
	BS2_LED_COLOR_WHITE,
};

typedef uint8_t BS2_LED_COLOR;

/**
 *	BS2LedSignal
 */
typedef struct {
	BS2_LED_COLOR	color;			///< 1 byte
	uint8_t			reserved[1];	///< 1 bytes (packing)
	uint16_t		duration;		///< 2 bytes
	uint16_t		delay;			///< 2 bytes
} BS2LedSignal;

/**
 *  BS2LedAction
 */
typedef struct {
	uint16_t		count;			///< 2 bytes (0 = infinite)
	uint8_t			reserved[2];	///< 2 bytes (packing)
	BS2LedSignal	signal[BS2_LED_SIGNAL_NUM];		///< 18 bytes
} BS2LedAction;

/**
 *	BS2_BUZZER_TONE
 */
enum {
	BS2_BUZZER_TONE_OFF,
	BS2_BUZZER_TONE_LOW,
	BS2_BUZZER_TONE_MIDDLE,
	BS2_BUZZER_TONE_HIGH,
};

typedef uint8_t BS2_BUZZER_TONE;

/**
 *	BS2BuzzerSignal
 */
typedef struct {
	BS2_BUZZER_TONE	tone;			///< 1 byte
	BS2_BOOL		fadeout;        ///< 1 byte
	uint16_t		duration;   	///< 2 bytes
	uint16_t		delay;			///< 2 bytes
} BS2BuzzerSignal;

/**
 *	BS2BuzzerAction
 */
typedef struct {
	uint16_t		count;			///< 2 bytes (0 = infinite)
	uint8_t			reserved[2];	///< 2 bytes (packing)
	BS2BuzzerSignal	signal[BS2_BUZZER_SIGNAL_NUM];		///< 18 bytes
} BS2BuzzerAction;

/**
 *  BS2DisplayAction
 *  @note	not used yet
 */
typedef struct {
	uint8_t		duration;			///< 1 byte
	uint8_t		reserved[3];		///< 3 bytes (packing)
	BS2_UID		displayID;			///< 4 bytes
	BS2_UID		resourceID;			///< 4 bytes
} BS2DisplayAction;

/**
 *  BS2SoundAction
 */
typedef struct {
	uint8_t		count;                       ///< 1 byte
	BS2_SOUND_INDEX		soundIndex;          ///< 2 bytes
	uint16_t delay;													///< 2 bytes - deprecated
	uint8_t		reserved[15];                 ///< 15 bytes (packing)
} BS2SoundAction;

/**
 * BS2_LIFT_ACTION_TYPE
 */
enum {
	BS2_LIFT_ACTION_ACTIVATE_FLOORS,
	BS2_LIFT_ACTION_DEACTIVATE_FLOORS,
	BS2_LIFT_ACTION_RELEASE_FLOORS,
};

typedef uint8_t BS2_LIFT_ACTION_TYPE;

/**
 * BS2LiftAction
 */
typedef struct {
	BS2_LIFT_ID liftID;
	BS2_LIFT_ACTION_TYPE type;
} BS2LiftAction;

/**
 *  BS2_STOP_FLAG
 */
enum {
	BS2_STOP_NONE = 0,

	BS2_STOP_ON_DOOR_CLOSED = 0x01,		// When the door is closed, its alarm action should be stopped.

	BS2_STOP_BY_CMD_RUN_ACTION = 0x02,

	BS2_STOP_NUM,
};

typedef uint8_t	BS2_STOP_FLAG;

/**
 *  BS2Action
 */
typedef struct {
	BS2_DEVICE_ID		deviceID;		///< 4 bytes
	BS2_ACTION_TYPE		type;			///< 1 byte
	BS2_STOP_FLAG stopFlag;			///< 1 byte
	uint16_t				delay;	///< 2 bytes
	union {
		BS2RelayAction		relay;		///< type = BS2_ACTION_RELAY
		BS2OutputPortAction	outputPort;	///< type = BS2_ACTION_TTL
		BS2DisplayAction	display;	///< type = BS2_ACTION_DISPLAY
		BS2SoundAction		sound;		///< type = BS2_ACTION_SOUND
		BS2LedAction		led;		///< type = BS2_ACTION_LED
		BS2BuzzerAction		buzzer;		///< type = BS2_ACTION_BUZZER
		BS2LiftAction lift;			///< type = BS2_ACTION_LIFT
	};
} BS2Action;

#endif	// __BS2_ACTION_H__
