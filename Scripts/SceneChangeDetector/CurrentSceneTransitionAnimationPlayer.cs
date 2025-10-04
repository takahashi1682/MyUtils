using UnityEngine;
using UnityEngine.SceneManagement;

namespace TUtils.SceneChangeDetector
{
    /// <summary>
    /// シーンの遷移時にアニメーションを再生するクラス
    /// </summary>
    public class CurrentSceneTransitionAnimationPlayer : AbstractSceneChangeDetector
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _animationName;
        [SerializeField] private string _speedParameterName = "speed";

        protected override void OnCurrentSceneEnter(Scene scene, LoadSceneMode mode) => Play(0, 1f); // 再生
        protected override void OnCurrentSceneExit(Scene scene) => Play(1, -1f); // 逆再生

        private void Play(float normalizedTime, float speed)
        {
            _animator.SetFloat(Animator.StringToHash(_speedParameterName), speed);
            _animator.Play(_animationName, 0, normalizedTime);
        }
    }
}