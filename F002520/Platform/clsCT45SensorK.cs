using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    class clsCT45SensorK : clsSensorKBase
    {


        #region MDCS

        private struct TestResult
        {
            public bool TestPassed;
            public int TestFailCode;
            public string TestFailMessage;
            public string TestStatus;
        }

        private struct TestRecord
        {
            public string SN;
            public string SKU;
            public string Model;
            public string ToolNumber;
            public string ToolRev;

            public string IMEI;
            public string MEID;
            public string IMEI2;
            public string MEID2;
            public string BDA;
            public string MACAddress;

            public string AccelerometerTest;
            public string GyroscopeSensorTest;
            public string PressureSensorTest;
            public string ProximitySensorTest;
            public string AllTestResult;

            public double TestTotalTime;
        }

        private struct TestSaveData
        {
            public TestResult TestResult;
            public TestRecord TestRecord;
        }

        #endregion



        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override bool RunTest()
        {
            throw new NotImplementedException();
        }

        public override bool InitRun()
        {
            throw new NotImplementedException();
        }



    }
}
