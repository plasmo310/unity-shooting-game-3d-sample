using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Scenes.Title.View.Window
{
    /// <summary>
    /// ノーマルウィンドウ
    /// </summary>
    public class NormalModeWindow : MonoBehaviour
    {
        [SerializeField] private GameObject normalModeWindow;
        [SerializeField] private EventTrigger normalModeWindowMaskEventTrigger;
        [SerializeField] private Button normalModeGoButton;
        [SerializeField] private Text normalModeStageDetailText;
        [SerializeField] private Text normalModeEnemyCountText;
        [SerializeField] private Text normalModeHighScoreText;
        [SerializeField] private Toggle normalModeEasyToggle;
        [SerializeField] private Toggle normalModeNormalToggle;
        [SerializeField] private Toggle normalModeHardToggle;
        [SerializeField] private RectTransform normalModeSelectStageFrameRectTransform;
        public void SetActiveNormalModeWindow(bool isActive)
        {
            normalModeWindow.SetActive(isActive);
        }
        public bool IsActiveNormalModeWindow()
        {
            return normalModeWindow.activeSelf;
        }
        public void AddListenerNormalModeWindowMask(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call =>
            {
                action();
            });
            normalModeWindowMaskEventTrigger.triggers.Add(entry);
        }
        public void SetInteractableNormalModeGoButton(bool isActive)
        {
            normalModeGoButton.interactable = isActive;
        }
        public void AddListenerNormalModeGoButton(UnityAction action)
        {
            normalModeGoButton.onClick.AddListener(action);
        }
        public void AddListenerNormalModeEasyToggle(UnityAction<bool> action)
        {
            normalModeEasyToggle.onValueChanged.AddListener(action);
        }
        public void AddListenerNormalModeNormalToggle(UnityAction<bool> action)
        {
            normalModeNormalToggle.onValueChanged.AddListener(action);
        }
        public void AddListenerNormalModeHardToggle(UnityAction<bool> action)
        {
            normalModeHardToggle.onValueChanged.AddListener(action);
        }
        public void MovePositionNormalModeSelectStageFrame(Vector3 position)
        {
            normalModeSelectStageFrameRectTransform.DOMove(position, 0.2f);
        }
        
        /// <summary>
        /// 選択モード切り替え処理
        /// </summary>
        /// <param name="selectLevel">選択したレベル</param>
        /// <param name="stageDetailsInfo">ステージ情報</param>
        /// <param name="highScore">ハイスコア</param>>
        public void ChangeNormalModeSelectLevel(int selectLevel, JsonSchema.Response.StageDetailsInfo stageDetailsInfo, int highScore)
        {
            Vector3 position = Vector3.zero;
            
            // Toggleを切り替える
            switch (selectLevel)
            {
                case GameConst.GameNormalModeLevelEasy:
                    normalModeEasyToggle.isOn = true;
                    if (normalModeEasyToggle.IsActive())
                        position = normalModeEasyToggle.transform.position;
                    break;
                case GameConst.GameNormalModeLevelNormal:
                    normalModeNormalToggle.isOn = true;
                    if (normalModeNormalToggle.IsActive())
                        position = normalModeNormalToggle.transform.position;
                    break;
                case GameConst.GameNormalModeLevelHard:
                    normalModeHardToggle.isOn = true;
                    if (normalModeHardToggle.IsActive())
                        position = normalModeHardToggle.transform.position;
                    break;
            }
            
            // 選択フレームを移動
            if (position != Vector3.zero)
            {
                MovePositionNormalModeSelectStageFrame(position);
            }
            
            // ステージ情報を更新
            normalModeStageDetailText.text = stageDetailsInfo.detail;
            normalModeEnemyCountText.text = stageDetailsInfo.enemyCount.ToString();
            normalModeHighScoreText.text = highScore > 0 ? highScore.ToString() : "-----";
        }
    }
}
