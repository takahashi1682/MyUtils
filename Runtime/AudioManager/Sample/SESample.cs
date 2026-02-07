using System;
using Cysharp.Threading.Tasks;
using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;
using UnityEngine;

namespace MyUtils.AudioManager.Sample
{
    public class SESample : MonoBehaviour
    {
        [SerializeField] private AudioSetting _se1;

        private async void Start()
        {
            // SEの再生
            var sePlayer = SEManager.Play(_se1);
            
            // 2秒待機
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            
            // その他基本的な操作はBGMManagerと同様です。
        }
    }
}