using System;
using System.Threading;

namespace ReactIb.Utils
{
    public class ObservableWrapper<T> : IObservable<T>
    {
        private readonly IObservable<T> _observable;
        private readonly Action _subscribeAction;
        private readonly Action _disposeAction;

        private int _subscribed;

        public ObservableWrapper(IObservable<T> observable, Action subscribeAction, Action disposeAction)
        {
            _observable = observable;
            _subscribeAction = subscribeAction;
            _disposeAction = disposeAction;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var disposable = _observable.Subscribe(observer);

            if (Interlocked.CompareExchange(ref _subscribed, 1, 0) == 0)
            {
                _subscribeAction();
            }

            return new DisposableWrapper(disposable, _disposeAction);
        }

        private class DisposableWrapper : IDisposable
        {
            private readonly IDisposable _disposable;
            private readonly Action _disposeAction;

            public DisposableWrapper(IDisposable disposable, Action disposeAction)
            {
                _disposable = disposable;
                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                _disposable.Dispose();
                _disposeAction();
            }
        }
    }
}
