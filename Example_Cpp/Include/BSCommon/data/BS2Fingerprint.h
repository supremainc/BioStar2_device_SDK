/**
 *  Fingerprint
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

#ifndef __BS2_FINGERPRINT_H__
#define __BS2_FINGERPRINT_H__

#include "../BS2Types.h"

/**
 *	constants for BS2Fingerprint
 */
enum {
	BS2_FINGER_TEMPLATE_SIZE	= 384,
	BS2_TEMPLATE_PER_FINGER		= 2,
};

/**
 * BS2_FINGER_INDEX
 */
enum {
	BS2_FINGER_INDEX_UNDEFINED = 0xFF,
};

/**
 *  BS2_FINGER_FLAG
 */
enum {
	BS2_FINGER_FLAG_DURESS		= 0x01,
};

typedef uint8_t BS2_FINGER_FLAG;

/**
 *	BS2RawImage
 */
typedef struct {
	uint16_t width;		///< 2 bytes
	uint16_t height;	///< 2 bytes
	uint32_t size;		///< 4 bytes
	uint8_t data[1];	///< n bytes (not-fixed size)
} BS2RawImage;

/**
 *	BS2ScanFingerprint
 */
typedef struct {
	uint8_t fingerTemplate[BS2_FINGER_TEMPLATE_SIZE];

	// optional
	BS2RawImage rawImage;
} BS2ScanFingerprint;

typedef struct {
	BS2_USER_ID userID;
	uint8_t fingerTemplate[BS2_FINGER_TEMPLATE_SIZE];
} BS2VerifyFingerprint;

/**
 *	BS2Fingerprint
 */
typedef struct {
	uint8_t index;			///< 1 bytes
	uint8_t flag;			///< 1 bytes
	uint8_t reserved[2];	///< 2 bytes
	uint8_t data[BS2_TEMPLATE_PER_FINGER][BS2_FINGER_TEMPLATE_SIZE];		///< 2 * 384 bytes
} BS2Fingerprint;
#endif	// __BS2_FINGERPRINT_H__
