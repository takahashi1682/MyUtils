using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.Grid
{
    public class UnitManager : AbstractSingletonBehaviour<UnitManager>
    {
        public readonly UniTaskCompletionSource<Grid<UnitMarker>> OnLoadAsObservable = new();

        [SerializeField] private MapLoader _mapLoader;
        private Grid<UnitMarker> _grid;


        protected override async void Awake()
        {
            base.Awake();

            var map =
                await _mapLoader.OnLoadAsObservable.Task.AttachExternalCancellation(destroyCancellationToken);
            _grid = new Grid<UnitMarker>(map.Height, map.Width);
            OnLoadAsObservable.TrySetResult(_grid);
        }

        public void AddUnitAt(UnitMarker unit)
        {
            var gridPos = unit.GridPos;

            if (gridPos.y < 0 || gridPos.y >= _grid.RowCount ||
                gridPos.x < 0 || gridPos.x >= _grid.ColumnCount)
            {
                Debug.LogWarning($"❌ UnitMarkerの位置がGridの範囲外です: {gridPos}");
                return;
            }

            if (_grid[gridPos.y, gridPos.x] != null)
            {
                Debug.LogWarning($"❌ すでにUnitMarkerが存在します: {gridPos}");
                return;
            }

            _grid[gridPos.y, gridPos.x] = unit;
            Debug.Log($"✅ UnitMarkerを追加しました: {gridPos}");
        }
    }
}