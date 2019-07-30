/*
 * BS2IpConfigExt.h
 *
 *  Created on: 2015. 9. 2.
 *      Author: scpark
 */

#ifndef _BS2_IP_CONFIG_EXT_H_
#define _BS2_IP_CONFIG_EXT_H_

#include "../BS2Types.h"

/**
 *	BS2IpConfigExt
 */
typedef struct {
	BS2_IPV4_ADDR dnsAddr;                      ///< 16 bytes
	BS2_URL serverUrl;                          ///< 256 bytes
	uint8_t reserved[32];                       ///< 32 bytes (packing)
} BS2IpConfigExt;

#endif /* _BS2_IP_CONFIG_EXT_H_ */
