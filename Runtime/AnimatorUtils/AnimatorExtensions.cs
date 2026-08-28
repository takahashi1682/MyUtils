using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.AnimatorUtils
{
    public static class AnimatorExtensions
    {
        public static UniTask PlayAsync(this Animator animator, string stateName, int layer = 0,
            float normalizedTime = float.NegativeInfinity,
            CancellationToken cancellationToken = default)
            => animator.PlayAsync(Animator.StringToHash(stateName), layer, normalizedTime, cancellationToken);

        public static async UniTask PlayAsync(this Animator animator, int stateNameHash, int layer = 0,
            float normalizedTime = float.NegativeInfinity,
            CancellationToken cancellationToken = default)
        {
            animator.Play(stateNameHash, layer, normalizedTime);

            var timing = animator.updateMode == AnimatorUpdateMode.Fixed
                ? PlayerLoopTiming.FixedUpdate
                : PlayerLoopTiming.Update;
            await UniTask.Yield(timing, cancellationToken);

            await UniTask.WaitUntil(
                () => animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1.0f,
                cancellationToken: cancellationToken);
        }
    }
}