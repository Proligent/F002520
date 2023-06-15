using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    public abstract class clsSensorKBase
    {
        #region Struct

     

        #endregion

        #region Abstract TestItem

        public abstract bool TestInit(ref string strErrorMessage);
        public abstract bool TestPowerOn(ref string strErrorMessage);
        public abstract bool TestCheckDeviceReady(ref string strErrorMessage);
        public abstract bool TestReadMFGData(ref string strErrorMessage);
        public abstract bool TestCheckRFResult(ref string strErrorMessage);
        public abstract bool TestCheckPreStation(ref string strErrorMessage);
        public abstract bool TestAutoChangeOver(ref string strErrorMessage);
        public abstract bool TestScreenOff(ref string strErrorMessage);
        public abstract bool TestMoveDamBoardUp(ref string strErrorMessage);
        public abstract bool TestCheckSensorList(ref string strErrorMessage);
        // Calibration
        public abstract bool TestGSensorCalibation(ref string strErrorMessage);       // 重力加速度传感器
        public abstract bool TestGYROSensorCalibration(ref string strErrorMessage);   // 陀螺仪
        public abstract bool TestPSensorCalibration(ref string strErrorMessage);      // 距离传感器
        public abstract bool TestPSensorFunction(ref string strErrorMessage);         // 距离传感器功能测试
        public abstract bool TestAudioCalibration(ref string strErrorMessage);        // 音频芯片
        public abstract bool TestBarometerSensorOffset(ref string strErrorMessage);   // 气压传感器 
        public abstract bool TestReboot(ref string strErrorMessage);
        public abstract bool TestEnd(ref string strErrorMessage);

        #endregion

        #region Abstract Method

        public abstract bool EjectUSBCable();
        public abstract bool InsertUSBCable();

        //public abstract bool SendDataToMDCS();
        //public abstract bool SendMES();

        #endregion
















    }
}
