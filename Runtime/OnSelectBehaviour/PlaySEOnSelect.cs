using MyUtils.AudioManager.Manager;
using UnityEngine;

namespace MyUtils.OnSelectBehaviour
{
    /// <summary>
    /// UIが選択された時に音を再生する機能
    /// </summary>
    public class PlaySEOnSelect : AbstractOnSelectBehaviour
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