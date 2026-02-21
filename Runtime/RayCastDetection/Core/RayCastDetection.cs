using R3;
using R3.Triggers;
using UnityEditor;
using UnityEngine;

namespace MyUtils.RayCastDetection.Core
{
    public class RayCastDetection : MonoBehaviour, IRayCastDetection
    {
        [SerializeField] private Transform _rayPosition;
        [SerializeField] private Vector3 _rayDirection = Vector3.forward;
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
                    Physics.Raycast(
                        _rayPosition.position,
                        _rayPosition.rotation * _rayDirection,
                        out RaycastHit hitInfo,
                        _maxRayDistance,
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

            Vector3 from = _rayPosition.position;
            Vector3 direction = _rayPosition.rotation * _rayDirection;
            if (Application.isPlaying)
            {
                Debug.DrawRay(from, direction * _hitDistance.CurrentValue, Color.red);
                Handles.Label(from + _labelOffset, $"{_hitDistance.Value}\n{_hitObject.Value.collider?.gameObject}",
                    GUI.skin.box);
            }
            else
            {
                Debug.DrawRay(from, direction * _maxRayDistance, Color.red);
            }
        }
#endif
    }
}