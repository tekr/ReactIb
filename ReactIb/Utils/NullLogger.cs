using System;

namespace ReactIb.Utils
{
    public class NullLogger : ILog
    {
        public static ILog Instance { get; } = new NullLogger();

        public void Info(object msg)
        {
        }

        public void Debug(object msg)
        {
        }

        public void Error(object msg, Exception e = null)
        {
        }

        public void Fatal(object msg)
        {
        }

        public void Warn(object msg)
        {
        }

        private static void Show(string msg)
        {
        }
    }
}