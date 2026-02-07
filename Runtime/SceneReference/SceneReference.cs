using System;
using UnityEngine;

namespace MyUtils.SceneReference
{
    [Serializable]
    public class SceneReference
    {
#if UNITY_EDITOR
        public UnityEditor.SceneAsset SceneAsset;
#endif

        [SerializeField]
        private string _sceneName;

        public string SceneName => _sceneName;

        public void SetSceneName(string name)
        {
            _sceneName = name;
        }
    }
}