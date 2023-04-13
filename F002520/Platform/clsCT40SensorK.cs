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

        public override void Start()
        {

        }

        public override bool RunTest()
        {
            string strErrorMessage = "";

            try
            {
                #region Init

                if (m_bRunInitialized == false)
                {
                    if (InitRun() == false)
                    {
                        return false;
                    }
                    return true;
                }

                #endregion

         
            }
            catch (Exception ex)
            {
                strErrorMessage = "RunTest Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        public override bool InitRun()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region TestItem

        private override bool TestPowerOn()
        { 
        
        
        }

        #endregion



    }
}
