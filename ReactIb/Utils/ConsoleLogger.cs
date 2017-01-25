using System;

namespace ReactIb.Utils
{
    public class ConsoleLogger : ILog
    {
        public void Info(object msg)
        {
            Show("INFO: " + msg);
        }

        public void Debug(object msg)
        {
            Show("DEBUG: " + msg);
        }

        public void Error(object msg, Exception e = null)
        {
            if (e != null)
            {
                Show("ERROR: " + msg + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
            }
            else
            {
                Show("ERROR: " + msg);
            }
        }

        public void Fatal(object msg)
        {
            Show("FATAL: " + msg);
        }

        public void Warn(object msg)
        {
            Show("WARN: " + msg);
        }

        private static void Show(string msg)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - {msg}");
        }
    }
}