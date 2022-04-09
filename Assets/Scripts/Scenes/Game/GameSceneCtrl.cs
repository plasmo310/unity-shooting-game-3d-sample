using Scenes.Game.Actor;
using Scenes.Game.Presenter;
using Services;
using UnityEngine;
using Utils;
using Utils.Components;

namespace Scenes.Game
{
    /// <summary>
    /// GameScene管理クラス
    /// </summary>
    public partial class GameSceneCtrl : MonoBehaviour
    {
        [SerializeField] private CameraCtrl cameraCtrl;
        
        private GameObject _shipPrefab;  // プレイヤーPrefab
        private GameObject _enemyPrefab; // 敵Prefab
        
        private GameInfoPresenter _gameInfoPresenter;
        private GameCtrlPresenter _gameCtrlPresenter;

        private ShipCtrl _player; // プレイヤー
        private Vector3 _lastDestroyEnemyPosition; // 最後に倒した敵の位置

        // ステート関連
        private enum StateType
        {
            Ready,      // 準備
            Game,       // ゲーム中
            SlowMotion, // スローモーション
            GameClear,  // クリア
            GameOver,   // ゲームオーバー
        }
        private StateMachine<GameSceneCtrl> _stateMachine;
        
        // ゲームモード、レベル
        private int _gameMode;
        private int _gameLevel;

        /// <summary>
        /// ゲームモード、レベルの設定
        /// SceneLoadEventで設定される
        /// </summary>
        /// <param name="mode">選択したモード</param>
        /// <param name="level">選択したレベル</param>
        public void SetGameModeInfo(int mode, int level = -1)
        {
            _gameMode = mode;
            _gameLevel = level;
        }
        
        private void Awake()
        {
            _shipPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.ActorNameShip);
            _enemyPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.ActorNameEnemy);
            
            // BGM再生
            ServiceLocator.Resolve<IAudioService>().PlayBGM(GameConst.AudioNameShotThunder);
            // バナー広告非表示
            ServiceLocator.Resolve<IAdmobService>().HideBanner();
        }

        private void Start()
        {
            // 初期化
            Initialize();

            // ステートマシン初期化して開始
            _stateMachine = new StateMachine<GameSceneCtrl>(this);
            _stateMachine.Add<StateReady>((int) StateType.Ready);
            if (_gameMode == GameConst.GameModeNormal)
            {
                _stateMachine.Add<StateGameNormalMode>((int) StateType.Game);
            }
            else if (_gameMode == GameConst.GameModeEndless)
            {
                _stateMachine.Add<StateGameEndlessMode>((int) StateType.Game);
            }
            _stateMachine.Add<StateSlowMotion>((int) StateType.SlowMotion);
            _stateMachine.Add<StateGameClear>((int) StateType.GameClear);
            _stateMachine.Add<StateGameOver>((int) StateType.GameOver);
            _stateMachine.OnStart((int) StateType.Ready);
        }

        private void Update()
        {
            _stateMachine.OnUpdate();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            // プレイヤーを生成
            var player = Instantiate(_shipPrefab);
            player.transform.position = Vector3.zero;
            _player = player.GetComponent<ShipCtrl>();
            
            // カメラターゲットに設定
            cameraCtrl.Target = player.transform;

            // Presenterクラスの初期化
            _gameInfoPresenter = new GameInfoPresenter(_gameMode, _gameLevel);
            _gameCtrlPresenter = new GameCtrlPresenter(_player);
            
            // Presenterを初期化
            _gameInfoPresenter.Initialize();
            _gameCtrlPresenter.Initialize();
        }
    }
}
