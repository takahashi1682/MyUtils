using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils.FPSController
{
    public class BasicPlayerMove : MonoBehaviour
    {
        [Header("References")]
        public InputActionReference MoveActionReference;
        public Transform Target;

        [Header("Settings")]
        public bool Enabled = true;
        public float MoveSpeed = 5f;

        protected virtual void Update()
        {
            if (!Enabled) return;
            if (!TryGetMoveAction(out var moveAction)) return;

            var inputMove = ReadMoveInput(moveAction);
            if (inputMove.sqrMagnitude <= 0f) return;

            ApplyMove(inputMove);
        }

        private bool TryGetMoveAction(out InputAction moveAction)
        {
            moveAction = MoveActionReference != null ? MoveActionReference.action : null;
            return moveAction != null;
        }

        private static Vector2 ReadMoveInput(InputAction moveAction)
        {
            var inputMove = moveAction.ReadValue<Vector2>();

            // 斜め移動時の速度増加を抑える。
            return Vector2.ClampMagnitude(inputMove, 1f);
        }

        private void ApplyMove(Vector2 inputMove)
        {
            var moveDirection = new Vector3(inputMove.x, 0f, inputMove.y);
            moveDirection = Target.TransformDirection(moveDirection);
            transform.position += moveDirection * (MoveSpeed * Time.deltaTime);
        }
    }
}