using System;

namespace ReactIb.Utils
{
    public interface ILog
    {
        void Info(object msg);

        void Debug(object msg);

        void Error(object msg, Exception e = null);

        void Fatal(object msg);

        void Warn(object msg);
    }
}