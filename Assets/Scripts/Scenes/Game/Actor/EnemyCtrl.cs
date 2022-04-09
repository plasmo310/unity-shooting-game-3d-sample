using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Utils;
using Utils.Components;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Scenes.Game.Actor
{
    /// <summary>
    /// エネミー操作クラス
    /// </summary>
    public class EnemyCtrl : MonoBehaviour
    {
        private const float AppearDistance = 150.0f; // 出現時の距離
        public bool isDead = false; // 死亡しているか？

        // 以下はsetterで設定する
        private float _scale = 1.0f;  // 大きさ
        private float _speed = 20.0f; // 移動速度
        private Vector3 _shakeWidth = Vector3.zero;   // 揺れの量
        private Vector3 _initPosition = Vector3.zero; // 初期位置
        private float _waitTime = 0.0f;  // 待機時間

        // ステート関連
        private enum StateType
        {
            Wait,  // 待機
            Move,  // 移動
            Happy, // 喜び
        }
        private StateMachine<EnemyCtrl> _stateMachine;

        private void Start()
        {
            // 最初は非表示
            transform.localScale = Vector3.zero;
            
            // ステートマシン初期化して開始
            _stateMachine = new StateMachine<EnemyCtrl>(this);
            _stateMachine.Add<StateWait>((int) StateType.Wait);
            _stateMachine.Add<StateMove>((int) StateType.Move);
            _stateMachine.Add<StateHappy>((int) StateType.Happy);
            _stateMachine.OnStart((int) StateType.Wait);
        }
        
        private void Update()
        {
            _stateMachine.OnUpdate();
        }

        /// <summary>
        /// 破棄された際の通知用
        /// </summary>
        public event Action<Vector3, int> OnDestroyed;
        
        /// <summary>
        /// ミサイルヒット処理
        /// </summary>
        /// <param name="hitType">ヒットタイプ</param>
        public void MissileHit(int hitType)
        {
            // 効果音再生
            var seFileNameList = GetHitSeFileNameList(hitType);
            foreach (var seFileName in seFileNameList)
            {
                ServiceLocator.Resolve<IAudioService>().PlayOneShot(seFileName);
            }
            
            // エフェクト再生
            var effectFileName = GetEffectFileName(hitType);
            var effectBomb = ServiceLocator.Resolve<IAssetsService>().LoadAssets(effectFileName);
            Instantiate(effectBomb, gameObject.transform.position, Quaternion.identity);
            
            // 自身を破棄する
            OnDestroyed?.Invoke(gameObject.transform.position, hitType);
            Destroy(gameObject);
        }
        
        /// <summary>
        /// ヒット時の効果音ファイル名を返却する
        /// </summary>
        /// <param name="hitType">ヒットタイプ</param>
        /// <returns>効果音ファイル名</returns>
        private static List<string> GetHitSeFileNameList(int hitType)
        {
            var seNameList = new List<string>();
            switch (hitType)
            {
                case GameConst.EnemyHitTypePerfect:
                    seNameList.Add(GameConst.AudioNameSeBombBig);
                    seNameList.Add(GameConst.AudioNameSeLucky);
                    break;
                case GameConst.EnemyHitTypeGreat:
                    seNameList.Add(GameConst.AudioNameSeBombBig);
                    break;
                case GameConst.EnemyHitTypeGood:
                    seNameList.Add(GameConst.AudioNameSeBomb);
                    break;
                case GameConst.EnemyHitTypeBeam:
                    seNameList.Add(GameConst.AudioNameSeBombBig);
                    break;
                default:
                    Debug.LogError("not hit type: " + hitType);
                    break;
            }
            return seNameList;
        }

        /// <summary>
        /// ヒット時のエフェクトファイル名を返却する
        /// </summary>
        /// <param name="hitType">ヒットタイプ</param>
        /// <returns>エフェクトファイル名</returns>
        private static string GetEffectFileName(int hitType)
        {
            var effectName = "";
            switch (hitType)
            {
                case GameConst.EnemyHitTypePerfect:
                case GameConst.EnemyHitTypeBeam:
                    effectName = GameConst.EffectNameBombPerfect;
                    break;
                case GameConst.EnemyHitTypeGreat:
                    effectName = GameConst.EffectNameBombGreat;
                    break;
                case GameConst.EnemyHitTypeGood:
                    effectName = GameConst.EffectNameBombGood;
                    break;
                default:
                    Debug.LogError("not hit type: " + hitType);
                    break;
            }
            return effectName;
        }

        /// <summary>
        /// 指定角度の位置に設定する
        /// </summary>
        /// <param name="degree">
        /// 正面を0度とした場合の生成角度(例：30度の場合、斜め前に設定される)
        /// </param>
        public void SetInitPositionByDegree(float degree)
        {
            // 指定角度の離れた位置に生成
            // 0度指定時の位置をY軸クォータニオンで回転させる
            var zeroPos = AppearDistance * Vector3.forward;
            var qy = Quaternion.Euler(0.0f, degree, 0.0f);
            transform.position = qy * zeroPos;
            // (0, 0)の位置を向かせる
            transform.LookAt(Vector3.zero);
            // 初期位置を設定
            _initPosition = transform.position;
        }

        /// <summary>
        /// 大きさ設定
        /// </summary>
        /// <param name="scale">大きさ</param>
        public void SetScale(float scale)
        {
            _scale = scale;
        }
        
        /// <summary>
        /// 大きさ取得
        /// </summary>
        public float GetScale()
        {
            return _scale;
        }

        /// <summary>
        /// 移動情報設定
        /// </summary>
        /// <param name="speed">移動速度</param>
        /// <param name="shakeWidth">揺れの大きさ</param>
        public void SetMoveInfo(float speed, Vector3? shakeWidth = null)
        {
            _speed = speed;
            _shakeWidth = shakeWidth ?? Vector3.zero;
        }

        /// <summary>
        /// 待機時間設定
        /// </summary>
        /// <param name="time">待機時間</param>
        public void SetWaitTime(float time)
        {
            _waitTime = time;
        }

        // 各ステート定義
        // ----- wait -----
        private class StateWait : StateMachine<EnemyCtrl>.StateBase
        {
            public override void OnStart()
            {
                // 待機コルーチン開始
                var handler = ServiceLocator.Resolve<IMonoBehaviorService>();
                handler.DoStartCoroutine(WaitCoroutine());
            }

            public override void OnUpdate() { }

            public override void OnEnd() { }
            
            private IEnumerator WaitCoroutine()
            {
                // 待機時間待つ
                yield return new WaitForSeconds(Owner._waitTime);

                if (!Owner) yield break;
                
                // 大きさを設定して表示する
                Owner.transform.localScale = Vector3.one * Owner._scale;
                
                // 移動ステートへ遷移
                StateMachine.ChangeState((int) StateType.Move);
            }
        }
        
        // ----- move -----
        private class StateMove : StateMachine<EnemyCtrl>.StateBase
        {
            private const float ChangeHappyDistance = 2.0f; // 喜びステートへ遷移する距離
            private float _totalMove; // 移動量合計

            public override void OnStart()
            {
                _totalMove = 0.0f;
            }

            public override void OnUpdate()
            {
                var ownerTransform = Owner.transform;
                
                // 中心(0, 0, 0)に近づいたら喜びステートへ遷移
                if (Vector3.Distance(ownerTransform.position, Vector3.zero) <= ChangeHappyDistance)
                {
                    StateMachine.ChangeState((int) StateType.Happy);
                    return;
                }
                
                // 出現時の距離分進む
                var speed = Owner._speed;
                if (_totalMove < AppearDistance)
                {
                    _totalMove += (speed * ownerTransform.forward * Time.deltaTime).magnitude;
                }
            
                // 揺れの量を求める
                var shakeWidth = Owner._shakeWidth;
                var shakeVec = shakeWidth.x * ownerTransform.right + shakeWidth.y * Vector3.up;
                shakeVec = Mathf.Sin(Time.frameCount / 10.0f) * shakeVec;
            
                // 位置の更新
                var initPosition = Owner._initPosition;
                ownerTransform.position = initPosition + (_totalMove * ownerTransform.forward) + shakeVec;
            }

            public override void OnEnd() { }
        }
        
        // ----- happy -----
        private class StateHappy : StateMachine<EnemyCtrl>.StateBase
        {
            private const float HappyRotSpeed = 1200.0f; // 喜びの舞の回転速度
            
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // くるくる周る
                Owner.transform.Rotate(HappyRotSpeed * Time.deltaTime * Vector3.up);
            }

            public override void OnEnd() { }
        }
    }
}