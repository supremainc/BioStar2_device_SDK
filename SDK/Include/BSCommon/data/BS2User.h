/**
 *  User Definitions
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

#ifndef __BS2_USER_H__
#define __BS2_USER_H__

#include "../BS2Types.h"
#include "BS2CSNCard.h"
#include "BS2Fingerprint.h"
#include "BS2Face.h"

/**
 *	BS2_USER_MASK
 */
enum {
	BS2_USER_MASK_ID_ONLY		= 0x0000,
	BS2_USER_MASK_DATA			= 0x0001,
	BS2_USER_MASK_SETTING		= 0x0002,
	BS2_USER_MASK_NAME			= 0x0004,
	BS2_USER_MASK_PHOTO			= 0x0008,
	BS2_USER_MASK_PIN			= 0x0010,
	BS2_USER_MASK_CARD			= 0x0020,
	BS2_USER_MASK_FINGER		= 0x0040,
	BS2_USER_MASK_FACE			= 0x0080,
	BS2_USER_MASK_ACCESS_GROUP	= 0x0100,
	BS2_USER_MASK_JOB	          = 0x0200,
	BS2_USER_MASK_PHRASE	      = 0x0400,
	BS2_USER_MASK_ALL			= 0xFFFF,
};

typedef uint32_t BS2_USER_MASK;

/**
 *	constants
 */
enum {
	BS2_USER_PIN_SIZE		= 32,			///< 16 byte -> 32 byte hash value
	BS2_USER_NAME_SIZE		= 48 * 4,		///< UTF-8 Encoding
	BS2_USER_PHOTO_SIZE		= 16 * 1024,
	BS2_MAX_JOB_SIZE = 16,
	BS2_MAX_JOBLABEL_LEN = 16 * 3,
	BS2_USER_PHRASE_SIZE		= 32 * 4,		///< UTF-8 Encoding

	BS2_INVALID_USER_ID = 0,
};

/**
 *	BS2_USER_PIN
 */
typedef BS2_HASH256 BS2_USER_PIN;

/**
 *	BS2_USER_NAME
 */
typedef uint8_t BS2_USER_NAME[BS2_USER_NAME_SIZE];

/**
 *	BS2UserPhoto
 */
typedef struct {
#if 0
	int32_t		format;
#endif
	uint32_t	size;
	uint8_t		data[BS2_USER_PHOTO_SIZE];
} BS2UserPhoto;

/**
 *	BS2_USER_PHRASE
 */
typedef uint8_t BS2_USER_PHRASE[BS2_USER_PHRASE_SIZE];

/**
 *	BS2_USER_FLAG
 */
enum {
	BS2_USER_FLAG_NONE		= 0x00,		///< Same as server
	BS2_USER_FLAG_CREATED	= 0x01,		///< User is created on device
	BS2_USER_FLAG_UPDATED	= 0x02,		///< User is updated on device
	BS2_USER_FLAG_DELETED	= 0x04,		///< User is deleted from device (not used yet)
	BS2_USER_FLAG_DISABLED	= 0x80,

	BS2_USER_FLAG_ALL		= 0xFF,
};

typedef uint8_t BS2_USER_FLAG;

/**
 *	BS2_USER_SECURITY_LEVEL
 */
enum {
	BS2_USER_SECURITY_LEVEL_DEFAULT,
	BS2_USER_SECURITY_LEVEL_LOWER,
	BS2_USER_SECURITY_LEVEL_LOW,
	BS2_USER_SECURITY_LEVEL_NORMAL,
	BS2_USER_SECURITY_LEVEL_HIGH,
	BS2_USER_SECURITY_LEVEL_HIGHER,
};

typedef uint8_t BS2_USER_SECURITY_LEVEL;

/**
*	BS2_JOB_CODE
*/
enum {
	BS2_JOB_CODE_UNSPECIFIED = 0,
	BS2_MAX_JOB_CODE = 16,
};

typedef uint32_t BS2_JOB_CODE;

/**
*	BS2_JOB_LABEL
*/
typedef uint8_t BS2_JOB_LABEL[BS2_MAX_JOBLABEL_LEN];

/**
*	BS2Job
*/
typedef struct {
	uint8_t numJobs;                     ///< 1 byte
	uint8_t reserved[3];                 ///< 3 bytes

	struct {
		BS2_JOB_CODE code;                 ///< 4 bytes
		BS2_JOB_LABEL label;               ///< 48 bytes
	} jobs[BS2_MAX_JOB_SIZE];
} BS2Job;

/**
 *	BS2User
 */
typedef struct {
	BS2_USER_ID			userID;				///< 32 bytes

	uint8_t				formatVersion;		///< 1 byte (User format version)
	BS2_USER_FLAG		flag;				///< 1 byte
	uint16_t			version;			///< 2 byte

	uint8_t				numCards;			///< 1 byte
	uint8_t				numFingers;			///< 1 byte
	uint8_t				numFaces;			///< 1 byte
	uint8_t				reserved2[1];		///< 1 byte

#if BS2_SDK_V1_BUILD
	BS2_CHECKSUM		fingerChecksum;		///< 4 bytes
#else
	BS2_AUTH_GROUP_ID	authGroupID;		///< 4 bytes
#endif
	BS2_CHECKSUM		faceChecksum;		///< 4 bytes
} BS2User;

/**
 *	BS2UserSetting
 */
typedef struct {
	BS2_DATETIME		startTime;			///< 4 bytes
	BS2_DATETIME		endTime;			///< 4 bytes

	uint8_t				fingerAuthMode;		///< 1 byte
	uint8_t				cardAuthMode;		///< 1 byte
	uint8_t				idAuthMode;			///< 1 byte
	uint8_t				securityLevel;		///< 1 byte

	// Optional Information (varies by mask)
	/*
	BS2User				user;					///< 32 bytes		- User configuration
	BS2_USER_NAME		name;					///< 48*4 bytes		- User name (encoded with UTF-8)
	BS2_USER_PHOTO		photo;					///< n bytes		- User photo size (photoSize)
	BS2_HASH256			pin;					///< 32 bytes		- User PIN (hashed by SHA-256)
	BS2CSNCard			cards[n];				///< User cards
	BS2FingerTemplate	fingerTemplates[n];		///< User fingerprint templates
	BS2FaceTemplate		faceTemplates[n];		///< User Face templates
	BS2_ACCESS_GROUP_ID	accessGroupID[n];		///< User access group
	BS2Job	jobs;		///< User job codes
	*/
} BS2UserSetting;

#endif	// __BS2_USER_H__
