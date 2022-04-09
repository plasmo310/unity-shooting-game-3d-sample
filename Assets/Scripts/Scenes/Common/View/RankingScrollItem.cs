using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Common.View
{
    public class RankingScrollItem : MonoBehaviour
    {
        [SerializeField] private Text positionText;
        [SerializeField] private Text displayNameText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Image backGroundImage;

        /// <summary>
        /// ランキング情報の設定
        /// </summary>
        /// <param name="position"></param>
        /// <param name="displayName"></param>
        /// <param name="score"></param>
        /// <param name="isError"></param>
        public void SetRankingInfo(string position, string displayName, string score, bool isError = false)

        {
            positionText.text = position;
            displayNameText.text = displayName;
            scoreText.text = score;
            
            // 親オブジェクトに引きずられてしまわないようscaleを調整
            transform.localScale = Vector3.one;
            
            // 背景色を変える
            if (int.TryParse(position, out var positionNumber))
            {
                if (positionNumber % 2 != 0)
                {
                    var color = backGroundImage.color;
                    color.a = 15.0f / 255.0f;
                    backGroundImage.color = color;
                }
            }
            
            // エラーメッセージ
            if (isError)
            {
                positionText.color = Color.red;
                displayNameText.color = Color.red;
                scoreText.color = Color.red;
            }
        }
    }
}
