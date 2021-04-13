#include "stdafx.h"
#include <iostream>
#include "AccessControl.h"
#include "BS_Errno.h"
#include "Utility.h"


extern void TRACE(const char* fmt, ...);
using namespace std;


AccessControl::AccessControl(void* sdkContext) : context_(sdkContext)
{
}


AccessControl::~AccessControl()
{
}


int AccessControl::getAccessSchedule(BS2_DEVICE_ID id, BS2Schedule& schedule)
{
	return BS_SDK_ERROR_NOT_SUPPORTED;
}

int AccessControl::setAccessSchedule(BS2_DEVICE_ID id, const BS2Schedule& schedule)
{
	return BS_SDK_ERROR_NOT_SUPPORTED;
}

int AccessControl::getAllAccessSchedule(BS2_DEVICE_ID id, vector<BS2Schedule>& schedules)
{
	BS2Schedule* scheduleObj = NULL;
	uint32_t numOfSchedule(0);
	int sdkResult = BS2_GetAllAccessSchedule(context_, id, &scheduleObj, &numOfSchedule);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("BS2_GetAllAccessSchedule call failed: %d", sdkResult);
		return sdkResult;
	}

	for (uint32_t idx = 0; idx < numOfSchedule; idx++)
	{
		//print(scheduleObj[idx]);
		schedules.push_back(scheduleObj[idx]);
	}

	BS2_ReleaseObject(scheduleObj);

	return sdkResult;
}

void AccessControl::print(const BS2Schedule& schedule) const
{
	TRACE("==[BS2Schedule]==");
	TRACE("id:%d", schedule.id);
	TRACE("name:%s", schedule.name);
	TRACE("isDaily:%d", schedule.isDaily);

	if (schedule.isDaily)
	{
		TRACE("== Daily schedule (%u) ==", schedule.schedule.daily.numDays);
		TRACE("startDate:%d", schedule.schedule.daily.startDate);
		TRACE("numDays:%d", schedule.schedule.daily.numDays);
		for (uint8_t idx = 0; idx < schedule.schedule.daily.numDays; idx++)
			print(schedule.schedule.daily.schedule[idx]);
	}
	else
	{
		TRACE("== Weekly schedule ==");
		for (uint8_t idx = 0; idx < BS2_NUM_WEEKDAYS; idx++)
			print(schedule.schedule.weekly.schedule[idx]);
	}

	TRACE("== Holiday schedule ==");
	TRACE("numHolidaySchedules:%d", schedule.numHolidaySchedules);
	for (uint8_t idx = 0; idx < schedule.numHolidaySchedules; idx++)
		print(schedule.holidaySchedules[idx]);
}

void AccessControl::print(const BS2DaySchedule& schedule) const
{
	TRACE("numPeriods:%u", schedule.numPeriods);
	if (0 < schedule.numPeriods)
	{
		stringstream msg;
		for (uint32_t idx = 0; idx < schedule.numPeriods; idx++)
		{
			msg << "[" << idx << "]";
			msg << "s:" << (uint32_t)schedule.periods[idx].startTime << ",";
			msg << "e:" << (uint32_t)schedule.periods[idx].endTime << ", ";
		}

		TRACE("%s", msg.str().c_str());
	}
};

void AccessControl::print(const BS2HolidaySchedule schedule) const
{
	TRACE("id:%u", schedule.id);
	print(schedule.schedules);
}
