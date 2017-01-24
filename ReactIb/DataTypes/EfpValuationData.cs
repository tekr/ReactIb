using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class EfpValuationData
    {
        public int TickerId { get; }

        public TickType TickType { get; }

        public double AnnualisedBasisPoints { get; }

        public string FormattedAnnualisedBasisPoints { get; }

        public double ImpliedFuturesPrice { get; }

        public int HoldDays { get; }

        public string FuturesExpiry { get; }

        public double DividendImpact { get; }

        public double DividendsToExpiry { get; }

        public EfpValuationData(int tickerId, TickType tickType, double annualisedBasisPoints, string formattedAnnualisedBasisPoints,
            double impliedFuturesPrice, int holdDays, string futuresExpiry, double dividendImpact, double dividendsToExpiry)
        {
            TickerId = tickerId;
            DividendsToExpiry = dividendsToExpiry;
            DividendImpact = dividendImpact;
            FuturesExpiry = futuresExpiry;
            HoldDays = holdDays;
            ImpliedFuturesPrice = impliedFuturesPrice;
            FormattedAnnualisedBasisPoints = formattedAnnualisedBasisPoints;
            AnnualisedBasisPoints = annualisedBasisPoints;
            TickType = tickType;
        }
    }
}