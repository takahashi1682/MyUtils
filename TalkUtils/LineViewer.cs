using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    ///  セリフ表示機能
    /// </summary>
    public class LineViewer : MonoBehaviour
    {
        [SerializeField] private GameObject _talkWindow;
        [SerializeField] private TMPro.TextMeshProUGUI _nameText;
        [SerializeField] private TMPro.TextMeshProUGUI _linesText;
        [SerializeField] private GameObject _nextIcon;

        [Header("表示設定")]
        [SerializeField, Tooltip("1文字ずつ表示")] private bool _isIntervalEnabled;

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            // 会話開始時にセリフを表示
            TalkManager.Singleton.LineStart.Subscribe(lines =>
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                DisplayLinesAsync(lines).Forget();
            }).AddTo(this);

            // 会話終了時にセリフを非表示
            TalkManager.Singleton.LineEnd.Subscribe(_ =>
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = null;
                _talkWindow.SetActive(false);
            }).AddTo(this);
        }

        /// <summary>
        ///  セリフを表示
        /// </summary>
        /// <param name="lineData"></param>
        private async UniTask DisplayLinesAsync(LineData lineData)
        {
            if (_nameText != null)
            {
                _nameText.text = lineData.Name;
            }

            _linesText.text = string.Empty;

            if (_nextIcon != null)
            {
                _nextIcon.SetActive(false);
            }

            _talkWindow.SetActive(true);

            if (_isIntervalEnabled)
            {
                // 1文字ずつ表示
                await DisplayTextAsync(lineData);

                if (_nextIcon != null)
                {
                    _nextIcon.SetActive(true);
                }
            }
            else
            {
                // 通常表示
                _linesText.text = lineData.Lines;
            }
        }

        // 1文字ずつ表示
        private async UniTask DisplayTextAsync(LineData lineData)
        {
            for (var i = 0; i < lineData.Lines.Length; i++)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) break;

                // 特殊文字の場合は処理
                if (lineData.Lines[i] == '\\')
                {
                    // 改行コードの場合は改行
                    if (lineData.Lines[i + 1] == 'n')
                    {
                        _linesText.text += '\n';
                        i++;
                    }
                }
                else
                {
                    // 通常の文字の場合は1文字ずつ表示
                    _linesText.text += lineData.Lines[i];
                }

                await UniTask.Delay(TimeSpan.FromSeconds(TalkManager.Singleton.OneCharInterval),
                    cancellationToken: _cancellationTokenSource.Token);
            }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}