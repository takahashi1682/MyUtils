using UnityEngine;

namespace MyUtils.UICommon
{
    public class ProjectVersionViewer : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI VersionText;
        public string VersionFormat = "Ver.{0}";

        private void Start()
            => VersionText.text = string.Format(VersionFormat, Application.version);
    }
}