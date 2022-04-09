using Services;
using System.Collections;
using UnityEngine;
using Utils.Components;

namespace Scenes.Game
{
    /// <summary>
    /// GameScene管理クラス
    /// SlowMotionState
    /// </summary>
    public partial class GameSceneCtrl
    {
        private class StateSlowMotion : StateMachine<GameSceneCtrl>.StateBase
        {
            public override void OnStart()
            {
                // スローモーション開始
                ServiceLocator.Resolve<IMonoBehaviorService>().DoStartCoroutine(ChangeSlowMotion());
            }

            public override void OnUpdate()
            {
                // スローモーションが終わったらクリア状態に遷移
                if (!_isSlowMotion)
                {
                    StateMachine.ChangeState((int) StateType.GameClear);
                }
            }
            
            public override void OnEnd() { }
            
            // スローモーション処理
            private bool _isSlowMotion;
            private const float SlowMotionTimeScale = 0.2f;
            private const float SlowMotionAudioPitch = 0.45f;
            private const float SlowMotionTime = 0.6f;
            private IEnumerator ChangeSlowMotion()
            {
                // スローモーションフラグをON
                _isSlowMotion = true;
                
                // カメラをクリア状態に切り替える
                Owner.cameraCtrl.StartClearState(Owner._lastDestroyEnemyPosition);
                
                // BGM停止
                ServiceLocator.Resolve<IAudioService>().StopBGM();
                
                // 指定時間分スローモーションにする
                Time.timeScale = SlowMotionTimeScale;
                ServiceLocator.Resolve<IAudioService>().ChangeSePitch(SlowMotionAudioPitch);
                yield return new WaitForSeconds(SlowMotionTime);
                // 元に戻す
                Time.timeScale = 1.0f;
                ServiceLocator.Resolve<IAudioService>().ChangeSePitch(1.0f);

                // カメラを元に戻す
                Owner.cameraCtrl.EndClearState();
                
                // スローモーションフラグをOFF
                _isSlowMotion = false;
                
                // クリア状態に遷移する
                StateMachine.ChangeState((int) StateType.GameClear);
            }
        }
    }
}
