using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private WiegandControl wc = new WiegandControl();

        protected override void runImpl(UInt32 deviceID)
        {
            wc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for wiegand control";
            program.run();
        }
    }
}
