using System.Collections;
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
        private Coroutine _animationCoroutine;

        protected override void Start()
        {
            float lastValue = 0;

            _inValue.Value.CurrentValue
                .Subscribe(afterValue =>
                {
                    // 既存のアニメーションを停止
                    if (_animationCoroutine != null)
                    {
                        StopCoroutine(_animationCoroutine);
                    }

                    // 新しいアニメーション開始
                    _animationCoroutine = StartCoroutine(AnimateValue(lastValue, afterValue, _duration));

                    lastValue = afterValue;
                })
                .AddTo(this);
        }

        private IEnumerator AnimateValue(float startValue, float endValue, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration); // 0～1 の補間値
                float currentValue = Mathf.Lerp(startValue, endValue, t);
                _outText.text = string.Format(_textFormat, currentValue);
                yield return null;
            }

            // 最終値を確実に設定
            _outText.text = string.Format(_textFormat, endValue);
        }
    }
}