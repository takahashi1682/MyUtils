using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.Grid.Map
{
    [Serializable]
    public class MapLoader : MonoBehaviour
    {
        public TextAsset JsonFile;
        [SerializeField] private bool _isAutoLoad = true;

        public UniTaskCompletionSource<Grid<int>> OnLoadAsObservable = new();

        private void Awake()
        {
            if (_isAutoLoad)
                Load();
        }

        public void Load()
        {
            try
            {
                var grid = JsonUtility.FromJson<Grid<int>>(JsonFile.text);
                Debug.Log($"✅ Gridデータを読み込みました ({grid.RowCount}x{grid.ColumnCount})");
                OnLoadAsObservable.TrySetResult(grid);
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ JSONの読み込みに失敗しました: {e.Message}");
            }
        }
    }
}