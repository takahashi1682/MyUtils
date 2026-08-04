using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyUtils.UI
{
    public class UISelectedOnEnable : MonoBehaviour
    {
        public Selectable TargetSelectable;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(TargetSelectable.gameObject);
        }
    }
}