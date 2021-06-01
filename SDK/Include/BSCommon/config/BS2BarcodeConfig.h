#ifndef __BS2_BARCODE_CONFIG_H__
#define __BS2_BARCODE_CONFIG_H__

#include "BS2Types.h"

enum {
	BS2_BARCODE_TIMEOUT_DEFAULT = 4,
    BS2_BARCODE_TIMEOUT_MIN     = BS2_BARCODE_TIMEOUT_DEFAULT,
    BS2_BARCODE_TIMEOUT_MAX     = 10,
};

typedef struct {
	BS2_BOOL useBarcode;
    uint8_t scanTimeout;
    uint8_t reserved[14];
} BS2BarcodeConfig;

#endif
