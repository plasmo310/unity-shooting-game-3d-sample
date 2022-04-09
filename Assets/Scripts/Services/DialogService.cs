using Scenes.Common.View;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using Utils.Components;

namespace Services
{
    /// <summary>
    /// ダイアログ共通Service
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly DialogCanvas _dialogCanvas;

        public DialogService()
        {
            var dialogCanvasPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.UIDialogCanvas);
            var dialogCanvasObj = ServiceLocator.Resolve<IMonoBehaviorService>().DoInstantiate(dialogCanvasPrefab);
            dialogCanvasObj.AddComponent<DontDestroyObject>(); // DontDestroyにする
            _dialogCanvas = dialogCanvasObj.GetComponent<DialogCanvas>();
            _dialogCanvas.SetActiveDialogCanvas(false);
        }

        /// <summary>
        /// ノーマルダイアログ表示
        /// </summary>
        /// <param name="text"></param>
        /// <param name="callback"></param>
        public void ShowNormalDialog(string text, UnityAction callback)
        {
            _dialogCanvas.SetTextNormalDialog(text);
            _dialogCanvas.UpdateListenerNormalDialogOkButton(callback);;
            _dialogCanvas.SetActiveDialogCanvas(true);
        }

        /// <summary>
        /// ノーマルダイアログ非表示
        /// </summary>
        public void HideNormalDialog()
        {
            _dialogCanvas.SetActiveDialogCanvas(false);
        }
    }
}
