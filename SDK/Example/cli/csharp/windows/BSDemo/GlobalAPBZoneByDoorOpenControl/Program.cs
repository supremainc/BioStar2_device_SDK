using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private GlobalAPBZoneByDoorOpenControl control = new GlobalAPBZoneByDoorOpenControl();

        protected override void runImpl(UInt32 deviceID)
        {
            control.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for Global APB Zone By Door Open";
            program.run();
        }
    }
}
