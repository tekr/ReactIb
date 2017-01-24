using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using IBApi;
using ReactIb.DataTypes;
using ReactIb.Enums;

namespace ReactIb
{
    public enum ConnectionStatus { Connected, Disconnected }

    public interface ITwsApi : IDisposable
    {
        int ClientId { get; }

        IObservable<ConnectionStatus> ConnectionStatus { get; }

        IObservable<OrderData> OpenOrders { get; }
        IObservable<OrderStatusData> OrderStatus { get; }
        IObservable<ExecutionData> Execution { get; }

        IObservable<PortfolioData> Portfolio { get; }
        IObservable<AccountData> Account { get; }
        IObservable<IObservable<AccountData>> Account2 { get; }
        IObservable<DateTime> AccountTime { get; }

        IObservable<DataTypes.MessageData> Message { get; }
        IObservable<MessageData> Error { get; }

        Order CreateOrder();
        Task<DateTime> SendOrderAsync(int orderId, Contract contract, Order order);
        Task CancelOrderAsync(int id);

        Task<DateTime> GetCurrentTimeAsync();

        Task<IEnumerable<OrderData>> GetOpenOrdersAsync(IScheduler scheduler = null);
        Task<IEnumerable<ExecutionData>> GetExecutionsAsync(IScheduler scheduler = null);
        Task<IEnumerable<ContractDetailsData>> GetContractDetailsAsync(Contract contract, IScheduler scheduler = null);
        Task<IEnumerable<BarData>> GetHistoricalBarsAsync(Contract contract, DateTime startDateTime, DateTime endDateTime, BarSize barSize, HistoricalBarType barType, IScheduler scheduler = null);
        Task<IEnumerable<PositionData>> GetPositionsAsync(IScheduler scheduler = null);
        Task<IEnumerable<AccountSummaryData>> GetAccountSummaryAsync(string group, IEnumerable<string> tags, IScheduler scheduler = null);

        Task SubscribeAccountUpdatesAsync(string account = null);
        Task<IObservable<TickData>> SubscribeRealtimeTaqAsync(Contract contract);
        Task<IObservable<BarData>> SubscribeRealtimeBarsAsync(Contract contract, RealtimeBarType barType, bool regularTradingHoursOnly = false);
        Task<IObservable<DepthData>> SubscribeRealtimeDepthAsync(Contract contract, int numRows = 5);

    }
}