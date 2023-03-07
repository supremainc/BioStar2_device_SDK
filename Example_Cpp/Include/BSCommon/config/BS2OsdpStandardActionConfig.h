/*
 * BS2OsdpStandardActionConfig.h
 *
 *  Created on: 2022. 8. 21.
 *      Author: yhlee
 */

#ifndef __BS2_OSDP_STANDARD_ACTION_CONFIG_H__
#define __BS2_OSDP_STANDARD_ACTION_CONFIG_H__

#include "BS2Types.h"
#include "data/BS2OsdpStandardAction.h"


#define BS2_OSDP_STANDARD_ACTION_MAX_COUNT      32
#define BS2_OSDP_STANDARD_ACTION_MAX_LED        2

/**
 *	BS2_OSDP_STANDARD_ACTION_TYPE
 */
enum {
    BS2_OSDP_STANDARD_ACTION_TYPE_NONE = 0,
    BS2_OSDP_STANDARD_ACTION_TYPE_SUCCESS,
    BS2_OSDP_STANDARD_ACTION_TYPE_FAIL,
    BS2_OSDP_STANDARD_ACTION_TYPE_WAIT_INPUT,

    BS2_OSDP_STANDARD_ACTION_TYPE_COUNT = BS2_OSDP_STANDARD_ACTION_TYPE_WAIT_INPUT, 
};
typedef uint8_t     BS2_OSDP_STANDARD_ACTION_TYPE;


/**
 *	BS2OsdpStandardAction
 */
typedef struct {
    BS2_OSDP_STANDARD_ACTION_TYPE   actionType;     ///< 1 byte     
    uint8_t                         reserved[3];    ///< 3 bytes
    BS2OsdpStandardLedAction        led[BS2_OSDP_STANDARD_ACTION_MAX_LED];         ///< 16 x 2 = 32 bytes
    BS2OsdpStandardBuzzerAction     buzzer;         ///< 8 bytes
} BS2OsdpStandardAction;                            ///< 44 bytes


/**
 *	BS2OsdpStandardActionConfig
 */
typedef struct {
    uint8_t                 version;                ///< 1 byte 
    uint8_t                 reserved[3];            ///< 3 bytes
    BS2OsdpStandardAction   actions[BS2_OSDP_STANDARD_ACTION_MAX_COUNT]; ///< 44 x 32  = 1408
} BS2OsdpStandardActionConfig;                      ///< 1412 bytes

#endif