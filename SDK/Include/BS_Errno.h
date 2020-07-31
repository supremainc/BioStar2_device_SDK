/*
 * BS_Errno.h
 *
 *  Created on: 2015. 4. 27.
 *      Author: scpark
 */

#ifndef CORE_JNI_SRC_BS_ERRNO_H_
#define CORE_JNI_SRC_BS_ERRNO_H_

#define BS_SDK_SUCCESS                                      1
#define BS_SDK_DURESS_SUCCESS                               2
#define BS_SDK_FIRST_AUTH_SUCCESS                           3
#define BS_SDK_SECOND_AUTH_SUCCESS                          4
#define BS_SDK_DUAL_AUTH_SUCCESS                            5
#define BS_SDK_WIEGAND_BYPASS_SUCCESS                       11

// Driver errors
#define BS_SDK_ERROR_FROM_DEVICE_DRIVER                     -1

// Communication errors
#define BS_SDK_ERROR_CANNOT_OPEN_SOCKET                     -101
#define BS_SDK_ERROR_CANNOT_CONNECT_SOCKET                  -102
#define BS_SDK_ERROR_CANNOT_LISTEN_SOCKET                   -103
#define BS_SDK_ERROR_CANNOT_ACCEPT_SOCKET                   -104
#define BS_SDK_ERROR_CANNOT_READ_SOCKET                     -105
#define BS_SDK_ERROR_CANNOT_WRITE_SOCKET                    -106
#define BS_SDK_ERROR_SOCKET_IS_NOT_CONNECTED                -107
#define BS_SDK_ERROR_SOCKET_IS_NOT_OPEN                     -108
#define BS_SDK_ERROR_SOCKET_IS_NOT_LISTENED                 -109
#define BS_SDK_ERROR_SOCKET_IN_PROGRESS                     -110
//Custom => [IPv6]
#define BS_SDK_ERROR_IPV4_IS_NOT_ENABLE					    -111
#define BS_SDK_ERROR_IPV6_IS_NOT_ENABLE					    -112
#define BS_SDK_ERROR_NOT_SUPPORTED_SPECIFIED_DEVICE_INFO    -113
#define BS_SDK_ERROR_NOT_ENOUGTH_BUFFER					    -114
#define BS_SDK_ERROR_NOT_SUPPORTED_IPV6					    -115
#define BS_SDK_ERROR_INVALID_ADDRESS                        -116
//Custom <=

// Packet errors
#define BS_SDK_ERROR_INVALID_PARAM                          -200
#define BS_SDK_ERROR_INVALID_PACKET                         -201
#define BS_SDK_ERROR_INVALID_DEVICE_ID                      -202
#define BS_SDK_ERROR_INVALID_DEVICE_TYPE                    -203
#define BS_SDK_ERROR_PACKET_CHECKSUM                        -204
#define BS_SDK_ERROR_PACKET_INDEX                           -205
#define BS_SDK_ERROR_PACKET_COMMAND                         -206
#define BS_SDK_ERROR_PACKET_SEQUENCE                        -207
#define BS_SDK_ERROR_NO_PACKET                              -209
#define BS_SDK_ERROR_INVALID_CODE_SIGN                      -210 //=> [Check Firmware CodeSign][ET] <=

//Fingerprint errors
#define BS_SDK_ERROR_EXTRACTION_FAIL                        -300
#define BS_SDK_ERROR_VERIFY_FAIL                            -301
#define BS_SDK_ERROR_IDENTIFY_FAIL                          -302
#define BS_SDK_ERROR_IDENTIFY_TIMEOUT                       -303
#define BS_SDK_ERROR_FINGERPRINT_CAPTURE_FAIL               -304
#define BS_SDK_ERROR_FINGERPRINT_SCAN_TIMEOUT               -305
#define BS_SDK_ERROR_FINGERPRINT_SCAN_CANCELLED             -306
#define BS_SDK_ERROR_NOT_SAME_FINGERPRINT                   -307
#define BS_SDK_ERROR_EXTRACTION_LOW_QUALITY                 -308
#define BS_SDK_ERROR_CAPTURE_LOW_QUALITY                    -309
#define BS_SDK_ERROR_CANNOT_FIND_FINGERPRINT                -310
#define BS_SDK_ERROR_FAKE_FINGER_DETECTED                   -311
#define BS_SDK_ERROR_FAKE_FINGER_TRY_AGAIN                  -312
#define BS_SDK_ERROR_FAKE_FINGER_SENSOR_ERROR               -313
#define BS_SDK_ERROR_CANNOT_FIND_FACE                       -314
#define BS_SDK_ERROR_FACE_CAPTURE_FAIL                      -315
#define BS_SDK_ERROR_FACE_SCAN_TIMEOUT                      -316
#define BS_SDK_ERROR_FACE_SCAN_CANCELLED                    -317
#define BS_SDK_ERROR_FACE_SCAN_FAILED                       -318

//File I/O errors
#define BS_SDK_ERROR_CANNOT_OPEN_DIR                        -400
#define BS_SDK_ERROR_CANNOT_OPEN_FILE                       -401
#define BS_SDK_ERROR_CANNOT_WRITE_FILE                      -402
#define BS_SDK_ERROR_CANNOT_SEEK_FILE                       -403
#define BS_SDK_ERROR_CANNOT_READ_FILE                       -404
#define BS_SDK_ERROR_CANNOT_GET_STAT                        -405
#define BS_SDK_ERROR_CANNOT_GET_SYSINFO                     -406
#define BS_SDK_ERROR_DATA_MISMATCH                          -407
#define BS_SDK_ERROR_ALREADY_OPEN_DIR                       -408

// I/O errors
#define BS_SDK_ERROR_INVALID_RELAY                          -500
#define BS_SDK_ERROR_CANNOT_WRITE_IO_PACKET                 -501
#define BS_SDK_ERROR_CANNOT_READ_IO_PACKET                  -502
#define BS_SDK_ERROR_CANNOT_READ_INPUT                      -503
#define BS_SDK_ERROR_READ_INPUT_TIMEOUT                     -504
#define BS_SDK_ERROR_CANNOT_ENABLE_INPUT                    -505
#define BS_SDK_ERROR_CANNOT_SET_INPUT_DURATION              -506
#define BS_SDK_ERROR_INVALID_PORT                           -507
#define BS_SDK_ERROR_INVALID_INTERPHONE_TYPE                -508
#define BS_SDK_ERROR_INVALID_LCD_PARAM                      -510
#define BS_SDK_ERROR_CANNOT_WRITE_LCD_PACKET                -511
#define BS_SDK_ERROR_CANNOT_READ_LCD_PACKET                 -512
#define BS_SDK_ERROR_INVALID_LCD_PACKET                     -513
#define BS_SDK_ERROR_INPUT_QUEUE_FULL                       -520
#define BS_SDK_ERROR_WIEGAND_QUEUE_FULL                     -521
#define BS_SDK_ERROR_MISC_INPUT_QUEUE_FULL                  -522
#define BS_SDK_ERROR_WIEGAND_DATA_QUEUE_FULL                -523
#define BS_SDK_ERROR_WIEGAND_DATA_QUEUE_EMPTY               -524

//Util errors
#define BS_SDK_ERROR_NOT_SUPPORTED                          -600
#define BS_SDK_ERROR_TIMEOUT                                -601
#define BS_SDK_ERROR_CANNOT_SET_TIME                        -602

//Database errors
#define BS_SDK_ERROR_INVALID_DATA_FILE                      -700
#define BS_SDK_ERROR_TOO_LARGE_DATA_FOR_SLOT                -701
#define BS_SDK_ERROR_INVALID_SLOT_NO                        -702
#define BS_SDK_ERROR_INVALID_SLOT_DATA						-703
#define BS_SDK_ERROR_CANNOT_INIT_DB                         -704
#define BS_SDK_ERROR_DUPLICATE_ID                           -705
#define BS_SDK_ERROR_USER_FULL                              -706
#define BS_SDK_ERROR_DUPLICATE_TEMPLATE                     -707
#define BS_SDK_ERROR_FINGERPRINT_FULL                       -708
#define BS_SDK_ERROR_DUPLICATE_CARD                         -709
#define BS_SDK_ERROR_CARD_FULL                              -710
#define BS_SDK_ERROR_NO_VALID_HDR_FILE                      -711
#define BS_SDK_ERROR_INVALID_LOG_FILE						-712
#define BS_SDK_ERROR_CANNOT_FIND_USER                       -714
#define BS_SDK_ERROR_ACCESS_LEVEL_FULL                      -715
#define BS_SDK_ERROR_INVALID_USER_ID                        -716
#define BS_SDK_ERROR_BLACKLIST_FULL                         -717
#define BS_SDK_ERROR_USER_NAME_FULL                         -718
#define BS_SDK_ERROR_USER_IMAGE_FULL                        -719
#define BS_SDK_ERROR_USER_IMAGE_SIZE_TOO_BIG                -720
#define BS_SDK_ERROR_SLOT_DATA_CHECKSUM                     -721
#define BS_SDK_ERROR_CANNOT_UPDATE_FINGERPRINT              -722
#define BS_SDK_ERROR_TEMPLATE_FORMAT_MISMATCH               -723
#define BS_SDK_ERROR_NO_ADMIN_USER                          -724
#define BS_SDK_ERROR_CANNOT_FIND_LOG                        -725
#define BS_SDK_ERROR_DOOR_SCHEDULE_FULL                     -726
#define BS_SDK_ERROR_DB_SLOT_FULL                           -727
#define BS_SDK_ERROR_ACCESS_GROUP_FULL                      -728
#define BS_SDK_ERROR_ACCESS_SCHEDULE_FULL                   -730
#define BS_SDK_ERROR_HOLIDAY_GROUP_FULL                     -731
#define BS_SDK_ERROR_HOLIDAY_FULL                           -732
#define BS_SDK_ERROR_TIME_PERIOD_FULL                       -733
#define BS_SDK_ERROR_NO_CREDENTIAL                          -734
#define BS_SDK_ERROR_NO_BIOMETRIC_CREDENTIAL                -735
#define BS_SDK_ERROR_NO_CARD_CREDENTIAL                     -736
#define BS_SDK_ERROR_NO_PIN_CREDENTIAL                      -737
#define BS_SDK_ERROR_NO_BIOMETRIC_PIN_CREDENTIAL            -738
#define BS_SDK_ERROR_NO_USER_NAME                           -739
#define BS_SDK_ERROR_NO_USER_IMAGE                          -740
#define BS_SDK_ERROR_READER_FULL                            -741
#define BS_SDK_ERROR_CACHE_MISSED                           -742
#define BS_SDK_ERROR_OPERATOR_FULL                          -743
#define BS_SDK_ERROR_INVALID_LINK_ID                        -744
#define BS_SDK_ERROR_TIMER_CANCELED                         -745
#define BS_SDK_ERROR_USER_JOB_FULL                          -746
#define BS_SDK_ERROR_CANNOT_UPDATE_FACE                     -747
#define BS_SDK_ERROR_FACE_FULL                              -748
#define BS_SDK_ERROR_FLOOR_SCHEDULE_FULL                    -749
#define BS_SDK_ERROR_CANNOT_FIND_AUTH_GROUP                 -750
#define BS_SDK_ERROR_AUTH_GROUP_FULL                        -751
#define BS_SDK_ERROR_USER_PHRASE_FULL                       -752
#define BS_SDK_ERROR_DST_SCHEDULE_FULL                      -753
#define BS_SDK_ERROR_CANNOT_FIND_DST_SCHEDULE               -754
#define BS_SDK_ERROR_INVALID_SCHEDULE                       -755
#define BS_SDK_ERROR_CANNOT_FIND_OPERATOR                   -756
#define BS_SDK_ERROR_DUPLICATE_FINGERPRINT                  -757
#define BS_SDK_ERROR_DUPLICATE_FACE                         -758


//Config errors
#define BS_SDK_ERROR_INVALID_CONFIG                         -800
#define BS_SDK_ERROR_CANNOT_OPEN_CONFIG_FILE                -801
#define BS_SDK_ERROR_CANNOT_READ_CONFIG_FILE                -802
#define BS_SDK_ERROR_INVALID_CONFIG_FILE                    -803
#define BS_SDK_ERROR_INVALID_CONFIG_DATA                    -804
#define BS_SDK_ERROR_CANNOT_WRITE_CONFIG_FILE               -805
#define BS_SDK_ERROR_INVALID_CONFIG_INDEX                   -806

//Device errors
#define BS_SDK_ERROR_CANNOT_SCAN_FINGER                     -900
#define BS_SDK_ERROR_CANNOT_SCAN_CARD                       -901
#define BS_SDK_ERROR_CANNOT_OPEN_RTC                        -902
#define BS_SDK_ERROR_CANNOT_SET_RTC                         -903
#define BS_SDK_ERROR_CANNOT_GET_RTC                         -904
#define BS_SDK_ERROR_CANNOT_SET_LED                         -905
#define BS_SDK_ERROR_CANNOT_OPEN_DEVICE_DRIVER              -906
#define BS_SDK_ERROR_CANNOT_FIND_DEVICE                     -907
#define BS_SDK_ERROR_CANNOT_SCAN_FACE                       -908
#define BS_SDK_ERROR_SLAVE_FULL                             -910
#define BS_SDK_ERROR_CANNOT_ADD_DEVICE                      -911

//Door errors
#define BS_SDK_ERROR_CANNOT_FIND_DOOR                       -1000
#define BS_SDK_ERROR_DOOR_FULL                              -1001
#define BS_SDK_ERROR_CANNOT_LOCK_DOOR                       -1002
#define BS_SDK_ERROR_CANNOT_UNLOCK_DOOR                     -1003
#define BS_SDK_ERROR_CANNOT_RELEASE_DOOR                    -1004
#define BS_SDK_ERROR_CANNOT_FIND_LIFT                       -1005
#define BS_SDK_ERROR_LIFT_FULL                              -1006

//Access control errors
#define BS_SDK_ERROR_ACCESS_RULE_VIOLATION                  -1100
#define BS_SDK_ERROR_DISABLED                               -1101
#define BS_SDK_ERROR_NOT_YET_VALID                          -1102
#define BS_SDK_ERROR_EXPIRED                                -1103
#define BS_SDK_ERROR_BLACKLIST                              -1104
#define BS_SDK_ERROR_CANNOT_FIND_ACCESS_GROUP               -1105
#define BS_SDK_ERROR_CANNOT_FIND_ACCESS_LEVEL               -1106
#define BS_SDK_ERROR_CANNOT_FIND_ACCESS_SCHEDULE            -1107
#define BS_SDK_ERROR_CANNOT_FIND_HOLIDAY_GROUP              -1108
#define BS_SDK_ERROR_CANNOT_FIND_BLACKLIST                  -1109
#define BS_SDK_ERROR_AUTH_TIMEOUT                           -1110
#define BS_SDK_ERROR_DUAL_AUTH_TIMEOUT                      -1111
#define BS_SDK_ERROR_INVALID_AUTH_MODE                      -1112
#define BS_SDK_ERROR_AUTH_UNEXPECTED_USER                   -1113
#define BS_SDK_ERROR_AUTH_UNEXPECTED_CREDENTIAL             -1114
#define BS_SDK_ERROR_DUAL_AUTH_FAIL                         -1115
#define BS_SDK_ERROR_BIOMETRIC_AUTH_REQUIRED                -1116
#define BS_SDK_ERROR_CARD_AUTH_REQUIRED                     -1117
#define BS_SDK_ERROR_PIN_AUTH_REQUIRED                      -1118
#define BS_SDK_ERROR_BIOMETRIC_OR_PIN_AUTH_REQUIRED         -1119
#define BS_SDK_ERROR_TNA_CODE_REQUIRED                      -1120
#define BS_SDK_ERROR_AUTH_SERVER_MATCH_REFUSAL              -1121
#define BS_SDK_ERROR_CANNOT_FIND_FLOOR_LEVEL                -1122
#define BS_SDK_ERROR_AUTH_FAIL                              -1123
#define BS_SDK_ERROR_AUTH_GROUP_REQUIRED                    -1124
#define BS_SDK_ERROR_IDENTIFICATION_REQUIRED                -1125
#define BS_SDK_ERROR_ANTI_TAILGATE_VIOLATION                -1126

//Zone errors
#define BS_SDK_ERROR_CANNOT_FIND_ZONE                       -1200
#define BS_SDK_ERROR_ZONE_FULL                              (DEPRECATED_MAC)-1201	// Deprecated 2.6.0
#define BS_SDK_ERROR_SET_ZONE                               -1201
#define BS_SDK_ERROR_HARD_APB_VIOLATION                     -1202
#define BS_SDK_ERROR_SOFT_APB_VIOLATION                     -1203
#define BS_SDK_ERROR_HARD_TIMED_APB_VIOLATION               -1204
#define BS_SDK_ERROR_SOFT_TIMED_APB_VIOLATION               -1205
#define BS_SDK_ERROR_SCHEDULED_LOCK_VIOLATION               -1206
#define BS_SDK_ERROR_SCHEDULED_UNLOCK_VIOLATION             (DEPRECATED_MAC)-1207
#define BS_SDK_ERROR_INTRUSION_ALARM_VIOLATION              -1207
#define BS_SDK_ERROR_SET_FIRE_ALARM                         -1208
#define BS_SDK_ERROR_TIMED_APB_ZONE_FULL                    -1209
#define BS_SDK_ERROR_FIRE_ALARM_ZONE_FULL                   -1210
#define BS_SDK_ERROR_SCHEDULED_LOCK_UNLOCK_ZONE_FULL        -1211
#define BS_SDK_ERROR_INACTIVE_ZONE                          -1212
#define BS_SDK_ERROR_INTRUSION_ALARM_ZONE_FULL              -1213
#define BS_SDK_ERROR_CANNOT_ARM                             -1214
#define BS_SDK_ERROR_CANNOT_DISARM                          -1215
#define BS_SDK_ERROR_CANNOT_FIND_ARM_CARD                   -1216
#define BS_SDK_ERROR_HARD_ENTRANCE_LIMIT_COUNT_VIOLATION    -1217
#define BS_SDK_ERROR_SOFT_ENTRANCE_LIMIT_COUNT_VIOLATION    -1218
#define BS_SDK_ERROR_HARD_ENTRANCE_LIMIT_TIME_VIOLATION     -1219
#define BS_SDK_ERROR_SOFT_ENTRANCE_LIMIT_TIME_VIOLATION     -1220
#define BS_SDK_ERROR_INTERLOCK_ZONE_DOOR_VIOLATION          -1221
#define BS_SDK_ERROR_INTERLOCK_ZONE_INPUT_VIOLATION         -1222
#define BS_SDK_ERROR_INTERLOCK_ZONE_FULL                    -1223
#define BS_SDK_ERROR_AUTH_LIMIT_SCHEDULE_VIOLATION          -1224
#define BS_SDK_ERROR_AUTH_LIMIT_COUNT_VIOLATION             -1225
#define BS_SDK_ERROR_AUTH_LIMIT_USER_VIOLATION              -1226
#define BS_SDK_ERROR_SOFT_AUTH_LIMIT_VIOLATION              -1227
#define BS_SDK_ERROR_HARD_AUTH_LIMIT_VIOLATION              -1228

//Card errors
#define BS_SDK_ERROR_CARD_IO                                -1300
#define BS_SDK_ERROR_CARD_INIT_FAIL                         -1301
#define BS_SDK_ERROR_CARD_NOT_ACTIVATED                     -1302
#define BS_SDK_ERROR_CARD_CANNOT_READ_DATA                  -1303
#define BS_SDK_ERROR_CARD_CIS_CRC                           -1304
#define BS_SDK_ERROR_CARD_CANNOT_WRITE_DATA                 -1305
#define BS_SDK_ERROR_CARD_READ_TIMEOUT                      -1306
#define BS_SDK_ERROR_CARD_READ_CANCELLED                    -1307
#define BS_SDK_ERROR_CARD_CANNOT_SEND_DATA                  -1308
#define BS_SDK_ERROR_CANNOT_FIND_CARD                       -1310

// Operation
#define BS_SDK_ERROR_INVALID_PASSWORD                       -1400

// System
#define BS_SDK_ERROR_CAMERA_INIT_FAIL                       -1500
#define BS_SDK_ERROR_JPEG_ENCODER_INIT_FAIL                 -1501
#define BS_SDK_ERROR_CANNOT_ENCODE_JPEG                     -1502
#define BS_SDK_ERROR_JPEG_ENCODER_NOT_INITIALIZED           -1503
#define BS_SDK_ERROR_JPEG_ENCODER_DEINIT_FAIL               -1504
#define BS_SDK_ERROR_CAMERA_CAPTURE_FAIL                    -1505
#define BS_SDK_ERROR_CANNOT_DETECT_FACE                     -1506

//ETC.
#define BS_SDK_ERROR_FILE_IO                                -2000
#define BS_SDK_ERROR_ALLOC_MEM                              -2002
#define BS_SDK_ERROR_CANNOT_UPGRADE                         -2003
#define BS_SDK_ERROR_DEVICE_LOCKED                          -2004
#define BS_SDK_ERROR_CANNOT_SEND_TO_SERVER                  -2005
#define BS_SDK_ERROR_CANNOT_UPGRADE_MEMOEY                  -2006

//SSL
#define BS_SDK_ERROR_SSL_INIT                               -3000
#define BS_SDK_ERROR_SSL_EXIST                              -3001
#define BS_SDK_ERROR_SSL_IS_NOT_CONNECTED                   -3002
#define BS_SDK_ERROR_SSL_ALREADY_CONNECTED                  -3003
#define BS_SDK_ERROR_SSL_INVALID_CA                         -3004
#define BS_SDK_ERROR_SSL_VERIFY_CA                          -3005
#define BS_SDK_ERROR_SSL_INVALID_KEY                        -3006
#define BS_SDK_ERROR_SSL_VERIFY_KEY                         -3007

// Mobile access
#define BS_SDK_ERROR_MOBILE_PORTAL                          -3100

#define BS_SDK_ERROR_NULL_POINTER                           -10000
#define BS_SDK_ERROR_UNINITIALIZED                          -10001
#define BS_SDK_ERROR_CANNOT_RUN_SERVICE                     -10002
#define BS_SDK_ERROR_CANCELED                               -10003
#define BS_SDK_ERROR_EXIST                                  -10004
#define BS_SDK_ERROR_ENCRYPT                                -10005
#define BS_SDK_ERROR_DECRYPT                                -10006
#define BS_SDK_ERROR_DEVICE_BUSY							-10007
#define BS_SDK_ERROR_INTERNAL								-10008
#define BS_SDK_ERROR_INVALID_FILE_FORMAT					-10009
#define BS_SDK_ERROR_INVALID_SCHEDULE_ID					-10010
#define BS_SDK_ERROR_UNKNOWN_FINGER_TEMPLATE                -10011

// Mobile access (defined from SDK)
#define BS_SDK_ERROR_MOBILE_STATUS_FAULT                    -10100
#define BS_SDK_ERROR_MOBILE_NO_RESPONSE                     -10101
#define BS_SDK_ERROR_MOBILE_NOT_LOGIN						-10103
#define BS_SDK_ERROR_MOBILE_ADD_DEVICE_FAULT                -10104
#define BS_SDK_ERROR_MOBILE_DEVICE_NOT_FOUND				-10105

#endif /* CORE_JNI_SRC_BS_ERRNO_H_ */
