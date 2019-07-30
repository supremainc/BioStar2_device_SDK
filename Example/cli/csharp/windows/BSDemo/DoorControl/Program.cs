using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private DoorControl dc = new DoorControl();

        protected override void runImpl(UInt32 deviceID)
        {
            dc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for door control";
            program.run();
        }
    }
}
