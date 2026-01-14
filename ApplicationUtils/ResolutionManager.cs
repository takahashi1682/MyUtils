using MyUtils.DataStore;
using UnityEngine;

namespace MyUtils.ApplicationUtils
{
    public enum EResolution
    {
        W1920H1080,
        W16000H900,
        W1280H720,
        W960H540,
        W540H360,
    }

    public static class ResolutionManager
    {
        public static readonly (int width, int height)[] ResolutionList =
        {
            (1920, 1080),
            (1600, 900),
            (1280, 720),
            (960, 540),
            (540, 360),
        };

        public static void ApplyResolution(int resolutionIndex)
            => ApplyResolution((EResolution)resolutionIndex);

        public static async void ApplyResolution(EResolution resolution)
        {
            // 解像度の保存
            var setting = await PlayerSettingsStore.AsyncInstance;
            setting.Current.Resolution = resolution;

            // 画面解像度の変更
            (int width, int height) = ResolutionList[(int)resolution];
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
    }
}