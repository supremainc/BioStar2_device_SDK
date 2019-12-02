/**
 *  Access Group
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

#ifndef __BS2_ACCESS_GROUP_H__
#define __BS2_ACCESS_GROUP_H__

#include "../BS2Types.h"
#include "BS2User.h"
#include "BS2AccessLevel.h"
#include "BS2FloorLevel.h"

enum {
	BS2_MAX_ACCESS_LEVEL_PER_ACCESS_GROUP	= 128,		// #accessLevels + #floorLevels
	BS2_MAX_ACCESS_GROUP_PER_USER			= 16,
	BS2_MAX_ACCESS_GROUP_NAME_LEN			= 48 * 3,

	BS2_INVALID_ACCESS_GROUP_ID = 0,
};

/**
 *	BS2AccessGroupUsers
 */
typedef struct {
	BS2_ACCESS_GROUP_ID	accessGroupID;	///< 4 bytes
	uint32_t			numUsers;		///< 4 bytes
	BS2_USER_ID			*userID;		///< 32 bytes * n
} BS2AccessGroupUsers;

/**
 *	BS2UserAccessGroups
 */
typedef struct {
	uint8_t				numAccessGroups;	///< 1 byte
	uint8_t				reserved[3];		///< 3 bytes (packing)
	BS2_ACCESS_GROUP_ID	accessGroupID[BS2_MAX_ACCESS_GROUP_PER_USER];	///< 64 bytes
} BS2UserAccessGroups;

/**
 *  BS2AccessGroup
 */
typedef struct {
	BS2_ACCESS_GROUP_ID		id;	///< 4 bytes
	char					name[BS2_MAX_ACCESS_GROUP_NAME_LEN];	///< 144 bytes
	union {
		uint8_t					numAccessLevels;	///< 1 byte
		uint8_t					numFloorLevels;	///< 1 byte
	};
	uint8_t					reserved[3];	///< 3 bytes (packing)
	union {
		BS2_ACCESS_LEVEL_ID		accessLevels[BS2_MAX_ACCESS_LEVEL_PER_ACCESS_GROUP];	///< 4 * 128 bytes
		BS2_FLOOR_LEVEL_ID		floorLevels[BS2_MAX_ACCESS_LEVEL_PER_ACCESS_GROUP];	///< 4 * 128 bytes
	};
} BS2AccessGroup;

#endif	// __BS2_ACCESS_GROUP_H__
