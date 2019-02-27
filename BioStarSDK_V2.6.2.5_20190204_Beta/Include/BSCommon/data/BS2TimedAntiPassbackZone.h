/*
 * BS2TimedAntiPassbackZone.h
 *
 *  Created on: 2014. 12. 9.
 *      Author: smlee
 */

#ifndef BS2TIMEDANTIPASSBACKZONE_H_
#define BS2TIMEDANTIPASSBACKZONE_H_

#include "BS2Types.h"
#include "BS2Action.h"
#include "BS2Zone.h"

enum {
	BS2_MAX_READERS_PER_TIMED_APB_ZONE = 64,
	BS2_MAX_BYPASS_GROUPS_PER_TIMED_APB_ZONE = 16,
	BS2_MAX_TIMED_APB_ALARM_ACTION			= 5,
};

/**
 *  BS2_TIMED_APB_ZONE_TYPE
 */
enum {
	BS2_TIMED_APB_ZONE_HARD		= 0x00,
	BS2_TIMED_APB_ZONE_SOFT		= 0x01,
};

typedef uint8_t BS2_TIMED_APB_ZONE_TYPE;

typedef struct {
	BS2_DEVICE_ID deviceID;			///< 4 bytes
	uint8_t reserved[4];			///< 4 bytes (packing)
} BS2TimedApbMember;

typedef struct {
	BS2_ZONE_ID zoneID;		///< 4 bytes
	char name[BS2_MAX_ZONE_NAME_LEN];		///< 48 * 3 bytes

	BS2_TIMED_APB_ZONE_TYPE type;		///< 1 byte
	uint8_t numReaders;		///< 1 byte
	uint8_t numBypassGroups;		///< 1 byte
	BS2_BOOL disabled;				///< 1 byte

	BS2_BOOL alarmed;		///< 1 byte
	uint8_t reserved[3];			///< 3 bytes (packing)

	uint32_t resetDuration;		///< 4 bytes: in seconds, 0: no reset

	BS2Action alarm[BS2_MAX_TIMED_APB_ALARM_ACTION];		///< 32 * 5 bytes

	BS2TimedApbMember readers[BS2_MAX_READERS_PER_TIMED_APB_ZONE];		///< 8 * 64 bytes
	uint8_t reserved2[8 * 40];		///< 8 * 40 bytes (packing)

	BS2_ACCESS_GROUP_ID bypassGroupIDs[BS2_MAX_BYPASS_GROUPS_PER_TIMED_APB_ZONE];		///< 4 * 16 bytes
} BS2TimedAntiPassbackZone;

#endif /* BS2TIMEDANTIPASSBACKZONE_H_ */
