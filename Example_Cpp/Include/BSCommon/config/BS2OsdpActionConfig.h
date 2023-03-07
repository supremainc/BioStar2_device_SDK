/*
 * BS2OsdpActionConfig.h
 *
 *  Created on: 2022. 8. 21.
 *      Author: yhlee
 */

#ifndef __BS2_OSDP_ACTION_CONFIG_H__
#define __BS2_OSDP_ACTION_CONFIG_H__

#include "BS2Types.h"
#include "data/BS2OsdpStandardAction.h"

#define MAX_OSDP_ACTION_COUNT   32

enum{
    BS2_OSDP_ACTION_NONE = 0,
    BS2_OSDP_ACTION_SUCCESS,
    BS2_OSDP_ACTION_FAIL,
    BS2_OSDP_ACTION_WAIT_INPUT,

    BS2_OSDP_ACTION_MAX_COUNT = BS2_OSDP_ACTION_WAIT_INPUT, 
};

typedef uint8_t BS2OsdpActionType;

typedef struct {
    BS2OsdpActionType       actionType;     ///< 1 byte     
    uint8_t                 reserved[3];    ///< 3 bytes
    BS2OsdpLedAction        led[2];         ///< 16 x 2 = 32 bytes
    BS2OsdpBuzzerAction     buzzer;         ///< 8 bytes
} BS2OsdpAction;                            ///< 44 bytes

typedef struct 
{
    uint8_t         version;                ///< 1 byte 
    uint8_t         reserved[3];            ///< 3 byes
    BS2OsdpAction   actions[MAX_OSDP_ACTION_COUNT]; ///< 44 x 32  = 1408
} BS2OsdpActionConfig;                      ///< 1412 bytes

#endif