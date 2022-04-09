using System;
using System.Collections;
using DG.Tweening;
using Scenes.Title.View.Window;
using Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;
using Utils.Components;

namespace Scenes.Title.View
{
    /// <summary>
    /// TitleCanvas
    /// </summary>
    public class TitleCanvas : MonoBehaviour
    {
        private event Action OnDestroyed; // Destroy時に呼ばれる処理
        
        // ----- 各ウィンドウ -----
        public NormalModeWindow NormalModeWindow { get; private set; }
        public EndlessModeWindow EndlessModeWindow { get; private set; }
        public HelpWindow HelpWindow { get; private set; }
        public SettingWindow SettingWindow { get; private set; }
        public InputNameWindow InputNameWindow { get; private set; }
        
        private void Awake()
        {
            // Prefabから各UIを生成する
            NormalModeWindow = CreateCanvasUI<NormalModeWindow>(GameConst.UITitleNormalModeWindow);
            EndlessModeWindow = CreateCanvasUI<EndlessModeWindow>(GameConst.UITitleEndlessModeWindow);
            HelpWindow = CreateCanvasUI<HelpWindow>(GameConst.UITitleHelpWindow);
            SettingWindow = CreateCanvasUI<SettingWindow>(GameConst.UITitleSettingWindow);
            InputNameWindow = CreateCanvasUI<InputNameWindow>(GameConst.UITitleInputNameWindow);
        }
        
        /// <summary>
        /// CanvasのUI生成処理
        /// </summary>
        /// <param name="prefabName">UIPrefab名</param>
        /// <typeparam name="T">UIクラス</typeparam>
        /// <returns>UIクラス</returns>
        private T CreateCanvasUI<T>(string prefabName)
        {
            var prefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(prefabName);
            return Instantiate(prefab, transform, false).GetComponent<T>();
        }
        
        // ----- タイトルCanvas -----
        [SerializeField] private GameObject titleCanvas;
        public void SetActiveTitleCanvas(bool isActive)
        {
            titleCanvas.SetActive(isActive);
        }
        
        // ----- カニ -----
        [SerializeField] private KaniBoundCtrl kaniBound;
        public void ShowKaniBound()
        {
            kaniBound.Show();
        }
        
        // ----- 画面TOPボタン -----
        [SerializeField] private GameObject topButtonArea;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button moreAppButton;
        public void SetActiveTopButtonArea(bool isActive)
        {
            topButtonArea.SetActive(isActive);
        }
        public void AddListenerHelpButton(UnityAction action)
        {
            helpButton.onClick.AddListener(action);
        }
        public void AddListenerSettingButton(UnityAction action)
        {
            settingButton.onClick.AddListener(action);
        }
        public void AddListenerMoreAppButton(UnityAction action)
        {
            moreAppButton.onClick.AddListener(action);
        }

        // ----- プレイヤー名 -----
        [SerializeField] private GameObject playerNameArea;
        [SerializeField] private Text playerNameText;
        public void SetActivePlayerNameArea(bool isActive)
        {
            playerNameArea.gameObject.SetActive(isActive);
        }
        public void SetPlayerNameText(string text)
        {
            playerNameText.text = text;
        }
        
        // ----- タイトル画像 -----
        [SerializeField] private Image titleImage;
        [SerializeField] private RectTransform titleImageRectTransform;
        [SerializeField] private ShakeEffect titleImageShakeEffect;
        public void SetActiveTitleImage(bool isActive)
        {
            titleImage.gameObject.SetActive(isActive);
        }
        public bool IsActiveTitleImage()
        {
            return titleImage.IsActive();
        }
        public void ShowTitleImage(UnityAction callback)
        {
            var sequence = DOTween.Sequence();
            // 手前から勢いよく現れる
            sequence.Append(DOTweenUtil.DoScaleTransform(titleImageRectTransform, 10.0f * titleImageRectTransform.localScale,
                titleImageRectTransform.localScale, 1.0f, Ease.InExpo));
            sequence.AppendCallback(() => titleImageShakeEffect.StartShake(0.2f, 35.0f, 15.0f));
            sequence.AppendCallback(() => ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeBomb));
            
            // ドーンのタイミングでコールバック実行
            sequence.AppendCallback(() => callback());
            
            sequence.AppendInterval(1.2f);
            sequence.AppendCallback(() =>
            {
                // 一定間隔でバウンドする
                var loopSequence = DOTween.Sequence();
                loopSequence.Append(titleImageRectTransform.DOScale(new Vector3(0.1f, 0.1f), 0.1f).SetRelative());
                loopSequence.Append(titleImageRectTransform.DOScale(new Vector3(-0.1f, -0.1f), 0.15f).SetRelative());
                loopSequence.AppendInterval(0.75f);
                loopSequence.SetLoops(-1);
                OnDestroyed += () => { loopSequence.Kill(); }; // Destroy時にKillする
            });
        }
        
        // ----- モード選択関連 -----
        [SerializeField] private Button normalModeButton;
        [SerializeField] private Button endlessModeButton;
        [SerializeField] private RectTransform selectModeFrameRectTransform;

        public void SetActiveNormalModeButton(bool isActive)
        {
            normalModeButton.gameObject.SetActive(isActive);
        }
        public void SetActiveEndlessModeButton(bool isActive)
        {
            endlessModeButton.gameObject.SetActive(isActive);
        }
        public void AddListenerNormalModeButton(UnityAction action)
        {
            normalModeButton.onClick.AddListener(action);
        }
        public void AddListenerEndlessModeButton(UnityAction action)
        {
            endlessModeButton.onClick.AddListener(action);
        }
        public void SetActiveSelectModeFrame(bool isActive)
        {
            selectModeFrameRectTransform.gameObject.SetActive(isActive);
        }
        public void MovePositionSelectModeFrame(Vector3 position)
        {
            selectModeFrameRectTransform.DOMove(position, 0.2f);
        }

        /// <summary>
        /// 選択モード切り替え処理
        /// </summary>
        public void ChangeSelectMode(int selectMode)
        {
            var position = Vector3.zero;
            
            switch (selectMode)
            {
                case GameConst.GameModeNormal:
                    if (normalModeButton.IsActive())
                        position = normalModeButton.transform.position;
                    break;
                case GameConst.GameModeEndless:
                    if (endlessModeButton.IsActive())
                        position = endlessModeButton.transform.position;
                    break;
            }

            // 選択フレームを移動する
            if (position != Vector3.zero)
            {
                MovePositionSelectModeFrame(position);
            }
        }

        // ----- マスク -----
        [SerializeField] private Image maskImage;
        private const float ShowMaskSpeed = 0.07f;
        public void HideMask()
        {
            maskImage.fillAmount = 0.0f;
        }
        public void ShowMask(UnityAction callback)
        {
            maskImage.transform.SetAsLastSibling(); // 最前面にする
            StartCoroutine(ShowMaskCoroutine(callback));
        }
        private IEnumerator ShowMaskCoroutine(UnityAction callback)
        {
            while (true)
            {
                // マスクを徐々に表示させる
                maskImage.fillAmount += ShowMaskSpeed;
                // 1.0fになったらコールバック実行して抜ける
                if (Mathf.Approximately(maskImage.fillAmount, 1.0f))
                {
                    callback();
                    yield break;
                }
                yield return null;
            }
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();;
        }
    }
}
