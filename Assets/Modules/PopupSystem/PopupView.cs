using TMPro;
using UnityEngine;

namespace OctoGamesTest.PopupSystem
{
    [PresenterBinding(presenter: typeof(PopupPresenter))]
    public sealed class PopupView : PopupViewBase<MessagePopupParameters>
    {
        [SerializeField]
        private TextMeshProUGUI _title;

        [SerializeField]
        private TextMeshProUGUI _description;

        [SerializeField]
        private Transform _buttonsContainer;

        [SerializeField]
        private PopupButtonView _buttonTemplate;

        public TextMeshProUGUI Title => _title;

        public TextMeshProUGUI Description => _description;

        public Transform ButtonsContainer => _buttonsContainer;

        public PopupButtonView ButtonTemplate => _buttonTemplate;
    }
}
