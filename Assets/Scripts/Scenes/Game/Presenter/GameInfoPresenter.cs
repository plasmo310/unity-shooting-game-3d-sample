using System;
using Scenes.Common;
using Scenes.Game.Model;
using Scenes.Game.View;
using Services;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Utils;

namespace Scenes.Game.Presenter
{
    /// <summary>
    /// GameInfoCanvas操作クラス
    /// </summary>
    public class GameInfoPresenter
    {
        private readonly GameInfoCanvas _view;
        private readonly GameInfoModel _model;
        private readonly int _gameMode;  // ゲームモード
        private readonly int _gameLevel; // ゲームレベル

        /// <summary>
        /// 敵のスコア情報
        /// </summary>
        private readonly JsonSchema.Response.EnemyScore _enemyScore;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gameMode">ゲームモード</param>
        /// <param name="gameLevel">ゲームレベル</param>
        public GameInfoPresenter(int gameMode, int gameLevel)
        {
            // Canvas生成
            var canvasPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.UIGameInfoCanvas);
            _view = ServiceLocator.Resolve<IMonoBehaviorService>().DoInstantiate(canvasPrefab)?.GetComponent<GameInfoCanvas>();
            _model = new GameInfoModel();
            _gameMode = gameMode;
            _gameLevel = gameLevel;

            // 敵のスコア情報を取得
            _enemyScore = ServiceLocator.Resolve<IDataAccessService>().GetEnemyScoreInfo();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            // カニゲージ
            _view.SetActiveKaniGauge(false);
            _view.SetRemainRatioKaniBar(1.0f);
            // カニブレイクエリア
            _view.SetActiveKaniBreakArea(false);
            _view.SetKaniBreakCountText(0);
            // スコア
            _view.SetActiveScore(false);
            _view.SetScoreText(0.0f);
            // ゲーム結果関連
            _view.NormalModeResultWindow.SetActiveResultWindow(false);
            _view.EndlessModeResultWindow.SetActiveResultWindow(false);
            _view.EndlessModeRankingWindow.SetActiveRankingWindow(false);
            _view.SetActiveGameClearMessage(false);
            _view.SetActiveGameOverMessage(false);
            _view.SetActiveFinishMsgImage(false);
            _view.SetActiveLevelUpMsgImage(false);
        }

        /// <summary>
        /// ゲーム更新処理
        /// </summary>
        /// <param name="deltaTime"></param>
        public void OnGameUpdate(float deltaTime)
        {
            // 累計時間を更新
            _model.UpdateTotalTime(deltaTime);
        }

        /// <summary>
        /// Canvas表示
        /// </summary>
        public void ShowCanvas()
        {
            // ----- ノーマルモード -----
            if (_gameMode == GameConst.GameModeNormal)
            {
                // カニゲージ表示
                _view.SetActiveKaniGauge(true);
            }
            // ----- エンドレスモード -----
            else if (_gameMode == GameConst.GameModeEndless)
            {
                // カニブレイクエリア表示
                _view.SetActiveKaniBreakArea(true);
            }
            
            // スコア表示
            _view.SetActiveScore(true);
        }

        /// <summary>
        /// Canvas非表示
        /// </summary>
        public void HideCanvas()
        {
            // ----- ノーマルモード -----
            if (_gameMode == GameConst.GameModeNormal)
            {
                // カニゲージ非表示
                _view.SetActiveKaniGauge(false);
            }
            // ----- エンドレスモード -----
            else if (_gameMode == GameConst.GameModeEndless)
            {
                _view.SetActiveKaniBreakArea(false);
            }
            
            // スコア非表示
            _view.SetActiveScore(false);
        }

        /// <summary>
        /// 生成する敵の数を設定
        /// </summary>
        /// <param name="generateCount">生成する敵の数</param>
        public void SetGenerateEnemyCount(int generateCount)
        {
            _model.SetGenerateEnemyCount(generateCount);
        }
        
        /// <summary>
        /// 残りの敵の数を減らす
        /// </summary>
        /// <param name="hitType">ヒットタイプ</param>
        public void DecrementEnemyCount(int hitType)
        {
            // 敵の数を減らす
            _model.DecrementEnemyCount();
            
            // スコア情報の更新
            _model.UpdateScoreInfoByHitType(hitType, _enemyScore);
            var score = _model.GetGameTotalScore();
            _view.SetScoreText(score);
            
            // ----- ノーマルモード -----
            if (_gameMode == GameConst.GameModeNormal)
            {
                // カニバーの更新
                var ratio = _model.GetRemainEnemyRatio();
                _view.SetRemainRatioKaniBar(ratio);
                _view.ShakeKaniGauge(); // 揺らす
            }
            // ----- エンドレスモード -----
            else if (_gameMode == GameConst.GameModeEndless)
            {
                // カニブレイクエリアの更新
                var breakCount = _model.GetTotalBreakEnemyCount();
                _view.SetKaniBreakCountText(breakCount);
                _view.ShakeEffectKaniBreakArea(); // 揺らす
            }
        }

        /// <summary>
        /// 敵が全て破棄されたか？
        /// </summary>
        public bool IsAllEnemyDestroy()
        {
            return _model.IsAllEnemyDestroy();
        }

        /// <summary>
        /// Enterボタン押下時処理
        /// </summary>
        private event UnityAction PushEnterAction;
        
        /// <summary>
        /// クリア画面キー入力有効化
        /// </summary>
        public void OnRegisterClearInputKey()
        {
            // Enterボタン押下時処理
            ServiceLocator.Resolve<IInputKeyService>().RegisterEnterKey(
                IInputKeyService.KeyActionType.KeyDown,
                () =>
                {
                    // 入力中はキー判定しない
                    if (EventSystem.current?.currentSelectedGameObject?.GetComponent<InputField>() != null)
                    {
                        Debug.Log("input...");
                        return;
                    }
                    // キー入力検知
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        PushEnterAction?.Invoke();
                    }
                });
        }
        
        /// <summary>
        /// クリア画面キー入力無効化
        /// </summary>
        public void OnUnRegisterClearInputKey()
        {
            // 登録したキー入力処理をクリア
            ServiceLocator.Resolve<IInputKeyService>().UnRegisterEnterKey();
        }

        /// <summary>
        /// 結果ウィンドウ表示
        /// </summary>
        public void ShowResultWindow()
        {
            // ----- ノーマルモード -----
            if (_gameMode == GameConst.GameModeNormal)
            {
                ServiceLocator.Resolve<IAudioService>().PlayBGM(GameConst.AudioNameSpaceClear);
                
                // レベルに応じたTimeScale情報を取得
                var timeScale = ServiceLocator.Resolve<IDataAccessService>().GetScoreTimeBonusInfo(GameConst.GameModeNormal, _gameLevel);
            
                // ハイスコアの場合、PlayerPrefsに保存
                var scoreInfo = _model.GetResultScoreInfo(timeScale);
                var totalScore = Mathf.RoundToInt(scoreInfo.ResultTotalScore);
                var isHighScore = false;
                if (totalScore > ShootingPlayerPrefs.GetNormalModeHighScore(_gameLevel))
                {
                    Debug.Log("High Score: " + totalScore);
                    isHighScore = true;
                    ShootingPlayerPrefs.SetNormalModeHighScore(_gameLevel, totalScore);
                }
            
                // スコア情報を表示
                _view.ShowNormalModeResultInfo(
                    scoreInfo.PerfectCount, scoreInfo.PerfectTotalScore,
                    scoreInfo.GreatCount, scoreInfo.GreatTotalScore,
                    scoreInfo.GoodCount, scoreInfo.GoodTotalScore,
                    scoreInfo.TotalTime, scoreInfo.TimeScale,
                    scoreInfo.ResultTotalScore, isHighScore,
                    () =>
                    {
                        // NEXTボタン押下イベントを設定
                        _view.NormalModeResultWindow.SetActiveResultNextButton(true);
                        _view.NormalModeResultWindow.AddListenerResultNextButton(DoNextScene);
                        PushEnterAction = DoNextScene;
                    });
            }
            // ----- エンドレスモード -----
            else if (_gameMode == GameConst.GameModeEndless)
            {
                // BGM停止
                ServiceLocator.Resolve<IAudioService>().StopBGM();
                
                // スコア情報取得
                var scoreInfo = _model.GetResultScoreInfo();
                
                // PlayerPrefs保存処理
                // ハイスコア
                var totalScore = Mathf.RoundToInt(scoreInfo.ResultTotalScore);
                var isHighScore = false;
                if (totalScore > ShootingPlayerPrefs.GetEndlessModeHighScore())
                {
                    Debug.Log("High Score: " + totalScore);
                    isHighScore = true;
                    ShootingPlayerPrefs.SetEndlessModeHighScore(totalScore);
                }
                // 倒した数
                var totalCount = scoreInfo.ResultTotalCount;
                if (totalCount > ShootingPlayerPrefs.GetEndlessModeCount())
                {
                    Debug.Log("Best Count: " + totalCount);
                    ShootingPlayerPrefs.SetEndlessModeCount(totalCount);
                }
                
                // スコア情報を表示
                _view.ShowEndlessModeResultInfo(
                    scoreInfo.PerfectCount, scoreInfo.PerfectTotalScore,
                    scoreInfo.GreatCount, scoreInfo.GreatTotalScore,
                    scoreInfo.GoodCount, scoreInfo.GoodTotalScore,
                    scoreInfo.ResultTotalScore, isHighScore,
                    () =>
                    {
                        // 終了メッセージ表示後、BGMを再開
                        ServiceLocator.Resolve<IAudioService>().PlayBGM(GameConst.AudioNameSpaceClear);
                    },
                    ()=>
                    {
                        // NEXTボタン押下イベントを設定
                        UnityAction showRankingInfo = () =>
                        {
                            ShowEndlessModeRankingWindow(totalScore, totalCount);
                        };
                        _view.EndlessModeResultWindow.SetActiveResultNextButton(true);
                        _view.EndlessModeResultWindow.AddListenerResultNextButton(showRankingInfo);
                        PushEnterAction = showRankingInfo;
                    });
            }
        }
        
        /// <summary>
        /// シーン遷移処理
        /// </summary>
        private void DoNextScene()
        {
            // タイトル画面へ戻る
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
            SceneManager.LoadScene(GameConst.SceneNameTitle);
        }

        /// <summary>
        /// ランキングウィンドウ表示処理
        /// </summary>
        /// <param name="totalScore"></param>
        /// <param name="totalCount"></param>
        private void ShowEndlessModeRankingWindow(int totalScore, int totalCount)
        {
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
            
            // EnterActionを無効化
            PushEnterAction = null;
            
            // ランキング情報の取得
            var leaderBoardName = _view.EndlessModeRankingWindow.IsOnRankingCountToggle()
                ? GameConst.LeaderBoardNameEndlessModeBreakCount
                : GameConst.LeaderBoardNameEndlessModeHighScore;
            SetEndlessModeRankingInfo(leaderBoardName,
                () =>
                {
                    // 完了したらウィンドウを開く
                    _view.EndlessModeRankingWindow.AddListenerRankingCountToggle(PushEndlessModeRankingCountToggle);
                    _view.EndlessModeRankingWindow.AddListenerRankingScoreToggle(PushEndlessModeRankingScoreToggle);
                    _view.EndlessModeRankingWindow.ShowRankingWindow(ShootingPlayerPrefs.PlayerName,
                        totalScore.ToString(), totalCount.ToString(),
                        () =>
                        {
                            PushRankingRegisterButton(totalScore, totalCount);
                        });
                    
                    // NEXTボタン押下イベントを設定
                    _view.EndlessModeRankingWindow.AddListenerNextButton(DoNextScene);
                    PushEnterAction = DoNextScene;
                });
        }

        /// <summary>
        /// エンドレスモードのランキング情報を設定する
        /// </summary>
        /// <param name="leaderBoardName">LeaderBoard名</param>
        /// <param name="callback">設定後の処理</param>
        private void SetEndlessModeRankingInfo(string leaderBoardName, Action callback)
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
                    _view.EndlessModeRankingWindow.ClearAllRankingItem();
                    
                    if (result?.entries == null || result.entries.Length == 0)
                    {
                        // エラーメッセージを表示
                        _view.EndlessModeRankingWindow.AddRankingItem("XXX", "ランキング取得エラー", "", true);
                    }
                    else
                    {
                        // ランキングアイテム追加
                        foreach (var entry in result.entries)
                        {
                            _view.EndlessModeRankingWindow.AddRankingItem((entry.position+1).ToString(), entry.displayName, entry.statValue.ToString());
                        }
                    }
                    
                    // コールバック実行
                    callback();
                });
        }

        /// <summary>
        /// ランキング登録ボタン押下時処理
        /// </summary>
        private void PushRankingRegisterButton(int totalScore, int totalCount)
        {
            // ランキング登録ボタン押下時
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
            _view.EndlessModeRankingWindow.SetInteractableRegisterButton(false);

            // 登録する名前を取得
            var displayName = _view.EndlessModeRankingWindow.GetTextRegisterName();
            if (string.IsNullOrEmpty(displayName))
            {
                _view.EndlessModeRankingWindow.SetTextRegisterResult("名前を入力してください", true);
                _view.EndlessModeRankingWindow.SetInteractableRegisterButton(true); // 失敗時には再度登録できるようにする
                return;
            }
            
            // ローディング表示
            ServiceLocator.Resolve<ILoadingService>().ShowLoading();
            
            // ハイスコアのランキング登録
            ServiceLocator.Resolve<IRankingService>().SendScore(displayName, totalScore, totalCount,
                (result) =>
                {
                    // ローディング非表示
                    ServiceLocator.Resolve<ILoadingService>().HideLoading();
                            
                    // エラー時は処理中断
                    if (!result)
                    {
                        _view.EndlessModeRankingWindow.SetTextRegisterResult("登録に失敗しました", true);
                        _view.EndlessModeRankingWindow.SetInteractableRegisterButton(true); // 失敗時には再度登録できるようにする
                        return;
                    }
                            
                    // ランキング情報の再取得
                    var leaderBoardName = _view.EndlessModeRankingWindow.IsOnRankingCountToggle()
                        ? GameConst.LeaderBoardNameEndlessModeBreakCount
                        : GameConst.LeaderBoardNameEndlessModeHighScore;
                    SetEndlessModeRankingInfo(leaderBoardName,
                        () =>
                        {
                            // 登録完了メッセージを表示
                            _view.EndlessModeRankingWindow.SetTextRegisterResult("登録成功！");
                        });
                });
        }
        
        /// <summary>
        /// ランキング情報切替ボタン押下時処理(倒した数)
        /// </summary>
        /// <param name="isOn"></param>
        private void PushEndlessModeRankingCountToggle(bool isOn)
        {
            // OFFに切り替わった場合は対象外
            if (!isOn) return;
            
            // ランキング情報を切り替える
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            SetEndlessModeRankingInfo(GameConst.LeaderBoardNameEndlessModeBreakCount, () => { });
        }
        
        /// <summary>
        /// ランキング情報切替ボタン押下時処理(ハイスコア)
        /// </summary>
        /// <param name="isOn"></param>
        private void PushEndlessModeRankingScoreToggle(bool isOn)
        {
            // OFFに切り替わった場合は対象外
            if (!isOn) return;
            
            // ランキング情報を切り替える
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            SetEndlessModeRankingInfo(GameConst.LeaderBoardNameEndlessModeHighScore, () => { });
        }

        /// <summary>
        /// レベルアップメッセージ表示
        /// </summary>
        public void ShowLevelUpMsg()
        {
            _view.ShowLevelUpMsgImage();
        }

        /// <summary>
        /// ゲームオーバーメッセージ表示
        /// </summary>
        public void ShowGameOverMessage()
        {
            _view.SetActiveGameOverMessage(true);
        }
    }
}
