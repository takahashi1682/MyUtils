using UnityEngine;
using MyUtils.Abstract;

namespace MyUtils.SpriteAnimation
{
    public abstract class AbstractAnimation<T> : AbstractTargetBehaviour<T> where T : Component
    {
        public enum EMode
        {
            One,
            Repeat,
            PingPong
        }

        public EMode Mode = EMode.One;
        public float FPS = 16.0f;

        public bool IsPlayOnAwake = true;
        public bool IsPlayOnEnable;
        public bool IsAutoDestroy = true;

        /// <summary>
        /// Normal: Time.deltaTime（Time.timeScaleの影響を受ける）
        /// UnscaledTime: Time.unscaledDeltaTime（Time.timeScaleの影響を受けない）
        /// ※AnimatePhysicsは非推奨のため未対応（Normalと同じ扱い）
        /// </summary>
        public AnimatorUpdateMode UpdateMode = AnimatorUpdateMode.Normal;

        public Sprite[] Sprites;

        private bool _isPlaying;
        private float _elapsed;
        private int _count;

        protected override void Start()
        {
            base.Start();
            if (IsPlayOnAwake) Play();
        }

        protected virtual void OnEnable()
        {
            if (IsPlayOnEnable) Play();
        }

        public void Play()
        {
            if (_isPlaying) return;
            _isPlaying = true;
            _elapsed = 0f;
            _count = 0;
            SetSprite(GetIndex(0));
        }

        protected virtual void OnDisable()
        {
            Stop();
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        private void Update()
        {
            if (!_isPlaying) return;
            Tick(UpdateMode == AnimatorUpdateMode.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        private void Tick(float deltaTime)
        {
            bool isLoop = Mode != EMode.One;
            int spriteLength = Sprites.Length;
            float interval = 1f / FPS;

            _elapsed += deltaTime;

            while (_elapsed >= interval)
            {
                _elapsed -= interval;
                _count++;

                if (!isLoop && _count >= spriteLength)
                {
                    _isPlaying = false;
                    if (IsAutoDestroy) Destroy(gameObject);
                    return;
                }

                SetSprite(GetIndex(_count));
            }
        }

        private int GetIndex(int count)
        {
            int spriteLength = Sprites.Length;
            return Mode switch
            {
                EMode.Repeat => (int)Mathf.Repeat(count, spriteLength),
                EMode.PingPong => (int)Mathf.PingPong(count, spriteLength - 1),
                _ => count
            };
        }

        protected abstract void SetSprite(int index);
    }
}
