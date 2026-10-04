using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    /// セリフ表示機能
    /// </summary>
    public sealed class TalkLineViewer : MonoBehaviour
    {
        [SerializeField] private TalkManager _talkManager;
        [SerializeField] private GameObject _talkWindow;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _linesText;
        [SerializeField] private GameObject _nextIcon;

        [Header("表示設定")]
        [SerializeField, Tooltip("1文字ずつ表示")]
        private bool _isIntervalEnabled;

        private CancellationTokenSource _lineCts;

        private void Awake()
        {
            _talkManager.TalkStart.Subscribe(_ => _talkWindow.SetActive(true)).AddTo(this);
            _talkManager.TalkEnd.Subscribe(_ => _talkWindow.SetActive(false)).AddTo(this);

            // セリフの開始で表示し、終了で表示処理を止める
            _talkManager.LineStart.Subscribe(line => ShowLineAsync(line).Forget()).AddTo(this);
            _talkManager.LineEnd.Subscribe(_ => CancelLine()).AddTo(this);
        }

        private void OnDestroy()
        {
            CancelLine();
        }

        private async UniTask ShowLineAsync(TalkLine talkLine)
        {
            // 前のセリフの表示が残っていたら止める
            CancelLine();
            _lineCts = new CancellationTokenSource();
            var token = _lineCts.Token;

            if (_nameText != null)
            {
                _nameText.text = talkLine.Speaker;
            }

            SetNextIcon(false);

            // CSVの "\n" は改行として扱う
            _linesText.text = talkLine.Text.Replace("\\n", "\n");

            if (_isIntervalEnabled)
            {
                await ShowByCharAsync(token);
            }
            else
            {
                _linesText.maxVisibleCharacters = int.MaxValue;
            }

            SetNextIcon(true);
        }

        // 1文字ずつ表示
        private async UniTask ShowByCharAsync(CancellationToken token)
        {
            _linesText.maxVisibleCharacters = 0;
            _linesText.ForceMeshUpdate();

            int characterCount = _linesText.textInfo.characterCount;
            for (int i = 1; i <= characterCount; i++)
            {
                _linesText.maxVisibleCharacters = i;
                await UniTask.Delay(TimeSpan.FromSeconds(_talkManager.OneCharInterval), cancellationToken: token);
            }
        }

        private void CancelLine()
        {
            if (_lineCts == null) return;

            _lineCts.Cancel();
            _lineCts.Dispose();
            _lineCts = null;
        }

        private void SetNextIcon(bool isActive)
        {
            if (_nextIcon != null)
            {
                _nextIcon.SetActive(isActive);
            }
        }
    }
}
