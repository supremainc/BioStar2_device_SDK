/**
 *  System Configuration Definitions
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

#ifndef __BS2_SYSTEM_CONFIG_H__
#define __BS2_SYSTEM_CONFIG_H__

#include "BS2Types.h"
#include "BS2DisplayConfig.h"

/**
 *	BS2SystemConfig
 */
typedef struct {
#if 0
	char tnaLabel[BS2_MAX_TNA_KEY][BS2_MAX_TNA_LABEL_LEN];
#else
	uint8_t notUsed[16 * 16 * 3];
#endif

	int32_t timezone;                                    ///< 4 bytes (offset of GMT in second)

	BS2_BOOL syncTime;                                   ///< 1 byte (sync time with server)
	BS2_BOOL serverSync;                                 ///< 1 byte
	BS2_BOOL deviceLocked;                               /// 1 byte
	BS2_BOOL useInterphone;                              ///< 1 byte

	BS2_BOOL useUSBConnection;                           ///< 1 byte
	BS2_BOOL keyEncrypted;                               ///< 1 byte
	BS2_BOOL useJobCode;                                 ///< 1 byte
	BS2_BOOL useAlphanumericID;                          ///< 1 byte

	uint32_t cameraFrequency;                            ///< 4 bytes

	BS2_BOOL secureTamper;                               ///< 1 byte
private:
	BS2_BOOL tamperOn;                                   ///< 1 byte   (write protected)
	uint8_t reserved[2];                                 ///< 2 bytes
	uint8_t reserved2[20];                               ///< 20 bytes (reserved)
} BS2SystemConfig;

#endif	// __BS2_SYSTEM_CONFIG_H__
