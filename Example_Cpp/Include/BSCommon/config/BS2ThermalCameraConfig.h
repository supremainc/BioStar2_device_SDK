#ifndef __BS2_THERMAL_CAMERA_CONFIG_H__
#define __BS2_THERMAL_CAMERA_CONFIG_H__

#include "BS2Types.h"

enum {
	BS2_THERMAL_CAMERA_DISTANCE_DEFAULT = 100,
	BS2_THERMAL_CAMERA_EMISSION_RATE_DEFAULT = 98,
	BS2_THERMAL_CAMERA_EMISSIVITY_DEFAULT = 98,

	BS2_THERMAL_CAMERA_ROI_X_DEFAULT = 30,				// F2
	BS2_THERMAL_CAMERA_ROI_Y_DEFAULT = 25,				// F2
	BS2_THERMAL_CAMERA_ROI_WIDTH_DEFAULT = 50,			// F2
	BS2_THERMAL_CAMERA_ROI_HEIGHT_DEFAULT = 55,			// F2
	BS2_THERMAL_CAMERA_ROI_X_DEFAULT_FS2 = 47,			// FS2
	BS2_THERMAL_CAMERA_ROI_Y_DEFAULT_FS2 = 45,			// FS2
	BS2_THERMAL_CAMERA_ROI_WIDTH_DEFAULT_FS2 = 15,		// FS2
	BS2_THERMAL_CAMERA_ROI_HEIGHT_DEFAULT_FS2 = 10,		// FS2
};

// Deprecated in V2.8. Instead, use the BS2_THERMAL_CAMERA_EMISSIVITY_DEFAULT
#define BS2_THERMAL_CAMERA_EMISSION_RATE_DEFAULT (DEPRECATED_ENUM)BS2_THERMAL_CAMERA_EMISSION_RATE_DEFAULT

typedef struct {
    uint8_t distance;              ///< 1 byte : 0 ~ 244 (cm)
    uint8_t emissionRate;          ///< 1 byte : 95 / 97 / 98 (emissivity)

    struct {
		uint16_t x;                 ///< 2 bytes : 0 ~ 99 (%)
		uint16_t y;                 ///< 2 bytes : 0 ~ 99 (%)
		uint16_t width;             ///< 2 bytes : 0 ~ 99 (%)
		uint16_t height;            ///< 2 bytes : 0 ~ 99 (%)
    } roi;

    BS2_BOOL useBodyCompensation;  ///< 1 byte
    int8_t compensationTemperature;	///< 1 byte : -50 ~ 50
} BS2ThermalCameraConfig;

#endif
