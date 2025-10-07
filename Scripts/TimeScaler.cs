using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// タイムスケールを制御するクラス
    /// </summary>
    public class TimeScaler : MonoBehaviour
    {
        [SerializeField] private float _targetTimeScale = 1;
        [SerializeField] private bool _isApplyOnEnable = true;
        [SerializeField] private bool _isResetOnDisable = true;
        private float _initialTimeScale;

        private void Awake()
        {
            _initialTimeScale = Time.timeScale;
        }

        private void OnEnable()
        {
            if (_isApplyOnEnable) SetTimeScale(_targetTimeScale);
        }

        private void OnDisable()
        {
            if (_isResetOnDisable) SetTimeScale(_initialTimeScale);
        }

        public static void SetTimeScale(float timeScale) => Time.timeScale = timeScale;
    }
}