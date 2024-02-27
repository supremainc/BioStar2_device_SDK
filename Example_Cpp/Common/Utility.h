#pragma once
#include <string>
#include <iostream>
#include <memory>
#include <vector>
#include <sstream>
#include <ostream>
#include <stdarg.h>
#include "DeviceList.h"
#include "../Include/BS_API.h"


#define VER_272_OR_HIGHER			1
#define VER_271_OR_HIGHER			1


#define BITMAP_SIGNATURE			0x4D42
#define BITMAP_SIGNATURE_SIZE		14
#define MAX_BUFFER_SIZE				1024

#define DELIMITER_COMMA				','
#define DELIMITER_COLON				':'

#define MENU_BREAK				0
#define MENU_SEPARATOR			-1

typedef struct
{
	int			index;
	std::string disc;
} MENU_ITEM;

typedef struct __BITMAPINFOHEADER
{
	unsigned long	biSize;
	long			biWidth;
	long			biHeight;
	unsigned short	biPlanes;
	unsigned short	biBitCount;
	unsigned long	biCompression;
	unsigned long	biSizeImage;
	long			biXPelsPerMeter;
	long			biYPelsPerMeter;
	unsigned long	biClrUsed;
	unsigned long	biClrImportant;
} BITMAPINFOHEADER_;

typedef struct __RGBQUAD
{
	unsigned char	rgbBlue;
	unsigned char	rgbGreen;
	unsigned char	rgbRed;
	unsigned char	rgbReserved;
} RGBQUAD_;

typedef struct __BITMAPINFO
{
	BITMAPINFOHEADER_   bmiHeader;
	RGBQUAD_            bmiColors[1];
} BITMAPINFO_;


template <typename T>
struct ArrayDeleter
{
	void operator() (T* ptr) { delete[] ptr; }
};


class Utility
{
public:
	template <typename T>
	static T getInput(std::string msgFormat, ...);
	template <typename T>
	static T getInputHexaChar(std::string msgFormat, ...);
	static std::string getLine(std::string msg);
	template <typename T>
	static bool getLineWiegandBits(std::string msg, T* data, uint32_t size);
	template <typename T>
	static bool getLineHexaString(std::string msg, T* data, uint32_t size);
	template <typename T>
	static std::vector<T> getLineNumbers(std::string msg, const char delimiter = ',');
	static void displayConnectedDevices(const DeviceList& devices, bool includeSlave = false, bool includeWiegand = false);

	static std::string getLocalTime(bool milliSec = true);
	static std::string getIPAddress(uint32_t ip);
	static std::string convertTimeUTC2String(uint32_t utcTime);
	static uint32_t convertTimeString2UTC(std::string time);
	static uint32_t getResourceSize(std::string file);
	static bool getResourceFromFile(std::string file, std::shared_ptr<uint8_t> buffer, uint32_t size = 0);
	static bool setResourceToFile(std::string file, uint8_t* buffer, uint32_t size);
	static bool setResourceToFile(std::string file, std::shared_ptr<uint8_t> buffer, uint32_t size);

	static std::string replaceValue(std::string data, std::string token, std::string replase);

	static std::vector<std::string> tokenizeString(const std::string& data, const char delimiter = ' ');
	template <typename T>
	static std::vector<T> tokenizeInteger(const std::string& data, const char delimiter = ' ');
	template <typename T>
	static std::vector<T> tokenizeHexaString(const std::string& data, const char delimiter = ' ');
	template <typename T>
	static T convAString2Int(const std::string& data);

	static std::string getHexaString(const uint8_t* data, uint32_t size);
	static int saveBMP(FILE* fp, unsigned char* data, int width, int height);

	static BS2_BOOL isYes(std::string msgFormat, ...);
	static BS2_BOOL isNo(std::string msgFormat, ...);

	static std::string getStringOfDeviceType(BS2_DEVICE_TYPE type);
	static std::string getStringOfConnectMode(BS2_CONNECTION_MODE mode);

	static std::string convertHexByte2String(const std::string& input);
	static int hex_value(char hex_digit);
	static std::string convertString2HexByte(const std::string& input);

	template <typename T>
	static std::string convertArrayToString(const T* data, int numOfData, char delimiter);

	// Solution common functions
	static uint32_t showMenu(std::vector<MENU_ITEM>& info);
	static uint32_t getSelectedIndex();
	static std::vector<uint32_t> getSelectedIndexes();
	static BS2_DEVICE_ID getSelectedDeviceID(const DeviceInfo& info);
	static bool getSelectedDeviceID(const DeviceInfo& info, BS2_DEVICE_ID& id, BS2_DEVICE_TYPE& type);
	static BS2_DEVICE_ID selectDeviceID(const DeviceList& deviceList, bool includeSlave = false, bool includeWiegand = false);
	static BS2_DEVICE_ID selectMasterOrSlaveID(const DeviceList& deviceList, bool& useMaster);
	static BS2_DEVICE_ID selectSlaveID();
	static bool selectDeviceIDAndType(const DeviceList& deviceList, bool includeSlave, BS2_DEVICE_ID& selectedID, BS2_DEVICE_TYPE& selectedType);
	static void selectDeviceIDs(const DeviceList& deviceList, BS2_DEVICE_ID& masterID, std::vector<BS2_DEVICE_ID>& selectedDevices, bool includeSlave, bool includeWiegand);
	static int searchAndConnect(void* context, DeviceList& deviceList);
	static int connectViaIP(void* context, DeviceInfo& device);
	static int connectViaIP(void* context, DeviceList& deviceList);
	static int connectSlave(void* context, DeviceInfo& device);
	static int connectWiegand(void* context, DeviceInfo& device);
	static int connectWiegand(void* context, DeviceList& deviceList);
	static int searchAndAddSlave(void* context, DeviceList& deviceList);
	static int searchSlave(void* context, std::vector<BS2_DEVICE_ID_TYPE>& deviceList, BS2_DEVICE_ID& masterID);
	static int searchSlave(void* context, DeviceList& deviceList, BS2_DEVICE_ID& masterID);
	static int searchCSTSlave(void* context, std::vector<BS2_DEVICE_ID_TYPE>& deviceList, BS2_DEVICE_ID& masterID);
	static int searchCSTSlave(void* context, DeviceList& deviceList, BS2_DEVICE_ID& masterID);
	static int searchWiegand(void* context, BS2_DEVICE_ID& masterID, BS2_DEVICE_ID& wiegandID);
	static int getSlaveConnectionStatus(void* context, BS2_DEVICE_ID id, BS2_DEVICE_TYPE type);
	static void displayDeviceList(const std::vector<BS2SimpleDeviceInfo>& devices);
	static void displaySlaveList(const std::vector<BS2Rs485SlaveDevice>& devices);
	static void displayCSTSlaveList(const std::vector<BS2Rs485SlaveDeviceEX>& devices);
	static void displayWiegandList(const std::vector<BS2_DEVICE_ID>& devices);
	static int getAllLogsFromDevice(void* context, BS2_DEVICE_ID id, int32_t timezone);
	static int getLogsFromDevice(void* context, BS2_DEVICE_ID id, int& latestIndex, int timezone);
	static int getImageLog(void* context, BS2_DEVICE_ID id, BS2_EVENT_ID eventID, uint8_t* imageBuf, uint32_t& imageSize);
	static std::vector<std::string> Utility::getHostIPAddress();

private:
	static void writeBMPSign(unsigned char* buf, unsigned short type, unsigned long size, unsigned long off_bits);
};

template <typename T>
inline T Utility::getInput(std::string msgFormat, ...)
{
	va_list ap;
	va_start(ap, msgFormat);
	char buf[MAX_BUFFER_SIZE] = { 0, };
	vsprintf(buf, msgFormat.c_str(), ap);
	va_end(ap);

	std::cout << buf << std::endl;
	std::cout << "==> ";
	T value;
	std::cin >> value;

	return value;
}

template <typename T>
inline T Utility::getInputHexaChar(std::string msgFormat, ...)
{
	va_list ap;
	va_start(ap, msgFormat);
	char buf[MAX_BUFFER_SIZE] = { 0, };
	vsprintf(buf, msgFormat.c_str(), ap);
	va_end(ap);

	std::cout << buf << std::endl;
	std::cout << "==> 0x";
	std::string value;
	std::cin >> value;
	std::stringstream conv;
	uint32_t intVal;
	conv << std::hex << value;
	conv >> intVal;

	return static_cast<T>(intVal);
}

template <typename T>
inline bool Utility::getLineWiegandBits(std::string msg, T* data, uint32_t size)
{
	auto result = Utility::tokenizeHexaString<T>(Utility::getLine(msg));
	uint32_t startIdx = size - result.size();
	for (uint32_t idx = startIdx, dataIdx = 0; idx < size; idx++, dataIdx++)
		data[idx] = result[dataIdx];

	return true;
}

template <typename T>
inline bool Utility::getLineHexaString(std::string msg, T* data, uint32_t size)
{
	auto result = Utility::tokenizeHexaString<T>(Utility::getLine(msg));
	uint32_t count = min(size, result.size());
	for (uint32_t idx = 0; idx < count; idx++)
		data[idx] = result[idx];

	return true;
}

template <typename T>
inline std::vector<T> Utility::getLineNumbers(std::string msg, const char delimiter)
{
	std::string inputStr = Utility::getLine(msg);
	return Utility::tokenizeInteger<T>(inputStr, delimiter);
}

template <typename T>
inline std::vector<T> Utility::tokenizeHexaString(const std::string& data, const char delimiter)
{
	std::vector<T> result;
	std::string tokenized;
	std::stringstream ss(data);
	uint32_t tempValue;
	while (std::getline(ss, tokenized, delimiter))
	{
		std::stringstream tss;
		tss << std::hex << tokenized;
		tss >> tempValue;
		result.push_back(static_cast<T>(tempValue));
	}

	return result;
}

template <typename T>
inline std::vector<T> Utility::tokenizeInteger(const std::string& data, const char delimiter)
{
	std::vector<T> result;
	std::string tokenized;
	std::stringstream ss(data);
	while (std::getline(ss, tokenized, delimiter))
	{
		T tok = convAString2Int<T>(tokenized);
		result.push_back(tok);
	}

	return result;
}

template <typename T>
inline T Utility::convAString2Int(const std::string& data)
{
	if (data.size() == 0)
		return (T)0;

	return static_cast<T>(atoi(data.c_str()));
}

template <typename T>
inline std::string Utility::convertArrayToString(const T* data, int numOfData, char delimiter)
{
	std::ostringstream str;
	if (0 < numOfData)
	{
		str << static_cast<int>(data[0]);
		for (int i = 1; i < numOfData; i++)
			str << delimiter << static_cast<int>(data[i]);
	}
	return str.str();
}