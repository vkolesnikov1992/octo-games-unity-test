using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OctoGamesTest.RefactoringTask
{
    public sealed class CharactersView : MonoBehaviour
    {
        private const string EmptyText = "Characters: 0 Avg value: 0";

        [SerializeField]
        private List<Character> _characters = new();

        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        [Min(1)]
        private int _updateEveryFrames = 1;

        [SerializeField]
        [Min(0f)]
        private float _updateIntervalSeconds = 0.25f;

        [SerializeField]
        private bool _logRefreshes;

        private readonly HashSet<Character> _subscribedCharacters = new();
        private bool _isDirty;
        private int _nextRefreshFrame;
        private float _nextRefreshTime;
        private string _lastText;

        private void Awake()
        {
            Subscribe();
            Refresh();
        }

        private void Update()
        {
            if (!_isDirty || Time.frameCount < _nextRefreshFrame || Time.unscaledTime < _nextRefreshTime)
            {
                return;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            foreach (var character in _characters)
            {
                if (character == null || !_subscribedCharacters.Add(character))
                {
                    continue;
                }

                character.ValueChanged += MarkDirty;
            }
        }

        private void Unsubscribe()
        {
            foreach (var character in _subscribedCharacters)
            {
                character.ValueChanged -= MarkDirty;
            }

            _subscribedCharacters.Clear();
        }

        private void MarkDirty()
        {
            _isDirty = true;
        }

        private void Refresh()
        {
            _isDirty = false;
            _nextRefreshFrame = Time.frameCount + Mathf.Max(1, _updateEveryFrames);
            _nextRefreshTime = Time.unscaledTime + Mathf.Max(0f, _updateIntervalSeconds);

            var count = 0;
            var totalValue = 0f;

            foreach (var character in _characters)
            {
                if (character == null)
                {
                    continue;
                }

                count++;
                totalValue += character.Value;
            }

            var text = count == 0
                ? EmptyText
                : $"Characters: {count} Avg value: {totalValue / count:0.##}";

            if (_text != null && _lastText != text)
            {
                _text.text = text;
                _lastText = text;
            }

            if (_logRefreshes)
            {
                Debug.Log(text);
            }
        }
    }
}
