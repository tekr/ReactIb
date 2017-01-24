using IBApi;

namespace ReactIb.DataTypes
{
    public class PortfolioData
    {
        public string Account { get; }

        public Contract Contract { get; }

        public int Position { get; }

        public decimal MarketPrice { get; }

        public decimal MarketValue { get; }

        public decimal AverageCost { get; }

        public decimal UnrealizedPnl { get; }

        public decimal RealizedPnl { get; }

        public PortfolioData(string account, Contract contract, int position, decimal marketPrice, decimal marketValue,
                                decimal averageCost, decimal unrealizedPnl, decimal realizedPnl)
        {
            Account = account;
            Contract = contract;
            Position = position;
            MarketPrice = marketPrice;
            MarketValue = marketValue;
            AverageCost = averageCost;
            UnrealizedPnl = unrealizedPnl;
            RealizedPnl = realizedPnl;
        }
    }
}