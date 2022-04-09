using System;

namespace JsonSchema.Response
{
    /// <summary>
    /// ステージ詳細情報マスタ
    /// </summary>
    [Serializable]
    public class StageDetail
    {
        public StageDetailsInfo[] stage_details;
    }
    
    /// <summary>
    /// ステージ詳細情報
    /// </summary>
    [Serializable]
    public class StageDetailsInfo
    {
        public int mode;
        public int level;
        public string detail;  // ステージ詳細
        public int enemyCount; // 敵の数
    }

    /// <summary>
    /// 敵生成情報マスタ
    /// </summary>
    [Serializable]
    public class EnemyGenerate
    {
        public EnemyGenerateInfo[] generate_infos;
    }
    
    /// <summary>
    /// 敵生成情報
    /// </summary>
    [Serializable]
    public class EnemyGenerateInfo
    {
        public int mode;  // モード
        public int level; // レベル
        
        public int generateCount; // 敵の生成数
        
        public float minAppearDegree; // 前回の位置からの出現角度(Min)
        public float maxAppearDegree; // 前回の位置からの出現角度(Max)
        
        public float minScale; // 大きさ(Min)
        public float maxScale; // 大きさ(Max)
        
        public float minSpeed; // 速度(Min)
        public float maxSpeed; // 速度(Max)
        
        public float addSpeed; // 一匹ごとに加える速度

        public float minShakeWidthX; // 揺れ幅X(Min)
        public float maxShakeWidthX; // 揺れ幅X(Max)
        public float minShakeWidthY; // 揺れ幅Y(Min)
        public float maxShakeWidthY; // 揺れ幅Y(Max)
        
        public int shakeWidthYCount; // 指定数ごとに縦揺れさせる

        public float minWaitTime; // 待機時間(Min)
        public float maxWaitTime; // 待機時間(Max)
        
        public int appearEachCount; // 指定数ごとに出現させる
    }

    /// <summary>
    /// 敵のスコア情報マスタ
    /// </summary>
    [Serializable]
    public class EnemyScore
    {
        public float goodScore;    // Good時のスコア
        public float greatScore;   // Great時のスコア
        public float perfectScore; // Perfect時のスコア
    }
    
    /// <summary>
    /// スコア時間ボーナス情報マスタ
    /// </summary>
    [Serializable]
    public class ScoreTimeBonus
    {
        public ScoreTimeBonusInfo[] time_bonus_infos;
    }
    
    /// <summary>
    /// スコア時間ボーナス情報
    /// </summary>
    [Serializable]
    public class ScoreTimeBonusInfo
    {
        public int mode;
        public int level;
        public float standardTime;  // 通常(x1.0)となる時間
        public float bonusTime;     // ボーナス値を付与する時間
        public float bonusAddScale; // BonusTime時に加算するScale値
    }
    
    /// <summary>
    /// ランダム名称マスタ
    /// </summary>
    [Serializable]
    public class RandomNameInfo
    {
        public RandomName[] names;
    }
    
    /// <summary>
    /// ランダム名称
    /// </summary>
    [Serializable]
    public class RandomName
    {
        public string name;
    }

    /// <summary>
    /// LeaderBoard情報
    /// </summary>
    [Serializable]
    public class LeaderBoardInfo
    {
        public LeaderBoardEntry[] entries;
    }
    
    /// <summary>
    /// LeaderBoardアイテム
    /// </summary>
    [Serializable]
    public class LeaderBoardEntry
    {
        public int position;       // 順位
        public string displayName; // 表示名
        public int statValue;      // 値
    }
}
