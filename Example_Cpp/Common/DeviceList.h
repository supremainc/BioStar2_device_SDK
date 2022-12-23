#pragma once

#include <string>
#include <map>
#include <vector>
#include <memory>
#include <mutex>
#include "BS_API.h"


typedef struct  
{
	BS2_DEVICE_ID		id;
	BS2_DEVICE_TYPE		type;
} BS2_DEVICE_ID_TYPE;

class DeviceInfo
{
public:
	DeviceInfo(BS2_DEVICE_ID id, BS2_DEVICE_TYPE type, uint32_t ip, BS2_PORT port, int32_t timezone) : id_(id), type_(type), ip_(ip), port_(port), timezone_(timezone), connected_(false) {}

	BS2_DEVICE_ID id_;
	BS2_DEVICE_TYPE type_;
	uint32_t ip_;
	BS2_PORT port_;
	int32_t timezone_;
	bool connected_;
	std::vector<BS2_DEVICE_ID_TYPE> slaveDevices_;
	std::vector<BS2_DEVICE_ID> wiegandDevices_;
};


class DeviceList
{
public:
	DeviceList();
	virtual ~DeviceList();


public:
	bool appendDevice(BS2_DEVICE_ID id, BS2_DEVICE_TYPE type, uint32_t ip, BS2_PORT port, int32_t timezone);
	bool removeDevice(BS2_DEVICE_ID id);
	bool findDevice(BS2_DEVICE_ID id) /*const*/;
	bool findSlave(BS2_DEVICE_ID slaveID) /*const*/;
	std::shared_ptr<DeviceInfo>& getDevice(BS2_DEVICE_ID id);
	void clearDevices();

	bool appendSlave(BS2_DEVICE_ID hostID, BS2_DEVICE_ID slaveID, BS2_DEVICE_TYPE slaveType);
	bool appendWiegand(BS2_DEVICE_ID hostID, BS2_DEVICE_ID wiegandID);

	const std::map<BS2_DEVICE_ID, std::shared_ptr<DeviceInfo>>& getAllDevices() const { return devList_; }
	uint32_t getSize() { return static_cast<uint32_t>(devList_.size()); };
	bool updateStatus(BS2_DEVICE_ID id, bool connected);
	int32_t getTimezone(BS2_DEVICE_ID id);


private:
	std::map<BS2_DEVICE_ID, std::shared_ptr<DeviceInfo>> devList_;
	std::mutex deviceListLock_;
};
