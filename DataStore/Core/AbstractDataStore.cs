using Cysharp.Threading.Tasks;
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
        public static AbstractDataStore<TType, TAsset> Instance { get; private set; }
        private static UniTaskCompletionSource<TType> _source = new();
        public static UniTask<TType> WaitInstanceAsync => _source.Task;

        [Header("Data")]
        [field: SerializeField] public TAsset Default { get; private set; }
        [SerializeField] private TType _current;
        public TType Current => _current;

        [Header("Settings")]
        public bool LoadToOnAwake = true;
        public bool SaveToOnDestroy = true;
        [field: SerializeField] public EncryptSetting EncryptSetting { get; private set; } = new();

        protected void Awake()
        {
            if (LoadToOnAwake) LoadData();

            if (Instance != null && Instance != this)
            {
                Debug.LogError($"{this} は既に存在しています。重複したインスタンスを破棄します。");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _source.TrySetResult(Current);
        }

        protected void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                _source = new UniTaskCompletionSource<TType>();
            }

            if (SaveToOnDestroy) SaveData();
        }

        public bool LoadData()
            => EncryptedJsonFileHandler<TType>.LoadData(out _current, Default.Data, EncryptSetting);

        public void SaveData()
            => EncryptedJsonFileHandler<TType>.SaveData(_current, EncryptSetting);
    }
}