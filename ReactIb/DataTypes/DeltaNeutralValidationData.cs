using IBApi;

namespace ReactIb.DataTypes
{
    public class DeltaNeutralValidationData
    {
        public int RequestId { get; }

        public UnderComp UnderlyingComponent { get; }

        public DeltaNeutralValidationData(int requestId, UnderComp underlyingComponent)
        {
            RequestId = requestId;
            UnderlyingComponent = underlyingComponent;
        }
    }
}