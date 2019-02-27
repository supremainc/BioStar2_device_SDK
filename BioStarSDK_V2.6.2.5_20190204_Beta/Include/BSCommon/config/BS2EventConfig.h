/*
 * BS2EventConfig.h
 *
 *  Created on: 2015. 9. 2.
 *      Author: scpark
 */

#ifndef _BS2_EVENT_CONFIG_H_
#define _BS2_EVENT_CONFIG_H_

#include "BS2Types.h"

enum {
	BS2_EVENT_MAX_IMAGE_CODE_COUNT	= 32,
};

/**
 *	BS2EventConfig
 */
typedef struct {
	uint32_t numImageEventFilter;                        ///< 4 bytes

	struct {
	  uint8_t mainEventCode;                             ///< 1 bytes
	  uint8_t reserved[3];                               ///< 3 bytes(padding)
	  BS2_SCHEDULE_ID scheduleID;                        ///< 4 bytes
	} imageEventFilter[BS2_EVENT_MAX_IMAGE_CODE_COUNT];  ///< 256 bytes

	uint8_t reserved[32];                                ///< 32 bytes
} BS2EventConfig;


#endif /* _BS2_EVENT_CONFIG_H_ */
