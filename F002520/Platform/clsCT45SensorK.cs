using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using SmartFactory.ExternalDLL;
using System.IO;
using System.Text.RegularExpressions;

namespace F002520
{
    class clsCT45SensorK : clsSensorKBase
    {
       
        #region Variable

        private bool m_bIsWWAN = false;
        private bool m_bIsTDKBaroSensor = false;
        private double m_dCurrentDamBoardPosition = 0.0;

        private const string PSensorName = "android.sensor.proximity";
        private const string GSensorName = "android.sensor.accelerometer";
        private const string BarometerName = "android.sensor.pressure";

        private clsExecProcess clsProcess = new clsExecProcess();
        private UnitDeviceInfo m_stUnitDeviceInfo = new UnitDeviceInfo();

        #endregion

        #region TestItem

        public override bool TestInit(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                InitUnitDeviceInfo();

                InitNIPort();
            }
            catch(Exception ex)
            {
                strErrorMessage = "TestInit Exception:" + ex.Message;
                return false;  
            }

            return true;
        }

        public override bool TestPowerOn(ref string strErrorMessage)
        {
            strErrorMessage = ""; 
            bool bRes = false;

            try
            {     
        
                bRes = InsertUSBCable();
                if (bRes == false)
                {
                    strErrorMessage = "Fail to insert USB cable !!!";
                    return false;
                }

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
            strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            bool bFlag = false;
            int ReConnectTimes = 0;

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                ReConnectTimes = int.Parse(GetTestItemParameter(strTestItem, "ReConnectTimes"));
                if (ReConnectTimes > 3)
                {
                    ReConnectTimes = 3;
                }
                Logger.Info("ReConnectTimes: " + ReConnectTimes.ToString());

                #endregion

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

                // Change Log File Name
                string strDate = DateTime.Now.ToString("yyyyMMdd_hhmmss");
                string logFileName = string.Format("Debug_{0}_{1}.log", strSN, strDate);
                string folder = Application.StartupPath + @"\log\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\"; ;
                logFileName = folder + logFileName;
                Logger.ChangeLogFileName("RollingFile", logFileName);

                Logger.Info("Start Unit Test.");
                Logger.Info("SN:{0}", strSN);

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "TestCheckDeviceReady Exception: " + ex.Message;
                return false;
            }

            return true;
        }

        public override bool TestReadMFGData(ref string strErrorMessage)
        {
            strErrorMessage = "";
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

                #region Get Android OS Version

                for (int i = 0; i < 3; i++)
                {
                    if (ReadOSVersion(ref strErrorMessage) == false)
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
                    strErrorMessage = "Fail to Read Android OS Version: " + strErrorMessage;
                    return false;
                }

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "TestReadMFGData Exception: " + ex.Message;
                return false;
            
            }

            return true;
        }

        public override bool TestCheckRFResult(ref string strErrorMessage)
        {
            strErrorMessage = "";
            //bool bFlag = false;

            try
            { 
            


            }
            catch(Exception ex)
            {
                strErrorMessage = "TestCheckRFResult Exception: " + ex.Message;
                return false;
            }

            return true;
        }

        public override bool TestCheckPreStation(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            bool bFlag = false;   
            string strURL = "";
            string PreStationDeviceName = "";
            string PreStationVarName = "";
            string PreStationVarTargetResult = "";     
            string strSN = m_stUnitDeviceInfo.SN;

            try
            {
                DisplayMessage("Start to Check Pre Station Result.");
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                // PreStationDeviceName
                PreStationDeviceName = GetTestItemParameter(strTestItem, "PreStationDeviceName");
                if (string.IsNullOrWhiteSpace(PreStationDeviceName))
                {
                    strErrorMessage = "Fail to get PreStationDeviceName value !";
                    return false;
                }

                string[] DeviceNameList = PreStationDeviceName.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                if (DeviceNameList.Length > 1)
                {
                    PreStationDeviceName = "";
                    for (int i = 0; i < DeviceNameList.Length; i++)
                    {
                        string deviceName = DeviceNameList[i].Trim();
                        Match match = Regex.Match(deviceName, @"(CT45P|CT45)");
                        if (match.Success)
                        {
                            string ModelValue = match.Value;
                            if (ModelValue != m_stUnitDeviceInfo.Model)
                            {
                                strErrorMessage = string.Format("DeviceName: {0}, Not Match Device Model !!!", deviceName);
                                //DisplayMessage(strErrorMessage, "ERROR");
                                bFlag = false;
                                continue;
                            }
                            else
                            {
                                PreStationDeviceName = deviceName.Trim();
                                bFlag = true; 
                                break;                   
                            }
                        }
                        else
                        {
                            strErrorMessage = string.Format("DeviceName: {0}, Not Contain Device Model !!!", deviceName);       
                            bFlag = false;
                            break;
                        }
                    }     
                    if (bFlag == false)
                    {
                        DisplayMessage(strErrorMessage, "ERROR");
                        return false;
                    }
                }

                DisplayMessage("PreStation Device Name: " + PreStationDeviceName);

                // VarName
                PreStationVarName = GetTestItemParameter(strTestItem, "VarName");
                if (string.IsNullOrWhiteSpace(PreStationVarName))
                {
                    strErrorMessage = "Fail to get PreStationVarName value !";
                    return false;
                }

                // VarTargetResult
                PreStationVarTargetResult = GetTestItemParameter(strTestItem, "VarTargetResult");
                if (string.IsNullOrWhiteSpace(PreStationVarTargetResult))
                {
                    strErrorMessage = "Fail to get PreStationVarTargetResult value !";
                    return false;
                }

                // URL
                strURL = GetTestConfig("MDCS", "URL");
                if (string.IsNullOrWhiteSpace(strURL))
                {
                    strErrorMessage = "Fail to get MDCS URL value !";
                    return false;
                }

                #endregion

                #region Get MDCS Variable

                clsMDCS obj_SaveMDCS = new clsMDCS();
                obj_SaveMDCS.ServerName = strURL;
                obj_SaveMDCS.DeviceName = PreStationDeviceName;
                obj_SaveMDCS.UseModeProduction = true;

                string strResult = "";
                for (int i = 0; i < 3; i++)
                {
                    bRes = obj_SaveMDCS.GetMDCSVariable(PreStationDeviceName, PreStationVarName, strSN, ref strResult, ref strErrorMessage);
                    if (bRes == false)
                    {
                        bFlag = false;
                        strErrorMessage = "GetMDCSVariable fail.";
                        clsUtil.Dly(2.0);
                        continue;
                    }
                    else
                    {
                        if (strResult != PreStationVarTargetResult)
                        {
                            bFlag = false;
                            strErrorMessage = "Compare value fail. Result: " + strResult;
                            clsUtil.Dly(1.0);
                            continue;
                        }
                        else
                        {
                            bFlag = true;
                            break;
                        }
                    }
                }
                if (bFlag == false)
                {
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }

                #endregion

                DisplayMessage("Test Check Pre Station MDCS Result Sucessful.");
            }
            catch(Exception ex)
            {
                strErrorMessage = "TestCheckPreStation Exception:" + ex.Message;
                return false;
            }

            return true;   
        }

        public override bool TestAutoChangeOver(ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;
            bool bFlag = false;
            //bool MES_Enable = false;
            string strCmd = "";
            string strEID = "";
            string strWorkOrder = "";
            string strTestItem = "";        
            //string OptionMES_Enable = Program.g_mainForm.m_stOptionData.MES_Enable;   
            string OptionMES_Enable = frmMain.m_stOptionData.MES_Enable;   
            string XmlMES_Enable = "";
            string MESStationName = ""; 
            string ScanSheetStationName = "";
            string SWVersionControl = "";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                XmlMES_Enable = GetTestConfig("MES", "Enable").ToUpper();
                if (string.IsNullOrWhiteSpace(XmlMES_Enable))
                {
                    strErrorMessage = "Fail to get MES Enable value !";
                    return false;
                }
                if (XmlMES_Enable == "TRUE" && OptionMES_Enable == "1")
                {
                    frmMain.m_bMESEnable = true;
                }

                MESStationName = GetTestConfig("MES", "Station");
                if (string.IsNullOrWhiteSpace(MESStationName))
                {
                    strErrorMessage = "Fail to get MESStationName value !";
                    return false;
                }

                ScanSheetStationName = GetTestItemParameter(strTestItem, "ScanSheetStation");
                if (string.IsNullOrWhiteSpace(ScanSheetStationName))
                {
                    strErrorMessage = "Fail to get ScanSheetStationName value !";
                    return false;
                }

                SWVersionControl = GetTestItemParameter(strTestItem, "SWVersionControl").ToUpper();
                if ((SWVersionControl != "TRUE") && (SWVersionControl != "FALSE"))
                {
                    strErrorMessage = "Invalid SWVersionControl Value !";
                    return false;
                }

                #endregion

                #region Get Workorder Property

                if (frmMain.m_bMESEnable == true)
                {
                    DisplayMessage("Get WorkOrder Property.");
                    strCmd = "adb shell getprop persist.sys.WorkOrder";

                    for (int i = 0; i < 3; i++)
                    {
                        bRes = clsProcess.ExcuteCmd(strCmd, 500, ref strWorkOrder);     
                        if (bRes && !string.IsNullOrWhiteSpace(strWorkOrder))
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
                        strErrorMessage = "Get WorkOrder property fail.";
                        return false;
                    }
                    m_stUnitDeviceInfo.WorkOrder = strWorkOrder;
                    frmMain.m_stTestSaveData.TestRecord.WorkOrder = strWorkOrder;
                    DisplayMessage("WorkOrder: " + strWorkOrder);    
                }
                else
                {
                    DisplayMessage("Skip to Get WorkOrder Property.");
                }

                #endregion

                #region Get EID Property

                if (frmMain.m_bMESEnable == true)
                {
                    DisplayMessage("Get EID Property.");
                    strCmd = "adb shell getprop persist.sys.FLASH";

                    for (int i = 0; i < 3; i++)
                    {
                        bRes = clsProcess.ExcuteCmd(strCmd, 500, ref strEID);  
                        if (bRes && !string.IsNullOrWhiteSpace(strEID))
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
                        strErrorMessage = "Get EID property fail.";
                        return false;
                    }
                    m_stUnitDeviceInfo.EID = strEID;
                    frmMain.m_stTestSaveData.TestRecord.EID = strEID;
                    DisplayMessage("EID: " + strEID);     
                }
                else
                {
                    DisplayMessage("Skip to Get EID Property.");
                }

                #endregion

                #region Check MES Data

                if (frmMain.m_bMESEnable == true)
                {
                    DisplayMessage("Check MES Data");
                    if (clsUploadMES.MESCheckData(strEID, MESStationName, strWorkOrder, ref strErrorMessage) == false)
                    {
                        DisplayMessage("Failed to Check MES Data.");
                        return false;
                    }

                    DisplayMessage("Check MES Data Successful.");
                }
                else
                {
                    DisplayMessage("Skip to Check MES Data");
                }

                #endregion

                #region Get ScanSheet


                #endregion

                #region Check SW Version

                if (SWVersionControl == "TRUE")
                {
                    DisplayMessage("Check Software Version.");
                    if (SWVersionCheck(ref strErrorMessage) == false)
                    {
                        MessageBox.Show("Software Vesion Not Matched !!!");
                        return false;
                    }
                }
                else
                {
                    DisplayMessage("Skip to Check SW Version");
                }

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "TestAutoChangeOver Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestScreenOff(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";  
            bool bFlag = false;
        
            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                for (int i = 0; i < 5; i++)
                {
                    if (ScreenOffAction(ref strErrorMessage) == false)
                    {
                        bFlag = false;
                        clsUtil.Dly(2.0);
                        continue;                   }
                    else
                    {
                        bFlag = true;
                        break;
                    }
                }
                if (bFlag == false)
                {
                    strErrorMessage = "Fail to control screen off, " + strErrorMessage;
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }

                DisplayMessage("Test Control Screen Off Successful.");
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestScreenOff Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestMoveDamBoardUp(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";     
            double dHome = 0.0;
            double dPosition = 0.0;

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                dHome = Convert.ToDouble(clsConfigHelper.servoMotor.Home);
                dPosition = Convert.ToDouble(GetTestItemParameter(strTestItem, "Position"));  
                DisplayMessage("Home: " + dHome.ToString());
                DisplayMessage("Position: " + dPosition.ToString());

                #endregion

                // Move to 5cm Distance to Screen
                m_dCurrentDamBoardPosition = 0.0;
                if (MoveDamBoardUp(dPosition, dHome, ref strErrorMessage) == false)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestMoveDamBoardUp Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestCheckSensorList(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bFlag = false;

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                for (int i = 0; i < 3; i++)
                {
                    if (CheckSensorList(ref strErrorMessage) == false)
                    {
                        bFlag = false;
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
                    strErrorMessage = "Fail to check sensor list, " + strErrorMessage;
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestCheckSensorList Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestGSensorCalibation(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            bool bFlag = false;
            string strRunCmd = "";
            string strValueCmd = "";
            string strResult = "";
            string ACCEL_ZERO_OFFSET_BEFORE = m_stUnitDeviceInfo.ACCEL_ZERO_OFFSET_BEFORE;
            string ACCEL_ZERO_OFFSET_AFTER = "";

            frmMain.m_stTestSaveData.TestRecord.TestGSensorCalibration = "Fail";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML 

                // Calibration CMD
                strRunCmd = GetTestItemParameter(strTestItem, "CalibrationCmd");
                if (string.IsNullOrWhiteSpace(strRunCmd))
                {
                    strErrorMessage = "Fail to parse CalibrationCmd parameter !";
                    return false;
                }
                DisplayMessage("Param CalibrationCmd: " + strRunCmd);

                // Get Offset CMD
                strValueCmd = GetTestItemParameter(strTestItem, "GetMDBCmd");
                if (string.IsNullOrWhiteSpace(strValueCmd))
                {
                    strErrorMessage = "Fail to parse GetMDBCmd parameter !";
                    return false;
                }
                DisplayMessage("Param GetMDBCmd: " + strValueCmd);

                #endregion

                DisplayMessage("Before GSensor Calibration, ACCEL_ZERO_OFFSET=" + ACCEL_ZERO_OFFSET_BEFORE);

                #region GSensor(ACC) Calibration

                for (int i = 0; i < 3; i++)
                {
                    DisplayMessage(string.Format("LOOP_{0}: Do GSensor Calibration.", i.ToString()));
                    DisplayMessage("Run CMD: " + strRunCmd);

                    bRes = clsProcess.ExcuteCmd(strRunCmd, 2000, ref strResult);   // 15s              
                    DisplayMessage("Result: \r\n" + strResult);
                    if (strResult.IndexOf("calibration PASS", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        strErrorMessage = "Run Cmd to do GSensor Calibration fail !!!";
                        bFlag = false;
                        clsUtil.Dly(3.0);
                        continue;
                    }
                    else
                    {
                        // Calibration Pass
                        #region Compare Before and After Value

                        //string cmd = "adb shell su 0 mfg-tool -g ACCEL_ZERO_OFFSET";
                        bRes = clsProcess.ExcuteCmd(strValueCmd, 200, ref strResult);
                        DisplayMessage("Run CMD: " + strValueCmd);
                        DisplayMessage("After GSensor Calibration, ACCEL_ZERO_OFFSET=" + strResult.ToUpper());
                        ACCEL_ZERO_OFFSET_AFTER = strResult.ToUpper();
                        m_stUnitDeviceInfo.ACCEL_ZERO_OFFSET_AFTER = strResult.ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.ACCEL_ZERO_OFFSET_AFTER = strResult.ToUpper();

                        if (strResult.Contains("000000000000000000000000"))  // Equal to default value
                        {
                            strErrorMessage = "GSensor calibration fail, ACCEL_ZERO_OFFSET still default value !!!";
                            bFlag = false;
                            clsUtil.Dly(3.0);
                            continue;
                        }
                        if (ACCEL_ZERO_OFFSET_AFTER == ACCEL_ZERO_OFFSET_BEFORE)
                        {
                            strErrorMessage = "Before and After calibration, ACCEL_ZERO_OFFSET value not changed !!!";
                            bFlag = false;
                            clsUtil.Dly(3.0);
                            continue;
                        }
                        else
                        {
                            bFlag = true;
                            break;
                        }

                        #endregion              
                    }             
                }
                if (bFlag == false)
                {
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }

                // ACCELEROMETER_CALIBRATION_AFTER
                string strCmd = "adb shell su 0 mfg-tool -g ACCELEROMETER_CALIBRATION";
                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);
                DisplayMessage("Run CMD: " + strCmd);
                DisplayMessage("After GSensor Calibration, ACCELEROMETER_CALIBRATION=" + strResult.ToUpper());
                frmMain.m_stTestSaveData.TestRecord.ACCELEROMETER_CALIBRATION_AFTER = strResult.ToUpper();
  
                #endregion

                frmMain.m_stTestSaveData.TestRecord.TestGSensorCalibration = "Pass";
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestGSensorCalibation Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestGYROSensorCalibration(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            bool bFlag = false;
            string strRunCmd = "";
            string strValueCmd = "";
            string strResult = "";
            string GYRO_ZERO_OFFSET_BEFORE = m_stUnitDeviceInfo.GYRO_ZERO_OFFSET_BEFORE;
            string GYRO_ZERO_OFFSET_AFTER = "";

            frmMain.m_stTestSaveData.TestRecord.TestGYROSensorCalibration = "Fail";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                // Calibration CMD
                strRunCmd = GetTestItemParameter(strTestItem, "CalibrationCmd");
                if (string.IsNullOrWhiteSpace(strRunCmd))
                {
                    strErrorMessage = "Fail to parse CalibrationCmd parameter !";
                    return false;
                }
                DisplayMessage("Param CalibrationCmd: " + strRunCmd);

                // Get Offset CMD
                strValueCmd = GetTestItemParameter(strTestItem, "GetMDBCmd");
                if (string.IsNullOrWhiteSpace(strValueCmd))
                {
                    strErrorMessage = "Fail to parse GetMDBCmd parameter !";
                    return false;
                }
                DisplayMessage("Param GetMDBCmd: " + strValueCmd);

                #endregion

                DisplayMessage("Before GYRO Sensor Calibration, GYRO_ZERO_OFFSET=" + GYRO_ZERO_OFFSET_BEFORE);

                #region GYRO Sensor Calibration

                for (int i = 0; i < 3; i++)
                {
                    DisplayMessage(string.Format("LOOP_{0}: Do GYRO Sensor Calibration.", i.ToString()));
                    DisplayMessage("Run CMD: " + strRunCmd);

                    bRes = clsProcess.ExcuteCmd(strRunCmd, 2000, ref strResult);    // 15s
                    DisplayMessage("Result: \r\n" + strResult);
                    if (strResult.IndexOf("PASS", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        strErrorMessage = "Run Cmd to do GYRO Sensor Calibration fail !!!";
                        bFlag = false;
                        clsUtil.Dly(3.0);
                        continue;
                    }
                    else
                    {
                        #region Compare Before and After Value

                        //string cmd = "adb shell su 0 mfg-tool -g GYRO_ZERO_OFFSET";
                        bRes = clsProcess.ExcuteCmd(strValueCmd, 200, ref strResult);
                        DisplayMessage("Run CMD: " + strValueCmd);
                        DisplayMessage("After GYRO Sensor Calibration, GYRO_ZERO_OFFSET=" + strResult);
                        GYRO_ZERO_OFFSET_AFTER = strResult.ToUpper();
                        m_stUnitDeviceInfo.GYRO_ZERO_OFFSET_AFTER = GYRO_ZERO_OFFSET_AFTER;
                        frmMain.m_stTestSaveData.TestRecord.GYRO_ZERO_OFFSET_AFTER = GYRO_ZERO_OFFSET_AFTER;

                        if (strResult.Contains("000000000000000000000000"))  // Equal to default value
                        {
                            strErrorMessage = "GYRO Sensor calibration fail, GYRO_ZERO_OFFSET still default value !!!";
                            bFlag = false;
                            clsUtil.Dly(3.0);
                            continue;
                        }
                        if (GYRO_ZERO_OFFSET_AFTER == GYRO_ZERO_OFFSET_BEFORE)
                        {
                            strErrorMessage = "Before and After calibration, GYRO_ZERO_OFFSET value not changed !!!";
                            bFlag = false;
                            clsUtil.Dly(3.0);
                            continue;
                        }
                        else
                        {
                            bFlag = true;
                            break;
                        }

                        #endregion       
                    }      
                }
                if (bFlag == false)
                {
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }

                // GYROSCOPE_CALIBRATION_AFTER
                string strCmd = "adb shell su 0 mfg-tool -g GYROSCOPE_CALIBRATION";
                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);
                DisplayMessage("Run CMD: " + strCmd);
                DisplayMessage("After GYRO Sensor Calibration, GYROSCOPE_CALIBRATION=" + strResult.ToUpper());
                frmMain.m_stTestSaveData.TestRecord.GYROSCOPE_CALIBRATION_AFTER = strResult.ToUpper();

                #endregion

                frmMain.m_stTestSaveData.TestRecord.TestGYROSensorCalibration = "Pass";
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestGYROSensorCalibration Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestPSensorCalibration(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            bool bFlag = false;
            string strRunCmd = "";
            string strValueCmd = "";
            string strResult = "";
            string NEAR_THRESHOLD = "";
            string FAR_THRESHOLD = "";
            string DEFAULT_NEAR_THRESHOLD = "800.000000";
            string DEFAULT_FAR_THRESHOLD = "600.000000";
            string PROXIMITY_CALIBRATION_EXTEND_BEFORE = m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_EXTEND_BEFORE;
            string PROXIMITY_CALIBRATION_EXTEND_AFTER = "";

            frmMain.m_stTestSaveData.TestRecord.TestPSensorCalibration = "Fail";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                // Calibration CMD
                strRunCmd = GetTestItemParameter(strTestItem, "CalibrationCmd");
                if (string.IsNullOrWhiteSpace(strRunCmd))
                {
                    strErrorMessage = "Fail to parse CalibrationCmd parameter !";
                    return false;
                }
                DisplayMessage("Param CalibrationCmd: " + strRunCmd);

                // Get Offset CMD
                strValueCmd = GetTestItemParameter(strTestItem, "GetMDBCmd");
                if (string.IsNullOrWhiteSpace(strValueCmd))
                {
                    strErrorMessage = "Fail to parse GetMDBCmd parameter !";
                    return false;
                }
                DisplayMessage("Param GetMDBCmd: " + strValueCmd);

                #endregion

                DisplayMessage("Before Proximity Sensor Calibration, PROXIMITY_CALIBRATION_EXTEND=" + PROXIMITY_CALIBRATION_EXTEND_BEFORE);

                #region Proximity Sensor Calibration

                /* ********************************** Default Threshold *********************************
                 * PREV near_threshold: "800.000000"
                 * PREV far_threshold: "600.000000"
                 * 
                 * After Calibration, The Threshold Value Must Changed.
                 * 
                 ****************************************************************************************/
                for (int i = 0; i < 3; i++)
                {
                    DisplayMessage(string.Format("LOOP_{0}: Do Proximity Sensor Calibration.", i.ToString()));
                    DisplayMessage("Run CMD: " + strRunCmd);

                    bRes = clsProcess.ExcuteCmd(strRunCmd, 2000, ref strResult);    // 15s
                    DisplayMessage("Result: \r\n" + strResult);

                    if (strResult.IndexOf("FAIL", StringComparison.OrdinalIgnoreCase) != -1)    // Somewhere appear fail
                    {
                        strErrorMessage = "Fail to do Proximity Sensor Calibration !!!";
                        bFlag = false;
                        clsUtil.Dly(3.0);
                        continue;
                    }
                    else if (strResult.IndexOf("calibration PASS", StringComparison.OrdinalIgnoreCase) != -1)   // calibration PASS, if Near/Far threshold also default value, still fail.
                    {
                        //string[] LineArray = strResult.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] LineArray = strResult.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        string line = "";
                        int startIndex = 0;
                        int endIndex = 0;
                        for (int j = 0; j < LineArray.Length; j++)
                        {
                            line = LineArray[j].Trim();
                            if (line.StartsWith("near_threshold:"))
                            {
                                startIndex = line.IndexOf("\"") + 1;
                                endIndex = line.LastIndexOf("\"");
                                NEAR_THRESHOLD = line.Substring(startIndex, endIndex - startIndex);
                            }

                            if (line.StartsWith("far_threshold:"))
                            {
                                startIndex = line.IndexOf("\"") + 1;
                                endIndex = line.LastIndexOf("\"");
                                FAR_THRESHOLD = line.Substring(startIndex, endIndex - startIndex);
                            }
                        }
                        DisplayMessage("After Calibration, NEAR_THRESHOLD: " + NEAR_THRESHOLD);
                        DisplayMessage("After Calibration, FAR_THRESHOLD: " + FAR_THRESHOLD);

                        // 使用正则表达式匹配浮点数模式
                        Regex regex = new Regex(@"^\d+(\.\d+)?$");
                        if ((regex.IsMatch(NEAR_THRESHOLD) == false) || (regex.IsMatch(FAR_THRESHOLD) == false))
                        {
                            strErrorMessage = "Fail to Get Near/Far threshold value !!!";
                            bFlag = false;
                            clsUtil.Dly(3.0);
                            continue;
                        }

                        if ((NEAR_THRESHOLD == DEFAULT_NEAR_THRESHOLD) || (FAR_THRESHOLD == DEFAULT_FAR_THRESHOLD))
                        {
                            strErrorMessage = "After Calibration, the Near/Far threshold value not change !!!";
                            bFlag = false;
                            clsUtil.Dly(3.0);
                            continue;
                        }
                        else
                        {
                            bFlag = true;
                            break;
                        }
                    }
                }
                if (bFlag == false)
                {
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }

                DisplayMessage("Run Cmd to do Proximity Sensor Calibration Successful.");

                #endregion

                #region Compare Before and After Value

                //string cmd = "adb shell su 0 mfg-tool -g PROXIMITY_CALIBRATION_EXTEND";
                bRes = clsProcess.ExcuteCmd(strValueCmd, 200, ref strResult);
                DisplayMessage("Run CMD: " + strValueCmd);
                DisplayMessage("After Proximity Sensor Calibration, PROXIMITY_CALIBRATION_EXTEND=" + strResult.ToUpper());
                PROXIMITY_CALIBRATION_EXTEND_AFTER = strResult.ToUpper();
                m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_EXTEND_AFTER = PROXIMITY_CALIBRATION_EXTEND_AFTER;
                frmMain.m_stTestSaveData.TestRecord.PROXIMITY_CALIBRATION_EXTEND_AFTER = PROXIMITY_CALIBRATION_EXTEND_AFTER;

                if (strResult.Contains("00000000"))  // Equal to default value
                {
                    strErrorMessage = "Proximity Sensor calibration fail, PROXIMITY_CALIBRATION_EXTEND still default value !!!";
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }
                if (PROXIMITY_CALIBRATION_EXTEND_AFTER == PROXIMITY_CALIBRATION_EXTEND_BEFORE)
                {
                    strErrorMessage = "Before and After calibration, GYRO_ZERO_OFFSET value not changed !!!";
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }
         
                #endregion 

                #region PROXIMITY_CALIBRATION_AFTER

                string strCmd = "adb shell su 0 mfg-tool -g PROXIMITY_CALIBRATION";
                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);
                DisplayMessage("Run CMD: " + strCmd);
                DisplayMessage("After Proximity Sensor Calibration, PROXIMITY_CALIBRATION=" + strResult.ToUpper());
                frmMain.m_stTestSaveData.TestRecord.PROXIMITY_CALIBRATION_AFTER = strResult.ToUpper();

                #endregion

                frmMain.m_stTestSaveData.TestRecord.TestPSensorCalibration = "Pass";
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestPSensorCalibration Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestPSensorFunction(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bFlag = false;
            string strNearPosition = "";
            string strFarPosition = "";
            string PSENSOR_GET_LOG = "";

            frmMain.m_stTestSaveData.TestRecord.TestPSensorFunction = "Fail";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                // Near-Position Value
                strNearPosition = GetTestItemParameter(strTestItem, "NearPosition");
                if (string.IsNullOrWhiteSpace(strNearPosition))
                {
                    strErrorMessage = "Fail to parse Near-Position parameter !";
                    return false;
                }
                DisplayMessage("Param NearPosition: " + strNearPosition);

                // Far-Position Value
                strFarPosition = GetTestItemParameter(strTestItem, "FarPosition");
                if (string.IsNullOrWhiteSpace(strFarPosition))
                {
                    strErrorMessage = "Fail to parse Far-Position parameter !";
                    return false;
                }
                DisplayMessage("Param FarPosition: " + strFarPosition);

                // PSensor Get Log Cmd
                PSENSOR_GET_LOG = GetTestItemParameter(strTestItem, "GetPSensorLog");
                if (string.IsNullOrWhiteSpace(PSENSOR_GET_LOG))
                {
                    strErrorMessage = "Fail to parse GetPSensorLog parameter !";
                    return false;
                }
                DisplayMessage("Param GetPSensorLog: " + PSENSOR_GET_LOG);

                #endregion

                #region Trun On Display

                for (int i = 0; i < 3; i++)
                {
                    if (ScreenOnAction(ref strErrorMessage) == false)
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
                    strErrorMessage = "Fail to control screen on, " + strErrorMessage;
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }

                #endregion

                #region Check PSensor Near-Position Function

                if (CheckNearPositionFunction(strNearPosition, PSENSOR_GET_LOG, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Fail to Check PSensor Near Function, " + strErrorMessage;
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }

                #endregion

                #region Check PSensor Far-Position Function

                if (CheckFarPositionFunction(strFarPosition, PSENSOR_GET_LOG, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Fail to Check PSensor Far Function, " + strErrorMessage;
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }

                #endregion

                frmMain.m_stTestSaveData.TestRecord.TestPSensorFunction = "Pass";
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestPSensorFunction Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestAudioCalibration(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            bool bFlag = false;
            string strCmd = "";
            string strResult = "";
            string AudioPANameCmd = "";
            string CalibrationCmd = "";
            string GetMDBCmd = "";
            string MAX98390L_TROOM_BEFORE = m_stUnitDeviceInfo.MAX98390L_TROOM_BEFORE;
            string MAX98390L_RDC_BEFORE = m_stUnitDeviceInfo.MAX98390L_RDC_BEFORE;
            string MAX98390L_TROOM_AFTER = "";
            string MAX98390L_RDC_AFTER = "";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                // Get Audio PA Name Cmd
                AudioPANameCmd = GetTestItemParameter(strTestItem, "GetPAName");
                if (string.IsNullOrWhiteSpace(AudioPANameCmd))
                {
                    strErrorMessage = "Fail to parse GetPAName parameter !";
                    return false;
                }        
                DisplayMessage("Param GetPANameCmd: " + AudioPANameCmd);

                // Audio Calibration Cmd
                CalibrationCmd = GetTestItemParameter(strTestItem, "CalibrationCmd");
                if (string.IsNullOrWhiteSpace(CalibrationCmd))
                {
                    strErrorMessage = "Fail to parse CalibrationCmd parameter !";
                    return false;
                }
                DisplayMessage("Param CalibrationCmd: " + CalibrationCmd);

                // Get MDB Cmd
                GetMDBCmd = GetTestItemParameter(strTestItem, "GetMDBCmd");
                if (string.IsNullOrWhiteSpace(GetMDBCmd))
                {
                    strErrorMessage = "Fail to parse GetMDBCmd parameter !";
                    return false;
                }
                DisplayMessage("Param GetMDBCmd: " + GetMDBCmd);

                #endregion

                #region Get Audio PA Name

                DisplayMessage("Get Audio PA Name.");
                bRes = clsProcess.ExcuteCmd(AudioPANameCmd, 200, ref strResult);
                DisplayMessage("Send Cmd: " + AudioPANameCmd);
                DisplayMessage("Audio PA Name: " + strResult);
                m_stUnitDeviceInfo.AudioPAName = strResult;
                frmMain.m_stTestSaveData.TestRecord.AudioPAName = strResult;

                if (strResult.IndexOf("max98390xx", StringComparison.OrdinalIgnoreCase) == -1) // Not Max Audio Chip
                {
                    DisplayMessage("Not max98390xx Audio Chip, Skip to do Audio Calibration ...");
                    return true;
                }

                #endregion

                frmMain.m_stTestSaveData.TestRecord.TestAudioCalibration = "Fail";

                #region Get MDB Value

                DisplayMessage("Before Audio Calibration.");
                DisplayMessage("MAX98390L_TROOM = " + MAX98390L_TROOM_BEFORE);
                DisplayMessage("MAX98390L_RDC = " + MAX98390L_RDC_BEFORE);

                #endregion

                #region Turn On Display

                DisplayMessage("Turn On Display ...");

                for (int i = 0; i < 3; i++)
                {
                    if (ScreenOnAction(ref strErrorMessage) == false)
                    {
                        strErrorMessage = "Fail to turn on display !";
                        bFlag = false;
                        clsUtil.Dly(3.0);
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
                    DisplayMessage(strErrorMessage);
                    return false;
                }

                DisplayMessage("Turn On Display Successful.");

                #endregion

                #region Adjust the Volume to Maximum

                DisplayMessage("Adjust the volume to Maximum.");
                strCmd = "adb shell input keyevent 24";

                for (int i = 0; i <= 13; i++ )
                {
                    DisplayMessage("Loop_" + i.ToString() + ": Raise the volume");
                    DisplayMessage("Send Cmd: " + strCmd);
                    bRes = clsProcess.ExcuteCmd(strCmd, 200);
                    clsUtil.Dly(0.2);
                }

                #endregion

                #region Audio Calibration

                DisplayMessage("Start to do Audio Calibration.");
                bRes = clsProcess.ExcuteCmd(CalibrationCmd, 2000, ref strResult);

                DisplayMessage("Send Cmd: " + CalibrationCmd);
                DisplayMessage("Response: " + strResult);

                #endregion

                #region Get MDB Value

                DisplayMessage("After Audio Calibration.");

                // MAX98390L_TROOM
                strCmd = "adb shell mfg-tool -g MAX98390L_TROOM";
                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);
                if (string.IsNullOrWhiteSpace(strResult))
                {
                    strErrorMessage = "Fail to get MAX98390L_TROOM value.";
                    return false;    
                }
                MAX98390L_TROOM_AFTER = strResult.ToUpper();
                m_stUnitDeviceInfo.MAX98390L_TROOM_AFTER = MAX98390L_TROOM_AFTER;
                frmMain.m_stTestSaveData.TestRecord.MAX98390L_TROOM_AFTER = MAX98390L_TROOM_AFTER;
                DisplayMessage("MAX98390L_TROOM = " + MAX98390L_TROOM_AFTER);

                // MAX98390L_RDC
                bRes = clsProcess.ExcuteCmd(GetMDBCmd, 200, ref strResult);
                if (string.IsNullOrWhiteSpace(strResult))
                {
                    strErrorMessage = "Fail to get MAX98390L_RDC value.";
                    return false;
                }
                MAX98390L_RDC_AFTER = strResult.ToUpper();
                m_stUnitDeviceInfo.MAX98390L_RDC_AFTER = MAX98390L_RDC_AFTER;
                frmMain.m_stTestSaveData.TestRecord.MAX98390L_RDC_AFTER = MAX98390L_RDC_AFTER;
                DisplayMessage("MAX98390L_RDC = " + MAX98390L_RDC_AFTER);

                #endregion

                #region RDC Value Must Be Changed

                if (MAX98390L_RDC_AFTER == MAX98390L_RDC_BEFORE)
                {
                    DisplayMessage("MAX98390L_RDC value is not changed before and after calibration !!!");
                    return false;
                }

                #endregion

                frmMain.m_stTestSaveData.TestRecord.TestAudioCalibration = "Pass";
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestAudioCalibration Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestBarometerSensorOffset(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            string strCmd = "";
            string strResult = "";
            string OFFSETVALUE = "";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                // Offset Value
                OFFSETVALUE = GetTestItemParameter(strTestItem, "OffsetValue");
                if (string.IsNullOrWhiteSpace(OFFSETVALUE))
                {
                    strErrorMessage = "Fail to parse OFFSETVALUE parameter !";
                    return false;
                }
                DisplayMessage("Param OffsetValue: " + OFFSETVALUE);

                #endregion

                #region Check

                if (m_bIsWWAN == false) // WLAN
                {
                    DisplayMessage("This is WLAN Model, Skip to do Offset !!!");
                    return true;
                }

                if(m_bIsTDKBaroSensor == false) // TDK?
                {
                    DisplayMessage("This is not TDK Barometric Sensor, Skip to do Offset !!!");
                    return true;
                }

                #endregion

                frmMain.m_stTestSaveData.TestRecord.TestBarometerCalibration = "Fail";

                #region Write Offset Value

                DisplayMessage("Write Offset Value.");
                strCmd = string.Format("adb shell su 0  sensorK -sensor=pressure -calibration=1 -presscal={0}", OFFSETVALUE);
                DisplayMessage("Send Cmd:" + strCmd);
                bRes = clsProcess.ExcuteCmd(strCmd, 200);      
 
                #endregion

                #region Enable Immediately

                DisplayMessage("Enable Immediately.");
                strCmd = "adb shell ssc_drva_test -sensor=pressure -sample_rate=-1 -duration=5 -factory_test=2";
                DisplayMessage("Send Cmd:" + strCmd);
                bRes = clsProcess.ExcuteCmd(strCmd, 200);    

                #endregion

                #region CHECK OFFSET VALUE

                DisplayMessage("Check Offset Value.");
                strCmd = "adb shell su 0 cat /mnt/vendor/persist/sensors/registry/registry/icp201xx_0_platform.pressure.fac_cal.bias";
                DisplayMessage("Send Cmd:" + strCmd);
                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);

                if (strResult.Contains(OFFSETVALUE) == false)
                {
                    strErrorMessage = "Failed to check offset value !!!";
                    return false;
                }
                DisplayMessage("Write Offset Value Success.");

                m_stUnitDeviceInfo.OFFSET_VALUE = OFFSETVALUE;
                frmMain.m_stTestSaveData.TestRecord.BarometerOffsetValue = OFFSETVALUE;
                frmMain.m_stTestSaveData.TestRecord.TestBarometerCalibration = "Pass";

                #endregion   
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestBarometerSensorOffset Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestReboot(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
         
            string REBOOT_MODE = "";
            string APK_CMD = "";
            string Delay_SECOND = "";

            try
            {
                strTestItem = MethodBase.GetCurrentMethod().Name;

                #region Parse XML

                // Mode
                REBOOT_MODE = GetTestItemParameter(strTestItem, "Mode").ToUpper();
                if (string.IsNullOrWhiteSpace(REBOOT_MODE))
                {
                    strErrorMessage = "Fail to parse MODE parameter !";
                    return false;
                }
                DisplayMessage("Param Mode: " + REBOOT_MODE);

                // APK CMD
                APK_CMD = GetTestItemParameter(strTestItem, "ApkCmd");
                if (string.IsNullOrWhiteSpace(APK_CMD))
                {
                    strErrorMessage = "Fail to parse ApkCmd parameter !";
                    return false;
                }
                DisplayMessage("Param ApkCmd: " + APK_CMD);

                // Delay
                Delay_SECOND = GetTestItemParameter(strTestItem, "DelaySec");
                if (string.IsNullOrWhiteSpace(Delay_SECOND))
                {
                    strErrorMessage = "Fail to parse DelaySec parameter !";
                    return false;
                }
                double delayTime = Convert.ToDouble(Delay_SECOND);
                DisplayMessage("Param DelaySec: " + Delay_SECOND);

                #endregion

                #region REBOOT

                if (REBOOT_MODE == "REBOOT")    // adb reboot
                {
                    DisplayMessage("Use adb reboot to shut down device ...");
                    bRes = AdbRebootDevice();
                    clsUtil.Dly(delayTime);           // must add delay
                }
                else if (REBOOT_MODE == "APK")  // apk reboot, not real reboot indeed.
                {
                    DisplayMessage("Use Apk cmd to shut down device ...");
                    bRes = ApkRebootDevice(APK_CMD);
                    clsUtil.Dly(delayTime);  
                }
                else
                {
                    DisplayMessage("Undefined Reboot Mode !!!");
                    return false;
                }

                #endregion
            }
            catch (Exception ex)
            {
                strErrorMessage = "TestAutoChangeOver Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        public override bool TestEnd(ref string strErrorMessage)
        {
            strErrorMessage = "";
            //bool bFlag = false;
            //double dValue_AI0 = 0.0;
            //double dValue_AI1 = 0.0;

            try
            {
                #region Eject USB Pogopin
    
                //DisplayMessage("Eject USB Pogopin");
                //NISetDigital(0, 0, 0);  // DO0_0 L
                //NISetDigital(0, 1, 1);  // DO0_1 H
                //clsUtil.Dly(0.5);
 
                //for (int i = 0; i < 5; i++)
                //{
                //    dValue_AI0 = NIGetAnalog(0);    // AI0 > 3.0 V
                //    dValue_AI1 = NIGetAnalog(1);    // AI1 < 2.0 V
                //    if (dValue_AI0 > 3.0 && dValue_AI1 < 2.0)
                //    {
                //        bFlag = true;
                //        break;
                //    }
                //    else
                //    {
                //        bFlag = false;
                //        clsUtil.Dly(1.0);
                //        continue;
                //    }
                //}
                //if (bFlag == false)
                //{
                //    strErrorMessage = "Fail to Eject USB Pogopin !";
                //    return false;
                //}

                #endregion    
   

            }
            catch (Exception ex)
            {
                strErrorMessage = "TestAutoChangeOver Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        #region Obsolote

        //public override bool SendDataToMDCS()
        //{
        //    string strErrorMessage = "";

        //    try
        //    {        

        //    }
        //    catch(Exception ex)
        //    {
        //        strErrorMessage = "Exception:" + ex.Message;
        //        return false;
        //    }

        //    return true; 
        //}

        //public override bool SendMES()
        //{
        //    string strErrorMessage = "";

        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        strErrorMessage = "Exception:" + ex.Message;
        //        return false;
        //    }

        //    return true; 
        //}

        #endregion

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

        private void DisplayMessage(string message, string level = "INFO")
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

        #region Motor



        private bool OMRONMoveAbsolute(byte slave, int pos, int vel, uint acceleration, uint deceleration, ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;

            try
            {
                bRes = Program.g_mainForm.m_objEquipmentInitial.m_objOMORN.MoveAbsolute(slave, pos, vel, acceleration, deceleration, ref strErrorMessage);
                if (bRes == false)
                {
                    strErrorMessage = "Fail to move absolute position." + strErrorMessage;
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

        #endregion

        #region Public

        public override bool EjectUSBCable()
        {
            bool bFlag = false;
            double dValue_AI0 = 0.0;
            double dValue_AI1 = 0.0;

            try
            {
                #region Eject USB Pogopin

                //DisplayMessage("Eject USB Pogopin");
                NISetDigital(0, 0, 0);  // DO0_0 L
                NISetDigital(0, 1, 1);  // DO0_1 H
                clsUtil.Dly(0.5);

                #endregion

                #region Check Cylinder Position

                for (int i = 0; i < 5; i++)
                {
                    dValue_AI0 = NIGetAnalog(0);    // AI0 > 3.0 V
                    dValue_AI1 = NIGetAnalog(1);    // AI1 < 2.0 V
                    if (dValue_AI0 > 3.0 && dValue_AI1 < 2.0)
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
                    DisplayMessage("Fail to Eject USB Pogopin !");
                    return false;
                }

                #endregion       
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override bool InsertUSBCable()
        {
            bool bFlag = false;
            double dValue_AI0 = 0.0;
            double dValue_AI1 = 0.0;

            try
            {
                #region Insert USB Pogopin

                DisplayMessage("Insert USB Pogopin");
                NISetDigital(0, 0, 1);  // DO0_0 H
                NISetDigital(0, 1, 0);  // DO0_1 L
                clsUtil.Dly(0.5);

                #endregion

                #region Check Cylinder Position

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
                    DisplayMessage("Fail to Insert USB Pogopin !");
                    return false;
                }

                #endregion
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private

        private void InitNIPort()
        {
            NISetDigital(0, 0, 0);  // DO0_0 L
            NISetDigital(0, 1, 0);  // DO0_1 L
            //NISetDigital(0, 2, 0);  // DO0_2 L
            //NISetDigital(0, 3, 0);  // DO0_3 L
        }

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
            m_stUnitDeviceInfo.OSPN = "";
            m_stUnitDeviceInfo.Status = "";

            // MDB
            m_stUnitDeviceInfo.WLAN_MAC_ADDRESS = "";
            m_stUnitDeviceInfo.WLAN_AUX_MAC_ADDRESS = "";
            m_stUnitDeviceInfo.BLUETOOTH_DEVICE_ADDRESS = "";
            m_stUnitDeviceInfo.SECOND_BLUETOOTH_DEVICE_ADDRESS = "";

            // Before
            m_stUnitDeviceInfo.ACCEL_ZERO_OFFSET_BEFORE = "";
            m_stUnitDeviceInfo.ACCELEROMETER_CALIBRATION_BEFORE = "";
            m_stUnitDeviceInfo.GYRO_ZERO_OFFSET_BEFORE = "";
            m_stUnitDeviceInfo.GYROSCOPE_CALIBRATION_BEFORE = "";
            m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_BEFORE = "";
            m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_EXTEND_BEFORE = "";
            m_stUnitDeviceInfo.MAX98390L_TROOM_BEFORE = "";
            m_stUnitDeviceInfo.MAX98390L_RDC_BEFORE = "";

            // After
            m_stUnitDeviceInfo.ACCEL_ZERO_OFFSET_AFTER = "";
            m_stUnitDeviceInfo.ACCELEROMETER_CALIBRATION_AFTER = "";
            m_stUnitDeviceInfo.GYRO_ZERO_OFFSET_AFTER = "";
            m_stUnitDeviceInfo.GYROSCOPE_CALIBRATION_AFTER = "";
            m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_AFTER = "";
            m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_EXTEND_AFTER = "";
            m_stUnitDeviceInfo.MAX98390L_TROOM_AFTER = "";
            m_stUnitDeviceInfo.MAX98390L_RDC_AFTER = "";

            m_stUnitDeviceInfo.OFFSET_VALUE = "";
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
                        frmMain.m_stTestSaveData.TestRecord.ConfigurationNumber = dic_MFGData[key];
                        Logger.Info("EX_CONFIGURATION_NUMBER={0}", dic_MFGData[key]);
                    }
                    // Serial Number: 21352B4497
                    else if (key == "EX_SERIAL_NUMBER")
                    {
                        m_stUnitDeviceInfo.SN = dic_MFGData[key];
                        frmMain.m_stTestSaveData.TestRecord.SN = dic_MFGData[key];
                        Logger.Info("EX_SERIAL_NUMBER={0}", dic_MFGData[key]);
                    }
                    // Part Number: CT40P-L1N-27R11BE
                    else if (key == "EX_PART_NUMBER")
                    {
                        m_stUnitDeviceInfo.SKU = dic_MFGData[key];
                        frmMain.m_stTestSaveData.TestRecord.SKU = dic_MFGData[key];
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
                        m_stUnitDeviceInfo.Model = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.Model = dic_MFGData[key].ToUpper();
                        Logger.Info("MODEL_NUMBER={0}", dic_MFGData[key].ToUpper());
                    }
                    // Android License:
                    //else if (key == "ANDROID_LICENSE")
                    //{
                    //    objSaveData.TestRecord.AndroidLicenseKey = dic_MFGData[key];         
                    //}
                    // IMEI1: 990016300436126
                    else if (key == "IMEI_NUMBER")
                    {
                        m_stUnitDeviceInfo.IMEI = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.IMEI = dic_MFGData[key].ToUpper();
                        Logger.Info("IMEI_NUMBER={0}", dic_MFGData[key].ToUpper());
                    }
                    // MEID1: 99001630043612
                    else if (key == "MEID_NUMBER")
                    {
                        m_stUnitDeviceInfo.MEID = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.MEID = dic_MFGData[key].ToUpper();
                        Logger.Info("MEID_NUMBER={0}", dic_MFGData[key].ToUpper());
                    }
                    // IMEI2
                    else if (key == "IMEI_NUMBER_2")
                    {
                        m_stUnitDeviceInfo.IMEI2 = dic_MFGData[key].ToUpper();    
                        frmMain.m_stTestSaveData.TestRecord.IMEI2 = dic_MFGData[key].ToUpper();
                        Logger.Info("IMEI_NUMBER_2={0}", dic_MFGData[key].ToUpper());
                    }
                    // MEID2
                    else if (key == "MEID_NUMBER_2")
                    {
                        m_stUnitDeviceInfo.MEID2 = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.MEID2 = dic_MFGData[key].ToUpper();
                        Logger.Info("MEID_NUMBER_2={0}", dic_MFGData[key].ToUpper());
                    }
                    // HARDWARE_VERSION
                    else if (key == "HARDWARE_VERSION")
                    {
                        frmMain.m_stTestSaveData.TestRecord.HWVersion = dic_MFGData[key].ToUpper();
                        Logger.Info("HARDWARE_VERSION={0}", dic_MFGData[key].ToUpper());
                    }
                    // PCB_VENDOR
                    else if (key == "PCB_VENDOR")
                    {
                        frmMain.m_stTestSaveData.TestRecord.PCBAVendor = dic_MFGData[key].ToUpper();
                        Logger.Info("PCB_VENDOR={0}", dic_MFGData[key].ToUpper());
                    }

                    // Before Calibration MDB Value
                    // ACCEL_ZERO_OFFSET
                    else if (key == "ACCEL_ZERO_OFFSET")
                    {
                        m_stUnitDeviceInfo.ACCEL_ZERO_OFFSET_BEFORE = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.ACCEL_ZERO_OFFSET_BEFORE = dic_MFGData[key].ToUpper();
                        Logger.Info("ACCEL_ZERO_OFFSET={0}", dic_MFGData[key].ToUpper());
                    }
                    // ACCELEROMETER_CALIBRATION
                    else if (key == "ACCELEROMETER_CALIBRATION")
                    {
                        m_stUnitDeviceInfo.ACCELEROMETER_CALIBRATION_BEFORE = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.ACCELEROMETER_CALIBRATION_BEFORE = dic_MFGData[key].ToUpper();
                        Logger.Info("ACCELEROMETER_CALIBRATION={0}", dic_MFGData[key].ToUpper());
                    }
                    // GYRO_ZERO_OFFSET
                    else if (key == "GYRO_ZERO_OFFSET")
                    {
                        m_stUnitDeviceInfo.GYRO_ZERO_OFFSET_BEFORE = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.GYRO_ZERO_OFFSET_BEFORE = dic_MFGData[key].ToUpper();
                        Logger.Info("GYRO_ZERO_OFFSET={0}", dic_MFGData[key].ToUpper());
                    }
                    // GYROSCOPE_CALIBRATION
                    else if (key == "GYROSCOPE_CALIBRATION")
                    {
                        m_stUnitDeviceInfo.GYROSCOPE_CALIBRATION_BEFORE = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.GYROSCOPE_CALIBRATION_BEFORE = dic_MFGData[key].ToUpper();
                        Logger.Info("GYROSCOPE_CALIBRATION={0}", dic_MFGData[key].ToUpper());
                    }
                    // PROXIMITY_CALIBRATION
                    else if (key == "PROXIMITY_CALIBRATION")
                    {
                        m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_BEFORE = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.PROXIMITY_CALIBRATION_BEFORE = dic_MFGData[key].ToUpper();
                        Logger.Info("PROXIMITY_CALIBRATION={0}", dic_MFGData[key].ToUpper());
                    }
                    // PROXIMITY_CALIBRATION_EXTEND
                    else if (key == "PROXIMITY_CALIBRATION_EXTEND")
                    {
                        m_stUnitDeviceInfo.PROXIMITY_CALIBRATION_EXTEND_BEFORE = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.PROXIMITY_CALIBRATION_EXTEND_BEFORE = dic_MFGData[key].ToUpper();
                        Logger.Info("PROXIMITY_CALIBRATION_EXTEND={0}", dic_MFGData[key].ToUpper());
                    }
                    // MAX98390L_TROOM
                    else if (key == "MAX98390L_TROOM")
                    {
                        m_stUnitDeviceInfo.MAX98390L_TROOM_BEFORE = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.MAX98390L_TROOM_BEFORE = dic_MFGData[key].ToUpper();
                        Logger.Info("MAX98390L_TROOM={0}", dic_MFGData[key].ToUpper());
                    }
                    // MAX98390L_RDC
                    else if (key == "MAX98390L_RDC")
                    {
                        m_stUnitDeviceInfo.MAX98390L_RDC_BEFORE = dic_MFGData[key].ToUpper();
                        frmMain.m_stTestSaveData.TestRecord.MAX98390L_RDC_BEFORE = dic_MFGData[key].ToUpper();
                        Logger.Info("MAX98390L_RDC={0}", dic_MFGData[key].ToUpper());
                    }
                }

                #endregion

                #region Check MFG Data

                // SN
                string strSN = m_stUnitDeviceInfo.SN;
                frmMain.m_strSN = m_stUnitDeviceInfo.SN;
                if (string.IsNullOrWhiteSpace(strSN) || (strSN.Length != 10))
                {
                    strErrorMessage = "Read mfg data fail: Invalid SN.";
                    return false;
                }
                // SKU
                string strSKU = m_stUnitDeviceInfo.SKU;
                frmMain.m_strSKU = m_stUnitDeviceInfo.SKU;
                if (string.IsNullOrWhiteSpace(strSKU))
                {
                    strErrorMessage = "Read mfg data fail: Invalid SKU.";
                    return false;
                }
                // Truncate Model
                int index = strSKU.IndexOf("-");
                string skuModel = strSKU.Substring(0, index).Trim();

                // MODEL
                string strModel = m_stUnitDeviceInfo.Model;
                if (string.IsNullOrWhiteSpace(strModel))
                {
                    strErrorMessage = "Read mfg data fail: Invalid Model.";
                    return false;
                }
                if (strModel != skuModel)
                {
                    strErrorMessage = "Read MDB Model Not Match SKU Model !!!";
                    return false;
                }
                if (strModel.Contains(frmMain.m_strModel) == false)  // take care !!!
                {
                    MessageBox.Show("The Product Not Match the Production Line That You Selected !!!");
                    return false;
                }
                frmMain.m_strModel = strModel;   // Confirm Device Model

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

        private bool ReadOSVersion(ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;
            string strCmd = "";
            string strResult = "";

            try
            {
                DisplayMessage("Read Andriod OS Version.");
                strCmd = "adb shell su 0 getprop ro.build.display.id";
                DisplayMessage("Send CMD: " + strCmd);

                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);      
                DisplayMessage("Res: " + strResult);
                if (string.IsNullOrWhiteSpace(strResult))
                {
                    strErrorMessage = "Fail to read andriod OS version.";
                    return false;
                }

                frmMain.m_stTestSaveData.TestRecord.AndroidOSVersion = strResult;    
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool SWVersionCheck(ref string strErrorMessage)
        {
            strErrorMessage = "";
            string SoftwareNumber = Program.g_strToolNumber.ToUpper();
            string SoftwareVersion = Program.g_strToolRev.ToUpper();
            string strSKU = m_stUnitDeviceInfo.SKU;
            //string strKeyWord = "";

            try
            {
                //bool bFlag = false;
                ApiResult result = ScanSheet.Get(strSKU);
                if (result.Status == 0)
                {
                    //string Station = "";
                    //string BarcodeValue = "";
                    //string[] strArray;
                    string sJasonStr = JsonConvert.SerializeObject(result);
                    ScanSheetRes res = JsonConvert.DeserializeObject<ScanSheetRes>(sJasonStr);

                    string strTestSoftwareControl = "";
                    string strTestSoftwareNumber = "";
                    string strTestSoftwareRev = "";

                    #region Get Data

                    if (res.Data.Count > 0)
                    {
                        var item = res.Data[0];
                        strTestSoftwareControl = item.TestSoftwareRevControl.ToString().Trim();
                        strTestSoftwareNumber = item.TestSoftware.ToString().Trim().ToUpper();
                        strTestSoftwareRev = item.TestSoftwareRev.ToString().Trim().ToUpper();
                    }
                    else
                    {
                        strErrorMessage = "Don't found any ScanSheet. SKU:" + strSKU;
                        return false;
                    }

                    #endregion

                    #region Check Data

                    if (string.IsNullOrWhiteSpace(strTestSoftwareControl) || string.IsNullOrWhiteSpace(strTestSoftwareNumber) || string.IsNullOrWhiteSpace(strTestSoftwareRev))
                    {
                        strErrorMessage = "Get ScanSheet Data fail.";
                        return false;
                    }

                    frmMain.m_stTestSaveData.TestRecord.SoftwareVersionControl = strTestSoftwareControl;

                    if (strTestSoftwareControl == "1")
                    {
                        // Check
                        if (strTestSoftwareNumber != SoftwareNumber)
                        {
                            strErrorMessage = "Software Number Not Matched !!!";
                            return false;
                        }
                        if (strTestSoftwareRev != SoftwareVersion)
                        {
                            strErrorMessage = "Software Version Not Matched !!!";
                            return false;
                        }
                    }
                    else
                    {
                        // Not Check
                        DisplayMessage("TestSoftwareRevControl = 0, No Need to Check Software Version ...");
                    }

                    #endregion
                }
                else
                {
                    strErrorMessage = string.Format("FailMessage:{0}, Status:{1}", result.Message, result.Status.ToString());
                    DisplayMessage("Get ScanSheet fail." + strErrorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool ScreenOffAction(ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;
            string strCmd = "";
            string strResult = "";

            try
            {
                DisplayMessage("Check Screen Status First.");
                strCmd = "adb shell dumpsys power | find \"Display Power: state=\"";

                bRes = clsProcess.ExcuteCmd(strCmd, 2000, ref strResult);
                if (strResult.Contains("state=OFF"))
                {
                    DisplayMessage("The Screen Already Turn off !");
                    return true;
                }

                // The Screen is ON
                DisplayMessage("The Screen Currently is ON, Ready to Turn Off");
                strCmd = "adb shell input keyevent 26";
              
                // Turn off
                bRes = clsProcess.ExcuteCmd(strCmd, 1000, ref strResult);
                DisplayMessage("Send Cmd: " + strCmd);
                clsUtil.Dly(0.5);

                // Check status
                strCmd = "adb shell dumpsys power | find \"Display Power: state=\"";
                DisplayMessage("Check Screen Status, Send Cmd: " + strCmd);
                bRes = clsProcess.ExcuteCmd(strCmd, 2000, ref strResult);

                DisplayMessage("Response: " + strResult);       
                if (strResult.IndexOf("state=OFF", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception: " + ex.Message;
                return false;
            }
        }

        private bool ScreenOnAction(ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;
            string strCmd = "";
            string strResult = "";

            try
            {
                DisplayMessage("Check Screen Status First.");
                strCmd = "adb shell dumpsys power | find \"Display Power: state=\"";

                bRes = clsProcess.ExcuteCmd(strCmd, 2000, ref strResult);
              
                if (strResult.IndexOf("state=ON", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    DisplayMessage("The Screen Already Turn ON !");
                    return true;
                }

                // The Screen is Off
                DisplayMessage("The Screen Currently is Off, Ready to Turn On");
                strCmd = "adb shell input keyevent 26";
               
                // Turn on
                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);
                DisplayMessage("Send Cmd: " + strCmd);
                clsUtil.Dly(1.0);

                // Check status
                strCmd = "adb shell dumpsys power | find \"Display Power: state=\"";
                DisplayMessage("Check Screen Status, Send Cmd: " + strCmd);
                bRes = clsProcess.ExcuteCmd(strCmd, 2000, ref strResult);
                DisplayMessage("Response: " + strResult);

                if (strResult.IndexOf("state=ON", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception: " + ex.Message;
                return false;
            }
        }

        private bool MoveDamBoardUp(double dDistance, double dHome, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                int iStep = 0;
                iStep = (int)((dHome - dDistance) / 0.012 *10);

                if (OMRONMoveAbsolute(1, iStep, 14000, 40000, 40000, ref strErrorMessage) == false)
                {
                    return false;
                }

                m_dCurrentDamBoardPosition = dDistance;      
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool MoveDamBoardDown(double dDistance, double dHome, ref string strErrorMessage)
        {
            strErrorMessage = "";

            if (m_dCurrentDamBoardPosition == dDistance)
            {
                return true;
            }

            try
            {
                int iStep = 0;
                iStep = (int)((dHome - dDistance) / 0.012 * 10);

                if (OMRONMoveAbsolute(1, iStep, 14000, 40000, 40000, ref strErrorMessage) == false)
                {
                    return false;
                }

                m_dCurrentDamBoardPosition = dDistance;         
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;  
            }

            return true;
        }

        private bool CheckSensorList(ref string strErrorMessage)
        { 
            strErrorMessage = "";
            bool bFlag = false;
            bool bPSensor = false;
            bool bGSensor = false;
            //bool bBarometerSensor = false;

            string strFilePath = "";

            try
            {
                #region Check ADB Connect

                if (CheckADBConnected(10) == false)
                {
                    strErrorMessage = "Check adb connect failed !!!";               
                    return false;
                }

                #endregion

                #region Delete Local File

                strFilePath = Application.StartupPath + @"\sensor_list";
                if (File.Exists(strFilePath))
                {
                    File.Delete(strFilePath);
                    clsUtil.Dly(0.5);
                    if (File.Exists(strFilePath))
                    {
                        strErrorMessage = "Delete Local Sensor List File fail.";
                        return false;
                    }
                }

                #endregion

                #region Get Sensor List

                for (int i = 0; i < 3; i++)
                {
                    if (GetSensorList(ref strErrorMessage) == false)
                    {
                        strErrorMessage = "Fail to execute GetSensorList.bat";
                        bFlag = false;
                        clsUtil.Dly(1.0);
                        continue;
                    }

                    clsUtil.Dly(2.0);
                    // Check Local sensor_list file
                    if (File.Exists(strFilePath) == false)
                    {
                        clsUtil.Dly(2.0);
                        if (File.Exists(strFilePath) == false)
                        {
                            strErrorMessage = "Fail to check local sensor_list file exist.";
                            bFlag = false;
                            continue;
                        }         
                    }   
      
                    bFlag = true;
                    break;      
                }
                if (bFlag == false)
                {
                    return false;
                }

                #endregion

                #region Check Sensor Whether Exist In Sensor_List

                if (CheckSensorExist(strFilePath, ref bPSensor, ref bGSensor, ref strErrorMessage) == false)
                {
                    return false;
                }

                if (bPSensor == false)
                {
                    strErrorMessage = "This device doesn't have PSensor !!!";
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }
            
                if (bGSensor == false)
                {
                    strErrorMessage = "This device doesn't have GSensor !!!";
                    DisplayMessage(strErrorMessage, "ERROR");
                    return false;
                }
                //if (bBarometerSensor == false)
                //{
                //    strErrorMessage = "This device doesn't have Barometer Sensor !!!";
                //    return false;
                //}
                DisplayMessage("PSensor and GSensor Exist.");

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true; 
        }

        private bool GetSensorList(ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;
            string strResult = "";
            string batchFile = "GetSensorList.bat";

            try
            {
                DisplayMessage("Execute Batch File: " + batchFile);
                bRes = clsProcess.ExcuteCmd(batchFile, 2000, ref strResult);
                if (strResult.Contains("sensor_list") == false)
                {
                    strErrorMessage = "Fail to get the sensor_list";
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

        private bool CheckSensorExist(string strFilePath, ref bool IsPSensorExist, ref bool IsGSensorExist, ref string strErrorMessage)
        {
            strErrorMessage = "";
            IsPSensorExist = false;
            IsGSensorExist = false;

            //bool IsBaroSensorExist = false;
       
            try
            { 
                DisplayMessage("Check PSensor and GSensor Exist");
                string[] FileArray = File.ReadAllLines(strFilePath);

                foreach (var line in FileArray)
                {
                    if (line.Contains(PSensorName))
                    {
                        IsPSensorExist = true;
                        break;     
                    } 
                }

                foreach (var line in FileArray)
                {
                    if (line.Contains(GSensorName))
                    {
                        IsGSensorExist = true;
                        break;
                    }
                }

                // Check Barometer Vendor Whether TDK
                if (m_bIsWWAN == true)
                {
                    string Line = "";
                    string Vendor = "";
                    for (int i = 0; i < FileArray.Length; i++)
                    {
                        Line = FileArray[i];
                        if (Line.Contains(BarometerName))
                        {
                            //IsBaroSensorExist = true;
                            Vendor = FileArray[i + 1];
                            if (Vendor.Contains("TDK"))
                            {
                                m_bIsTDKBaroSensor = true;
                                break;
                            }
                        }
                    }   
                } 
          
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool CheckNearPositionFunction(string strNearPosition, string PSENSOR_GET_LOG, ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;
            bool bFlag = false;
            string strCmd = "";
            string strResult = "";

            string NEARSPEC = "0.000000";

            try
            {
                DisplayMessage("Check PSensor Near-Position Function.");

                #region  Get PROXIMITY_CALIBRATION Value

                strCmd = "adb shell su 0 mfg-tool -g PROXIMITY_CALIBRATION";
                bRes = clsProcess.ExcuteCmd(strCmd, 100, ref strResult);
                DisplayMessage("Before Check PSensor Near Function, PROXIMITY_CALIBRATION=", strResult);

                #endregion

                #region Move Dame Board TO Near-Position

                DisplayMessage("Move Dame Board to Near-Position, Make sure distance is: " + strNearPosition + "cm.");
                double dDistance = Convert.ToDouble(strNearPosition);
                double dHome = Convert.ToDouble(clsConfigHelper.servoMotor.Home);
                //double dHome = Convert.ToDouble(GetTestConfig("MOTOR", "Home"));

                if (MoveDamBoardDown(dDistance, dHome, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Fail move dame board to near position:" + strErrorMessage;
                    return false;
                }

                #endregion

                clsUtil.Dly(3.0);   // Add delay to make sure the dame board go to the position

                #region Get PSensor Calibration LOG And Compare Spec

                for (int i = 0; i < 3; i++)
                {
                    if (GetPSensorLog(PSENSOR_GET_LOG, ref strResult) == false)
                    {
                        strErrorMessage = "Fail to get PSensor calibration log.";
                        bFlag = false;
                        clsUtil.Dly(3.0);
                        continue;
                    }

                    if (CheckPSensorValueMatchSpec(strResult, NEARSPEC) == false)
                    {
                        strErrorMessage = "Test PSensor Near Function Failed.";
                        bFlag = false;
                        clsUtil.Dly(3.0);
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
                    DisplayMessage(strErrorMessage);
                    return false;
                }

                DisplayMessage("Check PSensor Near Position Function Success.");
                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;     
        }

        private bool CheckFarPositionFunction(string strFarPosition, string PSENSOR_GET_LOG, ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;
            bool bFlag = false;
            string strCmd = "";
            string strResult = "";

            string FARSPEC = "5.000000";

            try
            {
                DisplayMessage("Check PSensor Far-Position Function.");

                #region  Get PROXIMITY_CALIBRATION Value

                strCmd = "adb shell su 0 mfg-tool -g PROXIMITY_CALIBRATION";
                bRes = clsProcess.ExcuteCmd(strCmd, 100, ref strResult);
                DisplayMessage("Before Check PSensor Far Function, PROXIMITY_CALIBRATION=", strResult);

                #endregion

                #region Move Dame Board TO Far-Position

                DisplayMessage("Move Dame Board to Far-Position, Make sure distance is: " + strFarPosition + "cm.");
                double dDistance = Convert.ToDouble(strFarPosition);
                //double dHome = Convert.ToDouble(GetTestConfig("MOTOR", "Home"));
                double dHome = Convert.ToDouble(clsConfigHelper.servoMotor.Home);

                if (MoveDamBoardUp(dDistance, dHome, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Fail move dam board to Far position:" + strErrorMessage;
                    return false;
                }

                #endregion

                clsUtil.Dly(3.0);   // Add delay to make sure the dame board go to the position

                #region Get PSensor Calibration LOG And Compare Spec

                for (int i = 0; i < 3; i++)
                {
                    if (GetPSensorLog(PSENSOR_GET_LOG, ref strResult) == false)
                    {
                        strErrorMessage = "Fail to get PSensor calibration log.";
                        bFlag = false;
                        clsUtil.Dly(3.0);
                        continue;
                    }

                    if (CheckPSensorValueMatchSpec(strResult, FARSPEC) == false)
                    {
                        strErrorMessage = "Test PSensor Far Function Failed.";
                        bFlag = false;
                        clsUtil.Dly(3.0);
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
                    DisplayMessage(strErrorMessage);
                    return false;
                }

                DisplayMessage("Check PSensor Far Position Function Success.");
                #endregion
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool GetPSensorLog(string strCmd, ref string strResult)
        {
            strResult = "";
            bool bRes = false;

            DisplayMessage("Get PSensor Calibration Log.");
            DisplayMessage("Send Cmd: " + strCmd);

            bRes = clsProcess.ExcuteCmd(strCmd, 6000, ref strResult);   // 6s

            Logger.Info("Result: " + strResult);
            if (string.IsNullOrWhiteSpace(strResult))
            {
                return false;
            }

            return true;
        }

        private bool CheckPSensorValueMatchSpec(string strResult, string strSpec)
        {
            string strErrorMessage = "";
            bool bFlag =false;

            try
            {
                List<string> ValueList = new List<string>();
                string[] LineArray = strResult.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string line in LineArray)
                {
                    string temp = line.Trim();
                    string[] TempArray = temp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < TempArray.Length; i++)
                    {
                        if (i == 3)
                        {
                            ValueList.Add(TempArray[i]);
                        }
                    }
                }

                if (ValueList.Count != 0)
                {
                    for (int j = 0; j < ValueList.Count; j++ )
                    {
                        ValueList[j] = ValueList[j].Trim();
                        if (ValueList[j].CompareTo(strSpec) != 0)
                        {
                            strErrorMessage = string.Format("PSensor Log Value = {0}, Spec = {1}, Not Matched !!!", ValueList[j], strSpec);
                            bFlag = false;
                            continue;  
                        }
                        else
                        {
                            DisplayMessage(string.Format("PSensor Log Value = {0}, is Matched with Spec !!!", ValueList[j]));
                            bFlag = true;
                            break;
                        }
                    }
                }
                else
                {
                    strErrorMessage = "Get PSensor Log Value is Empty !!!";
                    bFlag = false;          
                }
                if (bFlag == false)
                {
                    DisplayMessage(strErrorMessage);
                    return false;
                }
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception: " + ex.Message;
                return false;
            }

            return true;  
        }

        private bool AdbRebootDevice()
        {
            string strCmd = "";
            bool bRes = false;

            try
            {
                strCmd = "adb root";
                bRes = clsProcess.ExcuteCmd(strCmd, 200);

                strCmd = "adb wait-for-device";
                bRes = clsProcess.ExcuteCmd(strCmd, 200);

                strCmd = "adb shell reboot -p";
                bRes = clsProcess.ExcuteCmd(strCmd, 200);
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        private bool ApkRebootDevice(string strCmd)
        {
            bool bRes = false;
            string Cmd = "";
            string strResult = "";

            try
            {
                Cmd = "adb root";
                bRes = clsProcess.ExcuteCmd(Cmd, 200);

                Cmd = "adb wait-for-device";
                bRes = clsProcess.ExcuteCmd(Cmd, 200);

                // APK CMD
                bRes = clsProcess.ExcuteCmd(strCmd, 200, ref strResult);
                DisplayMessage("Send Cmd: " + strCmd);
                DisplayMessage("Response: " + strResult);
                if (strResult.IndexOf("Starting: Intent", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    DisplayMessage("Execute apk cmd to reboot fail !!!");
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
