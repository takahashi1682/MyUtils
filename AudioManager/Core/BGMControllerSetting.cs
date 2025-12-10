using System;
using UnityEditor;
using UnityEngine;

namespace MyUtils.AudioManager.Core
{
    public enum AudioPlayMode
    {
        Play, // 最初から再生
        HasPlay, // 再生中でなければ再生
        Stop,　// 停止
        FadeIn,　// フェードイン
        FadeOut　// フェードアウト
    }

    [Serializable]
    public class PlayBGMSetting
    {
        public SceneAsset SceneAsset;
        public AudioPlayMode AudioPlayMode;
        public AudioSetting AudioSetting;
    }

    [CreateAssetMenu(fileName = "BGMControllerSetting", menuName = "MyUtils/AudioManager/BGMControllerSetting")]
    public class BGMControllerSetting : ScriptableObject
    {
        [field: SerializeField] public PlayBGMSetting[] SceneEnterAudioSettings { get; private set; }
        [field: SerializeField] public PlayBGMSetting[] SceneExitAudioSettings { get; private set; }
    }
}