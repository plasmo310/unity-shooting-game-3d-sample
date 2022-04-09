using JsonSchema.Response;
using Services;
using UnityEngine;
using Utils;

namespace Scenes.Game
{
    /// <summary>
    /// GameScene管理クラス
    /// GameState エンドレスモード用
    /// </summary>
    public partial class GameSceneCtrl
    {
        private class StateGameEndlessMode : StateGameTemplate
        {
            private EnemyGenerateInfo _lastGenerateInfo; // 最後に生成した情報
            private int _generateLevel; // 敵生成レベル
            
            /// <summary>
            /// MAXレベルを超えた場合の追加速度
            /// </summary>
            private const float OverLevelAddSpeed = 10.0f;

            /// <summary>
            /// ゲーム開始時の敵生成情報取得
            /// </summary>
            protected override EnemyGenerateInfo GetInitEnemyGenerateInfo()
            {
                // 敵生成レベルに1を指定して開始する
                _generateLevel = 1;
                _lastGenerateInfo = ServiceLocator.Resolve<IDataAccessService>().GetEnemyGenerateInfo(Owner._gameMode, _generateLevel);
                return _lastGenerateInfo;
            }
            
            /// <summary>
            /// プレイヤーが破壊された時の処理
            /// </summary>
            protected override void PlayerDestroyAction()
            {
                // ゲーム終了
                Owner.cameraCtrl.ShakeCamera(); // カメラを揺らす
                Owner._player.StartStateStop(); // プレイヤーを停止状態にする
                StateMachine.ChangeState((int) StateType.GameClear);
            }

            /// <summary>
            /// 敵が全て破壊された時の処理
            /// </summary>
            protected override void AllEnemyDestroyAction()
            {
                // 敵生成レベルアップ
                _generateLevel++;
                Debug.Log("generate level: " + _generateLevel);
                
                // レベルアップメッセージを表示
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeLucky);
                Owner._gameInfoPresenter.ShowLevelUpMsg();
                
                // 敵の生成情報を取得
                var generateInfo = ServiceLocator.Resolve<IDataAccessService>().GetEnemyGenerateInfo(Owner._gameMode, _generateLevel);
                if (generateInfo != null)
                {
                    // 取得できた場合、生成情報を保持
                    _lastGenerateInfo = generateInfo;
                }
                else
                {
                    // 取得できなくなった場合(MAXレベルを超えた場合)、
                    // 最後の生成情報に速度を加えていく
                    _lastGenerateInfo.minSpeed += OverLevelAddSpeed;
                    _lastGenerateInfo.maxSpeed += OverLevelAddSpeed;
                    generateInfo = _lastGenerateInfo;
                }
                
                // 追加で敵を生成する
                GenerateEnemy(generateInfo, Owner._enemyPrefab, 3.0f); // 3秒待機する
            }
        }
    }
}