﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    public class clsMDCS
    {
        #region Variable

        private MDCS.MDCSDeviceSetup m_obj_MDCSDevice;
        private string m_str_ServerName;
        private string m_str_DeviceName;
        private string m_str_TestType;
        private bool m_b_ModeProduction;
        private TestSaveData m_obj_Data;

        #endregion

        #region Construct

        public clsMDCS()
        {
            m_obj_MDCSDevice = new MDCS.MDCSDeviceSetup();

            m_str_ServerName = "http://hsm-mdcsws-ch3u.honeywell.com/MDCSWebService/MDCSService.asmx";
            m_obj_MDCSDevice.ServerURL = m_str_ServerName;

            m_str_DeviceName = "";
            m_obj_MDCSDevice.DeviceName = m_str_DeviceName;

            m_str_TestType = MDCS.MDCSTestModes.PRODUCTION;
            m_obj_MDCSDevice.TestType = m_str_TestType;

            m_b_ModeProduction = true;
            m_obj_Data = new TestSaveData();
        }

        ~clsMDCS()
        {
        }

        #endregion

        #region Property

        public string ServerName
        {
            get
            {
                return m_str_ServerName;
            }
            set
            {
                m_str_ServerName = value;
            }
        }

        public string DeviceName
        {
            get
            {
                return m_str_DeviceName;
            }
            set
            {
                m_str_DeviceName = value;
            }
        }

        public string TestType
        {
            get
            {
                return m_str_TestType;
            }
            set
            {
                m_str_TestType = value;
            }
        }

        public bool UseModeProduction
        {
            get
            {
                return m_b_ModeProduction;
            }
            set
            {
                m_b_ModeProduction = value;
            }
        }

        public TestSaveData p_TestData
        {
            get
            {
                return m_obj_Data;
            }
            set
            {
                m_obj_Data = value;
            }
        }

        #endregion

        #region Function

        public bool GetMDCSVariable(string str_DeviceName, string str_VariableName, string str_SN, ref string str_Value, ref string str_ErrorMessage)
        {
            str_Value = "";
            str_ErrorMessage = "";

            try
            {
                m_obj_MDCSDevice.ServerURL = m_str_ServerName;
                m_obj_MDCSDevice.DeviceName = str_DeviceName;
                m_obj_MDCSDevice.TestType = MDCS.MDCSTestModes.PRODUCTION;

                str_Value = m_obj_MDCSDevice.GetMDCSVariable(str_DeviceName, str_VariableName, str_SN);

                if (str_Value == "")
                {
                    str_ErrorMessage = "Failed to get variable value.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                str_ErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        public bool GetMDCSVariable(string str_VariableName, string str_SN, ref string str_Value, ref string str_ErrorMessage)
        {
            str_Value = "";
            str_ErrorMessage = "";

            try
            {
                m_obj_MDCSDevice.ServerURL = m_str_ServerName;
                m_obj_MDCSDevice.DeviceName = m_str_DeviceName;
                m_obj_MDCSDevice.TestType = MDCS.MDCSTestModes.PRODUCTION;

                str_Value = m_obj_MDCSDevice.GetMDCSVariable(m_str_DeviceName, str_VariableName, str_SN);

                if (str_Value == "")
                {
                    str_ErrorMessage = "Failed to get variable value.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                str_ErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        public bool SerialNumberRequest(ref string str_SN, ref string str_ErrorMessage)
        {
            str_SN = "";
            str_ErrorMessage = "";

            try
            {
                m_obj_MDCSDevice.ServerURL = m_str_ServerName;
                m_obj_MDCSDevice.DeviceName = m_str_DeviceName;
                m_obj_MDCSDevice.TestType = MDCS.MDCSTestModes.PRODUCTION;

                str_SN = m_obj_MDCSDevice.SerialNumber_Request("F0705", "B", "F0705", "F0705", 1);

                if (str_SN.Length != 10)
                {
                    str_ErrorMessage = "Failed to SerialNumberRequest.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                str_ErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        public bool SerialNumberRequest_Standard(ref string str_SN, ref string str_ErrorMessage)
        {
            str_SN = "";
            str_ErrorMessage = "";

            try
            {
                m_obj_MDCSDevice.ServerURL = m_str_ServerName;
                m_obj_MDCSDevice.DeviceName = m_str_DeviceName;
                m_obj_MDCSDevice.TestType = MDCS.MDCSTestModes.PRODUCTION;

                str_SN = m_obj_MDCSDevice.SerialNumber_Standard_Request("F0705", "F0705", "F0705", 1);

                if (str_SN.Length != 10)
                {
                    str_ErrorMessage = "Failed to SerialNumber_Standard_Request.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                str_ErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        public bool SendQCDCS(string str_Serial, string str_Model, string str_BFModel, string str_QADLine, string str_Software, ref string str_ErrorMessage)
        {
            str_ErrorMessage = "";

            try
            {
                MDCS.QCDCS obj = new MDCS.QCDCS();
                MDCS.QCDCSDataInfo stData = new MDCS.QCDCSDataInfo();

                stData.Serial = str_Serial;
                stData.Model = str_Model;
                stData.BF_Model = str_BFModel;
                stData.QAD_Line = str_QADLine;
                stData.Software = str_Software;
                stData.FA_Status = 2;
                stData.Pre_Status = 2;
                stData.Post_Status = 2;
                stData.XFR_Disp = 0;

                if (obj.InsertQCDCS(stData, ref str_ErrorMessage) == false)
                {
                    str_ErrorMessage = "Failed to insert QCDCS.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                str_ErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        public bool SendMDCSData(ref string str_ErrorMessage)
        {
            try
            {
                m_obj_MDCSDevice.ServerURL = m_str_ServerName;
                m_obj_MDCSDevice.DeviceName = m_str_DeviceName;
                if (m_b_ModeProduction == true)
                {
                    m_obj_MDCSDevice.TestType = MDCS.MDCSTestModes.PRODUCTION;
                }
                else
                {
                    m_obj_MDCSDevice.TestType = MDCS.MDCSTestModes.TESTMODE;
                }

                // Key
                m_obj_MDCSDevice.Key = m_obj_Data.TestRecord.SN;

                // String Variable
                m_obj_MDCSDevice.AddStringVariable("FailCode", m_obj_Data.TestResult.TestFailCode.ToString());
                m_obj_MDCSDevice.AddStringVariable("FailMessage", m_obj_Data.TestResult.TestFailMessage);
                m_obj_MDCSDevice.AddStringVariable("ToolNumber", m_obj_Data.TestRecord.ToolNumber);
                m_obj_MDCSDevice.AddStringVariable("ToolRev", m_obj_Data.TestRecord.ToolRev);
                m_obj_MDCSDevice.AddStringVariable("SN", m_obj_Data.TestRecord.SN);
                m_obj_MDCSDevice.AddStringVariable("Model", m_obj_Data.TestRecord.Model);
                m_obj_MDCSDevice.AddStringVariable("SKU", m_obj_Data.TestRecord.SKU);
                m_obj_MDCSDevice.AddStringVariable("IMEI", m_obj_Data.TestRecord.IMEI);
                m_obj_MDCSDevice.AddStringVariable("MEID", m_obj_Data.TestRecord.MEID);
                m_obj_MDCSDevice.AddStringVariable("IMEI2", m_obj_Data.TestRecord.IMEI2);
                m_obj_MDCSDevice.AddStringVariable("MEID2", m_obj_Data.TestRecord.MEID2);
                m_obj_MDCSDevice.AddStringVariable("EID", m_obj_Data.TestRecord.EID);
                m_obj_MDCSDevice.AddStringVariable("WorkOrder", m_obj_Data.TestRecord.WorkOrder);
                m_obj_MDCSDevice.AddStringVariable("Vendor", m_obj_Data.TestRecord.PCBAVendor);
                m_obj_MDCSDevice.AddStringVariable("HWVersion", m_obj_Data.TestRecord.HWVersion);
                m_obj_MDCSDevice.AddStringVariable("ConfigNumber", m_obj_Data.TestRecord.ConfigurationNumber);
                m_obj_MDCSDevice.AddStringVariable("AudioPAName", m_obj_Data.TestRecord.AudioPAName);
                m_obj_MDCSDevice.AddStringVariable("BarometerOffsetValue", m_obj_Data.TestRecord.BarometerOffsetValue);
                m_obj_MDCSDevice.AddStringVariable("SoftwareVersionControl", m_obj_Data.TestRecord.SoftwareVersionControl);
                m_obj_MDCSDevice.AddStringVariable("AndroidOS", m_obj_Data.TestRecord.AndroidOSVersion);

                // TestItem Result
                m_obj_MDCSDevice.AddStringVariable("GSensorCalibration", m_obj_Data.TestRecord.TestGSensorCalibration);
                m_obj_MDCSDevice.AddStringVariable("GYROSensorCalibration", m_obj_Data.TestRecord.TestGYROSensorCalibration);
                m_obj_MDCSDevice.AddStringVariable("PSensorCalibration", m_obj_Data.TestRecord.TestPSensorCalibration);
                m_obj_MDCSDevice.AddStringVariable("PSensorFunction", m_obj_Data.TestRecord.TestPSensorFunction);
                m_obj_MDCSDevice.AddStringVariable("AudioCalibration", m_obj_Data.TestRecord.TestAudioCalibration);
                m_obj_MDCSDevice.AddStringVariable("BarometerCalibration", m_obj_Data.TestRecord.TestBarometerCalibration);
              
                // MDB Value
                m_obj_MDCSDevice.AddStringVariable("ACCEL_ZERO_OFFSET_AFTER", m_obj_Data.TestRecord.ACCEL_ZERO_OFFSET_AFTER);
                m_obj_MDCSDevice.AddStringVariable("ACCEL_ZERO_OFFSET_BEFORE", m_obj_Data.TestRecord.ACCEL_ZERO_OFFSET_BEFORE);
                m_obj_MDCSDevice.AddStringVariable("ACCELEROMETER_CALIBRATION_AFTER", m_obj_Data.TestRecord.ACCELEROMETER_CALIBRATION_AFTER);
                m_obj_MDCSDevice.AddStringVariable("ACCELEROMETER_CALIBRATION_BEFORE", m_obj_Data.TestRecord.ACCELEROMETER_CALIBRATION_BEFORE);
                m_obj_MDCSDevice.AddStringVariable("GYRO_ZERO_OFFSET_AFTER", m_obj_Data.TestRecord.GYRO_ZERO_OFFSET_AFTER);
                m_obj_MDCSDevice.AddStringVariable("GYRO_ZERO_OFFSET_BEFORE", m_obj_Data.TestRecord.GYRO_ZERO_OFFSET_BEFORE);
                m_obj_MDCSDevice.AddStringVariable("GYROSCOPE_CALIBRATION_AFTER", m_obj_Data.TestRecord.GYROSCOPE_CALIBRATION_AFTER);
                m_obj_MDCSDevice.AddStringVariable("GYROSCOPE_CALIBRATION_BEFORE", m_obj_Data.TestRecord.GYROSCOPE_CALIBRATION_BEFORE);
                m_obj_MDCSDevice.AddStringVariable("PROXIMITY_CALIBRATION_AFTER", m_obj_Data.TestRecord.PROXIMITY_CALIBRATION_AFTER);
                m_obj_MDCSDevice.AddStringVariable("PROXIMITY_CALIBRATION_BEFORE", m_obj_Data.TestRecord.PROXIMITY_CALIBRATION_BEFORE);
                m_obj_MDCSDevice.AddStringVariable("PROXIMITY_CALIBRATION_EXTEND_AFT", m_obj_Data.TestRecord.PROXIMITY_CALIBRATION_EXTEND_AFTER);
                m_obj_MDCSDevice.AddStringVariable("PROXIMITY_CALIBRATION_EXTEND_BEF", m_obj_Data.TestRecord.PROXIMITY_CALIBRATION_EXTEND_BEFORE);
                m_obj_MDCSDevice.AddStringVariable("MAX98390L_RDC_AFTER", m_obj_Data.TestRecord.MAX98390L_RDC_AFTER);
                m_obj_MDCSDevice.AddStringVariable("MAX98390L_RDC_BEFORE", m_obj_Data.TestRecord.MAX98390L_RDC_BEFORE);
                m_obj_MDCSDevice.AddStringVariable("MAX98390L_TROOM_AFTER", m_obj_Data.TestRecord.MAX98390L_TROOM_AFTER);
                m_obj_MDCSDevice.AddStringVariable("MAX98390L_TROOM_BEFORE", m_obj_Data.TestRecord.MAX98390L_TROOM_BEFORE);

                // Numeric Variable
                //m_obj_MDCSDevice.AddNumericVariable("TestTotalTime", m_obj_Data.TestRecord.TestTotalTime);
                m_obj_MDCSDevice.AddStringVariable("TestTotalTime", m_obj_Data.TestRecord.TestTotalTime);

                // Send Data
                if (m_obj_MDCSDevice.SendMDCSTestRecord() == false)
                {
                    str_ErrorMessage = "Failed to send data to MDCS.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                str_ErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        #endregion

    }
}
