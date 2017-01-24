using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class OptionValuationData
    {
        public int RequestId { get; }

        public TickType TickType { get; }

        public double ImpliedVol { get; }

        public double Price { get; }
        public double UnderlyingPrice { get; }

        public double DividendPresentValue { get; }

        public double Delta { get; }
        public double Gamma { get; }
        public double Theta { get; }
        public double Vega { get; }

        public OptionValuationData(int requestId, TickType tickType, double impliedVol, double delta, double price,
            double dividendPresentValue, double gamma, double vega, double theta, double underlyingPrice)
        {
            RequestId = requestId;
            DividendPresentValue = dividendPresentValue;
            Delta = delta;
            Price = price;
            ImpliedVol = impliedVol;
            TickType = tickType;
            Gamma = gamma;
            Vega = vega;
            Theta = theta;
            UnderlyingPrice = underlyingPrice;
        }
    }
}