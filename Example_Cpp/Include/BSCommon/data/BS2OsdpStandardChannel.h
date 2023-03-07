/**
 *  Osdp Channel Setting
 *
 *  @author yhlee@suprema.co.kr
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

#ifndef __BS2_OSDP_STANDARD_CHANNEL_H__
#define __BS2_OSDP_STANDARD_CHANNEL_H__

#include "BS2Types.h"
#include "BS2Rs485Channel.h"


/**
 *	BS2OsdpStandardDeviceNotify
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	BS2_DEVICE_TYPE	deviceType;		///< 2 bytes
	BS2_BOOL		enableOSDP;		///< 1 byte
	BS2_BOOL		connected;		///< 1 bytes

	uint8_t			channelInfo;	///< 1 byte
	uint8_t			osdpID;			///< 1 byte
	BS2_BOOL		supremaSearch;	///< 1 byte
	BS2_BOOL		activate;		///< 1 byte
	
	BS2_BOOL		useSecure;		///< 1 byte
	uint8_t			vendorCode[3];	///< 3 bytes

	BS2_VERSION		fwVersion;		///< 4 bytes

	uint8_t			modelNumber;	///< 1 byte
	uint8_t			modelVersion;	///< 1 byte
	BS2_BOOL		readInfo;		///< 1 byte
	uint8_t			reserved[5];	///< 5 byte (packing)
} BS2OsdpStandardDeviceNotify;		///< 28 bytes


/**
 *	BS2OsdpStandardDevice
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	BS2_DEVICE_TYPE	deviceType;		///< 2 bytes
	BS2_BOOL		enableOSDP;		///< 1 byte
	BS2_BOOL		connected;		///< 1 byte

	uint8_t			channelInfo;	///< 1 byte
	uint8_t			osdpID;			///< 1 byte
	BS2_BOOL		supremaSearch;	///< 1 byte
	BS2_BOOL		activate;		///< 1 byte
	
	BS2_BOOL		useSecure;		///< 1 byte
	uint8_t			vendorCode[3];	///< 3 bytes

	BS2_VERSION		fwVersion;		///< 4 bytes

	uint8_t			modelNumber;	///< 1 byte
	uint8_t			modelVersion;	///< 1 byte
	BS2_BOOL		readInfo;		///< 1 byte
	uint8_t			reserved[25];	///< 25 byte (packing)
} BS2OsdpStandardDevice;				///< 48 bytes


/**
 *	BS2OsdpStandardChannel
 */
typedef struct {
	uint32_t				baudRate;			///< 4 bytes
	uint8_t					channelIndex;		///< 1 byte
	uint8_t					useRegistance;		///< 1 byte
	uint8_t					numOfDevices;		///< 1 byte
	BS2_OSDP_CHANNEL_TYPE 	channelType;		///< 1 byte	
	BS2OsdpStandardDevice 	slaveDevices[BS2_RS485_MAX_SLAVES_PER_CHANNEL];	///< 28 * 32 = 896 bytes
	uint8_t					reserved[4];		///< 4 bytes
} BS2OsdpStandardChannel;    					///< 908 bytes

#endif	// __BS2_OSDP_STANDARD_CHANNEL_H__
