using System;
using UnityEngine;

namespace MyUtils.Grid
{
    public static class GridMath
    {
        /// <summary>
        /// 2つのグリッド座標間のマンハッタン距離を計算します。
        /// （上下左右移動のみの距離）
        /// </summary>
        public static int GetManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
        
        /// <summary>
        /// Vector2 を四捨五入して Vector2Int に変換します。
        /// </summary>
        public static Vector2Int RoundToInt(Vector2 screenPos)
        {
            return new Vector2Int(
                (int)Math.Round(screenPos.x, 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(screenPos.y, 0, MidpointRounding.AwayFromZero)
            );
        }
        
        /// <summary>
        /// 2つの座標の差から、4方向（上・下・左・右）の単位ベクトルを返します。
        /// 対角線方向は最も距離が大きい軸に基づいて補正されます。
        /// </summary>
        public static Vector2Int GetDirection4(Vector2Int from, Vector2Int to)
        {
            var diff = to - from;

            // どちらの軸の差が大きいかで主方向を決定
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                return diff.x > 0 ? Vector2Int.right : Vector2Int.left;
            }

            return diff.y > 0 ? Vector2Int.up : Vector2Int.down;
        }
    }
}