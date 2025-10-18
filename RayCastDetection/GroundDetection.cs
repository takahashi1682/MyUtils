using MyUtils.RayCastDetection.Core;
using R3;
using UnityEngine;

namespace MyUtils.RayCastDetection
{
    /// <summary>
    /// 地面を検出する機能(3D)
    /// </summary>
    public class GroundDetection : LineCastDetection, IGroundDetectionObservable
    {
        [Header("地面までの距離がこの値以下なら地面と判定")]
        [SerializeField] private float _isGroundThreshold = 0.1f;

        public ReadOnlyReactiveProperty<bool> IsGround =>
            HitDistance.Select(v => v < _isGroundThreshold).ToReadOnlyReactiveProperty();

        public ReadOnlyReactiveProperty<bool> IsAir =>
            HitDistance.Select(v => v > _isGroundThreshold).ToReadOnlyReactiveProperty();
    }
}