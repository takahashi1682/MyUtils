using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.InputTrigger
{
    public enum ObjectActivateMode
    {
        Activate,
        Deactivate,
        Toggle
    }

    public class ObjectActivateOnInputTrigger : AbstractInputTrigger
    {
        [Header("ObjectActivateOnInputTrigger")]
        public GameObject[] TargetObjects;
        public ObjectActivateMode ActivateMode = ObjectActivateMode.Toggle;

        protected override UniTask OnPressed(CancellationToken ct)
        {
            foreach (var obj in TargetObjects)
            {
                if (obj != null)
                {
                    switch (ActivateMode)
                    {
                        case ObjectActivateMode.Activate:
                            obj.SetActive(true);
                            break;

                        case ObjectActivateMode.Deactivate:
                            obj.SetActive(false);
                            break;

                        case ObjectActivateMode.Toggle:
                            obj.SetActive(!obj.activeSelf);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return UniTask.CompletedTask;
        }
    }
}