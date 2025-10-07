using MyUtils.AudioManager;
using MyUtils.AudioManager.Manager;
using R3;
using TNRD;
using UnityEngine;

namespace MyUtils.UICommon
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