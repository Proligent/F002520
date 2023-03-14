using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    class clsCT40_SensorK : clsSensorK
    {
        #region Enum


        #endregion

        #region Struct


        #endregion

        #region Variable

        private bool m_bRunInitialized = false;

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




        #endregion


    

    }
}
