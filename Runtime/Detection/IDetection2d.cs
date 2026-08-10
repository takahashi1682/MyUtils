using R3;
using UnityEngine;

namespace MyUtils.Detection
{
    public interface IDetection2d
    {
        /// <summary>
        /// ヒットしたオブジェクト
        /// </summary>
        ReadOnlyReactiveProperty<RaycastHit2D> Hit2D { get; }

        /// <summary>
        /// ヒットしたオブジェクトまでの距離
        /// </summary>
        ReadOnlyReactiveProperty<float> HitDistance { get; }

        /// <summary>
        /// ヒットしているかどうか
        /// </summary>
        ReadOnlyReactiveProperty<bool> IsHit { get; }
    }
}