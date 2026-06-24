using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OctoGamesTest.PopupSystem
{
    public sealed class PopupButtonView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _label;

        [SerializeField]
        private Button _button;

        private PopupButtonParameters _parameters;

        public void Initialize(PopupButtonParameters parameters)
        {
            _label.text = parameters.Text;
            _parameters = parameters;

            _button.onClick.RemoveListener(OnClicked);
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            _parameters.Callback?.Invoke();
        }
    }
}
