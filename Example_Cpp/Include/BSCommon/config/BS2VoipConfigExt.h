/*
 * BS2VoipConfigExt.h
 *
 *  Created on: 2022. 2. 21.
 *      Author: smlee
 */

#ifndef BS2VOIPCONFIG_EXT_H_
#define BS2VOIPCONFIG_EXT_H_

#include "BS2Types.h"

enum {
	BS2_EXT_VOIP_MAX_EXTENSION_NUMBER = 128,
	BS2_EXT_VOIP_MAX_DESCRIPTION_LEN = 48 * 3,

	BS2_EXT_VOIP_REGISTRATION_DURATION_DEFAULT = 300,
	BS2_EXT_VOIP_PORT_DEFAULT = 5060,

	BS2_EXT_VOIP_VOLUME_MIN = 0,
	BS2_EXT_VOIP_VOLUME_MAX = 100,
	BS2_EXT_VOIP_VOLUME_DEFAULT = 50,
};

typedef struct {
	BS2_USER_ID phoneNumber;
    char description[BS2_EXT_VOIP_MAX_DESCRIPTION_LEN];

	uint8_t reserved[32];		///< 32 bytes (reserved)
} BS2ExtensionNumber;

typedef struct {
	BS2_BOOL enabled;
	BS2_BOOL useOutboundProxy;
	uint16_t registrationDuration;	// 60 ~ 600

	BS2_URL address;		// CAUTION: might not be NULL-terminated

	BS2_PORT port;		///< 2 bytes
	struct {
		uint8_t speaker;		// 0 ~ 100
		uint8_t mic;			// 0 ~ 100
	} volume;			///< 2 bytes

	BS2_USER_ID id;				// CAUTION: might not be NULL-terminated
	BS2_USER_ID password;		// CAUTION: might not be NULL-terminated
	BS2_USER_ID authorizationCode;		// CAUTION: might not be NULL-terminated

	struct {
		BS2_URL address;		// CAUTION: might not be NULL-terminated

		BS2_PORT port;
		uint8_t reserved[2];		///< 2 bytes (packing)
	} outboundProxy;

	uint8_t exitButton;		/// *, #, 0~9
	uint8_t reserved1;
	uint8_t numPhoneBook;
	BS2_BOOL showExtensionNumber;

	BS2ExtensionNumber phonebook[BS2_EXT_VOIP_MAX_EXTENSION_NUMBER];

	uint8_t reserved2[32];		///< 32 bytes (reserved)
} BS2VoipConfigExt;

#endif /* BS2VOIPCONFIG_EXT_H_ */
