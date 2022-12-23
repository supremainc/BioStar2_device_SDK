#ifndef __BS2_BARCODE_CONFIG_H__
#define __BS2_BARCODE_CONFIG_H__

#include "BS2Types.h"

/**
 *	Barcode timeout (QR sensor)
 */
enum {
	BS2_BARCODE_TIMEOUT_DEFAULT = 4,
    BS2_BARCODE_TIMEOUT_MIN     = BS2_BARCODE_TIMEOUT_DEFAULT,
    BS2_BARCODE_TIMEOUT_MAX     = 10,
};

typedef uint8_t					BS2_BARCODE_TIMEOUT;

/**
 *	Motion sensitivity
 */
enum {
    BS2_MOTION_SENSITIVITY_LOW      = 0,
    BS2_MOTION_SENSITIVITY_NORMAL   = 1,
    BS2_MOTION_SENSITIVITY_HIGH     = 2,
};

typedef uint8_t					BS2_MOTION_SENSITIVITY;

/**
 *	Barcode timeout (Visual camera)
 */
enum {
	BS2_VISUAL_BARCODE_TIMEOUT_DEFAULT = 10,
    BS2_VISUAL_BARCODE_TIMEOUT_MIN     = 3,
    BS2_VISUAL_BARCODE_TIMEOUT_MAX     = 20,
};

typedef uint8_t					BS2_VISUAL_BARCODE_TIMEOUT;

/**
 *	BS2BarcodeConfig
 */
typedef struct {
	BS2_BOOL 				    useBarcode;
    BS2_BARCODE_TIMEOUT 	    scanTimeout;
    BS2_BOOL 				    bypassData;
    BS2_BOOL 				    treatAsCSN;

	BS2_BOOL 				    useVisualBarcode;
	BS2_MOTION_SENSITIVITY 	    motionSensitivity;
    BS2_VISUAL_BARCODE_TIMEOUT  visualCameraScanTimeout;
    uint8_t 				    reserved[9];
} BS2BarcodeConfig;

#endif
