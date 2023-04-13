using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace F002520
{

    public static class Logger
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(Logger));
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static Logger()
        {
            XmlConfigurator.Configure(); // 加载 log4net 配置文件
        }

        #region Debug

        //public static void Debug(string message)
        //{
        //    if (Log.IsDebugEnabled)
        //    {
        //        Log.Debug(message);
        //    }
        //}

        //public static void Debug(string message, params object[] args)
        //{
        //    if (Log.IsDebugEnabled)
        //    {
        //        Log.DebugFormat(message, args);
        //    }
        //}

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

        #endregion

        #region Info

        //public static void Info(string message)
        //{
        //    if (Log.IsInfoEnabled)
        //    {
        //        Log.Info(message);
        //    }
        //}

        //public static void Info(string message, params object[] args)
        //{
        //    if (Log.IsInfoEnabled)
        //    {
        //        Log.InfoFormat(message, args);
        //    }
        //}

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

        #endregion

        #region Warn

        //public static void Warn(string message)
        //{
        //    if (Log.IsWarnEnabled)
        //    {
        //        Log.Warn(message);
        //    }
        //}

        //public static void Warn(string message, params object[] args)
        //{
        //    if (Log.IsWarnEnabled)
        //    {
        //        Log.WarnFormat(message, args);
        //    }
        //}

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

        #endregion

        #region Error

        //public static void Error(string message)
        //{
        //    if (Log.IsErrorEnabled)
        //    {
        //        Log.Error(message);
        //    }
        //}

        //public static void Error(string message, params object[] args)
        //{
        //    if (Log.IsErrorEnabled)
        //    {
        //        Log.ErrorFormat(message, args);
        //    }
        //}

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

        #endregion

        #region Fatal

        //public static void Fatal(string message)
        //{
        //    if (Log.IsFatalEnabled)
        //    {
        //        Log.Fatal(message);
        //    }
        //}

        //public static void Fatal(string message, params object[] args)
        //{
        //    if (Log.IsFatalEnabled)
        //    {
        //        Log.FatalFormat(message, args);
        //    }
        //}

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
