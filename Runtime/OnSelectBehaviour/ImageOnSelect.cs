using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.OnSelectBehaviour
{
    public class ImageOnSelect : AbstractOnSelectBehaviour
    {
        public Image TargetImage;
        public Sprite SelectedSprite;
        public Sprite SubmittedSprite;
        private Sprite _defaultSprite;

        protected override void Start()
        {
            base.Start();
            _defaultSprite = TargetImage.sprite;
        }

        protected override void SelectedAction()
        {
            TargetImage.sprite = SelectedSprite;
        }

        protected override void SubmitAction()
        {
            TargetImage.sprite = SubmittedSprite;
        }

        protected override void DeselectAction()
        {
            TargetImage.sprite = _defaultSprite;
        }
    }
}