using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    // Simple Factory Design Pattern
    public enum ModelID
    {
        CT40,
        CT45,
        CT47,
        CW45,
        NULL
    }

    #region Obsolote
    //public class clsSensorKTestFactory
    //{
    //    public static clsSensorKBase CreateSensorKTest(string strModel)
    //    {
    //        clsSensorKBase objSensorKTest = null;
    //        switch (strModel)
    //        {
    //            case "CT40":
    //                objSensorKTest = new clsCT40SensorK();
    //                break;

    //            case "CT45":
    //                objSensorKTest = new clsCT45SensorK();
    //                break;

    //            case "CT47":
    //                objSensorKTest = new clsCT47SensorK();
    //                break;

    //            case "CW45":
    //                objSensorKTest = new clsCW45SensorK();
    //                break;
            
    //            default:
    //                objSensorKTest = null;
    //                break;     
    //        }

    //        return objSensorKTest;
    //    }
    //}
    #endregion

    public class clsSensorKTestFactory
    {
        public static clsSensorKBase CreateSensorKTest(ModelID Type)
        {
            clsSensorKBase objSensorKTest = null;
            switch (Type)
            {
                case ModelID.CT40:
                    objSensorKTest = new clsCT40SensorK();
                    break;

                case ModelID.CT45:
                    objSensorKTest = new clsCT45SensorK();
                    break;

                case ModelID.CT47:
                    objSensorKTest = new clsCT47SensorK();
                    break;

                case ModelID.CW45:
                    objSensorKTest = new clsCW45SensorK();
                    break;

                default:
                    objSensorKTest = null;
                    break;
            }

            return objSensorKTest;
        }
    }

}
