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

        /// <summary>InputActionReferenceが設定されているか</summary>
        protected bool HasInputAction => _inputAction != null;

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
            ).Where(IsValidInput).Select(_ => Unit.Default);
        }

        /// <summary>
        /// performedのうち、入力として扱うものを判定する(既定はすべて)
        /// </summary>
        protected virtual bool IsValidInput(InputAction.CallbackContext context) => true;

        protected virtual void OnEnable() => _inputAction?.Enable();
        protected virtual void OnDisable() => _inputAction?.Disable();
        protected virtual void OnDestroy() => _inputAction?.Dispose();
    }
}