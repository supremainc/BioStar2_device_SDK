/*
 * BS2EventExt.h
 *
 *  Created on: 2015. 9. 1.
 *      Author: scpark
 */

#ifndef __BS2_EVENT_EXT_H__
#define __BS2_EVENT_EXT_H__

#include "BS2Event.h"
#include "../protocol/BS2Packet.h"

enum {
	BS2_EVENT_MAX_IMAGE_SIZE = 16*1024,
};

enum {
	BS2_EVENT_MASK_NONE                           = 0,
	BS2_EVENT_MASK_INFO                           = (0x1 << 0), // Returns payload data : BS2EventExtInfo (8 bytes)
	BS2_EVENT_MASK_USER_ID                        = (0x1 << 1), // Returns payload data : BS2_USER_ID (32 bytes)
	BS2_EVENT_MASK_CARD_ID                        = (0x1 << 2), // Returns payload data : uint8_t[32] (32 bytes)
	BS2_EVENT_MASK_DOOR_ID                        = (0x1 << 3), // Returns payload data : BS2_DOOR_ID (4 bytes)
	BS2_EVENT_MASK_ZONE_ID                        = (0x1 << 4), // Returns payload data : BS2_ZONE_ID (4 bytes)
	BS2_EVENT_MASK_IODEVICE                       = (0x1 << 5), // Returns payload data : BS2EventExtIoDevice (8 bytes)
	BS2_EVENT_MASK_TNA_KEY                        = (0x1 << 6), // Returns payload data : BS2_TNA_KEY (1 byte)
	BS2_EVENT_MASK_JOB_CODE                       = (0x1 << 7), // Returns payload data : BS2_JOB_CODE (4 bytes)
	BS2_EVENT_MASK_IMAGE                          = (0x1 << 8), // Returns payload data : uint8_t[BS2_EVENT_MAX_IMAGE_SIZE]  (16384 bytes, jpg 16/24 bits image)
	BS2_EVENT_MASK_TEMPERATURE                    = (0x1 << 9), // Returns payload data : BS2_TEMPERATURE (4 bytes)

	BS2_EVENT_MASK_ALL                            = 0xFFFF,
};

typedef struct {
	BS2_DEVICE_ID ioDeviceID;                     ///< 4 bytes
	BS2_PORT port;                                ///< 2 bytes
	BS2_PORT_VALUE value;                         ///< 1 bytes
	uint8_t reserved[1];                          ///< packing
} BS2EventExtIoDevice;

typedef struct {
	BS2_TIMESTAMP dateTime;                       ///< 4 bytes
	BS2_DEVICE_ID deviceID;                       ///< 4 bytes
	union {                                       ///< 2 bytes
		BS2_EVENT_CODE code;
		struct {
			uint8_t subCode;
			uint8_t mainCode;
		};
	};
	uint8_t reserved[2];                          ///< packing
} BS2EventExtInfo;


#endif /* __BS2_EVENT_EXT_H__ */
