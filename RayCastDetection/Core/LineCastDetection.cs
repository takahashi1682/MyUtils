using R3;
using R3.Triggers;
using UnityEditor;
using UnityEngine;

namespace MyUtils.RayCastDetection.Core
{
    public interface ILineCastDetection : IRayCastDetection
    {
    }

    /// <summary>
    ///  LineCastの当たり判定を行う機能
    /// </summary>
    public class LineCastDetection : MonoBehaviour, ILineCastDetection
    {
        [SerializeField] private Transform _rayPosition;
        [SerializeField] private Vector3 _rayDirection = Vector3.down;
        [SerializeField] private float _maxRayDistance = 10;
        [SerializeField] private LayerMask _layerMask = int.MaxValue;

        [Header("Debug")]
#if UNITY_EDITOR
        [SerializeField] private bool _isShowGizmos = true;
#endif
        [SerializeField] private Vector3 _labelOffset = Vector3.right;

        private readonly ReactiveProperty<RaycastHit> _hitObject = new();
        public ReadOnlyReactiveProperty<RaycastHit> HitObject => _hitObject;

        private readonly ReactiveProperty<float> _hitDistance = new();
        public ReadOnlyReactiveProperty<float> HitDistance => _hitDistance;

        protected virtual void Awake()
        {
            _hitObject.AddTo(this);
            _hitDistance.AddTo(this);

            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    Physics.Linecast(
                        _rayPosition.position,
                        _rayPosition.position + _rayDirection * _maxRayDistance,
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
        /// BoxCastの当たり判定を描画
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_isShowGizmos) return;

            var from = _rayPosition.position;
            Debug.DrawRay(from, _rayDirection * _hitDistance.CurrentValue, Color.red);
            Handles.Label(from + _labelOffset, $"{_hitDistance.Value}\n{_hitObject.Value.collider?.gameObject}",
                GUI.skin.box);
        }
#endif
    }
}