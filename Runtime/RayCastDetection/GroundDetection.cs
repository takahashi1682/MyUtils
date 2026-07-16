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

        private ReadOnlyReactiveProperty<bool> _isGround;
        public ReadOnlyReactiveProperty<bool> IsGround =>
            _isGround ??= HitDistance.Select(v => v < _isGroundThreshold).ToReadOnlyReactiveProperty().AddTo(this);

        private ReadOnlyReactiveProperty<bool> _isAir;
        public ReadOnlyReactiveProperty<bool> IsAir =>
            _isAir ??= HitDistance.Select(v => v > _isGroundThreshold).ToReadOnlyReactiveProperty().AddTo(this);
    }
}