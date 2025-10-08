using UnityEngine;

namespace MyUtils
{
    /// <summary>
    ///  Spriteをスクロールさせる機能
    /// </summary>
    public class SpriteScroller : MonoBehaviour
    {
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private float _scrollSpeed = 1.0f;
        [SerializeField] private Vector3 _scrollDirection = Vector2.down;
        private Vector3 _size;

        private void Start()
        {
            _size = _renderers[0].bounds.size;
        }

        private void Update()
        {
            foreach (var r in _renderers)
            {
                var t = r.transform;
                t.position += _scrollDirection * (_scrollSpeed * Time.deltaTime);

                // 画面外に出たら反対側に移動
                if (t.position.y < -_size.y)
                {
                    float diff = _size.y + t.position.y;
                    t.position = new Vector3(t.position.x, _size.y + diff, t.position.z);
                }
                else if (t.position.y > _size.y)
                {
                    float diff = t.position.y - _size.y;
                    t.position = new Vector3(t.position.x, -_size.y - diff, t.position.z);
                }
                else if (t.position.x < -_size.x)
                {
                    float diff = _size.x - t.position.x;
                    t.position = new Vector3(_size.x, t.position.y - diff, t.position.z);
                }
                else if (t.position.x > _size.x)
                {
                    float diff = t.position.x - _size.x;
                    t.position = new Vector3(-_size.x, t.position.y - diff, t.position.z);
                }
            }
        }
    }
}