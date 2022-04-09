using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utils
{
    /// <summary>
    /// DOTween共通クラス
    /// </summary>
    public class DOTweenUtil : MonoBehaviour
    {
        /// <summary>
        /// テキストを加算する
        /// </summary>
        public static Tween IncrementText(Text text, float current, float target, float duration = 1.0f, UnityAction<float> setAction = null)
        {
            return DOTween.To( (val) =>
            {
                current = val;
                if (setAction != null)
                {
                    setAction(val);
                }
                else
                {
                    text.text = Mathf.RoundToInt(val).ToString();
                }
            }, current, target, duration);
        }

        /// <summary>
        /// Scaleを変更する
        /// </summary>
        public static Tween DoScaleTransform(Transform transform, Vector3 current, Vector3 target, float duration = 1.0f, Ease ease = Ease.Linear)
        {
            transform.localScale = current;
            return transform.DOScale(target, duration).SetEase(ease);
        }

        /// <summary>
        /// localPositionを変更する
        /// </summary>
        public static Tween DoLocalMoveTransform(Transform transform, Vector3 current, Vector3 target, float duration = 1.0f, Ease ease = Ease.Linear)
        {
            transform.localPosition = current;
            return transform.DOLocalMove(target, duration).SetEase(ease);
        }
        
        /// <summary>
        /// Alpha値をフェードする
        /// </summary>
        public static Tween FeedImageAlpha(Image image, float current, float target, float duration = 1.0f)
        {
            var tmp = image.color;
            tmp.a = current;
            image.color = tmp;
            return DOTween.ToAlpha(
                () => image.color,
                color => image.color = color,
                target,
                duration);
        }
    }
}
