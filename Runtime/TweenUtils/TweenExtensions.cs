using System;
using System.Globalization;

namespace MyUtils.DOTweenUtils
{
    public static class TweenExtensions
    {
        public static Tween DOTextInt(this TMPro.TextMeshProUGUI text, int initialValue, int finalValue,
            float duration,
            Func<int, string> convertor) =>
            Tween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tween DOTextInt(this TMPro.TextMeshProUGUI text, int initialValue, int finalValue,
            float duration)
            => DOTextInt(text, initialValue, finalValue, duration, it => it.ToString());

        public static Tween DOTextFloat(this TMPro.TextMeshProUGUI text, float initialValue, float finalValue,
            float duration,
            Func<float, string> convertor) =>
            Tween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tween DOTextFloat(this TMPro.TextMeshProUGUI text, float initialValue, float finalValue,
            float duration)
            => DOTextFloat(text, initialValue, finalValue, duration, it => it.ToString(CultureInfo.CurrentCulture));

        public static Tween DOTextLong(this TMPro.TextMeshProUGUI text, long initialValue, long finalValue,
            float duration,
            Func<long, string> convertor) =>
            Tween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tween DOTextLong(this TMPro.TextMeshProUGUI text, long initialValue, long finalValue,
            float duration)
            => DOTextLong(text, initialValue, finalValue, duration, it => it.ToString());

        public static Tween DOTextDouble(this TMPro.TextMeshProUGUI text, double initialValue, double finalValue,
            float duration,
            Func<double, string> convertor)
        {
            return Tween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tween DOTextDouble(this TMPro.TextMeshProUGUI text, double initialValue, double finalValue,
            float duration)
            => DOTextDouble(text, initialValue, finalValue, duration, it => it.ToString(CultureInfo.CurrentCulture));
    }
}
