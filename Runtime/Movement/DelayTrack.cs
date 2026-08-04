using UnityEngine;

namespace MyUtils.Movement
{
    /// <summary>
    /// 対象(Target)の位置・回転に遅延して追従する機能
    /// </summary>
    public class DelayTrack : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [field: SerializeField] public float Speed { get; set; } = 5f;
        public bool IsMove = true;
        public bool IsLook = true;

        private void FixedUpdate()
        {
            if (_target == null) return;

            float t = Speed * Time.fixedDeltaTime;

            if (IsMove)
                transform.position = Vector3.Lerp(transform.position, _target.position, t);

            if (IsLook)
                transform.rotation = Quaternion.Lerp(transform.rotation, _target.rotation, t);
        }
    }
}
