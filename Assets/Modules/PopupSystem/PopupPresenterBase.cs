using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace OctoGamesTest.PopupSystem
{
    public abstract class PopupPresenterBase<TView, TParameters> : IPopupPresenter
        where TView : PopupViewBase<TParameters>
        where TParameters : PopupParameters
    {
        private readonly UniTaskCompletionSource<object> _closedSource = new();
        private readonly CancellationTokenSource _disposeCancellation = new();

        private CancellationTokenSource _activationCancellation;
        private bool _isEntered;
        private bool _attached;
        private bool _disposed;
        private bool _closed;
        private bool _closeStarted;
        private bool _hasResult;
        private object _result;

        protected TView View { get; private set; }

        protected TParameters Parameters { get; private set; }

        protected CancellationToken LifetimeToken => _disposeCancellation.Token;

        protected CancellationToken ActivationToken =>
            _activationCancellation?.Token ?? CancellationToken.None;

        public Type ViewType => typeof(TView);

        public Type ParametersType => typeof(TParameters);

        public UniTask<object> ClosedAsync => _closedSource.Task;

        public void Attach(PopupViewBase view, PopupParameters parameters)
        {
            ThrowIfDisposed();

            if (_attached)
            {
                throw new InvalidOperationException($"Presenter '{GetType().Name}' is already attached.");
            }

            if (view is not TView typedView)
            {
                throw new InvalidOperationException(
                    $"Presenter '{GetType().Name}' requires view '{typeof(TView).Name}', received '{view?.GetType().Name ?? "null"}'.");
            }

            if (parameters is not TParameters typedParameters)
            {
                throw new InvalidOperationException(
                    $"Presenter '{GetType().Name}' requires parameters '{typeof(TParameters).Name}', received '{parameters?.GetType().Name ?? "null"}'.");
            }

            View = typedView;
            Parameters = typedParameters;
            _attached = true;
            typedView.Disabled += OnViewDisabled;
            typedView.Destroyed += OnViewDestroyed;
            OnAttached(typedView, typedParameters);
        }

        public async UniTask EnterAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            ThrowIfNotAttached();

            if (_isEntered)
            {
                return;
            }

            _isEntered = true;
            _activationCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                _disposeCancellation.Token,
                cancellationToken);

            try
            {
                await OnEnterAsync(_activationCancellation.Token);
            }
            catch
            {
                _isEntered = false;
                CancelActivation();
                DisposeActivation();
                throw;
            }
        }

        public async UniTask ExitAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            ThrowIfNotAttached();

            RequestViewClose();
            await _closedSource.Task.AttachExternalCancellation(cancellationToken);
        }

        public async UniTask CloseAsync(object result = null)
        {
            if (_closed)
            {
                return;
            }

            ThrowIfDisposed();
            ThrowIfNotAttached();

            SetResult(result);
            RequestViewClose();
            await _closedSource.Task;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            CancelActivation();
            _disposeCancellation.Cancel();

            try
            {
                UnsubscribeView();

                if (View != null)
                {
                    View.SetVisible(false);
                }

                OnDispose();
                CompleteClose(GetResultOrDefault());
            }
            finally
            {
                DisposeActivation();
                _disposeCancellation.Dispose();
                View = null;
                Parameters = null;
                _attached = false;
            }
        }

        protected virtual void OnAttached(TView view, TParameters parameters)
        {
        }

        protected virtual UniTask OnEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnExitAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnDispose()
        {
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private void ThrowIfNotAttached()
        {
            if (!_attached)
            {
                throw new InvalidOperationException($"Presenter '{GetType().Name}' is not attached to a view.");
            }
        }

        private void OnViewDisabled(PopupViewBase view)
        {
            HandleViewClosed();
        }

        private void OnViewDestroyed(PopupViewBase view)
        {
            HandleViewClosed();
        }

        private void RequestViewClose()
        {
            if (View != null)
            {
                View.SetVisible(false);
            }

            HandleViewClosed();
        }

        private void HandleViewClosed()
        {
            if (_disposed || _closed || _closeStarted)
            {
                return;
            }

            _closeStarted = true;
            ExitAfterViewClosedAsync(CancellationToken.None).Forget(HandleCloseException);
        }

        private async UniTask ExitAfterViewClosedAsync(CancellationToken cancellationToken)
        {
            if (_isEntered)
            {
                _isEntered = false;
                CancelActivation();

                try
                {
                    await OnExitAsync(cancellationToken);
                }
                finally
                {
                    DisposeActivation();
                }
            }

            CompleteClose(GetResultOrDefault());
        }

        private void SetResult(object result)
        {
            _hasResult = true;
            _result = result;
        }

        private object GetResultOrDefault()
        {
            return _hasResult ? _result : null;
        }

        private void CompleteClose(object result)
        {
            if (_closed)
            {
                return;
            }

            _closed = true;
            UnsubscribeView();
            _closedSource.TrySetResult(result);
        }

        private void HandleCloseException(Exception exception)
        {
            if (!_closed)
            {
                _closed = true;
                UnsubscribeView();
                _closedSource.TrySetException(exception);
            }

            Debug.LogException(exception);
        }

        private void UnsubscribeView()
        {
            if (!View)
            {
                return;
            }

            View.Disabled -= OnViewDisabled;
            View.Destroyed -= OnViewDestroyed;
        }

        private void CancelActivation()
        {
            if (_activationCancellation != null && !_activationCancellation.IsCancellationRequested)
            {
                _activationCancellation.Cancel();
            }
        }

        private void DisposeActivation()
        {
            _activationCancellation?.Dispose();
            _activationCancellation = null;
        }
    }
}
