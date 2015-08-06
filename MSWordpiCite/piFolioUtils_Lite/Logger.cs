using System;
using System.Collections.Generic;
using System.Text;

namespace MSWordpiCite.Tools
{
    using System.Data;
    using System.Reflection;
    using System.IO;
    using System.Configuration;
    using System.Text.RegularExpressions;

    // Logger class 

    public enum LogType
    {

        Info = 1,
        Warning = 2,
        Error = 3
    }

    public class Logger
    {

        // Privates
        private bool isReady = false;
        private StreamWriter swLog;
        private string strLogFile;
        private string strClassName;

        // Constructors
        public Logger() : this("NoClass", true, "") { }

        public Logger(string strClassName) : this(strClassName, true, "") { }

        public Logger(string strClassName, string strFileName) : this(strClassName, false, strFileName) { }

        public Logger(string strClassName, bool bDefaultLogFile, string strFileName)
        {
            if (ConfigurationManager.AppSettings["LOG_FILE_PATH"] != null && ConfigurationManager.AppSettings["LOG_FILE_NAME"] != null)
            {
                this.strLogFile = ConfigurationManager.AppSettings["LOG_FILE_PATH"];
                this.strLogFile += bDefaultLogFile ? ConfigurationManager.AppSettings["LOG_FILE_NAME"] : strFileName;
            }
            else
            {
                this.strLogFile = Properties.Settings.Default.DefaultLogFilePath;
                this.strLogFile += bDefaultLogFile ? Properties.Settings.Default.DefaultLogFileName : strFileName;
            }

            this.strClassName = strClassName;
            if (!File.Exists(strLogFile))
            {
                if (!createFile(strLogFile))
                    return;
            }
        }

        public Logger(string strClassName, string strFilePath, bool bUsingExternalSetting)
        {
            this.strLogFile = strFilePath;
            this.strClassName = strClassName;
            if (!File.Exists(strLogFile))
            {
                if (!createFile(strLogFile))
                    return;
            }
        }

        private bool createFile(string strFilePath)
        {
            try
            {
                File.Create(strFilePath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void openFile()
        {
            try
            {
                swLog = File.AppendText(strLogFile);
                isReady = true;
            }
            catch
            {
                isReady = false;
            }
        }

        private void closeFile()
        {
            if (isReady)
            {
                try
                {
                    swLog.Close();
                }
                catch
                {
                }
            }
        }

        public static string GetNewLogFilename()
        {
            AppDomain Ad = AppDomain.CurrentDomain;
            return Ad.BaseDirectory + DateTime.Now.ToString("dd-MM-yyyy") + ".log";
        }

        public void WriteLine(LogType logtype, string message)
        {
            WriteLine(logtype, "method", message);
        }

        public void WriteLine(LogType logtype, string strMethodName, string message)
        {

            string stub = DateTime.Now.ToString("dd-MM-yyyy @ HH:MM:ss  ");
            switch (logtype)
            {
                case LogType.Info:
                    stub += "Informational , ";
                    break;
                case LogType.Warning:
                    stub += "Warning       , ";
                    break;
                case LogType.Error:
                    stub += "Fatal error   , ";
                    break;
            }
            stub += strClassName + "::" + strMethodName + "::" + message;
            openFile();
            _writelog(stub);
            closeFile();
        }

        private void _writelog(string msg)
        {
            if (isReady)
            {
                if(msg.Length > 0)
                    swLog.WriteLine("\r\n" + msg);
            }
        }
    }
}
