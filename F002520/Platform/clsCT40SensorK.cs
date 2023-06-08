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

     

        #endregion

        #region MDCS

    

        #endregion

        #region Variable

   
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

        public override bool TestCheckRFResult()
        {
            throw new NotImplementedException();
        }

        public override bool TestCheckPreStation()
        {
            throw new NotImplementedException();
        }

        public override bool TestAutoChangeOver()
        {
            throw new NotImplementedException();
        }

        public override bool TestScreenOff()
        {
            throw new NotImplementedException();
        }

        public override bool TestCheckSensorList()
        {
            throw new NotImplementedException();
        }

        public override bool TestMoveDamBoardUp()
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

        public override bool SendDataToMDCS()
        {
            throw new NotImplementedException();
        }

        public override bool SendMES()
        {
            throw new NotImplementedException();
        }

        #endregion



    }
}
