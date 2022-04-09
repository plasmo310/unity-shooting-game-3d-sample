using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.Common.View
{
    /// <summary>
    /// 音量スライダー
    /// </summary>
    public class AudioSlider : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] private Slider slider;
        
        /// <summary>
        /// 値を変更した時(離した時)のイベント
        /// </summary>
        public event Action<float> OnValueChanged;
        
        /// <summary>
        /// 値の設定
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(float value)
        {
            slider.value = value;
        }

        /// <summary>
        /// スライダーを離した時
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            // 小数第一位以降は切り捨てる
            var value = float.Parse(slider.value.ToString("N1"));
            slider.value = value;
            // イベント発火
            OnValueChanged?.Invoke(value);
        }
    }
}
