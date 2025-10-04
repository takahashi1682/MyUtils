using System.Collections.Generic;
using R3;
using UnityEngine;

namespace TUtils.CustomScrollView
{
    public interface ICustomScrollView
    {
        Subject<int> ButtonClickSubject { get; }
        float MoveSpeed { get; }
    }

    public abstract class CustomScrollView<T> : MonoBehaviour, ICustomScrollView where T : CustomScrollViewItem
    {
        public Subject<int> ButtonClickSubject { get; } = new();
        public Vector2 Spacing = new(250, 200);

        [field: SerializeField] public float MoveSpeed { get; set; } = 5;

        protected List<T> _viewItems;

        [SerializeField] protected SerializableReactiveProperty<int> _selectId = new();

        /// <summary>
        /// リストアイテムの生成
        /// </summary>
        protected abstract List<T> CreateAndInitializeList();

        protected virtual void Start()
        {
            _selectId.AddTo(this);
            ButtonClickSubject.AddTo(this);

            // メニューアイテムの生成と初期化
            _viewItems = CreateAndInitializeList();

            // マウスホイールの入力を取得
            // UnityEngine.Input.mouseScrollDelta
            //     .Where(x => x != 0)
            //     .Select(Mathf.Sign)
            //     .Subscribe(x =>
            //     {
            //         var v = (int)Mathf.Clamp(_selectId.CurrentValue - x, 0, _viewItems.Count - 1);
            //         if (_selectId.CurrentValue != v)
            //         {
            //             ButtonClickSubject.OnNext(v);
            //         }
            //     }).AddTo(this);

            // 初期位置を設定
            UpdateListPosition(true);

            // 選択されたIDが変更されたら、リストの位置を更新
            _selectId.Subscribe(_ => UpdateListPosition(false)).AddTo(this);

            // ボタンが押されたら、選択されたIDを取得
            ButtonClickSubject.Subscribe(x =>
            {
                if (x == _selectId.CurrentValue)
                    Submit(x);
                else
                    Select(x);

                _selectId.Value = x;
            }).AddTo(this);
        }

        /// <summary>
        /// リストアイテムの位置を更新
        /// </summary>
        protected virtual void UpdateListPosition(bool isSet)
        {
            foreach (var item in _viewItems)
            {
                var indexDiff = item.Id - _selectId.CurrentValue;
                var x = Spacing.x * Mathf.Abs(indexDiff);
                var y = -Spacing.y * indexDiff;
                if (isSet)
                    item.SetPosition(new Vector3(x, y));
                else
                    item.TargetPosition = new Vector3(x, y);
            }
        }

        /// <summary>
        /// リストアイテムが2回連続で選択された時
        /// </summary>
        protected virtual void Submit(int id)
        {
            Debug.Log($"{id}で決定された");
        }

        /// <summary>
        /// リストアイテムが選択されたときの処理
        /// </summary>
        protected virtual void Select(int id)
        {
            Debug.Log($"{id}が選択された");
        }
    }
}