/**
 *  Packet between Devices and BioStar Server
 *
 *  @author jylee@suprema.co.kr
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

#ifndef __BS2_SYSTEM_INFO_H__
#define __BS2_SYSTEM_INFO_H__

/**
 *	SSL method Type
 */
enum {
    BS2_SSL_METHOD_MASK_NONE                              = 0x00000000,
    BS2_SSL_METHOD_MASK_SSL2                              = 0x00000001, ///< SSLv23_client_method
    BS2_SSL_METHOD_MASK_SSL3                              = 0x00000002, ///< SSLv3_client_method
    BS2_SSL_METHOD_MASK_TLS1                              = 0x00000004, ///< TLSv1_client_method
    BS2_SSL_METHOD_MASK_TLS1_1                            = 0x00000008, ///< TLSv1_1_client_method
    BS2_SSL_METHOD_MASK_TLS1_2                            = 0x00000010, ///< TLSv1_2_client_method
    BS2_SSL_METHOD_MASK_ALL                               = 0xFFFFFFFF, ///< 4 bytes
};

typedef uint32_t BS2_SSL_METHOD_MASK;

/**
 *	System ifno Type
 */
enum {
    BS2_SYSTEM_INFO_MASK_NONE                             = 0x00000000,
    BS2_SYSTEM_INFO_MASK_DEVICE                           = 0x00000001, ///< Device information
    BS2_SYSTEM_INFO_MASK_SSL                              = 0x00000002, ///< SSL information
    BS2_SYSTEM_INFO_MASK_CREDENTIAL                       = 0x00000004, ///< Credential information
    BS2_SYSTEM_INFO_MASK_ALL                              = 0xFFFFFFFF, ///< 4 bytes
};

typedef uint32_t BS2_SYSTEM_INFO_MASK;

#endif	// __BS2_SYSTEM_INFO_H__
