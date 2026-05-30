
#ifndef _AUTH_FAIL_STATUS_H_
#define _AUTH_FAIL_STATUS_H_

#include "BS2Types.h"
#include "config/BS2AuthConfigExt.h"


//COUNT_OF_AUTH_FAIL  3~15 (5)
//TIME_WINDOW_OF_AUTH_FAIL 30~300s (60s)
//ACCUMULATED_LOCKOUT_TIME 

enum{
    MAX_AUTH_FAIL_STATUS_DETAIL_COUNT = 5, 
};

enum{
    BS2_LOCKOUT_LEVEL_1  = 1, 
    BS2_LOCKOUT_LEVEL_2 = 2, 

	MAX_LOCKOUT_LEVEL = BS2_LOCKOUT_LEVEL_2, 
};

typedef uint8_t BS2_LOCKOUT_LEVEL; 

enum{
    BS2_LOCKOUT_TYPE_NONE = 0, 
    BS2_LOCKOUT_TYPE_AUTH = 0x01, 
    BS2_LOCKOUT_TYPE_OTHERS = 0x02, 
};

typedef uint16_t BS2_LOCKOUT_TYPE; 

typedef struct{

    BS2_LOCKOUT_TYPE lockoutType; 
    uint16_t remaining_count; 

    union{
        BS2_DATETIME startTime;			///< External(case when protocol)
        BS2_TICK    startTick;          ///< internal
    };
    uint32_t numOfOccurred; 

    union{
        BS2_DATETIME whenAuthFailOccurred[MAX_COUNT_OF_AUTH_FAIL+1];			///< External(case when protocol)
        BS2_TICK whenAuthFailOccurredTick[MAX_COUNT_OF_AUTH_FAIL+1];            ///< internal
    };

    union{
        BS2_DATETIME lastLockoutTime;
        BS2_TICK lastLockoutTick;
    };

    uint8_t reserved[32];
}  BS2AuthFailStatusDetail; 

typedef struct{
    BS2_DEVICE_ID deviceID;
    
    BS2_BOOL  isLockouted;
    BS2_LOCKOUT_LEVEL lockoutLevel;  
    uint8_t numOfDetails; 

    BS2AuthFailStatusDetail details[MAX_AUTH_FAIL_STATUS_DETAIL_COUNT ];
    

} BS2AuthFailStatus;

#endif