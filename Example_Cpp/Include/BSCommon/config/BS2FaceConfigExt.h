#ifndef __BS2_FACE_CONFIG_EXT_H__
#define __BS2_FACE_CONFIG_EXT_H__

#include "BS2Types.h"

enum {
	BS2_FACE_CHECK_MODE_OFF = 0,
	BS2_FACE_CHECK_MODE_HARD,		// prevent access
	BS2_FACE_CHECK_MODE_SOFT,		// overlook violation
	BS2_FACE_CHECK_MODE_DENY_MASK,	// deny mask BDP-938

	BS2_FACE_CHECK_MODE_DEFAULT = BS2_FACE_CHECK_MODE_OFF,
};

typedef uint8_t BS2_FACE_CHECK_MODE;

enum {
	BS2_FACE_CHECK_AFTER_AUTH = 0,
	BS2_FACE_CHECK_BEFORE_AUTH,
	BS2_FACE_CHECK_WITHOUT_AUTH,

	BS2_FACE_CHECK_ORDER_DEFAULT = BS2_FACE_CHECK_AFTER_AUTH,
};

typedef uint8_t BS2_FACE_CHECK_ORDER;

enum {
	BS2_MASK_DETECTION_OFF,
	BS2_MASK_DETECTION_STRICT,
	BS2_MASK_DETECTION_MORE_STRICT,
	BS2_MASK_DETECTION_MOST_STRICT,

	BS2_MASK_DETECTION_LEVEL_DEFAULT = BS2_MASK_DETECTION_OFF,
};

typedef uint8_t BS2_MASK_DETECTION_LEVEL;

enum {
	BS2_TEMPERATURE_FORMAT_FAHRENHEIT = 0,
	BS2_TEMPERATURE_FORMAT_CELSIUS = 1,

	BS2_TEMPERATURE_FORMAT_DEFAULT = BS2_TEMPERATURE_FORMAT_CELSIUS,

	BS2_THERMAL_THRESHOLD_MIN = 1 * 100,
	BS2_THERMAL_THRESHOLD_MAX = 45 * 100,

	BS2_THERMAL_THRESHOLD_LOW_DEFAULT = 32 * 100,
	BS2_THERMAL_THRESHOLD_HIGH_DEFAULT = 38 * 100,
	BS2_THERMAL_THRESHOLD_DEFAULT = 3800,	// Deprecated V2.7.2
};

#define BS2_THERMAL_THRESHOLD_DEFAULT		(DEPRECATED_ENUM)BS2_THERMAL_THRESHOLD_DEFAULT

typedef struct {
	BS2_FACE_CHECK_MODE thermalCheckMode;			///< 1 byte
	BS2_FACE_CHECK_MODE maskCheckMode;					///< 1 byte
	uint8_t reserved[2];				///< 2 bytes (packing)

	uint8_t thermalFormat;						///< 1 byte
	uint8_t reserved2;							///< 1 byte (packing)
	uint16_t thermalThresholdLow;		///< 2 bytes : Celsius * 100

	uint16_t thermalThresholdHigh;				///< 2 bytes : Celsius * 100
	BS2_MASK_DETECTION_LEVEL maskDetectionLevel;		///< 1 byte
	BS2_BOOL auditTemperature;					///< 1 byte

	BS2_BOOL useRejectSound;				///< 1 byte
	BS2_BOOL useOverlapThermal;					///< 1 byte
	BS2_BOOL useDynamicROI;										///< 1 byte
	BS2_FACE_CHECK_ORDER faceCheckOrder;				///< 1 byte
} BS2FaceConfigExt;

#endif
