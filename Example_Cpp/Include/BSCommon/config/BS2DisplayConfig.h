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
    BS2_BG_THEME_DEFAULT    = 4,

	BS2_VOLUME_DEFAULT 		= 50,

	//timeout
	BS2_TIMEOUT_INDEFINITE	= 0,

	BS2_BACKLIGHT_TIMEOUT_0 = 0,
	BS2_BACKLIGHT_TIMEOUT_10 = 10,
	BS2_BACKLIGHT_TIMEOUT_20 = 20,
	BS2_BACKLIGHT_TIMEOUT_30 = 30,
	BS2_BACKLIGHT_TIMEOUT_40 = 40,
	BS2_BACKLIGHT_TIMEOUT_50 = 50,
	BS2_BACKLIGHT_TIMEOUT_60 = 60,
	BS2_BACKLIGHT_TIMEOUT_DEFAULT = BS2_BACKLIGHT_TIMEOUT_20,

	BS2_MENU_TIMEOUT_0 = 0,
	BS2_MENU_TIMEOUT_10 = 10,
	BS2_MENU_TIMEOUT_20 = 20,
	BS2_MENU_TIMEOUT_30 = 30,
	BS2_MENU_TIMEOUT_40 = 40,
	BS2_MENU_TIMEOUT_50 = 50,
	BS2_MENU_TIMEOUT_60 = 60,
	BS2_MENU_TIMEOUT_DEFAULT = BS2_MENU_TIMEOUT_20,

	BS2_MSG_TIMEOUT_500MS	= 500,
	BS2_MSG_TIMEOUT_1S		= 1000,
	BS2_MSG_TIMEOUT_2S		= 2000,
	BS2_MSG_TIMEOUT_3S		= 3000,
	BS2_MSG_TIMEOUT_4S		= 4000,
	BS2_MSG_TIMEOUT_5S		= 5000,
	BS2_MSG_TIMEOUT_MIN 	= BS2_MSG_TIMEOUT_500MS,
	BS2_MSG_TIMEOUT_MAX 	= BS2_MSG_TIMEOUT_5S,
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

enum {
	BS2_OSDP_LED_SHOW_TIMEOUT	= 3000, 	// 3s (ms)
};

/**
 *	BS2_SHOW_OSDP_RESULT
 */
enum {
	BS2_SHOW_OSDP_RESULT_ON = 0,
	BS2_SHOW_OSDP_RESULT_OFF = 1,
};

typedef uint8_t BS2_SHOW_OSDP_RESULT;

// BDP-910
enum{
	BS2_USER_INFO_SHOW_ALL = 0,
	BS2_USER_INFO_SHOW_PARTIAL = 1,
	BS2_USER_INFO_SHOW_NOTHING = 2
};

typedef uint8_t BS2_AUTHMSG_USERINFO;

// BDP-948
enum {
	BS2_KEYBOARD_MODE_SCRAMBLE = 0,		// 0 : Scramble keyboard(Default)
	BS2_KEYBOARD_MODE_NON_SCRAMBLE = 1,	// 1 : Non-scramble keyborad
};

typedef uint8_t BS2_SCRAMBLE_KEYBOARD_MODE;

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
	BS2_BOOL queryUserPhrase;		///< 1 byte

	uint8_t shortcutHome[BS2_MAX_SHORTCUT_HOME];	///< 8 bytes
	uint8_t tnaIcon[BS2_MAX_TNA_KEY];		///< 16 bytes : tnaIcon

	BS2_BOOL useScreenSaver;		///< 1 byte
	BS2_SHOW_OSDP_RESULT showOsdpResult;	///< 1 byte	

	BS2_AUTHMSG_USERINFO authMsgUserName;			///< 1 byte
	BS2_AUTHMSG_USERINFO authMsgUserId;			///< 1 byte

	BS2_SCRAMBLE_KEYBOARD_MODE scrambleKeyboardMode ;		///< 1 byte

	uint8_t reserved3[27];		///< 27 bytes (padding)
} BS2DisplayConfig;

#endif	// __BS2_DISPLAY_CONFIG_H__
