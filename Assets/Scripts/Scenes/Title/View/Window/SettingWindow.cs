using Scenes.Common.View;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.Title.View.Window
{
    /// <summary>
    /// 設定ウィンドウ
    /// </summary>
    public class SettingWindow : MonoBehaviour
    {
        [SerializeField] private GameObject settingWindow;
        [SerializeField] private EventTrigger settingWindowMaskEventTrigger;
        [SerializeField] private Button settingChangeNameButton;
        [SerializeField] private AudioSlider settingBgmSlider;
        [SerializeField] private AudioSlider settingSeSlider;
        public void SetActiveSettingWindow(bool isActive)
        {
            settingWindow.SetActive(isActive);
        }
        public bool IsActiveSettingWindow()
        {
            return settingWindow.activeSelf;
        }
        public void AddListenerSettingWindowMask(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call =>
            {
                action();
            });
            settingWindowMaskEventTrigger.triggers.Add(entry);
        }
        public void SetActiveSettingChangeNameButton(bool isActive)
        {
            settingChangeNameButton.gameObject.SetActive(isActive);
        }
        public void AddListenerSettingChangeNameButton(UnityAction action)
        {
            settingChangeNameButton.onClick.AddListener(action);
        }
        public void SetValueSettingBgmSlider(float value)
        {
            settingBgmSlider.SetValue(value);
        }
        public void SetValueSettingSeSlider(float value)
        {
            settingSeSlider.SetValue(value);
        }
        public void AddValueChangeSettingBgmSlider(Action<float> action)
        {
            settingBgmSlider.OnValueChanged += action;
        }
        public void AddValueChangeSettingSeSlider(Action<float> action)
        {
            settingSeSlider.OnValueChanged += action;
        }
    }
}
