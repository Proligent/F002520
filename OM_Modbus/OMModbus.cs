using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using OMLib;
using OMLib.Communication;
using OMLib.Communication.ModbusInfo;

namespace OM_Modbus
{
    #region Comment

    /*********************************************************************
    * File: OM_Modbus.dll (depend on mdbsmnglib.dll and mdbslib.dll)
    * Author : Calvin Xie
    * Created: 2023/06/26
    * Last Modified: 2023/07/20
    * Description: This dll is used for OM Motor Control.
    * 
     * 
    * Step:
    * 1. PortOpen();
    * 2. GetAlarm();
    * 3. ClearAlarm();          // If Alarm Code not 0000H
    * 4. Home();
    * 5. MoveAbsolution();
    * 6. ReadActualPosition();
    * 7. Stop();
    * 8. PortClose();
    * 
    *********************************************************************/

    #endregion

    public class OMModbus
    {
        #region Struct

        public struct SerialSetting
        {
            public int PortNum;
            public int Baud;
            public int DataBits;
            public Parity Parity;
            public StopBits StopBits;
        }

        #endregion

        #region Variable

        private Modbus modbus = null;

        #endregion

        #region Construct

        public OMModbus(PRODUCT product)
        {
            modbus = new Modbus(product);
            Modbus.SerchDllPath("x86", "x64");  // Auto select x86/x64 mdbslib.dll
        }

        #endregion

        #region Function

        /// <summary>
        /// Open the Serial Port
        /// </summary>
        /// <param name="serialSetting"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool PortOpen(SerialSetting serialSetting, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                // If the SerialPort not Open, Open the port.
                if (modbus.IsPortOpen() == false)
                {
                    modbus.PortOpen("COM" + serialSetting.PortNum.ToString("D1"),
                                    serialSetting.Baud,
                                    Parity.Even,
                                    StopBits.One);
                }

                if (modbus.IsPortOpen() == false)
                {
                    strErrorMessage = "Fail to open serial port !!!";
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

        /// <summary>
        /// Close the Serial Port
        /// </summary>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool PortClose(ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                // If the port is open, close the port.
                if (modbus.IsPortOpen())
                {
                    modbus.PortClose();
                }

                if (modbus.IsPortOpen())
                {
                    strErrorMessage = "Fail to close serial port !!!";
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

        /// <summary>
        /// Reset Alarm Code / 解除警报
        /// </summary>
        /// <param name="slave"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool ResetAlarm(byte slave)
        {
            string strErrorMessage = "";

            try
            {
                // If the port is opened, Reset the alarm
                if (modbus.IsPortOpen())
                {
                    modbus.AlarmReset(slave);
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slave"></param>
        /// <param name="strResponse"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool GetAlarm(byte slave, ref string strResponse, ref string strErrorMessage)
        {
            strResponse = "";
            strErrorMessage = "";

            try
            {
                int AlarmCode;

                // 如果串口被打开的话，获取报警代码
                if (modbus.IsPortOpen())
                {
                    // 读取报警代码，并将读取结果的判读值(例: 错误代码) 赋给变量ret.
                    var ret = modbus.GetAlarm(slave,            // Slave address
                                              out AlarmCode);   // Output the Alarm Code.

                    // 执行获取成功，显示监视的报警代码
                    if (ret == ErrorCode.ERROR_NONE)
                    {
                        strResponse = AlarmCode.ToString("x4") + "H" + "(" + Convert.ToString(AlarmCode) + ")";
                    }
                    else
                    {
                        strErrorMessage = "Fail to get Alarm Code, ErrorMessage: " + Enum.GetName(typeof(ErrorCode), ret);
                        return false;
                    }
                }
                else
                {
                    strErrorMessage = "The Serial Port is not Opened !!!";
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

        /// <summary>
        /// Move Motor to Home
        /// </summary>
        /// <param name="slave"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool Home(byte slave, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                var ret = modbus.Home(slave);
                if (ret != ErrorCode.ERROR_NONE)
                {
                    strErrorMessage = "Fail to return to Home, ErrorMessage: " + Enum.GetName(typeof(ErrorCode), ret);
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

        /// <summary>
        /// Move to Absolute Position
        /// </summary>
        /// <param name="slave">从站地址</param>
        /// <param name="pos">位置</param>
        /// <param name="vel">速度</param>
        /// <param name="acceleration">加速度斜率(kHz/s), 1000(默认)</param>
        /// <param name="deceleration">减速度斜率(kHz/s), 1000(默认)</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool MoveAbsolute(byte slave, int pos, int vel, uint acceleration, uint deceleration, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                var ret = modbus.MoveAbsolute(slave, pos, vel, acceleration, deceleration);
                if (ret != ErrorCode.ERROR_NONE)
                {
                    strErrorMessage = "Fail to move absolute position, ErrorMessage: " + Enum.GetName(typeof(ErrorCode), ret);
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

        /// <summary>
        /// Move to Relative Position
        /// </summary>
        /// <param name="slave">从站地址</param>
        /// <param name="pos">位置</param>
        /// <param name="vel">速度</param>
        /// <param name="speedChangeRate">加速度斜率(kHz/s), 1000(默认)</param>
        /// <param name="stopRate">减速度斜率(kHz/s), 1000(默认)</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool MoveRelative(byte slave, int pos, int vel, uint speedChangeRate, uint stopRate, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                var ret = modbus.MoveRelative(slave, pos, vel, speedChangeRate, stopRate);
                if (ret != ErrorCode.ERROR_NONE)
                {
                    strErrorMessage = "Fail to move relative position, ErrorMessage: " + Enum.GetName(typeof(ErrorCode), ret);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slave">从站地址</param>
        /// <param name="vel">位置</param>
        /// <param name="speedChangeRate">加速度斜率(kHz/s), 1000(默认)</param>
        /// <param name="stopRate">减速度斜率(kHz/s), 1000(默认)</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool MoveVelocity(byte slave, int vel, uint speedChangeRate, uint stopRate, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                var ret = modbus.MoveVelocity(slave, vel, speedChangeRate, stopRate);
                if (ret != ErrorCode.ERROR_NONE)
                {
                    strErrorMessage = "Fail to move velocity position, ErrorMessage: " + Enum.GetName(typeof(ErrorCode), ret);
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

        /// <summary>
        /// Read Motor Actual Position
        /// </summary>
        /// <param name="slave">从站地址</param>
        /// <param name="actualPosition">电机的实际位置</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool ReadActualPosition(byte slave, ref int Position, ref string strErrorMessage)
        {
            strErrorMessage = "";
            Position = 0;

            try
            {
                int? actualPosition;

                var ret = modbus.ReadActualPosition(slave, out actualPosition);
                if (ret != ErrorCode.ERROR_NONE)
                {
                    strErrorMessage = "Fail to read actual position, ErrorMessage: " + Enum.GetName(typeof(ErrorCode), ret);
                    return false;
                }

                Position = actualPosition ?? 0;
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read Command Position
        /// </summary>
        /// <param name="slave"></param>
        /// <param name="strCommandPosition"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool ReadCommandPosition(byte slave, ref string strCommandPosition, ref string strErrorMessage)
        {
            strErrorMessage = "";
            strCommandPosition = "";

            try
            {
                int? commandPosition;

                var ret = modbus.ReadCommandPosition(slave, out commandPosition);
                if (ret != ErrorCode.ERROR_NONE)
                {
                    strErrorMessage = "Fail to read command position, ErrorMessage: " + Enum.GetName(typeof(ErrorCode), ret);
                    return false;
                }

                strCommandPosition = commandPosition.ToString();
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Stop the Operation
        /// </summary>
        /// <param name="slave"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool Stop(byte slave, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                var ret = modbus.Stop(slave);
                if (ret != ErrorCode.ERROR_NONE)
                {
                    strErrorMessage = "Fail to stop operation, ErrorMessage: " + Enum.GetName(typeof(ErrorCode), ret);
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

        #endregion

    }
}
