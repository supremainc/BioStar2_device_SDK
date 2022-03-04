/*
 * BS_API.h
 *
 *  Created on: 2015. 4. 10.
 *      Author: scpark
 */

#ifndef CORE_SRC_BS_API_H_
#define CORE_SRC_BS_API_H_

#include "BSCommon/BS2Types.h"
#include "BSCommon/config/BS2FactoryConfig.h"
#include "BSCommon/config/BS2SystemConfig.h"
#include "BSCommon/config/BS2AuthConfig.h"
#include "BSCommon/config/BS2StatusConfig.h"
#include "BSCommon/config/BS2DisplayConfig.h"
#include "BSCommon/config/BS2IpConfig.h"
#include "BSCommon/config/BS2IPV6Config.h" //[IPv6 Cfg] 
#include "BSCommon/config/BS2IpConfigExt.h"
#include "BSCommon/config/BS2TnaExtConfig.h"
#include "BSCommon/config/BS2CardConfig.h"
#include "BSCommon/config/BS2FingerprintConfig.h"
#include "BSCommon/config/BS2Rs485Config.h"
#include "BSCommon/config/BS2WiegandConfig.h"
#include "BSCommon/config/BS2WiegandDeviceConfig.h"
#include "BSCommon/config/BS2InputConfig.h"
#include "BSCommon/config/BS2WlanConfig.h"
#include "BSCommon/config/BS2TriggerActionConfig.h"
#include "BSCommon/config/BS2EventConfig.h"
#include "BSCommon/config/BS2WiegandMultiConfig.h"
#include "BSCommon/config/BS1CardConfig.h"
#include "BSCommon/config/BS2SystemConfigExt.h"
#include "BSCommon/config/BS2VoipConfig.h"
#include "BSCommon/config/BS2FaceConfig.h"
#include "BSCommon/config/BS2DstConfig.h"
#include "BSCommon/config/BS2ConfigHeader.h"
#include "BSCommon/config/BS2ConfigHeaderEx.h"
#include "BSCommon/config/BS2DeviceZoneMasterConfig.h"
#include "BSCommon/config/BS2DeviceZoneConfig.h"
#include "BSCommon/data/BS2AccessGroup.h"
#include "BSCommon/data/BS2AccessLevel.h"
#include "BSCommon/data/BS2FloorLevel.h"
#include "BSCommon/data/BS2Action.h"
#include "BSCommon/data/BS2AntiPassbackZone.h"
#include "BSCommon/data/BS2BlackList.h"
#include "BSCommon/data/BS2CSNCard.h"
#include "BSCommon/data/BS2DaySchedule.h"
#include "BSCommon/data/BS2Device.h"
#include "BSCommon/data/BS2Door.h"
#include "BSCommon/data/BS2Lift.h"
#include "BSCommon/data/BS2Event.h"
#include "BSCommon/data/BS2EventExt.h"
#include "BSCommon/data/BS2Face.h"
#include "BSCommon/data/BS2FaceEx.h"
#include "BSCommon/data/BS2Fingerprint.h"
#include "BSCommon/data/BS2FireAlarmZone.h"
#include "BSCommon/data/BS2ScheduledLockUnlockZone.h"
#include "BSCommon/data/BS2Holiday.h"
#include "BSCommon/data/BS2Resource.h"
#include "BSCommon/data/BS2Rs485Channel.h"
#include "BSCommon/data/BS2Rs485SlaveDeviceSetting.h"
#include "BSCommon/data/BS2Schedule.h"
#include "BSCommon/data/BS2SmartCard.h"
#include "BSCommon/data/BS2TimedAntiPassbackZone.h"
#include "BSCommon/data/BS2Trigger.h"
#include "BSCommon/data/BS2User.h"
#include "BSCommon/data/BS2Zone.h"
#include "BSCommon/data/BS2AuthGroup.h"
#include "BSCommon/data/BS2IntrusionAlarmZone.h"
#include "BSCommon/data/BS2InterlockZone.h"
#include "BSCommon/data/BS2LiftLockUnlockZone.h"
#include "BSCommon/data/BS2Operator.h" //[Admin 1000] 
#include "BSCommon/protocol/BS2UdpDiscover.h"
#include "BSCommon/protocol/BS2SystemInfo.h"
#include "BSCommon/config/BS2AuthConfigExt.h"
#include "BSCommon/config/BS2FaceConfigExt.h"
#include "BSCommon/config/BS2ThermalCameraConfig.h"
#include "BSCommon/config/BS2BarcodeConfig.h"
#include "BSCommon/protocol/BS2DeviceInfo.h"
#include "BSCommon/config/BS2RelayActionConfig.h"
#include "BS_Deprecated.h"

#ifdef BS_SDK_V2_DLL
#define BS_API_EXPORT __declspec(dllimport)
#define BS_CALLING_CONVENTION __cdecl
#else
#define BS_API_EXPORT
#define BS_CALLING_CONVENTION
#endif


#ifndef BS2_MAX_NUM_OF_CARD_PER_USER
#define BS2_MAX_NUM_OF_CARD_PER_USER                8
#endif

#ifndef BS2_MAX_NUM_OF_FINGER_PER_USER
#define BS2_MAX_NUM_OF_FINGER_PER_USER              10
#endif

#ifndef BS2_MAX_NUM_OF_FACE_PER_USER
#define BS2_MAX_NUM_OF_FACE_PER_USER                5
#endif

#ifndef BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER
#define BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER        16
#endif

#define BS2_SSL_CONNECT_TIMEOUT						(20*1000)

#define DEFAULT_RESPONSE_TIMEOUT_MS					(10*1000) //[Set/Get default response wait timeout] 

#define All_nodes_in_the_interface_local	"FF01::1" //All node multicast (in the interface local)
#define All_nodes_in_the_link_local			"FF02::1" //All node multicast (in the link local)
#define All_routers_in_the_interface_local	"FF01::2" //All router multicast (in the interface local)
#define All_routers_in_the_link_local		"FF02::2" //All router multicast (in the link local)
#define All_routers_in_the_site_local		"FF05::2" //All router multicast (in the site local)
#define OSPFIGP_routers						"FF02::5" //OSPF router (link-local)
#define OSPFIGP_designated_routers			"FF02::6" //OSPF DR/BDR (link-local)
#define RIP_routers							"FF02::9" //RIPng router (link-local)
#define EIGRP_routers						"FF02::A" //EIGRP router (link-local)
#define Mobile_agents						"FF02::B" //Mobile agents (link-local)
#define All_PIM_routers						"FF02::D" //PIM router (link-local)
#define All_MLDv2_capable_routers			"FF02::16" //MLDv2 capable router (link-local)
#define All_dhcp_agents						"FF02::1:2" //DHCPv6 agents (link-local)
#define All_dhcp_servers					"FF05::1:3" //DHCPv6 servers (site-local)
//#define 									"FF02:1:FF::" //Solicited node multicast (link-local)

#define DEFAULT_MULTICAST_IPV6_ADDRESS 		All_routers_in_the_site_local
#define DEFAULT_BROADCAST_IPV4_ADDRESS 		"255.255.255.255"

#pragma pack(1)

typedef struct
{
	BS2User user;
	BS2UserSetting setting;
	BS2_USER_NAME user_name;
	BS2UserPhoto user_photo;
	BS2_USER_PIN pin;
	BS2CSNCard* cardObjs;
	BS2Fingerprint* fingerObjs;
	BS2Face* faceObjs;
	BS2_ACCESS_GROUP_ID accessGroupId[BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER];
}BS2UserBlob;

typedef struct
{
	BS2User user;
	BS2UserSetting setting;
	BS2_USER_NAME user_name;
	BS2UserPhoto user_photo;
	BS2_USER_PIN pin;
	BS2CSNCard* cardObjs;
	BS2Fingerprint* fingerObjs;
	BS2Face* faceObjs;
	BS2Job job;
	BS2_USER_PHRASE phrase;
	BS2_ACCESS_GROUP_ID accessGroupId[BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER];
}BS2UserBlobEx;

typedef struct
{
	uint16_t eventMask;
	BS2_EVENT_ID id;
	BS2EventExtInfo info;                         // valid if eventMask has BS2_EVENT_MASK_INFO
	union
	{
		BS2_USER_ID userID;                       // valid if eventMask has BS2_EVENT_MASK_USER_ID
		uint8_t cardID[BS2_CARD_DATA_SIZE];       // valid if eventMask has BS2_EVENT_MASK_CARD_ID
		BS2_DOOR_ID doorID;                       // valid if eventMask has BS2_EVENT_MASK_DOOR_ID
		BS2_ZONE_ID zoneID;                       // valid if eventMask has BS2_EVENT_MASK_ZONE_ID
		BS2EventExtIoDevice ioDevice;             // valid if eventMask has BS2_EVENT_MASK_IODEVICE
	};
	BS2_TNA_KEY tnaKey;                           // valid if eventMask has BS2_EVENT_MASK_TNA_KEY
	BS2_JOB_CODE jobCode;                         // valid if eventMask has BS2_EVENT_MASK_JOB_CODE
	uint16_t imageSize;                           // valid if eventMask has BS2_EVENT_MASK_IMAGE
	uint8_t image[BS2_EVENT_MAX_IMAGE_SIZE];      // valid if eventMask has BS2_EVENT_MASK_IMAGE
	uint8_t reserved;
}BS2EventBlob;

typedef struct
{
	BS2_DEVICE_ID id;
	BS2_DEVICE_TYPE type;
	BS2_CONNECTION_MODE connectionMode;
    uint32_t ipv4Address;
    BS2_PORT port;
    uint32_t maxNumOfUser;
	uint8_t userNameSupported;
	uint8_t userPhotoSupported;
	uint8_t pinSupported;
	uint8_t cardSupported;
	uint8_t fingerSupported;
	uint8_t faceSupported;
	uint8_t wlanSupported;
	uint8_t tnaSupported;
	uint8_t triggerActionSupported;
	uint8_t wiegandSupported;
	uint8_t imageLogSupported;
	uint8_t dnsSupported;
	uint8_t jobCodeSupported;
	uint8_t wiegandMultiSupported;
	BS2_RS485_MODE rs485Mode;
	uint8_t sslSupported;
	uint8_t rootCertExist;
	uint8_t dualIDSupported;
	uint8_t useAlphanumericID;
	uint32_t connectedIP;
	uint8_t phraseSupported;
	uint8_t card1xSupported;
	uint8_t systemExtSupported;
	uint8_t voipSupported;	
	uint8_t rs485ExSupported;
	uint8_t	cardExSupported;		
}BS2SimpleDeviceInfo;

typedef struct
{
	enum
	{
		BS2_SUPPORT_RS485EX 		= 0x00000001,
		BS2_SUPPORT_CARDEX  		= 0x00000002,
		BS2_SUPPORT_DST     		= 0x00000004,
		BS2_SUPPORT_DESFIREEX		= 0x00000008,
		
		BS2_SUPPORT_FACE_EX			= 0x00000010,
		BS2_SUPPORT_QR				= 0x00000020,

		BS2_SUPPORT_FINGER_SCAN		= 0x00010000,
		BS2_SUPPORT_FACE_SCAN		= 0x00020000,
		BS2_SUPPORT_FACE_EX_SCAN	= 0x00040000,
		BS2_SUPPORT_QR_SCAN			= 0x00080000,

		BS2_SUPPORT_ALL     		= BS2_SUPPORT_RS485EX |
									  BS2_SUPPORT_CARDEX |
									  BS2_SUPPORT_DST |
									  BS2_SUPPORT_DESFIREEX |
									  BS2_SUPPORT_FACE_EX |
									  BS2_SUPPORT_QR |
									  BS2_SUPPORT_FINGER_SCAN |
									  BS2_SUPPORT_FACE_SCAN |
									  BS2_SUPPORT_FACE_EX_SCAN |
									  BS2_SUPPORT_QR_SCAN,
	};

	uint32_t supported;
	uint8_t  reserved[4];
}BS2SimpleDeviceInfoEx;

typedef struct
{
	uint32_t configMask;
	BS2FactoryConfig factoryConfig;
	BS2SystemConfig systemConfig;
	BS2AuthConfig authConfig;
	BS2StatusConfig statusConfig;
	BS2DisplayConfig displayConfig;
	BS2IpConfig ipConfig;
	BS2IpConfigExt ipConfigExt;
	BS2TNAConfig tnaConfig;
	BS2CardConfig cardConfig;
	BS2FingerprintConfig fingerprintConfig;
	BS2Rs485Config rs485Config;
	BS2WiegandConfig wiegandConfig;
	BS2WiegandDeviceConfig wiegandDeviceConfig;
	BS2InputConfig inputConfig;
	BS2WlanConfig wlanConfig;
	BS2TriggerActionConfig triggerActionConfig;
	BS2EventConfig eventConfig;
	BS2WiegandMultiConfig wiegandMultiConfig;
	BS1CardConfig card1xConfig;
	BS2SystemConfigExt systemExtConfig;
	BS2VoipConfig voipConfig;
	BS2FaceConfig faceConfig;
}BS2Configs;

typedef struct
{
	BS2_RESOURCE_TYPE type;
	uint32_t numResData;
	struct {
		uint8_t index;
		uint32_t dataLen;
		uint8_t* data;
	}resData[128];
}BS2ResourceElement;

typedef struct
{
	uint8_t isSmartCard;
	union
	{
		BS2CSNCard card;
		BS2SmartCardData smartCard;
	};
}BS2Card;

typedef struct
{
	BS2_DEVICE_ID parentDeviceID;
	BS2_DEVICE_ID deviceID;
	BS2_DEVICE_TYPE deviceType;
}BS2DeviceNode;

typedef struct
{
	int numSensors;  
	BS2FireSensor sensor[BS2_MAX_FIRE_SENSORS_PER_FIRE_ALARM_ZONE];
	BS2Action action[BS2_MAX_FIRE_ALARM_ACTION];
}BS2FireAlarmZoneProperty;

typedef struct
{
	BS2Action action[BS2_MAX_APB_ALARM_ACTION];
}BS2APBZoneProperty;

typedef struct
{
	int zoneId;
	int zoneType;
	union
	{
		BS2APBZoneProperty apbProperty;
		BS2FireAlarmZoneProperty fireAlarmProperty;
	};
}BS2ZoneEx;

/**
 *  BS2AntiPassbackZone
 */
typedef struct {
	BS2_ZONE_ID zoneID;					///< 4 bytes
	char name[BS2_MAX_ZONE_NAME_LEN];		///< 48 * 3 bytes

	BS2_APB_ZONE_TYPE type;				///< 1 byte
	uint8_t numReaders;					///< 1 byte
	uint8_t numBypassGroups;				///< 1 byte
	BS2_BOOL disabled;				///< 1 byte

	BS2_BOOL alarmed;				///< 1 byte
	uint8_t reserved[3];				///< 3 bytes (packing)

	uint32_t resetDuration;				///< 4 bytes: in seconds, 0: no reset

	BS2Action alarm[BS2_MAX_APB_ALARM_ACTION];		///< 32 * 5 bytes

	BS2ApbMember readers[BS2_MAX_READERS_PER_APB_ZONE];		///< 8 * 64 bytes
	uint8_t reserved2[8 * 64];		///< 8 * 64 bytes (packing)

	BS2_ACCESS_GROUP_ID bypassGroupIDs[BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE];		///< 4 * 16 bytes
} BS2APBZoneConfig;

typedef struct
{
	uint32_t authGroupID;
	uint32_t numUsers;
	BS2_USER_ID* userObjs;
}BS2AuthUserBlob;

typedef struct
{
	BS2IntrusionAlarmZone IntrusionAlarmZone;
	BS2AlarmZoneMember* memberObjs;
	BS2AlarmZoneInput* inputObjs;
	BS2AlarmZoneOutput* outputObjs;
	BS2CSNCard* cardObjs;
	BS2_DOOR_ID* doorIDs;
	BS2_ACCESS_GROUP_ID* groupIDs;
}BS2IntrusionAlarmZoneBlob;

typedef struct
{
	BS2InterlockZone InterlockZone;
	BS2InterlockZoneInput* inputObjs;
	BS2InterlockZoneOutput* outputObjs;
	BS2_DOOR_ID* doorIDs;
}BS2InterlockZoneBlob;


enum
{
	BS2_ENC_KEY_SIZE      = 32,
};

/**
 * BS2EncryptKey
 */
typedef struct
{
	uint8_t key[BS2_ENC_KEY_SIZE];
	uint8_t reserved[32];
} BS2EncryptKey;

//[IPv6]
typedef struct
{
	BS2_DEVICE_ID id;
	uint8_t reserved[1];
	uint8_t bIPv6Mode;
	BS2_IPV6_ADDR ipv6Address;
    BS2_PORT portV6;	
	BS2_IPV6_ADDR connectedIPV6;	
	uint8_t numOfAllocatedAddressV6;
	BS2_IPV6_ADDR allocatedIpAddressV6[BS2_MAX_IPV6_ALLOCATED_ADDR]; 
}BS2IPv6DeviceInfo;

typedef enum 
{
	BS2_SPECIFIED_DEVICE_INFO_SIMPLE 		= 1,
	BS2_SPECIFIED_DEVICE_INFO_SIMPLE_EX 	= 2,
	BS2_SPECIFIED_DEVICE_INFO_IPV6			= 3,
}BS2EnumSpecifiedDeviceInfo;


//User Small Blob
typedef struct
{
	BS2User user;
	BS2UserSetting setting;
	BS2_USER_NAME user_name;
	BS2UserPhoto* user_photo_obj;
	BS2_USER_PIN pin;
	BS2CSNCard* cardObjs;
	BS2Fingerprint* fingerObjs;
	BS2Face* faceObjs;
	BS2_ACCESS_GROUP_ID accessGroupId[BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER];
}BS2UserSmallBlob;

typedef struct
{
	BS2User user;
	BS2UserSetting setting;
	BS2_USER_NAME user_name;
	BS2UserPhoto* user_photo_obj;
	BS2_USER_PIN pin;
	BS2CSNCard* cardObjs;
	BS2Fingerprint* fingerObjs;
	BS2Face* faceObjs;
	BS2Job job;
	BS2_USER_PHRASE phrase;
	BS2_ACCESS_GROUP_ID accessGroupId[BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER];
}BS2UserSmallBlobEx;

typedef struct
{
	BS2User user;
	BS2UserSetting setting;
	BS2_USER_NAME user_name;
	BS2UserPhoto* user_photo_obj;
	BS2_USER_PIN pin;
	BS2CSNCard* cardObjs;
	BS2Fingerprint* fingerObjs;
	BS2Face* faceObjs;						// FS2, FL
	BS2Job job;
	BS2_USER_PHRASE phrase;
	BS2_ACCESS_GROUP_ID accessGroupId[BS2_MAX_NUM_OF_ACCESS_GROUP_PER_USER];

	BS2UserSettingEx settingEx;				// F2
	BS2FaceEx* faceExObjs;					// F2
} BS2UserFaceExBlob;


typedef struct
{
	uint16_t eventMask;
	BS2_EVENT_ID id;
	BS2EventExtInfo info;                         // valid if eventMask has BS2_EVENT_MASK_INFO
	union
	{
		BS2_USER_ID userID;                       // valid if eventMask has BS2_EVENT_MASK_USER_ID
		uint8_t cardID[BS2_CARD_DATA_SIZE];       // valid if eventMask has BS2_EVENT_MASK_CARD_ID
		BS2_DOOR_ID doorID;                       // valid if eventMask has BS2_EVENT_MASK_DOOR_ID
		BS2_ZONE_ID zoneID;                       // valid if eventMask has BS2_EVENT_MASK_ZONE_ID
		BS2EventExtIoDevice ioDevice;             // valid if eventMask has BS2_EVENT_MASK_IODEVICE
	};
	BS2_TNA_KEY tnaKey;                           // valid if eventMask has BS2_EVENT_MASK_TNA_KEY
	BS2_JOB_CODE jobCode;                         // valid if eventMask has BS2_EVENT_MASK_JOB_CODE
	uint16_t imageSize;                           // valid if eventMask has BS2_EVENT_MASK_IMAGE
	uint8_t* imageObj;                            // valid if eventMask has BS2_EVENT_MASK_IMAGE
	uint8_t reserved;
}BS2EventSmallBlob;

typedef struct
{
	uint16_t eventMask;
	BS2_EVENT_ID id;
	BS2EventExtInfo info;                         // valid if eventMask has BS2_EVENT_MASK_INFO
	union
	{
		BS2_USER_ID userID;                       // valid if eventMask has BS2_EVENT_MASK_USER_ID
		uint8_t cardID[BS2_CARD_DATA_SIZE];       // valid if eventMask has BS2_EVENT_MASK_CARD_ID
		BS2_DOOR_ID doorID;                       // valid if eventMask has BS2_EVENT_MASK_DOOR_ID
		BS2_ZONE_ID zoneID;                       // valid if eventMask has BS2_EVENT_MASK_ZONE_ID
		BS2EventExtIoDevice ioDevice;             // valid if eventMask has BS2_EVENT_MASK_IODEVICE
	};
	BS2_TNA_KEY tnaKey;                           // valid if eventMask has BS2_EVENT_MASK_TNA_KEY
	BS2_JOB_CODE jobCode;                         // valid if eventMask has BS2_EVENT_MASK_JOB_CODE
	uint16_t imageSize;                           // valid if eventMask has BS2_EVENT_MASK_IMAGE
	uint8_t* imageObj;                            // valid if eventMask has BS2_EVENT_MASK_IMAGE
	uint8_t reserved;
	uint32_t temperature;                         // valid if eventMask has BS2_EVENT_MASK_TEMPERATURE
} BS2EventSmallBlobEx;

typedef uint32_t BS2_CONFIG_MASK;

#pragma pack()

typedef void (*OnDeviceFound)(BS2_DEVICE_ID deviceId);
typedef void (*OnDeviceAccepted)(BS2_DEVICE_ID deviceId);
typedef void (*OnDeviceConnected)(BS2_DEVICE_ID deviceId);
typedef void (*OnDeviceDisconnected)(BS2_DEVICE_ID deviceId);
typedef void (*OnReadyToScan)(BS2_DEVICE_ID deviceId, uint32_t sequence);
typedef void (*OnProgressChanged)(BS2_DEVICE_ID deviceId, uint32_t progressPercentage);
typedef void (*OnLogReceived)(BS2_DEVICE_ID deviceId, const BS2Event* event);
typedef void (*OnLogReceivedEx)(BS2_DEVICE_ID deviceId, const BS2Event* event, BS2_TEMPERATURE temperature);
typedef void (*OnAlarmFired)(BS2_DEVICE_ID deviceId, const BS2Event* event);
typedef void (*OnInputDetected)(BS2_DEVICE_ID deviceId, const BS2Event* event);
typedef void (*OnBarcodeScanned)(BS2_DEVICE_ID deviceId, const char* barcode);
typedef void (*OnConfigChanged)(BS2_DEVICE_ID deviceId, uint32_t configMask);
typedef void (*OnVerifyUser)(BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, uint8_t isCard, uint8_t cardType, const uint8_t* data, uint32_t dataLen);
typedef void (*OnIdentifyUser)(BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, BS2_FINGER_TEMPLATE_FORMAT format, const uint8_t* templateData, uint32_t templateSize);
typedef int (*IsAcceptableUserID)(const char* uid);
typedef void(*OnSendRootCA)(BS2_DEVICE_ID deviceId, int result);
typedef void (*OnCheckGlobalAPBViolation)(BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, const char* userID_1, const char* userID_2, bool isDualAuth);
typedef void (*OnCheckGlobalAPBViolationByDoorOpen)(BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, const char* userID_1, const char* userID_2, bool isDualAuth);
typedef void (*OnUpdateGlobalAPBViolationByDoorOpen)(BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, const char* userID_1, const char* userID_2, bool isDualAuth);
typedef void (*OnUserPhrase)(BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, const char* userID);

typedef uint32_t (*PreferMethod)(BS2_DEVICE_ID deviceID);
typedef const char* (*GetRootCaFilePath)(BS2_DEVICE_ID deviceID);
typedef const char* (*GetServerCaFilePath)(BS2_DEVICE_ID deviceID);
typedef const char* (*GetServerPrivateKeyFilePath)(BS2_DEVICE_ID deviceID);
typedef const char* (*GetPassword)(BS2_DEVICE_ID deviceID);
typedef void (*OnErrorOccured)(BS2_DEVICE_ID deviceID, int errCode);


//Debug
//Debug Print
#define DEBUG_KEEP_ALIVE                (0x1 << 0)
#define DEBUG_SOCKET_MANAGER            (0x1 << 1)
#define DEBUG_SOCKETBASE_HANDLER        (0x1 << 2)
#define DEBUG_DEVICE                    (0x1 << 3)
#define DEBUG_DEVICE_MANAGER            (0x1 << 4)
#define DEBUG_EVENT_DISPATCHER          (0x1 << 5)
#define DEBUG_API                       (0x1 << 6)
#define DEBUG_ALL                       0xFFFFFFFF
typedef void (*CBDebugPrint)(char* msg);

//DebugEx Print
#define DEBUG_MODULE_KEEP_ALIVE			(DEBUG_KEEP_ALIVE)
#define DEBUG_MODULE_SOCKET_MANAGER		(DEBUG_SOCKET_MANAGER)
#define DEBUG_MODULE_SOCKET_HANDLER		(DEBUG_SOCKETBASE_HANDLER)
#define DEBUG_MODULE_DEVICE				(DEBUG_DEVICE)
#define DEBUG_MODULE_DEVICE_MANAGER		(DEBUG_DEVICE_MANAGER)
#define DEBUG_MODULE_EVENT_DISPATCHER	(DEBUG_EVENT_DISPATCHER)
#define DEBUG_MODULE_API				(DEBUG_API)
#define DEBUG_MODULE_MISC				(0x1 << 7)
#define DEBUG_MODULE_PACKET				(0x1 << 8)
#define DEBUG_MODULE_MOBILEACCESS		(0x1 << 9)
#define DEBUG_MODULE_NOTIFY_MANAGER		(0x1 << 10)
#define DEBUG_MODULE_EVENT				(0x1 << 11)
#define DEBUG_MODULE_USB				(0x1 << 12)
#define DEBUG_MODULE_ALL				(DEBUG_ALL)
#define DEBUG_LOG_FATAL					(0x1 << 0)
#define DEBUG_LOG_ERROR					(0x1 << 1)
#define DEBUG_LOG_WARN					(0x1 << 2)
#define DEBUG_LOG_API					(0x1 << 3)		// Print IN and OUT of API
#define DEBUG_LOG_INFO					(0x1 << 4)
#define DEBUG_LOG_TRACE					(0x1 << 8)		// Modified bit mask value (V2.6.3.12)
#define DEBUG_LOG_SYSTEM				(0x0000000F)	// DEBUG_LOG_FATAL | DEBUG_LOG_ERROR | DEBUG_LOG_WARN | DEBUG_LOG_API
#define DEBUG_LOG_OPERATION_ALL			(0x000000FF)	// Output support except trace logs (V2.6.3.12)
#define DEBUG_LOG_ALL					(0xFFFFFFFF)
typedef void (*CBDebugExPrint)(uint32_t level, uint32_t module, const char* msg);

#ifdef __cplusplus
extern "C"
{
#endif

//Debug
// - Replaced
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDebugFileLog(uint32_t level, uint32_t module, const char* logPath);
// - Deprecated
DEPRECATED_FUNC BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDebugLevel(CBDebugPrint ptrCBDebugPrint, uint32_t debugLevel);
DEPRECATED_FUNC BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDebugExCallback(CBDebugExPrint ptrCBDebugExPrint, uint32_t level, uint32_t module);

//SDK API
BS_API_EXPORT const char* BS_CALLING_CONVENTION BS2_Version();
BS_API_EXPORT void* BS_CALLING_CONVENTION BS2_AllocateContext();
BS_API_EXPORT void BS_CALLING_CONVENTION BS2_ReleaseContext(void* context);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_Initialize(void* context);
BS_API_EXPORT void BS_CALLING_CONVENTION BS2_ReleaseObject(void* object);

//Timeout
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceSearchingTimeout(void* context, uint32_t second);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSSLHandshakeTimeout(void* context, uint32_t* second);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSSLHandshakeTimeout(void* context, uint32_t second);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetMaxThreadCount(void* context, uint32_t maxThreadCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetMaxConnectThreadCount(void* context, uint32_t maxThreadCount);

//Communication API
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_IsAutoConnection(void* context, int* enable);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAutoConnection(void* context, int enable);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceEventListener(void* context,
                                            OnDeviceFound ptrDeviceFound,
											OnDeviceAccepted ptrDeviceAccepted,
                                            OnDeviceConnected ptrDeviceConnected,
                                            OnDeviceDisconnected ptrDeviceDisconnected);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetNotificationListener(void* context,
											OnAlarmFired ptrAlarmFired,
											OnInputDetected ptrInputDetected,
											OnConfigChanged ptrConfigChanged);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetBarcodeScanListener(void* context,
											OnBarcodeScanned ptrBarcodeScanned);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetServerPort(void* context, BS2_PORT serverPort);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SearchDevices(void* context);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SearchDevicesEx(void* context, const char* hostipAddr);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDevices(void* context, BS2_DEVICE_ID** deviceListObj, uint32_t* numDevice);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceInfo(void* context, BS2_DEVICE_ID deviceId, BS2SimpleDeviceInfo* deviceInfo);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceInfoEx(void* context, BS2_DEVICE_ID deviceId, BS2SimpleDeviceInfo* deviceInfo, BS2SimpleDeviceInfoEx* deviceInfoEx);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ConnectDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ConnectDeviceViaIP(void* context, const char* deviceAddress, BS2_PORT defaultDevicePort, BS2_DEVICE_ID* deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_DisconnectDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceTopology(void* context, BS2_DEVICE_ID deviceId, BS2DeviceNode** networkNodeObj, uint32_t* numNetworkNode);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceTopology(void* context, BS2_DEVICE_ID deviceId, BS2DeviceNode* networkNode, uint32_t numNetworkNode);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_IsConnected(void* context, BS2_DEVICE_ID deviceId, int* connected);

//SSL API
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSSLServerPort(void* context, BS2_PORT sslServerPort); 
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSSLHandler(void* context, PreferMethod ptrPreferMethod, GetRootCaFilePath ptrGetRootCaFilePath, GetServerCaFilePath ptrGetServerCaFilePath, GetServerPrivateKeyFilePath ptrGetServerPrivateKeyFilePath, GetPassword ptrGetPassword, OnErrorOccured ptrOnErrorOccured);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceSSLEventListener(void* context, OnSendRootCA ptrSendRootCA);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_DisableSSL(void* context, BS2_DEVICE_ID deviceId);

// AccessControl api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAccessGroup(void* context, BS2_DEVICE_ID deviceId, BS2_ACCESS_GROUP_ID* accessGroupIds, uint32_t accessGroupIdCount, BS2AccessGroup** accessGroupObj, uint32_t* numAccessGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAccessGroup(void* context, BS2_DEVICE_ID deviceId, BS2AccessGroup** accessGroupObj, uint32_t* numAccessGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAccessGroup(void* context, BS2_DEVICE_ID deviceId, BS2AccessGroup* accessGroups, uint32_t accessGroupCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAccessGroup(void* context, BS2_DEVICE_ID deviceId, BS2_ACCESS_GROUP_ID* accessGroupIds, uint32_t accessGroupIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAccessGroup(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAccessLevel(void* context, BS2_DEVICE_ID deviceId, BS2_ACCESS_LEVEL_ID* accessLevelIds, uint32_t accessLevelIdCount, BS2AccessLevel** accessLevelObj, uint32_t* numAccessLevel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAccessLevel(void* context, BS2_DEVICE_ID deviceId, BS2AccessLevel** accessLevelObj, uint32_t* numAccessLevel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAccessLevel(void* context, BS2_DEVICE_ID deviceId, BS2AccessLevel* accessLevels, uint32_t accessLevelCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAccessLevel(void* context, BS2_DEVICE_ID deviceId, BS2_ACCESS_LEVEL_ID* accessLevelIds, uint32_t accessLevelIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAccessLevel(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAccessSchedule(void* context, BS2_DEVICE_ID deviceId, BS2_SCHEDULE_ID* accessSheduleIds, uint32_t accessScheduleIdCount, BS2Schedule** accessScheduleObj, uint32_t* numAccessSchedule);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAccessSchedule(void* context, BS2_DEVICE_ID deviceId, BS2Schedule** accessScheduleObj, uint32_t* numAccessSchedule);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAccessSchedule(void* context, BS2_DEVICE_ID deviceId, BS2Schedule* accessShedules, uint32_t accessSheduleCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAccessSchedule(void* context, BS2_DEVICE_ID deviceId, BS2_SCHEDULE_ID* accessSheduleIds, uint32_t accessScheduleIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAccessSchedule(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetHolidayGroup(void* context, BS2_DEVICE_ID deviceId, BS2_HOLIDAY_GROUP_ID* holidayGroupIds, uint32_t holidayGroupIdCount, BS2HolidayGroup** holidayGroupObj, uint32_t* numHolidayGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllHolidayGroup(void* context, BS2_DEVICE_ID deviceId, BS2HolidayGroup** holidayGroupObj, uint32_t* numHolidayGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetHolidayGroup(void* context, BS2_DEVICE_ID deviceId, BS2HolidayGroup* holidayGroups, uint32_t holidayGroupCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveHolidayGroup(void* context, BS2_DEVICE_ID deviceId, BS2_HOLIDAY_GROUP_ID* holidayGroupIds, uint32_t holidayGroupIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllHolidayGroup(void* context, BS2_DEVICE_ID deviceId);

// Blacklist api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetBlackList(void* context, BS2_DEVICE_ID deviceId, BS2BlackList* blacklists, uint32_t blacklistCount, BS2BlackList** blacklistObj, uint32_t* numBlacklist);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllBlackList(void* context, BS2_DEVICE_ID deviceId, BS2BlackList** blacklistObj, uint32_t* numBlacklist);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetBlackList(void* context, BS2_DEVICE_ID deviceId, BS2BlackList* blacklists, uint32_t blacklistCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveBlackList(void* context, BS2_DEVICE_ID deviceId, BS2BlackList* blacklists, uint32_t blacklistCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllBlackList(void* context, BS2_DEVICE_ID deviceId);

// Card api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanCard(void* context, BS2_DEVICE_ID deviceId, BS2Card* card, OnReadyToScan ptrReadyToScan);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_WriteCard(void* context, BS2_DEVICE_ID deviceId, BS2SmartCardData* smartCard);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EraseCard(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_WriteQRCode(const char* qrText, BS2CSNCard* card);

// Config api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearDatabase(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ResetConfig(void* context, BS2_DEVICE_ID deviceId, uint8_t includingDB);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ResetConfigExceptNetInfo(void* context, BS2_DEVICE_ID deviceId, uint8_t includingDB);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetConfig(void* context, BS2_DEVICE_ID deviceId, BS2Configs* configs);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetConfig(void* context, BS2_DEVICE_ID deviceId, BS2Configs* configs);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFactoryConfig(void* context, BS2_DEVICE_ID deviceId, BS2FactoryConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSystemConfig(void* context, BS2_DEVICE_ID deviceId, BS2SystemConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSystemConfig(void* context, BS2_DEVICE_ID deviceId, BS2SystemConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAuthConfig(void* context, BS2_DEVICE_ID deviceId, BS2AuthConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAuthConfig(void* context, BS2_DEVICE_ID deviceId, BS2AuthConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetStatusConfig(void* context, BS2_DEVICE_ID deviceId, BS2StatusConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetStatusConfig(void* context, BS2_DEVICE_ID deviceId, BS2StatusConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDisplayConfig(void* context, BS2_DEVICE_ID deviceId, BS2DisplayConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDisplayConfig(void* context, BS2_DEVICE_ID deviceId, BS2DisplayConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPConfig(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPConfigExt(void* context, BS2_DEVICE_ID deviceId, BS2IpConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPConfigViaUDP(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPConfig(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPConfigExt(void* context, BS2_DEVICE_ID deviceId, BS2IpConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPConfigViaUDP(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetTNAConfig(void* context, BS2_DEVICE_ID deviceId, BS2TNAConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetTNAConfig(void* context, BS2_DEVICE_ID deviceId, BS2TNAConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetCardConfig(void* context, BS2_DEVICE_ID deviceId, BS2CardConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetCardConfig(void* context, BS2_DEVICE_ID deviceId, BS2CardConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFingerprintConfig(void* context, BS2_DEVICE_ID deviceId, BS2FingerprintConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFingerprintConfig(void* context, BS2_DEVICE_ID deviceId, BS2FingerprintConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetRS485Config(void* context, BS2_DEVICE_ID deviceId, BS2Rs485Config* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetRS485Config(void* context, BS2_DEVICE_ID deviceId, BS2Rs485Config* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWiegandConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetWiegandConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWiegandDeviceConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandDeviceConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetWiegandDeviceConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandDeviceConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetInputConfig(void* context, BS2_DEVICE_ID deviceId, BS2InputConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetInputConfig(void* context, BS2_DEVICE_ID deviceId, BS2InputConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWlanConfig(void* context, BS2_DEVICE_ID deviceId, BS2WlanConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetWlanConfig(void* context, BS2_DEVICE_ID deviceId, BS2WlanConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetTriggerActionConfig(void* context, BS2_DEVICE_ID deviceId, BS2TriggerActionConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetTriggerActionConfig(void* context, BS2_DEVICE_ID deviceId, BS2TriggerActionConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetEventConfig(void* context, BS2_DEVICE_ID deviceId, BS2EventConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetEventConfig(void* context, BS2_DEVICE_ID deviceId, BS2EventConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWiegandMultiConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandMultiConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetWiegandMultiConfig(void* context, BS2_DEVICE_ID deviceId, BS2WiegandMultiConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetCard1xConfig(void* context, BS2_DEVICE_ID deviceId, BS1CardConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetCard1xConfig(void* context, BS2_DEVICE_ID deviceId, BS1CardConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSystemExtConfig(void* context, BS2_DEVICE_ID deviceId, BS2SystemConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSystemExtConfig(void* context, BS2_DEVICE_ID deviceId, BS2SystemConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetVoipConfig(void* context, BS2_DEVICE_ID deviceId, BS2VoipConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetVoipConfig(void* context, BS2_DEVICE_ID deviceId, BS2VoipConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFaceConfig(void* context, BS2_DEVICE_ID deviceId, BS2FaceConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFaceConfig(void* context, BS2_DEVICE_ID deviceId, BS2FaceConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetCardConfigEx(void* context, BS2_DEVICE_ID deviceId, BS2CardConfigEx* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetCardConfigEx(void* context, BS2_DEVICE_ID deviceId, BS2CardConfigEx* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetRS485ConfigEx(void* context, BS2_DEVICE_ID deviceId, BS2Rs485ConfigEX* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetRS485ConfigEx(void* context, BS2_DEVICE_ID deviceId, BS2Rs485ConfigEX* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSupportedConfigMask(void* context, BS2_DEVICE_ID deviceId, BS2_CONFIG_MASK* configMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDstConfig(void* context, BS2_DEVICE_ID deviceId, BS2DstConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDstConfig(void* context, BS2_DEVICE_ID deviceId, BS2DstConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDesFireCardConfigEx(void* context, BS2_DEVICE_ID deviceId, BS2DesFireCardConfigEx* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDesFireCardConfigEx(void* context, BS2_DEVICE_ID deviceId, BS2DesFireCardConfigEx* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAuthConfigExt(void* context, BS2_DEVICE_ID deviceId, BS2AuthConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAuthConfigExt(void* context, BS2_DEVICE_ID deviceId, const BS2AuthConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFaceConfigExt(void* context, BS2_DEVICE_ID deviceId, BS2FaceConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFaceConfigExt(void* context, BS2_DEVICE_ID deviceId, const BS2FaceConfigExt* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetThermalCameraConfig(void* context, BS2_DEVICE_ID deviceId, BS2ThermalCameraConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetThermalCameraConfig(void* context, BS2_DEVICE_ID deviceId, const BS2ThermalCameraConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetBarcodeConfig(void* context, BS2_DEVICE_ID deviceId, BS2BarcodeConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetBarcodeConfig(void* context, BS2_DEVICE_ID deviceId, const BS2BarcodeConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetInputConfigEx(void* context, BS2_DEVICE_ID deviceId, BS2InputConfigEx* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetInputConfigEx(void* context, BS2_DEVICE_ID deviceId, const BS2InputConfigEx* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetRelayActionConfig(void* context, BS2_DEVICE_ID deviceId, BS2RelayActionConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetRelayActionConfig(void* context, BS2_DEVICE_ID deviceId, const BS2RelayActionConfig* config);

// Door api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_ID* doorIds, uint32_t doorIdCount, BS2Door** doorObj, uint32_t* numDoor);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllDoor(void* context, BS2_DEVICE_ID deviceId, BS2Door** doorObj, uint32_t* numDoor);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDoorStatus(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_ID* doorIds, uint32_t doorIdCount, BS2DoorStatus** doorStatusObj, uint32_t* numDoorStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllDoorStatus(void* context, BS2_DEVICE_ID deviceId, BS2DoorStatus** doorStatusObj, uint32_t* numDoorStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDoor(void* context, BS2_DEVICE_ID deviceId, BS2Door* doors, uint32_t doorCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDoorAlarm(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_ALARM_FLAG flag, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllDoor(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ReleaseDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_FLAG flag, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_LockDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_FLAG flag, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_UnlockDoor(void* context, BS2_DEVICE_ID deviceId, BS2_DOOR_FLAG flag, BS2_DOOR_ID* doorIds, uint32_t doorIdCount);

// Lift api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLift(void* context, BS2_DEVICE_ID deviceId, BS2_LIFT_ID* liftIds, uint32_t liftIdCount, BS2Lift** liftObj, uint32_t* numLift);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllLift(void* context, BS2_DEVICE_ID deviceId, BS2Lift** liftObj, uint32_t* numLift);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLiftStatus(void* context, BS2_DEVICE_ID deviceId, BS2_LIFT_ID* liftIds, uint32_t liftIdCount, BS2LiftStatus** liftStatusObj, uint32_t* numLiftStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllLiftStatus(void* context, BS2_DEVICE_ID deviceId, BS2LiftStatus** liftStatusObj, uint32_t* numLiftStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetLift(void* context, BS2_DEVICE_ID deviceId, BS2Lift* lifts, uint32_t liftCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetLiftAlarm(void* context, BS2_DEVICE_ID deviceId, BS2_LIFT_ALARM_FLAG flag, BS2_LIFT_ID* liftIds, uint32_t liftIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveLift(void* context, BS2_DEVICE_ID deviceId, BS2_LIFT_ID* liftIds, uint32_t liftIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllLift(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ReleaseFloor(void* context, BS2_DEVICE_ID deviceId, BS2_FLOOR_FLAG flag, BS2_LIFT_ID liftID, uint16_t* floorIndexs, uint8_t floorIndexCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ActivateFloor(void* context, BS2_DEVICE_ID deviceId, BS2_FLOOR_FLAG flag, BS2_LIFT_ID liftID, uint16_t* floorIndexs, uint8_t floorIndexCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_DeActivateFloor(void* context, BS2_DEVICE_ID deviceId, BS2_FLOOR_FLAG flag, BS2_LIFT_ID liftID, uint16_t* floorIndexs, uint8_t floorIndexCount);

// Fingerprint api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLastFingerprintImage(void* context, BS2_DEVICE_ID deviceId, uint8_t** imageObj, uint32_t* imageWidth, uint32_t* imageHeight);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanFingerprint(void* context, BS2_DEVICE_ID deviceId,
															BS2Fingerprint* finger,
															uint32_t templateIndex,
															uint32_t quality,
															uint8_t templateFormat, //BS2_FINGER_TEMPLATE_FORMAT
															OnReadyToScan ptrReadyToScan);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanFingerprintEx(void* context, BS2_DEVICE_ID deviceId,
															BS2Fingerprint* finger,
															uint32_t templateIndex,
															uint32_t quality,
															uint8_t templateFormat, //BS2_FINGER_TEMPLATE_FORMAT
															uint32_t* outquality,
															OnReadyToScan ptrReadyToScan);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanFingerprintImage(void* context, BS2_DEVICE_ID deviceId,
															uint32_t templateIndex,
															uint32_t quality,
															uint8_t templateFormat, //BS2_FINGER_TEMPLATE_FORMAT
															unsigned char* imageData,
															OnReadyToScan ptrReadyToScan);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_VerifyFingerprint(void* context, BS2_DEVICE_ID deviceId, BS2Fingerprint* finger);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFingerTemplateQuality(const uint8_t* templateBuffer, const int32_t templateSize, int32_t* score);

// Log api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLog(void* context, BS2_DEVICE_ID deviceId, BS2_EVENT_ID eventId, uint32_t amount, BS2Event** logsObj, uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogBlob(void* context, BS2_DEVICE_ID deviceId, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventBlob** logsObj, uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFilteredLog(void* context, BS2_DEVICE_ID deviceId, char* uid, BS2_EVENT_CODE eventCode, BS2_TIMESTAMP start, BS2_TIMESTAMP end, uint8_t tnakey, 
															BS2Event** logsObj,uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFilteredLogSinceEventId(void* context, BS2_DEVICE_ID deviceId, char* uid, BS2_EVENT_CODE eventCode, BS2_TIMESTAMP start, BS2_TIMESTAMP end, uint8_t tnakey, BS2_EVENT_ID lastEventId, uint32_t amount, 
															BS2Event** logsObj, uint32_t* numLog);
																					
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetImageLog(void* context, BS2_DEVICE_ID deviceId, BS2_EVENT_ID eventId, uint8_t** imageObj, uint32_t* imageSize);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearLog(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_StartMonitoringLog(void* context, BS2_DEVICE_ID deviceId, OnLogReceived ptrLogReceived);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_StartMonitoringLogEx(void* context, BS2_DEVICE_ID deviceId, OnLogReceivedEx ptrLogReceivedEx);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_StopMonitoringLog(void* context, BS2_DEVICE_ID deviceId);

// Misc api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_FactoryReset(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RebootDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_LockDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_UnlockDevice(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceTime(void* context, BS2_DEVICE_ID deviceId, BS2_TIMESTAMP* gmtTime);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceTime(void* context, BS2_DEVICE_ID deviceId, BS2_TIMESTAMP gmtTime);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_UpgradeFirmware(void* context, BS2_DEVICE_ID deviceId, uint8_t* firmwareData, uint32_t firmwareDataLen, uint8_t keepVerifyingSlaveDevice, OnProgressChanged ptrProgressChanged);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_UpdateResource(void* context, BS2_DEVICE_ID deviceId, BS2ResourceElement* resourceElement, uint8_t keepVerifyingSlaveDevice, OnProgressChanged ptrProgressChanged);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetKeepAliveTimeout(void* context, long ms);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_MakePinCode(void* context, char* plaintext, unsigned char* ciphertext);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_MakePinCodeWithKey(void* context, char* plaintext, unsigned char* ciphertext, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ComputeCRC16CCITT(unsigned char* data, uint32_t dataLen, uint16_t* crc);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetCardModel(char* modelName, BS2_CARD_MODEL* cardModel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceCapabilities(void* context, BS2_DEVICE_ID deviceId, BS2DeviceCapabilities* capabilities);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RunAction(void* context, BS2_DEVICE_ID deviceId, const BS2Action* action);

// Slave Control api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSlaveDevice(void* context, BS2_DEVICE_ID deviceId, BS2Rs485SlaveDevice** slaveDeviceObj, uint32_t* slaveDeviceCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSlaveDevice(void* context, BS2_DEVICE_ID deviceId, BS2Rs485SlaveDevice* slaveDevices, uint32_t slaveDeviceCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveSlaveDevice(void* context, BS2_DEVICE_ID deviceId);

// Server Matching api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetServerMatchingHandler(void* context, OnVerifyUser ptrVerifyUser, OnIdentifyUser ptrIdentifyUser);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_VerifyUser(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_IdentifyUser(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserBlob* userBlob);

// User api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatabaseInfo(void* context, BS2_DEVICE_ID deviceId, uint32_t* numUsers, uint32_t* numCards, uint32_t* numFingers, uint32_t* numFaces, IsAcceptableUserID ptrIsAcceptableUserID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserList(void* context, BS2_DEVICE_ID deviceId,
														char** uidsObj, uint32_t* numUid, 
														IsAcceptableUserID ptrIsAcceptableUserID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfos(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatas(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserBlob* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EnrolUser(void* context, BS2_DEVICE_ID deviceId, BS2UserBlob* userBlob, uint32_t userCount, uint8_t overwrite);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EnrollUser(void* context, BS2_DEVICE_ID deviceId, BS2UserBlob* userBlob, uint32_t userCount, uint8_t overwrite);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveUser(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllUser(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSupportedUserMask(void* context, BS2_DEVICE_ID deviceId, BS2_USER_MASK* userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetUserPhraseHandler(void* context, OnUserPhrase ptrQuery);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ResponseUserPhrase(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, const BS2_USER_PHRASE userPhrase);

// Wiegand api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SearchWiegandDevices(void* context, BS2_DEVICE_ID deviceId, BS2_DEVICE_ID** wiegandDeviceObj, uint32_t* numWiegandDevice);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetWiegandDevices(void* context, BS2_DEVICE_ID deviceId, BS2_DEVICE_ID** wiegandDeviceObj, uint32_t* numWiegandDevice);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_AddWiegandDevices(void* context, BS2_DEVICE_ID deviceId, BS2_DEVICE_ID* wiegandDevice, uint32_t numWiegandDevice);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveWiegandDevices(void* context, BS2_DEVICE_ID deviceId, BS2_DEVICE_ID* wiegandDevice, uint32_t numWiegandDevice);

// Zone api
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2AntiPassbackZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2AntiPassbackZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2AntiPassbackZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAntiPassbackZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID, char* uids, uint32_t uidCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearAllAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2TimedAntiPassbackZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2TimedAntiPassbackZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetTimedAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllTimedAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2TimedAntiPassbackZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetTimedAntiPassbackZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllTimedAntiPassbackZone(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearTimedAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID, char* uids, uint32_t uidCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearAllTimedAntiPassbackZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFireAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2FireAlarmZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllFireAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2FireAlarmZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFireAlarmZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllFireAlarmZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFireAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2FireAlarmZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFireAlarmZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveFireAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllFireAlarmZone(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ScheduledLockUnlockZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2ScheduledLockUnlockZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetScheduledLockUnlockZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllScheduledLockUnlockZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2ScheduledLockUnlockZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetScheduledLockUnlockZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllScheduledLockUnlockZone(void* context, BS2_DEVICE_ID deviceId);

//FloorLevel
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFloorLevel(void* context, BS2_DEVICE_ID deviceId, BS2_FLOOR_LEVEL_ID* floorLevelIds, uint32_t floorLevelIdCount, BS2FloorLevel** floorLevelObj, uint32_t* numFloorLevel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllFloorLevel(void* context, BS2_DEVICE_ID deviceId, BS2FloorLevel** floorLevelObj, uint32_t* numFloorLevel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetFloorLevel(void* context, BS2_DEVICE_ID deviceId, BS2FloorLevel* floorLevels, uint32_t floorLevelCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveFloorLevel(void* context, BS2_DEVICE_ID deviceId, BS2_FLOOR_LEVEL_ID* floorLevelIds, uint32_t floorLevelIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllFloorLevel(void* context, BS2_DEVICE_ID deviceId);

//Face
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanFace(void* context, BS2_DEVICE_ID deviceId, BS2Face* face, uint8_t enrollmentThreshold, OnReadyToScan ptrReadyToScan);

 //FaceEx
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ScanFaceEx(void* context, BS2_DEVICE_ID deviceId, BS2FaceEx* faceEx, uint8_t enrollmentThreshold, OnReadyToScan ptrReadyToScan);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ExtractTemplateFaceEx(void* context, BS2_DEVICE_ID deviceId, const uint8_t* imageData, uint32_t imageDataLen, int isWarped, BS2TemplateEx* templateEx);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetNormalizedImageFaceEx(void* context, BS2_DEVICE_ID deviceId, const uint8_t* unwarpedImage, uint32_t unwarpedImageLen, uint8_t* warpedImage, uint32_t* warpedImageLen);

//AuthGroup
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAuthGroup(void* context, BS2_DEVICE_ID deviceId, BS2_AUTH_GROUP_ID* authGroupIds, uint32_t authGroupIdCount, BS2AuthGroup** authGroupObj, uint32_t* numAuthGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAuthGroup(void* context, BS2_DEVICE_ID deviceId, BS2AuthGroup** authGroupObj, uint32_t* numAuthGroup);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAuthGroup(void* context, BS2_DEVICE_ID deviceId, BS2AuthGroup* authGroups, uint32_t authGroupCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAuthGroup(void* context, BS2_DEVICE_ID deviceId, BS2_AUTH_GROUP_ID* authGroupIds, uint32_t authGroupIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAuthGroup(void* context, BS2_DEVICE_ID deviceId);

//UserEx
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_VerifyUserEx(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserBlobEx* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_IdentifyUserEx(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserBlobEx* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfosEx(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserBlobEx* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatasEx(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserBlobEx* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EnrolUserEx(void* context, BS2_DEVICE_ID deviceId, BS2UserBlobEx* userBlob, uint32_t userCount, uint8_t overwrite);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EnrollUserEx(void* context, BS2_DEVICE_ID deviceId, BS2UserBlobEx* userBlob, uint32_t userCount, uint8_t overwrite);

//USB Exported 
DEPRECATED_FUNC BS_API_EXPORT void* BS_CALLING_CONVENTION BS2_AllocateUsbContext(const char* szDir);
DEPRECATED_FUNC BS_API_EXPORT void BS_CALLING_CONVENTION BS2_ReleaseUsbContext(void* context);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatabaseInfoFromDir(void* context, const char* szDir, uint32_t* numUsers, uint32_t* numCards, uint32_t* numFingers, uint32_t* numFaces, IsAcceptableUserID ptrIsAcceptableUserID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserListFromDir(void* context, const char* szDir, char** uidsObj, uint32_t* numUid, IsAcceptableUserID ptrIsAcceptableUserID);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfosFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatasFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserBlob* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfosExFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserBlobEx* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatasExFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserBlobEx* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogFromDir(void* context, const char* szDir, BS2_EVENT_ID eventId, uint32_t amount, BS2Event** logsObj, uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogBlobFromDir(void* context, const char* szDir, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventBlob** logsObj, uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFilteredLogFromDir(void* context, const char* szDir, char* uid, BS2_EVENT_CODE eventCode, BS2_TIMESTAMP start, BS2_TIMESTAMP end, uint8_t tnakey, BS2Event** logsObj, uint32_t* numLog);
//BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFilteredLogSinceEventIdFromDir(void* context, const char* szDir, char* uid, BS2_EVENT_CODE eventCode, BS2_TIMESTAMP start, BS2_TIMESTAMP end, uint8_t tnakey, BS2_EVENT_ID lastEventId, uint32_t amount, BS2Event** logsObj, uint32_t* numLog);

//SlaveEx
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSlaveExDevice(void* context, BS2_DEVICE_ID deviceId, uint32_t channelPort, BS2Rs485SlaveDeviceEX** slaveDevices, uint32_t* outchannelPort, uint32_t* slaveDeviceCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSlaveExDevice(void* context, BS2_DEVICE_ID deviceId, uint32_t channelPort, BS2Rs485SlaveDeviceEX* slaveDevices, uint32_t slaveDeviceCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SearchDevicesCoreStation(void* context);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDevicesCoreStation(void* context, BS2_DEVICE_ID** deviceListObj, uint32_t* numDevice);

//IntrusionAlarmZone
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIntrusionAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2IntrusionAlarmZoneBlob* zoneBlob, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIntrusionAlarmZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllIntrusionAlarmZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIntrusionAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2IntrusionAlarmZoneBlob* zoneBlob, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIntrusionAlarmZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveIntrusionAlarmZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllIntrusionAlarmZone(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIntrusionAlarmZoneArm(void* context, BS2_DEVICE_ID deviceId, uint8_t armed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);

//InterlockZone
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetInterlockZone(void* context, BS2_DEVICE_ID deviceId, BS2InterlockZoneBlob* zoneBlob, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetInterlockZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllInterlockZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetInterlockZone(void* context, BS2_DEVICE_ID deviceId, BS2InterlockZoneBlob* zoneBlob, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetInterlockZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveInterlockZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllInterlockZone(void* context, BS2_DEVICE_ID deviceId);

//LiftLockUnlockZone
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLiftLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2LiftLockUnlockZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllLiftLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2LiftLockUnlockZone** zoneObj, uint32_t* numZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLiftLockUnlockZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllLiftLockUnlockZoneStatus(void* context, BS2_DEVICE_ID deviceId, BS2ZoneStatus** zoneStatusObj, uint32_t* numZoneStatus);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetLiftLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2LiftLockUnlockZone* zones, uint32_t zoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetLiftLockUnlockZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveLiftLockUnlockZone(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllLiftLockUnlockZone(void* context, BS2_DEVICE_ID deviceId);

//Ethernet Zone
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceZoneMasterConfig(void* context, BS2_DEVICE_ID deviceId, BS2DeviceZoneMasterConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceZoneMasterConfig(void* context, BS2_DEVICE_ID deviceId, const BS2DeviceZoneMasterConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveDeviceZoneMasterConfig(void* context, BS2_DEVICE_ID deviceId);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceZone(void* context, BS2_DEVICE_ID deviceId, const BS2_DEVICE_ZONE_TABLE_ID* Ids, uint32_t IdCount, BS2DeviceZone** deviceZoneObj, uint32_t* numDeviceZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllDeviceZone(void* context, BS2_DEVICE_ID deviceId, BS2DeviceZone** deviceZoneObj, uint32_t* numDeviceZone);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceZone(void* context, BS2_DEVICE_ID deviceId, const BS2DeviceZone* deviceZones, uint32_t deviceZoneCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveDeviceZone(void* context, BS2_DEVICE_ID deviceId, const BS2_DEVICE_ZONE_TABLE_ID* Ids, uint32_t IdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllDeviceZone(void* context, BS2_DEVICE_ID deviceId);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceZoneConfig(void* context, BS2_DEVICE_ID deviceId, BS2DeviceZoneConfig* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceZoneConfig(void* context, BS2_DEVICE_ID deviceId, const BS2DeviceZoneConfig* config);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceZoneAlarm(void* context, BS2_DEVICE_ID deviceId, uint8_t alarmed, BS2_ZONE_ID* zoneIds, uint32_t zoneIdCount);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearDeviceZoneAccessRecord(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID, char* uids, uint32_t uidCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ClearAllDeviceZoneAccessRecord(void* context, BS2_DEVICE_ID deviceId, BS2_ZONE_ID zoneID);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDeviceZoneAGEntranceLimit(void* context, BS2_DEVICE_ID deviceId, const BS2_ZONE_ID* Ids, uint32_t IdCount, BS2DeviceZoneAGEntranceLimit** deviceZoneAGEntranceLimitObj, uint32_t* numDeviceZoneAGEntranceLimit);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllDeviceZoneAGEntranceLimit(void* context, BS2_DEVICE_ID deviceId, BS2DeviceZoneAGEntranceLimit** deviceZoneAGEntranceLimitObj, uint32_t* numDeviceZoneAGEntranceLimit);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDeviceZoneAGEntranceLimit(void* context, BS2_DEVICE_ID deviceId, const BS2DeviceZoneAGEntranceLimit* deviceZoneAGEntranceLimits, uint32_t deviceZoneAGEntranceLimitCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveDeviceZoneAGEntranceLimit(void* context, BS2_DEVICE_ID deviceId, const BS2_ZONE_ID* Ids, uint32_t IdCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllDeviceZoneAGEntranceLimit(void* context, BS2_DEVICE_ID deviceId);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetCheckGlobalAPBViolationHandler(void* context, OnCheckGlobalAPBViolation ptrCheckGlobalAPBViolation);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_CheckGlobalAPBViolation(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2_ZONE_ID zoneID);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetGlobalAPBViolationByDoorOpenHandler(void* context, OnCheckGlobalAPBViolationByDoorOpen ptrCheck, OnUpdateGlobalAPBViolationByDoorOpen ptrUpdate);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_CheckGlobalAPBViolationByDoorOpen(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2_ZONE_ID zoneID);

// Encryption Key
DEPRECATED_FUNC BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDataEncryptKey(void* context, BS2_DEVICE_ID deviceId, BS2EncryptKey* keyInfo);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDataEncryptKey(void* context, BS2_DEVICE_ID deviceId, const BS2EncryptKey* keyInfo);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveDataEncryptKey(void* context, BS2_DEVICE_ID deviceId);

//[IPv6]
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPConfigViaUDPEx(void* context, BS2_DEVICE_ID deviceId, BS2IpConfig* config, const char* hostipAddr);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPConfigViaUDPEx(void* context, BS2_DEVICE_ID deviceId, const BS2IpConfig* config, const char* hostipAddr);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPV6Config(void* context, BS2_DEVICE_ID deviceId, BS2IPV6Config* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPV6Config(void* context, BS2_DEVICE_ID deviceId, const BS2IPV6Config* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPV6ConfigViaUDP(void* context, BS2_DEVICE_ID deviceId, BS2IPV6Config* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPV6ConfigViaUDP(void* context, BS2_DEVICE_ID deviceId, const BS2IPV6Config* config);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetIPV6ConfigViaUDPEx(void* context, BS2_DEVICE_ID deviceId, BS2IPV6Config* config, const char* hostipAddr);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetIPV6ConfigViaUDPEx(void* context, BS2_DEVICE_ID deviceId, const BS2IPV6Config* config, const char* hostipAddr);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetEnableIPV4(void* context, int* enable);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetEnableIPV4(void* context, int enable);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetEnableIPV6(void* context, int* enable);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetEnableIPV6(void* context, int enable);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetServerPortIPV6(void* context, BS2_PORT serverPort);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetServerPortIPV6(void* context, BS2_PORT* serverPort);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetSSLServerPortIPV6(void* context, BS2_PORT sslServerPort);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSSLServerPortIPV6(void* context, BS2_PORT* sslServerPort);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSpecifiedDeviceInfo(void* context, BS2_DEVICE_ID deviceId, BS2EnumSpecifiedDeviceInfo specifiedDeviceInfo, void* pOutDeviceInfo, uint32_t nDeviceInfoSize, uint32_t* pOutDeviceInfoSize);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_ConnectDeviceIPV6(void* context, BS2_DEVICE_ID deviceId);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SearchDevicesCoreStationEx(void* context, const char* hostipAddr);


//[Get ServerPort]
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetServerPort(void* context, BS2_PORT* serverPort);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetSSLServerPort(void* context, BS2_PORT* sslServerPort);


//[Set/Get default response wait timeout]
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetDefaultResponseTimeout(void* context, long ms);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetDefaultResponseTimeout(void* context, long* poMs);


//[User Small Blob]
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_VerifyUserSmall(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserSmallBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_IdentifyUserSmall(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserSmallBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallInfos(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserSmallBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallDatas(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserSmallBlob* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EnrollUserSmall(void* context, BS2_DEVICE_ID deviceId, BS2UserSmallBlob* userBlob, uint32_t userCount, uint8_t overwrite);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_VerifyUserSmallEx(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserSmallBlobEx* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_IdentifyUserSmallEx(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserSmallBlobEx* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallInfosEx(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserSmallBlobEx* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallDatasEx(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserSmallBlobEx* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EnrollUserSmallEx(void* context, BS2_DEVICE_ID deviceId, BS2UserSmallBlobEx* userBlob, uint32_t userCount, uint8_t overwrite);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallInfosFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserSmallBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallDatasFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserSmallBlob* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallInfosExFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserSmallBlobEx* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallDatasExFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserSmallBlobEx* userBlob, BS2_USER_MASK userMask);


//[Admin 1000]
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAuthOperatorLevelEx(void* context, BS2_DEVICE_ID deviceId, BS2_USER_ID* userIDs, uint32_t userIDCount, BS2AuthOperatorLevel** operatorlevelObj, uint32_t* numOperatorlevel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetAllAuthOperatorLevelEx(void* context, BS2_DEVICE_ID deviceId, BS2AuthOperatorLevel** operatorlevelObj, uint32_t* numOperatorlevel);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_SetAuthOperatorLevelEx(void* context, BS2_DEVICE_ID deviceId, BS2AuthOperatorLevel* operatorlevels, uint32_t operatorlevelCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAuthOperatorLevelEx(void* context, BS2_DEVICE_ID deviceId, BS2_USER_ID* userIDs, uint32_t userIDCount);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_RemoveAllAuthOperatorLevelEx(void* context, BS2_DEVICE_ID deviceId);


BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogSmallBlob(void* context, BS2_DEVICE_ID deviceId, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventSmallBlob** logsObj, uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogSmallBlobFromDir(void* context, const char* szDir, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventSmallBlob** logsObj, uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogSmallBlobEx(void* context, BS2_DEVICE_ID deviceId, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventSmallBlobEx** logsObj, uint32_t* numLog);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogSmallBlobExFromDir(void* context, const char* szDir, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventSmallBlobEx** logsObj, uint32_t* numLog);

// FaceEx
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_EnrollUserFaceEx(void* context, BS2_DEVICE_ID deviceId, BS2UserFaceExBlob* userBlob, uint32_t userCount, uint8_t overwrite);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfosFaceEx(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserFaceExBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatasFaceEx(void* context, BS2_DEVICE_ID deviceId, char* uids, uint32_t uidCount, BS2UserFaceExBlob* userBlob, BS2_USER_MASK userMask);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfosFaceExFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserFaceExBlob* userBlob);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatasFaceExFromDir(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserFaceExBlob* userBlob, BS2_USER_MASK userMask);

//USB Exported (+V2.8.2.4)
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatabaseInfoFromDirWithKey(void* context, const char* szDir, uint32_t* numUsers, uint32_t* numCards, uint32_t* numFingers, uint32_t* numFaces, IsAcceptableUserID ptrIsAcceptableUserID, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserListFromDirWithKey(void* context, const char* szDir, char** uidsObj, uint32_t* numUid, IsAcceptableUserID ptrIsAcceptableUserID, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfosFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserBlob* userBlob, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatasFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserBlob* userBlob, BS2_USER_MASK userMask, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfosExFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserBlobEx* userBlob, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatasExFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserBlobEx* userBlob, BS2_USER_MASK userMask, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogFromDirWithKey(void* context, const char* szDir, BS2_EVENT_ID eventId, uint32_t amount, BS2Event** logsObj, uint32_t* numLog, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetFilteredLogFromDirWithKey(void* context, const char* szDir, char* uid, BS2_EVENT_CODE eventCode, BS2_TIMESTAMP start, BS2_TIMESTAMP end, uint8_t tnakey, BS2Event** logsObj, uint32_t* numLog, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallInfosFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserSmallBlob* userBlob, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallDatasFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserSmallBlob* userBlob, BS2_USER_MASK userMask, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallInfosExFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserSmallBlobEx* userBlob, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserSmallDatasExFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserSmallBlobEx* userBlob, BS2_USER_MASK userMask, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserInfosFaceExFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserFaceExBlob* userBlob, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetUserDatasFaceExFromDirWithKey(void* context, const char* szDir, char* uids, uint32_t uidCount, BS2UserFaceExBlob* userBlob, BS2_USER_MASK userMask, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogBlobFromDirWithKey(void* context, const char* szDir, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventBlob** logsObj, uint32_t* numLog, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogSmallBlobFromDirWithKey(void* context, const char* szDir, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventSmallBlob** logsObj, uint32_t* numLog, const BS2EncryptKey* key);
BS_API_EXPORT int BS_CALLING_CONVENTION BS2_GetLogSmallBlobExFromDirWithKey(void* context, const char* szDir, uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, BS2EventSmallBlobEx** logsObj, uint32_t* numLog, const BS2EncryptKey* key);

BS_API_EXPORT int BS_CALLING_CONVENTION BS2_VerifyUserFaceEx(void* context, BS2_DEVICE_ID deviceId, BS2_PACKET_SEQ seq, int handleResult, BS2UserFaceExBlob* userBlob);


#ifdef __cplusplus
}
#endif


#endif /* CORE_SRC_BS_API_H_ */
