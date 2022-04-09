using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Scenes.Game.Actor
{
    /// <summary>
    /// ビーム操作クラス
    /// </summary>
    public class ShipBeamCtrl : MonoBehaviour
    {
        [SerializeField] private ParticleSystem beamParticle;
        [SerializeField] private GameObject auraCollider;

        private const float BeamReleaseTime = 1.5f;
        private const float DestroyTime = 3.0f;
        private bool _isBeamReleased; // ビームを親からリリース済みか？
        private float _totalBeamTime;

        private void Awake()
        {
            _totalBeamTime = 0.0f;
            _isBeamReleased = false;
            
            // エフェクトにスクリプトをアタッチ
            var beam = beamParticle.gameObject.AddComponent<BeamParticle>();
            beam.SetOnCollision(OnCollisionEnemy);
            var aura = auraCollider.AddComponent<AuraCollider>();
            aura.SetOnCollision(OnCollisionEnemy);
        }

        private void Update()
        {
            // ビームをリリースするまでの時間経過
            _totalBeamTime += Time.deltaTime;
            if (_totalBeamTime > BeamReleaseTime && !_isBeamReleased)
            {
                _isBeamReleased = true;
                // ビームを親から外す
                gameObject.transform.parent = null;
                // オーラを破棄
                Destroy(auraCollider);
            }
            // 自身を破棄
            if (_totalBeamTime > DestroyTime)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// エネミーとの衝突処理
        /// BeamとAuraで共通
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionEnemy(GameObject other)
        {
            if (!other) return;
            
            // エネミーと衝突したら破棄する
            if (other.transform.CompareTag(GameConst.TagNameEnemy))
            {
                // 死亡済なら何もしない
                var enemyCtrl = other.gameObject.GetComponentInParent<EnemyCtrl>();
                if (enemyCtrl.isDead) return;
                enemyCtrl.isDead = true;
                
                // エネミー破壊
                var hitType = GameConst.EnemyHitTypeBeam;
                enemyCtrl.MissileHit(hitType);
            }
        }

        /// <summary>
        /// BeamParticle用クラス
        /// </summary>
        private class BeamParticle : MonoBehaviour
        {
            private UnityAction<GameObject> _onCollision;
            public void SetOnCollision(UnityAction<GameObject> onCollision)
            {
                _onCollision = onCollision;
            }
            private void OnParticleCollision(GameObject other)
            {
                _onCollision(other);
            }
        }
        
        /// <summary>
        /// AuraCollider用クラス
        /// </summary>
        private class AuraCollider : MonoBehaviour
        {
            private UnityAction<GameObject> _onCollision;
            public void SetOnCollision(UnityAction<GameObject> onCollision)
            {
                _onCollision = onCollision;
            }
            private void OnTriggerEnter(Collider other)
            {
                _onCollision(other.gameObject);
            }
        }
    }
}
