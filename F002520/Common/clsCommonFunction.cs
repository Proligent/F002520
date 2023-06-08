using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFactory.ExternalDLL;
using System.IO;

namespace F002520
{
    class clsCommonFunction
    {

        #region Variable

        private static clsExecProcess clsProcess = new clsExecProcess();


        #endregion


        public static bool CheckADBConnected()
        {
            bool bRes = false;
            bool IsConnected = false;
            string strResult = "";

            try
            {
                bRes = clsProcess.ExcuteCmd("adb kill-server", 100);
                bRes = clsProcess.ExcuteCmd("adb start-server", 100);
                bRes = clsProcess.ExcuteCmd("adb root", 100);
                bRes = clsProcess.ExcuteCmd("adb devices", 500, ref strResult);
                if (strResult.Contains("List of devices attached") && strResult.Contains("\tdevice"))
                {
                    IsConnected = true;
                }
                else
                {
                    IsConnected = false;
                }
            }
            catch
            {
                return false;
            }

            return IsConnected;
        }

        public static bool DeleteMDCSSqueueXmlFile()
        {
            try
            {
                string strXMLPath = System.IO.Path.GetTempPath() + "\\" + "mdcsqueue.xml";

                if (File.Exists(strXMLPath) == true)
                {
                    File.Delete(strXMLPath);
                }
            }
            catch (Exception ex)
            {
                string strr = ex.Message;
                return false;
            }

            return true;
        }

     


    }
}
