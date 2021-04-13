/**
 *  Auth Group Definitions
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

#ifndef __AUTH_GROUP_H__
#define __AUTH_GROUP_H__

#include "../BS2Types.h"

enum {
	BS2_MAX_AUTH_GROUP_ITEMS		= 10,
	BS2_MAX_AUTH_GROUP_NAME_LEN	= 48 * 3,

	BS2_INVALID_AUTH_GROUP_ID = 0,
};

/**
 *	BS2AuthGroup
 */
typedef struct {
	BS2_AUTH_GROUP_ID		id;		///< 4 bytes
	char					name[BS2_MAX_AUTH_GROUP_NAME_LEN];		///< 144 bytes
	uint8_t					reserved[32];			///< 32 bytes (packing)
} BS2AuthGroup;

/**
 *	BS2AuthGroupUsers
 */
typedef struct {
	BS2_AUTH_GROUP_ID	authGroupID;	///< 4 bytes
	uint32_t			numUsers;		///< 4 bytes
	BS2_USER_ID			*userID;		///< 32 bytes * n
} BS2AuthGroupUsers;

#endif	// __AUTH_GROUP_H__
