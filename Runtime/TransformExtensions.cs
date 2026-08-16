using UnityEngine;

namespace MyUtils
{
    public static class TransformExtensions
    {
        // Transformのワールド座標を盤面のグリッド座標(整数)に変換する
        public static Vector2Int ToVector2Int(this Transform transform)
        {
            return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }
    }
}
