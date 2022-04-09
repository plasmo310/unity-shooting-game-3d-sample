using System;

namespace Services
{
    /// <summary>
    /// ランキングDummyService
    /// </summary>
    public class RankingDummyService : IRankingService
    {
        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="callback">ログイン後の処理</param>
        public void Login(Action<bool> callback)
        {
            callback(true);
        }

        /// <summary>
        /// プレイヤー名設定
        /// </summary>
        /// <param name="name">プレイヤー名</param>
        /// <param name="callback">設定後の処理</param>
        public void SetPlayerName(string name, Action<bool> callback)
        {
            callback(true);
        }
        
        /// <summary>
        /// スコア送信
        /// </summary>
        /// <param name="displayName">表示名</param>
        /// <param name="score">スコア</param>
        /// <param name="count">倒した数</param>
        /// <param name="callback">送信後の処理</param>
        public void SendScore(string displayName, int score, int count, Action<bool> callback)
        {
            callback(true);
        }

        /// <summary>
        /// ランキング情報取得
        /// </summary>
        /// <param name="leaderBoardName">LeaderBoard名</param>
        /// <param name="callback">取得後の処理</param>
        public void GetLeaderBoardInfo(string leaderBoardName, Action<JsonSchema.Response.LeaderBoardInfo> callback)
        {
            // ダミーデータを返却
            var leaderBoardInfo = new JsonSchema.Response.LeaderBoardInfo();
            leaderBoardInfo.entries = new JsonSchema.Response.LeaderBoardEntry[3];
            leaderBoardInfo.entries[0] = new JsonSchema.Response.LeaderBoardEntry
            {
                position = 0,
                displayName = "test1",
                statValue = 100
            };
            leaderBoardInfo.entries[1] = new JsonSchema.Response.LeaderBoardEntry
            {
                position = 1,
                displayName = "test2",
                statValue = 50
            };
            leaderBoardInfo.entries[2] = new JsonSchema.Response.LeaderBoardEntry
            {
                position = 2,
                displayName = "test3",
                statValue = 30
            };
            callback(leaderBoardInfo);
        }
    }
}
