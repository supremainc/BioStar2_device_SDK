using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
	
namespace Suprema
{

    using BS2_SCHEDULE_ID = UInt32;

    public static class BS2Environment
    {
        public const int BS2_TCP_DEVICE_PORT_DEFAULT = 51211;
        public const int BS2_TCP_SERVER_PORT_DEFAULT = 51212;
        public const int BS2_TCP_SSL_SERVER_PORT_DEFAULT = 51213; //=> [Get ServerPort] <=

        //=> [IPv6]
        public const int BS2_IPV6_ADDR_SIZE = 40;
        public const int BS2_MAX_IPV6_ALLOCATED_ADDR = 8;
        //<=

        //=> [IPv6]
        public const int BS2_TCP_DEVICE_PORT_DEFAULT_V6 = 52211;
        public const int BS2_TCP_SERVER_PORT_DEFAULT_V6 = 52212;
        public const int BS2_TCP_SSL_SERVER_PORT_DEFAULT_V6 = 52213;
        //<=

        public const int BS2_MAX_NUM_OF_FINGER_PER_USER = 10;
        public const int BS2_MAX_NUM_OF_CARD_PER_USER = 8;
        public const int BS2_MAX_NUM_OF_FACE_PER_USER = 5;
        public const int BS2_NUM_OF_AUTH_MODE = 11;
        public const int BS2_MAC_ADDR_LEN = 6;
        public const int BS2_MODEL_NAME_LEN = 32;
        public const int BS2_KERNEL_REV_LEN = 32;
        public const int BS2_BSCORE_REV_LEN = 32;
        public const int BS2_FIRMWARE_REV_LEN = 32;
        public const int BS2_IPV4_ADDR_SIZE = 16;
        public const int BS2_URL_SIZE = 256;
        public const int BS2_USER_ID_SIZE = 32;
        public const int BS2_USER_NAME_LEN = 48 * 4;
        public const int BS2_USER_PHOTO_SIZE = 16 * 1024;
        public const int BS2_PIN_HASH_SIZE = 32;
        public const int BS2_MAX_OPERATORS = 10;
        public const int BS2_DEVICE_STATUS_NUM = (int)BS2DeviceStatus.NUM_OF_STATUS;
        public const int BS2_MAX_SHORTCUT_HOME = 8;
        public const int BS2_MAX_TNA_KEY = 16;
        public const int BS2_MAX_TNA_LABEL_LEN = 16 * 3;
        public const int BS2_CARD_KEY_SIZE = 32;
        public const int BS2_CARD_DATA_SIZE = 32;
        public const int BS2_FINGER_TEMPLATE_SIZE = 384;
        public const int BS2_TEMPLATE_PER_FINGER = 2;
        public const int BS2_FACE_TEMPLATE_LENGTH = 3008;
        public const int BS2_TEMPLATE_PER_FACE = 30;
        public const int BS2_WIEGAND_MAX_FIELDS = 4;
        public const int BS2_WIEGAND_MAX_PARITIES = 4;
        public const int BS2_WIEGAND_FIELD_SIZE = 32;
        public const int BS2_MAX_ACCESS_GROUP_NAME_LEN = 144;
        public const int BS2_MAX_ACCESS_GROUP_PER_USER = 16;
        public const int BS2_MAX_ACCESS_LEVEL_PER_ACCESS_GROUP = 128;
        public const int BS2_MAX_ACCESS_LEVEL_NAME_LEN = 144;
        public const int BS2_MAX_ACCESS_LEVEL_ITEMS = 128;
        public const int BS2_MAX_TIME_PERIODS_PER_DAY = 5;
        public const int BS2_NUM_WEEKDAYS = 7;
        public const int BS2_MAX_DAYS_PER_DAILY_SCHEDULE = 90;
        public const int BS2_MAX_SCHEDULE_NAME_LEN = 144;
        public const int BS2_MAX_HOLIDAY_GROUPS_PER_SCHEDULE = 4;
        public const int BS2_MAX_HOLIDAY_GROUP_NAME_LEN = 144;
        public const int BS2_MAX_HOLIDAYS_PER_GROUP = 128;
        public const int BS2_RS485_MAX_CHANNELS = 4;
        public const int BS2_RS485_MAX_SLAVES_PER_CHANNEL = 32;
        public const int BS2_WIEGAND_STATUS_NUM = 2;
        public const int BS2_MAX_INPUT_NUM = 8;
        public const int BS2_WLAN_SSID_SIZE = 32;
        public const int BS2_WLAN_KEY_SIZE = 64;
        public const int BS2_EVENT_MAX_IMAGE_CODE_COUNT = 32;
        public const int MAX_WIEGAND_IN_COUNT = 15;
        public const int BS2_LED_SIGNAL_NUM = 3;
        public const int BS2_BUZZER_SIGNAL_NUM = 3;
        public const int BS2_MAX_TRIGGER_ACTION = 128;
        public const int BS2_MAX_DOOR_NAME_LEN = 144;
        public const int BS2_MAX_FORCED_OPEN_ALARM_ACTION = 5;
        public const int BS2_MAX_HELD_OPEN_ALARM_ACTION = 5;
        public const int BS2_MAX_DUAL_AUTH_APPROVAL_GROUP = 16;
        public const int BS2_MAX_RESOURCE_ITEM_COUNT = 128;
        public const int BS2_MAX_BLACK_LIST_SLOTS = 1000;        
        public const int BS2_SMART_CARD_MAX_TEMPLATE_COUNT = 4;
        public const int BS2_SMART_CARD_MAX_ACCESS_GROUP_COUNT = 16;
        public const int BS2_SMART_CARD_MIN_TEMPLATE_SIZE = 300;
        public const int BS2_MAX_ZONE_NAME_LEN = 144;
        public const int BS2_MAX_APB_ALARM_ACTION = 5;
        public const int BS2_MAX_READERS_PER_APB_ZONE = 64;
        public const int BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE = 16;
        public const int BS2_MAX_TIMED_APB_ALARM_ACTION = 5;
        public const int BS2_MAX_READERS_PER_TIMED_APB_ZONE = 64;
        public const int BS2_MAX_BYPASS_GROUPS_PER_TIMED_APB_ZONE = 16;
        public const int BS2_MAX_FIRE_SENSORS_PER_FIRE_ALARM_ZONE = 8;
        public const int BS2_MAX_FIRE_ALARM_ACTION = 5;
        public const int BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE = 32;
        public const int BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION = 5;
        public const int BS2_MAX_DOORS_IN_SCHEDULED_LOCK_UNLOCK_ZONE = 32;
        public const int BS2_MAX_BYPASS_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE = 16;
        public const int BS2_MAX_UNLOCK_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE = 16;
        public const int BS2_VOIP_MAX_PHONEBOOK = 32;
        public const int BS2_VOIP_MAX_PHONEBOOK_EXT = 128;
        public const int BS2_MAX_DESCRIPTION_NAME_LEN = 144;
        public const int BS2_VOIP_MAX_DESCRIPTION_LEN_EXT = 48 * 3;
        public const int BS2_FACE_WIDTH_MIN_DEFAULT = 66;               // F2
        public const int BS2_FACE_WIDTH_MAX_DEFAULT = 250;              // F2
        public const int BS2_FACE_SEARCH_RANGE_X_DEFAULT = 144;         // F2
        public const int BS2_FACE_SEARCH_RANGE_WIDTH_DEFAULT = 432;     // F2
        public const int BS2_FACE_DETECT_DISTANCE_MIN_MIN = 30;         // BS3
        public const int BS2_FACE_DETECT_DISTANCE_MIN_MAX = 100;        // BS3
        public const int BS2_FACE_DETECT_DISTANCE_MIN_DEFAULT = 30;     // BS3      (60 -> 30)
        public const int BS2_FACE_DETECT_DISTANCE_MAX_MIN = 40;         // BS3
        public const int BS2_FACE_DETECT_DISTANCE_MAX_MAX = 100;        // BS3
        public const int BS2_FACE_DETECT_DISTANCE_MAX_INF = 255;        // BS3
        public const int BS2_FACE_DETECT_DISTANCE_MAX_DEFAULT = 100;    // BS3
        public const int BS2_FACE_IMAGE_SIZE = 16 * 1024;
        public const int BS2_CAPTURE_IMAGE_MAXSIZE = 1280 * 720 * 3;
		public const int BS2_MAX_AUTH_GROUP_NAME_LEN = 144;
        public const int BS2_MAX_JOB_SIZE = 16;
        public const int BS2_MAX_JOBLABEL_LEN = 48;
        public const int BS2_USER_PHRASE_SIZE = 128;
		public const int BS2_MAX_FLOOR_LEVEL_NAME_LEN = 144;
        public const int BS2_MAX_FLOOR_LEVEL_ITEMS = 128;
		public const int BS2_MAX_LIFT_NAME_LEN = 144;
        public const int BS2_MAX_DEVICES_ON_LIFT = 4;
        public const int BS2_MAX_FLOORS_ON_LIFT = 255;
        public const int BS2_MAX_DUAL_AUTH_APPROVAL_GROUP_ON_LIFT = 16;
        public const int BS2_MAX_ALARMS_ON_LIFT = 2;
        public const int BS2_EVENT_MAX_IMAGE_SIZE = 16384;
        public const int BS2_RS485_MAX_CHANNELS_EX = 8;
        public const int BS2_MAX_DST_SCHEDULE = 2;

        public const int BS2_ENC_KEY_SIZE = 32;

        // F2 support
        public const int BS2_MAX_NUM_OF_EXT_AUTH_MODE = 128;

        public const int BS2_FACE_EX_TEMPLATE_SIZE = 552;
	    public const int BS2_VISUAL_TEMPLATES_PER_FACE_EX = 10;
	    public const int BS2_IR_TEMPLATES_PER_FACE_EX = 10;
	    public const int BS2_MAX_TEMPLATES_PER_FACE_EX = BS2_VISUAL_TEMPLATES_PER_FACE_EX + BS2_IR_TEMPLATES_PER_FACE_EX;

        public const int BS2_MAX_WARPED_IMAGE_LENGTH = 40 * 1024;
        public const int BS2_MAX_WARPED_IR_IMAGE_LENGTH = 30 * 1024;
        // F2 support

        public const int BS2_THERMAL_CAMERA_DISTANCE_DEFAULT = 100;
        [Obsolete] public const int BS2_THERMAL_CAMERA_EMISSION_RATE_DEFAULT = 98;
        public const int BS2_THERMAL_CAMERA_EMISSIVITY_DEFAULT = 98;

        // Default (F2)
        public const int BS2_THERMAL_CAMERA_ROI_X_DEFAULT = 30;
        public const int BS2_THERMAL_CAMERA_ROI_Y_DEFAULT = 25;
        public const int BS2_THERMAL_CAMERA_ROI_WIDTH_DEFAULT = 50;
        public const int BS2_THERMAL_CAMERA_ROI_HEIGHT_DEFAULT = 55;

        // Default (FS2)
	    public const int BS2_THERMAL_CAMERA_ROI_X_DEFAULT_FS2 = 47;
	    public const int BS2_THERMAL_CAMERA_ROI_Y_DEFAULT_FS2 = 45;
	    public const int BS2_THERMAL_CAMERA_ROI_WIDTH_DEFAULT_FS2 = 15;
        public const int BS2_THERMAL_CAMERA_ROI_HEIGHT_DEFAULT_FS2 = 10;

        public const int BS2_MOBILE_ACCESS_KEY_SIZE = 124;

        // BS2InputConfigEx
        public const int BS2_MAX_INPUT_NUM_EX = 16;

        // BS2RelayActionConfig
        public const int BS2_MAX_RELAY_ACTION = 4;
        public const int BS2_MAX_RELAY_ACTION_INPUT = 16;

        #region DEVICE_ZONE_SUPPORTED
        public const int BS2_TCP_DEVICE_ZONE_MASTER_PORT_DEFAULT = 51214;
        public const int BS2_MAX_DEVICE_ZONE = 8;
        
        #region ENTRNACE_LIMIT
        public const int BS2_MAX_READERS_PER_DEVICE_ZONE_ENTRANCE_LIMIT	= 64;
	    public const int BS2_MAX_BYPASS_GROUPS_PER_DEVICE_ZONE_ENTRANCE_LIMIT = 16;
	    public const int BS2_MAX_DEVICE_ZONE_ENTRANCE_LIMIT_ALARM_ACTION = 5;
	    public const int BS2_MAX_ENTRANCE_LIMIT_PER_ZONE = 24;
        public const int BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT = 16;
        public const int BS2_ENTRY_COUNT_FOR_ACCESS_GROUP_ENTRANCE_LIMIT = -2;
        public const int BS2_OTHERWISE_ACCESS_GROUP_ID = -1;
        public const int BS2_ENTRY_COUNT_NO_LIMIT = -1;
        #endregion
        
        #region FIRE_ALARM
        public const int BS2_MAX_READERS_PER_DEVICE_ZONE_FIRE_ALARM = 64;
        public const int BS2_MAX_DEVICE_ZONE_FIRE_ALARM_ALARM_ACTION = 5;
        public const int BS2_MAX_FIRE_SENSORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER = 8;
        public const int BS2_MAX_DOORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER = 8;
        #endregion
        #endregion

        #region INTERLOCK_ZONE_SUPPORTED
        public const int BS2_MAX_INTERLOCK_ZONE = 32;
        public const int BS2_MAX_INPUTS_IN_INTERLOCK_ZONE = 4;
        public const int BS2_MAX_OUTPUTS_IN_INTERLOCK_ZONE = 8;
        public const int BS2_MAX_DOORS_IN_INTERLOCK_ZONE = 4;
        #endregion

        //=> [IPv6]
        public static readonly string All_routers_in_the_site_local = "FF05::2"; //All router multicast (in the site local)
        public static readonly string DEFAULT_MULTICAST_IPV6_ADDRESS = All_routers_in_the_site_local;
        public static readonly string DEFAULT_BROADCAST_IPV4_ADDRESS = "255.255.255.255";
        //<=

	    public const int BS2_MAX_LIFT_LOCK_UNLOCK_ALARM_ACTION = 5;
	    public const int BS2_MAX_LIFTS_IN_LIFT_LOCK_UNLOCK_ZONE = 32;
	    public const int BS2_MAX_BYPASS_GROUPS_IN_LIFT_LOCK_UNLOCK_ZONE = 16;
	    public const int BS2_MAX_UNLOCK_GROUPS_IN_LIFT_LOCK_UNLOCK_ZONE = 16;

	    public const int BS2_DEVICE_ID_MIN = 0x01000000;
        public const int BS2_DEVICE_ID_MAX = 0x3FFFFFFF;

        public const int BS2_BARCODE_TIMEOUT_DEFAULT = 4;
        public const int BS2_BARCODE_TIMEOUT_MIN = BS2_BARCODE_TIMEOUT_DEFAULT;
        public const int BS2_BARCODE_TIMEOUT_MAX = 10;
        public const int BS2_VISUAL_BARCODE_TIMEOUT_DEFAULT = 10;
        public const int BS2_VISUAL_BARCODE_TIMEOUT_MIN = 3;
        public const int BS2_VISUAL_BARCODE_TIMEOUT_MAX = 20;

        // Intelligent PD
        public const int BS2_RS485_MAX_EXCEPTION_CODE_LEN = 8;
        public const int BS2_IPD_OUTPUT_CARDID = 0;
	    public const int BS2_IPD_OUTPUT_USERID = 1;

        // Device license
        public const int BS2_MAX_LICENSE_COUNT = 16;

        // OSDP standard
        public const int BS2_OSDP_STANDARD_ACTION_MAX_COUNT = 32;
        public const int BS2_OSDP_STANDARD_ACTION_MAX_LED = 2;
        public const int BS2_OSDP_STANDARD_KEY_SIZE = 16;
        public const int BS2_OSDP_STANDARD_MAX_DEVICE_PER_CHANNEL = 8;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Version
    {
        public byte major;
        public byte minor;
        public byte ext;
        public byte reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FactoryConfig
    {
        public UInt32 deviceID;        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAC_ADDR_LEN)]
        public byte[] macAddr;
        public UInt16 reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MODEL_NAME_LEN)]
        public byte[] modelName;
        public BS2Version boardVer;
        public BS2Version kernelVer;
        public BS2Version bscoreVer;
        public BS2Version firmwareVer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_KERNEL_REV_LEN)]
        public byte[] kernelRev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_BSCORE_REV_LEN)]
        public byte[] bscoreRev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_FIRMWARE_REV_LEN)]
        public byte[] firmwareRev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SystemConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 16 * 3)]
        public byte[] notUsed;
        public Int32 timezone; //offset of GMT in second
        public byte syncTime;
        public byte serverSync;
        public byte deviceLocked;
        public byte useInterphone;
        public byte useUSBConnection;
        public byte keyEncrypted;
        public byte useJobCode;
        public byte useAlphanumericID;
        public UInt32 cameraFrequency;
        public byte secureTamper;
        public byte reserved0;                  // write protected
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        public UInt32 useCardOperationMask;     // [+2.6.4]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AuthOperatorLevel
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] userID;
        public byte level;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AuthConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_NUM_OF_AUTH_MODE)]
        public UInt32[] authSchedule;
        public byte useGlobalAPB;
        public byte globalAPBFailAction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public byte[] reserved;
        public byte usePrivateAuth;
        public byte faceDetectionLevel;
        public byte useServerMatching;
        public byte useFullAccess;
        public byte matchTimeout;
        public byte authTimeout;
        public byte numOperators;
        public byte reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_OPERATORS)]
        public BS2AuthOperatorLevel[] operators;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AuthConfigExt
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_NUM_OF_EXT_AUTH_MODE)]
        public UInt32[] extAuthSchedule;
        public byte useGlobalAPB;
        public byte globalAPBFailAction;
	    public byte useGroupMatching;
        public byte reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved2;
        public byte usePrivateAuth;
        public byte faceDetectionLevel;
        public byte useServerMatching;
        public byte useFullAccess;
        public byte matchTimeout;
        public byte authTimeout;
        public byte numOperators;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] reserved3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_OPERATORS)]
        public BS2AuthOperatorLevel[] operators;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] reserved4;
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FaceConfigExt
    {
        public byte thermalCheckMode;
        public byte maskCheckMode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;

        public byte thermalFormat;
        public byte reserved2;

        public UInt16 thermalThresholdLow;
        public UInt16 thermalThresholdHigh;
        public byte maskDetectionLevel;
        public byte auditTemperature;

        public byte useRejectSound;
        public byte useOverlapThermal;
        public byte useDynamicROI;
        public byte faceCheckOrder;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ThermalCameraROI
    {
        public UInt16 x;
        public UInt16 y;
        public UInt16 width;
        public UInt16 height;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ThermalCameraConfig
    {
        public byte distance;
        public byte emissionRate;       // (emissivity)
        public BS2ThermalCameraROI roi;
        public byte useBodyCompensation;
        public sbyte compensationTemperature;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LedStatusConfig
    {
        public byte enabled;
        public byte reserved;
        public UInt16 count; //(0 = infinite)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_LED_SIGNAL_NUM)]
        public BS2LedSignal[] signal;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2BuzzerStatusConfig
    {
        public byte enabled;
        public byte reserved;
        public UInt16 count; //(0 = infinite)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_LED_SIGNAL_NUM)]
        public BS2BuzzerSignal[] signal;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2StatusConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_DEVICE_STATUS_NUM)]
        public BS2LedStatusConfig[] led;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_DEVICE_STATUS_NUM)]
        public BS2BuzzerStatusConfig[] buzzer;
        public byte configSyncRequired;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DisplayConfig
    {
        public UInt32 language;
        public byte background;
        public byte volume; //(0-100)
        public byte bgTheme;
        public byte dateFormat;
        public UInt16 menuTimeout;//(0-255 sec)
        public UInt16 msgTimeout;//(500-5000ms)
        public UInt16 backlightTimeout; //in seconds
        public byte displayDateTime;
        public byte useVoice;
        public byte timeFormat;
        public byte homeFormation;
        public byte useUserPhrase;
        public byte queryUserPhrase;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_SHORTCUT_HOME)]
        public byte[] shortcutHome;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TNA_KEY)]
        public byte[] tnaIcon;
        public byte useScreenSaver;         // FS2, F2
        public byte showOsdpResult;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]		// FISSDK-83 memory resizing bug when adding useScreenSaver (32->31)
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2IpConfig
    {
        public byte connectionMode;
        public byte useDHCP;
        public byte useDNS;
        public byte reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV4_ADDR_SIZE)]
        public byte[] ipAddress;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV4_ADDR_SIZE)]
        public byte[] gateway;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV4_ADDR_SIZE)]
        public byte[] subnetMask;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV4_ADDR_SIZE)]
        public byte[] serverAddr;
        public UInt16 port;
        public UInt16 serverPort;
        public UInt16 mtuSize;
        public byte baseband;
        public byte reserved2;
        public UInt16 sslServerPort;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public byte[] reserved3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2IpConfigExt
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV4_ADDR_SIZE)]
        public byte[] dnsAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_URL_SIZE)]
        public byte[] serverUrl;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TNAInfo
    {
        public byte tnaMode;
        public byte tnaKey;
        public byte tnaRequired;
        public byte reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TNA_KEY)]
        public UInt32[] tnaSchedule;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TNA_KEY)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TNAExtInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TNA_KEY * BS2Environment.BS2_MAX_TNA_LABEL_LEN)]
        public byte[] tnaLabel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TNA_KEY)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TNAConfig
    {
        public BS2TNAInfo tnaInfo;
        public BS2TNAExtInfo tnaExtInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2CSNCard
    {
        public byte type;
        public byte size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_CARD_DATA_SIZE)]
        public byte[] data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SmartCardHeader
    {
        public UInt16 hdrCRC;
        public UInt16 cardCRC;
        public byte cardType;
        public byte numOfTemplate;
        public UInt16 templateSize;
        public UInt16 issueCount;
        public byte duressMask;
        public byte cardAuthMode;
        public byte useAlphanumericID;
        public byte cardAuthModeEx;         // for FaceStation F2 only
        public byte numOfFaceTemplate;      // for FaceStation F2 only
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SmartCardCredentials
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_PIN_HASH_SIZE)]
        public byte[] pin;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_SMART_CARD_MAX_TEMPLATE_COUNT*BS2Environment.BS2_FINGER_TEMPLATE_SIZE)]
        public byte[] templateData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AccessOnCardData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_SMART_CARD_MAX_ACCESS_GROUP_COUNT)]
        public UInt16[] accessGroupID;
        public UInt32 startTime;
        public UInt32 endTime;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SmartCardData
    {
        public BS2SmartCardHeader header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_CARD_DATA_SIZE)]
        public byte[] cardID;
        public BS2SmartCardCredentials credentials;
        public BS2AccessOnCardData accessOnData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Card
    {
        public byte isSmartCard;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1656)]
        public byte[] cardUnion; // BS2CSNCard or BS2SmartCardData  
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2MifareCard
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] primaryKey;
        public UInt16 reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] secondaryKey;
        public UInt16 reserved2;
        public UInt16 startBlockIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2IClassCard
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] primaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] secondaryKey;
        public UInt16 startBlockIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DesFireCard
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] primaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] secondaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] appID;
        public byte fileID;
        public byte encryptionType;                 // 0: DES/3DES, 1: AES
        public byte operationMode;                  // 0: legacy(use picc master key), 1: new mode(use app master, file read, file write key)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DesFireAppLevelKey
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] appMasterKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] fileReadKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] fileWriteKey;
        public byte fileReadKeyNumber;
        public byte fileWriteKeyNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DesFireCardConfigEx
    {
        public BS2DesFireAppLevelKey desfireAppKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2CardConfig
    {
        // CSN
        public byte byteOrder;
        public byte useWiegandFormat;

        // Smart card
        public byte dataType;
        public byte useSecondaryKey;
        public BS2MifareCard mifare;
        public BS2IClassCard iclass;
        public BS2DesFireCard desfire;
        public UInt32 formatID; //card format ID, use only application for effective management

        public byte cipher;     // 1 byte (true : make card data from key) for XPASS - D2 KEYPAD

        public byte smartCardByteOrder;             // [+ V2.8.2.7]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Fingerprint
    {
        public byte index;
        public byte flag;
        public UInt16 reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE)]
        public byte[] data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2VerifyFingerprint
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] userID;
	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_FINGER_TEMPLATE_SIZE)]
	    public byte[] fingerTemplate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FingerprintConfig
    {
        public byte securityLevel;
        public byte fastMode;
        public byte sensitivity;
        public byte sensorMode;
        public byte templateFormat;
        public byte reserved;
        public UInt16 scanTimeout;
        public byte successiveScan;
        public byte advancedEnrollment;
        public byte showImage;
        public byte lfdLevel; //0: off, 1~3: on
        public byte checkDuplicate;    // [+2.6.4]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
        public byte[] reserved3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Rs485SlaveDevice
    {
        public UInt32 deviceID;
        public UInt16 deviceType;
        public byte enableOSDP;
        public byte connected;
    }

    // [+2.8]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2IntelligentPDInfo
    {
        public byte supportConfig;
        public byte useExceptionCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_EXCEPTION_CODE_LEN)]
        public byte[] exceptionCode;
        public byte outputFormat;
        public byte osdpID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Rs485Channel
    {
        public UInt32 baudRate;
        public byte channelIndex;
        public byte useRegistance;
        public byte numOfDevices;
        public byte reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_SLAVES_PER_CHANNEL)]
        public BS2Rs485SlaveDevice[] slaveDevices;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Rs485Config
    {
        public byte mode;
        public byte numOfChannels;
        public UInt16 reserved;
        public BS2IntelligentPDInfo intelligentInfo;            // [+2.8]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]   // [*2.8]  32->16
        public byte[] reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_CHANNELS)]
        public BS2Rs485Channel[] channels;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WiegandFormat
    {
        public UInt32 length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_WIEGAND_MAX_FIELDS * BS2Environment.BS2_WIEGAND_FIELD_SIZE)]
        public byte[] idFields;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_WIEGAND_MAX_PARITIES * BS2Environment.BS2_WIEGAND_FIELD_SIZE)]
        public byte[] parityFields;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_WIEGAND_MAX_PARITIES)]
        public byte[] parityType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_WIEGAND_MAX_PARITIES)]
        public byte[] parityPos;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WiegandConfig
    {
        public byte mode;
        public byte useWiegandBypass;
        public byte useFailCode;
        public byte failCode;
        public UInt16 outPulseWidth; //(20 ~ 100 us)
        public UInt16 outPulseInterval; //(200 ~ 20000 us)
        public UInt32 formatID;
        public BS2WiegandFormat format;
        public UInt16 wiegandInputMask; //(bitmask , no use 0 postion bit, 1~15 bit)
        public UInt16 wiegandCardMask; //(bitmask , no use 0 postion bit, 1~15 bit)
        public byte wiegandCSNIndex; //(1~15)
        public byte useWiegandUserID;           // [+ V2.7.2]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WiegandTamperInput
    {
        public UInt32 deviceID;
        public UInt16 port;
        public byte switchType;
        public byte reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WiegandLedOutput
    {
        public UInt32 deviceID;
        public UInt16 port;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WiegandBuzzerOutput
    {
        public UInt32 deviceID;
        public UInt16 port;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 34)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WiegandDeviceConfig
    {
        public BS2WiegandTamperInput tamper;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_WIEGAND_STATUS_NUM)]
        public BS2WiegandLedOutput[] led;
        public BS2WiegandBuzzerOutput buzzer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public UInt32[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SVInputRange
    {
        public UInt16 minValue; //0 ~ 3300 (0 ~ 3.3v)
        public UInt16 maxValue; //0 ~ 3300 (0 ~ 3.3v)
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SupervisedInputConfig
    {
        public BS2SVInputRange shortInput;
        public BS2SVInputRange openInput;
        public BS2SVInputRange onInput;
        public BS2SVInputRange offInput;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SupervisedInputConfigSet
    {
        public byte portIndex;
        public byte enabled;
        public byte supervised_index;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] reserved;
        public BS2SupervisedInputConfig config;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2InputConfig
    {
        public byte numInputs;
        public byte numSupervised;
        public UInt16 reseved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_INPUT_NUM)]
        public BS2SupervisedInputConfigSet[] supervised_inputs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WlanConfig
    {
        public byte enabled;
        public byte operationMode; // BS2WlanOperationModeEnum
        public byte authType; // BS2WlanAuthTypeEnum
        public byte encryptionType; // BS2WlanEncryptionTypeEnum
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_WLAN_SSID_SIZE)]
        public byte[] essid;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_WLAN_KEY_SIZE)]
        public byte[] authKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;
    }   

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EventTrigger
    {
        public UInt16 code;
        public UInt16 reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2InputTrigger
    {
        public byte port;
        public byte switchType;
        public UInt16 duration;
        public UInt32 scheduleID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ScheduleTrigger
    {
        public UInt32 type;
        public UInt32 scheduleID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Trigger
    {
        public UInt32 deviceID;
        public byte type;
        public byte reserved;
        public UInt16 ignoreSignalTime;             // [+ 2.9.6]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] triggerUnion; //BS2XXXXTrigger
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Signal
    {
        public UInt32 signalID;
        public UInt16 count;
        public UInt16 onDuration;
        public UInt16 offDuration;
        public UInt16 delay;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OutputPortAction
    {
        public byte portIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        public BS2Signal signal;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2RelayAction
    {
        public byte relayIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        public BS2Signal signal;
    }

    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct BS2ReleaseAlarmUnion
    {
        [FieldOffset(0)]
        public UInt32 deviceID;

        [FieldOffset(0)]
        public UInt32 doorID;

        [FieldOffset(0)]
        public UInt32 zoneID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ReleaseAlarmAction
    {
        public byte targetType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        public BS2ReleaseAlarmUnion releaseAlarm;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LedSignal
    {
        public byte color;
        public byte reserved;
        public UInt16 duration;
        public UInt16 delay;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LedAction
    {
        public UInt16 count; //(0 = infinite)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_LED_SIGNAL_NUM)]
        public BS2LedSignal[] signal;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2BuzzerSignal
    {
        public byte tone;
        public byte fadeout;
        public UInt16 duration;
        public UInt16 delay;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2BuzzerAction
    {
        public UInt16 count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_BUZZER_SIGNAL_NUM)]
        public BS2BuzzerSignal[] signal;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DisplayAction
    {
        public byte duration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        public UInt32 displayID;
        public UInt32 resourceID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SoundAction
    {
        public byte count;
        public byte unused;             // padding      BS2AFW-241
        public UInt16 soundIndex;
        public UInt16 delay;        ///< deprecated
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] reserved;
    }

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LiftAction
    {
        public UInt32 liftID;
        public byte type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;     // [+ V2.8]
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Action
    {
        public UInt32 deviceID;
        public byte type;
        public byte stopFlag;
        public UInt16 delay;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] actionUnion; // BS2XXXAction  
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TriggerAction
    {
        public BS2Trigger trigger;
        public BS2Action action;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TriggerActionConfig
    {
        public byte numItems;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TRIGGER_ACTION)]
        public BS2TriggerAction[] items;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ImageEventFilter
    {
        public byte mainEventCode;
        public byte subEventCode;               // [+ V2.8]
        public byte subCodeIncluded;            // [+ V2.8]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] reserved2;
        public UInt32 scheduleID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EventConfig
    {
        public UInt32 numImageEventFilter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_EVENT_MAX_IMAGE_CODE_COUNT)]
        public BS2ImageEventFilter[] imageEventFilter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] unused;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WiegandInConfig
    {
        public UInt32 formatID;
        public BS2WiegandFormat format;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WiegandMultiConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.MAX_WIEGAND_IN_COUNT)]
        public BS2WiegandInConfig[] formats;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS1CardConfig
    {
        public UInt32 magicNo;
        public UInt32 disabled;
        public UInt32 useCSNOnly;
        public UInt32 bioentryCompatible;
        public UInt32 useSecondaryKey;
        public UInt32 reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] primaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] secondaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved3;
        public UInt32 cisIndex;
        public UInt32 numOfTemplate;
        public UInt32 templateSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt32[] templateStartBlock;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public byte[] reserve4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SystemConfigExt
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] primarySecureKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] secondarySecureKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2UserPhoneItem
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] phoneNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DESCRIPTION_NAME_LEN)]
        public byte[] descript;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2VoipConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_URL_SIZE)]
        public byte[] serverUrl;
        public UInt16 serverPort;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] userID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] userPW;
        public byte exitButton;
        public byte dtmfMode;
        public byte bUse;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reseverd;
        public UInt32 numPhonBook;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_VOIP_MAX_PHONEBOOK)]
        public BS2UserPhoneItem[] phonebook;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FaceWidth
    {
        public UInt16 min;
        public UInt16 max;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SearchRange
    {
        public UInt16 x;
        public UInt16 width;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DetectDistance
    {
        public byte min;
        public byte max;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FaceConfig
    {
        public byte securityLevel;
        public byte lightCondition;
        public byte enrollThreshold;
        public byte detectSensitivity;
        public UInt16 enrollTimeout;
        public byte lfdLevel;
        public byte	quickEnrollment;			    // [+ 2.6.4]
        public byte previewOption;			        // [+ 2.6.4]

        public byte checkDuplicate;                 // [+ 2.6.4]
        public byte operationMode;                  // [+ 2.7.1]        FSF2 support
        public byte maxRotation;                    // [+ 2.7.1]        FSF2 support
        public BS2FaceWidth faceWidth;              // [+ 2.7.1]        FSF2 support
        public BS2SearchRange searchRange;          // [+ 2.7.1]        FSF2 support
        public BS2DetectDistance detectDistance;	// [+ 2.8.3]        BS3 support
        public byte wideSearch;                 	// [+ 2.8.3]        BS3 support

        public byte unused;

        public byte unableToSaveImageOfVisualFace;  // [+ 2.9.6]        Save template only
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Rs485SlaveDeviceEX
    {
        public UInt32 deviceID;
        public UInt16 deviceType;
        public byte enableOSDP;
        public byte connected;
        public byte channelInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reseverd;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Rs485ChannelEX
    {
        public UInt32 baudRate;
        public byte channelIndex;
        public byte useRegistance;
        public byte numOfDevices;
        public byte channelType;			        // + 2.9.1
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_SLAVES_PER_CHANNEL)]
        public BS2Rs485SlaveDeviceEX[] slaveDevices;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Rs485ConfigEX
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] mode;
        public UInt16 numOfChannels;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reseverd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_CHANNELS_EX)]
        public BS2Rs485ChannelEX[] channels;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SeosCard
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] oid_ADF;
        public byte size_ADF;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] oid_DataObjectID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UInt16[] size_DataObject;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] primaryKeyAuth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] secondaryKeyAuth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2CardConfigEx
    {
        public BS2SeosCard seos;        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] reserved;
    }

    #region DEVICE_ZONE_SUPPORTED
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneMasterConfig
    {
        public byte enable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        byte[] reserved1;
        public UInt16 listenPort;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        byte[] reserved;
    }

    #region ENTRANCE_LIMIT
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneEntranceLimitMemberInfo
    {
        public UInt32 readerID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneEntranceLimitMaster
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;

        public byte type; //BS2_DEVICE_ZONE_ENTRANCE_LIMIT_TYPE
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        byte[] reserved1;

        public UInt32 entryLimitInterval_s;

        public byte numEntranceLimit;
        public byte numReaders;
        public byte numAlarm;
        public byte numBypassGroups;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE)]
        public byte[] maxEntry;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE)]
        public UInt32[] periodStart_s;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE)]
        public UInt32[] periodEnd_s;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_READERS_PER_DEVICE_ZONE_ENTRANCE_LIMIT)]
        public BS2DeviceZoneEntranceLimitMemberInfo[] readers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DEVICE_ZONE_ENTRANCE_LIMIT_ALARM_ACTION)]
        public BS2Action[] alarm;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_DEVICE_ZONE_ENTRANCE_LIMIT)]
        public UInt32[] bypassGroupIDs;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * 4)]
        byte[] reserved3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneAGEntranceLimit
    {
        public UInt32 zoneID;
        public UInt16 numAGEntranceLimit;
        public UInt16 reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE)]
        public UInt32[] periodStart_s;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE)]
        public UInt32[] periodEnd_s;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE)]
        public UInt16[] numEntry;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE * BS2Environment.BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT)]
        public UInt16[] maxEntry;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ENTRANCE_LIMIT_PER_ZONE * BS2Environment.BS2_MAX_ACCESS_GROUP_ENTRANCE_LIMIT_PER_ENTRACE_LIMIT)]
        public UInt32[] accessGroupID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneEntranceLimitMember
    {
        public UInt16 masterPort;
        public byte actionInDisconnect; //BS2_DEVICE_ZONE_ENTRANCE_LIMIT_DISCONNECTED_ACTION_TYPE
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        byte[] reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV4_ADDR_SIZE)]
        public byte[] masterIP;
    }
    #endregion

    #region FIRE_ALARM
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneFireSensor
    {
        public UInt32 deviceID;
        public byte port;
        public byte switchType;     //BS2SwitchTypeEnum
        public UInt16 duration;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneFireAlarmMemberInfo
    {
        public UInt32 readerID;			///< 4 bytes
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneFireAlarmMaster
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;

        public byte numReaders;
        public byte numAlarm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        byte[] reserved1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_READERS_PER_DEVICE_ZONE_FIRE_ALARM)]
        public BS2DeviceZoneFireAlarmMemberInfo[] readers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DEVICE_ZONE_FIRE_ALARM_ALARM_ACTION)]
        public BS2Action[] alarm;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * 40)]
        byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceZoneFireAlarmMember
    {
        public UInt16 masterPort;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        byte[] reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV4_ADDR_SIZE)]
        public byte[] masterIP;

        public byte numSensors;
        public byte numDoors;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        byte[] reserved2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_FIRE_SENSORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER)]
        public BS2DeviceZoneFireSensor[] sensor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DOORS_PER_DEVICE_ZONE_FIRE_ALARM_MEMBER)]
        public UInt32[] doorIDs;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        byte[] reserved3;
    }
    #endregion

    public struct BS2DeviceZone
    {
        public UInt32 zoneID;						///< 4 bytes
        public byte zoneType; //BS2_DEVICE_ZONE_TYPE
        public byte nodeType; //BS2_DEVICE_ZONE_NODE_TYPE
        public byte enable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 884)]
        // Entrance Limit BS2DeviceZoneEntranceLimitMaster or BS2DeviceZoneEntranceLimitMember      
        public byte[] zoneUnion;
    }

    public struct BS2DeviceZoneConfig
    {
        public Int32 numOfZones; //0 ~ BS_MAX_ZONE_PER_NODE
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DEVICE_ZONE)]
        public BS2DeviceZone[] zone;
    } ;

    #endregion

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Configs
    {
        public UInt32 configMask;
        public BS2FactoryConfig factoryConfig;
        public BS2SystemConfig systemConfig;
        public BS2AuthConfig authConfig;
        public BS2StatusConfig statusConfig;
        public BS2DisplayConfig displayConfig;
        public BS2IpConfig ipConfig;
        public BS2IpConfigExt ipConfigExt;
        public BS2TNAConfig tnaConfig;
        public BS2CardConfig cardConfig;
        public BS2FingerprintConfig fingerprintConfig;
        public BS2Rs485Config rs485Config;
        public BS2WiegandConfig wiegandConfig;
        public BS2WiegandDeviceConfig wiegandDeviceConfig;
        public BS2InputConfig inputConfig;
        public BS2WlanConfig wlanConfig;
        public BS2TriggerActionConfig triggerActionConfig;
        public BS2EventConfig eventConfig;
        public BS2WiegandMultiConfig wiegandMultiConfig;
        public BS1CardConfig card1xConfig;
        public BS2SystemConfigExt systemExtConfig;
        public BS2VoipConfig voipConfig;
        public BS2FaceConfig faceConfig;        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DoorRelay
    {
        public UInt32 deviceID;
        public byte port;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DoorSensor
    {
        public UInt32 deviceID;
        public byte port;
        public byte switchType;
        public byte apbUseDoorSensor;	// [+2.7.0]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ExitButton
    {
        public UInt32 deviceID;
        public byte port;
        public byte switchType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DoorStatus
    {
        public UInt32 id;
        public byte opened;
        public byte unlocked;
        public byte heldOpened;
        public byte unlockFlags;
        public byte lockFlags;
        public byte alarmFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        public UInt32 lastOpenTime;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BS2Door
    {
        public UInt32 doorID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DOOR_NAME_LEN)]
        public byte[] name;
        public UInt32 entryDeviceID;
        public UInt32 exitDeviceID;
        public BS2DoorRelay relay;
        public BS2DoorSensor sensor;
        public BS2ExitButton button;
        public UInt32 autoLockTimeout;
        public UInt32 heldOpenTimeout;
        public byte instantLock;
        public byte unlockFlags;
        public byte lockFlags;
        public byte unconditionalLock; //alarmFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_FORCED_OPEN_ALARM_ACTION)]
        public BS2Action[] forcedOpenAlarm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_HELD_OPEN_ALARM_ACTION)]
        public BS2Action[] heldOpenAlarm;
        public UInt32 dualAuthScheduleID;
        public byte dualAuthDevice;
        public byte dualAuthApprovalType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;         // [+ V2.8]
        public UInt32 dualAuthTimeout;
        public byte numDualAuthApprovalGroups;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DUAL_AUTH_APPROVAL_GROUP)]
        public UInt32[] dualAuthApprovalGroupID;
        public BS2AntiPassbackZone apbZone;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ResourceItem
    {
        public byte index;
        public UInt32 dataLen;
        public IntPtr data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ResourceElement
    {
        public byte type;
        public UInt32 numResData;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_RESOURCE_ITEM_COUNT)]
        public BS2ResourceItem[] resData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Face
    {
        public byte faceIndex;
        public byte numOfTemplate;
        public byte flag;
        public byte reserved;
        public UInt16 imageLen;
        public UInt16 reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_FACE_IMAGE_SIZE)]
        public byte[] imageData;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_TEMPLATE_PER_FACE * BS2Environment.BS2_FACE_TEMPLATE_LENGTH)]
        public byte[] templateData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SimpleDeviceInfo
    {
        public UInt32 id;
        public UInt16 type;
        public byte connectionMode;
        public UInt32 ipv4Address;
        public UInt16 port;
        public UInt32 maxNumOfUser;
        public byte userNameSupported;
        public byte userPhotoSupported;
        public byte pinSupported;
        public byte cardSupported;
        public byte fingerSupported;
        public byte faceSupported;
        public byte wlanSupported;
        public byte tnaSupported;
        public byte triggerActionSupported;
        public byte wiegandSupported;
        public byte imageLogSupported;
        public byte dnsSupported;
        public byte jobCodeSupported;        
        public byte wiegandMultiSupported;
        public byte rs485Mode;
        public byte sslSupported;
        public byte rootCertExist;
        public byte dualIDSupported;
        public byte useAlphanumericID;
        public UInt32 connectedIP;
        public byte phraseCodeSupported;
        public byte card1xSupported;
        public byte systemExtSupported;
        public byte voipSupported;
        public byte rs485ExSupported;
        public byte cardExSupported;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2SimpleDeviceInfoEx
    {
        public UInt32 supported;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2User
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] userID;
        public byte formatVersion;
        public byte flag;
        public UInt16 version;
        public byte numCards;
        public byte numFingers;
        public byte numFaces;
        public byte infoMask;       // [+V2.8.3]
#if OLD_CODE
        public UInt32 fingerChecksum;
#else
        public UInt32 authGroupID;
#endif
        public UInt32 faceChecksum;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2UserSetting
    {
        public UInt32 startTime;
        public UInt32 endTime;
        public byte fingerAuthMode;
        public byte cardAuthMode;
        public byte idAuthMode;
        public byte securityLevel;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2UserPhoto
    {
        public UInt32 size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_PHOTO_SIZE)]
        public byte[] data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2JobData
    {
        public UInt32 code;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_JOBLABEL_LEN)]
        public byte[] label;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Job
    {
        public byte numJobs;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_JOB_SIZE)]
        public BS2JobData[] jobs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2UserBlob
    {
        public BS2User user;
        public BS2UserSetting setting;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_NAME_LEN)]
        public byte[] name;
        public BS2UserPhoto photo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_PIN_HASH_SIZE)]
        public byte[] pin;
        public IntPtr cardObjs;
        public IntPtr fingerObjs;
        public IntPtr faceObjs;        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER)]
        public UInt32[] accessGroupId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2UserBlobEx
    {
        public BS2User user;
        public BS2UserSetting setting;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_NAME_LEN)]
        public byte[] name;
        public BS2UserPhoto photo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_PIN_HASH_SIZE)]
        public byte[] pin;
        public IntPtr cardObjs;
        public IntPtr fingerObjs;
        public IntPtr faceObjs;
        public BS2Job job;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_PHRASE_SIZE)]
        public byte[] phrase;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER)]
        public UInt32[] accessGroupId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]     // [+V2.8.3]
    public struct BS2UserStatistic
    {
        public UInt32 numUsers;
        public UInt32 numCards;
        public UInt32 numFingerprints;
        public UInt32 numFaces;
        public UInt32 numNames;
        public UInt32 numImages;
        public UInt32 numPhrases;
    }

    [StructLayout(LayoutKind.Explicit, Size = BS2Environment.BS2_USER_ID_SIZE)]
    public struct BS2EventDetail
    {
        [FieldOffset(0)] public UInt32 doorID;

        [FieldOffset(0)] public UInt32 liftID;

        [FieldOffset(0)] public UInt32 zoneID;

        // IO
        [FieldOffset(0)] public UInt32 ioDeviceID;
        [FieldOffset(4)] public UInt16 port;
        [FieldOffset(6)] public byte value;

        // Alarm
        [FieldOffset(0)] public UInt32 alarmZoneID;
        [FieldOffset(4)] public UInt32 alarmDoorID;
        [FieldOffset(8)] public UInt32 alarmIoDeviceID;
        [FieldOffset(12)] public UInt16 alarmPort;

        // Interlock
        [FieldOffset(0)] public UInt32 interlockZoneID;
        [FieldOffset(4)] public UInt32 interlockDoorID_0;
        [FieldOffset(8)] public UInt32 interlockDoorID_1;
        [FieldOffset(12)] public UInt32 interlockDoorID_2;
        [FieldOffset(16)] public UInt32 interlockDoorID_3;

        // RelayAction
        [FieldOffset(0)] public UInt16 relayActionRelayPort;
        [FieldOffset(2)] public UInt16 relayActionInputPort;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Event
    {
        public UInt32 id;
        public UInt32 dateTime;
        public UInt32 deviceID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] userID;

        public UInt16 code;
        public byte param;
        public byte image;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EventExtIoDevice
    {
        public UInt32 ioDeviceID;
        public UInt16 port;
        public byte value;
        public byte reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EventExtUnion
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] userID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_CARD_DATA_SIZE)]
        public byte[] cardID;

        public UInt32 doorID;
        public UInt32 zoneID;
        public BS2EventExtIoDevice ioDevice;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EventBlob
    {
        public UInt16 eventMask;
        public UInt32 id;
        public BS2EventExtInfo info;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] objectID; //BS2EventExtUnion

        public byte tnaKey;
        public UInt32 jobCode;
        public UInt16 imageSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_EVENT_MAX_IMAGE_SIZE)]
        public byte[] image;
        public byte reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EventSmallBlob
    {
        public UInt16 eventMask;
        public UInt32 id;
        public BS2EventExtInfo info;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] objectID; //BS2EventExtUnion

        public byte tnaKey;
        public UInt32 jobCode;
        public UInt16 imageSize;
        public IntPtr imageObj;
        public byte reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EventSmallBlobEx
    {
        public UInt16 eventMask;
        public UInt32 id;
        public BS2EventExtInfo info;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] objectID; //BS2EventExtUnion

        public byte tnaKey;
        public UInt32 jobCode;
        public UInt16 imageSize;
        public IntPtr imageObj;
        public byte reserved;
	    public UInt32 temperature;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EventExtInfo
    {
        public UInt32 dateTime;
        public UInt32 deviceID;
        public UInt16 code;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;

    }  

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2UserAccessGroups
    {
        public byte numAccessGroups;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER)]
        public UInt32[] accessGroupID;
    }

    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct BS2NumOfLevelUnion
    {
        [FieldOffset(0)]
        public byte numAccessLevels;

        [FieldOffset(0)]
        public byte numFloorLevels;
    }

    [StructLayout(LayoutKind.Explicit, Size = 512)]
    public struct BS2LevelUnion
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_LEVEL_PER_ACCESS_GROUP)]
        public UInt32[] accessLevels;

        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_LEVEL_PER_ACCESS_GROUP)]
        public UInt32[] floorLevels;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AccessGroup
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN)]
        public byte[] name;
        public BS2NumOfLevelUnion numOflevelUnion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        public BS2LevelUnion levelUnion;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DoorSchedule
    {
        public UInt32 doorID;
        public UInt32 scheduleID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AccessLevel
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_LEVEL_NAME_LEN)]
        public byte[] name;
        public byte numDoorSchedules;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_LEVEL_ITEMS)]
        public BS2DoorSchedule[] doorSchedules;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TimePeriod
    {
        public UInt16 startTime;
        public UInt16 endTime;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DaySchedule
    {
        public byte numPeriods;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TIME_PERIODS_PER_DAY)]
        public BS2TimePeriod[] periods;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WeeklySchedule
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_NUM_WEEKDAYS)]
        public BS2DaySchedule[] schedule;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DailySchedule
    {
        public UInt32 startDate;
        public byte numDays;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DAYS_PER_DAILY_SCHEDULE)]
        public BS2DaySchedule[] schedule;
    }

    //Discarded
    //    [StructLayout(LayoutKind.Explicit, Size = 2168)]
    //    public struct BS2ScheduleUnion
    //    {   
    //        [FieldOffset(0)]
    //        public BS2DailySchedule daily;
    //
    //        [FieldOffset(0)]
    //        public BS2WeeklySchedule weekly;
    //    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2HolidaySchedule
    {
        public UInt32 id;
        public BS2DaySchedule schedule;
    }

    //Discarded
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public struct BS2Schedule
    //{
    //    public UInt32 id;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_SCHEDULE_NAME_LEN)]
    //    public byte[] name;
    //    public byte isDaily;
    //    public byte numHolidaySchedules;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public byte[] reserved;
    //    public BS2ScheduleUnion scheduleUnion;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_HOLIDAY_GROUPS_PER_SCHEDULE)]
    //    public BS2HolidaySchedule[] holidaySchedules;
    //}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Holiday
    {
        public UInt32 date;        
        public byte recurrence;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
    }

    public struct BS2HolidayGroup
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_HOLIDAY_GROUP_NAME_LEN)]
        public byte[] name;
        public byte numHolidays;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_HOLIDAYS_PER_GROUP)]
        public BS2Holiday[] holidays;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2BlackList
    {        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_CARD_DATA_SIZE)]
        public byte[] cardID;
        public UInt16 issueCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ZoneStatus
    {
        public UInt32 id;
        public byte status; //BS2ZoneStatusEnum
        public byte disabled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ApbMember
    {
        public UInt32 deviceID;
        public byte type; //BS2APBZoneReaderTypeEnum
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AntiPassbackZone
    {
        public UInt32 zoneID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;
        public byte type; //BS2APBZoneTypeEnum
        public byte numReaders;
        public byte numBypassGroups;
        public byte disabled;
        public byte alarmed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        public UInt32 resetDuration; //in seconds, 0: no reset
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_APB_ALARM_ACTION)]
        public BS2Action[] alarm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_READERS_PER_APB_ZONE)]
        public BS2ApbMember[] readers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_APB_ZONE)]
        public UInt32[] bypassGroupIDs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TimedApbMember
    {
        public UInt32 deviceID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TimedAntiPassbackZone
    {
        public UInt32 zoneID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;
        public byte type; //BS2TimedAPBZoneTypeEnum
        public byte numReaders;
        public byte numBypassGroups;
        public byte disabled;
        public byte alarmed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        public UInt32 resetDuration; //in seconds, 0: no reset
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TIMED_APB_ALARM_ACTION)]
        public BS2Action[] alarm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_READERS_PER_TIMED_APB_ZONE)]
        public BS2TimedApbMember[] readers;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 320)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_BYPASS_GROUPS_PER_TIMED_APB_ZONE)]
        public UInt32[] bypassGroupIDs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FireSensor
    {
        public UInt32 deviceID;
        public byte port;
        public byte switchType; //BS2SwitchTypeEnum
        public UInt16 duration;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FireAlarmZone
    {
        public UInt32 zoneID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;
        public byte numSensors;
        public byte numDoors;
        public byte alarmed;
        public byte disabled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_FIRE_SENSORS_PER_FIRE_ALARM_ZONE)]
        public BS2FireSensor[] sensor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_FIRE_ALARM_ACTION)]
        public BS2Action[] alarm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DOORS_PER_FIRE_ALARM_ZONE)]
        public UInt32[] doorIDs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ScheduledLockUnlockZone
    {
        public UInt32 zoneID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;
        public UInt32 lockScheduleID;
        public UInt32 unlockScheduleID;
        public byte numDoors;
        public byte numBypassGroups;
        public byte numUnlockGroups;
        public byte bidirectionalLock;
        public byte disabled;
        public byte alarmed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_SCHEDULED_LOCK_UNLOCK_ALARM_ACTION)]
        public BS2Action[] alarm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DOORS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)]
        public UInt32[] doorIDs;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_BYPASS_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)]
        public UInt32[] bypassGroupIDs;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_UNLOCK_GROUPS_IN_SCHEDULED_LOCK_UNLOCK_ZONE)]
        public UInt32[] unlockGroupIDs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LiftFloors
    {
        public UInt32 liftID;
        public UInt16 numFloors;
        public UInt16 reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] floorIndices;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LiftLockUnlockZone
    {
        public UInt32 zoneID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;
        public UInt32 unlockScheduleID;
        public UInt32 lockScheduleID;
        public byte numLifts;
        public byte numBypassGroups;
        public byte numUnlockGroups;
        public byte unused;
        public byte disabled;
        public byte alarmed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_LIFT_LOCK_UNLOCK_ALARM_ACTION)]
        public BS2Action[] alarm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_LIFTS_IN_LIFT_LOCK_UNLOCK_ZONE)]
        public BS2LiftFloors[] lifts;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_BYPASS_GROUPS_IN_LIFT_LOCK_UNLOCK_ZONE)]
        public UInt32[] bypassGroupIDs;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_UNLOCK_GROUPS_IN_LIFT_LOCK_UNLOCK_ZONE)]
        public UInt32[] unlockGroupIDs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceNode
    {
        public UInt32 parentDeviceID;
        public UInt32 deviceID;
        public UInt16 deviceType;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AuthGroup
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_AUTH_GROUP_NAME_LEN)]
        public byte[] name;        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FloorSchedule
    {
        public UInt32 liftID;
        public UInt16 floorIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        public UInt32 scheduleID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FloorLevel
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_FLOOR_LEVEL_NAME_LEN)]
        public byte[] name;
        public byte numFloorSchedules;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_FLOOR_LEVEL_ITEMS)]
        public BS2FloorSchedule[] floorSchedules;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FloorStatus
    {
        public byte activated;
        public byte activateFlags;
        public byte deactivateFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LiftFloor
    {
        public UInt32 deviceID;
        public byte port;
        public BS2FloorStatus status;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LiftSensor
    {
        public UInt32 deviceID;
        public byte port;
        public byte switchType;
        public UInt16 duration;
        public UInt32 scheduleID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LiftAlarm
    {
        public BS2LiftSensor sensor;
        public BS2Action action;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2Lift
    {
        public UInt32 liftID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_LIFT_NAME_LEN)]
        public byte[] name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DEVICES_ON_LIFT)]
        public UInt32[] deviceID;
        public UInt32 activateTimeout;
        public UInt32 dualAuthTimeout;
        public byte numFloors;
        public byte numDualAuthApprovalGroups;
        public byte dualAuthApprovalType;
        public byte tamperOn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DEVICES_ON_LIFT)]
        public byte[] dualAuthRequired;
        public UInt32 dualAuthScheduleID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_FLOORS_ON_LIFT)]
        public BS2LiftFloor[] floor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DUAL_AUTH_APPROVAL_GROUP_ON_LIFT)]
        public UInt32[] dualAuthApprovalGroupID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ALARMS_ON_LIFT)]
        public BS2LiftAlarm[] alarm;
        public BS2LiftAlarm tamper;
        public byte alarmFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LiftStatus
    {
        public UInt32 liftID;
        public UInt16 numFloors;
        public byte alarmFlags;
        public byte tamperOn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_FLOORS_ON_LIFT)]
        public BS2FloorStatus[] floors;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CSP_BS2ScheduleUnion
    {
        public BS2DailySchedule daily;

        public BS2WeeklySchedule weekly;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CSP_BS2Schedule
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_SCHEDULE_NAME_LEN)]
        public byte[] name;
        public byte isDaily;
        public byte numHolidaySchedules;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        public CSP_BS2ScheduleUnion scheduleUnion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_HOLIDAY_GROUPS_PER_SCHEDULE)]
        public BS2HolidaySchedule[] holidaySchedules;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CXX_BS2Schedule
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_SCHEDULE_NAME_LEN)]
        public byte[] name;
        public byte isDaily;
        public byte numHolidaySchedules;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2168)]       
        public byte[] scheduleUnion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_HOLIDAY_GROUPS_PER_SCHEDULE)]
        public BS2HolidaySchedule[] holidaySchedules;
    }

    public class Translator<TSource, TOutput>  //: TranslatorBase<TSource, TOutput>
    {
        public void Translate<TSource, TOutput>(ref TSource src, ref TOutput output)
        {
            Util.TranslatePrimitive<TSource, TOutput>(ref src, ref output);
            try
            {
                Type type = typeof(Translator<TSource, TOutput>);
                MethodInfo extTranslate = type.GetMethod("Translate_", new Type[] { typeof(TSource).MakeByRefType(), typeof(TOutput).MakeByRefType()} );
                if (extTranslate != null)
                {
                    output = (TOutput)extTranslate.Invoke(this, new object[] { src, output });
                }
            }
            finally
            {

            }
        }

        public CXX_BS2Schedule Translate_(ref CSP_BS2Schedule src, ref CXX_BS2Schedule output)
        {
            Util.TranslatePrimitive<CSP_BS2Schedule, CXX_BS2Schedule>(ref src, ref output);
            byte[] bytes = new byte[0];
            if (src.isDaily != 0)
                bytes = Util.StructToBytes<BS2DailySchedule>(ref src.scheduleUnion.daily);
            else
                bytes = Util.StructToBytes<BS2WeeklySchedule>(ref src.scheduleUnion.weekly);
            Array.Clear(output.scheduleUnion, 0, output.scheduleUnion.Length);
            Array.Copy(bytes, output.scheduleUnion, bytes.Length);

            return output;
        }

        public CSP_BS2Schedule Translate_(ref CXX_BS2Schedule src, ref CSP_BS2Schedule output)
        {
            Util.TranslatePrimitive<CXX_BS2Schedule, CSP_BS2Schedule>(ref src, ref output);
            if (src.isDaily != 0)
                output.scheduleUnion.daily = Util.BytesToStruct<BS2DailySchedule>(ref src.scheduleUnion);
            else
                output.scheduleUnion.weekly = Util.BytesToStruct<BS2WeeklySchedule>(ref src.scheduleUnion);

            return output;
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2IntrusionAlarmZone
    {
        public UInt32 zoneID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;
        public byte armDelay;
        public byte alarmDelay;
        public byte disabled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] reserved;
        public byte numReaders;
        public byte numInputs;
        public byte numOutputs;
        public byte numCards;
        public byte numDoors;
        public byte numGroups;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2IntrusionAlarmZoneBlob
    {
        public BS2IntrusionAlarmZone IntrusionAlarmZone;
        public IntPtr memberObjs;
        public IntPtr inputObjs;
        public IntPtr outputObjs;
        public IntPtr cardObjs;
        public IntPtr doorIDs;
        public IntPtr groupIDs;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AlarmZoneMember
    {
        public UInt32 deviceID;
        public byte inputType;
        public byte operationType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AlarmZoneInput
    {
        public UInt32 deviceID;
        public byte port;
        public byte switchType;
        public UInt16 duration;
        public byte operationType;       
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AlarmZoneOutput
    {
        public UInt16 eventcode;        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        public BS2Action action;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2WeekTime
    {
        public UInt16 year;
        public byte month;
        public sbyte ordinal;
        public byte weekDay;
        public byte hour;
        public byte minute;
        public byte second;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DstSchedule
    {
        public BS2WeekTime startTime;
        public BS2WeekTime endTime;
        public Int32 timeOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DstConfig
    {
        public byte numSchedules;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DST_SCHEDULE)]
        public BS2DstSchedule[] schedules;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2EncryptKey
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_ENC_KEY_SIZE)]
        public byte[] key;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;
    }
	
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2InterlockZoneInput
    {
        public UInt32 deviceID;
        public byte port;
        public byte switchType;
        public UInt16 duration;
        public byte operationType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2InterlockZoneOutput
    {
        public UInt16 eventcode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        public BS2Action action;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2InterlockZone
    {
        public UInt32 zoneID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ZONE_NAME_LEN)]
        public byte[] name;
        public byte disabled;
        public byte numInputs;
        public byte numOutputs;
        public byte numDoors;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2InterlockZoneBlob
    {
        public BS2InterlockZone InterlockZone;
        public IntPtr inputObjs;
        public IntPtr outputObjs;
        public IntPtr doorIDs;
    }

    //=> [IPv6]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BS2IPV6Config
    {
        public byte useIPV6;
        public byte reserved1; 	//Not yet apply //useIPV4;
        public byte useDhcpV6;
        public byte useDnsV6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE)]
        public byte[] staticIpAddressV6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE)]
        public byte[] staticGatewayV6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE)]
        public byte[] dnsAddrV6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE)]
        public byte[] serverIpAddressV6;
        public UInt16 serverPortV6;
        public UInt16 sslServerPortV6;		
        public UInt16 portV6;
        public byte numOfAllocatedAddressV6;
        public byte numOfAllocatedGatewayV6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE * BS2Environment.BS2_MAX_IPV6_ALLOCATED_ADDR)]
        public byte[] allocatedIpAddressV6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE * BS2Environment.BS2_MAX_IPV6_ALLOCATED_ADDR)]
        public byte[] allocatedGatewayV6;
    }
    //<=

    //=> [IPv6]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BS2IPv6DeviceInfo
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] reserved;
        public byte bIPv6Mode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE)]
        public byte[] ipv6Address;
        public UInt16 portV6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE)]	
        public byte[] connectedIPV6;	
        public byte numOfAllocatedAddressV6;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_IPV6_ADDR_SIZE * BS2Environment.BS2_MAX_IPV6_ALLOCATED_ADDR )]
        public byte[] allocatedIpAddressV6; 
    }
    //<=

    //User Small Blob
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BS2UserSmallBlob
    {
        public BS2User user;
        public BS2UserSetting setting;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_NAME_LEN)]
        public byte[] name;
        public IntPtr user_photo_obj; //BS2UserPhoto
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_PIN_HASH_SIZE)]
        public byte[] pin;
        public IntPtr cardObjs;
        public IntPtr fingerObjs;
        public IntPtr faceObjs;        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER)]
        public UInt32[] accessGroupId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BS2UserSmallBlobEx
    {
        public BS2User user;
        public BS2UserSetting setting;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_NAME_LEN)]
        public byte[] name;
        public IntPtr user_photo_obj;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_PIN_HASH_SIZE)]
        public byte[] pin;
        public IntPtr cardObjs;
        public IntPtr fingerObjs;
        public IntPtr faceObjs;
        public BS2Job job;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_PHRASE_SIZE)]
        public byte[] phrase;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER)]
        public UInt32[] accessGroupId;
    }
    //<=

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2UserSettingEx
    {
        public byte faceAuthMode;
        public byte fingerprintAuthMode;
        public byte cardAuthMode;
        public byte idAuthMode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2TemplateEx
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_FACE_EX_TEMPLATE_SIZE)]
        public byte[] data;
        public byte isIR;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FaceExWarped
    {
        public byte faceIndex;
        public byte numOfTemplate;
        public byte flag;
        public byte reserved;
        public UInt32 imageLen;
        public UInt16 irImageLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] unused;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_WARPED_IMAGE_LENGTH)]
        public byte[] imageData;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_WARPED_IR_IMAGE_LENGTH)]
        public byte[] irImageData;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_TEMPLATES_PER_FACE_EX)]
        public BS2TemplateEx[] templateEx;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2FaceExUnwarped
    {
        public byte faceIndex;
        public byte numOfTemplate;
        public byte flag;
        public byte reserved;
        public UInt32 imageLen;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 82808)]
        //public byte[] image;
        //public IntPtr image;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]         // + 2.9.6
    public struct BS2FaceExTemplateOnly
    {
        public byte faceIndex;
        public byte numOfTemplate;
        public byte flag;
        public byte reserved;
        public UInt32 imageLen;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2UserFaceExBlob
    {
        public BS2User user;
        public BS2UserSetting setting;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_NAME_LEN)]
        public byte[] name;
        public IntPtr user_photo_obj;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_PIN_HASH_SIZE)]
        public byte[] pin;
        public IntPtr cardObjs;
        public IntPtr fingerObjs;
        public IntPtr faceObjs;
        public BS2Job job;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_PHRASE_SIZE)]
        public byte[] phrase;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER)]
        public UInt32[] accessGroupId;

        public BS2UserSettingEx settingEx;
        public IntPtr faceExObjs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2BarcodeConfig              // [+ V2.8]
    {
	    public byte useBarcode;
        public byte scanTimeout;

        public byte bypassData;                 // [+ V2.8.2.7]
        public byte treatAsCSN;                 // [+ V2.8.2.7]

        public byte useVisualBarcode;           // [+ V2.9.1]
        public byte motionSensitivity;          // [+ V2.9.1]
        public byte visualCameraScanTimeout;    // [+ V2.9.1]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BS2ICExInput                // [+ V2.8.1]
    {
        public byte portIndex;
        public byte switchType;
        public UInt16 duration;
		
		public byte reserved;
		public byte supervisedResistor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] reserved1;
		
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
		public byte[] reserved2;
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2InputConfigEx          // [+ V2.8.1]
    {
        public byte numInputs;
        public byte numSupervised;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_INPUT_NUM_EX)]
        public BS2ICExInput[] inputs;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2RACInput                // [+ V2.8.1]
	{
		public byte port;
        public byte	type;
	    public byte mask;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2RACRelay                // [+ V2.8.1]
	{
		public byte port;
        public byte reserved0;
        public byte disconnEnabled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		public byte[] reserved;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_RELAY_ACTION_INPUT)]
        public BS2RACInput[] input;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2RelayActionConfig      // [+ V2.8.1]
    {
        public UInt32 deviceID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_RELAY_ACTION)]
        public BS2RACRelay[] relay;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 152)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LagacyAuth          // [+ V2.8]
	{
		public byte biometricAuthMask;
        //biomerticOnly: 1;
        //biometricPIN: 1;
        //unused: 6;
		public byte cardAuthMask;
        //rdOnly: 1;
        //rdBiometric: 1;
        //rdPIN: 1;
        //rdBiometricOrPIN: 1;
        //rdBiometricPIN: 1;
        //used: 3;
		public byte idAuthMask;
		//Biometric: 1;
		//PIN: 1;
		//BiometricOrPIN: 1;
		//BiometricPIN: 1;
		//used: 4;
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ExtendedAuth          // [+ V2.8]
    {
		public UInt32 faceAuthMask;
        //faceOnly: 1;
        //faceFingerprint: 1;
        //facePIN: 1;
        //faceFingerprintOrPIN: 1;
        //faceFingerprintPIN: 1;
        //unused: 27;
		public UInt32 fingerprintAuthMask;
		//fingerprintOnly: 1;
		//fingerprintFace: 1;
		//fingerprintPIN: 1;
		//fingerprintFaceOrPIN: 1;
		//fingerprintFacePIN: 1;
		//unused: 27;
		public UInt32 cardAuthMask;
		//cardOnly: 1;
		//cardFace: 1;
		//cardFingerprint: 1;
		//cardPIN: 1;
		//cardFaceOrFingerprint: 1;
		//cardFaceOrPIN: 1;
		//cardFingerprintOrPIN: 1;
		//cardFaceOrFingerprintOrPIN: 1;
		//cardFaceFingerprint: 1;
		//cardFacePIN: 1;
		//cardFingerprintFace: 1;
		//cardFingerprintPIN: 1;
		//cardFaceOrFingerprintPIN: 1;
		//cardFaceFingerprintOrPIN: 1;
		//cardFingerprintFaceOrPIN: 1;
		//unused: 17;
		public UInt32 idAuthMask;
        //idFace: 1;
        //idFingerprint: 1;
        //idPIN: 1;
        //idFaceOrFingerprint: 1;
        //idFaceOrPIN: 1;
        //idFingerprintOrPIN: 1;
        //idFaceOrFingerprintOrPIN: 1;
        //idFaceFingerprint: 1;
        //idFacePIN: 1;
        //idFingerprintFace: 1;
        //idFingerprintPIN: 1;
        //idFaceOrFingerprintPIN: 1;
        //idFaceFingerprintOrPIN: 1;
        //idFingerprintFaceOrPIN: 1;
        //unused: 18;
	}

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2AuthSupported
    {
		public byte extendedMode;
		public byte credentialsMask;
        //card: 1;
        //fingerprint: 1;
        //face: 1;
        //id: 1;
        //pin: 1;
        //reserved: 3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] auth;
        //public BS2LagacyAuth lagacy;
        //public BS2ExtendedAuth extended;
	}


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2DeviceCapabilities
    {
	    public UInt32 maxUsers;
	    public UInt32 maxEventLogs;
	    public UInt32 maxImageLogs;
	    public UInt32 maxBlacklists;
	    public UInt32 maxOperators;
	    public UInt32 maxCards;		///< 4 bytes
	    public UInt32 maxFaces;		///< 4 bytes
	    public UInt32 maxFingerprints;		///< 4 bytes
	    public UInt32 maxUserNames;		///< 4 bytes
	    public UInt32 maxUserImages;		///< 4 bytes
	    public UInt32 maxUserJobs;		///< 4 bytes
	    public UInt32 maxUserPhrases;		///< 4 bytes
	    public byte maxCardsPerUser;		///< 1 byte
	    public byte maxFacesPerUser;		///< 1 byte
	    public byte maxFingerprintsPerUser;		///< 1 byte
	    public byte maxInputPorts;		///< 1 byte
	    public byte maxOutputPorts;		///< 1 byte
	    public byte maxRelays;			///< 1 byte
	    public byte maxRS485Channels;		///< 1 byte

	    public byte systemSupported;
        //cameraSupported: 1;
        //tamperSupported: 1;
        //wlanSupported: 1;
        //displaySupported: 1;
        //thermalSupported: 1;
        //maskSupported: 1;
        //faceExSupported: 1;
        //unused: 1;
	    //voipExSupported: 1;
	    public UInt32 cardSupportedMask;
        //EM: 1;
        //HIDProx: 1;
        //MifareFelica: 1;
        //iClass: 1;
        //ClassicPlus: 1;
        //DesFireEV1: 1;
        //SRSE: 1;
        //SEOS: 1;
        //NFC: 1;
        //BLE: 1;
        //reserved: 21;
        //useCardOperation: 1;
	    public BS2AuthSupported authSupported;
	    public byte functionSupported;
        //intelligentPDSupported: 1;
   	    //updateUserSupported: 1;
	    //simulatedUnlockSupported: 1;
        //smartCardByteOrderSupported: 1;
        //treatAsCSNSupported: 1;
        //rtspSupported: 1;
        //lfdSupported: 1;
        //visualQRSupported: 1;

	    public byte maxVoipExtensionNumbers;        // [+V2.8.3]

        public byte functionSupported2;             // [+V2.9.1]
        //osdpStandardCentralSupported : 1;
        //enableLicenseFuncSupported : 1;
        //keypadBacklightSupported : 1              // [+V2.9.4]
        //uzWirelessLockDoorSupported : 1
        //customSmartCardSupported : 1
        //tomSupported : 1
       	//tomEnrollSupported: 1;
	    //showOsdpResultbyLED: 1;

        public byte functionSupported3;             // [+ 2.9.6]
  	    //customSmartCardFelicaSupported: 1;
	    //ignoreInputAfterWiegandOut: 1;
	    //setSlaveBaudrateSupported: 1;

        public UInt16 visualFaceTemplateVersion;    // [+ 2.9.6]

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 429)]
	    public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2VoipConfigExtOutboundProxy
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_URL_SIZE)]
        public byte[] address;
        public UInt16 port;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2ExtensionNumber
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] phoneNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_DESCRIPTION_NAME_LEN)]
        public byte[] description;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2VoipConfigExtVolume
    {
        public byte speaker;
        public byte mic;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2VoipConfigExt
    {
        public byte enabled;
        public byte useOutboundProxy;
        public UInt16 registrationDuration;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_URL_SIZE)]
        public byte[] address;
        public UInt16 port;
        public BS2VoipConfigExtVolume volume;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] password;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] authorizationCode;

        public BS2VoipConfigExtOutboundProxy outboundProxy;

        public byte exitButton;
        public byte reserved1;
        public byte numPhoneBook;
        public byte showExtensionNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_VOIP_MAX_PHONEBOOK_EXT)]
        public BS2ExtensionNumber[] phonebook;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2RtspConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] password;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_URL_SIZE)]
        public byte[] address;
        public UInt16 port;

        public byte enabled;
        public byte reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardLedAction
    {
        public byte use;
        public byte readerNumber;
        public byte ledNumber;

        public byte tempCommand;
        public byte tempOnTime;
        public byte tempOffTime;
        public byte tempOnColor;
        public byte tempOffColor;
        public UInt16 tempRunTime;

        public byte permCommand;
        public byte permOnTime;
        public byte permOffTime;
        public byte permOnColor;
        public byte permOffColor;

        public byte reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardBuzzerAction
    {
        public byte use;
        public byte readerNumber;
        public byte tone;
        public byte onTime;
        public byte offTime;
        public byte numOfCycle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardAction
    {
        public byte actionType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public BS2OsdpStandardLedAction[] led;
        public BS2OsdpStandardBuzzerAction buzzer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardActionConfig
    {
        public byte version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_OSDP_STANDARD_ACTION_MAX_COUNT)]
        public BS2OsdpStandardAction[] actions;
    }

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BS2OsdpStandardDeviceNotify
	{
        public UInt32 deviceID;
        public UInt16 deviceType;
        public byte enableOSDP;
        public byte connected;
        public byte channelInfo;
        public byte osdpID;
        public byte supremaSearch;
        public byte activate;
        public byte useSecure;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] vendorCode;
        public UInt32 fwVersion;
        public byte modelNumber;
        public byte modelVersion;
        public byte readInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardDevice
    {
        public UInt32 deviceID;
        public UInt16 deviceType;
        public byte enableOSDP;
        public byte connected;
        public byte channelInfo;
        public byte osdpID;
        public byte supremaSearch;
        public byte activate;
        public byte useSecure;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] vendorCode;
        public UInt32 fwVersion;
        public byte modelNumber;
        public byte modelVersion;
        public byte readInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardChannel
    {
        public UInt32 baudRate;
        public byte channelIndex;
        public byte useRegistance;
        public byte numOfDevices;
        public byte channelType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_SLAVES_PER_CHANNEL)]
        public BS2OsdpStandardDevice[] slaveDevices;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2License                                        // + 2.9.1
    {
        public byte index;
        public byte hasCapability;
        public byte enable;
        public byte reserved;
        public UInt16 licenseType;
        public UInt16 licenseSubType;
        public UInt32 enableTime;
        public UInt32 expiredTime;
        public UInt32 issueNumber;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_USER_ID_SIZE)]
        public byte[] name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LicenseConfig                                  // + 2.9.1
    {
        public byte version;
        public byte numOfLicense;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_MAX_LICENSE_COUNT)]
        public BS2License[] license;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] reserved1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LicenseBlob                                    // + 2.9.1
    {
        public UInt16 licenseType;
        public UInt16 numOfDevices;
        public IntPtr deviceIDObjs;
        public UInt32 licenseLen;
        public IntPtr licenseObj;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2LicenseResult                                  // + 2.9.1
    {
        public UInt32 deviceID;
        public byte status;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardDeviceResult                       // + 2.9.1
    {
        public UInt32 deviceID;
        public byte result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardDeviceSecurityKey                        // + 2.9.1
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_OSDP_STANDARD_KEY_SIZE)]
        public byte[] key;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardDeviceCapabilityItem
    {
        public byte compliance;
        public byte count;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardDeviceCapability
    {
        public BS2OsdpStandardDeviceCapabilityItem input;
        public BS2OsdpStandardDeviceCapabilityItem output;
        public BS2OsdpStandardDeviceCapabilityItem led;
        public BS2OsdpStandardDeviceCapabilityItem audio;
        public BS2OsdpStandardDeviceCapabilityItem textOutput;
        public BS2OsdpStandardDeviceCapabilityItem reader;

        public UInt16 recvBufferSize;
        public UInt16 largeMsgSize;

        public byte osdpVersion;
        public byte cardFormat;
        public byte timeKeeping;
        public byte canCommSecure;

        public byte crcSupport;
        public byte smartCardSupport;
        public byte biometricSupport;
        public byte securePinEntrySupport;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardDeviceAdd
    {
        public byte osdpID;
        public byte activate;
        public byte useSecureSession;
        public byte deviceType;
        public UInt32 deviceID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardDeviceUpdate
    {
        public byte osdpID;
        public byte activate;
        public byte useSecureSession;
        public byte deviceType;
        public UInt32 deviceID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardChannelInfo
    {
        public byte channelIndex;
        public byte channelType;
        public byte maxOsdpDevice;
        public byte numOsdpAvailableDevice;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UInt32[] deviceIDs;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardDeviceAvailable
    {
        public byte numOfChannel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_CHANNELS_EX)]
        public BS2OsdpStandardChannelInfo[] channels;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2OsdpStandardConfig
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_CHANNELS_EX)]
        public byte[] mode;
        public UInt16 numOfChannels;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BS2Environment.BS2_RS485_MAX_CHANNELS_EX)]
        public BS2OsdpStandardChannel[] channels;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2CustomMifareCard
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] primaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] secondaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved2;
        public UInt16 startBlockIndex;
        public byte dataSize;
        public byte skipBytes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2CustomDesFireCard
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] primaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] secondaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] appID;
        public byte fileID;
        public byte encryptionType;                 // 0: DES/3DES, 1: AES
        public byte operationMode;                  // 0: legacy(use picc master key), 1: new mode(use app master, file read, file write key)
        public byte dataSize;
        public byte skipBytes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved;
        public BS2DesFireAppLevelKey desfireAppKey;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BS2CustomCardConfig
    {
        public byte dataType;
        public byte useSecondaryKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved1;

        public BS2CustomMifareCard mifare;
        public BS2CustomDesFireCard desfire;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] reserved2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
        public byte[] reserved3;

        public byte smartCardByteOrder;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved4;
        public UInt32 formatID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] reserved5;
    }
}
