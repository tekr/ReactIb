using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ReactIb.DataTypes;
using ReactIb.Exceptions;

namespace ReactIb
{
    public partial class TwsApi
    {
        private class HistoricalDataListFetcher : ListFetcher<BarData, HistoricalBarEndData>
        {
            public HistoricalDataListFetcher(TwsApi twsApi, IObservable<BarData> itemObservable, IObservable<HistoricalBarEndData> itemEndObservable,
                    Func<BarData, int> itemRequestIdExtractor = null, Func<HistoricalBarEndData, int> itemListEndRequestIdExtractor = null)
                : base(twsApi, itemObservable, itemEndObservable, itemRequestIdExtractor, itemListEndRequestIdExtractor)
            {
            }

            protected override Exception GetErrorFromMessage(DataTypes.MessageData message) =>
                        // TODO: Need to handle error code 321 in here?
                        !IsCompletionMessage(message) && message.Code == 162 || message.Code == 354
                                                            ? new TwsException(message.Code, message.Value,
                                                                    (message.Code != 162 || !message.Value.Contains("No market data permissions")) &&
                                                                    (message.Code != 354 || !message.Value.Contains("market data is not subscribed")))
                                                            : base.GetErrorFromMessage(message);

            protected override bool IsCompletionMessage(DataTypes.MessageData message) =>
                        message.Code == 162 && message.Value.Contains("query returned no data") || base.IsCompletionMessage(message);
        }

        private class ContractDetailsListFetcher : ListFetcher<ContractDetailsData, int>
        {
            public ContractDetailsListFetcher(TwsApi twsApi, IObservable<ContractDetailsData> itemObservable, IObservable<int> itemEndObservable,
                    Func<ContractDetailsData, int> itemRequestIdExtractor = null, Func<int, int> itemListEndRequestIdExtractor = null)
                : base(twsApi, itemObservable, itemEndObservable, itemRequestIdExtractor, itemListEndRequestIdExtractor)
            {
            }

            protected override bool IsCompletionMessage(DataTypes.MessageData message) =>
                        message.Code == 200 && message.Value.Contains("No security definition has been found") || base.IsCompletionMessage(message);
        }

        private class ListFetcher<TItemArgs, TItemListEndArgs>
        {
            private const int MaxAttempts = 5;
            private const int RetryDelayMs = 500;
            private readonly TwsApi _twsApi;

            private readonly IObservable<TItemArgs> _itemObservable;
            private readonly IObservable<TItemListEndArgs> _itemEndObservable;
            private readonly Func<TItemArgs, int> _itemRequestIdExtractor;
            private readonly Func<TItemListEndArgs, int> _itemListEndRequestIdExtractor;

            private bool UsesRequestIds => _itemRequestIdExtractor != null && _itemListEndRequestIdExtractor != null;
            
            public ListFetcher(TwsApi twsApi, IObservable<TItemArgs> itemObservable, IObservable<TItemListEndArgs> itemEndObservable,
                        Func<TItemArgs, int> itemRequestIdExtractor = null, Func<TItemListEndArgs, int> itemListEndRequestIdExtractor = null)
            {
                _twsApi = twsApi;

                _itemObservable = itemObservable;
                _itemEndObservable = itemEndObservable;

                _itemRequestIdExtractor = itemRequestIdExtractor;
                _itemListEndRequestIdExtractor = itemListEndRequestIdExtractor;
            }

            public async Task<IEnumerable<TItemArgs>> RunAsync(Action<int> sendAction, IScheduler scheduler)
            {
                scheduler = scheduler ?? ImmediateScheduler.Instance;
                var attemptsRemaining = MaxAttempts;
                Exception lastException = null;

                while (attemptsRemaining-- > 0)
                {
                    var requestId = -1;

                    try
                    {
                        var results = new ReplaySubject<TItemArgs>();

                        await _twsApi.ConnectionReadyAsync();
                        requestId = UsesRequestIds ? _twsApi.GetNextRequestId() : 0;

                        var ourMessages = _twsApi.Message.Where(e => UsesRequestIds && e.RequestId == requestId);

                        using (_itemObservable.Where(i => IsForThisRequest(i, _itemRequestIdExtractor, requestId)).ObserveOn(scheduler).Subscribe(results))
                        using (_itemEndObservable.Any(i => IsForThisRequest(i, _itemListEndRequestIdExtractor, requestId)).Merge(ourMessages.Any(IsCompletionMessage))
                                    .ObserveOn(scheduler).Subscribe(_ => results.OnCompleted()))
                        using (_twsApi.ConnectionStatus.Where(s => s == ReactIb.ConnectionStatus.Disconnected).Select(_ =>
                                new ConnectionLostException()).Merge(ourMessages.Select(GetErrorFromMessage).Where(e => e != null)).ObserveOn(scheduler).Subscribe(e => results.OnError(e)))
                        {
                            sendAction(requestId);

                            // Don't want an Exception thrown if there result list is empty
                            await results.DefaultIfEmpty();

                            return results.ToEnumerable();
                        }
                    }
                    catch (ConnectionLostException e)
                    {
                         _twsApi._log.Warn($"Lost connection during list request with id {requestId}");
                        lastException = e;
                    }
                    catch (TwsException e) when (!e.MayBeTransient)
                    {
                        throw;
                    }

                    await Task.Delay(RetryDelayMs);
                }

                _twsApi._log.Error($"Giving up on request {sendAction} after {MaxAttempts} attempts");
                throw new TwsException(-1, "Retries exceeded", false, lastException);
            }


            protected virtual Exception GetErrorFromMessage(DataTypes.MessageData message) => !IsCompletionMessage(message)
                                                                                                ? new TwsException(message.Code, message.Value, message.Code != 200)
                                                                                                : null;

            protected virtual bool IsCompletionMessage(DataTypes.MessageData message) => false;

            private bool IsForThisRequest<T>(T item, Func<T, int> idExtractor, int id) => !UsesRequestIds || idExtractor(item) == id;
        }
    }
}
