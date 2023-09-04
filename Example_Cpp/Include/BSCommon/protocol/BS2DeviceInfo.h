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

#ifndef __BS2_DEVICE_INFO_H__
#define __BS2_DEVICE_INFO_H__

#include "../BS2Types.h"
#include "../config/BS2FactoryConfig.h"

/**
 *	BS2DeviceInfo
 */
typedef struct {
	BS2_DEVICE_ID deviceID;				///< 4 bytes
	BS2_DEVICE_TYPE deviceType;			///< 2 bytes
	uint8_t unused[2];					///< 2 bytes
	char modelName[BS2_MODEL_NAME_LEN];	///< 32 bytes
	Version firmwareVer;				///< 4 bytes

#ifndef VER_260_OR_HIGHER
	uint32_t	maxUsers;			///< 4 bytes
	uint32_t	maxLogs;			///< 4 bytes
	uint8_t		maxInputPorts;		///< 1 byte
	uint8_t		maxOutputPorts;		///< 1 byte
	uint8_t		maxRelays;			///< 1 byte
	uint8_t		maxRs485Channels;	///< 1 byte
	BS2_BOOL	cameraSupported;	///< 1 byte
	BS2_BOOL	tamperSupported;	///< 1 byte
	BS2_BOOL	wlanSupported;		///< 1 byte
	BS2_BOOL	displaySupported;	///< 1 byte
	uint32_t	cardSupportedMask;	///< 4 bytes : supported card mask
#endif

	uint8_t reserved[256];		///< 256 bytes
} BS2DeviceInfo;

typedef struct {
	/*
	 * Maximum number of users that can be stored on the device
	 */
	uint32_t maxUsers;		///< 4 bytes

	/*
	 * Maximum number of event logs that can be stored on the device
	 */
	uint32_t maxEventLogs;		///< 4 bytes

	/*
	 * Maximum number of image logs that can be stored on the device
	 */
	uint32_t maxImageLogs;		///< 4 bytes

	/*
	 * Maximum number of blacklists that can be stored on the device
	 */
	uint32_t maxBlacklists;		///< 4 bytes

	/*
	 * Maximum number of operators that can be stored on the device
	 */
	uint32_t maxOperators;		///< 4 bytes

	/*
	 * Maximum number of cards that can be stored on the device
	 */
	uint32_t maxCards;		///< 4 bytes

	/*
	 * Maximum number of faces that can be stored on the device
	 */
	uint32_t maxFaces;		///< 4 bytes

	/*
	 * Maximum number of fingerprints that can be stored on the device
	 */
	uint32_t maxFingerprints;		///< 4 bytes

	/*
	 * Maximum number of user's names that can be stored on the device
	 */
	uint32_t maxUserNames;		///< 4 bytes

	/*
	 * Maximum number of user's images that can be stored on the device
	 */
	uint32_t maxUserImages;		///< 4 bytes

	/*
	 * Maximum number of user's jobs that can be stored on the device
	 */
	uint32_t maxUserJobs;		///< 4 bytes

	/*
	 * Maximum number of user's phrases that can be stored on the device
	 */
	uint32_t maxUserPhrases;		///< 4 bytes

	/*
	 * Maximum number of cards a user can hold
	 */
	uint8_t maxCardsPerUser;		///< 1 byte

	/*
	 * Maximum number of faces a user can hold
	 */
	uint8_t maxFacesPerUser;		///< 1 byte

	/*
	 * Maximum number of fingerprints a user can hold
	 */
	uint8_t maxFingerprintsPerUser;		///< 1 byte

	/*
	 * Maximum number of input ports of the device
	 */
	uint8_t maxInputPorts;		///< 1 byte

	/*
	 * Maximum number of output ports of the device
	 */
	uint8_t maxOutputPorts;		///< 1 byte

	/*
	 * Maximum number of relay ports of the device
	 */
	uint8_t maxRelays;			///< 1 byte

	/*
	 * Maximum number of RS485 channels of the device
	 */
	uint8_t maxRS485Channels;		///< 1 byte

	/*
	 * If true, image log is supported using the equipped camera.
	 */
	uint8_t cameraSupported: 1;

	/*
	 * If true, tamper is supported.
	 */
	uint8_t tamperSupported: 1;

	/*
	 * If true, WLAN is supported.
	 */
	uint8_t wlanSupported: 1;

	/*
	 * If true, UI is supported.
	 */
	uint8_t displaySupported: 1;

	/*
	 * If true, thermal camera is supported.
	 */
	uint8_t thermalSupported: 1;

	/*
	 * If true, mask detection is supported.
	 */
	uint8_t maskSupported: 1;

	/*
	 * If true, BS2FaceEx is supported.
	 */
	uint8_t faceExSupported: 1;

	/*
	 * If true, BS2VoipConfigEx is supported.
	 */
	uint8_t voipExSupported: 1;		// BDP-567

	/*
	 * Mask for card type supported by the device
	 */
	union {
		uint32_t mask;		///< 4 bytes

		struct {
			uint32_t EM: 1;
			uint32_t HIDProx: 1;
			uint32_t MifareFelica: 1;
			uint32_t iClass: 1;
			uint32_t ClassicPlus: 1;
			uint32_t DesFireEV1: 1;
			uint32_t SRSE: 1;
			uint32_t SEOS: 1;
			uint32_t NFC: 1;
			uint32_t BLE: 1;
			uint32_t reserved: 21;
			uint32_t useCardOperation: 1;
		};
	} cardSupported;

	struct {
		/*
		 * If true, only extended authentication modes are supported.
		 * Otherwise only legacy authentication modes are supported.
		 */
		BS2_BOOL extendedMode;		///< 1 byte

		/*
		 * Credentials supported by the device
		 * Note: This means only credential can be directly input from the device. (Not from slave)
		 */
		union {
			uint8_t mask;		///< 1 byte

			struct {
				uint8_t card: 1;
				uint8_t fingerprint: 1;
				uint8_t face: 1;
				uint8_t id: 1;
				uint8_t pin: 1;
				uint8_t reserved: 3;
			};
		} credentials;

		uint8_t reserved[2];		///< 2 bytes

		/*
		 * Authentication modes supported by the device
		 */
		union {
			/*
			 * Legacy authentication modes supported by the device
			 */
			struct {
				/*
				 * Biometric-first authentication modes supported by the device
				 */
				union {
					uint8_t mask;		///< 1 byte

					struct {
						uint8_t biometricOnly: 1;
						uint8_t biometricPIN: 1;
						uint8_t unused: 6;
					};
				} biometricAuth;

				/*
				 * Card-first authentication modes supported by the device
				 */
				union {
					uint8_t mask;		///< 1 byte

					struct {
						uint8_t cardOnly: 1;
						uint8_t cardBiometric: 1;
						uint8_t cardPIN: 1;
						uint8_t cardBiometricOrPIN: 1;
						uint8_t cardBiometricPIN: 1;
						uint8_t unused: 3;
					};
				} cardAuth;

				/*
				 * ID-first authentication modes supported by the device
				 */
				union {
					uint8_t mask;		///< 1 byte

					struct {
						uint8_t idBiometric: 1;
						uint8_t idPIN: 1;
						uint8_t idBiometricOrPIN: 1;
						uint8_t idBiometricPIN: 1;
						uint8_t unused: 4;
					};
				} idAuth;
			} legacy;

			/*
			 * Extended authentication modes supported by the device
			 */
			struct {
				/*
				 * Face-first authentication modes supported by the device
				 */
				union {
					uint32_t mask;		///< 4 bytes

					struct {
						uint32_t faceOnly: 1;
						uint32_t faceFingerprint: 1;
						uint32_t facePIN: 1;
						uint32_t faceFingerprintOrPIN: 1;
						uint32_t faceFingerprintPIN: 1;
						uint32_t unused: 27;
					};
				} faceAuth;

				/*
				 * Fingerprint-first authentication modes supported by the device
				 */
				union {
					uint32_t mask;			///< 4 bytes

					struct {
						uint32_t fingerprintOnly: 1;
						uint32_t fingerprintFace: 1;
						uint32_t fingerprintPIN: 1;
						uint32_t fingerprintFaceOrPIN: 1;
						uint32_t fingerprintFacePIN: 1;
						uint32_t unused: 27;
					};
				} fingerprintAuth;

				/*
				 * Card-first authentication modes supported by the device
				 */
				union {
					uint32_t mask;			///< 4 bytes

					struct {
						uint32_t cardOnly: 1;
						uint32_t cardFace: 1;
						uint32_t cardFingerprint: 1;
						uint32_t cardPIN: 1;
						uint32_t cardFaceOrFingerprint: 1;
						uint32_t cardFaceOrPIN: 1;
						uint32_t cardFingerprintOrPIN: 1;
						uint32_t cardFaceOrFingerprintOrPIN: 1;
						uint32_t cardFaceFingerprint: 1;
						uint32_t cardFacePIN: 1;
						uint32_t cardFingerprintFace: 1;
						uint32_t cardFingerprintPIN: 1;
						uint32_t cardFaceOrFingerprintPIN: 1;
						uint32_t cardFaceFingerprintOrPIN: 1;
						uint32_t cardFingerprintFaceOrPIN: 1;
						uint32_t unused: 17;
					};
				} cardAuth;

				/*
				 * ID-first authentication modes supported by the device
				 */
				union {
					uint32_t mask;			///< 4 bytes

					struct {
						uint32_t idFace: 1;
						uint32_t idFingerprint: 1;
						uint32_t idPIN: 1;
						uint32_t idFaceOrFingerprint: 1;
						uint32_t idFaceOrPIN: 1;
						uint32_t idFingerprintOrPIN: 1;
						uint32_t idFaceOrFingerprintOrPIN: 1;
						uint32_t idFaceFingerprint: 1;
						uint32_t idFacePIN: 1;
						uint32_t idFingerprintFace: 1;
						uint32_t idFingerprintPIN: 1;
						uint32_t idFaceOrFingerprintPIN: 1;
						uint32_t idFaceFingerprintOrPIN: 1;
						uint32_t idFingerprintFaceOrPIN: 1;
						uint32_t unused: 18;
					};
				} idAuth;
			} extended;
		};
	} authSupported;

	/*
	 * If true, intelligentPD is supported.
	 */
	uint8_t intelligentPDSupported: 1;		// BDP-92

	/*
	 * If true, partial update is supported.
	 */
	uint8_t updateUserSupported: 1;		// BDP-561

	/*
	 * If true, simulatedUnlock is supported.
	 */
	uint8_t simulatedUnlockSupported: 1;		// BDP-523

	/*
	 * If true, smartCardByteOrder is supported.
	 */
	uint8_t smartCardByteOrderSupported: 1;		// BDP-511

	/*
	 * If true, treatAsCSN is supported.
	 */
	uint8_t treatAsCSNSupported: 1;		// BDP-488

	/*
	 * If true, RTSP is supported.
	 */
	uint8_t rtspSupported: 1;		// BDP-568

	/*
	 * If true, LFD is supported.
	 */
	uint8_t lfdSupported: 1;		// BDP-588

	/*
	 * If true, visual QR is supported.
	 */
	uint8_t visualQRSupported: 1;		///< 1 byte

	/*
	 * Maximum number of VOIP extension numbers that can be stored on the device
	 */
	uint8_t maxVoipExtensionNumbers;		///< 1 byte

	/*
	 * OSDP Standard Central supported.
	 */
	uint8_t osdpStandardCentralSupported : 1;		///< 1 byte

	/*
	 * License Enable function supported.
	 */
	uint8_t enableLicenseFuncSupported : 1;   ///< 1 byte

	/*
	 * Keypad Backlight Supported.
	 */
	uint8_t keypadBacklightSupported: 1;

	/*
	 * UZ Wireless Lock Supported.
	 */
	uint8_t uzWirelessLockDoorSupported: 1;

	/*
	 * If true, Custom Smard Card is supported.
	 */
	uint8_t customSmartCardSupported: 1; 

	/*
	 * If true, Template On Mobile is supported.
	 */
	uint8_t tomSupported: 1;

	uint8_t unused2: 2;		///< 1 byte

	uint8_t reserved[429];		///< 429 bytes
} BS2DeviceCapabilities;

#endif	// __BS2_DEVICE_INFO_H__
