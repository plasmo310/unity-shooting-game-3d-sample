using UnityEngine;

namespace Utils.Components
{
    /// <summary>
    /// Particleの再生が終了次第、自動で破棄する
    /// </summary>
    public class AutoDestroyParticleObject : MonoBehaviour
    {
        private ParticleSystem[] _particleArray;
        
        void Start()
        {
            _particleArray = gameObject.GetComponentsInChildren<ParticleSystem>();
            if (_particleArray == null) return;
            
            // particle開始
            foreach (var particle in _particleArray)
            {
                particle.Play();
            }
        }
        
        void Update()
        {
            if (_particleArray == null) return;
            
            // 再生中のparticleが一つでもあればreturn
            foreach (var particle in _particleArray)
            {
                if (particle.isPlaying)
                {
                    return;
                }
            }
            
            // 再生中のparticleが無ければ破棄
            Destroy(gameObject);
        }
    }
}
