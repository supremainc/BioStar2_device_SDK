/*
 * BS2ScheduledLockUnlockZone.h
 *
 *  Created on: 2014. 12. 9.
 *      Author: smlee
 */

#ifndef BS2SCHEDULEDLOCKUNLOCKZONE_H_
#define BS2SCHEDULEDLOCKUNLOCKZONE_H_

#include "../BS2Types.h"
#include "BS2Action.h"
#include "BS2Zone.h"

enum {
	BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION	 = 5,
	BS2_MAX_DOORS_IN_SCHEDULED_LOCK_UNLOCK_ZONE = 32,
	BS2_MAX_BYPASS_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE = 16,
	BS2_MAX_UNLOCK_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE = 16,
};

typedef struct {
	BS2_ZONE_ID zoneID;		///< 4 bytes
	char name[BS2_MAX_ZONE_NAME_LEN];		///< 48 * 3 bytes

	BS2_SCHEDULE_ID lockScheduleID;		///< 4 bytes
	BS2_SCHEDULE_ID unlockScheduleID;		///< 4 bytes

	uint8_t numDoors;		///< 1 byte
	uint8_t numBypassGroups;		///< 1 byte
	uint8_t numUnlockGroups;		///< 1 byte
	BS2_BOOL bidirectionalLock;		///< 1 byte

	BS2_BOOL disabled;		///< 1 byte
	BS2_BOOL alarmed;		///< 1 byte
	uint8_t reserved[6];		///< 6 bytes (packing)

	BS2Action alarm[BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION];		///< 32 * 5 bytes

	uint8_t reserved2[32];		///< 32 bytes (packing)

	BS2_DOOR_ID doorIDs[BS2_MAX_DOORS_IN_SCHEDULED_LOCK_UNLOCK_ZONE];		///< 4 * 32 bytes
	BS2_ACCESS_GROUP_ID bypassGroupIDs[BS2_MAX_BYPASS_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE];		///< 4 * 16 bytes
	BS2_ACCESS_GROUP_ID unlockGroupIDs[BS2_MAX_UNLOCK_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE];		///< 4 * 16 bytes
} BS2ScheduledLockUnlockZone;

#endif /* BS2SCHEDULEDLOCKUNLOCKZONE_H_ */
