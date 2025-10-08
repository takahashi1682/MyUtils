using UnityEngine;

namespace MyUtils.UISelectable
{
    /// <summary>
    /// UIが選択された時にサイズを変更する機能
    /// </summary>
    public class SelectScaler : AbstractTargetSelectable
    {
        [SerializeField] private float _selectedScale = 1.1f;
        private Vector3 _defaultScale;

        protected override void Awake()
        {
            base.Awake();
            _defaultScale = _target.localScale;
        }

        protected override void SelectedAction()
            => _target.localScale = _defaultScale * _selectedScale;

        protected override void SubmitAction()
        {
        }

        protected override void DeselectAction()
            => _target.localScale = _defaultScale;
    }
}