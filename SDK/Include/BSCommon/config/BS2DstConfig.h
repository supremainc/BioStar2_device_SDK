/*
 * BS2DstConfig.h
 *
 *  Created on: 2016. 8. 17.
 *      Author: smlee
 */

#ifndef BS2DSTCONFIG_H_
#define BS2DSTCONFIG_H_

#include "../BS2Types.h"

enum {
	BS2_MAX_DST_SCHEDULE = 2,
};

typedef struct {
// #ifdef DST_YEAR_SUPPORT
	BS2_YEAR year;				///< 2 bytes, year, 0 means every year.
// #else
// 	uint8_t reserved[2];
// #endif
	BS2_MONTH month;			///< 1 byte, [0, 11] : months since January
	BS2_ORDINAL ordinal;		///< 1 byte, [0, -1] : first, second, ..., last
	BS2_WEEKDAY weekDay;		///< 1 byte, [0, 6] : days since Sunday
	uint8_t hour;				///< 1 byte, [0, 23]
	uint8_t minute;				///< 1 byte, [0, 59]
	uint8_t second;				///< 1 byte, [0, 59]
} BS2WeekTime;

typedef struct {
	BS2WeekTime startTime;		///< 8 bytes, When the clock time hits this value, the clock should be put forward as much as the offset.
	BS2WeekTime endTime;		///< 8 bytes, When the clock time hits this value, the clock should be put backward as much as the offset.
	int32_t timeOffset;			///< 4 bytes, in seconds
	uint8_t reserved[4];		///< 4 bytes (packing)
} BS2DstSchedule;

typedef struct {
	uint8_t numSchedules;		///< 1 byte
	uint8_t reserved[31];		///< 31 bytes (packing)

	BS2DstSchedule schedules[BS2_MAX_DST_SCHEDULE];		///< 24 * 2 bytes
} BS2DstConfig;

#endif /* BS2DSTCONFIG_H_ */
