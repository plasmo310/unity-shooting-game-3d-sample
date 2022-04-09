using Scenes.Common.View;
using Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Scenes.Game.View.Window
{
    /// <summary>
    /// エンドレスモード ランキングウィンドウ
    /// </summary>
    public class EndlessModeRankingWindow : MonoBehaviour
    {
        [SerializeField] private GameObject rankingWindow;
        [SerializeField] private Text resultCountText;
        [SerializeField] private Text resultScoreText;
        [SerializeField] private InputField registerNameText;
        [SerializeField] private Text registerResultText;
        [SerializeField] private GameObject rankingScrollContent;
        [SerializeField] private Toggle rankingCountToggle;
        [SerializeField] private Toggle rankingScoreToggle;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button nextButton;
        public void SetActiveRankingWindow(bool isActive)
        {
            rankingWindow.SetActive(isActive);
        }
        public void SetTextResultCount(string text)
        {
            resultCountText.text = text;
        }
        public void SetTextResultScore(string text)
        {
            resultScoreText.text = text;
        }
        public string GetTextRegisterName()
        {
            return registerNameText.text;
        }
        public void SetTextRegisterName(string text)
        {
            Debug.Log(text);
            registerNameText.text = text;
        }
        public void SetTextRegisterResult(string text, bool isError = false)
        {
            registerResultText.text = text;
            registerResultText.color = isError ? Color.red : Color.cyan;
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
        public void SetInteractableRegisterButton(bool isActive)
        {
            registerButton.interactable = isActive;
        }
        public void AddListenerRegisterButton(UnityAction action)
        {
            registerButton.onClick.AddListener(action);
        }
        public void SetInteractableNextButton(bool isActive)
        {
            nextButton.interactable = isActive;
        }
        public void AddListenerNextButton(UnityAction action)
        {
            nextButton.onClick.AddListener(action);
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
        
        /// <summary>
        /// ランキング表示処理
        /// </summary>
        /// <param name="diaplayName">表示名</param>
        /// <param name="score">スコア</param>
        /// <param name="count">倒した数</param>
        /// <param name="registerAction">登録ボタン押下時Action</param>
        public void ShowRankingWindow(string diaplayName, string score, string count,
            UnityAction registerAction)
        {
            SetTextResultScore(score);
            SetTextResultCount(count);
            SetTextRegisterName(diaplayName);
            SetTextRegisterResult("", false); // 空にする
            AddListenerRegisterButton(registerAction);
            // ランキングウィンドウ表示
            SetActiveRankingWindow(true);
        }
    }
}
