using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    /// <summary>
    /// キー入力検知Service
    /// </summary>
    public class InputKeyService : MonoBehaviour, IInputKeyService
    {
        /// <summary>
        /// キーアクション定義
        /// </summary>
        private class KeyAction
        {
            public IInputKeyService.KeyActionType Type; // キーアクション種類
            public Action<bool> Action; // キーアクション
        }
        
        /// <summary>
        /// 各キーアクション
        /// </summary>
        private KeyAction _leftKeyAction;  // 左キー
        private KeyAction _rightKeyAction; // 右キー
        private KeyAction _downKeyAction;  // 下キー
        private KeyAction _upKeyAction;    // 上キー
        private KeyAction _enterKeyAction; // Enterキー
        private KeyAction _backKeyAction;  // BackSpaceキー
        private KeyAction _shotKeyAction;  // Kキー
        private KeyAction _beamKeyAction;  // Lキー

        private void Awake()
        {
            SceneManager.sceneUnloaded += UnLoadScene;
        }

        private void Update()
        {
            // キー入力を検知してアクションを実行する
            _leftKeyAction?.Action(IsInputKey(KeyCode.A, _leftKeyAction) || IsInputKey(KeyCode.LeftArrow, _leftKeyAction));
            _rightKeyAction?.Action(IsInputKey(KeyCode.D, _rightKeyAction) || IsInputKey(KeyCode.RightArrow, _rightKeyAction));
            _downKeyAction?.Action(IsInputKey(KeyCode.S, _downKeyAction) || IsInputKey(KeyCode.DownArrow, _downKeyAction));
            _upKeyAction?.Action(IsInputKey(KeyCode.W, _upKeyAction) || IsInputKey(KeyCode.UpArrow, _upKeyAction));
            _enterKeyAction?.Action(IsInputKey(KeyCode.Return, _enterKeyAction));
            _backKeyAction?.Action(IsInputKey(KeyCode.Delete, _backKeyAction) || IsInputKey(KeyCode.Backspace, _backKeyAction));
            _shotKeyAction?.Action(IsInputKey(KeyCode.K, _shotKeyAction));
            _beamKeyAction?.Action(IsInputKey(KeyCode.L, _beamKeyAction));
        }

        /// <summary>
        /// キーが入力されているか？
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="keyAction"></param>
        /// <returns></returns>
        private bool IsInputKey(KeyCode keyCode, KeyAction keyAction)
        {
            // アクションが登録されていなければ処理しない
            if (keyAction == null)
            {
                return false;
            }
            // 種類によってキー入力を検知する
            var isDoAction = false;
            switch (keyAction.Type)
            {
                case IInputKeyService.KeyActionType.KeyDown:
                    isDoAction = Input.GetKeyDown(keyCode);
                    break;
                case IInputKeyService.KeyActionType.KeyUp:
                    isDoAction = Input.GetKeyUp(keyCode);
                    break;
                case IInputKeyService.KeyActionType.KeyPressing:
                    isDoAction = Input.GetKey(keyCode);
                    break;
            }
            return isDoAction;
        }
        
        /// <summary>
        /// キーアクション登録
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RegisterLeftKey(IInputKeyService.KeyActionType type, Action action)
        {
            _leftKeyAction = new KeyAction { Type = type, Action = CreateBoolAction(action), };
        }
        public void RegisterRightKey(IInputKeyService.KeyActionType type, Action action)
        {
            _rightKeyAction = new KeyAction { Type = type, Action = CreateBoolAction(action), };
        }
        public void RegisterDownKey(IInputKeyService.KeyActionType type, Action action)
        {
            _downKeyAction = new KeyAction { Type = type, Action = CreateBoolAction(action), };
        }
        public void RegisterUpKey(IInputKeyService.KeyActionType type, Action action)
        {
            _upKeyAction = new KeyAction { Type = type, Action = CreateBoolAction(action), };
        }
        public void RegisterEnterKey(IInputKeyService.KeyActionType type, Action action)
        {
            _enterKeyAction = new KeyAction { Type = type, Action = CreateBoolAction(action), };
        }
        public void RegisterBackKey(IInputKeyService.KeyActionType type, Action action)
        {
            _backKeyAction = new KeyAction { Type = type, Action = CreateBoolAction(action), };
        }
        public void RegisterShotKey(IInputKeyService.KeyActionType type, Action action)
        {
            _shotKeyAction = new KeyAction { Type = type, Action = CreateBoolAction(action), };
        }
        public void RegisterBeamKey(IInputKeyService.KeyActionType type, Action action)
        {
            _beamKeyAction = new KeyAction { Type = type, Action = CreateBoolAction(action), };
        }
        
        private Action<bool> CreateBoolAction(Action action)
        {
            // ボタンが押下された時のみ実行する
            return (isPush) =>
            {
                if (isPush) action();
            };
        }
        
        /// <summary>
        /// キーアクション登録(bool値受け取り)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RegisterLeftKey(IInputKeyService.KeyActionType type, Action<bool> action)
        {
            _leftKeyAction = new KeyAction { Type = type, Action = action, };
        }
        public void RegisterRightKey(IInputKeyService.KeyActionType type, Action<bool> action)
        {
            _rightKeyAction = new KeyAction { Type = type, Action = action, };
        }
        public void RegisterDownKey(IInputKeyService.KeyActionType type, Action<bool> action)
        {
            _downKeyAction = new KeyAction { Type = type, Action = action, };
        }
        public void RegisterUpKey(IInputKeyService.KeyActionType type, Action<bool> action)
        {
            _upKeyAction = new KeyAction { Type = type, Action = action, };
        }
        public void RegisterEnterKey(IInputKeyService.KeyActionType type, Action<bool> action)
        {
            _enterKeyAction = new KeyAction { Type = type, Action = action, };
        }
        public void RegisterBackKey(IInputKeyService.KeyActionType type, Action<bool> action)
        {
            _backKeyAction = new KeyAction { Type = type, Action = action, };
        }
        public void RegisterShotKey(IInputKeyService.KeyActionType type, Action<bool> action)
        {
            _shotKeyAction = new KeyAction { Type = type, Action = action, };
        }
        public void RegisterBeamKey(IInputKeyService.KeyActionType type, Action<bool> action)
        {
            _beamKeyAction = new KeyAction { Type = type, Action = action, };
        }
        
        /// <summary>
        /// キーアクション解除
        /// </summary>
        public void UnRegisterLeftKey()
        {
            _leftKeyAction = null;
        }
        public void UnRegisterRightKey()
        {
            _rightKeyAction = null;
        }
        public void UnRegisterDownKey()
        {
            _downKeyAction = null;
        }
        public void UnRegisterUpKey()
        {
            _upKeyAction = null;
        }
        public void UnRegisterEnterKey()
        {
            _enterKeyAction = null;
        }
        public void UnRegisterBackKey()
        {
            _backKeyAction = null;
        }
        public void UnRegisterShotKey()
        {
            _shotKeyAction = null;
        }
        public void UnRegisterBeamKey()
        {
            _beamKeyAction = null;
        }
        
        /// <summary>
        /// キーコマンド全削除
        /// </summary>
        public void ClearAll()
        {
            // 各キーアクションをクリア
            _leftKeyAction = null;
            _rightKeyAction = null;
            _downKeyAction = null;
            _upKeyAction = null;
            _enterKeyAction = null;
            _backKeyAction = null;
            _shotKeyAction = null;
            _beamKeyAction = null;
        }
        
        /// <summary>
        /// Sceneアンロード検知
        /// </summary>
        /// <param name="scene"></param>
        private void UnLoadScene(Scene scene)
        {
            // Sceneアンロード時にクリアする
            ClearAll();
        }
    }
}
