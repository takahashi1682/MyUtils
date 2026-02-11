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
        [field: SerializeField] public TAsset OverrideData { get; private set; }
        [field: SerializeField] public TAsset DefaultData { get; private set; }
        [SerializeField] private TType _current;
        public TType Current => _current;

        [Header("Settings")]
        public bool LoadToOnAwake = true;
        public bool SaveToOnDestroy = true;
        [field: SerializeField] public EncryptSetting EncryptSetting { get; private set; } = new();

        protected virtual void Awake()
        {
            if (LoadToOnAwake) LoadData();
        }

        protected virtual void OnDestroy()
        {
            if (SaveToOnDestroy)
            {
                try
                {
                    SaveData();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"データ保存中にエラーが発生しました: {ex.Message}");
                }
            }
        }

        public bool LoadData()
        {
            if (OverrideData != null)
            {
                _current = OverrideData.Data;
                Debug.Log($"{OverrideData.name} からデータを読み込みました。");
                return true;
            }

            EncryptedJsonFileHandler<TType>.LoadData(out _current, DefaultData.Data, EncryptSetting);
            return true;
        }

        public void SaveData()
        {
            if (OverrideData != null)
            {
                Debug.LogWarning($"{OverrideData.name} が設定されているため、データの保存をスキップしました。");
                return;
            }

            EncryptedJsonFileHandler<TType>.SaveData(_current, EncryptSetting);
        }
    }
}