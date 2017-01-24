using IBApi;

namespace ReactIb.DataTypes
{
    public class ContractDetailsData
    {
        internal int RequestId { get; }

        public ContractDetails ContractDetails { get; }

        public bool IsBond { get; }

        public ContractDetailsData(int requestId, ContractDetails contractDetails, bool isBond)
        {
            RequestId = requestId;
            ContractDetails = contractDetails;
            IsBond = isBond;
        }
    }
}