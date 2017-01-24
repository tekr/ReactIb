using IBApi;

namespace ReactIb.DataTypes
{
    public class PositionData
    {
        public string Account { get; }

        public Contract Contract { get; }

        public int Position { get; }

        public decimal AverageCost { get; }

        public PositionData(string account, Contract contract, int position, decimal averageCost)
        {
            Account = account;
            AverageCost = averageCost;
            Contract = contract;
            Position = position;
        }

        public override string ToString()
        {
            return $"Account: {Account} Contract: {Contract} Position: {Position} AvgCost: {AverageCost}";
        }
    }
}