using System;
using UnityEngine;

namespace MyUtils.Grid
{
    /// <summary>
    /// 2次元グリッドデータ構造体。UnityのJsonUtilityでシリアライズ可能にするため、
    /// 行を[Serializable]なインナークラスとして持つ。
    /// </summary>
    [Serializable]
    public class Grid<T>
    {
        public int RowCount;
        public int ColumnCount;
        public T[] Data;

        /// <summary>
        /// グリッドを初期化します。
        /// </summary>
        /// <param name="rowCount">行数（Height/Y）</param>
        /// <param name="columnCount">列数（Width/X）</param>
        public Grid(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            Data = new T[rowCount * columnCount];
        }

        /// <summary>
        /// グリッドセルにアクセスします (Rows[y].values[x])
        /// </summary>
        /// <param name="y">行インデックス (Height)</param>
        /// <param name="x">列インデックス (Width)</param>
        public T this[int y, int x]
        {
            get => Data[y * ColumnCount + x];
            set => Data[y * ColumnCount + x] = value;
        }

        /// <summary>
        /// グリッドセルに Vector2Int 座標でアクセスします。
        /// </summary>
        /// <param name="pos">Y=pos.y, X=pos.x として扱われます。</param>
        public T this[Vector2Int pos]
        {
            get => this[pos.y, pos.x];
            set => this[pos.y, pos.x] = value;
        }

        /// <summary>
        /// グリッドの列数（幅）を取得します。行がない場合は 0 を返します。
        /// </summary>
        // public int ColumnCount => rows.Length > 0 ? rows[0].values.Length : 0;
        public bool IsInside(Vector2Int pos)
        {
            return 0 <= pos.x && pos.x < ColumnCount &&
                   0 <= pos.y && pos.y < RowCount;
        }

        // 互換性プロパティ
        public int Width => ColumnCount;
        public int Height => RowCount;
    }
}