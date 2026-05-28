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

        [Header("Data")]
        [field: SerializeField] public TAsset Override { get; protected set; }
        [field: SerializeField] public TAsset Default { get; protected set; }
        [field: SerializeField] public SerializableReactiveProperty<TType> Current { get; private set; } = new();

        public TType CurrentValue => Current.CurrentValue;
        public TType Value
        {
            get => Current.Value;
            set => Current.Value = value;
        }

        [Header("Settings")]
        public bool LoadOnAwake = true;
        public bool AutoSaveOnChange;
        public bool ShowDebugInfo = true;

        [Header("Encrypted Json")]
        [SerializeField] protected bool _isEncrypt = true;
        [SerializeField] protected string _aesKey = "e5Cp29Pda8n5Qv13";

        protected virtual void Awake()
        {
            Current.AddTo(this);
            if (LoadOnAwake) LoadData();
            if (AutoSaveOnChange) Current.Skip(1).Subscribe(_ => SaveData()).AddTo(this);
        }

        public bool LoadData(int slotNumber = 0) => LoadData(out _, slotNumber);

        public bool LoadData(out TType current, int slotNumber = 0) =>
            LoadData($"{slotNumber}_{FileName}", out current);

        protected bool LoadData(string fileName, out TType current)
        {
            if (Override != null)
            {
                current = Override.Clone();
                if (ShowDebugInfo) Debug.LogWarning($"{Override.name} (Override) が設定されているため、ファイルからのデータ読み込みをスキップしました。");
            }
            else if (EncryptedJsonFileHandler<TType>.LoadData(out current, fileName, _isEncrypt, _aesKey))
            {
                if (ShowDebugInfo) Debug.Log($"データの読み込みに成功しました: {fileName}");
            }
            else if (Default != null)
            {
                current = Default.Clone();
                if (ShowDebugInfo) Debug.LogWarning($"データの読み込みに失敗したため、Defaultのデータを使用しました: {fileName}");
            }
            else
            {
                current = CurrentValue;
                if (ShowDebugInfo) Debug.LogError($"データの読み込みに失敗し、Defaultも設定されていないため、Currentは初期値のままになります: {fileName}");
                return false;
            }

            Current.Value = current;
            return true;
        }

        public void SaveData(int slotNumber = 0) => SaveData($"{slotNumber}_{FileName}");

        protected void SaveData(string fileName)
        {
            if (Override != null)
            {
                if (ShowDebugInfo) Debug.Log($"{Override.name} が設定されているため、データの保存をスキップしました。");
                return;
            }

            EncryptedJsonFileHandler<TType>.SaveData(Current.Value, fileName, _isEncrypt, _aesKey);
            if (ShowDebugInfo) Debug.Log($"データの保存に成功しました: {fileName}");
        }
    }
}