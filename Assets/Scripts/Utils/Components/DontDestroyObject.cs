using UnityEngine;

namespace Utils.Components
{
    /// <summary>
    /// DontDestroyOnLoadを付与する
    /// </summary>
    public class DontDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
