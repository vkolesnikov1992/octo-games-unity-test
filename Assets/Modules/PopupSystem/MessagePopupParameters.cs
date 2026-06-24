using System;
using System.Collections.Generic;

namespace OctoGamesTest.PopupSystem
{
    public sealed class MessagePopupParameters : PopupParameters
    {
        public const int MinButtons = 1;
        public const int MaxButtons = 5;

        public MessagePopupParameters(
            string title,
            string body,
            params PopupButtonParameters[] buttons)
            : this(title, body, (IEnumerable<PopupButtonParameters>)buttons)
        {
        }

        public MessagePopupParameters(
            string title,
            string body,
            IEnumerable<PopupButtonParameters> buttons)
        {
            if (buttons == null)
            {
                throw new ArgumentNullException(nameof(buttons));
            }

            var buttonList = new List<PopupButtonParameters>();
            foreach (var button in buttons)
            {
                if (string.IsNullOrWhiteSpace(button.Text))
                {
                    throw new ArgumentException("Button text cannot be empty.", nameof(buttons));
                }

                buttonList.Add(button);
            }

            if (buttonList.Count < MinButtons || buttonList.Count > MaxButtons)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(buttons),
                    $"Popup requires from {MinButtons} to {MaxButtons} buttons.");
            }

            Title = title ?? string.Empty;
            Body = body ?? string.Empty;
            Buttons = buttonList;
        }

        public string Title { get; }

        public string Body { get; }

        public IReadOnlyList<PopupButtonParameters> Buttons { get; }
    }
}
