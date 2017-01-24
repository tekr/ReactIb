using System.ComponentModel;

namespace ReactIb.Enums
{
    public enum RealtimeBarType
    {
        [Description("TRADES")] Trades,
        [Description("BID")] Bid,
        [Description("ASK")] Ask,
        [Description("MIDPOINT")] Midpoint
    }
}
