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
    public partial class frmMES : Form
    {

        #region Variable

        private string m_strEID = "";
        private string m_strWorkOrder = "";

        #endregion

        #region Propery

        public string EID
        {
            get
            {
                return m_strEID;
            }
        }

        public string WorkOrder
        {
            get
            {
                return m_strWorkOrder;
            }
        }

        #endregion

        public frmMES()
        {
            InitializeComponent();
        }

        private void frmMES_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            txtBoxEID.Focus();
        }

        #region Event

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;

            // EID
            m_strEID = this.txtBoxEID.Text.Trim();
            if (m_strEID.Length != 7)
            {
                return;
            }

            // WorkOrder
            m_strWorkOrder = this.txtBoxWorkOrder.Text.Trim();
            if (m_strWorkOrder.Length <= 0)
            {
                return;
            }

            this.DialogResult = DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void txtBoxEID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                txtBoxEID.Text = "S000001"; 
                txtBoxWorkOrder.Focus();
            }
            if (e.KeyValue == 13)
            {
                txtBoxWorkOrder.Focus();
            }
        }

        private void txtBoxWorkOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                txtBoxWorkOrder.Text = "20183622";
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
