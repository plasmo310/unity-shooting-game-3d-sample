using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Scenes.Game.View
{
    /// <summary>
    /// GameCtrlCanvas
    /// </summary>
    public class GameCtrlCanvas : MonoBehaviour
    {
        private event Action OnDestroyed; // Destroy時に呼ばれる処理
        
        // ----- Canvas全体 -----
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        // ----- ショットボタン関連 -----
        [SerializeField] private Sprite shotButtonPushSprite;
        [SerializeField] private Sprite shotButtonNoPushSprite;
        [SerializeField] private Image shotButtonImage;
        [SerializeField] private EventTrigger shotButtonEventTrigger;
        [SerializeField] private Image shotUpArrowImage;
        [SerializeField] private RectTransform shotUpArrowRectTransform;
        [SerializeField] private Image shotDownArrowImage;
        [SerializeField] private RectTransform shotDownArrowRectTransform;
        private Sequence _shotUpArrowSequence;
        private Vector3 _shotUpArrowInitPosition;
        private Sequence _shotDownArrowSequence;
        private Vector3 _shotDownArrowInitPosition;
        public void AddListenerPointerDownShotButton(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call =>
            {
                action();
            });
            shotButtonEventTrigger.triggers.Add(entry);
        }
        public void AddListenerPointerUpShotButton(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener(call =>
            {
                action();
            });
            shotButtonEventTrigger.triggers.Add(entry);
        }
        public void ChangeSpriteShotButton(bool isPush)
        {
            shotButtonImage.sprite = isPush ? shotButtonPushSprite : shotButtonNoPushSprite;
        }
        public void SetActiveShotUpArrowImage(bool isActive)
        {
            // 変更されていなければ処理終了
            if (shotUpArrowImage.IsActive() == isActive)
            {
                return;
            }
            shotUpArrowImage.gameObject.SetActive(isActive);
            
            // Sprite更新処理
            if (isActive)
            {
                // シーケンスの作成
                if (_shotUpArrowSequence == null)
                {
                    _shotUpArrowInitPosition = shotUpArrowRectTransform.localPosition;
                    _shotUpArrowSequence = DOTween.Sequence();
                    _shotUpArrowSequence.Append(shotUpArrowRectTransform.DOMove(30.0f * Vector3.up, 0.5f).SetRelative());
                    _shotUpArrowSequence.Join(DOTweenUtil.FeedImageAlpha(shotUpArrowImage, 0.0f, 1.0f, 0.5f));
                    _shotUpArrowSequence.AppendInterval(0.2f);
                    _shotUpArrowSequence.AppendCallback(() => shotUpArrowRectTransform.localPosition = _shotUpArrowInitPosition);
                    _shotUpArrowSequence.SetLoops(-1);
                    OnDestroyed += () => { _shotUpArrowSequence.Kill(); }; // Destroy時にKillする
                    return;
                }
                // 再生を再開する
                _shotUpArrowSequence?.Restart();
            }
            else
            {
                // 一時停止する
                _shotUpArrowSequence?.Pause();
            }
        }
        public void SetActiveShotDownArrowImage(bool isActive)
        {
            // 変更されていなければ処理終了
            if (shotDownArrowImage.IsActive() == isActive)
            {
                return;
            }
            shotDownArrowImage.gameObject.SetActive(isActive);
            
            // Sprite更新処理
            if (isActive)
            {
                // シーケンスの作成
                if (_shotDownArrowSequence == null)
                {
                    _shotDownArrowInitPosition = shotDownArrowRectTransform.localPosition;
                    _shotDownArrowSequence = DOTween.Sequence();
                    _shotDownArrowSequence.Append(shotDownArrowRectTransform.DOMove(-30.0f * Vector3.up, 0.5f).SetRelative());
                    _shotDownArrowSequence.Join(DOTweenUtil.FeedImageAlpha(shotDownArrowImage, 0.0f, 1.0f, 0.5f));
                    _shotDownArrowSequence.AppendInterval(0.2f);
                    _shotDownArrowSequence.AppendCallback(() => shotDownArrowRectTransform.localPosition = _shotDownArrowInitPosition);
                    _shotDownArrowSequence.SetLoops(-1);
                    OnDestroyed += () => { _shotDownArrowSequence.Kill(); }; // Destroy時にKillする
                    return;
                }
                // 再生を再開する
                _shotDownArrowSequence?.Restart();
            }
            else
            {
                // 一時停止する
                _shotDownArrowSequence?.Pause();
            }
        }

        // ----- ビームボタン関連 -----
        [SerializeField] private GameObject beamButton;
        [SerializeField] private Image beamGaugeImage;
        [SerializeField] private EventTrigger beamButtonEventTrigger;
        [SerializeField] private Image beamButtonImage;
        [SerializeField] private Sprite beamButtonMaxSprite;
        [SerializeField] private Sprite beamButtonDisableSprite;
        public void SetBeamGaugeValue(float ratio)
        {
            beamGaugeImage.fillAmount = ratio;
        }
        public float GetBeamGaugeValue()
        {
            return beamGaugeImage.fillAmount;
        }
        public void AddListenerPointerDownBeamButton(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call =>
            {
                action();
            });
            beamButtonEventTrigger.triggers.Add(entry);
        }
        public void SetBeamButtonImage(bool isMax)
        {
            beamButtonImage.sprite = isMax ? beamButtonMaxSprite : beamButtonDisableSprite;
        }
        
        // ----- 移動スティックボタン関連 -----
        [SerializeField] private EventTrigger moveStickEventTrigger;
        [SerializeField] private RectTransform moveStickRectTransform;
        [SerializeField] private Image moveStickImage;
        [SerializeField] private Sprite moveStickSprite;
        [SerializeField] private Sprite moveStickLSprite;
        [SerializeField] private Sprite moveStickRSprite;
        [SerializeField] private Sprite moveStickWeakLSprite;
        [SerializeField] private Sprite moveStickWeakRSprite;
        public RectTransform RectTransformMoveStickMoveInButton => moveStickRectTransform;
        
        public void AddListenerPointerDownMoveStick(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call => action());
            moveStickEventTrigger.triggers.Add(entry);
        }
        
        public void AddListenerPointerUpMoveStick(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener(call => action());
            moveStickEventTrigger.triggers.Add(entry);
        }
        public bool ChangeMoveStickSprite(bool isLeft, bool isRight, bool isWeakLeft, bool isWeakRight)
        {
            Sprite sprite;
            if (isLeft == isRight && isWeakLeft == isWeakRight)
            {
                sprite = moveStickSprite;
            }
            else if (isLeft)
            {
                sprite = moveStickLSprite;
            }
            else if (isRight)
            {
                sprite = moveStickRSprite;
            }
            else if (isWeakLeft)
            {
                sprite = moveStickWeakLSprite;
            }
            else
            {
                sprite = moveStickWeakRSprite;
            }
            if (moveStickImage.sprite != sprite)
            {
                moveStickImage.sprite = sprite;
                return true;
            }
            return false;
        }
        
        private void OnDestroy()
        {
            OnDestroyed?.Invoke();;
        }
    }
}
