using MyUtils.AudioManager.Core;
using UnityEngine;

namespace MyUtils.AudioManager.Player
{
    public abstract class AbstractPlayer : MonoBehaviour
    {
        public bool IsPlayOnAwake = true;
        public bool IsPlayOnEnable;
        public bool IsStopOnDestroy = true;

        protected AudioPlayer _audioPlayer;

        public abstract void Play();

        public virtual void Pause()
        {
            if (_audioPlayer == null) return;
            _audioPlayer.Pause();
        }

        public virtual void UnPause()
        {
            if (_audioPlayer == null) return;
            _audioPlayer.UnPause();
        }

        public virtual void Stop()
        {
            if (_audioPlayer == null) return;
            _audioPlayer.Stop();
        }

        protected virtual void Start()
        {
            if (!IsPlayOnAwake) return;
            Play();
        }

        protected virtual void OnEnable()
        {
            if (!IsPlayOnEnable) return;
            Play();
        }

        protected virtual void OnDestroy()
        {
            if (!IsStopOnDestroy) return;
            Stop();
        }
    }
}