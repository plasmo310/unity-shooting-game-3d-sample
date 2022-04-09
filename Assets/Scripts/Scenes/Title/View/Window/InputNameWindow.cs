using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.Title.View.Window
{
    /// <summary>
    /// 名前入力ウィンドウ
    /// </summary>
    public class InputNameWindow : MonoBehaviour
    {
        [SerializeField] private GameObject inputNameWindow;
        [SerializeField] private EventTrigger inputNameWindowMaskEventTrigger;
        [SerializeField] private InputField inputNameText;
        [SerializeField] private Button inputNameOkButton;
        public void SetActiveInputNameWindow(bool isActive)
        {
            inputNameWindow.SetActive(isActive);
        }
        public bool IsActiveInputNameWindow()
        {
            return inputNameWindow.activeSelf;
        }
        public void UpdateListenerInputNameWindowMask(UnityAction action)
        {
            // 登録されているイベントをクリア
            inputNameWindowMaskEventTrigger.triggers.Clear();
            
            // イベントを登録
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call =>
            {
                action();
            });
            inputNameWindowMaskEventTrigger.triggers.Add(entry);
        }
        public void SetInputNameText(string text)
        {
            inputNameText.text = text;
        }
        public string GetInputNameText()
        {
            return inputNameText.text;
        }
        public void UpdateListenerInputNameOkButton(UnityAction action)
        {
            // 削除して追加
            inputNameOkButton.onClick.RemoveAllListeners();
            inputNameOkButton.onClick.AddListener(action);
        }
    }
}
