using UnityEngine;

namespace MyUtils.SpriteAnimation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : AbstractAnimation<SpriteRenderer>
    {
        protected override void SetSprite(int index)
        {
            Target.sprite = Sprites[index];
        }
    }
}