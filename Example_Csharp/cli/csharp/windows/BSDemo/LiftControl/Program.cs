using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Suprema;


namespace BSDemo
{
    class Program : UnitTest
    {
        private LiftControl dc = new LiftControl();

        protected override void runImpl(UInt32 deviceID)
        {
            dc.execute(sdkContext, deviceID, true);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Title = "Test for Lift control";
            program.run();
        }
    }
}
