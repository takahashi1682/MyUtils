using System;
using Cysharp.Threading.Tasks;
using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;
using UnityEngine;

namespace MyUtils.AudioManager.Sample
{
    public class BGMSample : MonoBehaviour
    {
        [SerializeField] private AudioSetting _bgm1;
        [SerializeField] private AudioSetting _bgm2;

        private async void Start()
        {
            // -------------------通常再生------------------------

            // BGMの再生
            var bgmPlayer = BGMManager.Play(_bgm1);
            
            // 5秒待機
            await UniTask.Delay(TimeSpan.FromSeconds(5));

            // BGMの停止方法①
            BGMManager.Stop(_bgm1.Resource); // Resource名で停止

            // BGMの停止方法②
            bgmPlayer.Stop();　// 再生中のプレイヤーから停止

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            // -------------------フェードアウト再生------------------------

            // BGMのフェードイン再生
            await BGMManager.FadeInAsync(_bgm1);

            // 5秒待機
            await UniTask.Delay(TimeSpan.FromSeconds(5));

            // BGMのフェードアウト停止
            await BGMManager.FadeOutAsync(_bgm1);

            // ---------------------その他-----------------------

            // BGMが再生中か確認
            if (!BGMManager.HasPlay(_bgm1))
            {
                BGMManager.Play(_bgm1); // 再生中でなければ再生
            }

            // BGMのクロスフェード再生
            await BGMManager.CrossFadeAsync(bgmPlayer, _bgm2);
        }
    }
}