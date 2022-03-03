#pragma once

#include <vector>
#include <string>
#include "BS_API.h"

#include "Utility.h"

class ConfigControl
{
public:
	ConfigControl(void* sdkContext);
	virtual ~ConfigControl();


public:
	int getConfig(BS2_DEVICE_ID id, BS2Configs& config);
	int getSystemConfig(BS2_DEVICE_ID id, BS2SystemConfig& config);
	int setSystemConfig(BS2_DEVICE_ID id, const BS2SystemConfig& config);
	int getTimezone(BS2_DEVICE_ID id, int& timezone);
	int getDisplayConfig(BS2_DEVICE_ID id, BS2DisplayConfig& config);
	int setDisplayConfig(BS2_DEVICE_ID id, const BS2DisplayConfig& config);
	int getIPConfig(BS2_DEVICE_ID id, BS2IpConfig& config) const;
	int setIPConfig(BS2_DEVICE_ID id, const BS2IpConfig& config);
	int getFactoryConfig(BS2_DEVICE_ID id, BS2FactoryConfig& config);
	int getInputConfig(BS2_DEVICE_ID id, BS2InputConfig& config);

	int getFingerprintConfig(BS2_DEVICE_ID id, BS2FingerprintConfig& config);
	int setFingerprintConfig(BS2_DEVICE_ID id, const BS2FingerprintConfig& config);
	int getFaceConfig(BS2_DEVICE_ID id, BS2FaceConfig& config);
	int setFaceConfig(BS2_DEVICE_ID id, const BS2FaceConfig& config);
	int getDesFireCardConfigEx(BS2_DEVICE_ID id, BS2DesFireCardConfigEx& config);
	int setDesFireCardConfigEx(BS2_DEVICE_ID id, const BS2DesFireCardConfigEx& config);
	int getAuthConfigEx(BS2_DEVICE_ID id, BS2AuthConfigExt& config);
	int setAuthConfigEx(BS2_DEVICE_ID id, const BS2AuthConfigExt& config);
	int getFaceConfigEx(BS2_DEVICE_ID id, BS2FaceConfigExt& config);
	int setFaceConfigEx(BS2_DEVICE_ID id, const BS2FaceConfigExt& config);
	int getThermalCameraConfig(BS2_DEVICE_ID id, BS2ThermalCameraConfig& config);
	int setThermalCameraConfig(BS2_DEVICE_ID id, const BS2ThermalCameraConfig& config);
	int getEventConfig(BS2_DEVICE_ID id, BS2EventConfig& config);
	int setEventConfig(BS2_DEVICE_ID id, const BS2EventConfig& config);
	int getTriggerActionConfig(BS2_DEVICE_ID id, BS2TriggerActionConfig& config);
	int setTriggerActionConfig(BS2_DEVICE_ID id, const BS2TriggerActionConfig& config);
	int getBarcodeConfig(BS2_DEVICE_ID id, BS2BarcodeConfig& config);
	int setBarcodeConfig(BS2_DEVICE_ID id, const BS2BarcodeConfig& config);
	int getRS485Config(BS2_DEVICE_ID id, BS2Rs485Config& config);
	int setRS485Config(BS2_DEVICE_ID id, const BS2Rs485Config& config);
	int getInputConfigEx(BS2_DEVICE_ID id, BS2InputConfigEx& config);
	int setInputConfigEx(BS2_DEVICE_ID id, const BS2InputConfigEx& config);
	int getRelayActionConfig(BS2_DEVICE_ID id, BS2RelayActionConfig& config);
	int setRelayActionConfig(BS2_DEVICE_ID id, const BS2RelayActionConfig& config);

	int updateConnectionModeViaUDP(BS2_DEVICE_ID id, BS2_CONNECTION_MODE mode = BS2_CONNECTION_MODE_SERVER_TO_DEVICE);
	int updateConnectModeDevice2Server(BS2_DEVICE_ID id, std::string serverIP, BS2_PORT serverPort);
	int updateConnectModeServer2Device(BS2_DEVICE_ID id);
	int updateRS485OperationMode(BS2_DEVICE_ID id, BS2_RS485_MODE mode = BS2_RS485_MODE_DEFAULT);

	int resetConfigExceptNetInfo(BS2_DEVICE_ID id, bool includeDB = true);

public:
	static void print(const BS2SystemConfig& config);
	static void print(const BS2DisplayConfig& config);
	static void print(const BS2IpConfig& config);
	static void print(const BS2FactoryConfig& config);
	static void print(const BS2FingerprintConfig& config);
	static void print(const BS2FaceConfig& config);
	static void print(const BS2DesFireCardConfigEx& config);
	static void print(const BS2AuthConfigExt& config);
	static void print(const BS2FaceConfigExt& config);
	static void print(const BS2ThermalCameraConfig& config);
	static void print(const BS2EventConfig& config);
	static void print(const BS2InputConfig& config);
	static void print(const BS2SupervisedInputConfig& config);
	static void print(const BS2SVInputRange& inputRange);
	static void print(const BS2TriggerActionConfig& config);
	static void print(const BS2Trigger& trigger);
	static void print(const BS2Action& action);
	static void print(const BS2BarcodeConfig& config);
	static void print(const BS2Rs485Config& config);
	static void print(const BS2Rs485Channel& channel);
	static void print(const BS2Rs485SlaveDevice& device);
	static void print(const BS2InputConfigEx& config);
	static void print(const BS2RelayActionConfig& config);


private:
	void* context_;
};
