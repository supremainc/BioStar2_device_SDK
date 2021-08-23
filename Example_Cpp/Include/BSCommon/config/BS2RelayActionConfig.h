/*
 * BS2RelayActionConfig.h
 *
 *  Created on: 2021. 2. 16.
 *      Author: sbjang
 */

#ifndef __BS2_RELAY_ACTION_CONFIG_H__
#define __BS2_RELAY_ACTION_CONFIG_H__

#include "BS2Types.h"

/**
 *	Constants for BS2RelayActionConfig
 */
enum {
	BS2_MAX_RELAY_ACTION		= 4,
	BS2_MAX_RELAY_ACTION_INPUT	= 16,
};

/**
 *	BS2_RELAY_ACTION_INPUT_TYPE
 */
enum {
	BS2_RELAY_ACTION_INPUT_TYPE_NONE	 = 0x00,
	BS2_RELAY_ACTION_INPUT_TYPE_LINKAGE  = 0x01,
	BS2_RELAY_ACTION_INPUT_TYPE_LATCHING = 0x02,
	BS2_RELAY_ACTION_INPUT_TYPE_RELEASE  = 0x03,
};
typedef uint8_t BS2_RELAY_ACTION_INPUT_TYPE;

/**
 *	BS2_RELAY_ACTION_INPUT_MASK
 */
enum {
	BS2_RELAY_ACTION_INPUT_MASK_NONE	= 0x00,
	BS2_RELAY_ACTION_INPUT_MASK_ALARM	= 0x01,
	BS2_RELAY_ACTION_INPUT_MASK_FAULT	= 0x02,
};
typedef uint8_t BS2_RELAY_ACTION_INPUT_MASK;


/**
 *	BS2RelayActionConfig
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;							///< 4 bytes
	uint8_t			reserved[16];						///< 16 bytes
	
	struct {
		uint8_t		port;								///< 1 byte	(relay port)
		uint8_t		reserved0;							///< 1 byte
		BS2_BOOL	disconnEnabled;						///< 1 byte (RS485 disconnection)
		uint8_t		reserved[9];						///< 9 bytes
		
		struct {
			uint8_t		port;							///< 1 byte	(input port)
			BS2_RELAY_ACTION_INPUT_TYPE	type;			///< 1 byte (linkage/latching/release)
			BS2_RELAY_ACTION_INPUT_MASK	mask;			///< 1 byte (alarm/fault)
			uint8_t		reserved[9];					///< 9 bytes
		} input[BS2_MAX_RELAY_ACTION_INPUT];			///< 192 bytes
	} relay[BS2_MAX_RELAY_ACTION]; 						///< 816 bytes
	
	uint8_t			reserved2[152];						///< 152 bytes
} BS2RelayActionConfig;									///< 988 bytes

#endif	// __BS2_RELAY_ACTION_CONFIG_H__
