/**
 *  Operator Configuration
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

#ifndef __BS2_OPERATOR_H__
#define __BS2_OPERATOR_H__

#include "../BS2Types.h"
#include "../config/BS2AuthConfig.h"

typedef struct {
	BS2_USER_ID userID;							///< 32 bytes
	BS2_OPERATOR_LEVEL level;			///< 1 byte
	uint8_t reserved[3];								///< 3 bytes
} BS2Operator;

typedef BS2Operator BS2AuthOperatorLevel;

#endif	// __BS2_OPERATOR_H__
