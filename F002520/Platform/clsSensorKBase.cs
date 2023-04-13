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

        #region MDCS



        #endregion

        #region Variable



        #endregion

        #region Abstract

        public abstract bool TestPowerOn();
        public abstract bool TestCheckDeviceReady(ref string strErrorMessage);
        public abstract bool TestReadMFGData(ref string strErrorMessage);
        public abstract bool TestCheckPreStation(ref string strErrorMessage);
        public abstract bool TestGSensorCalibation(ref string strErrorMessage);       // 重力加速度传感器
        public abstract bool TestGYROSensorCalibration(ref string strErrorMessage);   // 陀螺仪
        public abstract bool TestPSensorCalibration(ref string strErrorMessage);      // 距离传感器
        public abstract bool TestPSensorFunction(ref string strErrorMessage);         // 距离传感器功能测试
        public abstract bool TestAudioCalibration(ref string strErrorMessage);        // 音频芯片
        public abstract bool TestBarometerSensorOffset(ref string strErrorMessage);   // 气压传感器  
        public abstract bool TestReboot();
        public abstract bool TestEnd();

        #endregion

        #region Virtual Function

        public virtual void Start()
        { 



        }

        public virtual bool RunTest()
        {

            return true;
        }

        public virtual bool InitRun()
        {

            return true;
        }

        public virtual bool InitHW()
        {

            return true;
        }

        #endregion
















    }
}
