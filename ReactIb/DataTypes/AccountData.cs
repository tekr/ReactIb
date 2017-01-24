using System;

namespace ReactIb.DataTypes
{
    public class AccountData
    {
        public string Account { get; }

        public string Currency { get; }

        public string Key { get; }

        public string Value { get; }

        public AccountData(string account, string currency, string key, string value)
        {
            Account = account;
            Currency = currency;
            Key = key;
            Value = value;
        }

        public override string ToString() => $"Account: {Account} Ccy: {Currency} Key: {Key} Value: {Value}";
    }
}