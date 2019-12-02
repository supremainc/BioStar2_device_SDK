/**
 *  Device Zone
 *
 *  @author mkkim@suprema.co.kr
 *  @see
 */

/*
 *  Copyright (c) 2014 Suprema Co., Ltd. All Rights Reserved.
 *
 *  This software is the confidential and proprietary information of
 *  Suprema Co., Ltd. ("Confidential Information").  You shall not
 *  disclose such Confidential Information and shall use it only in
 *  accordance with the terms of the license agreement you entered into
 *  with Suprema.
 */

#ifndef __BS2_DEVICE_ZONE_H__
#define __BS2_DEVICE_ZONE_H__


#include <string.h>
#include "../BS2Types.h"
#include "../data/BS2Action.h"
#include "../data/BS2Zone.h"

/**
 *  Constants
 */
enum {
	BS2_MAX_DEVICE_ZONE = 8,
};

/**
 *  BS2_DEVICE_ZONE_NODE_TYPE
 */
enum {
	BS2_DEVICE_ZONE_NODE_TYPE_MASTER		= 0x01,
	BS2_DEVICE_ZONE_NODE_TYPE_MEMBER		= 0x02,
};
typedef uint8_t BS2_DEVICE_ZONE_NODE_TYPE;

/**
 *  BS2_DEVICE_ZONE_TYPE
 */
enum {
	BS2_DEVICE_ZONE_TYPE_ENTRANCE_LIMIT		= 0x03,
	BS2_DEVICE_ZONE_TYPE_FIRE_ALARM			= 0x05,
};
typedef uint8_t BS2_DEVICE_ZONE_TYPE;

typedef struct {
	BS2_ZONE_ID zoneID;
	uint32_t nodeType;
} BS2_DEVICE_ZONE_TABLE_ID;

#define ALL_DEVICE_ID						(0xffffffff)
#define ALL_DOOR_ID							(0xffffffff)
#define ALL_ID_NODE_TYPE					(0xffffffff)

//#########################################################################
///////////////// Device Zone Entrance Limit /////////////////////////////
/**
 *  Constants
 */
enum {
	BS2_MAX_READERS_PER_DEVICE_ZONE_ENTRANCE_LIMIT		= 64,
	BS2_MAX_BYPASS_GROUPS_PER_DEVICE_ZONE_ENTRANCE_LIMIT	= 16,
	BS2_MAX_DEVICE_ZONE_ENTRANCE_LIMIT_ALARM_ACTION			= 5,
	BS2_MAX_ENTRANCE_LIMIT_PER_ZONE				= 24,
	BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT = 16,
	BS2_ENTRY_COUNT_FOR_ACCESS_GROUP_ENTRANCE_LIMIT = -2,
	BS2_OTHERWISE_ACCESS_GROUP_ID = -1,
	BS2_ENTRY_COUNT_NO_LIMIT = -1,
};

/**
 *  BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE
 */
enum {
	BS2_DEVICE_ZONE_ENTRANCE_LIMIT_SOFT		= 0x01,
	BS2_DEVICE_ZONE_ENTRANCE_LIMIT_HARD		= 0x02,
};
typedef uint8_t BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE;

/**
 *  BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_TYPE
 */
enum {
	BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_SOFT		= 0x01,
	BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_HARD		= 0x02,
};
typedef uint8_t BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_TYPE;


typedef struct {
	BS2_DEVICE_ID readerID;			///< 4 bytes
} BS2DeviceZoneEntranceLimitMemberInfo;

/**
 *  BS2DeviceZoneEntranceLimitMaster
 */

typedef struct {
	char name[BS2_MAX_ZONE_NAME_LEN];																///< 48 * 3 bytes

	BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE type;														///< 1 byte
	uint8_t reserved1[3];																			///< 3 bytes (packing)

	uint32_t entryLimitInterval_s;																	///< 4 bytes: in seconds, 0: no limit

	uint8_t numEntranceLimit; 																		///< 1 byte
	uint8_t numReaders;																				///< 1 byte
	uint8_t numAlarm;																				///< 1 byte
	uint8_t numBypassGroups;																		///< 1 byte

	uint8_t maxEntry[BS2_MAX_ENTRANCE_LIMIT_PER_ZONE]; 												///< 1 * 24 bytes // 0 (always limit)
	uint32_t periodStart_s[BS2_MAX_ENTRANCE_LIMIT_PER_ZONE];										///< 4 * 24 bytes: in seconds,
	uint32_t periodEnd_s[BS2_MAX_ENTRANCE_LIMIT_PER_ZONE];											///< 4 * 24 bytes: in seconds,

	BS2DeviceZoneEntranceLimitMemberInfo readers[BS2_MAX_READERS_PER_DEVICE_ZONE_ENTRANCE_LIMIT];	///< 4 * 64 bytes
	BS2Action alarm[BS2_MAX_DEVICE_ZONE_ENTRANCE_LIMIT_ALARM_ACTION];								///< 32 * 5 bytes
	BS2_ACCESS_GROUP_ID bypassGroupIDs[BS2_MAX_BYPASS_GROUPS_PER_DEVICE_ZONE_ENTRANCE_LIMIT];		///< 4 * 16 bytes

	uint8_t reserved3[8 * 4];																		///< 8 * 4 bytes
} BS2DeviceZoneEntranceLimitMaster;																	///884 bytes

typedef struct {
	BS2_ZONE_ID zoneID;
	uint16_t numAGEntranceLimit;
	uint16_t reserved1;	
	uint32_t periodStart_s[BS2_MAX_ENTRANCE_LIMIT_PER_ZONE];
	uint32_t periodEnd_s[BS2_MAX_ENTRANCE_LIMIT_PER_ZONE];
	uint16_t numEntry[BS2_MAX_ENTRANCE_LIMIT_PER_ZONE];	
	uint16_t maxEntry[BS2_MAX_ENTRANCE_LIMIT_PER_ZONE][BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT];	
	BS2_ACCESS_GROUP_ID accessGroupID[BS2_MAX_ENTRANCE_LIMIT_PER_ZONE][BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT];	
} BS2DeviceZoneAGEntranceLimit;

/**
 *  BS2DeviceZoneEntranceLimitMember
 */
typedef struct {
	BS2_PORT masterPort;																		///< 2 bytes	
	BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_TYPE actionInDisconnect;					///< 1 byte
	uint8_t reserved1[1];																		///< 1 byte (packing)	
	BS2_IPV4_ADDR masterIP;																		///< 16 bytes
} BS2DeviceZoneEntranceLimitMember;																///20 bytes
//#########################################################################

//#########################################################################
///////////////// Device Zone Fire Alarm /////////////////////////////
/**
 *  Constants
 */
enum {
	BS2_MAX_READERS_PER_DEVICE_ZONE_FIRE_ALARM				= 64,
	BS2_MAX_DEVICE_ZONE_FIRE_ALARM_ALARM_ACTION				= 5,
	BS2_MAX_FIRE_SENSORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER 	= 8,
	BS2_MAX_DOORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER 		= 8,
};

/**
 *  BS2_DEVICE_ZONE_ALARMED_STATUS_TYPE
 */
enum {
	BS2_DEVICE_ZONE_ALARMED_DISALARM			= 0x00, //Disalarmed 상태
	BS2_DEVICE_ZONE_ALARMED_ALARM 				= 0x01, //Alarm의 경우, Master가 Fire Alarm Alarmed/Disalarmed 상태를 전달해 온 상태이다. Self 후 이 상태가 되면 정보를 Master에게 보내지 않아도 된다.
	BS2_DEVICE_ZONE_ALARMED_SELF				= 0x02, //Self인 경우, 자신은 Alarmed 상태이지만 Master에게 Fire Alarm Detected 정보가 아직 전달되지 않은 상태이다. 이 경우 주기적으로 정보를 Master에게 보내야 한다.
};
typedef uint8_t BS2_DEVICE_ZONE_ALARMED_STATUS_TYPE;

typedef struct {
	BS2_DEVICE_ID		deviceID;			///< 4 bytes
	uint8_t				port;				///< 1 byte
	BS2_SWITCH_TYPE		switchType;			///< 1 byte
	uint16_t			duration;			///< 2 bytes
} BS2DeviceZoneFireSensor;

typedef struct {
	BS2_DEVICE_ID 		readerID;			///< 4 bytes
} BS2DeviceZoneFireAlarmMemberInfo;

/**
 *  BS2DeviceZoneFireAlaramMaster
 */

typedef struct {
	char name[BS2_MAX_ZONE_NAME_LEN];																///< 48 * 3 bytes

	uint8_t numReaders;																				///< 1 byte
	uint8_t numAlarm;																				///< 1 byte	
	uint8_t reserved1[2];																			///< 2 byte (packing)

	BS2DeviceZoneFireAlarmMemberInfo readers[BS2_MAX_READERS_PER_DEVICE_ZONE_FIRE_ALARM];			///< 4 * 64 bytes
	BS2Action alarm[BS2_MAX_DEVICE_ZONE_FIRE_ALARM_ALARM_ACTION];									///< 32 * 5 bytes

	uint8_t reserved2[8 * 40];																		///< 8 * 40 bytes
} BS2DeviceZoneFireAlarmMaster;																		///884 bytes

/**
 *  BS2DeviceZoneEntranceLimitMember
 */
typedef struct {
	BS2_PORT masterPort;																			///< 2 bytes
	uint8_t reserved1[2];																			///< 2 byte (packing)
	BS2_IPV4_ADDR masterIP;																			///< 16 bytes
	
	uint8_t numSensors;																				///< 1 byte
	uint8_t numDoors;																				///< 1 byte	
	uint8_t reserved2[2];																			///< 2 byte (packing)
	BS2DeviceZoneFireSensor	sensor[BS2_MAX_FIRE_SENSORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER];			///< 8 * 8 bytes
	union {
		BS2_DOOR_ID	doorIDs[BS2_MAX_DOORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER];								///< 4 * 8 bytes
		BS2_LIFT_ID	liftIDs[BS2_MAX_DOORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER];								
	};
	uint8_t reserved3[40];																			///< 40 byte (packing)
} BS2DeviceZoneFireAlarmMember;																		/// 160 bytes
//#########################################################################

typedef struct {
    BS2_ZONE_ID zoneID;						///< 4 bytes
    BS2_DEVICE_ZONE_TYPE zoneType;			///< 1 bytes
    BS2_DEVICE_ZONE_NODE_TYPE nodeType;		///< 1 bytes
	BS2_BOOL enable;						///< 1 byte	
	uint8_t reserved[1];					///< 1 bytes (packing)
	union {
		BS2DeviceZoneEntranceLimitMaster entranceLimitMaster;	//<<<884 bytes
		BS2DeviceZoneEntranceLimitMember entranceLimitMember;   //<<<20 bytes
		BS2DeviceZoneFireAlarmMaster fireAlarmMaster;			//<<<884 bytes
		BS2DeviceZoneFireAlarmMember fireAlarmMember;   		//<<<160 bytes
	};
} BS2DeviceZone;							//892 bytes

typedef struct {
    int numOfZones; //0 ~ BS_MAX_ZONE_PER_NODE	///< 4 bytes
    BS2DeviceZone zone[BS2_MAX_DEVICE_ZONE];	/// 892 * 4 bytes
} BS2DeviceZoneConfig;	//2572 bytes



#endif	// __BS2_DEVICE_ZONE_H__