using UnityEngine;

namespace MyUtils.UICommon.UISelectable
{
    /// <summary>
    /// UIが選択された時にサイズを変更する機能
    /// </summary>
    public class UISelectScaler : AbstractUISelectable
    {
        [SerializeField] private float _selectedScale = 1.1f;
        private Vector3 _defaultScale;

        protected override void Awake()
        {
            base.Awake();
            _defaultScale = TargetRect.localScale;
        }

        protected override void SelectedAction()
            => TargetRect.localScale = _defaultScale * _selectedScale;
        
        protected override void SubmitAction()
        {
        }

        protected override void DeselectAction()
            => TargetRect.localScale = _defaultScale;
    }
}