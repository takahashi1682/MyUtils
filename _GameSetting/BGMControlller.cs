using System;
using MyUtils.AudioManager;
using MyUtils.AudioManager.Manager;
using MyUtils.SceneChangeDetector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils
{
    public class CurrentSceneBGMManager : AbstractSceneChangeDetector
    {
        [SerializeField] private AudioSetting _titleBGMResource;
        [SerializeField] private AudioSetting _outGameBGMResource;
        [SerializeField] private AudioSetting _inGameBGMResource;

        protected override void OnSceneEnter(Scene scene, LoadSceneMode mode)
        {
            if (!Enum.TryParse(scene.name, out ESceneName sceneName)) return;
            switch (sceneName)
            {
                case ESceneName.Title:
                    BGMManager.StopAll();
                    BGMManager.HasPlay(_titleBGMResource);
                    break;

                case ESceneName.MainMenu:
                case ESceneName.Result:
                    BGMManager.Stop(_inGameBGMResource.Resource);
                    BGMManager.Stop(_titleBGMResource.Resource);
                    BGMManager.HasPlay(_outGameBGMResource);
                    break;

                case ESceneName.Game:
                    BGMManager.FadeInAsync(_inGameBGMResource);
                    break;

                case ESceneName.Splash:
                case ESceneName.Loading:
                    BGMManager.StopAll();
                    break;
            }
        }

        protected override void OnSceneExit(Scene scene)
        {
            if (!Enum.TryParse(scene.name, out ESceneName sceneName)) return;

            switch (sceneName)
            {
                case ESceneName.Title:
                    BGMManager.FadeOutAsync(_titleBGMResource);
                    break;

                case ESceneName.Game:
                    BGMManager.FadeOutAsync(_inGameBGMResource);
                    break;
            }
        }
    }
}