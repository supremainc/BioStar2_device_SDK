#pragma once

#if defined(_WIN32) || defined(_WIN64)
#define OS_WINDOWS
#else
#define OS_LINUX
#endif

#if defined(OS_WINDOWS)
#include "targetver.h"

#include <stdio.h>
#include <tchar.h>
#endif
