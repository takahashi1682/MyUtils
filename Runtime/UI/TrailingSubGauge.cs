using MyUtils.Abstract;
using UnityEngine;

namespace MyUtils.UI
{
    /// <summary>
    /// 指定した<see cref="CustomGauge"/>のTarget位置に遅れて追従するサブゲージ。
    /// 値が増えた(回復)場合は瞬時に追従し、減った(ダメージ)場合はゆっくり追いつく。
    /// HPバーで「減った分が少し遅れて追いつく」演出に使う。
    /// </summary>
    public class TrailingSubGauge : AbstractTargetBehaviour<RectTransform>
    {
        [Header("追従する対象のゲージ")]
        [SerializeField] private CustomGauge _source;

        [Tooltip("ダメージ時に追いつく速さ(1秒あたりの移動量)")]
        [SerializeField] private float _speed = 100f;

        private float? _previousValue;

        protected override void Start()
        {
            base.Start();
            if (_source == null)
            {
                Debug.LogError($"{gameObject.name} に追従先の CustomGauge が設定されていません。");
            }
        }

        protected virtual void Update()
        {
            if (_source == null) return;

            var sourcePosition = _source.Target.localPosition;
            var rate = _source.Value;

            // 初回、または回復(値が増加)した場合は瞬時に追従させる。
            // 減少(ダメージ)した場合は、Vector3.MoveTowardsでゆっくり追いつかせる。
            if (_previousValue == null || rate > _previousValue)
            {
                Target.localPosition = sourcePosition;
            }
            else if (Target.localPosition != sourcePosition)
            {
                Target.localPosition = Vector3.MoveTowards(
                    Target.localPosition, sourcePosition, Time.deltaTime * _speed);
            }

            _previousValue = rate;
        }
    }
}
