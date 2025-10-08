using UnityEngine;

namespace MyUtils
{
    public enum EResolution
    {
        W1920H1080,
        W16000H900,
        W1280H720,
        W960H540,
        W540H360,
    }

    public class ResolutionController : MonoBehaviour
    {
        public static readonly (int width, int height)[] ResolutionList =
        {
            (1920, 1080),
            (1600, 900),
            (1280, 720),
            (960, 540),
            (540, 360),
        };

        public static void ApplyResolution(int index)
        {
            var settings = ResolutionList[index];
            Screen.SetResolution(settings.width, settings.height, FullScreenMode.Windowed);
        }

        public void ApplyResolution(EResolution index) => ApplyResolution((int)index);
    }
}