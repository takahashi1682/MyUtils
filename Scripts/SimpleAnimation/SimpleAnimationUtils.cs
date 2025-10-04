using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TUtils.SimpleAnimation
{
    public static class SimpleAnimationUtils
    {
        public static async UniTask ShakeAsync(
            this Transform transform,
            Vector3 from,
            Vector3 to,
            CancellationToken token,
            float duration = 0.1f)
        {
            var elapsedTime = 0f;
            while (!token.IsCancellationRequested && elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var normalizedTime = elapsedTime / duration; // Normalize time
                var value = Mathf.Sin(normalizedTime * Mathf.PI); // Smooth 0 -> 1 -> 0
                transform.position = Vector3.Lerp(from, to, value);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            }

            transform.position = from;
        }
    }
}