using UnityEngine;
using Utils;

namespace Scenes.Game.Actor
{
    /// <summary>
    /// ミサイル操作クラス
    /// </summary>
    public class ShipMissileCtrl : MonoBehaviour
    {
        [SerializeField] private float speed; // 移動速度
        [SerializeField] private float destroyDistance; // 破棄される距離

        void Update()
        {
            // 前方へ進む
            transform.position += speed * Time.deltaTime * transform.forward;
            
            // 一定距離離れたら破棄
            if (transform.position.magnitude > destroyDistance)
            {
                Destroy(gameObject);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other) return;

            // エネミーにヒットした場合
            if (other.transform.CompareTag(GameConst.TagNameEnemy))
            {
                // 死亡済なら何もしない
                var enemyCtrl = other.gameObject.GetComponentInParent<EnemyCtrl>();
                if (enemyCtrl.isDead) return;
                enemyCtrl.isDead = true;

                // ヒットタイプ取得
                var hitType = JudgeEnemyHitType(other.transform, transform, enemyCtrl.GetScale());
                
                // エネミー破壊
                enemyCtrl.MissileHit(hitType);
                // 自身も破壊
                Destroy(gameObject);
            }
        }

        // ミサイルの衝突位置調整用
        private const float MissileColOffset = 1.5f;
        
        // ヒットスコア範囲
        private static readonly Vector2 PerfectHitArea = new Vector2(0.3f, 0.3f);
        private static readonly Vector2 GreatHitArea = new Vector2(0.5f, 0.5f);
        
        /// <summary>
        /// ミサイル衝突位置とエネミー位置からヒットタイプを判定する
        /// </summary>
        /// <param name="enemyTransform">エネミーTransform</param>
        /// <param name="missileTransform">ミサイルTransform</param>
        /// <param name="scale">エネミー大きさ</param>
        /// <returns>ヒットタイプ</returns>
        private int JudgeEnemyHitType(Transform enemyTransform, Transform missileTransform, float scale)
        {
            // 衝突位置と敵位置の差分を求める
            var colPos = missileTransform.position + missileTransform.forward * MissileColOffset; // ミサイルの先端にポジションを合わせる
            var diffVec = colPos - enemyTransform.position;
            
            // 差分ベクトル と エネミー正面ベクトル の角度を求める
            var diffVecXZ = new Vector2(diffVec.x, diffVec.z).normalized;
            var enemyForwardVecXZ = new Vector2(enemyTransform.forward.x, enemyTransform.forward.z);
            var angle = Vector2.Angle(diffVecXZ, enemyForwardVecXZ);
            // エネミーから見た横方向の差分 = XZ差分の大きさ * cos(90 - 求めた角度)
            // エネミーから見た上方向の差分 = Y差分の大きさ
            var magnitudeX = diffVecXZ.magnitude * Mathf.Cos((90.0f - angle) * Mathf.Deg2Rad);
            var magnitudeY = Mathf.Abs(diffVec.y); // Y差分はそのまま
            
            // スコア判定
            var perfectArea = PerfectHitArea * scale;
            var greatArea = GreatHitArea * scale;
            // perfect判定範囲か？
            if (magnitudeX < perfectArea.x && magnitudeY < perfectArea.y)
            {
                return GameConst.EnemyHitTypePerfect;
            }
            // great判定範囲か？
            if (magnitudeX < greatArea.x && magnitudeY < greatArea.y)
            {
                return GameConst.EnemyHitTypeGreat;
            }
            // それ以外はgood判定
            return GameConst.EnemyHitTypeGood;
        }
    }
}
