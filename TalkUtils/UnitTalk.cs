using Cysharp.Threading.Tasks;
using Projects.Level;
using Projects.Level.Player;
using Projects.Level.Unit;
using R3;
using UnityEngine;

namespace MyUtils.TalkUtils
{
    public class UnitTalk : MonoBehaviour, IUnitEvent
    {
        protected readonly Subject<Unit> _onTalkBegin = new();
        protected readonly Subject<Unit> _onTalkEnd = new();
        public Observable<Unit> OnTalkBegin => _onTalkBegin;
        public Observable<Unit> OnTalkEnd => _onTalkEnd;

        public string DefaultTalkKey = "talk_1";
        protected TalkData _talkData;

        public void Initialize(TalkData talkData)
        {
            _onTalkBegin.AddTo(this);
            _onTalkEnd.AddTo(this);
            _talkData = talkData;
        }

        public UniTask OnTalk(string key)
        {
            var lines = _talkData.GetLines(key);
            return TalkManager.Singleton.TalkAsync(lines);
        }

        public bool IsActive() => true;

        public virtual async UniTask OnInteract(PlayerCore player)
        {
            _onTalkBegin.OnNext(Unit.Default);
            await OnTalk(DefaultTalkKey);
            _onTalkEnd.OnNext(Unit.Default);
        }
    }
}