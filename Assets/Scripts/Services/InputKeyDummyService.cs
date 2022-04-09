using System;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// キー入力検知DummyService
    /// 何も処理を行わない
    /// </summary>
    public class InputKeyDummyService : MonoBehaviour, IInputKeyService
    {
        /// <summary>
        /// キーアクション登録
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RegisterLeftKey(IInputKeyService.KeyActionType type, Action action) { }

        public void RegisterRightKey(IInputKeyService.KeyActionType type, Action action) { }
        public void RegisterDownKey(IInputKeyService.KeyActionType type, Action action) { }
        public void RegisterUpKey(IInputKeyService.KeyActionType type, Action action) { }
        public void RegisterEnterKey(IInputKeyService.KeyActionType type, Action action) { }
        public void RegisterBackKey(IInputKeyService.KeyActionType type, Action action) { }
        public void RegisterShotKey(IInputKeyService.KeyActionType type, Action action) { }
        public void RegisterBeamKey(IInputKeyService.KeyActionType type, Action action) { }
        
        /// <summary>
        /// キーアクション登録(bool値受け取り)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RegisterLeftKey(IInputKeyService.KeyActionType type, Action<bool> action) { }
        public void RegisterRightKey(IInputKeyService.KeyActionType type, Action<bool> action) { }
        public void RegisterDownKey(IInputKeyService.KeyActionType type, Action<bool> action) { }
        public void RegisterUpKey(IInputKeyService.KeyActionType type, Action<bool> action) { }
        public void RegisterEnterKey(IInputKeyService.KeyActionType type, Action<bool> action) { }
        public void RegisterBackKey(IInputKeyService.KeyActionType type, Action<bool> action) { }
        public void RegisterShotKey(IInputKeyService.KeyActionType type, Action<bool> action) { }
        public void RegisterBeamKey(IInputKeyService.KeyActionType type, Action<bool> action) { }

        /// <summary>
        /// キーアクション解除
        /// </summary>
        public void UnRegisterLeftKey() { }
        public void UnRegisterRightKey() { }
        public void UnRegisterDownKey() { }
        public void UnRegisterUpKey() { }
        public void UnRegisterEnterKey() { }
        public void UnRegisterBackKey() { }
        public void UnRegisterShotKey() { }
        public void UnRegisterBeamKey() { }

        /// <summary>
        /// キーコマンド全削除
        /// </summary>
        public void ClearAll() { }
    }
}
