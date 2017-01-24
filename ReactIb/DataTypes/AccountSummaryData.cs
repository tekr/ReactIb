namespace ReactIb.DataTypes
{
    public class AccountSummaryData
    {
        internal int RequestId { get; }

        public string Account { get; }

        public string Key { get; }

        public string Value { get; }

        public string Currency { get; }

        public AccountSummaryData(int requestId, string account, string key, string value, string currency)
        {
            RequestId = requestId;
            Account = account;
            Key = key;
            Value = value;
            Currency = currency;
        }

        public override string ToString() => $"RequestId: {RequestId} Account: {Account} Ccy: {Currency} Key: {Key} Value: {Value}";
    }
}