using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private ServerMatchingControl smc = new ServerMatchingControl();

        protected override void runImpl(UInt32 deviceID)
        {
            smc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for server matching";
            program.run();
        }
    }
}
