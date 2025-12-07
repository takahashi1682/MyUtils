using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.Grid
{
    public static class UnitManager
    {
        public static List<UnitIdentity> Units { get; private set; } = new();

        public static void AddUnitAt(UnitIdentity unit)
        {
            Units.Add(unit);
            Debug.Log($"✅ UnitMarkerを追加しました: {unit.GridPos}");
        }

        public static UnitIdentity GetPosUnit(Vector2Int gridPos)
            => Units.Find(unit => unit.GridPos == gridPos);


        // public readonly UniTaskCompletionSource<Grid<UnitIdentity>> OnLoadAsObservable = new();

        // [SerializeField] private MapLoader _mapLoader;
        // private Grid<UnitIdentity> _grid;
        //
        //
        // protected override async void Awake()
        // {
        //     base.Awake();
        //
        //     // var map =
        //     //     await _mapLoader.OnLoadAsObservable.Task.AttachExternalCancellation(destroyCancellationToken);
        //     // _grid = new Grid<UnitIdentity>(map.Height, map.Width);
        //     // OnLoadAsObservable.TrySetResult(_grid);
        // }

        // public void AddUnitAt(UnitIdentity unit)
        // {
        //     var gridPos = unit.GridPos;
        //
        //     if (gridPos.y < 0 || gridPos.y >= _grid.RowCount ||
        //         gridPos.x < 0 || gridPos.x >= _grid.ColumnCount)
        //     {
        //         Debug.LogWarning($"❌ UnitMarkerの位置がGridの範囲外です: {gridPos}");
        //         return;
        //     }
        //
        //     if (_grid[gridPos.y, gridPos.x] != null)
        //     {
        //         Debug.LogWarning($"❌ すでにUnitMarkerが存在します: {gridPos}");
        //         return;
        //     }
        //
        //     _grid[gridPos.y, gridPos.x] = unit;
        //     Debug.Log($"✅ UnitMarkerを追加しました: {gridPos}");
        // }
    }
}