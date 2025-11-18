using UnityEngine.UI;

namespace MyUtils
{
    public class ImageAnimation : AbstractAnimation<Image>
    {
        protected override void SetSprite(int index)
        {
            Target.sprite = Sprites[index];
        }
    }
}