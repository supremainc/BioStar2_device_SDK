/**
 *  Osdp Standard Config
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

#ifndef __BS2_OSDP_STANDARD_CONFIG_H__
#define __BS2_OSDP_STANDARD_CONFIG_H__

#include "BS2Types.h"
#include "BS2Rs485Config.h"
#include "data/BS2OsdpStandardChannel.h"


#define BS2_OSDP_STANDARD_MAX_DEVICE_PER_CHANNEL		8

/**
 *	BS2_OSDP_RESULT
 */
enum {
    BS2_OSDP_RESULT_FAIL,
    BS2_OSDP_RESULT_SUCCESS,
};

typedef uint8_t 			BS2_OSDP_RESULT;


/**
 *	BS2OsdpStandardDeviceResult
 */
#pragma pack(push, 1)
typedef struct {
    BS2_DEVICE_ID       	deviceID;                    ///< 4 bytes
    BS2_OSDP_RESULT     	result;                      ///< 1 bytes
} BS2OsdpStandardDeviceResult; 		                     ///< 5 bytes
#pragma pack(pop)


/**
 *	BS2_OSDP_STANDARD_KEY_ENC
 */
enum {
    BS2_OSDP_STANDARD_KEY_ENC_USER_DEFINE    = 0,
    BS2_OSDP_STANDARD_KEY_ENC_DEVICE_DEFAULT = 1,
    BS2_OSDP_STANDARD_KEY_ENC_PLAIN_TEXT     = 2,

    BS2_OSDP_STANDARD_KEY_SIZE               = 16,
};

typedef uint8_t            BS2_OSDP_STANDARD_KEY_ENC;


/**
 * BS2OsdpStandardDeviceSecurityKey
 */
typedef struct {
	uint8_t key[BS2_OSDP_STANDARD_KEY_SIZE];
	uint8_t reserved[32];
} BS2OsdpStandardDeviceSecurityKey;


/**
 *	BS2OsdpStandardDeviceCapabilityItem
 */
typedef struct {
	uint8_t					compliance;
	uint8_t					count;
} BS2OsdpStandardDeviceCapabilityItem;


/**
 *	BS2OsdpStandardDeviceCapability
 */
typedef struct {
	BS2OsdpStandardDeviceCapabilityItem input;			///< 2 bytes
	BS2OsdpStandardDeviceCapabilityItem	output;			///< 2 bytes
	BS2OsdpStandardDeviceCapabilityItem	led;			///< 2 bytes
	BS2OsdpStandardDeviceCapabilityItem	audio;			///< 2 bytes
	BS2OsdpStandardDeviceCapabilityItem	textOutput;		///< 2 bytes
	BS2OsdpStandardDeviceCapabilityItem	reader;			///< 2 bytes

	uint16_t				recvBufferSize;				///< 2 bytes
	uint16_t				largeMsgSize;				///< 2 bytes

	uint8_t					osdpVersion;				///< 1 byte
	uint8_t					cardFormat;					///< 1 byte
	uint8_t					timeKeeping;				///< 1 byte
	uint8_t					canCommSecure;				///< 1 byte

	BS2_BOOL				crcSupport;					///< 1 byte
	BS2_BOOL				smartCardSupport;			///< 1 byte
	BS2_BOOL				biometricSupport;			///< 1 byte
	BS2_BOOL				securePinEntrySupport;		///< 1 byte

	uint8_t					reserved[4];				///< 4 bytes
} BS2OsdpStandardDeviceCapability;						///< 28 bytes


/**
 *	BS2OsdpStandardDeviceAdd
 */
typedef struct {
	uint8_t 		osdpID;                         ///< 1 byte
	uint8_t			activate;                       ///< 1 byte
	uint8_t			useSecureSession;               ///< 1 byte
	uint8_t			deviceType;                     ///< 1 byte
	BS2_DEVICE_ID	deviceID;                       ///< 4 bytes
} BS2OsdpStandardDeviceAdd;    						///< 8 bytes

/**
 *	BS2OsdpStandardDeviceUpdate
 */
typedef struct {
	uint8_t 		osdpID;                         ///< 1 byte
	uint8_t			activate;                       ///< 1 byte
	uint8_t			useSecureSession;               ///< 1 byte
	uint8_t			deviceType;                     ///< 1 byte
	BS2_DEVICE_ID	deviceID;                       ///< 4 bytes
} BS2OsdpStandardDeviceUpdate; 						///< 8 bytes


/**
 *	BS2OsdpStandardChannelInfo
 */
typedef struct {
	uint8_t					channelIndex;               ///< 1 byte
	BS2_OSDP_CHANNEL_TYPE	channelType;                ///< 1 byte
	uint8_t					maxOsdpDevice;              ///< 1 byte
	uint8_t					numOsdpAvailableDevice;     ///< 1 byte
	BS2_DEVICE_ID			deviceIDs[8];               ///< 4 x 8 = 32 bytes
} BS2OsdpStandardChannelInfo;							///< 36 bytes


/**
 *	BS2OsdpStandardDeviceAvailable
 */
typedef struct {
    uint8_t                 numOfChannel;           	///< 1 byte
    uint8_t                 reserved[3];            	///< 3 bytes
	BS2OsdpStandardChannelInfo channels[BS2_RS485_MAX_CHANNELS_EX]; ///< 36 x 8 = 288 bytes
	uint8_t                 reserved1[32];          	///< 32 bytes
} BS2OsdpStandardDeviceAvailable;    					///< 288 bytes + 36


/**
 *	BS2OsdpStandardConfig
 */
typedef struct {
	BS2_RS485_MODE 			mode[BS2_RS485_MAX_CHANNELS_EX];		///< 8 byte
	uint16_t 				numOfChannels;							///< 2 byte
	uint8_t 				reserved[2];							///< 2 bytes (packing)
	uint8_t 				reserved1[32];							///< 32 bytes (reserved)
	BS2OsdpStandardChannel 	channels[BS2_RS485_MAX_CHANNELS_EX]; 	///< 908 * 8 bytes  = 7264 bytes
} BS2OsdpStandardConfig;   											///< 7308 bytes

#endif	// __BS2_OSDP_STANDARD_CONFIG_H__
