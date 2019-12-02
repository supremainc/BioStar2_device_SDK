#include "stdafx.h"
#include "DeviceList.h"
#include "BS_Errno.h"


extern void TRACE(const char* fmt, ...);
using namespace std;

DeviceList::DeviceList()
{
}


DeviceList::~DeviceList()
{
}


bool DeviceList::appendDevice(BS2_DEVICE_ID id, BS2_DEVICE_TYPE type, uint32_t ip, BS2_PORT port, int32_t timezone)
{
	shared_ptr<DeviceInfo> ptr(new DeviceInfo(id, type, ip, port, timezone));
	auto result = devList_.insert(pair<BS2_DEVICE_ID, shared_ptr<DeviceInfo>>(id, ptr));
	if (result.second == false)
	{
		TRACE("The device(%u) already exist.", id);
		return false;
	}

	TRACE("The device(%u) was added.", id);
	return true;
}

bool DeviceList::removeDevice(BS2_DEVICE_ID id)
{
	auto it = devList_.find(id);
	if (it == devList_.end())
		return false;

	devList_.erase(it);
	return true;
}

bool DeviceList::findDevice(BS2_DEVICE_ID id) const
{
	return (devList_.find(id) != devList_.end());
}

shared_ptr<DeviceInfo>& DeviceList::getDevice(BS2_DEVICE_ID id)
{
	return devList_[id];
}

void DeviceList::clearDevices()
{
	devList_.clear();
}

bool DeviceList::appendSlave(BS2_DEVICE_ID hostID, BS2_DEVICE_ID slaveID)
{
	auto it = devList_.find(hostID);
	if (it == devList_.end())
		return false;

	devList_[hostID]->slaveDevices_.push_back(slaveID);
	return true;
}

bool DeviceList::appendWiegand(BS2_DEVICE_ID hostID, BS2_DEVICE_ID wiegandID)
{
	auto it = devList_.find(hostID);
	if (it == devList_.end())
		return false;

	devList_[hostID]->wiegandDevices_.push_back(wiegandID);
	return true;
}

bool DeviceList::updateStatus(BS2_DEVICE_ID id, bool connected)
{
	auto it = devList_.find(id);
	if (it == devList_.end())
		return false;

	it->second->connected_ = connected;
	return true;
}

int32_t DeviceList::getTimezone(BS2_DEVICE_ID id)
{
	auto it = devList_.find(id);
	return it == devList_.end() ? 0 : it->second->timezone_;
}