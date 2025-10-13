using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.SpriteAnimation
{
    public class ImageAnimation : AbstractAnimation<Image>
    {
        protected override void SetSprite(Sprite sprite)
        {
            Target.sprite = sprite;
        }
    }
}