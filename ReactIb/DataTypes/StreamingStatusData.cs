
using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class StreamingStatusData
    {
        public int RequestId { get; }

        public StreamingStatus StreamingStatus { get; }

        public StreamingStatusData(int requestId, StreamingStatus streamingStatus)
        {
            RequestId = requestId;
            StreamingStatus = streamingStatus;
        }
    }
}