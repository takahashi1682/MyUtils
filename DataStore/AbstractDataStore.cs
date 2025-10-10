using MyUtils.JsonUtils;
using UnityEngine;

namespace MyUtils.DataStore
{
    /// <summary>
    ///  JSONファイルにデータを保存・読み込みするための抽象クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbstractDataStore<T> : AbstractSingletonBehaviour<AbstractDataStore<T>>
        where T : class, new()
    {
        [field: SerializeField] public T Current { get; private set; }

        [Header("保存設定")]
        public bool SaveToOnDestroy = true;
        public string FileName = "store.json";

        [Header("暗号化設定")]
        [Tooltip("暗号鍵（16文字）")]
        public bool IsEncrypt = true;
        public string IvFileName = "store.iv";
        public string AesKey = "e5Cp29Pda8n5Qv13";

        protected override void Awake()
        {
            base.Awake();

            // JSONファイルのフルパスを取得し、設定をロード
            Current = EncryptedJsonFileHandler<T>.LoadData(FileName, IsEncrypt, IvFileName, AesKey);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // 設定をJSONファイルに保存
            if (SaveToOnDestroy) SaveSettings();
        }

        public void SaveSettings()
            => EncryptedJsonFileHandler<T>.SaveData(FileName, Current, IsEncrypt, IvFileName, AesKey);
    }
}