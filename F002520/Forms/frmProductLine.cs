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
    public partial class frmProductLine : Form
    {

        #region Variable

        private string m_sProductLine = "";

        #endregion

        #region Propery

        public string ProductionLine
        {
            get
            {
                return m_sProductLine;
            }
        }

        #endregion

        #region Load

        public frmProductLine()
        {
            InitializeComponent();
        }

        private void frmProductLine_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.comboxPdLine.SelectedIndex = 1; // Select First Selection
            this.comboxPdLine.Focus();
        }

        #endregion

        #region Event

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;

            m_sProductLine = this.comboxPdLine.Text.ToUpper();
            if (string.IsNullOrWhiteSpace(m_sProductLine))
            {
                return;
            }

            this.DialogResult = DialogResult.Yes;
        }

        #endregion
    }
}
