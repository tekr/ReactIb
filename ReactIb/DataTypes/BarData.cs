using System;

namespace ReactIb.DataTypes
{
    public class BarData
    {
        internal int RequestId { get; }

        public DateTime DateTime { get; }

        public decimal Open { get; }

        public decimal High { get; }

        public decimal Low { get; }

        public decimal Close { get; }

        public long Volume { get; }

        public int Trades { get; }

        public double Wap { get; }

        public bool HasGaps { get; }

        public BarData(int requestId, DateTime dateTime, decimal open, decimal high, decimal low, decimal close, long volume, int trades, double wap, bool hasGaps)
        {
            RequestId = requestId;
            DateTime = dateTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            Trades = trades;
            Wap = wap;
            HasGaps = hasGaps;
        }

        public override string ToString() => $"ReqId: {RequestId} Date: {DateTime} Op: {Open} Hi: {High} Lo: {Low} Cl: {Close} Vol: {Volume} Trades: {Trades} Wap: {Wap}";
    }
}