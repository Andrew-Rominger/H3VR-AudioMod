﻿using System;
using System.IO;

namespace AudioMod
{
    public class Logger
    {
        private FileInfo _logFile = new FileInfo("LogFile.log");
        private static Logger m_instance;
        public static Logger Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Logger();
                return m_instance;
            }

        }

        public static void Log(Exception e)
        {
            Log($"Exception: {e.Message}\n{e.StackTrace}");
        }

        public Logger()
        {
            if (_logFile.Exists)
            {
                File.Delete(_logFile.FullName);
            }
        }

        public static void Log(string message)
        {
            Instance.LogMessage(message);
        }

        private void LogMessage(string message)
        {
            File.AppendAllText(_logFile.FullName, $"[{DateTime.Now:T}] | {message} \n");
        }
    }
}