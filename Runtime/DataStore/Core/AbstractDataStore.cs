using MyUtils.JsonUtils;
using R3;
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
        [field: SerializeField] private SerializableReactiveProperty<TType> _current = new(new TType());
        public TType Current => _current.Value;

        [Header("Settings")]
        public bool LoadToOnAwake = true;
        [SerializeField] private bool _isEncrypt = true;
        [SerializeField] private string _aesKey = "e5Cp29Pda8n5Qv13";

        protected virtual void Awake()
        {
            _current.AddTo(this);
            if (LoadToOnAwake) LoadData(out _, 0);
        }

        public bool LoadData() => LoadData(out _);

        public bool LoadData(out TType current, int slotNumber = 0) =>
            LoadData($"{slotNumber}_{FileName}", out current);

        private bool LoadData(string fileName, out TType current)
        {
            bool isLoaded = false;
            if (OverrideData != null)
            {
                current = _current.CurrentValue;
                Debug.Log($"{OverrideData.name} からデータを読み込みました。");
            }
            else
            {
                isLoaded = EncryptedJsonFileHandler<TType>.LoadData(out current, DefaultData.Data, fileName, _isEncrypt,
                    _aesKey);
            }

            _current.Value = current;
            return isLoaded;
        }

        public void SaveData(int slotNumber = 0) => SaveData($"{slotNumber}_{FileName}");

        private void SaveData(string fileName)
        {
            if (OverrideData != null)
            {
                Debug.LogWarning($"{OverrideData.name} が設定されているため、データの保存をスキップしました。");
                return;
            }

            EncryptedJsonFileHandler<TType>.SaveData(_current.Value, fileName, _isEncrypt, _aesKey);
        }
    }
}