using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace F002520
{

    public class Logger
    {
        private static readonly ILog Log = LogManager.GetLogger("Main");
        //private static readonly log4net.ILog Log = LogManager.GetLogger(typeof(Logger));
        //private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure(); // 加载 log4net配置文件
        }

        #region Function

        public static bool ChangeLogFileName(string AppenderName, string NewFileName)
        {
            log4net.Repository.ILoggerRepository RootRep = Log.Logger.Repository;

            foreach (log4net.Appender.IAppender iApp in RootRep.GetAppenders())
            {
                //string appenderName = iApp.Name;
                if (iApp.Name.CompareTo(AppenderName) == 0 && iApp is log4net.Appender.FileAppender)
                {
                    log4net.Appender.FileAppender fApp = (log4net.Appender.FileAppender)iApp;
                    fApp.File = NewFileName;
                    fApp.ActivateOptions();
                    return true;    // appender found and name changed to NewFileName
                }
            }

            return false;   // appender not found
        }

        #endregion

        #region Level

        public static void Debug(string message, params object[] args)
        {
            if (Log.IsDebugEnabled)
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }

                Log.Debug(message);
            }
        }

        public static void Info(string message, params object[] args)
        {
            if (Log.IsInfoEnabled)
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }

                Log.Info(message);
            }
        }

        public static void Warn(string message, params object[] args)
        {
            if (Log.IsWarnEnabled)
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }

                Log.Warn(message);
            }
        }

        public static void Error(string message, params object[] args)
        {
            if (Log.IsErrorEnabled)
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }

                Log.Error(message);
            }
        }

        public static void Fatal(string message, params object[] args)
        {
            if (Log.IsFatalEnabled)
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }

                Log.Fatal(message);
            }
        }

        #endregion

    }
}
