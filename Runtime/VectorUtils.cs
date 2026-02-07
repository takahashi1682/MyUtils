using UnityEngine;
using System.Collections.Generic;

namespace MyUtils
{
    public static class VectorUtils
    {
        public enum Direction
        {
            Down = 0,
            Left = 1,
            Right = 2,
            Up = 3,
        }

        // 外部からの書き換えを防止しつつ公開
        public static readonly IReadOnlyList<Vector2Int> Directions2Int = new[]
        {
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
        };

        /// <summary>
        /// Direction を Vector2Int に変換（拡張メソッド）
        /// </summary>
        public static Vector2Int ToVector2Int(this Direction direction)
        {
            return direction switch
            {
                Direction.Down  => Vector2Int.down,
                Direction.Left  => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                Direction.Up    => Vector2Int.up,
                _ => Vector2Int.zero
            };
        }

        /// <summary>
        /// Direction を Vector2 に変換（拡張メソッド）
        /// </summary>
        public static Vector2 ToVector2(this Direction direction)
        {
            // Vector2Int からの暗黙的キャストを利用して共通化
            return ToVector2Int(direction);
        }

        /// <summary>
        /// ベクトルから最も近い Direction を判定する
        /// </summary>
        public static Direction ToDirection(this Vector2Int vector)
        {
            if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
            {
                return vector.x > 0 ? Direction.Right : Direction.Left;
            }
            return vector.y > 0 ? Direction.Up : Direction.Down;
        }

        /// <summary>
        /// 方向を反転させる
        /// </summary>
        public static Direction Opposite(this Direction direction)
        {
            return direction switch
            {
                Direction.Down  => Direction.Up,
                Direction.Left  => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.Up    => Direction.Down,
                _ => direction
            };
        }
    }
}