/**
 *  Display Configuration Definitions
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

#ifndef __BS2_DISPLAY_CONFIG_H__
#define __BS2_DISPLAY_CONFIG_H__

//#include <string>

#include "../BS2Types.h"
#include "config/BS2TnaExtConfig.h"

/**
 *	Constants for BS2DisplayConfig
 */
enum {
	//background
	BS2_BG_LOGO				= 0,
	BS2_BG_NOTICE			= 1,
	BS2_BG_SLIDE			= 2,
	BS2_BG_PDF				= 3,

	//bgTheme
	BS2_BG_THEME_01			= 0,
	BS2_BG_THEME_02			= 1,
	BS2_BG_THEME_03			= 2,
	BS2_BG_THEME_04			= 3,

	BS2_VOLUME_DEFAULT 		= 50,

	//timeout
	BS2_TIMEOUT_INDEFINITE	= 0,

	BS2_BACKLIGHT_TIMEOUT_DEFAULT = 20,
	BS2_MENU_TIMEOUT_DEFAULT = 20,

	BS2_MSG_TIMEOUT_MIN 	= 500,
	BS2_MSG_TIMEOUT_MAX 	= 5000,
	BS2_MSG_TIMEOUT_DEFAULT = 2000,

	//dateFormat
	BS2_DATE_FORMAT_YMD		= 0,	///< YYYY/MM/DD
	BS2_DATE_FORMAT_MDY		= 1,	///< MM/DD/YYYY
	BS2_DATE_FORMAT_DMY		= 2,	///< DD/MM/YYYY

	//displayDateTime
	BS2_NOT_USE				= 0,
	BS2_USE					= 1,

	//timeFormat
	BS2_TIME_FORMAT_12_HOUR	= 0,
	BS2_TIME_FORMAT_24_HOUR	= 1,

	// homeFormation
	BS2_HOME_INTERPHONE		= 1,
	BS2_HOME_SHORTCUT1		= 2,
	BS2_HOME_SHORTCUT2		= 3,
	BS2_HOME_SHORTCUT3		= 4,
	BS2_HOME_SHORTCUT4		= 5,

	// shortcutHome
	BS2_SC_MENU				= 0,
	BS2_SC_TNA				= 1,
	BS2_SC_LANGUAGE			= 2,
	BS2_SC_ID				= 3,
	BS2_SC_FINGERPRINT		= 4,
	BS2_SC_INTERPHONE		= 5,
	BS2_SC_ARM				= 6,
	BS2_SC_VOLUME			= 7,

	// T&A ID : 100 ~ 115

	BS2_MAX_SHORTCUT_HOME	= 8,
};

/**
 *	BS2_LANGUAGE
 */
enum {
	BS2_LANGUAGE_KOREAN		= 0,
	BS2_LANGUAGE_ENGLISH	= 1,
	BS2_LANGUAGE_CUSTOM		= 2,

	BS2_MAX_LANGUAGE
};

typedef uint32_t BS2_LANGUAGE;

/**
 *	BS2DisplayConfig
 */
typedef struct {
	BS2_LANGUAGE language;		///< 4 byte
	uint8_t background;			///< 1 byte
	uint8_t volume;				///< 1 byte (0 ~ 100)
	uint8_t bgTheme;			///< 1 byte
	uint8_t dateFormat;			///< 1 byte

	uint16_t menuTimeout;		///< 2 bytes (0 ~ 255 sec)
	uint16_t msgTimeout;		///< 2 bytes (500 ~ 5000 ms)
	uint16_t backlightTimeout;	///< 2 bytes (in seconds)
	BS2_BOOL displayDateTime;	///< 1 byte
	BS2_BOOL useVoice;			///< 1 byte

	uint8_t timeFormat;			///< 1 byte
	uint8_t homeFormation;		///< 1 byte
	BS2_BOOL useUserPhrase;		///< 1 byte
	uint8_t reserved[1];		///< 1 byte (packing)

	uint8_t shortcutHome[BS2_MAX_SHORTCUT_HOME];	///< 8 bytes
	uint8_t tnaIcon[BS2_MAX_TNA_KEY];		///< 16 bytes : tnaIcon

	uint8_t reserved1[32];		///< 32 bytes (reserved)
} BS2DisplayConfig;

#endif	// __BS2_DISPLAY_CONFIG_H__
