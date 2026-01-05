using UnityEngine;

namespace MyUtils.AudioManager.Core
{
    public abstract class AbstractPlayer : MonoBehaviour
    {
        public AudioSetting Setting;
        public bool IsPlayOnAwake = true;
        public bool IsPlayOnEnable;
        public bool IsStopOnDestroy = true;

        protected abstract void Play();
        protected abstract void Stop();

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