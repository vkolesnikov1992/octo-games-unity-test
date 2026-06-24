using System;
using Zenject;

namespace OctoGamesTest.PopupSystem
{
    public static class PopupSystemModule
    {
        public static void Register(DiContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Bind<PopupPresenter>().AsTransient();
        }
    }
}
