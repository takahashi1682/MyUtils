using System;
using System.Globalization;
using DG.Tweening;

namespace MyUtils.DOTweenUtils
{
    public static class DoTweenExtensions
    {
        public static Tweener DOTextInt(this TMPro.TextMeshProUGUI text, int initialValue, int finalValue,
            float duration,
            Func<int, string> convertor) =>
            DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tweener DOTextInt(this TMPro.TextMeshProUGUI text, int initialValue, int finalValue,
            float duration)
            => DOTextInt(text, initialValue, finalValue, duration, it => it.ToString());

        public static Tweener DOTextFloat(this TMPro.TextMeshProUGUI text, float initialValue, float finalValue,
            float duration,
            Func<float, string> convertor) =>
            DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tweener DOTextFloat(this TMPro.TextMeshProUGUI text, float initialValue, float finalValue,
            float duration)
            => DOTextFloat(text, initialValue, finalValue, duration, it => it.ToString(CultureInfo.CurrentCulture));

        public static Tweener DOTextLong(this TMPro.TextMeshProUGUI text, long initialValue, long finalValue,
            float duration,
            Func<long, string> convertor) =>
            DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tweener DOTextLong(this TMPro.TextMeshProUGUI text, long initialValue, long finalValue,
            float duration)
            => DOTextLong(text, initialValue, finalValue, duration, it => it.ToString());

        public static Tweener DOTextDouble(this TMPro.TextMeshProUGUI text, double initialValue, double finalValue,
            float duration,
            Func<double, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextDouble(this TMPro.TextMeshProUGUI text, double initialValue, double finalValue,
            float duration)
            => DOTextDouble(text, initialValue, finalValue, duration, it => it.ToString(CultureInfo.CurrentCulture));
    }
}