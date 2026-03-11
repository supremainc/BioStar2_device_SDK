#pragma once

#include <string>
#include <sstream>

class BS2EventStringUtil {
public:
	static std::string getEventString(uint16_t eventCode);

private:
	static std::string getComplexEventString(uint16_t eventCode);
	static std::string getMainEventString(uint16_t mainEvent);
	static std::string getSubEventString(uint16_t mainEvent, uint16_t subEvent);

	static std::string getSubEventStringVerify(uint16_t subEvent);
	static std::string getSubEventStringVerifyFail(uint16_t subEvent);
	static std::string getSubEventStringIdentify(uint16_t subEvent);
	static std::string getSubEventStringDualAuthFail(uint16_t subEvent);
	static std::string getSubEventStringAuthFail(uint16_t subEvent);
	static std::string getSubEventStringAccessDenied(uint16_t subEvent);
	static std::string getSubEventStringBypassFail(uint16_t subEvent);
	static std::string getSubEventStringAbnormalTemperature(uint16_t subEvent);
	static std::string getSubEventStringUnmaskedFace(uint16_t subEvent);
	static std::string getSubEventStringUserEnroll(uint16_t subEvent);
	static std::string getSubEventStringUserEnrollFail(uint16_t subEvent);
	static std::string getSubEventStringUserUpdateFail(uint16_t subEvent);
	static std::string getSubEventStringDoor(uint16_t subEvent);
	static std::string getSubEventStringFloor(uint16_t subEvent);
	static std::string getSubEventStringAPBZone(uint16_t subEvent);
	static std::string getSubEventStringEntranceLimitZone(uint16_t subEvent);
	static std::string getSubEventStringInterlockZone(uint16_t subEvent);
	static std::string getSubEventStringAuthLimitZone(uint16_t subEvent);
	static std::string getSubEventStringDeviceIO(uint16_t subEvent);
};