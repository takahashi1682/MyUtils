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
        [field: SerializeField] public TAsset OverrideData { get; protected set; }
        [field: SerializeField] public TAsset DefaultData { get; protected set; }
        [field: SerializeField] public SerializableReactiveProperty<TType> CurrentData { get; private set; } = new();

        [Header("Settings")]
        public bool LoadToOnAwake = true;
        [SerializeField] protected bool _isEncrypt = true;
        [SerializeField] protected string _aesKey = "e5Cp29Pda8n5Qv13";
        [SerializeField] protected bool _showDebugInfo = true;

        protected virtual void Awake()
        {
            CurrentData.AddTo(this);
            if (LoadToOnAwake) LoadData();
        }

        public bool LoadData(int slotNumber = 0) => LoadData(out _, slotNumber);

        public bool LoadData(out TType current, int slotNumber = 0) =>
            LoadData($"{slotNumber}_{FileName}", out current);

        protected bool LoadData(string fileName, out TType current)
        {
            bool isLoaded = false;
            if (OverrideData != null)
            {
                current = OverrideData.Data;
                if (_showDebugInfo) Debug.Log($"{OverrideData.name} が設定されているため、データの読み込みをスキップしました。");
            }
            else
            {
                isLoaded = EncryptedJsonFileHandler<TType>.LoadData(out current, DefaultData.Data, fileName,
                    _isEncrypt,
                    _aesKey);

                if (_showDebugInfo)
                {
                    if (isLoaded)
                    {
                        Debug.Log($"データの読み込みに成功しました: {fileName}");
                    }
                    else
                    {
                        Debug.LogWarning($"データの読み込みに失敗しました。デフォルトデータを使用します: {fileName}");
                    }
                }
            }

            CurrentData.OnNext(current);
            return isLoaded;
        }

        public void SaveData(int slotNumber = 0) => SaveData($"{slotNumber}_{FileName}");

        protected void SaveData(string fileName)
        {
            if (OverrideData != null)
            {
                if (_showDebugInfo) Debug.Log($"{OverrideData.name} が設定されているため、データの保存をスキップしました。");
                return;
            }

            EncryptedJsonFileHandler<TType>.SaveData(CurrentData.Value, fileName, _isEncrypt, _aesKey);
            if (_showDebugInfo) Debug.Log($"データの保存に成功しました: {fileName}");
        }
    }
}