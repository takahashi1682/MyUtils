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
        IPointerEnterHandler,
        IPointerExitHandler,
        ISelectHandler,
        IDeselectHandler
    {
        [SerializeField] protected bool _isFullyExited;
        
        /// <summary>
        /// このUIが非表示になった時に呼ばれる
        /// </summary>
        protected void OnDisable()
        {
            DeselectAction();
        }

        /// <summary>
        /// このUIの選択が解除された時に呼ばれる
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDeselect(BaseEventData eventData)
        {
            DeselectAction();
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

            SelectedAction();
        }

        /// <summary>
        /// このUIからカーソルが出た時に呼ばれる
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            // 親から子オブジェクトにカーソルが移動した時は何もしない
            if (_isFullyExited && !eventData.fullyExited) return;

            DeselectAction();
        }

        /// <summary>
        /// このUIが選択された時に呼ばれる
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSelect(BaseEventData eventData)
        {
            if (!Target.IsInteractable()) return;

            SelectedAction();
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