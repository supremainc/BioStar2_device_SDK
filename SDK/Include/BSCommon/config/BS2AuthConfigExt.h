/**
 *  Authentication Configuration
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

#ifndef __BS2_AUTH_CONFIG_EXT_H__
#define __BS2_AUTH_CONFIG_EXT_H__

#include "BS2AuthConfig.h"

enum {
	BS2_EXT_AUTH_MODE_FACE_ONLY	= BS2_NUM_OF_AUTH_MODE,		//TODO: confirm me
	BS2_EXT_AUTH_MODE_FACE_FINGERPRINT,
	BS2_EXT_AUTH_MODE_FACE_PIN,
	BS2_EXT_AUTH_MODE_FACE_FINGERPRINT_OR_PIN,
	BS2_EXT_AUTH_MODE_FACE_FINGERPRINT_PIN,

	BS2_EXT_AUTH_MODE_FINGERPRINT_ONLY,
	BS2_EXT_AUTH_MODE_FINGERPRINT_FACE,
	BS2_EXT_AUTH_MODE_FINGERPRINT_PIN,
	BS2_EXT_AUTH_MODE_FINGERPRINT_FACE_OR_PIN,
	BS2_EXT_AUTH_MODE_FINGERPRINT_FACE_PIN,

	BS2_EXT_AUTH_MODE_CARD_ONLY,
	BS2_EXT_AUTH_MODE_CARD_FACE,
	BS2_EXT_AUTH_MODE_CARD_FINGERPRINT,
	BS2_EXT_AUTH_MODE_CARD_PIN,
	BS2_EXT_AUTH_MODE_CARD_FACE_OR_FINGERPRINT,
	BS2_EXT_AUTH_MODE_CARD_FACE_OR_PIN,
	BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_OR_PIN,
	BS2_EXT_AUTH_MODE_CARD_FACE_OR_FINGERPRINT_OR_PIN,
	BS2_EXT_AUTH_MODE_CARD_FACE_FINGERPRINT,
	BS2_EXT_AUTH_MODE_CARD_FACE_PIN,
	BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_FACE,
	BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_PIN,
	BS2_EXT_AUTH_MODE_CARD_FACE_OR_FINGERPRINT_PIN,
	BS2_EXT_AUTH_MODE_CARD_FACE_FINGERPRINT_OR_PIN,
	BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_FACE_OR_PIN,
//	BS2_EXT_AUTH_MODE_CARD_FACE_FINGERPRINT_PIN,
//	BS2_EXT_AUTH_MODE_CARD_FINGERPRINT_FACE_PIN,

	BS2_EXT_AUTH_MODE_ID_FACE,
	BS2_EXT_AUTH_MODE_ID_FINGERPRINT,
	BS2_EXT_AUTH_MODE_ID_PIN,
	BS2_EXT_AUTH_MODE_ID_FACE_OR_FINGERPRINT,
	BS2_EXT_AUTH_MODE_ID_FACE_OR_PIN,
	BS2_EXT_AUTH_MODE_ID_FINGERPRINT_OR_PIN,
	BS2_EXT_AUTH_MODE_ID_FACE_OR_FINGERPRINT_OR_PIN,
	BS2_EXT_AUTH_MODE_ID_FACE_FINGERPRINT,
	BS2_EXT_AUTH_MODE_ID_FACE_PIN,
	BS2_EXT_AUTH_MODE_ID_FINGERPRINT_FACE,
	BS2_EXT_AUTH_MODE_ID_FINGERPRINT_PIN,
	BS2_EXT_AUTH_MODE_ID_FACE_OR_FINGERPRINT_PIN,
	BS2_EXT_AUTH_MODE_ID_FACE_FINGERPRINT_OR_PIN,
	BS2_EXT_AUTH_MODE_ID_FINGERPRINT_FACE_OR_PIN,
//	BS2_EXT_AUTH_MODE_ID_FACE_FINGERPRINT_PIN,
//	BS2_EXT_AUTH_MODE_ID_FINGERPRINT_FACE_PIN,

	BS2_MAX_NUM_OF_EXT_AUTH_MODE = 128,		//TODO: confirm me
};

/**
 *  BS2AuthConfigExt
 */
typedef struct {
	BS2_SCHEDULE_ID extAuthSchedule[BS2_MAX_NUM_OF_EXT_AUTH_MODE];			///< 4 bytes * 128

	BS2_BOOL useGlobalAPB;													///< 1 byte
	BS2_GLOBAL_APB_FAIL_ACTION_TYPE globalAPBFailAction;		///< 1 byte
	BS2_BOOL useGroupMatching;											///< 1 byte
	uint8_t reserved;															///< 1 byte (packing)

	uint8_t reserved2[4];																		///< 4 bytes (reserved)

	BS2_BOOL usePrivateAuth;									///< 1 byte
	BS2_FACE_DETECTION_LEVEL faceDetectionLevel;		///< 1 byte
	BS2_BOOL useServerMatching;								///< 1 byte
	BS2_BOOL useFullAccess;										///< 1 byte

	uint8_t matchTimeout;			///< 1 byte
	uint8_t authTimeout;			///< 1 byte
	uint8_t numOperators;			///< 1 byte
	uint8_t reserved3[1];			///< 1 byte (packing)

	struct {
		BS2_USER_ID userID;					///< 32 bytes
		BS2_OPERATOR_LEVEL level;			///< 1 byte
		uint8_t reserved[3];					///< 3 bytes
	} operators[BS2_MAX_OPERATORS];

	uint8_t reserved4[256];		///< 256 bytes (reserved)
} BS2AuthConfigExt;

#endif	// __BS2_AUTH_CONFIG_EXT_H__
