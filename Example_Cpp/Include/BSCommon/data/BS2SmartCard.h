/*
 * BS2SmartCard.h
 *
 *  Created on: 2015. 9. 2.
 *      Author: scpark
 */

#ifndef _BS2_SMART_CARD_H_
#define _BS2_SMART_CARD_H_

#include "../BS2Types.h"
#include "BS2CSNCard.h"
#include "BS2Fingerprint.h"


/**
 *  Card Data Structure
 *
 * [1K Data Card (Total Data Block = 720 bytes)]
 * header(16) + cardID(32) + pin(32) + Templates(300*2) + AccessGroup(32) + ExpiryDate(8) = 720 bytes
 *
 * [4K Data Card (Total Data Block = 1656 bytes)]
 * header(16) + cardID(32) + pin(32) + Templates(384*4) + AccessGroup(32) + ExpiryDate(8) = 1656 bytes
 */

#define MAX_SMART_CARD_DATA_SIZE	1656

enum {
	BS2_SMART_CARD_MAX_TEMPLATE_COUNT = 4,
	BS2_SMART_CARD_MIN_TEMPLATE_SIZE = 300,
	BS2_SMART_CARD_MAX_ACCESS_GROUP_COUNT = 16,
};

typedef struct {
	uint16_t hdrCRC;				///< 2 bytes
	uint16_t cardCRC;				///< 2 bytes

	BS2_CARD_TYPE cardType;			///< 1 byte, Can operate in BS2_CARD_TYPE_SECURE or BS2_CARD_TYPE_ACCESS.
	uint8_t numOfTemplate;			///< 1 byte
	uint16_t templateSize;			///< 2 bytes, BS2_SMART_CARD_MIN_TEMPLATE_SIZE ~ BS2_FINGER_TEMPLATE_SIZE, or BS2_FACE_TEMPLATE_SIZE

	uint16_t issueCount;			///< 2 bytes
	uint8_t duressMask;				///< 1 byte
	uint8_t cardAuthMode;			///< 1 byte

	uint8_t useAlphanumericID;		///< 1 byte
	uint8_t cardAuthModeEx;			///< 1 byte : for FaceStation F2 only
	uint8_t numOfFaceTemplate;		///< 1 byte : for FaceStation F2 only
	uint8_t reserved[1];			///< 1 byte (packing)
} BS2SmartCardHeader;

typedef struct {
	uint8_t pin[BS2_PIN_HASH_SIZE];		///< 32 bytes
	uint8_t templateData[BS2_SMART_CARD_MAX_TEMPLATE_COUNT * BS2_FINGER_TEMPLATE_SIZE];		///< 4 * 384 bytes
} BS2SmartCardCredentials;

typedef struct {
	uint16_t accessGroupID[BS2_SMART_CARD_MAX_ACCESS_GROUP_COUNT];		///< 2 * 16 bytes
	BS2_DATETIME startTime;		///< 4 bytes
	BS2_DATETIME endTime;			///< 4 bytes
} BS2AccessOnCardData;

typedef struct {
	BS2SmartCardHeader header;                        ///< 16 bytes
	uint8_t cardID[BS2_CARD_DATA_SIZE];               ///< 32 bytes
	BS2SmartCardCredentials credentials;              ///< 1568 bytes
	BS2AccessOnCardData accessOnData;                 ///< 40 bytes
} BS2SmartCardData;                                 ///< 1656 bytes

#endif /* _BS2_SMART_CARD_H_ */
