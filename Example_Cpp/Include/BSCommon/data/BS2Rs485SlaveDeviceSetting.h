/**
 *  RS485 Channel Setting
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

#ifndef __BS2_RS485_SLAVE_DEVICE_SETTING_H__
#define __BS2_RS485_SLAVE_DEVICE_SETTING_H__

#include "../BS2Types.h"

/**
 *	BS2Rs485SlaveDeviceSetting
 */
typedef struct {
	BS2_DEVICE_ID	deviceID;		///< 4 bytes
	BS2_DEVICE_TYPE	deviceType;		///< 2 bytes
	uint8_t			reserved[2];	///< 2 bytes (packing)
} BS2Rs485SlaveDeviceSetting;

#endif	// __BS2_RS485_SLAVE_DEVICE_SETTING_H__
