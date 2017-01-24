namespace ReactIb.Enums
{
    public enum StreamingStatus
    {
        Unknown = 0,
        Realtime = 1,
        /// <summary>
        /// Last data recorded. Frozen data will automatically switch to real-time market data during trading hours.
        /// </summary>
        Frozen = 2
    }
}