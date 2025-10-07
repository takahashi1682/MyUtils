using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.SceneChangeDetector
{
    public abstract class AbstractSceneChangeDetector : MonoBehaviour
    {
        protected virtual void Awake() => RegisterSceneEvents();

        protected void Start()
        {
            // 初回起動時の呼び出し
            var activeScene = SceneManager.GetActiveScene();
            OnSceneLoaded(activeScene, LoadSceneMode.Single);
            OnActiveSceneChanged(default, activeScene);
        }

        protected virtual void OnDestroy() => UnregisterSceneEvents();

        private void RegisterSceneEvents()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void UnregisterSceneEvents()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnSceneEnter(scene, mode);

            if (IsCurrentScene(scene))
                OnCurrentSceneEnter(scene, mode);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            OnSceneExit(scene);

            if (IsCurrentScene(scene))
                OnCurrentSceneExit(scene);
        }

        private bool IsCurrentScene(Scene scene)
            => gameObject.scene == scene;

        protected virtual void OnSceneEnter(Scene scene, LoadSceneMode mode) { }
        protected virtual void OnSceneExit(Scene scene) { }
        protected virtual void OnCurrentSceneEnter(Scene scene, LoadSceneMode mode) { }
        protected virtual void OnCurrentSceneExit(Scene scene) { }
        protected virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene) { }
    }
}