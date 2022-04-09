using UnityEngine;

namespace Services
{
    /// <summary>
    /// Assets関連管理クラス
    /// </summary>
    public interface IAssetsService
    {
        /// <summary>
        /// Assetsファイルの読み込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>Asset(GameObject)</returns>
        public GameObject LoadAssets(string fileName);
    }
}
