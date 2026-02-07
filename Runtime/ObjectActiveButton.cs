using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// ボタン押下時に指定したGameObject群の活性/非活性を切り替えるコンポーネント
    /// </summary>
    public sealed class GameObjectToggleAsyncButton : AbstractAsyncButton
    {
        [SerializeField] private bool _targetState = true;
        [SerializeField] private GameObject[] _targets;

        protected override UniTask OnClick(CancellationToken ct)
        {
            if (_targets == null || _targets.Length == 0)
            {
                return UniTask.CompletedTask;
            }

            // ループ内の処理を最適化
            for (var i = 0; i < _targets.Length; i++)
            {
                var target = _targets[i];

                // UnityのObject比較は重いため、暗黙的なboolチェックを利用
                if (target)
                {
                    // 現在の状態と異なる場合のみ SetActive を呼ぶことで、
                    // 無駄なエンジンの内部処理（OnEnable/OnDisable）を抑制できる
                    if (target.activeSelf != _targetState)
                    {
                        target.SetActive(_targetState);
                    }
                }
            }

            return UniTask.CompletedTask;
        }

        // 必要に応じて外部からターゲットを上書きできるようにプロパティを公開
        public void SetTargetState(bool state) => _targetState = state;
    }
}