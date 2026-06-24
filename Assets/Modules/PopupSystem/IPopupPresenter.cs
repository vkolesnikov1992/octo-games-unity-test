using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace OctoGamesTest.PopupSystem
{
    public interface IPopupPresenter : IDisposable
    {
        Type ViewType { get; }

        Type ParametersType { get; }

        UniTask<object> ClosedAsync { get; }

        void Attach(PopupViewBase view, PopupParameters parameters);

        UniTask EnterAsync(CancellationToken cancellationToken);

        UniTask ExitAsync(CancellationToken cancellationToken);

        UniTask CloseAsync(object result = null);
    }
}
