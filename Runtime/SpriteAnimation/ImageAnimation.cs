using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.SpriteAnimation
{
    [RequireComponent(typeof(Image))]
    public class ImageAnimation : AbstractAnimation<Image>
    {
        protected override void SetSprite(int index)
        {
            Target.sprite = Sprites[index];
        }
    }
}