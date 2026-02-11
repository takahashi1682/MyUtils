using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MyUtils.InputTrigger
{
    public class UnityEventOnInputTrigger : AbstractInputTrigger
    {
        [Header("ObjectActivateOnInputTrigger")]
        public UnityEvent OnTrigger;

        protected override UniTask OnPressed(CancellationToken ct)
        {
            OnTrigger?.Invoke();
            return UniTask.CompletedTask;
        }
    }
}