#pragma once

#include <vector>
#include <string>
#include "BS_API.h"


class LogControl
{
public:
	LogControl(void* sdkContext);
	virtual ~LogControl();

	static std::string getEventString(BS2_DEVICE_ID id, const BS2Event& event, int32_t timezone, BS2_TEMPERATURE temperature = 0);

public:
	int getLogSmallBlob(BS2_DEVICE_ID id);
	int getLogSmallBlobEx(BS2_DEVICE_ID id);

//private:
	static void print(const BS2Event& log);
	static void print(const BS2Event* logs, size_t numLog);
	static void print(const BS2EventBlob& log);
	static void print(const BS2EventBlob* logs, size_t numLog);
	static void print(const BS2EventSmallBlob& log);
	static void print(const BS2EventSmallBlob* logs, size_t numLog);
	static void print(const BS2EventSmallBlobEx& log);
	static void print(const BS2EventSmallBlobEx* logs, size_t numLog);
	static void print(const BS2EventExtInfo& info);


private:
	static void* context_;
};
