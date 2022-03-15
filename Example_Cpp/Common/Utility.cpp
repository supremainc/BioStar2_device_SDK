#include "stdafx.h"
#include <time.h>
#include <fstream>
#include <sstream>
#include <iostream>
#include <iomanip>
#include <stdexcept>
#include <sys/timeb.h>
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
		return "Xps2";
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