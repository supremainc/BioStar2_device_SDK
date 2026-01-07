
//SUPSDK-94 (BDP-953)
#ifndef __BS2_USER_OVERRIDE_H__
#define __BS2_USER_OVERRIDE_H__

#include "../BS2Types.h"

typedef struct{
	BS2_USER_ID userID;                     ///< 32 bytes
	BS2_BOOL useExtendedAutoLockTimeout;	///<  1 byte			//BDP-953
	uint8_t reserved[11];					///< 11 bytes
}  BS2UserOverride;


#endif //__BS2_USER_OVERIDE_H__
