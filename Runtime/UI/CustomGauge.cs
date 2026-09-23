using MyUtils.Abstract;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UI
{
    /// <summary>
    /// 割合(0〜1)に応じてTargetの位置を動かすゲージ。UnityのSliderと同様の使い方ができる。
    /// サブゲージ(遅れて追従する表現)が必要な場合は、別途 <see cref="TrailingSubGauge"/> を組み合わせる。
    /// </summary>
    public class CustomGauge : AbstractTargetBehaviour<RectTransform>
    {
        [SerializeField] protected Slider.Direction _direction = Slider.Direction.LeftToRight;
        [SerializeField] protected int _offset;
        [SerializeField, Range(0f, 1f)] protected float _value = 1f;

        protected Vector3 _startPosition;
        protected bool _isStarted;

        /// <summary>
        /// ゲージの割合(0〜1)。UnityのSlider.valueと同様、外部から直接設定する。
        /// </summary>
        public float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp01(value);
                if (_isStarted)
                {
                    ApplyValue(_value);
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            _startPosition = Target.localPosition;
            _isStarted = true;
            ApplyValue(_value);
        }

        /// <summary>
        /// Inspectorから_valueを編集した際に反映させる(Unity標準のSlider.OnValidateと同様の挙動)。
        /// Start前はTarget等が未初期化のため何もしない。
        /// </summary>
        protected virtual void OnValidate()
        {
            if (_isStarted)
            {
                ApplyValue(_value);
            }
        }

        protected virtual void ApplyValue(float rate)
        {
            Target.localPosition = CalculatePosition(rate);
        }

        /// <summary>
        /// 割合(0〜1)から、方向とオフセットを反映したTargetのローカル座標を求める。
        /// </summary>
        protected virtual Vector3 CalculatePosition(float rate)
        {
            var position = _startPosition;
            switch (_direction)
            {
                case Slider.Direction.LeftToRight:
                    position.x = (Target.sizeDelta.x - _offset) * (rate - 1f);
                    break;
                case Slider.Direction.RightToLeft:
                    position.x = (Target.sizeDelta.x - _offset) * (1f - rate);
                    break;
                case Slider.Direction.BottomToTop:
                    position.y = (Target.sizeDelta.y - _offset) * (rate - 1f);
                    break;
                case Slider.Direction.TopToBottom:
                    position.y = (Target.sizeDelta.y - _offset) * (1f - rate);
                    break;
            }
            return position;
        }
    }
}
