using UnityEngine;
using UnityEngine.UI;

namespace TUtils.SpriteAnimation
{
    public class ImageAnimation : AbstractAnimation
    {
        [SerializeField] private Image _image;

        protected override void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}