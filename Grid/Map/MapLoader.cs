using System;
using R3;
using UnityEngine;

namespace MyUtils.Grid.Map
{
    public class MapLoader : AbstractSingletonBehaviour<MapLoader>
    {
        [SerializeField] private TextAsset _defaultJson;
        [SerializeField] private bool _isAutoLoad = true;

        public readonly BehaviorSubject<Grid<int>> OnLoadAsObservable = new(null);

        protected override void Awake()
        {
            base.Awake();
            OnLoadAsObservable.AddTo(this);

            if (_isAutoLoad)
                Load(_defaultJson);
        }

        public void Load(TextAsset gridJson)
        {
            try
            {
                var grid = JsonUtility.FromJson<Grid<int>>(gridJson.text);
                Debug.Log($"✅ Gridデータを読み込みました ({grid.RowCount}x{grid.ColumnCount})");

                OnLoadAsObservable.OnNext(grid);
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ JSONの読み込みに失敗しました: {e.Message}");
            }
        }
    }
}