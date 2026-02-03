using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.InputTrigger
{
    public class ObjectActivateOnInputTrigger : AbstractInputTrigger
    {
        public GameObject[] TargetObjects;
        public bool IsSetActive = true;

        protected override UniTask OnPressed(CancellationToken ct)
        {
            foreach (var obj in TargetObjects)
            {
                if (obj != null)
                {
                    Debug.Log(obj);
                    obj.SetActive(IsSetActive);
                }
            }

            return UniTask.CompletedTask;
        }
    }
}