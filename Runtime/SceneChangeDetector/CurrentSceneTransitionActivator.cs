using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MyUtils.SceneChangeDetector
{
    /// <summary>
    /// シーン切替に応じてオブジェクトのアクティブ状態を切り替える機能
    /// </summary>
    public class CurrentSceneTransitionActivator : AbstractSceneChangeDetector
    {
        public List<GameObjectPair> TargetObjects;
        public List<BehaviorPair> TargetBehaviors;
        public UnityEvent<bool> OnSceneActivated;

        protected override void OnCurrentSceneEnter(Scene scene, LoadSceneMode mode)
        {
            SetActive(true);
            OnSceneActivated?.Invoke(true);
        }

        protected override void OnCurrentSceneExit(Scene scene)
        {
            SetActive(false);
            OnSceneActivated?.Invoke(false);
        }

        private void SetActive(bool isActive)
        {
            foreach (var targetObject in TargetObjects)
            {
                targetObject.TargetObject.SetActive(targetObject.IsInverse ? !isActive : isActive);
            }

            foreach (var targetBehavior in TargetBehaviors)
            {
                targetBehavior.TargetBehavior.enabled = targetBehavior.IsInverse ? !isActive : isActive;
            }
        }
    }

    [Serializable]
    public class GameObjectPair
    {
        public GameObject TargetObject;
        public bool IsInverse;
    }

    [Serializable]
    public class BehaviorPair
    {
        public MonoBehaviour TargetBehavior;
        public bool IsInverse;
    }
}