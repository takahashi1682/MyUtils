using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    /// <summary>
    /// Tween.SetLink で対象GameObjectに自動アタッチされ、GameObjectのライフサイクルをTweenへ転送する。
    /// </summary>
    internal sealed class TweenGameObjectLink : MonoBehaviour
    {
        private Tween _tween;
        private LinkBehaviour _behaviour;

        public static void Attach(GameObject target, Tween tween, LinkBehaviour behaviour)
        {
            var link = target.AddComponent<TweenGameObjectLink>();
            link.hideFlags = HideFlags.HideInInspector;
            link._tween = tween;
            link._behaviour = behaviour;
        }

        private void OnEnable()
        {
            switch (_behaviour)
            {
                case LinkBehaviour.PauseOnDisablePlayOnEnable:
                    _tween.Play();
                    break;
                case LinkBehaviour.PauseOnDisableRestartOnEnable:
                case LinkBehaviour.RestartOnDisableRestartOnEnable:
                    _tween.Restart();
                    break;
            }
        }

        private void OnDisable()
        {
            switch (_behaviour)
            {
                case LinkBehaviour.PauseOnDisable:
                case LinkBehaviour.PauseOnDisablePlayOnEnable:
                case LinkBehaviour.PauseOnDisableRestartOnEnable:
                    _tween.Pause();
                    break;
                case LinkBehaviour.KillOnDisable:
                    _tween.Kill();
                    break;
                case LinkBehaviour.RestartOnDisableRestartOnEnable:
                    _tween.Restart();
                    break;
            }
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}
