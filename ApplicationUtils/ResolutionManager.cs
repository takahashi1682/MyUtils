using MyUtils.DataStore;
using R3;
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

    public class ResolutionManager : AbstractSingletonBehaviour<ResolutionManager>
    {
        public static readonly (int width, int height)[] ResolutionList =
        {
            (1920, 1080),
            (1600, 900),
            (1280, 720),
            (960, 540),
            (540, 360),
        };

        public SerializableReactiveProperty<EResolution> CurrentResolution;

        protected override void Awake()
        {
            base.Awake();

            var resolution = PlayerSettingsStore.Singleton.Current.Resolution;
            CurrentResolution = new SerializableReactiveProperty<EResolution>(resolution);
            CurrentResolution.Subscribe(x =>
            {
                ApplyResolution(x);
                resolution = x;
            }).AddTo(this);
        }

        private static void ApplyResolution(EResolution resolution)
        {
            (int width, int height) = ResolutionList[(int)resolution];
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
    }
}