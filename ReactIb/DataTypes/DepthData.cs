using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class DepthData
    {
        public int RequestId { get; }

        public int RowNum { get; }

        public string ExchangeName { get; }

        public DepthOperation Operation { get; }

        public DepthSide Side { get; }

        public decimal Price { get; }

        public int Size { get; }

        public DepthData(int requestId, int rowNum, string exchangeName, DepthOperation operation, DepthSide side, decimal price, int size)
        {
            RequestId = requestId;
            RowNum = rowNum;
            ExchangeName = exchangeName;
            Operation = operation;
            Side = side;
            Price = price;
            Size = size;
        }

        public override string ToString() => $"RequestId: {RequestId} RowNum: {RowNum} ExchangeName: {ExchangeName} Operation: {Operation} Side: {Side} Price: {Price} Size: {Size}";
    }
}