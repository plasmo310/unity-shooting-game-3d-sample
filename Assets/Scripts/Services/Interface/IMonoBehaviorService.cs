using System.Collections;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// MonoBehaviourService
    /// </summary>
    public interface IMonoBehaviorService
    {
        /// <summary>
        /// コルーチン実行
        /// </summary>
        public void DoStartCoroutine(IEnumerator coroutine);

        /// <summary>
        /// オブジェクト生成
        /// </summary>
        public GameObject DoInstantiate(GameObject obj);
    }
}
