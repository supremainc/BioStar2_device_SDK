/**
 *  Trigger Action Configuration Definitions
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

#ifndef __BS2_TRIGGER_ACTION_CONFIG_H__
#define __BS2_TRIGGER_ACTION_CONFIG_H__

#include "../BS2Types.h"
#include "../data/BS2Trigger.h"
#include "../data/BS2Action.h"

/**
 *	Constants
 */
enum {
	BS2_MAX_TRIGGER_ACTION	= 128,
};

typedef struct {
	BS2Trigger	trigger;
	BS2Action	action;
} BS2TriggerAction;

/**
 *	BS2TriggerActionConfig
 */
typedef struct {
	uint8_t numItems;			///< 1 byte
	uint8_t reserved[3];		///< 3 bytes

	BS2TriggerAction items[BS2_MAX_TRIGGER_ACTION];

	uint8_t reserved1[32];		///< 32 bytes (reserved)
} BS2TriggerActionConfig;

#endif	// __BS2_TRIGGER_ACTION_CONFIG_H__
