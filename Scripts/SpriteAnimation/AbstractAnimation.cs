using System.Collections;
using UnityEngine;

namespace TUtils.SpriteAnimation
{
    public abstract class AbstractAnimation : MonoBehaviour
    {
        public enum EMode
        {
            One,
            Repeat,
            PingPong
        }

        [SerializeField] private EMode _mode = EMode.One;
        [SerializeField] private float _fps = 16.0f;
        [SerializeField] private bool _isAutoDestroy = true;
        [SerializeField] private Sprite[] _sprites;

        protected virtual void OnEnable()
        {
            StartCoroutine(MainProcess(1f / _fps));
        }

        protected virtual IEnumerator MainProcess(float interval)
        {
            var isLoop = _mode != EMode.One;
            var spriteLength = _sprites.Length;
            var secondWait = new WaitForSeconds(interval);
            for (var count = 0; isLoop || count < spriteLength; count++)
            {
                var index = _mode switch
                {
                    EMode.Repeat => (int)Mathf.Repeat(count, spriteLength),
                    EMode.PingPong => (int)Mathf.PingPong(count, spriteLength - 1),
                    _ => count
                };

                SetSprite(_sprites[index]);

                yield return secondWait;
            }

            if (_isAutoDestroy)
            {
                Destroy(gameObject);
            }
        }

        protected abstract void SetSprite(Sprite sprite);
    }
}