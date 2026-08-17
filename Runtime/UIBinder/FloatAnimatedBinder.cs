using System.Collections;
using UnityEngine;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  数値をテキストにバインドしアニメーションする機能
    /// </summary>
    public class FloatAnimatedBinder : AbstractValueBinder<float>
    {
        [SerializeField] private float _duration = 1;
        private Coroutine _animationCoroutine;
        private float _lastValue;

        protected override void OnValueChanged(float value)
        {
            // 既存のアニメーションを停止
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            // 新しいアニメーション開始
            _animationCoroutine = StartCoroutine(AnimateValue(_lastValue, value, _duration));

            _lastValue = value;
        }

        private IEnumerator AnimateValue(float startValue, float endValue, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration); // 0～1 の補間値
                float currentValue = Mathf.Lerp(startValue, endValue, t);
                Target.text = string.Format(_textFormat, currentValue);
                yield return null;
            }

            // 最終値を確実に設定
            Target.text = string.Format(_textFormat, endValue);
        }
    }
}
