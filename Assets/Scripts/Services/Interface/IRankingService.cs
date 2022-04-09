using System;

namespace Services
{
    /// <summary>
    /// ランキング用Service
    /// </summary>
    public interface IRankingService
    {
        public void Login(Action<bool> callback);
        public void SetPlayerName(string name, Action<bool> callback);
        public void SendScore(string displayName, int score, int count, Action<bool> callback);
        public void GetLeaderBoardInfo(string leaderBoardName, Action<JsonSchema.Response.LeaderBoardInfo> callback);
    }
}
