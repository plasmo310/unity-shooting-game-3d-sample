using System.Collections;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// MonoBehaviourService
    /// 非MonoBehaviorクラスはこの関数を使用する
    /// </summary>
    public class MonoBehaviorService : MonoBehaviour, IMonoBehaviorService
    {
        /// <summary>
        /// コルーチン実行
        /// </summary>
        public void DoStartCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }
        
        /// <summary>
        /// オブジェクト生成
        /// </summary>
        public GameObject DoInstantiate(GameObject obj)
        {
            return Instantiate(obj);
        }
    }
}
