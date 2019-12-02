/**
 *  Device Zone Master
 *
 *  @author mkkim@suprema.co.kr
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

#ifndef __BS2_DEVICE_ZONE_MASTER_H__
#define __BS2_DEVICE_ZONE_MASTER_H__


#include <string.h>
#include "../BS2Types.h"
#include "../data/BS2Action.h"
#include "../data/BS2Zone.h"

typedef struct 
{
	bool enable;
	uint8_t reserved1[1];		//packing
	BS2_PORT listenPort;		//51214	
	uint8_t reserved[4];		//packing
} BS2DeviceZoneMasterConfig;	// 8 bytes


#endif	// __BS2_DEVICE_ZONE_MASTER_H__