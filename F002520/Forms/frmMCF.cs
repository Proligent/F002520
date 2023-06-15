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
    public partial class frmMCF : Form
    {

        #region Variable

        private string m_strModel = "";
        private string m_strSKU = "";

        #endregion

        #region Propery

        public string Model
        {
            get
            {
                return m_strModel;
            }
        }

        public string SKU
        {
            get
            {
                return m_strSKU;
            }
        }

        #endregion

        #region Load

        public frmMCF()
        {
            InitializeComponent();
        }

        private void frmMCF_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.txtBoxSKU.ForeColor = Color.FromArgb(105, 89, 205);
            txtBoxSKU.Focus();
        }

        #endregion

        #region Event

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;

            // Model
            //m_strModel = this.txtBoxModel.Text.Trim();
            //if (m_strModel.Length <= 0)
            //{
            //    return;
            //}

            // SKU
            m_strSKU = this.txtBoxSKU.Text.Trim().ToUpper();
            if (m_strSKU.Length <= 0)
            {
                return;
            }

            this.DialogResult = DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void txtBoxSKU_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                txtBoxSKU.Text = "CT45L1N-27D120G";
                btnOK.Focus();
            }
            if (e.KeyCode == Keys.F2)
            {
                txtBoxSKU.Text = "CT47-X1N-3ED120G";
                btnOK.Focus();
            }

            if (e.KeyValue == 13)
            {
                btnOK.Focus();
            }
        }

        #endregion

    }
}
