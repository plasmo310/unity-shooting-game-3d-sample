using Services;
using UnityEngine;
using Utils;
using Utils.Components;

namespace Scenes.Game.Actor
{
    /// <summary>
    /// 宇宙船操作クラス
    /// </summary>
    public class ShipCtrl : MonoBehaviour
    {
        private GameObject _missilePrefab; // ミサイルPrefab
        private GameObject _beamPrefab;    // ビームPrefab
        private GameObject _effectBomb;    // 爆発エフェクト

        // ステート関連
        private enum StateType
        {
            Wait,   // 待機
            Active, // 操作可能
            Stop,   // 停止
        }
        private StateMachine<ShipCtrl> _stateMachine;

        private void Start()
        {
            _missilePrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.ActorNameMissile);
            _beamPrefab = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.ActorNameBeam);
            _effectBomb = ServiceLocator.Resolve<IAssetsService>().LoadAssets(GameConst.EffectNameBombGood);
            
            _stateMachine = new StateMachine<ShipCtrl>(this);
            _stateMachine.Add<StateWait>((int) StateType.Wait);
            _stateMachine.Add<StateActive>((int) StateType.Active);
            _stateMachine.Add<StateStop>((int) StateType.Stop);
            _stateMachine.OnStart((int) StateType.Wait);
        }

        private void Update()
        {
            _stateMachine.OnUpdate();
        }

        private bool _isEnemyHit; // エネミーヒット済か？
        private void OnTriggerEnter(Collider other)
        {
            // エネミーと衝突したら自身を破棄する
            if (other.transform.CompareTag(GameConst.TagNameEnemy))
            {
                if (_isEnemyHit) return;
                _isEnemyHit = true;
                
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeBomb); // 効果音再生
                Instantiate(_effectBomb, gameObject.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        private bool IsStateActive { set; get; } // アクティブステートか？
        private bool IsStateStop { set; get; }   // 停止ステートか？
        
        /// <summary>
        /// アクティブ状態に設定する
        /// </summary>
        public void StartStateActive()
        {
            IsStateActive = true;
        }
        
        /// <summary>
        /// 停止状態に設定する
        /// </summary>
        public void StartStateStop()
        {
            IsStateStop = true;
        }

        /// <summary>
        /// 外部からの入力用
        /// </summary>
        private bool _isShot = false;
        private bool _isBeam = false;
        private float _moveLeftVelocity = 0.0f;
        private float _moveRightVelocity = 0.0f;
        private float _moveUpVelocity = 0.0f;
        private float _moveDownVelocity = 0.0f;
        public void SetIsShot(bool value)
        {
            _isShot = value;
        }
        public void SetIsBeam(bool value)
        {
            _isBeam = value;
        }
        public void SetMoveLeftVelocity(float velocity)
        {
            _moveLeftVelocity = velocity;
        }
        public void SetMoveRightVelocity(float velocity)
        {
            _moveRightVelocity = velocity;
        }
        public void SetMoveUpVelocity(float velocity)
        {
            _moveUpVelocity = velocity;
        }
        public void SetMoveDownVelocity(float velocity)
        {
            _moveDownVelocity = velocity;
        }

        // 各ステート定義
        // ----- wait -----
        private class StateWait : StateMachine<ShipCtrl>.StateBase
        {
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // Sceneから受け取ったフラグからStateを変更する
                if (Owner.IsStateActive)
                {
                    Owner.IsStateActive = false;
                    StateMachine.ChangeState((int) StateType.Active);
                    return;
                }
            }
            public override void OnEnd() { }
        }
        
        // ----- active -----
        private class StateActive : StateMachine<ShipCtrl>.StateBase
        {
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // Sceneから受け取ったフラグからStateを変更する
                if (Owner.IsStateStop)
                {
                    Owner.IsStateStop = false;
                    ResetTransform();
                    StateMachine.ChangeState((int) StateType.Stop);
                    return;
                }
                
                UpdateTransform();   // 位置、回転情報の更新
                UpdateShotMissile(); // ミサイルショット処理
                UpdateShotBeam();    // ビームショット処理
            }
            public override void OnEnd() { }

            // 位置、回転情報のリセット
            private void ResetTransform()
            {
                // 角度をリセットする
                var angles = Owner.transform.localEulerAngles;
                angles.z = 0.0f;
                Owner.transform.localEulerAngles = angles;
            }
            
            // 移動関連
            // private const float RotYSpeedWeak = 30.0f; // Y軸の回転速度(小) 
            private const float RotYSpeed = 70.0f;     // Y軸の回転速度
            // private const float TiltRotWeak = 15.0f;   // 船の傾き角度(小)
            private const float TiltRot     = 30.0f;   // 船の傾き角度
            
            // 位置、回転情報の更新
            private void UpdateTransform()
            {
                var rotY = 0.0f; // Y軸の回転角度
                var tilt = 0.0f; // 傾き角度

                // 押下キーに応じて傾きと回転角度を設定
                if (Owner._moveLeftVelocity > 0.0f)
                {
                    rotY -= RotYSpeed * Owner._moveLeftVelocity;
                    tilt += TiltRot * Owner._moveLeftVelocity;
                }
                if (Owner._moveRightVelocity > 0.0f)
                {
                    rotY += RotYSpeed * Owner._moveRightVelocity;
                    tilt -= TiltRot * Owner._moveRightVelocity;
                }

                // ワールド座標でY軸回転
                Owner.transform.Rotate(rotY * Vector3.up * Time.deltaTime, Space.World);
                
                // ローカル座標で傾きを設定
                var angles = Owner.transform.localEulerAngles;
                angles.z = tilt;
                Owner.transform.localEulerAngles = angles;
            }
            
            // ショット関連
            private const float ShotRotVertical = 15.0f; // ショットを撃てる縦方向の角度
            private const float ShotOffsetPos = 3.0f; // ミサイル生成offset位置
            private const float CanShotTime = 0.15f;  // ミサイルが撃てるようになるまでの時間
            private float _shotTime = 0.0f;  // ミサイルを撃ってからの経過時間
            private bool _isCanShot = false; // ミサイルを撃てる状態か？

            // ミサイルショット処理
            private void UpdateShotMissile()
            {
                // 撃つ間隔を開ける
                if (!_isCanShot)
                {
                    _shotTime += Time.deltaTime;
                    if (_shotTime > CanShotTime)
                    {
                        _isCanShot = true;
                        _shotTime = 0.0f;
                    }
                }
                
                // ミサイル発射処理
                if (Owner._isShot && _isCanShot)
                {
                    // ショットを撃てなくする
                    _isCanShot = false;
                    
                    // 効果音再生
                    ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeShot); 

                    // 宇宙船のtransformを元に設定
                    var ownerTransform = Owner.transform;
                    var missile = Instantiate(Owner._missilePrefab);
                    missile.transform.position = ownerTransform.position + ShotOffsetPos * ownerTransform.forward;
                    
                    // 角度調整
                    var angles = ownerTransform.eulerAngles;
                    if (Owner._moveUpVelocity > 0.0f)
                    {
                        angles.x -= ShotRotVertical * Owner._moveUpVelocity;
                    }
                    if (Owner._moveDownVelocity > 0.0f)
                    {
                        angles.x += ShotRotVertical * Owner._moveDownVelocity;
                    }
                    missile.transform.eulerAngles = angles;
                }
            }
            
            // ビームショット処理
            private void UpdateShotBeam()
            {
                // ビームボタン押下時
                if (Owner._isBeam)
                {
                    Owner._isBeam = false;
                    ServiceLocator.Resolve<IAudioService>().PlayOneShot(GameConst.AudioNameSeBeam);
                    Instantiate(Owner._beamPrefab, Owner.transform);
                }
            }
        }
        
        // ----- stop -----
        private class StateStop : StateMachine<ShipCtrl>.StateBase
        {
            public override void OnStart() { }
            public override void OnUpdate() { }
            public override void OnEnd() { }
        }
    }
}
