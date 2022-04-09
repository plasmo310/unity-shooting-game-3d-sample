using UnityEngine;
using Utils.Components;

namespace Scenes.Common.Actor
{
    /// <summary>
    /// Skybox操作クラス
    /// ２つの球体を回転させてSkyBoxを再現
    /// </summary>
    public class SkyboxCtrl : SingletonMonoBehavior<SkyboxCtrl>
    {
        [SerializeField] private GameObject bgBack;
        [SerializeField] private GameObject bgFront;
        private const float BackRotSpeed = 5.0f;
        private const float FrontRotSpeed = 7.5f;
        
        private void Update()
        {
            // ２つの球体を回転
            bgBack.transform.Rotate(
                BackRotSpeed * Vector3.up * Time.deltaTime,
                Space.World);
            bgFront.transform.Rotate(
                FrontRotSpeed * Vector3.up * Time.deltaTime,
                Space.World);
        }
    }
}
