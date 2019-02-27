/**
 *  System Configuration Definitions
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

#ifndef __BS2_SYSTEM_CONFIG_EXT_H__
#define __BS2_SYSTEM_CONFIG_EXT_H__

#include "BS2Types.h"

enum {
	SEC_KEY_SIZE = 16,
};

/**
 *	BS2SystemConfigExt
 */
typedef struct {
	uint8_t primarySecureKey[SEC_KEY_SIZE];
	uint8_t secondarySecureKey[SEC_KEY_SIZE];

	uint8_t reserved3[32];                               ///< 32 bytes (reserved)
} BS2SystemConfigExt;

#endif	// __BS2_SYSTEM_CONFIG_EXT_H__
