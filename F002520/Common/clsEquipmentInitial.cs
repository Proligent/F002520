using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace F002520
{
    public class clsEquipmentInitial
    {
        // DAQ
        public clsNI6251 m_objNIDAQ = new clsNI6251();
        public clsJYDAMDAQ m_objJYDAMDAQ = new clsJYDAMDAQ();

        // OMORN Motor
        private modbus_hon.ModbusOp.serialSettings m_modbusSetting = new modbus_hon.ModbusOp.serialSettings();
        public  modbus_hon.ModbusOp m_objOMORN = new modbus_hon.ModbusOp(OMLib.PRODUCT.AR); 

        // Panasonic Motor


        #region 

        public static bool LoadEquipment(ref string strErrorMessage)
        {
            strErrorMessage = "";

            if (clsConfigHelper.LoadEquipmentParams(ref strErrorMessage) == false)
            {
                strErrorMessage = "Failed to load EquipmentConfig.xml file: " + strErrorMessage;
                return false;
            }

            return true;
        }

        #endregion


        #region HardWare Initial

        // DAQ
        public bool InitNIDAQ(string devicName)
        {
            if (m_objNIDAQ.Init(devicName) == false)
            {
                return false;
            }

            if (m_objNIDAQ.Reset() == false)
            {
                return false;
            }

            return true;
        }

        public bool InitJYDAMDAQ()
        {

            return true;
        }

     
        // Motor
        public bool InitOMORNMotor(ref string strErrorMessage)
        {
            strErrorMessage = "";

            // Modbus Comport Setting
            m_modbusSetting.PortNum = clsConfigHelper.servoMotor.PortNum;
            m_modbusSetting.Baud = clsConfigHelper.servoMotor.BaudRate;
            m_modbusSetting.DataBits = clsConfigHelper.servoMotor.DataBits;
            m_modbusSetting.Parity = clsConfigHelper.servoMotor.Parity;
            m_modbusSetting.StopBits = StopBits.One;

            // Open Port
            if (m_objOMORN.PortOpen(m_modbusSetting, ref strErrorMessage) == false)
            {
                strErrorMessage = "Fail to open Modbus Port: " + strErrorMessage;
                return false;
            }

            // Get Alarm
            string strResponse = "";
            if (m_objOMORN.GetAlarm(ref strResponse, 1, ref strErrorMessage) == false)
            {
                strErrorMessage = "Fail to Get Alarm: " + strErrorMessage;
                return false;
            }
            if (!string.IsNullOrWhiteSpace(strResponse))
            {
                DisplayMessage(string.Format("Warning, Get Alarm Message:{0}, ErrorMessage:{1}", strResponse, strErrorMessage));
                m_objOMORN.ClearAlarm(1, ref strErrorMessage);

                if (strResponse.IndexOf("0000H", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    MessageBox.Show("请确认挡板在治具中间位置，重启治具电源后重新打开测试程序 !!!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // Go Home
            if (m_objOMORN.Home(1, ref strErrorMessage) == false)
            {
                strErrorMessage = "Fail to Go Home: " + strErrorMessage;
                return false;
            }

            DisplayMessage("Motor Go Back to Home Successful.");
            return true;
        }

        public bool MotorMoveToHome(ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                string MotorType = clsConfigHelper.servoMotor.DeviceType;
                if (MotorType == "OMORN")
                {       
                    // Go Home
                    if (m_objOMORN.Home(1, ref strErrorMessage) == false)
                    {
                        strErrorMessage = "Fail to Go Home: " + strErrorMessage;
                        return false;
                    }    
                }
                else if (MotorType == "PANASONIC")
                {

                }
                else
                {
                    strErrorMessage = "Invalid Motor Type !!!";
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

        public bool InitPANASONICMotor()
        {



            return true;
        }

     

        // PLC


        #endregion

        #region UI

        private void ShowTestItem(string message)
        {
            Program.g_mainForm.ShowTestItem(message);
        }

        private void DisplayMessage(string message)
        {
            Program.g_mainForm.DisplayMessage(message);  
        }

        #endregion
    }
}
