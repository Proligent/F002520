using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    class clsExecProcess
    {
        #region Variable

        private string m_str_ErrMsg = "";

        #endregion

        #region Property

        public string ErrMsg
        {
            get
            {
                return m_str_ErrMsg;
            }
        }

        #endregion

        #region Construct

        public clsExecProcess()
        {
        }

        #endregion

        #region Function

        public bool ExcuteCmd(string str_cmd)
        {
            // 检查输入参数
            if (str_cmd == "")
            {
                return false;
            }

            try
            {
                // 实例一个Process类
                Process p = new Process();

                // Process类有一个StartInfo属性 
                p.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";   // 设定程序名
                p.StartInfo.Arguments = " /c " + str_cmd;                  // 设定程式执行参数 /c是关闭Shell的使用   
                p.StartInfo.UseShellExecute = false;                   // 直接启动进程
                p.StartInfo.RedirectStandardInput = false;             // 重定向标准输入
                p.StartInfo.RedirectStandardOutput = false;            // 重定向标准输出   
                p.StartInfo.RedirectStandardError = false;             // 重定向错误输出 
                //p.StartInfo.CreateNoWindow = false;                  // 显示cmd窗口
                p.StartInfo.CreateNoWindow = true;                     // 不显示cmd窗口
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                p.Start();

                p.WaitForExit();
                p.Close();
                p.Dispose();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return false;
            }

            return true;
        }

        public bool ExcuteCmd(string str_cmd, int milliSeconds)
        {
            // 检查输入参数
            if (str_cmd == "")
            {
                return false;
            }

            try
            {
                // 实例一个Process类
                Process p = new Process();

                // Process类有一个StartInfo属性 
                p.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";   // 设定程序名
                p.StartInfo.Arguments = " /c " + str_cmd;                  // 设定程式执行参数 /c是关闭Shell的使用   
                p.StartInfo.UseShellExecute = false;                   // 直接启动进程
                p.StartInfo.RedirectStandardInput = false;             // 重定向标准输入
                p.StartInfo.RedirectStandardOutput = false;            // 重定向标准输出   
                p.StartInfo.RedirectStandardError = false;             // 重定向错误输出 
                //p.StartInfo.CreateNoWindow = false;                  // 显示cmd窗口
                p.StartInfo.CreateNoWindow = true;                     // 不显示cmd窗口
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                p.Start();

                if (milliSeconds == 0)
                {
                    p.WaitForExit();
                }
                else
                {
                    p.WaitForExit(milliSeconds);    //等待进程结束，等待时间为指定的毫秒
                }

                p.Close();
                p.Dispose();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return false;
            }

            return true;
        }

        public bool ExcuteCmd(string str_cmd, string str_Target)
        {
            // 检查输入参数
            if (str_cmd == "")
            {
                m_str_ErrMsg = "Invaild paramter.";
                return false;
            }

            try
            {
                // 实例一个Process类
                Process p = new Process();

                // Process类有一个StartInfo属性 
                p.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";                   // 设定程序名   
                p.StartInfo.Arguments = " /c " + str_cmd;           // 设定程式执行参数 /c是关闭Shell的使用   
                p.StartInfo.UseShellExecute = false;                // 直接启动进程
                p.StartInfo.RedirectStandardInput = true;           // 重定向标准输入
                p.StartInfo.RedirectStandardOutput = true;          // 重定向标准输出   
                p.StartInfo.RedirectStandardError = true;           // 重定向错误输出 
                //p.StartInfo.CreateNoWindow = false;               // 显示cmd窗口
                p.StartInfo.CreateNoWindow = true;                  // 不显示cmd窗口

                p.Start();                                          // 启动      

                // 从输出流取得命令执行结果
                string str_Output = "";
                str_Output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
                p.Dispose();

                // 检查值
                if (str_Output.IndexOf(str_Target) == -1)
                {
                    m_str_ErrMsg = "Check return value fail.";
                    return false;
                }

                //string[] sArray = Regex.Split(str_Output, "\r", RegexOptions.IgnoreCase);
                //bool b_SearchResult = false;
                //for (int i = 0; i < sArray.Length; i++)
                //{
                //    if (sArray[i].IndexOf(str_Result) >= 0)
                //    {
                //        b_SearchResult = true;
                //    }
                //}
                //if (b_SearchResult == false)
                //{
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                m_str_ErrMsg = "Exception:" + str;
                return false;
            }

            return true;
        }

        public bool ExcuteCmd(string str_cmd, ref string str_Result)
        {
            str_Result = "";

            // 检查输入参数
            if (str_cmd == "")
            {
                m_str_ErrMsg = "Invaild paramter.";
                return false;
            }

            try
            {
                // 实例一个Process类
                Process p = new Process();

                // Process类有一个StartInfo属性 
                p.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";                   // 设定程序名   
                p.StartInfo.Arguments = " /c " + str_cmd;           // 设定程式执行参数 /c是关闭Shell的使用   
                p.StartInfo.UseShellExecute = false;                // 直接启动进程
                p.StartInfo.RedirectStandardInput = true;           // 重定向标准输入
                p.StartInfo.RedirectStandardOutput = true;          // 重定向标准输出   
                p.StartInfo.RedirectStandardError = true;           // 重定向错误输出 
                //p.StartInfo.CreateNoWindow = false;               // 显示cmd窗口
                p.StartInfo.CreateNoWindow = true;                  // 不显示cmd窗口

                p.Start();                                          // 启动      

                // 从输出流取得命令执行结果
                string str_Output = "";
                str_Output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
                p.Dispose();

                str_Result = str_Output;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                m_str_ErrMsg = "Exception:" + str;
                return false;
            }

            return true;
        }

        public bool ExcuteCmd(string str_cmd, int milliSeconds, ref string str_Result)
        {
            str_Result = "";

            // 检查输入参数
            if (str_cmd == "")
            {
                m_str_ErrMsg = "Invaild paramter.";
                return false;
            }

            try
            {
                // 实例一个Process类
                Process p = new Process();

                // Process类有一个StartInfo属性 
                p.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";                   // 设定程序名   
                p.StartInfo.Arguments = " /c " + str_cmd;           // 设定程式执行参数 /c是关闭Shell的使用   
                p.StartInfo.UseShellExecute = false;                // 直接启动进程
                p.StartInfo.RedirectStandardInput = true;           // 重定向标准输入
                p.StartInfo.RedirectStandardOutput = true;          // 重定向标准输出   
                p.StartInfo.RedirectStandardError = true;           // 重定向错误输出 
                //p.StartInfo.CreateNoWindow = false;               // 显示cmd窗口
                p.StartInfo.CreateNoWindow = true;                  // 不显示cmd窗口

                p.Start();                                          // 启动      

                if (milliSeconds == 0)
                {
                    p.WaitForExit();
                }
                else
                {
                    p.WaitForExit(milliSeconds);                    //等待进程结束，等待时间为指定的毫秒
                }

                // 从输出流取得命令执行结果
                str_Result = p.StandardOutput.ReadToEnd().Trim();
   
                p.Close();
                p.Dispose();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                m_str_ErrMsg = "Exception:" + str;
                return false;
            }

            return true;
        }

        public bool ExcuteCmd(string str_cmd, int milliSeconds, ref List<string> ResultList)
        { 
            // 检查输入参数
            if (str_cmd == "")
            {
                m_str_ErrMsg = "Invaild paramter.";
                return false;
            }

            List<string> List = new List<string>();
            List.Clear();

            Process process = new Process();

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
                startInfo.Arguments = " /c " + str_cmd;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;           // 重定向错误输出 
                startInfo.CreateNoWindow = true;
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo = startInfo;

                process.Start();

                if (milliSeconds == 0)
                {
                    process.WaitForExit();
                }
                else
                {
                    process.WaitForExit(milliSeconds);                    //等待进程结束，等待时间为指定的毫秒
                }

                string line = "";
                StreamReader streamReader = process.StandardOutput;
                while (((line = streamReader.ReadLine()) != null))
                {
                    if (line.Contains("id="))
                    {
                        List.Add(line);
                    }
                }

                ResultList = List;
            }
            catch (Exception ex)
            {
                m_str_ErrMsg = "Exception:" + ex.Message;
                return false;
            }
            finally
            {   
                if (process != null)
                {
                    process.Dispose();
                }
            }
          
             return true;
        }

        public bool FindProcess(string str_ProcessName)
        {
            if (str_ProcessName == "")
            {
                return false;
            }

            int PID = -1;
            try
            {
                Process[] arrP = Process.GetProcesses();
                foreach (Process p in arrP)
                {
                    if (p.ProcessName == str_ProcessName)
                    {
                        PID = p.Id;
                        break;
                    }
                }
            }
            catch
            {
                return false;
            }

            if (PID == -1)
            {
                return false;
            }

            return true;
        }

        public bool KillProcess(string str_ProcessName)
        {
            if (str_ProcessName == "")
            {
                return false;
            }

            try
            {
                Process[] arrP = Process.GetProcesses();
                foreach (Process p in arrP)
                {
                    if (p.ProcessName == str_ProcessName)
                    {
                        p.Kill();
                        break;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

    }
}
