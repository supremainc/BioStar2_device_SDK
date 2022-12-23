/*
 * BS2LicenseConfig.h
 *
 *  Created on: 2022. 8. 22.
 *      Author: yhlee
 */

#ifndef __BS2_LICENSE_CONFIG_H__
#define __BS2_LICENSE_CONFIG_H__

#include "BS2Types.h"


#define BS2_MAX_LICENSE_COUNT     16

/**
 *	BS2_LICENSE_STATUS
 */
enum {
    BS2_LICENSE_STATUS_NOT_SUPPORTED,
    BS2_LICENSE_STATUS_DISABLE,
    BS2_LICENSE_STATUS_ENABLE,
    BS2_LICENSE_STATUS_EXPIRED,
};

typedef uint8_t            BS2_LICENSE_STATUS;


/**
 *	BS2LicenseResult
 */
#pragma pack(push, 1)
typedef struct {
    BS2_DEVICE_ID       deviceID;
    BS2_LICENSE_STATUS  status;
} BS2LicenseResult;
#pragma pack(pop)


/**
 *	BS2_LICENSE_TYPE
 */
enum {
    BS2_LICENSE_TYPE_NONE           = 0x0000,
    BS2_LICENSE_TYPE_VISUAL_QR_MASK = 0x0001,
    BS2_LICENSE_TYPE_MAX_MASK       = BS2_LICENSE_TYPE_VISUAL_QR_MASK,
};

typedef uint16_t			BS2_LICENSE_TYPE;


/**
 *	BS2_LICENSE_SUB_TYPE
 */
enum {
    BS2_LICENSE_SUB_TYPE_NONE,
    BS2_LICENSE_SUB_TYPE_VISUAL_QR_CODE_CORP,
};

typedef uint16_t			BS2_LICENSE_SUB_TYPE;


/*
 *  BS2LicenseBlob
 */
typedef struct {
    BS2_LICENSE_TYPE    licenseType;
    uint16_t            numOfDevices;
    BS2_DEVICE_ID*      deviceIDObjs;
    uint32_t            licenseLen;
    uint8_t*            licenseObj;
} BS2LicenseBlob;


/**
 *	BS2License
 */
typedef struct {
    uint8_t               index;          ///< 1 byte
    uint8_t               hasCapability;  ///< 1 byte 
    uint8_t               enable;         ///< 1 byte
    uint8_t               reserved;       ///< 1 byte
    BS2_LICENSE_TYPE      licenseType;    ///< 2 bytes
    BS2_LICENSE_SUB_TYPE  licenseSubType; ///< 2 bytes   
    uint32_t              enableTime;     ///< 4 bytes
    uint32_t              expiredTime;    ///< 4 bytes
    uint32_t              issueNumber;    ///< 4 bytes
    uint8_t               name[BS2_USER_ID_SIZE]; ///< 32 bytes
} BS2License;            ///< 52 bytes


/**
 *	BS2LicenseConfig
 */
typedef struct {
    uint8_t         version;                ///< 1 byte 
    uint8_t         numOfLicense;           ///< 1 byte
    uint8_t         reserved[2];            ///< 2 bytes
    BS2License      license[BS2_MAX_LICENSE_COUNT]; ///< 20 x 16  = 832
    uint8_t         reserved1[16];           ///< 16 bytes
} BS2LicenseConfig;      ///< 842 bytes

#endif