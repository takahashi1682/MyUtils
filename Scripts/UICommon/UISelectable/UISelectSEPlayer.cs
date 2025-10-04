using TUtils.AudioManager.Manager;
using UnityEngine;

namespace TUtils.UICommon.UISelectable
{
    /// <summary>
    /// UIが選択された時に音を再生する機能
    /// </summary>
    public class UISelectSEPlayer : AbstractUISelectable
    {
        public AudioClip SelectedSound;
        public AudioClip SubmitSound;

        protected override void SelectedAction()
        {
            SEManager.Play(SelectedSound);
        }

        protected override void SubmitAction()
        {
            SEManager.Play(SubmitSound);
        }

        protected override void DeselectAction()
        {
        }
    }
}