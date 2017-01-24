using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class TickStringData
    {
        public int RequestId { get; }

        public TickType TickType { get; }

        public string Value { get; }

        public TickStringData(int requestId, TickType tickType, string value)
        {
            RequestId = requestId;
            Value = value;
            TickType = tickType;
        }

        public override string ToString()
        {
            return $"RequestId: {RequestId} TickType: {TickType} Value: {Value}";
        }
    }
}