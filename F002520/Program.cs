using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F002520
{
    /****************************************************************************************
     *  
     *  History Log:
     *  REV     AUTHOR      DATE            COMMENTS
     *  A       CalvinXie   2023/03/15      First Version.
     *  
     *  
     ****************************************************************************************/

    static class Program
    {
        public static frmMain g_mainForm;
        public static string g_strToolNumber = "";
        public static string g_strToolRev = "";
        private static System.Threading.Mutex mutex;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            g_strToolNumber = "F002520";
            g_strToolRev = "01.01";

            mutex = new System.Threading.Mutex(false, "F002520 SensorK Fixture");
            if (!mutex.WaitOne(0, false))
            {
                mutex.Close();
                mutex = null;
            }
            if (mutex != null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                g_mainForm = new frmMain();
                Application.Run(g_mainForm);
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("F002520 Already Running !!!");
            }
        }
    }
}
