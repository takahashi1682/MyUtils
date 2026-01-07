using System;

namespace MyUtils.Grid.Core
{
    public enum CellType
    {
        None = 0, // 未判定
        Move = 2, // 移動可能
        Unit = 5,　// ユニットあり
        Wall = 10,　// 壁
    }

    [Serializable]
    public class Cell
    {
        public int Cost;
        public CellType CellType = CellType.None;
    }
}