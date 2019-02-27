/**
 *  Resource Definitions
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

#ifndef __BS2_RESOURCE_H__
#define __BS2_RESOURCE_H__

#include "BS2Types.h"

#define BS2_RESOURCE_TEXT	BS2_FOURCC('T', 'E', 'X', 'T')
#define BS2_RESOURCE_PNG	BS2_FOURCC('P', 'N', 'G', '\0')
#define BS2_RESOURCE_JPEG	BS2_FOURCC('J', 'P', 'E', 'G')
#define BS2_RESOURCE_WAVE	BS2_FOURCC('W', 'A', 'V', 'E')

/**
 *  BS2_RESOURCE_TYPE
 */
enum {
	BS2_RESOURCE_TYPE_UI		= 0,
	BS2_RESOURCE_TYPE_NOTICE	= 1,
	BS2_RESOURCE_TYPE_IMAGE		= 2,
	BS2_RESOURCE_TYPE_SLIDE		= 3,
	BS2_RESOURCE_TYPE_SOUND		= 4,

	BS2_RESOURCE_TYPE_BLE 		= 0xFE,  
	BS2_RESOURCE_TYPE_FIRMWARE 	= 0xFF,
};

typedef uint8_t BS2_RESOURCE_TYPE;

enum {
	BS2_SOUND_INDEX_WELCOME = 0,
	BS2_SOUND_INDEX_AUTH_SUCCESS = 1,
	BS2_SOUND_INDEX_AUTH_FAIL = 2,
	BS2_SOUND_INDEX_ALARM_1 = 3,
	BS2_SOUND_INDEX_ALARM_2 = 4,
};

typedef uint16_t BS2_SOUND_INDEX;

/**
 *  BS2ResourceHeader
 */
typedef struct {
	uint32_t index;
	uint32_t type;
	uint32_t size;
} BS2ResourceHeader;

#endif	// __BS2_RESOURCE_H__
