#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyUtils.DebugMethod
{
    /// <summary>
    /// すべてのMonoBehaviourのインスペクター下部にデバッグボタンを自動追加する拡張
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class DebugMethodInspector : Editor
    {
        private List<DebugMethodCache> _componentMethods;

        private void OnEnable()
        {
            var mono = target as MonoBehaviour;
            if (mono == null) return;
            _componentMethods = DebugMethodRegistry.GetMethodsForTarget(mono);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_componentMethods == null || _componentMethods.Count == 0) return;

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("⚡ Debug Methods", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            foreach (var cache in _componentMethods)
            {
                if (cache.Target == null) continue;

                // 共通GUIを呼び出し
                DebugMethodGUI.DrawMethodButton(cache);
                EditorGUILayout.Space(2);
            }
        }
    }
}
#endif