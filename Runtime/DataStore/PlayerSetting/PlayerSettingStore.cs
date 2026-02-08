using MyUtils.DataStore.Core;

namespace MyUtils.DataStore.PlayerSetting
{
    /// <summary>
    /// 他クラスから参照するためのシングルトンクラス
    /// </summary>
    public class PlayerSettingStore : AbstractDataStoreSingleton<PlayerSetting, PlayerSettingAsset>
    {
    }
}

