using MyUtils.Csv;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    /// <summary>
    ///  会話機能
    /// </summary>
    public sealed class TestQuestionListController : MonoBehaviour
    {
        [SerializeField] private TalkData _loadData = new();
        [SerializeField] private TextAsset _talkData;

        private async void Start()
        {
            // ①csvから会話データを読み込む
            _loadData.AddRange(CsvUtils<LineData>.Parse(_talkData));

            // ②読み込んだ会話データから特定のデータを取得する
            //var attackLine = _loadData.GetLine("player_attack");
            //await TalkManager.Instance.LineAsync(attackLine);

            // // ①csvから会話データを読み込む
            _loadData.AddRange(CsvUtils<LineData>.Parse(_talkData));
            // ②読み込んだ会話データを表示する
            await TalkManager.Instance.TalkAsync(_loadData);
        }
    }
}