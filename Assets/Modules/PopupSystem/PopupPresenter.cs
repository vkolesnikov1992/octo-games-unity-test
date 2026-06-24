using Cysharp.Threading.Tasks;
using System.Threading;

namespace OctoGamesTest.PopupSystem
{
    public class PopupPresenter : PopupPresenterBase<PopupView, MessagePopupParameters>
    {
        protected override UniTask OnEnterAsync(CancellationToken cancellationToken)
        {
            View.Title.text = Parameters.Title;
            View.Description.text = Parameters.Body;

            foreach (var parametersButton in Parameters.Buttons)
            {
                CreateButton(parametersButton);
            }
            
            return UniTask.CompletedTask;
        }

        private void CreateButton(PopupButtonParameters parameters)
        {
            var button = UnityEngine.Object.Instantiate(View.ButtonTemplate, View.ButtonsContainer);
            button.gameObject.SetActive(true);
            button.Initialize(parameters);
        }
    }
}
