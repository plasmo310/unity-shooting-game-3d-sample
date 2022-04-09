using UnityEngine;

namespace Utils
{
    /// <summary>
    /// RectTransform関連クラス
    /// </summary>
    public static class RectTransformUtil
    {
        /// <summary>
        /// 四角座標格納用
        /// </summary>
        private static readonly Vector3[] Corners = new Vector3[4];
    
        /// <summary>
        /// RectTransformの四角のスクリーン座標を取得
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3[] GetScreenCorners(this RectTransform self)
        {
            var canvas = self.GetComponentInParent<Canvas>();
            // ScreenSpaceCameraの場合にはcameraが設定されていなければならない
            // Overlayの場合はnullで渡す
            Camera camera = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                camera = canvas.worldCamera;
                if (camera == null)
                {
                    return null;
                }
            }
            return self.GetScreenCorners(camera);
        }
    
        /// <summary>
        /// RectTransformの四角のスクリーン座標を取得
        /// </summary>
        /// <param name="self"></param>
        /// <param name="camera"></param>
        /// <returns>スクリーン座標配列([0]: 左下, [1]: 左上, [2]: 右上, [3]: 右下)</returns>
        public static Vector3[] GetScreenCorners(this RectTransform self, Camera camera)
        {
            // RectTransformの四角の座標を取得
            self.GetWorldCorners(Corners);
        
            // スクリーン座標に変換して返却
            var screenCorners = new Vector3[4];
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(camera, Corners[0]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(camera, Corners[1]);
            screenCorners[2] = RectTransformUtility.WorldToScreenPoint(camera, Corners[2]);
            screenCorners[3] = RectTransformUtility.WorldToScreenPoint(camera, Corners[3]);
            return screenCorners;
        }
    }
}