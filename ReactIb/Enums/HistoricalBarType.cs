using System.ComponentModel;

namespace ReactIb.Enums
{
    public enum HistoricalBarType
    {
        [Description("TRADES")] Trades,
        [Description("MIDPOINT")] Midpoint,
        [Description("BID")] Bid,
        [Description("ASK")] Ask,
        /// <summary>
        /// Return Bid / Ask price only
        /// </summary>
        [Description("BID_ASK")] BidAsk,
    }
}
