using UnityEngine;

namespace MyUtils.Grid.Control
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerCore _playerCorePrefab;
        [SerializeField] private Vector2 _spawnPosition;

        private static PlayerCore _instance;

        private void Start()
        {
            if (_instance != null) return;

            _instance = Instantiate(_playerCorePrefab, _spawnPosition, Quaternion.identity);
            _instance.UnitMover.SetPosition(Vector2Int.RoundToInt(_spawnPosition));
        }
    }
}