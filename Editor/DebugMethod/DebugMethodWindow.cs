#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyUtils.DebugMethod
{
    /// <summary>
    /// シーン内のデバッグメソッドを一画面で全列挙する専用ウィンドウ
    /// </summary>
    public class DebugMethodWindow : EditorWindow
    {
        private Vector2 _scrollPos;

        [MenuItem("Window/MyUtils/Debug Method Window")]
        public static void ShowWindow() => GetWindow<DebugMethodWindow>("Debug Method Window");

        private void OnEnable()
        {
            DebugMethodRegistry.RefreshCache();
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += (_, _) => DebugMethodRegistry.RefreshCache();
        }

        private void OnFocus() => DebugMethodRegistry.RefreshCache();

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("🔄 Refresh List", EditorStyles.toolbarButton)) DebugMethodRegistry.RefreshCache();
                GUILayout.FlexibleSpace();
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var cachedMethods = DebugMethodRegistry.GlobalCache;
            if (cachedMethods.Count == 0)
            {
                EditorGUILayout.HelpBox("[TestInvokeMethod] 属性がついたメソッドがシーン内にありません。", MessageType.Info);
            }

            MonoBehaviour lastMono = null;
            foreach (var cache in cachedMethods)
            {
                if (cache.Target == null) continue;

                if (lastMono != cache.Target)
                {
                    if (lastMono != null) EditorGUILayout.EndVertical();

                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button($"{cache.Target.gameObject.name} ({cache.Target.GetType().Name})",
                                EditorStyles.boldLabel))
                        {
                            Selection.activeGameObject = cache.Target.gameObject;
                        }

                        if (GUILayout.Button(new GUIContent("🔍", "オブジェクトへ移動"), GUILayout.Width(30)))
                        {
                            Selection.activeGameObject = cache.Target.gameObject;
                            EditorGUIUtility.PingObject(cache.Target.gameObject);
                            SceneView.FrameLastActiveSceneView();
                        }
                    }

                    lastMono = cache.Target;
                }

                // 共通GUIを呼び出し
                DebugMethodGUI.DrawMethodButton(cache);
            }

            if (lastMono != null) EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }

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