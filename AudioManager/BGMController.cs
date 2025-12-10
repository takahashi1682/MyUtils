using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;
using MyUtils.SceneChangeDetector;
using UnityEngine.SceneManagement;

namespace MyUtils.AudioManager
{
    public class BGMController : AbstractSceneChangeDetector
    {
        public BGMControllerSetting Setting;

        protected override void OnSceneEnter(Scene scene, LoadSceneMode mode)
        {
            if (Setting == null) return;
            ProcessBGMBySceneName(Setting.SceneEnterAudioSettings, scene.name);
        }

        protected override void OnSceneExit(Scene scene)
        {
            if (Setting == null) return;
            ProcessBGMBySceneName(Setting.SceneExitAudioSettings, scene.name);
        }

        /// <summary>
        /// シーン名に基づいてBGM設定を処理する
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="sceneName"></param>
        private static void ProcessBGMBySceneName(PlayBGMSetting[] settings, string sceneName)
        {
            if (settings == null || settings.Length == 0) return;

            foreach (var setting in settings)
            {
                if (setting.SceneAsset == null) continue;

                if (setting.SceneAsset.name == sceneName)
                {
                    ExecuteBGMAction(setting);
                }
            }
        }

        /// <summary>
        /// BGM再生処理を実行する共通メソッド
        /// </summary>
        private static void ExecuteBGMAction(PlayBGMSetting setting)
        {
            switch (setting.AudioPlayMode)
            {
                case AudioPlayMode.Play:
                    BGMManager.Play(setting.AudioSetting);
                    break;

                case AudioPlayMode.HasPlay:
                    BGMManager.HasPlay(setting.AudioSetting);
                    break;

                case AudioPlayMode.Stop:
                    BGMManager.Stop(setting.AudioSetting.Resource);
                    break;

                case AudioPlayMode.FadeIn:
                    BGMManager.FadeInAsync(setting.AudioSetting);
                    break;

                case AudioPlayMode.FadeOut:
                    BGMManager.FadeOutAsync(setting.AudioSetting.Resource);
                    break;
            }
        }
    }
}