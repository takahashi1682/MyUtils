using R3;
using UnityEngine;

namespace MyUtils.RayCastDetection.Core
{
    public interface IRayCast2dDetection
    {
        /// <summary>
        /// ヒットしたオブジェクト
        /// </summary>
        ReadOnlyReactiveProperty<RaycastHit2D> Hit2D { get; }

        /// <summary>
        /// ヒットしたオブジェクトまでの距離
        /// </summary>
        ReadOnlyReactiveProperty<float> HitDistance { get; }
    }
}