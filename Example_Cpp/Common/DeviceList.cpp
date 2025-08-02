#include "stdafx.h"
#include <algorithm>
#include "DeviceList.h"
#include "BS_Errno.h"
#include "Utility.h"

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
	ptr->connected_ = true;

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

bool DeviceList::findSlave(BS2_DEVICE_ID slaveID)
{
	lock_guard<mutex> lock(deviceListLock_);

	for (auto item : devList_)
	{
		for (auto slave : item.second->slaveDevices_)
		{
			if (slaveID == slave.id) return true;			
		}
	}

	return false;
}

bool DeviceList::findSlave(BS2_DEVICE_ID slaveID, BS2_DEVICE_TYPE& slaveType, BS2_DEVICE_ID& masterID)
{
	lock_guard<mutex> lock(deviceListLock_);

	for (auto item : devList_)
	{
		for (auto slave : item.second->slaveDevices_)
		{
			if (slaveID == slave.id) {
				masterID = item.first;
				slaveType = slave.type;
				return true;
			}
		}
	}

	return false;
}


bool DeviceList::findSlave(BS2_DEVICE_ID slaveID, std::vector<BS2_DEVICE_GSLAVE_TYPE>** gSlaveDevices, bool needlock)
{
	if (needlock)
		lock_guard<mutex> lock(deviceListLock_);

	for (auto& item : devList_)
	{
		for (auto& slave : item.second->slaveDevices_)
		{
			if (slaveID == slave.id) {
				*gSlaveDevices = &slave.gSlaveDevices_;
				printf("DeviceList::findSlave(), slave[%d] found\n", slaveID);
				return true;
			}
		}
	}

	return false;
}

shared_ptr<DeviceInfo>& DeviceList::getDevice(BS2_DEVICE_ID id)
{
	lock_guard<mutex> lock(deviceListLock_);

	return devList_[id];
}

shared_ptr<DeviceInfo>& DeviceList::getMasterDevice(BS2_DEVICE_ID id)
{
	lock_guard<mutex> lock(deviceListLock_);
	
	// find master device of the id.
	for (auto& item : devList_)
	{
		for (auto& slave : item.second->slaveDevices_)
		{
			if (id == slave.id) {
				return devList_[item.first];
			}
			for (auto& gSlave : slave.gSlaveDevices_)
			{
				if (id == gSlave.id) {
					return devList_[item.first];
				}
			}
		}
	}
	
	// failed to find master device.
	static shared_ptr<DeviceInfo> emptyDevice;
	return emptyDevice;
}

void DeviceList::clearDevices()
{
	lock_guard<mutex> lock(deviceListLock_);

	devList_.clear();
}

bool DeviceList::removeSlave(BS2_DEVICE_ID hostID, BS2_DEVICE_ID slaveID, bool isSlave)
{
	lock_guard<mutex> lock(deviceListLock_);

	if (isSlave)
	{
		std::vector<BS2_DEVICE_GSLAVE_TYPE>* gSlaveList;
		if (!findSlave(hostID, &gSlaveList, false)) {
			printf("no slave: %d\n", hostID);
			return false;
		}

		auto it = find_if(gSlaveList->begin(), gSlaveList->end(), [slaveID](const BS2_DEVICE_GSLAVE_TYPE& item) { return item.id == slaveID; });
		if (it == gSlaveList->end())
			return false;

		gSlaveList->erase(it);
		printf("OK, grand gslave %d is removed, gSlaveList.size:%zu\n", slaveID, gSlaveList->size());
	}
	else
	{
		auto it = devList_.find(hostID);
		if (it == devList_.end())
			return false;

		auto& vec = devList_[hostID]->slaveDevices_;
		auto itw = find_if(vec.begin(), vec.end(), [slaveID](const BS2_DEVICE_ID_TYPE& item) { return item.id == slaveID; });
		if (itw == vec.end())
			return false;

		vec.erase(itw);
		printf("OK,  slave %d is removed, \n", slaveID);
	}

	return true;
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
		if (slaveID == slave.id)
			return true;
	}

	BS2_DEVICE_ID_TYPE item;
	item.id = slaveID;
	item.type = slaveType;
	vec.push_back(item);

	return true;
}

bool DeviceList::appendSlave(BS2_DEVICE_ID hostID, BS2_DEVICE_ID slaveID, BS2_DEVICE_TYPE slaveType, bool isSlave)
{
	lock_guard<mutex> lock(deviceListLock_);

	if (isSlave)
	{
		std::vector<BS2_DEVICE_GSLAVE_TYPE>* gSlaveList;
		if (!findSlave(hostID, &gSlaveList, false)) {
			printf("no slave: %d\n", hostID);
			return false;
		}

		for (auto& gslave : *gSlaveList) {
			if (gslave.id == slaveID) {
				printf("already exist %d\n", slaveID);
				return true;
			}
		}
		BS2_DEVICE_GSLAVE_TYPE gSlave;
		gSlave.id = slaveID;
		gSlave.slaveType = SLAVETYPE_OSDP;
		gSlaveList->push_back(gSlave);
		printf("OK, grand slave %d is inserted, gSlaveList.size:%zu\n", slaveID, gSlaveList->size());
	}
	else
	{
		auto it = devList_.find(hostID);
		if (it == devList_.end())
			return false;

		auto& vec = devList_[hostID]->slaveDevices_;
		for (auto slave : vec)
		{
			if (slaveID == slave.id)
				return true;
		}

		BS2_DEVICE_ID_TYPE item;
		item.id = slaveID;
		item.type = slaveType;
		vec.push_back(item);
	}


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

bool DeviceList::appendWiegand(BS2_DEVICE_ID hostID, BS2_DEVICE_ID wiegandID, bool isSlave)
{
	lock_guard<mutex> lock(deviceListLock_);
	
	printf("DeviceList::appendWiegand.....\n");
	if (isSlave) {
		std::vector<BS2_DEVICE_GSLAVE_TYPE>* gSlaveList;
		if (!findSlave(hostID, &gSlaveList, false)) {
			printf("no slave: %d\n", hostID);
			return false;
		}

		for (auto& gslave : *gSlaveList) {
			if (gslave.id == wiegandID) {
				printf("already exist %d\n", wiegandID);
				return true;
			}
		}
		BS2_DEVICE_GSLAVE_TYPE wdevice;
		wdevice.id = wiegandID;
		wdevice.slaveType = SLAVETYPE_WIEGAND;
		gSlaveList->push_back(wdevice);
		printf("OK, grand slave %d is inserted, gSlaveList.size:%zu\n", wiegandID, gSlaveList->size());		
	}
	else
	{
		auto it = devList_.find(hostID);
		if (it == devList_.end())
			return false;

		auto& vec = devList_[hostID]->wiegandDevices_;
		auto itw = find(vec.begin(), vec.end(), wiegandID);
		if (itw == vec.end())
			vec.push_back(wiegandID);
	}

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