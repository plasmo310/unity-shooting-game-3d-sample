
namespace Scenes.Common
{
    /// <summary>
    /// Game共通状態クラス
    /// </summary>
    public static class ShootingGameState
    {
        /// <summary>
        /// モバイルプラットフォームか？
        /// </summary>
        public static bool IsMobilePlatform = false;
        
        /// <summary>
        /// ゲーム準備完了か？
        /// ログイン処理完了後にtrueに設定される
        /// </summary>
        public static bool IsGameReady = false;
    }
}
