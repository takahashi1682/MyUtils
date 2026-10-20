using MyUtils.TweenUtils;
using UnityEngine;

namespace MyUtils.OnSelectBehaviour
{
    /// <summary>
    /// UIがクリックされた時に、一瞬だけ縮んで元に戻る機能
    /// </summary>
    public class PunchScaleOnSelect : AbstractOnSelectBehaviour
    {
        [Tooltip("縮める対象。空ならこのUI自身。ScaleOnSelectと併用する場合は子のTransformを指定する")]
        [SerializeField] private Transform _scaleTarget;
        [SerializeField] private float _pressedScale = 0.9f;
        [SerializeField] private float _duration = 0.05f; // 縮む時間(戻る時間も同じ)
        [SerializeField] private Ease _ease = Ease.InSine;

        private Vector3 _defaultScale;
        private Tween _tween;

        protected override void Start()
        {
            base.Start();
            if (_scaleTarget == null) _scaleTarget = Target.transform;
            _defaultScale = _scaleTarget.localScale;
        }

        protected override void SelectedAction()
        {
        }

        protected override void SubmitAction()
        {
            // 連打されたら前のアニメーションを止め、元の大きさから縮め直す
            _tween?.Kill();
            _scaleTarget.localScale = _defaultScale;

            // 縮んで、Yoyo で元に戻す(2ループで 往路→復路)
            _tween = _scaleTarget.TweenScale(_defaultScale * _pressedScale, _duration)
                .SetEase(_ease)
                .SetLoops(2, LoopType.Yoyo)
                .SetLink(gameObject);
            _tween.Play();
        }

        protected override void DeselectAction()
        {
        }

        private void OnDestroy() => _tween?.Kill();
    }
}