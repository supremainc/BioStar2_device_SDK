/*
 * BS2OsdpStandardAction.h
 *
 *  Created on: 2022. 8. 21.
 *      Author: yhlee
 */

#ifndef __BS2_OSDP_STANDARD_ACTION_H__
#define __BS2_OSDP_STANDARD_ACTION_H__

#include "BS2Types.h"

/**
 *	BS2_OSDP_STANDARD_COLOR
 */
enum {
    BS2_OSDP_STANDARD_COLOR_BLACK = 0,
    BS2_OSDP_STANDARD_COLOR_RED,
    BS2_OSDP_STANDARD_COLOR_GREEN,
    BS2_OSDP_STANDARD_COLOR_AMBER,
    BS2_OSDP_STANDARD_COLOR_BLUE,
    BS2_OSDP_STANDARD_COLOR_MAGENTA,
    BS2_OSDP_STANDARD_COLOR_CYAN,
    BS2_OSDP_STANDARD_COLOR_WHITE
}; 
typedef uint8_t 	BS2_OSDP_STANDARD_COLOR;


/**
 *	BS2_OSDP_STANDARD_LED_COMMAND
 */
enum {
    BS2_OSDP_STANDARD_LED_COMMAND_NOP,
    BS2_OSDP_STANDARD_LED_COMMAND_CANCEL,
    BS2_OSDP_STANDARD_LED_COMMAND_SET
};
typedef uint8_t 	BS2_OSDP_STANDARD_LED_COMMAND;

typedef struct {
    BS2_BOOL                           use;            ///< 1 byte
    uint8_t                            readerNumber;   ///< 1 byte
    uint8_t                            ledNumber;      ///< 1 byte

    BS2_OSDP_STANDARD_LED_COMMAND      tempCommand;    ///< 1 byte
    uint8_t                            tempOnTime;     ///< 1 byte
    uint8_t                            tempOffTime;    ///< 1 byte
    BS2_OSDP_STANDARD_COLOR            tempOnColor;    ///< 1 byte
    BS2_OSDP_STANDARD_COLOR            tempOffColor;   ///< 1 byte
    uint16_t                           tempRunTime;    ///< 2 bytes

    BS2_OSDP_STANDARD_LED_COMMAND      permCommand;    ///< 1 byte
    uint8_t                            permOnTime;     ///< 1 byte
    uint8_t                            permOffTime;    ///< 1 byte
    BS2_OSDP_STANDARD_COLOR            permOnColor;    ///< 1 byte
    BS2_OSDP_STANDARD_COLOR            permOffColor;   ///< 1 byte

    uint8_t                            reserved;       ///< 1 byte
} BS2OsdpStandardLedAction;            ///< 16 bytes


/**
 *	BS2_OSDP_STANDARD_TONE
 */
enum {
    BS2_OSDP_STANDARD_TONE_NONE   = 0,
    BS2_OSDP_STANDARD_TONE_OFF, 
    BS2_OSDP_STANDARD_TONE_ON
};
typedef uint8_t 	BS2_OSDP_STANDARD_TONE;


/**
 *	BS2_OSDP_STANDARD_ACTION_TYPE
 */
typedef struct {
    BS2_BOOL                    use;            ///< 1 byte
    uint8_t                     readerNumber;   ///< 1 byte
    BS2_OSDP_STANDARD_TONE      tone;           ///< 1 byte
    uint8_t                     onTime;         ///< 1 byte
    uint8_t                     offTime;        ///< 1 byte
    uint8_t                     numOfCycle;     ///< 1 byte
    uint8_t                     reserved[2];    ///< 2 bytes
} BS2OsdpStandardBuzzerAction;                  ///< 8 bytes

#endif