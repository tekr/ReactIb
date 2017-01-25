using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using IBApi;
using ReactIb.DataTypes;
using ReactIb.Enums;
using ReactIb.Utils;

namespace ReactIb
{
    public partial class TwsApi : ITwsApi
    {
        private const string DefaultHost = "localhost";
        private const int DefaultPort = 7496;
        private const int ReconectIntervalMs = 1000;

        public int ClientId { get; }

        public IObservable<ConnectionStatus> ConnectionStatus { get; }

        public IObservable<PortfolioData> Portfolio { get; }

        public IObservable<OrderData> OpenOrders { get; }
        public IObservable<OrderStatusData> OrderStatus { get; }
        public IObservable<ExecutionData> Execution { get; }

        public IObservable<AccountData> Account { get; }
        public IObservable<DateTime> AccountTime { get; }
        public IObservable<string> AccountEnd { get; }

        //TODO: Check if this works!
        public IObservable<IObservable<AccountData>> Account2 => _ibClient.Account.GroupByUntil(ad =>
                                ad.Account, ad => ad, grp => _ibClient.AccountEnd.Where(acct => acct == grp.Key)); 

        public IObservable<DataTypes.MessageData> Message { get; }

        public IObservable<MessageData> Error { get; }

        private readonly ISubject<ConnectionStatus> _twsConnection = new Subject<ConnectionStatus>();
        private readonly ISubject<MessageData> _error = new Subject<MessageData>();

        private readonly AsyncManualResetEvent _readyWaitHandle = new AsyncManualResetEvent();
        private readonly ManualResetEvent _processingWaitHandle = new ManualResetEvent(false);
        private readonly EWrapperImp _ibClient = new EWrapperImp();
        private EReader _reader;

        private readonly ILog _log;
        private readonly int _port;
        private readonly string _host;

        private int _nextRequestId;
        private int _nextOrderId;
        private int _reconnecting;
 
        private bool _disposed;
        private Thread _processingThread;

        public TwsApi(string host = null, int? port = null, int clientId = 0, ILog log = null)
        {
            _host = host ?? DefaultHost;
            _port = port ?? DefaultPort;
            ClientId = clientId;
            _log = log ?? NullLogger.Instance;

            Message = _ibClient.Message;
            OpenOrders = _ibClient.OpenOrder;
            OrderStatus = _ibClient.OrderStatus;

            // Filter out executions received for a specific request id (which will be >= 0)
            Execution = _ibClient.Execution.Where(ed => ed.RequestId < 0);
            Portfolio = _ibClient.Portfolio;
            Account = _ibClient.Account;
            AccountTime = _ibClient.AccountTime;
            AccountEnd = _ibClient.AccountEnd;
            Error = _error;

            Message.Subscribe(HandleMessage);

            ConnectionStatus = _twsConnection.Merge(_ibClient.ConnectionClosed.Select(_ => ReactIb.ConnectionStatus.Disconnected)).DistinctUntilChanged();
            ConnectionStatus.Subscribe(s => _log.Info($"TWS connection now {s}"));

            _ibClient.NextValidId.Subscribe(e =>
            {
                _nextOrderId = e;

                _readyWaitHandle.Set();
                _twsConnection.OnNext(ReactIb.ConnectionStatus.Connected);

                _log.Debug($"Connection opened. Next order id: {e}");
            });

            _ibClient.ConnectionClosed.Subscribe(async _ =>
            {
                _readyWaitHandle.Reset();
                await Task.Delay(ReconectIntervalMs);

                Reinitialise();
            });

            Reinitialise();
        }

        public async Task<DateTime> GetCurrentTimeAsync()
        {
            var observable = _ibClient.CurrentTime.FirstAsync().Replay();

            using (observable.Connect())
            {
                await ConnectionReadyAsync();
                _ibClient.ClientSocket.reqCurrentTime();
                return await observable;
            }
        }

        public Order CreateOrder()
        {
            // Need to ensure we're connected so order ID is valid
            ConnectionReadyAsync().Wait();

            return new Order
                   {
                       OrderId = Interlocked.Increment(ref _nextOrderId)
                   };
        }

        public async Task<DateTime> SendOrderAsync(int orderId, Contract contract, Order order)
        {
            await ConnectionReadyAsync();
            _ibClient.ClientSocket.placeOrder(orderId, contract, order);

            return DateTime.UtcNow;
        }

        public async Task CancelOrderAsync(int id)
        {
            await ConnectionReadyAsync();
            _ibClient.ClientSocket.cancelOrder(id);
        }

        public Task<IEnumerable<OrderData>> GetOpenOrdersAsync(IScheduler scheduler)
        {
            return new ListFetcher<OrderData, object>(this, _ibClient.OpenOrder,
                            _ibClient.OpenOrderEnd).RunAsync(r => _ibClient.ClientSocket.reqAllOpenOrders(), scheduler);
        }

        public Task<IEnumerable<ExecutionData>> GetExecutionsAsync(IScheduler scheduler)
        {
            return new ListFetcher<ExecutionData, int>(this, _ibClient.Execution, _ibClient.ExecutionEnd, e => e.RequestId,
                            e => e).RunAsync(r => _ibClient.ClientSocket.reqExecutions(r, new ExecutionFilter()), scheduler);
        }

        public Task<IEnumerable<ContractDetailsData>> GetContractDetailsAsync(Contract contract, IScheduler scheduler)
        {
            return new ContractDetailsListFetcher(this, _ibClient.ContractDetails, _ibClient.ContractDetailsEnd,
                            e => e.RequestId, e => e).RunAsync(r => _ibClient.ClientSocket.reqContractDetails(r, contract), scheduler);
        }

        public Task<IEnumerable<PositionData>> GetPositionsAsync(IScheduler scheduler)
        {
            return new ListFetcher<PositionData, object>(this, _ibClient.Position, _ibClient.PositionEnd).RunAsync(r =>
                            _ibClient.ClientSocket.reqPositions(), scheduler);
        }

        public Task<IEnumerable<AccountSummaryData>> GetAccountSummaryAsync(string group, IEnumerable<string> tags, IScheduler scheduler)
        {
            return new ListFetcher<AccountSummaryData, int>(this, _ibClient.AccountSummary, _ibClient.AccountSummaryEnd, e => e.RequestId,
                            e => e).RunAsync(r => _ibClient.ClientSocket.reqAccountSummary(r, group ?? "All", string.Join(",", tags)), scheduler);
        }

        public async Task SubscribeAccountUpdatesAsync(string account = null)
        {
            await ConnectionReadyAsync();
            _ibClient.ClientSocket.reqAccountUpdates(true, account);
        }

        public async Task<IObservable<TickData>> SubscribeRealtimeTaqAsync(Contract contract)
        {
            await ConnectionReadyAsync();
            var requestId = GetNextRequestId();

            return new ObservableWrapper<TickData>(_ibClient.RealtimeTaq.Where(r => r.RequestId == requestId),
                            () =>
                                {
                                    _log.Debug($"Subscribing to realtime TAQ data for contract id {contract.ConId} with request id {requestId}");
                                    _ibClient.ClientSocket.reqMktData(requestId, contract, null, false, null);
                                },
                            async () =>
                                {
                                    await ConnectionReadyAsync();
                                    _log.Debug($"Unsubscribing from realtime TAQ data for contract id {contract.ConId} with request id {requestId}");
                                    _ibClient.ClientSocket.cancelMktData(requestId);
                                });
        }

        public async Task<IObservable<BarData>> SubscribeRealtimeBarsAsync(Contract contract, RealtimeBarType barType, bool regularTradingHoursOnly = false)
        {
            await ConnectionReadyAsync();
            var requestId = GetNextRequestId();

            return new ObservableWrapper<BarData>(_ibClient.RealtimeBar.Where(r => r.RequestId == requestId),
                            () =>
                                {
                                    _log.Debug($"Subscribing to realtime bars for contract id {contract.ConId} with request id {requestId}");
                                    // Only 5-second bar interval currently supported
                                    _ibClient.ClientSocket.reqRealTimeBars(requestId, contract, 5, GetEnumDescription(barType), regularTradingHoursOnly, null);
                                },
                            async () =>
                                {
                                    await ConnectionReadyAsync();
                                    _log.Debug($"Unsubscribing from realtime bars for contract id {contract.ConId} with request id {requestId}");
                                    _ibClient.ClientSocket.cancelRealTimeBars(requestId);
                                });
        }

        public async Task<IObservable<DepthData>> SubscribeRealtimeDepthAsync(Contract contract, int numRows = 5)
        {
            await ConnectionReadyAsync();
            var requestId = GetNextRequestId();

            return new ObservableWrapper<DepthData>(_ibClient.RealtimeDepth.Where(r => r.RequestId == requestId),
                            () =>
                                {
                                    _log.Debug($"Subscribing to realtime depth for contract id {contract.ConId} with request id {requestId}");
                                    _ibClient.ClientSocket.reqMarketDepth(requestId, contract, numRows, null);
                                },
                            async () =>
                                {
                                    await ConnectionReadyAsync();
                                    _log.Debug($"Unsubscribing from realtime depth for contract id {contract.ConId} with request id {requestId}");
                                    _ibClient.ClientSocket.cancelMktDepth(requestId);
                                });
        }

        public Task<IEnumerable<BarData>> GetHistoricalBarsAsync(Contract contract, DateTime startDateTime, DateTime endDateTime, BarSize barSize, HistoricalBarType barType, IScheduler scheduler)
        {
            var duration = endDateTime - startDateTime;
            var requester = new HistoricalDataListFetcher(this, _ibClient.HistoricalBar, _ibClient.HistoricalBarEnd, e => e.RequestId, e => e.RequestId);

            return requester.RunAsync(r => _ibClient.ClientSocket.reqHistoricalData(r, contract,
                                        endDateTime.ToUniversalTime().ToString("yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture) + " GMT",
                                        GetDurationString(duration), GetEnumDescription(barSize), GetEnumDescription(barType), 0, 2, null), scheduler);
        }

        public void Dispose()
        {
            _disposed = true;
            _ibClient.ClientSocket.Close();
        }

        private Task ConnectionReadyAsync()
        {
            return _readyWaitHandle.WaitAsync();
        }

        private void HandleMessage(DataTypes.MessageData message)
        {
            var error = new MessageData(message.Code, message.RequestId, message.Value);
            _error.OnNext(error);

            if (!error.Handled || error.RequestId == -1)
            {
                switch (message.Code)
                {
                    // TWS <-> IB connectivity down
                    case 2110:
                    case 1100:
                        _readyWaitHandle.Reset();
                        _twsConnection.OnNext(ReactIb.ConnectionStatus.Disconnected);
                        _log.Error("TWS to IB connectivity lost");
                        break;

                    // Connectivity restored
                    case 1101:
                    case 1102:
                        _log.Error($"Connectivity restored. Subscriptions {(message.Code == 1101 ? "lost" : "maintained")}");
                        _ibClient.ClientSocket.reqIds(1); 
                        break;

                    case 202:
                        _log.Debug($"Order cancelled - {message}");
                        break;

                    // Market data farm connection being opened
                    case 2119:
                        
                    // Realtime/historical server connection restored
                    case 2104:
                    case 2106:
                        _log.Debug($"TWS response - {message}");
                        break;
                    
                    // Realtime/historical server connection lost
                    case 2103:
                    case 2105:
                        _log.Warn($"TWS response - {message}");
                        break;

                    // Market data server inactive
                    case 2107:
                    case 2108:
                        _log.Debug($"TWS response - {message}");
                        break;

                    // Occurs when trying to send FX order under EUR 20K to IDEALPRO
                    case 399:
                        _log.Warn($"TWS response - {message}");
                        break;

                    // Order rejected
                    case 201:
                        if (error.Message.ToLower().Contains("margin"))
                        {
                            _log.Fatal("ORDER REJECTED DUE TO INSUFFICIENT MARGIN - {error}");
                        }
                        else
                        {
                            _log.Error($"Order rejected - {message}");
                        }

                        break;

                    default:
                        _log.Error($"TWS error - {message}");
                        break;
                }
            }
        }

        private async void Reinitialise()
        {
            // Only want one thread in here at a time
            if (!_disposed && Interlocked.CompareExchange(ref _reconnecting, 1, 0) == 0)
            {
                try
                {
                    _log.Info($"Connecting to TWS @ {_host ?? "localhost"} on port {_port} with client ID {ClientId}");

                    // Necessary to stop a race between the processing thread below and the message
                    // processing done inside the IB API when the connection is established.
                    // Without this, the eConnect call won't return if the processing thread grabs
                    // the connected message first on a reconnection.
                    _processingWaitHandle.Reset();
                    _ibClient.Signal.issueSignal();

                    while (true)
                    {
                        _ibClient.ClientSocket.eConnect(_host ?? string.Empty, _port, ClientId);

                        if (_ibClient.ClientSocket.IsConnected())
                        {
                            // Can't instantiate reader until we are connected, as it fetches properties
                            // from the ClientSocket that are initialised on connection
                            _reader = _reader ?? new EReader(_ibClient.ClientSocket, _ibClient.Signal);

                            // Need to restart after each connection
                            _reader.Start();

                            _processingWaitHandle.Set();

                            if (_processingThread == null)
                            {
                                _processingThread = new Thread(() =>
                                                        {
                                                            while (true)
                                                            {
                                                                _processingWaitHandle.WaitOne();

                                                                _ibClient.Signal.waitForSignal();
                                                                _reader.processMsgs();
                                                            }
                                                        }) {IsBackground = true};

                                _processingThread.Start();
                            }

                            break;
                        }

                        await Task.Delay(ReconectIntervalMs);
                    }

                    _log.Debug("(Re)connection successful");
                }
                finally
                {
                    _reconnecting = 0;
                }
            }
        }

        private int GetNextRequestId() => Interlocked.Increment(ref _nextRequestId);

        private static string GetDurationString(TimeSpan duration)
        {
            return duration.TotalDays >= 365                        // Max weeks fetchable = 52
                ? Math.Ceiling(duration.TotalDays / 365) + "Y"
                : duration.TotalDays >= 34                          // Max days fetchable = 34
                    ? Math.Ceiling(duration.TotalDays / 7) + "W"
                    : duration.TotalSeconds > 86400                 // Max seconds fetchable = 86400
                        ? Math.Ceiling(duration.TotalDays) + " D"
                        : Math.Ceiling(duration.TotalSeconds) + " S";
        }

        public string GetEnumDescription(Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.FirstOrDefault()?.Description ?? value.ToString();
        }
    }
}