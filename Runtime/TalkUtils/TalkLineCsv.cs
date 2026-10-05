using System;
using MyUtils.Csv;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    /// CSVの1行(key, name, lines, emotion, voice)を読み込むデータ。
    /// emotion と voice は省略可能(voice はボイスのアドレス)。
    /// <see cref="TalkData"/> が <see cref="TalkLine"/> に変換して使う。
    /// </summary>
    [Serializable]
    public class TalkLineCsv : AbstractCsvData
    {
        private const int KeyColumn = 0;
        private const int SpeakerColumn = 1;
        private const int TextColumn = 2;
        private const int EmotionColumn = 3; // 省略可
        private const int VoiceColumn = 4; // 省略可

        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public string Speaker { get; private set; }
        [field: SerializeField] public string Text { get; private set; }
        [field: SerializeField] public ETalkEmotion TalkEmotion { get; private set; }
        [field: SerializeField] public string VoiceAddress { get; private set; }

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

            // ボイスのアドレス列は省略可能。読み込みは TalkManager が ITalkVoiceLoader で行う
            if (parameter.Length > VoiceColumn)
            {
                VoiceAddress = parameter[VoiceColumn].Trim();
            }
        }
    }
}
