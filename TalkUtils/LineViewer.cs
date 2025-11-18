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
        [SerializeField] protected GameObject _talkWindow;
        [SerializeField] protected TMPro.TextMeshProUGUI _nameText;
        [SerializeField] protected TMPro.TextMeshProUGUI _linesText;
        [SerializeField] protected GameObject _nextIcon;

        [Header("表示設定")]
        [SerializeField, Tooltip("1文字ずつ表示")] protected bool _isIntervalEnabled;

        protected CancellationTokenSource _cancellationTokenSource;

        protected virtual void Awake()
        {
            TalkManager.Singleton.TalkStart.Subscribe(_ =>
            {
                _talkWindow.SetActive(true);
            }).AddTo(this);

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
            }).AddTo(this);

            TalkManager.Singleton.TalkEnd.Subscribe(_ =>
            {
                _talkWindow.SetActive(false);
            }).AddTo(this);
        }

        /// <summary>
        ///  セリフを表示
        /// </summary>
        /// <param name="lineData"></param>
        protected virtual async UniTask DisplayLinesAsync(LineData lineData)
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
        protected virtual async UniTask DisplayTextAsync(LineData lineData)
        {
            for (int i = 0; i < lineData.Lines.Length; i++)
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

        protected virtual void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}