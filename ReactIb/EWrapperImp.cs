using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Subjects;
using IBApi;
using ReactIb.DataTypes;
using ReactIb.Enums;
using TickType = ReactIb.Enums.TickType;

namespace ReactIb
{
    public class EWrapperImp : EWrapper
    {
        private readonly ISubject<DataTypes.MessageData> _message = new Subject<DataTypes.MessageData>();

        private readonly ISubject<TickData> _realtimeTaq = new Subject<TickData>();
        private readonly ISubject<TickStringData> _realtimeTickString = new Subject<TickStringData>();
        private readonly ISubject<OptionValuationData> _optionValuation = new Subject<OptionValuationData>();
        private readonly ISubject<EfpValuationData> _efpValuation = new Subject<EfpValuationData>();
        private readonly ISubject<DeltaNeutralValidationData> _deltaNeutralValidation = new Subject<DeltaNeutralValidationData>();
        private readonly ISubject<int> _tickSnapshotEnd = new Subject<int>();

        private readonly ISubject<BarData> _realtimeBar = new Subject<BarData>();
        private readonly ISubject<DepthData> _realtimeDepth = new Subject<DepthData>();
        private readonly ISubject<StreamingStatusData> _streamingStatus = new Subject<StreamingStatusData>();

        private readonly ISubject<BarData> _historicalBar = new Subject<BarData>();
        private readonly ISubject<HistoricalBarEndData> _historicalBarEnd = new Subject<HistoricalBarEndData>();

        private readonly ISubject<OrderData> _openOrder = new Subject<OrderData>();
        private readonly ISubject<object> _openOrderEnd = new Subject<object>();
        private readonly ISubject<OrderStatusData> _orderStatus = new Subject<OrderStatusData>();

        private readonly ISubject<ExecutionData> _execution = new Subject<ExecutionData>();
        private readonly ISubject<int> _executionEnd = new Subject<int>();

        private readonly ISubject<ContractDetailsData> _contractDetails = new Subject<ContractDetailsData>();
        private readonly ISubject<int> _contractDetailsEnd = new Subject<int>();

        private readonly ISubject<AccountData> _account = new Subject<AccountData>();
        private readonly ISubject<DateTime> _accountTime = new Subject<DateTime>();
        private readonly ISubject<string> _accountEnd = new Subject<string>();

        private readonly ISubject<AccountSummaryData> _accountSummary = new Subject<AccountSummaryData>();
        private readonly ISubject<int> _accountSummaryEnd = new Subject<int>();

        private readonly ISubject<PortfolioData> _portfolio = new Subject<PortfolioData>();
        private readonly ISubject<PositionData> _position = new Subject<PositionData>();
        private readonly ISubject<object> _positionEnd = new Subject<object>();

        private readonly ISubject<ManagedAccountsData> _managedAccounts = new Subject<ManagedAccountsData>();
        private readonly ISubject<FinancialAdvisorData> _financialAdvisorData = new Subject<FinancialAdvisorData>();

        private readonly ISubject<FundamentalDetailsData> _fundamentalDetails = new Subject<FundamentalDetailsData>();

        private readonly ISubject<CommissionReport> _commissionReport = new Subject<CommissionReport>();

        private readonly ISubject<NewsBulletinData> _newsBulletin = new Subject<NewsBulletinData>();

        private readonly ISubject<DateTime> _currentTime = new Subject<DateTime>();
        private readonly ISubject<int> _nextValidId = new Subject<int>();

        private readonly ISubject<object> _connectionClosed = new Subject<object>();

        private readonly ISubject<string> _scannerParameters = new Subject<string>();
        private readonly ISubject<ScannerData> _scanner = new Subject<ScannerData>();
        private readonly ISubject<int> _scannerEnd = new Subject<int>();

        internal EReaderSignal Signal { get; } = new EReaderMonitorSignal();

        public IObservable<TickData> RealtimeTaq => _realtimeTaq;
        public IObservable<TickStringData> RealtimeTickString => _realtimeTickString;
        public IObservable<OptionValuationData> OptionValuation => _optionValuation;
        public IObservable<EfpValuationData> EfpValuation => _efpValuation;
        public IObservable<DeltaNeutralValidationData> DeltaNeutralValidation => _deltaNeutralValidation;
        public IObservable<int> TickSnapshotEnd => _tickSnapshotEnd;

        public IObservable<BarData> RealtimeBar => _realtimeBar;
        public IObservable<DepthData> RealtimeDepth => _realtimeDepth;

        public IObservable<StreamingStatusData> StreamingStatus => _streamingStatus;

        public IObservable<BarData> HistoricalBar => _historicalBar;
        public IObservable<HistoricalBarEndData> HistoricalBarEnd => _historicalBarEnd;

        public IObservable<OrderData> OpenOrder => _openOrder;
        public IObservable<object> OpenOrderEnd => _openOrderEnd;
        public IObservable<OrderStatusData> OrderStatus => _orderStatus;

        public IObservable<ExecutionData> Execution => _execution;
        public IObservable<int> ExecutionEnd => _executionEnd;

        public IObservable<ContractDetailsData> ContractDetails => _contractDetails;
        public IObservable<int> ContractDetailsEnd => _contractDetailsEnd;

        public IObservable<AccountData> Account => _account;
        public IObservable<DateTime> AccountTime => _accountTime;
        public IObservable<string> AccountEnd => _accountEnd;

        public IObservable<AccountSummaryData> AccountSummary => _accountSummary;
        public IObservable<int> AccountSummaryEnd => _accountSummaryEnd;

        public IObservable<PortfolioData> Portfolio => _portfolio;
        public IObservable<PositionData> Position => _position;
        public IObservable<object> PositionEnd => _positionEnd;

        public IObservable<ManagedAccountsData> ManagedAccounts => _managedAccounts;
        public IObservable<FinancialAdvisorData> FinancialAdvisorData => _financialAdvisorData;

        public IObservable<CommissionReport> CommissionReport => _commissionReport;

        public IObservable<NewsBulletinData> NewsBulletin => _newsBulletin;
        public IObservable<FundamentalDetailsData> FundamentalDetails => _fundamentalDetails;

        /// <summary>
        /// XML scanner parameters
        /// </summary>
        public IObservable<string> ScannerParameters => _scannerParameters;
        public IObservable<ScannerData> Scanner => _scanner;
        public IObservable<int> ScannerEnd => _scannerEnd;

        public IObservable<int> NextValidId => _nextValidId;
        public IObservable<DateTime> CurrentTime => _currentTime;

        public IObservable<object> ConnectionClosed => _connectionClosed;
        public IObservable<DataTypes.MessageData> Message => _message;

        public EClientSocket ClientSocket { get; }

        public EWrapperImp()
        {
            ClientSocket = new EClientSocket(this, Signal);
        }

        public void error(Exception e)
        {
            // This method only seems to be called when a fatal error has occurred (e.g. timeout connecting,
            // dropped connection, etc.), but there doesn't appear to be any automatic closing of the client
            // socket or notification of closed connection to the client. The below causes both these things
            // to happen
            ClientSocket.Close();
        }

        public void error(string str)
        {
            _message.OnNext(new DataTypes.MessageData(0, 0, str));
        }

        public void error(int id, int errorCode, string errorMsg)
        {
            _message.OnNext(new DataTypes.MessageData(id, errorCode, errorMsg));
        }

        public void currentTime(long time)
        {
            _currentTime.OnNext(DateTimeFromSeconds(time));
        }

        public void tickPrice(int tickerId, int field, double price, int canAutoExecute)
        {
            _realtimeTaq.OnNext(new TickData(tickerId, (TickType)field, (decimal)price, canAutoExecute != 0));
        }

        public void tickSize(int tickerId, int field, int size)
        {
            _realtimeTaq.OnNext(new TickData(tickerId, (TickType)field, size, false));
        }

        public void tickString(int tickerId, int field, string value)
        {
            _realtimeTickString.OnNext(new TickStringData(tickerId, (TickType)field, value));
        }

        public void tickGeneric(int tickerId, int field, double value)
        {
            _realtimeTaq.OnNext(new TickData(tickerId, (TickType)field, (decimal)value, false));
        }

        public void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture,
            int holdDays, string futureExpiry, double dividendImpact, double dividendsToExpiry)
        {
            _efpValuation.OnNext(new EfpValuationData(tickerId, (TickType) tickType, basisPoints, formattedBasisPoints,
                impliedFuture, holdDays, futureExpiry, dividendImpact, dividendsToExpiry));
        }

        public void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            _deltaNeutralValidation.OnNext(new DeltaNeutralValidationData(reqId, underComp));
        }

        public void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice,
            double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            _optionValuation.OnNext(new OptionValuationData(tickerId, (TickType)field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice));
        }

        public void tickSnapshotEnd(int tickerId)
        {
            _tickSnapshotEnd.OnNext(tickerId);
        }

        public void nextValidId(int orderId)
        {
            _nextValidId.OnNext(orderId);
        }

        public void managedAccounts(string accountsList)
        {
            _managedAccounts.OnNext(new ManagedAccountsData(accountsList.Split(',')));
        }

        public void connectionClosed()
        {
            _connectionClosed.OnNext(null);
        }

        public void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            _accountSummary.OnNext(new AccountSummaryData(reqId, account, tag, value, currency));
        }

        public void accountSummaryEnd(int reqId)
        {
            _accountSummaryEnd.OnNext(reqId);
        }

        public void updateAccountValue(string key, string value, string currency, string accountName)
        {
            _account.OnNext(new AccountData(accountName, currency, key, value));
        }

        public void updatePortfolio(Contract contract, double position, double marketPrice, double marketValue, double averageCost,
            double unrealisedPnl, double realisedPnl, string accountName)
        {
            _portfolio.OnNext(new PortfolioData(accountName, contract, (int)position, (decimal)marketPrice,
                (decimal)marketValue, (decimal)averageCost, (decimal)unrealisedPnl, (decimal)realisedPnl));
        }

        public void updateAccountTime(string timestamp)
        {
            // Sometimes (e.g. on weekends) the timestamp is empty
            if (!string.IsNullOrWhiteSpace(timestamp))
            {
                _accountTime.OnNext(DateTime.Parse(timestamp));
            }
        }

        public void accountDownloadEnd(string account)
        {
            _accountEnd.OnNext(account);
        }

        public void orderStatus(int orderId, string status, double filled, double remaining, double avgFillPrice, int permId, int parentId,
            double lastFillPrice, int clientId, string whyHeld)
        {
            _orderStatus.OnNext(new OrderStatusData(orderId, permId, string.IsNullOrEmpty(status) ? Enums.OrderStatus.None :
                (OrderStatus)Enum.Parse(typeof(OrderStatus), status), (int)filled, (int)remaining,
                (decimal)lastFillPrice, (decimal)avgFillPrice, parentId, clientId, whyHeld));
        }

        public void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            _openOrder.OnNext(new OrderData(orderId, contract, order, orderState));
        }

        public void openOrderEnd()
        {
            _openOrderEnd.OnNext(null);
        }

        public void contractDetails(int reqId, ContractDetails contractDetails)
        {
            _contractDetails.OnNext(new ContractDetailsData(reqId, contractDetails, false));
        }

        public void contractDetailsEnd(int reqId)
        {
            _contractDetailsEnd.OnNext(reqId);
        }

        public void bondContractDetails(int reqId, ContractDetails contractDetails)
        {
            _contractDetails.OnNext(new ContractDetailsData(reqId, contractDetails, true));
        }

        public void execDetails(int reqId, Contract contract, Execution execution)
        {
            _execution.OnNext(new ExecutionData(reqId, execution.OrderId, contract, execution));
        }

        public void execDetailsEnd(int reqId)
        {
            _executionEnd.OnNext(reqId);
        }

        public void commissionReport(CommissionReport commissionReport)
        {
            _commissionReport.OnNext(commissionReport);
        }

        public void fundamentalData(int reqId, string data)
        {
            _fundamentalDetails.OnNext(new FundamentalDetailsData(reqId, data));
        }

        public void historicalData(int reqId, string date, double open, double high, double low, double close, int volume, int count, double wap, bool hasGaps)
        {
            _historicalBar.OnNext(new BarData(reqId, DateTimeFromString(date), (decimal)open, (decimal)high, (decimal)low, (decimal)close, volume, count, wap, hasGaps));
        }

        public void historicalDataEnd(int reqId, string start, string end)
        {
            _historicalBarEnd.OnNext(new HistoricalBarEndData(reqId, DateTimeFromString(start.Split(' ')[0]), DateTimeFromString(end.Split(' ')[0])));
        }

        public void marketDataType(int reqId, int marketDataType)
        {
            _streamingStatus.OnNext(new StreamingStatusData(reqId, (StreamingStatus)marketDataType));
        }

        public void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            _realtimeDepth.OnNext(new DepthData(tickerId, position, null, (DepthOperation)operation, (DepthSide)side, (decimal)price, size));
        }

        public void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            _realtimeDepth.OnNext(new DepthData(tickerId, position, marketMaker, (DepthOperation)operation, (DepthSide)side, (decimal)price, size));
        }

        public void updateNewsBulletin(int msgId, int msgType, string message, string origExchange)
        {
            _newsBulletin.OnNext(new NewsBulletinData(msgId, (NewsType)msgType, message, origExchange));
        }

        public void position(string account, Contract contract, double pos, double avgCost)
        {
            _position.OnNext(new PositionData(account, contract, (decimal)pos, (decimal)avgCost));
        }

        public void positionEnd()
        {
            _positionEnd.OnNext(null);
        }

        public void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double wap, int count)
        {
            _realtimeBar.OnNext(new BarData(reqId, DateTimeFromSeconds(time), (decimal)open, (decimal)high, (decimal)low, (decimal)close, volume, count, wap, false));
        }

        public void scannerParameters(string xml)
        {
            _scannerParameters.OnNext(xml);
        }

        public void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            _scanner.OnNext(new ScannerData(reqId, rank, contractDetails, distance, benchmark, projection, legsStr));
        }

        public void scannerDataEnd(int reqId)
        {
            _scannerEnd.OnNext(reqId);
        }

        public void receiveFA(int faDataType, string faXmlData)
        {
            _financialAdvisorData.OnNext(new FinancialAdvisorData((FADataType)faDataType, faXmlData));
        }

        public void verifyMessageAPI(string apiData)
        {
            // TODO: New Event
        }

        public void verifyCompleted(bool isSuccessful, string errorText)
        {
            // TODO: New Event
        }

        public void displayGroupList(int reqId, string groups)
        {
            // TODO: New Event
        }

        public void displayGroupUpdated(int reqId, string contractInfo)
        {
            // TODO: New Event
        }

        public void verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
        {
            // TODO: New Event
        }

        public void verifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            // TODO: New Event
        }

        public void positionMulti(int requestId, string account, string modelCode, Contract contract, double pos, double avgCost)
        {
            // TODO: New Event
        }

        public void positionMultiEnd(int requestId)
        {
            // TODO: New Event
        }

        public void accountUpdateMulti(int requestId, string account, string modelCode, string key, string value, string currency)
        {
            // TODO: New Event
        }

        public void accountUpdateMultiEnd(int requestId)
        {
            // TODO: New Event
        }

        public void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass,
            string multiplier, HashSet<string> expirations, HashSet<double> strikes)
        {
            // TODO: New Event
        }

        public void securityDefinitionOptionParameterEnd(int reqId)
        {
            // TODO: New Event
        }

        public void softDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            // TODO: New Event
        }

        public void connectAck()
        {
            if (ClientSocket.AsyncEConnect)
            {
                ClientSocket.startApi();
            }
        }

        private static DateTime DateTimeFromSeconds(long seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
        }

        private static DateTime DateTimeFromString(string str)
        {
            var num = long.Parse(str, CultureInfo.InvariantCulture);

            // str could be the number of seconds since 1970, or date in YYYYMMDD format
            return num < 30000000
                ? new DateTime(int.Parse(str.Substring(0, 4)), int.Parse(str.Substring(4, 2)), int.Parse(str.Substring(6, 2)), 0, 0, 0, DateTimeKind.Utc)
                : DateTimeFromSeconds(num);
        }
    }
}
