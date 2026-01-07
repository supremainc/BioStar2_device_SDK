/**
 *  	Input Ouput Status Codes
 *
 *  	@author yhlee@suprema.co.kr
 *  	@see
 */

/*
 *  Copyright (c) 2012 Suprema Co., Ltd. All Rights Reserved.
 *
 *  This software is the confidential and proprietary information of
 *  Suprema Co., Ltd. ("Confidential Information").  You shall not
 *  disclose such Confidential Information and shall use it only in
 *  accordance with the terms of the license agreement you entered into
 *  with Suprema.
 */

#ifndef BS2_IO_H_INCLUDED
#define BS2_IO_H_INCLUDED

#include "../BS2Types.h"

enum {
    MAX_IO_PORT_COUNT = 32,
    MAX_SIO_PORT_COUNT = 16,
    MAX_TIME_LIMIT_OF_RUN_ACTION = 86400,   //
};

enum{
    BS2_IO_TYPE_NONE = 0,

    BS2_IO_TYPE_AUX_IN,
    BS2_IO_TYPE_AUX_OUT,
    BS2_IO_TYPE_TAMPER,
    BS2_IO_TYPE_SUPERVISOR_INPUT,    
    BS2_IO_TYPE_INPUT,          //BS2_ACTION_GENERAL_INPUT
    
    BS2_IO_TYPE_RELAY,          //BS2_ACTION_RELAY
    BS2_IO_TYPE_OUTPUT,         //BS2_ACTION_TTL
};

typedef uint8_t BS2_IO_TYPES; 


typedef struct {
    BS2_DEVICE_ID deviceID;
    BS2_IO_TYPES ioType;
    uint8_t ioIndex;
    BS2_PORT_VALUE oldStatus;
    BS2_PORT_VALUE newStatus;	
} BS2IONotification; 

typedef struct{
    uint8_t count;
    uint8_t reserved[3];
    BS2_PORT_VALUE status[MAX_IO_PORT_COUNT];
} BS2_IO_STATUS;

typedef struct{
    BS2_DEVICE_ID deviceID;
    
    BS2_IO_STATUS input;
    BS2_IO_STATUS output;
    BS2_IO_STATUS relay;
    BS2_IO_STATUS tamper;
    BS2_IO_STATUS auxIn;
    BS2_IO_STATUS auxOut;
    
    uint8_t numOfSupervisorInput;
    BS2_PORT_VALUE supervisorInputStatus[MAX_SIO_PORT_COUNT];
    uint8_t reserved1[3];
    uint8_t reserved2[16];

} BS2IOStatus;

#endif
