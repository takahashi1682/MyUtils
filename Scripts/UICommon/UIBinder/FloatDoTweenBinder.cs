using DG.Tweening;
using R3;
using TMPro;
using TNRD;
using UnityEngine;

namespace TUtils.UICommon.UIBinder
{
    /// <summary>
    ///  数値をテキストにバインドしアニメーションする機能
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FloatDoTweenBinder : MonoBehaviour
    {
        [SerializeField] private SerializableInterface<IValueBinder<float>> _target;
        [SerializeField] private string _textFormat = "{0:00000000}";
        [SerializeField] private float _duration = 1;
        private Tween _tween;

        private void Start()
        {
            if (TryGetComponent<TextMeshProUGUI>(out var text))
            {
                float lastValue = 0;

                _target.Value.CurrentValue
                    .Subscribe(afterValue =>
                    {
                        float currentValue = lastValue;

                        _tween?.Kill();
                        _tween = DOTween.To(
                                () => currentValue, // 開始値
                                v => currentValue = v, // 更新時の処理
                                afterValue,
                                _duration)
                            .OnKill(() => currentValue = afterValue)
                            .OnUpdate(() => text.text = string.Format(_textFormat, currentValue));
                        _tween.Play();

                        lastValue = afterValue;
                    })
                    .AddTo(this);
            }
        }
    }
}