using UnityEngine;

namespace Utils.Components
{
    /// <summary>
    /// SingletonMonoBehavior
    /// MonoBehavior有りのSingletonクラスが欲しい場合に継承する
    /// MonoBehavior無しはstaticクラスで作ればよい
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var t = typeof(T);
                _instance = (T)FindObjectOfType(t);
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // 他のゲームオブジェクトにアタッチされていれば破棄
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
