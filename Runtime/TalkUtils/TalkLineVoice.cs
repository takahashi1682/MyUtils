using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;
using R3;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    /// セリフのボイス再生機能
    /// </summary>
    public class TalkLineVoice : MonoBehaviour
    {
        [SerializeField] private TalkManager _talkManager;
        private AudioPlayer _audioPlayer;

        private void Awake()
        {
            // セリフの開始でボイスを再生
            _talkManager.LineStart.Subscribe(line =>
            {
                if (line.Voice)
                {
                    _audioPlayer = VoiceManager.Play(line.Voice);
                }
            }).AddTo(this);

            // セリフの終了でボイスを停止
            _talkManager.LineEnd.Subscribe(_ =>
            {
                _audioPlayer?.Stop();
                _audioPlayer = null;
            }).AddTo(this);
        }
    }
}
