using System;
using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    /// <summary>
    ///     シンプルな移動機能
    /// </summary>
    public class DoLocalMove : AbstractDoTween<Transform>
    {
        [Header("DoLocalMove")]
        public bool IsRelative;
        public Vector3 EndValue = new(300, 0, 0);
        
        protected override Tween CreateTween()
            => Target.DOLocalMove(EndValue, Duration).SetRelative(IsRelative);
    }
}