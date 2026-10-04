using System;
using MyUtils.Csv;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    /// CSVの1行(key, name, lines, emotion)を読み込むデータ。
    /// <see cref="TalkData"/> が <see cref="TalkLine"/> に変換して使う。
    /// </summary>
    [Serializable]
    public class TalkLineCsv : AbstractCsvData
    {
        private const int KeyColumn = 0;
        private const int SpeakerColumn = 1;
        private const int TextColumn = 2;
        private const int EmotionColumn = 3; // 省略可

        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public string Speaker { get; private set; }
        [field: SerializeField] public string Text { get; private set; }
        [field: SerializeField] public ETalkEmotion TalkEmotion { get; private set; }
        [field: SerializeField] public AudioClip Voice { get; private set; }

        public override void SetParameter(string[] parameter)
        {
            Key = parameter[KeyColumn];
            Speaker = parameter[SpeakerColumn];
            Text = parameter[TextColumn];

            // 感情列は省略可能。空欄や未知の値はDefaultにする
            if (parameter.Length > EmotionColumn &&
                Enum.TryParse(parameter[EmotionColumn], true, out ETalkEmotion emotion))
            {
                TalkEmotion = emotion;
            }

            // TODO ボイスデータの読み込み(未実装。以下はAddressables利用時の参考コード)
#if Addressables
            try
            {
                if (await AddressablesUtils.Exists(_key))
                {
                    var voiceHandle = Addressables.LoadAssetAsync<AudioClip>(_key);
                    await voiceHandle.Task;
                    _voice = voiceHandle.Result;
                }
            }
            catch (InvalidKeyException)
            {
                Debug.Log($"{_key} の読み込みに失敗しました");
            }
#endif
        }
    }
}
