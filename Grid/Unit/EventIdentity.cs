using UnityEngine;

namespace MyUtils.Grid
{
    public class EventIdentity : UnitIdentity
    {
        [Header("Draw Gizmos")]
        [SerializeField] private bool _isDrawGizmos = true;
        [SerializeField] private Color _color = new(1, 1, 0, 0.5f);

        /// <summary>
        /// ギズモの描画
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!_isDrawGizmos) return;

            Gizmos.color = _color;
            Gizmos.DrawCube(transform.position, Vector2.one);
        }
    }
}