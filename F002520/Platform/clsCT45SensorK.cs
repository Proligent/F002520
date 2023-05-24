using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F002520
{
    class clsCT45SensorK : clsSensorKBase
    {
       
        #region Variable

        private bool m_bIsWWAN = false;
        //clsMDCS m_objMDCS = new clsMDCS();
        private clsExecProcess clsProcess = new clsExecProcess();
        private UnitDeviceInfo m_stUnitDeviceInfo = new UnitDeviceInfo();

        #endregion

      
        #region TestItem

        public override bool TestInit()
        {
            string strErrorMessage = "";
            string strTestItem = "";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                InitUnitDeviceInfo();    

            }
            catch(Exception ex)
            {
                strErrorMessage = "TestInit Exception:" + ex.Message;
                return false;  
            }

            DisplayMessage("Completed !!!");
            return true;
        }

        public override bool TestPowerOn()
        {
            string strErrorMessage = "";
            bool bFlag = false;
            double dValue_AI0 = 0.0;
            double dValue_AI1 = 0.0;

            try
            {
                // Pluge USB Pogopin and Check Cylinder Position
                #region Pluge USB Pogopin

                NISetDigital(0, 0, 1);  // DO0_0 H
                NISetDigital(0, 1, 0);  // DO0_1 L
                clsUtil.Dly(0.5);

                #endregion

                #region Check Position

                for (int i = 0; i < 5; i++)
                {             
                    dValue_AI0 = NIGetAnalog(0);    // AI0 < 2.0 V
                    dValue_AI1 = NIGetAnalog(1);    // AI1 > 3.0 V
                    if (dValue_AI0 < 2.0 && dValue_AI1 > 3.0)
                    {
                        bFlag = true;
                        break;
                    }
                    else
                    {
                        bFlag = false;
                        clsUtil.Dly(1.0);
                        continue;
                    }
                }
                if (bFlag == false)
                {
                    strErrorMessage = "Fail to Pluge USB Pogopin !";
                    return false;
                }

                #endregion
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestPowerOn Exception:" + ex.Message;
                return false;
            }

            DisplayMessage("Completed !!!");
            return true;
        }

        public override bool TestCheckDeviceReady()
        {
            string strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            bool bFlag = false;
            int ReConnectTimes = 0;

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;
                ReConnectTimes = int.Parse(GetTestItemParameter(strTestItem, "ReConnectTimes"));
                if (ReConnectTimes > 3)
                {
                    ReConnectTimes = 3;
                }

                #region Check ADB Connected

                if (ReConnectTimes != 0)
                {
                    for (int i = 0; i < (ReConnectTimes + 1); i++)
                    {
                        if (CheckADBConnected(10) == false)
                        {
                            bFlag = false;          
                            Reconnect();                  
                            continue;
                        }
                        else
                        {
                            bFlag = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (CheckADBConnected(25) == false)
                    {
                        bFlag = false; 
                    }   
                    else
                    {
                        bFlag = true; 
                    }
                }
                if (bFlag == false)
                {
                    strErrorMessage = "Check ADB Connect Fail !!!";
                    return false;
                }

                #endregion

                #region Check Reboot Completed Status

                if (CheckRebootCompleted(ref strErrorMessage) == false)
                {
                    strErrorMessage = "Check Device Reboot Completed Fail.";
                    return false;
                }

                #endregion   

                #region Get SN and Init Log

                string strSN = "";
                string strCmd = "adb shell su 0 mfg-tool -g EX_SERIAL_NUMBER";
            
                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strSN);
                DisplayMessage("Device SN: " + strSN);
                if(strSN.Length != 10)
                {
                    strErrorMessage = "Fail to Get SN !";
                    return false;
                }

                string strDate = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
                string logFileName = string.Format("Debug_{0}_{1}.log", strSN, strDate);
                string folder = Application.StartupPath + @"\log\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\"; ;
                logFileName = folder + logFileName;
                Logger.ChangeLogFileName("RollingFile", logFileName);
                Logger.Info("SN:{0}", strSN);

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "TestCheckDeviceReady Exception: " + ex.Message;
                return false;
            }

            DisplayMessage("Completed !!!");
            return true;
        }

        public override bool TestReadMFGData()
        {
            string strErrorMessage = "";
            bool bFlag = false;

            try
            {
                #region Read MFG Data

                for (int i = 0; i < 3; i++)
                {
                    if (ReadMFGData(ref strErrorMessage) == false)
                    {
                        bFlag = false;
                        clsUtil.Dly(2.0);
                        continue;
                    }
                    else
                    {
                        bFlag = true;
                        break;
                    }
                }
                if (bFlag == false)
                {
                    strErrorMessage = "Fail to Read MFG Data: " + strErrorMessage;
                    return false;
                }

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "TestReadMFGData Exception: " + ex.Message;
                return false;
            
            }

            DisplayMessage("Completed !!!");
            return true;
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

        #region Get XML Param

        private string GetTestItemParameter(string testItem, string name)
        {
            string strValue = "";
            strValue = Program.g_mainForm.GetTestItemParameter(testItem, name);

            return strValue;
        }

        private string GetTestConfig(string strItem, string strName)
        {
            string strValue = "";
            strValue = Program.g_mainForm.GetTestConfig(strItem, strName);

            return strValue;
        }


        #endregion

        #region UI

        private void DisplayMessage(string message, string level = "Info")
        {
            Program.g_mainForm.DisplayMessage(message, level);
            return;
        }

        private void ShowTestItem(string testItem)
        {
            Program.g_mainForm.ShowTestItem(testItem);
            return;
        }

        #endregion

        #region NI

        private void NISetDigital(int iPort, int iLine, int iValue)
        {
            Program.g_mainForm.m_objEquipmentInitial.m_objNIDAQ.SetDigital(iPort, iLine, iValue, 0.1);
            return;
        }

        private double NIGetAnalog(int iAI)
        {
            double dValue = 0.0;
            Program.g_mainForm.m_objEquipmentInitial.m_objNIDAQ.GetAnalog(iAI, 10, ref dValue, 0.1);
            return dValue;
        }

        #endregion

        #region Private

        private void Reconnect()
        {
            DisplayMessage("Eject USB Pogopin");
            NISetDigital(0, 0, 0);  // DO0_0 L
            NISetDigital(0, 1, 1);  // DO0_1 H

            clsUtil.Dly(2.0);

            DisplayMessage("Connect USB Pogopin");
            NISetDigital(0, 0, 1);  // DO0_0 H
            NISetDigital(0, 1, 0);  // DO0_1 L  
        }

        private void InitUnitDeviceInfo()
        {
            m_stUnitDeviceInfo.SN = "";
            m_stUnitDeviceInfo.SKU = "";
            m_stUnitDeviceInfo.Model = "";
            m_stUnitDeviceInfo.IMEI = "";
            m_stUnitDeviceInfo.MEID = "";
            m_stUnitDeviceInfo.IMEI2 = "";
            m_stUnitDeviceInfo.MEID2 = "";
            m_stUnitDeviceInfo.Vendor = "";
            m_stUnitDeviceInfo.HWVersion = "";
            m_stUnitDeviceInfo.AndroidOS = "";
            m_stUnitDeviceInfo.AudioPAName = "";
            m_stUnitDeviceInfo.ConfigNumber = "";
            m_stUnitDeviceInfo.EID = "";
            m_stUnitDeviceInfo.WorkOrder = "";
        }

        private bool CheckADBConnected(int times)
        {
            bool bFlag = false;

            for (int i = 0; i < times; i++)
            {
                if (clsCommonFunction.CheckADBConnected() == false)
                {
                    bFlag = false;
                    clsUtil.Dly(1.0);
                    continue;
                }
                else
                {
                    bFlag = true;
                    break;
                }
            }  
            if (bFlag == false)
            {
                return false;
            }

            return true;
        }

        private bool CheckRebootCompleted(ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bFlag = false;
            bool bRes = false;

            string strResult = "";
            string strCmd = "adb shell getprop 'dev.bootcomplete'";

            try
            {

                for (int i = 0; i < 60; i++)
                {
                    DisplayMessage("Wait for Reboot Complete Status ...");
                    bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);
                    DisplayMessage("Wait for Reboot Complete Status: " + strResult);

                    if (strResult.Contains("1"))
                    {
                        bFlag = true;
                        break;
                    }
                    else
                    {
                        bFlag = false;
                        clsUtil.Dly(1.0);
                        continue;
                    }
                }
                if (bFlag == false)
                {
                    strErrorMessage = "Wait Device Reboot Completed fail !!!";
                    return false;
                }
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool ReadMFGData(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strCmd = "";

            List<string> LineData = new List<string>(); // Every Line Data
            Dictionary<string, string> dic_MFGData = new Dictionary<string, string>();  // Dictonary<ID, Data>

            try
            {
                #region Read MFG Data

                strCmd = "adb shell su 0 mfg-tool -p";

                if (clsProcess.ExcuteCmd(strCmd, 1000, ref LineData) == false)
                {
                    strErrorMessage = "Excute cmd fail: " + strCmd;
                    return false;
                }

                if (LineData.Count() == 0)
                {
                    strErrorMessage = "Get MFG Data Empty !";
                    return false;
                }
                Logger.Info("Line data list count is: {0}", LineData.Count());

                #endregion

                #region Parse Each Line

                string ID = "";
                string Data = "";
                int idIndex = 0;
                int sizeIndex = 0;
                int dataIndex = 0;

                foreach(string line in LineData)
                {
                    Logger.Info("Line: {0}", line);

                    idIndex = line.IndexOf("id=");
                    if (idIndex != -1)
                    {
                        idIndex = idIndex + "id=".Length;
                        sizeIndex = line.IndexOf("size=", idIndex);
                        dataIndex = line.IndexOf("data=", sizeIndex);

                        ID = line.Substring(idIndex, sizeIndex - idIndex).Trim();
                        Data = line.Substring(dataIndex + "data=".Length).Trim();   // Bingo, remove whitespace leading and trailing
                        if (string.IsNullOrWhiteSpace(Data))
                        {
                            Data = "";
                        }

                        Logger.Info("ID: {0}, Data: {1}", ID, Data);

                        try
                        {
                            dic_MFGData.Add(ID, Data);
                        }
                        catch (ArgumentException)
                        {
                            strErrorMessage = string.Format("The ID={0} with Data={1}, already exists!", ID, Data);
                            Logger.Error(strErrorMessage);
                            continue;
                        }
                    }
                }
                Logger.Info("Dictionary<ID, Data> count is: {0}", dic_MFGData.Count());

                #endregion

                #region Read MDB Info

                Logger.Info("Read Dictionary MDB Info...");
                foreach (string key in dic_MFGData.Keys)
                {
                    // Configuration Number: CT40P-L1N-27R11BE
                    if (key == "EX_CONFIGURATION_NUMBER")
                    {
                        m_stUnitDeviceInfo.ConfigNumber = dic_MFGData[key];
                        Logger.Info("EX_CONFIGURATION_NUMBER={0}", dic_MFGData[key]);
                    }
                    // Serial Number: 21352B4497
                    else if (key == "EX_SERIAL_NUMBER")
                    {
                        m_stUnitDeviceInfo.SN = dic_MFGData[key];
                        Logger.Info("EX_SERIAL_NUMBER={0}", dic_MFGData[key]);
                    }
                    // Part Number: CT40P-L1N-27R11BE
                    else if (key == "EX_PART_NUMBER")
                    {
                        m_stUnitDeviceInfo.SKU = dic_MFGData[key];
                        Logger.Info("EX_PART_NUMBER={0}", dic_MFGData[key]);
                    }
                    // Mac Address: 001020f43e0c
                    else if (key == "WLAN_MAC_ADDRESS")
                    {
                        m_stUnitDeviceInfo.WLAN_MAC_ADDRESS = dic_MFGData[key].ToUpper();   
                        Logger.Info("WLAN_MAC_ADDRESS={0}", dic_MFGData[key].ToUpper());
                    }
                    // Aux Mac Address: 
                    else if (key == "WLAN_AUX_MAC_ADDRESS")
                    {
                        m_stUnitDeviceInfo.WLAN_AUX_MAC_ADDRESS = dic_MFGData[key].ToUpper();   
                        Logger.Info("WLAN_AUX_MAC_ADDRESS={0}", dic_MFGData[key].ToUpper());
                    }
                    // BlueTooth Address: 001020f43e0d
                    else if (key == "BLUETOOTH_DEVICE_ADDRESS")
                    {
                        m_stUnitDeviceInfo.BLUETOOTH_DEVICE_ADDRESS = dic_MFGData[key].ToUpper();
                        Logger.Info("BLUETOOTH_DEVICE_ADDRESS={0}", dic_MFGData[key].ToUpper());
                    }
                    // Second BlueTooth Address: 0c2369319d66
                    else if (key == "SECOND_BLUETOOTH_DEVICE_ADDRESS")
                    {
                        m_stUnitDeviceInfo.SECOND_BLUETOOTH_DEVICE_ADDRESS = dic_MFGData[key].ToUpper();
                        Logger.Info("SECOND_BLUETOOTH_DEVICE_ADDRESS={0}", dic_MFGData[key].ToUpper());
                    }
                    // Model Number: CT40
                    else if (key == "MODEL_NUMBER")
                    {
                        m_stUnitDeviceInfo.Model = dic_MFGData[key];
                        Logger.Info("MODEL_NUMBER={0}", dic_MFGData[key]);
                    }
                    // Android License:
                    //else if (key == "ANDROID_LICENSE")
                    //{
                    //    objSaveData.TestRecord.AndroidLicenseKey = dic_MFGData[key];         
                    //}
                    // IMEI1: 990016300436126
                    else if (key == "IMEI_NUMBER")
                    {
                        m_stUnitDeviceInfo.IMEI = dic_MFGData[key];
                        Logger.Info("IMEI_NUMBER={0}", dic_MFGData[key]);
                    }
                    // MEID1: 99001630043612
                    else if (key == "MEID_NUMBER")
                    {
                        m_stUnitDeviceInfo.MEID = dic_MFGData[key];
                        Logger.Info("MEID_NUMBER={0}", dic_MFGData[key]);
                    }
                    // IMEI2
                    else if (key == "IMEI_NUMBER_2")
                    {
                        m_stUnitDeviceInfo.IMEI2 = dic_MFGData[key];    
                        Logger.Info("IMEI_NUMBER_2={0}", dic_MFGData[key]);
                    }
                    // MEID2
                    else if (key == "MEID_NUMBER_2")
                    {
                        m_stUnitDeviceInfo.MEID2 = dic_MFGData[key];
                        Logger.Info("MEID_NUMBER_2={0}", dic_MFGData[key]);
                    }
                    // Before Calibration MDB Value
                    // ACCEL_ZERO_OFFSET
                    else if (key == "ACCEL_ZERO_OFFSET")
                    {
                        m_stUnitDeviceInfo.ACCEL_ZERO_OFFSET = dic_MFGData[key].ToUpper();
                        Logger.Info("ACCEL_ZERO_OFFSET={0}", dic_MFGData[key].ToUpper());
                    }
                    // ACCELEROMETER_CALIBRATION
                    else if (key == "ACCELEROMETER_CALIBRATION")
                    {
                        m_stUnitDeviceInfo.ACCELEROMETER_CALIBRATION = dic_MFGData[key].ToUpper();
                        Logger.Info("ACCELEROMETER_CALIBRATION={0}", dic_MFGData[key].ToUpper());
                    }
                    // GYRO_ZERO_OFFSET
                    else if (key == "GYRO_ZERO_OFFSET")
                    {
                        m_stUnitDeviceInfo.GYRO_ZERO_OFFSET = dic_MFGData[key].ToUpper();
                        Logger.Info("GYRO_ZERO_OFFSET={0}", dic_MFGData[key].ToUpper());
                    }
                    // GYROSCOPE_CALIBRATION
                    else if (key == "GYROSCOPE_CALIBRATION")
                    {
                        m_stUnitDeviceInfo.GYROSCOPE_CALIBRATION = dic_MFGData[key].ToUpper();
                        Logger.Info("GYROSCOPE_CALIBRATION={0}", dic_MFGData[key].ToUpper());
                    }
                    // PROXIMITY_CALIBRATION
                    else if (key == "PROXIMITY_CALIBRATION")
                    {
                        m_stUnitDeviceInfo.PROXIMITY_CALIBRATION = dic_MFGData[key].ToUpper();
                        Logger.Info("PROXIMITY_CALIBRATION={0}", dic_MFGData[key].ToUpper());
                    }
                    // PROXIMITY_CALIBRATION_EXTEND
                    else if (key == "PROXIMITY_CALIBRATION_EXTEND")
                    {
                        m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_EXTEND = dic_MFGData[key].ToUpper();
                        Logger.Info("PROXIMITY_CALIBRATION_EXTEND={0}", dic_MFGData[key].ToUpper());
                    }
                    // MAX98390L_TROOM
                    else if (key == "MAX98390L_TROOM")
                    {
                        m_stUnitDeviceInfo.MAX98390L_TROOM = dic_MFGData[key].ToUpper();
                        Logger.Info("MAX98390L_TROOM={0}", dic_MFGData[key].ToUpper());
                    }
                    // MAX98390L_RDC
                    else if (key == "MAX98390L_RDC")
                    {
                        m_stUnitDeviceInfo.MAX98390L_RDC = dic_MFGData[key].ToUpper();
                        Logger.Info("MAX98390L_RDC={0}", dic_MFGData[key].ToUpper());
                    }
                }

                #endregion

                #region Check MFG Data

                // SN
                string strSN = m_stUnitDeviceInfo.SN;
                if (string.IsNullOrWhiteSpace(strSN) || (strSN.Length != 10))
                {
                    strErrorMessage = "Read mfg data fail: Invalid SN.";
                    return false;
                }
                // SKU
                string strSKU = m_stUnitDeviceInfo.SKU;
                if (string.IsNullOrWhiteSpace(strSKU))
                {
                    strErrorMessage = "Read mfg data fail: Invalid SKU.";
                    return false;
                }
                // 截取 Model

                // MODEL
                string strModel = m_stUnitDeviceInfo.Model;
                if (string.IsNullOrWhiteSpace(strModel))
                {
                    strErrorMessage = "Read mfg data fail: Invalid Model.";
                    return false;
                }  
                if (strModel.Contains(Program.g_mainForm.m_strModel) == false)
                {
                    MessageBox.Show("The Product Not Match the Production Line That You Selected !!!");
                    return false;
                }
                // ISWWAN
                if (strModel == "CT45")
                {
                    if (strSKU.Substring(6, 1) == "1")
                    {
                        m_bIsWWAN = true;
                    }
                    else if (strSKU.Substring(6, 1) == "0")
                    {
                        m_bIsWWAN = false;
                    }
                    else
                    {
                        DisplayMessage("Failed to check SKU WWAN flag: " + strSKU);
                        return false;
                    }
                }
                else if (strModel == "CT45P")
                {
                    if (strSKU.Substring(7, 1) == "1")
                    {
                        m_bIsWWAN = true;
                    }
                    else if (strSKU.Substring(7, 1) == "0")
                    {
                        m_bIsWWAN = false;
                    }
                    else
                    {
                        DisplayMessage("Failed to check SKU WWAN flag: " + strSKU);
                        return false;
                    }
                }
                // IMEI and MEID
                string IMEI = m_stUnitDeviceInfo.IMEI;
                string MEID = m_stUnitDeviceInfo.MEID;
                if (m_bIsWWAN == true)
                {
                    if ((IMEI.Length != 15) || (!IMEI.StartsWith("9900")))
                    {
                        strErrorMessage = "Read mfg data fail: Invalid IMEI.";
                        return false;
                    }
                    if ((MEID.Length != 14) || (!MEID.StartsWith("9900")))
                    {
                        strErrorMessage = "Read mfg data fail: Invalid MEID.";
                        return false;
                    }
                }

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }



        #endregion
    }
}
