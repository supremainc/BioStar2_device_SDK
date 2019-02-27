/*
 * BS2VoipConfig.h
 *
 *  Created on: 2016. 2. 25.
 *      Author: yhlee
 */

#ifndef BS2VOIPCONFIG_H_
#define BS2VOIPCONFIG_H_

#include "BS2Types.h"

#define BS2_VOIP_MAX_PHONEBOOK	32
#define BS2_MAX_DESCRIPTION_NAME_LEN 48 *3

typedef struct{
	BS2_USER_ID		phoneNumber;
    char            descript[BS2_MAX_DESCRIPTION_NAME_LEN];

	uint8_t reserved2[32];		///< 32 bytes (reserved)
} BS2UserPhoneItem;

typedef struct {
	BS2_URL			serverUrl;			/// << server IP
	BS2_PORT		serverPort;			/// << server Port
	BS2_USER_ID		userID;				/// << userID
	BS2_USER_ID		userPW;				/// << userPassWord

	uint8_t			exitButton;			/// << *, #, 0~9
	uint8_t			dtmfMode;			/// << DTMF Mode
	BS2_BOOL		bUse;				/// << use Voip
	uint8_t			reseverd[1];		/// << reserved

    uint32_t        numPhonBook;        
	BS2UserPhoneItem 	phonebook[BS2_VOIP_MAX_PHONEBOOK];  /// << phone list  MAX 32

	uint8_t reserved2[32];		///< 32 bytes (reserved)
} BS2VoipConfig;

#endif /* BS2VOIPCONFIG_H_ */
