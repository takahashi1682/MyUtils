using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyUtils.UISelectable
{
    /// <summary>
    /// UIが選択された時に何かする機能
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public abstract class AbstractTargetSelectable : AbstractTargetBehaviour<Selectable>,
        ISubmitHandler,
        IPointerClickHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler
    {
        [SerializeField] protected bool _isFullyExited = true;
        private readonly ReactiveProperty<bool> _isSelected = new(false);

        protected override void Start()
        {
            base.Start();
            _isSelected.AddTo(this);

            _isSelected
                .Skip(1) // 初期値の変更を無視
                .Subscribe(isSelected =>
                {
                    if (isSelected)
                    {
                        SelectedAction();
                    }
                    else
                    {
                        DeselectAction();
                    }
                }).AddTo(this);
        }

        /// <summary>
        /// このUIが非表示になった時に呼ばれる
        /// </summary>
        protected void OnDisable()
        {
            _isSelected.Value = false;
        }

        /// <summary>
        /// このUIの選択が解除された時に呼ばれる
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDeselect(BaseEventData eventData)
        {
            _isSelected.Value = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Target.IsInteractable()) return;

            SubmitAction();
        }

        /// <summary>
        /// このUIにカーソルが入った時に呼ばれる
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Target.IsInteractable()) return;

            _isSelected.Value = true;
        }

        /// <summary>
        /// このUIからカーソルが出た時に呼ばれる
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            // カーソルが完全に出た場合のみ選択解除
            if (_isFullyExited && eventData.fullyExited)
            {
                Debug.Log(2);
                _isSelected.Value = false;
            }
        }

        /// <summary>
        /// このUIが選択された時に呼ばれる
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect(BaseEventData eventData)
        {
            if (!Target.IsInteractable()) return;

            _isSelected.Value = true;
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!Target.IsInteractable()) return;

            SubmitAction();
        }

        /// <summary>
        /// 選択された時に呼ばれる処理
        /// </summary>
        protected abstract void SelectedAction();

        /// <summary>
        /// クリックされた時に呼ばれる処理
        /// </summary>
        protected abstract void SubmitAction();

        /// <summary>
        /// 選択されなくなった時に呼ばれる処理
        /// </summary>
        protected abstract void DeselectAction();
    }
}