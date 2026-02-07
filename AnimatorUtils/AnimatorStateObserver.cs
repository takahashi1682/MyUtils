using System;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MyUtils.AnimatorUtils
{
    public class AnimatorStateObserver : AbstractTargetBehaviour<Animator>
    {
        private readonly ReactiveProperty<int> _currentStateHash = new(0);
        public ReadOnlyReactiveProperty<int> CurrentStateHash => _currentStateHash;

        private readonly Subject<int> _onEntry = new();
        public Observable<int> OnEntry => _onEntry;

        private readonly Subject<int> _onExit = new();
        public Observable<int> OnExit => _onExit;

        private void Awake()
        {
            _currentStateHash.AddTo(this);
            _onEntry.AddTo(this);
            _onExit.AddTo(this);

            var trigger = Target.GetBehaviour<ObservableStateMachineTrigger>();
            if (trigger == null)
            {
                throw new Exception(
                    $"{nameof(ObservableStateMachineTrigger)} が {Target.name} のAnimatorレイヤーにアタッチされていません。");
            }

            // ステート進入の購読
            trigger.OnStateEnterAsObservable()
                .Subscribe(info =>
                {
                    int hash = info.StateInfo.shortNameHash;
                    _currentStateHash.Value = hash;

                    _onEntry.OnNext(hash);
                })
                .AddTo(this);

            // ステート退出の購読
            trigger.OnStateExitAsObservable()
                .Subscribe(info =>
                {
                    _onExit.OnNext(info.StateInfo.shortNameHash);
                })
                .AddTo(this);
        }
        
        /// <summary> 指定したステート名に入った瞬間に通知を送る </summary>
        public Observable<Unit> OnEntryState(string stateName)
        {
            int targetHash = Animator.StringToHash(stateName);
            return _onEntry
                .Where(hash => hash == targetHash)
                .AsUnitObservable();
        }

        /// <summary> 指定したステート名から出た瞬間に通知を送る </summary>
        public Observable<Unit> OnExitState(string stateName)
        {
            int targetHash = Animator.StringToHash(stateName);
            return _onExit
                .Where(hash => hash == targetHash)
                .AsUnitObservable();
        }
    }
}