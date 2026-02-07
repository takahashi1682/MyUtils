using System;
using MyUtils.ApplicationUtils;

namespace MyUtils.DataStore.PlayerSetting
{
    /// <summary>
    /// 実際に使用されるデータ
    /// </summary>
    [Serializable]
    public class PlayerSettingData
    {
        public float[] Volumes = { 1f, 1f, 1f, 1f };
        public EResolution Resolution = EResolution.W1920H1080;
    }
}