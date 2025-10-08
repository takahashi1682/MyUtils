using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// 汎用的な移動機能
    /// </summary>
    public class ObjectMover : MonoBehaviour
    {
        [SerializeField] private Vector3 _direction = Vector3.up;
        [SerializeField] private float _speed = 5;
        [SerializeField] private bool _isGlobal;

        private void FixedUpdate()
        {
            float delta = _speed * Time.deltaTime;
            transform.position +=
                _isGlobal ?
                    _direction * delta : // グローバル座標で移動
                    transform.rotation * _direction * delta; // ローカル座標で移動
        }
    }
}