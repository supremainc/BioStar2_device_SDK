using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private CardControl cc = new CardControl();

        protected override void runImpl(UInt32 deviceID)
        {
            cc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for card control";
            program.run();
        }
    }
}
