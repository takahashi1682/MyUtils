using Cysharp.Threading.Tasks;
using MyUtils.AudioManager.Manager;
using UnityEngine;

namespace MyUtils.AudioManager.Test
{
    public class AudioTest : MonoBehaviour
    {
        [SerializeField] private AudioSetting _audioSetting;
        [SerializeField] private AudioSetting _audioSetting2;

        private async void Start()
        {
            await UniTask.Delay(1000);
            await BGMManager.FadeInAsync(_audioSetting, duration: 3);
            await UniTask.Delay(3000);
            await BGMManager.FadeOutAsync(_audioSetting, duration: 3);
        }
    }
}