using UnityEngine;

namespace Services
{
    /// <summary>
    /// ローディング表示Service
    /// </summary>
    public interface ILoadingService
    {
        public void ShowLoading();
        public void HideLoading();
    }
}
