using Utils.Components;

namespace Scenes.Game
{
    /// <summary>
    /// GameScene管理クラス
    /// GameClearState
    /// </summary>
    public partial class GameSceneCtrl
    {
        private class StateGameClear : StateMachine<GameSceneCtrl>.StateBase
        {
            public override void OnStart()
            {
                // 結果ウィンドウ表示
                Owner._gameInfoPresenter.ShowResultWindow();
                // キー入力を有効化
                Owner._gameInfoPresenter.OnRegisterClearInputKey();
            }

            public override void OnUpdate() { }

            public override void OnEnd()
            {
                // キー入力を無効化
                Owner._gameInfoPresenter.OnUnRegisterClearInputKey();
            }
        }
    }
}
