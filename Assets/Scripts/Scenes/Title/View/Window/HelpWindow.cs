using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.Title.View.Window
{
    /// <summary>
    /// ヘルプウィンドウ
    /// </summary>
    public class HelpWindow : MonoBehaviour
    {
        [SerializeField] private GameObject helpWindow;
        [SerializeField] private GameObject helpWindowScrollContent;
        [SerializeField] private GameObject helpMobileManual;
        [SerializeField] private GameObject helpConsoleManual;
        [SerializeField] private Button helpOkButton;
        [SerializeField] private EventTrigger helpWindowMaskEventTrigger;
        public void SetActiveHelpWindow(bool isActive)
        {
            // ウィンドウを開く時にスクロール位置をリセットする
            if (isActive)
            {
                var position = helpWindowScrollContent.transform.localPosition;
                position.y = 0;
                helpWindowScrollContent.transform.localPosition = position;
            }
            helpWindow.SetActive(isActive);
        }
        public bool IsActiveHelpWindow()
        {
            return helpWindow.activeSelf;
        }
        public void SetActiveHelpMobileManual(bool isActive)
        {
            helpMobileManual.SetActive(isActive);
        }
        public void SetActiveHelpConsoleManual(bool isActive)
        {
            helpConsoleManual.SetActive(isActive);
        }
        public void AddListenerHelpOkButton(UnityAction action)
        {
            helpOkButton.onClick.AddListener(action);
        }
        public void AddListenerHelpWindowMask(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call =>
            {
                action();
            });
            helpWindowMaskEventTrigger.triggers.Add(entry);
        }
    }
}
