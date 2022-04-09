using System.Collections;
using DG.Tweening;
using Scenes.Game.View.Window;
using Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;
using Utils.Components;

namespace Scenes.Game.View
{
    /// <summary>
    /// GameInfoCanvas
    /// </summary>
    public class GameInfoCanvas : MonoBehaviour
    {
        // ----- 各ウィンドウ -----
        public NormalModeResultWindow NormalModeResultWindow { get; private set; }
        public EndlessModeResultWindow EndlessModeResultWindow { get; private set; }
        public EndlessModeRankingWindow EndlessModeRankingWindow { get; private set; }

        private void Awake()
        {
            // カニゲージのバーの長さを保持
            _kaniBarLength = kaniBarBackTransform.sizeDelta.x;
            
            // Prefabから各UIを生成する
            NormalModeResultWindow = CreateCanvasUI<NormalModeResultWindow>(GameConst.UIGameInfoNormalModeResultWindow);
            EndlessModeResultWindow = CreateCanvasUI<EndlessModeResultWindow>(GameConst.UIGameInfoEndlessModeResultWindow);
            EndlessModeRankingWindow = CreateCanvasUI<EndlessModeRankingWindow>(GameConst.UIGameInfoEndlessModeRankingWindow);
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
        
        // ----- メッセージ関連 -----
        [SerializeField] private Image gameClearMsgImage;
        [SerializeField] private Image gameOverMsgImage;
        [SerializeField] private Image gameFinishMsgImage;
        [SerializeField] private Image levelUpMsgImage;

        public void SetActiveGameClearMessage(bool isActive)
        {
            gameClearMsgImage.gameObject.SetActive(isActive);
        }
        
        public void SetActiveGameOverMessage(bool isActive)
        {
            gameOverMsgImage.gameObject.SetActive(isActive);
        }

        public void SetActiveFinishMsgImage(bool isActive)
        {
            gameFinishMsgImage.gameObject.SetActive(isActive);
        }

        public void SetActiveLevelUpMsgImage(bool isActive)
        {
            levelUpMsgImage.gameObject.SetActive(isActive);
        }

        public void ShowLevelUpMsgImage()
        {
            StartCoroutine(StartLevelUpMsgAnimation());
        }

        private IEnumerator StartLevelUpMsgAnimation()
        {
            // 点滅させる
            levelUpMsgImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            levelUpMsgImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            levelUpMsgImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            levelUpMsgImage.gameObject.SetActive(false);
        }
        
        // ----- カニゲージ(ノーマルモード)関連 -----
        [SerializeField] private GameObject kaniGauge;
        [SerializeField] private RectTransform kaniBarBackTransform;
        [SerializeField] private RectTransform kaniBarFrontTransform;
        [SerializeField] private ShakeEffect kaniGaugeShakeEffect;
        private float _kaniBarLength;

        public void SetActiveKaniGauge(bool isActive)
        {
            kaniGauge.SetActive(isActive);
        }

        /// <summary>
        /// 残りカニ割合からカニバーを設定
        /// </summary>
        /// <param name="ratio">残りカニの割合(0.0~1.0)</param>
        public void SetRemainRatioKaniBar(float ratio)
        {
            var sizeDelta = kaniBarFrontTransform.sizeDelta;
            sizeDelta.x = _kaniBarLength * ratio;
            kaniBarFrontTransform.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// カニゲージを揺らす
        /// </summary>
        public void ShakeKaniGauge()
        {
            kaniGaugeShakeEffect.StartShake(0.5f, 30.0f, 50.0f);
        }

        // ----- カニブレイクエリア(エンドレスモード)関連 -----
        [SerializeField] private GameObject kaniBreakArea;
        [SerializeField] private Text kaniBreakCountText;
        [SerializeField] private ShakeEffect kaniBreakAreaShakeEffect;
        public void SetActiveKaniBreakArea(bool isActive)
        {
            kaniBreakArea.SetActive(isActive);
        }
        public void SetKaniBreakCountText(int count)
        {
            DOTweenUtil.IncrementText(kaniBreakCountText, int.Parse(kaniBreakCountText.text), count, 0.2f);
        }
        public void ShakeEffectKaniBreakArea()
        {
            kaniBreakAreaShakeEffect.StartShake(0.5f, 30.0f, 50.0f);
        }

        // ----- スコア関連 -----
        [SerializeField] private GameObject gameScore;
        [SerializeField] private Text gameScoreText;
        public void SetActiveScore(bool isActive)
        {
            gameScore.SetActive(isActive);
        }
        public void SetScoreText(float score)
        {
            DOTweenUtil.IncrementText(gameScoreText, int.Parse(gameScoreText.text), score, 0.5f);
        }

        // ----- リザルト関連 -----
        /// <summary>
        /// ノーマルモード結果表示処理
        /// </summary>
        /// <param name="perfectCount">Perfect数</param>
        /// <param name="perfectScore">Perfect累計スコア</param>
        /// <param name="greatCount">Great数</param>
        /// <param name="greatScore">Great累計スコア</param>
        /// <param name="goodCount">Good数</param>
        /// <param name="goodScore">Good累計スコア</param>
        /// <param name="time">クリア時間</param>
        /// <param name="timeScale">TimeScale</param>
        /// <param name="totalScore">累計スコア</param>
        /// <param name="isHighScore">ハイスコアか？</param>
        /// <param name="callback">コールバック処理</param>
        public void ShowNormalModeResultInfo(
            int perfectCount, float perfectScore,
            int greatCount, float greatScore,
            int goodCount, float goodScore,
            float time, float timeScale,
            float totalScore, bool isHighScore,
            UnityAction callback)
        {
            // ゲームクリアメッセージ表示
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() => SetActiveGameClearMessage(true));
            sequence.AppendInterval(1.0f);
            sequence.AppendCallback(() => SetActiveGameClearMessage(false));

            // リザルトウィンドウ表示
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() =>
            {
                NormalModeResultWindow.ShowResultWindow(
                    perfectCount, perfectScore,
                    greatCount, greatScore,
                    goodCount, goodScore,
                    time, timeScale,
                    totalScore, isHighScore,
                    callback);
            });
        }

        /// <summary>
        /// エンドレスモード結果表示処理
        /// </summary>
        /// <param name="perfectCount">Perfect数</param>
        /// <param name="perfectScore">Perfect累計スコア</param>
        /// <param name="greatCount">Great数</param>
        /// <param name="greatScore">Great累計スコア</param>
        /// <param name="goodCount">Good数</param>
        /// <param name="goodScore">Good累計スコア</param>
        /// <param name="totalScore">累計スコア</param>
        /// <param name="isHighScore">ハイスコアか？</param>
        /// <param name="afterFinishCallback">終了メッセージ後のコールバック処理</param>
        /// <param name="callback">コールバック処理</param>
        public void ShowEndlessModeResultInfo(
            int perfectCount, float perfectScore,
            int greatCount, float greatScore,
            int goodCount, float goodScore,
            float totalScore, bool isHighScore,
            UnityAction afterFinishCallback,
            UnityAction callback)
        {
            // フィニッシュメッセージ表示
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() => SetActiveFinishMsgImage(true));
            sequence.AppendInterval(1.0f);
            sequence.AppendCallback(() => SetActiveFinishMsgImage(false));

            // リザルトウィンドウ表示
            sequence.AppendInterval(0.8f);
            sequence.AppendCallback(() => afterFinishCallback());
            sequence.AppendCallback(() =>
            {
                EndlessModeResultWindow.ShowResultWindow(
                    perfectCount, perfectScore,
                    greatCount, greatScore,
                    goodCount, goodScore,
                    totalScore, isHighScore,
                    callback);
            });
        }
    }
}
