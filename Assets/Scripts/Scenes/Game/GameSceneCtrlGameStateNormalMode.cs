using JsonSchema.Response;
using Services;

namespace Scenes.Game
{
    /// <summary>
    /// GameScene管理クラス
    /// GameState ノーマルモード用
    /// </summary>
    public partial class GameSceneCtrl
    {
        private class StateGameNormalMode : StateGameTemplate
        {
            /// <summary>
            /// ゲーム開始時の敵生成情報取得
            /// </summary>
            protected override EnemyGenerateInfo GetInitEnemyGenerateInfo()
            {
                return ServiceLocator.Resolve<IDataAccessService>().GetEnemyGenerateInfo(Owner._gameMode, Owner._gameLevel);
            }
            
            /// <summary>
            /// プレイヤーが破壊された時の処理
            /// </summary>
            protected override void PlayerDestroyAction()
            {
                // ゲームオーバー
                Owner.cameraCtrl.ShakeCamera(); // カメラを揺らす
                Owner._player.StartStateStop(); // プレイヤーを停止状態にする
                StateMachine.ChangeState((int) StateType.GameOver);
            }

            /// <summary>
            /// 敵が全て破壊された時の処理
            /// </summary>
            protected override void AllEnemyDestroyAction()
            {
                // ゲームクリア
                Owner._player.StartStateStop(); // プレイヤーを停止状態にする
                StateMachine.ChangeState((int) StateType.SlowMotion); // クリア前にスローモーション状態に遷移
            }
        }
    }
}
