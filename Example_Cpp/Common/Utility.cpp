#include "stdafx.h"
#include <time.h>
#include <fstream>
#include <sstream>
#include <iostream>
#include <iomanip>
#include <stdexcept>
#include <sys/timeb.h>
#include "../Common/Utility.h"
#include "../Include/BS_Errno.h"
#include "../Common/CommControl.h"
#include "../Common/ConfigControl.h"
#include "../Common/DeviceControl.h"
#include "../Common/LogControl.h"

#define STR2INT(x)				isdigit(x.c_str()[0]) ? atoi(x.c_str()) : 0;
#define MAX_RECV_LOG_AMOUNT		32768
#define MAX_SIZE_IMAGE_LOG		50 * 1024

extern void TRACE(const char* fmt, ...);


using namespace std;

string Utility::getLine(string msg)
{
	cout << "==> " << msg << " ";
	string line;
	getline(cin >> ws, line);

	return line;
}

string Utility::getIPAddress(uint32_t ip)
{
	char buf[16] = { 0, };
	sprintf(buf, "%u.%u.%u.%u",
		(ip & 0x000000FF),
		(ip & 0x0000FF00) >> 8,
		(ip & 0x00FF0000) >> 16,
		(ip & 0xFF000000) >> 24);

	return buf;
}

string Utility::getLocalTime(bool milliSec)
{
	struct timeb tb;
	struct tm tstruct;
	char buf[128] = { 0, };

	ftime(&tb);
	if (0 == localtime_s(&tstruct, &tb.time))
	{
		if (milliSec)
		{
			sprintf(buf, "%04d-%02d-%02d %02d:%02d:%02d.%03d",
				tstruct.tm_year + 1900,
				tstruct.tm_mon + 1,
				tstruct.tm_mday,
				tstruct.tm_hour,
				tstruct.tm_min,
				tstruct.tm_sec,
				tb.millitm);
		}
		else
		{
			sprintf(buf, "%04d-%02d-%02d %02d:%02d:%02d",
				tstruct.tm_year + 1900,
				tstruct.tm_mon + 1,
				tstruct.tm_mday,
				tstruct.tm_hour,
				tstruct.tm_min,
				tstruct.tm_sec);
		}
	}

	return buf;
}

string Utility::convertTimeUTC2String(uint32_t utcTime)
{
	time_t tmp(utcTime);
	struct tm* stTime;

	stTime = gmtime(&tmp);

	char buff[128] = { 0, };
	sprintf(buff, "%04d-%02d-%02d %02d:%02d:%02d",
		stTime->tm_year + 1900,
		stTime->tm_mon + 1,
		stTime->tm_mday,
		stTime->tm_hour,
		stTime->tm_min,
		stTime->tm_sec);

	return buff;
}

uint32_t Utility::convertTimeString2UTC(string time)
{
	int32_t year = STR2INT(time.substr(0, 4));
	int32_t month = STR2INT(time.substr(5, 2));
	int32_t day = STR2INT(time.substr(8, 2));
	int32_t hour = STR2INT(time.substr(11, 2));
	int32_t min = STR2INT(time.substr(14, 2));
	int32_t sec = STR2INT(time.substr(17));

	putenv("TZ=UTC");

	struct tm loc;
	loc.tm_year = year - 1900;
	loc.tm_mon = month - 1;
	loc.tm_mday = day;
	loc.tm_hour = hour;
	loc.tm_min = min;
	loc.tm_sec = sec;
	loc.tm_isdst = 0;

	time_t lTime = mktime(&loc);
	// 	putenv("TZ=Asia/Seoul");

	return static_cast<uint32_t>(lTime);
}

uint32_t Utility::getResourceSize(string file)
{
	if (0 == file.size())
		return 0;

	ifstream is;
	is.open(file, ios::binary);
	is.seekg(0, ios::end);
	uint32_t len = static_cast<uint32_t>(is.tellg());
	is.seekg(0, ios::beg);

	return len;
}

bool Utility::getResourceFromFile(string file, shared_ptr<uint8_t> buffer, uint32_t size)
{
	if (0 == file.size() || !buffer)
		return false;

	if (0 == size)
		size = getResourceSize(file);

	ifstream is;
	is.open(file, ios::binary | ios::in);

	is.read(reinterpret_cast<char*>(buffer.get()), size);
	is.close();

	return true;
}

bool Utility::setResourceToFile(string file, uint8_t* buffer, uint32_t size)
{
	if (0 == file.size() || !buffer || 0 == size)
		return false;

	ofstream os;
	os.open(file, ios::binary | ios::trunc);
	os.write(reinterpret_cast<char*>(buffer), size);
	os.close();

	return true;
}

bool Utility::setResourceToFile(string file, shared_ptr<uint8_t> buffer, uint32_t size)
{
	if (0 == file.size() || !buffer || 0 == size)
		return false;

	ofstream os;
	os.open(file, ios::binary | ios::trunc);
	os.write(reinterpret_cast<char*>(buffer.get()), size);
	os.close();

	return true;
}

string Utility::replaceValue(std::string data, std::string token, std::string replase)
{
	std::stringstream result;
	size_t posFrom, posTo;
	posFrom = posTo = 0;

	while (std::string::npos != (posTo = data.find(token, posFrom)))
	{
		result << data.substr(posFrom, posTo - posFrom);
		result << replase;
		posFrom = posTo + token.size();
	}

	result << data.substr(posFrom);
	return result.str();
}

vector<string> Utility::tokenizeString(const string& data, const char delimiter)
{
	vector<string> result;
	string tokenized;
	stringstream ss(data);
	while (getline(ss, tokenized, delimiter))
		result.push_back(tokenized);

	return result;
}

string Utility::getHexaString(const uint8_t* data, uint32_t size)
{
	stringstream ss;
	ss << hex;

	for (uint32_t index = 0; index < size; index++)
		ss << setw(2) << setfill('0') << (int)data[index];

	return ss.str();
}

void Utility::writeBMPSign(unsigned char* buf, unsigned short type, unsigned long size, unsigned long off_bits)
{
	buf[0] = 'B';
	buf[1] = 'M';
	buf[2] = (unsigned char)(type & 0xff);
	buf[3] = (unsigned char)(type >> 8);
	buf[4] = 0; //(unsigned char)(type >> 16);
	buf[5] = 0; //(unsigned char)(type >> 24);
	buf[6] = 0;
	buf[7] = 0;
	buf[8] = 0;
	buf[9] = 0;
	buf[10] = (unsigned char)(off_bits & 0xff);
	buf[11] = (unsigned char)(off_bits >> 8);
	buf[12] = (unsigned char)(off_bits >> 16);
	buf[13] = (unsigned char)(off_bits >> 24);
}

int Utility::saveBMP(FILE* fp, unsigned char* data, int width, int height)
{
	int newWidth = (width + 3) / 4 * 4;
	const int infoSize = sizeof(BITMAPINFOHEADER_) + sizeof(RGBQUAD_) * 256;
	int imageSize = infoSize + newWidth * height;

	unsigned char sign[BITMAP_SIGNATURE_SIZE];
	writeBMPSign(sign, BITMAP_SIGNATURE, BITMAP_SIGNATURE_SIZE + imageSize,
		(unsigned long)(BITMAP_SIGNATURE_SIZE + infoSize));

	BITMAPINFO_* image = (BITMAPINFO_*)calloc(imageSize, 1);
	if (image == NULL)
		return 0;

	image->bmiHeader.biSize = sizeof(BITMAPINFOHEADER_);
	image->bmiHeader.biWidth = newWidth;
	image->bmiHeader.biHeight = height;
	image->bmiHeader.biPlanes = 1;
	image->bmiHeader.biBitCount = 8;
	image->bmiHeader.biCompression = 0L;
	image->bmiHeader.biSizeImage = width * height;
	image->bmiHeader.biXPelsPerMeter = 0;
	image->bmiHeader.biYPelsPerMeter = 0;
	image->bmiHeader.biClrUsed = 0;
	image->bmiHeader.biClrImportant = 0;
	for (int k = 0; k < 256; k++)
	{
		image->bmiColors[k].rgbRed = image->bmiColors[k].rgbGreen
									= image->bmiColors[k].rgbBlue
									= (unsigned char)(k);
	}

	unsigned char* buf = ((unsigned char*)image) + infoSize;
	unsigned char* pos = data + (height - 1) * width;
	for (int i = height - 1; i >= 0; i--)
	{
		memcpy(buf, pos, width);
		buf += newWidth;
		pos -= width;
	}

	fwrite(sign, BITMAP_SIGNATURE_SIZE, 1, fp);
	fwrite(image, imageSize, 1, fp);

	free(image);

	return 1;
}

BS2_BOOL Utility::isYes(string msg)
{
	msg += " [y/n]";
	char selected = Utility::getInput<char>(msg);
	return (selected == 'y' || selected == 'Y');
}

BS2_BOOL Utility::isNo(string msg)
{
	msg += " [y/n]";
	char selected = Utility::getInput<char>(msg);
	return (selected == 'n' || selected == 'N');
}

string Utility::getStringOfDeviceType(BS2_DEVICE_TYPE type)
{
	switch (type)
	{
	case BS2_DEVICE_TYPE_BIOENTRY_PLUS:
		return "BEPL";
	case BS2_DEVICE_TYPE_BIOENTRY_W:
		return "BEW";
	case BS2_DEVICE_TYPE_BIOLITE_NET:
		return "BLN";
	case BS2_DEVICE_TYPE_XPASS:
		return "Xps";
	case BS2_DEVICE_TYPE_XPASS_S2:
		return "XpsS2";
	case BS2_DEVICE_TYPE_SECURE_IO_2:
		return "SIO2";
	case BS2_DEVICE_TYPE_DOOR_MODULE_20:
		return "DM-20";
	case BS2_DEVICE_TYPE_BIOSTATION_2:
		return "BS2";
	case BS2_DEVICE_TYPE_BIOSTATION_A2:
		return "BSA2";
	case BS2_DEVICE_TYPE_FACESTATION_2:
		return "FS2";
	//case BS2_DEVICE_TYPE_IO_DEVICE:
	//	return "IO";
	case BS2_DEVICE_TYPE_BIOSTATION_L2:
		return "BSL2";
	case BS2_DEVICE_TYPE_BIOENTRY_W2:
		return "BEW2";
	case BS2_DEVICE_TYPE_CORESTATION_40:
		return "CS-40";
	case BS2_DEVICE_TYPE_OUTPUT_MODULE:
		return "OM";
	//case BS2_DEVICE_TYPE_INPUT_MODULE:
	//	return "IM";
	case BS2_DEVICE_TYPE_BIOENTRY_P2:
		return "BEP2";
	case BS2_DEVICE_TYPE_BIOLITE_N2:
		return "BLN2";
	case BS2_DEVICE_TYPE_XPASS2:
		return "XP2";
	case BS2_DEVICE_TYPE_XPASS_S3:
		return "XpsS3";
	case BS2_DEVICE_TYPE_BIOENTRY_R2:
		return "BER2";
	case BS2_DEVICE_TYPE_XPASS_D2:
		return "XPD2";
	//case BS2_DEVICE_TYPE_DOOR_MODULE_21:
	//	return "DM21";
	case BS2_DEVICE_TYPE_XPASS_D2_KEYPAD:
		return "XPD2-K";
	case BS2_DEVICE_TYPE_FACELITE:
		return "FL";
	case BS2_DEVICE_TYPE_XPASS2_KEYPAD:
		return "XP2-K";
	case BS2_DEVICE_TYPE_XPASS_D2_REV:
		return "XPD2 Rev";
	case BS2_DEVICE_TYPE_XPASS_D2_KEYPAD_REV:
		return "XPD2-K Rev";
	case BS2_DEVICE_TYPE_FACESTATION_F2_FP:
		return "FSF2-Fp";
	case BS2_DEVICE_TYPE_FACESTATION_F2:
		return "FSF2";
	case BS2_DEVICE_TYPE_XSTATION_2_QR:
		return "XS2-QR";
	case BS2_DEVICE_TYPE_XSTATION_2:
		return "XS2";
	case BS2_DEVICE_TYPE_IM_120:
		return "IM-120";
	case BS2_DEVICE_TYPE_XSTATION_2_FP:
		return "XS2-Fp";
	case BS2_DEVICE_TYPE_BIOSTATION_3:
		return "BS3";
	case BS2_DEVICE_TYPE_UNKNOWN:
	default:
		break;
	}
	return "Unknown";
}

string Utility::getStringOfConnectMode(BS2_CONNECTION_MODE mode)
{
	switch (mode)
	{
	case BS2_CONNECTION_MODE_SERVER_TO_DEVICE:
		return "S2D";
	case BS2_CONNECTION_MODE_DEVICE_TO_SERVER:
		return "D2S";
	default:
		break;
	}
	return "N/S";
}

string Utility::convertHexByte2String(const string& input)
{
	static const char hex_digits[] = "0123456789ABCDEF";

	std::string output;
	output.reserve(input.length() * 2);
	for (unsigned char c : input)
	{
		output.push_back(hex_digits[c >> 4]);
		output.push_back(hex_digits[c & 15]);
	}
	return output;
}

int Utility::hex_value(char hex_digit)
{
	switch (hex_digit) {
	case '0': case '1': case '2': case '3': case '4':
	case '5': case '6': case '7': case '8': case '9':
		return hex_digit - '0';

	case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
		return hex_digit - 'A' + 10;

	case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
		return hex_digit - 'a' + 10;
	}
	throw std::invalid_argument("invalid hex digit");
}

string Utility::convertString2HexByte(const string& input)
{
	const auto len = input.length();
	if (len & 1) throw std::invalid_argument("odd length");

	string output;
	output.reserve(len / 2);
	for (auto it = input.begin(); it != input.end(); )
	{
		int hi = hex_value(*it++);
		int lo = hex_value(*it++);
		output.push_back(hi << 4 | lo);
	}
	return output;
}

BS2_DEVICE_ID Utility::getSelectedDeviceID(const DeviceInfo& info)
{
	cout << "(M) - " << info.id_ << endl;
	auto data = info.slaveDevices_;
	for (auto item : data)
	{
		cout << "(S) - " << item.id << endl;
	}

	return Utility::getInput<BS2_DEVICE_ID>("Please enter the device ID:");
}

bool Utility::getSelectedDeviceID(const DeviceInfo& info, BS2_DEVICE_ID& id, BS2_DEVICE_TYPE& type)
{
	BS2_DEVICE_ID selected = Utility::getSelectedDeviceID(info);
	if (selected == info.id_)
	{
		id = info.id_;
		type = info.type_;
		return true;
	}

	auto data = info.slaveDevices_;
	for (auto item : data)
	{
		if (selected == item.id)
		{
			id = item.id;
			type = item.type;
			return true;
		}
	}

	return false;
}

void Utility::displayConnectedDevices(const DeviceList& devices, bool includeSlave, bool includeWiegand)
{
	const auto& mapDevices = devices.getAllDevices();
	for (auto it = mapDevices.begin(); it != mapDevices.end(); it++)
	{
		printf("[%c] Device:%10u, IP:%-15s, Port:%u, Type:%-10s (M)\n",
			it->second->connected_ ? '+' : '-',
			it->second->id_,
			Utility::getIPAddress(it->second->ip_).c_str(),
			it->second->port_,
			Utility::getStringOfDeviceType(it->second->type_).c_str());

		if (includeSlave)
			for (auto slave : it->second->slaveDevices_)
				printf("[%c] Master:%10u, Device:%10u (S)\n",
					it->second->connected_ ? '+' : '-',
					it->second->id_,
					slave.id);

		if (includeWiegand)
			for (auto id : it->second->wiegandDevices_)
				printf("[%c] Master:%10u, Device:%10u (W)\n",
					it->second->connected_ ? '+' : '-',
					it->second->id_,
					id);
	}
}

uint32_t Utility::showMenu(vector<MENU_ITEM>& info)
{
	for (const auto& item : info)
	{
		if (item.index == MENU_SEPARATOR)
			cout << "------------------------------------" << endl;
		else
			cout << item.index << ") " << item.disc << endl;
	}

	return Utility::getSelectedIndex();
}

uint32_t Utility::getSelectedIndex()
{
	return Utility::getInput<uint32_t>("Select number:");
}

BS2_DEVICE_ID Utility::selectDeviceID(const DeviceList& deviceList, bool includeSlave, bool includeWiegand)
{
	Utility::displayConnectedDevices(deviceList, includeSlave, includeWiegand);
	return Utility::getInput<BS2_DEVICE_ID>("Please enter the device ID:");
}

bool Utility::selectDeviceIDAndType(const DeviceList& deviceList, bool includeSlave, BS2_DEVICE_ID& selectedID, BS2_DEVICE_TYPE& selectedType)
{
	Utility::displayConnectedDevices(deviceList, includeSlave, false);
	selectedID = Utility::getInput<BS2_DEVICE_ID>("Please enter the device ID:");
	for (auto item : deviceList.getAllDevices())
	{
		if (selectedID == item.second->id_)
		{
			selectedType = item.second->type_;
			return true;
		}

		if (includeSlave)
		{
			for (auto slave : item.second->slaveDevices_)
			{
				if (selectedID == slave.id)
				{
					selectedType = slave.type;
					return true;
				}
			}
		}
	}
	return false;
}

void Utility::selectDeviceIDs(const DeviceList& deviceList, BS2_DEVICE_ID& masterID, std::vector<BS2_DEVICE_ID>& selectedDevices, bool includeSlave, bool includeWiegand)
{
	cout << "==> Select upgrade order." << endl;
	selectedDevices.clear();
	Utility::displayConnectedDevices(deviceList, includeSlave, includeWiegand);
	while (true)
	{
		BS2_DEVICE_ID id = Utility::getInput<BS2_DEVICE_ID>("Please enter the slave device ID by order (0: Stop) :");
		if (0 == id)
			break;
		selectedDevices.push_back(id);
	}
	masterID = Utility::getInput<BS2_DEVICE_ID>("Please enter the master device ID :");
}

int Utility::searchAndConnect(void* context, DeviceList& deviceList)
{
	vector<BS2SimpleDeviceInfo> searchedList;
	CommControl cm(context);
	int sdkResult = cm.searchDevices(searchedList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	Utility::displayDeviceList(searchedList);

	uint32_t selected(0);
	if (MENU_BREAK != (selected = Utility::getSelectedIndex()) && selected <= searchedList.size())
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

int Utility::connectViaIP(void* context, DeviceInfo& device)
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

	BS2SimpleDeviceInfo info = { 0, };
	if (BS_SDK_SUCCESS != (sdkResult = dc.getDeviceInfo(id, info)))
	{
		cm.disconnectDevice(id);
		return sdkResult;
	}

	device.id_ = id;
	device.type_ = info.type;
	device.ip_ = info.ipv4Address;
	device.port_ = port;
	device.timezone_ = timezone;
	device.connected_ = true;

	return sdkResult;
}

int Utility::connectViaIP(void* context, DeviceList& deviceList)
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

	BS2SimpleDeviceInfo info = { 0, };
	if (BS_SDK_SUCCESS != (sdkResult = dc.getDeviceInfo(id, info)))
	{
		cm.disconnectDevice(id);
		return sdkResult;
	}

	BS2_DEVICE_TYPE type = info.type;
	deviceList.appendDevice(id, type, info.ipv4Address, info.port, timezone);

	return sdkResult;
}

int Utility::connectSlave(void* context, DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	if (Utility::isYes("Do you want to find slave devices?"))
	{
		BS2_DEVICE_ID slaveID = 0;
		ConfigControl cc(context);

		switch (device.type_)
		{
		case BS2_DEVICE_TYPE_CORESTATION_40:
			sdkResult = Utility::searchCSTSlave(context, device.slaveDevices_, device.id_);
			break;

		default:
			sdkResult = cc.updateRS485OperationMode(device.id_, BS2_RS485_MODE_MASTER);
			if (BS_SDK_SUCCESS == sdkResult)
				sdkResult = Utility::searchSlave(context, device.slaveDevices_, device.id_);
			break;
		}

		//if (BS_SDK_SUCCESS == sdkResult && 0 < slaveID)
		//	device.slaveDevices_.push_back(slaveID);
	}

	return sdkResult;
}

int Utility::connectWiegand(void* context, DeviceInfo& device)
{
	int sdkResult = BS_SDK_SUCCESS;
	if (Utility::isYes("Do you want to find wiegand devices?"))
	{
		BS2_DEVICE_ID wiegandID = 0;
		int sdkResult = Utility::searchWiegand(context, device.id_, wiegandID);
		if (BS_SDK_SUCCESS == sdkResult)
			device.wiegandDevices_.push_back(wiegandID);
	}

	return sdkResult;
}

int Utility::connectWiegand(void* context, DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	if (Utility::isYes("Do you want to find wiegand devices?"))
	{
		Utility::displayConnectedDevices(deviceList, true);
		BS2_DEVICE_ID masterID = Utility::getInput<BS2_DEVICE_ID>("Please enter the device ID:");

		if (!deviceList.findDevice(masterID) && !deviceList.findSlave(masterID))
		{
			cout << "Abort wiegand device discovery" << endl;
			return BS_SDK_ERROR_CANNOT_FIND_DEVICE;
		}

		BS2_DEVICE_ID wiegandID = 0;
		sdkResult = Utility::searchWiegand(context, masterID, wiegandID);
		if (BS_SDK_SUCCESS == sdkResult)
			deviceList.appendWiegand(masterID, wiegandID);
	}

	return sdkResult;
}


int Utility::searchAndAddSlave(void* context, DeviceList& deviceList)
{
	int sdkResult = BS_SDK_SUCCESS;
	if (Utility::isYes("Do you want to find slave devices?"))
	{
		Utility::displayConnectedDevices(deviceList);
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
			sdkResult = Utility::searchCSTSlave(context, deviceList, masterID);
			break;

		default:
			sdkResult = cc.updateRS485OperationMode(masterID, BS2_RS485_MODE_MASTER);
			if (BS_SDK_SUCCESS == sdkResult)
				sdkResult = Utility::searchSlave(context, deviceList, masterID);
			break;
		}

		//if (BS_SDK_SUCCESS == sdkResult && 0 < slaveID)
		//	deviceList.appendSlave(masterID, slaveID);
	}

	return sdkResult;
}

int Utility::searchSlave(void* context, vector<BS2_DEVICE_ID_TYPE>& deviceList, BS2_DEVICE_ID& masterID)
{
	CommControl cm(context);
	vector<BS2Rs485SlaveDevice> slaveList;
	int sdkResult = cm.searchSlaveDevice(masterID, slaveList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	Utility::displaySlaveList(slaveList);

	if (0 == slaveList.size())
		return BS_SDK_SUCCESS;

	bool connectAll = false;
	if (Utility::isYes("Do you want to add all discovered slave devices?"))
		connectAll = true;

	for (auto& slaveDevice : slaveList)
	{
		if (connectAll)
		{
			slaveDevice.enableOSDP = true;
		}
		else
		{
			ostringstream oss;
			oss << "Do you want to add slave device " << slaveDevice.deviceID << " ?";
			slaveDevice.enableOSDP = Utility::isYes(oss.str());
		}
	}

	sdkResult = cm.addSlaveDevice(masterID, slaveList);
	if (sdkResult)
	{
		for (const auto& slaveDevice : slaveList)
		{
			if (slaveDevice.enableOSDP)
			{
				BS2_DEVICE_ID_TYPE item;
				item.id = slaveDevice.deviceID;
				item.type = slaveDevice.deviceType;
				cout << "Added slave:" << item.id << ", type:" << (uint32_t)item.type << endl;
				deviceList.push_back(item);
			}
		}
	}

	return sdkResult;
}

int Utility::searchSlave(void* context, DeviceList& deviceList, BS2_DEVICE_ID& masterID)
{
	CommControl cm(context);
	vector<BS2Rs485SlaveDevice> slaveList;
	int sdkResult = cm.searchSlaveDevice(masterID, slaveList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	Utility::displaySlaveList(slaveList);

	if (0 == slaveList.size())
		return BS_SDK_SUCCESS;

	bool connectAll = false;
	if (Utility::isYes("Do you want to add all discovered slave devices?"))
		connectAll = true;

	for (auto& slaveDevice : slaveList)
	{
		if (connectAll)
		{
			slaveDevice.enableOSDP = true;
		}
		else
		{
			ostringstream oss;
			oss << "Do you want to add slave device " << slaveDevice.deviceID << " ?";
			slaveDevice.enableOSDP = Utility::isYes(oss.str());
		}
	}

	sdkResult = cm.addSlaveDevice(masterID, slaveList);

	for (const auto& slaveDevice : slaveList)
	{
		if (slaveDevice.enableOSDP)
		{
			BS2_DEVICE_ID id = slaveDevice.deviceID;
			BS2_DEVICE_TYPE type = slaveDevice.deviceType;
			cout << "Added slave:" << id << ", type:" << (uint32_t)type << endl;
			deviceList.appendSlave(masterID, id, type);
		}
	}

	return sdkResult;
}

int Utility::searchCSTSlave(void* context, vector<BS2_DEVICE_ID_TYPE>& deviceList, BS2_DEVICE_ID& masterID)
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

	Utility::displayCSTSlaveList(slaveList);

	bool connectAll = false;
	if (Utility::isYes("Do you want to add all discovered slave devices?"))
		connectAll = true;

	for (auto& slaveDevice : slaveList)
	{
		if (connectAll)
		{
			slaveDevice.enableOSDP = true;
		}
		else
		{
			ostringstream oss;
			oss << "Do you want to add slave device " << slaveDevice.deviceID << "?";
			slaveDevice.enableOSDP = Utility::isYes(oss.str());
		}
	}

	sdkResult = cm.addCSTSlaveDevice(masterID, chSelected, slaveList);
	if (BS_SDK_SUCCESS == sdkResult)
	{
		for (const auto& slaveDevice : slaveList)
		{
			if (slaveDevice.enableOSDP)
			{
				BS2_DEVICE_ID_TYPE item;
				item.id = slaveDevice.deviceID;
				item.type = slaveDevice.deviceType;
				cout << "Added slave:" << item.id << ", type:" << (uint32_t)item.type << endl;
				deviceList.push_back(item);
			}
		}
	}

	return sdkResult;
}

int Utility::searchCSTSlave(void* context, DeviceList& deviceList, BS2_DEVICE_ID& masterID)
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

	Utility::displayCSTSlaveList(slaveList);

	bool connectAll = false;
	if (Utility::isYes("Do you want to add all discovered slave devices?"))
		connectAll = true;

	for (auto& slaveDevice : slaveList)
	{
		if (connectAll)
		{
			slaveDevice.enableOSDP = true;
		}
		else
		{
			ostringstream oss;
			oss << "Do you want to add slave device " << slaveDevice.deviceID << "?";
			slaveDevice.enableOSDP = Utility::isYes(oss.str());
		}
	}

	sdkResult = cm.addCSTSlaveDevice(masterID, chSelected, slaveList);

	for (const auto& slaveDevice : slaveList)
	{
		if (slaveDevice.enableOSDP)
		{
			BS2_DEVICE_ID id = slaveDevice.deviceID;
			BS2_DEVICE_TYPE type = slaveDevice.deviceType;
			cout << "Added slave:" << id << ", type:" << (uint32_t)type << endl;
			deviceList.appendSlave(masterID, id, type);
		}
	}

	return sdkResult;
}

int Utility::searchWiegand(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& wiegandID)
{
	CommControl cm(context);
	vector<BS2_DEVICE_ID> wiegandList;
	int sdkResult = cm.searchWiegandDevice(masterID, wiegandList);
	if (BS_SDK_SUCCESS != sdkResult)
		return sdkResult;

	Utility::displayWiegandList(wiegandList);

	uint32_t selected(0);
	if (MENU_BREAK != (selected = Utility::getSelectedIndex()) && selected <= wiegandList.size())
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

int Utility::getSlaveConnectionStatus(void* context, BS2_DEVICE_ID id, BS2_DEVICE_TYPE type)
{
	ConfigControl cc(context);
	int sdkResult = BS_SDK_SUCCESS;

	BS2Rs485ConfigEX configEx = { 0, };
	BS2Rs485Config config = { 0, };
	switch (type)
	{
	case BS2_DEVICE_TYPE_CORESTATION_40:
		sdkResult = cc.getRS485ConfigEx(id, configEx);
		if (BS_SDK_SUCCESS == sdkResult)
			ConfigControl::printRS485Status(configEx);
		break;

	default:
		sdkResult = cc.getRS485Config(id, config);
		if (BS_SDK_SUCCESS == sdkResult)
			ConfigControl::printRS485Status(config);
		break;
	}

	return sdkResult;
}

void Utility::displayDeviceList(const vector<BS2SimpleDeviceInfo>& devices)
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
			Utility::getStringOfConnectMode(info.connectionMode).c_str(),
			Utility::getStringOfDeviceType(info.type).c_str(),
			info.dualIDSupported);
	}
}

void Utility::displaySlaveList(const vector<BS2Rs485SlaveDevice>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		const BS2Rs485SlaveDevice& info = device;
		printf("%2u - Device:%10u, Type:%-10s, OSDP:%d, Connected:%d\n",
			++index,
			info.deviceID,
			Utility::getStringOfDeviceType(info.deviceType).c_str(),
			info.enableOSDP,
			info.connected);
	}
}

void Utility::displayCSTSlaveList(const vector<BS2Rs485SlaveDeviceEX>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		const BS2Rs485SlaveDeviceEX& info = device;
		printf("%2u - Device:%10u, Type:%-10s, OSDP:%d, Connected:%d, Channel:%u\n",
			++index,
			info.deviceID,
			Utility::getStringOfDeviceType(info.deviceType).c_str(),
			info.enableOSDP,
			info.connected,
			info.channelInfo);
	}
}

void Utility::displayWiegandList(const vector<BS2_DEVICE_ID>& devices)
{
	int index = 0;
	printf("%2u - Skip\n", index);
	for (const auto& device : devices)
	{
		printf("%2u - Device:%u\n", ++index, device);
	}
}

int Utility::getAllLogsFromDevice(void* context, BS2_DEVICE_ID id, int32_t timezone)
{
	int logIndex = 0;
	int sdkResult = BS_SDK_SUCCESS;

	// 1. Get the last log index from the database.
	// logIndex = ????

	// 2. Retrieve all bulk logs when disconnected
	if (BS_SDK_SUCCESS == (sdkResult = Utility::getLogsFromDevice(context, id, logIndex, timezone)))
	{
		// 3. Retrieve logs that may have occurred during bulk log reception
		sdkResult = Utility::getLogsFromDevice(context, id, logIndex, timezone);
	}

	return sdkResult;
}

int Utility::getLogsFromDevice(void* context, BS2_DEVICE_ID id, int& latestIndex, int timezone)
{
	int sdkResult = BS_SDK_SUCCESS;
	BS2Event* logObj = NULL;
	uint32_t numOfLog = 0;

	do
	{
		numOfLog = 0;
		sdkResult = BS2_GetLog(context, id, latestIndex, MAX_RECV_LOG_AMOUNT, &logObj, &numOfLog);
		if (BS_SDK_SUCCESS == sdkResult)
		{
			for (uint32_t index = 0; index < numOfLog; ++index)
			{
				BS2Event& event = logObj[index];
				latestIndex = event.id;
				cout << LogControl::getEventString(id, event, timezone) << endl;

				if (event.image & 0x01)
				{
					uint32_t imageSize(0);
					uint8_t* imageBuf = new uint8_t[MAX_SIZE_IMAGE_LOG];
					memset(imageBuf, 0x0, sizeof(uint8_t) * MAX_SIZE_IMAGE_LOG);
					if (BS_SDK_SUCCESS == Utility::getImageLog(context, id, event.id, imageBuf, imageSize))
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

int Utility::getImageLog(void* context, BS2_DEVICE_ID id, BS2_EVENT_ID eventID, uint8_t* imageBuf, uint32_t& imageSize)
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
