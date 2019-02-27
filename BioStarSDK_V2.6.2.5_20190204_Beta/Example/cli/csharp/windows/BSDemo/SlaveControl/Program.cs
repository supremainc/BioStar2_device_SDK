using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private SlaveControl sc = new SlaveControl();

        protected override void runImpl(UInt32 deviceID)
        {
            sc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for slave control";
            program.run();
        }
    }
}
