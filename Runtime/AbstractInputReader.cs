using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils
{
    public abstract class AbstractInputReader : ScriptableObject
    {
        private readonly Dictionary<(InputAction action, object target), Action<InputAction.CallbackContext>>
            _vector2Callbacks = new();
        private readonly Dictionary<(InputAction action, object target), Action<InputAction.CallbackContext>>
            _buttonCallbacks = new();

        [SerializeField] private InputActionAsset _inputActions;
        protected InputActionAsset InputActions => _inputActions;

        public void EnableInput(string actionMapName)
            => SetMapEnabled(actionMapName, true);

        public void DisableInput(string actionMapName)
            => SetMapEnabled(actionMapName, false);

        protected virtual void OnEnable()
        {
            if (_inputActions == null) return;

            foreach (var actionMap in _inputActions.actionMaps)
            {
                actionMap.Enable();
            }
        }

        protected virtual void OnDisable()
        {
            if (_inputActions == null) return;

            foreach (var actionMap in _inputActions.actionMaps)
            {
                actionMap.Disable();
            }
        }
        
        private void SetMapEnabled(string actionMapName, bool isEnabled)
        {
            if (_inputActions == null || string.IsNullOrWhiteSpace(actionMapName)) return;

            var actionMap = _inputActions.FindActionMap(actionMapName);
            if (actionMap == null)
            {
                Debug.LogWarning($"{nameof(AbstractInputReader)}: Action map '{actionMapName}' was not found.", this);
                return;
            }

            if (isEnabled)
                actionMap.Enable();
            else
                actionMap.Disable();
        }

        protected void RegisterVector2Action(InputActionReference actionReference,
            ReactiveProperty<Vector2> reactiveProperty)
        {
            var action = actionReference?.action;
            if (action == null || reactiveProperty == null) return;

            var key = (action, (object)reactiveProperty);
            if (_vector2Callbacks.ContainsKey(key)) return;

            Action<InputAction.CallbackContext> callback = context
                => reactiveProperty.Value = context.ReadValue<Vector2>();
            _vector2Callbacks[key] = callback;

            action.performed += callback;
            action.canceled += callback;
        }

        protected void UnregisterVector2Action(InputActionReference actionReference,
            ReactiveProperty<Vector2> reactiveProperty)
        {
            var action = actionReference?.action;
            if (action == null || reactiveProperty == null) return;

            var key = (action, (object)reactiveProperty);
            if (!_vector2Callbacks.TryGetValue(key, out var callback)) return;

            action.performed -= callback;
            action.canceled -= callback;
            _vector2Callbacks.Remove(key);
        }

        protected void RegisterButtonAction(InputActionReference actionReference,
            ReactiveProperty<bool> reactiveProperty)
        {
            var action = actionReference?.action;
            if (action == null || reactiveProperty == null) return;

            var key = (action, (object)reactiveProperty);
            if (_buttonCallbacks.ContainsKey(key)) return;

            Action<InputAction.CallbackContext> callback = context
                => reactiveProperty.Value = context.ReadValueAsButton();
            _buttonCallbacks[key] = callback;

            action.started += callback;
            action.performed += callback;
            action.canceled += callback;
        }

        protected void UnregisterButtonAction(InputActionReference actionReference,
            ReactiveProperty<bool> reactiveProperty)
        {
            var action = actionReference?.action;
            if (action == null || reactiveProperty == null) return;

            var key = (action, (object)reactiveProperty);
            if (!_buttonCallbacks.TryGetValue(key, out var callback)) return;

            action.started -= callback;
            action.performed -= callback;
            action.canceled -= callback;
            _buttonCallbacks.Remove(key);
        }
    }
}