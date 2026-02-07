using R3;
using R3.Triggers;
using UnityEngine;

namespace MyUtils.AnimatorUtils
{
    public class AnimatorStateObserver : AbstractTargetBehaviour<Animator>
    {
        private readonly ReactiveProperty<ObservableStateMachineTrigger.OnStateInfo> _currentStateHash = new(null);
        public ReadOnlyReactiveProperty<ObservableStateMachineTrigger.OnStateInfo> CurrentStateHash =>
            _currentStateHash;

        private readonly Subject<ObservableStateMachineTrigger.OnStateInfo> _onEntry = new();
        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnEntry => _onEntry;

        private readonly Subject<ObservableStateMachineTrigger.OnStateInfo> _onExit = new();
        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnExit => _onExit;

        private void Awake()
        {
            _currentStateHash.AddTo(this);
            _onEntry.AddTo(this);
            _onExit.AddTo(this);

            var trigger = Target.GetBehaviour<ObservableStateMachineTrigger>();
            if (trigger == null)
            {
                Debug.LogError(
                    $"[AnimatorStateProvider] {Target.name} の Animator 内に ObservableStateMachineTrigger が見つかりません。");
                return;
            }

            trigger.OnStateEnterAsObservable().Subscribe(info => _currentStateHash.Value = info).AddTo(this);
            trigger.OnStateEnterAsObservable().Subscribe(_onEntry.OnNext).AddTo(this);
            trigger.OnStateExitAsObservable().Subscribe(_onExit.OnNext).AddTo(this);
        }

        /// <summary>
        /// 現在のステートが指定したものかどうかを監視する
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public Observable<bool> IsState(string stateName, int layerIndex = -1)
        {
            int targetHash = Animator.StringToHash(stateName);
            return _currentStateHash
                .Where(info => info != null)
                .Where(info => layerIndex == -1 || info.LayerIndex == layerIndex)
                .Select(hash => hash.StateInfo.shortNameHash == targetHash);
        }

        /// <summary>
        /// 指定したステート名に入った瞬間に通知を送る
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnEnterState(string stateName, int layerIndex = -1)
        {
            int targetHash = Animator.StringToHash(stateName);
            return _onEntry
                .Where(info => info != null)
                .Where(info => layerIndex == -1 || info.LayerIndex == layerIndex)
                .Where(info => info.StateInfo.shortNameHash == targetHash);
        }

        /// <summary>
        /// 指定したステート名から出た瞬間に通知を送る
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public Observable<ObservableStateMachineTrigger.OnStateInfo> OnExitState(string stateName, int layerIndex = -1)
        {
            int targetHash = Animator.StringToHash(stateName);
            return _onExit
                .Where(info => info != null)
                .Where(info => layerIndex == -1 || info.LayerIndex == layerIndex)
                .Where(info => info.StateInfo.shortNameHash == targetHash);
        }
    }
}