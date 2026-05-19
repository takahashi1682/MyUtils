using MyUtils.JsonUtils;
using UnityEngine;

namespace MyUtils.DataStore.Core
{
    /// <summary>
    ///  JSONファイルにデータを保存・読み込みするための抽象クラス
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TAsset"></typeparam>
    public abstract class AbstractDataStore<TType, TAsset> : MonoBehaviour
        where TType : new()
        where TAsset : AbstractDataAsset<TType>
    {
        protected abstract string FileName { get; }

        [Header("Reference")]
        [field: SerializeField] public TAsset OverrideData { get; private set; }
        [field: SerializeField] public TAsset DefaultData { get; private set; }
        [SerializeField] private TType _current;
        public TType Current => _current;

        [Header("Settings")]
        public bool LoadToOnAwake = true;
        [SerializeField] private bool _isEncrypt = true;
        [SerializeField] private string _aesKey = "e5Cp29Pda8n5Qv13";

        protected virtual void Awake()
        {
            if (LoadToOnAwake) LoadData();
        }

        public void LoadData() => LoadData(FileName);

        public void LoadData(int slotNumber) => LoadData($"{slotNumber}_{FileName}");

        public void LoadData(string fileName)
        {
            if (OverrideData != null)
            {
                _current = OverrideData.Data;
                Debug.Log($"{OverrideData.name} からデータを読み込みました。");
            }

            EncryptedJsonFileHandler<TType>.LoadData(out _current, DefaultData.Data, fileName, _isEncrypt, _aesKey);
        }

        public void SaveData() => SaveData(FileName);

        public void SaveData(int slotNumber) => SaveData($"{slotNumber}_{FileName}");

        public void SaveData(string fileName)
        {
            if (OverrideData != null)
            {
                Debug.LogWarning($"{OverrideData.name} が設定されているため、データの保存をスキップしました。");
                return;
            }

            EncryptedJsonFileHandler<TType>.SaveData(_current, fileName, _isEncrypt, _aesKey);
        }
    }
}