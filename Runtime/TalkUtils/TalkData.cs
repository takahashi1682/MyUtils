using System.Collections.Generic;

namespace MyUtils.TalkUtils
{
    /// <summary>
    /// キーごとの会話(セリフの並び)をまとめたデータ
    /// </summary>
    public class TalkData : Dictionary<string, List<TalkLine>>
    {
        /// <summary>
        /// CSVから読み込んだ行をキーごとにまとめて初期化する
        /// </summary>
        public TalkData(IEnumerable<TalkLineCsv> csvLines)
        {
            foreach (var csvLine in csvLines)
            {
                if (!TryGetValue(csvLine.Key, out var lines))
                {
                    lines = new List<TalkLine>();
                    this[csvLine.Key] = lines;
                }

                lines.Add(new TalkLine(csvLine.Speaker, csvLine.Text, csvLine.TalkEmotion,
                    voiceAddress: csvLine.VoiceAddress));
            }
        }
    }
}