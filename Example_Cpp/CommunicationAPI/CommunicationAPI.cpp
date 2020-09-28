#include "stdafx.h"
#include <sstream>
#include "CommunicationAPI.h"
#include "../Common/Utility.h"
#include "../Common/CommControl.h"
#include "../Common/LogControl.h"
#include "../Common/UserControl.h"


extern void TRACE(const char* fmt, ...);
using namespace std;

static void* sdkContext = NULL;
static BS2_DEVICE_ID connectedID = 0;
static DeviceList deviceList;


void onLogReceived(BS2_DEVICE_ID id, const BS2Event* event)
{
	int32_t timezone = deviceList.getTimezone(id);
	stringstream buf;
	buf << "Device(" << std::to_string(id) << ") " << Utility::getEventString(*event, timezone);
	cout << buf.str() << endl;
}

// Thermal supported callback
void onLogReceivedEx(BS2_DEVICE_ID id, const BS2Event* event, BS2_TEMPERATURE temperature)
{
	int32_t timezone = deviceList.getTimezone(id);
	stringstream buf;
	buf << "Device(" << std::to_string(id) << ") " << Utility::getEventStringWithThermal(*event, timezone, temperature);
	cout << buf.str() << endl;
}


void onDeviceAccepted(BS2_DEVICE_ID id)
{
	TRACE("Device(%d) accepted", id);

	int sdkResult = BS2_ConnectDevice(sdkContext, id);
	if (sdkResult != BS_SDK_SUCCESS)
		TRACE("BS2_ConnectDevice call failed: %d", sdkResult);
}


void onDeviceConnected(BS2_DEVICE_ID id)
{
	int32_t timezone = 0;
	DeviceControl dc(sdkContext);

	if (!getDeviceLogs(id, timezone))
		return;

#if REALTIME_LOG_TEMPERATURE
	int sdkResult = BS2_StartMonitoringLogEx(sdkContext, id, onLogReceivedEx);
#else
	int sdkResult = BS2_StartMonitoringLog(sdkContext, id, onLogReceived);
#endif
	if (BS_SDK_SUCCESS != sdkResult)
		TRACE("BS2_StartMonitoringLogEx call failed: %d", sdkResult);

	if (!deviceList.findDevice(id))
	{
		// Device to server connection
		BS2SimpleDeviceInfo info = { 0, };
		if (BS_SDK_SUCCESS != (sdkResult = dc.getDeviceInfo(id, info)))
			BS2_DisconnectDevice(sdkContext, id);

		deviceList.appendDevice(id, info.type, info.ipv4Address, info.port, timezone);
	}

	deviceList.updateStatus(id, true);

	TRACE("Device(%d) connected", id);
}


void onDeviceDisconnected(BS2_DEVICE_ID id)
{
	deviceList.updateStatus(id, false);

	TRACE("Device(%d) disconnected", id);
}

bool getDeviceLogs(BS2_DEVICE_ID id, int& timezone)
{
	ConfigControl cc(sdkContext);

	if (deviceList.findDevice(id))
		timezone = deviceList.getTimezone(id);
	else
		cc.getTimezone(id, timezone);

	int sdkResult = sdkResult = getAllLogsFromDevice(sdkContext, id, timezone);
	if (BS_SDK_SUCCESS != sdkResult)
	{
		TRACE("An error occurred while receiving bulk logs from device: %d", sdkResult);
		BS2_DisconnectDevice(sdkContext, id);
		return false;
	}

	return true;
}

int main(int argc, char* argv[])
{
	// Set debugging SDK log (to current working directory)
	BS2Context::getInstance()->setDebugFileLog(DEBUG_LOG_ALL, DEBUG_MODULE_ALL, ".");

	// Create SDK context and initialize
	sdkContext = BS2Context::getInstance()->initSDK();
	if (!sdkContext)
	{
		BS2Context::getInstance()->releaseInstance();
		return -1;
	}

	BS2Context::getInstance()->setDeviceEventListener(onDeviceAccepted, onDeviceConnected, onDeviceDisconnected);

	connectTestDevice(sdkContext, deviceList);

	BS2Context::getInstance()->releaseInstance();
	return 0;
}

void connectTestDevice(void* context, DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	bool menuBreak = false;
	while (!menuBreak)
	{
		uint32_t selected = showMenu(menuInfoTop);
		switch (selected)
		{
		case MENU_TOP_BREAK:
			menuBreak = true;
			break;
		case MENU_TOP_SEARCH_N_CONN:
			sdkResult = searchAndConnect(context, deviceList);
			break;
		case MENU_TOP_DIRECT_IPADDR:
			sdkResult = connectViaIP(context, deviceList);
			break;
		case MENU_TOP_SEARCH_SLAVE:
			sdkResult = connectSlave(context, deviceList);
			break;
		case MENU_TOP_SEARCH_WIEGAND:
			sdkResult = connectWiegand(context, deviceList);
			break;
		case MENU_TOP_VIEW_DEVICE:
			displayConnectedDevices(deviceList, true, true);
			break;

		default:
			break;
		}
	}

	if (BS_SDK_SUCCESS != sdkResult)
		return;

	runAPIs(context, deviceList);
}

uint32_t showMenu(vector<MENU_ITEM>& info)
{
	for (const auto& item : info)
	{
		cout << item.index << ") " << item.disc << endl;
	}

	return getSelectedIndex();
}

int searchAndConnect(void* context, DeviceList& deviceList)
{
	vector<BS2SimpleDeviceInfo> searchedList;
	CommControl cm(context);
	int sdkResult = cm.searchDevices(searchedList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	displayDeviceList(searchedList);

	uint32_t selected(0);
	if (MENU_TOP_BREAK != (selected = getSelectedIndex()) && selected <= searchedList.size())
	{
		uint32_t ip = searchedList[selected - 1].ipv4Address;
		string ipAddr = Utility::getIPAddress(ip);
		BS2_PORT port = searchedList[selected - 1].port;
		BS2_DEVICE_ID id = searchedList[selected - 1].id;
		BS2_DEVICE_TYPE type = searchedList[selected - 1].type;

		TRACE("Now connect to device (ID:%u, IP:%s, Port:%u)", id, ipAddr.c_str(), port);

		sdkResult = cm.connectDevice(id);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		int timezone(0);
		ConfigControl cc(context);
		if (BS_SDK_SUCCESS != (sdkResult = cc.getTimezone(id, timezone)))
		{
			cm.disconnectDevice(id);
			return sdkResult;
		}

		deviceList.appendDevice(id, type, ip, port, timezone);
	}

	return sdkResult;
}

int connectViaIP(void* context, DeviceList& deviceList)
{
	DeviceControl dc(context);
	ConfigControl cc(context);
	CommControl cm(context);
	string ip = Utility::getInput<string>("Device IP:");
	BS2_PORT port = Utility::getInput<BS2_PORT>("Port:");
	BS2_DEVICE_ID id = 0;

	TRACE("Now connect to device (IP:%s, Port:%u)", ip.c_str(), port);

	int sdkResult = cm.connectDevice(id, ip, port);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	int timezone(0);
	if (BS_SDK_SUCCESS != (sdkResult = cc.getTimezone(id, timezone)))
	{
		cm.disconnectDevice(id);
		return sdkResult;
	}

	BS2SimpleDeviceInfo info = {0,};
	if (BS_SDK_SUCCESS != (sdkResult = dc.getDeviceInfo(id, info)))
	{
		cm.disconnectDevice(id);
		return sdkResult;
	}

	BS2_DEVICE_TYPE type = info.type;
	deviceList.appendDevice(id, type, info.ipv4Address, info.port, timezone);

	return sdkResult;
}

int connectSlave(void* context, DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	char selected = Utility::getInput<char>("Do you want to find slave devices? [y/n]");
	if ('y' == selected || 'Y' == selected)
	{
		displayConnectedDevices(deviceList);
		BS2_DEVICE_ID masterID = Utility::getInput<BS2_DEVICE_ID>("Please enter the device ID:");

		if (!deviceList.findDevice(masterID))
		{
			cout << "Abort slave device discovery" << endl;
			return BS_SDK_ERROR_CANNOT_FIND_DEVICE;
		}

		auto device = deviceList.getDevice(masterID);
		BS2_DEVICE_TYPE type = device->type_;
		BS2_DEVICE_ID slaveID = 0;
		ConfigControl cc(context);

		switch (type)
		{
		case BS2_DEVICE_TYPE_CORESTATION_40:
			sdkResult = searchCSTSlave(context, masterID, slaveID);
			break;

		default:
			sdkResult = cc.updateRS485OperationMode(masterID, BS2_RS485_MODE_MASTER);
			if (BS_SDK_SUCCESS == sdkResult)
				sdkResult = searchSlave(context, masterID, slaveID);
			break;
		}

		if (BS_SDK_SUCCESS == sdkResult && 0 < slaveID)
			deviceList.appendSlave(masterID, slaveID);
	}

	return sdkResult;
}

int connectWiegand(void* context, DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	char selected = Utility::getInput<char>("Do you want to find wiegand devices? [y/n]");
	if ('y' == selected || 'Y' == selected)
	{
		displayConnectedDevices(deviceList);
		BS2_DEVICE_ID masterID = Utility::getInput<BS2_DEVICE_ID>("Please enter the device ID:");

		if (!deviceList.findDevice(masterID))
		{
			cout << "Abort wiegand device discovery" << endl;
			return BS_SDK_ERROR_CANNOT_FIND_DEVICE;
		}

		BS2_DEVICE_ID wiegandID = 0;
		sdkResult = searchWiegand(context, masterID, wiegandID);
		if (BS_SDK_SUCCESS == sdkResult)
			deviceList.appendWiegand(masterID, wiegandID);
	}

	return sdkResult;
}

uint32_t getSelectedIndex()
{
	return Utility::getInput<uint32_t>("Select ID:");
}

int searchSlave(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& slaveID)
{
	CommControl cm(context);
	vector<BS2Rs485SlaveDevice> slaveList;
	int sdkResult = cm.searchSlaveDevice(masterID, slaveList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	displaySlaveList(slaveList);

	uint32_t selected(0);
	if (MENU_BREAK != (selected = getSelectedIndex()) && selected <= slaveList.size())
	{
		BS2_DEVICE_ID id = slaveList[selected - 1].deviceID;

		TRACE("Now connect to slave device (Host:%u, Slave:%u)", masterID, id);

		sdkResult = cm.addSlaveDevice(masterID, id);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		slaveID = id;
		cout << "Added slave " << slaveID << endl;
	}

	return sdkResult;
}

int searchCSTSlave(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& slaveID)
{
	stringstream msg;
	msg << "Please select a channel to search. [0, 1, 2, 3, 4(All)]";
	uint32_t chSelected = Utility::getInput<uint32_t>(msg.str());
	switch (chSelected)
	{
	case RS485_HOST_CH_0:
	case RS485_HOST_CH_1:
	case RS485_HOST_CH_2:
	case RS485_HOST_CH_3:
		break;
	case 4:
	default:
		chSelected = RS485_HOST_CH_ALL;
		break;
	}

	CommControl cm(context);
	vector<BS2Rs485SlaveDeviceEX> slaveList;
	int sdkResult = cm.searchCSTSlaveDevice(masterID, chSelected, slaveList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	displayCSTSlaveList(slaveList);

	uint32_t selected(0);
	if (MENU_BREAK != (selected = getSelectedIndex()) && selected <= slaveList.size())
	{
		BS2_DEVICE_ID id = slaveList[selected - 1].deviceID;
		uint8_t chPort = slaveList[selected - 1].channelInfo;

		TRACE("Now connect to slave device (Host:%u, Slave:%u, Channel:%u)", masterID, id, chPort);

		sdkResult = cm.addCSTSlaveDevice(masterID, chPort, id);
		if (BS_SDK_SUCCESS != sdkResult)
			return sdkResult;

		slaveID = id;
		cout << "Added slave " << slaveID << endl;
	}

	return sdkResult;
}

int searchWiegand(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& wiegandID)
{
	CommControl cm(context);
	vector<BS2_DEVICE_ID> wiegandList;
	int sdkResult = cm.searchWiegandDevice(masterID, wiegandList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	displayWiegandList(wiegandList);

	uint32_t selected(0);
	if (MENU_BREAK != (selected = getSelectedIndex()) && selected <= wiegandList.size())
	{
		BS2_DEVICE_ID id = wiegandList[selected - 1];

		TRACE("Now connect to wiegand device (Host:%u, Slave:%u)", masterID, id);

		sdkResult = cm.addWiegandDevice(masterID, id);
		if (BS_SDK_SUCCESS == sdkResult)
		{
			wiegandID = id;
			cout << "Added wiegand slave " << wiegandID << endl;
		}
	}

	return sdkResult;
}

BS2_DEVICE_ID selectDeviceID(const DeviceList& deviceList, bool includeSlave, bool includeWiegand)
{
	displayConnectedDevices(deviceList, includeSlave, includeWiegand);
	return Utility::getInput<BS2_DEVICE_ID>("Please enter the device ID:");
}

int runAPIs(void* context, const DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	int selectedTop(0);
	CommControl cm(context);
	ConfigControl cc(context);
	LogControl lc(context);
	UserControl uc(context);

	cout << endl << endl << "== CommunicationAPI Test ==" << endl;
	BS2_DEVICE_ID id = 0;
	BS2FactoryConfig facConfig = { 0, };

	while (BS_SDK_SUCCESS == sdkResult && MENU_COMM_BREAK != (selectedTop = showMenu(menuInfoCommAPI)))
	{
		switch (selectedTop)
		{
		case MENU_COMM_BREAK:
			return BS_SDK_SUCCESS;
		case MENU_COMM_SET_KEEP_ALIVE:
			sdkResult = cm.setKeepAliveTimeout();
			break;
		case MENU_COMM_GET_SERVER_PORT:
			sdkResult = cm.getServerPort();
			break;
		case MENU_COMM_SET_SERVER_PORT:
			sdkResult = cm.setServerPort();
			break;
		case MENU_COMM_GET_SERVER_SSLPORT:
			sdkResult = cm.getSSLServerPort();
			break;
		case MENU_COMM_SET_SERVER_SSLPORT:
			sdkResult = cm.setSSLServerPort();
			break;
		case MENU_COMM_IS_CONNECTED:
			id = selectDeviceID(deviceList);
			sdkResult = cm.isConnected(id);
			break;
		case MENU_COMM_IS_AUTO_CONNECT:
			sdkResult = cm.isAutoConnection();
			break;
		case MENU_COMM_SET_AUTO_CONNECT:
			sdkResult = cm.setAutoConnection();
			break;
		case MENU_COMM_GET_ENABLEIPV4:
			sdkResult = cm.getEnableIPV4();
			break;
		case MENU_COMM_SET_ENABLEIPV4:
			sdkResult = cm.setEnableIPV4();
			break;
		case MENU_COMM_GET_ENABLEIPV6:
			sdkResult = cm.getEnableIPV6();
			break;
		case MENU_COMM_SET_ENABLEIPV6:
			sdkResult = cm.setEnableIPV6();
			break;
		case MENU_COMM_GET_SERVERPORTV6:
			sdkResult = cm.getServerPortIPV6();
			break;
		case MENU_COMM_SET_SERVERPORTV6:
			sdkResult = cm.setServerPortIPV6();
			break;
		case MENU_COMM_GET_SERVERSSLPORTV6:
			sdkResult = cm.getSSLServerPortIPV6();
			break;
		case MENU_COMM_SET_SERVERSSLPORTV6:
			sdkResult = cm.setSSLServerPortIPV6();
			break;
		case MENU_COMM_GET_DEF_RES_TIMEOUT:
			sdkResult = cm.getDefaultResponseTimeout();
			break;
		case MENU_COMM_SET_DEF_RES_TIMEOUT:
			sdkResult = cm.setDefaultResponseTimeout();
			break;
		case MENU_CONF_GET_FACCONFIG:
			id = selectDeviceID(deviceList, true, false);
			sdkResult = cc.getFactoryConfig(id, facConfig);
			break;
		case MENU_ELOG_GET_EVENTSMALLBLOB:
			id = selectDeviceID(deviceList, true, false);
			sdkResult = lc.getLogSmallBlob(id);
			break;
		case MENU_ELOG_GET_EVENTSMALLBLOBEX:
			id = selectDeviceID(deviceList, true, false);
			sdkResult = lc.getLogSmallBlobEx(id);
		case MENU_USER_ENROLL_FACE:
			id = selectDeviceID(deviceList, true, false);
			sdkResult = uc.enrollUser(id);
			break;
		case MENU_CONF_UPD_DEVICE_2_SERVER:
			id = selectDeviceID(deviceList, false, false);
			sdkResult = updateConnectModeDevice2Server(context, id);
			break;
		case MENU_CONF_UPD_SERVER_2_DEVICE:
			id = selectDeviceID(deviceList, false, false);
			sdkResult = updateConnectModeServer2Device(context, id);
			break;
		default:
			break;
		}
	}

	return sdkResult;
}


void displayDeviceList(const vector<BS2SimpleDeviceInfo>& devices)
{
	int index = 0;
	printf("%2u - Exit\n", index);
	for (const auto& device : devices)
	{
		const BS2SimpleDeviceInfo& info = device;
		printf("%2u - Device:%10u, IP:%-15s, Port:%u, Connected:%-15s, Mode:%s, Type:%-10s, DualID:%u\n",
			++index,
			info.id,
			Utility::getIPAddress(info.ipv4Address).c_str(),
			info.port,
			(info.connectedIP == 0xFFFFFFFF) ? "" : Utility::getIPAddress(info.connectedIP).c_str(),
			CONNECT_MODE[info.connectionMode],
			DEVICE_NAME[info.type],
			info.dualIDSupported);
	}
}

void displaySlaveList(const vector<BS2Rs485SlaveDevice>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		const BS2Rs485SlaveDevice& info = device;
		printf("%2u - Device:%10u, Type:%-10s, OSDP:%d, Connected:%d\n",
			++index,
			info.deviceID,
			DEVICE_NAME[info.deviceType],
			info.enableOSDP,
			info.connected);
	}
}

void displayCSTSlaveList(const vector<BS2Rs485SlaveDeviceEX>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		const BS2Rs485SlaveDeviceEX& info = device;
		printf("%2u - Device:%10u, Type:%-10s, OSDP:%d, Connected:%d, Channel:%u\n",
			++index,
			info.deviceID,
			DEVICE_NAME[info.deviceType],
			info.enableOSDP,
			info.connected,
			info.channelInfo);
	}
}

void displayWiegandList(const vector<BS2_DEVICE_ID>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		printf("%2u - Device:%u\n", ++index, device);
	}
}

void displayConnectedDevices(const DeviceList& devices, bool includeSlave, bool includeWiegand)
{
	const auto& mapDevices = devices.getAllDevices();
	for (auto it = mapDevices.begin(); it != mapDevices.end(); it++)
	{
		printf("[%c] Device:%10u, IP:%-15s, Port:%u, Type:%-10s (M)\n",
			it->second->connected_ ? '+' : '-',
			it->second->id_,
			Utility::getIPAddress(it->second->ip_).c_str(),
			it->second->port_,
			DEVICE_NAME[it->second->type_]);

		if (includeSlave)
			for (auto id : it->second->slaveDevices_)
				printf("[%c] Master:%10u, Device:%10u (S)\n",
					it->second->connected_ ? '+' : '-',
					it->second->id_,
					id);

		if (includeWiegand)
			for (auto id : it->second->wiegandDevices_)
				printf("[%c] Master:%10u, Device:%10u (W)\n",
					it->second->connected_ ? '+' : '-',
					it->second->id_,
					id);
	}
}

int getAllLogsFromDevice(void* context, BS2_DEVICE_ID id, int32_t timezone)
{
	int logIndex = 0;
	int sdkResult = BS_SDK_SUCCESS;

	// 1. Get the last log index from the database.
	// logIndex = ????

	// 2. Retrieve all bulk logs when disconnected
	if (BS_SDK_SUCCESS == (sdkResult = getLogsFromDevice(context, id, logIndex, timezone)))
	{
		// 3. Retrieve logs that may have occurred during bulk log reception
		sdkResult = getLogsFromDevice(context, id, logIndex, timezone);
	}

	return sdkResult;
}

int getLogsFromDevice(void* context, BS2_DEVICE_ID id, int& latestIndex, int timezone)
{
	int sdkResult = BS_SDK_SUCCESS;
	BS2Event* logObj = NULL;
	uint32_t numOfLog = 0;

	do
	{
		sdkResult = BS2_GetLog(context, id, latestIndex, MAX_RECV_LOG_AMOUNT, &logObj, &numOfLog);
		if (BS_SDK_SUCCESS == sdkResult)
		{
			for (uint32_t index = 0; index < numOfLog; ++index)
			{
				BS2Event& event = logObj[index];
				latestIndex = event.id;
				stringstream buf;
				buf << "Device(" << std::to_string(id) << ") " << Utility::getEventString(event, timezone);
				cout << buf.str() << endl;

				if (event.image & 0x01)
				{
					uint32_t imageSize(0);
					uint8_t* imageBuf = new uint8_t[MAX_SIZE_IMAGE_LOG];
					memset(imageBuf, 0x0, sizeof(uint8_t) * MAX_SIZE_IMAGE_LOG);
					if (BS_SDK_SUCCESS == getImageLog(context, id, event.id, imageBuf, imageSize))
					{
						// Your job.
						cout << "Image log received from " << id << " dateTime:" << event.dateTime + timezone
							<< " Event:" << event.id << endl;
					}

					delete[] imageBuf;
				}
			}

			if (logObj)
			{
				BS2_ReleaseObject(logObj);
				logObj = NULL;
			}
		}
		else
		{
			TRACE("BS2_GetLog call failed: %d", sdkResult);
			return sdkResult;
		}
	} while (MAX_RECV_LOG_AMOUNT <= numOfLog);

	return sdkResult;
}

int getImageLog(void* context, BS2_DEVICE_ID id, BS2_EVENT_ID eventID, uint8_t* imageBuf, uint32_t& imageSize)
{
	if (!imageBuf)
		return BS_SDK_ERROR_NULL_POINTER;

	uint8_t* imageObj = NULL;
	uint32_t size(0);
	int sdkResult = BS2_GetImageLog(context, id, eventID, &imageObj, &size);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		memcpy(imageBuf, imageObj, size);
		imageSize = size;
		if (imageObj)
			BS2_ReleaseObject(imageObj);
	}

	return sdkResult;
}

int updateConnectModeDevice2Server(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);
	string msg;

	msg = "Insert server IP";
	string ip = Utility::getInput<string>(msg);

	msg = "Insert server port";
	BS2_PORT port = (BS2_PORT)Utility::getInput<uint32_t>(msg);

	return cc.updateConnectModeDevice2Server(id, ip, port);
}

int updateConnectModeServer2Device(void* context, BS2_DEVICE_ID id)
{
	ConfigControl cc(context);

	return cc.updateConnectModeServer2Device(id);
}

#if 0
DWORD WINAPI onWaiting(LPVOID lpParam)
{
	int* standby = (int*)lpParam;
	cout << "Is the connection completed from the device?" << endl;
	while (true)
	{
		Sleep(1000);
		char cont = Utility::getInput<char>("Press 'c' continue calling APIs");

		if (cont == 'q' || cont == 'Q')
			break;
	}

	*standby = 0;

	return 0;
}

void waitForConnection()
{
	int standby = 1;
	DWORD thID;
	HANDLE handle = CreateThread(NULL, 0, onWaiting, &standby, 0, &thID);
	if (handle)
	{
		while (standby) { Sleep(1); }
	}
}

#endif