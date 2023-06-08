using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using SmartFactory.ExternalDLL;
using F002520.Properties;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace F002520
{
    public partial class frmMain : Form
    {
        #region Enum

        #endregion

        #region Struct

        private struct MCFData
        {
            public string SKU;
            public string Model;   
        }

        private struct MESData
        {
            public string EID;
            public string WorkOrder;
        }

        // Option.ini
        public struct OptionData
        {     
            // TestMode
            public string TestMode;

            // AutoChangeOver
            public string AutoChangeOver;
          
            // MES
            public string MES_Enable;
            public string MES_Station;

            // MDCS
            //public string MDCSEnable;
            //public string MDCSMode;
            //public string MDCSURL;
            //public string MDCSDeviceName;
            //public string PreStationResultCheck;
            //public string PreStationDeviceName;
            //public string PreStationVarName;
            //public string PreStationVarValue;

            // XML
            public string TestItemXMLFile;
            public string EquipmentXMLFile;
        }

        #endregion

        #region Variable

        private bool m_bStop = false;
        private bool m_bRunInitialized = false;
        public static bool m_bMESEnable = false;

        private ModelID m_Type;
        private string m_strSKU = "";
        public string m_strModel = "";    
        public static string m_strTestItemXMLFile = "";
        public static string m_strEquipmentXMLFile = "";

        private MCFData m_stMCFData = new MCFData();
        private MESData m_stMESData = new MESData();
        public OptionData m_stOptionData = new OptionData();
        private List<string> m_TestItemList = new List<string>();  
        public clsEquipmentInitial m_objEquipmentInitial = new clsEquipmentInitial();

 
        // PLC
        private clsPLCDaveHelper m_PLC;
        private string sErrorPlcThread = "";
        private string sResultPlcThread = "";
        private System.Threading.Timer PLCTimer = null;
        private System.Threading.Timer PLCWatchDog = null;
        private int iWatchDog = 0;
        private bool isTestRunning = false; // true:测试中  false:空闲
        private bool isPLCConnected = false;
        private bool isManual = false;
        private bool bPLCThreadRun = false;

        // MES

        // MDCS
        public static TestSaveData m_stTestSaveData = new TestSaveData();

        #endregion

        #region Property

        #endregion

        #region Form

        public frmMain()
        {
            InitializeComponent();

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string strErrorMessage = "";
            isTestRunning = false;
            isPLCConnected = false;

            lblTitleBar.Text = Program.g_strToolNumber + " Sensor Calibration Fixture";
            lblVersion.Text = Program.g_strToolRev;

            // Init Log
            log4net.GlobalContext.Properties["DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
            log4net.GlobalContext.Properties["LogFileName"] = "Prepare";
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            Logger.Info("MainForm Loading ...");

            // Init Run
            if (InitRun(ref strErrorMessage) == false)
            {
                strErrorMessage = "InitRun fail, " + strErrorMessage;
                DisplayMessage(strErrorMessage);
                MessageBox.Show(strErrorMessage);
                return;
            }
        
            InitCompleted();

            Logger.Info("MainForm Load Completed.");
            return;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            string strErrorMessage = "";

            ReleaseHW(ref strErrorMessage);

            #region PLC

            if (PLCTimer != null)
            {
                PLCTimer.Dispose();
                PLCTimer = null;
            }
            if (PLCWatchDog != null)
            {
                PLCWatchDog.Dispose();
                PLCWatchDog = null;
            }

            if (isPLCConnected == true)
            {
                m_PLC.DisConnect(ref strErrorMessage);
            }

            // PLC ERROR (配合PLC 解决 PLC Bug)
            if (isPLCConnected == true)
            {
                bool bFlag = false;
                string strError = "";
                for (int i = 0; i < 3; i++)
                {
                    DisplayMessage("PLC:Send, ERROR Status", "ERROR");
                    if (m_PLC.CurrentStatusReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnStatus.ERROR, ref strError))
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
                    DisplayMessage("PLC:Send ERROR Status Fail." + strError, "ERROR");
                    return;
                }

                DisplayMessage("PLC:Send ERROR Status Success.");
            }

            #endregion

        }

        #endregion

        #region Menu


        #endregion

        #region Event

        private void btnStart_Click(object sender, EventArgs e)
        {
            RunStart();  
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            m_bStop = true;

            btnStop.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // Maximized
                this.WindowState = FormWindowState.Maximized;
                this.btnMaximize.Image = Resources.normal_16;
                //if (m_bCollapse)
                //{
                //    this.panelMenu.Width = 80;
                //}
                //else
                //{
                //    this.panelMenu.Width = 250;
                //}

                //this.panelLog.Height = 160;
            }
            else
            {
                // Normal
                this.WindowState = FormWindowState.Normal;
                this.btnMaximize.Image = Resources.maximize_16;
                //if (m_bCollapse)
                //{
                //    this.panelMenu.Width = 80;
                //}
                //else
                //{
                //    this.panelMenu.Width = 200;
                //}

                //this.panelLog.Height = 145;
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lblTitleBar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Only can use mouse right button, because left button event conflict with MouseDown event.
            if (e.Button == MouseButtons.Right)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Maximized;
                    this.btnMaximize.Image = Resources.normal_16;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    this.btnMaximize.Image = Resources.maximize_16;
                }
            }
        }

        private void lblTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            Win32.ReleaseCapture();
            Win32.SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        #endregion

        #region Function

    

        #region PLC

        private bool InitPLC(ref string sErrorMessage)
        {
            if (clsConfigHelper.plcConfig.Enable == true)
            {
                m_PLC = new clsPLCDaveHelper();

                if (m_PLC.Connect(clsConfigHelper.plcConfig.PLCIP, clsConfigHelper.plcConfig.PLCPort, ref sErrorMessage) == false)
                {
                    isPLCConnected = false;
                    m_PLC.DisConnect(ref sErrorMessage);
                    sErrorMessage = "PLC Connect Failure!";
                    return false;
                }
                else
                {
                    isPLCConnected = true;
                }
                DisplayMessage("Connect PLC success.");
            }
            else
            {
                isPLCConnected = false;
                SetManual(true);
            }

            return true;
        }

        private void SetPLCConnectStatus(string sValue)
        {
            //if (this.txtPLCConnectStatus.Text == sValue)
            //{
            //    return;
            //}
            //Invoke((Action)delegate()
            //{
            //    this.txtPLCConnectStatus.Text = sValue;
            //});
        }

        public void Thread_Timer_PLC_WatchDog(object obj)
        {
            try
            {
                iWatchDog %= 255;

                //Logger.Info("WatchDog: {0}", iWatchDog.ToString("D"));
                if (m_PLC.WatchDogAdd(clsConfigHelper.plcConfig.WriteDB, iWatchDog.ToString()) == false)
                {
                    isPLCConnected = false;
                    DisplayMessage("PLC:WatchDog fail.");          
                    return;
                }
                else
                {     
                    iWatchDog++;
                }
            }
            catch (Exception ex)
            {
                isPLCConnected = false;
                DisplayMessage("PLC:WatchDog Exception:" + ex.Message.ToString());   
                return;
            }
            finally
            {
                if (isPLCConnected == false)
                {
                    if (PLCConnect() == false)
                    {
                    }
                }
            }
        }

        public void Thread_Timer_PLC(object obj)
        {
            string strErrorMessage = "";

            try
            {
                if (bPLCThreadRun == true)
                {
                    return;
                }
                else
                {
                    bPLCThreadRun = true;
                }

                #region Product Detect

                if (isTestRunning == false && isPLCConnected == true)
                {
                    #region Read Auto/Manual

                    DisplayMessage("PLC:Read, Read Auto or Manual.");
                    if (m_PLC.ReadAutoManual(clsConfigHelper.plcConfig.ReadDB, ref sResultPlcThread, ref sErrorPlcThread) == false)
                    {
                        isManual = true;   
                        SetManual(isManual);
                        DisplayMessage("PLC:ReadAutoManual fail.");
                        m_PLC.DisConnect(ref sErrorPlcThread);
                    }
                    else
                    {              
                        // Auto
                        if (sResultPlcThread == "1")
                        {
                            DisplayMessage("TestMode = Auto");
                            isManual = false;
                            SetManual(isManual);
                        }

                        // Manual
                        else if (sResultPlcThread == "2")
                        {
                            DisplayMessage("TestMode = Manual");
                            isManual = true;
                            SetManual(isManual);
                        }
                        else
                        {
                            DisplayMessage("Invalid Auto or Manual Value.", "ERROR");
                            return;
                        }

                        DisplayMessage("PLC:Read, Read Auto or Manual SUCCESS.");
                    }

                    #endregion

                    #region ProductDetect

                    // 检查USB Pogopin弹出， Holder里面没有产品，给PLC Ready信号，表示当前治具空闲，可以进行测试，
                    // PLC自动检测待测区域，或者RF测试完，有产品需要抓取，就会自动抓取放到SensorK, 并WaitForSignal返回 1.
                    // WaitForSignal返回 1，表示机械手正在抓取产品放入SensorK治具，然后返回给PLC Busy信号，表示该治具正在测试中.

                    DisplayMessage("Check Product Ready.");
                    if (ProductDetect(m_objEquipmentInitial.m_objNIDAQ, ref strErrorMessage) == true)
                    {         
                        #region BUSY

                        bool bFlag = false;
                        for (int i = 0; i < 10; i++)
                        {             
                            if (m_PLC.CurrentStatusReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnStatus.BUSY, ref sErrorPlcThread))
                            {
                                bFlag = true;
                                break;
                            }
                            else
                            {
                                bFlag = false;
                                continue;
                            }
                        }
                        if (bFlag == false)
                        {  
                            DisplayMessage("PLC Send Busy Status Fail !!!");
                            return;
                        }
                        else
                        {
                            DisplayMessage("PLC Send Busy Status Success.");
                        }

                        #endregion

                        #region Start

                        DisplayMessage("Start RunTest.");
                        RunStart();

                        #endregion
                    }

                    #endregion
                }

                #endregion
            }
            catch (Exception ex)
            {
                DisplayMessage("PLC Thread Exception:" + ex.Message.ToString());
                return;
            }
            finally
            {
                bPLCThreadRun = false;
            }
        }

        private bool PLCConnect()
        {
            try
            {        
                m_PLC.DisConnect(ref sErrorPlcThread);
                if (m_PLC.Connect(clsConfigHelper.plcConfig.PLCIP, clsConfigHelper.plcConfig.PLCPort, ref sErrorPlcThread) == false)
                {
                    isPLCConnected = false;
                    DisplayMessage("PLC:Connect fail.");      
                    return false;
                }
                else
                {
                    isPLCConnected = true;
                }
            }
            catch (Exception ex)
            {
                isPLCConnected = false;
                DisplayMessage("PLC Connect Exception:" + ex.Message.ToString());        
                return false;
            }

            return true;
        }

        private bool ProductDetect(clsNI6251 clsNIDAQ, ref string strErrorMessage)
        {
            strErrorMessage = "";
            double dValue = 0.0;
            double dValue_AI0 = 0.0;
            double dValue_AI1 = 0.0;
            bool bRes = false;
            bool bFlag = false;

            try
            {
                #region Detect USB Cable Whether Ejected(弹出)

                for (int i = 0; i < 3; i++)
                {
                    bRes = clsNIDAQ.GetAnalog(0, 10, ref dValue_AI0, 0.1);  // AI0
                    bRes = clsNIDAQ.GetAnalog(1, 10, ref dValue_AI1, 0.1);  // AI1

                    Logger.Info("Check USB Pogopin Ejected, AI0: {0} V, AI1: {1} V", dValue_AI0.ToString("F3"), dValue_AI1.ToString("F3"));
                    if ((dValue_AI0 > 3.0) && (dValue_AI1 < 2.0))
                    {
                        bFlag = true;
                        Logger.Info("USB Pogopin Already Ejected.");
                        break;
                    }
                    else
                    {
                        strErrorMessage = "Detect USB Cable are not Ejected !";
                        bFlag = false;
                        clsUtil.Dly(1.0);
                        continue;
                    }
                }
                if (bFlag == true)
                {
                    #region Light Sensor

                    // Detect Whether Have Product
                    bRes = clsNIDAQ.GetAnalog(2, 10, ref dValue, 0.1);  // AI2
                    Logger.Info("Check Holder Have Product, AI2: {0} V", dValue.ToString("F3"));
                    if (dValue > 3.0)
                    {
                        m_PLC.CurrentStatusReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnStatus.HAVEPRODUCT, ref strErrorMessage);
                        Logger.Warn("PLC Write: Have Product.");
                    }
                    else
                    {
                        m_PLC.CurrentStatusReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnStatus.READY, ref strErrorMessage);
                        Logger.Info("PLC Write: Holder is Empty, Give PLC Ready Signal.");
                    }

                    #endregion

                    #region WaitForSignal

                    DisplayMessage("PLC:Read, Wait for PLC Singal.");
                    if (WaitForSignal(ref strErrorMessage) == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    #endregion
                }
                else
                {
                    m_PLC.CurrentStatusReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnStatus.ERROR, ref strErrorMessage);
                    return false;
                }

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }
        }

        private bool WaitForSignal(ref string sError)
        {
            try
            {       
                string sResult = "";
                m_PLC.ReadCommand(clsConfigHelper.plcConfig.ReadDB, ref sResult, ref sError);

                DisplayMessage("Wait for PLC Signal, Res: " + sResult);
                if (sResult == "1")
                {
                    return true;
                }
                else
                {
                    sError = "PLC Wait For Singal fail. Res: " + sResult;
                    //DisplayMessage(sError);
                    return false;
                }
            }
            catch (Exception ex)
            {
                sError = "Exception:" + ex.Message;
                Logger.Error("WaitForSingal Exception:" + ex.Message);
                return false;
            }
        }

        private bool ReportError(string sErrorMessage)
        {
            string sError = "";

            try
            {
                DisplayMessage("PLC:Send FAIL result.");
                if (m_PLC.ErrorCodeReturn(clsConfigHelper.plcConfig.WriteDB, sErrorMessage, ref sError) == false)
                {
                    //FrmMain.TraceLog("PLC:Return FAIL result fail.");
                    //FrmMain.AutomationLog("PLC:Send FAIL error message FAIL.");
                    return false;
                }

                if (m_PLC.CommandExecResultReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnExecResult.FAIL, ref sError) == false)
                {
                    //FrmMain.TraceLog("PLC:Return FAIL result fail.");
                    //FrmMain.AutomationLog("PLC:Send FAIL result FAIL.");
                    return false;
                }

                //FrmMain.AutomationLog("PLC:Send FAIL result SUCCESS.");

                return true;
            }
            catch (Exception ex)
            {
                string strr = ex.Message;
                //FrmMain.TraceLog("reportError Exception:" + strr);
                return false;
            }
        }

        private bool ReportSuccess()
        {
            string sError = "";

            try
            {
                //FrmMain.AutomationLog("PLC:Send SUCCESS result.");
                //FrmMain.TraceLog("PLC:Return SUCCESS result...");
                if (m_PLC.CommandExecResultReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnExecResult.SUCCESS, ref sError) == false)
                {
                    //FrmMain.TraceLog("PLC:Return SUCCESS result fail.");
                    //FrmMain.AutomationLog("PLC:Send SUCCESS result FAIL.");
                    return false;
                }

                //FrmMain.AutomationLog("PLC:Send SUCCESS result SUCCESS.");

                return true;
            }
            catch (Exception ex)
            {
                string strr = ex.Message;
                //FrmMain.TraceLog("reportSuccess Exception:" + strr);
                return false;
            }
        }

        private bool ReadAutoStatus(ref string strStatus)
        {
            try
            {
                strStatus = "";
                string sError = "";
                bool bRes = false;
                string sResult = "";
                for (int i = 0; i < 10; i++)
                {
                    //FrmMain.AutomationLog("PLC:Read PLC auto status.");
                    if (m_PLC.ReadAutoStatus(clsConfigHelper.plcConfig.ReadDB, ref sResult, ref sError) == false)
                    {
                        bRes = false;
                        continue;
                    }
                    else
                    {
                        bRes = true;
                        break;
                    }
                }
                if (bRes == false)
                {
                    //FrmMain.TraceLog("PLC:Read Auto Status fail.");
                    //FrmMain.AutomationLog("PLC:Read PLC auto status FAIL.");
                    return false;
                }
                //FrmMain.AutomationLog("PLC:Read PLC auto status SUCEESS.");

                strStatus = sResult.Trim();
            }
            catch (Exception ex)
            {
                string strr = ex.Message;
                //FrmMain.TraceLog("ReadAutoStatus Exception:" + ex.Message);
                return false;
            }

            return true;
        }

        #endregion


        #endregion

        #region Private

        private bool SelectLine()
        {
            frmProductLine frmLine = new frmProductLine();
            if (frmLine.ShowDialog() != DialogResult.Yes)
            {
                return false;
            }

            m_strModel = frmLine.ProductionLine;
            return true;
        }

        private bool ScanMES()
        {
            frmMES frmMES = new frmMES();
            if (frmMES.ShowDialog() != DialogResult.Yes)
            {
                return false;
            }
            m_stMESData.EID = frmMES.EID;
            m_stMESData.WorkOrder = frmMES.WorkOrder;

            return true;
        }

        private bool ScanMCF()
        {
            frmMCF frmMCF = new frmMCF();
            if (frmMCF.ShowDialog() != DialogResult.Yes)
            {
                return false;
            }

            m_stMCFData.SKU = frmMCF.SKU;
        
            #region Get Model by SKU

            string strModel = "";
            if (GetModelBySKU(ref strModel) == false)
            {
                return false;
            }

            m_stMCFData.Model = strModel;
            m_strModel = strModel;

            #endregion

            return true;
        }

        private bool GetModelBySKU(ref string strModel)
        {
            try
            {
                strModel = "";
                string strSKU = m_stMCFData.SKU;
                int iIndex = strSKU.IndexOf("-");
                if (iIndex == -1)
                {
                    return false;
                }

                strModel = strSKU.Substring(0, iIndex).ToUpper();
                if (strModel.Contains("L"))
                {
                    iIndex = strModel.IndexOf("L"); 
                    strModel = strModel.Substring(0, iIndex);
                }
                if (strModel.Contains("X"))
                {
                    iIndex = strModel.IndexOf("X");
                    strModel = strModel.Substring(0, iIndex);
                }
            }
            catch (Exception ex)
            {
                string strr = ex.Message;
                return false;
            }

            return true;
        }

        private void InitObject()
        {
            m_strSKU = "";
            m_strModel = "";
            

        }

        private bool ReadOptionFile(ref string strErrorMessage)
        {
            strErrorMessage = "";
           
            try
            {
                string fileName = "Option.ini";
                string filePath = Application.StartupPath + "\\" + fileName;
                clsIniFile objOptionFile = new clsIniFile(filePath);
            
                // Check File Exist
                if (File.Exists(filePath) == false)
                {
                    strErrorMessage = "File not exist: " + filePath;
                    return false;
                }

                #region TestMode

                m_stOptionData.TestMode = objOptionFile.ReadString("TestMode", "Mode");
                if ((m_stOptionData.TestMode != "0") && (m_stOptionData.TestMode != "1"))
                {
                    strErrorMessage = "Invalid TestMode Mode: " + m_stOptionData.TestMode;
                    return false;
                }

                #endregion

                #region AutoChangeOver

                m_stOptionData.AutoChangeOver = objOptionFile.ReadString("AutoChangeOver", "Enable");
                if ((m_stOptionData.AutoChangeOver != "0") && (m_stOptionData.AutoChangeOver != "1"))
                {
                    strErrorMessage = "Invalid AutoChangeOver Enable: " + m_stOptionData.AutoChangeOver;
                    return false;
                }

                //m_stOptionData.ScanSheetStation = objOptionFile.ReadString("AutoChangeOver", "Station");
                //if (m_stOptionData.AutoChangeOver == "1" && string.IsNullOrWhiteSpace(m_stOptionData.ScanSheetStation))
                //{
                //    strErrorMessage = "Invalid ScanSheetStation Value: " + m_stOptionData.ScanSheetStation;
                //    return false;
                //}

                #endregion

                #region MES

                m_stOptionData.MES_Enable = objOptionFile.ReadString("MES", "Enable");
                if ((m_stOptionData.MES_Enable != "0") && (m_stOptionData.MES_Enable != "1"))
                {
                    strErrorMessage = "Invalid MES_Enable: " + m_stOptionData.MES_Enable;
                    return false;
                }

                m_stOptionData.MES_Station = objOptionFile.ReadString("MES", "Station");
                if (m_stOptionData.MES_Enable == "1" && string.IsNullOrWhiteSpace(m_stOptionData.MES_Station))
                {
                    strErrorMessage = "Invalid MES_Station: " + m_stOptionData.MES_Station;
                    return false;
                }

                #endregion

                #region MDCS

                //m_stOptionData.MDCSEnable = objOptionFile.ReadString("MDCS", "Enable");
                //if ((m_stOptionData.MDCSEnable != "0") && (m_stOptionData.MDCSEnable != "1"))
                //{
                //    strErrorMessage = "Invalid MDCS_Enable:" + m_stOptionData.MDCSEnable;
                //    return false;
                //}

                //m_stOptionData.MDCSURL = objOptionFile.ReadString("MDCS", "URL");
                //if (string.IsNullOrWhiteSpace(m_stOptionData.MDCSURL))
                //{
                //    strErrorMessage = "Invalid MDCS_URL:" + m_stOptionData.MDCSURL;
                //    return false;
                //}

                //m_stOptionData.MDCSDeviceName = objOptionFile.ReadString("MDCS", "DeviceName");
                //if (string.IsNullOrWhiteSpace(m_stOptionData.MDCSDeviceName))
                //{
                //    strErrorMessage = "Invalid MDCS DeviceName:" + m_stOptionData.MDCSDeviceName;
                //    return false;
                //}

                //m_stOptionData.PreStationResultCheck = objOptionFile.ReadString("MDCS", "PreStationResultCheck");
                //if ((m_stOptionData.PreStationResultCheck != "0") && (m_stOptionData.PreStationResultCheck != "1"))
                //{
                //    strErrorMessage = "Invalid MDCS PreStationResultCheck:" + m_stOptionData.PreStationResultCheck;
                //    return false;
                //}

                //m_stOptionData.PreStationDeviceName = objOptionFile.ReadString("MDCS", "PreStationDeviceName");
                //if (string.IsNullOrWhiteSpace(m_stOptionData.PreStationDeviceName))
                //{
                //    strErrorMessage = "Invalid MDCS PreStationDeviceName:" + m_stOptionData.PreStationDeviceName;
                //    return false;
                //}

                //m_stOptionData.PreStationVarName = objOptionFile.ReadString("MDCS", "PreStationVarName");
                //if (string.IsNullOrWhiteSpace(m_stOptionData.PreStationVarName))
                //{
                //    strErrorMessage = "Invalid MDCS PreStationVarName:" + m_stOptionData.PreStationVarName;
                //    return false;
                //}

                //m_stOptionData.PreStationVarValue = objOptionFile.ReadString("MDCS", "PreStationVarValue");
                //if (string.IsNullOrWhiteSpace(m_stOptionData.PreStationVarValue))
                //{
                //    strErrorMessage = "Invalid MDCS PreStationVarValue:" + m_stOptionData.PreStationVarValue;
                //    return false;
                //}

                #endregion

                #region XML

                // Equipment
                m_stOptionData.EquipmentXMLFile = objOptionFile.ReadString("XML", "EquipmentConfig");
                if (string.IsNullOrWhiteSpace(m_stOptionData.EquipmentXMLFile))
                {
                    strErrorMessage = "Invalid EquipmentXMLFile Name: " + m_stOptionData.EquipmentXMLFile;
                    return false;  
                }
                m_strEquipmentXMLFile = m_stOptionData.EquipmentXMLFile;

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception: " + ex.Message;
                return false;
            }

            return true;
        }

        private bool InitTestItemList(ref string strErrorMessage)
        {
            try
            { 
                m_TestItemList.Clear();
         
                foreach (string key in clsConfigHelper.dicTestItemList.Keys)
                {
                    if (clsConfigHelper.dicTestItemList[key] == true)
                    {
                        // Option.ini文件中的AutoChangeOver选项可以直接控制，不用同时配置XML中为false.
                        if (key == "TestAutoChangeOver" && m_stOptionData.AutoChangeOver == "0") 
                        {
                            continue;
                        }

                        m_TestItemList.Add(key);
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

        private bool InitRun(ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            { 
                #region InitUI

                this.lblTestResult.Visible = false;
                this.lblTestItem.Text = "Init Run ...";
                this.lblTestItem.BackColor = Color.DarkGray;
                this.radioBtnProduction.Checked = true;
                this.rtbTestLog.Clear();

                #endregion

                #region Init Object

                InitObject();

                #endregion

                #region Load Option.ini

                DisplayMessage("Read Option.ini file.");
                if (ReadOptionFile(ref strErrorMessage) == false)
                {
                    strErrorMessage = "Fail to read Option.ini: " + strErrorMessage;
                    return false;
                }
                DisplayMessage("Read Option.ini file Success.");

                #endregion

                #region ScanSheet

                if (m_stOptionData.AutoChangeOver == "0")
                {
                    // Manual Input ScanSheet 
                    if (m_stOptionData.MES_Enable == "1")
                    {
                        DisplayMessage("MES input.");
                        if (ScanMES() == false)
                        {
                            DisplayMessage("Failed to MES input.");
                            return false;
                        }
                        DisplayMessage("EID:" + m_stMESData.EID);
                        DisplayMessage("WorkOrder:" + m_stMESData.WorkOrder);

                        // Check MES Data
                        if (clsUploadMES.MESCheckData(m_stMESData.EID, m_stOptionData.MES_Station, m_stMESData.WorkOrder, ref strErrorMessage) == false)
                        {
                            DisplayMessage("Failed to Check MES Data.");
                            return false;
                        }
                    }
                  
                    // MCF
                    DisplayMessage("Scan Sheet.");
                    if (ScanMCF() == false)
                    {
                        DisplayMessage("Failed to Scan Sheet.");
                        return false;
                    }
                    DisplayMessage("Model:" + m_strModel);
                    DisplayMessage("SKU:" + m_stMCFData.SKU);
                }
                else
                {
                    // AutoChangeOver
                    if (SelectLine() == false)
                    {
                        strErrorMessage = "Failed to select production line.";
                        return false;
                    }
                    DisplayMessage("Select Production Line: " + m_strModel);
                }

                #endregion

                #region Model Type

                if (m_strModel.Contains("CT40"))
                {
                    m_Type = ModelID.CT40;
                }
                else if (m_strModel.Contains("CT45"))
                {
                    m_Type = ModelID.CT45;
                }
                else if (m_strModel.Contains("CT47"))
                {
                    m_Type = ModelID.CT47;
                }
                else if (m_strModel.Contains("CW45"))
                {
                    m_Type = ModelID.CW45;
                }
                else
                {
                    m_Type = ModelID.NULL;
                }

                #endregion

                #region Load EquipmentConfig.xml

                DisplayMessage("Load Equipment Config XML File.");
                DisplayMessage("FileName: " + m_strEquipmentXMLFile); 
                m_strEquipmentXMLFile = Application.StartupPath + @"\TestConfig\Equipment\" + m_strEquipmentXMLFile;
              
                if (File.Exists(m_strEquipmentXMLFile) == false)
                {
                    strErrorMessage = "Equipment XML File not Exist: " + m_strEquipmentXMLFile;
                    return false;
                }
                if (clsEquipmentInitial.LoadEquipment(ref strErrorMessage) == false)
                {
                    strErrorMessage = "Failed to Load Equipment XML File: " + strErrorMessage;
                    return false;  
                }

                DisplayMessage("Load EquipmentConfig XML File Success.");

                #endregion

                #region Load TestItem XML File

                string Model = "";
                if (m_strModel.Length > 4)
                {
                    Model = m_strModel.Substring(0, 4);
                }
                else
                {
                    Model = m_strModel;
                }

                DisplayMessage("Load TestItem XML File.");
                m_strTestItemXMLFile = string.Format("{0}_SensorK_TestItem.xml", Model);   // TestItem XML File Name
                DisplayMessage("FileName: " + m_strTestItemXMLFile);

                m_strTestItemXMLFile = Application.StartupPath + @"\TestConfig\TestItem\" + Model + "\\" + m_strTestItemXMLFile;
                if (File.Exists(m_strTestItemXMLFile) == false)
                {
                    strErrorMessage = "TestItem XML File not Exist: " + m_strTestItemXMLFile;
                    return false;
                }

                // TestItem Enable Param
                if (LoadTestItemList(frmMain.m_strTestItemXMLFile, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Load TestItemList Enable Param fail." + strErrorMessage;
                    return false;
                }

                // TestItem Config Param
                if (LoadTestItemParameter(frmMain.m_strTestItemXMLFile, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Load TestItemList Config Param fail." + strErrorMessage;
                    return false;
                }

                // TestConfig Param
                if (LoadTestConfig(frmMain.m_strTestItemXMLFile, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Load TestConfig Param fail." + strErrorMessage;
                    return false;
                }

                DisplayMessage("Load TestItem XML File Success.");

                #endregion

                #region Init Hardware

                DisplayMessage("Init HW ...");

                if (InitHW(ref strErrorMessage) == false)
                {
                    strErrorMessage = "Failed to Init Hardware: " + strErrorMessage;
                    return false;
                }

                DisplayMessage("Init All HW Completed");

                #endregion

                #region Init PLC

                if (m_stOptionData.TestMode == "1")
                {
                    DisplayMessage("Auto TestMode, Init PLC ...");
                    if (InitPLC(ref strErrorMessage) == false)
                    {
                        return false;
                    }

                    // Start Timer
                    DisplayMessage("Start Timer.");
                    PLCWatchDog = new System.Threading.Timer(Thread_Timer_PLC_WatchDog, null, 1000, 1000);
                    PLCTimer = new System.Threading.Timer(Thread_Timer_PLC, null, 1000, 1000);
                }
              
                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "InitRun Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private void RunStart()
        {
            string strErrorMessage = "";
            isTestRunning = true;

            try
            {
                InitButtons(false);
                KillAdbProcess();

                RunTest();

                KillAdbProcess();
                InitButtons(true);
            }
            catch (Exception ex)
            {
                strErrorMessage = "RunStart Exception: " + ex.Message;
                return;
            }
            finally
            {
                isTestRunning = false;     
                clsUtil.Dly(1.0);
            }
        }

        private bool RunTest()
        {
            string strErrorMessage = "";

            bool bRes = false;     
            bool bFlag = false;    
            long lStartTime = 0;

            //double dTestTotalTime = 0;
            //m_bRunning = true;

            #region Init

            // 当获得到SN后，才开始初始化本产品的log
            //if (InitLog4net() == false)
            //{
            //    DisplayMessage("Init Log4net Fail.");
            //    return false;
            //}

            ClearTestLog();

            #endregion

            #region Init Variable

            InitMDCSVariable();
           
            #endregion

            WriteTestReportHeader();
            lStartTime = clsUtil.StartTimeInTicks(); 
     
            try
            {           

                InitTestItemList(ref strErrorMessage);

                #region TestItem

                bRes = RunTestItem();

                #endregion

                #region TestEnd



                #endregion

                #region Save Data

                // MDCS
                string MDCS_Enable = GetTestConfig("MDCS", "Enable").ToUpper();           
                if (MDCS_Enable == "TRUE")
                {
                    clsCommonFunction.DeleteMDCSSqueueXmlFile();

                    DisplayMessage("Upload to MDCS ...");  
                    if (SaveDataToMDCS(ref strErrorMessage) == false)
                    {
                        DisplayMessage("Fail to Save MDCS Data." + strErrorMessage, "ERROR");
                        return false;
                    }

                    DisplayMessage("Upload to MDCS Successful.");
                }
                else
                {
                    DisplayMessage("Skip to Upload MDCS.");
                }
            
                // MES
                if (m_bMESEnable == true)
                {
                    DisplayMessage("Upload to MES ...");
                    if (UploadToMES(ref strErrorMessage) == false)
                    {
                        DisplayMessage("Fail to Upload MES Data." + strErrorMessage, "ERROR");
                        return false;
                    }

                    DisplayMessage("Upload to MES Successful.");
                }
                else
                {
                    DisplayMessage("Skip to Upload MES.");
                }

                #endregion

            
                WriteTestReportBooter();
                m_stTestSaveData.TestRecord.TestTotalTime = clsUtil.ElapseTimeInSeconds(lStartTime).ToString("F1");
                DisplayMessage("Total TestTime: " + m_stTestSaveData.TestRecord.TestTotalTime + "s");
            }
            catch(Exception ex)
            {
                strErrorMessage = "RunTest Exception:" + ex.Message;



                return false;
            }
            finally
            {



                #region Show Result


                #endregion

                // End unit log file

            }

            return true;  
        }

        private bool RunTestItem()
        {
            string strErrorMessage = "";
            string strTestItem = "";
            bool bFailFlag = false;
            bool bRes = false;
              
            if (m_TestItemList.Count() < 1)
            {
                strErrorMessage = "TestItem List is Empty !!!";
                return false;
            }

            clsSensorKBase sensorKTest = clsSensorKTestFactory.CreateSensorKTest(m_Type);

            try
            {
                for (int i = 0; i < m_TestItemList.Count(); i++)
                {
                    bFailFlag = false;
                    // Init MDCS Data
                    m_stTestSaveData.TestResult.TestStatus = "1";
                    m_stTestSaveData.TestResult.TestFailCode = 0;
                    m_stTestSaveData.TestResult.TestFailMessage = "";

                    #region User Stop

                    if (m_bStop == true)
                    {
                        bFailFlag = true;
                        strErrorMessage = "User Stop Test !!!";
                        DisplayMessage(strErrorMessage);
                        break;
                    }

                    #endregion

                    strTestItem = m_TestItemList[i]; 
                    ShowTestItem(strTestItem);
                    DisplayMessage("Test Item: " + strTestItem);    
                    DisplayMessage("Start TestTime: " + DateTime.Now.ToString("HH:mm:ss:ff"));

                    var MethodInfo = typeof(clsSensorKBase).GetMethod(strTestItem);
                    if (MethodInfo == null)
                    {
                        bFailFlag = true;
                        DisplayMessage("Failed to Function Reflection: " + strTestItem);
                        m_stTestSaveData.TestResult.TestPassed = false;
                    }
                    else
                    {
                        bRes = false;
                        bRes = (bool)MethodInfo.Invoke(sensorKTest, null);  // Call TestMethod
                        if (bRes == true)
                        {
                            m_stTestSaveData.TestResult.TestPassed = true;
                            DisplayMessage("SubtestIterate PASS.");
                        }
                        else
                        {
                            bFailFlag = true;
                            m_stTestSaveData.TestResult.TestPassed = false;
                            DisplayMessage("SubtestIterate FAIL.");
                            break;
                        }

                        MethodInfo = null;
                    }

                    DisplayMessage("Completed !!!" + "\r\n");
                    System.Threading.Thread.Sleep(50);
                    Application.DoEvents();
                }



                
            }
            catch(Exception ex)
            {
                strErrorMessage = "RunTestItem Exception: " + ex.Message;
                return false;
            }

            return true;
        }

        private bool InitHW(ref string strErrorMessage)
        {
            strErrorMessage = "";
            m_bRunInitialized = false;

            try
            { 
                // DAQ
                if (clsConfigHelper.Daq.Enable == true)
                {
                    if (clsConfigHelper.Daq.DeviceType == "NI")
                    {
                        DisplayMessage("Init NI DAQ.");
                        if (m_objEquipmentInitial.InitNIDAQ(clsConfigHelper.Daq.DeviceName) == false)
                        {
                            strErrorMessage = "Init NI DAQ Fail !!!";
                            return false;
                        }  

                    }
                    else if (clsConfigHelper.Daq.DeviceType == "JY-DAM")
                    {
                        DisplayMessage("Init JY-DAM DAQ.");
                        if (m_objEquipmentInitial.InitJYDAMDAQ() == false)
                        {
                            strErrorMessage = "Init JY-DAM DAQ Fail !!!";
                            return false;
                        }  
                    }
                    DisplayMessage("Init DAQ Completed.");
                }

                // MOTOR
                if (clsConfigHelper.servoMotor.Enable == true)
                {
                    if (clsConfigHelper.servoMotor.DeviceType == "OMORN")
                    {
                        DisplayMessage("Init OMORN Motor.");
                        if (m_objEquipmentInitial.InitOMORNMotor(ref strErrorMessage) == false)
                        {
                            strErrorMessage = "Init OMORN Motor Fail !!!" + strErrorMessage;
                            return false;        
                        }
                    }
                    else if (clsConfigHelper.servoMotor.DeviceType == "PANASONIC")
                    {
                        DisplayMessage("Init PANASONIC Motor.");
                        if (m_objEquipmentInitial.InitPANASONICMotor() == false)
                        {
                            strErrorMessage = "Init Panasonic Motor Fail !!!";
                            return false;
                        }
                    }
                    DisplayMessage("Init Motor Completed.");
                }
       
                m_bRunInitialized = true;
            }
            catch(Exception ex)
            {
                strErrorMessage = "InitHW Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool ReleaseHW(ref string strErrorMessage)
        {
            try
            {
                if (clsConfigHelper.Daq.Enable == true)
                {
                    if (clsConfigHelper.Daq.DeviceType == "NI")
                    {
                        if (m_objEquipmentInitial.m_objNIDAQ != null)
                        {
                            m_objEquipmentInitial.m_objNIDAQ.Reset();
                            m_objEquipmentInitial.m_objNIDAQ = null;
                        }
                    }
                }

                if (clsConfigHelper.servoMotor.Enable == true)
                {
                    if (m_objEquipmentInitial.m_objOMORN != null)
                    {
                        m_objEquipmentInitial.m_objOMORN.PortClose(ref strErrorMessage);
                    }     
                }
            
            
            }
            catch(Exception ex)
            {
                strErrorMessage = "ReleaseHW Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool KillAdbProcess()
        {
            try
            {
                clsExecProcess objprocess = new clsExecProcess();
                string strProcess = "adb";
                if (objprocess.KillProcess(strProcess) == false)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string strr = ex.Message;
                return false;
            }

            return true;
        }

        private bool InitLog4net()
        {
            try
            {
                string strSN = "";
                string strDate = DateTime.Now.ToString("yyyyMMdd_hhmmss");
                string logFileName = string.Format("Debug_{0}_{1}.log", strSN, strDate);
                string folder = Application.StartupPath + @"\log\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\"; ;
               
                logFileName = folder + logFileName;
                Logger.ChangeLogFileName("RollingFile", logFileName);
            }
            catch(Exception ex)
            {
                string strErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private bool LoadTest(ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                #region Load TestItem XML

                DisplayMessage("Load TestItem XML File");
                // TestItem Enable Param
                if (LoadTestItemList(frmMain.m_strTestItemXMLFile, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Load TestItemList Enable Param fail." + strErrorMessage;
                    return false;
                }

                // TestItem Param
                if (LoadTestItemParameter(frmMain.m_strTestItemXMLFile, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Load TestItemList Config Param fail." + strErrorMessage;
                    return false;
                }

                #endregion

                InitTestItemList(ref strErrorMessage);

            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private void InitMDCSVariable()
        {
            // TestResult
            m_stTestSaveData.TestResult.TestPassed = false;
            m_stTestSaveData.TestResult.TestFailCode = 0;
            m_stTestSaveData.TestResult.TestFailMessage = "";
            m_stTestSaveData.TestResult.TestStatus = "";

            // TestRecord
            m_stTestSaveData.TestRecord.ToolNumber = Program.g_strToolNumber;
            m_stTestSaveData.TestRecord.ToolRev = Program.g_strToolRev;
            m_stTestSaveData.TestRecord.SKU = "";
            m_stTestSaveData.TestRecord.Model = "";
            m_stTestSaveData.TestRecord.SN = "";
            m_stTestSaveData.TestRecord.IMEI = "";
            m_stTestSaveData.TestRecord.MEID = "";
            m_stTestSaveData.TestRecord.IMEI2 = "";
            m_stTestSaveData.TestRecord.MEID2 = "";
            m_stTestSaveData.TestRecord.EID = "";
            m_stTestSaveData.TestRecord.WorkOrder = "";
            m_stTestSaveData.TestRecord.PCBAVendor = "";
            m_stTestSaveData.TestRecord.HWVersion = "";
            m_stTestSaveData.TestRecord.AndroidOSVersion = "";
            m_stTestSaveData.TestRecord.AudioPAName = "";
            m_stTestSaveData.TestRecord.ConfigurationNumber = "";
            m_stTestSaveData.TestRecord.BarometerOffsetValue = "0";
            m_stTestSaveData.TestRecord.SoftwareVersionControl = "";

            // TestItem Result
            m_stTestSaveData.TestRecord.TestGSensorCalibration = "NA";
            m_stTestSaveData.TestRecord.TestGYROSensorCalibration = "NA";
            m_stTestSaveData.TestRecord.TestPSensorCalibration = "NA";
            m_stTestSaveData.TestRecord.TestPSensorFunction = "NA";
            m_stTestSaveData.TestRecord.TestAudioCalibration = "NA";
            m_stTestSaveData.TestRecord.TestBarometerCalibration = "NA";

            // Before
            m_stTestSaveData.TestRecord.ACCEL_ZERO_OFFSET_BEFORE = "";
            m_stTestSaveData.TestRecord.ACCELEROMETER_CALIBRATION_BEFORE = "";
            m_stTestSaveData.TestRecord.GYRO_ZERO_OFFSET_BEFORE = "";
            m_stTestSaveData.TestRecord.GYROSCOPE_CALIBRATION_BEFORE = "";
            m_stTestSaveData.TestRecord.PROXIMITY_CALIBRATION_BEFORE = "";
            m_stTestSaveData.TestRecord.PROXIMITY_CALIBRATION_EXTEND_BEFORE = "";
            m_stTestSaveData.TestRecord.MAX98390L_TROOM_BEFORE = "";
            m_stTestSaveData.TestRecord.MAX98390L_RDC_BEFORE = "";

            // After
            m_stTestSaveData.TestRecord.ACCEL_ZERO_OFFSET_AFTER = "";
            m_stTestSaveData.TestRecord.ACCELEROMETER_CALIBRATION_AFTER = "";
            m_stTestSaveData.TestRecord.GYRO_ZERO_OFFSET_AFTER = "";
            m_stTestSaveData.TestRecord.GYROSCOPE_CALIBRATION_AFTER = "";
            m_stTestSaveData.TestRecord.PROXIMITY_CALIBRATION_AFTER = "";
            m_stTestSaveData.TestRecord.PROXIMITY_CALIBRATION_EXTEND_AFTER = "";
            m_stTestSaveData.TestRecord.MAX98390L_TROOM_AFTER = "";
            m_stTestSaveData.TestRecord.MAX98390L_RDC_AFTER = "";

            m_stTestSaveData.TestRecord.TestTotalTime = "0.0";  
        }

        private void WriteTestReportHeader()
        {
            string startDate, startTime;
            startDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            startTime = System.DateTime.Now.ToString("HH:mm:ss");
            DisplayMessage("****************************************************************");  
            DisplayMessage("****************************  Start ****************************");
            DisplayMessage("****************************************************************");  
            DisplayMessage("Timestamp: " + startDate + " " + startTime);
            return;
        }

        private void WriteTestReportBooter()
        {
            string endDate, endTime;
            endDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            endTime = System.DateTime.Now.ToString("HH:mm:ss");
            DisplayMessage("EndTime : " + endDate + " " + endTime);
            DisplayMessage("****************************************************************");  
            DisplayMessage("****************************  End  *****************************");
            DisplayMessage("****************************************************************");  
            return;
        }

        private bool SaveDataToMDCS(ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bRes = false;
            bool bFlag = false; 
            string MDCSDeviceName = "";

            string strURL = GetTestConfig("MDCS", "URL");
            string strDeviceName = GetTestConfig("MDCS", "DeviceName");
            if (strDeviceName.IndexOf(Program.g_strToolNumber, StringComparison.OrdinalIgnoreCase) == -1)
            {
                strErrorMessage = "Invalid MDCS Device Name !!!";
                return false;
            }
           
            try
            {
                #region Confirm Device Name

                string[] DeviceNameList = strDeviceName.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                if (DeviceNameList.Length > 1) // Multi-DeviceName
                {        
                    for (int i = 0; i < DeviceNameList.Length; i++)
                    {               
                        string deviceName = DeviceNameList[i].Trim();
                        //string pattern = @"\b" + m_strModel + @"\b(?!.*\b" + m_strModel + @"\b)";
                        string pattern = string.Format("^(?!.*{0}.*$).*$", Regex.Escape(m_strModel));

                        bool isMatch = Regex.IsMatch(deviceName, pattern);             
                        if (isMatch)
                        {
                            MDCSDeviceName = deviceName;
                            bFlag = true;
                            break;             
                        }
                        else
                        {
                            strErrorMessage = "The DeviceName is Not Match model!, DeviceName: " + deviceName;
                            bFlag = false;
                            continue;
                        }
                    }
                    if (bFlag == false)
                    {
                        strErrorMessage = "Can't find Matched Device Name, " + strErrorMessage;
                        return false;
                    }
                }
                else  // Only One Line
                {
                    string pattern = string.Format("^(?!.*{0}.*$).*$", Regex.Escape(m_strModel));
                    bool isMatch = Regex.IsMatch(strDeviceName, pattern);   
                    if (isMatch)
                    {
                        MDCSDeviceName = strDeviceName;
                    }
                    else
                    {
                        strErrorMessage = "The Device Name is Not Matched With Model !!!";
                        return false;
                    } 
                }

                DisplayMessage("Device Name: " + MDCSDeviceName);

                #endregion

                #region Send MDCS

                clsMDCS obj_SaveMDCS = new clsMDCS();
                obj_SaveMDCS.ServerName = strURL;
                obj_SaveMDCS.DeviceName = MDCSDeviceName;
                Invoke((Action)delegate()
                {
                    obj_SaveMDCS.UseModeProduction = this.radioBtnProduction.Checked;
                });
                obj_SaveMDCS.p_TestData = m_stTestSaveData;

                DisplayMessage("Send MDCS Data ......");

                for (int i = 0; i < 3; i++)
                {
                    bRes = obj_SaveMDCS.SendMDCSData(ref strErrorMessage);
                    if (bRes == false)
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
                    strErrorMessage = "Fail to Send MDCS Data !!!";
                    return false;
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

        private bool UploadToMES(ref string strErrorMessage)
        {
            strErrorMessage = "";
            bool bFlag = false;
            bool bUploadMES = false;
            bool bPassFailFlag = false;
            string strStation = "";
            string strEID = m_stTestSaveData.TestRecord.EID;     
            string strWorkOrder = m_stTestSaveData.TestRecord.WorkOrder;
            string strSN = m_stTestSaveData.TestRecord.SN;

            try
            {
                #region MES Station

                strStation = GetTestConfig("MES", "Station");
                if (string.IsNullOrWhiteSpace(strStation))
                {
                    strErrorMessage = "Fail to get MES Station Name value !";
                    return false;
                }

                #endregion

                #region Upload MES

                if (m_stTestSaveData.TestResult.TestPassed == true)
                {
                    bPassFailFlag = true;
                }
                else
                {
                    bPassFailFlag = false;
                }

                for (int i = 0; i < 3; i++)
                {
                    bUploadMES = clsUploadMES.MESUploadData(strEID, strStation, strWorkOrder, strSN, bPassFailFlag, ref strErrorMessage);
                    if (bUploadMES == false)
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
                    strErrorMessage = "Fail to Upload MES Data !!!";
                    return false;
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

        #region Load XML

        private bool LoadTestItemList(string strFilePath, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                if (clsConfigHelper.LoadTestItemList(strFilePath, ref strErrorMessage) == false)
                {
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

        private bool LoadTestItemParameter(string strFilePath, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                if (clsConfigHelper.LoadTestItemParameter(strFilePath, ref strErrorMessage) == false)
                {
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

        private bool LoadTestConfig(string strFilePath, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                if (clsConfigHelper.LoadTestConfig(strFilePath, ref strErrorMessage) == false)
                {
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

        public string GetTestItemParameter(string strItem, string strName)
        {
            string strValue = "";

            try
            { 
                if (string.IsNullOrWhiteSpace(strItem) || string.IsNullOrWhiteSpace(strName))
                {
                    return string.Empty;
                }

                strValue = clsConfigHelper.dicTestItemParamList[strItem][strName].ToString(); //.Trim();

                if (string.IsNullOrWhiteSpace(strValue))
                {
                    return string.Empty;
                }
            }
            catch(Exception ex)
            {
                string message = ex.Message;
                return string.Empty;
            }

            return strValue;
        }

        public string GetTestConfig(string strItem, string strName)
        {
            string strValue = "";

            try
            {
                if (string.IsNullOrWhiteSpace(strItem) || string.IsNullOrWhiteSpace(strName))
                {
                    return string.Empty;
                }
 
                strValue = clsConfigHelper.dicTestConfig[strItem][strName].ToString().Trim();

                if (string.IsNullOrWhiteSpace(strValue))
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return string.Empty;
            }

            return strValue;
        }

     

        #endregion

        #region UI

        public void ShowTestItem(string str_TestItem)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(str_TestItem))
                {
                    return;
                }

                if (lblTestItem.InvokeRequired)
                {
                    lblTestItem.Invoke(new Action(delegate
                    {

                        lblTestItem.ForeColor = Color.Black;
                        lblTestItem.Text = str_TestItem;
                    }));
                }
                else
                {
                    lblTestItem.ForeColor = Color.Black;
                    lblTestItem.Text = str_TestItem;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return;
            }

            return;
        }

        public void DisplayMessage(string str_Message, string level = "INFO")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str_Message))
                {
                    return;
                }

                #region Logging
                switch (level.ToUpper())
                {
                    case "DEBUG":
                        Logger.Debug(str_Message);
                        break;

                    case "INFO":
                        Logger.Info(str_Message);
                        break;

                    case "WARN":
                        Logger.Warn(str_Message);
                        break;

                    case "ERROR":
                        Logger.Error(str_Message);
                        break;

                    case "FATAL":
                        Logger.Fatal(str_Message);
                        break;

                    case "NULL":
                        // Show but not logging
                        break;

                    default:
                        break;
                }
                #endregion

                if (rtbTestLog.InvokeRequired)
                {
                    rtbTestLog.Invoke((Action)delegate()
                    {
                        rtbTestLog.AppendText(str_Message + Convert.ToChar(13) + Convert.ToChar(10));
                        rtbTestLog.ScrollToCaret();
                        rtbTestLog.Refresh();
                    });
                }
                else
                {
                    rtbTestLog.AppendText(str_Message + Convert.ToChar(13) + Convert.ToChar(10));
                    rtbTestLog.ScrollToCaret();
                    rtbTestLog.Refresh();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return;
            }

            return;
        }

        private void DisplayResult(bool bResult)
        {
            try
            {
                if (bResult)
                {
                    Invoke((Action)delegate()
                    {           
                        this.lblTestResult.Visible = true;
                        this.lblTestResult.Text = "PASS";
                        this.lblTestResult.BackColor = Color.Green;
                    });
                }
                else
                {
                    Invoke((Action)delegate()
                    {
                        this.lblTestResult.Visible = true;
                        this.lblTestResult.Text = "FAIL";
                        this.lblTestResult.BackColor = Color.Red;
                    });
                }
            }
            catch (Exception ex)
            {
                string strErrorMessage = "Exception:" + ex.Message;
                return;
            }

            return;
        }
        
        public void ClearTestLog()
        {
            if (rtbTestLog.InvokeRequired)
            {
                rtbTestLog.Invoke((Action)delegate()
                {
                    rtbTestLog.Clear();
                });
            }
            else
            {
                rtbTestLog.Clear();
            }

            return;
        }

        private void ClearTestResult()
        {
            try
            {
                lblTestResult.Text = "";
            }
            catch(Exception ex)
            {
                string strErrorMessage = "Exception:" + ex.Message;
                return;
            }

            return;
        }

        private void InitButtons(bool bFlag)
        {   
            Invoke((Action) delegate()
            {
                if (bFlag == true)
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                }
                else
                {
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    this.lblTestResult.Visible = false;       
                }
            });
        }

        private void InitCompleted()
        {
            Invoke((Action) delegate()
            {
                this.radioBtnProduction.Checked = true;
                this.lblProject.Text = m_strModel;
                this.lblTestItem.Text = "Init Completed !";

                if (m_stOptionData.TestMode == "1")
                {
                    this.radiobtnAuto.Checked = true;
                    this.btnStart.Visible = false;
                    this.btnStop.Visible = false;
                }
                else
                {
                    this.radiobtnManual.Checked = true;
                    this.btnStart.Visible = true;
                    this.btnStop.Visible = true;
                }          
            });
        }

        private void SetManual(bool bIsManual)
        {
            Invoke((Action)delegate()
            {
                if (bIsManual)
                {
                    this.panelButton.Visible = true;
                }
                else
                {
                    this.panelButton.Visible = false;
                }
            });
        }

        #endregion
    
    }
}
