using System;

namespace Services
{
    /// <summary>
    /// キー入力検知interface
    /// </summary>
    public interface IInputKeyService
    {
        /// <summary>
        /// キーアクション種類
        /// </summary>
        public enum KeyActionType
        {
            KeyDown,
            KeyUp,
            KeyPressing,
        }
        
        /// <summary>
        /// キーアクション登録
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RegisterLeftKey(KeyActionType type, Action action);
        public void RegisterRightKey(KeyActionType type, Action action);
        public void RegisterDownKey(KeyActionType type, Action action);
        public void RegisterUpKey(KeyActionType type, Action action);
        public void RegisterEnterKey(KeyActionType type, Action action);
        public void RegisterBackKey(KeyActionType type, Action action);
        public void RegisterShotKey(KeyActionType type, Action action);
        public void RegisterBeamKey(KeyActionType type, Action action);
        
        /// <summary>
        /// キーアクション登録(bool値受け取り)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RegisterLeftKey(KeyActionType type, Action<bool> action);
        public void RegisterRightKey(KeyActionType type, Action<bool> action);
        public void RegisterDownKey(KeyActionType type, Action<bool> action);
        public void RegisterUpKey(KeyActionType type, Action<bool> action);
        public void RegisterEnterKey(KeyActionType type, Action<bool> action);
        public void RegisterBackKey(KeyActionType type, Action<bool> action);
        public void RegisterShotKey(KeyActionType type, Action<bool> action);
        public void RegisterBeamKey(KeyActionType type, Action<bool> action);
        
        /// <summary>
        /// キーアクション解除
        /// </summary>
        public void UnRegisterLeftKey();
        public void UnRegisterRightKey();
        public void UnRegisterDownKey();
        public void UnRegisterUpKey();
        public void UnRegisterEnterKey();
        public void UnRegisterBackKey();
        public void UnRegisterShotKey();
        public void UnRegisterBeamKey();
        
        /// <summary>
        /// キーコマンド全削除
        /// </summary>
        public void ClearAll();
    }
}
