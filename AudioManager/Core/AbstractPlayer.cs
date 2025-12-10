using UnityEngine;

namespace MyUtils.AudioManager.Core
{
    public abstract class AbstractPlayer : MonoBehaviour
    {
        public AudioSetting Setting;
        public bool IsPlayOnStart = true;
        public bool IsPlayOnEnable;
        public bool IsStopOnDestroy = true;

        protected AudioPlayer _currentPlayer;

        protected abstract AudioPlayer Play();

        protected virtual void Start()
        {
            if (IsPlayOnStart) _currentPlayer = Play();
        }

        protected virtual void OnEnable()
        {
            if (IsPlayOnEnable) _currentPlayer = Play();
        }

        protected virtual void OnDestroy()
        {
            if (IsStopOnDestroy && _currentPlayer) _currentPlayer.Stop();
        }
    }
}