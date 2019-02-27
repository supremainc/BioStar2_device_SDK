/**
 *  Face
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

#ifndef __BS2_FACE_H__
#define __BS2_FACE_H__

#include "BS2Types.h"

/**
 *	constants for BS2Face
 */
enum {
	BS2_FACE_TEMPLATE_LENGTH = 3008,		// assert(BS2_FACE_TEMPLATE_LENGTH * 30 % 16 == 0)
	BS2_TEMPLATE_PER_FACE	= 30,

	BS2_FACE_IMAGE_SIZE = 16 * 1024,
};

/**
 *	BS2Face
 */
typedef struct {
	uint8_t faceIndex;		///< 1 byte
	uint8_t numOfTemplate;			///< 1 byte
	uint8_t flag;		///< 1 byte
	uint8_t reserved;			///< 1 byte

	uint16_t imageLen;		///< 2 bytes
	uint8_t reserved2[2];			///< 2 bytes

	uint8_t imageData[BS2_FACE_IMAGE_SIZE];
	uint8_t templateData[BS2_TEMPLATE_PER_FACE][BS2_FACE_TEMPLATE_LENGTH];
} BS2Face;

#endif	// __BS2_FACE_H__
