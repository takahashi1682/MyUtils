using R3;
using TNRD;
using TUtils.AudioManager;
using TUtils.AudioManager.Manager;
using UnityEngine;

namespace TUtils.UICommon
{
    public class InputTriggerSound : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IInputTriggerObservable> _inputTriggerInterface;
        [SerializeField] private AudioSetting _audioSetting;

        private void Start()
        {
            _inputTriggerInterface.Value.OnTriggerObservable
                .Subscribe(_ =>
                {
                    SEManager.Play(_audioSetting);
                }).AddTo(this);
        }
    }
}