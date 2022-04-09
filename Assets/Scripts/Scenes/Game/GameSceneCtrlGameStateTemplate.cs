using JsonSchema.Response;
using Scenes.Game.Actor;
using Services;
using Utils;
using UnityEngine;
using Utils.Components;

namespace Scenes.Game
{
    /// <summary>
    /// GameScene管理クラス
    /// GameState 各モードでこのクラスを継承する(Template Method)
    /// </summary>
    public partial class GameSceneCtrl
    {
        private abstract class StateGameTemplate : StateMachine<GameSceneCtrl>.StateBase
        {
            public override void OnStart()
            {
                // Canvasを表示
                Owner._gameInfoPresenter.ShowCanvas();
                Owner._gameCtrlPresenter.ShowTouchCanvas();
                
                // キー入力を有効化
                Owner._gameCtrlPresenter.OnRegisterGameInputKey();
                
                // 敵を生成
                var generateInfo = GetInitEnemyGenerateInfo();
                GenerateEnemy(generateInfo, Owner._enemyPrefab);
            }

            public override void OnUpdate()
            {
                // Presenter更新
                Owner._gameInfoPresenter.OnGameUpdate(Time.deltaTime);
                Owner._gameCtrlPresenter.OnGameUpdate();
                
                // Playerが破棄された時
                if (!Owner._player)
                {
                    PlayerDestroyAction();
                }
                // 敵が全て破壊された時
                else if (Owner._gameInfoPresenter.IsAllEnemyDestroy())
                {
                    AllEnemyDestroyAction();
                }
            }

            public override void OnEnd()
            {
                // キー入力を無効化
                Owner._gameCtrlPresenter.OnUnRegisterGameInputKey();
                
                // Canvasを非表示にする
                Owner._gameInfoPresenter.HideCanvas();
                Owner._gameCtrlPresenter.HideTouchCanvas();
            }
            
            // エネミー生成処理
            protected void GenerateEnemy(EnemyGenerateInfo generateInfo, GameObject enemyPrefab, float initWaitTime = 1.0f)
            {
                // 生存している敵の数を初期化
                Owner._gameInfoPresenter.SetGenerateEnemyCount(generateInfo.generateCount);

                // エネミー生成
                var tmpDegree = 0.0f;
                for (var i = 0; i < generateInfo.generateCount; i++)
                {
                    var enemy = ServiceLocator.Resolve<IMonoBehaviorService>().DoInstantiate(enemyPrefab);
                    var enemyCtrl = enemy.GetComponent<EnemyCtrl>();
                    // 前回の出現位置+-60度で生成
                    tmpDegree += Random.Range(generateInfo.minAppearDegree, generateInfo.maxAppearDegree);
                    enemyCtrl.SetInitPositionByDegree(tmpDegree);
                    // 大きさ
                    enemyCtrl.SetScale(Random.Range(generateInfo.minScale, generateInfo.maxScale));
                    // 速度 徐々に速く
                    var speed = Random.Range(generateInfo.minSpeed, generateInfo.maxSpeed) * (1.0f + i * generateInfo.addSpeed);
                    // 揺れ幅 基本は横のみ、数匹ごとに縦揺れも追加
                    var shakeWidth = new Vector3(Random.Range(generateInfo.minShakeWidthX, generateInfo.maxShakeWidthX), 0.0f, 0.0f);
                    if (i % generateInfo.shakeWidthYCount == generateInfo.shakeWidthYCount - 1)
                    {
                        shakeWidth.y = Random.Range(generateInfo.minShakeWidthY, generateInfo.maxShakeWidthY);
                    }
                    enemyCtrl.SetMoveInfo(speed, shakeWidth);
                    // 待機時間
                    var time = i / (float) generateInfo.appearEachCount * Random.Range(generateInfo.minWaitTime, generateInfo.maxWaitTime) + initWaitTime;
                    enemyCtrl.SetWaitTime(time);
                    
                    // 破棄時のイベントを追加
                    enemyCtrl.OnDestroyed += EnemyDestroy;
                }
            }

            // 敵を破棄した時のイベント
            private void EnemyDestroy(Vector3 pos, int hitType)
            {
                // 倒した敵の位置を保持
                Owner._lastDestroyEnemyPosition = pos;
                
                // 残りの敵の数を減らす
                Owner._gameInfoPresenter.DecrementEnemyCount(hitType);
                
                // ビームゲージを追加
                Owner._gameCtrlPresenter.AddBeamGauge(0.1f);
            }
            
            /// <summary>
            /// ゲーム開始時の敵生成情報取得
            /// </summary>
            protected abstract EnemyGenerateInfo GetInitEnemyGenerateInfo();
            
            /// <summary>
            /// プレイヤーが破壊された時の処理
            /// </summary>
            protected abstract void PlayerDestroyAction();
            
            /// <summary>
            /// 敵が全て破壊された時の処理
            /// </summary>
            protected abstract void AllEnemyDestroyAction();
        }
    }
}
