using MyUtils.DataStore.Core;
using UnityEngine;

namespace MyUtils.DataStore.PlayerSetting
{
    [CreateAssetMenu(menuName = "MyUtils/DataStore/" + nameof(PlayerSettingAsset))]
    public class PlayerSettingAsset : AbstractDataAsset<PlayerSettingData>
    {
    }
}