using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyUtils.Detector
{
    public interface ILineCastDetector : IDetector
    {
    }

    /// <summary>
    ///  LineCastの当たり判定を行う機能
    /// </summary>
    public class LineCastDetector : MonoBehaviour, ILineCastDetector
    {
        [Header("Settings")]
        [SerializeField] private Transform _rayPosition;
        [FormerlySerializedAs("_rayDirection")] public Vector3 RayDirection = Vector3.down;
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
                    Physics.Linecast(
                        _rayPosition.position,
                        _rayPosition.position + RayDirection * _maxRayDistance,
                        out var hitInfo,
                        _layerMask
                    );

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
        /// LineCastの当たり判定を描画
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_isShowGizmos) return;

            var from = _rayPosition.position;

            if (Application.isPlaying)
            {
                Debug.DrawRay(from, RayDirection * _hitDistance.CurrentValue, _isHit.Value ? Color.red : Color.yellow);
                Handles.Label(from + _labelOffset, $"{_hitDistance.Value}\n{_hitObject.Value.collider?.gameObject}",
                    GUI.skin.box);
            }
            else
            {
                Debug.DrawRay(from, RayDirection * _maxRayDistance, Color.yellow);
            }
        }
#endif
    }
}