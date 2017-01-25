using System;

namespace ReactIb.Utils
{
    /// <summary>
    /// Clients can provide a custom implementation of this interface (e.g. one that adapts to your
    /// logging framework of choice) to the TwsApi constructor to determine where ReactIb will send
    /// logging calls.
    /// </summary>
    public interface ILog
    {
        void Info(object msg);

        void Debug(object msg);

        void Error(object msg, Exception e = null);

        void Fatal(object msg);

        void Warn(object msg);
    }
}