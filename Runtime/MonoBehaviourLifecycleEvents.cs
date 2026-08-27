using UnityEngine;
using UnityEngine.Events;

namespace MyUtils.Misc
{
    /// <summary>
    /// MonoBehaviourのライフサイクル(Awake/Start/OnEnable/OnDisable/OnDestroy)を
    /// UnityEventとして中継し、Inspectorから配線できるようにする
    /// </summary>
    public class MonoBehaviourLifecycleEvents : MonoBehaviour
    {
        public UnityEvent AwakeEvent;
        public UnityEvent StartEvent;
        public UnityEvent EnableEvent;
        public UnityEvent DisableEvent;
        public UnityEvent DestroyEvent;

        private void Awake() => AwakeEvent?.Invoke();
        private void Start() => StartEvent?.Invoke();
        private void OnEnable() => EnableEvent?.Invoke();
        private void OnDisable() => DisableEvent?.Invoke();
        private void OnDestroy() => DestroyEvent?.Invoke();
    }
}
