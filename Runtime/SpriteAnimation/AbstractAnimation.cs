using System.Collections;
using UnityEngine;

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

        public Sprite[] Sprites;
        private Coroutine _animationCoroutine;

        protected override void Start()
        {
            base.Start();
            if (IsPlayOnAwake) Play();
        }

        protected virtual void OnEnable()
        {
            if (IsPlayOnEnable) Play();
        }

        public void Play() => _animationCoroutine ??= StartCoroutine(MainProcess(1f / FPS));

        protected virtual void OnDisable()
        {
            Stop();
        }

        public void Stop()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
        }

        protected virtual IEnumerator MainProcess(float interval)
        {
            bool isLoop = Mode != EMode.One;
            int spriteLength = Sprites.Length;
            var secondWait = new WaitForSeconds(interval);
            for (int count = 0; isLoop || count < spriteLength; count++)
            {
                int index = Mode switch
                {
                    EMode.Repeat => (int)Mathf.Repeat(count, spriteLength),
                    EMode.PingPong => (int)Mathf.PingPong(count, spriteLength - 1),
                    _ => count
                };

                SetSprite(index);

                yield return secondWait;
            }

            if (IsAutoDestroy)
            {
                Destroy(gameObject);
            }
        }

        protected abstract void SetSprite(int index);
    }
}