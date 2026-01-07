/*
 *  Copyright (c) 2024 Suprema Co., Ltd. All Rights Reserved.
 *
 *  This software is the confidential and proprietary information of
 *  Suprema Co., Ltd. ("Confidential Information").  You shall not
 *  disclose such Confidential Information and shall use it only in
 *  accordance with the terms of the license agreement you entered into
 *  with Suprema.
 */
#ifndef __BS2_LOCK_OVERRIDES_H__
#define __BS2_LOCK_OVERRIDES_H__

#include "../BS2Types.h"
#include "../data/BS2CSNCard.h"

enum{
	BS2_MAX_LOCKOVERRIDE = 1000,	///< Maximum number of lock overrides
};

typedef struct {
	uint8_t cardID[BS2_CARD_DATA_SIZE];		///< 32 bytes
	uint16_t issueCount;					///< 2 bytes	

	BS2_CARD_TYPE type;						///< 1 byte
	uint8_t	size;							///< 1 byte

	BS2_USER_ID userID;						///< 32 bytes
	uint8_t reserved[4];					///< 4 bytes

} BS2LockOverride;


#endif //__BS2_LOCK_OVERRIDES_H__
