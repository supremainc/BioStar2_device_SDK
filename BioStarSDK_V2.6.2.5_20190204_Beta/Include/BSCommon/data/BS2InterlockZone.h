/*
 * BS2InterlockZone.h
 *
 *  Created on: 2017. 10. 17.
 *      Author: idkim
 */

#ifndef BS2INTERLOCKZONE_H_
#define BS2INTERLOCKZONE_H_

#include "BS2Types.h"
#include "BS2Action.h"
#include "BS2Event.h"
#include "BS2Zone.h"

enum {
	BS2_MAX_INPUTS_IN_INTERLOCK_ZONE = 4,//128,
	BS2_MAX_OUTPUTS_IN_INTERLOCK_ZONE = 8,//128,
	BS2_MAX_DOORS_IN_INTERLOCK_ZONE = 4,//128/2,
};

enum {
	INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_NONE = 0x00,
	INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_ENRTY = 0x01,
	INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_EXIT = 0x02,
	INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_ALL = 0xFF,
};

typedef struct {
	BS2_DEVICE_ID deviceID;  ///< 4 bytes

	uint8_t port;  ///< 1 byte
	BS2_SWITCH_TYPE switchType;  ///< 1 byte
	uint16_t duration;   ///< 2 bytes

	uint8_t operationType;      ///< 1 byte - INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_*
	uint8_t reserved[3];     	/// 3 bytes (packing)
} BS2InterlockZoneInput;

typedef struct {
	BS2_EVENT_CODE event;  ///< 2 byte -  BS2_EVENT_ZONE_INTERLOCK_ALARM_[INDOOR | DOOR_OPEN]_DENIED
	uint8_t reserved[2];     /// 2 bytes (packing)
	BS2Action action;
} BS2InterlockZoneOutput;

typedef struct {
	BS2_ZONE_ID zoneID;   ///< 4 bytes
	char name[BS2_MAX_ZONE_NAME_LEN];  ///< 48 * 3 bytes

	BS2_BOOL disabled;  ///< 1 byte
	uint8_t numInputs;  ///< 1 byte
	uint8_t numOutputs; ///< 1 byte
	uint8_t numDoors;   ///< 1 byte

	uint8_t reserved[8];  ///< 8 bytes (packing)

#if 0
	BS2InterlockZoneInput inputs[numInputs];
	BS2InterlockZoneOutput outputs[numOutputs];
	BS2_DOOR_ID doorIDs[numDoors];
#endif

} BS2InterlockZone;

#endif /* BS2INTERLOCKZONE_H_ */
