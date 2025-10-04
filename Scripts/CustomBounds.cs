using UnityEngine;

namespace TUtils
{
    /// <summary>
    /// 四角形の範囲を設定する機能
    /// </summary>
    public class CustomBounds : MonoBehaviour
    {
        [field: SerializeField] public Bounds Bounds { get; private set; }

        [Header("Draw Gizmos")]
        [SerializeField] private bool _isDrawGizmos = true;
        [SerializeField] private Color _color = Color.green;

        /// <summary>
        /// ギズモの描画
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!_isDrawGizmos) return;

            Gizmos.color = _color;
            Gizmos.DrawCube(Bounds.center, Bounds.size);
        }
    }
}