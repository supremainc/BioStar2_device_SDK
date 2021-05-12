#pragma once

#include <vector>
#include <string>
#include "BS_API.h"


class AccessControl
{
public:
	AccessControl(void* sdkContext);
	virtual ~AccessControl();


public:
	int getAccessSchedule(BS2_DEVICE_ID id, BS2Schedule& schedule);
	int setAccessSchedule(BS2_DEVICE_ID id, const BS2Schedule& schedule);
	int getAllAccessSchedule(BS2_DEVICE_ID id, std::vector<BS2Schedule>& schedules);


private:
	void print(const BS2Schedule& schedule) const;
	void print(const BS2DaySchedule& schedule) const;
	void print(const BS2HolidaySchedule schedule) const;


private:
	void* context_;
};