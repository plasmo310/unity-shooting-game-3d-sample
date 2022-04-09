using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Scenes.Common.View
{
    /// <summary>
    /// 選択用フレーム
    /// 動的に変形できる
    /// </summary>
    public class SelectFrameMesh : Graphic
    {
        /// <summary>
        /// 基本情報
        /// </summary>
        [SerializeField] private float frameHeight = 200.0f;
        [SerializeField] private float frameWidth = 200.0f;
        [SerializeField] private Color lineColor = Color.white;
    
        /// <summary>
        /// 頂点生成情報
        /// </summary>
        [Serializable]
        private struct VertexGenerateInfo
        {
            public Vector2 outerOffset; // 外枠offset位置
            public Vector2 innerOffset; // 内枠offset位置
            public Vector2 vertexVec;   // 頂点の向き
            public float randomValue;   // ランダムに動かす大きさ
        }
        [SerializeField] private VertexGenerateInfo[] vertexGenerateInfos = new VertexGenerateInfo[]
        {
            new VertexGenerateInfo()
            {
                outerOffset = new Vector2(40.0f, -40.0f),
                innerOffset = new Vector2(20.0f, -20.0f),
                vertexVec = new Vector2(1.0f, -1.0f)
            },
            new VertexGenerateInfo()
            {
                outerOffset = new Vector2(40.0f, 40.0f),
                innerOffset = new Vector2(20.0f, 20.0f),
                vertexVec = new Vector2(1.0f, 1.0f)
            },
            new VertexGenerateInfo()
            {
                outerOffset = new Vector2(-40.0f, 40.0f),
                innerOffset = new Vector2(-20.0f, 20.0f),
                vertexVec = new Vector2(-1.0f, 1.0f)
            },
            new VertexGenerateInfo()
            {
                outerOffset = new Vector2(-40.0f, -40.0f),
                innerOffset = new Vector2(-20.0f, -20.0f),
                vertexVec = new Vector2(-1.0f, -1.0f)
            },
        };
        [SerializeField] private bool isClosed = true;
    
        /// <summary>
        /// 内枠、外枠の頂点ペア
        /// </summary>
        private struct VertexPair
        {
            public Vector2 OuterPos; // 外枠座標t位置
            public Vector2 InnerPos; // 内枠座標位置
        }
        private VertexPair[] _vertexPairs;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (vertexGenerateInfos.Length != 4)
            {
                Debug.LogError("４つ設定しておくんなまし");
                return;
            }

            // キャッシュクリア
            vh.Clear();
        
            // 生成情報から頂点を作成
            _vertexPairs = GenerateVertexPairs(vertexGenerateInfos, frameWidth, frameHeight);

            // 頂点の設定
            for (var i = 0; i < _vertexPairs.Length; i++)
            {
                // 適当に位置をずらす
                var randomValue = vertexGenerateInfos[i].randomValue;
                var randomOffset = new Vector2(Random.Range(-randomValue, randomValue), Random.Range(-randomValue, randomValue));
                var vertexPair = _vertexPairs[i];
                var pos = vertexPair.OuterPos;
                vh.AddVert(new UIVertex
                {
                    position = pos + randomOffset,
                    color = lineColor
                });

                pos = vertexPair.InnerPos;
                vh.AddVert(new UIVertex
                {
                    position = pos + randomOffset,
                    color = lineColor
                });
            }

            // 枠線を描画
            for (var i = 0; i < _vertexPairs.Length - 1; i++)
            {
                vh.AddTriangle(i*2,   i*2+1, i*2+2);
                vh.AddTriangle(i*2+1, i*2+2, i*2+3);
            }
            if (isClosed)
            {
                var i = _vertexPairs.Length - 1;
                vh.AddTriangle(0, 1, i*2);
                vh.AddTriangle(1, i*2+1, i*2);
            }
        }
    
        /// <summary>
        /// 内枠、外枠の頂点ペアを作成して返却
        /// </summary>
        private VertexPair[] GenerateVertexPairs(VertexGenerateInfo[] generateInfos, float width, float height)
        {
            var pairs = new VertexPair[generateInfos.Length];
            for (var i = 0; i < pairs.Length; i++)
            {
                // デフォルトの幅と頂点のベクトルから基準となる位置を設定
                var standardPos = new Vector2
                {
                    x = width / 2.0f * generateInfos[i].vertexVec.x,
                    y = height / 2.0f * generateInfos[i].vertexVec.y
                };
                // offsetを加えて枠の位置を設定
                var innerPos = standardPos + generateInfos[i].innerOffset;
                var outerPos = standardPos + generateInfos[i].outerOffset;
                pairs[i] = new VertexPair
                {
                    InnerPos = innerPos,
                    OuterPos = outerPos
                };
            }
            return pairs;
        }
    
        private const float VertexUpdateDuration = 0.05f; // 頂点を更新する間隔
        private float _time = 0.0f;
        private void Update()
        {
            // 一定間隔で頂点情報を再更新する
            _time += Time.deltaTime;
            if (_time > VertexUpdateDuration)
            {
                _time = 0.0f;
                SetVerticesDirty();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editorでのハンドル調整用
        /// </summary>
        [CustomEditor(typeof(SelectFrameMesh), true)]
        public class SelectFrameInspector : Editor
        {
            private void OnSceneGUI()
            {
                Tools.current = Tool.None;

                var component = target as SelectFrameMesh;
                var vertexGenerateInfos = component.vertexGenerateInfos;
                var width = component.frameWidth;
                var height = component.frameHeight;

                var rectTransform = component.rectTransform;
                var localToWorldMatrix = rectTransform.localToWorldMatrix;
                var worldToLocalMatrix = rectTransform.worldToLocalMatrix;

                for (var i = 0; i < vertexGenerateInfos.Length; i++)
                {
                    // 基準となる位置
                    var standardPos = new Vector2
                    {
                        x = width / 2.0f * vertexGenerateInfos[i].vertexVec.x,
                        y = height / 2.0f * vertexGenerateInfos[i].vertexVec.y
                    };
                    // 内枠の座標位置
                    var innerPos = standardPos + vertexGenerateInfos[i].innerOffset;
                    var innerWorldPos = localToWorldMatrix.MultiplyPoint(innerPos);
                    SetOffsetPositionHandle(ref vertexGenerateInfos[i].innerOffset, standardPos, worldToLocalMatrix, innerWorldPos);

                    // 外枠の座標位置
                    var outerPos = standardPos + vertexGenerateInfos[i].outerOffset;
                    var outerWorldPos = localToWorldMatrix.MultiplyPoint(outerPos);
                    SetOffsetPositionHandle(ref vertexGenerateInfos[i].outerOffset, standardPos, worldToLocalMatrix, outerWorldPos);
                }

                // 頂点情報を更新
                component.SetVerticesDirty();
            }

            /// <summary>
            /// ハンドル位置から座標オフセット位置を設定
            /// </summary>
            /// <param name="offsetPoint"></param>
            /// <param name="standardPoint"></param>
            /// <param name="worldToLocalMatrix"></param>
            /// <param name="currentPosition"></param>
            void SetOffsetPositionHandle(ref Vector2 offsetPoint, Vector2 standardPoint, Matrix4x4 worldToLocalMatrix, Vector3 currentPosition)
            {
                // ハンドル位置から設定
                var handleSize = HandleUtility.GetHandleSize(currentPosition) * 0.2f;
                var newWorldPosition = Handles.FreeMoveHandle(currentPosition, Quaternion.identity, handleSize,
                    new Vector3(1.0f, 1.0f, 0.0f), Handles.CircleHandleCap);
            
                // World -> Local
                var newPosition = worldToLocalMatrix.MultiplyPoint3x4(newWorldPosition);
            
                // オフセット位置を再設定
                offsetPoint.x = newPosition.x - standardPoint.x;
                offsetPoint.y = newPosition.y - standardPoint.y;
            }
        }
#endif
    }
}
