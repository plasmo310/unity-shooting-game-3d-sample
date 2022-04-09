using Scenes.Common;
using Scenes.Common.Model;
using Scenes.Game;
using Scenes.Title.View;
using Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utils;
using Utils.Components;
using Random = UnityEngine.Random;

namespace Scenes.Title.Presenter
{
    /// <summary>
    /// TitlePresenter
    /// </summary>
    public class TitlePresenter
    {
        private readonly TitleCanvas _view;
        private readonly SelectItemsModel<int> _selectModeModel;
        private readonly SelectItemsModel<int> _normalModeSelectLevelModel;
        
        // ステート関連
        private enum StateType
        {
            Ready,          // 準備中
            TitleTop,       // タイトルTOP
            NormalMode,     // ノーマルモード選択中
            EndlessMode,    // エンドレスモード選択中
            Setting,        // 設定
            Help,           // ヘルプ
            TranslateScene, // シーン遷移中
        }
        private readonly StateMachine<TitlePresenter> _stateMachine;
        
        public TitlePresenter()
        {
            // Canvas生成
            var canvasPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.UITitleCanvas);
            _view = ServiceLocator.Resolve<IMonoBehaviorService>().DoInstantiate(canvasPrefab)?.GetComponent<TitleCanvas>();

            // モード選択情報Model
            var selectModeArray = new int[]
            {
                GameConst.GameModeNormal,
                GameConst.GameModeEndless,
            };
            _selectModeModel = new SelectItemsModel<int>(selectModeArray);
            
            // ノーマルモードレベル選択情報Model
            var normalModeSelectLevelArray = new int[]
            {
                GameConst.GameNormalModeLevelEasy,
                GameConst.GameNormalModeLevelNormal,
                GameConst.GameNormalModeLevelHard
            };
            _normalModeSelectLevelModel = new SelectItemsModel<int>(normalModeSelectLevelArray);
            
            // UI初期化
            InitializeUI();
            
            // ステート初期化
            _stateMachine = new StateMachine<TitlePresenter>(this);
            _stateMachine.Add<StateReady>((int) StateType.Ready);
            _stateMachine.Add<StateTitleTop>((int) StateType.TitleTop);
            _stateMachine.Add<StateNormalMode>((int) StateType.NormalMode);
            _stateMachine.Add<StateEndlessMode>((int) StateType.EndlessMode);
            _stateMachine.Add<StateSetting>((int) StateType.Setting);
            _stateMachine.Add<StateHelp>((int) StateType.Help);
            _stateMachine.Add<StateTranslateScene>((int) StateType.TranslateScene);
        }

        /// <summary>
        /// UI初期化
        /// </summary>
        private void InitializeUI()
        {
            // マスク
            _view.HideMask();
            
            // 名前入力ウィンドウ
            _view.InputNameWindow.SetActiveInputNameWindow(false);
            _view.InputNameWindow.SetInputNameText("");
            
            // 画面上部ボタン
            _view.SetActiveTopButtonArea(false);
            
            // セッティング関連
            _view.AddListenerSettingButton(() =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                ServiceLocator.Resolve<IAdmobService>().HideBanner(); // バナー非表示
                _view.SettingWindow.SetActiveSettingWindow(true);
            });
            _view.SettingWindow.SetActiveSettingWindow(false);
            _view.SettingWindow.AddListenerSettingWindowMask(() =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                ServiceLocator.Resolve<IAdmobService>().ShowBanner(); // バナー表示
                _view.SettingWindow.SetActiveSettingWindow(false);
            });
            _view.SettingWindow.AddListenerSettingChangeNameButton(() =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                // 名前入力ウィンドウを表示
                _view.InputNameWindow.UpdateListenerInputNameWindowMask(() =>
                {
                    ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                    _view.InputNameWindow.SetActiveInputNameWindow(false);
                });
                _view.InputNameWindow.SetActiveInputNameWindow(true);
                _view.InputNameWindow.SetInputNameText(ShootingPlayerPrefs.PlayerName);
                _view.InputNameWindow.UpdateListenerInputNameOkButton(() =>
                {
                    PushInputPlayerNameOkButton();
                });
            });
            // モバイル以外の場合、名前入力ボタンは非表示にする
            if (!ShootingGameState.IsMobilePlatform)
            {
                _view.SettingWindow.SetActiveSettingChangeNameButton(false);
            }
            
            // 音量関連
            _view.SettingWindow.SetValueSettingBgmSlider(ShootingPlayerPrefs.BgmVolume);
            _view.SettingWindow.SetValueSettingSeSlider(ShootingPlayerPrefs.SeVolume);
            _view.SettingWindow.AddValueChangeSettingBgmSlider((value) =>
            {
                ServiceLocator.Resolve<IAudioService>().ChangeBgmVolume(value);
                ShootingPlayerPrefs.BgmVolume = value;
            });
            _view.SettingWindow.AddValueChangeSettingSeSlider((value) =>
            {
                ServiceLocator.Resolve<IAudioService>().ChangeSeVolume(value);
                ShootingPlayerPrefs.SeVolume = value;
            });
            
            // MoreAppボタン
            _view.AddListenerMoreAppButton(() =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
#if UNITY_ANDROID
                Application.OpenURL("https://play.google.com/store/apps/developer?id=MOLEGORO");
#elif UNITY_IPHONE
                Application.OpenURL("https://apps.apple.com/jp/developer/masato-watanabe/id1523138920");
#else
                Application.OpenURL("https://elekibear.com/molegoro_app");
#endif
            });
            
            // ヘルプ関連
            _view.AddListenerHelpButton(() =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                ServiceLocator.Resolve<IAdmobService>().HideBanner(); // バナー広告非表示
                _view.HelpWindow.SetActiveHelpWindow(true);
            });
            _view.HelpWindow.SetActiveHelpWindow(false);
            var isMobile = ShootingGameState.IsMobilePlatform;
            _view.HelpWindow.SetActiveHelpConsoleManual(!isMobile);
            _view.HelpWindow.SetActiveHelpMobileManual(isMobile);
            _view.HelpWindow.AddListenerHelpOkButton(() =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
                ServiceLocator.Resolve<IAdmobService>().ShowBanner(); // バナー広告表示
                _view.HelpWindow.SetActiveHelpWindow(false);
            });
            _view.HelpWindow.AddListenerHelpWindowMask(() =>
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                ServiceLocator.Resolve<IAdmobService>().ShowBanner(); // バナー広告表示
                _view.HelpWindow.SetActiveHelpWindow(false);
            });

            // プレイヤー名
            _view.SetActivePlayerNameArea(false);

            // タイトル画像
            _view.SetActiveTitleImage(false);
            
            // モード選択ボタン
            _view.SetActiveNormalModeButton(false);
            _view.SetActiveEndlessModeButton(false);
            _view.AddListenerNormalModeButton(PushNormalModeButton);
            _view.AddListenerEndlessModeButton(PushEndlessModeButton);
            _view.SetActiveSelectModeFrame(false);
            
            // ノーマルモードウィンドウ
            _view.NormalModeWindow.SetActiveNormalModeWindow(false);
            _view.NormalModeWindow.AddListenerNormalModeWindowMask(PushNormalWindowBackButton);
            _view.NormalModeWindow.AddListenerNormalModeEasyToggle(PushNormalModeEasyToggle);
            _view.NormalModeWindow.AddListenerNormalModeNormalToggle(PushNormalModeNormalToggle);
            _view.NormalModeWindow.AddListenerNormalModeHardToggle(PushNormalModeHardToggle);
            _view.NormalModeWindow.AddListenerNormalModeGoButton(PushNormalModeGoButton);
            
            // エンドレスモードウィンドウ
            _view.EndlessModeWindow.SetActiveEndlessModeWindow(false);
            _view.EndlessModeWindow.AddListenerEndlessModeWindowMask(PushEndlessWindowBackButton);
            _view.EndlessModeWindow.AddListenerEndlessModeGoButton(PushEndlessModeGoButton);
            _view.EndlessModeWindow.AddListenerRankingCountToggle(PushEndlessModeRankingCountToggle);
            _view.EndlessModeWindow.AddListenerRankingScoreToggle(PushEndlessModeRankingScoreToggle);

            // 選択モード、選択レベル表示を更新
            var selectMode = _selectModeModel.GetSelectItem();
            ChangeModeView(selectMode);
            var selectLevel = _normalModeSelectLevelModel.GetSelectItem();
            ChangeNormalModeLevelView(selectLevel);
            
            // 最初は非表示
            _view.SetActiveTitleCanvas(false);
        }
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        public void OnStart()
        {
            _stateMachine.OnStart((int) StateType.Ready);
        }
        
        /// <summary>
        /// 更新処理
        /// </summary>
        public void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }
        
        // 各ステート定義
        // ----- ready -----
        private class StateReady : StateMachine<TitlePresenter>.StateBase
        {
            private bool _isInitReady = false;
            
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // ゲームの準備が完了するまで待機
                if (!ShootingGameState.IsGameReady) return;
            
                // 画面の設定を行う
                if (!_isInitReady)
                {
                    _isInitReady = true;
                
                    // タイトルCanvasを表示
                    Owner._view.SetActiveTitleCanvas(true);
                
                    // モバイルの場合、プレイヤー名が登録されていなければ名前入力ウィンドウを表示する
                    if (string.IsNullOrEmpty(ShootingPlayerPrefs.PlayerName) && ShootingGameState.IsMobilePlatform)
                    {
                        // プレイヤー名が登録されていない場合、名前入力ウィンドウを表示する
                        Owner._view.InputNameWindow.UpdateListenerInputNameWindowMask(() => { });
                        Owner._view.InputNameWindow.SetActiveInputNameWindow(true);
                        Owner._view.InputNameWindow.SetInputNameText("");

                        // OKボタン押下時処理
                        Owner._view.InputNameWindow.UpdateListenerInputNameOkButton(() =>
                        {
                            // 設定が完了したらタイトルTOPに遷移
                            Owner.PushInputPlayerNameOkButton(() => StateMachine.ChangeState((int) StateType.TitleTop));
                        });
                        return;
                    }
                
                    // TitleTopStateに遷移
                    StateMachine.ChangeState((int) StateType.TitleTop);
                }
            }
            public override void OnEnd() { }
        }
        
        // ----- title top -----
        private class StateTitleTop : StateMachine<TitlePresenter>.StateBase
        {
            private bool _isInitTitleTop = false;

            public override void OnStart()
            {
                // 最初に表示された時のみ初期化を行う
                if (!_isInitTitleTop)
                {
                    // タイトルCanvasを表示
                    Owner._view.SetActiveTitleCanvas(true);
                        
                    // タイトル画像のアニメーション開始
                    Owner._view.SetActiveTitleImage(true);
                    Owner._view.ShowTitleImage(() =>
                    {
                        // モバイルの場合、ヘルプとプレイヤー表示を行う
                        if (ShootingGameState.IsMobilePlatform)
                        {
                            // 一度もヘルプを表示したことがなければ表示する
                            if (!ShootingPlayerPrefs.IsShowHelp)
                            {
                                Owner._view.HelpWindow.SetActiveHelpWindow(true);
                                ShootingPlayerPrefs.IsShowHelp = true;
                            }
                            else
                            {
                                // バナー広告表示
                                ServiceLocator.Resolve<IAdmobService>().ShowBanner();
                            }

                            // プレイヤー名表示
                            Owner._view.SetPlayerNameText(ShootingPlayerPrefs.PlayerName);
                            Owner._view.SetActivePlayerNameArea(true);
                        }

                        // ボタン類表示
                        Owner._view.SetActiveTopButtonArea(true);
                        Owner._view.SetActiveNormalModeButton(true);
                        Owner._view.SetActiveEndlessModeButton(true);
                        
                        // カニを出現させる
                        Owner._view.ShowKaniBound();
                    
                        // モバイル以外の場合、モード選択フレームを表示する
                        if (!ShootingGameState.IsMobilePlatform)
                        {
                            Owner._view.SetActiveSelectModeFrame(true);
                        }
                    });

                    _isInitTitleTop = true;
                }
                
                // キー入力検知
                // モード選択
                ServiceLocator.Resolve<IInputKeyService>().RegisterLeftKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    () =>
                    {
                        ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                        Owner._selectModeModel.ChangePrevItemIndex();
                        var selectMode = Owner._selectModeModel.GetSelectItem();
                        Owner.ChangeModeView(selectMode);
                    });
                ServiceLocator.Resolve<IInputKeyService>().RegisterRightKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    () =>
                    {
                        ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                        Owner._selectModeModel.ChangeNextSelectIndex();
                        var selectMode = Owner._selectModeModel.GetSelectItem();
                        Owner.ChangeModeView(selectMode);
                    });
                // 各モードを開く
                ServiceLocator.Resolve<IInputKeyService>().RegisterEnterKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    () =>
                    {
                        var selectMode = Owner._selectModeModel.GetSelectItem();
                        switch (selectMode)
                        {
                            case GameConst.GameModeNormal:
                                Owner.PushNormalModeButton();
                                break;
                            case GameConst.GameModeEndless:
                                Owner.PushEndlessModeButton();
                                break;
                        }
                    });
            }

            public override void OnUpdate()
            {
                // 開かれたウィンドウによって状態遷移する
                if (Owner._view.NormalModeWindow.IsActiveNormalModeWindow())
                {
                    StateMachine.ChangeState((int) StateType.NormalMode);
                    return;
                }
                if (Owner._view.EndlessModeWindow.IsActiveEndlessModeWindow())
                {
                    StateMachine.ChangeState((int) StateType.EndlessMode);
                    return;
                }
                if (Owner._view.SettingWindow.IsActiveSettingWindow())
                {
                    StateMachine.ChangeState((int) StateType.Setting);
                    return;
                }
                if (Owner._view.HelpWindow.IsActiveHelpWindow())
                {
                    StateMachine.ChangeState((int) StateType.Help);
                    return;
                }
            }

            public override void OnEnd()
            {
                // 登録したキー入力を解除
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterLeftKey();
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterRightKey();
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterEnterKey();
            }
        }
        
        // ----- normal mode -----
        private class StateNormalMode : StateMachine<TitlePresenter>.StateBase
        {
            public override void OnStart()
            {
                // キー入力検知
                // 閉じる
                ServiceLocator.Resolve<IInputKeyService>().RegisterBackKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    Owner.PushNormalWindowBackButton);
                // レベル選択
                ServiceLocator.Resolve<IInputKeyService>().RegisterUpKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    () =>
                    {
                        ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                        Owner._normalModeSelectLevelModel.ChangePrevItemIndex();
                        var selectLevel = Owner._normalModeSelectLevelModel.GetSelectItem();
                        Owner.ChangeNormalModeLevelView(selectLevel);
                    });
                ServiceLocator.Resolve<IInputKeyService>().RegisterDownKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    () =>
                    {
                        ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
                        Owner._normalModeSelectLevelModel.ChangeNextSelectIndex();
                        var selectLevel = Owner._normalModeSelectLevelModel.GetSelectItem();
                        Owner.ChangeNormalModeLevelView(selectLevel);
                    });
                // ゲーム開始
                ServiceLocator.Resolve<IInputKeyService>().RegisterEnterKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    Owner.PushNormalModeGoButton);
            }

            public override void OnUpdate()
            {
                // ウィンドウを閉じたらTitleTopStateへ遷移
                if (!Owner._view.NormalModeWindow.IsActiveNormalModeWindow())
                {
                    StateMachine.ChangeState((int) StateType.TitleTop);
                    return;
                }
            }

            public override void OnEnd()
            {
                // 登録したキー入力を解除
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterBackKey();
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterUpKey();
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterDownKey();
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterEnterKey();
            }
        }
        
        // ----- endless mode -----
        private class StateEndlessMode : StateMachine<TitlePresenter>.StateBase
        {
            public override void OnStart()
            {
                // キー入力検知
                // 閉じる
                ServiceLocator.Resolve<IInputKeyService>().RegisterBackKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    Owner.PushEndlessWindowBackButton);
                // ゲーム開始
                ServiceLocator.Resolve<IInputKeyService>().RegisterEnterKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    Owner.PushEndlessModeGoButton);
            }

            public override void OnUpdate()
            {
                // ウィンドウを閉じたらTitleTopStateへ遷移
                if (!Owner._view.EndlessModeWindow.IsActiveEndlessModeWindow())
                {
                    StateMachine.ChangeState((int) StateType.TitleTop);
                    return;
                }
            }

            public override void OnEnd()
            {
                // 登録したキー入力を解除
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterBackKey();
                ServiceLocator.Resolve<IInputKeyService>().UnRegisterEnterKey();
            }
        }
        
        // ----- setting -----
        private class StateSetting : StateMachine<TitlePresenter>.StateBase
        {
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // ウィンドウを閉じたらTitleTopStateへ遷移
                if (!Owner._view.SettingWindow.IsActiveSettingWindow()
                    && !Owner._view.InputNameWindow.IsActiveInputNameWindow())
                {
                    StateMachine.ChangeState((int) StateType.TitleTop);
                }
            }
            public override void OnEnd() { }
        }
        
        // ----- help -----
        private class StateHelp : StateMachine<TitlePresenter>.StateBase
        {
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // ウィンドウを閉じたらTitleTopStateへ遷移
                if (!Owner._view.HelpWindow.IsActiveHelpWindow())
                {
                    StateMachine.ChangeState((int) StateType.TitleTop);
                }
            }
            public override void OnEnd() { }
        }
        
        // ----- translate scene -----
        private class StateTranslateScene : StateMachine<TitlePresenter>.StateBase
        {
            public override void OnStart()
            {
                TranslateGameScene();
            }
            public override void OnUpdate() { }
            public override void OnEnd() { }
            
            /// <summary>
            /// GameScene遷移処理
            /// </summary>
            private void TranslateGameScene()
            {
                Owner._view.ShowMask(() =>
                {
                    SceneManager.sceneLoaded += GameSceneLoaded;
                    SceneManager.LoadScene(GameConst.SceneNameGame);
                });
            }

            /// <summary>
            /// GameSceneロード後処理
            /// </summary>
            private void GameSceneLoaded(Scene next, LoadSceneMode mode)
            {
                // 選択したモードとレベルを渡す
                var sceneManager = GameObject.FindWithTag(GameConst.TagNameSceneManager);
                var gameSceneCtrl = sceneManager.GetComponent<GameSceneCtrl>();
                var selectMode = Owner._selectModeModel.GetSelectItem();
                if (selectMode == GameConst.GameModeNormal)
                {
                    var selectLevel = Owner._normalModeSelectLevelModel.GetSelectItem();
                    gameSceneCtrl.SetGameModeInfo(selectMode, selectLevel);
                }
                else
                {
                    // エンドレスモードはレベル指定無し
                    gameSceneCtrl.SetGameModeInfo(selectMode);
                }
                // SceneLoadedから削除
                SceneManager.sceneLoaded -= GameSceneLoaded;
            }
        }
        
        // ボタンクリックイベント
        // ---- title top ----
        private void PushNormalModeButton()
        {
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            ServiceLocator.Resolve<IAdmobService>().HideBanner(); // バナー非表示

            // 違うモードが選択されてたら表示を更新する
            var selectMode = _selectModeModel.GetSelectItem();
            if (selectMode != GameConst.GameModeNormal)
            {
                _selectModeModel.SetSelectIndex(0);
                ChangeModeView(GameConst.GameModeNormal);
            }
            _view.NormalModeWindow.SetActiveNormalModeWindow(true); // ウィンドウを開く
        }
        
        private void PushEndlessModeButton()
        {
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            ServiceLocator.Resolve<IAdmobService>().HideBanner(); // バナー非表示
            
            // 違うモードが選択されてたら表示を更新する
            var selectMode = _selectModeModel.GetSelectItem();
            if (selectMode != GameConst.GameModeEndless)
            {
                _selectModeModel.SetSelectIndex(1);
                ChangeModeView(GameConst.GameModeEndless);
            }

            // エンドレスモードの情報を設定
            var highScore = ShootingPlayerPrefs.GetEndlessModeHighScore();
            var count = ShootingPlayerPrefs.GetEndlessModeCount();
            _view.EndlessModeWindow.SetEndlessModeInfo(count, highScore);
            
            // ランキング情報を設定
            var leaderBoardName = _view.EndlessModeWindow.IsOnRankingCountToggle()
                ? GameConst.LeaderBoardNameEndlessModeBreakCount
                : GameConst.LeaderBoardNameEndlessModeHighScore;
            SetEndlessModeRankingInfo(leaderBoardName,
                () =>
                {
                    // 完了したらウィンドウを開く
                    _view.EndlessModeWindow.SetActiveEndlessModeWindow(true);
                });
        }
        
        /// <summary>
        /// 選択モード表示更新処理
        /// </summary>
        /// <param name="selectMode"></param>
        private void ChangeModeView(int selectMode)
        {
            _view.ChangeSelectMode(selectMode);
        }

        /// <summary>
        /// ノーマルモードのレベル表示を更新する
        /// </summary>
        private void ChangeNormalModeLevelView(int selectLevel)
        {
            // ステージ情報を取得して更新
            var scoreInfo = ServiceLocator.Resolve<IDataAccessService>().GetStageDetailInfo(GameConst.GameModeNormal, selectLevel);
            var highScore = ShootingPlayerPrefs.GetNormalModeHighScore(selectLevel);
            _view.NormalModeWindow.ChangeNormalModeSelectLevel(selectLevel, scoreInfo, highScore);
        }
        
        /// <summary>
        /// エンドレスモードのランキング情報を設定する
        /// </summary>
        /// <param name="leaderBoardName">LeaderBoard名</param>
        /// <param name="callback">設定後の処理</param>
        private void SetEndlessModeRankingInfo(string leaderBoardName, System.Action callback)
        {
            // ローディング表示
            ServiceLocator.Resolve<ILoadingService>().ShowLoading();
            
            // ランキング情報取得
            ServiceLocator.Resolve<IRankingService>().GetLeaderBoardInfo(leaderBoardName,
                (result) =>
                {
                    // ローディング非表示
                    ServiceLocator.Resolve<ILoadingService>().HideLoading();
                    
                    // ランキングアイテム削除
                    _view.EndlessModeWindow.ClearAllRankingItem();
                    
                    if (result?.entries == null || result.entries.Length == 0)
                    {
                        // エラーメッセージを表示
                        _view.EndlessModeWindow.AddRankingItem("XXX", "ランキング取得エラー", "", true);
                    }
                    else
                    {
                        // ランキングアイテム追加
                        foreach (var entry in result.entries)
                        {
                            _view.EndlessModeWindow.AddRankingItem((entry.position+1).ToString(), entry.displayName, entry.statValue.ToString());
                        }
                    }
                    
                    // コールバック実行
                    callback();
                });
        }

        // ---- normal mode ----
        private void PushNormalWindowBackButton()
        {
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            ServiceLocator.Resolve<IAdmobService>().ShowBanner(); // バナー表示
            _view.NormalModeWindow.SetActiveNormalModeWindow(false); // ウィンドウを閉じる
        }
        private void PushNormalModeEasyToggle(bool isOn)
        {
            ChangeNormalModeLevelToggle(isOn, 0);
        }
        private void PushNormalModeNormalToggle(bool isOn)
        {
            ChangeNormalModeLevelToggle(isOn, 1);
        }
        private void PushNormalModeHardToggle(bool isOn)
        {
            ChangeNormalModeLevelToggle(isOn, 2);
        }
        private void PushNormalModeGoButton()
        {
            _view.NormalModeWindow.SetInteractableNormalModeGoButton(false);
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
            _stateMachine.ChangeState((int) StateType.TranslateScene);
        }

        /// <summary>
        /// ノーマルモード toggle切替処理
        /// </summary>
        /// <param name="isOn">ONになったか？</param>
        /// <param name="index">設定するレベルindex</param>
        private void ChangeNormalModeLevelToggle(bool isOn, int index)
        {
            // OFFに切り替わった場合は対象外
            if (!isOn) return;
            // 既にmodelで選択済の場合は対象外
            if (index == _normalModeSelectLevelModel.GetSelectIndex()) return;
            
            // 選択されたレベルに切り替える
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            _normalModeSelectLevelModel.SetSelectIndex(index);
            var selectLevel = _normalModeSelectLevelModel.GetSelectItem();
            ChangeNormalModeLevelView(selectLevel);
        }
        
        // ---- endless mode ----
        private void PushEndlessWindowBackButton()
        {
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            ServiceLocator.Resolve<IAdmobService>().ShowBanner(); // バナー表示
            _view.EndlessModeWindow.SetActiveEndlessModeWindow(false); // ウィンドウを閉じる
        }
        private void PushEndlessModeGoButton()
        {
            _view.EndlessModeWindow.SetInteractableEndlessModeGoButton(false);
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
            _stateMachine.ChangeState((int) StateType.TranslateScene);
        }
        private void PushEndlessModeRankingCountToggle(bool isOn)
        {
            // OFFに切り替わった場合は対象外
            if (!isOn) return;
            
            // ランキング情報を切り替える
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            SetEndlessModeRankingInfo(GameConst.LeaderBoardNameEndlessModeBreakCount, () => { });
        }
        private void PushEndlessModeRankingScoreToggle(bool isOn)
        {
            // OFFに切り替わった場合は対象外
            if (!isOn) return;
            
            // ランキング情報を切り替える
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            SetEndlessModeRankingInfo(GameConst.LeaderBoardNameEndlessModeHighScore, () => { });
        }
        
        // ---- setting ----
        /// <summary>
        /// プレイヤー名入力OKボタン押下時
        /// </summary>
        private void PushInputPlayerNameOkButton(UnityAction callback = null)
        {
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
            
            // ローディング表示
            ServiceLocator.Resolve<ILoadingService>().ShowLoading();

            // 名前を整形
            var name = _view.InputNameWindow.GetInputNameText();
            if (string.IsNullOrEmpty(name))
            {
                // ランダム名称の取得
                var randomNameInfo = ServiceLocator.Resolve<IDataAccessService>().GetRandomNameInfo();
                if (randomNameInfo?.names == null || randomNameInfo.names.Length == 0)
                {
                    name = "名無し";
                }
                else
                {
                    // 用意された名称 + 数値3桁
                    var index = Random.Range(0, randomNameInfo.names.Length);
                    var prefix = Random.Range(0, 999).ToString("D3");
                    name = randomNameInfo.names[index].name + prefix;
                }
            }
            
            // 10文字以内になるよう切り取る
            if (name.Length > 10)
            {
                name = name.Substring(0, 10); 
            }
            
            // プレイヤー名を設定する
            ServiceLocator.Resolve<IRankingService>().SetPlayerName(name,
                (result) =>
                {
                    // ローディング非表示
                    ServiceLocator.Resolve<ILoadingService>().HideLoading();
                    
                    // エラーが発生した場合、メッセージを表示してアプリ終了
                    if (!result)
                    {
                        ServiceLocator.Resolve<IDialogService>().ShowNormalDialog(
                            "接続エラーが発生しました。\r\nアプリを終了します。",
                            () =>
                            {
                                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
#if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                                Application.Quit();
#endif
                            });
                        return;
                    }
                    
                    // プレイヤー名を保存
                    ShootingPlayerPrefs.PlayerName = name;
                    _view.SetPlayerNameText(name);
                    
                    // ダイアログを表示してゲーム開始
                    ServiceLocator.Resolve<IDialogService>().ShowNormalDialog(
                        name + " さん\r\nようこそ！\r\n楽しんでいってくださいね",
                        () =>
                        {
                            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);

                            // ウィンドウを閉じる
                            ServiceLocator.Resolve<IDialogService>().HideNormalDialog();
                            _view.InputNameWindow.SetActiveInputNameWindow(false);
                            callback?.Invoke();
                        });
                });
        }
    }
}
