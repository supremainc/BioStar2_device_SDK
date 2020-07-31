/**
 *  Network (IP) Configuration
 *
 *  @author mkkim@suprema.co.kr
 *  @see
 */

/*
 *  Copyright (c) 2017 Suprema Co., Ltd. All Rights Reserved.
 *
 *  This software is the confidential and proprietary information of
 *  Suprema Co., Ltd. ("Confidential Information").  You shall not
 *  disclose such Confidential Information and shall use it only in
 *  accordance with the terms of the license agreement you entered into
 *  with Suprema.
 */

#ifndef __BS2_IPV6_CONFIG_H__
#define __BS2_IPV6_CONFIG_H__

#include "../BS2Types.h"

const char BS2_INVAILD_IPV6_ADDRESS[] = "::/0"; //"FE80::/10";

typedef uint8_t BS2_CONNECTION_MODE;

enum {
	BS2_MAX_IPV6_ALLOCATED_ADDR = 8,
};

/**
 *	BS2IPv6Config
 */
typedef struct {
	BS2_BOOL useIPV6;						///< 1 byte
	uint8_t  reserved1; 					///< 1 byte	//Not yet apply //useIPV4;
	BS2_BOOL useDhcpV6;						///< 1 byte
	BS2_BOOL useDnsV6;						///< 1 byte
	BS2_IPV6_ADDR staticIpAddressV6;		///< 40 bytes
	BS2_IPV6_ADDR staticGatewayV6;			///< 40 bytes
	BS2_IPV6_ADDR dnsAddrV6;				///< 40 bytes
	BS2_IPV6_ADDR serverIpAddressV6;		///< 40 bytes
	BS2_PORT serverPortV6; 					///< 2 bytes
	BS2_PORT sslServerPortV6;				///< 2 bytes
	BS2_PORT portV6;						///< 2 bytes
	uint8_t numOfAllocatedAddressV6;		///< 1 bytes
	uint8_t numOfAllocatedGatewayV6;		///< 1 bytes
	uint8_t reserved[8];					///< 8 bytes padding
	BS2_IPV6_ADDR allocatedIpAddressV6[BS2_MAX_IPV6_ALLOCATED_ADDR];	///< 40*8 320 bytes
	BS2_IPV6_ADDR allocatedGatewayV6[BS2_MAX_IPV6_ALLOCATED_ADDR];		///< 40*8 320 bytes	
} BS2IPV6Config;							///< 

#endif	// __BS2_IPV6_CONFIG_H__
