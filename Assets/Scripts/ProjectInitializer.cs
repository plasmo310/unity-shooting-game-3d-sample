using Services;
using UnityEngine;
using Utils;
using Utils.Components;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

/// <summary>
/// プロジェクト初期化クラス
/// </summary>
public static class ProjectInitializer
{
    /// <summary>
    /// 初期化処理
    /// シーンのロード前に呼ばれる
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // 30FPSに指定
        Application.targetFrameRate = 30;
        
        // モバイルプラットフォームか？
        // テスト用でEditorもモバイルに含めることも可能
        if (Application.isMobilePlatform)
        {
            Scenes.Common.ShootingGameState.IsMobilePlatform = true;
        }
        
#if UNITY_IOS
        // ATTトラッキング表示
        if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == 
           ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
        // サービス登録
        var serviceLocator = new GameObject("ServiceLocator");
        serviceLocator.AddComponent<DontDestroyObject>();
        ServiceLocator.Register<IAudioService>(serviceLocator.AddComponent<AudioService>());
        ServiceLocator.Register<IAssetsService>(serviceLocator.AddComponent<AssetsService>());
        ServiceLocator.Register<IMonoBehaviorService>(serviceLocator.AddComponent<MonoBehaviorService>());
        ServiceLocator.Register<IDataAccessService>(new DataAccessJsonService());
        ServiceLocator.Register<IPlayerPrefsService>(new PlayerPrefsService());
        // モバイルの場合、Admobを適用し入力検知を無効にする
        if (Scenes.Common.ShootingGameState.IsMobilePlatform)
        {
            ServiceLocator.Register<IAdmobService>(serviceLocator.AddComponent<AdmobDummyService>()); // SampleのためDummyを設定
            ServiceLocator.Register<IInputKeyService>(serviceLocator.AddComponent<InputKeyDummyService>());
        }
        else
        {
            ServiceLocator.Register<IAdmobService>(serviceLocator.AddComponent<AdmobDummyService>());
            ServiceLocator.Register<IInputKeyService>(serviceLocator.AddComponent<InputKeyService>());
        }
        // 下記は内部でServiceを呼んでいるため最後に呼ぶ
        ServiceLocator.Register<IRankingService>(new RankingDummyService()); // SampleのためDummyを設定
        ServiceLocator.Register<ILoadingService>(new LoadingService());
        ServiceLocator.Register<IDialogService>(new DialogService());

        // ゲーム開始
        GameStart();
    }
    
    /// <summary>
    /// ゲーム開始
    /// </summary>
    private static void GameStart()
    {
        // ログイン処理
        // ※PlayFabで使用していたが現在は使用していない
        ServiceLocator.Resolve<ILoadingService>().ShowLoading();
        Scenes.Common.ShootingGameState.IsGameReady = false;
        ServiceLocator.Resolve<IRankingService>().Login((result) =>
        {
            // エラーが発生した場合、メッセージを表示してアプリ終了
            if (!result)
            {
                ServiceLocator.Resolve<IDialogService>().ShowNormalDialog(
                    "接続エラーが発生しました。\r\nアプリを終了します。",
                    () =>
                    {
                        ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeDecide);
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                                Application.Quit();
#endif
                    });
                return;
            }
            
            // 準備完了
            ServiceLocator.Resolve<ILoadingService>().HideLoading();
            Scenes.Common.ShootingGameState.IsGameReady = true;
        });
    }
}
