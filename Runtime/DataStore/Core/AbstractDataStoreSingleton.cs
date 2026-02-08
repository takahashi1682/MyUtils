using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.DataStore.Core
{
    /// <summary>
    ///  JSONファイルにデータを保存・読み込みするための抽象クラス
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TAsset"></typeparam>
    public class AbstractDataStoreSingleton<TType, TAsset> : AbstractDataStore<TType, TAsset>
        where TType : new()
        where TAsset : AbstractDataAsset<TType>
    {
        public static AbstractDataStore<TType, TAsset> Instance { get; private set; }
        private static UniTaskCompletionSource<TType> _source = new();
        public static UniTask<TType> WaitInstanceAsync => _source.Task;

        [Header("Auto Load/Save Settings")]
        public bool LoadToOnAwake = true;
        public bool SaveToOnDestroy = true;

        public TType Current { get; private set; } = new();

        protected void Awake()
        {
            try
            {
                if (LoadToOnAwake)
                    Current = LoadData();

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
                    SaveData(Current);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"データ保存中にエラーが発生しました: {ex.Message}");
                }
            }
        }
    }
}