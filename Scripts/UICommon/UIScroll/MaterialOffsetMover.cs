using UnityEngine;

namespace MyUtils.UICommon.UIScroll
{
    public class MaterialOffsetMover : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID(PropertyName);
        private const string PropertyName = "_MainTex";

        [SerializeField] private SpriteRenderer _target;
        [SerializeField] private Vector2 _offsetSpeed;
        private Material _material;

        private void Start()
        {
            _material = _target.material;
        }

        private void Update()
        {
            if (_material is not null)
            {
                var offset = Time.time * _offsetSpeed;
                _material.SetTextureOffset(MainTex, offset);
            }
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                _material.SetTextureOffset(MainTex, Vector2.zero);
            }
        }
    }
}