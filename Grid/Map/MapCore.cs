using System;
using MyUtils.Grid.Player;
using UnityEngine;

namespace MyUtils.Grid.Map
{
    public class MapCore : MonoBehaviour
    {
        [field: SerializeField] public MapData MapData { get; private set; }
        [field: SerializeField] public MapLoader MapLoader { get; private set; }

        [SerializeField] private PlayerCore _playerCorePrefab;
        [SerializeField] private Vector2 _spawnPosition;
        private static PlayerCore _instance;

        private void Awake()
        {
            MapLoader.Initialize(MapData.GridData);

            // プレイヤーのスポーン
            if (_instance == null)
            {
                _instance = Instantiate(_playerCorePrefab, Vector2.zero, Quaternion.identity);
                _instance.UnitMover.SetPosition(Vector2Int.RoundToInt(_spawnPosition));
            }

            // プレイヤーの初期化
            _instance.Initialize(this);
        }
    }
}