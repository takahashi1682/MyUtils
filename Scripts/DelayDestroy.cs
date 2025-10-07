using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// 指定時間後に削除する機能
    /// </summary>
    public class DelayDestroy : MonoBehaviour
    {
        [SerializeField] private float _destroyTime = 10f;

        private void Start()
        {
            Destroy(gameObject, _destroyTime);
        }
    }
}