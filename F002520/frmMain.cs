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
            //public string ScanSheetStation;

            // MES
            public string MES_Enable;
            public string MES_Station;

            // Software Version Control
            //public string SWVersionControl;

            // XML
            public string TestItemXMLFile;
            public string EquipmentXMLFile;

            // DAQ
            public string DAQDevice;

            // PLC
            public string PLCIP;
            public string PLCPort;
            public int ReadDB;
            public int WriteDB;
            public string Location;

            // MODBUS
            public ComPortSetting ModbusSetting;
            public string Home;

            // MES
            //public string MESEnable;
            //public string MESStation;

            // MDCS
            //public string MDCSEnable;
            //public string MDCSMode;
            //public string MDCSURL;
            //public string MDCSDeviceName;      
            //public string PreStationResultCheck;
            //public string PreStationDeviceName;
            //public string PreStationVarName;
            //public string PreStationVarValue;    
        }

        #endregion

        #region Variable

        private bool m_bStop = false;
        private bool m_bRunning = false;
        private bool m_bRunInitialized = false;
        //private int m_nProductID = 0;

        private string m_strSKU = "";
        public string m_strModel = "";
        public static string m_strTestItemXMLFile = "";
        public static string m_strEquipmentXMLFile = "";

        private MCFData m_stMCFData = new MCFData();
        private MESData m_stMESData = new MESData();
        //private UnitDeviceInfo m_stUnitDeviceInfo = new UnitDeviceInfo();

        private List<string> m_TestItemList = new List<string>();
        public OptionData m_stOptionData = new OptionData();
        private clsConfigHelper m_objXmlConfig = new clsConfigHelper();
        public clsEquipmentInitial m_objEquipmentInitial = new clsEquipmentInitial();

 
        // PLC
        private clsPLCDaveHelper m_PLC;
        private string sErrorPlcThread = "";
        private string sResultPlcThread = "";
        //private string sPLCEnableReadBarcode = "";
        private System.Threading.Timer PLCTimer = null;
        private System.Threading.Timer PLCWatchDog = null;
        private int iWtchDog = 0;
        private bool isTestRun = true; // true:没有测试  false:测试中
        private bool isPLCRun = false;
        private bool isManual = false;
        private bool bPLCThreadRun = false;



        // MES

        // MDCS
        public clsMDCS m_objMDCS;
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
            lblTitleBar.Text = Program.g_strToolNumber + " Sensor Calibration Fixture";
            lblVersion.Text = Program.g_strToolRev;

            // Init Log
            log4net.GlobalContext.Properties["DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
            log4net.GlobalContext.Properties["LogFileName"] = "Prepare";
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            Logger.Info("Form Load");

            // Init Run
            if (InitRun(ref strErrorMessage) == false)
            {
                strErrorMessage = "InitRun fail, " + strErrorMessage;
                DisplayMessage(strErrorMessage);
                MessageBox.Show(strErrorMessage);
                return;
            }
        
            InitCompleted();
            Logger.Info("Load Completed.");

            return;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            //ReleaseHW();
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

        #region Obsolote

        //private bool SelectModelRun(ref string strErrorMessage)
        //{
        //    if (m_nProductID == 0)
        //    {
        //        strErrorMessage = "Invalid Product ID !!!";
        //        return false;
        //    }

        //    switch(m_nProductID)
        //    {
        //        case (int)ModelID.CT40:
        //        case (int)ModelID.CT40P:
        //            m_objSensorK = new clsCT40SensorK();
        //            m_objSensorK.Start();
        //            break;

        //        case (int)ModelID.CT45:
        //        case (int)ModelID.CT45P:
        //            m_objSensorK = new clsCT45SensorK();
        //            m_objSensorK.Start();
        //            break;

        //        case (int)ModelID.CT47:
        //            m_objSensorK = new clsCT47SensorK();
        //            m_objSensorK.Start();
        //            break;

        //        case (int)ModelID.CW45:
        //            m_objSensorK = new clsCW45SensorK();
        //            m_objSensorK.Start();
        //            break;

        //        default:

        //            break;
        //    }


        //    strErrorMessage = "";
        //    return true;
        //}

        #endregion


        #region PLC

        private bool InitPLC(ref string sErrorMessage)
        {
            if (clsConfigHelper.plcConfig.Enable == true)
            {
                m_PLC = new clsPLCDaveHelper();

                if (m_PLC.Connect(clsConfigHelper.plcConfig.PLCIP, clsConfigHelper.plcConfig.PLCPort, ref sErrorMessage) == false)
                {
                    isPLCRun = false;
                    m_PLC.DisConnect(ref sErrorMessage);
                    sErrorMessage = "PLC Connect Failure!";
                    return false;
                }
                else
                {
                    isPLCRun = true;
                }
            }
            else
            {
                isPLCRun = false;
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
                iWtchDog %= 255;

                if (m_PLC.WatchDogAdd(clsConfigHelper.plcConfig.WriteDB, iWtchDog.ToString()) == false)
                {
                    isPLCRun = false;
                    DisplayMessage("PLC:WatchDog fail.");          
                    return;
                }
                else
                {     
                    iWtchDog++;
                }
            }
            catch (Exception ex)
            {
                isPLCRun = false;
                DisplayMessage("PLC:WatchDog Exception:" + ex.Message.ToString());   
                return;
            }
            finally
            {
                if (isPLCRun == false)
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

                if (isTestRun == true && isPLCRun == true)
                {
                    #region Read Auto/Manual

                    DisplayMessage("PLC:Read Read Auto or Manual.");
                    if (m_PLC.ReadAutoManual(clsConfigHelper.plcConfig.ReadDB, ref sResultPlcThread, ref sErrorPlcThread) == false)
                    {
                        isManual = true;   
                        SetManual(isManual);
                        DisplayMessage("PLC:ReadAutoManual fail.");
                        m_PLC.DisConnect(ref sErrorPlcThread);
                    }
                    else
                    {
                        DisplayMessage("PLC:Read Read Auto or Manual SUCCESS.");
                        // Auto
                        if (sResultPlcThread == "1")
                        {
                            isManual = false;
                            SetManual(isManual);
                        }

                        // Manual
                        else if (sResultPlcThread == "2")
                        {
                            isManual = true;
                            SetManual(isManual);
                        }
                        else
                        {
                            // NA
                            //SetAutomationStatus("Invalid");
                        }
                    }

                    #endregion

                    #region ProductDetect

                    DisplayMessage("Detect Product ......");
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
                            //SetProductStatus("ERROR");
                            //FrmMain.TraceLog("PLC:Send BUSY fail.");
                            //FrmMain.AutomationLog("PLC:Send BUSY status FAIL.");
                            return;
                        }
                        else
                        {
                            //SetProductStatus("BUSY");
                            //FrmMain.AutomationLog("PLC:Send BUSY status SUCCESS.");
                        }

                        #endregion

                        #region Start

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
                    isPLCRun = false;
                    DisplayMessage("PLC:Connect fail.");      
                    return false;
                }
                else
                {
                    isPLCRun = true;
                }
            }
            catch (Exception ex)
            {
                isPLCRun = false;
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

                    if ((dValue_AI0 > 3.0) && (dValue_AI1 < 2.0))
                    {
                        bFlag = true;
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
                    if (dValue > 3.0)
                    {
                        m_PLC.CurrentStatusReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnStatus.HAVEPRODUCT, ref strErrorMessage);
                    }
                    else
                    {
                        m_PLC.CurrentStatusReturn(clsConfigHelper.plcConfig.WriteDB, clsPLCDaveHelper.ReturnStatus.READY, ref strErrorMessage);
                    }

                    #endregion

                    #region WaitForSignal

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
                DisplayMessage("PLC:Read Wait for PLC Singal.");
                string sResult = "";
                m_PLC.ReadCommand(clsConfigHelper.plcConfig.ReadDB, ref sResult, ref sError);
                if (sResult == "1")
                {
                    DisplayMessage("PLC:Read Wait for PLC Singal: " + sResult);
                    return true;
                }
                else
                {
                    sError = "PLC Wait For Singal fail. Res: ";
                    DisplayMessage(sError + sResult);
                    return false;
                }
            }
            catch (Exception ex)
            {
                sError = "Exception:" + ex.Message;
                //FrmMain.TraceLog("waitForSingal Exception:" + ex.Message);
                //FrmMain.AutomationLog("PLC:Read wait for PLC singal exception.");
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

                strModel = strSKU.Substring(0, iIndex).Trim().ToUpper();
                if (strModel.Contains("L"))
                {
                    iIndex = strModel.IndexOf("L");
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
                    strErrorMessage = "Invalid AutoChangeOver Enable Value: " + m_stOptionData.AutoChangeOver;
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
                    strErrorMessage = "Invalid MES_Enable Value: " + m_stOptionData.MES_Enable;
                    return false;
                }

                m_stOptionData.MES_Station = objOptionFile.ReadString("MES", "Station");
                if (m_stOptionData.MES_Enable == "1" && string.IsNullOrWhiteSpace(m_stOptionData.MES_Station))
                {
                    strErrorMessage = "Invalid MES Station Value: " + m_stOptionData.MES_Station;
                    return false;
                }

                #endregion

                #region SW Version Control

                //m_stOptionData.SWVersionControl = objOptionFile.ReadString("SoftwareVersionControl", "Enable");
                //if ((m_stOptionData.SWVersionControl != "0") && (m_stOptionData.SWVersionControl != "1"))
                //{
                //    strErrorMessage = "Invalid SWVersionControl Value: " + m_stOptionData.SWVersionControl;
                //    return false;
                //}

                #endregion

                #region XML

                // TestItem
                //m_stOptionData.TestItemXMLFile = objOptionFile.ReadString("XML", "TestItem");
                //if (   (m_stOptionData.TestItemXMLFile.Contains("CT40") == false)
                //    && (m_stOptionData.TestItemXMLFile.Contains("CT45") == false)
                //    && (m_stOptionData.TestItemXMLFile.Contains("CT47") == false)
                //    && (m_stOptionData.TestItemXMLFile.Contains("CW45") == false) )
                //{
                //    strErrorMessage = "Invalid TestItemXMLFile Name: " + m_stOptionData.TestItemXMLFile;
                //    return false;
                //}
                //m_strTestItemXMLFile = m_stOptionData.TestItemXMLFile;

                // Get Model
                //int index = m_stOptionData.TestItemXMLFile.IndexOf("_");
                //m_strModel = m_stOptionData.TestItemXMLFile.Substring(0, index).Trim().ToUpper();
                //if ((m_strModel != "CT40") && (m_strModel != "CT45") && (m_strModel != "CT47") && (m_strModel != "CW45"))
                //{
                //    strErrorMessage = "Invalid Model Value, Please Check TestItem XML File Name: " + m_stOptionData.TestItemXMLFile;
                //    return false;
                //}

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
                foreach(string key in m_objXmlConfig.dicTestItemList.Keys)
                {    
                    if (m_objXmlConfig.dicTestItemList[key] == true)
                    {
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
                this.lblTestItem.BackColor = Color.YellowGreen;
                this.radioButProduction.Checked = true;
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
                    #region Select Production Line

                    if (SelectLine() == false)
                    {
                        strErrorMessage = "Failed to select production line.";
                        return false;
                    }
                    DisplayMessage("Production Line: " + m_strModel);

                    #endregion
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

                DisplayMessage("Load TestItem XML File.");
                m_strTestItemXMLFile = string.Format("{0}_SensorK_TestItem.xml", m_strModel);   // TestItem XML File Name
                DisplayMessage("FileName: " + m_strTestItemXMLFile); 
                m_strTestItemXMLFile = Application.StartupPath + @"\TestConfig\TestItem\" + m_strModel + "\\" + m_strTestItemXMLFile;

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

                DisplayMessage("Load TestItem XML File Success.");

                #endregion

                #region Init Hardware

                if (InitHW(ref strErrorMessage) == false)
                {
                    strErrorMessage = "Failed to Init Hardware: " + strErrorMessage;
                    return false;
                }

                #endregion

                #region Init PLC

                if (m_stOptionData.TestMode == "1")
                {
                    if (InitPLC(ref strErrorMessage) == false)
                    {
                        return false;
                    }

                    // Start Timer
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
                string strErrorMessage = "RunStart Exception: " + ex.Message;
                return;
            }
            finally
            {
                isTestRun = true;     
                clsUtil.Dly(1.0);
            }
        }

        private bool RunTest()
        {
            string strErrorMessage = "";
            bool bRes = false;
            //bool bFlag = false;           
            long lStartTime = 0;
            double dTestTotalTime = 0;
            //string strTestItem = "";

            m_bRunning = true;
            this.rtbTestLog.Clear();

            #region Init Log

            // 当获得到SN后，才开始初始化本产品的log
            if (InitLog4net() == false)
            {
                DisplayMessage("Init Log4net Fail.");
                return false;
            }

            #endregion

            #region Init Variable

            InitMDCSVariable();
            InitUnitDeviceInfo();
           

            #endregion

            WriteTestReportHeader();
            lStartTime = clsUtil.StartTimeInTicks(); 
            m_stTestSaveData.TestResult.TestPassed = false;
  
            try
            {           
                InitTestItemList(ref strErrorMessage);

                #region Test

                bRes = RunTestItem();

                #endregion

                #region TestEnd


                #endregion

                #region Save Data

            

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
                
            }

            return true;  
        }

        private bool RunTestItem()
        {
            string strErrorMessage = "";
            string strTestItem = "";
            bool bRes = false;
            //bool bFlag = false;           
       
            if (m_TestItemList.Count() < 1)
            {
                strErrorMessage = "TestItem List is Empty !!!";
                return false;
            }

            clsSensorKBase sensorKTest = clsSensorKTestFactory.CreateSensorKTest(m_strModel);

            try
            {
                for (int i = 0; i < m_TestItemList.Count(); i++)
                {            
                    // Init MDCS Data
                    m_stTestSaveData.TestResult.TestStatus = "1";
                    m_stTestSaveData.TestResult.TestFailCode = 0;
                    m_stTestSaveData.TestResult.TestFailMessage = "";

                    strTestItem = m_TestItemList[i]; 
                    ShowTestItem(strTestItem);
                    DisplayMessage("Test Item: " + strTestItem);    
                    DisplayMessage("Start TestTime: " + DateTime.Now.ToString("HH:mm:ss:ff"));

                    var MethodInfo = typeof(clsSensorKBase).GetMethod(strTestItem);
                    if (MethodInfo == null)
                    {
                        DisplayMessage("Failed to Function Reflection: " + strTestItem);
                        m_stTestSaveData.TestResult.TestPassed = false;
                    }
                    else
                    {
                        bRes = false;
                        bRes = (bool)MethodInfo.Invoke(sensorKTest, null);
                        if (bRes == true)
                        {
                            m_stTestSaveData.TestResult.TestPassed = true;
                            DisplayMessage("SubtestIterate PASS.");
                        }
                        else
                        {
                            m_stTestSaveData.TestResult.TestPassed = false;
                            DisplayMessage("SubtestIterate FAIL.");
                        }

                        MethodInfo = null;
                    }

                    DisplayMessage("Completed" + "\r\n");
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
                        if (m_objEquipmentInitial.InitNIDAQ(clsConfigHelper.Daq.DeviceName) == false)
                        {
                            strErrorMessage = "Init NI DAQ Fail !!!";
                            return false;
                        }  
                    }
                    else if (clsConfigHelper.Daq.DeviceType == "JY-DAM")
                    {
                        if (m_objEquipmentInitial.InitJYDAMDAQ() == false)
                        {
                            strErrorMessage = "Init JY-DAM DAQ Fail !!!";
                            return false;
                        }  
                    }
                }

                // MOTOR
                if (clsConfigHelper.servoMotor.Enable == true)
                {
                    if (clsConfigHelper.servoMotor.DeviceType == "OMORN")
                    {
                        if (m_objEquipmentInitial.InitOMORNMotor(ref strErrorMessage) == false)
                        {
                            strErrorMessage = "Init OMORN Motor Fail !!!" + strErrorMessage;
                            return false;        
                        }
                    }
                    else if (clsConfigHelper.servoMotor.DeviceType == "PANASONIC")
                    {
                        if (m_objEquipmentInitial.InitPANASONICMotor() == false)
                        {
                            strErrorMessage = "Init Panasonic Motor Fail !!!";
                            return false;
                        }
                    }
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
                string strDate = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
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

        private void InitUnitDeviceInfo()
        {
            //m_stUnitDeviceInfo.SN = "";
            //m_stUnitDeviceInfo.SKU = "";
            //m_stUnitDeviceInfo.Model = "";
            //m_stUnitDeviceInfo.IMEI = "";
            //m_stUnitDeviceInfo.MEID = "";
            //m_stUnitDeviceInfo.IMEI2 = "";
            //m_stUnitDeviceInfo.MEID2 = "";
            //m_stUnitDeviceInfo.Vendor = "";
            //m_stUnitDeviceInfo.HWVersion = "";
            //m_stUnitDeviceInfo.AndroidOS = "";
            //m_stUnitDeviceInfo.AudioPAName = "";
            //m_stUnitDeviceInfo.ConfigNumber = "";
            //m_stUnitDeviceInfo.EID = "";
            //m_stUnitDeviceInfo.WorkOrder = "";
        }

        private void InitMDCSVariable()
        {
            m_stTestSaveData.TestRecord.ToolNumber = Program.g_strToolNumber;
            m_stTestSaveData.TestRecord.ToolRev = Program.g_strToolRev;
            m_stTestSaveData.TestRecord.SKU = "";
            m_stTestSaveData.TestRecord.Model = "";
            m_stTestSaveData.TestRecord.SN = "";
            m_stTestSaveData.TestRecord.PCBAVendor = "";
            m_stTestSaveData.TestRecord.HWVersion = "";
            m_stTestSaveData.TestRecord.AndroidOSVersion = "";
            m_stTestSaveData.TestRecord.AudioPAName = "";
            m_stTestSaveData.TestRecord.ConfigurationNumber = "";
            m_stTestSaveData.TestRecord.BarometerOffsetValue = "0";
            m_stTestSaveData.TestRecord.SoftwareVersionControl = "";

            // Result
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
            DisplayMessage("***********************  Start ***********************");    
            DisplayMessage("Timestamp: " + startDate + " " + startTime);
   
            return;
        }

        private void WriteTestReportBooter()
        {
            string endDate, endTime;
            endDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            endTime = System.DateTime.Now.ToString("HH:mm:ss");
            DisplayMessage("EndTime : " + endDate + " " + endTime);
            DisplayMessage("***********************  End  ***********************");
      
            return;
        }

        #endregion


        #region Load XML

        private bool LoadTestItemList(string strFilePath, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                if (m_objXmlConfig.LoadTestItemList(strFilePath, ref strErrorMessage) == false)
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
                if (m_objXmlConfig.LoadTestItemParameter(strFilePath, ref strErrorMessage) == false)
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

        private bool LoadTestConfig(string strFilePath, string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                if (m_objXmlConfig.LoadTestConfig(strFilePath, ref strErrorMessage) == false)
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

                strValue = m_objXmlConfig.dicTestItemParamList[strItem][strName].ToString();

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

                strValue = m_objXmlConfig.dicTestConfig[strItem][strName].ToString();

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

                        lblTestItem.ForeColor = Color.Red;
                        lblTestItem.Text = str_TestItem;
                    }));
                }
                else
                {
                    lblTestItem.ForeColor = Color.Red;
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
                this.radioButProduction.Checked = true;
                this.lblProject.Text = m_strModel;
                this.lblTestItem.Text = "Init Completed !";

                if (m_stOptionData.TestMode == "1")
                {
                    this.radiobutAuto.Checked = true;
                    this.btnStart.Visible = false;
                    this.btnStop.Visible = false;
                }
                else
                {
                    this.radiobutManual.Checked = true;
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
