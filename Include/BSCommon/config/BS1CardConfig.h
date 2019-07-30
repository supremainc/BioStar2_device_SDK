/*
 * BS1CardConfig.h
 *
 *  Created on: 2016. 2. 25.
 *      Author: yhlee
 */

#ifndef BS1CARDCONFIG_H_
#define BS1CARDCONFIG_H_

#include "../BS2Types.h"




enum {
    MIFARE_KEY_SIZE = 6,
    MIFARE_MAX_TEMPLATE = 4,

    VALID_MAGIC_NO = 0x1f1f1f1f,
};


/**
 *  BS1CardConfig
 */
typedef struct {

	// Options
	uint32_t 	magicNo;
	uint32_t 	disabled;
	uint32_t 	useCSNOnly; 		// default 0
	uint32_t 	bioentryCompatible; // default 0

	// Keys
	uint32_t	useSecondaryKey;
	uint32_t 	reserved1;
	uint8_t 	primaryKey[MIFARE_KEY_SIZE];
	uint8_t		reserved2[2];
	uint8_t 	secondaryKey[MIFARE_KEY_SIZE];
	uint8_t		reserved3[2];

	// Layout
	uint32_t 	cisIndex;
	uint32_t 	numOfTemplate;
	uint32_t 	templateSize;
	uint32_t 	templateStartBlock[MIFARE_MAX_TEMPLATE];

	uint32_t 	reserve4[15];

} BS1CardConfig;                             ///< 120 bytes

#endif /* BS1CARDCONFIG_H_ */
