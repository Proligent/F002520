﻿using F002520.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F002520
{
    public partial class frmMain : Form
    {
        #region Enum

        private enum ProductID : int
        { 
            CT40 = 1,
            CT40P,
            CT45,
            CT45P,
            CT47,
            CW45 
        }


        #endregion

        #region Struct

        private struct OptionData
        {     
            // Option.ini

        
        }


        #endregion

        #region Variable

        private bool m_bStop = false;
      
        private string m_sCurrentPath = "";
        private int m_nProductID = 0;
        private clsSensorK m_objSensorK;

        #endregion

        #region Property

        #endregion

        #region MainForm

        public frmMain()
        {
            InitializeComponent();

        }

        private void frmMain_Load(object sender, EventArgs e)
        {


        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            ReleaseHW();
        }

        #endregion

        #region Menu


        #endregion




        #region Event

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {

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

     

        #region TestItem


        #endregion

        #region Function

        private bool RunTest()
        {
            string strErrorMessage = "";

            try
            {
                #region Init

                if (m_bRunInitialized == false)
                {
                    if (InitRun() == false)
                    {
                        return false;
                    }
                    return true;
                }

                #endregion

                if (SelectModelRun(ref strErrorMessage) == false)
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                strErrorMessage = "RunTest Exception:" + ex.Message;
                return false;
            }

            return true;
        }

        private bool SelectModelRun(ref string strErrorMessage)
        {
            if (m_nProductID == 0)
            {
                strErrorMessage = "Invalid Product ID !!!";
                return false;
            }

            switch(m_nProductID)
            {
                case (int)ProductID.CT40:
                case (int)ProductID.CT40P:
                    m_objSensorK = new clsCT40_SensorK();
                    m_objSensorK.Start();
                    break;

                case (int)ProductID.CT45:
                case (int)ProductID.CT45P:
                    m_objSensorK = new clsCT45_SensorK();
                    m_objSensorK.Start();
                    break;

                case (int)ProductID.CT47:
                    m_objSensorK = new clsCT47_SensorK();
                    m_objSensorK.Start();
                    break;

                case (int)ProductID.CW45:
                    m_objSensorK = new clsCW45_SensorK();
                    m_objSensorK.Start();
                    break;

                default:

                    break;
            }



            strErrorMessage = "";
            return true;
        }

        















        #endregion







    }
}
