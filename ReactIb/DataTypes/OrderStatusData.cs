using ReactIb.Enums;

namespace ReactIb.DataTypes
{
    public class OrderStatusData
    {
        public int OrderId { get; }

        public int PermanentId { get; }

        public OrderStatus Status { get; }

        public int FilledQty { get; }

        public int RemainingQty { get; }

        public decimal AverageFillPrice { get; }

        public decimal LastFillPrice { get; }

        public int ParentId { get; }

        public int ClientId { get; }

        public string HeldReason { get; }

        public OrderStatusData(int orderId, int permanentId, OrderStatus status, int filledQty, int remainingQty, decimal lastFillPrice, decimal averageFillPrice, int parentId, int clientId, string heldReason)
        {
            OrderId = orderId;
            PermanentId = permanentId;
            Status = status;
            FilledQty = filledQty;
            AverageFillPrice = averageFillPrice;
            RemainingQty = remainingQty;
            LastFillPrice = lastFillPrice;
            ParentId = parentId;
            ClientId = clientId;
            HeldReason = heldReason;
        }
    }
}