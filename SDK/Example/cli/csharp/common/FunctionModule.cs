using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Globalization;
using System.Diagnostics;
using System.Threading;

namespace Suprema
{
    public abstract class FunctionModule
    {
        protected BS2SimpleDeviceInfo deviceInfo;
        protected BS2SimpleDeviceInfoEx deviceInfoEx;
        protected abstract List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> getFunctionList(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice);

        public void execute(IntPtr sdkContext, UInt32 deviceID, bool isMasterDevice, bool noConnection = false)
        {
#if OLD_CODE
            if (noConnection == false)
            { 
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceInfo(sdkContext, deviceID, out deviceInfo);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Can't get device information(errorCode : {0}).", result);
                    return;
                }
            }
#else
            if (noConnection == false)
            { 
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetDeviceInfoEx(sdkContext, deviceID, out deviceInfo, out deviceInfoEx);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    Console.WriteLine("Can't get device information(errorCode : {0}).", result);
                    return;
                }
            }
#endif


            List<KeyValuePair<string, Action<IntPtr, UInt32, bool>>> functionList = getFunctionList(sdkContext, deviceID, isMasterDevice);

            if (functionList.Count > 0)
            {
                int selection;
                bool running = true;

                while (running)
                {
                    Console.WriteLine("+-----------------------------------------------------------+");
                    int idx = 1;
                    for (; idx <= functionList.Count; ++idx)
                    {
                        Console.WriteLine("|{0,3}. {1,-54}|", idx, functionList[idx - 1].Key);
                    }
                    Console.WriteLine("|{0,3}. Exit                                                  |", idx);
                    Console.WriteLine("+-----------------------------------------------------------+");

                    Console.WriteLine("What would you like to do?");
                    Console.Write(">>>> ");

                    if (Util.GetInput(out selection))
                    {
                        if (selection > 0 && selection <= functionList.Count)
                        {
                            functionList[selection - 1].Value(sdkContext, deviceID, isMasterDevice);
                        }
                        else if (selection == functionList.Count + 1)
                        {
                            running = false;
                        }
                        else
                        {
                            Console.WriteLine("Invalid parameter : {0}", selection);
                        }
                    }
                }
            }
        }
    }
}
