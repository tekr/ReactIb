using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class TickData
    {
        public int RequestId { get; }

        public TickType TickType { get; }

        public decimal Value { get; }

        /// <summary>
        /// Only valid for price ticks; if true, price is available for auto-execution
        /// </summary>
        public bool CanAutoExecute { get; }

        public TickData(int requestId, TickType tickType, decimal value, bool canAutoExecute)
        {
            RequestId = requestId;
            CanAutoExecute = canAutoExecute;
            Value = value;
            TickType = tickType;
        }
    }
}