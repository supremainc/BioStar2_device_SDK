/**
 *  Time and Attendance Ext Configuration Definitions
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

#ifndef __BS2_TNA_EXT_CONFIG__
#define __BS2_TNA_EXT_CONFIG__

#include "../BS2Types.h"

/**
 *  Constants for BS2TnaExtConfig
 */
enum {
	BS2_MAX_TNA_LABEL_LEN = 16 * 3,
};

enum {
	//tnaMode
	BS2_TNA_UNUSED = 0,
	BS2_TNA_BY_USER = 1,
	BS2_TNA_BY_SCHEDULE = 2,
	BS2_TNA_LAST_CHOICE = 3,
	BS2_TNA_FIXED = 4,

	BS2_TNA_MODE_DEFAULT = BS2_TNA_BY_USER,
};

/**
 *  BS2_TNA_MODE
 */
typedef uint8_t BS2_TNA_MODE;

enum {
	// tnaKey
	BS2_TNA_UNSPECIFIED = 0,

	BS2_TNA_KEY_1,
	BS2_TNA_KEY_2,
	BS2_TNA_KEY_3,
	BS2_TNA_KEY_4,
	BS2_TNA_KEY_5,
	BS2_TNA_KEY_6,
	BS2_TNA_KEY_7,
	BS2_TNA_KEY_8,
	BS2_TNA_KEY_9,
	BS2_TNA_KEY_10,
	BS2_TNA_KEY_11,
	BS2_TNA_KEY_12,
	BS2_TNA_KEY_13,
	BS2_TNA_KEY_14,
	BS2_TNA_KEY_15,
	BS2_TNA_KEY_16,

	BS2_MAX_TNA_KEY = 16,
};

/**
 *  BS2_TNA_KEY
 */
typedef uint8_t BS2_TNA_KEY;

typedef struct {
	BS2_TNA_MODE tnaMode;		///< 1 byte
	BS2_TNA_KEY tnaKey;		///< 1 byte
	BS2_BOOL tnaRequired;		///< 1 byte
	uint8_t reserved[1];		///< 1 byte (packing)

	BS2_SCHEDULE_ID tnaSchedule[BS2_MAX_TNA_KEY];
#if 0
	BS2_BOOL tnaEnabled[BS2_MAX_TNA_KEY];
#else
	uint8_t unused[BS2_MAX_TNA_KEY];
#endif
} BS2TNAInfo;

typedef struct {
	char tnaLabel[BS2_MAX_TNA_KEY][BS2_MAX_TNA_LABEL_LEN];
#if 0
	uint8_t tnaKey[BS2_MAX_TNA_KEY];
#else
	uint8_t unused[BS2_MAX_TNA_KEY];
#endif
} BS2TNAExtInfo;

typedef struct {
	BS2TNAInfo tnaInfo;
	BS2TNAExtInfo tnaExtInfo;

	uint8_t reserved2[32];		///< 32 bytes (reserved)
} BS2TNAConfig;

#endif /* __BS2_TNA_EXT_CONFIG__ */
