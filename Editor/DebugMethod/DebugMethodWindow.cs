#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.DebugMethod
{
    /// <summary>
    /// シーン内のデバッグメソッドを一画面で全列挙する専用ウィンドウ
    /// </summary>
    public class DebugMethodWindow : EditorWindow
    {
        private Vector2 _scrollPos;
        private List<DebugMethodCache> _methods = new();

        [MenuItem("Window/MyUtils/Debug Method Window")]
        public static void ShowWindow() => GetWindow<DebugMethodWindow>("Debug Method Window");

        private void OnEnable()
        {
            Refresh();
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void OnFocus() => Refresh();

        private void OnSceneOpened(Scene scene, OpenSceneMode mode) => Refresh();

        // 再生の開始・終了でシーンのオブジェクトが作り直されるので、取り直す
        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.EnteredEditMode or PlayModeStateChange.EnteredPlayMode) Refresh();
        }

        // シーン内の全コンポーネントを探すので、毎フレームではなく必要なときだけ呼ぶ
        private void Refresh()
        {
            _methods = DebugMethodRegistry.FindAllInScene();
            Repaint();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("🔄 Refresh List", EditorStyles.toolbarButton)) Refresh();
                GUILayout.FlexibleSpace();
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (_methods.Count == 0)
            {
                EditorGUILayout.HelpBox("[DebugMethod] 属性がついたメソッドがシーン内にありません。", MessageType.Info);
            }

            MonoBehaviour lastMono = null;
            foreach (var cache in _methods)
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
}
#endif