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
    public partial class frmFail : Form
    {
        public frmFail()
        {
            InitializeComponent();
        }

        #region Property

        public string Message
        {
            get 
            {
                return lblErrorMessage.Text;
            }
            set
            {
                lblErrorMessage.Text = value;
            } 
        }

        #endregion

        private void btnContinue_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
