using Services;
using Utils;

namespace Scenes.Common
{
    /// <summary>
    /// ゲーム共通PlayerPrefs管理クラス
    /// </summary>
    public static class ShootingPlayerPrefs
    {
        /// <summary>
        /// プレイヤーID
        /// ※PlayFabで使用していたが現在は使用していない
        /// </summary>
        public static string PlayerId
        {
            get => GetPlayerPrefsStringValue(KeyPlayerId);
            set => SetPlayerPrefsStringValue(KeyPlayerId, value);
        }
        private const string KeyPlayerId = "PlayerId";
        
        /// <summary>
        /// プレイヤー名
        /// </summary>
        public static string PlayerName
        {
            get => GetPlayerPrefsStringValue(KeyPlayerName);
            set => SetPlayerPrefsStringValue(KeyPlayerName, value);
        }
        private const string KeyPlayerName = "PlayerName";
        
        /// <summary>
        /// ランキング送信に同意したか？
        /// </summary>
        public static bool IsConsentRanking
        {
            get => GetPlayerPrefsBoolValue(KeyIsConsentRanking);
            set => SetPlayerPrefsBoolValue(KeyIsConsentRanking, value);
        }
        private const string KeyIsConsentRanking = "IsConsentRanking";
        
        /// <summary>
        /// ヘルプを閲覧済か？
        /// </summary>
        public static bool IsShowHelp
        {
            get => GetPlayerPrefsBoolValue(KeyIsShowHelp);
            set => SetPlayerPrefsBoolValue(KeyIsShowHelp, value);
        }
        private const string KeyIsShowHelp = "IsShowHelp";
        
        /// <summary>
        /// BGM音量
        /// </summary>
        public static float BgmVolume
        {
            get => GetPlayerPrefsFloatValue(KeyBgmVolume, 0.8f); // 0.8がデフォルト
            set => SetPlayerPrefsFloatValue(KeyBgmVolume, value);
        }
        private const string KeyBgmVolume = "BgmVolume";
        
        /// <summary>
        /// SE音量
        /// </summary>
        public static float SeVolume
        {
            get => GetPlayerPrefsFloatValue(KeySeVolume, 0.8f); // 0.8がデフォルト
            set => SetPlayerPrefsFloatValue(KeySeVolume, value);
        }
        private const string KeySeVolume = "SeVolume";
        
        /// <summary>
        /// ノーマルモード Easy ハイスコア
        /// </summary>
        private static int NormalModeHighScoreEasy
        {
            get => GetPlayerPrefsIntValue(KeyNormalModeHighScoreEasy);
            set => SetPlayerPrefsIntValue(KeyNormalModeHighScoreEasy, value);
        }
        private const string KeyNormalModeHighScoreEasy = "NormalModeHighScoreEasy";
        
        /// <summary>
        /// ノーマルモード Normal ハイスコア
        /// </summary>
        private static int NormalModeHighScoreNormal
        {
            get => GetPlayerPrefsIntValue(KeyNormalModeHighScoreNormal);
            set => SetPlayerPrefsIntValue(KeyNormalModeHighScoreNormal, value);
        }
        private const string KeyNormalModeHighScoreNormal = "NormalModeHighScoreNormal";
        
        /// <summary>
        /// ノーマルモード Hard ハイスコア
        /// </summary>
        private static int NormalModeHighScoreHard
        {
            get => GetPlayerPrefsIntValue(KeyNormalModeHighScoreHard);
            set => SetPlayerPrefsIntValue(KeyNormalModeHighScoreHard, value);
        }
        private const string KeyNormalModeHighScoreHard = "NormalModeHighScoreHard";

        /// <summary>
        /// エンドレスモード ハイスコア
        /// </summary>
        private static int EndlessModeHighScore
        {
            get => GetPlayerPrefsIntValue(KeyEndlessModeHighScore);
            set => SetPlayerPrefsIntValue(KeyEndlessModeHighScore, value);
        }
        private const string KeyEndlessModeHighScore = "EndlessModeHighScore";

        /// <summary>
        /// エンドレスモード 倒した数(ベスト)
        /// </summary>
        private static int EndlessModeCount
        {
            get => GetPlayerPrefsIntValue(KeyEndlessModeCount);
            set => SetPlayerPrefsIntValue(KeyEndlessModeCount, value);
        }
        private const string KeyEndlessModeCount = "EndlessModeCount";

        /// <summary>
        /// ノーマルモード指定レベルのハイスコアを保存する
        /// </summary>
        /// <param name="gameLevel"></param>
        /// <param name="value"></param>
        public static void SetNormalModeHighScore(int gameLevel, int value)
        {
            switch (gameLevel)
            {
                case GameConst.GameNormalModeLevelEasy:
                    NormalModeHighScoreEasy = value;
                    break;
                case GameConst.GameNormalModeLevelNormal:
                    NormalModeHighScoreNormal = value;
                    break;
                case GameConst.GameNormalModeLevelHard:
                    NormalModeHighScoreHard = value;
                    break;
            }
        }
        
        /// <summary>
        /// ノーマルモード指定レベルのハイスコアを取得する
        /// </summary>
        /// <param name="gameLevel"></param>
        /// <returns></returns>
        public static int GetNormalModeHighScore(int gameLevel)
        {
            var highScore = 0;
            switch (gameLevel)
            {
                case GameConst.GameNormalModeLevelEasy:
                    highScore = NormalModeHighScoreEasy;
                    break;
                case GameConst.GameNormalModeLevelNormal:
                    highScore =  NormalModeHighScoreNormal;
                    break;
                case GameConst.GameNormalModeLevelHard:
                    highScore = NormalModeHighScoreHard;
                    break;
            }
            return highScore;
        }

        /// <summary>
        /// エンドレスモードのハイスコアを保存する
        /// </summary>
        /// <param name="value"></param>
        public static void SetEndlessModeHighScore(int value)
        {
            EndlessModeHighScore = value;
        }

        /// <summary>
        /// エンドレスモードのハイスコアを取得する
        /// </summary>
        /// <returns></returns>
        public static int GetEndlessModeHighScore()
        {
            return EndlessModeHighScore;
        }
        
        /// <summary>
        /// エンドレスモードの倒した数を保存する
        /// </summary>
        /// <param name="value"></param>
        public static void SetEndlessModeCount(int value)
        {
            EndlessModeCount = value;
        }

        /// <summary>
        /// エンドレスモードの倒した数を取得する
        /// </summary>
        /// <returns></returns>
        public static int GetEndlessModeCount()
        {
            return EndlessModeCount;
        }
        
        private static void SetPlayerPrefsIntValue(string key, int value)
        {
            ServiceLocator.Resolve<IPlayerPrefsService>().SetInt(key, value);
        }

        private static int GetPlayerPrefsIntValue(string key, int defaultValue = 0)
        {
            return ServiceLocator.Resolve<IPlayerPrefsService>().GetInt(key, defaultValue);
        }
        
        private static void SetPlayerPrefsFloatValue(string key, float value)
        {
            ServiceLocator.Resolve<IPlayerPrefsService>().SetFloat(key, value);
        }

        private static float GetPlayerPrefsFloatValue(string key, float defaultValue = 0.0f)
        {
            return ServiceLocator.Resolve<IPlayerPrefsService>().GetFloat(key, defaultValue);
        }
        
        private static void SetPlayerPrefsStringValue(string key, string value)
        {
            ServiceLocator.Resolve<IPlayerPrefsService>().SetString(key, value);
        }
        
        private static string GetPlayerPrefsStringValue(string key, string defaultValue = null)
        {
            return ServiceLocator.Resolve<IPlayerPrefsService>().GetString(key, defaultValue);
        }
        
        private static void SetPlayerPrefsBoolValue(string key, bool value)
        {
            ServiceLocator.Resolve<IPlayerPrefsService>().SetBool(key, value);
        }
        
        private static bool GetPlayerPrefsBoolValue(string key, bool defaultValue = false)
        {
            return ServiceLocator.Resolve<IPlayerPrefsService>().GetBool(key, defaultValue);
        }
    }
}
