using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.CustomScrollView
{
    public abstract class CustomScrollViewItem : MonoBehaviour
    {
        public int Id { get; protected set; }

        public Vector3 TargetPosition;

        protected ICustomScrollView _manager;
        protected Button _target;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="index"></param>
        public virtual void Initialize(ICustomScrollView manager, int index)
        {
            _manager = manager;
            Id = index;
            _target = GetComponent<Button>();
            _target.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// リストアイテムの位置を更新
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector3 position) => transform.localPosition = position;

        /// <summary>
        /// 押下処理
        /// </summary>
        protected virtual void OnClick()
        {
            // 押された自分の番号をManagerに通知
            _manager.ButtonClickSubject.OnNext(Id);
        }

        protected virtual void Update()
        {
            // ゆっくり目的地に移動する
            transform.localPosition =
                Vector3.Lerp(transform.localPosition, TargetPosition, Time.deltaTime * _manager.MoveSpeed);
        }
    }
}