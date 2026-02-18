using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UISelectable
{
    public class SelectColor : AbstractTargetSelectable
    {
        public Graphic TargetGraphic;
        public Color SelectedColor = Color.cyan;
        public Color SubmittedColor = Color.yellow;
        private Color _defaultPosition;

        protected override void Start()
        {
            base.Start();
            _defaultPosition = TargetGraphic.color;
        }

        protected override void SelectedAction()
        {
            TargetGraphic.color = SelectedColor;
        }

        protected override void SubmitAction()
        {
            TargetGraphic.color = SubmittedColor;
        }

        protected override void DeselectAction()
        {
            TargetGraphic.color = _defaultPosition;
        }
    }
}