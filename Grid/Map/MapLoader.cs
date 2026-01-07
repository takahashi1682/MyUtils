using System;
using MyUtils.Grid.Core;
using UnityEngine;

namespace MyUtils.Grid.Map
{
    public class MapLoader : MonoBehaviour
    {
        public Grid<int> Grid { get; private set; }

        public void Initialize(TextAsset mapJson)
        {
            try
            {
                Grid = JsonUtility.FromJson<Grid<int>>(mapJson.text);
                Debug.Log($"✅ Gridデータを読み込みました ({Grid.RowCount}x{Grid.ColumnCount})");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ JSONの読み込みに失敗しました: {e.Message}");
            }
        }
    }
}