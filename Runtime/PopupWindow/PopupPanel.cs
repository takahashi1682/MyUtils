using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.PopupWindow
{
    public class PopupPanel : AbstractSingletonBehaviour<PopupPanel>
    {
        [SerializeField] protected GameObject _panel;
        [SerializeField] protected TMPro.TextMeshProUGUI _messageText;
        [SerializeField] protected List<Button> _buttons;
        [SerializeField] protected Button _negativeButton;
        protected bool _isShowing;

        public virtual async UniTask<int> ShowPopupPanel(
            string message = null,
            CancellationToken ct = default)
        {
            if (_isShowing)
            {
                Debug.LogError("PopupPanel is already showing. Please wait until the current popup is closed.");
                return -1;
            }

            _isShowing = true;

            if (message != null) { _messageText.text = message; }

            _panel.SetActive(true);

            int winIndex = await UniTask.WhenAny(
                _buttons.Select(button => button.OnClickAsync(ct)).ToArray()
            );

            _panel.SetActive(false);
            _isShowing = false;
         
            return winIndex;
        }
    }
}