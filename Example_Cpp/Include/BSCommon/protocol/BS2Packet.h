/**
 *  Packet between Devices and BioStar Server
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

#ifndef __BS2_PAKCET_H__
#define __BS2_PAKCET_H__

typedef uint16_t BS2_PACKET_SEQ;

typedef struct {
	uint32_t		value1;
	uint32_t		value2;
} BS2PayloadHeader;

#endif	// __BS2_PAKCET_H__
