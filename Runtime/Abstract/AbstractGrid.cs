using System;
using UnityEngine;

namespace MyUtils.Abstract
{
    // 座標(Pos.yは0以下)でインデックスされたセル配列の共通実装
    public abstract class AbstractGrid<TCell>
    {
        public TCell[] Cells { get; }
        public int Width { get; }
        public int Height { get; }

        protected AbstractGrid(int width, int height, Func<Vector2Int, TCell> createCell)
        {
            Width = width;
            Height = height;
            Cells = new TCell[width * height];

            for (var i = 0; i < Cells.Length; i++)
            {
                var pos = new Vector2Int(i % Width, -i / Width);
                Cells[i] = createCell(pos);
            }
        }

        public TCell GetCell(Vector2Int pos) => GetCell(pos.x, pos.y);

        public TCell GetCell(int x, int y) => Cells[ToIndex(x, -y)];

        public TCell GetCell(int index) => Cells[index];

        private int ToIndex(int x, int y) => y * Width + x;
    }
}
