using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils.InputTrigger
{
    public abstract class AbstractActionInputTrigger : AbstractInputTrigger
    {
        [Header("Action Settings")]
        [SerializeField] private InputActionReference _inputActionReference;
        private InputAction _inputAction;

        protected virtual void Awake()
        {
            if (_inputActionReference != null)
                _inputAction = _inputActionReference.action.Clone();
        }

        protected override Observable<Unit> CreateInputObservable()
        {
            if (_inputAction == null) return Observable.Empty<Unit>();

            return Observable.FromEvent<InputAction.CallbackContext>(
                h => _inputAction.performed += h,
                h => _inputAction.performed -= h,
                destroyCancellationToken
            ).Select(_ => Unit.Default);
        }

        protected virtual void OnEnable() => _inputAction?.Enable();
        protected virtual void OnDisable() => _inputAction?.Disable();
        protected virtual void OnDestroy() => _inputAction?.Dispose();
    }
}