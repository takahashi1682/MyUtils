using R3;
using R3.Triggers;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyUtils.Detection
{
    public interface ILineCast2dDetection : IDetection2d
    {
    }

    /// <summary>
    /// LineCast(2D)の当たり判定を行う機能
    /// </summary>
    public class LineCast2dDetection : MonoBehaviour, ILineCast2dDetection
    {
        [Header("Settings")]
        [SerializeField] private Transform _rayPosition;
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
                    Vector2 from = _rayPosition.position;
                    Vector2 to = from + _rayDirection * _maxRayDistance;

                    _hit2D.Value = Physics2D.Linecast(from, to, _layerMask);

                    _hitDistance.Value =
                        _hit2D.Value
                            ? _hit2D.Value.distance
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
                Debug.DrawRay(from, _rayDirection * _hitDistance.CurrentValue, _isHit.Value ? Color.red : Color.yellow);
                Handles.Label(from + _labelOffset, $"{_hitDistance.Value}\n{_hit2D.Value.collider?.gameObject}",
                    GUI.skin.box);
            }
            else
            {
                Debug.DrawRay(from, _rayDirection * _maxRayDistance, Color.yellow);
            }
        }
#endif
    }
}
