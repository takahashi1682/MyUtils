using Cysharp.Threading.Tasks;
using MyUtils.Csv;
using MyUtils.TalkUtils;
using R3;
using UnityEngine;

namespace MyUtils.Grid
{
    public class UnitTalk : MonoBehaviour, IUnitEvent
    {
        private readonly Subject<Unit> _onTalkBegin = new();
        private readonly Subject<Unit> _onTalkEnd = new();
        public Observable<Unit> OnTalkBegin => _onTalkBegin;
        public Observable<Unit> OnTalkEnd => _onTalkEnd;

        [SerializeField] private TextAsset _talkData;
        public int GroupCount = 1;
        private readonly TalkData _loadData = new();

        private void Start()
        {
            _onTalkBegin.AddTo(this);
            _onTalkEnd.AddTo(this);
            _loadData.AddRange(CsvUtils<LineData>.Parse(_talkData));
        }

        public UniTask OnTalk(string key)
        {
            var lines = _loadData.GetLines(key);
            return TalkManager.Singleton.TalkAsync(lines);
        }

        public UniTask OnRandomTalk()
        {
            int groupId = Random.Range(1, GroupCount + 1);
            return OnTalk("talk_" + groupId);
        }

        public async UniTask OnInteract()
        {
            _onTalkBegin.OnNext(Unit.Default);
            await OnRandomTalk();
            _onTalkEnd.OnNext(Unit.Default);
        }
    }
}