using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// アニメーションを再生するクラス
    /// </summary>
    public class UISwitchAnimationPlayer : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _clipName;
        [SerializeField] private string _speedParameterName = "speed";
        private int _speedHash;
        private bool _isReverse;

        private void Start()
        {
            _speedHash = Animator.StringToHash(_speedParameterName);

            // 最初の再生をスキップ
            PlayAnimation(-1, 0);
        }

        public void OnPlay()
        {
            if (!_isReverse) return;
            _isReverse = false;

            PlayAnimation(-1, 1);
        }

        public void OnReverse()
        {
            if (_isReverse) return;
            _isReverse = true;

            PlayAnimation(1, 0);
        }

        private void PlayAnimation(float speed, float normalizedTime)
        {
            _animator.SetFloat(_speedHash, speed);
            _animator.Play(_clipName.name, 0, normalizedTime);
        }
    }
}