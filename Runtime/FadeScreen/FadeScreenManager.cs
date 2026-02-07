using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.FadeScreen
{
    /// <summary>
    /// 画面フェードを制御するシングルトンクラス
    /// </summary>
    public class FadeScreenManager
    {
        private const int SortingOrder = 9999;

        private static FadeScreenManager _instance;
        private static FadeScreenManager Instance => _instance ??= new FadeScreenManager();

        private Image _fadePanel;
        private GameObject _canvasObject;
        private CancellationTokenSource _cts;

        private FadeScreenManager()
        {
            InitializeCanvas();
            ResetToken();
        }

        private void InitializeCanvas()
        {
            _canvasObject = new GameObject("ScreenFadeCanvas")
            {
                layer = LayerMask.NameToLayer("UI")
            };

            Object.DontDestroyOnLoad(_canvasObject);

            var canvas = _canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SortingOrder;

            canvas.gameObject.AddComponent<GraphicRaycaster>();

            _fadePanel = _canvasObject.AddComponent<Image>();
            _fadePanel.raycastTarget = true;

            SetAlpha(0f);
        }

        private void SetAlpha(float alpha)
        {
            if (_fadePanel == null) return;
            var color = _fadePanel.color;
            color.a = alpha;
            _fadePanel.color = color;
        }

        private void ResetToken()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }

        /// <summary>画面フェードアウト（黒くなる）</summary>
        public static async UniTask BeginFadeOut(FadeSetting fadeSetting = null)
        {
            fadeSetting ??= FadeSetting.Default;
            Instance.ResetToken();
            Instance._fadePanel.raycastTarget = true;
            await Instance.FadeAsync(fadeSetting, 0f, 1f);
            Instance._fadePanel.raycastTarget = false;
        }

        /// <summary>画面フェードイン（透明になる）</summary>
        public static async UniTask BeginFadeIn(FadeSetting fadeSetting = null)
        {
            fadeSetting ??= FadeSetting.Default;
            Instance.ResetToken();
            Instance._fadePanel.raycastTarget = true;
            await Instance.FadeAsync(fadeSetting, 1f, 0f);
            Instance._fadePanel.raycastTarget = false;
        }

        private async UniTask FadeAsync(FadeSetting setting, float from, float to)
        {
            if (_fadePanel == null) return;

            _fadePanel.color = setting.Color;
            var startTime = Time.realtimeSinceStartup;
            var duration = Mathf.Max(0.01f, setting.Duration); // 最低0.01秒

            while (!_cts.IsCancellationRequested && Time.realtimeSinceStartup - startTime < duration)
            {
                var elapsed = Time.realtimeSinceStartup - startTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var alpha = Mathf.Lerp(from, to, t);
                SetAlpha(alpha);

                await UniTask.Delay(10, DelayType.Realtime, cancellationToken: _cts.Token);
            }

            SetAlpha(to);
        }
    }
}