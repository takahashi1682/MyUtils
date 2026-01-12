using System;
using MyUtils.ApplicationUtils;

namespace MyUtils.DataStore
{
    [Serializable]
    public class PlayerSetting
    {
        public float[] Volumes = { 1f, 1f, 1f, 1f };
        public EResolution Resolution = EResolution.W1920H1080;
    }
}