using Scenes.Common;
using Scenes.Title.Presenter;
using Services;
using UnityEngine;
using Utils;

namespace Scenes.Title
{
    /// <summary>
    /// TitleScene管理クラス
    /// </summary>
    public class TitleSceneCtrl : MonoBehaviour
    {
        private TitlePresenter _presenter;
        
        private void Awake()
        {
            // 保持している音量を設定
            ServiceLocator.Resolve<IAudioService>().ChangeBgmVolume(ShootingPlayerPrefs.BgmVolume);
            ServiceLocator.Resolve<IAudioService>().ChangeSeVolume(ShootingPlayerPrefs.SeVolume);
            // BGM再生
            ServiceLocator.Resolve<IAudioService>().PlayBGM(GameConst.AudioNameSpaceWorld);
        }

        private void Start()
        {
            _presenter = new TitlePresenter();
            _presenter.OnStart();
        }

        private void Update()
        {
            _presenter.OnUpdate();
        }
    }
}
