using System;
using System.IO;
using UnityEngine;

namespace MyUtils.JsonUtils
{
    /// <summary>
    ///  JSON 形式でパラメータを保存・復元する抽象クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractSingletonJsonDataService<T> :
        AbstractSingletonBehaviour<AbstractSingletonJsonDataService<T>>
        where T : new()
    {
        [SerializeField] protected string _dataFileName = "data_file.json";
        [field: SerializeField] public T Current { get; private set; } = new();
        protected string _dataPath;

        protected override void Awake()
        {
            base.Awake();
            _dataPath = Path.Combine(Application.persistentDataPath, _dataFileName);
            LoadJson();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SaveJson();
        }

        public void SaveJson()
        {
            string json = JsonUtility.ToJson(Current, false);
            File.WriteAllText(_dataPath, json);
        }

        public void LoadJson()
        {
            if (!File.Exists(_dataPath)) return;

            try
            {
                string json = File.ReadAllText(_dataPath);
                Current = JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}