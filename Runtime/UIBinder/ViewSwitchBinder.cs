using R3;
using TNRD;
using UnityEngine;

namespace MyUtils.UIBinder
{
    public interface IViewSwitchProvider
    {
        ReadOnlyReactiveProperty<bool> IsFull { get; }
        ReadOnlyReactiveProperty<bool> IsEmpty { get; }
    }

    /// <summary>
    ///  ビューの切り替えを制御するクラス
    /// </summary>
    public class ViewSwitchBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IViewSwitchProvider> _viewSwitcher;
        public bool IsActiveWhenFull = true;
        public bool IsActiveWhenEmpty;

        /// <summary>値の供給元。既定はInspectorで割り当てた参照。DIなどで差し替える場合はオーバーライドする。</summary>
        protected virtual IViewSwitchProvider ResolveProvider() => _viewSwitcher.Value;

        private void Start()
        {
            ResolveProvider().IsFull
                .Where(_ => IsActiveWhenFull)
                .Subscribe(x => gameObject.SetActive(x)
                ).AddTo(this);

            provider.IsEmpty
                .Where(_ => IsActiveWhenEmpty)
                .Subscribe(x => gameObject.SetActive(x)
                ).AddTo(this);
        }
    }
}