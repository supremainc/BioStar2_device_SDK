using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private DeviceZoneControl dzc = new DeviceZoneControl();

        protected override void runImpl(UInt32 deviceID)
        {
            dzc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for Device Zone control";
            program.run();
        }
    }
}
