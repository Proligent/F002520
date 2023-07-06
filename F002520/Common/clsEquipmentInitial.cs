using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMLib;
using OMLib.Communication;
using OMLib.Communication.ModbusInfo;
using OMLib.Products.AZSeries;
using OM_Modbus;

namespace F002520
{
    public class clsEquipmentInitial
    {
        // DAQ
        public clsNI6251 m_objNIDAQ = new clsNI6251();
        public clsJYDAMDAQ m_objJYDAMDAQ = new clsJYDAMDAQ();

        // OMORN Motor
        //private modbus_hon.ModbusOp.serialSettings m_modbusSetting = new modbus_hon.ModbusOp.serialSettings();
        //public modbus_hon.ModbusOp m_objOMORN = new modbus_hon.ModbusOp(OMLib.PRODUCT.AR);

        private OM_Modbus.OMModbus.SerialSetting m_modbusSetting = new OM_Modbus.OMModbus.SerialSetting();
        public OM_Modbus.OMModbus m_objOMORN = new OM_Modbus.OMModbus(OMLib.PRODUCT.AR);

        // Panasonic Motor


        #region Construct

        public clsEquipmentInitial()
        { 
        }

        #endregion


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
                return false;
            }

            // Get Alarm
            string strResponse = ""; 
            if (m_objOMORN.GetAlarm(1, ref strResponse, ref strErrorMessage) == false)
            {
                return false;
            }
            else
            {
                DisplayMessage(string.Format("Warning, Get Motor Alarm Code: {0} ", strResponse));
                if (strResponse.IndexOf("0000H", StringComparison.OrdinalIgnoreCase) == -1)
                {        
                    // Show Message
                    MessageBox.Show(string.Format("获取到电机报警代码: {0}", strResponse), "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show("将挡板移动到治具中间位置，重启治具电源后重新打开测试程序 !!!", "解决方法", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset Alarm
                    m_objOMORN.ResetAlarm(1);   
                    return false;
                }
            }

            #region Obsolote
            // Set Move Velocity (Can't Set Velocity here !!!)
            //if (m_objOMORN.MoveVelocity(1, 5000, 2000, 2000, ref strErrorMessage) == false)
            //{
            //    return false;
            //}
            #endregion

            // Go Home
            if (m_objOMORN.Home(1, ref strErrorMessage) == false)
            {
                return false;
            }

            // Check Position
            int iRange = 10;    // Compare to Zero Position
            int iPosition = 0;     
            bool bFlag = false;
            TimeSpan duration = TimeSpan.FromSeconds(12);
            DateTime startTime = DateTime.Now;
            while((DateTime.Now - startTime) < duration)
            {
                if (m_objOMORN.ReadActualPosition(1, ref iPosition, ref strErrorMessage) == false)
                {
                    bFlag = false;
                    clsUtil.Dly(2.0);
                    continue;
                }
                if (Math.Abs(iPosition) < iRange)
                {
                    bFlag = true;
                    break;
                }
                else
                {
                    bFlag = false;
                    clsUtil.Dly(2.0);
                    continue;             
                }   
            }
            if (bFlag == false)
            {
                // Get Alarm
                string AlarmCode = "";
                bool bRet = m_objOMORN.GetAlarm(1, ref strResponse, ref strErrorMessage);
                if (bRet && AlarmCode.IndexOf("0000H", StringComparison.OrdinalIgnoreCase) == -1)   //获取报警代码成功，并且报警代码不是0000(有警报)
                {
                    MessageBox.Show(string.Format("警告，获取到电机报警代码: {0}", AlarmCode), "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    m_objOMORN.ResetAlarm(1);   
                    // Feedback to PLC
                }

                DisplayMessage("Fail to Go Back to Home Position !!!", "ERROR");
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
                        return false;
                    }
                   

                    #region Obsolote

                    clsUtil.Dly(2.0);
                    // 影响测试时间， 开个线程去检查
                    // Check Position
                    //int iRange = 10;    // Compare to Zero position
                    //int iPosition = 0;
                    //bool bFlag = false;
                    //string strResponse = "";

                    //TimeSpan duration = TimeSpan.FromSeconds(15);
                    //DateTime startTime = DateTime.Now;
                    //while ((DateTime.Now - startTime) < duration)
                    //{
                    //    if (m_objOMORN.ReadActualPosition(1, ref iPosition, ref strErrorMessage) == false)
                    //    {
                    //        bFlag = false;
                    //        clsUtil.Dly(2.0);
                    //        continue;
                    //    }
                    //    if (Math.Abs(iPosition) < iRange)
                    //    {
                    //        bFlag = true;
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        bFlag = false;
                    //        clsUtil.Dly(2.0);
                    //        continue;
                    //    }
                    //}
                    //if (bFlag == false)
                    //{
                    //    // Get Alarm
                    //    string AlarmCode = "";
                    //    bool bRet = m_objOMORN.GetAlarm(1, ref strResponse, ref strErrorMessage);
                    //    if (bRet && AlarmCode.IndexOf("0000H", StringComparison.OrdinalIgnoreCase) == -1)   //获取报警代码成功，并且报警代码不是0000(有警报)
                    //    {
                    //        MessageBox.Show(string.Format("警告，获取到电机报警代码: {0}", AlarmCode), "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        m_objOMORN.ResetAlarm(1);
                    //        // Feedback to PLC
                    //    }
                    //    return false;
                    //}

                    #endregion

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

        private void DisplayMessage(string message, string level = "INFO")
        {
            Program.g_mainForm.DisplayMessage(message, level);  
        }

        #endregion
    }
}
