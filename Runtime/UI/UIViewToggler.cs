using UnityEngine;

namespace MyUtils.UI
{
    public class UIViewToggler : MonoBehaviour
    {
        [SerializeField] private GameObject[] _viewToToggle;

        public void ToggleView(bool isOn)
        {
            foreach (var view in _viewToToggle)
            {
                if (view != null)
                {
                    view.SetActive(isOn);
                }
            }
        }
    }
}