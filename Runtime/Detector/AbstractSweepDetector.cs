using UnityEngine;

namespace MyUtils.Detector
{
    public abstract class AbstractSweepDetector : MonoBehaviour
    {
        public float MinSweepDistance = 0.0001f;
        public LayerMask TargetLayers = ~0;

        private Vector3 _previousPosition;

        private void OnEnable()
        {
            _previousPosition = transform.position;
        }

        protected virtual void FixedUpdate()
        {
            if (TryDetectSweepHit(out var hitCollider))
            {
                OnHit(hitCollider);
            }
        }

        private bool TryDetectSweepHit(out Collider hitCollider)
        {
            Vector3 currentPosition = transform.position;
            Vector3 previousPosition = _previousPosition;
            Vector3 delta = currentPosition - previousPosition; // Sweep距離を計算
            _previousPosition = currentPosition;
            hitCollider = null;

            // Sweep距離がMinSweepDistance以下の場合は、ヒット判定を行わない
            if (delta.sqrMagnitude <= MinSweepDistance * MinSweepDistance) return false;

            // Sweep距離がMinSweepDistance以上の場合は、Raycastでヒット判定を行う
            float distance = delta.magnitude;
            bool didHit = Physics.Raycast(
                previousPosition, delta.normalized, out var hit, distance,
                TargetLayers, QueryTriggerInteraction.Ignore);

            hitCollider = hit.collider;
            return didHit;
        }

        protected abstract void OnHit(Collider hitCollider);
    }
}