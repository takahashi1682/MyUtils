using System;
using UnityEngine;

namespace MyUtils
{
    [Serializable]
    public class PlayBGMSetting
    {
        public SceneReference.SceneReference SceneName;
        public EAudioPlayMode AudioPlayMode;
        public BGMResource Resource;
    }

    [CreateAssetMenu(fileName = "BGMControllerSetting", menuName = "MyUtils/AudioManager/BGMControllerSetting")]
    public class BGMControllerSetting : ScriptableObject
    {
        public PlayBGMSetting[] SceneEnterAudioSettings;
        public PlayBGMSetting[] SceneExitAudioSettings;
    }
}