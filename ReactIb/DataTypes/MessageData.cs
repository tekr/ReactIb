namespace ReactIb.DataTypes
{
    public class MessageData
    {
        public int Code { get; }

        public string Value { get; }

        public int RequestId { get; }

        public MessageData(int requestId, int code, string value)
        {
            RequestId = requestId;
            Code = code;
            Value = value;
        }

        public override string ToString() => $"Code: {Code} ReqId: {RequestId} Value: {Value}";
    }
}