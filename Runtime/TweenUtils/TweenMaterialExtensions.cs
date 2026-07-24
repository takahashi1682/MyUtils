using UnityEngine;

namespace MyUtils.TweenUtils
{
    public static class TweenMaterialExtensions
    {
        /// <summary>呼び出し時点のmaterial.color.aからendValueへアルファ値をフェードする。</summary>
        public static Tween DOFade(this Material material, float endValue, float duration)
        {
            float start = material.color.a;
            return Tween.Create(duration, eased =>
            {
                Color color = material.color;
                color.a = Mathf.LerpUnclamped(start, endValue, eased);
                material.color = color;
            });
        }
    }
}
