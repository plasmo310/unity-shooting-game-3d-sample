using UnityEngine;
using Utils;
using Utils.Components;

namespace Services
{
    /// <summary>
    /// ローディング表示Service
    /// </summary>
    public class LoadingService : ILoadingService
    {
        /// <summary>
        /// ローディングCanvas
        /// </summary>
        private readonly GameObject _loadingCanvas;

        public LoadingService()
        {
            // ローディングCanvas生成
            var loadingCanvasPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.UILoadingCanvas);
            _loadingCanvas = ServiceLocator.Resolve<IMonoBehaviorService>().DoInstantiate(loadingCanvasPrefab);
            _loadingCanvas.AddComponent<DontDestroyObject>(); // DontDestroyにする
            _loadingCanvas.SetActive(false);
        }
        
        /// <summary>
        /// ローディング表示
        /// </summary>
        public void ShowLoading()
        {
            _loadingCanvas.SetActive(true);
        }
        
        /// <summary>
        /// ローディング非表示
        /// </summary>
        public void HideLoading()
        {
            _loadingCanvas.SetActive(false);
        }
    }
}
