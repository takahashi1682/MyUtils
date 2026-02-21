using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils.CameraUtils
{
    public class FPSCamera : MonoBehaviour
    {
        public InputActionReference LookActionReference;
        public Transform TargetHorizontal;
        public Transform TargetVertical;

        [Header("FPS Settings")]
        public bool Enabled = true;
        public float CamSpeedX = 2f;
        public float CamSpeedY = 0.2f;
        public float LookupLimit = -80f;
        public float LookdownLimit = 80f;

        protected InputAction _lookAction;
        protected float _currentPitch;
        protected float _currentYaw;

        protected virtual void Awake()
        {
            _lookAction = LookActionReference.action.Clone();

            if (TargetHorizontal)
            {
                _currentYaw = TargetHorizontal.eulerAngles.y;
            }

            if (TargetVertical)
            {
                _currentPitch = TargetVertical.eulerAngles.x;
                if (_currentPitch > 180) _currentPitch -= 360;
            }
        }

        protected virtual void OnEnable() => _lookAction.Enable();
        protected virtual void OnDisable() => _lookAction.Disable();

        protected virtual void Update()
        {
            if (!Enabled) return;

            var inputLook = LookActionReference.action.ReadValue<Vector2>();
            _currentYaw += inputLook.x * CamSpeedX;
            _currentPitch -= inputLook.y * CamSpeedY;
            _currentPitch = Mathf.Clamp(_currentPitch, LookupLimit, LookdownLimit);
        }

        protected virtual void LateUpdate()
        {
            if (!Enabled) return;

            if (TargetHorizontal)
                TargetHorizontal.localRotation = Quaternion.Euler(0, _currentYaw, 0);

            if (TargetVertical)
                TargetVertical.localRotation = Quaternion.Euler(_currentPitch, 0, 0);
        }
    }
}