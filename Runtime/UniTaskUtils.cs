using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils
{
    public static class UniTaskUtils
    {
        /// <summary>
        /// 指定した値の範囲を指定時間で補間し、指定のアクションを実行します。
        /// </summary>
        /// <param name="from">開始値</param>
        /// <param name="to">終了値</param>
        /// <param name="duration">補間時間（秒）</param>
        /// <param name="apply">補間値に対する処理</param>
        /// <param name="intervalMs">更新間隔（ミリ秒）</param>
        /// <param name="token">キャンセルトークン</param>
        public static async UniTask LerpAsync(
            float from,
            float to,
            float duration,
            Action<float> apply,
            int intervalMs = 10,
            CancellationToken token = default)
        {
            float start = Time.realtimeSinceStartup;

            while (!token.IsCancellationRequested)
            {
                float elapsed = Time.realtimeSinceStartup - start;
                if (elapsed >= duration) break;

                float t = Mathf.Clamp01(elapsed / duration);
                apply?.Invoke(Mathf.Lerp(from, to, t));
                await UniTask.Delay(intervalMs, DelayType.Realtime, cancellationToken: token);
            }

            apply?.Invoke(to); // 最終値を保証
        }
    }
}