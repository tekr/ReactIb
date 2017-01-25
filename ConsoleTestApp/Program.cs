using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBApi;
using ReactIb;
using ReactIb.Enums;
using ReactIb.Utils;

namespace ConsoleTestApp
{
    internal class Program
    {
        private static readonly ILog Log = new ConsoleLogger();

        private static void Main()
        {
            var program = new Program();
            program.Run().Wait();
        }

        private async Task Run()
        {
            ITwsApi twsApi = new TwsApi("192.168.1.101", 8001, 123, new ConsoleLogger());

            Console.WriteLine($"*** Current TWS time: {await twsApi.GetCurrentTimeAsync()}");

            Console.WriteLine("\n*** Positions:");
            await PrintTaskResults(twsApi.GetPositionsAsync(Scheduler.Default));

            Console.WriteLine("\n*** Account summary:");
            await PrintTaskResults(twsApi.GetAccountSummaryAsync(null,
            new[]
            {
                "AccountType", "Leverage", "SMA", "RegTMargin", "RegTEquity", "DayTradesRemaining", "NetLiquidation", "BuyingPower"
            }));

            var audUsdContract = new Contract
            {
                Currency = "USD",
                Exchange = "IDEALPRO",
                Symbol = "AUD",
                SecType = "CASH"
            };

            Console.WriteLine("\n*** AUD/USD contracts:");
            var contractsTask = twsApi.GetContractDetailsAsync(audUsdContract);
            await PrintTaskResults(contractsTask, c => $"ConId: {c.ContractDetails.Summary.ConId} Name: {c.ContractDetails.LongName}");
            //audUsdContract = contractsTask.Result.First().ContractDetails.Summary;

            Console.WriteLine("\n*** All executions:");
            await PrintTaskResults(twsApi.GetExecutionsAsync());

            Console.WriteLine("\n*** AUD/USD historical bars:");

            // Explicitly pass the default (=ThreadPool) scheduler as ReactIb defaults to immediate otherwise. Immediate will prevent realtime
            // events further down from being received since we block the callback thread waiting for console input, and Rx will internally try
            // to schedule the event callbacks on the same thread
            await PrintTaskResults(twsApi.GetHistoricalBarsAsync(audUsdContract, DateTime.Today.AddDays(-1), DateTime.Today, BarSize.OneHour, HistoricalBarType.Midpoint, Scheduler.Default));

            Console.WriteLine("\n*** Press any key to begin real-time data requests");
            Console.ReadKey();

            Console.WriteLine("\n*** Realtime trades & quotes (press any key to stop):");
            var taqRequest = await twsApi.SubscribeRealtimeTaqAsync(audUsdContract);
            using (taqRequest.Subscribe(t => Console.WriteLine($"Tick type: {t.TickType} Val: {t.Value}")))
            {
                Console.ReadKey();
            }

            Console.WriteLine("\n*** 5-second realtime bars (press any key to continue):");
            var barsRequest = await twsApi.SubscribeRealtimeBarsAsync(audUsdContract, RealtimeBarType.Midpoint);
            using (barsRequest.Subscribe(Console.WriteLine))
            {
                Console.ReadKey();

                Console.WriteLine("\n*** Add in 15 second average closing midpoint price (press any key to stop):");
                barsRequest.Window(TimeSpan.FromSeconds(15)).Subscribe(w => w.Average(bd => bd.Close).Subscribe(avg =>
                                                                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - Avg. midpoint close: {avg}")));
                Console.ReadKey();
            }

            Console.WriteLine("\n*** Realtime depth (press any key to stop):");
            var depthRequest = await twsApi.SubscribeRealtimeDepthAsync(audUsdContract);
            using (depthRequest.Subscribe(Console.WriteLine))
            {
                Console.ReadKey();
            }

            Console.WriteLine("\n*** Open orders:");
            
            // Use the default scheduler here for the same reason as above
            await PrintTaskResults(twsApi.GetOpenOrdersAsync(Scheduler.Default), o => $"OrdId: {o.Order.PermId} State: {o.OrderState.Status}");

            Console.WriteLine("\n*** Requesting periodic account updates (press any key to stop):");
            using (twsApi.Account.Subscribe(Console.WriteLine))
            {
                Console.ReadKey();
            }

            await twsApi.SubscribeAccountUpdatesAsync();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            twsApi.Dispose();
        }

        private static async Task PrintTaskResults<T>(Task<IEnumerable<T>> task, Func<T, object> toStringFunc = null)
        {
            toStringFunc = toStringFunc ?? (t => t);

            try
            {
                foreach (var item in await task)
                {
                    Console.WriteLine($"{toStringFunc(item)}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception! - {e}");
            }
        }
    }
}
