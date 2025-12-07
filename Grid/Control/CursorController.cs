using System;
using Cysharp.Threading.Tasks;
using MyUtils.Grid.Map;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUtils.Grid
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private MapLoader _mapLoader;

        [SerializeField] private SerializableReactiveProperty<Vector2Int> _pos = new(Vector2Int.zero);
        public ReadOnlyReactiveProperty<Vector2Int> Pos => _pos.ToReadOnlyReactiveProperty();

        private readonly Subject<Vector2Int> _clickSubject = new();
        public Observable<Vector2Int> OnClickAsObservable() => _clickSubject;

        private Camera _mainCamera;
        private Grid<int> _map;

        private async void Start()
        {
            _mainCamera = Camera.main;
            _pos.AddTo(this);
            _clickSubject.AddTo(this);

            _map = await _mapLoader.OnLoadAsObservable.Task.AttachExternalCancellation(destroyCancellationToken);
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