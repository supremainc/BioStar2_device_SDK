/**
 *  Card Configuration Definitions
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

#ifndef __BS2_CARD_CONFIG_H__
#define __BS2_CARD_CONFIG_H__

#include "BS2Types.h"

/**
 *  Card Config constants
 */
enum {
	BS2_CARD_KEY_SIZE		= 32,
	BS2_CARD_MAGIC_NO		= 0x1F1F1F1F,
	BS2_CARD_MAX_TEMPLATES	= 4,	             ///< Maximum number of templates for Smart Card
};

/**
 *	BS2_CARD_DATA_TYPE
 */
enum {
	BS2_CARD_DATA_BINARY	= 0,
	BS2_CARD_DATA_ASCII		= 1,
	BS2_CARD_DATA_UTF16		= 2,
	BS2_CARD_DATA_BCD		= 3,
};

typedef uint8_t BS2_CARD_DATA_TYPE;

/**
 *  BS2_CARD_BYTE_ORDER
 */
enum {
	BS2_CARD_BYTE_ORDER_MSB	= 0,
	BS2_CARD_BYTE_ORDER_LSB	= 1,
};

typedef uint8_t BS2_CARD_BYTE_ORDER;

/**
 *  BS2MifareCard
 */
typedef struct {
	uint8_t primaryKey[6];
	uint8_t reserved1[2];
	uint8_t secondaryKey[6];
	uint8_t reserved2[2];
	uint16_t startBlockIndex;
	uint8_t reserved[6];
} BS2MifareCard;                             //24 Bytes

/**
 *  BS2IClassCard
 */
typedef struct {
	uint8_t primaryKey[8];
	uint8_t secondaryKey[8];
	uint16_t startBlockIndex;
	uint8_t reserved[6];
} BS2IClassCard;                             //24 Bytes

/**
 *  BS2DesFireCard
 */
typedef struct {
	uint8_t primaryKey[16];
	uint8_t secondaryKey[16];
	uint8_t appID[3];
	uint8_t fileID;
	uint8_t encryptionType;                    //for DesFire DES/3DES or AES. AES will be provided at future(TBD).
	uint8_t reserved[3];
} BS2DesFireCard;                            //40 Bytes

/**
 *	BS2SEosCard
 */
typedef struct {
	uint8_t oid_ADF[13];
	uint8_t size_ADF;
	uint8_t reserved1[2];				///< 16 bytes
	uint8_t oid_DataObjectID[8];		///< 24 bytes
	uint16_t size_DataObject[8];		///< 40 bytes
	uint8_t primaryKeyAuth[16];
	uint8_t secondaryKeyAuth[16];		///< 72 bytes
	uint8_t reserved2[24];
} BS2SeosCard;  						///< 96 bytes

/**
 *  BS2CardConfig
 */
typedef struct {
	BS2_CARD_BYTE_ORDER byteOrder;             ///< 1 byte
	BS2_BOOL useWiegandFormat;                 ///< 1 byte

	BS2_CARD_DATA_TYPE dataType;               ///< 1 byte
	BS2_BOOL useSecondaryKey;                  ///< 1 byte

	BS2MifareCard mifare;                      ///< 24 bytes
	BS2IClassCard iclass;                      ///< 24 bytes
	BS2DesFireCard desfire;                    ///< 40 bytes

	BS2_UID formatID;                          ///< 4 bytes (card format ID / use only application)
	
	BS2_BOOL cipher;                           ///< 1 byte (true : make card data from key) for XPASS - D2 KEYPAD
	
	uint8_t reserved[23];                      ///< 23 bytes (packing)

} BS2CardConfig;                               ///< 120 bytes

/**
 *	BS2CardConfigEx
 */
typedef struct {
	BS2SeosCard seos;			///< 96 bytes
	uint8_t reserved[24];
} BS2CardConfigEx;				///< 120 bytes

struct callbackCardInfo_
{
    int protocolType;           //Kind of card
    int cardType;               //BS2_CARD_TYPE
    size_t dataSize;
    size_t uidLen;
    unsigned char *cardData;
	unsigned char *iClassPACs;			
};

enum {
    //callbackCardInfo_ protocolType
    HF_MF = 61,
    HF_14443B,
    HF_15693,
    HF_FELICA,
    LF_EM,
    LF_PROX,
    HF_ICLASS,                
	HF_NFC,
	HF_SEOS,
	HF_SEOS_CONFIG,
};

#endif	// __BS2_CARD_CONFIG_H__
