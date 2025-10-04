using R3;
using TNRD;
using UnityEngine;

namespace TUtils.UICommon.UIBinder
{
    public interface IViewSwitchBinder
    {
        ReadOnlyReactiveProperty<bool> IsFull { get; }
        ReadOnlyReactiveProperty<bool> IsEmpty { get; }
    }

    /// <summary>
    ///  ビューの切り替えを制御するクラス
    /// </summary>
    public class ViewSwitchBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IViewSwitchBinder> _viewSwitcher;
        public bool IsActiveWhenFull = true;
        public bool IsActiveWhenEmpty;

        private void Start()
        {
            _viewSwitcher.Value.IsFull
                .Where(_ => IsActiveWhenFull)
                .Subscribe(x => gameObject.SetActive(x)
                ).AddTo(this);

            _viewSwitcher.Value.IsEmpty
                .Where(_ => IsActiveWhenEmpty)
                .Subscribe(x => gameObject.SetActive(x)
                ).AddTo(this);
        }
    }
}