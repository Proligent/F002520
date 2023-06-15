using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    class clsCW45SensorK : clsSensorKBase
    {




     

        #region TestItem

        public override bool TestInit(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestPowerOn(ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {




            }
            catch (Exception ex)
            {
                strErrorMessage = "TestPowerOn Exception:" + ex.Message;
                return false;
            }


            return true;
        }

        public override bool TestCheckDeviceReady(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestReadMFGData(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestCheckRFResult(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestCheckPreStation(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestAutoChangeOver(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestScreenOff(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestCheckSensorList(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestMoveDamBoardUp(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestGSensorCalibation(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestGYROSensorCalibration(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestPSensorCalibration(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestPSensorFunction(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestAudioCalibration(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestBarometerSensorOffset(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestReboot(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool TestEnd(ref string strErrorMessage)
        {
            throw new NotImplementedException();
        }

        //public override bool SendDataToMDCS()
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool SendMES()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Public

        public override bool EjectUSBCable()
        {
            throw new NotImplementedException();
        }

        public override bool InsertUSBCable()
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
