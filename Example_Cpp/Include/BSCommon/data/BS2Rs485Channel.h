/**
 *  RS485 Channel Setting
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

#ifndef __BS2_RS485_CHANNEL_H__
#define __BS2_RS485_CHANNEL_H__

#include "../BS2Types.h"

/**
 *	Constants
 */
enum {
	BS2_RS485_MAX_SLAVES_PER_CHANNEL	= 32,
	BS2_RS485_INVALID_BAUD_RATE			= -1,		///< @TODO: must be zero
};

/**
 *	BS2_OSDP_CHANNEL_TYPE
 */
enum {
	BS2_OSDP_CHANNEL_TYPE_NORMAL,
	BS2_OSDP_CHANNEL_TYPE_SUPREMA,
	BS2_OSDP_CHANNEL_TYPE_OSDP_STANDARD,
	BS2_OSDP_CHANNEL_TYPE_OSDP_STANDARD_FULL
};

typedef uint8_t 	BS2_OSDP_CHANNEL_TYPE;

/**
 *	BS2Rs485SlaveDeviceEX
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	BS2_DEVICE_TYPE	deviceType;		///< 2 bytes
	BS2_BOOL		enableOSDP;		///< 1 byte
	BS2_BOOL		connected;		///< 1 bytes (packing)
	uint8_t			channelInfo;	///< 1byte
	uint8_t			osdpID;			///< 1 byte
	uint8_t			reserved;		///< 1 byte (packing)
	uint8_t			useSecureSession; ///< 1 byte
} BS2Rs485SlaveDeviceEX;			///< 12 bytes

/**
 *	BS2Rs485ChannelEX
 */
typedef struct {
	uint32_t				baudRate;			///< 4 bytes
	uint8_t					channelIndex;		///< 1 byte
	uint8_t					useRegistance;		///< 1 byte
	uint8_t					numOfDevices;		///< 1 byte
	BS2_OSDP_CHANNEL_TYPE	channelType;		///< 1 byte
	BS2Rs485SlaveDeviceEX 	slaveDevices[BS2_RS485_MAX_SLAVES_PER_CHANNEL];	///< 8 * 12 = 96 bytes
} BS2Rs485ChannelEX;

/**
 *	BS2Rs485SlaveDevice
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	BS2_DEVICE_TYPE	deviceType;		///< 2 bytes
	BS2_BOOL		enableOSDP;		///< 1 byte
	BS2_BOOL		connected;		///< 1 bytes (packing)
} BS2Rs485SlaveDevice;				///< 8 bytes

/**
 *	BS2Rs485Channel
 */
typedef struct {
	uint32_t	baudRate;			///< 4 bytes
	uint8_t		channelIndex;		///< 1 byte
	uint8_t		useRegistance;		///< 1 byte
	uint8_t		numOfDevices;		///< 1 byte
	uint8_t		reserved[1];		///< 1 bytes (packing)
	BS2Rs485SlaveDevice slaveDevices[BS2_RS485_MAX_SLAVES_PER_CHANNEL];	///< 8 * 8 = 64 bytes
} BS2Rs485Channel;					///< 72 bytes

#endif	// __BS2_RS485_CHANNEL_H__
