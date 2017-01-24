using System;
using System.Threading;
using ConsoleTestApp;
using IBApi;
using NUnit.Framework;
using ReactIb.Utils;

namespace ReactIb.ManualTests
{
    [TestFixture]
    public class OrderTests
    {
        private static readonly ILog Log = new SimpleConsoleLogger();

        private ITwsApi _twsApi;

        [SetUp]
        public void Setup()
        {
            _twsApi = new TwsApi(new SimpleConsoleLogger(), "192.168.1.102", 4001, 5);
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
