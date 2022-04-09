using Scenes.Common.View;
using Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Scenes.Title.View.Window
{
    /// <summary>
    /// エンドレスモードウィンドウ
    /// </summary>
    public class EndlessModeWindow : MonoBehaviour
    {
        [SerializeField] private GameObject endlessModeWindow;
        [SerializeField] private EventTrigger endlessModeWindowMaskEventTrigger;
        [SerializeField] private Button endlessModeGoButton;
        [SerializeField] private Text endlessModeCountText;
        [SerializeField] private Text endlessModeHighScoreText;
        [SerializeField] private GameObject rankingScrollContent;
        [SerializeField] private Toggle rankingCountToggle;
        [SerializeField] private Toggle rankingScoreToggle;
        public void SetActiveEndlessModeWindow(bool isActive)
        {
            endlessModeWindow.SetActive(isActive);
        }
        public bool IsActiveEndlessModeWindow()
        {
            return endlessModeWindow.activeSelf;
        }
        public void AddListenerEndlessModeWindowMask(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call =>
            {
                action();
            });
            endlessModeWindowMaskEventTrigger.triggers.Add(entry);
        }
        public void SetInteractableEndlessModeGoButton(bool isActive)
        {
            endlessModeGoButton.interactable = isActive;
        }
        public void AddListenerEndlessModeGoButton(UnityAction action)
        {
            endlessModeGoButton.onClick.AddListener(action);
        }
        public bool IsOnRankingCountToggle()
        {
            return rankingCountToggle.isOn;
        }
        public void AddListenerRankingCountToggle(UnityAction<bool> action)
        {
            rankingCountToggle.onValueChanged.AddListener(action);
        }
        public bool IsOnRankingScoreToggle()
        {
            return rankingScoreToggle.isOn;
        }
        public void AddListenerRankingScoreToggle(UnityAction<bool> action)
        {
            rankingScoreToggle.onValueChanged.AddListener(action);
        }
        /// <summary>
        /// エンドレスモード情報設定
        /// </summary>
        /// <param name="count">倒した数</param>
        /// <param name="highScore">ハイスコア</param>>
        public void SetEndlessModeInfo(int count, int highScore)
        {
            endlessModeCountText.text = count.ToString();
            endlessModeHighScoreText.text = highScore > 0 ? highScore.ToString() : "-----";
        }
        
        /// <summary>
        /// スクロールアイテム全削除
        /// </summary>
        public void ClearAllRankingItem()
        {
            // 子オブジェクトを全て削除する
            foreach (Transform child in rankingScrollContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        /// <summary>
        /// ランキングアイテム追加
        /// </summary>
        /// <param name="position"></param>
        /// <param name="displayName"></param>
        /// <param name="score"></param>
        /// <param name="isError"></param>
        public void AddRankingItem(string position, string displayName, string score, bool isError = false)
        {
            // ScrollContentの子オブジェクトとして作成
            var itemPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.UIRankingScrollItem);
            var itemObj = Instantiate(itemPrefab, rankingScrollContent.transform, true);
            // ランキング情報を設定する
            var scrollItem = itemObj.GetComponent<RankingScrollItem>();
            scrollItem.SetRankingInfo(position, displayName, score, isError);
        }
    }
}
