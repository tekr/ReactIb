using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class FinancialAdvisorData
    {
        public FADataType DataType { get; }

        public string XmlData { get; }

        public FinancialAdvisorData(FADataType dataType, string xmlData)
        {
            DataType = dataType;
            XmlData = xmlData;
        }
    }
}