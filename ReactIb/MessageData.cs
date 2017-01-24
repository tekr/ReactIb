namespace ReactIb
{
    public class MessageData
    {
        public int Code { get; }
        public int RequestId { get; }
        public string Message { get; }

        public bool Handled { get; private set; }

        public MessageData(int code, int requestId, string message)
        {
            Code = code;
            RequestId = requestId;
            Message = message;
        }

        public void SetHandled()
        {
            Handled = true;
        }

        public override string ToString() => $"Code: {Code} Request Id: {RequestId} Message: {Message}";
    }
}