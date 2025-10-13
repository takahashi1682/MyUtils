using MyUtils.JsonUtils;
using UnityEngine;

namespace MyUtils.DataStore.Core
{
    /// <summary>
    ///  JSONファイルにデータを保存・読み込みするための抽象クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbstractDataStore<T> : AbstractSingletonBehaviour<AbstractDataStore<T>>
        where T : class, new()
    {
        public bool SaveToOnDestroy = true;
        [field: SerializeField] public T Current;
        [field: SerializeField] public DataStoreSetting Setting { get; private set; } = new();

        protected override void Awake()
        {
            base.Awake();

            // JSONファイルのフルパスを取得し、設定をロード
            EncryptedJsonFileHandler<T>.LoadData(out Current, new T(), Setting);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // 設定をJSONファイルに保存
            if (SaveToOnDestroy) SaveSettings();
        }

        public void SaveSettings()
            => EncryptedJsonFileHandler<T>.SaveData(Current, Setting);
    }
}