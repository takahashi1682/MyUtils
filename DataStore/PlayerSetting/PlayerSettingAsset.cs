using MyUtils.DataStore.Core;
using UnityEngine;

namespace MyUtils.DataStore
{
    [CreateAssetMenu(menuName = "MyUtils/DataStore/" + nameof(PlayerSettingAsset))]
    public class PlayerSettingAsset : AbstractDataAsset<PlayerSettingData>
    {
    }
}