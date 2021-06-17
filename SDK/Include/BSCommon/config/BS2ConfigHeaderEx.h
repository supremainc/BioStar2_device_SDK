/**
 *  Configuration Header
 *
 *  @author smlee@suprema.co.kr
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

#ifndef __BS2_CONFIG_HEADER_EX_H__
#define __BS2_CONFIG_HEADER_EX_H__

#include "BS2Types.h"

/**
 *	Configuration Type
 */
enum {
	BS2_CONFIG_MASK_EX_NONE			= 0x0000,
	BS2_CONFIG_MASK_EX_AUTH			= 0x0001,		///< Authentication configuration
	BS2_CONFIG_MASK_EX_IPV6         = 0x0002,		///< TCP/IPv6 configuration
	BS2_CONFIG_MASK_EX_FACE         = 0x00000004,
	BS2_CONFIG_MASK_EX_THERMAL_CAMERA  = 0x00000008,
	BS2_CONFIG_MASK_EX_BARCODE  	= 0x00000010,
};

#define BS2_CONFIG_MASK_EX_ALL			0xFFFFFFFF	///< 4 bytes

#endif	// __BS2_CONFIG_HEADER_EX_H__
