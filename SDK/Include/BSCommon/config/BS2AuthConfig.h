/**
 *  Authentication Configuration
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

#ifndef __BS2_AUTH_CONFIG_H__
#define __BS2_AUTH_CONFIG_H__

#include "../BS2Types.h"
#include "../../BS_Deprecated.h"

/**
 *  Constants
 */
enum {
	BS2_MAX_OPERATORS			= 10,

	BS2_AUTH_TIMEOUT_MIN 		= 3,
	BS2_AUTH_TIMEOUT_MAX 		= 20,
#if defined(FACESTATION2) || defined(FACELITE) || defined(FACESTATION_F2)
	BS2_AUTH_TIMEOUT_DEFAULT	= 5,
#else
	BS2_AUTH_TIMEOUT_DEFAULT	= 10,
#endif

	BS2_MATCH_TIMEOUT_MIN 		= 1,
	BS2_MATCH_TIMEOUT_MAX 		= 20,
	BS2_MATCH_TIMEOUT_DEFAULT 	= 5,
};

/**
 * Global APB fail action
 */
enum {
	GLOBAL_APB_FAIL_ACTION_NONE,
	GLOBAL_APB_FAIL_ACTION_SOFT,
	GLOBAL_APB_FAIL_ACTION_HARD,
};

typedef uint8_t BS2_GLOBAL_APB_FAIL_ACTION_TYPE;

/**
 * Credential Type
 */
enum {
	CREDENTIAL_TYPE_NONE 		= 0x00,

	CREDENTIAL_TYPE_BIOMETRIC	= 0x01,
	CREDENTIAL_TYPE_FINGERPRINT = 0x01,
	CREDENTIAL_TYPE_CARD 		= 0x02,
	CREDENTIAL_TYPE_KEY 		= 0x04,
	CREDENTIAL_TYPE_FACE 		= 0x08,

	CREDENTIAL_TYPE_ALL 		= 0xFF
};

// Deprecated in V2.4.0. Instead, use the CREDENTIAL_TYPE_FINGERPRINT or CREDENTIAL_TYPE_FACE
#define CREDENTIAL_TYPE_BIOMETRIC		(DEPRECATED_ENUM)CREDENTIAL_TYPE_BIOMETRIC

typedef uint8_t BS2_CREDENTIAL_TYPE;

/**
 *	Authentication Mode
 */
enum {
	BS2_AUTH_MODE_NONE				= 255,	///< Authentication mode is not defined
	BS2_AUTH_MODE_PROHIBITED		= 254,	///< Authentication mode is prohibited

	BS2_AUTH_MODE_BIOMETRIC_ONLY	= 0,
	BS2_AUTH_MODE_BIOMETRIC_PIN,

	BS2_AUTH_MODE_CARD_ONLY,
	BS2_AUTH_MODE_CARD_BIOMETRIC,
	BS2_AUTH_MODE_CARD_PIN,
	BS2_AUTH_MODE_CARD_BIOMETRIC_OR_PIN,
	BS2_AUTH_MODE_CARD_BIOMETRIC_PIN,

	BS2_AUTH_MODE_ID_BIOMETRIC,
	BS2_AUTH_MODE_ID_PIN,
	BS2_AUTH_MODE_ID_BIOMETRIC_OR_PIN,
	BS2_AUTH_MODE_ID_BIOMETRIC_PIN,

	BS2_NUM_OF_AUTH_MODE,
};

typedef uint32_t BS2_AUTH_MODE;

/**
 *  BS2_OPERATOR_LEVEL
 */
enum {
	BS2_OPERATOR_LEVEL_NONE,
	BS2_OPERATOR_LEVEL_ADMIN,
	BS2_OPERATOR_LEVEL_CONFIG,
	BS2_OPERATOR_LEVEL_USER,

	BS2_NUM_OF_OPERATOR_LEVEL,
};

typedef uint8_t BS2_OPERATOR_LEVEL;

/**
 *  BS2_FACE_DETECTION_LEVEL
 */
enum {
	BS2_FACE_DETECTION_NONE,
	BS2_FACE_DETECTION_NORMAL,
	BS2_FACE_DETECTION_STRICT,
};

typedef uint8_t BS2_FACE_DETECTION_LEVEL;

/**
 *  BS2AuthConfig
 */
typedef struct {
	BS2_SCHEDULE_ID authSchedule[BS2_NUM_OF_AUTH_MODE];                ///< 4 bytes * n

	BS2_BOOL useGlobalAPB;                                             ///< 1 byte
	BS2_GLOBAL_APB_FAIL_ACTION_TYPE globalAPBFailAction;               ///< 1 byte
	BS2_BOOL useGroupMatching;                                         ///< 1 byte
	uint8_t reserved;                                                  ///< 1 byte (packing)

	uint8_t reserved2[28];                                              ///< 28 bytes (reserved)

	BS2_BOOL usePrivateAuth;                                           ///< 1 byte
	BS2_FACE_DETECTION_LEVEL faceDetectionLevel;                       ///< 1 byte
	BS2_BOOL useServerMatching;                                        ///< 1 byte
	BS2_BOOL useFullAccess;                                            ///< 1 byte

	uint8_t matchTimeout;                                              ///< 1 byte
	uint8_t authTimeout;                                               ///< 1 byte
	uint8_t numOperators;                                              ///< 1 byte
	uint8_t reserved3[1];                                              ///< 1 byte (packing)

	struct {
		BS2_USER_ID userID;                                              ///< 32 bytes
		BS2_OPERATOR_LEVEL level;                                        ///< 1 byte
		uint8_t reserved[3];                                             ///< 3 bytes
	} operators[BS2_MAX_OPERATORS];
} BS2AuthConfig;

#endif	// __BS2_AUTH_CONFIG_H__
