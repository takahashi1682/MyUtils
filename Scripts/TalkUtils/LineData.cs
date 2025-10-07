using System;
using MyUtils.Csv;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    ///   セリフデータ
    /// </summary>
    [Serializable]
    public class LineData : AbstractCsvData
    {
        // [SerializeField] private int _id;
        [SerializeField] private string _key;
        [SerializeField] private string _name;
        [SerializeField] private int _group;
        [SerializeField] private string _lines;
        [SerializeField] private EEmotions _emotion;
        [SerializeField] private AudioClip _voice;

        // public int Id => _id;
        public string Key => _key;
        public string Name => _name;
        public int Group => _group;
        public string Lines => _lines;
        public EEmotions Emotion => _emotion;
        public AudioClip Voice => _voice;

        public override void SetParameter(string[] parameter)
        {
            //_id = int.Parse(parameter[0]);
            _key = parameter[0];
            _name = parameter[1];
            _lines = parameter[2];
            _group = int.Parse(parameter[3]);

            // 感情の設定
            // _emotion = (EEmotions)Enum.Parse(typeof(EEmotions), parameter[4]);

            // ボイスデータの読み込み
            // try
            // {
            //     if (await AddressablesUtils.Exists(_key))
            //     {
            //         var voiceHandle = Addressables.LoadAssetAsync<AudioClip>(_key);
            //         await voiceHandle.Task;
            //         _voice = voiceHandle.Result;
            //     }
            // }
            // catch (InvalidKeyException)
            // {
            //     Debug.Log($"{_key} の読み込みに失敗しました");
            // }
        }
    }
}