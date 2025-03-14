/* 
 *  Copyright (c) 2014 Suprema Co., Ltd. All Rights Reserved.
 * 
 *  This software is the confidential and proprietary information of
 *  Suprema Co., Ltd. ("Confidential Information").  You shall not
 *  disclose such Confidential Information and shall use it only in
 *  accordance with the terms of the license agreement you entered into
 *  with Suprema.
 */

#ifndef _BS2_FACILITY_CODE_CONFIG_H_
#define _BS2_FACILITY_CODE_CONFIG_H_

#include "BS2Types.h"

enum{
	BS2_FACILITY_CODE_SIZE = 4, 
	BS2_MAX_NUMBER_FACILITY_CODE = 16, 
};

typedef struct{
    uint8_t code[BS2_FACILITY_CODE_SIZE]; 
} BS2FacilityCode; 

typedef struct{
		uint8_t numFacilityCode; 			// 1
		uint8_t reserved0[3]; 				// 3

		BS2FacilityCode facilityCodes[BS2_MAX_NUMBER_FACILITY_CODE]; 

        uint8_t reserved1[128]; 
} BS2FacilityCodeConfig;


#endif