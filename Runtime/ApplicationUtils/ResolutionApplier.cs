using UnityEngine;

namespace MyUtils.ApplicationUtils
{
    public enum EResolution
    {
        W3840H2160, // 4K
        W2560H1440, // QHD
        W1920H1080, // Full HD
        W16000H900, // HD+
        W1280H720, // HD
        W960H540, // qHD
        W540H360, // nHD
    }

    public static class ResolutionApplier
    {
        public static readonly (int width, int height)[] ResolutionList =
        {
            (3840, 2160), // 4K
            (2560, 1440), // QHD
            (1920, 1080), // Full HD
            (1600, 900), // HD+
            (1280, 720), // HD
            (960, 540), // qHD
            (540, 360), // nHD
        };

        public static void ApplyResolution(EResolution resolution)
            => ApplyResolution((int)resolution);

        /// <summary>
        /// 画面解像度の変更
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public static void ApplyResolution(int resolutionIndex)
        {
            (int width, int height) = ResolutionList[resolutionIndex];
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
    }
}