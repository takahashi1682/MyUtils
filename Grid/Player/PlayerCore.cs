using MyUtils.Grid.Map;
using UnityEngine;

namespace MyUtils.Grid.Player
{
    public class PlayerCore : MonoBehaviour
    {
        [field: SerializeField] public RouteUtils RouteUtils { get; private set; }
        [field: SerializeField] public CursorController CursorController { get; private set; }
        [field: SerializeField] public UnitMover UnitMover { get; private set; }
        [field: SerializeField] public PlayerController PlayerController { get; private set; }

        public void Initialize(MapCore mapCore)
        {
            // マップ情報の取得
            var grid = mapCore.MapLoader.Grid;
            RouteUtils.Initialize(grid);
            CursorController.Initialize(grid);
        }
    }
}