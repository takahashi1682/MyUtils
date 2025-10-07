using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UICommon
{
    public interface IButtonObservable
    {
        Observable<Unit> OnClickObservable { get; }
    }

    [RequireComponent(typeof(Button))]
    public abstract class AbstractButton : MonoBehaviour, IButtonObservable
    {
        [Header("Button Settings")]
        public AwaitOperation AwaitOperation = AwaitOperation.Drop;

        private readonly Subject<Unit> _onClickSubject = new();
        public Observable<Unit> OnClickObservable => _onClickSubject;

        private Button _target;
        private bool _isClicked;

        protected virtual void Start()
        {
            _onClickSubject.AddTo(this);

            if (TryGetComponent(out _target))
            {
                _target.OnClickAsObservable()
                    .SubscribeAwait(async (_, cts) =>
                    {
                        _onClickSubject.OnNext(Unit.Default);
                        await OnClick(cts);
                    }, AwaitOperation)
                    .AddTo(this);
            }
        }

        protected abstract UniTask OnClick(CancellationToken ct);
    }
}