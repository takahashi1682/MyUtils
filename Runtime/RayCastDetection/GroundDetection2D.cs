using MyUtils.RayCastDetection.Core;
using R3;
using UnityEngine;

namespace MyUtils.RayCastDetection
{
    /// <summary>
    /// 地面を検出する機能(2D)
    /// </summary>
    public class GroundDetection2D : BoxCast2dDetection, IGroundDetectionObservable
    {
        [Header("地面までの距離がこの値以下なら地面と判定")]
        [SerializeField] private float _isGroundThreshold = 0.01f;

        private ReadOnlyReactiveProperty<bool> _isGround;
        public ReadOnlyReactiveProperty<bool> IsGround =>
            _isGround ??= HitDistance.Select(v => v < _isGroundThreshold).ToReadOnlyReactiveProperty().AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isAir;
        public ReadOnlyReactiveProperty<bool> IsAir =>
            _isAir ??= HitDistance.Select(v => v > _isGroundThreshold).ToReadOnlyReactiveProperty().AddTo(this);
    }
}