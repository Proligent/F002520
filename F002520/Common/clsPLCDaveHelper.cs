using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    class clsPLCDaveHelper
    {
        private static libnodave.daveConnection dc;
        private static libnodave.daveInterface di;
        private static libnodave.daveOSserialType fds;
        private bool bConnected = false;
        private const int DataBlock = 132;
        private Object obj = new Object();
        private object locker = new object();

        #region Enum

        public enum ReturnStatus : int
        {
            READY = 1,
            BUSY,
            HAVEPRODUCT,
            ERROR
        }

        public enum ReturnExecResult : int
        {
            SUCCESS = 1,
            FAIL
        }

        public enum BarCodeStatus : int
        {
            OK = 1,
            NG
        }

        #endregion

        #region Connect and DisConnect
        //Connect TCP/IP
        public bool Connect(string sIPAddr, int iPort, ref string sError)
        {
            lock (obj)
            {
                sError = "";
                int iRes = -100;
                try
                {
                    fds.rfd = libnodave.openSocket(iPort, sIPAddr);
                    fds.wfd = fds.rfd;
                    di = new libnodave.daveInterface(fds, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);

                    di.setTimeout(10000);
                    iRes = di.initAdapter();
                    if (iRes != 0)
                    {
                        bConnected = false;
                        sError = string.Format("#Connect error, init adapter failed!");
                        return false;
                    }

                    dc = new libnodave.daveConnection(di, 0, 0, 1);
                    iRes = dc.connectPLC();
                    if (iRes != 0)
                    {
                        bConnected = false;
                        sError = string.Format("#Connect error, could't connect PLC!");
                        return false;
                    }
                    else
                    {
                        bConnected = true;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    bConnected = false;
                    sError = string.Format("#Connect exception, connection aborted!" + ex.Message.ToString());
                    return false;
                }
            }
        }

        //DisConnect
        public bool DisConnect(ref string sError)
        {
            lock (obj)
            {
                sError = "";
                try
                {
                    dc.disconnectPLC();
                    di.disconnectAdapter();
                    libnodave.closeSocket(fds.rfd);
                    return true;
                }
                catch (Exception e)
                {
                    sError = string.Format("#Disconnect exception, {0}", e.Message);
                    return false;
                }
                finally
                {
                    bConnected = false;
                }
            }
        }
        #endregion

        #region Read Byte and String

        private byte ConvertByteInToByte(byte FirstByte)
        {
            return FirstByte;
        }

        private bool ReadByte(ref string sResult, int iReadBlock, int iReadAddress, int iBytesToRead, ref string sError)
        {
            lock (obj)
            {
                sError = "";
                byte[] bArr = new byte[1025];
                int iRes = -100;
                byte bRead = 0;
                sResult = "";
                try
                {
                    if (bConnected)
                    {
                        iRes = dc.readBytes(DataBlock, iReadBlock, iReadAddress, iBytesToRead, bArr);
                        if (iRes != 0)
                        {
                            //DisConnect(ref sError);
                            return false;
                        }
                        bRead = ConvertByteInToByte(bArr[0]);
                        sResult = Convert.ToString(bRead);
                        return true;
                    }
                    else
                    {
                        sError = string.Format("#Read Value Error, PLC doesn't connect!");
                        return false;
                    }
                }
                catch
                {
                    sError = string.Format("#Read Value Exception!");
                    return false;
                }
            }
        }

        private bool ReadString(ref string sResult, int iReadBlock, int iReadAddress, int iBytesToRead, ref string sError)
        {
            lock (obj)
            {
                sError = "";
                byte[] bArr = new byte[1025];
                int iRes = -100;
                byte bFirst = 0, bSecond = 0;
                sResult = "";

                try
                {
                    if (bConnected)
                    {
                        iRes = dc.readManyBytes(DataBlock, iReadBlock, iReadAddress, iBytesToRead, bArr);
                        if (iRes != 0)
                        {
                            //DisConnect(ref sError);
                            return false;
                        }
                        bFirst = bArr[0];
                        bSecond = bArr[1];
                        if (bFirst >= bSecond)
                        {
                            sResult = Encoding.ASCII.GetString(bArr, 2, iBytesToRead);
                        }
                        else
                        {
                            sError = string.Format("#Read Error, data format is error!");
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        sError = string.Format("#Read Value Error, PLC doesn't connect!");
                        return false;
                    }
                }
                catch
                {
                    sError = string.Format("#Read Value Exception!");
                    return false;
                }
            }
        }
        
        #endregion

        #region Write Byte and String

        private bool WriteByte(byte bWrite, int iWriteBlock, int iWriteAddress, int iBytesToWrite, ref string sError)
        {
            lock (obj)
            {
                sError = "";
                byte[] bArr = new byte[1025];
                int iRes = -100;

                try
                {
                    if (bConnected)
                    {
                        bArr[0] = bWrite;
                        iRes = dc.writeBytes(DataBlock, iWriteBlock, iWriteAddress, iBytesToWrite, bArr);
                        if (iRes != 0)
                        {
                            //DisConnect(ref sError);
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        sError = string.Format("#Write Value Error, PLC doesn't connect!");
                        return false;
                    }
                }
                catch
                {
                    sError = string.Format("#Write Value Exception!");
                    return false;
                }
            }
        }

        private bool WriteString(string sData, int iWriteBlock, int iWriteAddress, int iBytesToWrite, ref string sError)
        {
            lock (obj)
            {
                sError = "";
                byte[] bArr = new byte[1025];
                int iRes = -100;

                try
                {
                    if (bConnected)
                    {
                        if (sData.Length > 50)
                            sData.Substring(0, 50);
                        byte[] bCmd = Encoding.ASCII.GetBytes(sData);
                        for (int i = 0; i < sData.Length; i++)
                        {
                            bArr[i + 2] = bCmd[i];
                        }
                        bArr[0] = (byte)iBytesToWrite;
                        bArr[1] = (byte)sData.Length;
                        iRes = dc.writeBytes(DataBlock, iWriteBlock, iWriteAddress, iBytesToWrite, bArr);
                        if (iRes != 0)
                        {
                            //DisConnect(ref sError);
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        sError = string.Format("#Write Value Error, PLC doesn't connect!");
                        return false;
                    }
                }
                catch
                {
                    sError = string.Format("#Write Value Exception!");
                    return false;
                }
            }
        }
        
        #endregion

        #region PC to PLC
        //Watch Dog
        public bool WatchDogAdd(int iBlock, string sCmd)
        {
            lock (locker)
            {
                string sError = "";
                byte bWrite;
                try
                {
                    bWrite = Convert.ToByte(sCmd);
                    if (WriteByte(bWrite, iBlock, 0, 1, ref sError) == false)
                        return false;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        //Return Current Status
        public bool CurrentStatusReturn(int iBlock, ReturnStatus returnStatus, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                string sResult = "";
                int iStatus = (int)returnStatus;
                if (WriteByte((byte)iStatus, iBlock, 1, 1, ref sError) == false)
                    return false;

                #region Read Byte
                if (ReadByte(ref sResult, iBlock, 1, 1, ref sError) == false)
                    return false;

                if (iStatus.ToString() == sResult)
                    return true;
                else
                {
                    sError = "#Error: cmd isn't same with return!";
                    return false;
                }
                #endregion
            }
        }

        //Auto & Manual    1-----2
        public bool ReadAutoManual(int iBlock, ref string sResult, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                if (ReadByte(ref sResult, iBlock, 1, 1, ref sError) == false)
                    return false;
                return true;
            }
        }

        //Exec command return
        public bool CommandExecResultReturn(int iBlock, ReturnExecResult execResult, ref string sError)
        {
            lock (locker)
            {

                sError = "";
                string sResult = "";
                int iResult = (int)execResult;
                if (WriteByte((byte)iResult, iBlock, 2, 1, ref sError) == false)
                    return false;
                if (ReadByte(ref sResult, iBlock, 2, 1, ref sError) == false)
                    return false;

                if (iResult.ToString() == sResult)
                    return true;
                else
                {
                    sError = "#Error: cmd isn't same with return!";
                    return false;
                }
            }
        }

        //Get Error Code
        public bool ErrorCodeReturn(int iBlock, string sErrorEvent, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                if (sErrorEvent.Length > 100)
                    sErrorEvent = sErrorEvent.Substring(0, 100);
                if (WriteString(sErrorEvent, iBlock, 4, 100, ref sError) == false)
                    return false;
                return true;
            }
        }

        public bool BarCodeReturn(int iBlock, BarCodeStatus barcodeStatus, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                string sResult = "";
                int iResult = (int)barcodeStatus;
                if (WriteByte((byte)iResult, iBlock, 3, 1, ref sError) == false)
                    return false;
                if (ReadByte(ref sResult, iBlock, 3, 1, ref sError) == false)
                    return false;

                if (iResult.ToString() == sResult)
                    return true;
                else
                {
                    sError = "#Error: cmd isn't same with return!";
                    return false;
                }
            }
        }

        #endregion

        #region PLC to PC
        //Read Command
        public bool ReadCommand(int iBlock, ref string sResult, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                if (ReadByte(ref sResult, iBlock, 0, 1, ref sError) == false)
                    return false;
                return true;
            }
        }

        //PLC　Enable　Scan　Barcode
        public bool ReadEnableScanBarcode(int iBlock, ref string sResult, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                if (ReadByte(ref sResult, iBlock, 88, 1, ref sError) == false)
                    return false;
                return true;
            }
        }

        //Read BarCode
        public bool ReadBarCode(int iBlock, ref string sResult, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                if (ReadString(ref sResult, iBlock, 4, 50, ref sError) == false)
                    return false;
                return true;
            }
        }

        //Read Part Number
        public bool ReadPartNumber(int iBlock, ref string sResult, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                if (ReadString(ref sResult, iBlock, 54, 50, ref sError) == false)
                    return false;
                return true;
            }
        }

        //Judge BarCode
        public bool JudgeBarCode(int iBlock, ref string sResult, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                if (ReadByte(ref sResult, iBlock, 2, 1, ref sError) == false)
                    return false;
                return true;
            }
        }

        // 获取PLC自动状态 1:自动
        public bool ReadAutoStatus(int iBlock, ref string sResult, ref string sError)
        {
            lock (locker)
            {
                sError = "";
                if (ReadByte(ref sResult, iBlock, 104, 1, ref sError) == false)
                {
                    return false;
                }

                return true;
            }
        }

        #endregion
    }
}
