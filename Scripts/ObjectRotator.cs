using UnityEngine;

namespace TUtils
{
    public enum ERotateMode { Update, Interval }

    /// <summary>
    /// 回転機能
    /// </summary>
    public class ObjectRotator : MonoBehaviour
    {
        public bool IsPlay = true;
        [SerializeField] private ERotateMode _rotateMode = ERotateMode.Update;
        [SerializeField, Min(0)] private float _rotationInterval;
        [SerializeField] private Vector3 _rotationAngles = new(0, 0, 30f);
        private float _elapsedTime;

        private void Update()
        {
            if (!IsPlay) return;

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > _rotationInterval)
            {
                _elapsedTime = 0;

                RotateObject();
            }
        }

        private void RotateObject()
        {
            if (_rotateMode == ERotateMode.Update)
            {
                transform.localRotation *= Quaternion.Euler(_rotationAngles * Time.deltaTime);
            }
            else
            {
                transform.localRotation *= Quaternion.Euler(_rotationAngles);
            }
        }
    }
}