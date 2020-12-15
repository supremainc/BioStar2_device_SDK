#pragma once

#include <vector>
#include <string>
#include "BS_API.h"


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
	int getTriggerActionConfig(BS2_DEVICE_ID id, BS2TriggerActionConfig& config);
	int setTriggerActionConfig(BS2_DEVICE_ID id, const BS2TriggerActionConfig& config);

	int updateConnectionModeViaUDP(BS2_DEVICE_ID id, BS2_CONNECTION_MODE mode = BS2_CONNECTION_MODE_SERVER_TO_DEVICE);
	int updateConnectModeDevice2Server(BS2_DEVICE_ID id, std::string serverIP, BS2_PORT serverPort);
	int updateConnectModeServer2Device(BS2_DEVICE_ID id);
	int updateRS485OperationMode(BS2_DEVICE_ID id, BS2_RS485_MODE mode = BS2_RS485_MODE_DEFAULT);

	int resetConfigExceptNetInfo(BS2_DEVICE_ID id, bool includeDB = true);

private:
	void print(const BS2SystemConfig& config) const;
	void print(const BS2DisplayConfig& config) const;
	void print(const BS2IpConfig& config) const;
	void print(const BS2FactoryConfig& config) const;
	void print(const BS2FingerprintConfig& config) const;
	void print(const BS2FaceConfig& config) const;
	void print(const BS2DesFireCardConfigEx& config) const;
	void print(const BS2AuthConfigExt& config) const;
	void print(const BS2FaceConfigExt& config) const;
	void print(const BS2ThermalCameraConfig& config) const;
	void print(const BS2InputConfig& config) const;
	void print(const BS2SupervisedInputConfig& config) const;
	void print(const BS2SVInputRange& inputRange) const;
	void print(const BS2TriggerActionConfig& config) const;
	void print(const BS2Trigger& trigger) const;
	void print(const BS2Action& action) const;


private:
	void* context_;
};
