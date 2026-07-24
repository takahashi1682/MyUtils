using UnityEngine;

namespace MyUtils.TweenUtils
{
    public static class TweenTransformExtensions
    {
        public static Tween DOLocalMove(this Transform transform, Vector3 endValue, float duration)
        {
            Vector3 start = transform.localPosition;
            Tween tween = null;
            tween = Tween.Create(duration, eased =>
            {
                Vector3 target = tween.IsRelative ? start + endValue : endValue;
                transform.localPosition = Vector3.LerpUnclamped(start, target, eased);
            });
            return tween;
        }

        public static Tween DOScale(this Transform transform, Vector3 endValue, float duration)
        {
            Vector3 start = transform.localScale;
            return Tween.Create(duration,
                eased => transform.localScale = Vector3.LerpUnclamped(start, endValue, eased));
        }

        /// <summary>
        /// start(現在位置)からendValueへ移動しつつ、その間にnumJumps回、jumpPowerの高さで放物線状にバウンドする。
        /// </summary>
        public static Tween DOLocalJump(this Transform transform, Vector3 endValue, float jumpPower, int numJumps,
            float duration)
        {
            Vector3 start = transform.localPosition;
            numJumps = Mathf.Max(1, numJumps);

            return Tween.Create(duration, (eased, raw) =>
            {
                Vector3 basePos = Vector3.LerpUnclamped(start, endValue, eased);
                float phase = raw * numJumps;
                float cycle = phase - Mathf.Floor(phase);
                float yOffset = 4f * jumpPower * cycle * (1f - cycle);
                transform.localPosition = basePos + Vector3.up * yOffset;
            });
        }

        /// <summary>
        /// DOTween本家のシェイクアルゴリズムの完全再現ではなく、vibrato回のランダムオフセットを
        /// 時間経過で減衰させながら補間する簡易的な再現。
        /// </summary>
        public static Tween DOShakePosition(this Transform transform, float duration, float strength, int vibrato,
            float randomness)
        {
            Vector3 start = transform.localPosition;
            vibrato = Mathf.Max(1, vibrato);

            var offsets = new Vector3[vibrato + 1];
            for (int i = 0; i < offsets.Length; i++)
            {
                if (i == offsets.Length - 1)
                {
                    offsets[i] = Vector3.zero;
                    continue;
                }

                float angle = Random.Range(0f, 360f + randomness);
                offsets[i] = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f) *
                             strength;
            }

            return Tween.Create(duration, (_, raw) =>
            {
                float decay = 1f - raw;
                float phase = raw * vibrato;
                int index = Mathf.Clamp(Mathf.FloorToInt(phase), 0, vibrato - 1);
                float segmentT = phase - index;
                Vector3 offset = Vector3.LerpUnclamped(offsets[index], offsets[index + 1], segmentT) * decay;
                transform.localPosition = start + offset;
            });
        }
    }
}
