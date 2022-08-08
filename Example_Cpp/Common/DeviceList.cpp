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
	lock_guard<mutex> lock(deviceListLock_);

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
	lock_guard<mutex> lock(deviceListLock_);

	auto it = devList_.find(id);
	if (it == devList_.end())
		return false;

	devList_.erase(it);
	return true;
}

bool DeviceList::findDevice(BS2_DEVICE_ID id) /*const*/
{
	lock_guard<mutex> lock(deviceListLock_);

	return (devList_.find(id) != devList_.end());
}

bool DeviceList::findSlave(BS2_DEVICE_ID slaveID) /*const*/
{
	lock_guard<mutex> lock(deviceListLock_);

	for (auto item : devList_)
	{
		for (auto id : item.second->slaveDevices_)
		{
			if (slaveID == id.first)
				return true;
		}
	}

	return false;
}

shared_ptr<DeviceInfo>& DeviceList::getDevice(BS2_DEVICE_ID id)
{
	lock_guard<mutex> lock(deviceListLock_);

	return devList_[id];
}

void DeviceList::clearDevices()
{
	lock_guard<mutex> lock(deviceListLock_);

	devList_.clear();
}

bool DeviceList::appendSlave(BS2_DEVICE_ID hostID, BS2_DEVICE_ID slaveID, BS2_DEVICE_TYPE slaveType)
{
	lock_guard<mutex> lock(deviceListLock_);

	auto it = devList_.find(hostID);
	if (it == devList_.end())
		return false;

	auto& vec = devList_[hostID]->slaveDevices_;
	for (auto slave : vec)
	{
		if (slaveID == slave.first)
			return true;
	}

	vec.push_back(make_pair(slaveID, slaveType));

	return true;
}

bool DeviceList::appendWiegand(BS2_DEVICE_ID hostID, BS2_DEVICE_ID wiegandID)
{
	lock_guard<mutex> lock(deviceListLock_);

	auto it = devList_.find(hostID);
	if (it == devList_.end())
		return false;
	
	auto& vec = devList_[hostID]->wiegandDevices_;
	auto itw = find(vec.begin(), vec.end(), wiegandID);
	if (itw == vec.end())
		vec.push_back(wiegandID);

	return true;
}

bool DeviceList::updateStatus(BS2_DEVICE_ID id, bool connected)
{
	lock_guard<mutex> lock(deviceListLock_);

	auto it = devList_.find(id);
	if (it == devList_.end())
		return false;

	it->second->connected_ = connected;
	return true;
}

int32_t DeviceList::getTimezone(BS2_DEVICE_ID id)
{
	lock_guard<mutex> lock(deviceListLock_);

	auto it = devList_.find(id);
	return it == devList_.end() ? 0 : it->second->timezone_;
}