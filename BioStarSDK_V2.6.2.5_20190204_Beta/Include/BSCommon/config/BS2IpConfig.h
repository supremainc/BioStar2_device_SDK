/**
 *  Network (IP) Configuration
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

#ifndef __BS2_IP_CONFIG_H__
#define __BS2_IP_CONFIG_H__

#include "BS2Types.h"

/**
 *	Connection mode between server and device.
 */
enum {
	BS2_CONNECTION_MODE_SERVER_TO_DEVICE	= 0,	///< server connects to device
	BS2_CONNECTION_MODE_DEVICE_TO_SERVER	= 1,	///< device connects to server

	BS2_CONNECTION_MODE_DEFAULT				= BS2_CONNECTION_MODE_SERVER_TO_DEVICE,

	BS2_TCP_MTU_SIZE_MIN					= 1078,
	BS2_TCP_MTU_SIZE_MAX					= 1514,
	BS2_TCP_MTU_SIZE_DEFAULT				= BS2_TCP_MTU_SIZE_MAX,

	BS2_TCP_SSL_SERVER_PORT_DEFAULT				= 51213,
	BS2_TCP_SERVER_PORT_DEFAULT				= 51212,
	BS2_TCP_DEVICE_PORT_DEFAULT				= 51211,
	BS2_UDP_BROADCAST_PORT_DEFAULT			= 51210,
	BS2_TCP_DEVICE_ZONE_MASTER_PORT_DEFAULT	= 51214,	
};

const char BS2_INVAILD_IP_ADDRESS[] = "169.254.0.1";

typedef uint8_t BS2_CONNECTION_MODE;

/**
 *	BS2_TCP_BASEBAND
 */
enum {
	BS2_TCP_BASEBAND_10BASE_T		= 0,
	BS2_TCP_BASEBAND_100BASE_T		= 1,
};

typedef uint8_t BS2_TCP_BASEBAND;

/**
 *	BS2IpConfig
 */
typedef struct {
	BS2_CONNECTION_MODE connectionMode;	///< 1 byte
	BS2_BOOL useDHCP;				///< 1 byte
	BS2_BOOL useDNS;	///< 1 byte
	uint8_t reserved[1];			///< 1 byte (packing)
	BS2_IPV4_ADDR ipAddress;		///< 16 bytes
	BS2_IPV4_ADDR gateway;			///< 16 bytes
	BS2_IPV4_ADDR subnetMask;		///< 16 bytes
	BS2_IPV4_ADDR serverAddr;		///< 16 bytes
	BS2_PORT port;					///< 2 bytes
	BS2_PORT serverPort;			///< 2 bytes
	uint16_t mtuSize;				///< 2 bytes
	BS2_TCP_BASEBAND baseband;		///< 1 byte
	uint8_t reserved2[1];			///< 1 byte (packing)

	BS2_PORT sslServerPort;		///< 2 bytes
	uint8_t reserved3[30];			///< 30 bytes (reserved)
} BS2IpConfig;

#endif	// __BS2_IP_CONFIG_H__
