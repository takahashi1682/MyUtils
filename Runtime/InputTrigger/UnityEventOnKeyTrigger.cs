using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MyUtils.InputTrigger
{
    public class UnityEventOnKeyTrigger : AbstractKeyInputTrigger
    {
        [Header("UnityActionOnKeyTrigger")]
        public UnityEvent OnTrigger;

        protected override UniTask OnPressed(CancellationToken ct)
        {
            OnTrigger?.Invoke();
            return UniTask.CompletedTask;
        }
    }
}