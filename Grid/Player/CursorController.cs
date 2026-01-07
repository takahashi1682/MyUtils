using MyUtils.Grid.Core;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils.Grid.Player
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private SerializableReactiveProperty<Vector2Int> _pos = new(Vector2Int.zero);
        public ReadOnlyReactiveProperty<Vector2Int> Pos => _pos.ToReadOnlyReactiveProperty();

        private readonly Subject<Vector2Int> _clickSubject = new();
        public Observable<Vector2Int> OnClickAsObservable() => _clickSubject;

        private Camera _mainCamera;
        private Grid<int> _map;

        public void Initialize(Grid<int> map)
        {
            _pos.AddTo(this);
            _clickSubject.AddTo(this);

            _mainCamera = Camera.main;
            _map = map;
        }

        private void Update()
        {
            if (_map == null) return;

            // マウスの座標をワールド座標に変換する
            var screenPos = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var newPos = GridMath.RoundToInt(screenPos);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _clickSubject.OnNext(newPos);
            }

            _pos.Value = newPos;
        }
    }
}