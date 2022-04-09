using UnityEngine;
using Utils;

namespace Scenes.Game.Model
{
    /// <summary>
    /// GameInfoModel
    /// </summary>
    public class GameInfoModel
    {
        /// <summary>
        /// 最大スコア
        /// </summary>
        private static readonly float MaxScore = 999999.0f;
        
        /// <summary>
        /// 最大ブレイク数
        /// </summary>
        private static readonly int MaxBreakCount = 999;
        
        /// <summary>
        /// 敵生成情報
        /// </summary>
        private int _generatedEnemyCount; // 生成した敵の数
        private int _remainEnemyCount;    // 残りの敵の数
        
        /// <summary>
        /// スコア計算情報
        /// </summary>
        public struct ScoreInfo
        {
            public int PerfectCount;        // Perfect数
            public float PerfectTotalScore; // Perfect累計スコア
            public int GreatCount;          // Great数
            public float GreatTotalScore;   // Great累計スコア
            public int GoodCount;           // Good数
            public float GoodTotalScore;    // Good累計スコア
            public float TotalTime;         // 累計時間
            public float TimeScale;         // 時間による倍率
            public int GameTotalCount;    // ゲーム中の累計数
            public float GameTotalScore;    // ゲーム中の累計スコア
            public int ResultTotalCount;  // 最終的な累計数
            public float ResultTotalScore;  // 最終的な累計スコア
        }
        private ScoreInfo _scoreInfo;

        public GameInfoModel()
        {
            _scoreInfo = new ScoreInfo();
        }

        /// <summary>
        /// 生成する敵の数を設定
        /// </summary>
        /// <param name="generateCount">生成する敵の数</param>
        public void SetGenerateEnemyCount(int generateCount)
        {
            _generatedEnemyCount = generateCount;
            // 残りの敵の数にも設定
            _remainEnemyCount = _generatedEnemyCount;
        }

        /// <summary>
        /// 残りの敵の数を減らす
        /// </summary>
        public void DecrementEnemyCount()
        {
            // 0より小さくはしない
            _remainEnemyCount--;
            if (_remainEnemyCount < 0) _remainEnemyCount = 0;
        }
        
        /// <summary>
        /// 敵が全て破棄されたか？
        /// </summary>
        public bool IsAllEnemyDestroy()
        {
            return _remainEnemyCount == 0;
        }

        /// <summary>
        /// 残りの敵の割合を取得
        /// </summary>
        public float GetRemainEnemyRatio()
        {
            // 残りの敵の数 / 生成した敵の数
            return (float) _remainEnemyCount / _generatedEnemyCount;
        }

        /// <summary>
        /// 倒した敵の数を取得
        /// </summary>
        public int GetTotalBreakEnemyCount()
        {
            return _scoreInfo.GameTotalCount;
        }

        /// <summary>
        /// ヒットオブジェクトのタイプからスコア情報を更新する
        /// </summary>
        /// <param name="hitType">ヒットタイプ</param>
        /// <param name="enemyScore">敵のスコア情報</param>
        /// <returns>スコア</returns>
        public void UpdateScoreInfoByHitType(int hitType, JsonSchema.Response.EnemyScore enemyScore)
        {
            // タグ名に応じてスコア情報を更新する
            var score = 0.0f;
            switch (hitType)
            {
                case GameConst.EnemyHitTypePerfect:
                case GameConst.EnemyHitTypeBeam:
                    score = enemyScore.perfectScore;
                    _scoreInfo.PerfectCount++;
                    _scoreInfo.PerfectTotalScore += score;
                    break;
                case GameConst.EnemyHitTypeGreat:
                    score = enemyScore.greatScore;
                    _scoreInfo.GreatCount++;
                    _scoreInfo.GreatTotalScore += score;
                    break;
                case GameConst.EnemyHitTypeGood:
                    score = enemyScore.goodScore;
                    _scoreInfo.GoodCount++;
                    _scoreInfo.GoodTotalScore += score;
                    break;
                default:
                    Debug.LogError("not hit type: " + hitType);
                    break;
            }
            // 累計スコア、倒した敵の数も加算
            _scoreInfo.GameTotalScore += score;
            _scoreInfo.GameTotalCount++;

            // 最大値を超えないよう調整
            _scoreInfo.PerfectCount = Mathf.Min(_scoreInfo.PerfectCount, MaxBreakCount);
            _scoreInfo.PerfectTotalScore = Mathf.Min(_scoreInfo.PerfectTotalScore, MaxScore);
            _scoreInfo.GreatCount = Mathf.Min(_scoreInfo.GreatCount, MaxBreakCount);
            _scoreInfo.GreatTotalScore = Mathf.Min(_scoreInfo.GreatTotalScore, MaxScore);
            _scoreInfo.GoodCount = Mathf.Min(_scoreInfo.GoodCount, MaxBreakCount);
            _scoreInfo.GoodTotalScore = Mathf.Min(_scoreInfo.GoodTotalScore, MaxScore);
            _scoreInfo.GameTotalCount = Mathf.Min(_scoreInfo.GameTotalCount, MaxBreakCount);
            _scoreInfo.GameTotalScore = Mathf.Min(_scoreInfo.GameTotalScore, MaxScore);
        }
        
        /// <summary>
        /// ゲーム中の累計スコアを返却
        /// </summary>
        /// <returns></returns>
        public float GetGameTotalScore()
        {
            return _scoreInfo.GameTotalScore;
        }
        
        /// <summary>
        /// 累計時間更新
        /// </summary>
        /// <param name="deltaTime">デルタタイム</param>
        public void UpdateTotalTime(float deltaTime)
        {
            _scoreInfo.TotalTime += deltaTime;
        }
        
        /// <summary>
        /// 結果画面のスコア情報取得
        /// </summary>
        public ScoreInfo GetResultScoreInfo(JsonSchema.Response.ScoreTimeBonusInfo timeBonus = null)
        {
            // TimeScaleを計算してスコアを計算(ノーマルモードのみ)
            _scoreInfo.TimeScale = timeBonus == null ? 1.0f : CalculateTimeScale(timeBonus.standardTime, timeBonus.bonusTime, timeBonus.bonusAddScale, _scoreInfo.TotalTime);
            _scoreInfo.ResultTotalScore = Mathf.Min(_scoreInfo.GameTotalScore * _scoreInfo.TimeScale, MaxScore);
            _scoreInfo.ResultTotalCount = Mathf.Min(_scoreInfo.GameTotalCount, MaxBreakCount);
            return _scoreInfo;
        }
        
        /// <summary>
        /// TimeScale計算処理
        /// </summary>
        /// <param name="standardTime">この秒数より低ければ x1.0</param>
        /// <param name="bonusTime">この秒数で x1.0+BonusScale</param>
        /// <param name="bonusAddScale">BonusTime時に加算するScale値</param>
        /// <param name="time">時間</param>
        /// <returns>時間に応じたscale値</returns>
        public float CalculateTimeScale(float standardTime, float bonusTime, float bonusAddScale, float time)
        {
            // StandardTimeよりかかっていれば x1.0
            if (time >= standardTime) return 1.0f;
            
            // BonusTimeよりかかっていればBonusScaleをMAXに計算する
            if (time >= bonusTime)
            {
                time = Mathf.Floor(time); // 小数点は切り捨てる
                var bonusDuration = standardTime - bonusTime;
                var timeDuration = time - bonusTime;
                var percent = 1.0f - timeDuration / bonusDuration;
                return 1.0f + bonusAddScale * percent;
            }
            
            // BonusTimeより早いほど加算する
            var diffTime = bonusTime - time;
            var addScale = (diffTime / 0.1f) * 0.02f; // 0.1秒超えるごとに+0.02
            return 1.0f + bonusAddScale + addScale;
        }
    }
}
