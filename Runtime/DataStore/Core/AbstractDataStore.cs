using MyUtils.JsonUtils;
using UnityEngine;

namespace MyUtils.DataStore.Core
{
    /// <summary>
    ///  JSONファイルにデータを保存・読み込みするための抽象クラス
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TAsset"></typeparam>
    public class AbstractDataStore<TType, TAsset> : MonoBehaviour
        where TType : new()
        where TAsset : AbstractDataAsset<TType>
    {
        [Header("Data")]
        [field: SerializeField] public TAsset DefaultData { get; private set; }

        [Header("Encrypt Settings")]
        [field: SerializeField] public EncryptSetting EncryptSetting { get; private set; } = new();
        
        public TType LoadData()
        {
            EncryptedJsonFileHandler<TType>.LoadData(out var loadData, DefaultData.Data, EncryptSetting);
            return loadData;
        }

        public void SaveData(TType data)
            => EncryptedJsonFileHandler<TType>.SaveData(data, EncryptSetting);
    }
}