/*
 * BS2SmartCard.h
 *
 *  Created on: 2015. 9. 2.
 *      Author: scpark
 */

#ifndef _BS2_SMART_CARD_H_
#define _BS2_SMART_CARD_H_

#include "BS2Types.h"
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
	uint16_t hdrCRC;
	uint16_t cardCRC;
	BS2_CARD_TYPE cardType;                           // Can operate in BS2_CARD_TYPE_SECURE or BS2_CARD_TYPE_ACCESS.
	uint8_t numOfTemplate;
	uint16_t templateSize;                            // The valid range is BS2_SMART_CARD_MIN_TEMPLATE_SIZE to BS2_FINGER_TEMPLATE_SIZE.	
	uint16_t issueCount;
	uint8_t duressMask;
	uint8_t cardAuthMode;
	uint8_t useAlphanumericID;
	uint8_t reserved[3];
} BS2SmartCardHeader;

typedef struct {
	uint8_t pin[BS2_PIN_HASH_SIZE];
	uint8_t templateData[BS2_SMART_CARD_MAX_TEMPLATE_COUNT * BS2_FINGER_TEMPLATE_SIZE];
} BS2SmartCardCredentials;

typedef struct {
	uint16_t accessGroupID[BS2_SMART_CARD_MAX_ACCESS_GROUP_COUNT];
	BS2_DATETIME startTime;
	BS2_DATETIME endTime;
} BS2AccessOnCardData;

typedef struct {
	BS2SmartCardHeader header;                        ///< 16 bytes
	uint8_t cardID[BS2_CARD_DATA_SIZE];               ///< 32 bytes
	BS2SmartCardCredentials credentials;              ///< 1568 bytes
	BS2AccessOnCardData accessOnData;                 ///< 40 bytes
} BS2SmartCardData;                                 ///< 1656 bytes

#endif /* _BS2_SMART_CARD_H_ */
