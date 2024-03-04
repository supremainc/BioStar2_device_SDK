using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suprema
{
    [Flags]
    public enum BS2ErrorCode
    {
        BS_SDK_SUCCESS                                      = 1,
        BS_SDK_DURESS_SUCCESS                               = 2,
        BS_SDK_FIRST_AUTH_SUCCESS                           = 3,
        BS_SDK_SECOND_AUTH_SUCCESS                          = 4,
        BS_SDK_DUAL_AUTH_SUCCESS                            = 5,
        BS_SDK_WIEGAND_BYPASS_SUCCESS                       = 11,
        BS_SDK_ANONYMOUS_SUCCESS                            = 12,

		// Driver errors
        BS_SDK_ERROR_FROM_DEVICE_DRIVER                     = -1,

        // Communication errors
        BS_SDK_ERROR_CANNOT_OPEN_SOCKET                     = -101,
        BS_SDK_ERROR_CANNOT_CONNECT_SOCKET                  = -102,
        BS_SDK_ERROR_CANNOT_LISTEN_SOCKET                   = -103,
        BS_SDK_ERROR_CANNOT_ACCEPT_SOCKET                   = -104,
        BS_SDK_ERROR_CANNOT_READ_SOCKET                     = -105,
        BS_SDK_ERROR_CANNOT_WRITE_SOCKET                    = -106,
        BS_SDK_ERROR_SOCKET_IS_NOT_CONNECTED                = -107,
        BS_SDK_ERROR_SOCKET_IS_NOT_OPEN                     = -108,
        BS_SDK_ERROR_SOCKET_IS_NOT_LISTENED                 = -109,
        BS_SDK_ERROR_SOCKET_IN_PROGRESS                     = -110,
        //=> [IPv6]
        BS_SDK_ERROR_IPV4_IS_NOT_ENABLE					    = -111,
        BS_SDK_ERROR_IPV6_IS_NOT_ENABLE					    = -112,
        BS_SDK_ERROR_NOT_SUPPORTED_SPECIFIED_DEVICE_INFO    = -113,
        BS_SDK_ERROR_NOT_ENOUGTH_BUFFER					    = -114,
        BS_SDK_ERROR_NOT_SUPPORTED_IPV6					    = -115,
        BS_SDK_ERROR_INVALID_ADDRESS                        = -116,
        //<=

        // Packet errors
        BS_SDK_ERROR_INVALID_PARAM                          = -200,
        BS_SDK_ERROR_INVALID_PACKET                         = -201,
        BS_SDK_ERROR_INVALID_DEVICE_ID                      = -202,
        BS_SDK_ERROR_INVALID_DEVICE_TYPE                    = -203,
        BS_SDK_ERROR_PACKET_CHECKSUM                        = -204,
        BS_SDK_ERROR_PACKET_INDEX                           = -205,
        BS_SDK_ERROR_PACKET_COMMAND                         = -206,
        BS_SDK_ERROR_PACKET_SEQUENCE                        = -207,
        BS_SDK_ERROR_NO_PACKET                              = -209,
        BS_SDK_ERROR_INVALID_CODE_SIGN                      = -210,

        //Fingerprint errors
        BS_SDK_ERROR_EXTRACTION_FAIL                        = -300,
        BS_SDK_ERROR_VERIFY_FAIL                            = -301,
        BS_SDK_ERROR_IDENTIFY_FAIL                          = -302,
        BS_SDK_ERROR_IDENTIFY_TIMEOUT                       = -303,
        BS_SDK_ERROR_FINGERPRINT_CAPTURE_FAIL               = -304,
        BS_SDK_ERROR_FINGERPRINT_SCAN_TIMEOUT               = -305,
        BS_SDK_ERROR_FINGERPRINT_SCAN_CANCELLED             = -306,
        BS_SDK_ERROR_NOT_SAME_FINGERPRINT                   = -307,
        BS_SDK_ERROR_EXTRACTION_LOW_QUALITY                 = -308,
        BS_SDK_ERROR_CAPTURE_LOW_QUALITY                    = -309,
        BS_SDK_ERROR_CANNOT_FIND_FINGERPRINT                = -310,
        BS_SDK_ERROR_NO_FINGER_DETECTED                     = BS_SDK_ERROR_FINGERPRINT_CAPTURE_FAIL,
        BS_SDK_ERROR_FAKE_FINGER_DETECTED                   = -311,
        BS_SDK_ERROR_FAKE_FINGER_TRY_AGAIN                  = -312,
        BS_SDK_ERROR_FAKE_FINGER_SENSOR_ERROR               = -313,
        BS_SDK_ERROR_CANNOT_FIND_FACE                       = -314,
        BS_SDK_ERROR_FACE_CAPTURE_FAIL                      = -315,
        BS_SDK_ERROR_FACE_SCAN_TIMEOUT                      = -316,
        BS_SDK_ERROR_FACE_SCAN_CANCELLED                    = -317,
        BS_SDK_ERROR_FACE_SCAN_FAILED                       = -318,
        BS_SDK_ERROR_NO_FACE_DETECTED                       = BS_SDK_ERROR_FACE_CAPTURE_FAIL,
        BS_SDK_ERROR_UNMASKED_FACE_DETECTED                 = -319,
        BS_SDK_ERROR_FAKE_FACE_DETECTED                     = -320,
        BS_SDK_ERROR_CANNOT_ESTIMATE                        = -321,
        BS_SDK_ERROR_NORMALIZE_FACE                         = -322,
        BS_SDK_ERROR_SMALL_DETECTION                        = -323,
        BS_SDK_ERROR_LARGE_DETECTION                        = -324,
        BS_SDK_ERROR_BIASED_DETECTION                       = -325,
        BS_SDK_ERROR_ROTATED_FACE                           = -326,
        BS_SDK_ERROR_OVERLAPPED_FACE                        = -327,
        BS_SDK_ERROR_UNOPENED_EYES                          = -328,
        BS_SDK_ERROR_NOT_LOOKING_FRONT                      = -329,
        BS_SDK_ERROR_OCCLUDED_MOUTH                         = -330,
        BS_SDK_ERROR_MATCH_FAIL                             = -331,
        BS_SDK_ERROR_INCOMPATIBLE_FACE                      = -332,     // [+V2.8.3]


        //File I/O errors
        BS_SDK_ERROR_CANNOT_OPEN_DIR                        = -400,
        BS_SDK_ERROR_CANNOT_OPEN_FILE                       = -401,
        BS_SDK_ERROR_CANNOT_WRITE_FILE                      = -402,
        BS_SDK_ERROR_CANNOT_SEEK_FILE                       = -403,
        BS_SDK_ERROR_CANNOT_READ_FILE                       = -404,
        BS_SDK_ERROR_CANNOT_GET_STAT                        = -405,
        BS_SDK_ERROR_CANNOT_GET_SYSINFO                     = -406,
        BS_SDK_ERROR_DATA_MISMATCH                          = -407,
        BS_SDK_ERROR_ALREADY_OPEN_DIR                       = -408,

        // I/O errors
        BS_SDK_ERROR_INVALID_RELAY                          = -500,
        BS_SDK_ERROR_CANNOT_WRITE_IO_PACKET                 = -501,
        BS_SDK_ERROR_CANNOT_READ_IO_PACKET                  = -502,
        BS_SDK_ERROR_CANNOT_READ_INPUT                      = -503,
        BS_SDK_ERROR_READ_INPUT_TIMEOUT                     = -504,
        BS_SDK_ERROR_CANNOT_ENABLE_INPUT                    = -505,
        BS_SDK_ERROR_CANNOT_SET_INPUT_DURATION              = -506,
        BS_SDK_ERROR_INVALID_PORT                           = -507,
        BS_SDK_ERROR_INVALID_INTERPHONE_TYPE                = -508,
        BS_SDK_ERROR_INVALID_LCD_PARAM                      = -510,
        BS_SDK_ERROR_CANNOT_WRITE_LCD_PACKET                = -511,
        BS_SDK_ERROR_CANNOT_READ_LCD_PACKET                 = -512,
        BS_SDK_ERROR_INVALID_LCD_PACKET                     = -513,
        BS_SDK_ERROR_INPUT_QUEUE_FULL                       = -520,
        BS_SDK_ERROR_WIEGAND_QUEUE_FULL                     = -521,
        BS_SDK_ERROR_MISC_INPUT_QUEUE_FULL                  = -522,
        BS_SDK_ERROR_WIEGAND_DATA_QUEUE_FULL                = -523,
        BS_SDK_ERROR_WIEGAND_DATA_QUEUE_EMPTY               = -524,

        //Util errors
        BS_SDK_ERROR_NOT_SUPPORTED                          = -600,
        BS_SDK_ERROR_TIMEOUT                                = -601,
        BS_SDK_ERROR_CANNOT_SET_TIME                        = -602,

        //Database errors
        BS_SDK_ERROR_INVALID_DATA_FILE                      = -700,
        BS_SDK_ERROR_TOO_LARGE_DATA_FOR_SLOT                = -701,
        BS_SDK_ERROR_INVALID_SLOT_NO                        = -702,
        BS_SDK_ERROR_INVALID_SLOT_DATA						= -703,
        BS_SDK_ERROR_CANNOT_INIT_DB                         = -704,
        BS_SDK_ERROR_DUPLICATE_ID                           = -705,
        BS_SDK_ERROR_USER_FULL                              = -706,
        BS_SDK_ERROR_DUPLICATE_TEMPLATE                     = -707,
        BS_SDK_ERROR_FINGERPRINT_FULL                       = -708,
        BS_SDK_ERROR_DUPLICATE_CARD                         = -709,
        BS_SDK_ERROR_CARD_FULL                              = -710,
        BS_SDK_ERROR_NO_VALID_HDR_FILE                      = -711,
        BS_SDK_ERROR_INVALID_LOG_FILE						= -712,
        BS_SDK_ERROR_CANNOT_FIND_USER                       = -714,
        BS_SDK_ERROR_ACCESS_LEVEL_FULL                      = -715,
        BS_SDK_ERROR_INVALID_USER_ID                        = -716,
        BS_SDK_ERROR_BLACKLIST_FULL                         = -717,
        BS_SDK_ERROR_USER_NAME_FULL                         = -718,
        BS_SDK_ERROR_USER_IMAGE_FULL                        = -719,
        BS_SDK_ERROR_USER_IMAGE_SIZE_TOO_BIG                = -720,
        BS_SDK_ERROR_SLOT_DATA_CHECKSUM                     = -721,
        BS_SDK_ERROR_CANNOT_UPDATE_FINGERPRINT              = -722,
        BS_SDK_ERROR_TEMPLATE_FORMAT_MISMATCH               = -723,
        BS_SDK_ERROR_NO_ADMIN_USER                          = -724,
        BS_SDK_ERROR_CANNOT_FIND_LOG                        = -725,
        BS_SDK_ERROR_DOOR_SCHEDULE_FULL                     = -726,
        BS_SDK_ERROR_DB_SLOT_FULL                           = -727,
        BS_SDK_ERROR_ACCESS_GROUP_FULL                      = -728,
        BS_SDK_ERROR_FLOOR_LEVEL_FULL                       = -729,
        BS_SDK_ERROR_ACCESS_SCHEDULE_FULL                   = -730,
        BS_SDK_ERROR_HOLIDAY_GROUP_FULL                     = -731,
        BS_SDK_ERROR_HOLIDAY_FULL                           = -732,
        BS_SDK_ERROR_TIME_PERIOD_FULL                       = -733,
        BS_SDK_ERROR_NO_CREDENTIAL                          = -734,
        BS_SDK_ERROR_NO_BIOMETRIC_CREDENTIAL                = -735,
        BS_SDK_ERROR_NO_CARD_CREDENTIAL                     = -736,
        BS_SDK_ERROR_NO_PIN_CREDENTIAL                      = -737,
        BS_SDK_ERROR_NO_BIOMETRIC_PIN_CREDENTIAL            = -738,
        BS_SDK_ERROR_NO_USER_NAME                           = -739,
        BS_SDK_ERROR_NO_USER_IMAGE                          = -740,
        BS_SDK_ERROR_READER_FULL                            = -741,
        BS_SDK_ERROR_CACHE_MISSED                           = -742,
        BS_SDK_ERROR_OPERATOR_FULL                          = -743,
        BS_SDK_ERROR_INVALID_LINK_ID                        = -744,
        BS_SDK_ERROR_TIMER_CANCELED                         = -745,
        BS_SDK_ERROR_USER_JOB_FULL                          = -746,
        BS_SDK_ERROR_CANNOT_UPDATE_FACE                     = -747,
        BS_SDK_ERROR_FACE_FULL                              = -748,
        BS_SDK_ERROR_FLOOR_SCHEDULE_FULL                    = -749,
        BS_SDK_ERROR_CANNOT_FIND_AUTH_GROUP                 = -750,
        BS_SDK_ERROR_AUTH_GROUP_FULL                        = -751,
        BS_SDK_ERROR_USER_PHRASE_FULL                       = -752,
        BS_SDK_ERROR_DST_SCHEDULE_FULL                      = -753,
        BS_SDK_ERROR_CANNOT_FIND_DST_SCHEDULE               = -754,
        BS_SDK_ERROR_INVALID_SCHEDULE                       = -755,
        BS_SDK_ERROR_CANNOT_FIND_OPERATOR                   = -756,
        BS_SDK_ERROR_DUPLICATE_FINGERPRINT                  = -757,
        BS_SDK_ERROR_DUPLICATE_FACE                         = -758,
        BS_SDK_ERROR_NO_FACE_CREDENTIAL                     = -759,
        BS_SDK_ERROR_NO_FINGERPRINT_CREDENTIAL              = -760,
        BS_SDK_ERROR_NO_FACE_PIN_CREDENTIAL                 = -761,
        BS_SDK_ERROR_NO_FINGERPRINT_PIN_CREDENTIAL          = -762,
        BS_SDK_ERROR_USER_IMAGE_EX_FULL                     = -763,

        //Config errors
        BS_SDK_ERROR_INVALID_CONFIG                         = -800,
        BS_SDK_ERROR_CANNOT_OPEN_CONFIG_FILE                = -801,
        BS_SDK_ERROR_CANNOT_READ_CONFIG_FILE                = -802,
        BS_SDK_ERROR_INVALID_CONFIG_FILE                    = -803,
        BS_SDK_ERROR_INVALID_CONFIG_DATA                    = -804,
        BS_SDK_ERROR_CANNOT_WRITE_CONFIG_FILE               = -805,
        BS_SDK_ERROR_INVALID_CONFIG_INDEX                   = -806,

        //Device errors
        BS_SDK_ERROR_CANNOT_SCAN_FINGER                     = -900,
        BS_SDK_ERROR_CANNOT_SCAN_CARD                       = -901,
        BS_SDK_ERROR_CANNOT_OPEN_RTC                        = -902,
        BS_SDK_ERROR_CANNOT_SET_RTC                         = -903,
        BS_SDK_ERROR_CANNOT_GET_RTC                         = -904,
        BS_SDK_ERROR_CANNOT_SET_LED                         = -905,
        BS_SDK_ERROR_CANNOT_OPEN_DEVICE_DRIVER              = -906,
        BS_SDK_ERROR_CANNOT_FIND_DEVICE                     = -907,
        BS_SDK_ERROR_CANNOT_SCAN_FACE                       = -908,
        BS_SDK_ERROR_SLAVE_FULL                             = -910,
        BS_SDK_ERROR_CANNOT_ADD_DEVICE                      = -911,

        //Door errors
        BS_SDK_ERROR_CANNOT_FIND_DOOR                       = -1000,
        BS_SDK_ERROR_DOOR_FULL                              = -1001,
        BS_SDK_ERROR_CANNOT_LOCK_DOOR                       = -1002,
        BS_SDK_ERROR_CANNOT_UNLOCK_DOOR                     = -1003,
        BS_SDK_ERROR_CANNOT_RELEASE_DOOR                    = -1004,
        BS_SDK_ERROR_CANNOT_FIND_LIFT                       = -1005,
        BS_SDK_ERROR_LIFT_FULL                              = -1006,

        //Access control errors
        BS_SDK_ERROR_ACCESS_RULE_VIOLATION                  = -1100,
        BS_SDK_ERROR_DISABLED                               = -1101,
        BS_SDK_ERROR_NOT_YET_VALID                          = -1102,
        BS_SDK_ERROR_EXPIRED                                = -1103,
        BS_SDK_ERROR_BLACKLIST                              = -1104,
        BS_SDK_ERROR_CANNOT_FIND_ACCESS_GROUP               = -1105,
        BS_SDK_ERROR_CANNOT_FIND_ACCESS_LEVEL               = -1106,
        BS_SDK_ERROR_CANNOT_FIND_ACCESS_SCHEDULE            = -1107,
        BS_SDK_ERROR_CANNOT_FIND_HOLIDAY_GROUP              = -1108,
        BS_SDK_ERROR_CANNOT_FIND_BLACKLIST                  = -1109,
        BS_SDK_ERROR_AUTH_TIMEOUT                           = -1110,
        BS_SDK_ERROR_DUAL_AUTH_TIMEOUT                      = -1111,
        BS_SDK_ERROR_INVALID_AUTH_MODE                      = -1112,
        BS_SDK_ERROR_AUTH_UNEXPECTED_USER                   = -1113,
        BS_SDK_ERROR_AUTH_UNEXPECTED_CREDENTIAL             = -1114,
        BS_SDK_ERROR_DUAL_AUTH_FAIL                         = -1115,
        BS_SDK_ERROR_BIOMETRIC_AUTH_REQUIRED                = -1116,
        BS_SDK_ERROR_CARD_AUTH_REQUIRED                     = -1117,
        BS_SDK_ERROR_PIN_AUTH_REQUIRED                      = -1118,
        BS_SDK_ERROR_BIOMETRIC_OR_PIN_AUTH_REQUIRED         = -1119,
        BS_SDK_ERROR_TNA_CODE_REQUIRED                      = -1120,
        BS_SDK_ERROR_AUTH_SERVER_MATCH_REFUSAL              = -1121,
        BS_SDK_ERROR_CANNOT_FIND_FLOOR_LEVEL                = -1122,
        BS_SDK_ERROR_AUTH_FAIL                              = -1123,
        BS_SDK_ERROR_AUTH_GROUP_REQUIRED                    = -1124,
        BS_SDK_ERROR_IDENTIFICATION_REQUIRED                = -1125,
        BS_SDK_ERROR_ANTI_TAILGATE_VIOLATION                = -1126,
        BS_SDK_ERROR_HIGH_TEMPERATURE_VIOLATION             = -1127,
        BS_SDK_ERROR_CANNOT_MEASURE_TEMPERATURE             = -1128,
        BS_SDK_ERROR_UNMASKED_FACE_VIOLATION                = -1129,

        // Required (Fingerprint/Face/PIN/Mask/Thermal ...)
        BS_SDK_MASK_CHECK_REQUIRED                          = -1130,
        BS_SDK_THERMAL_CHECK_REQUIRED                       = -1131,
        BS_SDK_FACE_AUTH_REQUIRED                           = -1132,
        BS_SDK_FINGERPRINT_AUTH_REQUIRED                    = -1133,
        BS_SDK_FACE_OR_PIN_AUTH_REQUIRED                    = -1134,
        BS_SDK_FINGERPRINT_OR_PIN_AUTH_REQUIRED             = -1135,

        //Zone errors
        BS_SDK_ERROR_CANNOT_FIND_ZONE                       = -1200,
        [Obsolete] BS_SDK_ERROR_ZONE_FULL                   = -1201,
        BS_SDK_ERROR_SET_ZONE                               = -1201,
        BS_SDK_ERROR_HARD_APB_VIOLATION                     = -1202,
        BS_SDK_ERROR_SOFT_APB_VIOLATION                     = -1203,
        BS_SDK_ERROR_HARD_TIMED_APB_VIOLATION               = -1204,
        BS_SDK_ERROR_SOFT_TIMED_APB_VIOLATION               = -1205,
        BS_SDK_ERROR_SCHEDULED_LOCK_VIOLATION               = -1206,
        [Obsolete] BS_SDK_ERROR_SCHEDULED_UNLOCK_VIOLATION  = -1207,
        BS_SDK_ERROR_INTRUSION_ALARM_VIOLATION              = -1207,
        [Obsolete] BS_SDK_ERROR_SET_FIRE_ALARM              = -1208,
        BS_SDK_ERROR_APB_ZONE_FULL                          = -1208,
        BS_SDK_ERROR_TIMED_APB_ZONE_FULL                    = -1209,
        BS_SDK_ERROR_FIRE_ALARM_ZONE_FULL                   = -1210,
        BS_SDK_ERROR_SCHEDULED_LOCK_UNLOCK_ZONE_FULL        = -1211,
        BS_SDK_ERROR_INACTIVE_ZONE                          = -1212,
        BS_SDK_ERROR_INTRUSION_ALARM_ZONE_FULL              = -1213,
        BS_SDK_ERROR_CANNOT_ARM                             = -1214,
        BS_SDK_ERROR_CANNOT_DISARM                          = -1215,
        BS_SDK_ERROR_CANNOT_FIND_ARM_CARD                   = -1216,
        BS_SDK_ERROR_HARD_ENTRANCE_LIMIT_COUNT_VIOLATION    = -1217,
        BS_SDK_ERROR_SOFT_ENTRANCE_LIMIT_COUNT_VIOLATION    = -1218,
        BS_SDK_ERROR_HARD_ENTRANCE_LIMIT_TIME_VIOLATION     = -1219,
        BS_SDK_ERROR_SOFT_ENTRANCE_LIMIT_TIME_VIOLATION     = -1220,
        BS_SDK_ERROR_INTERLOCK_ZONE_DOOR_VIOLATION          = -1221,
        BS_SDK_ERROR_INTERLOCK_ZONE_INPUT_VIOLATION         = -1222,
        BS_SDK_ERROR_INTERLOCK_ZONE_FULL                    = -1223,
        BS_SDK_ERROR_AUTH_LIMIT_SCHEDULE_VIOLATION          = -1224,
        BS_SDK_ERROR_AUTH_LIMIT_COUNT_VIOLATION             = -1225,
        BS_SDK_ERROR_AUTH_LIMIT_USER_VIOLATION              = -1226,
        BS_SDK_ERROR_SOFT_AUTH_LIMIT_VIOLATION              = -1227,
        BS_SDK_ERROR_HARD_AUTH_LIMIT_VIOLATION              = -1228,

        BS_SDK_ERROR_LIFT_LOCK_UNLOCK_ZONE_FULL             = -1229,
        BS_SDK_ERROR_LIFT_LOCK_VIOLATION                    = -1230,
        //Card errors
        BS_SDK_ERROR_CARD_IO                                = -1300,
        BS_SDK_ERROR_CARD_INIT_FAIL                         = -1301,
        BS_SDK_ERROR_CARD_NOT_ACTIVATED                     = -1302,
        BS_SDK_ERROR_CARD_CANNOT_READ_DATA                  = -1303,
        BS_SDK_ERROR_CARD_CIS_CRC                           = -1304,
        BS_SDK_ERROR_CARD_CANNOT_WRITE_DATA                 = -1305,
        BS_SDK_ERROR_CARD_READ_TIMEOUT                      = -1306,
        BS_SDK_ERROR_CARD_READ_CANCELLED                    = -1307,
        BS_SDK_ERROR_CARD_CANNOT_SEND_DATA                  = -1308,
        BS_SDK_ERROR_CANNOT_FIND_CARD                       = -1310,
        
        // Operation
        BS_SDK_ERROR_INVALID_PASSWORD                       = -1400,
        
        // System
        BS_SDK_ERROR_CAMERA_INIT_FAIL                       = -1500,
        BS_SDK_ERROR_JPEG_ENCODER_INIT_FAIL                 = -1501,
        BS_SDK_ERROR_CANNOT_ENCODE_JPEG                     = -1502,
        BS_SDK_ERROR_JPEG_ENCODER_NOT_INITIALIZED           = -1503,
        BS_SDK_ERROR_JPEG_ENCODER_DEINIT_FAIL               = -1504,
        BS_SDK_ERROR_CAMERA_CAPTURE_FAIL                    = -1505,
        BS_SDK_ERROR_CANNOT_DETECT_FACE                     = -1506,
        
        //ETC.
        BS_SDK_ERROR_FILE_IO                                = -2000,
        BS_SDK_ERROR_ALLOC_MEM                              = -2002,
        BS_SDK_ERROR_CANNOT_UPGRADE                         = -2003,
        BS_SDK_ERROR_DEVICE_LOCKED                          = -2004,
        BS_SDK_ERROR_CANNOT_SEND_TO_SERVER                  = -2005,
        BS_SDK_ERROR_CANNOT_UPGRADE_MEMORY                  = -2006,
        BS_SDK_ERROR_UPGRADE_NOT_SUPPORTED                  = -2007,

        //SSL
        BS_SDK_ERROR_SSL_INIT                               = -3000,
        BS_SDK_ERROR_SSL_NOT_SUPPORTED                      = -3001,
        BS_SDK_ERROR_SSL_CANNOT_CONNECT                     = -3002,
        BS_SDK_ERROR_SSL_ALREADY_CONNECTED                  = -3003,
        BS_SDK_ERROR_SSL_INVALID_CERT                       = -3004,
        BS_SDK_ERROR_SSL_VERIFY_CERT                        = -3005,
        BS_SDK_ERROR_SSL_INVALID_KEY                        = -3006,
        BS_SDK_ERROR_SSL_VERIFY_KEY                         = -3007,

        // Mobile access
        BS_SDK_ERROR_MOBILE_PORTAL                          = -3100,

        // OSDP
        BS_SDK_ERROR_NOT_OSDP_STANDARD_CHANNEL              = -4001,
        BS_SDK_ERROR_ALREADY_FULL_SLAVES                    = -4002,
        BS_SDK_ERROR_DUPLICATE_OSDP_ID                      = -4003,
        BS_SDK_ERROR_FAIL_ADD_OSDP_DEVICE                   = -4004,
        BS_SDK_ERROR_FAIL_UPDATE_OSDP_DEVICE                = -4005,
        BS_SDK_ERROR_INVALID_OSDP_DEVICE_ID                 = -4006,
        BS_SDK_ERROR_FAIL_MASTER_SET_KEY                    = -4007,
        BS_SDK_ERROR_FAIL_SLAVE_SET_KEY                     = -4008,
        BS_SDK_ERROR_DISCONNECT_SLAVE_DEVICE                = -4009,

        // license
        BS_SDK_ERROR_NO_LICENSE                             = -4010,
        BS_SDK_ERROR_LICENSE_CRC                            = -4011,
        BS_SDK_ERROR_LICENSE_FILE_NOT_VALID                 = -4012,
        BS_SDK_ERROR_LICENSE_PAYLOAD_LENGTH                 = -4013,
        BS_SDK_ERROR_LICENSE_PARRING_JSON                   = -4014,
        BS_SDK_ERROR_LICENSE_JSON_FORMAT                    = -4015,
        BS_SDK_ERROR_LICENSE_ENABLE_PARTIAL                 = -4016,
        BS_SDK_ERROR_LICENSE_NO_MATCH_DEVICE                = -4017,

        BS_SDK_ERROR_NULL_POINTER                           = -10000,
        BS_SDK_ERROR_UNINITIALIZED                          = -10001,
        BS_SDK_ERROR_CANNOT_RUN_SERVICE                     = -10002,
        BS_SDK_ERROR_CANCELED                               = -10003,
        BS_SDK_ERROR_EXIST                                  = -10004,
        BS_SDK_ERROR_ENCRYPT                                = -10005,
        BS_SDK_ERROR_DECRYPT                                = -10006,
        BS_SDK_ERROR_DEVICE_BUSY							= -10007,
        BS_SDK_ERROR_INTERNAL                               = -10008,
        BS_SDK_ERROR_INVALID_FILE_FORMAT                    = -10009,
        BS_SDK_ERROR_INVALID_SCHEDULE_ID                    = -10010,
        BS_SDK_ERROR_UNKNOWN_FINGER_TEMPLATE                = -10011,
    }

    [Flags]
    public enum BS2ConnectionModeEnum
    {
        SERVER_TO_DEVICE = 0,
        DEVICE_TO_SERVER = 1
    }

    [Flags]
    public enum BS2RS485ModeEnum
    {
        DISABLED = 0,
        MASTER = 1,
        SLAVE = 2,
        STANDALONE = 3,
    }

    [Flags]
    public enum BS2CardTypeEnum
    {
        UNKNOWN = 0x0,
        CSN = 0x01,
        SECURE = 0x02,
        ACCESS = 0x03,

        CSN_MOBILE = 0x04,
        WIEGAND_MOBILE = 0x05,
        QR = 0x06,
        SECURE_QR = 0x07,

        WIEGAND = 0x0A,
        CONFIG_CARD = 0x0B,
    	CUSTOM_SMART= 0x0D,
    }

    [Flags]
    public enum BS2CardDataTypeEnum
    {
        BINARY = 0,
        ASCII = 1,
        UTF16 = 2,
        BCD = 3,
    }

    [Flags]
    public enum BS2CardByteOrderEnum
    {
        MSB = 0,
        LSB = 1,
    }

    [Flags]
    public enum BS2CardModelEnum
    {
        OMPW = 0, // mifare card
        OIPW = 1, // iclass card
        OEPW = 2, // em card
        OHPW = 3, // hid card
        ODPW = 4, // mifare AND em card
        OAPW = 5, // ALL card
        ODSPW = 6,  // SAM socket model

        PMPW = 20,
        PIPW = 21,
        PEPW = 22,
        PHPW = 23,
        PDPW = 24,
        PAPW = 25,
        PDSPW = 26,

        ALL = 30,	// All Binary Model
    }

    [Flags]
    public enum BS2UserFlagEnum
    {
        NONE = 0x00,		///< Same as server
        CREATED = 0x01,		///< User is created on device
        UPDATED = 0x02,		///< User is updated on device
        DELETED = 0x04,		///< User is deleted from device (not used yet)
        DISABLED = 0x80,
    }

    [Flags]
    public enum BS2UserOperatorEnum
    {
        NONE = 0,
        ADMIN = 1,
        CONFIG = 2,
        USER = 3,
    }

    [Flags]
    public enum BS2UserSecurityLevelEnum
    {
        DEFAULT = 0,
        LOWER = 1,
        LOW = 2,
        NORMAL = 3,
        HIGH = 4,
        HIGHER = 5,
    }

    [Flags]
    public enum BS2FaceEnrollThreshold
    {
        THRESHOLD_0 = 0,
        THRESHOLD_1 = 1,
        THRESHOLD_2 = 2,
        THRESHOLD_3 = 3,
        THRESHOLD_4 = 4,
        THRESHOLD_5 = 5,
        THRESHOLD_6 = 6,
        THRESHOLD_7 = 7,
        THRESHOLD_8 = 8,
        THRESHOLD_9 = 9,

    	THRESHOLD_DEFAULT = THRESHOLD_4,
    }

    [Flags]
    public enum BS2FingerAuthModeEnum
    {
        NONE = 255,	///< Authentication mode is not defined
        PROHIBITED = 254,	///< Authentication mode is prohibited

        BIOMETRIC_ONLY = 0,
        BIOMETRIC_PIN = 1,
        NUM_OF_BIOMETRIC_AUTH_MODE = 2,
    }

    [Flags]
    public enum BS2FaceAuthModeEnum
    {
        NONE = 255,	///< Authentication mode is not defined
        PROHIBITED = 254,	///< Authentication mode is prohibited

        BIOMETRIC_ONLY = BS2FingerAuthModeEnum.BIOMETRIC_ONLY,
        BIOMETRIC_PIN = BS2FingerAuthModeEnum.BIOMETRIC_PIN,
        NUM_OF_BIOMETRIC_AUTH_MODE = BS2FingerAuthModeEnum.NUM_OF_BIOMETRIC_AUTH_MODE,
    }

    [Flags]
    public enum BS2CardAuthModeEnum
    {
        NONE = 255,	///< Authentication mode is not defined
        PROHIBITED = 254,	///< Authentication mode is prohibited

        CARD_ONLY = BS2FaceAuthModeEnum.NUM_OF_BIOMETRIC_AUTH_MODE,     // 2
        CARD_BIOMETRIC = 3,
        CARD_PIN = 4,
        CARD_BIOMETRIC_OR_PIN = 5,
        CARD_BIOMETRIC_PIN = 6,
        NUM_OF_CARD_AUTH_MODE = 7,
    }

    [Flags]
    public enum BS2IDAuthModeEnum
    {
        NONE = 255,	///< Authentication mode is not defined
        PROHIBITED = 254,	///< Authentication mode is prohibited

        ID_BIOMETRIC = BS2CardAuthModeEnum.NUM_OF_CARD_AUTH_MODE,       // 7
        ID_PIN = 8,
        ID_BIOMETRIC_OR_PIN = 9,
        ID_BIOMETRIC_PIN = 10,
        NUM_OF_ID_AUTH_MODE = 11,
    }

    // F2 support
    [Flags]
    public enum BS2ExtFaceAuthModeEnum
    {
        NONE = 255,	///< Authentication mode is not defined
        PROHIBITED = 254,	///< Authentication mode is prohibited

        EXT_FACE_ONLY = BS2IDAuthModeEnum.NUM_OF_ID_AUTH_MODE,    // 11
	    EXT_FACE_FINGERPRINT = 12,
	    EXT_FACE_PIN = 13,
	    EXT_FACE_FINGERPRINT_OR_PIN = 14,
	    EXT_FACE_FINGERPRINT_PIN = 15,
        NUM_OF_EXT_FACE_AUTH_MODE = 16,
    }

    [Flags]
    public enum BS2ExtFingerprintAuthModeEnum
    {
        NONE = 255,	///< Authentication mode is not defined
        PROHIBITED = 254,	///< Authentication mode is prohibited

        EXT_FINGERPRINT_ONLY = BS2ExtFaceAuthModeEnum.NUM_OF_EXT_FACE_AUTH_MODE,        // 16
	    EXT_FINGERPRINT_FACE = 17,
	    EXT_FINGERPRINT_PIN = 18,
	    EXT_FINGERPRINT_FACE_OR_PIN = 19,
	    EXT_FINGERPRINT_FACE_PIN = 20,
        NUM_OF_EXT_FINGERPRINT_AUTH_MODE = 21,
    }

    [Flags]
    public enum BS2ExtCardAuthModeEnum
    {
        NONE = 255,	///< Authentication mode is not defined
        PROHIBITED = 254,	///< Authentication mode is prohibited

        EXT_CARD_ONLY = BS2ExtFingerprintAuthModeEnum.NUM_OF_EXT_FINGERPRINT_AUTH_MODE,     // 21
	    EXT_CARD_FACE = 22,
	    EXT_CARD_FINGERPRINT = 23,
	    EXT_CARD_PIN = 24,
	    EXT_CARD_FACE_OR_FINGERPRINT = 25,
	    EXT_CARD_FACE_OR_PIN = 26,
	    EXT_CARD_FINGERPRINT_OR_PIN = 27,
	    EXT_CARD_FACE_OR_FINGERPRINT_OR_PIN = 28,
	    EXT_CARD_FACE_FINGERPRINT = 29,
	    EXT_CARD_FACE_PIN = 30,
	    EXT_CARD_FINGERPRINT_FACE = 31,
	    EXT_CARD_FINGERPRINT_PIN = 32,
	    EXT_CARD_FACE_OR_FINGERPRINT_PIN = 33,
	    EXT_CARD_FACE_FINGERPRINT_OR_PIN = 34,
	    EXT_CARD_FINGERPRINT_FACE_OR_PIN = 35,
        NUM_OF_EXT_CARD_AUTH_MODE = 36,
    }

    [Flags]
    public enum BS2ExtIDAuthModeEnum
    {
        NONE = 255,	///< Authentication mode is not defined
        PROHIBITED = 254,	///< Authentication mode is prohibited

        EXT_ID_FACE = BS2ExtCardAuthModeEnum.NUM_OF_EXT_CARD_AUTH_MODE,     // 36
        EXT_ID_FINGERPRINT = 37,
        EXT_ID_PIN = 38,
        EXT_ID_FACE_OR_FINGERPRINT = 39,
        EXT_ID_FACE_OR_PIN = 40,
        EXT_ID_FINGERPRINT_OR_PIN = 41,
        EXT_ID_FACE_OR_FINGERPRINT_OR_PIN = 42,
        EXT_ID_FACE_FINGERPRINT = 43,
        EXT_ID_FACE_PIN = 44,
        EXT_ID_FINGERPRINT_FACE = 45,
        EXT_ID_FINGERPRINT_PIN = 46,
        EXT_ID_FACE_OR_FINGERPRINT_PIN = 47,
        EXT_ID_FACE_FINGERPRINT_OR_PIN = 48,
        EXT_ID_FINGERPRINT_FACE_OR_PIN = 49,
        NUM_OF_EXT_ID_AUTH_MODE = 50,
    }

    [Flags]
    public enum BS2FingerprintFlagEnum
    {
        NORMAL = 0,
        DURESS = 1,
    }

    [Flags]
    public enum BS2FingerprintQualityEnum
    {
        QUALITY_LOW = 20,
        QUALITY_STANDARD = 40,
        QUALITY_HIGH = 60,
        QUALITY_HIGHEST = 80,
    }

    [Flags]
    public enum BS2FingerprintSecurityEnum
    {
        NORMAL = 0,
        HIGH = 1,
        HIGHEST = 2,
    }

    [Flags]
    public enum BS2FingerprintSensitivityEnum
    {
        SENSITIVE0 = 0,
        SENSITIVE1 = 1,
        SENSITIVE2 = 2,
        SENSITIVE3 = 3,
        SENSITIVE4 = 4,
        SENSITIVE5 = 5,
        SENSITIVE6 = 6,
        SENSITIVE7 = 7,
    }

    [Flags]
    public enum BS2FingerprintFastModeEnum
    {
        AUTO = 0,
        NORMAL = 1,
        FASTER = 2,
        FASTEST = 3,
    }

    [Flags]
    public enum BS2FingerprintTemplateFormatEnum
    {
        FORMAT_SUPREMA = 0,
        FORMAT_ISO = 1,
        FORMAT_ANSI = 2
    }

    [Flags]
    public enum BS2FingerprintSensorModeEnum
    {
        ALWAYS_ON = 0,
        PROXIMITY = 1,
    }

    [Flags]
    public enum BS2DeviceTypeEnum
    {
        UNKNOWN         = 0x00,

        BIOENTRY_PLUS   = 0x01,
        BIOENTRY_W      = 0x02,
        BIOLITE_NET     = 0x03,
        XPASS           = 0x04,
        XPASS_S2        = 0x05,
        SECURE_IO_2     = 0x06,
        DOOR_MODULE_20  = 0x07,
        BIOSTATION_2    = 0x08,
        BIOSTATION_A2   = 0x09,
        FACESTATION_2   = 0x0A,
        IO_DEVICE       = 0x0B,
        BIOSTATION_L2   = 0x0C,
        BIOENTRY_W2     = 0x0D,
        //CORE_STATION  = 0x0E,		// Deprecated 2.6.0
        CORESTATION_40  = 0x0E,
        OUTPUT_MODULE   = 0x0F,
        INPUT_MODULE    = 0x10,
        BIOENTRY_P2     = 0x11,
        BIOLITE_N2      = 0x12,
        XPASS2          = 0x13,
        XPASS_S3        = 0x14,
        BIOENTRY_R2     = 0x15,
        XPASS_D2        = 0x16,
        DOOR_MODULE_21  = 0x17,
        XPASS_D2_KEYPAD = 0x18,
        FACELITE        = 0x19,
        XPASS2_KEYPAD   = 0x1A,
        XPASS_D2_REV    = 0x1B,     // [+2.7]
        XPASS_D2_KEYPAD_REV = 0x1C, // [+2.7]
        FACESTATION_F2_FP = 0x1D,   // FSF2 support
        FACESTATION_F2  = 0x1E,     // FSF2 support
        XSTATION_2_QR   = 0x1F,     // [+2.8]
        XSTATION_2      = 0x20,     // [+2.8]
        IM_120          = 0x21,     // [+2.8.1]
        XSTATION_2_FP   = 0x22,     // [+2.8.1]
        BIOSTATION_3    = 0x23,     // [+2.8.3]
        THIRD_OSDP_DEVICE = 0x24,   // [+2.9.1]
        THIRD_OSDP_IO_DEVICE = 0x25,   // [+2.9.1]
        BIOSTATION_2A   = 0x26,     // [+2.9.4]
        BIOENTRY_W3     = 0x2A,     // [+2.9.6]


        TYPE_MAX        = BIOENTRY_W3,
        //UNKNOWN         = 0xFF,
    }

    [Flags]
    public enum BS2TCPBasebandEnum
    {
        BASE_10MB = 0,
        BASE_100MB = 1
    }

    [Flags]
    public enum BS2LanguageEnum
    {
        KOREAN = 0,
        ENGLISH = 1,
        CUSTOM = 2,
    }

    [Flags]
    public enum BS2BackgroundEnum
    {
        LOGO = 0,
        NOTICE = 1,
        SLIDE = 2,
        PDF = 3,
    }

    [Flags]
    public enum BS2BGThemeEnum
    {
        THEME1 = 0,
        THEME2 = 1,
        THEME3 = 2,
        THEME4 = 3,
    }

    [Flags]
    public enum BS2DateFormatEnum
    {
        YYYYMMDD = 0,
        MMDDYYYY = 1,
        DDMMYYYY = 2,
    }

    [Flags]
    public enum BS2TimeFormatEnum
    {
        HOUR12 = 0,
        HOUR24 = 1,
    }

    [Flags]
    public enum BS2HomeFormationEnum
    {
        INTERPHONE = 0,
        SHORTCUT1 = 1,
        SHORTCUT2 = 2,
        SHORTCUT3 = 3,
        SHORTCUT4 = 4,
    }

    [Flags]
    public enum BS2ShortCutHomeEnum
    {
        MENU = 0,
        TNA = 1,
        LANGUAGE = 2,
        ID = 3,
        FINGERPRINT = 4,
        INTERPHONE = 5,
        ARM = 6,
        VOLUME = 7,
    }

    [Flags]
    public enum BS2TNAModeEnum
    {
        UNUSED = 0,
        USER = 1,
        SCHEDULE = 2,
        LAST_CHOICE = 3,
        FIXED = 4,
    }

    [Flags]
    public enum BS2TNAKeyEnum
    {
        UNSPECIFIED = 0,
        KEY1 = 1,
        KEY2 = 2,
        KEY3 = 3,
        KEY4 = 4,
        KEY5 = 5,
        KEY6 = 6,
        KEY7 = 7,
        KEY8 = 8,
        KEY9 = 9,
        KEY10 = 10,
        KEY11 = 11,
        KEY12 = 12,
        KEY13 = 13,
        KEY14 = 14,
        KEY15 = 15,
        KEY16 = 16,
    }

    [Flags]
    public enum BS2WiegandInOutEnum
    {
        IN = 0,
        OUT = 1,
        INOUT = 2,      // FISSDK-147  missing I/O mode
    }

    [Flags]
    public enum BS2WiegandParityEnum
    {
        None = 0,
        Odd = 1,
        Even = 2,
    }

    [Flags]
    public enum BS2ScheduleIDEnum
    {
        NEVER = 0,
        ALWAYS = 1,
    }

    [Flags]
    public enum BS2SoundIndexEnum
    {
        WELCOME = 0,
        AUTH_SUCCESS = 1,
        AUTH_FAIL = 2,
        ALARM_1 = 3,    // FISSDK-95  Add missing sound index
        ALARM_2 = 4,    // FISSDK-95  Add missing sound index
    }

    [Flags]
    public enum BS2ResourceTypeEnum
    {
        BS2_RESOURCE_TYPE_UI = 0,
        BS2_RESOURCE_TYPE_NOTICE = 1,
        BS2_RESOURCE_TYPE_IMAGE = 2,
        BS2_RESOURCE_TYPE_SLIDE = 3,
        BS2_RESOURCE_TYPE_SOUND = 4,
    }

    [Flags]
    public enum BS2SwitchTypeEnum
    {
        NORMAL_OPEN = 0,
        NORMAL_CLOSE = 1
    }

    [Flags]
    public enum BS2DoorFlagEnum
    {
        NONE = 0,
        SCHEDULE = 1,
        EMERGENCY = 2,
        OPERATOR = 4,
	    ALL = 0xFF,
    }

    [Flags]
    public enum BS2DoorAlarmFlagEnum
    {
        NONE = 0,
        FORCED_OPEN = 1,
        HELD_OPEN = 2,
        APB = 4
    }

    [Flags]
    public enum BS2DualAuthDeviceEnum
    {
        NO_DEVICE = 0,
        ENTRY_DEVICE_ONLY = 1,
        EXIT_DEVICE_ONLY = 2,
        BOTH_DEVICE = 3
    }

    [Flags]
    public enum BS2LedColorEnum
    {
        OFF = 0,
        RED = 1,
        YELLOW = 2,
        GREEN = 3,
        CYAN = 4,
        BLUE = 5,
        MAGENTA = 6,
        WHITE = 7
    }

    [Flags]
    public enum BS2BuzzerToneEnum
    {
        OFF = 0,
        LOW = 1,
        MIDDLE = 2,
        HIGH = 3
    }

    [Flags]
    public enum BS2LiftActionTypeEnum
    {
        ACTIVATE_FLOORS = 0,
        DEACTIVATE_FLOORS = 1,
        RELEASE_FLOORS = 2,        
    }

    [Flags]
    public enum BS2ActionTypeEnum
    {
        NONE = 0,
        LOCK_DEVICE = 1,
        UNLOCK_DEVICE = 2,
        REBOOT_DEVICE = 3,
        RELEASE_ALARM = 4,
        GENERAL_INPUT = 5,
        RELAY = 6,
        TTL = 7,
        SOUND = 8,
        DISPLAY = 9,
        BUZZER = 10,
        LED = 11,
        FIRE_ALARM_INPUT = 12,
        AUTH_SUCCESS = 13,
        AUTH_FAIL = 14,
        LIFT = 15,
    }

    [Flags]
    public enum BS2TriggerTypeEnum
    {
        NONE = 0,
        EVENT = 1,
        INPUT = 2,
        SCHEDULE = 3,
    }

    [Flags]
    public enum BS2DualAuthApprovalEnum
    {
        NONE = 0,
        LAST = 1
    }

    [Flags]
    public enum BS2FaceDetectionLevelEnum
    {
        NONE = 0,
        NORMAL = 1,
        STRICT = 2
    }

    [Flags]
    public enum BS2GlobalAPBFailActionTypeEnum
    {
        NONE = 0,
        SOFT = 1,
        HARD = 2
    }

    [Flags]
    public enum BS2WlanOperationModeEnum
    {
        MANAGED = 0,
        ADHOC = 1
    }

    [Flags]
    public enum BS2WlanAuthTypeEnum
    {
        OPEN = 0,
        SHARED = 1,
        WPA_PSK = 2,
        WPA2_PSK = 3
    }

    [Flags]
    public enum BS2WlanEncryptionTypeEnum
    {
        NONE = 0,
        WEP = 1,
        TKIP_AES = 2,
        AES = 3,
        TKIP = 4,
    }

    [Flags]
    public enum BS2ZoneTypeEnum
    {
        APB = 0,
        TIMED_APB = 1,
        FIRE_ALARM = 2,
        SCHEDULED_LOCK_UNLOCK = 3,
    }

    [Flags]
    public enum BS2ZoneStatusEnum
    {
        NORMAL = 0,
        ALARM = 1,
        FORCED_LOCKED = 2,
        FORCED_UNLOCKED = 4,
        ARM = 8,
        DISARM = NORMAL,
    }

    [Flags]
    public enum BS2APBZoneTypeEnum
    {
        HARD = 0,
        SOFT = 1,
    }

    [Flags]
    public enum BS2APBZoneReaderTypeEnum
    {
        NONE = -1,
        ENTRY = 0,
        EXIT = 1,
    }

    [Flags]
    public enum BS2TimedAPBZoneTypeEnum
    {
        HARD = 0,
        SOFT = 1,
    }

    [Flags]
    public enum BS2SslMethodMaskEnum : uint
    {
        NONE = 0,
        SSL2 = 0x1,
        SSL3 = 0x2,
        TLS1 = 0x4,
        TLS1_1 = 0x8,
        TLS1_2 = 0x10,
        ALL = 0xFFFFFFFF,
    }

    [Flags]
    public enum BS2LiftAlarmFlagEnum
    {
        NONE = 0,
        FIRST = 0x1,
        SECOND = 0x2,
        TAMPER = 0x4,       
    }

    [Flags]
    public enum BS2FloorFlagEnum
    {
        NONE = 0,
        SCHEDULE = 0x1,
        EMERGENCY = 0x2,
        OPERATOR = 0x4,
        ACTION = 0x8,       // [+ 2.6.4]
    }

    [Flags]
    public enum BS2ConfigMaskEnum : uint
    {
        NONE = 0,
        FACTORY = 0x0001,                // Factory configuration
        SYSTEM = 0x0002,                 // System configuration
        IP = 0x0004,                     // TCP/IP configuration
        RS485 = 0x0008,                  // RS485 configuration
        WLAN = 0x0010,                   // Wireless LAN configuration
        AUTH = 0x0020,                   // Authentication configuration
        CARD = 0x0040,                   // Card configuration
        FINGERPRINT = 0x0080,            // Fingerprint configuration
        FACE = 0x0100,                   // Face configuration
        TRIGGER_ACTION = 0x0200,         // Trigger Action configuration
        DISPLAY = 0x0400,                // Display configuration
        SOUND = 0x0800,                  // Sound configuration
        STATUS = 0x1000,                 // Status Signal(LED, Buzzer) configuration
        WIEGAND = 0x2000,                // Wiegand configuration
        USB = 0x4000,                    // USB configuration
        TNA = 0x8000,                    // Time and Attendance configuration (@deprecated)
        VIDEOPHONE = 0x10000,            // Videophone configuration
        INTERPHONE = 0x20000,            // Interphone configuration
        VOIP = 0x40000,                  // Voice over IP configuration
        INPUT = 0x80000,                 // Input(Supervised input) configuration
        WIEGAND_IO = 0x100000,            // Wiegand IO Device configuration
        TNA_EXT = 0x200000,              // Time and Attendance configuration
        IP_EXT = 0x400000,               // DNS and Server url configuration
        EVENT = 0x800000,                // Event configuration
        CARD_1x = 0x1000000,             // Card_1x configuration
        WIEGAND_MULTI = 0x2000000,       // Wiegand Multi configuration
        SYSTEM_EXT = 0x4000000,       // Extended System configuration
        DST = 0x8000000,		      //< Daylight Saving configuration
        ALL = 0xFFFFFFFF,                // 4 bytes
    }

    [Flags]
    public enum BS2UserMaskEnum : uint
    {
        ID_ONLY = 0,                     // fill only user id in BS2User
        DATA = 0x0001,                   // BS2User
        SETTING = 0x0002,                // BS2UserSetting
        NAME = 0x0004,                   // BS2_USER_NAME
        PHOTO = 0x0008,                  // BS2UserPhoto
        PIN = 0x0010,                    // BS2_HASH256
        CARD = 0x0020,                   // BS2CSNCard
        FINGER = 0x0040,                 // BS2FingerTemplate
        FACE = 0x0080,                   // BS2FaceTemplate
        ACCESS_GROUP = 0x0100,           // BS2_ACCESS_GROUP_ID
        JOB = 0x0200,                    // BS2Job
        PHRASE = 0x0400,                 // BS2_USER_PHRASE
        FACE_EX = 0x0800,                // BS2FaceExWarped, BS2FaceExUnwarped
        SETTING_EX = 0x1000,             // BS2UserSettingEx
        ALL = 0xFFFF,                    // 4 bytes
    }

    [Flags]
    public enum BS2UserInfoMaskEnum      // [+V2.8.3]
    {
 	    PHRASE 	    = 0x01,
	    JOB_CODE    = 0x02,
	    NAME 	    = 0x04,
	    PHOTO 	    = 0x08,
	    PIN 		= 0x10,
	    CARD 	    = 0x20,
	    FINGER 	    = 0x40,
	    FACE 	    = 0x80,
    }

    [Flags]
    public enum BS2EventCodeEnum
    {
        ALL = 0x0000,
        MASK = 0xFF00,

        VERIFY_SUCCESS = 0x1000,
        VERIFY_SUCCESS_ID_PIN = 0x1001,
        VERIFY_SUCCESS_ID_FINGER = 0x1002,
        VERIFY_SUCCESS_ID_FINGER_PIN = 0x1003,
        VERIFY_SUCCESS_ID_FACE = 0x1004,
        VERIFY_SUCCESS_ID_FACE_PIN = 0x1005,
        VERIFY_SUCCESS_CARD = 0x1006,
        VERIFY_SUCCESS_CARD_PIN = 0x1007,
        VERIFY_SUCCESS_CARD_FINGER = 0x1008,
        VERIFY_SUCCESS_CARD_FINGER_PIN = 0x1009,
        VERIFY_SUCCESS_CARD_FACE = 0x100A,
        VERIFY_SUCCESS_CARD_FACE_PIN = 0x100B,
        VERIFY_SUCCESS_AOC = 0x100C,
        VERIFY_SUCCESS_AOC_PIN = 0x100D,
        VERIFY_SUCCESS_AOC_FINGER = 0x100E,
        VERIFY_SUCCESS_AOC_FINGER_PIN = 0x100F,
        VERIFY_SUCCESS_CARD_FACE_FINGER = 0x1010,
        VERIFY_SUCCESS_CARD_FINGER_FACE = 0x1011,
        VERIFY_SUCCESS_ID_FACE_FINGER = 0x1012,
        VERIFY_SUCCESS_ID_FINGER_FACE = 0x1013,
        VERIFY_SUCCESS_MOBLIE_CARD = 0x1016,
        VERIFY_SUCCESS_MOBILE_CARD_PIN = 0x1017,
        VERIFY_SUCCESS_MOBILE_CARD_FINGER = 0x1018,
        VERIFY_SUCCESS_MOBILE_CARD_FINGER_PIN = 0x1019,
        VERIFY_SUCCESS_MOBILE_CARD_FACE = 0x101A,
        VERIFY_SUCCESS_MOBILE_CARD_FACE_PIN = 0x101B,
        VERIFY_SUCCESS_MOBILE_CARD_FACE_FINGER = 0x1020,
        VERIFY_SUCCESS_MOBILE_CARD_FINGER_FACE = 0x1021,
        VERIFY_SUCCESS_QR = 0x1025,
        VERIFY_SUCCESS_QR_PIN = 0x1026,
        VERIFY_SUCCESS_QR_FINGER = 0x1027,
        VERIFY_SUCCESS_QR_FINGER_PIN = 0x1028,
        VERIFY_SUCCESS_QR_FACE = 0x1029,
        VERIFY_SUCCESS_QR_FACE_PIN = 0x102A,
        VERIFY_SUCCESS_QR_FACE_FINGER = 0x102B,
        VERIFY_SUCCESS_QR_FINGER_FACE = 0x102C,

        VERIFY_FAIL = 0x1100,
        VERIFY_FAIL_ID = 0x1101,
        VERIFY_FAIL_CARD = 0x1102,
        VERIFY_FAIL_PIN = 0x1103,
        VERIFY_FAIL_FINGER = 0x1104,
        VERIFY_FAIL_FACE = 0x1105,
        VERIFY_FAIL_AOC_PIN = 0x1106,
        VERIFY_FAIL_AOC_FINGER = 0x1107,
        VERIFY_FAIL_CREDENTIAL_MOBILE_CARD = 0x1108,
        VERIFY_FAIL_CREDENTIAL_QR = 0x110C,
        VERIFY_FAIL_NON_NUMERIC_QR = 0x1109,
        VERIFY_FAIL_NON_PRINTABLE_QR = 0x110A,
        VERIFY_FAIL_TOO_LONG_QR = 0x110B,

        VERIFY_DURESS = 0x1200,
        VERIFY_DURESS_ID_PIN = 0x1201,
        VERIFY_DURESS_ID_FINGER = 0x1202,
        VERIFY_DURESS_ID_FINGER_PIN = 0x1203,
        VERIFY_DURESS_ID_FACE = 0x1204,
        VERIFY_DURESS_ID_FACE_PIN = 0x1205,
        VERIFY_DURESS_CARD = 0x1206,
        VERIFY_DURESS_CARD_PIN = 0x1207,
        VERIFY_DURESS_CARD_FINGER = 0x1208,
        VERIFY_DURESS_CARD_FINGER_PIN = 0x1209,
        VERIFY_DURESS_CARD_FACCE = 0x120A,
        VERIFY_DURESS_CARD_FACE_PIN = 0x120B,
        VERIFY_DURESS_AOC = 0x120C,
        VERIFY_DURESS_AOC_PIN = 0x120D,
        VERIFY_DURESS_AOC_FINGER = 0x120E,
        VERIFY_DURESS_AOC_FINGER_PIN = 0x120F,
        VERIFY_DURESS_CARD_FACE_FINGER = 0x1210,
        VERIFY_DURESS_CARD_FINGER_FACE = 0x1211,
        VERIFY_DURESS_ID_FACE_FINGER = 0x1212,
        VERIFY_DURESS_ID_FINGER_FACE = 0x1213,
        VERIFY_DURESS_MOBLIE_CARD = 0x1216,
        VERIFY_DURESS_MOBILE_CARD_PIN = 0x1217,
        VERIFY_DURESS_MOBILE_CARD_FINGER = 0x1218,
        VERIFY_DURESS_MOBILE_CARD_FINGER_PIN = 0x1219,
        VERIFY_DURESS_MOBILE_CARD_FACE = 0x121A,
        VERIFY_DURESS_MOBILE_CARD_FACE_PIN = 0x121B,
        VERIFY_DURESS_MOBILE_CARD_FACE_FINGER = 0x1220,
        VERIFY_DURESS_MOBILE_CARD_FINGER_FACE = 0x1221,
        VERIFY_DURESS_QR = 0x1225,
        VERIFY_DURESS_QR_PIN = 0x1226,
        VERIFY_DURESS_QR_FINGER = 0x1227,
        VERIFY_DURESS_QR_FINGER_PIN = 0x1228,
        VERIFY_DURESS_QR_FACE = 0x1229,
        VERIFY_DURESS_QR_FACE_PIN = 0x122A,
        VERIFY_DURESS_QR_FACE_FINGER = 0x122B,
        VERIFY_DURESS_QR_FINGER_FACE = 0x122C,

        IDENTIFY_SUCCESS = 0x1300,
        IDENTIFY_SUCCESS_FINGER = 0x1301,
        IDENTIFY_SUCCESS_FINGER_PIN = 0x1302,
        IDENTIFY_SUCCESS_FACE = 0x1303,
        IDENTIFY_SUCCESS_FACE_PIN = 0x1304,
        IDENTIFY_SUCCESS_FACE_FINGER = 0x1305,
        IDENTIFY_SUCCESS_FACE_FINGER_PIN = 0x1306,
        IDENTIFY_SUCCESS_FINGER_FACE = 0x1307,
        IDENTIFY_SUCCESS_FINGER_FACE_PIN = 0x1308,

        IDENTIFY_FAIL = 0x1400,
        IDENTIFY_FAIL_ID = 0x1401,
        IDENTIFY_FAIL_CARD = 0x1402,
        IDENTIFY_FAIL_PIN = 0x1403,
        IDENTIFY_FAIL_FINGER = 0x1404,
        IDENTIFY_FAIL_FACE = 0x1405,
        IDENTIFY_FAIL_AOC_PIN = 0x1406,
        IDENTIFY_FAIL_AOC_FINGER = 0x1407,
        IDENTIFY_FAIL_CREDENTIAL_MOBILE_CARD = 0x1408,
        IDENTIFY_FAIL_CREDENTIAL_QR = 0x140C,
        IDENTIFY_FAIL_NON_NUMERIC_QR = 0x1409,
        IDENTIFY_FAIL_NON_PRINTABLE_QR = 0x140A,
        IDENTIFY_FAIL_TOO_LONG_QR = 0x140B,

        IDENTIFY_DURESS = 0x1500,
        IDENTIFY_DURESS_FINGER = 0x1501,
        IDENTIFY_DURESS_FINGER_PIN = 0x1502,
        IDENTIFY_DURESS_FACE = 0x1503,
        IDENTIFY_DURESS_FACE_PIN = 0x1504,
        IDENTIFY_DURESS_FACE_FINGER = 0x1505,
        IDENTIFY_DURESS_FACE_FINGER_PIN = 0x1506,
        IDENTIFY_DURESS_FINGER_FACE = 0x1507,
        IDENTIFY_DURESS_FINGER_FACE_PIN = 0x1508,

        DUAL_AUTH_SUCCESS = 0x1600,

        DUAL_AUTH_FAIL = 0x1700,
        DUAL_AUTH_FAIL_TIMEOUT = 0x1701,
        DUAL_AUTH_FAIL_ACCESS_GROUP = 0x1702,

        AUTH_FAILED = 0x1800,
        AUTH_FAILED_INVALID_AUTH_MODE = 0x1801,
        AUTH_FAILED_INVALID_CREDENTIAL = 0x1802,
        AUTH_FAILED_TIMEOUT = 0x1803,
        AUTH_FAILED_MATCHING_REFUSAL = 0x1804,

        ACCESS_DENIED = 0x1900,
        ACCESS_DENIED_ACCESS_GROUP = 0x1901,
        ACCESS_DENIED_DISABLED = 0x1902,
        ACCESS_DENIED_EXPIRED = 0x1903,
        ACCESS_DENIED_ON_BLACKLIST = 0x1904,
        ACCESS_DENIED_APB = 0x1905,
        ACCESS_DENIED_TIMED_APB = 0x1906,
        ACCESS_DENIED_SCHEDULED_LOCK = 0x1907,

        ACCESS_EXCUSED_APB = 0x1908,
        ACCESS_EXCUSED_TIMED_APB = 0x1909,

        ACCESS_DENIED_FACE_DETECTION = 0x190A,
        ACCESS_DENIED_CAMERA_CAPTURE = 0x190B,
        ACCESS_DENIED_FAKE_FINGER = 0x190C,
        ACCESS_DENIED_DEVICE_ZONE_ENTRANCE_LIMIT = 0x190D,
        ACCESS_DENIED_INTRUSION_ALARM = 0x190E,
        ACCESS_DENIED_INTERLOCK = 0x190F,
        ACCESS_EXCUSED_AUTH_LIMIT = 0x1910,         // [+V2.8.3]
        ACCESS_DENIED_AUTH_LIMIT = 0x1911,          // [+V2.8.3]
        ACCESS_DENIED_ANTI_TAILGATE = 0x1912,       // [+V2.8.3]
        ACCESS_DENIED_HIGH_TEMPERATURE = 0x1913,    // [+V2.8.3]
        ACCESS_DENIED_NO_TEMPERATURE = 0x1914,      // [+V2.8.3]
        ACCESS_DENIED_UNMASKED_FACE = 0x1915,       // [+V2.8.3]

        USER_ENROLL_SUCCESS = 0x2000,
        USER_ENROLL_FAIL = 0x2100,
        USER_ENROLL_FAIL_INVALID_FACE = 0x2101,
        USER_ENROLL_FAIL_MISMATCHED_FORMAT = 0x2102,       // [+V2.8.3]
        USER_ENROLL_FAIL_FULL_CREDENTIAL = 0x2103,         // [+V2.8.3]
        USER_ENROLL_FAIL_INVALID_USER = 0x2104,            // [+V2.8.3]
        USER_ENROLL_FAIL_INTERNAL_ERROR = 0x2109,          // [+V2.8.3]
        USER_UPDATE_SUCCESS = 0x2200,
        USER_UPDATE_FAIL = 0x2300,
        USER_DELETE_SUCCESS = 0x2400,
        USER_DELETE_FAIL = 0x2500,
        USER_DELETE_ALL_SUCCESS = 0x2600,
        USER_ISSUE_AOC_SUCCESS = 0x2700,
        USER_DUPLICATE_CREDENTIAL = 0x2800,
       	USER_UPDATE_PARTIAL_SUCCESS = 0x2900,               // [+V2.8.3]
	    USER_UPDATE_PARTIAL_FAIL = 0x2A00,                  // [+V2.8.3]
        USER_UPDATE_PARTIAL_FAIL_INVALID_FACE = 0x2A01,
        USER_UPDATE_PARTIAL_FAIL_MISMATCHED_FORMAT = 0x2A02,// [+V2.8.3]
        USER_UPDATE_PARTIAL_FAIL_FULL_CREDENTIAL = 0x2A03,  // [+V2.8.3]
        USER_UPDATE_PARTIAL_FAIL_INVALID_USER = 0x2A04,     // [+V2.8.3]
        USER_UPDATE_PARTIAL_FAIL_INTERNAL_ERROR = 0x2A09,   // [+V2.8.3]

        DEVICE_SYSTEM_RESET = 0x3000,
        DEVICE_SYSTEM_STARTED = 0x3100,
        DEVICE_TIME_SET = 0x3200,
        DEVICE_TIMEZONE_SET = 0x3201,
        DEVICE_DST_SET = 0x3202,
        DEVICE_LINK_CONNECTED = 0x3300,
        DEVICE_LINK_DISCONNECTED = 0x3400,
        DEVICE_DHCP_SUCCESS = 0x3500,
        DEVICE_ADMIN_MENU = 0x3600,
        DEVICE_UI_LOCKED = 0x3700,
        DEVICE_UI_UNLOCKED = 0x3800,
        DEVICE_COMM_LOCKED = 0x3900,
        DEVICE_COMM_UNLOCKED = 0x3A00,
        DEVICE_TCP_CONNECTED = 0x3B00,
        DEVICE_RTSP_CONNECTED = 0x3B10,
        DEVICE_TCP_DISCONNECTED = 0x3C00,
        DEVICE_RTSP_DISCONNECTED = 0x3C10,
        DEVICE_RS485_CONNECTED = 0x3D00,
        DEVICE_RS485_DISCONNECTED = 0x3E00,
        DEVICE_INPUT_DETECTED = 0x3F00,
        DEVICE_TAMPER_ON = 0x4000,
        DEVICE_TAMPER_OFF = 0x4100,
        DEVICE_EVENT_LOG_CLEARED = 0x4200,
        DEVICE_FIRMWARE_UPGRADED = 0x4300,
        DEVICE_RESOURCE_UPGRADED = 0x4400,
        DEVICE_CONFIG_RESET = 0x4500,
        DEVICE_DATABASE_RESET = 0x4501,
        DEVICE_FACTORY_RESET = 0x4502,
        DEVICE_CONFIG_RESET_EX = 0x4503,
        DEVICE_FACTORY_RESET_WITHOUT_ETHERNET = 0x4504,

        SUPERVISED_INPUT_SHORT = 0x4600,
        SUPERVISED_INPUT_OPEN = 0x4700,

        DEVICE_AC_FAIL = 0x4800,
        DEVICE_AC_SUCCESS = 0x4900,
        EXIT_BUTTON = 0x4A00,
        SIMULATED_EXIT_BUTTON = 0x4A01,
        OPERATOR_OPEN = 0x4B00,
        VOIP_OPEN = 0x4C00,

        DOOR_UNLOCKED = 0x5000,
        DOOR_UNLOCKED_BY_BUTTON = 0x5001,           // [+V2.8.3]
        DOOR_UNLOCKED_BY_OPERATOR = 0x5002,         // [+V2.8.3]
        DOOR_UNLOCKED_BY_SIMULATED_BUTTON = 0x5003, // [+V2.8.3]
        DOOR_UNLOCKED_BY_VOIP = 0x5004,             // [+V2.8.3]
        DOOR_LOCKED = 0x5100,
        DOOR_OPENED = 0x5200,
        DOOR_CLOSED = 0x5300,
        DOOR_FORCED_OPEN = 0x5400,
        DOOR_HELD_OPEN = 0x5500,
        DOOR_FORCED_OPEN_ALARM = 0x5600,
        DOOR_FORCED_OPEN_ALARM_CLEAR = 0x5700,
        DOOR_HELD_OPEN_ALARM = 0x5800,
        DOOR_HELD_OPEN_ALARM_CLEAR = 0x5900,
        DOOR_APB_ALARM = 0x5A00,
        DOOR_APB_ALARM_CLEAR = 0x5B00,
        DOOR_RELEASE_NONE = 0x5C00,
        DOOR_RELEASE_SCHEDULE = 0x5C01,
        DOOR_RELEASE_EMERGENCY = 0x5C02,
        DOOR_RELEASE_OPERATOR = 0x5C04,
        DOOR_LOCK_NONE = 0x5D00,
        DOOR_LOCK_SCHEDULE = 0x5D01,
        DOOR_LOCK_EMERGENCY = 0x5D02,
        DOOR_LOCK_OPERATOR = 0x5D04,
        DOOR_UNLOCK_NONE = 0x5E00,
        DOOR_UNLOCK_SCHEDULE = 0x5E01,
        DOOR_UNLOCK_EMERGENCY = 0x5E02,
        DOOR_UNLOCK_OPERATOR = 0x5E04,

        ZONE_APB_VIOLATION = 0x6000,
        ZONE_APB_VIOLATION_HARD = 0x6001,
        ZONE_APB_VIOLATION_SOFT = 0x6002,
        ZONE_APB_ALARM = 0x6100,
        ZONE_APB_ALARM_CLEAR = 0x6200,

        ZONE_TIMED_APB_VIOLATION = 0x6300,
        ZONE_TIMED_APB_VIOLATION_HARD = 0x6301,
        ZONE_TIMED_APB_VIOLATION_SOFT = 0x6302,
        ZONE_TIMED_APB_ALARM = 0x6400,
        ZONE_TIMED_APB_ALARM_CLEAR = 0x6500,

        ZONE_FIRE_ALARM_INPUT = 0x6600,
        ZONE_FIRE_ALARM = 0x6700,
        ZONE_FIRE_ALARM_CLEAR = 0x6800,

        ZONE_SCHEDULED_LOCK_VIOLATION = 0x6900,
        ZONE_SCHEDULED_LOCK_START = 0x6A00,
        ZONE_SCHEDULED_LOCK_END = 0x6B00,
        ZONE_SCHEDULED_UNLOCK_START = 0x6C00,
        ZONE_SCHEDULED_UNLOCK_END = 0x6D00,
        ZONE_SCHEDULED_LOCK_ALARM = 0x6E00,
        ZONE_SCHEDULED_LOCK_ALARM_CLEAR = 0x6F00,

        LIFT_FLOOR_ACTIVATED = 0x7000,
        LIFT_FLOOR_DEACTIVATED = 0x7100,
        LIFT_FLOOR_RELEASE = 0x7200,
        LIFT_FLOOR_RELEASE_SCHEDULE = 0x7201,
        LIFT_FLOOR_RELEASE_EMERGENCY = 0x7202,
        LIFT_FLOOR_RELEASE_OPERATOR = 0x7204,
        LIFT_FLOOR_ACTIVATE = 0x7300,
        LIFT_FLOOR_ACTIVATE_SCHEDULE = 0x7301,
        LIFT_FLOOR_ACTIVATE_EMERGENCY = 0x7302,
        LIFT_FLOOR_ACTIVATE_OPERATOR = 0x7304,
        LIFT_FLOOR_DEACTIVATE = 0x7400,
        LIFT_FLOOR_DEACTIVATE_SCHEDULE = 0x7401,
        LIFT_FLOOR_DEACTIVATE_EMERGENCY = 0x7402,
        LIFT_FLOOR_DEACTIVATE_OPERATOR = 0x7404,

        LIFT_ALARM_INPUT = 0x7500,
        LIFT_ALARM = 0x7600,
        LIFT_ALARM_CLEAR = 0x7700,
        LIFT_ALL_FLOOR_ACTIVATED = 0x7800,
        LIFT_ALL_FLOOR_DEACTIVATED = 0x7900,

        GLOBAL_APB_EXCUSED = 0x8000,

        ZONE_ENTRANCE_LIMIT_COUNT_VIOLATION = 0x8100,
        ZONE_ENTRANCE_LIMIT_COUNT_VIOLATION_HARD = 0x8101,
        ZONE_ENTRANCE_LIMIT_COUNT_VIOLATION_SOFT = 0x8102,
        ZONE_ENTRANCE_LIMIT_TIME_VIOLATION_HARD = 0x8103,
        ZONE_ENTRANCE_LIMIT_TIME_VIOLATION_SOFT = 0x8104,
        ZONE_ENTRANCE_LIMIT_ALARM = 0x8200,
        ZONE_ENTRANCE_LIMIT_ALARM_CLEAR = 0x8300,

        ZONE_INTRUSION_ALARM_VIOLATION = 0x9000,
        ZONE_INTRUSION_ALARM_ARM_GRANTED = 0x9100,
        ZONE_INTRUSION_ALARM_ARM_SUCCESS = 0x9200,
        ZONE_INTRUSION_ALARM_ARM_FAIL = 0x9300,
        ZONE_INTRUSION_ALARM_DISARM_GRANTED = 0x9400,
        ZONE_INTRUSION_ALARM_DISARM_SUCCESS = 0x9500,
        ZONE_INTRUSION_ALARM_DISARM_FAIL = 0x9600,
        ZONE_INTRUSION_ALARM_INPUT = 0x9700,
        ZONE_INTRUSION_ALARM = 0x9800,
        ZONE_INTRUSION_ALARM_CLEAR = 0x9900,
        ZONE_INTRUSION_ALARM_ARM_DENIED = 0x9A00,
        ZONE_INTRUSION_ALARM_DISARM_DENIED = 0x9B00,

        ZONE_INTERLOCK_VIOLATION = 0xA000,
        ZONE_INTERLOCK_ALARM = 0xA100,
        ZONE_INTERLOCK_ALARM_DOOR_OPEN_DENIED = 0xA200,
        ZONE_INTERLOCK_ALARM_INDOOR_DENIED = 0xA300,
        ZONE_INTERLOCK_ALARM_CLEAR = 0xA400,
        ZONE_AUTH_LIMIT_VIOLATION = 0xA500,
        GLOBAL_AUTH_LIMIT_EXCUSED = 0xA600,

        // Relay Action (Linakge & Latching)
        RELAY_ACTION_ON = 0xC300,
        RELAY_ACTION_OFF = 0xC400,
        RELAY_ACTION_KEEP = 0xC500,
    }

    [Flags]
    public enum BS2SubEventCodeEnum
    {
        SUB_EVENT_MASK = 0x00FF,

        VERIFY_ID_PIN = 0x01,
        VERIFY_ID_FINGER = 0x02,
        VERIFY_ID_FINGER_PIN = 0x03,
        VERIFY_ID_FACE = 0x04,
        VERIFY_ID_FACE_PIN = 0x05,
        VERIFY_CARD = 0x06,
        VERIFY_CARD_PIN = 0x07,
        VERIFY_CARD_FINGER = 0x08,
        VERIFY_CARD_FINGER_PIN = 0x09,
        VERIFY_CARD_FACE = 0x0A,
        VERIFY_CARD_FACE_PIN = 0x0B,
        VERIFY_AOC = 0x0C,
        VERIFY_AOC_PIN = 0x0D,
        VERIFY_AOC_FINGER = 0x0E,
        VERIFY_AOC_FINGER_PIN = 0x0F,
        VERIFY_CARD_FACE_FINGER = 0x10,
        VERIFY_CARD_FINGER_FACE = 0x11,
        VERIFY_ID_FACE_FINGER = 0x12,
        VERIFY_ID_FINGER_FACE = 0x13,

        VERIFY_MOBLIE_CARD = 0x16,
        VERIFY_MOBILE_CARD_PIN = 0x17,
        VERIFY_MOBILE_CARD_FINGER = 0x18,
        VERIFY_MOBILE_CARD_FINGER_PIN = 0x19,
        VERIFY_MOBILE_CARD_FACE = 0x1A,
        VERIFY_MOBILE_CARD_FACE_PIN = 0x1B,
        VERIFY_MOBILE_CARD_FACE_FINGER = 0x20,
        VERIFY_MOBILE_CARD_FINGER_FACE = 0x21,

        VERIFY_QR = 0x25,
        VERIFY_QR_PIN = 0x26,
        VERIFY_QR_FINGER = 0x27,
        VERIFY_QR_FINGER_PIN = 0x28,
        VERIFY_QR_FACE = 0x29,
        VERIFY_QR_FACE_PIN = 0x2A,
        VERIFY_QR_FACE_FINGER = 0x2B,
        VERIFY_QR_FINGER_FACE = 0x2C,
 
        // Identified authentication mode
        IDENTIFY_FINGER = 0x01,
        IDENTIFY_FINGER_PIN = 0x02,
        IDENTIFY_FACE = 0x03,
        IDENTIFY_FACE_PIN = 0x04,
        IDENTIFY_FACE_FINGER = 0x05,
        IDENTIFY_FACE_FINGER_PIN = 0x06,
        IDENTIFY_FINGER_FACE = 0x07,
        IDENTIFY_FINGER_FACE_PIN = 0x08,

        // Reason to be failed
        ENROLL_FAIL_INVALID_FACE = 0x01,
        UPDATE_FAIL_INVALID_FACE = 0x01,
        ENROLL_FAIL_MISMATCHED_FORMAT = 0x02,       // [+V2.8.3]
        UPDATE_FAIL_MISMATCHED_FORMAT = 0x02,       // [+V2.8.3]
        ENROLL_FAIL_FULL_CREDENTIAL = 0x03,         // [+V2.8.3]
        UPDATE_FAIL_FULL_CREDENTIAL = 0x03,         // [+V2.8.3]
        ENROLL_FAIL_INVALID_USER = 0x04,            // [+V2.8.3]
        UPDATE_FAIL_INVALID_USER = 0x04,            // [+V2.8.3]
        ENROLL_FAIL_INTERNAL_ERROR = 0x09,          // [+V2.8.3]
        UPDATE_FAIL_INTERNAL_ERROR = 0x09,          // [+V2.8.3]

        // Reason to be failed
        DUAL_AUTH_FAIL_TIMEOUT = 0x01,
        DUAL_AUTH_FAIL_ACCESS_GROUP = 0x02,

        // Bypass mode - soft violation
        BYPASS_NO_VIOLATION = 0x00,
        BYPASS_THERMAL_VIOLATION = 0x01,
        BYPASS_MASK_VIOLATION = 0x02,
        BYPASS_MASK_THERMAL_VIOLATION = 0x03,

        // Reason to be failed
        HIGH_TEMPERATURE = 0x00,
        NO_TEMPERATURE = 0x01,
        UNMASKED_FACE = 0x02,

        // Failed credential
        CREDENTIAL_ID = 0x01,
        CREDENTIAL_CARD = 0x02,
        CREDENTIAL_PIN = 0x03,
        CREDENTIAL_FINGER = 0x04,
        CREDENTIAL_FACE = 0x05,
        CREDENTIAL_AOC_PIN = 0x06,
        CREDENTIAL_AOC_FINGER = 0x07,
        CREDENTIAL_MOBILE_CARD = 0x08,
        //CREDENTIAL_QR = 0x09,             // [+ V2.8.2.7]
        CREDENTIAL_QR = 0x0C,
        NON_NUMERIC_QR = 0x09,
        NON_PRINTABLE_QR = 0x0A,
        TOO_LONG_QR = 0x0B, 

        // Reason to be failed
        AUTH_FAIL_INVALID_AUTH_MODE = 0x01,
        AUTH_FAIL_INVALID_CREDENTIAL = 0x02,
        AUTH_FAIL_TIMEOUT = 0x03,
        AUTH_FAIL_MATCHING_REFUSAL = 0x04,

        // Reason to be denied
        ACCESS_DENIED_ACCESS_GROUP = 0x01,
        ACCESS_DENIED_DISABLED = 0x02,
        ACCESS_DENIED_EXPIRED = 0x03,
        ACCESS_DENIED_ON_BLACKLIST = 0x04,
        ACCESS_DENIED_APB = 0x05,
        ACCESS_DENIED_TIMED_APB = 0x06,
        ACCESS_DENIED_SCHEDULED_LOCK = 0x07,
        ACCESS_DENIED_FORCED_LOCK = 0x07,		// Deprecated in V2.4.0.
        ACCESS_EXCUSED_APB = 0x08,
        ACCESS_EXCUSED_TIMED_APB = 0x09,
        ACCESS_DENIED_FACE_DETECTION = 0x0A,
        ACCESS_DENIED_CAMERA_CAPTURE = 0x0B,
        ACCESS_DENIED_FAKE_FINGER = 0x0C,
        ACCESS_DENIED_DEVICE_ZONE_ENTRANCE_LIMIT = 0x0D,
        ACCESS_DENIED_INTRUSION_ALARM = 0x0E,
        ACCESS_DENIED_INTERLOCK = 0x0F,
        ACCESS_EXCUSED_AUTH_LIMIT = 0x10,
        ACCESS_DENIED_AUTH_LIMIT = 0x11,
        ACCESS_DENIED_ANTI_TAILGATE = 0x12,
        ACCESS_DENIED_HIGH_TEMPERATURE = 0x13,
        ACCESS_DENIED_NO_TEMPERATURE = 0x14,
        ACCESS_DENIED_UNMASKED_FACE = 0x15,

        // Door flag type
        DOOR_FLAG_SCHEDULE = BS2DoorFlagEnum.SCHEDULE,
        DOOR_FLAG_OPERATOR = BS2DoorFlagEnum.OPERATOR,
        DOOR_FLAG_EMERGENCY = BS2DoorFlagEnum.EMERGENCY,

        // Floor flag type
        FLOOR_FLAG_SCHEDULE = BS2FloorFlagEnum.SCHEDULE,
        FLOOR_FLAG_OPERATOR = BS2FloorFlagEnum.OPERATOR,
        FLOOR_FLAG_ACTION = BS2FloorFlagEnum.ACTION,
        FLOOR_FLAG_EMERGENCY = BS2FloorFlagEnum.EMERGENCY,

        // Antipassback violation type
        ZONE_HARD_APB = 0x01,
        ZONE_SOFT_APB = 0x02,

        // Device Zone Entrance limit violation type
        DEVICE_ZONE_HARD_ENTRANCE_LIMIT_COUNT = 0x01,
        DEVICE_ZONE_SOFT_ENTRANCE_LIMIT_COUNT = 0x02,
        DEVICE_ZONE_HARD_ENTRANCE_LIMIT_TIME = 0x03,
        DEVICE_ZONE_SOFT_ENTRANCE_LIMIT_TIME = 0x04,

        // InterlockZone violation type
        INTERLOCKZONE_DOOR_OPEN = 0x01,
        INTERLOCK_INPUT_DETECT = 0x02,

        // Authentication Limit violation type
        ZONE_HARD_AUTH_LIMIT = 0x01,
        ZONE_SOFT_AUTH_LIMIT = 0x02,
        ZONE_SCHEDULE_AUTH_LIMIT = 0x03,
        ZONE_COUNT_AUTH_LIMIT = 0x04,
        ZONE_USER_AUTH_LIMIT = 0x05
    }

    [Flags]
    public enum BS2EventMaskEnum : ushort
    {
        NONE = 0,
        INFO = 0x0001,
        USER_ID = 0x0002,
        CARD_ID = 0x0004,
        DOOR_ID = 0x0008,
        ZONE_ID = 0x0010,
        IODEVICE = 0x0020,
        TNA_KEY = 0x0040,
        JOB_CODE = 0x0080,
        IMAGE = 0x0100,
        TEMPERATURE = 0X0200,               // [+ V2.8.2.7]
        QR_DATA = 0X0400,                   // [+ V2.8.2.7]
       
        ALL = 0xFFFF,
    }

    [Flags]
    public enum BS2_CRED_KEY_REQ
    {
        BS2_CRED_KEY_REQ_COMM = 0x0,
        BS2_CRED_KEY_REQ_DATA = 0x1,
    }

    #region DEVICE_ZONE_SUPPORTED
    [Flags]
    public enum BS2_DEVICE_ZONE_NODE_TYPE
    {
        BS2_DEVICE_ZONE_NODE_TYPE_MASTER = 0x01,
        BS2_DEVICE_ZONE_NODE_TYPE_MEMBER = 0x02,
    }

    public enum BS2_DEVICE_ZONE_TYPE
    {
        #region ENTRNACE_LIMIT
        BS2_DEVICE_ZONE_TYPE_ENTRANCE_LIMIT = 0x03,
        #endregion
        #region FIRE_ALRAM
        BS2_DEVICE_ZONE_TYPE_FIRE_ALARM = 0x05,
        #endregion
    }

    #region ENTRNACE_LIMIT
    public enum BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE
    {
        BS2_DEVICE_ZONE_ENTRANCE_LIMIT_SOFT = 0x01,
        BS2_DEVICE_ZONE_ENTRANCE_LIMIT_HARD = 0x02,
    }

    public enum BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_TYPE
    {
        BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_SOFT = 0x01,
        BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_HARD = 0x02,
    }
    #endregion

    #region FIRE_ALRAM
    public enum BS2_DEVICE_ZONE_ALARMED_STATUS_TYPE
    {
        BS2_DEVICE_ZONE_ALARMED_DISALARM = 0x00,
        BS2_DEVICE_ZONE_ALARMED_ALARM = 0x01,
        BS2_DEVICE_ZONE_ALARMED_SELF = 0x02,
    }
    #endregion
    #endregion

    [Flags]
    public enum BS2OperationTypeEnum
    {
        INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_NONE = 0x00,
        INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_ENRTY = 0x01,
        INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_EXIT = 0x02,
        INTERLOCK_ZONE_INPUT_SENSOR_OPERATION_MASK_ALL = 0xFF,
    }

    #region DEBUG
    static class Constants
    {
        public const UInt32 DEBUG_MODULE_KEEP_ALIVE         = (0x1 << 0);
        public const UInt32 DEBUG_MODULE_SOCKET_MANAGER     = (0x1 << 1);
        public const UInt32 DEBUG_MODULE_SOCKET_HANDLER     = (0x1 << 2);
        public const UInt32 DEBUG_MODULE_DEVICE             = (0x1 << 3);
        public const UInt32 DEBUG_MODULE_DEVICE_MANAGER     = (0x1 << 4);
        public const UInt32 DEBUG_MODULE_EVENT_DISPATCHER   = (0x1 << 5);
        public const UInt32 DEBUG_MODULE_API                = (0x1 << 6);
        public const UInt32 DEBUG_MODULE_ALL                = (0xffffffff);
        public const UInt32 DEBUG_LOG_FATAL                 = (0x1 << 0);
        public const UInt32 DEBUG_LOG_ERROR                 = (0x1 << 1);
        public const UInt32 DEBUG_LOG_WARN                  = (0x1 << 2);
        public const UInt32 DEBUG_LOG_INFO                  = (0x1 << 3);
        //public const UInt32 DEBUG_LOG_TRACE                 = (0x1 << 4);
        public const UInt32 DEBUG_LOG_TRACE                 = (0x1 << 8);
        public const UInt32 DEBUG_LOG_OPERATION_ALL         = (0x000000ff);
        public const UInt32 DEBUG_LOG_ALL                   = (0xffffffff);
    }
    #endregion

    [Flags]
    public enum BS2ParityTypeEnum
    {
        BS2_WIEGAND_PARITY_NONE = 0,
        BS2_WIEGAND_PARITY_ODD = 1,
        BS2_WIEGAND_PARITY_EVEN = 2,
    }

    [Flags]
    public enum BS2WiegandFormatEnum : ushort
    {
        BS2_WIEGAND_H10301_26 = (0),
        BS2_WIEGAND_H10302_37 = (1),
        BS2_WIEGAND_H10304_37 = (2),
        BS2_WIEGAND_C1000_35 = (3),
        BS2_WIEGAND_C1000_48 = (4),
    }

    [Flags]
    public enum BS2WiegandModeEnum
    {
        BS2_WIEGAND_IN_ONLY = 0,
        BS2_WIEGAND_OUT_ONLY = 1,
        BS2_WIEGAND_IN_OUT = 2,
    }

    [Flags]
    public enum BS2AuthModeEnum
    {
        BS2_AUTH_MODE_NONE = 255,
        BS2_AUTH_MODE_PROHIBITED = 254,

        BS2_AUTH_MODE_BIOMETRIC_ONLY = 0,
        BS2_AUTH_MODE_BIOMETRIC_PIN = 1,

        BS2_AUTH_MODE_CARD_ONLY = 2,
        BS2_AUTH_MODE_CARD_BIOMETRIC = 3,
        BS2_AUTH_MODE_CARD_PIN = 4,
        BS2_AUTH_MODE_CARD_BIOMETRIC_OR_PIN = 5,
        BS2_AUTH_MODE_CARD_BIOMETRIC_PIN = 6,

        BS2_AUTH_MODE_ID_BIOMETRIC = 7,
        BS2_AUTH_MODE_ID_PIN = 8,
        BS2_AUTH_MODE_ID_BIOMETRIC_OR_PIN = 9,
        BS2_AUTH_MODE_ID_BIOMETRIC_PIN = 10,

        BS2_NUM_OF_AUTH_MODE,
    }

    [Flags]
    public enum BS2DesfireCardEncryption
    {
	    BS2_DESFIRECARD_ENCRYPTION_DES_3DES = 0,
	    BS2_DESFIRECARD_ENCRYPTION_AES = 1,
    }

    [Flags]
    public enum BS2DesfireCardOperation
    {
        BS2_DESFIRECARD_OPERATION_MODE_LEGACY = 0,
        BS2_DESFIRECARD_OPERATION_MODE_APPLEVELKEY = 1,
    }

    //=> [IPv6]
    public enum BS2SpecifiedDeviceInfo : uint
    {
        BS2_SPECIFIED_DEVICE_INFO_SIMPLE = 1,
        BS2_SPECIFIED_DEVICE_INFO_SIMPLE_EX = 2,
        BS2_SPECIFIED_DEVICE_INFO_IPV6 = 3,
    }
    //<=

    // Log image field (BS2Event.image) -> 1 byte
    // BS2Event.image 00000000
    //---------------------------------
    //   Image        00000001
    //   DST          00000010
    //   Half         00000100
    //   Hour         01111000
    //   Negative     10000000
    public enum BS2EventImageBitPos : byte
    {
        BS2_IMAGEFIELD_POS_IMAGE = 0x01,
        BS2_IMAGEFIELD_POS_DST   = 0x02,
	    BS2_IMAGEFIELD_POS_HALF  = 0x78,
    	BS2_IMAGEFIELD_POS_HOUR  = 0x80,
    }

    [Flags]
    public enum BS2SystemConfigCardOperationMask : uint
    {
        CARD_OPERATION_MASK_DEFAULT = 0xFFFFFFFF,
        CARD_OPERATION_USE = 0x80000000,
        CARD_OPERATION_MASK_CUSTOM_DESFIRE_EV1 = 0x00000800,
        CARD_OPERATION_MASK_CUSTOM_CLASSIC_PLUS = 0x00000400,
        CARD_OPERATION_MASK_BLE = 0x00000200,
        CARD_OPERATION_MASK_NFC = 0x00000100,
        CARD_OPERATION_MASK_SEOS = 0x00000080,
        CARD_OPERATION_MASK_SR_SE = 0x00000040,
        CARD_OPERATION_MASK_DESFIRE_EV1 = 0x00000020,
        CARD_OPERATION_MASK_CLASSIC_PLUS = 0x00000010,
        CARD_OPERATION_MASK_ICLASS = 0x00000008,
        CARD_OPERATION_MASK_MIFARE_FELICA = 0x00000004,
        CARD_OPERATION_MASK_HIDPROX = 0x00000002,
        CARD_OPERATION_MASK_EM = 0x00000001,
    }

    [Flags]
    public enum BS2SupportedInfoMask : uint
    {
        BS2_SUPPORT_RS485EX = 0x00000001,
        BS2_SUPPORT_CARDEX = 0x00000002,
        BS2_SUPPORT_DST = 0x00000004,
        BS2_SUPPORT_DESFIREEX = 0x00000008,
        BS2_SUPPORT_FACE_EX = 0x00000010,       // F2 support
        BS2_SUPPORT_QR = 0x00000020,            // [+2.8]

        BS2_SUPPORT_FINGER_SCAN = 0x00010000,
        BS2_SUPPORT_FACE_SCAN = 0x00020000,
        BS2_SUPPORT_FACE_EX_SCAN = 0x00040000,
        BS2_SUPPORT_QR_SCAN = 0x00080000,       // [+2.8]

        BS2_SUPPORT_ALL = BS2_SUPPORT_RS485EX |
                          BS2_SUPPORT_CARDEX |
                          BS2_SUPPORT_DST |
                          BS2_SUPPORT_DESFIREEX |
                          BS2_SUPPORT_FACE_EX |
                          BS2_SUPPORT_QR |
                          BS2_SUPPORT_FINGER_SCAN |
                          BS2_SUPPORT_FACE_SCAN |
                          BS2_SUPPORT_FACE_EX_SCAN |
                          BS2_SUPPORT_QR_SCAN,
    }

    [Flags]
    public enum BS2DeviceStatus
    {
        NORMAL,
        LOCKED,
        RTC_ERROR,
        WAITING_INPUT,
        WAITING_DHCP,
        SCAN_FINGER,
        SCAN_CARD,
        SUCCESS,
        FAIL,
        DURESS,
        PROCESS_CONFIG_CARD,
        SUCCESS_CONFIG_CARD,
        SCAN_FACE,
        RESERVED3,
        RESERVED4,

        NUM_OF_STATUS,
    }

    [Flags]
    public enum BS2CapabilitySystemSupport
    {
        SYSTEM_SUPPORT_CAMERA   = 0x01,
	    SYSTEM_SUPPORT_TAMPER   = 0x02,
	    SYSTEM_SUPPORT_WLAN     = 0x04,
	    SYSTEM_SUPPORT_DISPLAY  = 0x08,
	    SYSTEM_SUPPORT_THERMAL  = 0x10,
	    SYSTEM_SUPPORT_MASK     = 0x20,
	    SYSTEM_SUPPORT_FACEEX   = 0x40,
        SYSTEM_SUPPORT_VOIPEX   = 0x80,         // [+V2.8.3]
	}

    [Flags]
    public enum BS2CapabilityCardSupport : uint
    {
        CARD_SUPPORT_EM                 = 0x00000001,
        CARD_SUPPORT_HIDPROX            = 0x00000002,
        CARD_SUPPORT_MIFAREFELICA       = 0x00000004,
        CARD_SUPPORT_ICLASS             = 0x00000008,
        CARD_SUPPORT_CLASSICPLUS        = 0x00000010,
        CARD_SUPPORT_DESFIREEV1         = 0x00000020,
        CARD_SUPPORT_SRSE               = 0x00000040,
        CARD_SUPPORT_SEOS               = 0x00000080,
        CARD_SUPPORT_NFC                = 0x00000100,
        CARD_SUPPORT_BLE                = 0x00000200,
        CARD_SUPPORT_CUSTOMCLASSICPLUS  = 0x00000400,   // [V2.9.6]
        CARD_SUPPORT_CUSTOMDESFIREEV1   = 0x00000800,   // [V2.9.6]
        CARD_SUPPORT_TOM_NFC            = 0x00001000,   // [V2.9.6]
        CARD_SUPPORT_TOM_BLE            = 0x00002000,   // [V2.9.6]
        CARD_SUPPORT_CUSTOMFELICA       = 0x00004000,
        CARD_SUPPORT_USECARDOPERATION   = 0x80000000,
    }

    [Flags]
    public enum BS2CapabilityFunctionSupport
    {
        FUNCTION_SUPPORT_INTELLIGENTPD      = 0x01,
        FUNCTION_SUPPORT_UPDATEUSER         = 0x02,     // [V2.8.3]
        FUNCTION_SUPPORT_SIMULATEDUNLOCK    = 0x04,     // [V2.8.3]
        FUNCTION_SUPPORT_SMARTCARDBYTEORDER = 0x08,
        FUNCTION_SUPPORT_TREATASCSN         = 0x10,
        FUNCTION_SUPPORT_RTSP               = 0x20,     // [V2.8.3]
		FUNCTION_SUPPORT_LFD                = 0x40,     // [V2.8.3]
		FUNCTION_SUPPORT_VISUALQR           = 0x80,     // [V2.8.3]
    }

    [Flags]
    public enum BS2CapabilityFunctionSupport2
    {
        FUNCTION2_SUPPORT_OSDPSTANDARDCENTRAL  = 0x01,  // [V2.9.1]
        FUNCTION2_SUPPORT_ENABLELICENSE        = 0x02,  // [V2.9.1]
	    FUNCTION2_SUPPORT_KEYPADBACKLIGHT      = 0x04,  // [V2.9.4]
	    FUNCTION2_SUPPORT_UZWIRELESSLOCKDOOR   = 0x08,  // [V2.9.4]
	    FUNCTION2_SUPPORT_CUSTOMSMARTCARD      = 0x10,  // [V2.9.4]
        FUNCTION2_SUPPORT_TOM                  = 0x20,  // [V2.9.4]
        FUNCTION2_SUPPORT_TOMENROLL            = 0x40,  // [V2.9.6]
        FUNCTION2_SUPPORT_SHOWOSDPRESULTBYLED  = 0x80,  // [V2.9.6]
    }

    [Flags]
    public enum BS2CapabilityFunctionSupport3
    {
        FUNCTION3_SUPPORT_CUSTOMSMARTCARDFELICA         = 0x01,  // [V2.9.6]
        FUNCTION3_SUPPORT_IGNOREINPUTAFTERWIEGANDOUT    = 0x02,  // [V2.9.6]
        FUNCTION3_SUPPORT_SETSLAVEBAUDRATE              = 0x04,  // [V2.9.6]
    }

    [Flags]
    public enum BS2SupervisedResistor
    {
        SUPERVISED_RESISTOR_1K      = 0,
        SUPERVISED_RESISTOR_2_2K    = 1,
        SUPERVISED_RESISTOR_4_7K    = 2,
        SUPERVISED_RESISTOR_10K     = 3,

        SUPERVISED_RESISTOR_UNUSED  = 254,
    }

    [Flags]
    public enum BS2RelayActionInputType
    {
	    RELAY_ACTION_INPUT_TYPE_NONE	 = 0x00,
	    RELAY_ACTION_INPUT_TYPE_LINKAGE  = 0x01,
	    RELAY_ACTION_INPUT_TYPE_LATCHING = 0x02,
	    RELAY_ACTION_INPUT_TYPE_RELEASE  = 0x03,
    }

    [Flags]
    public enum BS2RelayActionInputMask
    {
	    RELAY_ACTION_INPUT_MASK_NONE	= 0x00,
	    RELAY_ACTION_INPUT_MASK_ALARM	= 0x01,
	    RELAY_ACTION_INPUT_MASK_FAULT	= 0x02,
    }

    [Flags]
    public enum BS2MotionSensitivity                // + 2.9.1
    {
        LOW = 0,
        NORMAL = 1,
        HIGH = 2,
    }

    [Flags]
    public enum BS2LicenseStatus                    // + 2.9.1
    {
        NOT_SUPPORTED   = 0,
        DISABLE         = 1,
        ENABLE          = 2,
        EXPIRED         = 3,
    }

    [Flags]
    public enum BS2LicenseType                      // + 2.9.1
    {
        NONE           = 0x0000,
        VISUAL_QR_MASK = 0x0001,
        MAX_MASK       = VISUAL_QR_MASK,
    }

    [Flags]
    public enum BS2LicenseSubType                   // + 2.9.1
    {
        NONE                    = 0,
        VISUAL_QR_CODE_CORP     = 1,
    }

    [Flags]
    public enum BS2OsdpStandardActionType           // + 2.9.1
    {
        COUNT       = 3,

        NONE        = 0,
        SUCCESS     = 1,
        FAIL        = 2,
        WAIT_INPUT  = 3,
    }

    [Flags]
    public enum BS2OsdpStandardLEDCommand           // + 2.9.1
    {
        NOP     = 0,
        CANCEL  = 1,
        SET     = 2,
    }

    [Flags]
    public enum BS2OsdpStandardColor                // + 2.9.1
    {
        BLACK = 0,
        RED,
        GREEN,
        AMBER,
        BLUE,
        MAGENTA,
        CYAN,
        WHITE
    }

    [Flags]
    public enum BS2OsdpStandardTone                 // + 2.9.1
    {
        NONE = 0,
        OFF  = 1,
        ON   = 2,
    }

    [Flags]
    public enum BS2FaceExFlag
    {
        NONE            = 0x00,
        WARPED          = 0x01,
        TEMPLATE_ONLY   = 0x20,
        ALL             = 0xFF,
    }

    [Flags]
    public enum BS2StopFlag
    {
        NONE                = 0x00,
        ON_DOOR_CLOSED      = 0x01,
        BY_CMD_RUN_ACTION   = 0x02,
    }
}
