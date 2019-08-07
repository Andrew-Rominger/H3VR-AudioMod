using System;
using System.IO;

namespace AudioMod
{
    public class Logger
    {
        private const string logName = "AudioMod";
        private FileInfo _logFile = new FileInfo($"LogFile_{logName}.log");
        private static Logger _mInstance;
        public static Logger Instance => _mInstance ?? (_mInstance = new Logger());

        public static void Log(Exception e)
        {
            Log($"Exception: {e.Message}\n{e.StackTrace}");
        }

        public static void Log(object obj)
        {
            Log(obj.ToString());
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
