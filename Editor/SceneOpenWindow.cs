#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public class SceneOpenWindow : EditorWindow
    {
        [MenuItem("Window/Scene Open Window")]
        public static void OpenWindow()
        {
            GetWindow<SceneOpenWindow>("Scene Open Window");
        }

        private string[] _allScenePaths; // 全シーンのパス
        private string _filterText = "Assets/"; // 入力されたフィルター文字列
        private GUIStyle _buttonStyle;
        private GUIContent _sceneIconContent;
        private Vector2 _scrollPosition;

        private void OnEnable() => RefreshSceneList();

        private void OnGUI()
        {
            _buttonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fixedHeight = 20f,
                alignment = TextAnchor.MiddleLeft,
                imagePosition = ImagePosition.ImageLeft
            };

            // フィルター入力フィールド
            EditorGUILayout.LabelField("Filter by path:", EditorStyles.boldLabel);
            _filterText = EditorGUILayout.TextField(_filterText);
            EditorGUILayout.Space();

            // 更新ボタン
            if (GUILayout.Button("Refresh")) RefreshSceneList();
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            string[] displayedScenes = string.IsNullOrEmpty(_filterText)
                ? _allScenePaths
                : Array.FindAll(_allScenePaths, path => path.ToLower().Contains(_filterText.ToLower()));

            var sceneIconContent = EditorGUIUtility.IconContent("SceneAsset Icon");

            foreach (string scenePath in displayedScenes)
            {
                string label = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                var content = new GUIContent(label, sceneIconContent.image);

                if (GUILayout.Button(content, _buttonStyle))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void RefreshSceneList()
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            _allScenePaths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                _allScenePaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            Repaint();
        }
    }
}
#endif