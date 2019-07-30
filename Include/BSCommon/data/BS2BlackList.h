/**
 *  Card BlackList Definitions
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

#ifndef __BS2_BLACK_LIST_H__
#define __BS2_BLACK_LIST_H__

#include "BS2CSNCard.h"
#include "BS2User.h"

/**
 *	constants for black list
 */
enum {
	BS2_MAX_BLACK_LIST_SLOTS	= 1000,
};

/**
 *  BS2BlackList
 */
 typedef struct {
	uint8_t		cardID[BS2_CARD_DATA_SIZE];
	uint16_t	issueCount;
} BS2BlackList;

#endif	// __BS2_BLACK_LIST_H__
