using UnityEngine;

namespace MyUtils.Grid
{
    [System.Serializable]
    public class Area : Grid<Cell>
    {
        public Area(int rowCount, int columnCount) : base(rowCount, columnCount)
        {
            ForEachCell((y, x, _) => this[y, x] = new Cell());
        }

        public void ForEachCell(System.Action<int, int, Cell> action)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    action(y, x, this[y, x]);
                }
            }
        }

        public new Cell this[Vector2Int pos]
        {
            get => this[pos.y, pos.x];
            set => this[pos.y, pos.x] = value;
        }

        public bool IsInMap(Vector2Int pos) => IsInMap(pos.x, pos.y);

        public bool IsInMap(int x, int y)
        {
            return 0 <= y && y < Height && 0 <= x && x < Width;
        }
    }
}