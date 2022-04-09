using UnityEngine;

namespace Scenes.Game.Actor
{
    /// <summary>
    /// エネミーマーカー操作クラス
    /// </summary>
    public class EnemyMarkerCtrl : MonoBehaviour
    {
        [SerializeField] private RectTransform markerTransform;
        [SerializeField] private EnemyCtrl enemyCtrl;
        
        private const float OffsetStandardPos = 125.0f; // マーカー表示のオフセット
        private const float HideDistance = 145.0f;      // マーカーを非表示にする距離
        private const float MaxScaleDistance = 120.0f;  // マーカーを最大サイズにする距離

        private float _offsetPos = 0.0f;
        
        private Camera _camera;
        private Vector3 _playerScreenPos;
        
        /// <summary>
        /// 調整の基準とするスクリーン横幅
        /// </summary>
        private const float AdjustSpeedStandardScreenWidth = 1920.0f;
        
        /// <summary>
        /// スクリーン横幅による調整用
        /// </summary>
        private float _adjustScreenScale = 1.0f;
        
        private void Awake()
        {
            markerTransform.gameObject.SetActive(false);
        }

        private void Start()
        {
            _camera = Camera.main;
            // 画面サイズから速度調整用のスケールを設定
            _adjustScreenScale = Screen.width / AdjustSpeedStandardScreenWidth;
            // 位置オフセットを設定
            _offsetPos = OffsetStandardPos * _adjustScreenScale;
            // 最背面に設定
            transform.SetAsFirstSibling();
            // プレイヤーのスクリーン位置を取得
            var player = _camera.GetComponent<CameraCtrl>().Target;
            _playerScreenPos = _camera.WorldToScreenPoint(player.position);
        }

        private void LateUpdate()
        {
            // 中心(0,0)からある程度離れていれば非表示
            var centerDistance = Vector3.Distance(enemyCtrl.transform.position, Vector3.zero);
            if (centerDistance > HideDistance)
            {
                markerTransform.gameObject.SetActive(false);
                return;
            }
            
            // スクリーン座標を求める
            var screenPos = _camera.WorldToScreenPoint(gameObject.transform.position);

            // 画面に写っている場合は非表示
            if (0.0f <= screenPos.x && screenPos.x <= Screen.width &&
                0.0f <= screenPos.y && screenPos.y <= Screen.height &&
                screenPos.z >= 0.0f)
            {
                markerTransform.gameObject.SetActive(false);
                return;
            }

            // 位置を調整
            markerTransform.position = AdjustPosition(screenPos);

            // 向きを調整
            var pos = markerTransform.position;
            var angles = markerTransform.eulerAngles;
            markerTransform.eulerAngles = AdjustAngles(pos, angles);

            // 大きさを調整
            markerTransform.localScale = AdjustScale(centerDistance);
            
            // 表示する
            markerTransform.gameObject.SetActive(true);
        }

        /// <summary>
        /// 位置調整
        /// 画面外のマーカー位置を調整する
        /// </summary>
        /// <param name="pos">調整前の位置</param>
        /// <returns>調整後の位置</returns>
        private Vector3 AdjustPosition(Vector3 pos)
        {
            // 画面外のマーカー位置を画面内に収める
            if (pos.x > Screen.width) pos.x = Screen.width - _offsetPos;
            if (pos.x < 0.0f) pos.x = _offsetPos;
            if (pos.y > Screen.height) pos.y = Screen.height - _offsetPos;
            if (pos.y < 0.0f) pos.y = _offsetPos;

            // カメラ後方に写っている場合はX座標を反転させる
            if (pos.z < 0.0f)
            {
                pos.x = Screen.width - pos.x;
                pos.y = _offsetPos;
            }
            return pos;
        }

        /// <summary>
        /// 向き調整
        /// プレイヤーのスクリーン座標からの向きに調整する
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="angles">調整前の向き</param>
        /// <returns>調整後の向き</returns>
        private Vector3 AdjustAngles(Vector3 pos, Vector3 angles)
        {
            // プレイヤーのスクリーン座標からの向きに設定
            var lookVec = pos - _playerScreenPos;
            angles.z = (Mathf.Atan2(lookVec.y, lookVec.x) - Mathf.PI/2.0f) * Mathf.Rad2Deg; // atan2-90度
            return angles;
        }

        /// <summary>
        /// 大きさ調整
        /// 中心からの距離に応じて大きさを調整する
        /// </summary>
        /// <param name="distance">中心からの距離</param>
        /// <returns>調整後の大きさ</returns>
        private Vector3 AdjustScale(float distance)
        {
            // デフォルトは1.0
            var markerScale = 1.0f;
            if (distance >= MaxScaleDistance)
            {
                // 指定距離より離れてたら小さくする
                markerScale = 1.0f - (distance - MaxScaleDistance) / (HideDistance - MaxScaleDistance);
            }
            return markerScale * _adjustScreenScale * Vector3.one;
        }
    }
}
