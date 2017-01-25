using IBApi;

namespace ReactIb.DataTypes
{
    public class PositionData
    {
        public string Account { get; }

        public Contract Contract { get; }

        public decimal Position { get; }

        public decimal AverageCost { get; }

        public PositionData(string account, Contract contract, decimal position, decimal averageCost)
        {
            Account = account;
            AverageCost = averageCost;
            Contract = contract;
            Position = position;
        }

        public override string ToString() => $"Account: {Account} Contract: {Contract} Position: {Position} AvgCost: {AverageCost}";
    }
}