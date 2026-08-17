using MyUtils.Abstract;
using R3;
using TMPro;
using TNRD;
using UnityEngine;

namespace MyUtils.UIBinder
{
    // 共通インターフェース
    public interface IValueProvider<T>
    {
        ReadOnlyReactiveProperty<T> CurrentValue { get; }
    }

    /// <summary>
    /// 任意の型をTextMeshProにバインドする
    /// </summary>
    public abstract class AbstractValueBinder<T> : AbstractTargetBehaviour<TMP_Text>
    {
        [SerializeField] protected SerializableInterface<IValueProvider<T>> _inValue;
        [SerializeField] protected string _textFormat = "{0}"; // デフォルト書式

        protected override void Start()
        {
            base.Start();
            _inValue.Value.CurrentValue
                .Subscribe(OnValueChanged)
                .AddTo(this);
        }

        /// <summary>
        /// 値が変化するたびに呼ばれる。既定では書式に従いそのままテキストへ反映する。
        /// アニメーション等の独自表示を行いたい場合はオーバーライドする。
        /// </summary>
        protected virtual void OnValueChanged(T value)
        {
            Target.text = string.Format(_textFormat, value);
        }
    }
}