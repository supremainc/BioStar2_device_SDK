#ifndef __BS2_THERMAL_CAMERA_CONFIG_H__
#define __BS2_THERMAL_CAMERA_CONFIG_H__

#include "BS2Types.h"

typedef struct {
    uint8_t distance;              ///< 1 byte
    uint8_t emissionRate;          ///< 1 byte

    struct {
		 uint16_t x;                 ///< 2 bytes
		 uint16_t y;                 ///< 2 bytes
		 uint16_t width;             ///< 2 bytes
		 uint16_t height;            ///< 2 bytes
    } roi;

    BS2_BOOL useBodyCompensation;  ///< 1 byte
    int8_t compensationTemperature;	///< 1 byte : -50 ~ 50
} BS2ThermalCameraConfig;

#endif
