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

        public struct DeviceInfo
        {
            public string SN;
            public string SKU;
            public string Model;
            public string EID;
            public string WorkOrder;
        }

        #endregion


        #region Abstract TestItem

        public abstract bool TestInit();
        public abstract bool TestPowerOn();
        public abstract bool TestCheckDeviceReady();
        public abstract bool TestReadMFGData();
        public abstract bool TestCheckPreStation();
        // Calibration
        public abstract bool TestGSensorCalibation();       // 重力加速度传感器
        public abstract bool TestGYROSensorCalibration();   // 陀螺仪
        public abstract bool TestPSensorCalibration();      // 距离传感器
        public abstract bool TestPSensorFunction();         // 距离传感器功能测试
        public abstract bool TestAudioCalibration();        // 音频芯片
        public abstract bool TestBarometerSensorOffset();   // 气压传感器 

        public abstract bool TestReboot();
        public abstract bool TestEnd();

        #endregion

        #region Virtual Function

     

        #endregion
















    }
}
