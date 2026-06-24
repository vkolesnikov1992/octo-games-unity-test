using System;

namespace OctoGamesTest.PopupSystem
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PresenterBindingAttribute : Attribute
    {
        public PresenterBindingAttribute(Type presenter)
        {
            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            if (!typeof(IPopupPresenter).IsAssignableFrom(presenter))
            {
                throw new ArgumentException(
                    $"Presenter '{presenter.FullName}' must implement '{nameof(IPopupPresenter)}'.",
                    nameof(presenter));
            }

            PresenterType = presenter;
        }

        public Type PresenterType { get; }

        public static PresenterBindingAttribute GetBinding(Type viewType)
        {
            if (viewType == null)
            {
                throw new ArgumentNullException(nameof(viewType));
            }

            if (!typeof(PopupViewBase).IsAssignableFrom(viewType))
            {
                throw new InvalidOperationException(
                    $"Type '{viewType.FullName}' is not a popup view.");
            }

            return Attribute.GetCustomAttribute(
                viewType,
                typeof(PresenterBindingAttribute)) as PresenterBindingAttribute;
        }
    }
}
