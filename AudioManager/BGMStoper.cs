using MyUtils.AudioManager.Manager;
using UnityEngine;
using UnityEngine.Audio;

namespace MyUtils.AudioManager
{
    public class BGMStoper : MonoBehaviour
    {
        public AudioResource AudioResource;
        public bool IsAllStoped;
        public bool IsStopOnStart;
        public bool IsStopOnDestroy = true;

        private void Start()
        {
            if (!IsStopOnStart) return;

            if (IsAllStoped)
            {
                BGMManager.StopAll();
            }
            else if (AudioResource != null)
            {
                BGMManager.Stop(AudioResource);
            }
        }

        private void OnDestroy()
        {
            if (!IsStopOnDestroy) return;

            if (IsAllStoped)
            {
                BGMManager.StopAll();
            }
            else if (AudioResource != null)
            {
                BGMManager.Stop(AudioResource);
            }
        }
    }
}