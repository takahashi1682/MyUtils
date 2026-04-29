using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils.InputTrigger
{
    public abstract class AbstractKeyInputTrigger : AbstractInputTrigger
    {
        [Header("Keyboard Settings")]
        [SerializeField] private Key _targetKey = Key.Space;

        protected override Observable<Unit> CreateInputObservable()
        {
            // 毎フレーム入力をチェックするストリームを作成
            return Observable.EveryUpdate(destroyCancellationToken)
                .Where(_ => Keyboard.current != null && Keyboard.current[_targetKey].wasPressedThisFrame)
                .Select(_ => Unit.Default);
        }
    }
}