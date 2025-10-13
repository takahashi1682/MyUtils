using UnityEngine;

namespace MyUtils.SpriteAnimation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : AbstractAnimation<SpriteRenderer>
    {
        protected override void SetSprite(Sprite sprite)
        {
            Target.sprite = sprite;
        }
    }
}