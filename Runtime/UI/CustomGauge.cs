using MyUtils.Abstract;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UI
{
    public class Gauge : AbstractTargetBehaviour<RectTransform>
    {
        [SerializeField] protected RectTransform _subTarget;
        [SerializeField] protected float _subTargetSpeed = 100f;
        [SerializeField] protected Slider.Direction _direction = Slider.Direction.LeftToRight;
        [SerializeField] protected int _offset;
        [SerializeField, Range(0f, 1f)] protected float _value = 1f;

        protected Vector3 _startPosition;
        protected float? _previousRate;
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
            _startPosition = _subTarget.localPosition;
            _isStarted = true;
            ApplyValue(_value);
        }

        protected virtual void Update()
        {
            if (Target.localPosition == _subTarget.localPosition) return;

            _subTarget.localPosition = Vector3.MoveTowards(
                _subTarget.localPosition, Target.localPosition, Time.deltaTime * _subTargetSpeed);
        }

        protected virtual void ApplyValue(float rate)
        {
            Target.localPosition = CalculatePosition(rate);

            // 初回、または回復(rateが増加)した場合は、subゲージも瞬時に追従させる。
            // 減少(ダメージ)した場合は、Update側のMoveTowardsでゆっくり追いつかせる。
            if (_previousRate == null || rate > _previousRate)
            {
                _subTarget.localPosition = Target.localPosition;
            }
            _previousRate = rate;
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