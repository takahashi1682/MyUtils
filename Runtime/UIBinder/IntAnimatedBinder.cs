using System;
using System.Collections;
using UnityEngine;

namespace MyUtils.UIBinder
{
    /// <summary>
    ///  整数をテキストにバインドしアニメーションする機能。
    ///  floatを経由しないので、大きな値(約1,670万超)でも丸められずに表示できる。
    /// </summary>
    public class IntAnimatedBinder : AbstractValueBinder<int>
    {
        [SerializeField] private float _duration = 1;
        private Coroutine _animationCoroutine;
        private int _displayValue;
        private int _targetValue;
        private bool _hasValue;

        protected override void OnValueChanged(int value)
        {
            StopAnimation();

            _targetValue = value;

            // 最初の値と、非アクティブ中(コルーチンを開始できない)の値は演出せずそのまま表示する
            if (!_hasValue || !isActiveAndEnabled)
            {
                _hasValue = true;
                Show(value);
                return;
            }

            _animationCoroutine = StartCoroutine(AnimateValue(_displayValue, value, _duration));
        }

        // 非アクティブ化でコルーチンが止まるので、途中の値で止まらないよう最終値を表示しておく
        private void OnDisable()
        {
            if (_animationCoroutine == null) return;

            StopAnimation();
            Show(_targetValue);
        }

        private void StopAnimation()
        {
            if (_animationCoroutine == null) return;

            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }

        private void Show(int value)
        {
            _displayValue = value;
            Target.text = string.Format(_textFormat, value);
        }

        private IEnumerator AnimateValue(int startValue, int endValue, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                double t = Math.Clamp(elapsedTime / duration, 0d, 1d);
                // long/doubleで補間し、intの範囲でオーバーフローさせない
                Show((int)Math.Round(startValue + ((double)endValue - startValue) * t));
                yield return null;
            }

            Show(endValue);
            _animationCoroutine = null;
        }
    }
}
