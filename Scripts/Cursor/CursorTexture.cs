using UnityEngine;

namespace TUtils.Cursor
{
    /// <summary>
    /// カーソルテクスチャ設定
    /// </summary>
    public class CursorTexture : MonoBehaviour
    {
        [SerializeField] private Texture2D _cursorTexture;
        [SerializeField] private Vector2 _hotspotOffset = new(0.5f, 0.5f);

        private void Start()
        {
            var hotspot = new Vector2(_cursorTexture.width, _cursorTexture.height) * _hotspotOffset;
            UnityEngine.Cursor.SetCursor(_cursorTexture, hotspot, CursorMode.ForceSoftware);
        }
    }
}