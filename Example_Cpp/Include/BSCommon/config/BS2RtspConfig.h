/*
 * BS2RtspConfig.h
 *
 *  Created on: 2022. 2. 17.
 *      Author: smlee
 */

#ifndef _BS2_RTSP_CONFIG_H_
#define _BS2_RTSP_CONFIG_H_

#include "BS2Types.h"

enum {
	RTSP_ID_SIZE = 32,		// = BS2_USER_ID_SIZE
	RTSP_PW_SIZE = 32,		// = BS2_USER_ID_SIZE

	RTSP_PORT_DEFAULT = 554,		// BDP-568
};

/**
 *	BS2RtspConfig
 */
typedef struct {
	BS2_USER_ID id;					// CAUTION: might not be NULL-terminated
	BS2_USER_ID password;			// CAUTION: might not be NULL-terminated

	BS2_URL address;				// CAUTION: might not be NULL-terminated

	BS2_PORT port;
	BS2_BOOL enabled;
	uint8_t reserved;				///< 1 byte (packing)

	uint8_t reserved2[32];			///< 32 bytes (reserved)
} BS2RtspConfig;

#endif /* _BS2_RTSP_CONFIG_H_ */
