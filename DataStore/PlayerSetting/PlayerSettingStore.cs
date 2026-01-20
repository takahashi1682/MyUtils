using MyUtils.DataStore.Core;

namespace MyUtils.DataStore
{
    /// <summary>
    /// 他クラスから参照するためのシングルトンクラス
    /// </summary>
    public class PlayerSettingStore : AbstractDataStore<PlayerSettingData, PlayerSettingAsset>
    {
    }
}