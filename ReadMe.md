### What is the BioStar?

BioStar2 is a web-based, open, and integrated security platform that provides comprehensive functionality for access control and time & attendance. Featuring a modular, flexible framework, the platform supports both the BioStar 2 Device SDK which is used to integrate Suprema's terminals with thirdparty systems and web APIs which are used to integrate the functionality of the BioStar 2 platform with third-party systems. In addition, in step with the mobile era, the mobile app for BioStar 2 has been designed to enable you not only to remotely control the BioStar 2 platform but also to get a mobile card issued to you for use.
[BioStar2 Overview](https://www.supremainc.com/en/platform/hybrid-security-platform-biostar-2.asp)[, Product](https://www.supremainc.com/en/hardware/security-products-lineup.asp)

### Overview of the BioStar2 Device SDK

BioStar2 Device SDK development framework allows developers to seamlessly integrate core functionality of Suprema biometric devices and terminals with 3rd party software.
[SDK Introduction](https://www.supremainc.com/en/support/development-tools_biostar-2-sdk.asp)

### Get Started

SDK supports the following platforms:
* [SDK files and folders structure](https://kb.supremainc.com/bs2sdk/doku.php?id=en:getting_started#sdk_components)
* [Windows 32-bit](https://github.com/supremainc/BioStar2_device_SDK/tree/master/SDK_Release)
* [Windows 64-bit](https://github.com/supremainc/BioStar2_device_SDK/tree/master/SDK_Release)
* [Ubuntu 18.04 32-bit](https://github.com/supremainc/BioStar2_device_SDK/tree/master/SDK_Release)
* [Ubuntu 18.04 64-bit](https://github.com/supremainc/BioStar2_device_SDK/tree/master/SDK_Release)

### Learn More About The BioStar2 Device SDK

![Framework](http://kb.supremainc.com/bs2sdk/lib/exe/fetch.php?w=600&tok=70cf2a&media=ko:biostar_2_sdk_framework_1.png)

![Workflow](http://kb.supremainc.com/bs2sdk/lib/exe/fetch.php?w=650&tok=9f4e6e&media=ko:workflow.png)

Learn how to create a context for device management and connect devices. [Watch this tutorial](http://kb.supremainc.com/bs2sdk./doku.php?id=en:quick_guide)

### Example

**1.** The SDK currently being distributed comes with C # examples. This github provides additional C ++ examples. 
* [C++ Example](https://github.com/supremainc/BioStar2_device_SDK/tree/master/Example_Cpp)(Write in Visual C++ 2015)

**2.** To run the example, after installing the supplied SDK (Maybe 'C:/Program Files (x86)/BioStar 2 Device SDK'), copy the Include and Lib directories to the appropriate path.
* [C++ Example Include](https://github.com/supremainc/BioStar2_device_SDK/tree/master/Example_Cpp/Include)
* [C++ Example Lib](https://github.com/supremainc/BioStar2_device_SDK/tree/master/Example_Cpp/Lib)
* [C# Example Lib](https://github.com/supremainc/BioStar2_device_SDK/tree/master/Example_Csharp/cli/csharp/common/lib)

<br>
<br>

### Release Notes
<br>

**Note**: Feature enhancements, updates, and resolved issues from previous releases are available to view in [Release Notes](http://kb.supremainc.com/bs2sdk/doku.php?id=en:release_note).
<br>
<br>

### V2.8.2.9 released 04/14/2022:

**Modified Features**

**1.** Fix the problem that the device connection is disconnected in case of a simple communication error in SSL connection mode.

<br>

### V2.8.2.3 released 11/29/2021:

**Modified Features**

**1.** Supported Linux environments have been upgraded to Ubuntu 18.04.
|SDK Version|Supported Linux Version|
|---|---|
|Before V2.8.2|Ubuntu 16.04|
|Later V2.8.2|Ubuntu 18.04|

**2.** OpenSSL has been modified to support 1.1.1i from the existing 1.0.2n.
* Refer to Building a Development Environment

**3.** Fixed an issue where a BS_SDK_ERROR_SOCKET_IS_NOT_CONNECTED error occurred when calling BS2_GetFactoryConfig on an IPv6 operating device.

**4.** Fixed an issue that could cause a BS_SDK_ERROR_TIMEOUT error when enrolling many image-based users on FSF2 devices.

**5.** Fixed the problem that BS2_SetDebugExCallback does not work after 2.7.2.20.

**6.** Removed incorrect support configuration for FSF2 (WLAN).

**7.** Removed incorrect support configuration for XS2 (VoIP).

**8.** Added incorrect support configuration for BER2 devices (Wiegand).

**9.** Fix reserved2 resizing error due to adding useScreenSaver in C# example.

**10.** There were fixes for stabilizing the code inside the SDK.

<br>

### V2.8.1.0 released 08/05/2021:

**New Features**

* Supports X-Station 2 Finger (Product Name: XS2-OAPB, XS2-ODPB)
* Supports Input Module Feature (Product Name: IM-120)
* Supports the function of commanding certain actions to devices

**1.** X-Station 2 Finger Configurations are available to use.

**2.** Input Module(Product Name: IM-120) Configurations are available to use.
* Refer to Structure BS2InputConfigEx
* Refer to Structure BS2RelayActionConfig
* Refer to Structure BS2Event relayAction
* Refer to API BS2_GetInputConfigEx
* Refer to API BS2_SetInputConfigEx
* Refer to API BS2_GetRelayActionConfig
* Refer to API BS2_SetRelayActionConfig

**3.** You can now command certain actions to devices.
You can now turn on LED, alarm a buzzer, or control relay and perform more actions at the desired time with the COMMANDS below.
* Refer to Structure BS2Action
* Refer to API BS2_RunAction

**Modified Features**

**1.** Fixed the error in which Linux returns BS_SDK_ERROR_TIMEOUT and is not able to have device connections.

**2.** C# examples provided to guide through server-matching function implementation will not be supported anymore.
* Refer to the Server API Document Server API

<br>

### V2.8.0.6 released 06/07/2021:

**New Features**

* Supports X-Station 2
* Supports Intelligent Slave Feature (FW upgrade is required)
* Supports getting Face Image Warping (FaceStation F2 only)
* Supports the function of Device Capabilities to know the connected device capability
* Adds the relevant log events of Mobile Card Verification

**1.** X-Station 2 Configurations are available to use.
* Structure BS2BarcodeConfig has been added
* API BS2_GetBarcodeConfig has been added
* API BS2_SetBarcodeConfig has been added
* API BS2_WriteQRCode has been added

**2.** The relevant function of Intelligent Slave is available to use. Intelligent Slave is a function that enables many users' 1:1 or 1:N matching by transmitting the authentication result to OSDP card data when authentication using biometrics is performed in an environment in which the Suprema biometric device and a third-party controller are connected. Intelligent Slave Support.
* Refer to structure BS2Rs485Config
* Refer to API BS2_GetRS485Config
* Refer to API BS2_SetRS485Config

**3.** Getting Image Warping from the raw image file of user face is available to use.
* Refer to API BS2_GetNormalizedImageFaceEx

**4.** Getting Device Capacity Information has been added.
Refer to structure BS2DeviceCapabilities
Refer to API BS2_GetDeviceCapabilities

**5.** New log event 'Mobile Card Verification' has been added to distinguish the user credential.
Refer to structure BS2Event and refer to the subcode

**Modified Features**

**1.** Fixed the return value from BS_SDK_SUCCESS to BS_SDK_ERROR_NOT_SUPPORTED when requesting Config from FaceLite that does not support Event config.
Refer to API BS2_GetEventConfig

**2.** Fixed the issue that the SDK allocation and release were repeated, making device connection impossible if a certain number of times were exceeded.
Refer to API BS2_AllocateContext
Refer to API BS2_ReleaseContext

**3.** Fixed the issue that calling BS2_ReleaseContext cannot release the resource intermittently and the port 51212/port 51213 occupied.

**4.** Fixed the issue that a deadlock could occur during event processing due to repeated connecting device/disconnecting device calls.

**5.** Fixed the issue that the device could be reset if the fields of timeout were not assigned a multiple of 10 when updating the display settings.
Refer to structure BS2DisplayConfig menuTimeout, msgTimeout, backlightTimeout
Refer to API BS2_SetDisplayConfig

**6.** Fixed the issue that CPU Usages goes up to 99% when BS2_SearchDevices is called repeatedly in a 4-core environment.

**7.** FaceStation F2 Only The meaning and setting values of lightCondition in the BS2FaceConfig have been changed as follows.
|Supported Ver.|Meaning|0|1|2|3|
|---|---|---|---|---|---|
|1.0.0 ~ 1.0.5|Ambient Brightness|Indoor|Outdoor|Auto|-|
|1.1.0 or higher|Light Brightness|Normal|High|- `(Deprecated)`|Not used|

<br>

### V2.7.2.12 released 12/11/2020:

**New Features**

* Supports screensaver activation (FaceStation 2, FaceStation F2)
* FaceStation F2 server matching support (Card only)

**1.** You can turn the screensaver on and off. FaceStation 2, FaceStation F2 only.
* Refer to structure BS2DisplayConfig
* Refer to API BS2_GetDisplayConfig
* Refer to API BS2_SetDisplayConfig

**2.** FaceStation F2 server matching support. (Card only)
* Refer to API BS2_SetServerMatchingHandler
* Refer to API BS2_VerifyUserFaceEx


**Modified Features**

**1.** Fixed an issue where devices could not connect normally when in Device to Server connection mode.

**2.** In the process of unloading SDK through BS2_ReleaseContext, a problem that may not be normally unloaded due to a specific thread has been fixed.

**3.** Body temperature measurement range has been modified. (30º ~ 45º => 1º ~ 45º)
* Refer to structure BS2FaceConfigExt (thermalThresholdLow, thermalThresholdHigh)

**4.** Fixed a problem where BS_SDK_ERROR_NOT_SUPPORTED or BS_SDK_ERROR_TIMEOUT error was returned  when enrolling a user with multiple image to FaceStation F2.
* Refer to API BS2_EnrollUserFaceEx


**5.** Fixed a problem in which a specific function was called from some devices except BioStation 2, but the BS_SDK_ERROR_TIMEOUT error was returned as a result value.
* Refer to API BS2_ResetConfigExceptNetInfo
<br>
<br>

### V2.7.2.4 released 11/11/2020:

**New Features**

**1.** You can select either Card ID or User ID for Wiegand output setting.
* Refer to structure BS2WiegandConfig
* Refer to API BS2_GetWiegandConfig
* Refer to API BS2_SetWiegandConfig


**Modified Features**

**1.** File log output structure of internal SDK has been changed.
Before, it used to write a file from each event processing thread, but now there's a separate file writing thread.
It leads to improve the speed of event processing.
* Refer to API BS2_SetDebugFileLog

**2.** When enrolling multiple face users to FaceStation F2,
Fixed an issue that could cause the BS_SDK_ERROR_NO_FACE_CREDENTIAL(-759) error.
* Refer to API BS2_EnrollUserFaceEx

**3.** Fixed an issue where the callback may not work intermittenly when the device is disconnected.
* Refer to API BS2_SetDeviceEventListener

**4.** There were fixes for stabilizing the code inside the SDK.
<br>
<br>

### V2.7.1.1 released 09/22/2020:

**New Features**

**1.** FaceStation F2 is supported, user related structures & API are added.
* Structure BS2UserSettingEx has been added
* Structure BS2TemplateEx has been added
* Structure BS2FaceEx has been added
* Structure BS2UserFaceExBlob has been added
* API BS2_ScanFaceEx has been added
* API BS2_ExtractTemplateFaceEx has been added
* API BS2_EnrollUserFaceEx has been added
* API BS2_GetUserInfosFaceEx has been added
* API BS2_GetUserDatasFaceEx has been added

**2.** Several authentication modes for FaceStation F2 have been added, and you can configure with API and structure below.
* Structure BS2AuthConfigExt has been added
* API BS2_GetAuthConfigExt has been added
* API BS2_SetAuthConfigExt has been added

**3.** You can extract template by jpg image from FaceStation F2. The template file can be stored on AoC.
* Structure BS2SmartCardHeader has been added
* API BS2_ExtraceTemplateFaceEx has been added

**4.** Thermal camera - Facial temperature & Mask detection can be configured with API & structure below.
* Structure BS2FaceConfigExt has been added
* API BS2_GetFaceConfigExt has been added
* API BS2_SetFaceConfigExt has been added

**5.** Thermal camera information structure and configurable API are available.
* Structure BS2ThermalCameraConfig has been added
* API BS2_GetThermalCameraConfig has been added
* API BS2_SetThermalCameraConfig has been added

**6.** New event log structure & API to support temperature information have been added.
* Callback function OnLogReceivedEx has been added
* Structure BS2EventSmallBlobEx has been added
* API BS2_StartMonitoringLogEx has been added
* API BS2_GetLogSmallBlobEx has been added
* API BS2_GetLogSmallBlobExFromDir has been added

**7.** User-defined PIN encryption key can be changed, and the PIN can be extracted with APIs.
* API BS2_SetDataEncryptKey has been added
* API BS2_MakePinCodeWithKey has been added


**Modified features**

**1.** A structure is modified to support FaceStation F2's configuration.
* Structure BS2FaceConfig has been added

**2.** Masking information of some API retrieving user information with userMask has been added. (Refer to userMask parameter for each API)
* API BS2_GetUserDatas has been added
* API BS2_GetUserDatasEx has been added
* API BS2_GetSupportedUserMask has been added
* API BS2_GetUserSmallDatas has been added
* API BS2_GetUserSmallDatasEx has been added
* API BS2_GetUserDatasFromDir has been added
* API BS2_GetUserDatasExFromDir has been added
* API BS2_GetUserSmallDatasFromDir has been added
* API BS2_GetUserSmallDatasExFromDir has been added
* API BS2_GetUserInfosFaceEx has been added
* API BS2_GetUserDatasFaceEx has been added

**3.** In the past, parameters like fingerSupported, faceSupported were used meaning fingerprint authentication(matching) or facial authentication(matching).
This was confusing because FaceStation2 or returned fingerSupported = true.
From 2.7.1, additional information fingerScanSupported and faceScanSupported is supported to provide more accurate information.
With this, FaceStation2 returns fingerScanSupported = false.
* API BS2_GetDeviceInfoEx has been added
* Structure BS2SimpleDeviceInfoEx has been added

**4.** For security reasons, support for the function BS2_GetDataEncryptKey has been deprecated.
<br>
<br>

### V2.7.0.0 released 07/21/2020:

**New Features**

**1.** Device XPass D2 Revision, XPass D2 Keypad Revision are supported.

**2.** Lift lock/unlock zone which works based on schedules regardless of access group is supported.
  * Structure BS2LiftFloors has been added
  * Structure BS2LiftLockUnlockZone has been added
  * API BS2_GetLiftLockUnlockZone has been added
  * API BS2_GetAllLiftLockUnlockZone has been added
  * API BS2_GetLiftLockUnlockZoneStatus has been added
  * API BS2_GetAllLiftLockUnlockZoneStatus has been added
  * API BS2_SetLiftLockUnlockZone has been added
  * API BS2_SetLiftLockUnlockZoneAlarm has been added
  * API BS2_RemoveLiftLockUnlockZone has been added
  * API BS2_RemoveAllLiftLockUnlockZone has been added

**3.** User phrase feature that you can request the user phrase(private message) to the server is supported.
  * Structure BS2DisplayConfig has been added
  * API BS2_SetUserPhraseHandler has been added
  * API BS2_ResponseUserPhrase has been added

**4.** Global APB By Door Open is supported.
Global APB By Door Open has two steps; i) User authentication ii) Actual access based on the door sensor 
You can figure out the user who actually doesn't go in even after successful authentication and the door unlock using this feature.
  * Structure BS2DoorSensor has been added
  * API BS2_SetGlobalAPBViolationByDoorOpenHandler has been added
  * API BS2_CheckGlobalAPBViolationByDoorOpen has been added

**Modified Features**

**1.** Fixed the issue that the device returns BS_SDK_ERROR_CANNOT_FIND_DEVICE after successful connection due to corrupted communication key exchange. (Only some latest FW)

**2.** Fixed the issue that SDK can't process received packets from the device with IPv6 connection mode.
  * Structure BS2UserBlobEx has been added
  * API BS2_VerifyUserEx has been added
  * API BS2_IdentifyUserEx has been added
  * API BS2_EnrolUserEx has been added
  * API BS2_VerifyUserSmallEx has been added
  * API BS2_IdentifyUserSmallEx has been added
  * API BS2_EnrollUserSmallEx has been added

**3.** Fixed the issue with the error BS2_SDK_ERROR_INVALID_PACKET when received log from the device due to the image log larger than 16KB.
  * API BS2_GetLogBlob has been added
  * API BS2_GetLogSmallBlob has been added
  * API BS2_GetLogBlobFromDir has been added
  * API BS2_GetLogSmallBlobFromDir has been added
  * API BS2_GetImageLog has been added
<br>
<br>

### V2.6.4.1 released 02/25/2020:

**New Features**

**1.** The API retrieving logs filtered by Event Mask so that you can use memory efficiently than before.
* Structure BS2EventSmallBlob has been added
* API BS2_GetLogSmallBlob has been added
* API BS2_GetLogSmallBlobFromDir has been added

**2.** DesFire Advanced format setting is now supported.
* Structure BS2SimpleDeviceInfoEx has been added
* Structure BS2DesFireCardConfigEx has been added
* API BS2_GetDesFireCardConfigEx has been added
* API BS2_SetDesFireCardConfigEx has been added

**3.** Card type selection is available so that you can select/deselect the card type you want to read fro the device.
* Structure BS2SystemConfig has been added
* API BS2_SetSystemConfig has been added

**4.** Duplicate fingerprint and face check is available.
* Structure BS2FaceConfig has been added
* API BS2_GetFingerprintConfig has been added
* API BS2_SetFingerprintConfig has been added
* Structure BS2FingerprintConfig has been added
* API BS2_GetFaceConfig has been added
* API BS2_SetFaceConfig has been added

**Modified Features**

**1.** Fixed the issue of device malfunctioning and rebooting when BS2Job.numJobs in BS2UserBlobEx exceeds the maximum value 16.
* Refer to structure BS2UserBlobEx
* Refer to API BS2_VerifyUserEx
* Refer to API BS2_IdentifyUserEx
* Refer to API BS2_EnrolUserEx
* Refer to API BS2_VerifyUserSmallEx
* Refer to API BS2_IdentifyUserSmallEx
* Refer to API BS2_EnrollUserSmallEx

**2.** Fixed the issue of BioStation 2 not available for user images.

**3.** Fixed the error BS_SDK_INVALID_CODE_SIGN when upgrading SIO2.

**4.** Fixed the error BS_SDK_ERROR_NOT_SUPPORTED when calling BS2_GetCardModel for BLN2-PAB.
* Refer to API BS2_GetCardModel

**5.** Fixed the issue that SDK doesn't recognize device disconnection when the device is constantly disconnected.

**6.** Fixed the issue of Linux SDK crash.
<br>
<br>

### V2.6.3.16 released 11/04/2019:

**Modified features**

**1.** Fixed the problem where the devices listed below may fail to connect when connecting.
* BioEntry Plus 2.0
* BioEntry W 2.0
* BioLiteNet 2.0
* Xpass 2.0
* Xpass Slim 2.0

**2.** Fixes a problem where the return value dualIDSupported and useAlphanumericID becomes 0 when BS2_GetDeviceInfo is called after directly connection a device by IP or port instead of searching for and connecting a device.
* Refer to API BS2_SearchDevices
* Refer to API BS2_ConnectDevice
* Refer to API BS2_ConnectDeviceViaIP
* Refer to API BS2_GetDeviceInfo
* Refer to structure BS2SimpleDeviceInfo

**3.** Fixes a problem where a timeout error (BS_SDK_ERROR_TIMEOUT) may occur if the devices listed below attempt a factory reset.
* BioEntry Plus 2.0
* BioEntry W 2.0
* BioLiteNet 2.0
* Xpass 2.0
* Xpass Slim 2.0
Refer to API BS2_FactoryReset

**4.** Fixed a problem where the connection would be terminated normally if the device was disconnected and the packet was exchanged in the SDK without closing the connection yet.
<br>
<br>


### Release Notes (Beta)
<br>

### V2.8.2.7 Beta released 02/23/2022:

**New Features**

**1.** A barcode bypass function has been added so that you can subscribe to barcodes from devices that can scan barcodes.
* Refer to Structure BS2BarcodeConfig
* API BS2_SetBarcodeScanListener has been added
* Callback OnBarcodeScanned has been added

**2.** A function (Treat as CSN) has been added that allows you to treat Barcodes the same as regular CSN cards.
* Refer to Structure BS2BarcodeConfig

**3.** It has been modified to set the byte order of the smart card.
* Refer to Structure BS2CardConfig
* Refer to API BS2_GetCardConfig
* Refer to API BS2_SetCardConfig

**4.** Updated system support information for Smart card byte order, Treat as CSN function.
* Refer to Structure BS2DeviceCapabilities
* Refer to API BS2_GetDeviceCapabilities
<br>

### V2.8.2.5 Beta released 02/16/2022:

**New Features**

**1.** In devices supporting Ver.3 (GDPR supported), the following functions have been added to read USB files.
The functions below correspond to cases where the user information encryption key of the device has been changed by calling the BS2_SetDataEncryptKey function in the past.
For the newly added key parameter, you can specify the key set in the past.
* API BS2_GetUserDatabaseInfoFromDirWithKey has been added
* API BS2_GetUserListFromDirWithKey has been added
* API BS2_GetUserInfosFromDirWithKey has been added
* API BS2_GetUserDatasFromDirWithKey has been added
* API BS2_GetUserInfosExFromDirWithKey has been added
* API BS2_GetUserDatasExFromDirWithKey has been added
* API BS2_GetLogFromDirWithKey has been added
* API BS2_GetFilteredLogFromDirWithKey has been added
* API BS2_GetUserSmallInfosFromDirWithKey has been added
* API BS2_GetUserSmallDatasFromDirWithKey has been added
* API BS2_GetUserSmallInfosExFromDirWithKey has been added
* API BS2_GetUserSmallDatasExFromDirWithKey has been added
* API BS2_GetUserInfosFaceExFromDirWithKey has been added
* API BS2_GetUserDatasFaceExFromDirWithKey has been added
* API BS2_GetLogBlobFromDirWithKey has been added
* API BS2_GetLogSmallBlobFromDirWithKey has been added
* API BS2_GetLogSmallBlobExFromDirWithKey has been added

**Modified Features**

**1.** The USB file reading function has been modified to support Ver.3 (GDPR supported).
The functions below correspond to cases where the device's user information encryption key has never been changed by calling the BS2_SetDataEncryptKey function in the past.
The functions below decrypt the encryption information with the default key value and get it.

* Refer to API BS2_GetUserDatabaseInfoFromDir
* Refer to API BS2_GetUserListFromDir
* Refer to API BS2_GetUserInfosFromDir
* Refer to API BS2_GetUserDatasFromDir
* Refer to API BS2_GetUserInfosExFromDir
* Refer to API BS2_GetUserDatasExFromDir
* Refer to API BS2_GetLogFromDir
* Refer to API BS2_GetFilteredLogFromDir
* Refer to API BS2_GetUserSmallInfosFromDir
* Refer to API BS2_GetUserSmallDatasFromDir
* Refer to API BS2_GetUserSmallInfosExFromDir
* Refer to API BS2_GetUserSmallDatasExFromDir
* Refer to API BS2_GetUserInfosFaceExFromDir
* Refer to API BS2_GetUserDatasFaceExFromDir
* Refer to API BS2_GetLogBlobFromDir
* Refer to API BS2_GetLogSmallBlobFromDir
* Refer to API BS2_GetLogSmallBlobExFromDir

**2.** The following functions are deprecated.
* API BS2_AllocateUsbContext deprecated
* API BS2_ReleaseUsbContext deprecated

<br>
<br>

### V2.8.2.4 Beta released 01/13/2022:

**Modified Features**

**1.** Removed libxml2 library dependancy from SDK.
<br>
<br>

### V2.7.2.18 Beta released 04/19/2021:

**Modified Features**

**1.** Fixed a problem that could crash when calling BS2_ReleaseConext.
* Refer to API BS2_ReleaseContext
<br>
<br>

### V2.7.2.17 Beta released 04/12/2021:

**Modified Features**

**1.** Fixed BS2_GetEventConfig call result returned from BS_SDK_ERROR_NOT_SUPPORTED to BS_SDK_SUCCESS on FaceLite device.
* Refer to API BS2_GetEventConfig

**2.** Fixed a problem where the device connection could not be made by repeatedly calling BS2_AllocateContext, BS2_ReleaseContext, BS2_ConnectDeviceViaIP, BS2_DisconnectDevice in the SSL connection mode.
* Refer to API BS2_AllocateContext
* Refer to API BS2_ReleaseContext
* Refer to API BS2_ConnectDeviceViaIP
* Refer to API BS2_DisconnectDevice

**3.** BS2_GetCardModel function is modified to support the following device models.
* BEW2-ODPB
* BEW2-OAPB
* BEW2-OHPB

**4.** Added SSL connection example.
<br>
<br>

### V2.7.2.14 Beta released 01/06/2021:

**Modified Features**

**1.** Improved the problem that the transmission speed was slightly slower in the previous version when sending many users.
* Refer to API BS2_EnrolUser
* Refer to API BS2_EnrollUser
* Refer to API BS2_EnrolUserEx
* Refer to API BS2_EnrollUserEx
* Refer to API BS2_EnrollUserSmall
* Refer to API BS2_EnrollUserSmallEx
* Refer to API BS2_EnrollUserFaceEx
<br>
<br>

### V2.7.2.13 Beta released 12/24/2020:

**Modified Features**

**1.** `Master: CoreStation 40`, `Slave: FaceStation 2 / FaceLite` <br>
 When registering a user with a face template for FaceStation 2 and FaceLite to CoreStation, a problem that could fail with a BS_SDK_ERROR_TIMEOUT error has been fixed.
* Refer to API BS2_EnrolUser
* Refer to API BS2_EnrollUser
* Refer to API BS2_EnrolUserEx
* Refer to API BS2_EnrollUserEx
* Refer to API BS2_EnrollUserSmall
* Refer to API BS2_EnrollUserSmallEx
* Refer to API BS2_EnrollUserFaceEx
<br>
<br>

### V2.6.3.23 released 02/06/2020:
**1.** Fixed a problem where SDK may crash on Linux.
<br>
<br>

### V2.6.3.22 released 02/01/2020:
**1.** Fixes a problem where SDK will not detect disconnection when device disconnection is repeated.
<br>
<br>

### V2.6.3.21 released 01/15/2020:
**1.** Fixed BS_SDK_ERROR_NOT_SUPPORTED error when calling BS2_GetCardModel in BLN2-PAB model.
<br>
<br>

### V2.6.3.20 released 01/14/2020:
**1.** Fixes a problem where BS_SDK_INVALID_CODE_SIGN error occurs when upgrading SIO2 firmware.