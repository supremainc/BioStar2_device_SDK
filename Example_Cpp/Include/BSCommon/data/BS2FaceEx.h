/**
 *  Face
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

#ifndef __BS2_FACE_EX_H__
#define __BS2_FACE_EX_H__

#include "../BS2Types.h"

/**
 *	constants for BS2FaceEx
 */
enum {
	BS2_FACE_EX_TEMPLATE_SIZE = 552,		// sizeof(vfm::FACE_TEMPLATE)
	BS2_VISUAL_TEMPLATES_PER_FACE_EX = 10,
	BS2_IR_TEMPLATES_PER_FACE_EX = 10,
	BS2_MAX_TEMPLATES_PER_FACE_EX = BS2_VISUAL_TEMPLATES_PER_FACE_EX + BS2_IR_TEMPLATES_PER_FACE_EX,

	BS2_MAX_WARPED_IMAGE_LENGTH = 40 * 1024,		//TODO: confirm me
	BS2_MAX_WARPED_IR_IMAGE_LENGTH = 30 * 1024,		//TODO: confirm me
};

/**
 *	BS2_FACE_EX_FLAG
 */
enum {
	BS2_FACE_EX_FLAG_NONE	= 0x00,
	BS2_FACE_EX_FLAG_WARPED	= 0x01,		///< normalized image included

	BS2_FACE_EX_FLAG_TEMPLATE_ONLY = 0x20, 		///< use this in case of UNABLE_TO_SAVE_IMAGE_OF_VISUAL_FACE

	BS2_FACE_EX_FLAG_ALL	= 0xFF,
};

typedef uint8_t BS2_FACE_EX_FLAG;

/**
 * BS2_FACE_TEMPLATE_VERSION_F2
 * 0 = F2 V1.0.x, 1 = F2 V1.1.x, 2 = F2 V2.x
 *
 * 1'st byte value of BS2TemplateEx.data (Just for reference)
 */
enum {
	BS2_FACE_TEMPLATE_VERSION_F2_1_0_X = 0,		// F2 V1.0.x
	BS2_FACE_TEMPLATE_VERSION_F2_1_1_X = 1,		// F2 V1.1.x
	BS2_FACE_TEMPLATE_VERSION_F2_2_X_X = 2,		// F2 V2.x
};

/**
 * BS2_FACE_TEMPLATE_VERSION_BS3
 * 3 = BS3
 *
 * 1'st byte value of BS2TemplateEx.data (Just for reference)
 */
enum {
	BS2_FACE_TEMPLATE_VERSION_BS3 = 3,			// BS3
};

/**
 *	BS2TemplateEx
 */
typedef struct {
	uint8_t data[BS2_FACE_EX_TEMPLATE_SIZE];		///< 552 bytes
	uint8_t isIR;					///< 1 byte
	uint8_t reserved[3];		///< 3 bytes
} BS2TemplateEx;

/**
 *	BS2FaceEx
 */
typedef struct {
	uint8_t faceIndex;		///< 1 byte
	uint8_t numOfTemplate;		///< 1 byte
	BS2_FACE_EX_FLAG flag;			///< 1 byte
	uint8_t reserved;		///< 1 byte

	/**
	 * If "flag" includes BS2_FACE_EX_FLAG_WARPED, "imageLen" is for "imageData".
	 * Otherwise, "imageLen" is for "rawImageData".
	 */
	uint32_t imageLen;		///< 4 bytes

	union {
		struct {
			/**
			 * "irImageLen" is for "irImageData".
			 */
			uint16_t irImageLen;		///< 2 bytes
			uint8_t unused[6];			///< 6 bytes (packing)

			/**
			 * warped JPG
			 */
			uint8_t imageData[BS2_MAX_WARPED_IMAGE_LENGTH];		///< 40 * 1024 bytes

			/**
			 * warped IR JPG
			 */
			uint8_t irImageData[BS2_MAX_WARPED_IR_IMAGE_LENGTH];		///< 30 * 1024 bytes

			BS2TemplateEx templateEx[BS2_MAX_TEMPLATES_PER_FACE_EX];		///< 20 * 556 bytes
		};

		/**
		 * unwarped JPG
		 */
		uint8_t *rawImageData;

		BS2TemplateEx *onlyTemplateEx;	///< use this in case of UNABLE_TO_SAVE_IMAGE_OF_VISUAL_FACE
	};
} BS2FaceEx;

#endif	// __BS2_FACE_EX_H__
