using System;
using Suprema;

namespace BSDemo
{
    class Program : UnitTest
    {
        private USBControl ctrl = new USBControl();

        protected override void runImpl(UInt32 deviceID)
        {
            ctrl.execute(sdkContext, deviceID, true, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for log control";
            program.runWithoutConnection();
        }
    }
}
