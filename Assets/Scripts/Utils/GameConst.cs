
namespace Utils
{
    /// <summary>
    /// ゲーム全体定数クラス
    /// </summary>
    public static class GameConst
    {
        // Scene名
        public const string SceneNameTitle = "TitleScene";
        public const string SceneNameGame = "GameScene";
        
        // TAG名
        public const string TagNameSceneManager = "SceneManager";
        public const string TagNameEnemy = "Enemy";

        // Audioファイル名
        public const string AudioNameSpaceWorld = "SpaceWould";
        public const string AudioNameShotThunder = "ShotThunder";
        public const string AudioNameSpaceClear = "SpaceClear";
        public const string AudioNameSeClick = "se_click";
        public const string AudioNameSeDecide = "se_decide";
        public const string AudioNameSeCount = "se_count";
        public const string AudioNameSeMachine = "se_machine";
        public const string AudioNameSeShot = "se_shot";
        public const string AudioNameSeBeam = "se_beam";
        public const string AudioNameSeBomb = "se_bomb";
        public const string AudioNameSeBombBig = "se_bomb_big";
        public const string AudioNameSeLucky = "se_lucky";
        public const string AudioNameSeGameOver = "se_game_over";
        
        // ActorPrefab名
        public const string ActorNameEnemy = "ActorEnemy";
        public const string ActorNameShip = "ActorShip";
        public const string ActorNameMissile = "ActorMissile";
        public const string ActorNameBeam = "ActorBeam";
        
        // EffectPrefab名
        public const string EffectNameBombGood = "EffBombGood";
        public const string EffectNameBombGreat = "EffBombGreat";
        public const string EffectNameBombPerfect = "EffBombPerfect";
        
        // UIPrefab名
        public const string UITitleCanvas = "UITitleCanvas";
        public const string UITitleNormalModeWindow = "UITitleNormalModeWindow";
        public const string UITitleEndlessModeWindow = "UITitleEndlessModeWindow";
        public const string UITitleInputNameWindow = "UITitleInputNameWindow";
        public const string UITitleHelpWindow = "UITitleHelpWindow";
        public const string UITitleSettingWindow = "UITitleSettingWindow";
        
        public const string UIGameCtrlCanvas = "UIGameCtrlCanvas";
        public const string UIGameInfoCanvas = "UIGameInfoCanvas";
        public const string UIGameInfoNormalModeResultWindow = "UIGameInfoNormalModeResultWindow";
        public const string UIGameInfoEndlessModeResultWindow = "UIGameInfoEndlessModeResultWindow";
        public const string UIGameInfoEndlessModeRankingWindow = "UIGameInfoEndlessModeRankingWindow";
        
        public const string UILoadingCanvas = "UILoadingCanvas";
        public const string UIDialogCanvas = "UIDialogCanvas";
        public const string UIRankingScrollItem = "UIRankingScrollItem";
        
        // float比較用
        public const float FloatEpsilon = 1E-05f;

        // ゲームモード
        public const int GameModeNormal = 1;  // ノーマルモード
        public const int GameModeEndless = 2; // エンドレスモード
        
        // ノーマルモード ゲームレベル
        public const int GameNormalModeLevelEasy   = 1; // ノーマルモード Easy
        public const int GameNormalModeLevelNormal = 2; // ノーマルモード Normal
        public const int GameNormalModeLevelHard   = 3; // ノーマルモード Hard

        // 敵のヒットタイプ
        public const int EnemyHitTypeGood    = 0; // Good
        public const int EnemyHitTypeGreat   = 1; // Great
        public const int EnemyHitTypePerfect = 2; // Perfect
        public const int EnemyHitTypeBeam    = 3; // ビーム
        
        // LeaderBoard名(ランキング用)
        public const string LeaderBoardNameEndlessModeHighScore = "EndlessModeHighScore";
        public const string LeaderBoardNameEndlessModeBreakCount = "EndlessModeBreakCount";
    }
}
