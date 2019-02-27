/**
 *  Zone
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

#ifndef __BS2_ZONE_H__
#define __BS2_ZONE_H__

#include "BS2Types.h"

/**
 *  Zone Constants
 */
enum {
	BS2_MAX_ZONE_NAME_LEN				= 48 * 3,

	BS2_INVALID_ZONE_ID					= 0,
};

/**
 * BS2_ZONE_TYPE
 */
enum {
	BS2_ZONE_APB,
	BS2_ZONE_TIMED_APB,
	BS2_ZONE_FIRE_ALARM,
	BS2_ZONE_SCHEDULED_LOCK_UNLOCK,
	BS2_ZONE_INTRUSION_ALARM,
	BS2_ZONE_INTERLOCK_ALARM,
};

typedef uint8_t BS2_ZONE_TYPE;

/**
*	BS2_ZONE_STATUS
 */
enum {
	BS2_ZONE_STATUS_NORMAL             = 0x00,
	BS2_ZONE_STATUS_ALARM              = 0x01,
	BS2_ZONE_STATUS_SCHEDULED_LOCKED   = 0x02,
	BS2_ZONE_STATUS_SCHEDULED_UNLOCKED = 0x04,
	BS2_ZONE_STATUS_ARM                = 0x08,
	BS2_ZONE_STATUS_DISARM             = BS2_ZONE_STATUS_NORMAL,
};

typedef uint8_t BS2_ZONE_STATUS;

/**
 *  BS2ZoneStatus
 */
typedef struct {
	BS2_ZONE_ID         id;             ///< 4 bytes
	BS2_ZONE_STATUS     status;         ///< 1 byte
	BS2_BOOL            disabled;       ///< 1 byte
	uint8_t             reserved[6];    ///< 6 bytes (packing)
} BS2ZoneStatus;

#endif	// __BS2_ZONE_H__
