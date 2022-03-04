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
    BS2_BOOL bypassData;		// BDP-488 1.2
    BS2_BOOL treatAsCSN;		// BDP-488 1.1

    uint8_t reserved[12];
} BS2BarcodeConfig;

#endif
