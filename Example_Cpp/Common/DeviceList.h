#pragma once

#include <string>
#include <map>
#include <vector>
#include <memory>
#include <mutex>
#include "BS_API.h"


enum {
	SLAVETYPE_OSDP = 1,
	SLAVETYPE_WIEGAND
};

typedef struct
{
	BS2_DEVICE_ID		id;
	uint8_t				slaveType;	// 1:suprema-osdp, 2:wiegand
} BS2_DEVICE_GSLAVE_TYPE;

typedef struct  
{
	BS2_DEVICE_ID		id;
	BS2_DEVICE_TYPE		type;
	std::vector<BS2_DEVICE_GSLAVE_TYPE> gSlaveDevices_;	// Master - slave - grand slave
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
	bool findSlave(BS2_DEVICE_ID slaveID, BS2_DEVICE_TYPE& slaveType, BS2_DEVICE_ID& masterID);
	bool findSlave(BS2_DEVICE_ID slaveID, std::vector<BS2_DEVICE_GSLAVE_TYPE>** slaveDevices, bool needLock=true);
	std::shared_ptr<DeviceInfo>& getDevice(BS2_DEVICE_ID id);
	std::shared_ptr<DeviceInfo>& getMasterDevice(BS2_DEVICE_ID id);
	void clearDevices();

	bool appendSlave(BS2_DEVICE_ID hostID, BS2_DEVICE_ID slaveID, BS2_DEVICE_TYPE slaveType);
	bool appendSlave(BS2_DEVICE_ID hostID, BS2_DEVICE_ID slaveID, BS2_DEVICE_TYPE slaveType, bool isSlave=false);
	bool appendWiegand(BS2_DEVICE_ID hostID, BS2_DEVICE_ID wiegandID);
	bool appendWiegand(BS2_DEVICE_ID hostID, BS2_DEVICE_ID wiegandID, bool isSlave=false);

	bool removeSlave(BS2_DEVICE_ID hostID, BS2_DEVICE_ID slaveID, bool isSlave=false);

	const std::map<BS2_DEVICE_ID, std::shared_ptr<DeviceInfo>>& getAllDevices() const { return devList_; }
	uint32_t getSize() { return static_cast<uint32_t>(devList_.size()); };
	bool updateStatus(BS2_DEVICE_ID id, bool connected);
	int32_t getTimezone(BS2_DEVICE_ID id);


private:
	std::map<BS2_DEVICE_ID, std::shared_ptr<DeviceInfo>> devList_;
	std::mutex deviceListLock_;
};
