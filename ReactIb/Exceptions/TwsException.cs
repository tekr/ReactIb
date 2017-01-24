using System;

namespace ReactIb.Exceptions
{
    public class TwsException : Exception
    {
        public int Code { get; }

        /// <summary>
        /// Returns true for exceptions caused by requests that may have failed due to transient issues (e.g. connection dropped),
        /// and hence which may succeed if the request is attempted again
        /// </summary>
        public bool MayBeTransient { get; }

        public TwsException(int code, string message, bool mayBeTransient, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
            MayBeTransient = mayBeTransient;
        }
    }
}