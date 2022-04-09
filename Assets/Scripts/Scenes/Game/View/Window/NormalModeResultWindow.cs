using DG.Tweening;
using Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Scenes.Game.View.Window
{
    /// <summary>
    /// ノーマルモード 結果ウィンドウ
    /// </summary>
    public class NormalModeResultWindow : MonoBehaviour
    {
        [SerializeField] private GameObject resultWindow;
        [SerializeField] private Button resultNextButton;
        [SerializeField] private Text resultPerfectCountText;
        [SerializeField] private Text resultPerfectScoreText;
        [SerializeField] private Text resultGreatCountText;
        [SerializeField] private Text resultGreatScoreText;
        [SerializeField] private Text resultGoodCountText;
        [SerializeField] private Text resultGoodScoreText;
        [SerializeField] private Text resultTimeText;
        [SerializeField] private Text resultTimeScaleText;
        [SerializeField] private Text resultTotalScoreText;
        [SerializeField] private Image resultTotalScoreThunderImage01;
        [SerializeField] private Image resultTotalScoreThunderImage02;
        [SerializeField] private Image resultKaniImage;
        [SerializeField] private Image resultKaniSpeechImage;
        [SerializeField] private Text resultKaniSpeechText;
        public void SetActiveResultNextButton(bool isActive)
        {
            resultNextButton.gameObject.SetActive(isActive);
        }
        public void AddListenerResultNextButton(UnityAction action)
        {
            resultNextButton.onClick.AddListener(action);
        }
        private void InitializeScoreInfo()
        {
            var initZero = 0.ToString();
            resultPerfectCountText.text = initZero;
            resultPerfectScoreText.text = initZero;
            resultGreatCountText.text = initZero;
            resultGreatScoreText.text = initZero;
            resultGoodCountText.text = initZero;
            resultGoodScoreText.text = initZero;
            resultTotalScoreText.text = initZero;
            resultTimeText.text = DateUtil.ConvTimeToMmSs(0);
            resultTimeScaleText.text = 0.ToString("f2");
            resultTotalScoreThunderImage01.gameObject.SetActive(false);
            resultTotalScoreThunderImage02.gameObject.SetActive(false);
            resultKaniImage.gameObject.SetActive(false);
            resultKaniSpeechImage.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 結果ウィンドウ活性切替
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActiveResultWindow(bool isActive)
        {
            resultWindow.SetActive(isActive);
        }
        
        /// <summary>
        /// 結果ウィンドウ表示処理
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
        public void ShowResultWindow(
            int perfectCount, float perfectScore,
            int greatCount, float greatScore,
            int goodCount, float goodScore,
            float time, float timeScale,
            float totalScore, bool isHighScore,
            UnityAction callback)
        {
            // リザルト画面表示
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                InitializeScoreInfo();
                SetActiveResultWindow(true);
                SetActiveResultNextButton(false);
            });
            
            // 敵の倒した数を表示
            var duration = 0.5f;
            sequence.Append(DOTweenUtil.IncrementText(resultPerfectCountText, 0, perfectCount, duration));
            sequence.Join(DOTweenUtil.IncrementText(resultPerfectScoreText, 0, perfectScore, duration));
            sequence.Join(DOTweenUtil.IncrementText(resultGreatCountText, 0, greatCount, duration));
            sequence.Join(DOTweenUtil.IncrementText(resultGreatScoreText, 0, greatScore, duration));
            sequence.Join(DOTweenUtil.IncrementText(resultGoodCountText, 0, goodCount, duration));
            sequence.Join(DOTweenUtil.IncrementText(resultGoodScoreText, 0, goodScore, duration));
            sequence.Join(DOTweenUtil.IncrementText(null, 0, 1.0f, duration, (val) =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeCount);
            }));
            
            // タイムを表示
            sequence.AppendInterval(0.5f);
            sequence.Append(DOTweenUtil.IncrementText(resultTimeText, 0.0f, time, duration, (val) =>
            {
                resultTimeText.text = DateUtil.ConvTimeToMmSs(val);
            }));
            sequence.Join(
                DOTweenUtil.IncrementText(resultTimeScaleText, 0.0f, timeScale, duration, (val) =>
                {
                    resultTimeScaleText.text = "x" + val.ToString("f2");
                }));
            sequence.Join(DOTweenUtil.IncrementText(null, 0, 1.0f, duration, (val) =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeCount);
            }));
            
            // トータルスコアを表示
            var totalDuration = 0.5f;
            var totalEase = Ease.InExpo;
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => resultTotalScoreText.text = Mathf.RoundToInt(totalScore).ToString());
            sequence.Join(DOTweenUtil.DoLocalMoveTransform(resultTotalScoreText.rectTransform, new Vector3(-200.0f, 200.0f, 1.0f), resultTotalScoreText.rectTransform.localPosition, totalDuration, totalEase));
            sequence.Join(DOTweenUtil.DoScaleTransform(resultTotalScoreText.rectTransform, 10.0f * resultTotalScoreText.rectTransform.localScale, resultTotalScoreText.rectTransform.localScale, totalDuration, totalEase));
            sequence.AppendCallback(() => ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeBomb));

            // トータルスコアの稲妻を表示
            sequence.AppendCallback(() =>
            {
                resultTotalScoreThunderImage01.gameObject.SetActive(true);
                resultTotalScoreThunderImage02.gameObject.SetActive(true);
            });
            sequence.Join(DOTweenUtil.FeedImageAlpha(resultTotalScoreThunderImage01, 0.0f, 1.0f, 0.5f));
            sequence.Join(DOTweenUtil.FeedImageAlpha(resultTotalScoreThunderImage02, 0.0f, 1.0f, 0.5f));
            
            // カニを表示
            sequence.AppendInterval(0.3f);
            sequence.AppendCallback(() =>
            {
                resultKaniImage.gameObject.SetActive(true);
                resultKaniSpeechImage.gameObject.SetActive(true);
                resultKaniSpeechText.text = isHighScore ? "ハイスコアぞ" : "オソレイッタ";
            });
            sequence.Join(DOTweenUtil.FeedImageAlpha(resultKaniImage, 0.0f, 1.0f, 0.5f));
            sequence.Join(DOTweenUtil.FeedImageAlpha(resultKaniSpeechImage, 0.0f, 1.0f, 0.5f));

            // コールバック実行
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() =>
            {
                callback();
            });
        }
    }
}
