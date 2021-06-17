/*
 * BS2IntrusionAlarmZone.h
 *
 *  Created on: 2017. 4. 27.
 *      Author: smlee
 */

#ifndef BS2INTRUSIONALARMZONE_H_
#define BS2INTRUSIONALARMZONE_H_

#include "../BS2Types.h"
#include "BS2Action.h"
#include "BS2Event.h"
#include "BS2Zone.h"

enum {
	BS2_MAX_READERS_IN_INTRUSION_ALARM_ZONE = 128,
	BS2_MAX_INPUTS_IN_INTRUSION_ALARM_ZONE = 128,
	BS2_MAX_OUTPUTS_IN_INTRUSION_ALARM_ZONE = 128,
	BS2_MAX_CARDS_IN_INTRUSION_ALARM_ZONE = 128,
	BS2_MAX_DOORS_IN_INTRUSION_ALARM_ZONE = 128/2,
	BS2_MAX_GROUPS_IN_INTRUSION_ALARM_ZONE = 128,
};

enum {
	INTRUSION_ALARM_ZONE_INPUT_MASK_NONE = 0x00,
	INTRUSION_ALARM_ZONE_INPUT_MASK_CARD = 0x01,
	INTRUSION_ALARM_ZONE_INPUT_MASK_KEY = 0x02,
	INTRUSION_ALARM_ZONE_INPUT_MASK_ALL = 0xFF,
};

enum {
	INTRUSION_ALARM_ZONE_OPERATION_MASK_NONE = 0x00,
	INTRUSION_ALARM_ZONE_OPERATION_MASK_ARM = 0x01,
	INTRUSION_ALARM_ZONE_OPERATION_MASK_DISARM = 0x02,
	INTRUSION_ALARM_ZONE_OPERATION_MASK_TOGGLE = INTRUSION_ALARM_ZONE_OPERATION_MASK_ARM | INTRUSION_ALARM_ZONE_OPERATION_MASK_DISARM,
	INTRUSION_ALARM_ZONE_OPERATION_MASK_ALARM = 0x04,
	INTRUSION_ALARM_ZONE_OPERATION_MASK_ALARM_CLEAR = 0x08,
};

typedef struct {
	BS2_DEVICE_ID deviceID;  ///< 4 bytes
	uint8_t inputType;       ///< 1 byte  - INTRUSION_ALARM_ZONE_INPUT_MASK_CARD | INTRUSION_ALARM_ZONE_INPUT_MASK_KEY
	uint8_t operationType;  ///< 1 byte - INTRUSION_ALARM_ZONE_OPERATION_MASK_[ARM|DISARM|TOGGLE]
	uint8_t reserved[2];     ///< 2 bytes (packing)
} BS2AlarmZoneMember;

typedef struct {
	BS2_DEVICE_ID deviceID;  ///< 4 bytes

	uint8_t port;  ///< 1 byte
	BS2_SWITCH_TYPE switchType;  ///< 1 byte
	uint16_t duration;   ///< 2 bytes

	uint8_t operationType;      ///< 1 byte - INTRUSION_ALARM_ZONE_OPERATION_MASK_*
	uint8_t reserved[3];     /// 3 bytes (packing)
} BS2AlarmZoneInput;

typedef struct {
	BS2_EVENT_CODE event;  ///< 2 byte - BS2_EVENT_ZONE_INTRUSION_ALARM_[VIOLATION|(DIS)ARM(ED|_FAIL)|ALARM(_[INPUT|CLEAR])]
	uint8_t reserved[2];     /// 2 bytes (packing)
	BS2Action action;		///< 32 bytes
} BS2AlarmZoneOutput;

typedef struct {
	BS2_ZONE_ID zoneID;   ///< 4 bytes
	char name[BS2_MAX_ZONE_NAME_LEN];  ///< 144 bytes

	uint8_t armDelay;  ///< 1 byte
	uint8_t alarmDelay;  ///< 1 byte
	BS2_BOOL disabled;  ///< 1 byte
	uint8_t reserved[1];  ///< 1 byte (packing)

	uint8_t numReaders;  ///< 1 byte
	uint8_t numInputs;  ///< 1 byte
	uint8_t numOutputs;  ///< 1 byte
	uint8_t numCards;  ///< 1 byte

	uint8_t numDoors;  ///< 1 byte
	uint8_t numGroups;  ///< 1 byte
	uint8_t reserved2[10];  ///< 10 bytes (packing)

#if 0
	BS2AlarmZoneMember readers[numReaders];
	BS2AlarmZoneInput inputs[numInputs];
	BS2AlarmZoneOutput outputs[numOutputs];
	BS2Card cards[numCards];
	BS2_DOOR_ID doorIDs[numDoors];
	BS2_ACCESS_GROUP_ID groupIDs[numGroups];
#endif

} BS2IntrusionAlarmZone;

#endif /* BS2INTRUSIONALARMZONE_H_ */
