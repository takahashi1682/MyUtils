using System;
using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using MyUtils.SceneLoader;
using UnityEngine;

namespace MyUtils
{
    public class SplashController : MonoBehaviour
    {
        [SerializeField] private float _waitTimeSecond = 1f;
        [SerializeField] private SceneReference.SceneReference _nextScene;
        [SerializeField] private FadeSetting _fadeInSetting;
        [SerializeField] private FadeSetting _fadeOutSetting;

        private async void Start()
        {
            await FadeScreenManager.BeginFadeIn(_fadeInSetting);
            await UniTask.Delay(TimeSpan.FromSeconds(_waitTimeSecond), cancellationToken: destroyCancellationToken);
            await SceneLoaderUtils.LoadSceneAsync(_nextScene.SceneName, _fadeOutSetting);
        }
    }
}