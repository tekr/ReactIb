using System.ComponentModel;

namespace ReactIb.Enums
{
    public enum TickType
    {
        [Description("BID_SIZE")] BidSize = 0,
        [Description("BID")] BidPrice = 1,
        [Description("ASK")] AskPrice = 2,
        [Description("ASK_SIZE")] AskSize = 3,
        [Description("LAST")] LastPrice = 4,
        [Description("LAST_SIZE")] LastSize = 5,
        [Description("HIGH")] HighPrice = 6,
        [Description("LOW")] LowPrice = 7,
        [Description("VOLUME")] Volume = 8,
        [Description("CLOSE")] ClosePrice = 9,
        [Description("BID_OPTION")] BidOption = 10,
        [Description("ASK_OPTION")] AskOption = 11,
        [Description("LAST_OPTION")] LastOption = 12,
        [Description("MODEL_OPTION")] ModelOption = 13,
        [Description("OPEN")] OpenPrice = 14,
        [Description("LOW_13_WEEK")] Low13Week = 15,
        [Description("HIGH_13_WEEK")] High13Week = 16,
        [Description("LOW_26_WEEK")] Low26Week = 17,
        [Description("HIGH_26_WEEK")] High26Week = 18,
        [Description("LOW_52_WEEK")] Low52Week = 19,
        [Description("HIGH_52_WEEK")] High52Week = 20,
        [Description("AVG_VOLUME")] AverageVolume = 21,
        [Description("OPEN_INTEREST")] OpenInterest = 22,
        [Description("OPTION_HISTORICAL_VOL")] OptionHistoricalVolatility = 23,
        [Description("OPTION_IMPLIED_VOL")] OptionImpliedVolatility = 24,
        [Description("OPTION_BID_EXCH")] OptionBidExchange = 25,
        [Description("OPTION_ASK_EXCH")] OptionAskExchange = 26,
        [Description("OPTION_CALL_OPEN_INTEREST")] OptionCallOpenInterest = 27,
        [Description("OPTION_PUT_OPEN_INTEREST")] OptionPutOpenInterest = 28,
        [Description("OPTION_CALL_VOLUME")] OptionCallVolume = 29,
        [Description("OPTION_PUT_VOLUME")] OptionPutVolume = 30,
        [Description("INDEX_FUTURE_PREMIUM")] IndexFuturePremium = 31,
        [Description("BID_EXCH")] BidExchange = 32,
        [Description("ASK_EXCH")] AskExchange = 33,
        [Description("AUCTION_VOLUME")] AuctionVolume = 34,
        [Description("AUCTION_PRICE")] AuctionPrice = 35,
        [Description("AUCTION_IMBALANCE")] AuctionImbalance = 36,
        [Description("MARK_PRICE")] MarkPrice = 37,
        [Description("BID_EFP_COMPUTATION")] BidEfpComputation = 38,
        [Description("ASK_EFP_COMPUTATION")] AskEfpComputation = 39,
        [Description("LAST_EFP_COMPUTATION")] LastEfpComputation = 40,
        [Description("OPEN_EFP_COMPUTATION")] OpenEfpComputation = 41,
        [Description("HIGH_EFP_COMPUTATION")] HighEfpComputation = 42,
        [Description("LOW_EFP_COMPUTATION")] LowEfpComputation = 43,
        [Description("CLOSE_EFP_COMPUTATION")] CloseEfpComputation = 44,
        [Description("LAST_TIMESTAMP")] LastTimestamp = 45,
        [Description("SHORTABLE")] Shortable = 46,
        [Description("FUNDAMENTAL_RATIOS")] FundamentalRatios = 47,
        [Description("RTVOLUME")] RealTimeVolume = 48,
        [Description("HALTED")] Halted = 49,
        [Description("BID_YIELD")] BidYield = 50,
        [Description("ASK_YIELD")] AskYield = 51,
        [Description("LAST_YIELD")] LastYield = 52,
        /// <summary>
        /// returns calculated implied volatility as a result of an calculateImpliedVolatility( ) request.
        /// </summary>
        [Description("CUST_OPTION_COMPUTATION")] CustOptionComputation = 53,
        [Description("TRADE_COUNT")] TradeCount = 54,
        /// <summary>
        /// Trades per Minute
        /// </summary>
        [Description("TRADE_RATE")] TradeRate = 55,
        /// <summary>
        /// Volume per Minute
        /// </summary>
        [Description("VOLUME_RATE")] VolumeRate = 56,
        [Description("LAST_RTH_TRADE")] LastRthTrade = 57,
        [Description("REGULATORY_IMBALANCE")] RegulatoryImbalance = 57
    }
}