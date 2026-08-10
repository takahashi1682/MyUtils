using R3;
using R3.Triggers;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyUtils.Detection
{
    public interface IBoxCast2dDetection : IDetection2d
    {
    }

    /// <summary>
    /// BoxCast(2D)の当たり判定を行う機能
    /// </summary>
    public class BoxCast2dDetection : MonoBehaviour, IBoxCast2dDetection
    {
        [Header("Settings")]
        [SerializeField] private Transform _rayPosition;
        [SerializeField] private Vector2 _raySize = new(1f, 0.1f);
        [Tooltip("ボックスの回転（度数、Z軸）")]
        [SerializeField] private float _rayAngle;
        [SerializeField] private Vector2 _rayDirection = Vector2.down;
        [SerializeField] private float _maxRayDistance = 10;
        [SerializeField] private LayerMask _layerMask = int.MaxValue;

        [Header("Target")]
        [SerializeField] private float _isHitThreshold = 0.01f;

        [Header("Debug")]
#if UNITY_EDITOR
        [SerializeField] private bool _isShowGizmos = true;
#endif
        [SerializeField] private Vector3 _labelOffset = Vector3.right;

        private readonly ReactiveProperty<RaycastHit2D> _hit2D = new();
        public ReadOnlyReactiveProperty<RaycastHit2D> Hit2D => _hit2D;

        private readonly ReactiveProperty<float> _hitDistance = new();
        public ReadOnlyReactiveProperty<float> HitDistance => _hitDistance;

        private readonly ReactiveProperty<bool> _isHit = new();
        public ReadOnlyReactiveProperty<bool> IsHit => _isHit;

        protected virtual void Awake()
        {
            _hit2D.AddTo(this);
            _hitDistance.AddTo(this);
            _isHit.AddTo(this);

            _hitDistance
                .Subscribe(distance => _isHit.Value = distance < _isHitThreshold)
                .AddTo(this);

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _hit2D.Value =
                        Physics2D.BoxCast(
                            _rayPosition.position,
                            _raySize,
                            _rayAngle,
                            _rayDirection,
                            _maxRayDistance,
                            _layerMask);

                    _hitDistance.Value =
                        _hit2D.Value
                            ? _hit2D.Value.distance
                            : _maxRayDistance;
                })
                .AddTo(this);
        }

#if UNITY_EDITOR
        /// <summary>
        /// BoxCastの当たり判定を描画
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_isShowGizmos) return;

            var from = _rayPosition.position;
            var previousMatrix = Gizmos.matrix;

            if (Application.isPlaying)
            {
                var to = from + (Vector3)(_rayDirection * _hitDistance.Value);
                DrawBoxGizmo(from);
                DrawBoxGizmo(to);
                Gizmos.matrix = previousMatrix;

                Debug.DrawRay(from, _rayDirection * _hitDistance.CurrentValue, _isHit.Value ? Color.red : Color.yellow);
                Handles.Label(from + _labelOffset, $"{_hitDistance.Value}\n{_hit2D.Value.collider?.gameObject}",
                    GUI.skin.box);
            }
            else
            {
                var to = from + (Vector3)(_rayDirection * _maxRayDistance);
                DrawBoxGizmo(from);
                DrawBoxGizmo(to);
                Gizmos.matrix = previousMatrix;

                Debug.DrawRay(from, _rayDirection * _maxRayDistance, Color.yellow);
            }
        }

        /// <summary>
        /// _rayAngleの回転を反映したワイヤーキューブをGizmos.matrix経由で描画
        /// </summary>
        private void DrawBoxGizmo(Vector3 center)
        {
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, _rayAngle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _raySize);
        }
#endif
    }
}
