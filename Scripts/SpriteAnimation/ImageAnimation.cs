using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.SpriteAnimation
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