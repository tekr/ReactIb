using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using ConsoleTestApp;
using IBApi;
using NUnit.Framework;
using ReactIb.Enums;
using ReactIb.Utils;

namespace ReactIb.ManualTests
{
    [TestFixture]
    public class OrderTests
    {
        private static readonly ILog Log = new ConsoleLogger();

        private ITwsApi _twsApi;

        [SetUp]
        public void Setup()
        {
            _twsApi = new TwsApi("192.168.1.102", 4001, 5, new ConsoleLogger());
        }

        [Test]
        public async void TestFetchData()
        {
            // Connect to TWS on localhost at default port (7496)
            ITwsApi twsApi = new TwsApi();

            // Get IEnumerable of current positions asynchronously
            var positions = await twsApi.GetPositionsAsync();

            // Get IEnumerable of executions asynchronously
            var executions = await twsApi.GetExecutionsAsync();

            // Create minimally-populated contract object
            var audUsdContract = new Contract
            {
                Currency = "USD",
                Exchange = "IDEALPRO",
                Symbol = "AUD",
                SecType = "CASH"
            };

            // Fetch the last day's worth of hourly bars asynchronously
            var historicalBars = await twsApi.GetHistoricalBarsAsync(audUsdContract, DateTime.Today.AddDays(-1),
                                            DateTime.Today, BarSize.OneHour, HistoricalBarType.Midpoint);


            // Print entry notionals of positions by currency
            var notionalByCcy = (await twsApi.GetPositionsAsync(Scheduler.Default)).GroupBy(pd => pd.Contract.Currency).Select(g =>
                                        new { Currency = g.Key, Notional = g.Sum(pd => pd.Position * pd.AverageCost) });

            foreach (var entry in notionalByCcy)
            {
                Console.WriteLine($"Currency: {entry.Currency} Notional: {entry.Notional}");
            }
        }

        [Test]
        public async void TestExecuteOrder()
        {
            _twsApi.Execution.Subscribe(ed => Log.Info($"Execution received: {ed}"));

            var clContract = new Contract
            {
                Currency = "USD",
                Exchange = "IDEALPRO",
                Symbol = "AUD",
                SecType = "CASH"
            };

            var order = _twsApi.CreateOrder();
            order.LmtPrice = 0.9;
            order.TotalQuantity = 100000;
            order.Action = "BUY";
            order.OrderType = "LMT";
            order.Tif = "DAY";

            var dateTime = await _twsApi.SendOrderAsync(order.OrderId, clContract, order);

            Log.Info($"DateTime from sending: {dateTime}");

            Thread.Sleep(500);

            var executions = await _twsApi.GetExecutionsAsync();
            Log.Info("Executions received:");
            foreach (var execution in executions)
            {
                Log.Info(execution);
            }

            Thread.Sleep(5000);

            Log.Info("Done");
        }
    }
}
