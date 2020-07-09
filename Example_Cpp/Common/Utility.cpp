#include "stdafx.h"
#include <time.h>
#include <fstream>
#include <sstream>
#include <iostream>
#include <iomanip>
#include "../Common/Utility.h"

#define STR2INT(x)			isdigit(x.c_str()[0]) ? atoi(x.c_str()) : 0;


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

string Utility::replaceSlashToPeriod(const string& source)
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

vector<string> Utility::tokenizeString(const string& data, const char delimiter)
{
	vector<string> result;
	string tokenized;
	stringstream ss(data);
	while (getline(ss, tokenized, delimiter))
		result.push_back(tokenized);

	return result;
}

string Utility::getEventString(const BS2Event& event, int32_t timezone)
{
	char buffer[1024] = { 0, };
	switch (event.code & BS2_EVENT_MASK)
	{
	case BS2_EVENT_USER_ENROLL_SUCCESS:
	case BS2_EVENT_USER_ENROLL_FAIL:
	case BS2_EVENT_USER_UPDATE_SUCCESS:
	case BS2_EVENT_USER_UPDATE_FAIL:
	case BS2_EVENT_USER_DELETE_SUCCESS:
	case BS2_EVENT_USER_DELETE_FAIL:
		sprintf(buffer, "mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d) userID(%s) where(%s)",
			event.mainCode, event.subCode, event.dateTime + timezone, event.deviceID, event.userID, event.param ? "Device" : "Server");
		break;

	default:
		sprintf(buffer, "mainCode(0x%02x) subCode(0x%02x) dateTime(%d) deviceID(%d) userID(%s)",
			event.mainCode, event.subCode, event.dateTime + timezone, event.deviceID, event.userID);
		break;
	}

	return buffer;
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
