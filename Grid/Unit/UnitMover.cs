using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MyUtils.Grid
{
    public class UnitMover : MonoBehaviour
    {
        public float MoveSpeed = 5f;

        private const float ArrivalTolerance = 0.05f;

        private readonly Queue<Vector2Int> _moveRoute = new();

        private readonly ReactiveProperty<bool> _isMoving = new(false);
        public ReadOnlyReactiveProperty<bool> IsMoving => _isMoving.ToReadOnlyReactiveProperty();

        private readonly Subject<Vector2Int> _onArrivedSubject = new();
        public Observable<Vector2Int> OnArrivedAsObservable => _onArrivedSubject;

        private readonly Subject<Vector2Int> _onDirectionChangedSubject = new();
        public Observable<Vector2Int> OnDirectionChangedAsObservable => _onDirectionChangedSubject;

        private Vector3 _startPoint;
        public Vector3 NextPoint { get; private set; }

        private bool _isSteppingToNext;

        private void Awake()
        {
            _isMoving.AddTo(this);
            _onArrivedSubject.AddTo(this);
            _onDirectionChangedSubject.AddTo(this);

            _startPoint = transform.position;
            NextPoint = _startPoint;
        }

        /// <summary>
        /// 移動経路を設定します。
        /// </summary>
        public void SetRoute(Queue<Vector2Int> route)
        {
            _moveRoute.Clear();

            if (route == null || route.Count == 0)
            {
                _isMoving.Value = false;
                _onArrivedSubject.OnNext(Vector2Int.FloorToInt(NextPoint));
                return;
            }

            foreach (var step in route)
                _moveRoute.Enqueue(step);

            _isMoving.Value = true;
        }

        private void FixedUpdate()
        {
            if (!_isMoving.Value) return;

            if (_isSteppingToNext)
            {
                StepTowardsTarget();
            }
            else
            {
                MoveToNextPoint();
            }
        }

        /// <summary>
        /// 現在のターゲット地点へ移動を進める。
        /// </summary>
        private void StepTowardsTarget()
        {
            if (Vector3.Distance(transform.position, NextPoint) > ArrivalTolerance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    NextPoint,
                    Time.fixedDeltaTime * MoveSpeed
                );
                return;
            }

            // 到達したらスナップ
            transform.position = NextPoint;
            _isSteppingToNext = false;

            if (_moveRoute.Count == 0)
            {
                _isMoving.Value = false;
                _onArrivedSubject.OnNext(Vector2Int.FloorToInt(NextPoint));
            }
        }

        /// <summary>
        /// 次の移動方向をキューから取得し、移動を開始する。
        /// </summary>
        private void MoveToNextPoint()
        {
            if (_moveRoute.Count == 0)
            {
                _isMoving.Value = false;
                return;
            }

            var direction = _moveRoute.Dequeue();
            _onDirectionChangedSubject.OnNext(direction);

            _startPoint = transform.position;
            var target = _startPoint + new Vector3(direction.x, direction.y, 0);

            NextPoint = target;
            _isSteppingToNext = true;
        }
    }
}