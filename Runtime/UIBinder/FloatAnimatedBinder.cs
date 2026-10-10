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
        private float _displayValue;
        private bool _hasValue;

        protected override void OnValueChanged(float value)
        {
            // 既存のアニメーションを停止
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }

            // 最初の値は演出せずそのまま表示する(0から数え上げない)
            if (!_hasValue)
            {
                _hasValue = true;
                Show(value);
                return;
            }

            // 新しいアニメーション開始(途中なら表示中の値から再開する)
            _animationCoroutine = StartCoroutine(AnimateValue(_displayValue, value, _duration));
        }

        private void Show(float value)
        {
            _displayValue = value;
            Target.text = string.Format(_textFormat, value);
        }

        private IEnumerator AnimateValue(float startValue, float endValue, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration); // 0～1 の補間値
                Show(Mathf.Lerp(startValue, endValue, t));
                yield return null;
            }

            // 最終値を確実に設定
            Show(endValue);
            _animationCoroutine = null;
        }
    }
}
