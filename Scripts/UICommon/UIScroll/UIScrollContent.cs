using DG.Tweening;
using UnityEngine;

namespace MyUtils.UICommon.UIScroll
{
    /// <summary>
    /// UI�p�[�c�̃X�N���[�����s���N���X
    /// </summary>
    public class UIScrollContent : MonoBehaviour
    {
        [Tooltip("�X�N���[������Content�I�u�W�F�N�g���A�^�b�`.")]
        [SerializeField] private RectTransform _contentTransform;
        [Tooltip("�������W.")]
        [SerializeField] private Vector2 _initialPosition;
        [Tooltip("�X�N���[���A�j���[�V������I��.")]
        [SerializeField] private Ease _scrollEase;
        [Tooltip("�X�N���[�������i���j.")]
        [SerializeField] private float _scrollOffset = 600f;
        [Tooltip("�X�N���[���ɂ����鎞��.")]
        [SerializeField] private float _animationDuration = 0.3f;

        private bool _isAnimating;

        private void Start()
        {
            _contentTransform.anchoredPosition = _initialPosition + new Vector2(_scrollOffset, 0);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.A))
                ScrollLeft();
            if (UnityEngine.Input.GetKeyDown(KeyCode.D))
                ScrollRight();
        }

        public void ScrollLeft()
        {
            ContentScroll(_scrollOffset);
        }

        public void ScrollRight()
        {
            ContentScroll(-_scrollOffset);
        }

        private void ContentScroll(float offset)
        {
            if (_isAnimating) return;
            _isAnimating = true;

            Vector2 targetPosition = _initialPosition + new Vector2(offset, 0);
            _contentTransform.DOAnchorPos(targetPosition, _animationDuration)
                .SetEase(_scrollEase)
                .OnComplete(() => _isAnimating = false)
                .Play();
        }
    }
}


