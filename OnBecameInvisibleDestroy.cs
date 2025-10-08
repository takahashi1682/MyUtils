using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// 画面外に出たら削除する機能
    /// </summary>
    public class OnBecameInvisibleDestroy : MonoBehaviour
    {
        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}