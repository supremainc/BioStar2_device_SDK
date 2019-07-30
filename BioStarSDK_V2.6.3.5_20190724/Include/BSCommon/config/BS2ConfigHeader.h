/**
 *  Configuration Header
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

#ifndef __BS2_CONFIG_HEADER_H__
#define __BS2_CONFIG_HEADER_H__

#include "../BS2Types.h"
#include "../../BS_Deprecated.h"

/**
 *	Configuration Type
 */
enum {
	BS2_CONFIG_MASK_NONE			= 0x0000,
	BS2_CONFIG_MASK_FACTORY			= 0x0001,		///< Factory configuration
	BS2_CONFIG_MASK_SYSTEM			= 0x0002,		///< System configuration
	BS2_CONFIG_MASK_IP				= 0x0004,		///< TCP/IP configuration
	BS2_CONFIG_MASK_RS485			= 0x0008,		///< RS485 configuration
	BS2_CONFIG_MASK_WLAN			= 0x0010,		///< Wireless LAN configuration
	BS2_CONFIG_MASK_AUTH			= 0x0020,		///< Authentication configuration
	BS2_CONFIG_MASK_CARD			= 0x0040,		///< Card configuration
	BS2_CONFIG_MASK_FINGERPRINT		= 0x0080,		///< Fingerprint configuration
	BS2_CONFIG_MASK_FACE			= 0x0100,		///< Face configuration
	BS2_CONFIG_MASK_TRIGGER_ACTION	= 0x0200,		///< Trigger Action configuration
	BS2_CONFIG_MASK_DISPLAY			= 0x0400,		///< Display configuration
	BS2_CONFIG_MASK_SOUND			= 0x0800,		///< Sound configuration
	BS2_CONFIG_MASK_STATUS			= 0x1000,		///< Status Signal(LED, Buzzer) configuration
	BS2_CONFIG_MASK_WIEGAND			= 0x2000,		///< Wiegand configuration
	BS2_CONFIG_MASK_USB				= 0x4000,		///< USB configuration
	BS2_CONFIG_MASK_TNA				= 0x8000,		///< Time and Attendance configuration (@deprecated)
	BS2_CONFIG_MASK_VIDEOPHONE		= 0x10000,		///< Videophone configuration
	BS2_CONFIG_MASK_INTERPHONE		= 0x20000,		///< Interphone configuration
	BS2_CONFIG_MASK_VOIP			= 0x40000,		///< Voice over IP configuration
	BS2_CONFIG_MASK_INPUT			= 0x80000,		///< Input(Supervised input) configuration
	BS2_CONFIG_MASK_WIEGAND_IO		= 0x100000,		///< Wiegand IO Device configuration
	BS2_CONFIG_MASK_TNA_EXT			= 0x200000,		///< Time and Attendance configuration
	BS2_CONFIG_MASK_IP_EXT			= 0x400000,		///< DNS and Server url configuration
	BS2_CONFIG_MASK_EVENT			= 0x800000,		///< Event configuration
	BS2_CONFIG_MASK_CARD_1x			= 0x1000000, 	///< 1x Card configuration
	BS2_CONFIG_MASK_WIEGAND_MULTI	= 0x2000000,	///< Multi-Wiegand configuration
	BS2_CONFIG_MASK_SYSTEM_EXT		= 0x4000000,	///< Extended System configuration
	BS2_CONFIG_MASK_DST				= 0x8000000,	///< Daylight Saving configuration
	BS2_CONFIG_MASK_RS485_EX		= 0x10000000,	///< RS485 Extended configuration
	BS2_CONFIG_MASK_CARD_EX			= 0x20000000,	///< Extended Card configuration
	BS2_CONFIG_MASK_DST_EX			= 0x40000000,	///< Daylight Saving configuration
};

//Custom => [IPv6 Cfg]
enum {
	BS2_CONFIG_MASK_EX_NONE			= 0x0000,
	BS2_CONFIG_MASK_EX_RS485_ITSELF = 0x0001,		///< Rs485 ItSelf configuration
	BS2_CONFIG_MASK_EX_IPV6         = 0x0002,		///< TCP/IPv6 configuration
};
//Custom <=

// Deprecated in V2.x.x. Instead, use the BS2_CONFIG_MASK_TNA_EXT
#define BS2_CONFIG_MASK_TNA			(DEPRECATED_ENUM)BS2_CONFIG_MASK_TNA
// Deprecated in V2.5.0. Instead, use the BS2_CONFIG_MASK_DST_EX
#define BS2_CONFIG_MASK_DST			(DEPRECATED_ENUM)BS2_CONFIG_MASK_DST

#define BS2_CONFIG_MASK_ALL			0xFFFFFFFF	///< 4 bytes

#endif	// __BS2_CONFIG_HEADER_H__
