using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scenes.Common.View
{
    /// <summary>
    /// 共通ダイアログCanvas
    /// </summary>
    public class DialogCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject dialogCanvas;
        public void SetActiveDialogCanvas(bool isActive)
        {
            dialogCanvas.SetActive(isActive);
        }

        [SerializeField] private GameObject normalDialog;
        [SerializeField] private Text normalDialogText;
        [SerializeField] private Button normalDialogOkButton;
        public void SetActiveNormalDialog(bool isActive)
        {
            normalDialog.SetActive(isActive);
        }
        public void SetTextNormalDialog(string text)
        {
            normalDialogText.text = text;
        }
        public void UpdateListenerNormalDialogOkButton(UnityAction action)
        {
            normalDialogOkButton.onClick.RemoveAllListeners();
            normalDialogOkButton.onClick.AddListener(action);
        }
    }
}
