using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils.FPSController
{
    public class BasicCameraController : MonoBehaviour
    {
        public InputActionReference LookActionReference;
        public Transform TargetHorizontal;
        public Transform TargetVertical;

        [Header("Settings")]
        public bool Enabled = true;
        public float CamSpeedX = 0.1f;
        public float CamSpeedY = 0.1f;
        public float LookupLimit = -80f;
        public float LookdownLimit = 80f;

        protected float _currentPitch;
        protected float _currentYaw;

        private const string MouseDeviceName = "Mouse";
        private const float NonMouseLookScale = 1500f;

        protected virtual void Start()
        {
            if (TargetHorizontal)
            {
                _currentYaw = TargetHorizontal.localEulerAngles.y;
            }

            if (TargetVertical)
            {
                _currentPitch = NormalizePitchAngle(TargetVertical.localEulerAngles.x);
            }
        }

        protected virtual void Update()
        {
            if (!Enabled) return;
            if (!TryGetLookAction(out var lookAction)) return;

            var inputLook = ReadLookInput(lookAction);
            ApplyLook(inputLook);
        }

        protected virtual void LateUpdate()
        {
            if (!Enabled) return;

            if (TargetHorizontal)
                TargetHorizontal.localRotation = Quaternion.Euler(0, _currentYaw, 0);

            if (TargetVertical)
                TargetVertical.localRotation = Quaternion.Euler(_currentPitch, 0, 0);
        }

        private static float NormalizePitchAngle(float angle)
        {
            if (angle > 180f) angle -= 360f;
            return angle;
        }

        private bool TryGetLookAction(out InputAction lookAction)
        {
            lookAction = LookActionReference != null ? LookActionReference.action : null;
            return lookAction != null;
        }

        private static Vector2 ReadLookInput(InputAction lookAction)
        {
            var inputLook = lookAction.ReadValue<Vector2>();

            // Mouse以外はフレームレート非依存の補正をかける。
            string deviceName = lookAction.activeControl?.device?.name;
            if (deviceName != MouseDeviceName)
            {
                inputLook *= Time.deltaTime * NonMouseLookScale;
            }

            return inputLook;
        }

        private void ApplyLook(Vector2 inputLook)
        {
            _currentYaw += inputLook.x * CamSpeedX;
            _currentPitch -= inputLook.y * CamSpeedY;
            _currentPitch = Mathf.Clamp(_currentPitch, LookupLimit, LookdownLimit);
        }
    }
}