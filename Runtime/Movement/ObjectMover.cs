using UnityEngine;

namespace MyUtils.Movement
{
    /// <summary>
    /// 汎用的な移動機能
    /// </summary>
    public class ObjectMover : MonoBehaviour
    {
        public Vector3 Direction = Vector3.up;
        [SerializeField] private float _speed = 5;
        [SerializeField] private bool _isGlobal;

        private void FixedUpdate()
        {
            float delta = _speed * Time.deltaTime;
            transform.position +=
                _isGlobal ?
                    Direction * delta : // グローバル座標で移動
                    transform.rotation * Direction * delta; // ローカル座標で移動
        }
    }
}