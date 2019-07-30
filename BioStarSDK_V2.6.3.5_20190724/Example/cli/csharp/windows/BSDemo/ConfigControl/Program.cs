using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;
using System.Threading;

namespace BSDemo
{
    class Program : UnitTest
    {
        private ConfigControl dc = new ConfigControl();

        protected override void runImpl(UInt32 deviceID)
        {
            dc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for config control";
            program.run();
        }
    }
}
