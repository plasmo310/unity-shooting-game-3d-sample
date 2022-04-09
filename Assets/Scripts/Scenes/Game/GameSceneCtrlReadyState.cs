using UnityEngine;
using Utils.Components;

namespace Scenes.Game
{
    /// <summary>
    /// GameScene管理クラス
    /// ReadyState
    /// </summary>
    public partial class GameSceneCtrl
    {
        private class StateReady : StateMachine<GameSceneCtrl>.StateBase
        {
            public override void OnStart()
            {
                // アニメーション開始
                Owner.cameraCtrl.StartAppearAnimation();
            }

            public override void OnUpdate()
            {
                // アニメーションが完了したら次のステートへ
                if (!Owner.cameraCtrl.IsAppearAnimation)
                {
                    Owner._player.StartStateActive(); // プレイヤーをアクティブにする
                    StateMachine.ChangeState((int) StateType.Game);
                }
            }
            
            public override void OnEnd() { }
        }
    }
}
