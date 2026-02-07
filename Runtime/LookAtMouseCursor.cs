using R3;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// マウスカーソルのワールド座標の方向を向くコンポーネント
    /// </summary>
    public class LookAtMouseCursor : AbstractTargetBehaviour<Transform>
    {
        [SerializeField] private MouseToWorldRaycaster _mouseToWorldRaycaster;

        [Header("Rotation Axis Settings")]
        [SerializeField] private bool _useX = true;
        [SerializeField] private bool _useY;
        [SerializeField] private bool _useZ = true;

#if UNITY_EDITOR
        [Header("Debug Gizmo Settings")]
        public bool IsShowGizmo = true;
        public Color GizmoColor = Color.red;
#endif

        protected override void Start()
        {
            base.Start();
            _mouseToWorldRaycaster.CursorWorldPos
                .Subscribe(targetPos =>
                {
                    // ターゲットへの方向ベクトルを算出
                    Vector3 direction = targetPos - Target.position;

                    // 有効でない軸の回転を 0 にする（方向ベクトル側を加工）
                    if (!_useX) direction.x = 0;
                    if (!_useY) direction.y = 0;
                    if (!_useZ) direction.z = 0;

                    // 方向ベクトルがゼロ（同座標）でなければ回転を適用
                    if (direction != Vector3.zero)
                    {
                        Target.rotation = Quaternion.LookRotation(direction);
                    }
                })
                .AddTo(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!IsShowGizmo) return;
            if (Target == null) return;

            Gizmos.color = GizmoColor;
            Gizmos.DrawLine(Target.position, _mouseToWorldRaycaster.CursorWorldPos.CurrentValue);
        }
#endif
    }
}