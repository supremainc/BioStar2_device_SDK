using System;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private LogControl lc = new LogControl();

        protected override void runImpl(UInt32 deviceID)
        {
            lc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for log control";
            program.run();
        }
    }
}
