using IBApi;

namespace ReactIb.DataTypes
{
    public class ExecutionData
    {
        internal int RequestId { get; }

        public int OrderId { get; }

        public Contract Contract { get; }

        public Execution Execution { get; }

        public ExecutionData(int requestId, int orderId, Contract contract, Execution execution)
        {
            RequestId = requestId;
            OrderId = orderId;
            Execution = execution;
            Contract = contract;
        }

        public override string ToString() => $"ReqId: {RequestId} OrderIbApiId: {OrderId} OrderPermId: {Execution.PermId} ConId: {Contract.ConId} Qty: {Execution.Shares} Price: {Execution.Price}";
    }
}