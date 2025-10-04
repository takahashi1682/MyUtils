using DG.Tweening;
using UnityEngine;

namespace TUtils.DOTweenUtils
{
    /// <summary>
    ///     シンプルな移動機能
    /// </summary>
    public class DoLocalMove : AbstractDoTween
    {
        [Header("DoLocalMove")]
        public bool IsRelative;
        public Vector3 EndValue = new(300, 0, 0);

        private Transform _targetTransform;

        private void Awake()
        {
            _targetTransform = GetComponent<Transform>();
        }

        protected override Tween CreateTween()
            => _targetTransform.DOLocalMove(EndValue, Duration).SetRelative(IsRelative);
    }
}