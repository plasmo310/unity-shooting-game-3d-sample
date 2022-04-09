using Services;
using UnityEngine;
using Utils;

namespace Scenes.Title.View
{
    /// <summary>
    /// タイトル画面のカニ操作クラス
    /// </summary>
    public class KaniBoundCtrl : MonoBehaviour
    {
        private RectTransform _rectTransform;
        
        /// <summary>
        /// 速度
        /// </summary>
        private float _speed = 100.0f;
        
        /// <summary>
        /// 速度調整の基準とする横幅
        /// </summary>
        private const float AdjustSpeedStandardScreenWidth = 1920.0f;
        
        /// <summary>
        /// 速度調整用
        /// </summary>
        private float _adjustSpeedScale = 1.0f;
        
        /// <summary>
        /// 移動方向
        /// </summary>
        private Vector3 _moveVec = new Vector3(0.5f, 0.5f);
        
        /// <summary>
        /// 表示されている？
        /// </summary>
        private bool _isShow = false;
        
        /// <summary>
        /// 表示する
        /// </summary>
        public void Show()
        {
            // 方向をランダムに設定
            var random = Random.Range(-90.0f, 90.0f);
            _moveVec = Quaternion.Euler(0.0f, 0.0f, random) * _moveVec;
            
            // 表示する
            gameObject.SetActive(true);
            _isShow = true;
        }

        private void Awake()
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            // 画面サイズから速度調整用のスケールを設定
            _adjustSpeedScale = Screen.width / AdjustSpeedStandardScreenWidth;
            // 初めは非表示にする
            gameObject.SetActive(false);
        }

        void Update()
        {
            // 表示されるまで処理しない
            if (!_isShow) return;
            
            // 速度を丸める
            _speed = Mathf.Clamp(_speed, 20.0f, 100.0f);
            
            // 移動させる
            transform.position += _speed * _adjustSpeedScale * _moveVec;
            
            // 減速させる
            _speed *= 0.95f;
            
            // スクリーンの端を壁として反射させる
            ReflectScreenWall();
        }
        
        /// <summary>
        /// スクリーンの端を壁として反射させる
        /// </summary>
        private void ReflectScreenWall()
        {
            // 四隅のスクリーン座標を取得
            var screenCorners = _rectTransform.GetScreenCorners(null);
            if (screenCorners == null || screenCorners.Length != 4)
            {
                return;
            }
            
            // スクリーンをはみ出しているかチェック
            var isOverMinX = false;
            var isOverMaxX = false;
            var isOverMinY = false;
            var isOverMaxY = false;
            foreach (var screenCorner in screenCorners)
            {
                isOverMinX = isOverMinX || screenCorner.x <= 0;
                isOverMaxX = isOverMaxX || Screen.width <= screenCorner.x;
                isOverMinY = isOverMinY || screenCorner.y <= 0;
                isOverMaxY = isOverMaxY || Screen.height <= screenCorner.y;
            }
            
            // 超えていた場合は反射させる
            if (isOverMinX || isOverMaxX || isOverMinY || isOverMaxY)
            {
                if ((isOverMinX && _moveVec.x < 0) || (isOverMaxX && _moveVec.x > 0))
                {
                    _moveVec.x *= -1.0f;
                }
                if ((isOverMinY && _moveVec.y < 0) || (isOverMaxY && _moveVec.y > 0))
                {
                    _moveVec.y *= -1.0f;
                }
                
                // 加速させる
                _speed *= 3.0f;
            }
        }

        /// <summary>
        /// クリックされた時の処理
        /// </summary>
        public void OnPointerDown()
        {
            ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeClick);
            
            // 加速させる
            _speed *= 5.0f;
            
            // 少し向きを変える
            var random = Random.Range(-90.0f, 90.0f);
            _moveVec = Quaternion.Euler(0.0f, 0.0f, random) * _moveVec;
        }
    }
}
