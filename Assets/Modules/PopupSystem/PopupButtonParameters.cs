using System;

namespace OctoGamesTest.PopupSystem
{
    public readonly struct PopupButtonParameters
    {
        public PopupButtonParameters(string text, Action callback)
        {
            Text = text;
            Callback = callback;
        }

        public string Text { get; }

        public Action Callback { get; }
    }
}
