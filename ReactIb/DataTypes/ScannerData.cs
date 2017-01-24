using IBApi;

namespace ReactIb.DataTypes
{
    public class ScannerData
    {
        internal int RequestId { get; }

        public int Ranking { get; }

        public ContractDetails ContractDetails { get; }

        public string Distance { get; }

        public string Benchmark { get; }

        public string Projection { get; }

        public string ComboLegs { get; }

        public ScannerData(int requestId, int ranking, ContractDetails contractDetails, string distance, string benchmark, string projection, string comboLegs)
        {
            RequestId = requestId;
            ComboLegs = comboLegs;
            Projection = projection;
            Benchmark = benchmark;
            Distance = distance;
            ContractDetails = contractDetails;
            Ranking = ranking;
        }
    }
}