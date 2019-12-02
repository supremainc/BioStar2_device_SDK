#include "stdafx.h"
#include <time.h>
#include <fstream>
#include "../Common/Utils.h"

#define STR2INT(x)			isdigit(x.c_str()[0]) ? atoi(x.c_str()) : 0;


using namespace std;

string Utils::getLine(string msg)
{
	cout << "==> " << msg << " ";
	string line;
	getline(cin >> ws, line);

	return line;
}

string Utils::getIPAddress(uint32_t ip)
{
	char buf[16] = { 0, };
	sprintf(buf, "%u.%u.%u.%u",
		(ip & 0x000000FF),
		(ip & 0x0000FF00) >> 8,
		(ip & 0x00FF0000) >> 16,
		(ip & 0xFF000000) >> 24);

	return buf;
}

string Utils::convertTimeUTC2String(uint32_t utcTime)
{
	time_t tmp(utcTime);
	struct tm stTime;

	gmtime_s(&stTime, &tmp);

	char buff[128] = { 0, };
	sprintf(buff, "%04d-%02d-%02d %02d:%02d:%02d",
		stTime.tm_year + 1900,
		stTime.tm_mon + 1,
		stTime.tm_mday,
		stTime.tm_hour,
		stTime.tm_min,
		stTime.tm_sec);

	return buff;
}

uint32_t Utils::convertTimeString2UTC(string time)
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

uint32_t Utils::getResourceSize(string file)
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

bool Utils::getResourceFromFile(string file, shared_ptr<uint8_t> buffer, uint32_t size)
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

bool Utils::setResourceToFile(string file, shared_ptr<uint8_t> buffer, uint32_t size)
{
	if (0 == file.size() || !buffer || 0 == size)
		return false;

	ofstream os;
	os.open(file, ios::binary | ios::trunc);
	os.write(reinterpret_cast<char*>(buffer.get()), size);
	os.close();

	return true;
}

string Utils::replaceSlashToPeriod(const string& source)
{
	const string findStr = "/";
	const string replaceStr = ".";
	string resultStr(source);

	string::size_type offset = 0;
	while (true)
	{
		offset = resultStr.find(findStr, offset);
		if (string::npos == offset)
			break;

		resultStr.replace(offset, findStr.length(), replaceStr);
	}

	return resultStr;
}

vector<string> Utils::tokenizeString(const string& data, const char delimiter)
{
	vector<string> result;
	string tokenized;
	stringstream ss(data);
	while (getline(ss, tokenized, delimiter))
		result.push_back(tokenized);

	return result;
}