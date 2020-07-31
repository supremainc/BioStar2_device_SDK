using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private V2_6_3_ET_Control control = new V2_6_3_ET_Control();

        protected override void runImpl(UInt32 deviceID)
        {
            control.execute(sdkContext, deviceID, true, noConnectionMode);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for V2.6.3";
            program.runWithIPv6();
        }
    }

}
