using MyUtils.AudioManager.Manager;
using MyUtils.SceneChangeDetector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils
{
    public class BGMController : AbstractSceneChangeDetector
    {
        [SerializeField] public BGMControllerSetting Setting;

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
                if (setting.SceneName == null) continue;

                if (setting.SceneName.SceneName == sceneName)
                {
                    ExecuteBGMAction(setting);
                }
            }
        }

        public void PlayBGM(int index)
        {
            if (Setting == null ||
                Setting.SceneEnterAudioSettings == null ||
                index < 0 ||
                index >= Setting.SceneEnterAudioSettings.Length)
                return;

            ExecuteBGMAction(Setting.SceneEnterAudioSettings[index]);
        }

        public void PlayBGM(PlayBGMSetting setting) => ExecuteBGMAction(setting);

        /// <summary>
        /// BGM再生処理を実行する共通メソッド
        /// </summary>
        private static async void ExecuteBGMAction(PlayBGMSetting setting)
        {
            var audioSetting = setting.Resource?.ToAudioSetting();
            switch (setting.AudioPlayMode)
            {
                case EAudioPlayMode.Play:
                    if (audioSetting == null) return;
                    BGMManager.Play(audioSetting);
                    break;

                case EAudioPlayMode.PlayIfNotPlaying:
                    if (audioSetting == null) return;
                    if (!BGMManager.HasPlay(audioSetting))
                    {
                        BGMManager.Play(audioSetting);
                    }
                    break;

                case EAudioPlayMode.FadeInPlay:
                    if (audioSetting == null) return;
                    await BGMManager.FadeInAsync(audioSetting);
                    break;

                case EAudioPlayMode.Stop:
                    if (audioSetting == null) return;
                    BGMManager.Stop(audioSetting.Clip);
                    break;

                case EAudioPlayMode.FadeOutStop:
                    if (audioSetting == null) return;
                    await BGMManager.FadeOutAsync(audioSetting.Clip);
                    break;

                case EAudioPlayMode.CrossFadePlay:
                    if (audioSetting == null) return;
                    await BGMManager.CrossFadeAsync(audioSetting);
                    break;

                case EAudioPlayMode.StopAll:
                    BGMManager.StopAll();
                    break;

                case EAudioPlayMode.StopAllFadeOut:
                    await BGMManager.StopAllFadeOutAsync();
                    break;
            }
        }
    }
}