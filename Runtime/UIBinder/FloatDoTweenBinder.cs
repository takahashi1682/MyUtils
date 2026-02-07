using DG.Tweening;
using R3;
using UnityEngine;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  数値をテキストにバインドしアニメーションする機能
    /// </summary>
    public class FloatDoTweenBinder : AbstractValueBinder<float>
    {
        [SerializeField] private float _duration = 1;
        private Tween _tween;

        protected override void Start()
        {
            float lastValue = 0;

            _inValue.Value.CurrentValue
                .Subscribe(afterValue =>
                {
                    float currentValue = lastValue;

                    _tween?.Kill();
                    _tween = DOTween.To(
                            () => currentValue, // 開始値
                            v => currentValue = v, // 更新時の処理
                            afterValue,
                            _duration)
                        .OnKill(() => currentValue = afterValue)
                        .OnUpdate(() => _outText.text = string.Format(_textFormat, currentValue));
                    _tween.Play();

                    lastValue = afterValue;
                })
                .AddTo(this);
        }
    }
}