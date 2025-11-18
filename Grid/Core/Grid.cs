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
        [Serializable]
        public class Row
        {
            public T[] values;

            public Row(int columnCount)
            {
                values = new T[columnCount];
            }
        }

        public Row[] rows;

        /// <summary>
        /// グリッドを初期化します。
        /// </summary>
        /// <param name="rowCount">行数（Height/Y）</param>
        /// <param name="columnCount">列数（Width/X）</param>
        public Grid(int rowCount, int columnCount)
        {
            rows = new Row[rowCount];
            for (int y = 0; y < rowCount; y++)
            {
                rows[y] = new Row(columnCount);
            }
        }

        /// <summary>
        /// グリッドセルにアクセスします (Rows[y].values[x])
        /// </summary>
        /// <param name="y">行インデックス (Height)</param>
        /// <param name="x">列インデックス (Width)</param>
        public T this[int y, int x]
        {
            get => rows[y].values[x];
            set => rows[y].values[x] = value;
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
        /// グリッドの行数を取得します。
        /// </summary>
        public int RowCount => rows.Length;
        
        /// <summary>
        /// グリッドの列数（幅）を取得します。行がない場合は 0 を返します。
        /// </summary>
        public int ColumnCount => rows.Length > 0 ? rows[0].values.Length : 0;
        
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