using MyUtils.Grid.Map;
using UnityEngine;

namespace MyUtils.Grid.Control
{
    public class PlayerCore : MonoBehaviour
    {
        [field: SerializeField] public PlayerController PlayerController { get; private set; }
        [field: SerializeField] public CursorController CursorController { get; private set; }
        [field: SerializeField] public RouteUtils RouteUtils { get; private set; }
        [field: SerializeField] public MapLoader MapLoader { get; private set; }
        [field: SerializeField] public UnitMover UnitMover { get; private set; }
    }
}