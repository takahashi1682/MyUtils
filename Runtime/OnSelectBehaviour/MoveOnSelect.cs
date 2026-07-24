using System.Collections;
using UnityEngine;

namespace MyUtils.OnSelectBehaviour
{
    /// <summary>
    /// UIが選択された時に移動する機能
    /// </summary>
    public class MoveOnSelect : AbstractOnSelectBehaviour
    {
        [SerializeField] private RectTransform _moveTarget;
        [SerializeField] private Vector3 _selectedPosition = new(-30, 0);
        [SerializeField] private float _duration = 0.1f;

        private Vector3 _defaultPosition;
        private Coroutine _moveCoroutine;

        protected override void Start()
        {
            base.Start();

            _defaultPosition = _moveTarget.transform.localPosition;
        }

        protected override void SelectedAction()
        {
            // 既存のアニメーションを停止
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            _moveCoroutine = StartCoroutine(MoveToPosition(_selectedPosition, _duration));
        }

        protected override void SubmitAction()
        {
        }

        protected override void DeselectAction()
        {
            // 既存のアニメーションを停止
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            _moveCoroutine = StartCoroutine(MoveToPosition(_defaultPosition, _duration));
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = _moveTarget.transform.localPosition;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                _moveTarget.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            // 最終位置を確実に設定
            _moveTarget.transform.localPosition = targetPosition;
        }
    }
}