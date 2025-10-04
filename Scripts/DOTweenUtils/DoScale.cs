using DG.Tweening;
using UnityEngine;

namespace TUtils.DOTweenUtils
{
    public class DoScale : AbstractDoTween
    {
        [Header("DoScale")] public Vector3 ToScale = new(1, 1.2f, 1);

        private Transform _targetTransform;

        protected override Tween CreateTween()
            => transform.DOScale(ToScale, Duration);
    }
}