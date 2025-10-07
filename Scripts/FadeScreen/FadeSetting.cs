using System;

namespace MyUtils.FadeScreen
{
    [Serializable]
    public class FadeSetting
    {
        public static readonly FadeSetting Default = new();

        public float Duration = 0.5f;
        public UnityEngine.Color Color = UnityEngine.Color.black;
    }
}