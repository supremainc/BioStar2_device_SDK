/*
 * BS2EventConfig.h
 *
 *  Created on: 2015. 9. 2.
 *      Author: scpark
 */

#ifndef _BS2_EVENT_CONFIG_H_
#define _BS2_EVENT_CONFIG_H_

#include "../BS2Types.h"

enum {
	BS2_EVENT_MAX_IMAGE_CODE_COUNT	= 32,
};

/**
 *	BS2EventConfig
 */
typedef struct {
	uint32_t numImageEventFilter;                        ///< 4 bytes

	struct {
		uint8_t mainEventCode;                           ///< 1 byte
		uint8_t subEventCode;                            ///< 1 byte
		BS2_BOOL subCodeIncluded;                        ///< 1 byte - SHOULD be "true" for backward compatibility.
		uint8_t reserved2[1];                            ///< 1 byte (padding)
		BS2_SCHEDULE_ID scheduleID;                      ///< 4 bytes
	} imageEventFilter[BS2_EVENT_MAX_IMAGE_CODE_COUNT];  ///< 256 bytes

	uint8_t unused[32];						             ///< 32 bytes (reserved)
} BS2EventConfig;


#endif /* _BS2_EVENT_CONFIG_H_ */
