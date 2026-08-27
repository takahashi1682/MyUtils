using UnityEngine;
using UnityEngine.Events;

namespace MyUtils
{
    public class MonoBehaviourLifecycleEvents : MonoBehaviour
    {
        public UnityEvent AwakeEvent;
        public UnityEvent StartEvent;
        public UnityEvent EnableEvent;
        public UnityEvent DisableEvent;
        public UnityEvent VisibleEvent;
        public UnityEvent InvisibleEvent;
        public UnityEvent DestroyEvent;

        private void Awake() => AwakeEvent?.Invoke();
        private void Start() => StartEvent?.Invoke();

        private void OnEnable() => EnableEvent?.Invoke();
        private void OnDisable() => DisableEvent?.Invoke();

        private void OnBecameInvisible() => InvisibleEvent?.Invoke();
        private void OnBecameVisible() => VisibleEvent?.Invoke();

        private void OnDestroy() => DestroyEvent?.Invoke();
    }
}