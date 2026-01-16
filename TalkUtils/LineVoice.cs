using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;
using R3;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    ///  セリフボイス再生機能
    /// </summary>
    public class LineVoice : MonoBehaviour
    {
        private AudioPlayer _audioPlayer;

        private async void Awake()
        {
            var talk = await TalkManager.InitializeAsync;

            // 会話開始時にボイス再生
            talk.LineStart.Subscribe(lines =>
            {
                if (lines.Voice)
                {
                    _audioPlayer = VoiceManager.Play(lines.Voice);
                }
            }).AddTo(this);

            // 会話終了時にボイス停止
            talk.LineEnd.Subscribe(_ =>
            {
                _audioPlayer?.Stop();
            }).AddTo(this);
        }
    }
}