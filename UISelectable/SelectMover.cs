using DG.Tweening;
using UnityEngine;

namespace MyUtils.UISelectable
{
    /// <summary>
    /// UIが選択された時に移動する機能
    /// </summary>
    public class SelectMover : AbstractTargetSelectable
    {
        [SerializeField] private RectTransform _moveTarget;
        [SerializeField] private Vector3 _selectedPosition = new(-30, 0);
        [SerializeField] private float _duration = 0.1f;

        private Vector3 _defaultPosition;
        private Tween _endMoveAnimation;
        private Tween _startMoveAnimation;

        protected override void Start()
        {
            base.Start();

            _defaultPosition = _moveTarget.transform.localPosition;

            SetupMoveAnimation();
        }

        protected override void SelectedAction()
        {
            _startMoveAnimation.Restart();
        }

        protected override void SubmitAction()
        {
        }

        protected override void DeselectAction()
        {
            _endMoveAnimation.Restart();
        }

        private void SetupMoveAnimation()
        {
            // _startMoveAnimation = _moveTarget.transform.DOLocalMove(_selectedPosition, _duration);
            // _startMoveAnimation.SetAutoKill(false);
            // _startMoveAnimation.SetLink(gameObject);
            // _startMoveAnimation.Pause();
            //
            // _endMoveAnimation = _moveTarget.transform.DOLocalMove(_defaultPosition, _duration);
            // _endMoveAnimation.SetAutoKill(false);
            // _endMoveAnimation.SetLink(gameObject);
            // _endMoveAnimation.Pause();
        }
    }
}