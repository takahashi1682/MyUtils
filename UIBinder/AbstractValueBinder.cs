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
    public abstract class AbstractValueBinder<T> : MonoBehaviour
    {
        [SerializeField] protected SerializableInterface<IValueBinder<T>> _inValue;
        [SerializeField] protected TextMeshProUGUI _outText;
        [SerializeField] protected string _textFormat = "{0}"; // デフォルト書式

        protected virtual void Start()
        {
            _inValue.Value.CurrentValue
                .Subscribe(x => _outText.text = string.Format(_textFormat, x))
                .AddTo(this);
        }
    }
}