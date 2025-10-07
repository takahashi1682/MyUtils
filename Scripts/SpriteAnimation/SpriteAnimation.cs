using UnityEngine;

namespace MyUtils.SpriteAnimation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : AbstractAnimation
    {
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        protected override void SetSprite(Sprite sprite)
        {
            _renderer.sprite = sprite;
        }
    }
}