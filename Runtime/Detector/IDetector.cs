using R3;
using UnityEngine;

namespace MyUtils.Detector
{
    public interface IDetector
    {
        /// <summary>
        /// ヒットしたオブジェクト
        /// </summary>
        ReadOnlyReactiveProperty<RaycastHit> HitObject { get; }

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