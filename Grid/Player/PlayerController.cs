using System.Collections.Generic;
using MyUtils.Grid.Map;
using UnityEngine;
using R3;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyUtils.Grid
{
    public class PlayerController : AbstractSingletonBehaviour<PlayerController>
    {
        public static bool IsActive { get; set; } = true;

        [SerializeField] private UnitIdentity _unitIdentity;
        [SerializeField] private ParticleSystem _moveMarker;
        [SerializeField] private UnitMover _unitMover;
        [SerializeField] private UnitAnimation _unitAnimation;
        [SerializeField] private RouteUtils _routeUtils;
        [SerializeField] private CursorController _cursorController;

        [SerializeField] private bool _isCostDisplay;

        private Area _currentMoveArea;
        private Vector2Int _lastDirectionalPos;
        private bool _isBestRoute;

        private void Start()
        {
            UnitManager.Units.Add(_unitIdentity);

            _unitMover.IsMoving.Subscribe(x =>
            {
                _unitAnimation.IsRun = x;
                if (x)
                {
                    _moveMarker.Simulate(_moveMarker.main.duration, true, true);
                    _moveMarker.Play(true);
                }
                else
                {
                    _moveMarker.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }).AddTo(this);

            _unitMover.OnDirectionChangedAsObservable.Subscribe(x =>
            {
                _unitAnimation.SetDirection(x);
            }).AddTo(this);

            _unitMover.OnArrivedAsObservable.SubscribeAwait(async (pos, _) =>
            {
                // 最適ルートでない場合、移動終了時に向きを更新
                if (!_isBestRoute)
                {
                    _unitAnimation.SetDirection(_lastDirectionalPos);

                    // 移動先にユニットが存在する場合は中断
                    var targetPos = pos + _lastDirectionalPos;
                    var targetUnit = UnitManager.GetPosUnit(targetPos);
                    if (targetUnit != null)
                    {
                        // ユニットのイベントを発火
                        if (targetUnit.TryGetComponent(out UnitInteraction interaction))
                        {
                            IsActive = false;

                            var lastDirectionalPos = Vector2Int.down;
                            if (targetUnit.TryGetComponent(out UnitAnimation anim))
                            {
                                lastDirectionalPos = anim.LastDirectionalPos;
                                anim.SetDirection(-_lastDirectionalPos);
                            }

                            await interaction.OnInteract(_unitIdentity);

                            if (anim != null)
                            {
                                anim.SetDirection(lastDirectionalPos);
                            }

                            IsActive = true;
                        }
                    }
                }
            }).AddTo(this);

            // カーソルクリック時にルートを決定
            _cursorController.OnClickAsObservable()
                .Where(_ => IsActive)
                .Subscribe(HandleClick)
                .AddTo(this);
        }

        /// <summary>
        /// クリック位置に応じてルートを計算・移動開始。
        /// </summary>
        private void HandleClick(Vector2Int clickedPos)
        {
            // プレイヤー位置をグリッド座標へ変換
            Vector2Int playerGridPos = new((int)_unitMover.NextPoint.x, (int)_unitMover.NextPoint.y);

            // 移動可能エリアを計算
            _currentMoveArea = _routeUtils.GetMoveArea(playerGridPos);

            // クリック地点が移動可能セルかを判定
            _isBestRoute = _currentMoveArea.IsInside(clickedPos)
                           && _currentMoveArea[clickedPos].CellType == CellType.Move;

            Queue<Vector2Int> route;
            if (_isBestRoute)
            {
                // 目的地が移動可能セルなら最短経路を取得
                route = RouteUtils.GetAreaBestRoute(_currentMoveArea, playerGridPos, clickedPos);
            }
            else
            {
                // 移動不可セル → 近傍の最も近い移動可能地点を探索
                var nearestPos = RouteUtils.FindNearestMovablePoint(_currentMoveArea, clickedPos);
                route = RouteUtils.GetAreaBestRoute(_currentMoveArea, playerGridPos, nearestPos);

                _lastDirectionalPos = GridMath.GetDirection4(nearestPos, clickedPos);

                // 経路が存在しない場合は向きだけ更新
                // if (route.Count == 0)
                // {
                //     _unitMoveAnim.SetDirection(_lastDirectionalPos);
                //
                //     // 移動先にユニットが存在する場合は中断
                //     var target = playerGridPos + _lastDirectionalPos;
                //     if (_units[target] is var unit && unit != null)
                //     {
                //         // ユニットのイベントを発火
                //         if (unit.TryGetComponent(out UnitInteraction interaction))
                //         {
                //             interaction.OnInteract.Invoke();
                //         }
                //
                //         return;
                //     }
                // }
            }

            // 経路をセットして移動開始
            _unitMover.SetRoute(route);

            // マーカーをターゲットに移動
            _moveMarker.transform.position = new Vector3(clickedPos.x, clickedPos.y, 0);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_isCostDisplay || _currentMoveArea == null)
                return;

            Gizmos.color = Color.green;
            Handles.color = Color.white;

            for (int y = 0; y < _currentMoveArea.Height; y++)
            {
                for (int x = 0; x < _currentMoveArea.Width; x++)
                {
                    var cell = _currentMoveArea[y, x];
                    if (cell == null) continue;

                    Vector3 pos = new(x, y, 0);

                    Gizmos.DrawWireCube(pos, Vector3.one * 0.95f);
                    Handles.Label(pos + Vector3.up * 0.1f, cell.Cost.ToString());
                }
            }
        }
#endif
    }
}