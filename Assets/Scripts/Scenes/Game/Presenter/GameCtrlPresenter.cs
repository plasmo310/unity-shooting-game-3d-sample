using Scenes.Common.Model;
using Scenes.Game.Actor;
using Scenes.Game.View;
using Services;
using UnityEngine;
using Utils;

namespace Scenes.Game.Presenter
{
    /// <summary>
    /// GameCtrlCanvas操作クラス
    /// </summary>
    public class GameCtrlPresenter
    {
        private readonly GameCtrlCanvas _view;
        private readonly StickMoveModel _moveStickMoveModel;
        private readonly StickMoveModel _shotStickMoveModel;
        
        /// <summary>
        /// プレイヤー
        /// </summary>
        private readonly ShipCtrl _player;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public GameCtrlPresenter(ShipCtrl player)
        {
            var canvasPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.UIGameCtrlCanvas);
            _view = ServiceLocator.Resolve<IMonoBehaviorService>().DoInstantiate(canvasPrefab)?.GetComponent<GameCtrlCanvas>();
            _player = player;
            _moveStickMoveModel = new StickMoveModel();
            _shotStickMoveModel = new StickMoveModel();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            // タッチ用Canvasを非表示にする
            _view.SetActive(false);
        }
        
        /// <summary>
        /// Canvasの表示
        /// </summary>
        public void ShowTouchCanvas()
        {
            _view.SetActive(true);
            
            // タッチ用Canvasを初期化
            _view.ChangeMoveStickSprite(false, false, false, false);
            _view.AddListenerPointerDownMoveStick(PushDownMoveStickButton);
            _view.AddListenerPointerUpMoveStick(PushUpMoveStickButton);
            _view.AddListenerPointerDownShotButton(PushDownShotStickButton);
            _view.AddListenerPointerUpShotButton(PushUpShotStickButton);
            _view.AddListenerPointerDownBeamButton(PushDownBeamButton);
            
            // ショット矢印を非表示
            _view.SetActiveShotUpArrowImage(false);
            _view.SetActiveShotDownArrowImage(false);
            
            // ビームゲージのリセット
            ResetBeamGauge();
        }

        /// <summary>
        /// Canvasの非表示
        /// </summary>
        public void HideTouchCanvas()
        {
            // タッチ用Canvasを破棄
            _view.SetActive(false);
        }

        /// <summary>
        /// ビームゲージ追加
        /// </summary>
        /// <param name="value">追加する値</param>
        public void AddBeamGauge(float value)
        {
            var beamGauge = _view.GetBeamGaugeValue();
            beamGauge = Mathf.Clamp(beamGauge + value, 0.0f, 1.0f);
            _view.SetBeamGaugeValue(beamGauge);
            if (IsMaxBeamGauge())
            {
                _view.SetBeamButtonImage(true);
            }
        }

        /// <summary>
        /// ビームゲージリセット
        /// </summary>
        private void ResetBeamGauge()
        {
            _view.SetBeamGaugeValue(0.0f);
            _view.SetBeamButtonImage(false);
        }

        /// <summary>
        /// ビームゲージが最大か？
        /// </summary>
        private bool IsMaxBeamGauge()
        {
            var beamGauge = _view.GetBeamGaugeValue();
            return Mathf.Approximately(beamGauge, 1.0f);
        }
        
        // ショットキーを押下していたか？
        private static bool _isPrevPushShotKey = false;

        /// <summary>
        /// ゲーム画面キー入力の有効化
        /// </summary>
        public void OnRegisterGameInputKey()
        {
            // ショット
            ServiceLocator.Resolve<IInputKeyService>().RegisterShotKey(
                IInputKeyService.KeyActionType.KeyPressing,
                (isPush) =>
                {
                    // 変化があった場合、ショット状態を変更する
                    if (isPush != _isPrevPushShotKey)
                    {
                        _player.SetIsShot(isPush);
                        _view.ChangeSpriteShotButton(isPush);
                    }
                    // 押下した内容を保持
                    _isPrevPushShotKey = isPush;
                });
            // ビーム
            ServiceLocator.Resolve<IInputKeyService>().RegisterBeamKey(
                IInputKeyService.KeyActionType.KeyDown,
                PushDownBeamButton);
            // 方向キー入力
            ServiceLocator.Resolve<IInputKeyService>().RegisterLeftKey(
                IInputKeyService.KeyActionType.KeyPressing,
                (isPush) => _isPushLeftKey = isPush);
            ServiceLocator.Resolve<IInputKeyService>().RegisterRightKey(
                IInputKeyService.KeyActionType.KeyPressing,
                (isPush) => _isPushRightKey = isPush);
            ServiceLocator.Resolve<IInputKeyService>().RegisterUpKey(
                IInputKeyService.KeyActionType.KeyPressing,
                (isPush) => _isPushUpKey = isPush);
            ServiceLocator.Resolve<IInputKeyService>().RegisterDownKey(
                IInputKeyService.KeyActionType.KeyPressing,
                (isPush) => _isPushDownKey = isPush);
        }
        
        /// <summary>
        /// ゲーム画面キー入力の無効化
        /// </summary>
        public void OnUnRegisterGameInputKey()
        {
            // 登録したキー入力処理をクリアする
            ServiceLocator.Resolve<IInputKeyService>().UnRegisterShotKey();
            ServiceLocator.Resolve<IInputKeyService>().UnRegisterBeamKey();
            ServiceLocator.Resolve<IInputKeyService>().UnRegisterLeftKey();
            ServiceLocator.Resolve<IInputKeyService>().UnRegisterRightKey();
            ServiceLocator.Resolve<IInputKeyService>().UnRegisterUpKey();
            ServiceLocator.Resolve<IInputKeyService>().UnRegisterDownKey();
        }
        
        // --------------- タッチ処理用定義 ---------------
        // 移動スティック関連
        private const float MoveStickMoveMax = 60.0f;
        private const float MoveStickMoveMin = 10.0f;
        
        // ショットスティック関連
        private const float ShotStickMoveMax = 60.0f;
        private const float ShotStickMoveMin = 10.0f;

        // ビーム関連
        private const float CanBeamTime = 10.0f; // ミサイルが撃てるようになるまでの時間

        /// <summary>
        /// ゲーム更新処理
        /// </summary>
        public void OnGameUpdate()
        {
            // 移動スティックを更新
            UpdateMoveStick();
            
            // ショットスティックを更新
            UpdateShotStick();
            
            // プレイヤーの移動情報を更新
            _player.SetMoveRightVelocity(0.0f);
            _player.SetMoveLeftVelocity(0.0f);
            if (_isPushRightKey)
            {
                _player.SetMoveRightVelocity(1.0f);
            }
            if (_isPushLeftKey)
            {
                _player.SetMoveLeftVelocity(1.0f);
            }
            if (_moveStickMoveModel.StickDragDiffPosition.x > MoveStickMoveMin)
            {
                var dragDiffPosition = Mathf.Abs(_moveStickMoveModel.StickDragDiffPosition.x);
                _player.SetMoveRightVelocity((dragDiffPosition - MoveStickMoveMin) / MoveStickMoveMax);
            }
            if (_moveStickMoveModel.StickDragDiffPosition.x < -MoveStickMoveMin)
            {
                var dragDiffPosition = Mathf.Abs(_moveStickMoveModel.StickDragDiffPosition.x);
                _player.SetMoveLeftVelocity((dragDiffPosition - MoveStickMoveMin) / MoveStickMoveMax);
            }

            // ショットの移動方向を更新
            _player.SetMoveUpVelocity(0.0f);
            _player.SetMoveDownVelocity(0.0f);
            if (_isPushUpKey)
            {
                _player.SetMoveUpVelocity(1.0f);
            }
            if (_isPushDownKey)
            {
                _player.SetMoveDownVelocity(1.0f);
            }
            if (_shotStickMoveModel.StickDragDiffPosition.y > ShotStickMoveMin)
            {
                var dragDiffPosition = Mathf.Abs(_shotStickMoveModel.StickDragDiffPosition.y);
                _player.SetMoveUpVelocity((dragDiffPosition - ShotStickMoveMin) / ShotStickMoveMax);
            }
            if (_shotStickMoveModel.StickDragDiffPosition.y < -ShotStickMoveMin)
            {
                var dragDiffPosition = Mathf.Abs(_shotStickMoveModel.StickDragDiffPosition.y);
                _player.SetMoveDownVelocity((dragDiffPosition - ShotStickMoveMin) / ShotStickMoveMax);
            }

            // ビームゲージを更新
            AddBeamGauge(Time.deltaTime / CanBeamTime);
        }
        
        // 方向キーを押下しているか？
        private bool _isPushLeftKey = false;
        private bool _isPushRightKey = false;
        private bool _isPushUpKey = false;
        private bool _isPushDownKey = false;

        /// <summary>
        /// 移動スティック更新処理
        /// </summary>
        private void UpdateMoveStick()
        {
            // スティック情報を更新
            _moveStickMoveModel.OnUpdateStick();

            // スティックボタン画像を変える
            var isInputLeftStick = _moveStickMoveModel.StickDragDiffPosition.x < -MoveStickMoveMax;
            var isInputRightStick = _moveStickMoveModel.StickDragDiffPosition.x > MoveStickMoveMax;
            var isInputLeftStickWeak = _moveStickMoveModel.StickDragDiffPosition.x < -MoveStickMoveMin;
            var isInputRightStickWeak = _moveStickMoveModel.StickDragDiffPosition.x > MoveStickMoveMin;
            if (_view.ChangeMoveStickSprite(isInputLeftStick || _isPushLeftKey, isInputRightStick || _isPushRightKey, 
                    isInputLeftStickWeak, isInputRightStickWeak))
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeMachine);
            }
        }

        /// <summary>
        /// ショットスティック更新処理
        /// </summary>
        private void UpdateShotStick()
        {
            // スティック情報を更新
            _shotStickMoveModel.OnUpdateStick();
            
            // ショット矢印を表示
            var isActiveShotDownArrow = _shotStickMoveModel.StickDragDiffPosition.y < -ShotStickMoveMin;
            var isActiveShotUpArrow = _shotStickMoveModel.StickDragDiffPosition.y > ShotStickMoveMin;
            _view.SetActiveShotDownArrowImage(isActiveShotDownArrow || _isPushDownKey);
            _view.SetActiveShotUpArrowImage(isActiveShotUpArrow || _isPushUpKey);
        }
        
        // --------------- ボタン押下処理 ---------------
        
        /// <summary>
        /// 移動スティックを押下した時
        /// </summary>
        private void PushDownMoveStickButton()
        {
            _moveStickMoveModel.PushDownStick();
        }

        /// <summary>
        /// 移動スティックを離した時
        /// </summary>
        private void PushUpMoveStickButton()
        {
            _moveStickMoveModel.PushUpStick();
        }

        /// <summary>
        /// ショット(スティック)ボタンを押下した時
        /// </summary>
        private void PushDownShotStickButton()
        {
            _player.SetIsShot(true);
            _view.ChangeSpriteShotButton(true);
            if (!_shotStickMoveModel.IsPushStick)
            {
                _shotStickMoveModel.PushDownStick();
            }
        }
        
        /// <summary>
        /// ショット(スティック)ボタンを離した時
        /// </summary>
        private void PushUpShotStickButton()
        {
            _player.SetIsShot(false);
            _view.ChangeSpriteShotButton(false);
            _shotStickMoveModel.PushUpStick();
        }

        /// <summary>
        /// ビームボタンを押下した時
        /// </summary>
        private void PushDownBeamButton()
        {
            // ゲージがMAXまで溜まっていたらビームを放つ
            if (IsMaxBeamGauge())
            {
                _player.SetIsBeam(true);
                // ビームゲージをリセット
                ResetBeamGauge();
            }
        }
    }
}
