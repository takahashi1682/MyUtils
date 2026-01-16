using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.PopupWindow
{
    public enum PopupMode
    {
        SingleButton,
        DoubleButton,
    }

    public class PopupPanel : AbstractSingletonBehaviour<PopupPanel>
    {
        [SerializeField] protected GameObject _panel;
        [SerializeField] protected TMPro.TextMeshProUGUI _messageText;
        [SerializeField] protected Button _positiveButton;
        [SerializeField] protected Button _negativeButton;
        protected bool _isShowing;

        public virtual async UniTask<bool> ShowPopupPanel(
            string message = null,
            PopupMode mode = PopupMode.DoubleButton,
            CancellationToken ct = default)
        {
            if (_isShowing)
            {
                Debug.LogError("Popup is already showing.");
                return false;
            }

            _isShowing = true;

            if (message != null) { _messageText.text = message; }

            _panel.SetActive(true);

            bool isDouble = mode == PopupMode.DoubleButton;
            if (_negativeButton != null) _negativeButton.gameObject.SetActive(isDouble);

            bool result;

            if (isDouble)
            {
                int winIndex = await UniTask.WhenAny(
                    _positiveButton.OnClickAsync(ct),
                    _negativeButton.OnClickAsync(ct));

                result = winIndex == 0;
            }
            else
            {
                await _positiveButton.OnClickAsync(ct);
                result = true;
            }

            ClosePanel();
            return result;
        }

        protected virtual void ClosePanel()
        {
            _panel.SetActive(false);
            _isShowing = false;
        }
    }
}