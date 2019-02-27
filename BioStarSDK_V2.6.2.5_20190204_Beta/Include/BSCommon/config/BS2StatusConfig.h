/**
 *  Status Configuration Definitions
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

#ifndef __BS2_STATUS_CONFIG_H__
#define __BS2_STATUS_CONFIG_H__

#include "BS2Types.h"
#include "data/BS2Action.h"

/**
 *	BS2_DEVICE_STATUS
 */
enum {
	BS2_DEVICE_STATUS_NORMAL,
	BS2_DEVICE_STATUS_LOCKED,
	BS2_DEVICE_STATUS_RTC_ERROR,
	BS2_DEVICE_STATUS_WAITING_INPUT,
	BS2_DEVICE_STATUS_WAITING_DHCP,
	BS2_DEVICE_STATUS_SCAN_FINGER,
	BS2_DEVICE_STATUS_SCAN_CARD,
	BS2_DEVICE_STATUS_SUCCESS,
	BS2_DEVICE_STATUS_FAIL,
	BS2_DEVICE_STATUS_DURESS,
	BS2_DEVICE_STATUS_PROCESS_CONFIG_CARD,
	BS2_DEVICE_STATUS_SUCCESS_CONFIG_CARD,
	BS2_DEVICE_STATUS_RESERVED2,	///< not used yet
	BS2_DEVICE_STATUS_RESERVED3,	///< not used yet
	BS2_DEVICE_STATUS_RESERVED4,	///< not used yet
	BS2_DEVICE_STATUS_NUM,
};

typedef uint8_t BS2_DEVICE_STATUS;

/**
 *	BS2StatusConfig
 */
typedef struct {
	struct {
		BS2_BOOL enabled;		///< 1 byte
		uint8_t reserved[1];	///< 1 byte (packing)
		uint16_t count;			///< 2 bytes (0 = infinite)
		BS2LedSignal signal[BS2_LED_SIGNAL_NUM];
	} led[BS2_DEVICE_STATUS_NUM];

	uint8_t reserved1[32];		///< 32 bytes (reserved)

	struct {
		BS2_BOOL enabled;		///< 1 byte
		uint8_t reserved[1];	///< 1 byte (packing)
		uint16_t count;			///< 2 bytes (0 = infinite)
		BS2BuzzerSignal signal[BS2_BUZZER_SIGNAL_NUM];
	} buzzer[BS2_DEVICE_STATUS_NUM];

	BS2_BOOL configSyncRequired;
	uint8_t reserved2[31];		///< 31 bytes (reserved)
} BS2StatusConfig;

#endif	// __BS2_STATUS_CONFIG_H__
