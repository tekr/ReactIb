namespace ReactIb.DataTypes
{
    public class FundamentalDetailsData
    {
        internal int RequestId { get; }

        public string XmlData { get; }

        public FundamentalDetailsData(int requestId, string xmlData)
        {
            RequestId = requestId;
            XmlData = xmlData;
        }
    }
}