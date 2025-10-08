using R3;
using TMPro;
using TNRD;
using UnityEngine;

namespace MyUtils.UIBinder
{
    // 共通インターフェース
    public interface IValueBinder<T>
    {
        ReadOnlyReactiveProperty<T> CurrentValue { get; }
    }

    /// <summary>
    /// 任意の型をTextMeshProにバインドする
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class AbstractValueBinder<T> : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IValueBinder<T>> _target;
        [SerializeField] private string _textFormat = "{0}"; // デフォルト書式

        private void Start()
        {
            if (TryGetComponent<TextMeshProUGUI>(out var text))
            {
                _target.Value.CurrentValue
                    .Subscribe(x => text.text = string.Format(_textFormat, x))
                    .AddTo(this);
            }
        }
    }
}