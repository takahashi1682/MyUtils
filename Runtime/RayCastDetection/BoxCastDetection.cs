using R3;
using R3.Triggers;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyUtils.RayCastDetection
{
    public interface IBoxCastDetection : IRayCastDetection
    {
    }

    /// <summary>
    /// BoxCastの当たり判定を行う機能
    /// </summary>
    public class BoxCastDetection : MonoBehaviour, IBoxCastDetection
    {
        [Header("Settings")]
        [SerializeField] private Transform _rayPosition;
        [SerializeField] private Vector3 _raySize = Vector3.one;
        [Tooltip("ボックスの回転（オイラー角、度数）")]
        [SerializeField] private Vector3 _rayAngle = Vector3.zero;
        [SerializeField] private Vector3 _rayDirection = Vector3.down;
        [SerializeField] private float _maxRayDistance = 10;
        [SerializeField] private LayerMask _layerMask = int.MaxValue;

        [Header("Target")]
        [SerializeField] private float _isDistanceThreshold = 0.01f;

        [Header("Debug")]
#if UNITY_EDITOR
        [SerializeField] private bool _isShowGizmos = true;
#endif
        [SerializeField] private Vector3 _labelOffset = Vector3.right;

        private readonly ReactiveProperty<RaycastHit> _hitObject = new();
        public ReadOnlyReactiveProperty<RaycastHit> HitObject => _hitObject;

        private readonly ReactiveProperty<float> _hitDistance = new();
        public ReadOnlyReactiveProperty<float> HitDistance => _hitDistance;

        private readonly ReactiveProperty<bool> _isHit = new();
        public ReadOnlyReactiveProperty<bool> IsHit => _isHit;

        protected virtual void Awake()
        {
            _hitObject.AddTo(this);
            _hitDistance.AddTo(this);
            _isHit.AddTo(this);

            _hitDistance
                .Subscribe(distance => _isHit.Value = distance < _isDistanceThreshold)
                .AddTo(this);

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    Physics.BoxCast(
                        _rayPosition.position,
                        _raySize * 0.5f,
                        _rayDirection,
                        out var hitInfo,
                        Quaternion.Euler(_rayAngle),
                        _maxRayDistance,
                        _layerMask);

                    _hitObject.Value = hitInfo;
                    _hitDistance.Value =
                        _hitObject.Value.collider
                            ? _hitObject.Value.distance
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
            var to = from + _rayDirection * _hitDistance.Value;

            var previousMatrix = Gizmos.matrix;
            DrawBoxGizmo(from);
            DrawBoxGizmo(to);
            Gizmos.matrix = previousMatrix;

            Debug.DrawRay(from, _rayDirection * _hitDistance.CurrentValue, Color.red);

            Handles.Label(from + _labelOffset, $"{_hitDistance.Value}\n{_hitObject.Value.collider?.gameObject}",
                GUI.skin.box);
        }

        /// <summary>
        /// _rayAngleの回転を反映したワイヤーキューブをGizmos.matrix経由で描画
        /// </summary>
        private void DrawBoxGizmo(Vector3 center)
        {
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(_rayAngle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _raySize);
        }
#endif
    }
}