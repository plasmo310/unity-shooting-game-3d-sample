using UnityEngine;
using Utils.Components;
using Random = UnityEngine.Random;

namespace Scenes.Game.Actor
{
    /// <summary>
    /// カメラ操作クラス
    /// </summary>
    public class CameraCtrl : MonoBehaviour
    {
        [SerializeField] private ShakeEffect ShakeEffect;
        
        public Transform Target { set; get; } // ターゲット
        public bool IsAppearAnimation { private set; get; } // 登場アニメーション中か？
        public bool IsClearState { private set; get; }  // クリア状態か？
        private Vector3 _clearTargetEnemyPos; // クリア時に合わせる敵の位置

        // ステート関連
        private enum StateType
        {
            Normal,        // 通常
            AppearAnimate, // 登場アニメーション
            Clear,         // クリア時
        }
        private StateMachine<CameraCtrl> _stateMachine;

        private void Start()
        {
            _stateMachine = new StateMachine<CameraCtrl>(this);
            _stateMachine.Add<StateNormal>((int) StateType.Normal);
            _stateMachine.Add<StateAppearAnimate>((int) StateType.AppearAnimate);
            _stateMachine.Add<StateClear>((int) StateType.Clear);
            _stateMachine.OnStart((int) StateType.Normal);
        }

        /// <summary>
        /// ターゲット位置を元に更新するため
        /// LateUpdateを使用
        /// </summary>
        private void LateUpdate()
        {
            _stateMachine.OnUpdate();
        }

        /// <summary>
        /// 登場アニメーション開始
        /// </summary>
        public void StartAppearAnimation()
        {
            // アニメーション開始
            IsAppearAnimation = true;
        }
        
        /// <summary>
        /// クリア状態開始
        /// </summary>
        /// <param name="enemyPos">敵の位置</param>
        public void StartClearState(Vector3 enemyPos)
        {
            IsClearState = true;
            _clearTargetEnemyPos = enemyPos;
        }
        
        /// <summary>
        /// クリア状態終了
        /// </summary>
        public void EndClearState()
        {
            IsClearState = false;
            _clearTargetEnemyPos = Vector3.zero;
        }

        /// <summary>
        /// カメラを揺らす
        /// </summary>
        public void ShakeCamera()
        {
            ShakeEffect.SetShakeInitPosition(gameObject.transform.position);
            ShakeEffect.StartShake(0.3f, 5.0f, 100.0f);
        }

        // 各ステート定義
        // ----- normal ------
        private class StateNormal : StateMachine<CameraCtrl>.StateBase
        {
            private static readonly Vector3 OffsetCameraPos = new Vector3(0.0f, 5.0f, 10.0f); // offsetカメラ位置
            private static readonly Vector3 OffsetLookPos = new Vector3(0.0f, 3.0f, 0.0f);    // offset視点位置
            
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // ターゲットが設定されていなければ処理終了
                if (!Owner.Target) return;

                // 登場アニメーションフラグが設定されていれば開始
                if (Owner.IsAppearAnimation)
                {
                    StateMachine.ChangeState((int) StateType.AppearAnimate);
                    return;
                }

                // クリア状態フラグが設定されていれば開始
                if (Owner.IsClearState)
                {
                    StateMachine.ChangeState((int) StateType.Clear);
                    return;
                }
            
                // ターゲット位置の取得
                var targetPos = Owner.Target.position;
                // カメラ位置をターゲット位置からoffset分ずらす
                var position = targetPos;
                position -= OffsetCameraPos.z * Owner.Target.forward;
                position += OffsetCameraPos.y * Vector3.up;
                Owner.transform.position = position;
                // ターゲットの方向を向かせる
                Owner.transform.LookAt(targetPos + OffsetLookPos);
            }
            
            public override void OnEnd() { }
        }
        
        // ----- appear animate ------
        private class StateAppearAnimate : StateMachine<CameraCtrl>.StateBase
        {
            private const float AnimTime = 3.5f; // アニメーション時間
            private float _totalAnimTime; // アニメーション時間合計

            private const float AnimOffsetInitDistance = 6.5f; // 初期状態の距離
            private const float AnimOffsetLookForward = 4.5f;   // 注視点オフセット
            private static readonly Vector3 AnimLookVec = new Vector3(-0.8f, -1.2f, 0.85f); // 注視方向

            public override void OnStart()
            {
                // 初期位置を設定
                var position = AnimOffsetInitDistance * AnimLookVec;
                Owner.transform.position = position;
                
                // アニメーション時間初期化
                _totalAnimTime = 0.0f;
            }

            public override void OnUpdate()
            {
                // 位置更新
                Owner.transform.position += AnimLookVec * Time.deltaTime;
                
                // 注視点をターゲットの前方に設定
                var targetPos = Owner.Target.transform.position;
                var offsetLookPos = AnimOffsetLookForward * Owner.transform.forward;
                Owner.transform.LookAt(targetPos + offsetLookPos);

                // アニメーション時間を過ぎたら通常に戻る
                _totalAnimTime += Time.deltaTime;
                if (_totalAnimTime >= AnimTime)
                {
                    StateMachine.ChangeState((int) StateType.Normal);
                }
            }

            public override void OnEnd()
            {
                // アニメーション終了
                Owner.IsAppearAnimation = false;
            }
        }
        
        // ----- clear animation ------
        private class StateClear : StateMachine<CameraCtrl>.StateBase
        {
            // 位置情報
            private static readonly Vector3 OffsetPos = new Vector3(5.0f, 5.0f, 10.0f);
            private Vector3 _smoothTargetPos;
            private const float SmoothSpeed = 5.0f;
            private const float InitPosDistance = 5.0f;
            
            // 揺れ情報
            private const float ShakeStrength = 1.0f;
            private const float ShakeDuration = 0.5f;
            
            private float _totalTime = 0.0f;

            public override void OnStart()
            {
                _totalTime = 0.0f;
                // ターゲット位置を設定
                var forwardVec = Owner._clearTargetEnemyPos.normalized;
                var rightVec = Quaternion.Euler(0.0f, -90.0f, 0.0f) * forwardVec;
                var targetPos = Owner._clearTargetEnemyPos;
                targetPos += forwardVec * OffsetPos.z;
                targetPos += rightVec * OffsetPos.x;
                targetPos += Vector3.up * OffsetPos.y;
                _smoothTargetPos = targetPos;
                // 初期位置を設定
                Owner.transform.position = targetPos.normalized * InitPosDistance;
                Owner.transform.LookAt(Vector3.zero);
            }

            public override void OnUpdate()
            {
                // 終了通知を受け取ったら通常に戻る
                if (!Owner.IsClearState)
                {
                    StateMachine.ChangeState((int) StateType.Normal);
                }
                
                // ターゲット位置に着いたら何もしない
                if (Vector3.Distance(Owner.transform.position, _smoothTargetPos) < 0.5f)
                {
                    return;
                }
                
                // 臨場感出すため少し揺らす
                var ratio = Mathf.Clamp(1.0f - _totalTime / ShakeDuration, 0.0f, 1.0f);
                var randomOffsetX = Random.Range(-ShakeStrength, ShakeStrength);
                var randomOffsetY = Random.Range(-ShakeStrength, ShakeStrength);
                Owner.transform.position += randomOffsetX * ratio * Owner.transform.right;
                Owner.transform.position += randomOffsetY * ratio * Owner.transform.up;
                
                // ターゲット位置に向かって移動
                Owner.transform.position = Vector3.MoveTowards(Owner.transform.position, _smoothTargetPos, SmoothSpeed);
                Owner.transform.LookAt(Vector3.zero);
            }

            public override void OnEnd() { }
        }
    }
}
