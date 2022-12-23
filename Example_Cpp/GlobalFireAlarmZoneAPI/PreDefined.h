#pragma once

#define RETRIVE_BULK_LOGS			0
#define REALTIME_LOG_TEMPERATURE	1


enum
{
	CONN_TYPE_SEARCH_N_CONN = 1,	// SDK to Device connection (UDP Search and connection)
	CONN_TYPE_DIRECT_IPADDR = 2,	// SDK to Device connection (Direct connect with IP address)
	CONN_TYPE_LISTEN_N_ACCEPT = 3,	// Device to SDK connection
};

enum
{
	MAX_ENROLL_USER_FACELITE = 3000,
	MAX_ENROLL_USER_FACESTATION2 = 3000,
	MAX_ENROLL_USER_XPASS20 = 40000,
};
