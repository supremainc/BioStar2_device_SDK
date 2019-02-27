/*
 * BS2WiegandMultiConfig.h
 *
 *  Created on: 2016. 2. 25.
 *      Author: yhlee
 */

#ifndef BS2WIEGANDMULTICONFIG_H_
#define BS2WIEGANDMULTICONFIG_H_

#include "BS2WiegandConfig.h"

#define MAX_WIEGAND_IN_COUNT	15

/**
 *  BS2WiegandInConfig
 */
typedef struct {
	BS2_UID formatID;				///< 4 bytes (wiegand format ID : 1 ~ 15)
	BS2WiegandFormat format;		///< 268 bytes

	uint8_t reserved[32];			///< 32 bytes (reserved)
} BS2WiegandInConfig;  /// << 304 bytes

/**
 *  BS2WiegandMultiInConfig
 */

typedef struct {
	BS2WiegandInConfig formats[MAX_WIEGAND_IN_COUNT];  /// < 304 * 15 = 4560
	uint8_t reserved[32];			///< 32 bytes (reserved)
} BS2WiegandMultiConfig;  /// <<  4592 bytes

#endif /* BS2WIEGANDMULTICONFIG_H_ */
