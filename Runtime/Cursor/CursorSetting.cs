using UnityEngine;

namespace MyUtils.Cursor
{
    /// <summary>
    /// カーソル設定
    /// </summary>
    public class CursorSetting : MonoBehaviour
    {
        [SerializeField] private bool _isCursorVisible;
        [SerializeField] private CursorLockMode _isCursorLocked = CursorLockMode.Locked;

        private void Start()
        {
            UnityEngine.Cursor.visible = _isCursorVisible;
            UnityEngine.Cursor.lockState = _isCursorLocked;
        }

        private void OnDestroy()
        {
            // 元の設定に戻す
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
}