using R3;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    ///  マウスのスクリーン座標からワールド座標へのRaycastを行い、
    ///  ヒットしたワールド座標を提供するクラス
    /// </summary>
    public class MouseToWorldRaycaster : AbstractTargetBehaviour<Camera>
    {
        [SerializeField] private InputPlayerReader _input;
        [SerializeField] private float _maxDistance = 100f;
        [SerializeField] private LayerMask _layerMask = ~0;

        [SerializeField] private SerializableReactiveProperty<Vector3> _cursorWorldPos = new();
        public ReadOnlyReactiveProperty<Vector3> CursorWorldPos => _cursorWorldPos;

#if UNITY_EDITOR
        [Header("Debug Gizmo Settings")]
        public bool IsShowGizmo = true;
        public Color GizmoColor = Color.red;
#endif

        protected override void Start()
        {
            base.Start();
            _cursorWorldPos.AddTo(this);
            _input.MousePosition
                .Subscribe(screenPos =>
                {
                    // カメラからRayを飛ばし、ヒットしたオブジェクトを取得する
                    Ray ray = Target.ScreenPointToRay(screenPos);
                    if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _layerMask))
                    {
                        if (hit.collider)
                        {
                            _cursorWorldPos.Value = hit.point;
                        }
                    }
                }).AddTo(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!IsShowGizmo) return;

            Gizmos.color = GizmoColor;
            Gizmos.DrawSphere(_cursorWorldPos.Value, 0.5f);
        }
#endif
    }
}