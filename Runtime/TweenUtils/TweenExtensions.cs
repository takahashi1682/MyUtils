using System;
using System.Globalization;

namespace MyUtils.TweenUtils
{
    public static class TweenExtensions
    {
        public static Tween TweenTextInt(this TMPro.TextMeshProUGUI text, int initialValue, int finalValue,
            float duration,
            Func<int, string> convertor) =>
            Tween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tween TweenTextInt(this TMPro.TextMeshProUGUI text, int initialValue, int finalValue,
            float duration)
            => TweenTextInt(text, initialValue, finalValue, duration, it => it.ToString());

        public static Tween TweenTextFloat(this TMPro.TextMeshProUGUI text, float initialValue, float finalValue,
            float duration,
            Func<float, string> convertor) =>
            Tween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tween TweenTextFloat(this TMPro.TextMeshProUGUI text, float initialValue, float finalValue,
            float duration)
            => TweenTextFloat(text, initialValue, finalValue, duration, it => it.ToString(CultureInfo.CurrentCulture));

        public static Tween TweenTextLong(this TMPro.TextMeshProUGUI text, long initialValue, long finalValue,
            float duration,
            Func<long, string> convertor) =>
            Tween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );

        public static Tween TweenTextLong(this TMPro.TextMeshProUGUI text, long initialValue, long finalValue,
            float duration)
            => TweenTextLong(text, initialValue, finalValue, duration, it => it.ToString());

        public static Tween TweenTextDouble(this TMPro.TextMeshProUGUI text, double initialValue, double finalValue,
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

        public static Tween TweenTextDouble(this TMPro.TextMeshProUGUI text, double initialValue, double finalValue,
            float duration)
            => TweenTextDouble(text, initialValue, finalValue, duration, it => it.ToString(CultureInfo.CurrentCulture));
    }
}
