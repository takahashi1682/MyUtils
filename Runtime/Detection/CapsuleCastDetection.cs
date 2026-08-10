using R3;
using R3.Triggers;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyUtils.Detection
{
    public interface ICapsuleCastDetection : IDetection
    {
    }

    /// <summary>
    /// CapsuleCastの当たり判定を行う機能
    /// </summary>
    public class CapsuleCastDetection : MonoBehaviour, ICapsuleCastDetection
    {
        [Header("Settings")]
        [SerializeField] private Transform _rayPosition;
        [SerializeField] private float _capsuleRadius = 0.5f;
        [SerializeField] private float _capsuleHeight = 2f;
        [Tooltip("カプセルの回転（オイラー角、度数）。未回転時はY軸方向が高さ方向。")]
        [SerializeField] private Vector3 _rayAngle = Vector3.zero;
        [SerializeField] private Vector3 _rayDirection = Vector3.down;
        [SerializeField] private float _maxRayDistance = 10;
        [SerializeField] private LayerMask _layerMask = int.MaxValue;

        [Header("Target")]
        [SerializeField] private float _isHitThreshold = 0.01f;

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
                .Subscribe(distance => _isHit.Value = distance < _isHitThreshold)
                .AddTo(this);

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    GetCapsulePoints(_rayPosition.position, out var point1, out var point2);

                    Physics.CapsuleCast(
                        point1,
                        point2,
                        _capsuleRadius,
                        _rayDirection,
                        out var hitInfo,
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

        /// <summary>
        /// カプセルの上下半球の中心座標を算出する
        /// </summary>
        private void GetCapsulePoints(Vector3 center, out Vector3 point1, out Vector3 point2)
        {
            var halfLineHeight = Mathf.Max(_capsuleHeight * 0.5f - _capsuleRadius, 0f);
            var up = Quaternion.Euler(_rayAngle) * Vector3.up;
            point1 = center + up * halfLineHeight;
            point2 = center - up * halfLineHeight;
        }

#if UNITY_EDITOR
        /// <summary>
        /// CapsuleCastの当たり判定を描画
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_isShowGizmos) return;

            var from = _rayPosition.position;

            if (Application.isPlaying)
            {
                var to = from + _rayDirection * _hitDistance.Value;
                DrawCapsuleGizmo(from);
                DrawCapsuleGizmo(to);

                Debug.DrawRay(from, _rayDirection * _hitDistance.CurrentValue, _isHit.Value ? Color.red : Color.yellow);
                Handles.Label(from + _labelOffset, $"{_hitDistance.Value}\n{_hitObject.Value.collider?.gameObject}",
                    GUI.skin.box);
            }
            else
            {
                var to = from + _rayDirection * _maxRayDistance;
                DrawCapsuleGizmo(from);
                DrawCapsuleGizmo(to);

                Debug.DrawRay(from, _rayDirection * _maxRayDistance, Color.yellow);
            }
        }

        /// <summary>
        /// カプセル形状を上下半球のワイヤースフィアで簡易描画
        /// </summary>
        private void DrawCapsuleGizmo(Vector3 center)
        {
            GetCapsulePoints(center, out var point1, out var point2);
            Gizmos.DrawWireSphere(point1, _capsuleRadius);
            Gizmos.DrawWireSphere(point2, _capsuleRadius);
        }
#endif
    }
}
