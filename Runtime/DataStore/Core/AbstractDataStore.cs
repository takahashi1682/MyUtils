using Cysharp.Threading.Tasks;
using MyUtils.JsonUtils;
using UnityEngine;
using R3;

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
        [field: SerializeField] public TAsset OverrideData { get; private set; }
        [field: SerializeField] public TAsset DefaultData { get; private set; }
        [SerializeField] private TType _current;
        public TType Current => _current;

        [Header("Settings")]
        public bool LoadToOnAwake = true;
        public bool SaveToOnDestroy = true;
        [field: SerializeField] public EncryptSetting EncryptSetting { get; private set; } = new();

        protected void Awake()
        {
            try
            {
                if (LoadToOnAwake) LoadData();

                if (Instance != null && Instance != this)
                {
                    Debug.LogError($"{this} は既に存在しています。重複したインスタンスを破棄します。");
                    Destroy(gameObject);
                    return;
                }

                Instance = this;
                if (!_source.Task.Status.IsCompleted())
                {
                    _source.TrySetResult(Current);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Awake処理中にエラーが発生しました: {ex.Message}");
                _source.TrySetException(ex);
            }
        }

        protected void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                _source = new UniTaskCompletionSource<TType>();
            }

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