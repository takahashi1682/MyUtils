using UnityEngine;

namespace MyUtils.PopupWindow
{
    public class PopupDemo : MonoBehaviour
    {
        public async void OnShowPopup()
        {
            var popup = await PopupPanel.InitializeAsync;
            bool answer = await popup.ShowPopupPanel("Do you Playing Game?", PopupMode.DoubleButton);
            Debug.Log(answer);
        }
    }
}