using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Assets関連管理クラス
    /// </summary>
    public class AssetsService : MonoBehaviour, IAssetsService
    {
        /// <summary>
        /// Assetsファイル格納パス
        /// </summary>
        private const string AssetsPath = "Assets/";

        /// <summary>
        /// キャッシュしたAssets
        /// key: ファイル名
        /// </summary>
        private readonly IDictionary<string, GameObject> _cachedAssetsDictionary = new Dictionary<string, GameObject>();

        /// <summary>
        /// Assetsファイルの読み込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>Asset(GameObject)</returns>
        public GameObject LoadAssets(string fileName)
        {
            // ファイル名をキーとしてキャッシュする
            if (!_cachedAssetsDictionary.ContainsKey(fileName))
            {
                var asset = Resources.Load(AssetsPath + fileName) as GameObject;
                _cachedAssetsDictionary.Add(fileName, asset);
            }
            return _cachedAssetsDictionary[fileName];
        }
    }
}
