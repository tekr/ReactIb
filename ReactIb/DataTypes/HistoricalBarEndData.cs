using System;

namespace ReactIb.DataTypes
{
    public class HistoricalBarEndData
    {
        internal int RequestId { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public HistoricalBarEndData(int requestId, DateTime startDate, DateTime endDate)
        {
            RequestId = requestId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}