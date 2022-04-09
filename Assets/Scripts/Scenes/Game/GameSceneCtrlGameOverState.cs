using System.Collections;
using Services;
using Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Components;

namespace Scenes.Game
{
    /// <summary>
    /// GameScene管理クラス
    /// GameOverState
    /// </summary>
    public partial class GameSceneCtrl
    {
        private class StateGameOver : StateMachine<GameSceneCtrl>.StateBase
        {
            private bool _isActiveNext = false;
            
            public override void OnStart()
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeGameOver);
                
                // バナー広告表示
                ServiceLocator.Resolve<IAdmobService>().ShowBanner();
                
                // ゲームオーバーメッセージ表示
                Owner._gameInfoPresenter.ShowGameOverMessage();

                // 数秒待機させる
                ServiceLocator.Resolve<IMonoBehaviorService>().DoStartCoroutine(WaitActiveNext());
            }

            public override void OnUpdate()
            {
                if (!_isActiveNext) return;
                
                // ボタン押下でTitleSceneへ遷移
                if (TouchUtil.IsScreenTouch())
                {
                    TransitionTitleScene();
                }
            }
            
            private IEnumerator WaitActiveNext()
            {
                _isActiveNext = false;
                yield return new WaitForSeconds(1.0f);
                _isActiveNext = true;
                
                // キー入力処理を登録
                ServiceLocator.Resolve<IInputKeyService>().RegisterEnterKey(
                    IInputKeyService.KeyActionType.KeyDown,
                    TransitionTitleScene);
            }
            
            public override void OnEnd() { }
            
            /// <summary>
            /// タイトルシーン遷移処理
            /// </summary>
            private void TransitionTitleScene()
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
                SceneManager.LoadScene(GameConst.SceneNameTitle);
            }
        }
    }
}
