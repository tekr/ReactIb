using IBApi;

namespace ReactIb.DataTypes
{
    public class OrderData
    {
        public int OrderId { get; }

        public Contract Contract { get; }

        public Order Order { get; }

        public OrderState OrderState { get; }

        public OrderData(int orderId, Contract contract, Order order, OrderState orderState)
        {
            OrderId = orderId;
            Contract = contract;
            Order = order;
            OrderState = orderState;
        }
    }
}