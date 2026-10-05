using System;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    /// 1セリフ分のデータ
    /// </summary>
    [Serializable]
    public class TalkLine
    {
        /// <summary>話者の名前</summary>
        [field: SerializeField] public string Speaker { get; private set; }

        /// <summary>セリフ本文(\n は改行として扱う)</summary>
        [field: SerializeField] public string Text { get; private set; }

        [field: SerializeField] public ETalkEmotion TalkEmotion { get; private set; }

        /// <summary>ボイスのアドレス(Addressablesのアドレスなど。なければ空)</summary>
        [field: SerializeField] public string VoiceAddress { get; private set; }

        /// <summary>ボイス(未読み込み・なければnull)。アドレスからの読み込みは <see cref="TalkManager"/> が行う</summary>
        [field: SerializeField] public AudioClip Voice { get; internal set; }

        public TalkLine(string speaker, string text, ETalkEmotion talkEmotion, AudioClip voice = null,
            string voiceAddress = null)
        {
            Speaker = speaker;
            Text = text;
            TalkEmotion = talkEmotion;
            Voice = voice;
            VoiceAddress = voiceAddress;
        }
    }
}
