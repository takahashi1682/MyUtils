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
        public bool Enable = true;
        public float CamSpeedX = 2f;
        public float CamSpeedY = 0.2f;
        public float LookupLimit = -80f;
        public float LookdownLimit = 80f;

        private float _currentPitch;
        private float _currentYaw;

        private void Start()
        {
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

        private void OnEnable()
        {
            if (LookActionReference == null) return;
            LookActionReference.action.Enable();
        }

        private void OnDisable()
        {
            if (LookActionReference == null) return;
            LookActionReference.action.Disable();
        }

        private void Update()
        {
            if (!Enable) return;

            var inputLook = LookActionReference.action.ReadValue<Vector2>();
            _currentYaw += inputLook.x * CamSpeedX;
            _currentPitch -= inputLook.y * CamSpeedY;
            _currentPitch = Mathf.Clamp(_currentPitch, LookupLimit, LookdownLimit);
        }

        private void LateUpdate()
        {
            if (!Enable) return;

            if (TargetHorizontal)
                TargetHorizontal.localRotation = Quaternion.Euler(0, _currentYaw, 0);

            if (TargetVertical)
                TargetVertical.localRotation = Quaternion.Euler(_currentPitch, 0, 0);
        }
    }
}