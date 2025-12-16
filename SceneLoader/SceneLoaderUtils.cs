using System;
using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.SceneLoader
{
    public static class SceneLoaderUtils
    {
        private static bool _isRunning;

        public static async UniTask LoadSceneAsync(
            string nextScene,
            FadeSetting fadeSetting = null,
            LoadSceneMode mode = LoadSceneMode.Single,
            float minLoadingTime = 0f,
            IProgress<float> progress = null)
        {
            if (_isRunning)
            {
                Debug.LogWarning("SceneLoader: すでにロード処理が実行中です。");
                return;
            }

            _isRunning = true;
            try
            {
                await LoadSceneInternalAsync(nextScene, fadeSetting, mode, minLoadingTime, progress);
            }
            finally
            {
                _isRunning = false;
            }
        }

        private static async UniTask LoadSceneInternalAsync(
            string nextScene,
            FadeSetting fadeSetting,
            LoadSceneMode mode,
            float minLoadingTime,
            IProgress<float> progress)
        {
            var operation = SceneManager.LoadSceneAsync(nextScene, mode);

            if (operation == null)
            {
                throw new Exception($"SceneLoader: シーン '{nextScene}' の読み込みに失敗しました。シーン名が正しいか、ビルド設定に含まれているか確認してください。");
            }

            operation.allowSceneActivation = false;

            float startTime = Time.realtimeSinceStartup;

            // 0.9 までの読み込みを待機
            while (operation.progress < 0.9f)
            {
                progress?.Report(operation.progress);
                await UniTask.Delay(100, DelayType.Realtime);
            }

            progress?.Report(0.9f);

            // 最小ロード時間を保証
            float elapsed = Time.realtimeSinceStartup - startTime;
            float remaining = Mathf.Max(0, minLoadingTime - elapsed);
            await UniTask.Delay(TimeSpan.FromSeconds(remaining), DelayType.Realtime);

            // フェードアウト
            if (fadeSetting != null)
                await FadeScreenManager.BeginFadeOut(fadeSetting);

            operation.allowSceneActivation = true;

            progress?.Report(1.0f);
            await operation;

            // Additive読み込み時は明示的にアクティブ化
            if (mode == LoadSceneMode.Additive)
            {
                var loadedScene = SceneManager.GetSceneByName(nextScene);
                if (loadedScene.IsValid() && loadedScene.isLoaded)
                    SceneManager.SetActiveScene(loadedScene);
            }

            // フェードイン
            if (fadeSetting != null)
                await FadeScreenManager.BeginFadeIn(fadeSetting);
        }
    }
}