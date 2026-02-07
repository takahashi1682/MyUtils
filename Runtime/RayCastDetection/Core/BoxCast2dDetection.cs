using R3;
using R3.Triggers;
using UnityEditor;
using UnityEngine;

namespace MyUtils.RayCastDetection.Core
{
    public interface IBoxCast2dDetection : IRayCast2dDetection
    {
    }

    /// <summary>
    /// BoxCastの当たり判定を行う機能
    /// </summary>
    public class BoxCast2dDetection : MonoBehaviour, IBoxCast2dDetection
    {
        public Transform RayPosition;
        public Vector2 RaySize = new(1f, 0.1f);
        public float RayAngle;
        public Vector2 RayDirection = Vector2.down;
        public float MaxRayDistance = 10;
        public LayerMask LayerMask = int.MaxValue;

        [Header("Debug")]
        public bool IsShowGizmos = true;
        public Vector3 LabelOffset = Vector3.right;

        private readonly ReactiveProperty<RaycastHit2D> _hit2D = new();
        public ReadOnlyReactiveProperty<RaycastHit2D> Hit2D => _hit2D;

        private readonly ReactiveProperty<float> _hitDistance = new();
        public ReadOnlyReactiveProperty<float> HitDistance => _hitDistance;

        protected virtual void Awake()
        {
            _hit2D.AddTo(this);
            _hitDistance.AddTo(this);

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _hit2D.Value =
                        Physics2D.BoxCast(
                            RayPosition.position,
                            RaySize,
                            RayAngle,
                            RayDirection,
                            MaxRayDistance,
                            LayerMask);

                    _hitDistance.Value =
                        _hit2D.Value
                            ? _hit2D.Value.distance
                            : MaxRayDistance;
                })
                .AddTo(this);
        }

#if UNITY_EDITOR
        /// <summary>
        /// BoxCastの当たり判定を描画
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!IsShowGizmos) return;

            var from = RayPosition.position;
            var to = from + (Vector3)(RayDirection * _hitDistance.Value);

            Gizmos.DrawWireCube(from, RaySize);
            Debug.DrawRay(from, RayDirection * _hitDistance.CurrentValue, Color.red);
            Gizmos.DrawWireCube(to, RaySize);

            Handles.Label(from + LabelOffset, $"{_hitDistance.Value}\n{_hit2D.Value.collider?.gameObject}",
                GUI.skin.box);
        }
#endif
    }
}