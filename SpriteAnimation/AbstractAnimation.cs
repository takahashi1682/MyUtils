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
        public bool IsAutoDestroy = true;
        public Sprite[] Sprites;

        protected virtual void OnEnable()
        {
            StartCoroutine(MainProcess(1f / FPS));
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