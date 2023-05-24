using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    class clsCT40SensorK : clsSensorKBase
    {
        #region Enum


        #endregion

        #region Struct

        private struct ModelOptionData
        {
            // ScanSheet
            public string ScanSheetStation;

            // MODBUS
            public ComPortSetting ModbusSetting;
            public string HomePoint;

            // MDCS
            public string MDCSEnable;
            public string MDCSMode;
            public string MDCSURL;
            public string MDCSDeviceName;
            public string MDCSPreStationResultCheck;
            public string MDCSPreStationDeviceName;
            public string MDCSPreStationVarName;
            public string MDCSPreStationVarValue;   
        }

        #endregion

        #region MDCS

        private struct TestResult
        {
            public bool TestPassed;
            public int  TestFailCode;
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
            public string AndroidOS;
            public string HWVersion;
            public string AccelerometerTest;
            public string GyroscopeSensorTest;
            public string PressureSensorTest;
            public string ProximitySensorTest;
       
            // Value



            public double TestTotalTime;
        }

        private struct TestSaveData
        {
            public TestResult TestResult;
            public TestRecord TestRecord;
        }

        #endregion

        #region Variable

        private bool m_bRunInitialized = false;
        private ModelOptionData m_stModelOptionData;
        private TestSaveData m_stTestMDCSData;

        #endregion

        #region Function

     

        #endregion

        #region TestItem

        public override bool TestInit()
        {


            throw new NotImplementedException();
        }

        public override bool TestPowerOn()
        {
            string strErrorMessage = "";

            try
            { 
            



            }
            catch(Exception ex)
            {
                strErrorMessage = "TestPowerOn Exception:" + ex.Message;
                return false;
            }


            return true;
        }

        public override bool TestCheckDeviceReady()
        {
            throw new NotImplementedException();
        }

        public override bool TestReadMFGData()
        {
            throw new NotImplementedException();
        }

        public override bool TestCheckPreStation()
        {
            throw new NotImplementedException();
        }

        public override bool TestGSensorCalibation()
        {
            throw new NotImplementedException();
        }

        public override bool TestGYROSensorCalibration()
        {
            throw new NotImplementedException();
        }

        public override bool TestPSensorCalibration()
        {
            throw new NotImplementedException();
        }

        public override bool TestPSensorFunction()
        {
            throw new NotImplementedException();
        }

        public override bool TestAudioCalibration()
        {
            throw new NotImplementedException();
        }

        public override bool TestBarometerSensorOffset()
        {
            throw new NotImplementedException();
        }

        public override bool TestReboot()
        {
            throw new NotImplementedException();
        }

        public override bool TestEnd()
        {
            throw new NotImplementedException();
        }

        #endregion



    }
}
