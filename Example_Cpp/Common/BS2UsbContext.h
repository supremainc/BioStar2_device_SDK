#pragma once

#include "BS_API.h"
#include <vector>
#include "BS2Context.h"


class BS2UsbContext : public BS2InstantContext
{
public:
	BS2UsbContext(std::string dir);
	virtual ~BS2UsbContext();

	static std::string getUSBPath();

	int getUserDatabaseInfoFromDir(uint32_t& numUsers, uint32_t& numCards, uint32_t& numFingers, uint32_t& numFaces, IsAcceptableUserID cbAcceptableUID);
	int getUserListFromDir(IsAcceptableUserID cbAcceptableUID, std::vector<std::string>& userIDs);
	int getUserInfosFromDir(const std::vector<std::string>& userIDs, std::vector<BS2UserBlob>& userBlob);
	int getUserDatasFromDir(const std::vector<std::string>& userIDs, BS2_USER_MASK userMask, std::vector<BS2UserBlob>& userBlob);
	int getUserInfosExFromDir(const std::vector<std::string>& userIDs, std::vector<BS2UserBlobEx>& userBlob);
	int getUserDatasExFromDir(const std::vector<std::string>& userIDs, BS2_USER_MASK userMask, std::vector<BS2UserBlobEx>& userBlob);
	int getUserSmallInfosFromDir(const std::vector<std::string>& userIDs, std::vector<BS2UserSmallBlob>& userBlob);
	int getUserSmallDatasFromDir(const std::vector<std::string>& userIDs, BS2_USER_MASK userMask, std::vector<BS2UserSmallBlob>& userBlob);
	int getUserSmallInfosExFromDir(const std::vector<std::string>& userIDs, std::vector<BS2UserSmallBlobEx>& userBlob);
	int getUserSmallDatasExFromDir(const std::vector<std::string>& userIDs, BS2_USER_MASK userMask, std::vector<BS2UserSmallBlobEx>& userBlob);
	int getUserInfosFaceExFromDir(const std::vector<std::string>& userIDs, std::vector<BS2UserFaceExBlob>& userBlob);
	int getUserDatasFaceExFromDir(const std::vector<std::string>& userIDs, BS2_USER_MASK userMask, std::vector<BS2UserFaceExBlob>& userBlob);

	int getFilteredLogFromDir(char* uid, BS2_EVENT_CODE eventCode, BS2_TIMESTAMP startTime, BS2_TIMESTAMP endTime, uint8_t tnaKey, std::vector<BS2Event>& eventList);
	int getLogFromDir(BS2_EVENT_ID eventId, uint32_t amount, std::vector<BS2Event>& eventList);
	int getLogBlobFromDir(uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, std::vector<BS2EventBlob>& eventList);
	int getLogSmallBlobFromDir(uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, std::vector<BS2EventSmallBlob>& eventList);
	int getLogSmallBlobExFromDir(uint16_t eventMask, BS2_EVENT_ID eventId, uint32_t amount, std::vector<BS2EventSmallBlobEx>& eventList);


private:
	std::string dir_;
	bool inited_;
};